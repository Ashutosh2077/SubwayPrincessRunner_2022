using System;
using System.Collections;
using UnityEngine;

public class BoundJump : CharacterState
{
	public void Awake()
	{
		this.game = Game.Instance;
		this.trackController = TrackController.Instance;
		this.character = Character.Instance;
		this.characterController = this.character.characterController;
		this.characterTransform = this.characterController.transform;
		this.characterCamera = CharacterCamera.Instance;
		this.characterCameraTransform = this.characterCamera.transform;
		this.coinLineManager = CoinLineManager.Instance;
	}

	public override IEnumerator Begin()
	{
		this.isActive = true;
		GameStats instance = GameStats.Instance;
		instance.pickedUpPowerups++;
		this.coinLineManager.ToggleLines(true);
		this.character.characterModel.HideBlobShadow();
		this.game.ResetEnemy();
		this.character.inAirJump = false;
		this.character.IsGrounded.Value = false;
		this.game.Attachment.PauseInFlypackMode();
		if (this.character.IsStumbling)
		{
			this.character.StopStumble();
		}
		this.TransitionDownFirstUpdate = true;
		this.NotifyOnStart();
		float normalizedPosition = ObliqueMotion.CalcTB((this.characterTransform.position.y - this.startPosition.y) / this.jumpHeight);
		this.startPosition.z = this.characterTransform.position.z - this.jumpDistance * normalizedPosition;
		Vector3 endPosition = this.startPosition + Vector3.forward * this.totalDistance;
		float speed = this.game.currentSpeed;
		float progress = 0f;
		this.characterCamera.SetStartPositionY(this.characterCameraTransform.position.y);
		this.characterCamera.SetCameraTransition(CameraFollowMode.SpringUp, 1f);
		while (this.character.z < endPosition.z)
		{
			this.game.HandleControls();
			this.character.z += speed * Time.deltaTime;
			normalizedPosition = (this.character.z - this.startPosition.z) / this.jumpDistance;
			Vector3 pivot = this.trackController.GetPosition(this.character.x, this.character.z) + Vector3.up * (ObliqueMotion.CalcHeight(normalizedPosition) * this.jumpHeight + this.startPosition.y + this.yOffset);
			if (normalizedPosition <= this.fadeInPosition)
			{
				pivot.y = Mathf.Lerp(this.startPosition.y + this.yOffset, pivot.y, normalizedPosition / this.fadeInPosition);
			}
			this.characterTransform.position = pivot;
			progress = normalizedPosition * 2f;
			this.characterCamera.CurrentSpringProgress = progress;
			if (normalizedPosition > 0.5f && !this.TransitionDownFirstUpdate)
			{
				progress = Mathf.Clamp01((normalizedPosition * 2f - 1f) * (this.jumpDistance * 0.5f / (this.totalDistance - this.jumpDistance * 0.5f)));
				this.characterCamera.CurrentSpringProgress = progress;
			}
			if (normalizedPosition > 0.5f && this.TransitionDownFirstUpdate)
			{
				this.TransitionDownFirstUpdate = false;
			}
			this.characterCamera.UpdatePosition(pivot, Quaternion.identity, Time.deltaTime, false);
			this.game.UpdateMeters();
			this.game.LayTrackChunks();
			yield return null;
		}
		this.isActive = false;
		this.NotifyOnStop();
		this.game.ChangeState(this.game.Running);
		this.character.verticalSpeed = Mathf.Min(-this.character.CalculateJumpVerticalSpeed(ObliqueMotion.CalcHeight(this.totalDistance / this.jumpDistance) * this.jumpHeight + this.yOffset), this.character.verticalFallSpeedLimit) - 1f;
		this.character.IsFalling = true;
		this.game.Attachment.Resume();
		yield break;
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		if (swipeDir != SwipeDir.Left)
		{
			if (swipeDir == SwipeDir.Right)
			{
				this.character.ChangeTrack(1, this.characterChangeTrackLength / this.game.currentSpeed);
			}
		}
		else
		{
			this.character.ChangeTrack(-1, this.characterChangeTrackLength / this.game.currentSpeed);
		}
	}

	private void NotifyOnStart()
	{
		if (this.OnStart != null)
		{
			this.OnStart();
		}
	}

	private void NotifyOnStop()
	{
		if (this.OnStop != null)
		{
			this.OnStop();
		}
	}

	public void SetBoundJumpData(float jumpHeight, float jumpDistance, float totalDistance, Vector3 position)
	{
		this.jumpHeight = jumpHeight;
		this.jumpDistance = jumpDistance;
		this.totalDistance = totalDistance;
		this.startPosition = position;
	}

	public static BoundJump Instance
	{
		get
		{
			if (BoundJump.instance == null)
			{
				BoundJump.instance = (UnityEngine.Object.FindObjectOfType(typeof(BoundJump)) as BoundJump);
			}
			return BoundJump.instance;
		}
	}

	public override bool PauseActiveModifiers
	{
		get
		{
			return true;
		}
	}

	[HideInInspector]
	public bool isActive;

	[SerializeField]
	private float characterChangeTrackLength = 60f;

	[SerializeField]
	private float fadeInPosition = 0.1f;

	[SerializeField]
	private float yOffset = 1f;

	private float jumpHeight = 95f;

	private float jumpDistance = 800f;

	private float totalDistance = 800f;

	private bool TransitionDownFirstUpdate;

	private Character character;

	private CharacterCamera characterCamera;

	private Transform characterCameraTransform;

	private CharacterController characterController;

	private Transform characterTransform;

	private CoinLineManager coinLineManager;

	private Game game;

	private static BoundJump instance;

	private TrackController trackController;

	public BoundJump.OnStartDelegate OnStart;

	public BoundJump.OnStopDelegate OnStop;

	private Vector3 startPosition;

	public delegate void OnStartDelegate();

	public delegate void OnStopDelegate();
}
