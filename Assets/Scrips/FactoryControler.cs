using System;
using Scrips;
using UnityEngine;

public class FactoryControler : MonoBehaviour
{
	private Guid id = Guid.NewGuid();
	public Guid ID { get { return id; } }
	public Factory Factory;

	// Use this for initialization
	void Start () 
	{
		ObjectRegistry.Instance.AddFactory(ID, Factory);
	}
	
	// Update is called once per frame
	void Update () 
	{
		Factory.Update();
	}
}
