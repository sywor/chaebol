
using UnityEngine;

public abstract class Placeable : ScriptableObject
{
    public enum PlaceableType
    {
        UNKNOWN,
        ASSEMBLY_LINE_TIER_1,
        ASSEMBLY_LINE_TIER_2,
        ASSEMBLY_LINE_TIER_3,
        ASSEMBLY_LINE_TIER_4,
        ASSEMBLY_LINE_TIER_5
    }

    public PlaceableType Type = PlaceableType.UNKNOWN;
    public Vector3 Position = Vector3.zero;
    
    public abstract void Update();
}
