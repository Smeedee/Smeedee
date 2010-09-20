using System;

namespace Smeedee.Integration.CI.Hudson_XML.Helper_classes
{
    public class URL
    {
        public static string AppendToBaseURL(string baseURL, params string[] appends)
        {
            if (appends.Length == 0)
                return baseURL;

            var result = baseURL;

            foreach (string append in appends)
            {
                var beginningURL = result.TrimEnd('/');
                var atEnd = append.TrimStart('/');

                result = String.Format("{0}/{1}", beginningURL, atEnd);
            }

            return result;
        }

    }
}
