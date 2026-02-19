using System;

public class DoubleScoreMultiplierPickup : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (this.canPickup)
		{
			Game.Instance.Attachment.Add(Game.Instance.Attachment.DoubleScoreMultiplier);
			GameStats.Instance.doubleMultiplierPickups++;
			particles.PickedupPowerUp();
			base.NotifyPickup(particles);
		}
	}
}
