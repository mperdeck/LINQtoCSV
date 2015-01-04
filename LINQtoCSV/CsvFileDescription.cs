using System.Globalization;
using System.Text;

namespace LINQtoCSV
{
    /// <summary>
    /// Descriptors for a readable CSV file.
    /// </summary>
    public class CsvFileDescription
    {
        // Culture used to process the CSV values, specifically numbers and dates.
        private CultureInfo m_cultureInfo = CultureInfo.CurrentCulture;

        private int m_maximumNbrExceptions = 100;

        /// <summary>
        /// Gets or sets the character used to separate fields in the file.
        /// By default, this is comma (,).
        /// For a tab delimited file, you would set this to
        /// the tab character ('\t').
        /// </summary>
        /// <value>
        /// The separator character.
        /// </value>
        public char SeparatorChar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there is no separator character.
        /// </summary>
        /// <value>
        /// <c>true</c> if there is no separator character; otherwise, <c>false</c>.
        /// </value>
        public bool NoSeparatorChar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all fields are quoted, whatever their content.
        /// If false, only fields containing a FieldSeparator character, a quote or a newline are quoted.
        /// Only used when writing a file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if quoting all fields; otherwise, <c>false</c>.
        /// </value>
        public bool QuoteAllFields { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column names are written in the
        /// first line of the new file when writing, or whether the column names are read
        /// from the first line of the file when reading.
        /// </summary>
        /// <value>
        /// <c>true</c> if first line has or should write column names; otherwise, <c>false</c>.
        /// </value>
        public bool FirstLineHasColumnNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only public fields and properties with the
        /// <see cref="CsvColumnAttribute"/> attribute are recognized. If false, all public fields
        /// and properties are used.
        /// </summary>
        /// <value>
        /// <c>true</c> if, enforcing CSV column attribute; otherwise, <c>false</c>.
        /// </value>
        public bool EnforceCsvColumnAttribute { get; set; }

        /// <summary>
        /// Gets or sets the name of the file culture. FileCultureName
        /// and FileCultureInfo both get/set the CultureInfo used for
        /// the file.
        /// </summary>
        /// <remarks>
        /// For example, if the file uses Dutch date and number
        /// formats while the current culture is US English, set
        /// FileCultureName to "nl-NL". To simply use the current
        /// culture, leave the culture as is.
        /// </remarks>
        /// <value>
        /// The name of the file culture.
        /// </value>
        public string FileCultureName
        {
            get { return m_cultureInfo.Name; }
            set { m_cultureInfo = new CultureInfo(value); }
        }

        /// <summary>
        /// Gets or sets the info of the file culture. FileCultureName
        /// and FileCultureInfo both get/set the CultureInfo used for
        /// the file.
        /// </summary>
        /// <remarks>
        /// For example, if the file uses Dutch date and number
        /// formats while the current culture is US English, set
        /// FileCultureName to "nl-NL". To simply use the current
        /// culture, leave the culture as is.
        /// </remarks>
        /// <value>
        /// The information of the file culture.
        /// </value>
        public CultureInfo FileCultureInfo
        {
            get { return m_cultureInfo; }
            set { m_cultureInfo = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of exceptions before the AggregateException
        /// is thrown.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When reading a file, exceptions thrown while the file is being read
        /// are captured in an aggregate exception. That aggregate exception is then
        /// thrown at the end - to make it easier to solve multiple problems with the
        /// input file in one. 
        /// </para>
        /// <para>
        /// However, after MaximumNbrExceptions, the aggregate exception is thrown
        /// immediately.
        /// </para>
        /// <para>
        /// To not have a maximum at all, set this to -1.
        /// </para>
        /// </remarks>
        /// <value>
        /// The maximum number of exceptions.
        /// </value>
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
        /// If set to true, will read only the fields specified as attributes, and will discard other fields in the CSV file
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
