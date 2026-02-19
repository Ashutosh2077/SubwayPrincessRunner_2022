using System;
using UnityEngine;

public class TriggerOMoveOCrocodile : TriggerO
{
	protected override void Awake()
	{
		base.Awake();
		this.child = base.transform.GetChild(0);
		this.originLPosZForChild = this.child.localPosition.z;
		this.character = Character.Instance;
	}

	public override void OnActivate()
	{
		base.OnActivate();
		this.child.localPosition = new Vector3(0f, 0f, this.originLPosZForChild);
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		base.enabled = false;
	}

	public override void TriggerOnEnter(Collider collider)
	{
		if (collider.gameObject.layer == Layers.Instance.Character)
		{
			if (this.transitionClip != null)
			{
				this.anim.CrossFade(this.transitionClip, 0.1f);
				this.anim.CrossFadeQueued(this.triggerClip, 0.1f);
				this.delayTime = this.anim[this.transitionClip].length;
			}
			else
			{
				this.anim.CrossFade(this.triggerClip, 0.1f);
				this.delayTime = 0f;
			}
			this.time = 0f;
			this.moveFirstUpdate = true;
			base.enabled = true;
		}
	}

	private void Update()
	{
		if (this.time < this.delayTime)
		{
			this.time += Time.deltaTime;
			return;
		}
		if (this.moveFirstUpdate)
		{
			if (this.character.z >= base.transform.position.z)
			{
				UnityEngine.Debug.LogError("Character has already exceeded this Object.");
				base.enabled = false;
				return;
			}
			this.rate = this.originLPosZForChild / (base.transform.position.z - this.character.z);
			this.moveFirstUpdate = false;
		}
		this.child.localPosition = new Vector3(0f, 0f, (base.transform.position.z - this.character.z) * this.rate);
	}

	[SerializeField]
	private string transitionClip;

	[SerializeField]
	private string triggerClip;

	private Transform child;

	private float delayTime;

	private float time;

	private float originLPosZForChild;

	private float rate;

	private Character character;

	private bool moveFirstUpdate;
}
