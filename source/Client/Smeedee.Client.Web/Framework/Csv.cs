using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smeedee.Client.Web.Framework
{
    public static class Csv
    {
        private const char lineSeparator = '\a'; //unprintable character alert
        private const char columnSeparator = '\f'; //unprintable character form feed

        public static IEnumerable<string[]> FromCsv(string str)
        {
            return str.Split(lineSeparator).Select(s => s.Split(columnSeparator));
        }

        public static string ToCsv(IEnumerable<string[]> csv)
        {
            var rows = csv.Select(row => String.Join(columnSeparator.ToString(), row.Select(StripSpecialChars)));
            return String.Join(lineSeparator.ToString(), rows);
        }

        private static string StripSpecialChars(string str)
        {
            return str.Replace(columnSeparator.ToString(), "").Replace(lineSeparator.ToString(), "");
        }
    }
}