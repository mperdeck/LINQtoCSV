using LINQtoCSV;
using System;

namespace SampleCode
{
// Because the fields in this type are used only indirectly, the compiler
// will warn they are unused or unassigned. Disable those warnings.
#pragma warning disable 0169, 0414, 0649

    class Product
    {
        [CsvColumn(Name = "ProductName", FieldIndex = 1)]
        public string Name { get; set; }

        [CsvColumn(FieldIndex = 2, OutputFormat = "dd MMM HH:mm:ss")]
        public DateTime LaunchDate { get; set; }

        [CsvColumn(FieldIndex = 3, CanBeNull = false, OutputFormat = "C")]
        public decimal Price { get; set; }

        [CsvColumn(FieldIndex = 4)]
        public string Country { get; set; }

        [CsvColumn(FieldIndex = 5)]
        public string Description { get; set; }
    }

#pragma warning restore 0169, 0414, 0649
}
