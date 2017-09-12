using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject AssemblyLineTire1;
    public Collider MapCollider;
    
    public delegate void PlaceObjecDownDelegate();
    public delegate void CancelObjectDownDelegate();
    
    private readonly Dictionary<Vector3, GameObject> placedObjects = new Dictionary<Vector3, GameObject>();
    private GameObject placingObject;
    private Vector3 placePos;

    public void PlaceObject(Placeable _placeable)
    {
        switch (_placeable.Type)
        {
            case Placeable.PlaceableType.ASSEMBLY_LINE_TIER_1:
                SetPlaceObject();
                break;
            case Placeable.PlaceableType.ASSEMBLY_LINE_TIER_2:
                break;
            case Placeable.PlaceableType.ASSEMBLY_LINE_TIER_3:
                break;
            case Placeable.PlaceableType.ASSEMBLY_LINE_TIER_4:
                break;
            case Placeable.PlaceableType.ASSEMBLY_LINE_TIER_5:
                break;
            default:
                placingObject = null;
                break;
        }
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (placingObject == null) return;
        placingObject.SetActive(true);
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!MapCollider.Raycast(ray, out hit, Mathf.Infinity)) return;
        
        placePos = hit.point + new Vector3(0.0f, placingObject.transform.localScale.y / 2, 0.0f);
        placingObject.transform.position = placePos;
    }

    private void PlaceObjectDown()
    {
        Debug.Log("PlaceDown");
        if (placingObject != null)
        {
            var placingObjectTransform = placingObject.transform;
            var expectedBounds = new Bounds(placePos, placingObjectTransform.localScale);

            foreach (var placedObject in placedObjects)
            {
                var componentCollider = placedObject.Value.GetComponent<BoxCollider>();
                if (componentCollider.bounds.Intersects(expectedBounds))
                {
                    return;
                }
            }
            
            placedObjects.Add(placePos, placingObject);
            SetPlaceObject();
        }
    }

    private void CancelObjectDown()
    {
        Debug.Log("Cancel placedown");
        Destroy(placingObject);
    }
    
    private void SetPlaceObject()
    {
        placingObject = Instantiate(AssemblyLineTire1, transform);
        placingObject.SetActive(false);
        placingObject.GetComponent<PlaceDownTrigger>().SetTriggers(PlaceObjectDown, CancelObjectDown);
    }
}