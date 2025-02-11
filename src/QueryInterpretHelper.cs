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
            var raw = query.RawUserQuery.Trim().Substring(query.ActionKeyword.Length).Trim();
            queryBase = Base.Invalid;
            queryValue = "";
            isUpper = false;

            // Use C-Style: (only allow lowercase 'x' and 'b')
            // {value}   -> Decimal
            // 0{value}  -> Octal
            // 0x{value} -> Hex
            // 0b{value} -> Binary
            // "{value}" -> ASCII
            if (terms.Count == 1 || terms[0][0] == '"')
            {
                string decimalPattern = @$"^[+-]?([1-9][0-9{FilterPattern}]*)$";
                string octalPattern   = @$"^[+-]?(0[0-7{FilterPattern}]+)$";
                string hexPattern     = @$"^[+-]?(0x[0-9a-fA-F{FilterPattern}]+)$";
                string binaryPattern  = @$"^[+-]?(0b[01{FilterPattern}]+)$";
                string asciiPattern   = @$"^"".*""$";

                if (Regex.IsMatch(raw, decimalPattern) || raw == "0" || raw == "-0")
                {
                    queryBase = Base.Dec;
                    queryValue = raw;
                }
                else if (Regex.IsMatch(raw, octalPattern))
                {
                    queryBase = Base.Oct;
                    queryValue = ReplaceFirstOccurrence(raw, "0", "");
                }
                else if (Regex.IsMatch(raw, hexPattern))
                {
                    queryBase = Base.Hex;
                    queryValue = ReplaceFirstOccurrence(raw, "0x", "");
                }
                else if (Regex.IsMatch(raw, binaryPattern))
                {
                    queryBase = Base.Bin;
                    queryValue = ReplaceFirstOccurrence(raw, "0b", "");
                }
                else if (Regex.IsMatch(raw, asciiPattern))
                {
                    queryBase = Base.Ascii;
                    queryValue = raw[1..^1]; // only remove the first and last double quotes
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
                    "a" => Base.Ascii,
                    _ => Base.Invalid   
                };
                isUpper = char.IsUpper(terms[0][0]);
                queryValue = raw[terms[0].Length..].Trim();
            }

            if (queryBase != Base.Ascii)
            {
                queryValue = Regex.Replace(queryValue, $"[{FilterPattern}]", "");
                queryValue = queryValue.Trim().Replace(" ", "");
            }

            return;
        }
    }
}