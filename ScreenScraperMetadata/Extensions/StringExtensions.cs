using System.Text.RegularExpressions;

namespace ScreenScraperMetadata.Extensions
{
    public static class StringExtensions
    {

        private static readonly Regex SanitizeRegex = new Regex(@"((^the\s+)|([^A-Za-z\s]+\s*the(?![A-Za-z])))|((^an\s+)|([^A-Za-z\s]+\s*an(?![A-Za-z])))|((^a\s+)|([^A-Za-z\s]+\s*a(?![A-Za-z])))|[^A-Za-z0-9]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string Sanitize(this string text)
        {
            return SanitizeRegex.Replace(text, "").ToLower();
        }
    }
}
