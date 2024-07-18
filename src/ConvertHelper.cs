namespace PowerHexInspector;
public class ConvertResult(string raw, string formated)
{
    public string Raw { get; set; } = raw;
    public string Formated { get; set; } = formated;
}
public enum BaseType
{
    Bin = 2,
    Oct = 8,
    Dec = 10,
    Hex = 16,
}
public class Convert(SettingsHelper settingHelper)
{
    private readonly SettingsHelper settings = settingHelper;
    public bool is_upper;

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
    private string OctToBigEndian(string oct)
    {
        if (oct.Length < 3)
        {
            return oct; // No need to reverse
        }
        if (oct.Length % 3 != 0)
        {
            oct = oct.PadLeft(oct.Length + (3 - oct.Length % 3), '0');
        }
        string[] splited = new string[oct.Length / 3];
        for (int i = 0; i < oct.Length / 3; i++)
        {
            splited[i] = oct.Substring(i * 3, 3);
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
    private string OctToLittleEndian(string oct)
    {
        return OctToBigEndian(oct); // Same logic
    }
    private string SplitBinary(string bin)
    {
        if (bin.Length % 4 != 0)
        {
            bin = bin.PadLeft(bin.Length + (4 - bin.Length % 4), '0');
        }
        string[] splited = new string[bin.Length / 4];
        for (int i = 0; i < bin.Length / 4; i++)
        {
            splited[i] = bin.Substring(i * 4, 4);
        }
        return string.Join(" ", splited);
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
            return new ConvertResult(bin, SplitBinary(bin));
        }
        return new ConvertResult(bin, bin);
    }

    public ConvertResult OctFormat(string oct)
    {
        if (settings.InputEndian != settings.OutputEndian)
        {
            if (settings.InputEndian == SettingsHelper.LittleEndian && settings.OutputEndian == SettingsHelper.BigEndian)
            {
                oct = OctToBigEndian(oct);
            }
            else if (settings.InputEndian == SettingsHelper.BigEndian && settings.OutputEndian == SettingsHelper.LittleEndian)
            {
                oct = OctToLittleEndian(oct);
            }
        }

        return new ConvertResult(oct, oct);
    }

    public ConvertResult DecFormat(string dec)
    {
        return new ConvertResult(dec, dec);
    }

    public ConvertResult UniversalConvert(string input, BaseType fromBase, BaseType toBase)
    {
        try
        {
            string dec = settings.BitLength switch
            {
                8  => System.Convert.ToSByte(input, (int)fromBase).ToString(),
                16 => System.Convert.ToInt16(input, (int)fromBase).ToString(),
                32 => System.Convert.ToInt32(input, (int)fromBase).ToString(),
                64 => System.Convert.ToInt64(input, (int)fromBase).ToString(),
                _  => System.Convert.ToInt64(input, (int)fromBase).ToString()
            };

            string raw = settings.BitLength switch
            {
                8  => System.Convert.ToString(System.Convert.ToSByte(dec, 10), (int)toBase),
                16 => System.Convert.ToString(System.Convert.ToInt16(dec, 10), (int)toBase),
                32 => System.Convert.ToString(System.Convert.ToInt32(dec, 10), (int)toBase),
                64 => System.Convert.ToString(System.Convert.ToInt64(dec, 10), (int)toBase),
                _  => System.Convert.ToString(System.Convert.ToInt64(dec, 10), (int)toBase)
            };

            string formated = toBase switch
            {
                BaseType.Bin => BinFormat(raw).Formated,
                BaseType.Oct => OctFormat(raw).Formated,
                BaseType.Dec => DecFormat(raw).Formated,
                BaseType.Hex => HexFormat(raw, is_upper).Formated,
                _ => raw
            };

            return new ConvertResult(raw, formated);
        }
        catch (Exception e)
        when (e is FormatException || e is InvalidCastException || e is OverflowException || e is ArgumentNullException)
        {
            return new ConvertResult("Invalid input", "Invalid input");
        }
    }
}