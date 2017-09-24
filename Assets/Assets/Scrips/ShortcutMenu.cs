using UnityEngine;
using UnityEngine.UI;

public class ShortcutMenu : MonoBehaviour
{
    public Image[] QuickSlotBtnImages = new Image[20];
    public Image DragTarget;
    public ObjectPlacer ObjectPlacer;

    private int fromBtn = -1;
    private int toBtn = -1;
    private bool dropedOnBtn;
    private readonly PlaceableStack[] placeableStacks = new PlaceableStack[20];

    private void Start()
    {
        QuickSlotBtnImages[0].sprite = Resources.Load<Sprite>("assemblyline_icon");
        QuickSlotBtnImages[0].enabled = true;
        QuickSlotBtnImages[1].sprite = Resources.Load<Sprite>("assemblyline_icon");
        QuickSlotBtnImages[1].enabled = true;
        QuickSlotBtnImages[2].sprite = Resources.Load<Sprite>("assemblyline_icon");
        QuickSlotBtnImages[2].enabled = true;

        foreach (var image in QuickSlotBtnImages)
        {
            image.type = Image.Type.Simple;
        }

        for (var i = 3; i < 20; i++)
        {
            placeableStacks[i] = NullPlaceableStack.Instance;
        }

        placeableStacks[0] = PlaceableStack.Create(TypeRegistry.Instance.GetPlacableType("AssemblyLineT1Start"));
        placeableStacks[0].AddPlaceable(TypeRegistry.Instance.GetPlacableType("AssemblyLineT1Start"));

        placeableStacks[1] = PlaceableStack.Create(TypeRegistry.Instance.GetPlacableType("AssemblyLineT1Middle"));
        placeableStacks[1].AddPlaceable(TypeRegistry.Instance.GetPlacableType("AssemblyLineT1Middle"));
        placeableStacks[1].AddPlaceable(TypeRegistry.Instance.GetPlacableType("AssemblyLineT1Middle"));

        placeableStacks[2] = PlaceableStack.Create(TypeRegistry.Instance.GetPlacableType("AssemblyLineT1End"));
        placeableStacks[2].AddPlaceable(TypeRegistry.Instance.GetPlacableType("AssemblyLineT1End"));
    }

    public void ButtonClicked(int _buttonId)
    {
        if (placeableStacks[_buttonId].HasPlaceable())
        {
            var placeableType = placeableStacks[_buttonId].PopPlacable();
            ObjectPlacer.PlaceObject(placeableType);
            Debug.Log("Button " + _buttonId + " pressed, placing: " + placeableType);
        }
    }

    public void ButtonBeginDrag(int _buttonId)
    {
        fromBtn = _buttonId;
        var quickSlotBtnImage = QuickSlotBtnImages[fromBtn];
        DragTarget.sprite = quickSlotBtnImage.sprite;
        quickSlotBtnImage.enabled = false;
        DragTarget.enabled = true;

        Debug.Log("Button " + _buttonId + " BeginDrag!");
    }

    public void ButtonDraging()
    {
        var mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        DragTarget.transform.position = mousePosition;
    }

    public void ButtonDrop(int _buttonId)
    {
        toBtn = _buttonId;
        dropedOnBtn = true;
        Debug.Log("Button " + _buttonId + " Drop!");
    }

    public void ButtonEndDrag()
    {
        if (dropedOnBtn)
        {
            QuickSlotBtnImages[toBtn].sprite = DragTarget.sprite;
            QuickSlotBtnImages[toBtn].enabled = true;

            var tmpPlaceableStack = placeableStacks[toBtn];
            placeableStacks[toBtn] = placeableStacks[fromBtn];
            placeableStacks[fromBtn] = tmpPlaceableStack;

            Debug.Log("Dragged from: " + fromBtn + " droped on: " + toBtn + " placeable: " +
                      placeableStacks[toBtn].PlaceableType);
        }
        else
        {
            placeableStacks[fromBtn] = NullPlaceableStack.Instance;
            //Todo: Make sure that objects are added back to the inventory
            Debug.Log("Dragged from: " + fromBtn + " droped in space");
        }

        fromBtn = -1;
        toBtn = -1;
        dropedOnBtn = false;
        DragTarget.enabled = false;
    }
}
