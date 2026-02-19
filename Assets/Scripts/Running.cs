using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Running : CharacterState
{
	public void Awake()
	{
		this.game = Game.Instance;
		this.character = Character.Instance;
		this.characterRendering = CharacterRendering.Instance;
		this.characterTransform = this.character.transform;
		this.characterController = this.character.characterController;
		this.characterCamera = CharacterCamera.Instance;
		this.character.OnStumble += delegate(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
		{
			this.character.characterCamera.CameraShakeController.Shake();
		};
		this.character.OnLanding += this.UpdateGroundTag;
		this.game.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(this.game.OnIntroRun, new Game.OnIntroRunDelegate(this.OnIntroRun));
	}

	private void OnIntroRun()
	{
		this.intro = true;
		this.game.topMenu.AddPlayerOnStopEvent(new Action(this.IntroEnd));
	}

	private void IntroEnd()
	{
		this.intro = false;
		if (this.character.IsStumbling)
		{
			this.character.StopStumble();
		}
		this.game.GameStart();
		this.game.topMenu.RemovePlayerOnStopEvent(new Action(this.IntroEnd));
	}

	public override IEnumerator Begin()
	{
		this.inStair = false;
		this.transitionFromHeight = false;
		bool transitionWithRoll = false;
		bool fastTransition = false;
		float transitionTimeLength = 0f;
		if (this.characterTransform.position.y > 70f && this.characterTransform.position.y < 125f)
		{
			transitionTimeLength = this.transitionTimeFlypack;
			this.transitionFromHeight = true;
		}
		if (this.transitionFromPogostick)
		{
			transitionTimeLength = this.transitionTimeMax;
			this.transitionFromHeight = true;
			if (this.characterTransform.position.y < 160f)
			{
				transitionWithRoll = true;
			}
		}
		this.character.characterCollider.enabled = true;
		this.character.characterCamera.enabled = true;
		float transitionTime = 0f;
		this.character.lastGroundedY = this.character.transform.position.y;
		this.lastOnGrounded = this.characterController.isGrounded;
		this.characterController.Move(Vector3.down * 2f);
		this.tunnelStartPos = Vector3.zero;
		for (;;)
		{
			while (this.pause)
			{
				yield return null;
			}
			if (this.intro && this.characterTransform.position.z > 0f)
			{
				this.IntroEnd();
			}
			if (!this.intro && !this.game.GoingBackToCheckpoint)
			{
				this.game.LayTrackChunks();
				this.game.currentSpeed = this.game.currentLevelSpeed;
				this.game.HandleControls();
				this.character.ApplyGravity();
				this.character.MoveForward();
			}
			if (this.character.IsGrounded.Value && this.transitionFromHeight)
			{
				if (!transitionWithRoll)
				{
					transitionTimeLength -= Time.deltaTime * this.speedupTransitionTime;
				}
				else
				{
					transitionTimeLength -= Time.deltaTime * this.speedupTransitionTimeLow;
				}
			}
			Vector3 position = this.characterTransform.position;
			float y = this.character.lastGroundedY;
			if (this.game.GoingBackToCheckpoint)
			{
				y = this.character.lastGroundedY;
			}
			else if (this.inStair && !this.characterController.isGrounded)
			{
				y = position.y;
			}
			else if (this.character.InSuperShoesJump && !this.transitionFromHeight)
			{
				y = 0.5f * (this.character.lastGroundedY + position.y);
			}
			else if (!this.lastOnGrounded && this.characterController.isGrounded)
			{
				y = position.y;
			}
			else if (this.characterController.isGrounded)
			{
				y = this.character.lastGroundedY;
			}
			else if (position.y < this.character.lastGroundedY)
			{
				y = position.y;
			}
			position.y = y;
			this.lastOnGrounded = this.characterController.isGrounded;
			this.characterCamera.CurrentTunnelProgress = (this.character.z - this.tunnelStartPos.z) / this.tunnelTotalDistance;
			CharacterCamera characterCamera = this.characterCamera;
			float num = (this.character.z - this.tunnelStartPos.z) / this.tunnelTotalDistance;
			this.characterCamera.SubwayExitCameraProgress = num;
			characterCamera.SubwayEnterCameraProgress = num;
			if (this.character.IsInsideSubway)
			{
				this.characterCamera.SetMaxCameraHeight(this.character.subwayMaxY);
			}
			if (this.transitionFromHeight)
			{
				transitionTime += Time.deltaTime;
				this.characterCamera.CurrentSpringProgress = Mathf.Clamp01(transitionTime / transitionTimeLength);
				if (!transitionWithRoll)
				{
					this.transitionFromPogostick = false;
				}
				else
				{
					transitionWithRoll = false;
					this.characterCamera.SetCameraTransition(CameraFollowMode.SpringDown, 1f);
				}
				if (this.character.IsGrounded.Value || (this.character.inAirJump && this.characterController.isGrounded))
				{
					this.transitionFromHeight = false;
					this.transitionFromPogostick = false;
					transitionWithRoll = false;
					fastTransition = true;
					position.y = this.characterTransform.position.y;
					this.character.ForceLeaveSubway();
				}
				this.characterCamera.CurrentSpringProgress = Mathf.Clamp01(transitionTime / transitionTimeLength);
				this.characterCamera.UpdatePosition(position, Quaternion.identity, Time.deltaTime, false);
			}
			else if (fastTransition)
			{
				transitionTime += Time.deltaTime * 2f;
				this.characterCamera.CurrentSpringProgress = Mathf.Clamp01(transitionTime / transitionTimeLength);
				this.characterCamera.UpdatePosition(position, Quaternion.identity, Time.deltaTime, false);
				if (transitionTime >= transitionTimeLength)
				{
					fastTransition = false;
				}
			}
			else
			{
				this.characterCamera.UpdatePosition(position, Quaternion.identity, Time.deltaTime, true);
			}
			if (!this.intro)
			{
				this.character.CheckInAirJump();
				this.game.UpdateMeters();
				this.UpdateInAirRunPosition();
				this.UpdateRunStateMeters();
			}
			yield return null;
		}
		yield break;
	}

	public override void HandleCriticalHit(bool isShake = true)
	{
		if (isShake)
		{
			this.characterCamera.CameraShakeController.Shake();
		}
		this.game.WillDie();
	}

	public override void HandleDoubleTap()
	{
		if (this.intro)
		{
			return;
		}
		if (PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet) > 0 && !this.game.Attachment.IsActive(this.game.Attachment.Helmet))
		{
			this.game.Attachment.Add(this.game.Attachment.Helmet);
		}
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		if (this.intro)
		{
			return;
		}
		switch (swipeDir)
		{
		case SwipeDir.Up:
			if (this.inStair)
			{
				return;
			}
			this.character.Jump();
			break;
		case SwipeDir.Down:
			this.character.Roll();
			break;
		case SwipeDir.Left:
		{
			if (this.character.forceToOneway)
			{
				return;
			}
			Wall wall = this.character.CanWallWithSwipeDir(SwipeDir.Left);
			if (wall != null)
			{
				this.game.OnStartWall(SwipeDir.Left, wall);
				return;
			}
			this.character.ChangeTrack(-1, this.characterChangeTrackLength / this.game.currentSpeed);
			break;
		}
		case SwipeDir.Right:
		{
			if (this.character.forceToOneway)
			{
				return;
			}
			Wall wall2 = this.character.CanWallWithSwipeDir(SwipeDir.Right);
			if (wall2 != null)
			{
				this.game.OnStartWall(SwipeDir.Right, wall2);
				return;
			}
			this.character.ChangeTrack(1, this.characterChangeTrackLength / this.game.currentSpeed);
			break;
		}
		}
	}

	public void OnPause(bool pause)
	{
		this.pause = pause;
		if (pause)
		{
			IEnumerator enumerator = this.characterRendering.characterAnimation.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					AnimationState animationState = (AnimationState)obj;
					if (animationState.enabled)
					{
						Running.AnimationPlayingData animationPlayingData = new Running.AnimationPlayingData();
						animationPlayingData.state = animationState;
						animationPlayingData.time = animationState.time;
						animationPlayingData.isQueued = false;
						this.animatinData.Add(animationPlayingData);
						animationState.enabled = false;
					}
					else if (animationState.name.EndsWith("Queued Clone"))
					{
						Running.AnimationPlayingData animationPlayingData2 = new Running.AnimationPlayingData();
						animationPlayingData2.state = animationState;
						animationPlayingData2.time = 0f;
						animationPlayingData2.isQueued = true;
						this.animatinData.Add(animationPlayingData2);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			this.characterRendering.characterAnimation.Sample();
		}
		else
		{
			int i = 0;
			int count = this.animatinData.Count;
			while (i < count)
			{
				Running.AnimationPlayingData animationPlayingData3 = this.animatinData[i];
				if (!animationPlayingData3.isQueued)
				{
					animationPlayingData3.state.time = animationPlayingData3.time;
					animationPlayingData3.state.enabled = true;
				}
				else
				{
					this.characterRendering.characterAnimation.CrossFadeQueued(animationPlayingData3.state.clip.name, 0.2f);
				}
				i++;
			}
			this.animatinData.Clear();
		}
	}

	private void LandedOnTrain(Collider trainCollider)
	{
		if (this.character.helmet.IsActive && !this.GrindedTrains.Contains(trainCollider))
		{
			if (this.GrindedTrains.Count > this.GrindedTrainsBufferSize)
			{
				this.GrindedTrains.Dequeue();
			}
			this.GrindedTrains.Enqueue(trainCollider);
			GameStats gameStats = GameStats.Instance;
			gameStats.grindedTrains++;
		}
		TasksManager.Instance.PlayerDidThis(TaskTarget.LandOnTrainInRow, 1, -1);
	}

	public void StartTunnel(float tunnelLength)
	{
		this.tunnelStartPos = this.characterTransform.position;
		this.tunnelTotalDistance = tunnelLength;
		this.characterCamera.SetCameraToTunnleTransition();
	}

	public void StartUpstair(float stairLength, float slopeOfTunnel)
	{
		this.inStair = true;
		this.slopeOfTunnel = slopeOfTunnel;
		this.tunnelStartPos = this.characterTransform.position;
		this.tunnelTotalDistance = stairLength;
		this.characterCamera.SetCameraTransition(CameraFollowMode.StairUp, 1f);
	}

	public void StartDownstair(float stairLength, float slopeOfTunnel)
	{
		this.inStair = true;
		this.slopeOfTunnel = slopeOfTunnel;
		this.tunnelStartPos = this.characterTransform.position;
		this.tunnelTotalDistance = stairLength;
		this.characterCamera.SetCameraTransition(CameraFollowMode.StairDown, 1f);
	}

	public void EndUpstair()
	{
		this.inStair = false;
	}

	public void EndDownstair()
	{
		this.inStair = false;
		this.character.lastGroundedY = this.characterTransform.position.y;
	}

	public float GetStairYAtZ(float z)
	{
		return this.tunnelStartPos.y + (z - this.tunnelStartPos.z) * this.slopeOfTunnel;
	}

	private void UpdateGroundTag(Transform characterTransform)
	{
		Ray ray = new Ray(this.character.characterRoot.position, -Vector3.up);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit))
		{
			string tag = raycastHit.collider.tag;
			if (tag != null && tag != null)
			{
				if (!(tag == "Ground"))
				{
					if (!(tag == "HitTrain"))
					{
						if (!(tag == "HitMovingTrain"))
						{
							if (tag == "Station")
							{
								this.currentRunPosition = Running.RunPositions.station;
							}
						}
						else
						{
							this.currentRunPosition = Running.RunPositions.movingTrain;
							this.LandedOnTrain(raycastHit.collider);
						}
					}
					else
					{
						this.currentRunPosition = Running.RunPositions.train;
						this.LandedOnTrain(raycastHit.collider);
					}
				}
				else
				{
					this.currentRunPosition = Running.RunPositions.ground;
					TasksManager.Instance.RemoveProgressForThis(TaskTarget.LandOnTrainInRow);
				}
			}
		}
	}

	private void UpdateInAirRunPosition()
	{
		if (!this.characterController.isGrounded)
		{
			this.currentRunPosition = Running.RunPositions.air;
		}
	}

	private void UpdateRunStateMeters()
	{
		float num = this.game.currentSpeed * Time.deltaTime;
		GameStats gameStats = GameStats.Instance;
		if (this.currentRunPosition != Running.RunPositions.air)
		{
			if (this.character.trackIndex == 0)
			{
				gameStats.metersRunLeftTrack += num;
			}
			if (this.character.trackIndex == 1)
			{
				gameStats.metersRunCenterTrack += num;
			}
			if (this.character.trackIndex == 2)
			{
				gameStats.metersRunRightTrack += num;
			}
		}
		if (this.currentRunPosition == Running.RunPositions.ground)
		{
			GameStats gameStats2 = GameStats.Instance;
			gameStats2.metersRunGround += num;
		}
		if (this.currentRunPosition == Running.RunPositions.air)
		{
			GameStats gameStats3 = GameStats.Instance;
			gameStats3.metersFly += num;
		}
		if (this.currentRunPosition == Running.RunPositions.station)
		{
			GameStats gameStats4 = GameStats.Instance;
			gameStats4.metersRunStation += num;
		}
		if (this.currentRunPosition == Running.RunPositions.train)
		{
			GameStats gameStats5 = GameStats.Instance;
			gameStats5.metersRunTrain += num;
		}
		if (this.currentRunPosition == Running.RunPositions.movingTrain)
		{
			GameStats gameStats6 = GameStats.Instance;
			gameStats6.metersRunTrain += num;
		}
	}

	public static Running Instance
	{
		get
		{
			if (Running.instance == null)
			{
				Running.instance = (UnityEngine.Object.FindObjectOfType(typeof(Running)) as Running);
			}
			return Running.instance;
		}
	}

	public bool intro;

	public float transitionTimeFlypack = 1f;

	public float transitionTimeMax = 1.2f;

	public float characterChangeTrackLength = 30f;

	public float ySmoothDuration = 0.1f;

	public float speedupTransitionTime = 6f;

	public float speedupTransitionTimeLow = 10f;

	public Running.RunPositions currentRunPosition;

	private Character character;

	private CharacterCamera characterCamera;

	private CharacterController characterController;

	private CharacterRendering characterRendering;

	private Transform characterTransform;

	private Game game;

	public bool inStair;

	public bool transitionFromHeight;

	public bool transitionFromPogostick;

	private Queue<Collider> GrindedTrains = new Queue<Collider>();

	private int GrindedTrainsBufferSize = 5;

	private static Running instance;

	private bool lastOnGrounded;

	private Vector3 tunnelStartPos;

	private float slopeOfTunnel = 1f;

	private float tunnelTotalDistance = 300f;

	private bool pause;

	private List<Running.AnimationPlayingData> animatinData = new List<Running.AnimationPlayingData>();

	public enum RunPositions
	{
		ground,
		station,
		train,
		movingTrain,
		air
	}

	private class AnimationPlayingData
	{
		public AnimationState state;

		public float time;

		public bool isQueued;
	}
}
