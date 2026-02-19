using System;
using UnityEngine;

[Serializable]
public struct CameraState
{
	public CameraState(Camera camera)
	{
		if (camera != null)
		{
			this.Position = camera.transform.position;
			this.Rotation = camera.transform.rotation;
			this.FieldOfView = camera.fieldOfView;
		}
		else
		{
			this.Position = Vector3.zero;
			this.Rotation = Quaternion.identity;
			this.FieldOfView = 60f;
		}
	}

	public CameraState(Transform trans)
	{
		if (trans != null)
		{
			this.Position = trans.position;
			this.Rotation = trans.rotation;
			this.FieldOfView = 60f;
		}
		else
		{
			this.Position = Vector3.zero;
			this.Rotation = Quaternion.identity;
			this.FieldOfView = 60f;
		}
	}

	public void ApplyToCamera(Camera camera)
	{
		camera.transform.position = this.Position;
		camera.transform.rotation = this.Rotation;
		camera.fieldOfView = this.FieldOfView;
	}

	public void ApplyToObject(Transform trans)
	{
		trans.position = this.Position;
		trans.rotation = this.Rotation;
	}

	public Vector3 Position;

	public Quaternion Rotation;

	public float FieldOfView;
}
