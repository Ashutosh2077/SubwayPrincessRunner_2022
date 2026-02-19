using System;
using UnityEngine;

public class IPickup : BaseO
{
	public void SetVisible(bool visible)
	{
		if (this.meshRenderer)
		{
			this.meshRenderer.enabled = visible;
		}
		if (this.glow)
		{
			this.glow.SetVisible(visible);
		}
	}

	public virtual void Activate()
	{
		base.gameObject.SetActive(true);
		if (this.pickupCollider)
		{
			this.pickupCollider.enabled = true;
		}
		this.SetVisible(true);
		this.canPickup = true;
	}

	public virtual void Deactivate()
	{
		this.SetVisible(false);
		if (this.pickupCollider)
		{
			this.pickupCollider.enabled = false;
		}
		this.canPickup = false;
		base.gameObject.SetActive(false);
	}

	public override void OnActivate()
	{
		if (this.pickupCollider)
		{
			this.pickupCollider.enabled = true;
		}
		this.SetVisible(true);
		this.canPickup = true;
	}

	public override void OnDeactivate()
	{
		this.SetVisible(false);
		if (this.pickupCollider)
		{
			this.pickupCollider.enabled = false;
		}
		this.canPickup = false;
	}

	public virtual void NotifyPickup(PickupParticles pickupParticles)
	{
		if (this.pickupCollider)
		{
			this.pickupCollider.enabled = false;
		}
		this.SetVisible(false);
		this.canPickup = false;
	}

	public Collider pickupCollider;

	public MeshRenderer meshRenderer;

	public Glow glow;

	protected bool canPickup;
}
