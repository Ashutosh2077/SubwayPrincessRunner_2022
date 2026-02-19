using System;
using System.Collections.Generic;

public class WatchFreeViewSystem
{
	private WatchFreeViewSystem()
	{
		this.times = new Dictionary<string, TimeCoolDown>();
	}

	public static WatchFreeViewSystem Instance
	{
		get
		{
			if (WatchFreeViewSystem._instance == null)
			{
				WatchFreeViewSystem._instance = new WatchFreeViewSystem();
			}
			return WatchFreeViewSystem._instance;
		}
	}

	public void AddNewTime(string key, int interval)
	{
		TimeCoolDown value = new TimeCoolDown(key, interval);
		if (!this.times.ContainsKey(key))
		{
			this.times.Add(key, value);
		}
	}

	public void RemoveTime(string key)
	{
		if (this.times.ContainsKey(key))
		{
			this.times.Remove(key);
		}
	}

	public void SetFreeTime(string key)
	{
		if (this.times.ContainsKey(key))
		{
			this.times[key].SetFreeTime();
		}
	}

	public string GetCoolingDownTime(string key)
	{
		if (this.times.ContainsKey(key))
		{
			return this.times[key].GetCoolingDownTime();
		}
		return string.Empty;
	}

	public bool IsCoolingDownOver(string key)
	{
		return this.times.ContainsKey(key) && this.times[key].IsCoolingDownOver();
	}

	private static WatchFreeViewSystem _instance;

	public Dictionary<string, TimeCoolDown> times;
}
