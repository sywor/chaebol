
using System;
using UnityEngine;

public class AssemblyLineTier1 : Placeable
{
    private Vector3 position;
    private GameObject inGameObject;
    private Guid id;
    
    public override PlaceableType Type
    {
        get { return PlaceableType.ASSEMBLY_LINE_TIER_1; } 
    }

    public override Vector3 Position
    {
        get { return position; }
        protected set { position = value; }
    }

    public override GameObject InGameObject
    {
        get { return inGameObject; }
        protected set { inGameObject = value; }
    }

    public override Guid ID
    {
        get { return id; }
        protected set { id = value; }
    }

    public static Placeable Create(GameObject _inGameObject, Vector3 _position, Guid _id)
    {
        var assemblyLine = CreateInstance<AssemblyLineTier1>();
        
        assemblyLine.inGameObject = _inGameObject;
        assemblyLine.position = _position;
        assemblyLine.id = _id;
        
        return assemblyLine;
    }

    public override void Update()
    {
        
    }

    public override string ToString()
    {
        return "AseemblyLine";
    }
}
