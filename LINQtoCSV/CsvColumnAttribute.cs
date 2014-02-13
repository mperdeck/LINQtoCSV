using System;
using System.Globalization;

namespace LINQtoCSV
{

    /// <summary>
    /// Summary description for CsvColumnAttribute
    /// </summary>

    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property, 
                           AllowMultiple = false)
    ]
    public class CsvColumnAttribute : System.Attribute
    {
        internal const int mc_DefaultFieldIndex = Int32.MaxValue;

        public string Name { get; set; }
        public bool CanBeNull { get; set; }
        public int FieldIndex { get; set; }
        public NumberStyles NumberStyle { get; set; }
        public string OutputFormat { get; set; }
        public int CharLength { get; set; }

        public CsvColumnAttribute()
        {
            Name = "";
            FieldIndex = mc_DefaultFieldIndex;
            CanBeNull = true;
            NumberStyle = NumberStyles.Any;
            OutputFormat = "G";
        }

        public CsvColumnAttribute(
                    string name, 
                    int fieldIndex, 
                    bool canBeNull,
                    string outputFormat,
                    NumberStyles numberStyle,
                    int charLength)
        {
            Name = name;
            FieldIndex = fieldIndex;
            CanBeNull = canBeNull;
            NumberStyle = numberStyle;
            OutputFormat = outputFormat;

            CharLength = charLength;
        }
    }
}
