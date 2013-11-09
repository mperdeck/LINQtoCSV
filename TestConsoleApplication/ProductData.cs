using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Globalization;
using LINQtoCSV;

namespace TestConsoleApplication
{
// Because the fields in this type are used only indirectly, the compiler
// will warn they are unused or unassigned. Disable those warnings.
#pragma warning disable 0169, 0414, 0649

    class ProductData
    {
        [CsvColumn(FieldIndex = 1)]
        public string name;

        // OutputFormat uses the same codes as the standard ToString method (search MSDN).
        [CsvColumn(FieldIndex = 2, OutputFormat = "d")]
        public DateTime startDate;

        [CsvColumn(FieldIndex = 3, OutputFormat = "dd MMM HH:mm:ss")]
        public DateTime launchTime;

        // Can use both fields and properties
        [CsvColumn(FieldIndex = 4, CanBeNull = false, OutputFormat = "#,000.000")]
        public double weight { get; set; }

        // Following field has no CsvColumn attribute.
        // So will be ignored when library is told to only use data with CsvColumn attribute.
        public int nbrAvailable;

        // Ok to have gaps in FieldIndex order
        [CsvColumn(FieldIndex = 10)]
        public string shopsAvailable;

        // Override field name, so in data files, this field is known as "code" instead of "hexProductCode"
        // This field contains hexadecimal numbers, without leading 0x.
        // This requires setting the NumberStyle property to NumberStyles.HexNumber.
        // Don't forget to import the namespace System.Globalization
        [CsvColumn(Name = "code", FieldIndex = 11, NumberStyle = NumberStyles.HexNumber)]
        public int hexProductCode;

        public string unusedField;

        // FieldIndex order higher then that of next field.
        // So this field will come AFTER next field in the actual data file
        [CsvColumn(FieldIndex = 16)]
        public bool onsale;

        // Override field name, so in data files, this field is known as "price" instead of "retailPrice"
        // OutputFormat uses the same codes as the standard ToString method (search MSDN). Format "C" is for currency.
        [CsvColumn(Name = "price", FieldIndex = 14, CanBeNull = false, OutputFormat = "C")]
        public decimal retailPrice { get; set; }
        
        [CsvColumn(FieldIndex = 30)]
        public string description;

#pragma warning restore 0169, 0414, 0649

        public override string ToString()
        {
            return 
                "name=" + (string.IsNullOrEmpty(name) ? "<null>" : name) + " | " +
                "startDate=" + startDate.ToString() + " | " +
                "launchTime=" + launchTime.ToString() + " | " +
                "weight=" + weight.ToString() + " | " +
                "nbrAvailable=" + nbrAvailable.ToString() + " | " +
                "shopsAvailable=" + ((shopsAvailable==null) ? "<null>" : shopsAvailable) + " | " +
                "hexProductCode=" + hexProductCode.ToString() + " | " +
                "unusedField=" + ((unusedField==null) ? "<null>" : unusedField) + " | " +
                "onsale=" + onsale.ToString() + " |  " +
                "retailPrice=" + retailPrice.ToString() + " | " +
                "description=" + ((description==null) ? "<null>" : description);
        }
    }
}
