using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Utils
{
    public static string PrettyPrint(this Vector3 _vector3)
    {
        return "[ " + _vector3.x + ", " + _vector3.y + ", " + _vector3.z + " ]";
    }

    public static string PrettyPrint(this Vector2 _vector2)
    {
        return "[ " + _vector2.x + ", " + _vector2.y + " ]";
    }

    public static string PrettyPrint(this Bounds _bounds)
    {
        return "[ Center: " + PrettyPrint(_bounds.center) +
               " Size: " + PrettyPrint(_bounds.size) +
               " Max: " + PrettyPrint(_bounds.max) +
               " Min: " + PrettyPrint(_bounds.min) + " ]";
    }

    public static string PrettyPrint<T>(this List<T> _list)
    {
        if (_list.Count == 0)
            return "[ ]";

        var sb = new StringBuilder();
        sb.Append("[ ");

        for(var i = 0; i < _list.Count - 1; i++)
        {
            sb.Append(_list[i]).Append(", ");
        }

        sb.Append(_list.Last()).Append(" ]");
        return sb.ToString();
    }
}
