using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SpringJump : CharacterState
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
		this.characterCameraTransform = this.characterCamera.transform;
		this.coinsManager = JumpCoinsManager.Instance;
		this.coinLineManager = CoinLineManager.Instance;
		if (this.mysteryBoxPrefab != null)
		{
			this.mysteryBoxPrefab = UnityEngine.Object.Instantiate<GameObject>(this.mysteryBoxPrefab, new Vector3(0f, -200f, 0f), Quaternion.identity);
			this.list = this.mysteryBoxPrefab.GetComponentsInChildren<IPickup>();
			int num = 0;
			int i = 0;
			int num2 = this.weightList.Length;
			while (i < num2)
			{
				num += Upgrades.upgrades[this.weightList[i].type].spawnProbability;
				i++;
			}
			this.weightedList = new IPickup[num];
			int num3 = 0;
			for (int j = 0; j < this.weightList.Length; j++)
			{
				for (int k = 0; k < this.list.Length; k++)
				{
					if (!(this.weightList[j].pickupName != this.list[k].gameObject.name))
					{
						int l = 0;
						Upgrade upgrade = Upgrades.upgrades[this.weightList[j].type];
						while (l < upgrade.spawnProbability)
						{
							this.weightedList[num3] = this.list[k];
							num3++;
							l++;
						}
					}
				}
			}
			this.mysteryBoxPrefab.SetActive(false);
		}
	}

	public override IEnumerator Begin()
	{
		this.isActive = true;
		this.coinLineManager.ToggleLines(true);
		this.character.characterModel.HideBlobShadow();
		this.game.ResetEnemy();
		this.character.inAirJump = false;
		this.character.IsGrounded.Value = false;
		GameStats instance = GameStats.Instance;
		instance.pickedUpPowerups++;
		this.game.Attachment.PauseInFlypackMode();
		if (this.character.IsStumbling)
		{
			this.character.StopStumble();
		}
		this.characterController.detectCollisions = false;
		this.NotifyOnStart();
		float coinOffsetZPosition = this.jumpDistance / (float)this.rows;
		for (int i = 0; i < this.endRowPosition; i++)
		{
			if (i > this.startRowPosition)
			{
				Vector3 vector = new Vector3(0f, this.jumpCurve.curve.Evaluate((float)i / (float)this.rows) * this.jumpHeight, this.character.z + (float)i * coinOffsetZPosition);
				this.coinsManager.placeRow(vector.z, vector.y);
			}
		}
		int randomX = UnityEngine.Random.Range(0, 3);
		float xPosition = 0f;
		if (randomX == 0)
		{
			xPosition = -20f;
		}
		else if (randomX == 1)
		{
			xPosition = 0f;
		}
		else
		{
			xPosition = 20f;
		}
		Vector3 startPosition = this.characterTransform.position;
		Vector3 endPosition = startPosition + Vector3.forward * this.jumpDistance;
		base.StartCoroutine(this.RemovePickups(endPosition.z));
		if (this.mysteryBoxPrefab != null && this.willShowPickup)
		{
			this.mysteryBoxPrefab.transform.position = new Vector3(xPosition, this.jumpCurve.curve.Evaluate(1f) * this.jumpHeight, endPosition.z + 20f);
			this.mysteryBoxPrefab.SetActive(true);
			int index = UnityEngine.Random.Range(0, this.weightedList.Length);
			IPickup pickup = this.weightedList.ElementAt(index);
			for (int j = 0; j < this.list.Length; j++)
			{
				IPickup pickup2 = this.list[j];
				pickup2.Deactivate();
			}
			pickup.Activate();
		}
		float speed = this.game.currentSpeed;
		this.characterCamera.SetCameraTransition(CameraFollowMode.SpringUp, 1f);
		this.characterCamera.SetStartPositionY(this.characterCameraTransform.position.y);
		while (this.character.z < endPosition.z)
		{
			this.game.HandleControls();
			this.character.z += speed * Time.deltaTime;
			float normalizedPosition = (this.character.z - startPosition.z) / this.jumpDistance;
			Vector3 pivot = this.trackController.GetPosition(this.character.x, this.character.z) + Vector3.up * this.jumpCurve.curve.Evaluate(normalizedPosition) * this.jumpHeight;
			if (normalizedPosition <= this.fadeInPosition)
			{
				pivot.y = Mathf.Lerp(startPosition.y, pivot.y, normalizedPosition / this.fadeInPosition);
			}
			if (normalizedPosition > this.hangtimePosition && !this.reachedHangtime)
			{
				this.NotifyHangtime();
				this.reachedHangtime = true;
			}
			this.characterTransform.position = pivot;
			this.characterCamera.CurrentSpringProgress = normalizedPosition;
			this.characterCamera.UpdatePosition(pivot, Quaternion.identity, Time.deltaTime, false);
			this.game.UpdateMeters();
			this.game.LayTrackChunks();
			yield return null;
		}
		this.EndSpring();
		yield break;
	}

	private void EndSpring()
	{
		this.isActive = false;
		this.NotifyOnStop();
		this.characterController.detectCollisions = true;
		Running.Instance.transitionFromPogostick = true;
		this.game.ChangeState(this.game.Running);
		this.character.verticalSpeed = this.character.CalculateJumpVerticalSpeed(0f);
		this.game.Attachment.Resume();
		this.reachedHangtime = false;
	}

	private void HandleOnStageMenu()
	{
		if (this.mysteryBoxPrefab != null)
		{
			this.mysteryBoxPrefab.SetActive(false);
		}
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		if (swipeDir != SwipeDir.Down)
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
		else
		{
			this.EndSpring();
			this.character.Roll();
		}
	}

	private void NotifyHangtime()
	{
		if (this.OnHangtime != null)
		{
			this.OnHangtime();
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

	private IEnumerator RemovePickups(float endPosition)
	{
		while (this.character.z < endPosition + 100f)
		{
			yield return null;
		}
		this.coinsManager.ReleaseCoins();
		if (this.mysteryBoxPrefab != null)
		{
			this.mysteryBoxPrefab.SetActive(false);
		}
		this.ResetPickup();
		yield break;
	}

	private void ResetPickup()
	{
		int i = 0;
		int num = this.list.Length;
		while (i < num)
		{
			this.list[i].gameObject.SetActive(true);
			i++;
		}
	}

	public void Stop()
	{
		this.isActive = false;
		this.NotifyOnStop();
	}

	public static SpringJump Instance
	{
		get
		{
			if (SpringJump.instance == null)
			{
				SpringJump.instance = (UnityEngine.Object.FindObjectOfType(typeof(SpringJump)) as SpringJump);
			}
			return SpringJump.instance;
		}
	}

	public override bool PauseActiveModifiers
	{
		get
		{
			return true;
		}
	}

	public bool WillShowPickup
	{
		set
		{
			this.willShowPickup = value;
		}
	}

	[SerializeField]
	private float jumpHeight = 95f;

	[SerializeField]
	private SpringJump.MovementCurve jumpCurve;

	[SerializeField]
	private float jumpDistance = 800f;

	[SerializeField]
	private float characterChangeTrackLength = 60f;

	[SerializeField]
	private float fadeInPosition = 0.1f;

	[SerializeField]
	private float hangtimePosition = 0.5f;

	[HideInInspector]
	public JumpCoinsManager coinsManager;

	public bool isActive;

	[HideInInspector]
	public PropType powerType = PropType.springJump;

	private ActiveProp Powerup;

	[SerializeField]
	private int rows = 5;

	[SerializeField]
	private int startRowPosition = 1;

	[SerializeField]
	private int endRowPosition = 1;

	[SerializeField]
	private GameObject mysteryBoxPrefab;

	[SerializeField]
	private SpringJump.pickupWeight[] weightList;

	public SpringJump.OnStartDelegate OnStart;

	public SpringJump.OnStartDelegate OnHangtime;

	public SpringJump.OnStopDelegate OnStop;

	private Character character;

	private CharacterCamera characterCamera;

	private Transform characterCameraTransform;

	private CharacterController characterController;

	private Transform characterTransform;

	private CoinLineManager coinLineManager;

	private Game game;

	private static SpringJump instance;

	private IPickup[] list;

	private bool reachedHangtime;

	private TrackController trackController;

	private IPickup[] weightedList;

	private bool willShowPickup = true;

	[Serializable]
	public class MovementCurve
	{
		public AnimationCurve curve;
	}

	public delegate void OnReachedTopDelegate();

	public delegate void OnStartDelegate();

	public delegate void OnStopDelegate();

	[Serializable]
	public class pickupWeight
	{
		public string pickupName;

		public PropType type;
	}
}
