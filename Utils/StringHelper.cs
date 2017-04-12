using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Utils
{
    public static class StringHelper
    {
        public static string StripDomain(this string logonName)
        {
            int slashIndex = logonName.IndexOf("\\", StringComparison.OrdinalIgnoreCase);
            string userName = slashIndex >= 0 ? logonName.Substring(slashIndex + 1).ToUpper() : logonName.ToUpper();
            return userName;
        }

        public static string GetNameFromCommonNameMatch(this string commonName)
        {
            const string commonNameRegularExpression = "CN=(?<name>[^,]+),";
            Match managerMatch = Regex.Match(commonName, commonNameRegularExpression);
            if (!managerMatch.Success)
            {
                return string.Empty;
            }
            return managerMatch.Groups["name"].Value;
        }

        public static string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
    }
}
