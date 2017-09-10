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
}
