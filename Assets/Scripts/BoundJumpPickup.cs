using System;
using UnityEngine;

public class BoundJumpPickup : IPickup
{
	public override void NotifyPickup(PickupParticles particles)
	{
		if (this.canPickup && !Game.Instance.IsInFlypackMode)
		{
			Game.Instance.PickupBound(this.jumpHeight, this.jumpDistance, this.totalDistance, base.transform.position.y);
			particles.PickedupPowerUp();
			base.NotifyPickup(particles);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Vector3 vector = base.transform.position;
		Vector3 vector2 = vector;
		int num = 1;
		float num2 = (this.jumpDistance <= this.totalDistance) ? this.totalDistance : this.jumpDistance;
		while ((float)num < num2)
		{
			float num3 = (float)num / num2;
			vector2 = new Vector3(vector2.x, ObliqueMotion.CalcHeight(num3) * this.jumpHeight + base.transform.position.y, this.jumpDistance * num3 + base.transform.position.z);
			Gizmos.DrawLine(vector, vector2);
			vector = vector2;
			num++;
		}
	}

	public float jumpHeight = 70f;

	public float jumpDistance = 400f;

	public float totalDistance = 400f;
}
