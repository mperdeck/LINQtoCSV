using LINQtoCSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace LINQtoCSV.Tests
{
    [TestClass()]
    public class CsvContextWriteTests : Test
    {
        [TestMethod()]
        public void GoodFileCommaDelimitedNamesInFirstLineNLnl()
        {
            // Arrange

            List<ProductData> dataRows_Test = new List<ProductData>();
            dataRows_Test.Add(new ProductData { retailPrice = 4.59M, name = "Wooden toy", startDate = new DateTime(2008, 2, 1), nbrAvailable = 67 });
            dataRows_Test.Add(new ProductData { onsale = true, weight = 4.03, shopsAvailable = "Ashfield", description = "" });
            dataRows_Test.Add(new ProductData { name = "Metal box", launchTime = new DateTime(2009, 11, 5, 4, 50, 0), description = "Great\nproduct" });

            CsvFileDescription fileDescription_namesNl2 = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true,
                EnforceCsvColumnAttribute = false,
                TextEncoding = Encoding.Unicode,
                FileCultureName = "nl-Nl" // default is the current culture
            };

            string expected =
@"name,startDate,launchTime,weight,shopsAvailable,code,price,onsale,description,nbrAvailable,unusedField
Wooden toy,1-2-2008,01 jan 00:00:00,""000,000"",,0,""€ 4,59"",False,,67,
,1-1-0001,01 jan 00:00:00,""004,030"",Ashfield,0,""€ 0,00"",True,"""",0,
Metal box,1-1-0001,05 nov 04:50:00,""000,000"",,0,""€ 0,00"",False,""Great
product"",0,
";

            // Act and Assert

            AssertWrite(dataRows_Test, fileDescription_namesNl2, expected);
        }
    }
}
