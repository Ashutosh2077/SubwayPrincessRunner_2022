using System;
using System.Collections;
using UnityEngine;

public class SpeedUp : CharacterState
{
	public void Awake()
	{
		this.game = Game.Instance;
		this.game.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(this.game.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(this.HandleOnStageMenu));
		this.trackController = TrackController.Instance;
		this.character = Character.Instance;
		this.characterController = this.character.characterController;
		this.characterTransform = this.characterController.transform;
		this.characterCamera = CharacterCamera.Instance;
	}

	public override IEnumerator Begin()
	{
		this.isActive = true;
		this.character.characterModel.HideBlobShadow();
		this.character.inAirJump = false;
		this.character.IsGrounded.Value = false;
		if (this.character.IsStumbling)
		{
			this.character.StopStumble();
		}
		this.NotifyOnStart();
		this.extraSpeed = this.maxSpeed;
		float speed = this.game.currentSpeed + this.extraSpeed;
		this.characterCamera.SetCameraTransition(CameraFollowMode.SpeedUp, this.speedupAheadDuration);
		float t = 0f;
		while (this.character.IsInspeedup || t < this.speedupAheadDuration)
		{
			this.game.HandleControls();
			this.character.z += speed * Time.deltaTime;
			Vector3 pivot = this.trackController.GetPosition(this.character.x, this.character.z);
			this.characterTransform.position = pivot;
			this.characterCamera.UpdatePosition(this.characterTransform.position, Quaternion.identity, Time.deltaTime, false);
			this.game.UpdateMeters();
			this.game.LayTrackChunks();
			t += Time.deltaTime;
			yield return null;
		}
		this.characterCamera.SetCameraTransition(CameraFollowMode.SpeedDown, this.totalDuration - this.speedupAheadDuration);
		while (t < this.totalDuration)
		{
			float ratio = t / this.totalDuration;
			this.extraSpeed = this.speedCurve.Evaluate(ratio) * this.maxSpeed;
			speed = this.game.currentSpeed + this.extraSpeed;
			this.game.HandleControls();
			this.character.z += speed * Time.deltaTime;
			Vector3 pivot2 = this.trackController.GetPosition(this.character.x, this.character.z);
			this.characterTransform.position = pivot2;
			this.characterCamera.UpdatePosition(this.characterTransform.position, Quaternion.identity, Time.deltaTime, false);
			this.game.UpdateMeters();
			this.game.LayTrackChunks();
			t += Time.deltaTime;
			yield return null;
		}
		this.isActive = false;
		this.NotifyOnStop();
		this.game.ChangeState(this.game.Running);
		yield break;
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		if (swipeDir != SwipeDir.Left)
		{
			if (swipeDir == SwipeDir.Right)
			{
				if (this.character.forceToOneway)
				{
					return;
				}
				this.character.ChangeTrack(1, this.characterChangeTrackLength / this.game.currentSpeed);
			}
		}
		else
		{
			if (this.character.forceToOneway)
			{
				return;
			}
			this.character.ChangeTrack(-1, this.characterChangeTrackLength / this.game.currentSpeed);
		}
	}

	private void HandleOnStageMenu()
	{
		this.NotifyOnStop();
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

	public static SpeedUp Instance
	{
		get
		{
			if (SpeedUp.instance == null)
			{
				SpeedUp.instance = (UnityEngine.Object.FindObjectOfType(typeof(SpeedUp)) as SpeedUp);
			}
			return SpeedUp.instance;
		}
	}

	public override bool PauseActiveModifiers
	{
		get
		{
			return false;
		}
	}

	public bool isActive;

	public float characterChangeTrackLength = 30f;

	public AnimationCurve speedCurve;

	public float maxSpeed;

	public float speedupAheadDuration;

	public float totalDuration;

	public float overdriveDuration;

	private float extraSpeed;

	public SpeedUp.OnStartDelegate OnStart;

	public SpeedUp.OnStartDelegate OnHangtime;

	public SpeedUp.OnStopDelegate OnStop;

	private Character character;

	private CharacterCamera characterCamera;

	private CharacterController characterController;

	private Transform characterTransform;

	private TrackController trackController;

	private Game game;

	private static SpeedUp instance;

	public delegate void OnStartDelegate();

	public delegate void OnStopDelegate();
}
