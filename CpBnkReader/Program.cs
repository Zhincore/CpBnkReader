namespace CpBnkReader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var fs = File.Open(@"C:\Dev\sfx_container.bnk", FileMode.Open);
            using var br = new BinaryReader(fs);

            var file = new BnkFile();
            file.Read(br);

            var tmp = FindSfxSourceIds(file, 4091981167);
        }

        static List<uint> FindSfxSourceIds(BnkFile file, uint wwiseId)
        {
            var hirc = file.GetSection<HIRCSection>();
            if (hirc == null)
            {
                throw new Exception();
            }

            var result = new List<uint>();

            
            foreach (var evt in hirc.FindEntry(new EventObject(wwiseId)))
            {
                foreach (var eventEntry in evt.Events)
                {
                    foreach (var act in hirc.FindEntry(new ActionObject(eventEntry)))
                    {
                        if (act.Type != 1 && act.Type != 4)
                        {
                            continue;
                        }

                        if (act.GameObjectReferenceId == 0)
                        {
                            continue;
                        }

                        foreach (var rsc in hirc.FindEntry(new RanSeqCntrObject(act.GameObjectReferenceId)))
                        {
                            foreach (var child1 in rsc.Children)
                            {
                                foreach (var s in hirc.FindEntry(new SoundObject(child1)))
                                {
                                    result.Add(s.SourceId);
                                }
                            }
                        }
                    }
                }
            }

            result.Sort();

            return result;
        }
    }
}