using System;

public class FlypackPickup : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (this.canPickup)
		{
			Game.Instance.PickupFlypack();
			particles.PickedupPowerUp();
			base.NotifyPickup(particles);
		}
	}
}
