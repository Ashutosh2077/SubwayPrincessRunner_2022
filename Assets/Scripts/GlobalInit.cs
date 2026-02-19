using System;
using UnityEngine;

public class GlobalInit : MonoBehaviour
{
	private void Awake()
	{
		if (GlobalInit.Instance == null)
		{
			GlobalInit.Instance = this;
		}
		if (PlayerPrefs.GetInt("VideoTryRoleStatus") == 1)
		{
			PlayerPrefs.SetInt("VideoTryRoleStatus", 2);
		}
		RiseSdk.Instance.Init();
		Application.targetFrameRate = 60;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Strings.Language = this.SelectLanguage();
		TasksData.LoadFiles();
		Upgrades.LoadFile();
		Characters.LoadFile();
		Chest.LoadFile();
		CharacterThemes.LoadFile();
		DailyLandingAwards.LoadFile();
		Achievements.LoadFile();
		Helmets.Load();
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "app_game_startup", 0, null);
		RiseSdk.Instance.TrackEvent("app_game_startup", "default,default");
		AudioListener.volume = 0f;
		InAppManager.Init();
		Layers.Init();
		if (!PlayerPrefs.HasKey("FistInstallDate"))
		{
			PlayerPrefs.SetInt("FistInstallDate", 1);
			PlayerInfo.Instance.firstInstallDate = DateTime.UtcNow;
			PlayerInfo.Instance.isNewPlayer = true;
			TasksManager.Instance.PlayerDidThis(TaskTarget.HaveCharacters, 1, -1);
		}
		else
		{
			PlayerInfo.Instance.CheckOnlineNewDayOpen();
		}
		if (PlayerInfo.Instance.currentCharacter == 10 && PlayerInfo.Instance.currentThemeIndex == 1)
		{
			PlayerInfo.Instance.currentCharacter = 0;
			PlayerInfo.Instance.currentThemeIndex = 0;
		}
		PlayerInfo.Instance.OpenGameAppAmount++;
		this.CheckSendSingleUserTime();
		int days = (DateTime.UtcNow.Date - PlayerInfo.Instance.lastPlayDate.Date).Days;
		if (days < 0 || days > 1)
		{
			PlayerInfo.Instance.lastPlayDate = DateTime.UtcNow;
			PlayerInfo.Instance.playDailyTimes = 0;
			PlayerInfo.Instance.openAppCountDaily = 1;
			return;
		}
		if (days == 0)
		{
			PlayerInfo.Instance.playDailyTimes += PlayerInfo.Instance.playOnceTimes;
			PlayerInfo.Instance.openAppCountDaily++;
			return;
		}
		this.CheckSendDayEverageTime();
		this.CheckPlayerLevel();
		this.CheckGameOverDoubleVideo();
		PlayerInfo.Instance.lastPlayDate = DateTime.UtcNow;
		PlayerInfo.Instance.playDailyTimes = 0;
	}

	private void Start()
	{
		RiseSdkListener.OnResumeAdEvent -= this.onResumeAd;
		RiseSdkListener.OnResumeAdEvent += this.onResumeAd;
		PlayerInfo.Instance.UpdateGameOverDoubleCoinsShowCount();
		PlayerInfo.Instance.playOnceTimes = 0;
	}

	private void CheckPlayerLevel()
	{
		if (PlayerInfo.Instance.playerLevel == 3)
		{
			RiseSdk.Instance.TrackEvent("loyal_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "loyal_players", 0, null);
			return;
		}
		bool flag = false;
		int openAppCountDaily = PlayerInfo.Instance.openAppCountDaily;
		if (openAppCountDaily == 1 && PlayerInfo.Instance.playerLevel == 0)
		{
			RiseSdk.Instance.TrackEvent("loath_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "loath_players", 0, null);
			PlayerInfo.Instance.playerLevel = 1;
			flag = true;
		}
		else if ((openAppCountDaily == 2 || openAppCountDaily == 3) && PlayerInfo.Instance.playerLevel < 2)
		{
			RiseSdk.Instance.TrackEvent("normal_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "normal_players", 0, null);
			PlayerInfo.Instance.playerLevel = 2;
			flag = true;
		}
		else if (openAppCountDaily > 3 && PlayerInfo.Instance.playerLevel < 3)
		{
			RiseSdk.Instance.TrackEvent("loyal_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "loyal_players", 0, null);
			PlayerInfo.Instance.playerLevel = 3;
			flag = true;
		}
		if (PlayerInfo.Instance.playerLevel == 3)
		{
			return;
		}
		float num = (float)PlayerInfo.Instance.playDailyTimes * 0.001f / (float)openAppCountDaily;
		if (num < 180f && PlayerInfo.Instance.playerLevel == 0)
		{
			RiseSdk.Instance.TrackEvent("loath_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "loath_players", 0, null);
			PlayerInfo.Instance.playerLevel = 1;
			flag = true;
		}
		else if (num <= 360f && PlayerInfo.Instance.playerLevel < 2)
		{
			RiseSdk.Instance.TrackEvent("normal_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "normal_players", 0, null);
			PlayerInfo.Instance.playerLevel = 2;
			flag = true;
		}
		else if (PlayerInfo.Instance.playerLevel < 3)
		{
			RiseSdk.Instance.TrackEvent("loyal_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "loyal_players", 0, null);
			PlayerInfo.Instance.playerLevel = 3;
			flag = true;
		}
		if (!flag)
		{
			if (PlayerInfo.Instance.playerLevel == 2)
			{
				RiseSdk.Instance.TrackEvent("loath_players", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "loath_players", 0, null);
			}
			else if (PlayerInfo.Instance.playerLevel == 1)
			{
				RiseSdk.Instance.TrackEvent("normal_players", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "normal_players", 0, null);
			}
		}
	}

	private void CheckGameOverDoubleVideo()
	{
		if (PlayerInfo.Instance.watchDoublePlayerLevel == 3)
		{
			RiseSdk.Instance.TrackEvent("watch_double_10_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_10_players", 0, null);
			return;
		}
		int num = PlayerInfo.Instance.UpdateSpanDays();
		if (num == 0)
		{
			return;
		}
		bool flag = false;
		PlayerInfo.Instance.CheckGameOverDoubleCoinsSpanDays();
		int num2 = PlayerInfo.Instance.gameOverDoubleCoinsShowCountLastDay;
		if (num2 == 1 && PlayerInfo.Instance.watchDoublePlayerLevel == 0)
		{
			RiseSdk.Instance.TrackEvent("watch_double_1_4_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_1_4_players", 0, null);
			PlayerInfo.Instance.watchDoublePlayerLevel = 1;
			flag = true;
		}
		else if ((num2 == 2 || num2 == 3) && PlayerInfo.Instance.watchDoublePlayerLevel < 2)
		{
			RiseSdk.Instance.TrackEvent("watch_double_5_9_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_5_9_players", 0, null);
			PlayerInfo.Instance.watchDoublePlayerLevel = 2;
			flag = true;
		}
		else if (num2 > 3 && PlayerInfo.Instance.watchDoublePlayerLevel < 3)
		{
			RiseSdk.Instance.TrackEvent("watch_double_10_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_10_players", 0, null);
			PlayerInfo.Instance.watchDoublePlayerLevel = 3;
			flag = true;
		}
		if (num <= 3 && !flag)
		{
			if (PlayerInfo.Instance.watchDoublePlayerLevel == 1)
			{
				RiseSdk.Instance.TrackEvent("watch_double_1_4_players", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_1_4_players", 0, null);
			}
			else if (PlayerInfo.Instance.watchDoublePlayerLevel == 2)
			{
				RiseSdk.Instance.TrackEvent("watch_double_5_9_players", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_5_9_players", 0, null);
			}
		}
		if (PlayerInfo.Instance.watchDoublePlayerLevel == 3 || num <= 3)
		{
			return;
		}
		num2 = PlayerInfo.Instance.LastThreeDaysGameOverDoubleCoinsCount();
		if (num2 <= 4 && num2 >= 1 && PlayerInfo.Instance.watchDoublePlayerLevel == 0)
		{
			RiseSdk.Instance.TrackEvent("watch_double_1_4_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_1_4_players", 0, null);
			PlayerInfo.Instance.watchDoublePlayerLevel = 1;
			flag = true;
		}
		else if (num2 <= 9 && PlayerInfo.Instance.watchDoublePlayerLevel < 2)
		{
			RiseSdk.Instance.TrackEvent("watch_double_5_9_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_5_9_players", 0, null);
			PlayerInfo.Instance.watchDoublePlayerLevel = 2;
			flag = true;
		}
		else if (PlayerInfo.Instance.watchDoublePlayerLevel < 3)
		{
			RiseSdk.Instance.TrackEvent("watch_double_10_players", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_10_players", 0, null);
			PlayerInfo.Instance.watchDoublePlayerLevel = 3;
			flag = true;
		}
		if (!flag)
		{
			if (PlayerInfo.Instance.watchDoublePlayerLevel == 1)
			{
				RiseSdk.Instance.TrackEvent("watch_double_1_4_players", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_1_4_players", 0, null);
			}
			else if (PlayerInfo.Instance.watchDoublePlayerLevel == 2)
			{
				RiseSdk.Instance.TrackEvent("watch_double_5_9_players", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "watch_double_5_9_players", 0, null);
			}
		}
	}

	private void CheckSendSingleUserTime()
	{
		float num = (float)PlayerInfo.Instance.playOnceTimes * 0.001f;
		if (num < 30f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_0_30s", 0, null);
		}
		else if (num < 60f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_30_60s", 0, null);
		}
		else if (num < 120f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_1_2min", 0, null);
		}
		else if (num < 180f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_2_3min", 0, null);
		}
		else if (num < 300f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_3_5min", 0, null);
		}
		else if (num < 360f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_5_6min", 0, null);
		}
		else if (num < 420f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_6_7min", 0, null);
		}
		else if (num >= 420f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_single_7_min", 0, null);
		}
	}

	private void CheckSendDayEverageTime()
	{
		float num = (float)PlayerInfo.Instance.playDailyTimes * 0.001f;
		if (num < 60f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_per_day_0_1min", 0, null);
		}
		else if (num < 180f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_per_day_1_3min", 0, null);
		}
		else if (num < 600f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_per_day_3_10min", 0, null);
		}
		else if (num < 1800f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_per_day_10_30min", 0, null);
		}
		else if (num > 1800f)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "average_use_time_per_day_30_min", 0, null);
		}
	}

	private string SelectLanguage()
	{
		UnityEngine.Debug.Log(Application.systemLanguage);
		Strings.documentFormat = this.documentFormat;
		if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified)
		{
			return "chinese";
		}
		if (Application.systemLanguage == SystemLanguage.ChineseTraditional)
		{
			return "chinese_traditional";
		}
		return "english";
	}

	private void onResumeAd()
	{
		Game.Instance.showAdTime = Time.time;
		RiseSdk.Instance.TrackEvent("interstitial_lockscreen", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_lockscreen", 0, null);
	}

	private void OnDisable()
	{
		int playOnceTimes = (int)(Time.realtimeSinceStartup * 1000f);
		PlayerInfo.Instance.playOnceTimes = playOnceTimes;
		PlayerInfo.Instance.CalcOnlineTotalSeconds();
	}

	private const string English = "english";

	private const string ChineseSimplified = "chinese";

	private const string ChineseTraditional = "chinese_traditional";

	public bool debug;

	public string cityScenename;

	public string[] citiesScenename;

	public string languageKey = "english";

	public Strings.DocumentFormat documentFormat;

	public static GlobalInit Instance;
}
