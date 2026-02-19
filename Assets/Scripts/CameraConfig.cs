using System;
using UnityEngine;

[Serializable]
public class CameraConfig
{
	public void ApplyNewConfig(CameraConfig CameraConfig)
	{
		this.LookAtOffset = CameraConfig.LookAtOffset;
		this.PositionOffset = CameraConfig.PositionOffset;
		this.cameraFOV = CameraConfig.cameraFOV;
		this.CameraClamp = CameraConfig.CameraClamp;
	}

	public void Lerp(CameraConfig a, CameraConfig b, float t)
	{
		this.cameraFOV = Mathf.LerpUnclamped(a.cameraFOV, b.cameraFOV, t);
		this.LookAtOffset = Vector3.LerpUnclamped(a.LookAtOffset, b.LookAtOffset, t);
		this.PositionOffset = Vector3.LerpUnclamped(a.PositionOffset, b.PositionOffset, t);
		this.CameraClamp = Vector2.LerpUnclamped(a.CameraClamp, b.CameraClamp, t);
	}

	public Vector3 PositionOffset;

	public Vector3 LookAtOffset;

	public float cameraFOV;

	public Vector2 CameraClamp;
}
