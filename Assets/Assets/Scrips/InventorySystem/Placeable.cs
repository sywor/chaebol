
using System;
using UnityEngine;

public enum PlaceableType
{
    ASSEMBLY_LINE_TIER_1,
    ASSEMBLY_LINE_TIER_2,
    ASSEMBLY_LINE_TIER_3,
    ASSEMBLY_LINE_TIER_4,
    ASSEMBLY_LINE_TIER_5,
    UNKNOWN
}

public abstract class Placeable<T> : ScriptableObject, IPlaceable where T : Placeable<T>
{
//    private PlaceableType type;
//    private Vector3 position;
    private GameObject inGameObject;
    private Guid id;

//    public PlaceableType Type
//    {
//        get { return type; }
//        private set { type = value; }
//    }

    public Vector3 Position
    {
        get { return InGameObject.transform.position; }
    }

    public GameObject InGameObject
    {
        get { return inGameObject; }
        private set { inGameObject = value; }
    }

    public Guid ID
    {
        get { return id; }
        private set { id = value; }
    }

//    public Placeable(PlaceableType _placeableType)
//    {
//        type = _placeableType;
//    }

    public static T Create(GameObject _inGameObject, Guid _id)
    {
        var t = CreateInstance<T>();

        t.InGameObject = _inGameObject;
        t.ID = _id;

        return t;
    }

    public static T Create(GameObject _inGameObject)
    {
        var t = CreateInstance<T>();

        t.InGameObject = _inGameObject;
        t.ID = Guid.Empty;

        return t;
    }

    public void Destroy()
    {
        Destroy(InGameObject);
        Destroy(this);
    }

    public IPlaceable Instantiate()
    {
        return Instantiate(this);
    }

    public abstract void Update();
}

public interface IPlaceable
{
//    PlaceableType Type { get; }
    Vector3 Position { get; }
    GameObject InGameObject{ get; }
    Guid ID { get; }
    void Destroy();
    IPlaceable Instantiate();
}
