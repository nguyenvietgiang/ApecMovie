
namespace ApecMovieCore.Common
{
    public class Generator
    {
        private static readonly Random _random = new Random();

        public static int GenerateSixDigitRandomNumber()
        {
            return _random.Next(100000, 1000000);
        }
    }
}
