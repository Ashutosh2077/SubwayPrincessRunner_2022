using System;
using System.Collections.Generic;
using UnityEngine;

public class CoinLineManager : MonoBehaviour
{
	public void AddLine(CoinLine line)
	{
		this.topLevelPlaced.Add(line);
	}

	public void ClearTopLevelLines()
	{
		int i = 0;
		int count = this.topLevelPlaced.Count;
		while (i < count)
		{
			this.topLevelPlaced[i].ToggleCoinVisibility(true);
			this.topLevelPlaced[i].RemoveCoins();
			i++;
		}
		this.Reset();
	}

	public void RemoveLine(CoinLine line)
	{
		line.ToggleCoinVisibility(true);
		this.topLevelPlaced.Remove(line);
	}

	public void Reset()
	{
		this.topLevelPlaced.Clear();
	}

	public void ToggleLines(bool active)
	{
		int i = 0;
		int count = this.topLevelPlaced.Count;
		while (i < count)
		{
			this.topLevelPlaced[i].ToggleCoinVisibility(active);
			i++;
		}
	}

	public static CoinLineManager Instance
	{
		get
		{
			if (CoinLineManager.instance == null)
			{
				CoinLineManager.instance = (UnityEngine.Object.FindObjectOfType(typeof(CoinLineManager)) as CoinLineManager);
			}
			return CoinLineManager.instance;
		}
	}

	private static CoinLineManager instance;

	private List<CoinLine> topLevelPlaced = new List<CoinLine>();
}
