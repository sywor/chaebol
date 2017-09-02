using UnityEngine;

namespace Assets.GDI.Code.Tools
{
	public class KeyboardControls : MonoBehaviour
	{

		private float _rotationX;
		private float _rotationY;

		void Start ()
		{
			Cursor.lockState = CursorLockMode.None;
		}

		void Update ()
		{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				_rotationX += Input.GetAxis("Mouse X") * Time.deltaTime * 100;
				_rotationY += Input.GetAxis("Mouse Y") * Time.deltaTime * 100;
				_rotationY = Mathf.Clamp (_rotationY, -90, 90);
				transform.localRotation = Quaternion.AngleAxis(_rotationX, Vector3.up);
				transform.localRotation *= Quaternion.AngleAxis(_rotationY, Vector3.left);
			}

			if (Input.GetKey(KeyCode.Q)) transform.position += transform.up * Time.deltaTime * 10;
			if (Input.GetKey(KeyCode.E)) transform.position -= transform.up * Time.deltaTime * 10;

			transform.position += transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * 20;
			transform.position += transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * 20;

			if (Input.GetKeyDown (KeyCode.Space))
			{
				Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
			}
		}
	}
}