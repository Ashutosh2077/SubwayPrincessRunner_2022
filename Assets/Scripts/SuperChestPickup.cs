using System;

public class SuperChestPickup : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (this.canPickup)
		{
			RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperChest);
			GameStats.Instance.superChestPickups++;
			particles.PickedupPowerUp();
			this.canPickup = false;
		}
	}
}
