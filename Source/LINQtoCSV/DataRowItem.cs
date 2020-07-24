using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQtoCSV
{
    public class DataRowItem
    {
        private string m_value;
        private int m_lineNbr;

        public DataRowItem(string value, int lineNbr)
        {
            m_value = value;
            m_lineNbr = lineNbr;
        }

        public int LineNbr
        {
            get { return m_lineNbr; }
        }

        public string Value
        {
            get { return m_value; }
        }
    }
}
