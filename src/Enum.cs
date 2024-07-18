namespace PowerHexInspector
{
    public enum Base
    {
        Bin = 2,
        Oct = 8,
        Dec = 10,
        Hex = 16,
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
    }
}