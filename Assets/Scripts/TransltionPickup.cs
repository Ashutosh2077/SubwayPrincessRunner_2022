using System;

public class TransltionPickup : IPickup
{
	public override void NotifyPickup(PickupParticles pickupParticles)
	{
		if (this.canPickup)
		{
			Game.Instance.PickupTransition();
			base.NotifyPickup(pickupParticles);
		}
	}
}
