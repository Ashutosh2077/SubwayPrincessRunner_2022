using System;

public class ChestPickup : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (this.canPickup)
		{
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.Chest);
			GameStats.Instance.chestPickups++;
			particles.PickedupPowerUp();
			GameStats.Instance.AddScoreForPickup(PropType.chest);
			base.NotifyPickup(particles);
		}
	}
}
