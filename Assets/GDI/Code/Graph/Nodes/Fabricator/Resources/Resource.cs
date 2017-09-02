using System;
using System.Collections.Generic;
using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Nodes.Fabricator
{
    [Serializable]
    public class Resource
    {
        public static readonly Resource NULL_RESOURCE = new Resource();
        
        [Serializable]
        public class Ingredient
        {
            public InputSocket InputSocket { get; private set; }
            
            [SerializeField] public int RequiredAmount { get; set; }
            [SerializeField] public float Waste { get; set; }
            [SerializeField] public Resource Resource { get; set; }

            public Ingredient(InputSocket _inputSocket)
            {
                InputSocket = _inputSocket;
                RequiredAmount = 0;
                Waste = 0.0f;
                Resource = null;
            }
        }
        
        [SerializeField] public TimeSpan WorkTime { get; set; }
        [SerializeField] public string Name { get; set; }
        [SerializeField] public float Volume { get; set; }
        [SerializeField] public float Pollution { get; set; }
        [SerializeField] public float Energy { get; set; }
        [SerializeField] public Resource ByProduct { get; set; }
        [SerializeField] public float Weigth { get; set; }
        [SerializeField] public List<Ingredient> Ingredients { get; private set; }

        public Resource()
        {
            WorkTime = TimeSpan.Zero;
            Name = "";
            Volume = 0.0f;
            Pollution = 0.0f;
            Energy = 0.0f;
            ByProduct = NULL_RESOURCE;
            Weigth = 0.0f;
            Ingredients = new List<Ingredient>();
        }

        public void Add(Resource _resource)
        {
            WorkTime = WorkTime.Add(_resource.WorkTime);
            Pollution += _resource.Pollution;
            Energy += _resource.Energy;
            Weigth += _resource.Weigth;
            Ingredients.AddRange(_resource.Ingredients);
        }
    }
}