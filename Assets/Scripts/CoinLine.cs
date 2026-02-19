using System;
using System.Collections.Generic;
using UnityEngine;

public class CoinLine : BaseO
{
	protected override void Awake()
	{
		this.coinPool = CoinPool.Instance;
		this.coinLineManager = CoinLineManager.Instance;
		this.activeCoins = new List<TrackObject>();
		base.Awake();
	}

	public override void OnActivate()
	{
		for (float num = 0f; num < this.length; num += this.coinSpacing)
		{
			TrackObject coin = this.coinPool.GetCoin("CoinLine");
			coin.transform.parent = base.transform;
			coin.transform.position = base.transform.position + base.transform.forward * num;
			coin.transform.localScale = Vector3.one;
			if (coin != null)
			{
				coin.Activate();
			}
			this.activeCoins.Add(coin);
		}
		if (base.transform.position.y > this.topLevelHight)
		{
			this.coinLineManager.AddLine(this);
			if (Character.Instance.transform.position.y > this.topLevelHight && !Game.Instance.IsInFlypackMode)
			{
				this.ToggleCoinVisibility(true);
			}
			else
			{
				this.ToggleCoinVisibility(false);
			}
		}
	}

	public override void OnDeactivate()
	{
		this.RemoveCoins();
		if (base.transform.position.y > this.topLevelHight)
		{
			this.coinLineManager.RemoveLine(this);
		}
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * this.length);
		for (float num = 0f; num < this.length; num += this.coinSpacing)
		{
			Vector3 center = base.transform.position + base.transform.forward * num;
			Gizmos.DrawSphere(center, 2f);
		}
	}

	public void RemoveCoins()
	{
		int i = 0;
		int count = this.activeCoins.Count;
		while (i < count)
		{
			this.activeCoins[i].Deactivate();
			i++;
		}
		this.coinPool.Put(this.activeCoins);
		this.activeCoins.Clear();
	}

	public void ToggleCoinVisibility(bool active)
	{
		int i = 0;
		int count = this.activeCoins.Count;
		while (i < count)
		{
			Coin component = this.activeCoins[i].GetComponent<Coin>();
			if (component == null)
			{
				break;
			}
			component.SetVisible(active);
			i++;
		}
	}

	public float length = 100f;

	public float coinSpacing = 15f;

	private List<TrackObject> activeCoins;

	private CoinLineManager coinLineManager;

	private CoinPool coinPool;

	private float topLevelHight = 70f;
}
