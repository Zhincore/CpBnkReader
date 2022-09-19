namespace CpBnkReader;

public class DATASection : ISection
{
    private readonly BnkFile _parent;

    public DATASection(BnkFile parent)
    {
        _parent = parent;
    }

    public List<WemData> Wems { get; } = new();

    public void Read(BinaryReader br, uint length)
    {
        var didx = _parent.GetSection<DIDXSection>();
        if (didx == null)
        {
            throw new Exception();
        }

        var baseOffset = br.BaseStream.Position;
        foreach (var wemInfo in didx.WemInfos)
        {
            br.BaseStream.Position = baseOffset + wemInfo.Offset;

            Wems.Add(new WemData(br.ReadBytes((int)wemInfo.Length)));
        }
    }
}

public class WemData
{
    public WemData(byte[] data)
    {
        Data = data;
    }

    public byte[] Data { get; }
}