
using System;
using UnityEngine;

public abstract class Placeable : ScriptableObject
{
    public enum PlaceableType
    {
        ASSEMBLY_LINE_TIER_1,
        ASSEMBLY_LINE_TIER_2,
        ASSEMBLY_LINE_TIER_3,
        ASSEMBLY_LINE_TIER_4,
        ASSEMBLY_LINE_TIER_5,
        UNKNOWN
    }

    public abstract PlaceableType Type
    {
        get;
    }

    public abstract  Vector3 Position
    {
        get;
        protected set;
    }

    public abstract  GameObject InGameObject
    {
        get;
        protected set;
    }

    public abstract Guid ID
    {
        get; 
        protected set;
    }

    public abstract void Update();
}
