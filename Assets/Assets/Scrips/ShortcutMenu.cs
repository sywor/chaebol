using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class ShortcutMenu : MonoBehaviour
{
    public Image[] QuickSlotBtnImages = new Image[20];
    public Image DragTarget;

    private int fromBtn = -1;
    private int toBtn = -1;
    private bool dropedOnBtn;

    private void Start()
    {
        QuickSlotBtnImages[0].sprite = Resources.Load<Sprite>("assemblyline_icon");
        QuickSlotBtnImages[0].enabled = true;

        foreach (var image in QuickSlotBtnImages)
        {
            image.type = Image.Type.Simple;
        }
    }

    public void ButtonClicked(int _buttonId)
    {
        Debug.Log("Button " + _buttonId + " pressed!");
    }

    public void ButtonBeginDrag(int _buttonId)
    {
        fromBtn = _buttonId;
        var quickSlotBtnImage = QuickSlotBtnImages[fromBtn];
        DragTarget.sprite = quickSlotBtnImage.sprite;
        quickSlotBtnImage.enabled = false;
        DragTarget.enabled = true;
        //quickSlotBtnImage.enabled = false;
        
        Debug.Log("Button " + _buttonId + " BeginDrag!");
    }

    public void ButtonDraging()
    {
        var mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        DragTarget.transform.position = mousePosition;
        Debug.Log("Button draging: " + Utils.ToString(mousePosition));
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
            Debug.Log("Dragged from: " + fromBtn + " droped on: " + toBtn);
        }
        else
        {
            QuickSlotBtnImages[fromBtn].sprite = DragTarget.sprite;
            QuickSlotBtnImages[fromBtn].enabled = true;
            Debug.Log("Dragged from: " + fromBtn + " droped in space");
        }

        fromBtn = -1;
        toBtn = -1;
        dropedOnBtn = false;
        DragTarget.enabled = false;
    }

    private void Update()
    {

    }
}