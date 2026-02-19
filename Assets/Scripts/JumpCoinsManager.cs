using System;
using System.Collections.Generic;
using UnityEngine;

public class JumpCoinsManager
{
	public JumpCoinsManager()
	{
		this.trackController = TrackController.Instance;
		this.coinPool = CoinPool.Instance;
	}

	public static JumpCoinsManager Instance
	{
		get
		{
			if (JumpCoinsManager._instance == null)
			{
				JumpCoinsManager._instance = new JumpCoinsManager();
			}
			return JumpCoinsManager._instance;
		}
	}

	private void MoveCoin(Vector3 position)
	{
		TrackObject coin = this.coinPool.GetCoin("JumpCoinsManage");
		coin.transform.position = position;
		coin.transform.localScale = Vector3.one;
		coin.Activate();
		this.coins.Add(coin);
	}

	public void placeRow(float z, float height)
	{
		this.MoveCoin(this.trackController.GetPosition(0f, z) + Vector3.up * height);
		this.MoveCoin(this.trackController.GetPosition(20f, z) + Vector3.up * height);
		this.MoveCoin(this.trackController.GetPosition(40f, z) + Vector3.up * height);
	}

	public void ReleaseCoins()
	{
		int i = 0;
		int count = this.coins.Count;
		while (i < count)
		{
			this.coins[i].Deactivate();
			i++;
		}
		this.coinPool.Put(this.coins);
		this.coins.Clear();
	}

	private CoinPool coinPool;

	private List<TrackObject> coins = new List<TrackObject>();

	private TrackController trackController;

	private static JumpCoinsManager _instance;
}
