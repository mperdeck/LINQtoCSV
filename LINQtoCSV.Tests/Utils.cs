namespace LINQtoCSV.Tests
{
    public static class Utils
    {
        public static string NormalizeString(string s)
        {
            if (s == null)
            {
                return null;
            }

            return s.Replace("\r\n", "\n");
        }
    }
}
