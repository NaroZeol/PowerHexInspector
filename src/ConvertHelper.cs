namespace PowerHexInspector;

public static class Convert
{
    public class ConvertResult
    {
        public string Raw { get; set; }
        public string Format { get; set; }
        public ConvertResult(string raw, string format)
        {
            Raw = raw;
            Format = format;
        }
    }
    public static ConvertResult Dec2Hex(string dec, bool upper = true)
    {
        string raw = System.Convert.ToString(System.Convert.ToInt64(dec, 10), 16);
        if (upper)
        {
            return new ConvertResult(raw.ToUpper(), raw.ToUpper());
        }
        else
        {
            return new ConvertResult(raw.ToLower(), raw.ToLower());
        }
    }

    public static ConvertResult Dec2Bin(string dec, bool spilt = true)
    {
        string raw = System.Convert.ToString(System.Convert.ToInt64(dec, 10), 2);
        if (raw.Length % 4 != 0 && raw != "0")
        {
            raw = raw.PadLeft(raw.Length + (4 - raw.Length % 4), '0');
        }

        if (spilt) // split every 4 bits
        {
            string[] splited = new string[raw.Length / 4];
            for (int i = 0; i < raw.Length / 4; i++)
            {
                splited[i] = raw.Substring(i * 4, 4);
            }
            return new ConvertResult(raw, string.Join(" ", splited));
        }
        return new ConvertResult(raw, raw);
    }

    public static ConvertResult Hex2Dec(string hex)
    {
        string raw = System.Convert.ToInt64(hex, 16).ToString(); 
        return new ConvertResult(raw, raw);
    }

    public static ConvertResult Hex2Bin(string hex, bool spilt = true)
    {
        string raw = System.Convert.ToString(System.Convert.ToInt64(hex, 16), 2);
        if (raw.Length % 4 != 0 && raw != "0")
        {
            raw = raw.PadLeft(raw.Length + (4 - raw.Length % 4), '0'); // Align to 4 bits
        }

        if (spilt) // Insert space every 4 bits
        {
            string[] splited = new string[raw.Length / 4];
            for (int i = 0; i < raw.Length / 4; i++)
            {
                splited[i] = raw.Substring(i * 4, 4);
            }
            return new ConvertResult(raw, string.Join(" ", splited));
        }
        return new ConvertResult(raw, raw);
    }

    public static ConvertResult Bin2Dec(string bin)
    {
        string raw = System.Convert.ToInt64(bin, 2).ToString();
        return new ConvertResult(raw, raw);
    }

    public static ConvertResult Bin2Hex(string bin, bool upper = true)
    {
        string raw = System.Convert.ToString(System.Convert.ToInt64(bin, 2), 16);
        if (upper)
        {
            return new ConvertResult(raw.ToUpper(), raw.ToUpper());
        }
        else
        {
            return new ConvertResult(raw.ToLower(), raw.ToLower());
        }
    }

    public static ConvertResult HexFormat(string hex, bool upper = true)
    {
        if (upper)
        {
            return new ConvertResult(hex.ToUpper(), hex.ToUpper());
        }
        else
        {
            return new ConvertResult(hex.ToLower(), hex.ToLower());
        }
    }

    public static ConvertResult BinFormat(string bin, bool spilt = true)
    {
        bin = bin.PadLeft(bin.Length + (4 - bin.Length % 4), '0');
        if (spilt)
        {
            string[] splited = new string[bin.Length / 4];
            for (int i = 0; i < bin.Length / 4; i++)
            {
                splited[i] = bin.Substring(i * 4, 4);
            }
            return new ConvertResult(bin, string.Join(" ", splited));
        }
        return new ConvertResult(bin, bin);
    }

    public static ConvertResult DecFormat(string dec)
    {
        return new ConvertResult(dec, dec);
    }
}
