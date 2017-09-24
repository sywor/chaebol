using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRegistry : ScriptableObject
{
    private static ObjectRegistry instance;

    public static ObjectRegistry Instance
    {
        get
        {
            if (instance != null)
            {
                instance = FindObjectOfType<ObjectRegistry>();
            }

            if (instance == null)
            {
                instance = CreateInstance<ObjectRegistry>();
            }

            return instance;
        }
    }

    private readonly Dictionary<Guid, IPlaceable> placableDictionary = new Dictionary<Guid, IPlaceable>();

    public IPlaceable GetPlaceable(Guid _guid)
    {
        return placableDictionary[_guid];
    }

    public Guid AddPlaceable(IPlaceable _placeable)
    {
        var guid = Guid.NewGuid();
        placableDictionary.Add(guid, _placeable);
        return guid;
    }

    public bool RemovePlaceable(Guid _guid)
    {
        return placableDictionary.Remove(_guid);
    }

    /// <summary>
    /// Checks agains registerd objects if they collide with given bounds
    /// </summary>
    /// <param name="_expectedBounds">The bounds to check against</param>
    /// <param name="_collidingObject">If collission is detected this will be the collided object</param>
    /// <returns>True if a collision is found</returns>
    public bool CheckCollision(Bounds _expectedBounds, out GameObject _collidingObject)
    {
        foreach (var placedObject in placableDictionary.Values)
        {
            var componentCollider = placedObject.InGameObject.GetComponent<BoxCollider>();
            if (componentCollider.bounds.Intersects(_expectedBounds))
            {
                _collidingObject = componentCollider.gameObject;
                return true;
            }
        }

        _collidingObject = null;
        return false;
    }

    public IEnumerable<IPlaceable> GetAllPlaceables()
    {
        return placableDictionary.Values;
    }
}
