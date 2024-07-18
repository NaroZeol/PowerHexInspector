using System.Text.RegularExpressions;
using Wox.Plugin;

namespace PowerHexInspector
{
    public static class QueryInterpretHelper
    {
        private static string ReplaceFirstOccurrence(string input, string pattern, string replacement)
        {
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement, 1);
        }
        private static readonly string FilterPattern = "_,";
        public static void QueryInterpret(Query query, out string queryFormat, out string queryValue)
        {
            var terms = query.Terms;
            queryFormat = "";
            queryValue = "";

            // Use C-Style:
            // {value}   -> Decimal
            // 0{value}  -> Octal
            // 0x{value} -> Hex
            // 0b{value} -> Binary
            if (terms.Count == 1)
            {
                string decimalPattern = @$"^[+-]?([1-9][0-9{FilterPattern}]*)$";
                string octalPattern   = @$"^[+-]?(0[0-7{FilterPattern}]+)$";
                string hexPattern     = @$"^[+-]?(0x[0-9a-fA-F{FilterPattern}]+)$";
                string binaryPattern  = @$"^[+-]?(0b[01{FilterPattern}]+)$";

                if (Regex.IsMatch(terms[0], decimalPattern) || terms[0] == "0" || terms[0] == "-0")
                {
                    queryFormat = "d";
                    queryValue = terms[0];
                }
                else if (Regex.IsMatch(terms[0], octalPattern))
                {
                    queryFormat = "o";
                    queryValue = ReplaceFirstOccurrence(terms[0], "0", "");
                }
                else if (Regex.IsMatch(terms[0], hexPattern))
                {
                    queryFormat = "h";
                    queryValue = ReplaceFirstOccurrence(terms[0], "0x", "");
                }
                else if (Regex.IsMatch(terms[0], binaryPattern))
                {
                    queryFormat = "b";
                    queryValue = ReplaceFirstOccurrence(terms[0], "0b", "");
                }
            }
            else if (terms.Count >= 2) // Use specific Format: {Keyword} [Format] [Value]
            {
                queryFormat = terms[0];
                queryValue = string.Join(" ", terms.Skip(1));
            }

            queryValue = Regex.Replace(queryValue, $"[{FilterPattern}]", "");
            queryValue = queryValue.Trim().Replace(" ", "");

            return;
        }
    }
}