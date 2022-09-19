namespace CpBnkReader;

public class HIRCHelper
{
    public static void ReadNodeBaseParams(BinaryReader br)
    {
        int cnt;

        #region NodeInitialFxParams

        br.ReadByte();
        var numFx = br.ReadByte();
        if (numFx > 0)
        {
            br.BaseStream.Position += (numFx * 7) + 1;
        }

        #endregion NodeInitialFxParams

        br.BaseStream.Position += 12;

        cnt = br.ReadByte();
        br.BaseStream.Position += (cnt * 5);

        cnt = br.ReadByte();
        br.BaseStream.Position += (cnt * 9);

        var bitsPositioning = br.ReadByte();
        if ((bitsPositioning & 2) == 2)
        {
            br.BaseStream.Position += 1;
        }

        if ((bitsPositioning & 32) == 32 || (bitsPositioning & 64) == 64)
        {
            br.BaseStream.Position += 5;

            var numVertices = br.ReadUInt32();
            br.BaseStream.Position += (numVertices * 16);

            var numPlayListItem = br.ReadUInt32();
            br.BaseStream.Position += (numPlayListItem * 20);
        }

        var auxParams = br.ReadByte();
        if ((auxParams & 8) == 8)
        {
            br.BaseStream.Position += 16;
        }

        br.BaseStream.Position += 10;

        cnt = br.ReadByte();
        br.BaseStream.Position += (cnt * 3);

        cnt = br.ReadByte();
        for (int i = 0; i < cnt; i++)
        {
            br.BaseStream.Position += 5;

            var numStates = br.ReadByte();
            br.BaseStream.Position += (numStates * 8);
        }

        cnt = br.ReadUInt16();
        for (int i = 0; i < cnt; i++)
        {
            br.BaseStream.Position += 12;

            var size = br.ReadUInt16();
            br.BaseStream.Position += (size * 12);
        }
    }
}