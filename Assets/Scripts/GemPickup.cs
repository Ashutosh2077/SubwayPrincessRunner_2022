using System;

public class GemPickup : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (this.canPickup)
		{
			PlayerInfo.Instance.AddSaveGemToUnlock();
			GameStats.Instance.saveMeSymbolPickup++;
			particles.PickedupPowerUp();
			GameStats.Instance.AddScoreForPickup(PropType.gem);
			Statistics stats= PlayerInfo.Instance.stats;
			(stats )[Stat.KeysCollected] = stats[Stat.KeysCollected] + 1;
			base.NotifyPickup(particles);
		}
	}
}
