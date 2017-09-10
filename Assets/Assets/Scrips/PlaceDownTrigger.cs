using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceDownTrigger : MonoBehaviour
{
    private ObjectPlacer.PlaceObjecDownDelegate placeObjecDownDelegate;
    
    public void SetTrigger(ObjectPlacer.PlaceObjecDownDelegate _delegate)
    {
        placeObjecDownDelegate = _delegate;
    }

    private void OnMouseDown()
    {
        placeObjecDownDelegate();
    }
}
