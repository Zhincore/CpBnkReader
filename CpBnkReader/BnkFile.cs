namespace CpBnkReader;

public class BnkFile
{
    public List<ISection> Sections { get; } = new();

    public T? GetSection<T>()
    {
        foreach (var section in Sections)
        {
            if (section.GetType() == typeof(T))
            {
                return (T)section;
            }
        }

        return default;
    }

    public void Read(BinaryReader br)
    {
        while (br.BaseStream.Position < br.BaseStream.Length)
        {
            ReadSection(br);
        }
    }

    private void ReadSection(BinaryReader br)
    {
        var magic = br.ReadMagic();
        var length = br.ReadUInt32();

        var startPos = br.BaseStream.Position;

        ISection? section = null;
        switch (magic)
        {
            case "BKHD":
                break;

            case "DIDX":
                section = new DIDXSection(this);
                break;

            case "DATA":
                section = new DATASection(this);
                break;

            case "HIRC":
                section = new HIRCSection(this);
                break;

            case "STID":
                break;
        }

        if (section != null)
        {
            section.Read(br, length);

            Sections.Add(section);
        }

        br.BaseStream.Position = startPos + length;
    }
}
