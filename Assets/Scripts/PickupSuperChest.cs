using System;

public class PickupSuperChest : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (this.canPickup)
		{
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperChest);
			GameStats instance = GameStats.Instance;
			instance.superChestPickups++;
			particles.PickedupPowerUp();
			base.NotifyPickup(particles);
		}
	}
}
