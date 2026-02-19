using System;
using UnityEngine;

public class Coin : IPickup
{
	protected override void Awake()
	{
		this.character = Character.Instance;
		this.gameStats = GameStats.Instance;
		this.pivot = base.transform.GetChild(0);
		this.initialPivotPosition = this.pivot.localPosition;
		base.Awake();
		if (this.glow != null)
		{
			this.initialGlowPosition = this.glow.transform.localPosition;
		}
	}

	private void Start()
	{
		InitAssets.Instance.NotifyInitMaterials(new Renderer[]
		{
			this.meshRenderer,
			this.glow.meshRenderer
		});
	}

	public override void OnActivate()
	{
		base.OnActivate();
		this.pivot.localPosition = this.initialPivotPosition;
		if (this.glow != null)
		{
			this.glow.transform.localPosition = this.initialGlowPosition;
		}
	}

	public override void NotifyPickup(PickupParticles pickupParticles)
	{
		if (this.canPickup)
		{
			int num = 1;
			if (PlayerInfo.Instance.hasDoubleCoins)
			{
				num = 2;
			}
			this.gameStats.coins += num;
			if (Helmet.Instance.IsActive)
			{
				this.gameStats.coinsWithHelmet++;
			}
			if (this.character.IsAboveGround)
			{
				this.gameStats.coinsInAir++;
			}
			if (Flypack.Instance.isActive)
			{
				this.gameStats.coinsWithFlypack++;
			}
			if (SpringJump.Instance.isActive)
			{
				this.gameStats.coinsWithSpringJump++;
			}
			if (this.character.trackIndex == 0)
			{
				this.gameStats.coinsCollectedOnLeftTrack++;
			}
			else if (this.character.trackIndex == 1)
			{
				this.gameStats.coinsCollectedOnCenterTrack++;
			}
			else if (this.character.trackIndex == 2)
			{
				this.gameStats.coinsCollectedOnRightTrack++;
			}
			pickupParticles.PickedupCoin(this);
			base.SetVisible(false);
			this.canPickup = false;
		}
	}

	public Transform PivotTransform
	{
		get
		{
			return this.pivot;
		}
	}

	private Character character;

	private GameStats gameStats;

	private Vector3 initialGlowPosition;

	private Vector3 initialPivotPosition;

	private Transform pivot;
}
