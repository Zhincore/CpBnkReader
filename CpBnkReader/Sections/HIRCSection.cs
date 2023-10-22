using System;

namespace CpBnkReader;

public class HIRCSection : ISection
{
    public readonly string Type = "HIRC";
    private readonly BnkFile _parent;

    public HIRCSection(BnkFile parent)
    {
        _parent = parent;
    }

    public List<BaseHIRCObject> Entries { get; } = new();

    public List<T> FindEntry<T>(T obj) where T : BaseHIRCObject
    {
        var result = new List<T>();

        foreach (var entry in Entries)
        {
            if (entry.GetType() == typeof(T) && entry.Id == obj.Id)
            {
                result.Add((T) entry);
            }
        }

        return result;
    }

    public void Read(BinaryReader br, uint length)
    {
        var objCount = br.ReadUInt32();
        for (int i = 0; i < objCount; i++)
        {
            var type = br.ReadByte();
            var objLength = br.ReadUInt32();

            var startPos = br.BaseStream.Position;

            var id = br.ReadUInt32();

            BaseHIRCObject? obj = null;
            switch (type)
            {
                case 1: // State
                    obj = new StateObject(id);
                    break;

                case 2: // Sound
                    obj = new SoundObject(id);
                    break;

                case 3: // Action
                    obj = new ActionObject(id);
                    break;

                case 4: // Event
                    obj = new EventObject(id);
                    break;

                case 5: // RanSeqCntr
                    obj = new RanSeqCntrObject(id);
                    break;
                
                case 6: // SwitchCntr 
                    obj = new SwitchCntrObject(id);
                    break;

                case 7: // ActorMixer 
                    break;

                case 8: // AudioBus 
                    break;

                case 9: // BlendContainer  
                    break;

                case 10: // MusicSegment
                    obj = new MusicSegmentObject(id);
                    break;

                case 11: // MusicTrack
                    obj = new MusicTrackObject(id);
                    break;

                case 12: // MusicSwitchCntr
                    obj = new MusicSwitchCntrObject(id);
                    break;

                case 13: // MusicRanSeqCntr
                    obj = new MusicRanSeqCntrObject(id);
                    break;

                case 14: // Attenuation 
                    break;

                case 15: // DialogueEvent  
                    break;

                case 16: // MotionBus  
                    break;

                case 17: // MotionFx  
                    break;

                case 18: // Effect 
                    break;
                
                case 19: // Unknown
                    break;

                case 20: // AuxiliaryBus 
                    break;
            }

            if (obj != null)
            {
                obj.Read(br);

                Entries.Add(obj);
            }

            br.BaseStream.Position = startPos + objLength;
        }
    }
}

public abstract class BaseHIRCObject
{
    protected BaseHIRCObject(uint id, string entryType)
    {
        Id = id;
        EntryType = entryType;
    }

    public uint Id { get; }
    public string EntryType { get; }

    public abstract void Read(BinaryReader br);
}

public class StateObject : BaseHIRCObject
{
    public StateObject(uint id) : base(id, "State") {}

    public List<StateProp> Props = new();

    public override void Read(BinaryReader br)
    {
        var numProps = br.ReadUInt16();
        for (int i = 0; i < numProps; i++) {
            var prop = new StateProp();
            prop.Id = br.ReadUInt16();
            Props.Add(prop);
        }
        for (int i = 0; i < numProps; i++) {
            Props[i].Value = br.ReadSingle();
        }
    }
}

public class StateProp {
    public StateProp() {}

    public uint Id { get;  set; }
    public float Value { get; set; }
}

public class SoundObject : BaseHIRCObject
{
    public SoundObject(uint id) : base(id, "Sound") {}

    public uint AudioId { get; set; }
    public uint SourceId { get; set; }
    public override void Read(BinaryReader br)
    {
        br.BaseStream.Position += 1;

        AudioId = br.ReadUInt32();
        SourceId = br.ReadUInt32();
    }
}

public class ActionObject : BaseHIRCObject
{
    public ActionObject(uint id) : base(id, "Action") { }

    public byte Type { get; set; }
    public uint GameObjectReferenceId { get; set; }
    public override void Read(BinaryReader br)
    {
        br.BaseStream.Position += 1;

        Type = br.ReadByte();
        GameObjectReferenceId = br.ReadUInt32();
    }
}

public class EventObject : BaseHIRCObject
{
    public EventObject(uint id) : base(id, "Event") { }

    public List<uint> Events { get; } = new();

    public override void Read(BinaryReader br)
    {
        var objCount = br.ReadByte();
        for (int i = 0; i < objCount; i++)
        {
            Events.Add(br.ReadUInt32());
        }
    }
}

public class RanSeqCntrObject : BaseHIRCObject
{
    public RanSeqCntrObject(uint id) : base(id, "RanSeqCntr") { }

    public List<uint> Children { get; } = new();
    public override void Read(BinaryReader br)
    {
        HIRCHelper.ReadNodeBaseParams(br);

        br.BaseStream.Position += 24;

        var propsCount = br.ReadUInt32();
        for (int i = 0; i < propsCount; i++)
        {
            Children.Add(br.ReadUInt32());
        }
    }
}

public class SwitchCntrObject : BaseHIRCObject
{
    public SwitchCntrObject(uint id) : base(id, "SwitchCntr") { }

    public byte GroupType { get; set; }
    public uint GroupID { get; set; }
    public uint DefaultSwitch { get; set; }
    public bool IsContinuousValidation { get; set; }
    public List<uint> Groups { get; } = new();
    public override void Read(BinaryReader br)
    {
        HIRCHelper.ReadNodeBaseParams(br);

        GroupType = br.ReadByte();
        GroupID = br.ReadUInt32();
        DefaultSwitch = br.ReadUInt32();
        IsContinuousValidation = br.ReadBoolean();

        var groupCount = br.ReadUInt32();

        for (int i = 0; i < groupCount; i++)
        {
            Groups.Add(br.ReadUInt32());
        }
    }
}


public class MusicSegmentObject : BaseHIRCObject
{
    public MusicSegmentObject(uint id) : base(id, "MusicSegment") { }
    public List<uint> Children { get; } = new();
    public override void Read(BinaryReader br)
    {
        br.BaseStream.Position += 1;

        HIRCHelper.ReadNodeBaseParams(br);

        var propsCount = br.ReadUInt32();
        for (int i = 0; i < propsCount; i++)
        {
            Children.Add(br.ReadUInt32());
        }
    }
}

public class MusicTrackObject : BaseHIRCObject
{
    public MusicTrackObject(uint id) : base(id, "MusicTrack") { }
    public List<uint> Sources { get; } = new();
    public override void Read(BinaryReader br)
    {
        br.BaseStream.Position += 1;

        var numSources = br.ReadUInt32();
        for (int i = 0; i < numSources; i++)
        {
            br.BaseStream.Position += 5;
            Sources.Add(br.ReadUInt32());
        }
    }
}

public class MusicSwitchCntrObject : BaseHIRCObject
{
    public MusicSwitchCntrObject(uint id) : base(id, "MusicSwitchCntr") { }
    public List<uint> Children { get; } = new();
    public override void Read(BinaryReader br)
    {
        br.BaseStream.Position += 1;

        HIRCHelper.ReadNodeBaseParams(br);

        var propsCount = br.ReadUInt32();
        for (int i = 0; i < propsCount; i++)
        {
            Children.Add(br.ReadUInt32());
        }
    }
}

public class MusicRanSeqCntrObject : BaseHIRCObject
{
    public MusicRanSeqCntrObject(uint id) : base(id, "MusicRanSeqCntr") { }
    public List<uint> Children { get; } = new();
    public override void Read(BinaryReader br)
    {
        br.BaseStream.Position += 1;

        HIRCHelper.ReadNodeBaseParams(br);

        var propsCount = br.ReadUInt32();
        for (int i = 0; i < propsCount; i++)
        {
            Children.Add(br.ReadUInt32());
        }
    }
}
