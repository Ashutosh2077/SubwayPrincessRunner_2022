using System;
using System.Collections;
using UnityEngine;

public class WallWalking : CharacterState
{
	private void Awake()
	{
		this.game = Game.Instance;
		this.character = Character.Instance;
		this.characterCamera = CharacterCamera.Instance;
		this.trackController = TrackController.Instance;
	}

	public override IEnumerator Begin()
	{
		this.game.ResetEnemy();
		this.character.inAirJump = false;
		this.character.IsGrounded.Value = false;
		if (this.character.IsStumbling)
		{
			this.character.StopStumble();
		}
		this.character.characterController.detectCollisions = false;
		this.IsActive = true;
		this.game.Attachment.PauseInFlypackMode();
		if (this.OnStartWall != null)
		{
			this.OnStartWall(this.lastWallDir);
		}
		Vector3 startPosition = this.character.transform.position;
		float speed = this.game.currentLevelSpeed;
		float startX = this.character.x;
		float endX = this.trackController.GetTrackX(this.character.TrackIndexTarget) + (float)((this.lastWallDir != SwipeDir.Left) ? 1 : -1) * this.deltaX;
		float trackIndexPositionBegin = this.character.trackIndexPosition;
		float newTrackIndexPosition = trackIndexPositionBegin + (float)((this.lastWallDir != SwipeDir.Left) ? 1 : -1) * this.deltaX / 20f;
		float time = 0f;
		float normalizedPosition = 0f;
		this.characterCamera.SetCameraTransition(CameraFollowMode.WallUp, this.aheadDuration);
		if (this.OnJumpAheadStart != null)
		{
			this.OnJumpAheadStart(this.lastWallDir);
		}
		while (time < this.aheadDuration)
		{
			normalizedPosition = time / this.aheadDuration;
			this.character.x = Mathf.Lerp(startX, endX, normalizedPosition);
			this.character.trackIndexPosition = Mathf.Lerp(trackIndexPositionBegin, newTrackIndexPosition, normalizedPosition);
			this.character.z += speed * Time.deltaTime;
			Vector3 pivot = this.trackController.GetPosition(this.character.x, this.character.z) + Vector3.up * (startPosition.y + this.jumpAC.Evaluate(normalizedPosition) * this.jumpHeight);
			this.character.transform.position = pivot;
			this.characterCamera.UpdatePosition(pivot, Quaternion.identity, Time.deltaTime, true);
			time += Time.deltaTime;
			this.game.UpdateMeters();
			yield return null;
		}
		if (this.OnJumpAheadEnd != null)
		{
			this.OnJumpAheadEnd(this.lastWallDir);
		}
		while (this.character.z < this.endZ)
		{
			this.game.HandleControls();
			this.character.z += speed * Time.deltaTime;
			Vector3 pivot = this.trackController.GetPosition(endX, this.character.z) + Vector3.up * (startPosition.y + this.jumpHeight);
			this.character.transform.position = pivot;
			this.characterCamera.UpdatePosition(pivot, Quaternion.identity, Time.deltaTime, true);
			this.game.UpdateMeters();
			yield return null;
		}
		this.EndWallWalking();
		this.character.ForceChangeTrack(0, this.aheadDuration * 2f, true);
		yield break;
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		switch (swipeDir)
		{
		case SwipeDir.Up:
			this.EndWallWalking();
			this.character.ForceChangeTrack(0, 0.5f, true);
			this.character.Jump();
			break;
		case SwipeDir.Down:
			this.EndWallWalking();
			this.character.ForceChangeTrack(0, 0.05f, true);
			this.character.Roll();
			break;
		case SwipeDir.Left:
			if (this.lastWallDir == SwipeDir.Right && this.character.trackIndex != 0)
			{
				this.EndWallWalking();
				this.character.Jump();
				this.character.ChangeTrack(-1, this.characterChangeTrackLength / this.game.currentSpeed);
			}
			break;
		case SwipeDir.Right:
			if (this.lastWallDir == SwipeDir.Left && this.character.trackIndex != 2)
			{
				this.EndWallWalking();
				this.character.Jump();
				this.character.ChangeTrack(1, this.characterChangeTrackLength / this.game.currentSpeed);
			}
			break;
		}
	}

	public void EndWallWalking()
	{
		this.character.characterController.detectCollisions = true;
		this.IsActive = false;
		this.game.Attachment.Resume();
		if (this.OnLeaveWall != null)
		{
			this.OnLeaveWall(this.lastWallDir);
		}
		this.characterCamera.SetCameraTransition(CameraFollowMode.WallDown, this.aheadDuration);
		this.character.verticalSpeed = this.character.CalculateJumpVerticalSpeed(0f);
		this.character.characterController.Move(Vector3.up * 0.1f);
		this.game.ChangeState(this.game.Running);
		this.endZ = -1f;
		this.character.WallEndWithSwipeDir(this.lastWallDir);
	}

	public bool CanNotLeaveWall(SwipeDir dir)
	{
		return dir == this.lastWallDir;
	}

	public static WallWalking Instance
	{
		get
		{
			if (WallWalking.instance == null)
			{
				WallWalking.instance = (UnityEngine.Object.FindObjectOfType(typeof(WallWalking)) as WallWalking);
			}
			return WallWalking.instance;
		}
	}

	public void SetData(SwipeDir dir, float height, float endZ)
	{
		this.lastWallDir = dir;
		this.jumpHeight = height;
		this.endZ = endZ;
	}

	public bool IsActive { get; private set; }

	private Character character;

	private CharacterCamera characterCamera;

	private Game game;

	private TrackController trackController;

	[SerializeField]
	private float deltaX = 7.2f;

	private float jumpHeight;

	private SwipeDir lastWallDir;

	private float endZ;

	[SerializeField]
	private float aheadDuration;

	[SerializeField]
	private AnimationCurve jumpAC;

	[SerializeField]
	private float characterChangeTrackLength = 30f;

	private static WallWalking instance;

	public WallWalking.OnStartWallDelegate OnStartWall;

	public WallWalking.OnLeaveWallDelegate OnLeaveWall;

	public WallWalking.OnJumpAheadStartDelegate OnJumpAheadStart;

	public WallWalking.OnJumpAheadEndDelegate OnJumpAheadEnd;

	public delegate void OnStartWallDelegate(SwipeDir dir);

	public delegate void OnLeaveWallDelegate(SwipeDir dir);

	public delegate void OnJumpAheadStartDelegate(SwipeDir dir);

	public delegate void OnJumpAheadEndDelegate(SwipeDir dir);
}
