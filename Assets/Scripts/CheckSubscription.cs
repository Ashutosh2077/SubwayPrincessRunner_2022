using System;
using Network;
using UnityEngine;

public class CheckSubscription : MonoBehaviour
{
	private void Start()
	{
		this.time = 0f;
		this.Check();
	}

	private void Check()
	{
		this.lastFrameNetworkReachability = Application.internetReachability;
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return;
		}
		CheckSubscription.isSubscriptionActive = false;
		RiseSdkListener.OnPaymentEvent -= this.onCheckSubscriptionResult;
		RiseSdkListener.OnPaymentEvent += this.onCheckSubscriptionResult;
		RiseSdk.Instance.HasPaid(13);
	}

	private void onCheckSubscriptionResult(RiseSdk.PaymentResult result, int id)
	{
		bool flag = false;
		CheckSubscription.isSubscriptionActive = true;
		if (result == RiseSdk.PaymentResult.Success)
		{
			if (id == 13)
			{
				PlayerInfo.Instance.hasSubscribed = true;
				ServerManager.Instance.UploadSubscription("yes");
				flag = true;
			}
		}
		else if (result == RiseSdk.PaymentResult.PaymentSystemError)
		{
			CheckSubscription.isSubscriptionActive = false;
			ServerManager.Instance.UploadSubscription("no");
		}
		if (!flag)
		{
			PlayerInfo.Instance.hasSubscribed = false;
			ServerManager.Instance.UploadSubscription("no");
			if (PlayerInfo.Instance.currentCharacter == 2)
			{
				UIModelController.Instance.SelectCharacterForPlay(Characters.CharacterType.slick, 0);
			}
		}
		RiseSdkListener.OnPaymentEvent -= this.onCheckSubscriptionResult;
	}

	private void Update()
	{
		if (CheckSubscription.isSubscriptionActive)
		{
			base.enabled = false;
			return;
		}
		if (this.lastFrameNetworkReachability == NetworkReachability.NotReachable && this.lastFrameNetworkReachability != Application.internetReachability)
		{
			this.time += this.duration;
		}
		if (this.time < this.duration)
		{
			this.time += Time.deltaTime;
		}
		else
		{
			this.Check();
			this.time = 0f;
		}
	}

	public static bool isSubscriptionActive;

	public float duration = 300f;

	private float time;

	private NetworkReachability lastFrameNetworkReachability = NetworkReachability.ReachableViaCarrierDataNetwork;
}
