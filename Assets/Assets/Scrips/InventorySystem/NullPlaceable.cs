using System;
using UnityEngine;

public class NullPlaceable : Placeable
{
    private static NullPlaceable instance;
    public static NullPlaceable Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<NullPlaceable>();
            }

            if (instance == null)
            {
                instance = CreateInstance<NullPlaceable>();
                instance.InGameObject = null;
                instance.Position = Vector3.zero;
                instance.ID = Guid.Empty;
            }

            return instance;
        }
    }
    
    public override PlaceableType Type
    {
        get { return PlaceableType.UNKNOWN; }
    }

    public override Vector3 Position
    {
        get; 
        protected set;
    }

    public override GameObject InGameObject
    {
        get; 
        protected set;
    }

    public override Guid ID
    {
        get; 
        protected set;
    }

    public override void Update()
    {
        //Do nothing
    }
}
