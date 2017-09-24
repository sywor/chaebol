using UnityEngine;

public class Initializer : MonoBehaviour
{
    private void Awake()
    {
        TypeRegistry.Instance.Init();
    }
}
