using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace LINQtoCSV
{
    internal class FieldMapper<T> 
    {
        protected class TypeFieldInfo : IComparable<TypeFieldInfo>
        {
            public int index = CsvColumnAttribute.mc_DefaultFieldIndex;
            public string name = null;
            public bool canBeNull = true;
            public NumberStyles inputNumberStyle = NumberStyles.Any;
            public string outputFormat = null;
            public bool hasColumnAttribute = false;

            public MemberInfo memberInfo = null;
            public Type fieldType = null;

            // parseNumberMethod will remain null if the property is not a numeric type.
            // This would be the case for DateTime, Boolean, String and custom types.
            // In those cases, just use a TypeConverter.
            //
            // DateTime and Boolean also have Parse methods, but they don't provide
            // functionality that TypeConverter doesn't give you.

            public TypeConverter typeConverter = null;
            public MethodInfo parseNumberMethod = null;
            public MethodInfo parseExactMethod;
            public int charLength = 0;

            // ----

            public int CompareTo(TypeFieldInfo other)
            {
                return index.CompareTo(other.index);
            }

            public override string ToString() 
            {
                return string.Format("Index: {0}, Name: {1}", index, name);
            }
        }

        // -----------------------------

        // IndexToInfo is used to quickly translate the index of a field
        // to its TypeFieldInfo.
        protected TypeFieldInfo[] m_IndexToInfo = null;

        /// <summary>
        /// Contains a mapping between the CSV column indexes that will read and the property indexes in the business object.
        /// </summary>
        protected IDictionary<int, int> _mappingIndexes = new Dictionary<int, int>(); 

        // Used to build IndexToInfo
        protected Dictionary<string, TypeFieldInfo> m_NameToInfo = null;

        protected CsvFileDescription m_fileDescription;

        // Only used when throwing an exception
        protected string m_fileName;

        // -----------------------------
        // AnalyzeTypeField
        //
        private TypeFieldInfo AnalyzeTypeField(
                                MemberInfo mi,
                                bool allRequiredFieldsMustHaveFieldIndex,
                                bool allCsvColumnFieldsMustHaveFieldIndex)
        {
            TypeFieldInfo tfi = new TypeFieldInfo();

            tfi.memberInfo = mi;

            if (mi is PropertyInfo)
            {
                tfi.fieldType = ((PropertyInfo)mi).PropertyType;
            }
            else
            {
                tfi.fieldType = ((FieldInfo)mi).FieldType;
            }

            // parseNumberMethod will remain null if the property is not a numeric type.
            // This would be the case for DateTime, Boolean, String and custom types.
            // In those cases, just use a TypeConverter.
            //
            // DateTime and Boolean also have Parse methods, but they don't provide
            // functionality that TypeConverter doesn't give you.

            tfi.parseNumberMethod =
                tfi.fieldType.GetMethod("Parse",
                    new Type[] { typeof(String), typeof(NumberStyles), typeof(IFormatProvider) });

            if (tfi.parseNumberMethod == null)
            {
                if (m_fileDescription.UseOutputFormatForParsingCsvValue)
                {
                    tfi.parseExactMethod = tfi.fieldType.GetMethod("ParseExact",
                        new Type[] {typeof (string), typeof (string), typeof (IFormatProvider)});
                }

                tfi.typeConverter = null;
                if (tfi.parseExactMethod == null)
                {
                    tfi.typeConverter =
                        TypeDescriptor.GetConverter(tfi.fieldType);
                }
            }

            // -----
            // Process the attributes

            tfi.index = CsvColumnAttribute.mc_DefaultFieldIndex;
            tfi.name = mi.Name;
            tfi.inputNumberStyle = NumberStyles.Any;
            tfi.outputFormat = "";
            tfi.hasColumnAttribute = false;
            tfi.charLength = 0;

            foreach (Object attribute in mi.GetCustomAttributes(typeof(CsvColumnAttribute), true))
            {
                CsvColumnAttribute cca = (CsvColumnAttribute)attribute;

                if (!string.IsNullOrEmpty(cca.Name))
                {
                    tfi.name = cca.Name;
                }

                tfi.index = cca.FieldIndex;
                tfi.hasColumnAttribute = true;
                tfi.canBeNull = cca.CanBeNull;
                tfi.outputFormat = cca.OutputFormat;
                tfi.inputNumberStyle = cca.NumberStyle;
                tfi.charLength = cca.CharLength;
            }

            // -----

            if (allCsvColumnFieldsMustHaveFieldIndex &&
                tfi.hasColumnAttribute &&
                tfi.index == CsvColumnAttribute.mc_DefaultFieldIndex)
            {
                throw new ToBeWrittenButMissingFieldIndexException(
                                typeof(T).ToString(),
                                tfi.name);
            }

            if (allRequiredFieldsMustHaveFieldIndex &&
                (!tfi.canBeNull) &&
                (tfi.index == CsvColumnAttribute.mc_DefaultFieldIndex))
            {
                throw new RequiredButMissingFieldIndexException(
                                typeof(T).ToString(),
                                tfi.name);
            }

            // -----

            return tfi;
        }

        // -----------------------------
        // AnalyzeType
        //
        protected void AnalyzeType(
                        Type type, 
                        bool allRequiredFieldsMustHaveFieldIndex,
                        bool allCsvColumnFieldsMustHaveFieldIndex)
        {
            m_NameToInfo.Clear();

            // ------
            // Initialize NameToInfo

            foreach (MemberInfo mi in type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static ))
            {
                // Only process field and property members.
                if ((mi.MemberType == MemberTypes.Field) ||
                    (mi.MemberType == MemberTypes.Property))
                {
                    // Note that the compiler does not allow fields and/or properties
                    // with the same name as some other field or property.
                    TypeFieldInfo tfi = 
                        AnalyzeTypeField(
                                mi, 
                                allRequiredFieldsMustHaveFieldIndex,
                                allCsvColumnFieldsMustHaveFieldIndex);

                    m_NameToInfo[tfi.name] = tfi;
                }
            }

            // -------
            // Initialize IndexToInfo

            int nbrTypeFields = m_NameToInfo.Keys.Count;
            m_IndexToInfo = new TypeFieldInfo[nbrTypeFields];

            _mappingIndexes = new Dictionary<int, int>();
            
            int i=0;
            foreach (KeyValuePair<string, TypeFieldInfo> kvp in m_NameToInfo)
            {
                m_IndexToInfo[i++] = kvp.Value;
            }

            // Sort by FieldIndex. Fields without FieldIndex will 
            // be sorted towards the back, because their FieldIndex
            // is Int32.MaxValue.
            //
            // The sort order is important when reading a file that 
            // doesn't have the field names in the first line, and when
            // writing a file. 
            //
            // Note that for reading from a file with field names in the 
            // first line, method ReadNames reworks IndexToInfo.

            Array.Sort(m_IndexToInfo);

            // ----------
            // Make sure there are no duplicate FieldIndices.
            // However, allow gaps in the FieldIndex range, to make it easier to later insert
            // fields in the range.

            int lastFieldIndex = Int32.MinValue;
            string lastName = "";
            foreach(TypeFieldInfo tfi in m_IndexToInfo)
            {
                if ((tfi.index == lastFieldIndex) && 
                    (tfi.index != CsvColumnAttribute.mc_DefaultFieldIndex))
                {
                    throw new DuplicateFieldIndexException(
                                typeof(T).ToString(),
                                tfi.name,
                                lastName,
                                tfi.index);
                }

                lastFieldIndex = tfi.index;
                lastName = tfi.name;
            }
        }

        /// ///////////////////////////////////////////////////////////////////////
        /// FieldMapper
        /// 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileDescription"></param>
        public FieldMapper(CsvFileDescription fileDescription, string fileName, bool writingFile)
        {
            if ((!fileDescription.FirstLineHasColumnNames) &&
                (!fileDescription.EnforceCsvColumnAttribute))
            {
                throw new CsvColumnAttributeRequiredException();
            }

            // ---------

            m_fileDescription = fileDescription;
            m_fileName = fileName;

            m_NameToInfo = new Dictionary<string, TypeFieldInfo>();

            AnalyzeType(
                typeof(T), 
                !fileDescription.FirstLineHasColumnNames,
                writingFile && !fileDescription.FirstLineHasColumnNames);
        }

        /// ///////////////////////////////////////////////////////////////////////
        /// WriteNames
        /// 
        /// <summary>
        /// Writes the field names given in T to row.
        /// </summary>
        /// 
        public void WriteNames(List<string> row)
        {
            row.Clear();

            for (int i = 0; i < m_IndexToInfo.Length; i++)
            {
                TypeFieldInfo tfi = m_IndexToInfo[i];

                if (m_fileDescription.EnforceCsvColumnAttribute &&
                        (!tfi.hasColumnAttribute))
                {
                    continue;
                }

                // ----

                row.Add(tfi.name);
            }
        }


        /// ///////////////////////////////////////////////////////////////////////
        /// WriteObject
        /// 
        public void WriteObject(T obj, List<string> row)
        {
            row.Clear();

            for (int i = 0; i < m_IndexToInfo.Length; i++)
            {
                TypeFieldInfo tfi = m_IndexToInfo[i];

                if (m_fileDescription.EnforceCsvColumnAttribute &&
                        (!tfi.hasColumnAttribute))
                {
                    continue;
                }

                // ----

                Object objValue = null;

                if (tfi.memberInfo is PropertyInfo)
                {
                    objValue =
                        ((PropertyInfo)tfi.memberInfo).GetValue(obj, null);
                }
                else
                {
                    objValue =
                        ((FieldInfo)tfi.memberInfo).GetValue(obj);
                }

                // ------

                string resultString = null;
                if (objValue != null)
                {
                    if ((objValue is IFormattable))
                    {
                        resultString =
                            ((IFormattable)objValue).ToString(
                                tfi.outputFormat,
                                m_fileDescription.FileCultureInfo);
                    }
                    else
                    {
                        resultString = objValue.ToString();
                    }
                }

                // -----

                row.Add(resultString);
            }
        }
    }

    /// ///////////////////////////////////////////////////////////////////////
    // To do reading, the object needs to create an object of type T
    // to read the data into. This requires the restriction T : new()
    // However, for writing, you don't want to impose that restriction.
    //
    // So, use FieldMapper (without the restriction) for writing,
    // and derive a FieldMapper_Reading (with restrictions) for reading.
    //
    internal class FieldMapper_Reading<T> : FieldMapper<T> where T : new()
    {
        /// ///////////////////////////////////////////////////////////////////////
        /// FieldMapper
        /// 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileDescription"></param>
        public FieldMapper_Reading(
                    CsvFileDescription fileDescription, 
                    string fileName,
                    bool writingFile)
            : base(fileDescription, fileName, writingFile)
        {

        }


        /// ///////////////////////////////////////////////////////////////////////
        /// ReadNames
        /// 
        /// <summary>
        /// Assumes that the fields in parameter row are field names.
        /// Reads the names into the objects internal structure.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="firstRow"></param>
        /// <returns></returns>
        ///
        public void ReadNames(IDataRow row)
        {
            // It is now the order of the field names that determines
            // the order of the elements in m_IndexToInfo, instead of
            // the FieldIndex fields.

            // If there are more names in the file then fields in the type,
            // and IgnoreUnknownColumns is set to `false` one of the names will 
            // not be found, causing an exception.

            int currentNameIndex = 0;
            for (int i = 0; i < row.Count; i++) {
                if (!m_NameToInfo.ContainsKey(row[i].Value)) {
                    //If we have to ignore this column
                    if (m_fileDescription.IgnoreUnknownColumns) {
                        continue;
                    }

                    // name not found
                    throw new NameNotInTypeException(typeof (T).ToString(), row[i].Value, m_fileName);
                }

                // ----

                //Map the column index in the CSV file with the column index of the business object.
                _mappingIndexes.Add(i, currentNameIndex);
                currentNameIndex++;
            }

            //Loop to the 
            for (int i = 0; i < row.Count; i++) {
                if (!_mappingIndexes.ContainsKey(i)) {
                    continue;
                }

                m_IndexToInfo[_mappingIndexes[i]] = m_NameToInfo[row[i].Value];

                if (m_fileDescription.EnforceCsvColumnAttribute && (!m_IndexToInfo[i].hasColumnAttribute)) {
                    // enforcing column attr, but this field/prop has no column attr.
                    throw new MissingCsvColumnAttributeException(typeof (T).ToString(), row[i].Value, m_fileName);
                }
            }
        }

        public List<int> GetCharLengths()
        {
            if (!m_fileDescription.NoSeparatorChar)
                return null;

            return m_IndexToInfo.Select(e => e.charLength).ToList();
        }

        /// ///////////////////////////////////////////////////////////////////////
        /// ReadObject
        /// 
        /// <summary>
        /// Creates an object of type T from the data in row and returns that object.
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="firstRow"></param>
        /// <returns></returns>
        public T ReadObject(IDataRow row, AggregatedException ae) {
            //If there are more columns than the required
            if (row.Count > m_IndexToInfo.Length)
            {
                //Are we ignoring unknown columns?
                if (!m_fileDescription.IgnoreUnknownColumns) {
                    // Too many fields
                    throw new TooManyDataFieldsException(typeof (T).ToString(), row[0].LineNbr, m_fileName);
                }
            }

            // -----

            T obj = new T();

            //If we will be using the mappings, we just iterate through all the cells in this row
            int maxRowCount = _mappingIndexes.Count > 0 ? row.Count : Math.Min(row.Count, m_IndexToInfo.Length);

            for (int i = 0; i < maxRowCount; i++) {
                TypeFieldInfo tfi;
                //If there is some index mapping generated and the IgnoreUnknownColums is `true`
                if (m_fileDescription.IgnoreUnknownColumns && _mappingIndexes.Count > 0) {
                    if (!_mappingIndexes.ContainsKey(i)) {
                        continue;
                    }
                    tfi = m_IndexToInfo[_mappingIndexes[i]];
                }
                else {
                    tfi = m_IndexToInfo[i];
                }

                if (m_fileDescription.EnforceCsvColumnAttribute &&
                        (!tfi.hasColumnAttribute))
                {
                    // enforcing column attr, but this field/prop has no column attr.
                    // So there are too many fields in this record.
                    throw new TooManyNonCsvColumnDataFieldsException(typeof(T).ToString(), row[i].LineNbr, m_fileName);
                }

                // -----

                if ((!m_fileDescription.FirstLineHasColumnNames) &&
                        (tfi.index == CsvColumnAttribute.mc_DefaultFieldIndex))
                {
                    // First line in the file does not have field names, so we're 
                    // depending on the FieldIndex of each field in the type
                    // to ensure each value is placed in the correct field.
                    // However, now hit a field where there is no FieldIndex.
                    throw new MissingFieldIndexException(typeof(T).ToString(), row[i].LineNbr, m_fileName);
                }

                // -----

                if (m_fileDescription.UseFieldIndexForReadingData && (!m_fileDescription.FirstLineHasColumnNames) &&
                    (tfi.index > row.Count))
                {
                    // First line in the file does not have field names, so we're 
                    // depending on the FieldIndex of each field in the type
                    // to ensure each value is placed in the correct field.
                    // However, now hit a field where the FieldIndex is bigger
                    // than the total number of items in a row generated by the separatorChar
                    throw new WrongFieldIndexException(typeof(T).ToString(), row[i].LineNbr, m_fileName);
                }

                int index = m_fileDescription.UseFieldIndexForReadingData ? tfi.index - 1 : i;
                
                // value to put in the object
                string value = row[index].Value;

                if (value == null)
                {
                    if (!tfi.canBeNull)
                    {
                        ae.AddException(
                            new MissingRequiredFieldException(
                                    typeof(T).ToString(),
                                    tfi.name,
                                    row[i].LineNbr,
                                    m_fileName));
                    }
                }
                else
                {
                    try
                    {
                        Object objValue = null;

                        // Normally, either tfi.typeConverter is not null,
                        // or tfi.parseNumberMethod is not null. 
                        // 
                        if (tfi.typeConverter != null)
                        {
                            objValue = tfi.typeConverter.ConvertFromString(
                                           null,
                                           m_fileDescription.FileCultureInfo,
                                           value);
                        }
                        else if (tfi.parseExactMethod != null)
                        {
                            objValue =
                                tfi.parseExactMethod.Invoke(
                                    tfi.fieldType,
                                    new Object[] { 
                                    value, 
                                    tfi.outputFormat, 
                                    m_fileDescription.FileCultureInfo });
                        }   
                        else if (tfi.parseNumberMethod != null)
                        {
                            objValue =
                                tfi.parseNumberMethod.Invoke(
                                    tfi.fieldType,
                                    new Object[] { 
                                    value, 
                                    tfi.inputNumberStyle, 
                                    m_fileDescription.FileCultureInfo });
                        }
                        else
                        {
                            // No TypeConverter and no Parse method available.
                            // Try direct approach.
                            objValue = value;                          
                        }

                        if (tfi.memberInfo is PropertyInfo)
                        {
                            ((PropertyInfo)tfi.memberInfo).SetValue(obj, objValue, null);
                        }
                        else
                        {
                            ((FieldInfo)tfi.memberInfo).SetValue(obj, objValue);
                        }
                    }
                    catch (Exception e)
                    {
                        if (e is TargetInvocationException)
                        {
                            e = e.InnerException;
                        }

                        if (e is FormatException)
                        {
                            e = new WrongDataFormatException(
                                    typeof(T).ToString(),
                                    tfi.name,
                                    value,
                                    row[i].LineNbr,
                                    m_fileName,
                                    e);
                        }

                        ae.AddException(e);
                    }
                }
            }

            // Visit any remaining fields in the type for which no value was given
            // in the data row, to see whether any of those was required.
            // If only looking at fields with CsvColumn attribute, do ignore
            // fields that don't have that attribute.

            for (int i = row.Count; i < m_IndexToInfo.Length; i++)
            {
                TypeFieldInfo tfi = m_IndexToInfo[i];

                if (((!m_fileDescription.EnforceCsvColumnAttribute) ||
                     tfi.hasColumnAttribute) &&
                    (!tfi.canBeNull))
                {
                    ae.AddException(
                        new MissingRequiredFieldException(
                                typeof(T).ToString(),
                                tfi.name,
                                row[row.Count - 1].LineNbr,
                                m_fileName));
                }
            }

            return obj;
        }
    }
}
