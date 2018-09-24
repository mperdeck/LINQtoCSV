using System;
using System.Globalization;
using Xunit;

namespace LINQtoCSV.Tests
{
    // Because the fields in this type are used only indirectly, the compiler
    // will warn they are unused or unassigned. Disable those warnings.
#pragma warning disable 0169, 0414, 0649

    internal class ProductData : IAssertable<ProductData>
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

        public void AssertEqual(ProductData other)
        {
            Assert.NotNull(other);

            Assert.Equal(other.name, name);
            Assert.Equal(other.startDate, startDate);
            Assert.Equal(other.launchTime, launchTime);
            Assert.Equal(other.weight, weight);
            Assert.Equal(other.nbrAvailable, nbrAvailable);
            Assert.Equal(other.shopsAvailable, shopsAvailable);
            Assert.Equal(other.hexProductCode, hexProductCode);
            Assert.Equal(other.onsale, onsale);
            Assert.Equal(other.retailPrice, retailPrice);
            Assert.Equal(Utils.NormalizeString(other.description), Utils.NormalizeString(description));
        }
    }
}
