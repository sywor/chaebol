using System;
using UnityEngine;

public class CameraMovment : MonoBehaviour
{
	public float Speed = 0.5f;
	public float PosXZMax = 500.0f;
	public float PosXZMin = -500.0f;
	public float PosYMin = 5.0f;
	public float PosYMax = 500.0f;
	public float RotateYawSpeed = 2.0f;
	public float RotatePitchSpeed = 2.0f;
	public float ScrollSpeed = 5.0f;

	private Vector3 desiredPos;
	private Vector3 smoothSpeed = Vector3.zero;

	private float yaw;
	private float pitch;
	private float scroll;

	private void Start()
	{
		desiredPos = transform.position;
	}

	void Update ()
	{
		UpdatePosition();
		RotateCamera();
	}

	private void UpdatePosition()
	{
		if (Input.GetKey(KeyCode.W))
		{
			desiredPos += Speed * Vector3.Cross(transform.right, Vector3.up);
		}
		if (Input.GetKey(KeyCode.S))
		{
			desiredPos -= Speed * Vector3.Cross(transform.right, Vector3.up);
		}
		if (Input.GetKey(KeyCode.A))
		{
			desiredPos -= Speed * transform.right;
		}
		if (Input.GetKey(KeyCode.D))
		{
			desiredPos += Speed * transform.right;
		}

		var tmp = desiredPos + transform.forward * Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;

		if (tmp.y < PosYMax && tmp.y >= PosYMin &&
		    tmp.x < PosXZMax && tmp.x >= PosXZMin &&
		    tmp.z < PosXZMax && tmp.z >= PosXZMin)
		{
			desiredPos = tmp;
		}

		desiredPos.x = Mathf.Clamp(desiredPos.x, PosXZMin, PosXZMax);
		desiredPos.y = Mathf.Clamp(desiredPos.y, PosYMin, PosYMax);
		desiredPos.z = Mathf.Clamp(desiredPos.z, PosXZMin, PosXZMax);

		transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref smoothSpeed, 0.3f);
	}

	private void RotateCamera()
	{
		if (!Input.GetMouseButton(1)) return;

		yaw += RotateYawSpeed * Input.GetAxis("Mouse X");
		pitch -= RotatePitchSpeed * Input.GetAxis("Mouse Y");
		pitch = Mathf.Clamp(pitch, 0, 90);

		transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
	}
}
