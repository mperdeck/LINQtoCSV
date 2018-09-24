using Xunit;

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
            Assert.Equal(other.Name, Name);
            Assert.Equal(other.LastName, LastName);
            Assert.Equal(other.Age, Age);
        }
    }
}
