using System;
using UnityEngine;

public class CoinPlaceholder : BaseO
{
	protected override void Awake()
	{
		this.coinPool = CoinPool.Instance;
		base.Awake();
	}

	public override void OnActivate()
	{
		this.coin = this.coinPool.GetCoin("CoinPlaceholder");
		this.coin.transform.parent = base.transform;
		this.coin.transform.position = base.transform.position;
		this.coin.transform.localScale = Vector3.one;
		this.coin.Activate();
	}

	public override void OnDeactivate()
	{
		if (this.coin != null)
		{
			this.coin.Deactivate();
			this.coinPool.Put(this.coin);
			this.coin = null;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.position, 2f);
	}

	private TrackObject coin;

	private CoinPool coinPool;
}
