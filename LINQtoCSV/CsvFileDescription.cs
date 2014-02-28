using System;
using System.Globalization;
using System.Text;

namespace LINQtoCSV
{
    /// <summary>
    /// Summary description for CsvFileDescription
    /// </summary>
    public class CsvFileDescription
    {
        // Culture used to process the CSV values, specifically numbers and dates.
        private CultureInfo m_cultureInfo = CultureInfo.CurrentCulture;

        private int m_maximumNbrExceptions = 100;

        // --------------

        // Character used to separate fields in the file.
        // By default, this is comma (,).
        // For a tab delimited file, you would set this to
        // the tab character ('\t').
        public char SeparatorChar { get; set; }

        public bool NoSeparatorChar { get; set; }

        // Only used when writing a file
        //
        // If true, all fields are quoted whatever their content.
        // If false, only fields containing a FieldSeparator character,
        // a quote or a newline are quoted.
        //
        public bool QuoteAllFields { get; set; }

        // If true, then:
        // When writing a file, the column names are written in the
        // first line of the new file.
        // When reading a file, the column names are read from the first
        // line of the file.
        //
        public bool FirstLineHasColumnNames { get; set; }

        // If true, only public fields and properties with the
        // [CsvColumn] attribute are recognized.
        // If false, all public fields and properties are used.
        //
        public bool EnforceCsvColumnAttribute { get; set; }

        // FileCultureName and FileCultureInfo both get/set
        // the CultureInfo used for the file.
        // For example, if the file uses Dutch date and number formats
        // while the current culture is US English, set
        // FileCultureName to "nl-NL".
        //
        // To simply use the current culture, leave the culture as is.
        //
        public string FileCultureName
        {
            get { return m_cultureInfo.Name; }
            set { m_cultureInfo = new CultureInfo(value); }
        }

        public CultureInfo FileCultureInfo
        {
            get { return m_cultureInfo; }
            set { m_cultureInfo = value; }
        }

        // When reading a file, exceptions thrown while the file is being read
        // are captured in an aggregate exception. That aggregate exception is then
        // thrown at the end - to make it easier to solve multiple problems with the
        // input file in one. 
        //
        // However, after MaximumNbrExceptions, the aggregate exception is thrown
        // immediately.
        //
        // To not have a maximum at all, set this to -1.
        public int MaximumNbrExceptions
        {
            get { return m_maximumNbrExceptions; }
            set { m_maximumNbrExceptions = value; }
        }

        // Character encoding. Defaults should work in most cases.
        // However, when reading or writing non-English files, you may want to use
        // Unicode encoding.
        public Encoding TextEncoding { get; set; }
        public bool DetectEncodingFromByteOrderMarks { get; set; }

        public bool UseFieldIndexForReadingData { get; set; }
        public bool UseOutputFormatForParsingCsvValue { get; set; }
        public bool IgnoreTrailingSeparatorChar { get; set; }
        
        /// <summary>
        /// If set to true, wil read only the fields specified as attributes, and will discard other fields in the CSV file
        /// </summary>
        public bool IgnoreUnknownColumns { get; set; }

        // ---------------

        public CsvFileDescription()
        {
            m_cultureInfo = CultureInfo.CurrentCulture;
            FirstLineHasColumnNames = true;
            EnforceCsvColumnAttribute = false;
            QuoteAllFields = false;
            SeparatorChar = ',';
            TextEncoding = Encoding.UTF8;
            DetectEncodingFromByteOrderMarks = true;
            IgnoreTrailingSeparatorChar = false;
            NoSeparatorChar = false;
            UseFieldIndexForReadingData = false;
            UseOutputFormatForParsingCsvValue = false;
            IgnoreUnknownColumns = false;
        }
    }
}
