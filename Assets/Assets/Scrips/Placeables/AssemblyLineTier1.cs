
using System;
using UnityEngine;

public class AssemblyLineTier1 : Placeable<AssemblyLineTier1>
{
    public AssemblyLineTier1() : base(PlaceableType.ASSEMBLY_LINE_TIER_1) {}
    
    public override void Update()
    {
        
    }

    public override string ToString()
    {
        return "AseemblyLine";
    }
}
