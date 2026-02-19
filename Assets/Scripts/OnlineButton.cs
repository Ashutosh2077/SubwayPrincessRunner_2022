using System;
using UnityEngine;

public class OnlineButton : MonoBehaviour
{
	private void OnEnable()
	{
		this.onlineTime = PlayerInfo.Instance.GetOnlineTime();
		int i = 0;
		this.lastIndex = 0;
		OnlineRewardManager.Instance.PayedOut = 0;
		while (i < OnlineRewardManager.Instance.Zones.Length)
		{
			if (this.onlineTime >= OnlineRewardManager.Instance.Zones[i].deadline)
			{
				this.lastIndex++;
			}
			if (this.onlineTime >= OnlineRewardManager.Instance.Zones[i].deadline && !PlayerInfo.Instance.GetOnlineZonePayedOut(i))
			{
				OnlineRewardManager.Instance.PayedOut++;
			}
			i++;
		}
	}

	private void Update()
	{
		if (PlayerInfo.Instance.AllOnlineZonePayedOut())
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.tip.enabled = (OnlineRewardManager.Instance.PayedOut > 0);
		this.onlineTime = PlayerInfo.Instance.GetOnlineTime();
		if (this.lastIndex < OnlineRewardManager.Instance.Zones.Length && this.onlineTime >= OnlineRewardManager.Instance.Zones[this.lastIndex].deadline)
		{
			OnlineRewardManager.Instance.PayedOut++;
			this.lastIndex++;
		}
	}

	[SerializeField]
	private UISprite tip;

	private int onlineTime;

	private int lastIndex;
}
