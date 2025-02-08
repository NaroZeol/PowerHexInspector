using System.Text.RegularExpressions;
using Wox.Plugin;

namespace PowerHexInspector
{
    public static class QueryInterpretHelper
    {
        private static readonly string FilterPattern = "_,";

        private static string ReplaceFirstOccurrence(string input, string pattern, string replacement)
        {
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement, 1);
        }

        public static void QueryInterpret(Query query, out Base queryBase, out string queryValue, out bool isUpper)
        {
            var terms = query.Terms;
            queryBase = Base.Invalid;
            queryValue = "";
            isUpper = false;

            // Use C-Style: (only allow lowercase 'x' and 'b')
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
                    queryBase = Base.Dec;
                    queryValue = terms[0];
                }
                else if (Regex.IsMatch(terms[0], octalPattern))
                {
                    queryBase = Base.Oct;
                    queryValue = ReplaceFirstOccurrence(terms[0], "0", "");
                }
                else if (Regex.IsMatch(terms[0], hexPattern))
                {
                    queryBase = Base.Hex;
                    queryValue = ReplaceFirstOccurrence(terms[0], "0x", "");
                }
                else if (Regex.IsMatch(terms[0], binaryPattern))
                {
                    queryBase = Base.Bin;
                    queryValue = ReplaceFirstOccurrence(terms[0], "0b", "");
                }
            }
            else if (terms.Count >= 2) // Use specific Format: {Keyword} [Format] [Value]
            {
                queryBase = terms[0].ToLower() switch
                {
                    "h" => Base.Hex,
                    "b" => Base.Bin,
                    "d" => Base.Dec,
                    "o" => Base.Oct,
                    _ => Base.Invalid   
                };
                isUpper = char.IsUpper(terms[0][0]);
                queryValue = string.Join(" ", terms.Skip(1));
            }

            queryValue = Regex.Replace(queryValue, $"[{FilterPattern}]", "");
            queryValue = queryValue.Trim().Replace(" ", "");

            return;
        }
    }
}