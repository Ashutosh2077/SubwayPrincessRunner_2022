using System;
using UnityEngine;

[Serializable]
public class SubwayCameraSettings
{
	public AnimationCurve AscendLookAtYoffsetAC;

	public AnimationCurve AscendPositionYoffsetAC;

	public AnimationCurve DescendLookAtYoffsetAC;

	public AnimationCurve DescendPositionYoffsetAC;

	public CameraConfig SubwayDownCenterCameraConfig;

	public CameraConfig SubwayUpCenterCameraConfig;
}
