using System;
using System.Collections.Generic;
using Scrips;
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
    
    private readonly Dictionary<Guid, Factory> factoryDictionary = new Dictionary<Guid, Factory>();

    public Factory GetFactory(Guid _guid)
    {
        return factoryDictionary[_guid];
    }

    public void AddFactory(Guid _guid, Factory _factory)
    {
        factoryDictionary.Add(_guid, _factory);
    }

    public bool RemoveFactory(Guid _guid)
    {
        return factoryDictionary.Remove(_guid);
    }
}
