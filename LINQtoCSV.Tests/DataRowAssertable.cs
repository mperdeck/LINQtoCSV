using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQtoCSV.Tests
{
    public class DataRowAssertable : List<DataRowItem>, IDataRow, IAssertable<DataRowAssertable>
    {
        public void AssertEqual(DataRowAssertable other)
        {
            Assert.AreNotEqual(other, null);
            Assert.AreEqual(other.Count, this.Count);

            for (var index = 0; index < this.Count; index++)
            {
                Assert.AreEqual(other[index].Value, this[index].Value, index.ToString());
                Assert.AreEqual(other[index].LineNbr, this[index].LineNbr, index.ToString());
            }
        }
    }
}
