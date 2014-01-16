using LINQtoCSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;

namespace LINQtoCSV.Tests
{
    [TestClass()]
    public class CsvContextReadTests : Test
    {
        [TestMethod()]
        public void GoodFileNoSeparatorCharUSEnglish()
        {
            // Arrange

            CsvFileDescription fileDescription_namesUs = new CsvFileDescription
            {
                NoSeparatorChar = true,
                FirstLineHasColumnNames = false,
                EnforceCsvColumnAttribute = true, // default is false
                FileCultureName = "en-US" // default is the current culture
            };

            string testInput =
@"AAAAAAAA34.18405/23/08\n
BBBBBBBB10.31105/12/12\n
CCCCCCCC12.00012/23/08";

            var expected = new[] {
                new ProductDataSpecificFieldIndex() {
                    name = "AAAAAAAA", weight = 34.184, startDate = new DateTime(2008, 5, 23),
                },
                new ProductDataSpecificFieldIndex {
                    name = "BBBBBBBB", weight = 10.311, startDate = new DateTime(2012, 5, 12), 
                },
                new ProductDataSpecificFieldIndex {
                    name = "CCCCCCCC", weight = 12.000, startDate = new DateTime(2008, 12, 23),
                }
            };

            // Act and Assert

            AssertRead(testInput, fileDescription_namesUs, expected);
        }


        [TestMethod()]
        public void GoodFileCommaDelimitedNamesInFirstLineUSEnglish()
        {
            // Arrange

            CsvFileDescription fileDescription_namesUs = new CsvFileDescription
            {
                SeparatorChar = ',', // default is ','
                FirstLineHasColumnNames = true,
                EnforceCsvColumnAttribute = false, // default is false
                FileCultureName = "en-US" // default is the current culture
            };

            string testInput =
@"name,        weight,       startDate, launchTime,               nbrAvailable,onsale,shopsAvailable,    code,  price,    description
moonbuggy,   34.184,       5/23/08,   5-May-2009 4:11 pm,       1205,        true,  ""Paris, New York"", 1F,    $540.12,  newly launched product
""mouse trap"",45E-5,        1/2/1985,  ""7 August 1988, 0:00 am"", ""4,030"",     FALSE, ""This field has
a newline"", 100, ""$78,300"", ""This field has quotes(""""), and
two newlines
and a quoted """"string""""""
dog house,    ""45,230,990"",29 Feb 2004, ,                  -56,        True,"""",                  FF10, ""12,008""";

            var expected = new [] {
                new ProductData {
                    name = "moonbuggy", weight = 34.184, startDate = new DateTime(2008, 5, 23), launchTime = new DateTime(2009, 5, 5, 16, 11, 0),
                    nbrAvailable = 1205, onsale = true, shopsAvailable = "Paris, New York", hexProductCode = 31, retailPrice = 540.12M,
                    description = "newly launched product"
                },
                new ProductData {
                    name = "mouse trap", weight = 45E-5, startDate = new DateTime(1985, 1, 2), launchTime = new DateTime(1988, 8, 7, 0, 0, 0),
                    nbrAvailable = 4030, onsale = false, shopsAvailable = @"This field has
a newline", hexProductCode = 256, retailPrice = 78300M,
                    description = @"This field has quotes(""), and
two newlines
and a quoted ""string"""
                },
                new ProductData {
                    name = "dog house", weight = 45230990, startDate = new DateTime(2004, 2, 29), launchTime = default(DateTime),
                    nbrAvailable = -56, onsale = true, shopsAvailable = "", hexProductCode = 65296, retailPrice = 12008M,
                    description = null
                }
            };

            // Act and Assert

            AssertRead(testInput, fileDescription_namesUs, expected);
        }

        [TestMethod()]
        public void GoodFileTabDelimitedNoNamesInFirstLineNLnl()
        {
            // Arrange

            CsvFileDescription fileDescription_nonamesNl = new CsvFileDescription
            {
                SeparatorChar = '\t', // tab character
                FirstLineHasColumnNames = false,
                EnforceCsvColumnAttribute = true,
                FileCultureName = "nl-NL" // default is the current culture
            };

            string testInput =
"moonbuggy\t       23/5/08\t   5-Mei-2009 16:11 pm\t   34.184\t  \"Paris, New York\"\t 1F\t    €540,12\t        true\t  newly launched product\r\n\"mouse trap\"\t        2/1/1985\t  \"7 Augustus 1988\t 0:00\"\t45E-5\t \"This field has\r\na newline\"\t 100\t \"€78.300\"\t     FALSE\t \"This field has quotes(\"\"), and\r\ntwo newlines\r\nand a quoted \"\"string\"\"\"\r\ndog house\t29 Feb 2004\t \t    \"45.230.990\"\t\"\"\t                  FF10\t \"12.008\"\t        True";
            var expected = new[] {
                new ProductData {
                    name = "moonbuggy", weight = 34184, startDate = new DateTime(2008, 5, 23), launchTime = new DateTime(2009, 5, 5, 16, 11, 0),
                    nbrAvailable = 0, onsale = true, shopsAvailable = "Paris, New York", hexProductCode = 31, retailPrice = 540.12M,
                    description = "newly launched product"
                },
                new ProductData {
                    name = "mouse trap", weight = 45E-5, startDate = new DateTime(1985, 1, 2), launchTime = new DateTime(1988, 8, 7, 0, 0, 0),
                    nbrAvailable = 0, onsale = false, shopsAvailable = @"This field has
a newline", hexProductCode = 256, retailPrice = 78300M,
                    description = @"This field has quotes(""), and
two newlines
and a quoted ""string"""
                },
                new ProductData {
                    name = "dog house", weight = 45230990, startDate = new DateTime(2004, 2, 29), launchTime = default(DateTime),
                    nbrAvailable = 0, onsale = true, shopsAvailable = "", hexProductCode = 65296, retailPrice = 12008M,
                    description = null
                }
            };

            // Act and Assert

            AssertRead(testInput, fileDescription_nonamesNl, expected);
        }
    }
}
