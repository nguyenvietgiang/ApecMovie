
using System.Text.RegularExpressions;

namespace ApecMovieCore.Common
{
    public class Generator
    {
        private static readonly Random _random = new Random();

        public static int GenerateSixDigitRandomNumber()
        {
            return _random.Next(100000, 1000000);
        }

        public static string GenerateSlug(string stringName)
        {
            stringName = stringName.ToLowerInvariant();
            stringName = Regex.Replace(stringName, @"[^a-z0-9\s-]", "");
            stringName = Regex.Replace(stringName, @"\s+", "-").Trim();
            return stringName;
        }

    }
}
