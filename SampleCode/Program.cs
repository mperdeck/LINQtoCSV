using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SampleCode
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // NOT SHOWN IN EXAMPLE IN ARTICLE
            // ReadFileWithExceptionHandling();

            // ------------------
            // Simple Read example

            CsvContext cc = new CsvContext();

            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true,
                FileCultureName = "en-US" // default is the current culture
            };

            using (StreamReader rdr = new StreamReader("../../TestFiles/products.csv"))
            {
                IEnumerable<Product> products =
                    cc.Read<Product>(rdr, inputFileDescription);

                var productsByName =
                    from p in products
                    orderby p.Name
                    select new { p.Name, p.LaunchDate, p.Price, p.Description };

                foreach (var item in productsByName) { Console.WriteLine(item); }

                // ------------------
                // Simple Write example 1 - without using a class with attributes

                CsvFileDescription outputFileDescription = new CsvFileDescription
                {
                    QuoteAllFields = false,
                    SeparatorChar = '\t', // tab delimited
                    FirstLineHasColumnNames = false,
                    FileCultureName = "nl-NL" // language/country code of The Netherlands
                };

                var productsNetherlands =
                    from p in products
                    where p.Country == "Netherlands"
                    select new { p.Name, p.LaunchDate, p.Price, p.Description };

                using (StreamWriter wrt = new StreamWriter("../../TestFiles/output-products-Netherlands.csv"))
                {
                    cc.Write(
                        productsNetherlands,
                        wrt,
                        outputFileDescription);

                    // ------------------
                    // Simple Write example 2 - using a class with format attributes, etc.
                    // In this example, using class Product (defined in Product.cs)

                    List<Product> products2 = new List<Product>();
                    products2.Add(new Product { Name = "Desk Lamp", Country = "Netherlands", Price = 5.60M });
                    products2.Add(new Product { Name = "Garden Lamp", Country = "Germany", Price = 16.90M });
                    products2.Add(new Product { Name = "Chandelier", Country = "Austria", Price = 109.00M });

                    using (StreamWriter wrt2 = new StreamWriter("../../TestFiles/output-products-Netherlands-formatted.csv"))
                    {
                        cc.Write(
                            products2,
                            wrt2,
                            outputFileDescription);
                    }
                }
            }
        }

        public static void ShowErrorMessage(string errorMessage)
        {
            // show errorMessage to user
            // .....
        }

        public static void ReadFileWithExceptionHandling()
        {
            try
            {
                CsvContext cc = new CsvContext();

                CsvFileDescription inputFileDescription = new CsvFileDescription
                {
                    MaximumNbrExceptions = 50 // limit number of aggregated exceptions to 50
                };

                using (StreamReader rdr = new StreamReader("../../TestFiles/products.csv"))
                {
                    IEnumerable<Product> products =
                        cc.Read<Product>(rdr, inputFileDescription);

                    // NOT SHOWN IN EXAMPLE IN ARTICLE
                    foreach (var item in products) { Console.WriteLine(item); }
                }

                // Do data processing
                // ...........
            }
            catch (AggregatedException ae)
            {
                // Process all exceptions generated while processing the file

                List<Exception> innerExceptionsList =
                    (List<Exception>)ae.Data["InnerExceptionsList"];

                foreach (Exception e in innerExceptionsList)
                {
                    ShowErrorMessage(e.Message);
                }
            }
            catch (DuplicateFieldIndexException dfie)
            {
                // name of the class used with the Read method - in this case "Product"
                string typeName = Convert.ToString(dfie.Data["TypeName"]);

                // Names of the two fields or properties that have the same FieldIndex
                string fieldName = Convert.ToString(dfie.Data["FieldName"]);
                string fieldName2 = Convert.ToString(dfie.Data["FieldName2"]);

                // Actual FieldIndex that the two fields have in common
                int commonFieldIndex = Convert.ToInt32(dfie.Data["Index"]);

                // Do some processing with this information
                // .........

                // Inform user of error situation
                ShowErrorMessage(dfie.Message);
            }
            catch (Exception e)
            {
                ShowErrorMessage(e.Message);
            }
        }
    }
}