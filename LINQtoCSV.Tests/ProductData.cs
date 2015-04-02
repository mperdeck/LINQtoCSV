using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using LINQtoCSV;

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

		[CsvColumn(FieldIndex = 40)]
		public Guid id;

#pragma warning restore 0169, 0414, 0649

        public void AssertEqual(ProductData other)
        {
            Assert.AreNotEqual(other, null);

            Assert.AreEqual(other.name, name, "name");
            Assert.AreEqual(other.startDate, startDate, "startDate");
            Assert.AreEqual(other.launchTime, launchTime, "launchTime");
            Assert.AreEqual(other.weight, weight, "weight");
            Assert.AreEqual(other.nbrAvailable, nbrAvailable, "nbrAvailable");
            Assert.AreEqual(other.shopsAvailable, shopsAvailable, "shopsAvailable");
            Assert.AreEqual(other.hexProductCode, hexProductCode, "hexProductCode");
            Assert.AreEqual(other.onsale, onsale, "onsale");
            Assert.AreEqual(other.retailPrice, retailPrice, "retailPrice");
            Assert.AreEqual(Utils.NormalizeString(other.description), Utils.NormalizeString(description), "description");
			Assert.AreEqual(other.id, id, "id");
        }
    }
}
