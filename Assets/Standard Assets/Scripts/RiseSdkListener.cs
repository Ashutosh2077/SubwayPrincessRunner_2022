using System;
using UnityEngine;

public class RiseSdkListener : MonoBehaviour
{
	public static event Action<RiseSdk.PaymentResult, int> OnPaymentEvent;

	public static event Action<RiseSdk.SnsEventType, int> OnSNSEvent;

	public static event Action<bool, int, string> OnCacheUrlResult;

	public static event Action<bool, bool, string, string> OnLeaderBoardEvent;

	public static event Action<int, bool, string> OnReceiveServerResult;

	public static event Action<string> OnReceivePaymentsPrice;

	public static event Action<string> OnReceiveServerExtra;

	public static event Action<string> OnReceiveNotificationData;

	public static event Action<RiseSdk.AdEventType, int, string, int> OnAdEvent;

	public static event Action OnResumeAdEvent;

	public static RiseSdkListener Instance
	{
		get
		{
			if (!RiseSdkListener._instance)
			{
				RiseSdkListener._instance = (UnityEngine.Object.FindObjectOfType(typeof(RiseSdkListener)) as RiseSdkListener);
				if (!RiseSdkListener._instance)
				{
					GameObject gameObject = new GameObject("RiseSdkListener");
					RiseSdkListener._instance = gameObject.AddComponent<RiseSdkListener>();
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
			}
			return RiseSdkListener._instance;
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			RiseSdk.Instance.OnPause();
		}
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		if (focusStatus)
		{
			RiseSdk.Instance.OnResume();
		}
	}

	private void OnApplicationQuit()
	{
		RiseSdk.Instance.OnStop();
		RiseSdk.Instance.OnDestroy();
	}

	private void Awake()
	{
		RiseSdk.Instance.OnStart();
	}

	public void OnResumeAd()
	{
		if (RiseSdkListener.OnResumeAdEvent != null)
		{
			RiseSdkListener.OnResumeAdEvent();
		}
	}

	public void onPaymentSuccess(string billId)
	{
		if (RiseSdkListener.OnPaymentEvent != null && RiseSdkListener.OnPaymentEvent.GetInvocationList().Length > 0)
		{
			int arg = int.Parse(billId);
			RiseSdkListener.OnPaymentEvent(RiseSdk.PaymentResult.Success, arg);
		}
	}

	public void onPaymentFail(string billId)
	{
		if (RiseSdkListener.OnPaymentEvent != null && RiseSdkListener.OnPaymentEvent.GetInvocationList().Length > 0)
		{
			int arg = int.Parse(billId);
			RiseSdkListener.OnPaymentEvent(RiseSdk.PaymentResult.Failed, arg);
		}
	}

	public void onPaymentCanceled(string billId)
	{
		if (RiseSdkListener.OnPaymentEvent != null && RiseSdkListener.OnPaymentEvent.GetInvocationList().Length > 0)
		{
			int arg = int.Parse(billId);
			RiseSdkListener.OnPaymentEvent(RiseSdk.PaymentResult.Cancel, arg);
		}
	}

	public void onPaymentSystemError(string data)
	{
		if (RiseSdkListener.OnPaymentEvent != null && RiseSdkListener.OnPaymentEvent.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnPaymentEvent(RiseSdk.PaymentResult.PaymentSystemError, -1);
		}
	}

	public void onReceiveBillPrices(string data)
	{
		if (RiseSdkListener.OnReceivePaymentsPrice != null && RiseSdkListener.OnReceivePaymentsPrice.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnReceivePaymentsPrice(data);
		}
	}

	public void onPaymentSystemValid(string dummy)
	{
		RiseSdk.Instance.SetPaymentSystemValid(true);
	}

	public void onReceiveLoginResult(string result)
	{
		if (RiseSdkListener.OnSNSEvent != null && RiseSdkListener.OnSNSEvent.GetInvocationList().Length > 0)
		{
			int num = int.Parse(result);
			RiseSdkListener.OnSNSEvent((num != 0) ? RiseSdk.SnsEventType.LoginFailed : RiseSdk.SnsEventType.LoginSuccess, 0);
		}
	}

	public void onReceiveInviteResult(string result)
	{
		if (RiseSdkListener.OnSNSEvent != null && RiseSdkListener.OnSNSEvent.GetInvocationList().Length > 0)
		{
			int num = int.Parse(result);
			RiseSdkListener.OnSNSEvent((num != 0) ? RiseSdk.SnsEventType.InviteFailed : RiseSdk.SnsEventType.InviteSuccess, 0);
		}
	}

	public void onReceiveLikeResult(string result)
	{
		if (RiseSdkListener.OnSNSEvent != null && RiseSdkListener.OnSNSEvent.GetInvocationList().Length > 0)
		{
			int num = int.Parse(result);
			RiseSdkListener.OnSNSEvent((num != 0) ? RiseSdk.SnsEventType.LikeFailed : RiseSdk.SnsEventType.LikeSuccess, 0);
		}
	}

	public void onReceiveChallengeResult(string result)
	{
		if (RiseSdkListener.OnSNSEvent != null && RiseSdkListener.OnSNSEvent.GetInvocationList().Length > 0)
		{
			int num = int.Parse(result);
			RiseSdkListener.OnSNSEvent((num <= 0) ? RiseSdk.SnsEventType.ChallengeFailed : RiseSdk.SnsEventType.ChallengeSuccess, num);
		}
	}

	public void onSubmitSuccess(string leaderBoardTag)
	{
		if (RiseSdkListener.OnLeaderBoardEvent != null && RiseSdkListener.OnLeaderBoardEvent.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnLeaderBoardEvent(true, true, leaderBoardTag, string.Empty);
		}
	}

	public void onSubmitFailure(string leaderBoardTag)
	{
		if (RiseSdkListener.OnLeaderBoardEvent != null && RiseSdkListener.OnLeaderBoardEvent.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnLeaderBoardEvent(true, false, leaderBoardTag, string.Empty);
		}
	}

	public void onLoadSuccess(string data)
	{
		if (RiseSdkListener.OnLeaderBoardEvent != null && RiseSdkListener.OnLeaderBoardEvent.GetInvocationList().Length > 0)
		{
			string[] array = data.Split(new char[]
			{
				'|'
			});
			RiseSdkListener.OnLeaderBoardEvent(false, true, array[0], array[1]);
		}
	}

	public void onLoadFailure(string leaderBoardTag)
	{
		if (RiseSdkListener.OnLeaderBoardEvent != null && RiseSdkListener.OnLeaderBoardEvent.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnLeaderBoardEvent(false, false, leaderBoardTag, string.Empty);
		}
	}

	public void onServerResult(string data)
	{
		if (RiseSdkListener.OnReceiveServerResult != null && RiseSdkListener.OnReceiveServerResult.GetInvocationList().Length > 0)
		{
			string[] array = data.Split(new char[]
			{
				'|'
			});
			int arg = int.Parse(array[0]);
			bool arg2 = int.Parse(array[1]) == 0;
			RiseSdkListener.OnReceiveServerResult(arg, arg2, array[2]);
		}
	}

	public void onCacheUrlResult(string data)
	{
		if (RiseSdkListener.OnCacheUrlResult != null && RiseSdkListener.OnCacheUrlResult.GetInvocationList().Length > 0)
		{
			string[] array = data.Split(new char[]
			{
				'|'
			});
			int arg = int.Parse(array[0]);
			bool flag = int.Parse(array[1]) == 0;
			if (flag)
			{
				RiseSdkListener.OnCacheUrlResult(true, arg, array[2]);
			}
			else
			{
				RiseSdkListener.OnCacheUrlResult(false, arg, string.Empty);
			}
		}
	}

	public void onReceiveServerExtra(string data)
	{
		if (RiseSdkListener.OnReceiveServerExtra != null && RiseSdkListener.OnReceiveServerExtra.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnReceiveServerExtra(data);
		}
	}

	public void onReceiveNotificationData(string data)
	{
		if (RiseSdkListener.OnReceiveNotificationData != null && RiseSdkListener.OnReceiveNotificationData.GetInvocationList().Length > 0)
		{
			RiseSdkListener.OnReceiveNotificationData(data);
		}
	}

	public void onReceiveReward(string data)
	{
		if (RiseSdkListener.OnAdEvent != null && RiseSdkListener.OnAdEvent.GetInvocationList().Length > 0)
		{
			bool flag = false;
			int arg = -1;
			string arg2 = "Default";
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(new char[]
				{
					'|'
				});
				if (array != null && array.Length > 1)
				{
					flag = (int.Parse(array[0]) == 0);
					arg = int.Parse(array[1]);
					if (array.Length > 2)
					{
						arg2 = array[2];
						if (array.Length > 3)
						{
							int num = (int.Parse(array[3]) != 0) ? 0 : 1;
						}
					}
				}
			}
			if (flag)
			{
				RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.RewardAdShowFinished, arg, arg2, 2);
			}
			else
			{
				RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.RewardAdShowFailed, arg, arg2, 2);
			}
		}
	}

	public void onFullAdClosed(string data)
	{
		if (RiseSdkListener.OnAdEvent != null && RiseSdkListener.OnAdEvent.GetInvocationList().Length > 0)
		{
			string arg = "Default";
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(new char[]
				{
					'|'
				});
				if (array != null && array.Length > 0)
				{
					arg = array[0];
				}
			}
			RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.FullAdClosed, -1, arg, 1);
		}
	}

	public void onFullAdClicked(string data)
	{
		if (RiseSdkListener.OnAdEvent != null && RiseSdkListener.OnAdEvent.GetInvocationList().Length > 0)
		{
			string arg = "Default";
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(new char[]
				{
					'|'
				});
				if (array != null && array.Length > 0)
				{
					arg = array[0];
				}
			}
			RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.FullAdClicked, -1, arg, 1);
		}
	}

	public void onAdShow(string data)
	{
		if (RiseSdkListener.OnAdEvent != null && RiseSdkListener.OnAdEvent.GetInvocationList().Length > 0)
		{
			string arg = "Default";
			int arg2 = 1;
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(new char[]
				{
					'|'
				});
				if (array != null && array.Length > 1)
				{
					int.TryParse(array[0], out arg2);
					arg = array[1];
				}
			}
			RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.FullAdShown, -1, arg, arg2);
		}
	}

	public void onAdClicked(string data)
	{
		if (RiseSdkListener.OnAdEvent != null && RiseSdkListener.OnAdEvent.GetInvocationList().Length > 0)
		{
			string arg = "Default";
			int arg2 = 1;
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(new char[]
				{
					'|'
				});
				if (array != null && array.Length > 1)
				{
					int.TryParse(array[0], out arg2);
					arg = array[1];
				}
			}
			RiseSdk.AdEventType arg3 = RiseSdk.AdEventType.FullAdClicked;
			switch (arg2)
			{
			case 1:
				arg3 = RiseSdk.AdEventType.FullAdClicked;
				break;
			case 2:
				arg3 = RiseSdk.AdEventType.VideoAdClicked;
				break;
			case 3:
				arg3 = RiseSdk.AdEventType.BannerAdClicked;
				break;
			case 4:
				arg3 = RiseSdk.AdEventType.IconAdClicked;
				break;
			case 5:
				arg3 = RiseSdk.AdEventType.NativeAdClicked;
				break;
			}
			RiseSdkListener.OnAdEvent(arg3, -1, arg, arg2);
		}
	}

	public void onVideoAdClosed(string data)
	{
		if (RiseSdkListener.OnAdEvent != null && RiseSdkListener.OnAdEvent.GetInvocationList().Length > 0)
		{
			string arg = "Default";
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(new char[]
				{
					'|'
				});
				if (array != null && array.Length > 0)
				{
					arg = array[0];
				}
			}
			RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.RewardAdClosed, -1, arg, 2);
		}
	}

	public void onBannerAdClicked(string data)
	{
		if (RiseSdkListener.OnAdEvent != null && RiseSdkListener.OnAdEvent.GetInvocationList().Length > 0)
		{
			string arg = "Default";
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(new char[]
				{
					'|'
				});
				if (array != null && array.Length > 0)
				{
					arg = array[0];
				}
			}
			RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.BannerAdClicked, -1, arg, 3);
		}
	}

	public void onCrossAdClicked(string data)
	{
		if (RiseSdkListener.OnAdEvent != null && RiseSdkListener.OnAdEvent.GetInvocationList().Length > 0)
		{
			string arg = "Default";
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(new char[]
				{
					'|'
				});
				if (array != null && array.Length > 0)
				{
					arg = array[0];
				}
			}
			RiseSdkListener.OnAdEvent(RiseSdk.AdEventType.CrossAdClicked, -1, arg, -1);
		}
	}

	private static RiseSdkListener _instance;
}
