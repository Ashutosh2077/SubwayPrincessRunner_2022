using System;

[Serializable]
public class CameraFollowPlayerConfig
{
	public CameraFollowPlayerConfig(bool customConstructor)
	{
	}

	public void ApplyNewConfig(CameraFollowPlayerConfig cameraFollowPlayerConfig)
	{
		this.NormalConfig.ApplyNewConfig(cameraFollowPlayerConfig.NormalConfig);
		this.FlyingConfig.ApplyNewConfig(cameraFollowPlayerConfig.FlyingConfig);
		this.SpringConfig.ApplyNewConfig(cameraFollowPlayerConfig.SpringConfig);
		this.TunnelCenterConfig.ApplyNewConfig(cameraFollowPlayerConfig.TunnelCenterConfig);
		this.OverdriveConfig.ApplyNewConfig(cameraFollowPlayerConfig.OverdriveConfig);
	}

	public static CameraFollowPlayerConfig GetNewCameraFollowPlayerConfig()
	{
		return new CameraFollowPlayerConfig(true);
	}

	public CameraConfig BoundConfig = new CameraConfig();

	public CameraConfig FlyingConfig = new CameraConfig();

	public CameraConfig NormalConfig = new CameraConfig();

	public CameraConfig OverdriveConfig = new CameraConfig();

	public CameraConfig SpringConfig = new CameraConfig();

	public CameraConfig TunnelCenterConfig = new CameraConfig();

	public CameraConfig WallConfig = new CameraConfig();
}
