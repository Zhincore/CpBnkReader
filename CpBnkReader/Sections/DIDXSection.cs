namespace CpBnkReader;

public class DIDXSection : ISection
{
    public readonly string Type = "DIDX";
    private readonly BnkFile _parent;

    public DIDXSection(BnkFile parent)
    {
        _parent = parent;
    }

    public List<WemInfo> WemInfos { get; } = new();

    public void Read(BinaryReader br, uint length)
    {
        var objCount = length / 12;
        for (int i = 0; i < objCount; i++)
        {
            var id = br.ReadUInt32();
            var offset = br.ReadUInt32();
            var objLength = br.ReadUInt32();

            WemInfos.Add(new WemInfo(id, offset, objLength));
        }
    }
}

public class WemInfo
{
    public WemInfo(uint id, uint offset, uint length)
    {
        Id = id;
        Offset = offset;
        Length = length;
    }

    public uint Id { get; }
    public uint Offset { get; }
    public uint Length { get; }
}
