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
    
    private IPlaceable placingObject;
    private Vector3 placePos;

    public void PlaceObject(IPlaceable _placeable)
    {
        foreach (var placeable in ObjectRegistry.Instance.GetAllPlaceables())
        {
            placeable.InGameObject.GetComponent<HardPointVisability>().ShowHardPoints();
        }
        
        switch (_placeable.Type)
        {
            case PlaceableType.ASSEMBLY_LINE_TIER_1:
                InstantiateAssemblyLineT1();
                break;
            case PlaceableType.ASSEMBLY_LINE_TIER_2:
                break;
            case PlaceableType.ASSEMBLY_LINE_TIER_3:
                break;
            case PlaceableType.ASSEMBLY_LINE_TIER_4:
                break;
            case PlaceableType.ASSEMBLY_LINE_TIER_5:
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
        
        var placingObjectInGameObject = placingObject.InGameObject;
        
        placingObjectInGameObject.SetActive(true);
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!MapCollider.Raycast(ray, out hit, Mathf.Infinity)) return;
        
        placePos = hit.point + new Vector3(0.0f, placingObjectInGameObject.transform.localScale.y / 2, 0.0f);
        placingObjectInGameObject.transform.position = placePos;
    }

    private void PlaceObjectDown()
    {
        Debug.Log("PlaceDown");
        if (placingObject != null)
        {
            var placingObjectTransform = placingObject.InGameObject.transform;
            var expectedBounds = new Bounds(placePos, placingObjectTransform.localScale);

            if (ObjectRegistry.Instance.CheckCollision(expectedBounds))
            {
                return;
            }

            ObjectRegistry.Instance.AddPlaceable(placingObject);
            InstantiateAssemblyLineT1();
        }
    }

    private void CancelObjectDown()
    {
        foreach (var placeable in ObjectRegistry.Instance.GetAllPlaceables())
        {
            placeable.InGameObject.GetComponent<HardPointVisability>().HideHardPoints();
        }
        
        placingObject.Destroy();
        placingObject = null;
    }
    
    private void InstantiateAssemblyLineT1()
    {
        var newGuid = Guid.NewGuid();
        var inGameObject = Instantiate(AssemblyLineTire1, transform);
        inGameObject.SetActive(false);
        inGameObject.name = "ALT1:" + newGuid;
        inGameObject.GetComponent<PlaceDownTrigger>().SetTriggers(PlaceObjectDown, CancelObjectDown);

        placingObject = Placeable<AssemblyLineTier1>.Create(inGameObject, Vector3.zero, newGuid);
    }
}