using System;
using UnityEngine;
using UnityEngine.UI;

public class ShortcutMenu : MonoBehaviour
{
    public Image[] QuickSlotBtnImages = new Image[20];

    private enum DragState : byte
    {
        UNKNOWN = 0,
        POTENTIAL = 1,
        DRAG = 2,
        DROP = 4
    }
    
    private class DragProgress
    {
        public DragState State { get; set; }
        public int FromBtn { get; set; }
        public int ToBtn { get; set; }

        public DragProgress()
        {
            Reset();
        }

        public void Reset()
        {
            State = DragState.UNKNOWN;
            FromBtn = -1;
            ToBtn = -1;
        }

        public override string ToString()
        {
            return "FromBtn: " + FromBtn + " ToBtn: " + ToBtn + " DragState: " + State;
        }
    }

    private readonly DragProgress dragProgress = new DragProgress();

    public void ButtonClicked(int _buttonId)
    {
        Debug.Log("Button " + _buttonId + " pressed!");
    }

    public void ButtonPotentialDrag(int _buttonId)
    {
        dragProgress.FromBtn = _buttonId;
        dragProgress.State =  DragState.POTENTIAL;
    }
    
    public void ButtonDrag(int _buttonId)
    {
        dragProgress.State = DragState.DRAG;
    }
    
    public void ButtonDrop(int _buttonId)
    {
        dragProgress.ToBtn = _buttonId;
        dragProgress.State = DragState.DROP;
    }

    private void Update()
    {
        if (DragState.DROP == dragProgress.State)
        {
            Debug.Log("Moved item from: " + dragProgress.FromBtn + " To: " + dragProgress.ToBtn);
            dragProgress.Reset();
        }
    }
}