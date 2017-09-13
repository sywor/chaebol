using System;
using UnityEngine;

public class HardPointVisability : MonoBehaviour
{
    public GameObject Right;
    public GameObject Left;
    public GameObject Forward;
    public GameObject Rear;
    
    public void ShowHardPoints()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void HideHardPoints()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void HideSelectedHardPoint(Side _side)
    {
        switch (_side)
        {
            case Side.RIGHT:
                Right.SetActive(false);
                break;
            case Side.LEFT:
                Left.SetActive(false);
                break;
            case Side.FORWARD:
                Forward.SetActive(false);
                break;
            case Side.REAR:
                Rear.SetActive(false);
                break;
        }
    }
}
