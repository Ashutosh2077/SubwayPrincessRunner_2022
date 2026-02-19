using System;
using UnityEngine;

public class SpringJumpPickup : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (this.canPickup && !Game.Instance.IsInFlypackMode)
		{
			Game.Instance.PickupPogostick(this.willShowPickup);
			particles.PickedupPowerUp();
			base.NotifyPickup(particles);
		}
	}

	[SerializeField]
	private bool willShowPickup;
}
