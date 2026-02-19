using System;

public class SuperShoesPickup : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (this.canPickup)
		{
			Game.Instance.Attachment.Add(Game.Instance.Attachment.SuperShoes);
			GameStats.Instance.superShoesPickups++;
			particles.PickedupPowerUp();
			base.NotifyPickup(particles);
		}
	}
}
