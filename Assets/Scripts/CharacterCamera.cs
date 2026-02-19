using System;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
	public void Awake()
	{
		this._camera = base.GetComponent<Camera>();
		this._anim = base.transform.parent.GetComponent<Animation>();
		this.CurrentCameraConfig = new CameraConfig();
		this.StartCameraConfig = new CameraConfig();
		this.EndCameraConfig = new CameraConfig();
		this.topMenuCameraState = new CameraState(this._camera);
		this.topMenuCameraParentState = new CameraState(base.transform.parent);
	}

	private void NormalUpdatePosition(Vector3 position, Quaternion rotation, float fov, float deltaTime, bool allowHeightBlending)
	{
		Vector3 normalized = Vector3.Cross(rotation * Vector3.forward, Vector3.right).normalized;
		if (allowHeightBlending)
		{
			float num = Mathf.SmoothDamp(this.previousCharacterPosY, position.y, ref this.valueSpeedY, this.YFactor);
			if (!float.IsNaN(num))
			{
				position.y = num;
			}
		}
		float y;
		if (this.MaxCameraHeight < 0f)
		{
			y = position.y;
		}
		else
		{
			y = Mathf.Min(this.MaxCameraHeight, position.y);
		}
		Vector3 a = new Vector3((position.x >= 0f) ? Mathf.Min(this.CurrentCameraConfig.CameraClamp.y, position.x) : Mathf.Max(this.CurrentCameraConfig.CameraClamp.x, position.x), y, position.z);
		if (this.SmoothCameraHorizontalMovement)
		{
			float num = Mathf.SmoothDamp(this.previousCharacterPosX, a.x, ref this.valueSpeedX, this.XFactor);
			if (!float.IsNaN(num))
			{
				a.x = num;
			}
		}
		this.previousCharacterPosX = a.x;
		this.previousCharacterPosY = a.y;
		this.CurrentCameraState.Position = a + rotation * this.CurrentCameraConfig.PositionOffset + this.CameraShakeController.UpdateShakeController();
		this.CurrentCameraState.Rotation = Quaternion.LookRotation(Vector3.Normalize(a + rotation * Quaternion.Euler(this.CameraWobbleController.UpdateWobbleController()) * this.CurrentCameraConfig.LookAtOffset - this.CurrentCameraState.Position), normalized);
		this.CurrentCameraState.FieldOfView = Mathf.Lerp(this.CurrentCameraState.FieldOfView, fov / this._camera.aspect, this.FOVBlendSpeed * deltaTime);
	}

	public void Reset(Vector3 position, Quaternion rotation, bool resetPos = false)
	{
		this.previousCharacterPosX = position.x;
		this.previousCharacterPosY = position.y;
		this.TransitionTime = 0f;
		this.CurrentTransitionProgress = 0f;
		this.MaxCameraHeight = -1f;
		this.InternalSubwayExitCameraProgress = 0f;
		this.InternalSubwayEnterCameraProgress = 0f;
		this.currentSpringProgress = 0f;
		this.currentTunnelProgress = 0f;
		this.SmoothCameraHorizontalMovement = true;
		this.SetCameraToNormal();
		if (resetPos)
		{
			this._camera.transform.position = position + this.CurrentCameraConfig.PositionOffset;
			this._camera.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(position + this.CurrentCameraConfig.LookAtOffset - this._camera.transform.position), position);
			this._camera.fieldOfView = this.CurrentCameraConfig.cameraFOV / this._camera.aspect;
		}
		this.CurrentCameraState = new CameraState(this._camera);
		this.CurrentFollowMode = CameraFollowMode.Normal;
		this.UpdatePosition(position, rotation, 1f, true);
	}

	public void Revive()
	{
		this.SetMaxCameraHeight(-1f);
	}

	private void SetCameraToNormal()
	{
		this.CurrentCameraConfig.ApplyNewConfig(this.currentCameraFollowPlayerConfig.NormalConfig);
	}

	public void SetCameraToTopMenu()
	{
		this._anim.Stop();
		this.SetMaxCameraHeight(-1f);
		this.CurrentFollowMode = CameraFollowMode.TopMenu;
		this.topMenuCameraParentState.ApplyToObject(base.transform.parent);
		this.topMenuCameraState.ApplyToCamera(this._camera);
		this.CurrentCameraState = new CameraState(this._camera);
	}

	public void SetCameraToTunnleTransition()
	{
		this.SetMaxCameraHeight(-1f);
		this.CurrentFollowMode = CameraFollowMode.TunnelTransition;
		this.StartCameraConfig.ApplyNewConfig(this.CurrentCameraConfig);
		this.EndCameraConfig.ApplyNewConfig(this.currentCameraFollowPlayerConfig.TunnelCenterConfig);
	}

	public void SetCameraTransition(CameraFollowMode mode, float transitionTime = 1f)
	{
		this.SetMaxCameraHeight(-1f);
		this.TransitionTime = transitionTime;
		this.CurrentTransitionProgress = 0f;
		this.CurrentFollowMode = mode;
		this.StartCameraConfig.ApplyNewConfig(this.CurrentCameraConfig);
		switch (mode)
		{
		case CameraFollowMode.FlyUp:
			this.EndCameraConfig.ApplyNewConfig(this.currentCameraFollowPlayerConfig.FlyingConfig);
			break;
		case CameraFollowMode.FlyDown:
			this.EndCameraConfig.ApplyNewConfig(this.currentCameraFollowPlayerConfig.NormalConfig);
			break;
		case CameraFollowMode.StairDown:
			this.EndCameraConfig.ApplyNewConfig(this.currentCameraFollowPlayerConfig.NormalConfig);
			break;
		case CameraFollowMode.StairUp:
			this.EndCameraConfig.ApplyNewConfig(this.currentCameraFollowPlayerConfig.NormalConfig);
			break;
		case CameraFollowMode.SpringUp:
			this.EndCameraConfig.ApplyNewConfig(this.currentCameraFollowPlayerConfig.SpringConfig);
			break;
		case CameraFollowMode.SpringDown:
			this.StartCameraConfig.ApplyNewConfig(this.currentCameraFollowPlayerConfig.NormalConfig);
			this.EndCameraConfig.ApplyNewConfig(this.CurrentCameraConfig);
			break;
		case CameraFollowMode.SpeedUp:
			this.EndCameraConfig.ApplyNewConfig(this.currentCameraFollowPlayerConfig.OverdriveConfig);
			break;
		case CameraFollowMode.SpeedDown:
			this.EndCameraConfig.ApplyNewConfig(this.currentCameraFollowPlayerConfig.NormalConfig);
			break;
		case CameraFollowMode.WallUp:
			this.EndCameraConfig.ApplyNewConfig(this.currentCameraFollowPlayerConfig.WallConfig);
			break;
		case CameraFollowMode.WallDown:
			this.StartCameraConfig.ApplyNewConfig(this.EndCameraConfig);
			this.EndCameraConfig.ApplyNewConfig(this.currentCameraFollowPlayerConfig.NormalConfig);
			break;
		}
	}

	public void SetMaxCameraHeight(float value)
	{
		this.MaxCameraHeight = value;
	}

	public void SetStartPositionY(float startPositionY)
	{
		this.StartPostionY = startPositionY;
	}

	public void StartRun(Vector3 position, Quaternion rotation)
	{
		this.CurrentFollowMode = CameraFollowMode.Normal;
		this.Reset(position, rotation, false);
	}

	private void TransitionUpdatePosition(float deltaTime)
	{
		this.CurrentTransitionProgress += deltaTime / this.TransitionTime;
		this.CurrentCameraConfig.Lerp(this.StartCameraConfig, this.EndCameraConfig, this.CurrentTransitionProgress);
		if (this.CurrentTransitionProgress >= 1f)
		{
			this.CurrentTransitionProgress = 0f;
			if (this.CurrentFollowMode != CameraFollowMode.FlyUp && this.CurrentFollowMode != CameraFollowMode.SpeedUp && this.CurrentFollowMode != CameraFollowMode.WallUp)
			{
				if (this.CurrentFollowMode != CameraFollowMode.FlyDown && this.CurrentFollowMode != CameraFollowMode.SpeedDown && this.CurrentFollowMode != CameraFollowMode.WallDown)
				{
					throw new Exception("Implement camera position update state change");
				}
				this.CurrentFollowMode = CameraFollowMode.Normal;
			}
			else
			{
				this.CurrentFollowMode = CameraFollowMode.Flying;
			}
		}
	}

	private void UpdateJumpTransitionStepDownAfter(Vector3 position)
	{
		if (base.transform.position.y - position.y >= this.CurrentCameraConfig.PositionOffset.y)
		{
			this.CurrentCameraState.Position = new Vector3(this.CurrentCameraState.Position.x, position.y + this.CurrentCameraConfig.PositionOffset.y, this.CurrentCameraState.Position.z);
		}
		if (this.currentSpringProgress >= 1f)
		{
			this.currentSpringProgress = 0f;
			this.CurrentFollowMode = CameraFollowMode.Normal;
		}
	}

	private void UpdateJumpTransitionStepDownBefore()
	{
		float time = Mathf.Min(this.currentSpringProgress, 1f);
		float t = this.SubwaySpringDownCameraBlending.Evaluate(time);
		this.CurrentCameraConfig.Lerp(this.EndCameraConfig, this.StartCameraConfig, t);
	}

	private void UpdateJumpTransitionStepUpAfter()
	{
		float t = this.SubwayCameraSpringBlending.Evaluate(this.currentSpringProgress);
		float y = Mathf.Lerp(this.StartPostionY, this.CurrentCameraState.Position.y, t);
		this.CurrentCameraState.Position = new Vector3(this.CurrentCameraState.Position.x, y, this.CurrentCameraState.Position.z);
		if (this.currentSpringProgress >= 1f)
		{
			this.currentSpringProgress = 0f;
			this.StartPostionY = this.CurrentCameraState.Position.y;
			if (this.CurrentFollowMode != CameraFollowMode.SpringUp)
			{
				throw new Exception("Implement camera position update state change");
			}
			this.CurrentFollowMode = CameraFollowMode.SpringDown;
		}
	}

	private void UpdateJumpTransitionStepUpBefore()
	{
		float t = this.SubwaySpringUpCameraBlending.Evaluate(this.currentSpringProgress);
		this.CurrentCameraConfig.Lerp(this.StartCameraConfig, this.EndCameraConfig, t);
	}

	public void UpdatePosition(Vector3 position, Quaternion rotation, float deltaTime, bool allowHeightBlending)
	{
		switch (this.CurrentFollowMode)
		{
		case CameraFollowMode.TunnelTransition:
			this.UpdateTunnelTransitionStep();
			this.NormalUpdatePosition(position, rotation, this.CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
			break;
		case CameraFollowMode.Normal:
		case CameraFollowMode.Flying:
			this.NormalUpdatePosition(position, rotation, this.CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
			break;
		case CameraFollowMode.FlyUp:
		case CameraFollowMode.WallUp:
			this.TransitionUpdatePosition(deltaTime);
			this.NormalUpdatePosition(position, rotation, this.CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
			break;
		case CameraFollowMode.FlyDown:
		case CameraFollowMode.WallDown:
			this.TransitionUpdatePosition(deltaTime);
			this.NormalUpdatePosition(position, rotation, this.CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
			break;
		case CameraFollowMode.StairDown:
			this.UpdateStairTransitionStepDown();
			this.NormalUpdatePosition(position, rotation, this.CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
			break;
		case CameraFollowMode.StairUp:
			this.UpdateStairTransitionStepUp();
			this.NormalUpdatePosition(position, rotation, this.CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
			break;
		case CameraFollowMode.TransitionDown:
			this.UpdateStairTransitionStepDown();
			this.NormalUpdatePosition(position, rotation, this.CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
			break;
		case CameraFollowMode.TransitionUp:
			this.UpdateStairTransitionStepUp();
			this.NormalUpdatePosition(position, rotation, this.CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
			break;
		case CameraFollowMode.SpringUp:
			this.UpdateJumpTransitionStepUpBefore();
			this.NormalUpdatePosition(position, rotation, this.CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
			this.UpdateJumpTransitionStepUpAfter();
			break;
		case CameraFollowMode.SpringDown:
			this.UpdateJumpTransitionStepDownBefore();
			this.NormalUpdatePosition(position, rotation, this.CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
			this.UpdateJumpTransitionStepDownAfter(position);
			break;
		case CameraFollowMode.SpeedUp:
			this.TransitionUpdatePosition(deltaTime);
			this.NormalUpdatePosition(position, rotation, this.CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
			break;
		case CameraFollowMode.SpeedDown:
			this.TransitionUpdatePosition(deltaTime);
			this.NormalUpdatePosition(position, rotation, this.CurrentCameraConfig.cameraFOV, deltaTime, allowHeightBlending);
			break;
		}
		this.CurrentCameraState.ApplyToCamera(this._camera);
	}

	private void UpdateStairTransitionStepDown()
	{
		if (this.InternalSubwayEnterCameraProgress <= 0.6f)
		{
			float time = Mathf.Min(this.InternalSubwayEnterCameraProgress / 0.6f, 1f);
			float t = this.SubwayEnterStairCameraBlending.Evaluate(time);
			this.CurrentCameraConfig.Lerp(this.StartCameraConfig, this.SubwayStairCameraSettings.SubwayDownCenterCameraConfig, t);
			this.CurrentCameraConfig.LookAtOffset.y = Mathf.LerpUnclamped(this.StartCameraConfig.LookAtOffset.y, this.SubwayStairCameraSettings.SubwayDownCenterCameraConfig.LookAtOffset.y, this.SubwayStairCameraSettings.DescendLookAtYoffsetAC.Evaluate(time));
			this.CurrentCameraConfig.PositionOffset.y = Mathf.LerpUnclamped(this.StartCameraConfig.PositionOffset.y, this.SubwayStairCameraSettings.SubwayDownCenterCameraConfig.PositionOffset.y, this.SubwayStairCameraSettings.DescendPositionYoffsetAC.Evaluate(time));
		}
		else
		{
			float t = this.SubwayPostStairCameraBlending.Evaluate(this.InternalSubwayEnterCameraProgress);
			this.CurrentCameraConfig.Lerp(this.SubwayStairCameraSettings.SubwayDownCenterCameraConfig, this.EndCameraConfig, t);
		}
		if (this.InternalSubwayEnterCameraProgress >= 1f)
		{
			this.InternalSubwayEnterCameraProgress = 0f;
			this.CurrentCameraConfig.ApplyNewConfig(this.EndCameraConfig);
			this.CurrentFollowMode = CameraFollowMode.Normal;
		}
	}

	private void UpdateStairTransitionStepUp()
	{
		if (this.InternalSubwayExitCameraProgress < 0.6f)
		{
			float time = Mathf.Min(this.InternalSubwayExitCameraProgress / 0.6f, 1f);
			float t = this.SubwayExitStairCameraBlending.Evaluate(time);
			this.CurrentCameraConfig.Lerp(this.StartCameraConfig, this.SubwayStairCameraSettings.SubwayUpCenterCameraConfig, t);
			this.CurrentCameraConfig.LookAtOffset.y = Mathf.LerpUnclamped(this.StartCameraConfig.LookAtOffset.y, this.SubwayStairCameraSettings.SubwayUpCenterCameraConfig.LookAtOffset.y, this.SubwayStairCameraSettings.AscendLookAtYoffsetAC.Evaluate(time));
			this.CurrentCameraConfig.PositionOffset.y = Mathf.LerpUnclamped(this.StartCameraConfig.PositionOffset.y, this.SubwayStairCameraSettings.SubwayUpCenterCameraConfig.PositionOffset.y, this.SubwayStairCameraSettings.AscendPositionYoffsetAC.Evaluate(time));
		}
		else
		{
			float t = this.SubwayPostStairCameraBlending.Evaluate(Mathf.Min(this.InternalSubwayExitCameraProgress, 1f));
			this.CurrentCameraConfig.Lerp(this.SubwayStairCameraSettings.SubwayUpCenterCameraConfig, this.EndCameraConfig, t);
		}
		if (this.InternalSubwayExitCameraProgress >= 1f)
		{
			this.InternalSubwayExitCameraProgress = 0f;
			this.CurrentCameraConfig.ApplyNewConfig(this.EndCameraConfig);
			this.CurrentFollowMode = CameraFollowMode.Normal;
		}
	}

	private void UpdateTunnelTransitionStep()
	{
		float t = this.TunnelCameraBlending.Evaluate(this.currentTunnelProgress);
		this.CurrentCameraConfig.Lerp(this.StartCameraConfig, this.EndCameraConfig, t);
		if (this.currentTunnelProgress >= 1f)
		{
			this.currentTunnelProgress = 0f;
			this.CurrentCameraConfig.ApplyNewConfig(this.StartCameraConfig);
			this.CurrentFollowMode = CameraFollowMode.Normal;
		}
	}

	public Animation Animation
	{
		get
		{
			return this._anim;
		}
	}

	public Camera Camera
	{
		get
		{
			return this._camera;
		}
	}

	public CameraFollowPlayerConfig CurrentCameraFollowPlayerConfig
	{
		get
		{
			return this.currentCameraFollowPlayerConfig;
		}
	}

	public float CurrentSpringProgress
	{
		set
		{
			this.currentSpringProgress = value;
		}
	}

	public float CurrentTunnelProgress
	{
		set
		{
			this.currentTunnelProgress = value;
		}
	}

	public static CharacterCamera Instance
	{
		get
		{
			if (CharacterCamera.instance == null)
			{
				CharacterCamera.instance = (UnityEngine.Object.FindObjectOfType(typeof(CharacterCamera)) as CharacterCamera);
			}
			return CharacterCamera.instance;
		}
	}

	public float SubwayEnterCameraProgress
	{
		set
		{
			this.InternalSubwayEnterCameraProgress = value;
		}
	}

	public float SubwayExitCameraProgress
	{
		set
		{
			this.InternalSubwayExitCameraProgress = value;
		}
	}

	private Animation _anim;

	private Camera _camera;

	public CameraShakeController CameraShakeController;

	public CameraWobbleController CameraWobbleController;

	private CameraConfig CurrentCameraConfig;

	private CameraState CurrentCameraState;

	private CameraFollowMode CurrentFollowMode;

	private CameraConfig StartCameraConfig;

	private CameraConfig EndCameraConfig;

	[SerializeField]
	private CameraFollowPlayerConfig currentCameraFollowPlayerConfig;

	private float currentSpringProgress;

	private float CurrentTransitionProgress;

	private float currentTunnelProgress;

	private float InternalSubwayEnterCameraProgress;

	private static CharacterCamera instance;

	private float InternalSubwayExitCameraProgress;

	private float MaxCameraHeight = -1f;

	private float previousCharacterPosX;

	private float previousCharacterPosY;

	private bool SmoothCameraHorizontalMovement;

	private float StartPostionY;

	[SerializeField]
	private AnimationCurve SubwayEnterStairCameraBlending;

	[SerializeField]
	private AnimationCurve SubwayExitStairCameraBlending;

	[SerializeField]
	private AnimationCurve SubwayPostStairCameraBlending;

	[SerializeField]
	private AnimationCurve SubwayCameraSpringBlending;

	[SerializeField]
	private AnimationCurve SubwaySpringDownCameraBlending;

	[SerializeField]
	private AnimationCurve SubwaySpringUpCameraBlending;

	[SerializeField]
	private SubwayCameraSettings SubwayStairCameraSettings;

	private CameraState topMenuCameraState;

	private CameraState topMenuCameraParentState;

	private float TransitionTime;

	[SerializeField]
	private AnimationCurve TunnelCameraBlending;

	[SerializeField]
	private float XFactor = 0.05f;

	[SerializeField]
	private float YFactor = 0.1f;

	[SerializeField]
	private float FOVBlendSpeed;

	private float valueSpeedX;

	private float valueSpeedY;
}
