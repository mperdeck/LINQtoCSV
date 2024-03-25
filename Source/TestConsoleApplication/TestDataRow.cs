using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LINQtoCSV;

namespace TestConsoleApplication
{
    internal class TestDataRow : List<DataRowItem>, IDataRow
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                sb.AppendLine(string.Format("{0}) line: {1}, value: {2}", i, this[i].LineNbr, this[i].Value));
                sb.AppendLine("--------------");
            }

            return sb.ToString();
        }
    }
}
