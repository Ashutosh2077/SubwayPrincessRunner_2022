using System;
using UnityEngine;

public class MovingTrain : MovingO
{
	protected override void Awake()
	{
		base.Awake();
		Vector3 size = this.Collider.size;
		Vector3 center = this.Collider.center;
		if (this.type == MovingTrain.Type.Along)
		{
			this.Collider.size = new Vector3(size.x, size.y, size.z / (1f + this.speed));
			this.Collider.center = new Vector3(0f, center.y, ((float)this.halfLengthOfEachSection * this.trainCount + 1f) / (1f + this.speed));
		}
		if (this.trianPassClip != null)
		{
			this.trainPassSource = base.gameObject.AddComponent<AudioSource>();
			this.trainPassSource.minDistance = 0f;
			this.trainPassSource.maxDistance = 200f;
			this.trainPassSource.playOnAwake = false;
			this.trainPassSource.loop = true;
			this.trainPassSource.spatialBlend = 1f;
			this.trainPassSource.clip = this.trianPassClip;
			this.trainPassSource.rolloffMode = AudioRolloffMode.Linear;
		}
	}

	private void HandleOnPauseChange(bool pause)
	{
		if (pause)
		{
			if (this.trainPassSource != null && this.trainPassSource.isPlaying)
			{
				this.trainPassSource.Pause();
			}
			this.isPaused = true;
		}
		else
		{
			if (this.trainPassSource != null && this.isPaused)
			{
				this.trainPassSource.Play();
			}
			this.isPaused = false;
		}
	}

	protected override void Init()
	{
		if (!this.isInitialized)
		{
			this.isInitialized = true;
			this.hasStopped = false;
			this.curSpeed = this.speed;
		}
	}

	public override void OnActivate()
	{
		base.OnActivate();
		Game.Instance.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Combine(Game.Instance.OnPauseChange, new Game.OnPauseChangeDelegate(this.HandleOnPauseChange));
		this.startSound = true;
		this.hasStopped = false;
		this.curSpeed = this.speed;
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		Game instance = Game.Instance;
		instance.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Remove(instance.OnPauseChange, new Game.OnPauseChangeDelegate(this.HandleOnPauseChange));
		if (this.trainPassSource != null)
		{
			this.trainPassSource.Stop();
		}
		if (this.childFollow != null)
		{
			this.childFollow.transform.localPosition = this.child.localPosition;
		}
		this.hasStopped = false;
		this.curSpeed = this.speed;
	}

	protected override void Update()
	{
		if (this.startSound)
		{
			if (this.trainPassSource != null)
			{
				this.trainPassSource.pitch = UnityEngine.Random.Range(0.8f, 1.1f);
				this.trainPassSource.volume = UnityEngine.Random.Range(0.1f, 0.6f);
				this.trainPassSource.timeSamples = UnityEngine.Random.Range(0, this.trainPassSource.timeSamples);
				this.trainPassSource.Play();
			}
			this.startSound = false;
		}
		if (this.hasStopped)
		{
			return;
		}
		base.Update();
		if (!this.autoPilot && this.willStop && base.transform.position.z - MovingO.characterController.transform.position.z <= (float)this.distanceWhenStop)
		{
			this.hasStopped = true;
		}
	}

	private void LateUpdate()
	{
		if (this.hasStopped || this.childFollow == null)
		{
			return;
		}
		this.childFollow.transform.localPosition = this.child.localPosition;
	}

	protected override float Distance
	{
		get
		{
			return this.curTrans.position.z - MovingO.characterController.transform.position.z - (float)this.distanceWhenStop;
		}
	}

	protected override float Speed
	{
		get
		{
			if (this.willStop)
			{
				this.curSpeed = Mathf.Lerp(0f, this.speed, this.Distance / 500f);
			}
			return this.curSpeed = Mathf.Clamp(this.curSpeed, 0f, this.speed);
		}
	}

	public MovingTrain.Type type = MovingTrain.Type.Along;

	public int halfLengthOfEachSection = 30;

	public bool willStop;

	public int distanceWhenStop;

	public float trainCount = 3f;

	public AudioClip trianPassClip;

	public Transform childFollow;

	private bool hasStopped;

	private bool isInitialized;

	private bool isPaused;

	private bool startSound;

	private float curSpeed;

	private AudioSource trainPassSource;

	public enum Type
	{
		Across,
		Along
	}
}
