using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class RiseSdk
{
	public void SetPaymentSystemValid(bool valid)
	{
		this.paymentSystemValid = valid;
	}

	public static RiseSdk Instance
	{
		get
		{
			if (RiseSdk._instance == null)
			{
				RiseSdk._instance = new RiseSdk();
			}
			return RiseSdk._instance;
		}
	}

	public void Init()
	{
		RiseSdk.RiseEditorAd.hasInit = true;
		if (this._class != null)
		{
			return;
		}
		try
		{
			RiseSdkListener.Instance.enabled = true;
			this._class = new AndroidJavaClass("com.android.client.Unity");
			if (this._class != null)
			{
				AndroidJNIHelper.debug = true;
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					using (AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						this._class.CallStatic("onCreate", new object[]
						{
							@static
						});
					}
				}
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogWarning(ex.StackTrace);
			this._class = null;
		}
		finally
		{
			this.lruCache = new RiseSdk.FileLRUCache(20);
		}
	}

	public int GetScreenWidth()
	{
		return Screen.width;
	}

	public int GetScreenHeight()
	{
		return Screen.height;
	}

	public void ShowBanner(string tag, int pos)
	{
		if (this._class != null)
		{
			this._class.CallStatic("showBanner", new object[]
			{
				tag,
				pos
			});
			UnityEngine.Debug.LogWarning("showBanner");
		}
	}

	public void ShowBanner(int pos)
	{
		if (this._class != null)
		{
			this._class.CallStatic("showBanner", new object[]
			{
				pos
			});
			UnityEngine.Debug.LogWarning("showBanner");
		}
	}

	public void ShowBanner(string tag, int pos, int animate)
	{
		if (this._class != null)
		{
			this._class.CallStatic("showBanner", new object[]
			{
				tag,
				pos,
				animate
			});
			UnityEngine.Debug.LogWarning("showBanner");
		}
	}

	public bool HasBanner(string tag)
	{
		return this._class != null && this._class.CallStatic<bool>("hasBanner", new object[]
		{
			tag
		});
	}

	public void CloseBanner()
	{
		if (this._class != null)
		{
			this._class.CallStatic("closeBanner", new object[0]);
		}
	}

	public void ShowAd(string tag)
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class != null)
		{
			this._class.CallStatic("showFullAd", new object[]
			{
				tag
			});
		}
	}

	public bool HasInterstitial(string tag)
	{
		return this._class != null && this._class.CallStatic<bool>("hasFull", new object[]
		{
			tag
		});
	}

	public void ShowMore()
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class != null)
		{
			this._class.CallStatic("moreGame", new object[0]);
		}
	}

	public bool HasRewardAd()
	{
		return this._class != null && this._class.CallStatic<bool>("hasRewardAd", new object[0]);
	}

	public bool HasRewardAd(string tag)
	{
		return this._class != null && this._class.CallStatic<bool>("hasRewardAd", new object[]
		{
			tag
		});
	}

	public void ShowRewardAd(int rewardId)
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class != null)
		{
			this._class.CallStatic("showRewardAd", new object[]
			{
				rewardId
			});
		}
	}

	public void ShowRewardAd(string tag, int rewardId)
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class != null)
		{
			this._class.CallStatic("showRewardAd", new object[]
			{
				tag,
				rewardId
			});
		}
	}

	public void enableBackHomeAd(bool enabled, string adPos, int minPauseMillisecond = 20000)
	{
		this.BACK_HOME_ADPOS = adPos;
		this.BACK_HOME_AD_ENABLE = enabled;
		this.homeAdMinPauseMillisecond = minPauseMillisecond;
	}

	public void OnResume()
	{
		if (this._class != null)
		{
			this._class.CallStatic("onResume", new object[0]);
		}
		if (this.BACK_HOME_AD_ENABLE && this.canShowBackHomeAd && this.BACK_HOME_AD_TIME <= 0.0)
		{
			this.canShowBackHomeAd = false;
			if (RiseSdk.GetCurrentTimeInMills() - this.pauseTime > (double)this.homeAdMinPauseMillisecond)
			{
				this.ShowAd(this.BACK_HOME_ADPOS);
				RiseSdkListener.Instance.OnResumeAd();
			}
		}
	}

	public void OnPause()
	{
		if (this._class != null)
		{
			this._class.CallStatic("onPause", new object[0]);
		}
		if (this.BACK_HOME_AD_ENABLE)
		{
			double currentTimeInMills = RiseSdk.GetCurrentTimeInMills();
			double num = currentTimeInMills - this.BACK_HOME_AD_TIME;
			this.canShowBackHomeAd = (num > 2000.0);
			if (this.canShowBackHomeAd)
			{
				this.BACK_HOME_AD_TIME = 0.0;
			}
			this.pauseTime = currentTimeInMills;
		}
	}

	public void OnStart()
	{
		if (this._class != null)
		{
			this._class.CallStatic("onStart", new object[0]);
		}
	}

	public void OnStop()
	{
		if (this._class != null)
		{
			this._class.CallStatic("onStop", new object[0]);
		}
	}

	public void OnDestroy()
	{
		if (this._class != null)
		{
			this._class.CallStatic("onDestroy", new object[0]);
		}
	}

	public void OnExit()
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class != null)
		{
			this._class.CallStatic("onQuit", new object[0]);
		}
	}

	public void HasPaid(int billingId)
	{
		if (this._class != null)
		{
			this._class.CallStatic("query", new object[]
			{
				billingId
			});
		}
	}

	public bool IsPayEnabled()
	{
		return this.paymentSystemValid;
	}

	public void Pay(int billingId)
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class != null)
		{
			this._class.CallStatic("pay", new object[]
			{
				billingId
			});
		}
	}

	public void Share()
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class == null)
		{
			return;
		}
		this._class.CallStatic("share", new object[0]);
	}

	public string GetExtraData()
	{
		if (this._class == null)
		{
			return null;
		}
		return this._class.CallStatic<string>("getExtraData", new object[0]);
	}

	public void TrackEvent(string category, string action, string label, int value)
	{
		if (this._class == null)
		{
			return;
		}
		this._class.CallStatic("trackEvent", new object[]
		{
			category,
			action,
			label,
			value
		});
	}

	public void TrackEvent(string category, string keyValueData)
	{
		if (this._class == null)
		{
			return;
		}
		this._class.CallStatic("trackEvent", new object[]
		{
			category,
			keyValueData
		});
	}

	public void TrackFinishLevel(string level)
	{
		if (this._class == null)
		{
			return;
		}
		this._class.CallStatic("logFinishLevel", new object[]
		{
			level
		});
	}

	public void TrackFinishAchievement(string achievement)
	{
		if (this._class == null)
		{
			return;
		}
		this._class.CallStatic("logFinishAchievement", new object[]
		{
			achievement
		});
	}

	public void TrackFinishTutorial(string tutorial)
	{
		if (this._class == null)
		{
			return;
		}
		this._class.CallStatic("logFinishTutorial", new object[]
		{
			tutorial
		});
	}

	public void Rate()
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class == null)
		{
			return;
		}
		this._class.CallStatic("rate", new object[0]);
	}

	public void ShowNativeAd(string tag, int yPercent)
	{
		if (this._class != null)
		{
			this._class.CallStatic("showNative", new object[]
			{
				tag,
				yPercent
			});
		}
	}

	public bool ShowNativeAd(string tag, int xPixel, int yPixel, string configJson)
	{
		return this._class != null && this._class.CallStatic<bool>("showNativeBanner", new object[]
		{
			tag,
			xPixel,
			yPixel,
			configJson
		});
	}

	public bool ShowNativeAdWithFrame(string tag, float xPixel, float yPixel, float width, float height, string configJson)
	{
		return this._class != null && this._class.CallStatic<bool>("showNativeBanner", new object[]
		{
			tag,
			xPixel,
			yPixel,
			width,
			height,
			configJson
		});
	}

	public void CloseNativeAd(string tag)
	{
		if (this._class != null)
		{
			this._class.CallStatic("closeNativeBanner", new object[]
			{
				tag
			});
		}
	}

	public void HideNativeAd(string tag)
	{
		if (this._class != null)
		{
			this._class.CallStatic("hideNative", new object[]
			{
				tag
			});
		}
	}

	public bool HasNativeAd(string tag)
	{
		return this._class != null && this._class.CallStatic<bool>("hasNative", new object[]
		{
			tag
		});
	}

	public void ShowDeliciousIconAd(float x, float y, float w, float h, string configJson)
	{
		if (this._class != null)
		{
			this._class.CallStatic("showDeliciousIconAd", new object[]
			{
				(int)x,
				(int)y,
				(int)w,
				(int)h,
				configJson
			});
		}
	}

	public void CloseDeliciousIconAd()
	{
		if (this._class != null)
		{
			this._class.CallStatic("closeDeliciousIconAd", new object[0]);
		}
	}

	public void ShowDeliciousBannerAd(float x, float y, float w, float h, string configJson)
	{
		if (this._class != null)
		{
			this._class.CallStatic("showDeliciousBannerAd", new object[]
			{
				(int)x,
				(int)y,
				(int)w,
				(int)h,
				configJson
			});
		}
	}

	public void CloseDeliciousBannerAd()
	{
		if (this._class != null)
		{
			this._class.CallStatic("closeDeliciousBannerAd", new object[0]);
		}
	}

	public void ShowDeliciousVideoAd(string configJson)
	{
		if (this._class != null)
		{
			this._class.CallStatic("showDeliciousVideoAd", new object[]
			{
				configJson
			});
		}
	}

	public bool HasDeliciousAd()
	{
		return this._class != null && this._class.CallStatic<bool>("hasDeliciousAd", new object[0]);
	}

	public void Login()
	{
		if (this._class != null)
		{
			this._class.CallStatic("login", new object[0]);
		}
	}

	public bool IsLogin()
	{
		return this._class != null && this._class.CallStatic<bool>("isLogin", new object[0]);
	}

	public void Logout()
	{
		if (this._class != null)
		{
			this._class.CallStatic("logout", new object[0]);
		}
	}

	public void Invite()
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class != null)
		{
			this._class.CallStatic("invite", new object[0]);
		}
	}

	public void Challenge(string title, string message)
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class != null)
		{
			this._class.CallStatic("challenge", new object[]
			{
				title,
				message
			});
		}
	}

	public string Me()
	{
		if (this._class != null)
		{
			return this._class.CallStatic<string>("me", new object[0]);
		}
		return string.Empty;
	}

	public string GetFriends()
	{
		if (this._class != null)
		{
			return this._class.CallStatic<string>("friends", new object[0]);
		}
		return string.Empty;
	}

	public void Like()
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class != null)
		{
			this._class.CallStatic("like", new object[0]);
		}
	}

	public string GetMePictureURL()
	{
		string text = this.Me();
		string result = string.Empty;
		if (!string.IsNullOrEmpty(text) && text.Length > 5)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)RiseJson.Deserialize(text);
			if (dictionary.ContainsKey("picture"))
			{
				result = dictionary["picture"].ToString();
			}
		}
		return result;
	}

	public string GetPaymentsPrices()
	{
		if (this._class != null)
		{
			return this._class.CallStatic<string>("getPrices", new object[0]);
		}
		return "{}";
	}

	public string GetConfig(int configId)
	{
		if (this._class != null)
		{
			return this._class.CallStatic<string>("getConfig", new object[]
			{
				configId
			});
		}
		return "0";
	}

	public string CacheUrl(string url)
	{
		if (this._class != null)
		{
			return this._class.CallStatic<string>("cacheUrl", new object[]
			{
				url
			});
		}
		return string.Empty;
	}

	public void CacheUrl(int tag, string url)
	{
		if (this._class != null)
		{
			this._class.CallStatic("cacheUrl", new object[]
			{
				tag,
				url
			});
		}
	}

	public bool HasApp(string packageName)
	{
		return this._class != null && this._class.CallStatic<bool>("hasApp", new object[]
		{
			packageName
		});
	}

	public void LaunchApp(string packageName)
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class != null)
		{
			this._class.CallStatic("launchApp", new object[]
			{
				packageName
			});
		}
	}

	public void GetApp(string packageName)
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class != null)
		{
			this._class.CallStatic("getApp", new object[]
			{
				packageName
			});
		}
	}

	public string GetConfig(string packageName, int configId)
	{
		if (this._class != null)
		{
			return this._class.CallStatic<string>("getConfig", new object[]
			{
				packageName,
				configId
			});
		}
		return string.Empty;
	}

	public void Alert(string title, string message)
	{
		this.BACK_HOME_AD_TIME = RiseSdk.GetCurrentTimeInMills();
		if (this._class != null)
		{
			this._class.CallStatic("alert", new object[]
			{
				title,
				message
			});
		}
	}

	public void Toast(string message)
	{
		if (this._class != null)
		{
			this._class.CallStatic("toast", new object[]
			{
				message
			});
		}
	}

	public bool IsNetworkConnected()
	{
		return this._class == null || this._class.CallStatic<bool>("isNetworkConnected", new object[0]);
	}

	public bool HasGDPR()
	{
		return this._class == null || this._class.CallStatic<bool>("hasGDPR", new object[0]);
	}

	public void ResetGDPR()
	{
		if (this._class != null)
		{
			this._class.CallStatic("resetGDPR", new object[0]);
		}
	}

	public int GetRemoteConfigInt(string remoteKey)
	{
		if (this._class != null)
		{
			return this._class.CallStatic<int>("getRemoteConfigInt", new object[]
			{
				remoteKey
			});
		}
		return 0;
	}

	public long GetRemoteConfigLong(string remoteKey)
	{
		if (this._class != null)
		{
			return this._class.CallStatic<long>("getRemoteConfigLong", new object[]
			{
				remoteKey
			});
		}
		return 0L;
	}

	public double GetRemoteConfigDouble(string remoteKey)
	{
		if (this._class != null)
		{
			return this._class.CallStatic<double>("getRemoteConfigDouble", new object[]
			{
				remoteKey
			});
		}
		return 0.0;
	}

	public bool GetRemoteConfigBoolean(string remoteKey)
	{
		return this._class != null && this._class.CallStatic<bool>("getRemoteConfigBoolean", new object[]
		{
			remoteKey
		});
	}

	public string GetRemoteConfigString(string remoteKey)
	{
		if (this._class != null)
		{
			return this._class.CallStatic<string>("getRemoteConfigString", new object[]
			{
				remoteKey
			});
		}
		return string.Empty;
	}

	public void SetUserTag(string tag)
	{
		if (this._class != null)
		{
			this._class.CallStatic("setUserTag", new object[]
			{
				tag
			});
		}
	}

	public void SetUserProperty(string key, string value)
	{
		if (this._class != null)
		{
			this._class.CallStatic("setUserProperty", new object[]
			{
				key,
				value
			});
		}
	}

	public void UM_setPlayerLevel(int level)
	{
		if (this._class != null)
		{
			this._class.CallStatic("UM_setPlayerLevel", new object[]
			{
				level
			});
		}
	}

	public void UM_onEvent(string eventId)
	{
		if (this._class != null)
		{
			this._class.CallStatic("UM_onEvent", new object[]
			{
				eventId
			});
		}
	}

	public void UM_onEvent(string eventId, string eventLabel)
	{
		if (this._class != null)
		{
			this._class.CallStatic("UM_onEvent", new object[]
			{
				eventId,
				eventLabel
			});
		}
	}

	public void UM_onEventValue(string eventId, Dictionary<string, string> mapStr)
	{
		if (this._class != null)
		{
			AndroidJavaObject androidJavaObject = null;
			if (mapStr != null)
			{
				try
				{
					androidJavaObject = new AndroidJavaObject("java.util.Map", new object[0]);
					foreach (KeyValuePair<string, string> keyValuePair in mapStr)
					{
						androidJavaObject.Call<string>("put", new object[]
						{
							keyValuePair.Key,
							keyValuePair.Value
						});
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogError("UM_onEventValue Exception msg:\n" + ex.StackTrace);
				}
			}
			this._class.CallStatic("UM_onEventValue", new object[]
			{
				androidJavaObject,
				1
			});
		}
	}

	public void UM_onPageStart(string pageName)
	{
		if (this._class != null)
		{
			this._class.CallStatic("UM_onPageStart", new object[]
			{
				pageName
			});
		}
	}

	public void UM_onPageEnd(string pageName)
	{
		if (this._class != null)
		{
			this._class.CallStatic("UM_onPageEnd", new object[]
			{
				pageName
			});
		}
	}

	public void UM_startLevel(string level)
	{
		if (this._class != null)
		{
			this._class.CallStatic("UM_startLevel", new object[]
			{
				level
			});
		}
	}

	public void UM_failLevel(string level)
	{
		if (this._class != null)
		{
			this._class.CallStatic("UM_failLevel", new object[]
			{
				level
			});
		}
	}

	public void UM_finishLevel(string level)
	{
		if (this._class != null)
		{
			this._class.CallStatic("UM_finishLevel", new object[]
			{
				level
			});
		}
	}

	public void UM_pay(double money, string itemName, int number, double price)
	{
		if (this._class != null)
		{
			this._class.CallStatic("UM_pay", new object[]
			{
				money,
				itemName,
				number,
				price
			});
		}
	}

	public void UM_buy(string itemName, int count, double price)
	{
		if (this._class != null)
		{
			this._class.CallStatic("UM_buy", new object[]
			{
				itemName,
				count,
				price
			});
		}
	}

	public void UM_use(string itemName, int number, double price)
	{
		if (this._class != null)
		{
			this._class.CallStatic("UM_use", new object[]
			{
				itemName,
				number,
				price
			});
		}
	}

	public void UM_bonus(string itemName, int number, double price, int trigger)
	{
		if (this._class != null)
		{
			this._class.CallStatic("UM_bonus", new object[]
			{
				itemName,
				number,
				price,
				trigger
			});
		}
	}

	public string GetMeFirstName()
	{
		return "FirstName";
	}

	public string GetMeLastName()
	{
		return "LastName";
	}

	public string GetMeId()
	{
		return "MeId";
	}

	public string GetMeName()
	{
		return "MeName";
	}

	public void FetchFriends(bool invitable)
	{
	}

	public void FetchScores()
	{
	}

	public void Share(string contentURL, string tag, string quote)
	{
	}

	public void RestorePayments()
	{
	}

	public void SdkLog(string message)
	{
	}

	public void LoadAd(string tag)
	{
	}

	public void ShowPopupIconAd()
	{
	}

	public void DownloadFile(string url, Action<string, WWW> resultEvent)
	{
		this.lruCache.DownloadFile(url, resultEvent);
	}

	public void LoadLocalFile(string filePath, Action<string, WWW> resultEvent)
	{
		this.lruCache.LoadLocalFile(filePath, resultEvent);
	}

	public static double GetCurrentTimeInMills()
	{
		return DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
	}

	public static string CalculateMD5Hash(string input)
	{
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
			MD5 md = MD5.Create();
			byte[] array = Encoding.UTF8.GetBytes(input);
			array = md.ComputeHash(array);
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("X2"));
			}
			return stringBuilder.ToString().ToLower();
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("CalculateMD5Hash error:\n" + ex.StackTrace);
		}
		finally
		{
		}
		return stringBuilder.ToString();
	}

	private static RiseSdk _instance;

	private AndroidJavaClass _class;

	private bool paymentSystemValid;

	private string BACK_HOME_ADPOS = "custom";

	private bool BACK_HOME_AD_ENABLE;

	private double BACK_HOME_AD_TIME;

	private bool canShowBackHomeAd;

	private int homeAdMinPauseMillisecond = 1000;

	private double pauseTime;

	private RiseSdk.FileLRUCache lruCache;

	public const int POS_BANNER_LEFT_TOP = 1;

	public const int POS_BANNER_MIDDLE_TOP = 3;

	public const int POS_BANNER_RIGHT_TOP = 6;

	public const int POS_BANNER_MIDDLE_MIDDLE = 5;

	public const int POS_BANNER_LEFT_BOTTOM = 2;

	public const int POS_BANNER_MIDDLE_BOTTOM = 4;

	public const int POS_BANNER_RIGHT_BOTTOM = 7;

	public const int POS_BANNER_LEFT_MIDDLE = 8;

	public const int POS_BANNER_RIGHT_MIDDLE = 9;

	public const int ANIMATE_BANNER_NONE = 0;

	public const int ANIMATE_BANNER_TOP = 1;

	public const int ANIMATE_BANNER_BOTTOM = 2;

	public const int ANIMATE_BANNER_LEFT = 4;

	public const int ANIMATE_BANNER_RIGHT = 8;

	public const int ANIMATE_BANNER_ROTATION = 16;

	public const string M_START = "start";

	public const string M_PAUSE = "pause";

	public const string M_PASSLEVEL = "passlevel";

	public const string M_PASSLEVEL_1 = "passlevel1";

	public const string M_CUSTOM = "custom";

	public const int PAYMENT_RESULT_SUCCESS = 1;

	public const int PAYMENT_RESULT_FAILS = 2;

	public const int PAYMENT_RESULT_CANCEL = 3;

	public const int CONFIG_KEY_APP_ID = 1;

	public const int CONFIG_KEY_LEADER_BOARD_URL = 2;

	public const int CONFIG_KEY_API_VERSION = 3;

	public const int CONFIG_KEY_SCREEN_WIDTH = 4;

	public const int CONFIG_KEY_SCREEN_HEIGHT = 5;

	public const int CONFIG_KEY_LANGUAGE = 6;

	public const int CONFIG_KEY_COUNTRY = 7;

	public const int CONFIG_KEY_VERSION_CODE = 8;

	public const int CONFIG_KEY_VERSION_NAME = 9;

	public const int CONFIG_KEY_PACKAGE_NAME = 10;

	public const int CONFIG_KEY_UUID = 11;

	public const int ADTYPE_OTHER = -1;

	public const int ADTYPE_INTERTITIAL = 1;

	public const int ADTYPE_VIDEO = 2;

	public const int ADTYPE_BANNER = 3;

	public const int ADTYPE_ICON = 4;

	public const int ADTYPE_NATIVE = 5;

	public enum AdEventType
	{
		FullAdLoadCompleted = 1,
		FullAdLoadFailed,
		RewardAdLoadFailed,
		RewardAdLoadCompleted,
		RewardAdShowStart,
		RewardAdShowFinished,
		RewardAdShowFailed,
		RewardAdClosed,
		VideoAdClicked,
		FullAdClosed,
		FullAdShown,
		FullAdClicked,
		BannerAdClicked,
		CrossAdClicked,
		AdLoadCompleted,
		AdLoadFailed,
		AdShown,
		AdClosed,
		AdClicked,
		IconAdClicked,
		NativeAdClicked
	}

	public enum PaymentResult
	{
		Success = 1,
		Failed,
		Cancel,
		PaymentSystemError
	}

	public enum SnsEventType
	{
		LoginSuccess = 1,
		LoginFailed,
		InviteSuccess,
		InviteFailed,
		ChallengeSuccess,
		ChallengeFailed,
		LikeSuccess,
		LikeFailed,
		ShareSuccess,
		ShareFailed,
		ShareCancel
	}

	public enum LocalPushType
	{
		NoCycle,
		YearCycle = 4,
		MonthCycle = 8,
		DayCycle = 16,
		HourCycle = 32,
		MinuteCycle = 64,
		SecondCycle = 128,
		WeekDayCycle = 512,
		WeekDayOrDinalCycle = 1024
	}

	private class RiseEditorAd : MonoBehaviour
	{
		private void Awake()
		{
			if (RiseSdk.RiseEditorAd._editorAdInstance == null)
			{
				RiseSdk.RiseEditorAd._editorAdInstance = this;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (Screen.width > Screen.height)
			{
				this.originScreenWidth = 854;
				this.originScreenHeight = 480;
			}
			else
			{
				this.originScreenWidth = 480;
				this.originScreenHeight = 854;
			}
			this.scaleWidth = (float)Screen.width * 1f / (float)this.originScreenWidth;
			this.scaleHeight = (float)Screen.height * 1f / (float)this.originScreenHeight;
			this.toastStyle = new GUIStyle();
			this.toastStyle.fontStyle = FontStyle.Bold;
			this.toastStyle.alignment = TextAnchor.MiddleCenter;
			this.toastStyle.fontSize = 30;
		}

		public static RiseSdk.RiseEditorAd EditorAdInstance
		{
			get
			{
				if (RiseSdk.RiseEditorAd._editorAdInstance == null)
				{
					RiseSdk.RiseEditorAd._editorAdInstance = ((!(UnityEngine.Object.FindObjectOfType<RiseSdk.RiseEditorAd>() == null)) ? RiseSdk.RiseEditorAd._editorAdInstance : new GameObject("RiseEditorAd").AddComponent<RiseSdk.RiseEditorAd>());
				}
				if (!RiseSdk.RiseEditorAd.hasInit)
				{
					UnityEngine.Debug.LogError("Fatal Error: \nNeed Call RiseSdk.Instance.Init () First At Initialize Scene");
				}
				return RiseSdk.RiseEditorAd._editorAdInstance;
			}
		}

		public void ShowBanner(int pos)
		{
		}

		public void ShowBanner(string tag, int pos)
		{
		}

		public void ShowBanner(string tag, int pos, int animate)
		{
		}

		public void CloseBanner()
		{
		}

		private void SetBannerPos(int pos)
		{
		}

		public void ShowAd(string tag)
		{
		}

		public void ShowRewardAd(int id)
		{
		}

		public void ShowRewardAd(string tag, int id)
		{
		}

		public void ShowIconAd(float width, float xPercent, float yPercent)
		{
			this.iconAdShow = true;
			this.iconAdWidth = width;
			this.iconAdXPercent = xPercent;
			this.iconAdYPercent = yPercent;
		}

		public void CloseIconAd()
		{
			this.iconAdShow = false;
		}

		public void Pay(int billingId)
		{
		}

		public void Toast(string msg)
		{
		}

		private IEnumerator CheckToast(float time = 2f)
		{
			yield return new WaitForSeconds(time);
			if (this.toastList.Count > 0)
			{
				this.toastList.RemoveAt(0);
			}
			if (this.toastList.Count > 0)
			{
				base.StartCoroutine(this.CheckToast(2f));
			}
			else
			{
				this.timeCounting = false;
			}
			yield break;
		}

		public void Alert(string title, string msg)
		{
		}

		public void OnExit()
		{
		}

		private static RiseSdk.RiseEditorAd _editorAdInstance;

		public static bool hasInit;

		private Rect bannerPos;

		private bool bannerShow;

		private string bannerContent = string.Empty;

		private bool interstitialShow;

		private string interstitialContent = string.Empty;

		private bool rewardShow;

		private string rewardContent = string.Empty;

		private float scaleWidth = 1f;

		private float scaleHeight = 1f;

		private int originScreenWidth = 1;

		private int originScreenHeight = 1;

		private bool toastShow;

		private List<string> toastList = new List<string>();

		private GUIStyle toastStyle;

		private int rewardAdId = -10;

		private string rewardAdTag = "DEFAULT";

		private float iconAdWidth = 56f;

		private float iconAdXPercent = 0.2f;

		private float iconAdYPercent = 0.2f;

		private bool iconAdShow;

		private string iconAdContent = "Icon Ad";

		private EventSystem curEvent;

		private const int NONE_REWARD_ID = -10;

		private const string DEFAULT_REWARD_TAG = "DEFAULT";

		private const string BANNER_DEFAULT_TXT = "Banner AD";

		private const string INTERSTITIAL_DEFAULT_TXT = "\nInterstitial AD Test";

		private const string REWARD_DEFAULT_TXT = "Free Coin AD Test: ";

		private const int SCREEN_WIDTH = 854;

		private const int SCREEN_HEIGHT = 480;

		private const int GUI_DEPTH = -99;

		private const int BANNER_WIDTH = 320;

		private const int BANNER_HEIGHT = 50;

		private bool timeCounting;
	}

	private class FileLRUCache
	{
		public FileLRUCache(int capacity)
		{
			this.maxCapacity = capacity;
			this.cache = new Dictionary<string, RiseSdk.LinkedNode>();
			this.head = new RiseSdk.LinkedNode();
			this.tail = new RiseSdk.LinkedNode();
			this.head.prev = null;
			this.head.next = this.tail;
			this.tail.prev = this.head;
			this.tail.next = null;
			try
			{
				RiseSdk.FileLRUCache.defFilePath = Application.persistentDataPath + "/filecache/";
				if (!Directory.Exists(RiseSdk.FileLRUCache.defFilePath))
				{
					Directory.CreateDirectory(RiseSdk.FileLRUCache.defFilePath);
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("FileLRUCache init error\n" + ex.StackTrace);
				RiseSdk.FileLRUCache.defFilePath = Application.persistentDataPath + "/filecache/";
			}
			finally
			{
				if (!Directory.Exists(RiseSdk.FileLRUCache.defFilePath))
				{
					Directory.CreateDirectory(RiseSdk.FileLRUCache.defFilePath);
				}
				string text = RiseSdk.FileLRUCache.defFilePath + "filedirmeta";
				if (!File.Exists(text))
				{
					File.Create(text);
					RiseSdkListener.Instance.StartCoroutine(this.delayLoad(text, 1f));
				}
				else
				{
					this.LoadLocalFile(text, new Action<string, WWW>(this.loadCache));
				}
			}
		}

		public void DownloadFile(string url, Action<string, WWW> resultEvent)
		{
			if (string.IsNullOrEmpty(url))
			{
				if (resultEvent != null)
				{
					resultEvent(string.Empty, null);
				}
				return;
			}
			string text = RiseSdk.CalculateMD5Hash(url);
			if (!File.Exists(RiseSdk.FileLRUCache.defFilePath + text))
			{
				RiseSdkListener.Instance.StartCoroutine(this.Download(url, text, resultEvent));
			}
			else
			{
				RiseSdkListener.Instance.StartCoroutine(this.LoadLocal(null, text, resultEvent));
			}
		}

		public void LoadLocalFile(string filePath, Action<string, WWW> resultEvent)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				if (resultEvent != null)
				{
					resultEvent(filePath, null);
				}
			}
			else if (File.Exists(filePath))
			{
				RiseSdkListener.Instance.StartCoroutine(this.LoadLocal(filePath, null, resultEvent));
			}
			else if (resultEvent != null)
			{
				resultEvent(filePath, null);
			}
		}

		public bool FileDownloaded(string url)
		{
			string str = RiseSdk.CalculateMD5Hash(url);
			return File.Exists(RiseSdk.FileLRUCache.defFilePath + str);
		}

		private IEnumerator Download(string url, string saveName, Action<string, WWW> resultEvent)
		{
			if (string.IsNullOrEmpty(url))
			{
				if (resultEvent != null)
				{
					resultEvent(RiseSdk.FileLRUCache.defFilePath + saveName, null);
					UnityEngine.Debug.LogWarning("Download File error, url: " + url + ", saveName: " + saveName);
				}
			}
			else
			{
				WWW www = new WWW(url);
				yield return www;
				this.set(saveName, RiseSdk.FileLRUCache.defFilePath + saveName);
				if (string.IsNullOrEmpty(www.error))
				{
					if (www.bytes != null && www.bytes.Length > 1000)
					{
						byte[] bytes = www.bytes;
						File.WriteAllBytes(RiseSdk.FileLRUCache.defFilePath + saveName, bytes);
						if (resultEvent != null)
						{
							resultEvent(RiseSdk.FileLRUCache.defFilePath + saveName, www);
						}
					}
					else if (resultEvent != null)
					{
						resultEvent(RiseSdk.FileLRUCache.defFilePath + saveName, null);
					}
				}
				else if (resultEvent != null)
				{
					resultEvent(RiseSdk.FileLRUCache.defFilePath + saveName, null);
					UnityEngine.Debug.LogError(string.Concat(new string[]
					{
						"Download File error, url: ",
						url,
						", saveName: ",
						saveName,
						", www.error: ",
						www.error
					}));
				}
			}
			yield break;
		}

		private IEnumerator LoadLocal(string filePath, string saveName, Action<string, WWW> resultEvent)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				if (resultEvent != null)
				{
					resultEvent(filePath + saveName, null);
					UnityEngine.Debug.LogWarning("LoadLocal File error, filePath: " + filePath + ", saveName: " + saveName);
				}
			}
			else
			{
				if (string.IsNullOrEmpty(filePath))
				{
					filePath = RiseSdk.FileLRUCache.defFilePath;
				}
				if (saveName == null)
				{
					saveName = string.Empty;
				}
				string path = "file:///" + filePath + saveName;
				WWW www = new WWW(path);
				yield return www;
				this.set(saveName, filePath + saveName);
				if (string.IsNullOrEmpty(www.error))
				{
					if (www.bytes != null && www.bytes.Length > 1000)
					{
						if (resultEvent != null)
						{
							resultEvent(filePath + saveName, www);
						}
					}
					else if (resultEvent != null)
					{
						resultEvent(filePath + saveName, null);
					}
				}
				else if (resultEvent != null)
				{
					resultEvent(filePath + saveName, null);
					UnityEngine.Debug.LogError(string.Concat(new string[]
					{
						"LoadLocal File error, filePath: ",
						filePath,
						", saveName: ",
						saveName,
						", www.error: ",
						www.error
					}));
				}
			}
			yield break;
		}

		private IEnumerator delayLoad(string path, float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			this.LoadLocalFile(path, new Action<string, WWW>(this.loadCache));
			yield break;
		}

		private void loadCache(string path, WWW www)
		{
			if (www != null)
			{
				string text = www.text;
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = text.Split("@^@".ToCharArray());
					this.size = 0;
					int i = 0;
					int num = array.Length;
					while (i < num)
					{
						string[] array2 = null;
						if (!string.IsNullOrEmpty(array[i]))
						{
							array2 = array[i].Split("^_^".ToCharArray());
						}
						if (array2 != null && array2.Length > 1 && !string.IsNullOrEmpty(array2[0]) && !string.IsNullOrEmpty(array2[1]))
						{
							RiseSdk.LinkedNode linkedNode = new RiseSdk.LinkedNode();
							linkedNode.key = array2[0];
							linkedNode.value = array2[1];
							RiseSdk.LinkedNode prev = this.tail.prev;
							this.tail.prev = linkedNode;
							linkedNode.next = this.tail;
							linkedNode.prev = prev;
							prev.next = linkedNode;
							this.cache.Add(array2[0], linkedNode);
							this.size++;
						}
						i++;
					}
				}
			}
		}

		private void writeCache()
		{
			if (this.writting != null)
			{
				RiseSdkListener.Instance.StopCoroutine(this.writting);
			}
			this.writting = RiseSdkListener.Instance.StartCoroutine(this.delayWrite());
		}

		private IEnumerator delayWrite()
		{
			yield return new WaitForSeconds(1f);
			string str = string.Empty;
			RiseSdk.LinkedNode node = this.head.next;
			while (node != null && node != this.tail)
			{
				string text = str;
				str = string.Concat(new string[]
				{
					text,
					node.key,
					"^_^",
					node.value,
					"@^@"
				});
				node = node.next;
			}
			str = str.Remove(str.Length - "@^@".Length, "@^@".Length);
			File.WriteAllText(RiseSdk.FileLRUCache.defFilePath + "filedirmeta", str, Encoding.UTF8);
			yield break;
		}

		private void set(string key, string value)
		{
			if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
			{
				return;
			}
			if (this.cache.ContainsKey(key))
			{
				RiseSdk.LinkedNode linkedNode = this.cache[key];
				this.moveToFront(linkedNode);
			}
			else
			{
				RiseSdk.LinkedNode linkedNode = new RiseSdk.LinkedNode();
				linkedNode.key = key;
				linkedNode.value = value;
				this.linkAtFront(linkedNode);
				this.cache.Add(key, linkedNode);
				this.size++;
			}
			this.checkCapacity();
			this.writeCache();
		}

		private void checkCapacity()
		{
			while (this.size > this.maxCapacity)
			{
				this.size--;
				this.removeLast();
			}
		}

		private string get(string key)
		{
			if (!string.IsNullOrEmpty(key))
			{
				return this.cache[key].value;
			}
			return string.Empty;
		}

		private void linkAtFront(RiseSdk.LinkedNode node)
		{
			RiseSdk.LinkedNode next = this.head.next;
			this.head.next = node;
			node.prev = this.head;
			node.next = next;
			next.prev = node;
		}

		private void moveToFront(RiseSdk.LinkedNode node)
		{
			RiseSdk.LinkedNode prev = node.prev;
			RiseSdk.LinkedNode next = node.next;
			if (prev == null || next == null)
			{
				return;
			}
			prev.next = next;
			next.prev = prev;
			this.linkAtFront(node);
		}

		private void removeLast()
		{
			RiseSdk.LinkedNode prev = this.tail.prev;
			RiseSdk.LinkedNode prev2 = prev.prev;
			if (prev == this.head || prev2 == null)
			{
				return;
			}
			prev2.next = this.tail;
			this.tail.prev = prev2;
			this.cache.Remove(prev.key);
			File.Delete(prev.value);
		}

		private int maxCapacity = 10;

		private int size;

		private RiseSdk.LinkedNode head;

		private RiseSdk.LinkedNode tail;

		private Dictionary<string, RiseSdk.LinkedNode> cache;

		private const string CACHE_FILE = "filedirmeta";

		private const string SPLIT_FLAG = "@^@";

		private const string KEY_VALUE_SPLIT_FLAG = "^_^";

		private static string defFilePath = "/";

		private Coroutine writting;

		public enum FileType
		{
			Image,
			Text
		}
	}

	private class LinkedNode
	{
		public string key;

		public string value;

		public RiseSdk.LinkedNode prev;

		public RiseSdk.LinkedNode next;
	}
}
