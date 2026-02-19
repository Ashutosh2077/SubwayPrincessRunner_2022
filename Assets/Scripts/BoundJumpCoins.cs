using System;
using System.Collections.Generic;
using UnityEngine;

public class BoundJumpCoins : BaseO
{
	protected override void Awake()
	{
		this.pool = CoinPool.Instance;
		if (this.pool == null)
		{
			UnityEngine.Debug.LogWarning("The PoolManager has not a " + this.poolName + " spawnPool!!!");
			return;
		}
		this.boundJumpCoinManager = BoundJumpCoinManager.Instance;
		base.Awake();
	}

	public override void OnActivate()
	{
		Vector3 zero = Vector3.zero;
		int i = this.startRowPosition;
		while (i <= this.endRowPosition)
		{
			if (i == this.randomPickupPosition)
			{
				i++;
			}
			else
			{
				float num = (float)i / (float)this.rows;
				zero = new Vector3(this.coinX.Evaluate(num) * this.deltaX, ObliqueMotion.CalcHeight(num) * this.jumpHeight + base.transform.localPosition.y, this.jumpDistance * num);
				TrackObject coin = this.pool.GetCoin("BoundJumpCoins");
				Transform transform = coin.transform;
				transform.parent = base.transform;
				transform.localPosition = zero;
				transform.localScale = Vector3.one;
				coin.Activate();
				this.activeobjs.Add(coin);
				i++;
			}
		}
		this.boundJumpCoinManager.Add(this);
	}

	public override void OnDeactivate()
	{
		this.boundJumpCoinManager.Remove(this);
		this.RemoveCoins();
	}

	public void RemoveCoins()
	{
		int i = 0;
		int count = this.activeobjs.Count;
		while (i < count)
		{
			this.activeobjs[i].Deactivate();
			i++;
		}
		this.pool.Put(this.activeobjs);
		this.activeobjs.Clear();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		float num = (float)this.startRowPosition;
		Vector3 zero = Vector3.zero;
		while (num <= (float)this.endRowPosition)
		{
			if (num == (float)this.randomPickupPosition)
			{
				num += 1f;
			}
			else
			{
				float num2 = num / (float)this.rows;
				zero = new Vector3(base.transform.position.x + this.coinX.Evaluate(num2) * this.deltaX, ObliqueMotion.CalcHeight(num2) * this.jumpHeight + base.transform.position.y, this.jumpDistance * num2 + base.transform.position.z);
				Gizmos.DrawSphere(zero, 2f);
				num += 1f;
			}
		}
	}

	public void ToggleCoinVisibility(bool active)
	{
		int i = 0;
		int count = this.activeobjs.Count;
		while (i < count)
		{
			if (!active)
			{
				this.activeobjs[i].Deactivate();
			}
			else
			{
				this.activeobjs[i].Activate();
			}
			i++;
		}
	}

	public string poolName = "Coin";

	private CoinPool pool;

	public float deltaX;

	public int rows = 18;

	public int startRowPosition = 3;

	public int endRowPosition = 10;

	public int randomPickupPosition;

	public AnimationCurve coinX;

	public float jumpHeight = 70f;

	public float jumpDistance = 400f;

	private List<TrackObject> activeobjs = new List<TrackObject>();

	private BoundJumpCoinManager boundJumpCoinManager;
}
