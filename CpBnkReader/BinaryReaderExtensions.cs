namespace CpBnkReader;

public static class BinaryReaderExtensions
{
    public static string ReadMagic(this BinaryReader reader)
    {
        return System.Text.Encoding.UTF8.GetString(reader.ReadBytes(4));
    }
}