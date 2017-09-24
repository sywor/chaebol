using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnapPointController : MonoBehaviour
{
    private readonly Dictionary<SnapPoint, GameObject> snapPoints = new Dictionary<SnapPoint, GameObject>();

    public Dictionary<SnapPoint, GameObject> SnapPoints
    {
        get { return snapPoints; }
    }

    public void ShowSnapPoint(SnapPointController _other)
    {
        var connections = GetMatchingSnapPoints(_other);

        foreach (var connection in connections)
        {
            snapPoints[connection].SetActive(true);
        }
    }

    public void HideSnapPoint(SnapPointController _other)
    {
        var connections = GetMatchingSnapPoints(_other);

        foreach (var connection in connections)
        {
            snapPoints[connection].SetActive(false);
        }
    }

    private List<SnapPoint> GetMatchingSnapPoints(SnapPointController _other)
    {
        var allCombinations =
            snapPoints.Keys.SelectMany(_s => _other.SnapPoints.Keys.Select(_t => new Tuple<SnapPoint, SnapPoint>(_s, _t)));
        var connections = allCombinations.Where(_s => _s.Item2.ConnectsTo.Contains(name + ":" + _s.Item1.Name))
                                         .Select(_s => _s.Item1).ToList();
        return connections;
    }

    public void ShowAllSnapPoints()
    {
        foreach (var child in snapPoints.Values)
        {
            child.SetActive(true);
        }
    }

    public void HideAllSnapPoints()
    {
        foreach (var child in snapPoints.Values)
        {
            child.SetActive(false);
        }
    }

    public void AddSnapPoint(SnapPoint _snapPoint)
    {
        var snapPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        snapPoint.name = _snapPoint.Name;
        snapPoint.transform.position = new Vector3(_snapPoint.Position.x, 0.0f, _snapPoint.Position.y);
        snapPoint.transform.rotation = Quaternion.Euler(new Vector3(0.0f, _snapPoint.Rotation, 0.0f));

        snapPoints.Add(_snapPoint, snapPoint);
        snapPoint.transform.parent = transform;
    }
}
