using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Side
{
    RIGHT,
    LEFT,
    FORWARD,
    REAR
}

public class ObjectPlacer : MonoBehaviour
{
    public GameObject AssemblyLineTire1;
    public Collider MapCollider;
    
    public delegate void PlaceObjecDownDelegate();
    public delegate void CancelObjectDownDelegate();
    
    private IPlaceable placingObject;
    private Vector3 placePos;
    private HashSet<GameObject> collidedObjects = new HashSet<GameObject>();

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
        
        var inGameObject = placingObject.InGameObject;
        
        inGameObject.SetActive(true);
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!MapCollider.Raycast(ray, out hit, Mathf.Infinity)) return;
        
        var placingObjectTransform = inGameObject.transform;

        var yOffset = placingObjectTransform.localScale.y / 2;
        if (collidedObjects.Any())
        {
            var collidedObject = collidedObjects.First();

            var colliededTransformPos = collidedObject.transform.position;

            var collidedSide = GetCollidedSide(colliededTransformPos, inGameObject.transform.position);

            var xOffset = placingObjectTransform.localScale.x / 2 + collidedObject.transform.localScale.x / 2;
            var zOffset = placingObjectTransform.localScale.z / 2 + collidedObject.transform.localScale.z / 2;
            
            switch (collidedSide)
            {
                case Side.RIGHT:
                    placePos = new Vector3(
                        colliededTransformPos.x - xOffset,
                        yOffset,
                        colliededTransformPos.z);
                    break;
                case Side.LEFT:
                    placePos = new Vector3(
                        colliededTransformPos.x + xOffset,
                        yOffset,
                        colliededTransformPos.z);
                    break;
                case Side.FORWARD:
                    placePos = new Vector3(
                        colliededTransformPos.x,
                        yOffset,
                        colliededTransformPos.z - zOffset);
                    break;
                case Side.REAR:
                    placePos = new Vector3(
                        colliededTransformPos.x,
                        yOffset,
                        colliededTransformPos.z + zOffset);
                    break;
            }

            if ((hit.point - placingObjectTransform.position).magnitude > placingObjectTransform.localScale.magnitude)
            {
                collidedObjects.Clear();
            }
        }
        
        if(!collidedObjects.Any())
        {
            placePos = hit.point + new Vector3(0.0f, yOffset, 0.0f);
        }
        
        placingObjectTransform.position = placePos;
    }

    private Side GetCollidedSide(Vector3 _colliededTransformPos, Vector3 _placingTransformPos)
    {            
        var posDiff = _colliededTransformPos - _placingTransformPos;
        
        if (Mathf.Abs(posDiff.x) < Mathf.Abs(posDiff.z))
        {
            return posDiff.z > 0 ? Side.FORWARD : Side.REAR;
        }
        return posDiff.x > 0 ? Side.RIGHT : Side.LEFT;
    }

    public void SnapDetected(GameObject _first, GameObject _second)
    {
        if(placingObject == null) return;

        if (placingObject.InGameObject == _first)
        {
            collidedObjects.Add(_second);
        }
    }

    private void PlaceObjectDown()
    {
        if (placingObject != null)
        {
            foreach (var collidedObject in collidedObjects)
            {
                var collidedSide = GetCollidedSide(collidedObject.transform.position, placingObject.InGameObject.transform.position);
                placingObject.InGameObject.GetComponent<HardPointVisability>().HideSelectedHardPoint(collidedSide);
                
                switch (collidedSide)
                {
                    case Side.RIGHT:
                        collidedObject.GetComponent<HardPointVisability>().HideSelectedHardPoint(Side.LEFT);
                        break;
                    case Side.LEFT:
                        collidedObject.GetComponent<HardPointVisability>().HideSelectedHardPoint(Side.RIGHT);
                        break;
                    case Side.FORWARD:
                        collidedObject.GetComponent<HardPointVisability>().HideSelectedHardPoint(Side.REAR);
                        break;
                    case Side.REAR:
                        collidedObject.GetComponent<HardPointVisability>().HideSelectedHardPoint(Side.FORWARD);
                        break;
                }
            }
            
            collidedObjects.Clear();
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