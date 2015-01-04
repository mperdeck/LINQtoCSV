using System;
using System.Globalization;

namespace LINQtoCSV
{
    /// <summary>
    /// When set to a field or property, allows the framework to recognize it from a CSV file, according to the rules specified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class CsvColumnAttribute : Attribute
    {
        internal const int mc_DefaultFieldIndex = int.MaxValue;

        public string Name { get; set; }

        public bool CanBeNull { get; set; }

        public int FieldIndex { get; set; }

        public NumberStyles NumberStyle { get; set; }

        public DateTimeStyles DateTimeStyle { get; set; }

        public string OutputFormat { get; set; }

        public int CharLength { get; set; }

        public CsvColumnAttribute()
        {
            Name = "";
            FieldIndex = mc_DefaultFieldIndex;
            CanBeNull = true;
            NumberStyle = NumberStyles.Any;
            DateTimeStyle = DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces;
            OutputFormat = "G";
        }

        public CsvColumnAttribute(
                    string name,
                    int fieldIndex,
                    bool canBeNull,
                    string outputFormat,
                    NumberStyles numberStyle,
                    DateTimeStyles dateTimeStyle,
                    int charLength)
        {
            Name = name;
            FieldIndex = fieldIndex;
            CanBeNull = canBeNull;
            NumberStyle = numberStyle;
            DateTimeStyle = dateTimeStyle;
            OutputFormat = outputFormat;

            CharLength = charLength;
        }
    }
}