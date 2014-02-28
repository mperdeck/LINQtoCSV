using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQtoCSV.Tests
{
    public class Person : IAssertable<Person> {
        [CsvColumn(Name = "Name")]
        public string Name { get; set; }
        [CsvColumn(Name = "Last Name")]
        public string LastName { get; set; }
        [CsvColumn(Name = "Age")]
        public int Age { get; set; }

        public void AssertEqual(Person other) {
            Assert.AreEqual(other.Name, Name);
            Assert.AreEqual(other.LastName, LastName);
            Assert.AreEqual(other.Age, Age);
        }
    }
}
