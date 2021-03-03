using System.Linq;
using System.Text.RegularExpressions;

namespace RocketUI.Design.Host.Utilities
{
    public static class GlobExpression
    {
        private static readonly RegexOptions RegexOptions = RegexOptions.Compiled
                                                   | RegexOptions.Singleline
                                                   | RegexOptions.CultureInvariant
                                                   | RegexOptions.IgnoreCase
                                                   | RegexOptions.IgnorePatternWhitespace;

        public static Regex ParseFromString(string expr)
        {
            expr = MakeRegexString(expr);
            return new Regex(expr,RegexOptions);
        }

        public static Regex ParseFromStrings(params string[] expressions)
        {
            var expr = string.Join("|", expressions.Select(MakeRegexString));
            return new Regex($"^({expr})$", RegexOptions);
        }

        private static string MakeRegexString(string expr)
        {
            expr = Regex.Escape(expr);
            expr = expr.Replace("\\*", "(?:.*)");
            return expr;
        }
    }
}