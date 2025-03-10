﻿using System.Numerics;
using System.Text;

namespace Community.PowerToys.Run.Plugin.HexInspector;

public class ConvertResult(string raw, string formated)
{
    public string Raw { get; set; } = raw;
    public string Formated { get; set; } = formated;
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

    private string HexToLittleEndian(string hex)
    {
        return HexToBigEndian(hex); // Same logic
    }

    private string BinToLittleEndian(string bin)
    {
        return BinToBigEndian(bin); // Same logic
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
        // hex should be in little endian
        if (settings.OutputEndian == Endian.BigEndian)
        {
            hex = HexToBigEndian(hex);
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
        // bin should be in little endian
        if (settings.OutputEndian == Endian.BigEndian)
        {
            bin = BinToBigEndian(bin);
        }

        if (settings.SplitBinary)
        {
            return new ConvertResult(bin, SplitBinary(bin));
        }
        return new ConvertResult(bin, bin);
    }

    public ConvertResult OctFormat(string oct)
    {
        // No need to change octal format
        return new ConvertResult(oct, oct);
    }

    public ConvertResult DecFormat(string dec)
    {
        return new ConvertResult(dec, dec);
    }

    public ConvertResult AsciiFormat(string ascii)
    {
        // ascii should be in little endian
        if (settings.OutputEndian == Endian.BigEndian)
        {
            ascii = new string(ascii.Reverse().ToArray());
        }

        StringBuilder strb = new StringBuilder(ascii);
        for (int i = 0; i < strb.Length; i++)
        {
            if (char.IsControl(strb[i]))
            {
                strb[i] = ' ';
            }
        }
        ascii = strb.ToString();

        return new ConvertResult(ascii, ascii);
    }

    // Ascii to integer(Decimal)
    public string AsciiToInt(string ascii) => 
        System.Convert.ToInt64(
            System.BitConverter
                .ToString(System.Text.Encoding.ASCII.GetBytes(ascii) // ASCII to hex string, e.g. "AB" -> "42-41"
                .Reverse().ToArray()) // Convert to little endian
                .Replace("-", ""),
            (int)Base.Hex
        ).ToString();

    // integer(Decimal) to Ascii
    public string IntToAscii(string dec) => 
        new string (
            System.Text.Encoding.ASCII.GetString(
                System.BitConverter.GetBytes(System.Convert.ToInt64(dec, 10)))
                .TrimEnd('\0') // Remove null character
                .Reverse()
                .ToArray()
            );

    // Convert string to BigInteger(Decimal)
    public BigInteger BigIntegerConvert(string input, Base fromBase) => fromBase switch
    {
        Base.Bin => BigInteger.Parse("0"+input, System.Globalization.NumberStyles.BinaryNumber),
        Base.Oct => new Func<BigInteger>( // BigInterger.Parse() does not support octal, fxxk mixxxxxft
            () => {
                input = input.Replace(" ", ""); // Remove space
                BigInteger result = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    result += (input[i] - '0') * BigInteger.Pow(8, input.Length - i - 1);
                }
                return result;
            }
        )(),
        Base.Dec => BigInteger.Parse(input),
        Base.Hex => BigInteger.Parse("0"+input, System.Globalization.NumberStyles.HexNumber),
        Base.Ascii => new Func<BigInteger>(
            () => {
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(input).Reverse().ToArray();
                return new BigInteger(bytes);
            }
        )(),
        _ => throw new System.ArgumentException("Invalid base", nameof(fromBase))
    };

    // Convert BigInteger(Decimal) to string
    public string ConvertBigInteger(BigInteger input, Base toBase) => toBase switch
    {
        Base.Bin => input.ToString("B"),
        Base.Oct => new Func<string>(
            () => {
                if (input == 0)
                {
                    return "0";
                }

                string result = "";
                while (input > 0)
                {
                    result = (input % 8).ToString() + result;
                    input /= 8;
                }
                return result;
            }
        )(),
        Base.Dec => input.ToString(),
        Base.Hex => input.ToString("X"),
        Base.Ascii => new Func<string>(
            () => {
                byte[] bytes = input.ToByteArray().Reverse().ToArray();
                return System.Text.Encoding.ASCII.GetString(bytes);
            }
        )(),
        _ => throw new System.ArgumentException("Invalid base", nameof(toBase))
    };

    public ConvertResult UniversalConvert(string input, Base fromBase, Base toBase)
    {
        // Make sure the input is in the little endian before converting
        if (settings.InputEndian == Endian.BigEndian)
        {
            input = fromBase switch
            {
                Base.Bin => BinToLittleEndian(input),
                Base.Hex => HexToLittleEndian(input),
                Base.Ascii => new string(input.Reverse().ToArray()),
                _ => input
            };
        }

        try
        {
            string dec = settings.BitLength switch
            {
                BitLength.BYTE  when fromBase != Base.Ascii => System.Convert.ToSByte(input, (int)fromBase).ToString(),
                BitLength.WORD  when fromBase != Base.Ascii => System.Convert.ToInt16(input, (int)fromBase).ToString(),
                BitLength.DWORD when fromBase != Base.Ascii => System.Convert.ToInt32(input, (int)fromBase).ToString(),
                BitLength.QWORD when fromBase != Base.Ascii => System.Convert.ToInt64(input, (int)fromBase).ToString(),
                not BitLength.UNLIMITED when fromBase == Base.Ascii => AsciiToInt(input),
                BitLength.UNLIMITED => BigIntegerConvert(input, fromBase).ToString(),
                _  => throw new ArgumentException("Invalid base", nameof(fromBase))
            };

            string raw = settings.BitLength switch
            {
                BitLength.BYTE  when toBase != Base.Ascii => System.Convert.ToString(System.Convert.ToSByte(dec, 10), (int)toBase),
                BitLength.WORD  when toBase != Base.Ascii => System.Convert.ToString(System.Convert.ToInt16(dec, 10), (int)toBase),
                BitLength.DWORD when toBase != Base.Ascii => System.Convert.ToString(System.Convert.ToInt32(dec, 10), (int)toBase),
                BitLength.QWORD when toBase != Base.Ascii => System.Convert.ToString(System.Convert.ToInt64(dec, 10), (int)toBase),
                not BitLength.UNLIMITED when toBase == Base.Ascii => IntToAscii(dec),
                BitLength.UNLIMITED => ConvertBigInteger(BigInteger.Parse(dec), toBase),
                _  => throw new ArgumentException("Invalid base", nameof(toBase))
            };

            string formated = toBase switch
            {
                Base.Bin => BinFormat(raw).Formated,
                Base.Oct => OctFormat(raw).Formated,
                Base.Dec => DecFormat(raw).Formated,
                Base.Hex => HexFormat(raw, is_upper).Formated,
                Base.Ascii => AsciiFormat(raw).Formated,
                _ => raw
            };

            return new ConvertResult(raw, formated);
        }
        catch (Exception e)
        when (e is FormatException || e is InvalidCastException || e is OverflowException || e is ArgumentNullException)
        {
            return e switch 
            {
                FormatException         => new ConvertResult("Invalid format", "Invalid format"),
                InvalidCastException    => new ConvertResult("Invalid cast", "Invalid cast"),
                OverflowException       => new ConvertResult("Overflow", "Overflow"),
                ArgumentNullException   => new ConvertResult("Null argument", "Null argument"),
                _                       => new ConvertResult("Unknown error", "Unknown error")
            };
        }
    }
}