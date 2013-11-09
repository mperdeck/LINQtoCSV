using System;
using System.Globalization;

namespace LINQtoCSV
{
    /// <summary>
    /// Summary description for CsvInputFormat
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field |
                           System.AttributeTargets.Property)
    ]
    public class CsvInputFormatAttribute : System.Attribute
    {
        private NumberStyles m_NumberStyle = NumberStyles.Any;
        public NumberStyles NumberStyle
        {
            get { return m_NumberStyle; }
            set { m_NumberStyle = value; }
        }

        public CsvInputFormatAttribute(NumberStyles numberStyle)
        {
            m_NumberStyle = numberStyle;
        }
    }
}
