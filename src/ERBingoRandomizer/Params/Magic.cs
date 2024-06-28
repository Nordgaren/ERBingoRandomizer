using FSParam;


namespace ERBingoRandomizer.Params;

public class Magic
{
    private Param.Cell _ezStateBehaviorType;
    private Param.Cell _requirementFaith;
    private Param.Cell _requirementIntellect;
    private Param.Cell _requirementLuck;

    public Magic(Param.Row magic)
    {
        _ezStateBehaviorType = magic["ezStateBehaviorType"]!.Value;
        _requirementIntellect = magic["requirementIntellect"]!.Value;
        _requirementFaith = magic["requirementFaith"]!.Value;
        _requirementLuck = magic["requirementLuck"]!.Value;
    }
    public byte ezStateBehaviorType { get => (byte)_ezStateBehaviorType.Value; set => _ezStateBehaviorType.Value = value; }
    public byte requirementIntellect { get => (byte)_requirementIntellect.Value; set => _requirementIntellect.Value = value; }
    public byte requirementFaith { get => (byte)_requirementFaith.Value; set => _requirementFaith.Value = value; }
    public byte requirementLuck { get => (byte)_requirementLuck.Value; set => _requirementLuck.Value = value; }
}
