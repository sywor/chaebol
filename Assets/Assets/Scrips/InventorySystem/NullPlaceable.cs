using System;
using UnityEngine;

public class NullPlaceable : Placeable<NullPlaceable>
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
                instance = Create(null, Vector3.zero, Guid.Empty);
            }

            return instance;
        }
    }

    public NullPlaceable() : base(PlaceableType.UNKNOWN) {}
    
    public override void Update()
    {
        //Do nothing
    }
}
