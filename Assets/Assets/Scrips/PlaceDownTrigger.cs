using UnityEngine;

public class PlaceDownTrigger : MonoBehaviour
{
    private ObjectPlacer.PlaceObjecDownDelegate placeObjecDownDelegate;
    private ObjectPlacer.CancelObjectDownDelegate cancelObjectDownDelegate;

    public void SetTriggers(ObjectPlacer.PlaceObjecDownDelegate _place, ObjectPlacer.CancelObjectDownDelegate _cancel)
    {
        placeObjecDownDelegate = _place;
        cancelObjectDownDelegate = _cancel;
    }

    private void OnMouseOver()
    {
        var leftBtn = Input.GetMouseButtonDown(0);
        var rightBtn = Input.GetMouseButtonDown(1);

        if (leftBtn)
        {
            placeObjecDownDelegate();
        }

        if (rightBtn)
        {
            cancelObjectDownDelegate();
        }
    }
}
