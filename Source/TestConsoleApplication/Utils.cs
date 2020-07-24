using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using LINQtoCSV;

namespace TestConsoleApplication
{
    class Utils
    {
        /// ///////////////////////////////////////////////////////////////////////
        /// OutputException
        /// 
        public static void OutputException(Exception e)
        {
            Console.WriteLine("#################### Exception");
            Console.WriteLine(e.GetType().ToString());
            Console.WriteLine(e.Message);
            if (e.Data != null)
            {
                foreach (DictionaryEntry de in e.Data)
                {
                    Console.WriteLine(de.Key + "=" + de.Value);
                }
            }

            Console.WriteLine("");

            if (e is AggregatedException)
            {
                foreach (Exception e2 in ((AggregatedException)e).m_InnerExceptionsList)
                {
                    Console.WriteLine("#################### Inner Exception");
                    OutputException(e2);
                }
            }
        }

        /// ///////////////////////////////////////////////////////////////////////
        /// OutputException
        /// 
        public static void OutputData<T>(IEnumerable<T> dataRows, string title)
        {
            Console.WriteLine("--------------- " + title);
            foreach (T row in dataRows)
            {
                Console.WriteLine(row);
                Console.WriteLine("");
            }
        }
    }
}
