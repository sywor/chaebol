using System;
using UnityEngine;

public abstract class Placeable<T> : ScriptableObject, IPlaceable where T : Placeable<T>
{
    private GameObject inGameObject;
    private Guid id;

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

    public static T Create(string _name, GameObject _inGameObject)
    {
        var t = CreateInstance<T>();
        t.name = _name;
        t.InGameObject = _inGameObject;
        return t;
    }

    public void Destroy()
    {
        Destroy(InGameObject);
        Destroy(this);
    }

    public IPlaceable Copy(Transform _transform,
                           ObjectPlacer.PlaceObjecDownDelegate _placeObjectDown,
                           ObjectPlacer.CancelObjectDownDelegate _cancelObjectDown)
    {
        var guid = Guid.NewGuid();
        var gameObject = Instantiate(InGameObject, _transform);
        gameObject.SetActive(false);
        gameObject.name = name + " : " + guid;
        gameObject.GetComponent<PlaceDownTrigger>().SetTriggers(_placeObjectDown, _cancelObjectDown);

        return Create(name, gameObject);
    }

    public abstract void Update();
}

public interface IPlaceable
{
    Vector3 Position { get; }
    GameObject InGameObject { get; }
    Guid ID { get; }
    void Destroy();

    IPlaceable Copy(Transform _transform,
                    ObjectPlacer.PlaceObjecDownDelegate _placeObjectDown,
                    ObjectPlacer.CancelObjectDownDelegate _cancelObjectDown);
}
