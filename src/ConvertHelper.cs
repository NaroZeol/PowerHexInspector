namespace PowerHexInspector;

public class Convert
{
    private readonly SettingsHelper settings;
    public Convert(SettingsHelper settingHelper)
    {
        this.settings = settingHelper;
    }
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
    private string HexToBigEndian(string hex)
    {
        if (hex.Length < 2)
        {
            return hex; // No need to reverse
        }
        if (hex.Length % 2 != 0)
        {
            hex = hex.PadLeft(hex.Length + 1, '0');
        }
        string[] splited = new string[hex.Length / 2];
        for (int i = 0; i < hex.Length / 2; i++)
        {
            splited[i] = hex.Substring(i * 2, 2);
        }
        Array.Reverse(splited);
        return string.Join("", splited);
    }
    private string BinToBigEndian(string bin)
    {
        if (bin.Length < 8)
        {
            return bin; // No need to reverse
        }
        if (bin.Length % 8 != 0)
        {
            bin = bin.PadLeft(bin.Length + (8 - bin.Length % 8), '0');
        }
        string[] splited = new string[bin.Length / 8];
        for (int i = 0; i < bin.Length / 8; i++)
        {
            splited[i] = bin.Substring(i * 8, 8);
        }
        Array.Reverse(splited);
        return string.Join("", splited);
    }
    private string HexToLittleEndian(string hex)
    {
        return HexToBigEndian(hex); // Same logic
    }
    private string BinToLittleEndian(string bin)
    {
        return BinToBigEndian(bin); // Same logic
    }
    public ConvertResult Dec2Hex(string dec, bool upper)
    {
        string raw = System.Convert.ToString(System.Convert.ToInt64(dec, 10), 16);
        if (settings.InputEndian == SettingsHelper.BigEndian)
        {
            raw = HexToBigEndian(raw);
        }

        if (upper)
        {
            return new ConvertResult(raw.ToUpper(), raw.ToUpper());
        }
        else
        {
            return new ConvertResult(raw.ToLower(), raw.ToLower());
        }
    }
    public ConvertResult Dec2Bin(string dec)
    {
        string raw = System.Convert.ToString(System.Convert.ToInt64(dec, 10), 2);
        if (settings.InputEndian == SettingsHelper.BigEndian)
        {
            raw = BinToBigEndian(raw);
        }

        if (settings.SplitBinary) // split every 4 bits
        {
            if (raw.Length % 4 != 0)
            {
                raw = raw.PadLeft(raw.Length + (4 - raw.Length % 4), '0');
            }
            string[] splited = new string[raw.Length / 4];
            for (int i = 0; i < raw.Length / 4; i++)
            {
                splited[i] = raw.Substring(i * 4, 4);
            }
            return new ConvertResult(raw, string.Join(" ", splited));
        }
        return new ConvertResult(raw, raw);
    }

    public ConvertResult Hex2Dec(string hex)
    {
        if (settings.InputEndian == SettingsHelper.BigEndian) // input is big endian
        {
            hex = HexToLittleEndian(hex); // Convert to little endian
        }
        string raw = System.Convert.ToInt64(hex, 16).ToString();
        return new ConvertResult(raw, raw);
    }

    public ConvertResult Hex2Bin(string hex)
    {
        if (settings.InputEndian == SettingsHelper.BigEndian)
        {
            hex = HexToLittleEndian(hex);
        }
        string raw = System.Convert.ToString(System.Convert.ToInt64(hex, 16), 2);
        if (settings.OutputEndian == SettingsHelper.BigEndian)
        {
            raw = BinToBigEndian(raw);
        }

        if (settings.SplitBinary) // Insert space every 4 bits
        {
            if (raw.Length % 4 != 0)
            {
                raw = raw.PadLeft(raw.Length + (4 - raw.Length % 4), '0'); // Align to 4 bits
            }
            string[] splited = new string[raw.Length / 4];
            for (int i = 0; i < raw.Length / 4; i++)
            {
                splited[i] = raw.Substring(i * 4, 4);
            }
            return new ConvertResult(raw, string.Join(" ", splited));
        }
        return new ConvertResult(raw, raw);
    }

    public ConvertResult Bin2Dec(string bin)
    {
        if (settings.InputEndian == SettingsHelper.BigEndian)
        {
            bin = BinToLittleEndian(bin);
        }
        string raw = System.Convert.ToInt64(bin, 2).ToString();
        return new ConvertResult(raw, raw);
    }

    public ConvertResult Bin2Hex(string bin, bool upper)
    {
        if (settings.InputEndian == SettingsHelper.BigEndian)
        {
            bin = BinToLittleEndian(bin);
        }
        string raw = System.Convert.ToString(System.Convert.ToInt64(bin, 2), 16);
        if (settings.OutputEndian == SettingsHelper.BigEndian)
        {
            raw = HexToBigEndian(raw);
        }

        if (upper)
        {
            return new ConvertResult(raw.ToUpper(), raw.ToUpper());
        }
        else
        {
            return new ConvertResult(raw.ToLower(), raw.ToLower());
        }
    }

    public ConvertResult HexFormat(string hex, bool upper)
    {
        if (settings.InputEndian != settings.OutputEndian)
        {
            if (settings.InputEndian == SettingsHelper.LittleEndian && settings.OutputEndian == SettingsHelper.BigEndian)
            {
                hex = HexToBigEndian(hex);
            }
            else if (settings.InputEndian == SettingsHelper.BigEndian && settings.OutputEndian == SettingsHelper.LittleEndian)
            {
                hex = HexToLittleEndian(hex);
            }
        }
        if (upper)
        {
            return new ConvertResult(hex.ToUpper(), hex.ToUpper());
        }
        else
        {
            return new ConvertResult(hex.ToLower(), hex.ToLower());
        }
    }

    public ConvertResult BinFormat(string bin)
    {
        if (settings.InputEndian != settings.OutputEndian)
        {
            if (settings.InputEndian == SettingsHelper.LittleEndian && settings.OutputEndian == SettingsHelper.BigEndian)
            {
                bin = BinToBigEndian(bin);
            }
            else if (settings.InputEndian == SettingsHelper.BigEndian && settings.OutputEndian == SettingsHelper.LittleEndian)
            {
                bin = BinToLittleEndian(bin);
            }
        }
        if (settings.SplitBinary)
        {
            if (bin.Length % 4 != 0) // Align to 4 bits
            {
                bin = bin.PadLeft(bin.Length + (4 - bin.Length % 4), '0');
            }
            string[] splited = new string[bin.Length / 4];
            for (int i = 0; i < bin.Length / 4; i++)
            {
                splited[i] = bin.Substring(i * 4, 4);
            }
            return new ConvertResult(bin, string.Join(" ", splited));
        }
        return new ConvertResult(bin, bin);
    }

    public ConvertResult DecFormat(string dec)
    {
        return new ConvertResult(dec, dec);
    }
}
