namespace PowerHexInspector;

public class Convert
{
    public static string Dec2Hex(string dec)
    {
        return System.Convert.ToString(System.Convert.ToInt64(dec, 10), 16).ToUpper();
    }

    public static string Dec2Bin(string dec)
    {
        string raw = System.Convert.ToString(System.Convert.ToInt64(dec, 10), 2);
        if (raw.Length % 4 != 0 && raw != "0")
        {
            raw = raw.PadLeft(raw.Length + (4 - raw.Length % 4), '0');
        }
        return raw;
    }

    public static string Hex2Dec(string hex)
    {
        return System.Convert.ToInt64(hex, 16).ToString();
    }

    public static string Hex2Bin(string hex)
    {
        string raw = System.Convert.ToString(System.Convert.ToInt64(hex, 16), 2);
        if (raw.Length % 4 != 0 && raw != "0")
        {
            raw = raw.PadLeft(raw.Length + (4 - raw.Length % 4), '0');
        }
        return raw;
    }

    public static string Bin2Dec(string bin)
    {
        return System.Convert.ToInt64(bin, 2).ToString();
    }

    public static string Bin2Hex(string bin)
    {
        return System.Convert.ToString(System.Convert.ToInt64(bin, 2), 16).ToUpper();
    }
}
