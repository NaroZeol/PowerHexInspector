namespace Community.PowerToys.Run.Plugin.HexInspector
{
    public enum Base
    {
        Invalid = -1,
        Bin = 2,
        Oct = 8,
        Dec = 10,
        Hex = 16,
        Ascii = 256,
    }

    public enum Endian
    {
        LittleEndian = 0,
        BigEndian = 1,
    }

    public enum BitLength
    {
        BYTE = 8,
        WORD = 16,
        DWORD = 32,
        QWORD = 64,
        UNLIMITED = -1,
    }
}