using System;
using System.Collections;
using UnityEngine;

public class Wall : BaseO
{
	protected override void Awake()
	{
		base.Awake();
		this.game = Game.Instance;
		if (this.game != null)
		{
			this.character = this.game.character;
		}
		this.cityName = base.transform.root.GetComponent<Track>().cityName;
	}

	public override void OnActivate()
	{
		this.bCollider.enabled = true;
		this.Bounds = this.bCollider.bounds;
		this.willShowMesh = false;
		this.hasShowMesh = false;
		base.enabled = true;
	}

	public override void OnDeactivate()
	{
		this.bCollider.enabled = false;
	}

	public void OnTrigger()
	{
		this.bCollider.enabled = false;
		if (!PlayerInfo.Instance.CheckWallWalkingTutorial(this.cityName) || this.willShowMesh)
		{
			return;
		}
		if (this.trackIndex != this.character.TrackIndexTarget)
		{
			return;
		}
		this.distance = this.character.z - base.transform.position.z;
		if (this.distance > this.maxDistance)
		{
			return;
		}
		this.willShowMesh = true;
	}

	private void Update()
	{
		if (!this.willShowMesh)
		{
			return;
		}
		if (this.willShowMesh && !this.hasShowMesh && this.CanSwipe())
		{
			if (this.displayText)
			{
				UISliderInController.Instance.QueueMessage(Strings.Get(this.text));
			}
			if (this.displayMesh)
			{
				base.StartCoroutine(this.ShowMesh());
			}
			PlayerInfo.Instance.AddWallWalkingTutorial(this.cityName);
			this.hasShowMesh = true;
		}
		if (!this.CanSwipe())
		{
			base.StopAllCoroutines();
			this.Mesh.SetActive(false);
			base.enabled = false;
		}
	}

	private bool CanSwipe()
	{
		return this.character.characterController.isGrounded && this.trackIndex == this.character.TrackIndexTarget;
	}

	private IEnumerator ShowMesh()
	{
		this.Mesh.transform.rotation = Quaternion.AngleAxis(this.meshDir, new Vector3(0f, 0f, 1f)) * Quaternion.Euler(0f, 180f, 0f);
		this.Mesh.SetActive(true);
		float time = this.time / this.game.NormalizedGameSpeed;
		Vector3 pos = new Vector3(0f, 0f, 20f);
		yield return base.StartCoroutine(myTween.To(time, delegate(float t)
		{
			this.Mesh.transform.localPosition = Vector3.Lerp(pos - this.Mesh.transform.up * 5f, pos + this.Mesh.transform.up * 5f, t);
			this.Mesh.GetComponent<Renderer>().material.mainTextureOffset = Vector2.Lerp(Vector2.zero, new Vector2(0f, -0.035f), t);
			if (!this.game.IsInGame.Value)
			{
				this.Mesh.transform.localPosition = new Vector3(1000f, 1000f, 0f);
			}
		}));
		this.Mesh.SetActive(false);
		base.enabled = false;
		yield break;
	}

	public BoxCollider Collider
	{
		get
		{
			return this.bCollider;
		}
	}

	public float Height
	{
		get
		{
			return this.height;
		}
	}

	public Bounds Bounds { get; private set; }

	private GameObject Mesh
	{
		get
		{
			if (this._mesh == null)
			{
				this._mesh = Camera.main.transform.Find("Arrow").gameObject;
			}
			return this._mesh;
		}
	}

	[SerializeField]
	private int trackIndex;

	[SerializeField]
	private bool displayText;

	[SerializeField]
	private string text;

	[SerializeField]
	private bool displayMesh;

	[SerializeField]
	private float meshDir;

	[SerializeField]
	private float time = 1f;

	[SerializeField]
	private float maxDistance = 100f;

	public SwipeDir swipeDir;

	[SerializeField]
	private BoxCollider bCollider;

	[SerializeField]
	private float height;

	private GameObject _mesh;

	private Game game;

	private Character character;

	private float distance;

	private bool willShowMesh;

	private bool hasShowMesh;

	private string cityName;
}
