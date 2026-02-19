using System;

public class CoinMagnetPickup : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (this.canPickup)
		{
			Game.Instance.Attachment.Add(Game.Instance.Attachment.CoinMagnet);
			GameStats.Instance.coinMagnetsPickups++;
			particles.PickedupPowerUp();
			base.NotifyPickup(particles);
		}
	}
}
