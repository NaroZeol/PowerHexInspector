namespace test
{
    using PowerHexInspector;
    class Program
    {
        private static readonly List<string> hexTestCase = new List<string> {
            "0", "1", "ABCD", "12345678", "FFFFFFFF", "FFFFFFFFFFFFFFFF", "114514"
        };
        private static readonly List<string> decTestCase = new List<string> {
            "0", "1", "43981", "305419896", "4294967295", "-1", "1131796"
        };
        private static readonly List<string> binTestCase = new List<string> {
            "0", "0001", "1010101111001101", "00010010001101000101011001111000",
            "11111111111111111111111111111111",
            "1111111111111111111111111111111111111111111111111111111111111111",
            "000100010100010100010100"
        };
        private static bool TestDecConversion()
        {
            foreach (var (hex, dec, bin) in hexTestCase.Zip(decTestCase, binTestCase))
            {
                if (Convert.Hex2Dec(hex) != dec)
                {
                    Console.WriteLine(@$"TestDec2Dec {hex}:
                    Expected: {dec}, Actual: {Convert.Hex2Dec(hex)}");
                    return false;
                }
                if (Convert.Hex2Bin(hex) != bin)
                {
                    Console.WriteLine(@$"TestDec2Bin {hex}:
                    Expected: {bin}, Actual: {Convert.Hex2Bin(hex)}");
                    return false;
                }
            }
            return true;
        }

        private static bool TestBinConversion()
        {
            foreach (var (hex, dec, bin) in hexTestCase.Zip(decTestCase, binTestCase))
            {
                if (Convert.Bin2Dec(bin) != dec)
                {
                    Console.WriteLine(@$"TestBin2Dec {bin}:
                    Expected: {dec}, Actual: {Convert.Bin2Dec(bin)}");
                    return false;
                }
                if (Convert.Bin2Hex(bin) != hex)
                {
                    Console.WriteLine(@$"TestBin2Hex {bin}:
                    Expected: {hex}, Actual: {Convert.Bin2Hex(bin)}");
                    return false;
                }
            }
            return true;
        }

        private static bool TestHexConversion()
        {
            foreach (var (hex, dec, bin) in hexTestCase.Zip(decTestCase, binTestCase))
            {
                if (Convert.Dec2Hex(dec) != hex)
                {
                    Console.WriteLine(@$"TestHex2Hex {dec}:
                    Expected: {hex}, Actual: {Convert.Dec2Hex(dec)}");
                    return false;
                }
                if (Convert.Dec2Bin(dec) != bin)
                {
                    Console.WriteLine(@$"TestHex2Bin {dec}:
                    Expected: {bin}, Actual: {Convert.Dec2Bin(dec)}");
                    return false;
                }
            }
            return true;
        }

        // static void Main(string[] args)
        // {
        //     if (!TestDecConversion() || !TestBinConversion() || !TestHexConversion())
        //     {
        //         Console.WriteLine("Test failed");
        //     }
        //     else
        //     {
        //         Console.WriteLine("Test passed");
        //     }
        // }
    }
}