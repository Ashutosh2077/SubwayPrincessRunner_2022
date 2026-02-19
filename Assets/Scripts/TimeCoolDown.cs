using System;
using UnityEngine;

public class TimeCoolDown
{
	public TimeCoolDown(string key, int interval)
	{
		this.prefsKey = key;
		this.interval = interval;
		this.NextFreeDTime = DateTime.Parse(this.NextFreeTimeStr);
	}

	public string NextFreeTimeStr
	{
		get
		{
			return PlayerPrefs.GetString(this.prefsKey, DateTime.UtcNow.ToString());
		}
		set
		{
			PlayerPrefs.SetString(this.prefsKey, value);
		}
	}

	public bool IsCoolingDownOver()
	{
		return DateTime.Compare(DateTime.UtcNow, this.NextFreeDTime) > 0;
	}

	public string GetCoolingDownTime()
	{
		TimeSpan timeSpan = this.NextFreeDTime - DateTime.UtcNow;
		return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
	}

	public void SetFreeTime()
	{
		this.NextFreeDTime = DateTime.UtcNow.AddSeconds((double)this.interval);
		this.NextFreeTimeStr = this.NextFreeDTime.ToString();
	}

	public string prefsKey;

	public int interval;

	private DateTime NextFreeDTime;
}
