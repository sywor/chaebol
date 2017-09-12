using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRegistry : ScriptableObject
{
    private static ObjectRegistry instance;
    public static ObjectRegistry Instance
    {
        get
        {
            if (instance == null)
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
    
    private readonly Dictionary<Guid, Placeable> factoryDictionary = new Dictionary<Guid, Placeable>();

    public Placeable GetPlaceable(Guid _guid)
    {
        return factoryDictionary[_guid];
    }

    public Guid AddPlaceable(Placeable _placeable)
    {
        var guid = Guid.NewGuid();
        factoryDictionary.Add(guid, _placeable);
        return guid;
    }

    public bool RemovePlaceable(Guid _guid)
    {
        return factoryDictionary.Remove(_guid);
    }

    /// <summary>
    /// Checks agains registerd objects if they collide with given bounds
    /// </summary>
    /// <param name="_expectedBounds">The bounds to check against</param>
    /// <returns>True if a collision is found</returns>
    public bool CheckCollision(Bounds _expectedBounds)
    {
        foreach (var placedObject in factoryDictionary.Values)
        {
            var componentCollider = placedObject.InGameObject.GetComponent<BoxCollider>();
            if (componentCollider.bounds.Intersects(_expectedBounds))
            {
                return true;
            }
        }
        
        return false;
    }

    public IEnumerable<Placeable> GetAllPlaceables()
    {
        return factoryDictionary.Values;
    }
}
