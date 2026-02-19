using System;
using System.Collections.Generic;
using UnityEngine;

public class BoundJumpCoinManager : MonoBehaviour
{
	public void Add(BoundJumpCoins line)
	{
		this.topLevelPlaced.Add(line);
	}

	public void ClearTopLevelLines()
	{
		int i = 0;
		int count = this.topLevelPlaced.Count;
		while (i < count)
		{
			this.topLevelPlaced[i].RemoveCoins();
			i++;
		}
		this.Reset();
	}

	public void Remove(BoundJumpCoins line)
	{
		line.ToggleCoinVisibility(true);
		this.topLevelPlaced.Remove(line);
	}

	public void Reset()
	{
		this.topLevelPlaced.Clear();
	}

	public static BoundJumpCoinManager Instance
	{
		get
		{
			if (BoundJumpCoinManager.instance == null)
			{
				BoundJumpCoinManager.instance = (UnityEngine.Object.FindObjectOfType(typeof(BoundJumpCoinManager)) as BoundJumpCoinManager);
			}
			return BoundJumpCoinManager.instance;
		}
	}

	private static BoundJumpCoinManager instance;

	private List<BoundJumpCoins> topLevelPlaced = new List<BoundJumpCoins>();
}
