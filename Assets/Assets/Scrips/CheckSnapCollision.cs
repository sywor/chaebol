using UnityEngine;

public class CheckSnapCollision : MonoBehaviour
{
    private ObjectPlacer objectPlacer;

    private void Start()
    {
        var world = GameObject.FindGameObjectWithTag("World");
        objectPlacer = world.GetComponent<ObjectPlacer>();
    }

    private void OnTriggerEnter(Collider _other)
    {
        Debug.Log(_other.name);

        if (_other.CompareTag("Snapable"))
        {
            objectPlacer.SnapDetected(transform.parent.gameObject, _other.gameObject.transform.parent.gameObject);
        }
    }
}
