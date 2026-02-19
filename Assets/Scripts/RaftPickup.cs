using System;

public class RaftPickup : IPickup
{
	public override void NotifyPickup(PickupParticles pickupParticles)
	{
		if (this.canPickup)
		{
			Game.Instance.Attachment.Add(Game.Instance.Raft);
			if (this.pickupCollider)
			{
				this.pickupCollider.enabled = false;
			}
			this.canPickup = false;
		}
	}
}
