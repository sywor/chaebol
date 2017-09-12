using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardPointVisability : MonoBehaviour
{
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
}
