using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flypack : CharacterState
{
	public event Flypack.OnDeactivateTurboHeadstartDelegate OnDeactivateTurboHeadstart;

	public event Flypack.OnHidTurboHeadstartButtonsDelegate OnHidTurboHeadstartButtons;

	public void Awake()
	{
		this.game = Game.Instance;
		this.game.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(this.game.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(this.HandleOnStageMenuSequence));
		this.trackController = TrackController.Instance;
		this.character = Character.Instance;
		this.characterController = this.character.characterController;
		this.characterTransform = this.characterController.transform;
		this.characterCamera = CharacterCamera.Instance;
		this.coinsManager = this.FindObject<InAirCoinsManager>();
		this.HeadstartPickups = new GameObject[3];
		for (int i = 0; i < 3; i++)
		{
			this.HeadstartPickups[i] = UnityEngine.Object.Instantiate<GameObject>(this.HeadstartPickupPrefab);
			this.pickupLists.Add(this.HeadstartPickups[i].GetComponentsInChildren<IPickup>());
			this.HeadstartPickups[i].SetActive(false);
		}
	}

	public override IEnumerator Begin()
	{
		this.isActive = true;
		this.character.IsGrounded.Value = false;
		this.character.ResetOneway();
		if (this.powerType != PropType.headstart)
		{
			GameStats.Instance.pickedUpPowerups++;
		}
		this.Powerup = GameStats.Instance.RegisterPowerup(this.powerType);
		this.game.Attachment.PauseInFlypackMode();
		if (this.character.IsStumbling)
		{
			this.character.StopStumble();
		}
		if (SpringJump.Instance.isActive)
		{
			SpringJump.Instance.Stop();
		}
		this.NotifyOnStart(this.headStart);
		CoinLineManager.Instance.ClearTopLevelLines();
		BoundJumpCoinManager.Instance.ClearTopLevelLines();
		this.characterController.detectCollisions = false;
		this.character.characterCollider.enabled = false;
		float startY = this.characterTransform.position.y;
		float flyingDuration = 0f;
		if (!this.headStart)
		{
			flyingDuration = this.Powerup.timeLeft;
		}
		else
		{
			flyingDuration = PlayerInfo.Instance.GetPowerupDuration(this.powerType) - this.flyAheadDuration;
			this.headStartSpeed = PlayerInfo.Instance.GetPowerupSpeed(this.powerType);
		}
		this.flypackSpeed = (this.headStart ? this.headStartSpeed : (this.game.currentLevelSpeed * this.speedup));
		float flyDistance = this.flypackSpeed * flyingDuration;
		this.flyAheadDistance = this.flypackSpeed * this.flyAheadDuration;
		this.flypackDistance = this.flyAheadDistance + flyDistance;
		this.flypackDistance = this.trackController.LayJetpackPieces(this.character.z, this.flypackDistance) - this.stopBeforeLandingChunkDistance * Game.Instance.NormalizedGameSpeed;
		float extendedJetpackDuration = this.flypackDistance / this.flypackSpeed;
		this.extendedFlyDuration = extendedJetpackDuration - this.flyAheadDuration;
		flyDistance = this.extendedFlyDuration * this.flypackSpeed;
		float length = flyDistance - this.coinOffset;
		float startZ = this.character.z + this.flyAheadDistance + this.coinOffset;
		this.coinsManager.Spawn(startZ, length, this.flyHeight);
		this.game.currentSpeed = this.flypackSpeed;
		if (this.OnFlyAheadStart != null)
		{
			this.OnFlyAheadStart();
		}
		bool hasExploded = false;
		this.startTime = Time.time;
		this.characterCamera.SetCameraTransition(CameraFollowMode.FlyUp, this.flyAheadDuration);
		float ratio = 0f;
		while (ratio < 1f)
		{
			ratio = (Time.time - this.startTime) / this.flyAheadDuration;
			this.game.HandleControls();
			this.character.z += this.flypackSpeed * Time.deltaTime;
			Vector3 pivot = this.trackController.GetPosition(this.character.x, this.character.z) + Vector3.up * (startY + (this.flyHeight - startY) * Mathf.SmoothStep(0f, 1f, ratio));
			this.characterTransform.position = pivot;
			if (this.OnFlyAheadUpdate != null)
			{
				this.OnFlyAheadUpdate(ratio);
			}
			if (!hasExploded && pivot.y > this.hitCeilingZPosition && this.character.IsInsideSubway)
			{
				hasExploded = true;
				this.ceilingBrickExpolsion.gameObject.SetActive(true);
				this.ceilingBrickExpolsion.Play();
				this.character.ForceLeaveSubway();
				InitAssets.Instance.FieolnPubWmhniTmjfkwVyduit(1f);
			}
			this.characterCamera.SubwayExitCameraProgress = ratio;
			this.characterCamera.UpdatePosition(this.characterTransform.position, Quaternion.identity, Time.deltaTime, false);
			this.game.UpdateMeters();
			this.game.LayTrackChunks();
			yield return null;
		}
		if (this.OnFlyAheadEnd != null)
		{
			this.OnFlyAheadEnd();
		}
		this.character.characterCollider.enabled = true;
		this.startTime = Time.time;
		ratio = 0f;
		while (ratio < 1f)
		{
			ratio = (Time.time - this.startTime) / this.extendedFlyDuration;
			if (ratio > 0.8f && !this.activateHeadstart && this.OnHidTurboHeadstartButtons != null)
			{
				this.OnHidTurboHeadstartButtons();
			}
			if (ratio > 0.95f && this.activateHeadstart)
			{
				this.activateHeadstart = false;
				if (this.OnDeactivateTurboHeadstart != null)
				{
					this.OnDeactivateTurboHeadstart();
				}
			}
			this.game.HandleControls();
			this.character.z += this.flypackSpeed * Time.deltaTime;
			this.character.transform.position = this.trackController.GetPosition(this.character.x, this.character.z) + Vector3.up * this.flyHeight;
			this.characterCamera.UpdatePosition(this.characterTransform.position, Quaternion.identity, Time.deltaTime, false);
			this.game.UpdateMeters();
			this.game.LayTrackChunks();
			yield return null;
		}
		this.characterCamera.SetCameraTransition(CameraFollowMode.FlyDown, this.flyAfterDuration);
		SlideinPowerupHelper.Instance.HidePowerups();
		this.isActive = false;
		this.NotifyOnStop();
		this.characterController.detectCollisions = true;
		this.coinsManager.ReleaseCoins();
		if (this.headStart)
		{
			this.game.ChangeCurrentSpeed(this.game.speed.rampUpDuration * PlayerInfo.Instance.GetPowerupLandSpeed(PropType.headstart));
		}
		this.game.ChangeState(this.game.Running);
		this.game.Attachment.Resume();
		yield break;
	}

	private void HandleOnStageMenuSequence()
	{
		this.ResetPickups();
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

	private void NotifyOnStart(bool isHeadstart)
	{
		if (this.OnStart != null)
		{
			this.OnStart(isHeadstart);
		}
	}

	private void NotifyOnStop()
	{
		if (this.OnStop != null)
		{
			this.OnStop();
		}
	}

	public void PlacePickups(float z)
	{
		if (this.headStart)
		{
			float y = this.flyHeight - 5f;
			float z2 = z - 30f;
			int[] array = new int[]
			{
				-1,
				-1,
				-1
			};
			for (int i = 0; i < this.HeadstartPickups.Length; i++)
			{
				this.HeadstartPickups[i].SetActive(true);
				this.HeadstartPickups[i].transform.position = new Vector3((float)(-20 + 20 * i), y, z2);
				int num = -1;
				while (num == array[0] || num == array[1] || num == array[2])
				{
					num = UnityEngine.Random.Range(0, this.HeadstartPickups.Length);
					if (num != array[0] && num != array[1] && num != array[2])
					{
						array[i] = num;
						break;
					}
				}
				for (int j = 0; j < this.pickupLists[i].Length; j++)
				{
					if (j != num)
					{
						this.pickupLists[i][j].Deactivate();
					}
				}
				this.pickupLists[i][num].Activate();
			}
		}
	}

	private void ResetPickups()
	{
		int i = 0;
		int num = this.HeadstartPickups.Length;
		while (i < num)
		{
			this.HeadstartPickups[i].SetActive(false);
			i++;
		}
	}

	public void ResetTurboHeadstart()
	{
		this.activateHeadstart = false;
	}

	public bool ActivateTurboHeadstart
	{
		get
		{
			return this.activateHeadstart;
		}
	}

	public static Flypack Instance
	{
		get
		{
			if (Flypack.instance == null)
			{
				Flypack.instance = (UnityEngine.Object.FindObjectOfType(typeof(Flypack)) as Flypack);
			}
			return Flypack.instance;
		}
	}

	public override bool PauseActiveModifiers
	{
		get
		{
			return true;
		}
	}

	[SerializeField]
	private float speedup = 2f;

	[SerializeField]
	private float flyHeight = 95f;

	[SerializeField]
	private float hitCeilingZPosition = 10f;

	[SerializeField]
	private ParticleSystem ceilingBrickExpolsion;

	[SerializeField]
	private float coinOffset = 200f;

	[SerializeField]
	private float flyAheadDuration = 1.5f;

	[SerializeField]
	private float flyAfterDuration = 0.2f;

	[SerializeField]
	private float stopBeforeLandingChunkDistance = 50f;

	public float characterChangeTrackLength = 60f;

	[HideInInspector]
	public bool isActive;

	[HideInInspector]
	public bool headStart;

	[SerializeField]
	private GameObject HeadstartPickupPrefab;

	[HideInInspector]
	public PropType powerType;

	private ActiveProp Powerup;

	[HideInInspector]
	public InAirCoinsManager coinsManager;

	private Character character;

	private CharacterCamera characterCamera;

	private CharacterController characterController;

	private Transform characterTransform;

	private Game game;

	private static Flypack instance;

	private bool activateHeadstart;

	private float extendedFlyDuration;

	private float flyAheadDistance;

	private float headStartSpeed;

	private float flypackDistance;

	private float flypackSpeed;

	private float startTime;

	private List<IPickup[]> pickupLists = new List<IPickup[]>();

	private TrackController trackController;

	private GameObject[] HeadstartPickups;

	public Flypack.OnStartDelegate OnStart;

	public Flypack.OnStopDelegate OnStop;

	public Flypack.OnFlyAheadStartDelegate OnFlyAheadStart;

	public Flypack.OnFlyAheadUpdateDelegate OnFlyAheadUpdate;

	public Flypack.OnFlyAheadEndDelegate OnFlyAheadEnd;

	[Serializable]
	public class FlyAheadInfo
	{
		public AnimationCurve cameraMovement;
	}

	public delegate void OnActivateTurboHeadstartDelegate();

	public delegate void OnDeactivateTurboHeadstartDelegate();

	public delegate void OnFlyAheadStartDelegate();

	public delegate void OnFlyAheadUpdateDelegate(float ratio);

	public delegate void OnFlyAheadEndDelegate();

	public delegate void OnHidTurboHeadstartButtonsDelegate();

	public delegate void OnStartDelegate(bool isHeadStart);

	public delegate void OnStopDelegate();
}
