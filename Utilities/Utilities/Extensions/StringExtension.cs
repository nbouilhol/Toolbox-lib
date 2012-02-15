
namespace Utilities.Extensions
{
    public static class StringExtension
    {
        public static decimal DistanceToInPercent(this string source, string target)
        {
            return 1 - ((decimal)Levenshtein.CalculateDistance(source, target) / target.Length);
        }

        public static int DistanceTo(this string source, string target)
        {
            return Levenshtein.CalculateDistance(source, target);
        }

        public static int ToInt(this string source)
        {
            int result;
            return int.TryParse(source, out result)
                ? result
                : default(int);
        }
    }
}
