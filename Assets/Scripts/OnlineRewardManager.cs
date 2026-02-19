using System;
using UnityEngine;

public class OnlineRewardManager : MonoBehaviour
{
	public static OnlineRewardManager Instance
	{
		get
		{
			if (OnlineRewardManager._instance == null)
			{
				OnlineRewardManager._instance = Utils.FindObject<OnlineRewardManager>();
			}
			return OnlineRewardManager._instance;
		}
	}

	private void Awake()
	{
		if (OnlineRewardManager._instance == null)
		{
			OnlineRewardManager._instance = this;
		}
	}

	public int PayedOut { get; set; }

	public OnlineZone[] Zones
	{
		get
		{
			return this.zones;
		}
	}

	[SerializeField]
	private OnlineZone[] zones;

	public const int NUMBER_OF_ONLINEZONE = 4;

	private static OnlineRewardManager _instance;
}
