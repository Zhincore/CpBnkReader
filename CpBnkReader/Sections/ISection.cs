namespace CpBnkReader;

public interface ISection
{
    public void Read(BinaryReader br, uint length);
}
