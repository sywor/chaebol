using System;
using UnityEngine;

public class Utils
{
    public static String ToString(Vector3 _vector3)
    {
        return "[ " + _vector3.x + ", " + _vector3.y + ", " + _vector3.z + " ]";
    }

    public static String ToString(Vector2 _vector2)
    {
        return "[ " + _vector2.x + ", " + _vector2.y + " ]";
    }

    public static String ToString(Bounds _bounds)
    {
        return "[ Center: " + ToString(_bounds.center) +
               " Size: " + ToString(_bounds.size) +
               " Max: " + ToString(_bounds.max) +
               " Min: " + ToString(_bounds.min) + " ]";
    }
}