using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceObject : MonoBehaviour
{
    public Collider MapCollider;
    private readonly Dictionary<Vector3, Component> placedComponents = new Dictionary<Vector3, Component>();

    public void SpawnObject(int buttonIndex)
    {
        
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (!Input.GetMouseButtonDown(0)) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (MapCollider.Raycast(ray, out hit, Mathf.Infinity))
        {
//            var assemblerScale = Assembler.transform.localScale;
//            var heigthOffset = assemblerScale.y / 2;
//            var placePos = hit.point + new Vector3(0.0f, heigthOffset, 0.0f);
//            var expectedBounds = new Bounds(placePos, assemblerScale);
//
//            foreach (var component in placedComponents)
//            {
//                var componentCollider = component.Value.GetComponent<BoxCollider>();
//                var componentColliderBounds = componentCollider.bounds;
//                
//                if (componentColliderBounds.Intersects(expectedBounds))
//                {
//                    return;
//                }
//            }
//
//            var newInstace = Instantiate(Assembler, placePos, Assembler.transform.rotation);
//            
//            placedComponents.Add(placePos, newInstace);
        }
    }
}