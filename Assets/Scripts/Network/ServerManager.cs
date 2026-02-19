using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class ServerManager
	{
		public static ServerManager Instance
		{
			get
			{
				if (ServerManager._instance == null)
				{
					ServerManager._instance = new ServerManager();
				}
				return ServerManager._instance;
			}
		}

		public TopRunInfo GetTopRunInfo(TopRun topRun)
		{
			return this.GetTopRunInfo(topRun.userId);
		}

		public TopRunInfo GetTopRunInfo(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				return new TopRunInfo();
			}
			if (this.allTopRunDatas.ContainsKey(userId))
			{
				return this.allTopRunDatas[userId];
			}
			TopRunInfo topRunInfo = new TopRunInfo();
			this.allTopRunDatas.Add(userId, topRunInfo);
			return topRunInfo;
		}

		public TopRunInfo ResetMyTopRunInfo()
		{
			if (this.currentServerData == null)
			{
				return new TopRunInfo();
			}
			TopRunInfo topRunInfo = this.GetTopRunInfo(SecondManager.Instance.userId);
			topRunInfo.countryCode = this.currentServerData.countryCode.Value;
			topRunInfo.playerName = this.currentServerData.playerName.Value;
			topRunInfo.isVip = this.currentServerData.subscription.Value;
			topRunInfo.pictureUrl = this.currentServerData.pictureUrl.Value;
			return topRunInfo;
		}

		public void AddGlobalTopRun(TopRun topRun)
		{
			if (topRun == null)
			{
				return;
			}
			if (!this.globalDatas.ContainsKey(topRun.userId))
			{
				this.globalDatas.Add(topRun.userId, topRun);
			}
		}

		public TopRun GetGlobalTopRun(string userId)
		{
			if (this.globalDatas.ContainsKey(userId))
			{
				return this.globalDatas[userId];
			}
			return null;
		}

		public void AddFriendTopRun(TopRun topRun)
		{
			if (topRun == null)
			{
				return;
			}
			if (!this.friendDatas.ContainsKey(topRun.userId))
			{
				this.friendDatas.Add(topRun.userId, topRun);
			}
		}

		public TopRun GetFriendTopRun(string userId)
		{
			if (this.friendDatas.ContainsKey(userId))
			{
				return this.friendDatas[userId];
			}
			return null;
		}

		public void AddVipTopRun(TopRun topRun)
		{
			if (topRun == null)
			{
				return;
			}
			if (!this.vipDatas.ContainsKey(topRun.userId))
			{
				this.vipDatas.Add(topRun.userId, topRun);
			}
		}

		public TopRun GetVipTopRun(string userId)
		{
			if (this.vipDatas.ContainsKey(userId))
			{
				return this.vipDatas[userId];
			}
			return null;
		}

		public void RequestUserInformation(PlatFormType platForm, string userId, MonoBehaviour mono)
		{
			if (!ServeTimeUpdate.Instance.ChechCanRequestDatas())
			{
				return;
			}
			if (this.serverDatas.ContainsKey(userId))
			{
				this.currentServerData = this.serverDatas[userId];
				return;
			}
			ServerData serverData = new ServerData();
			serverData.playerName.onValueChange = this.OnPlayerNameChange;
			serverData.medals.onValueChange = this.OnMedalsChange;
			serverData.score_week.onValueChange = this.OnScoreChange;
			serverData.rankID_week.onValueChange = this.OnRankIDChange;
			serverData.countryCode.onValueChange = this.OnCountryCodeChange;
			serverData.pictureUrl.onDownloadImageSuccess = this.OnPictureURLChange;
			this.serverDatas.Add(userId, serverData);
			this.currentServerData = serverData;
			if (platForm == PlatFormType.guest)
			{
				mono.StartCoroutine(serverData.RequestGuest());
			}
			else if (platForm == PlatFormType.facebook)
			{
				mono.StartCoroutine(serverData.RequestFacebook());
			}
		}

		public PlayerName PlayerName
		{
			get
			{
				if (this.currentServerData == null)
				{
					return null;
				}
				return this.currentServerData.playerName;
			}
		}

		public bool CanUploadPlayerName()
		{
			return this.currentServerData != null && this.currentServerData.CanUploadPlayerName();
		}

		public void RequestPlayerName()
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.RequestPlayerName();
		}

		public void SynchronisePlayerName(string playerName)
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.SynchronisePlayerName(playerName);
		}

		public void UploadPlayerName(string playerName)
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.UploadPlayerName_Strict(playerName);
		}

		public void RegisterOnPlayerNameChange(Action onPlayerNameChange)
		{
			this.OnPlayerNameChange = (Action)Delegate.Remove(this.OnPlayerNameChange, onPlayerNameChange);
			this.OnPlayerNameChange = (Action)Delegate.Combine(this.OnPlayerNameChange, onPlayerNameChange);
		}

		public void UnregisterOnPlayerNameChange(Action onPlayerNameChange)
		{
			this.OnPlayerNameChange = (Action)Delegate.Remove(this.OnPlayerNameChange, onPlayerNameChange);
		}

		public void UploadFacebookID(string facebookID)
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.UploadFacebookID(facebookID);
		}

		public Medals Medals
		{
			get
			{
				if (this.currentServerData == null)
				{
					return null;
				}
				return this.currentServerData.medals;
			}
		}

		public void RequestMedals()
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.RequestMedals();
		}

		public void RegisterOnMedalsChange(Action onMedalsChange)
		{
			this.OnMedalsChange = (Action)Delegate.Remove(this.OnMedalsChange, onMedalsChange);
			this.OnMedalsChange = (Action)Delegate.Combine(this.OnMedalsChange, onMedalsChange);
		}

		public void UnregisterOnMedalsChange(Action onMedalsChange)
		{
			this.OnMedalsChange = (Action)Delegate.Remove(this.OnMedalsChange, onMedalsChange);
		}

		public Score Score_Week
		{
			get
			{
				if (this.currentServerData == null)
				{
					return null;
				}
				return this.currentServerData.score_week;
			}
		}

		public void RequestScore()
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.RequestScore();
		}

		public void UploadScore(int score)
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.UploadScore(score);
		}

		public void SynchroniseScore(int score)
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.SynchroniseScore(score);
		}

		public void RegisterOnScoreChange(Action onScoreChange)
		{
			this.OnScoreChange = (Action)Delegate.Remove(this.OnScoreChange, onScoreChange);
			this.OnScoreChange = (Action)Delegate.Combine(this.OnScoreChange, onScoreChange);
		}

		public void UnregisterOnScoreChange(Action onScoreChange)
		{
			this.OnScoreChange = (Action)Delegate.Remove(this.OnScoreChange, onScoreChange);
		}

		public void UploadScoreVip(float score)
		{
			if (this.currentServerData == null)
			{
				return;
			}
			ScoreRankRequest.Instance.UploadScore("vipRankKey", score, null);
		}

		public void UploadScoreGlobal(float score)
		{
			if (this.currentServerData == null)
			{
				return;
			}
			ScoreRankRequest.Instance.UploadScore("facebookRankKey", score, null);
		}

		public RankID RankID_Week
		{
			get
			{
				if (this.currentServerData == null)
				{
					return null;
				}
				return this.currentServerData.rankID_week;
			}
		}

		public void RequestRankID()
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.RequestRankID();
		}

		public void SynchroniseRankID(int rankId)
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.SynchroniseRankID(rankId);
		}

		public void RegisterOnRankIDChange(Action onRankIDChange)
		{
			this.OnRankIDChange = (Action)Delegate.Remove(this.OnRankIDChange, onRankIDChange);
			this.OnRankIDChange = (Action)Delegate.Combine(this.OnRankIDChange, onRankIDChange);
		}

		public void UnregisterOnRankIDChange(Action onRankIDChange)
		{
			this.OnRankIDChange = (Action)Delegate.Remove(this.OnRankIDChange, onRankIDChange);
		}

		public CountryCode CountryCode
		{
			get
			{
				if (this.currentServerData == null)
				{
					return null;
				}
				return this.currentServerData.countryCode;
			}
		}

		public void RequestCountryCode()
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.RequestCountryCode();
		}

		public void SynchroniseCountryCode(string countryCode)
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.SynchroniseCountryCode(countryCode);
		}

		public void RegisterOnCountryCodeChange(Action onCountryCodeChange)
		{
			this.OnCountryCodeChange = (Action)Delegate.Remove(this.OnCountryCodeChange, onCountryCodeChange);
			this.OnCountryCodeChange = (Action)Delegate.Combine(this.OnCountryCodeChange, onCountryCodeChange);
		}

		public void UnregisterOnCountryCodeChange(Action onCountryCodeChange)
		{
			this.OnCountryCodeChange = (Action)Delegate.Remove(this.OnCountryCodeChange, onCountryCodeChange);
		}

		public void RequestSubscription()
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.RequestSubscription();
		}

		public void UploadSubscription(string value)
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.UploadSubscription(value);
		}

		public PictureUrl PictureUrl
		{
			get
			{
				if (this.currentServerData == null)
				{
					return null;
				}
				return this.currentServerData.pictureUrl;
			}
		}

		public void RequestPictrueUrl()
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.RequestPictrueUrl();
		}

		public void UploadPictrueUrl(string url)
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.UploadPictrueUrl(url);
		}

		public void SynchronisePictrueUrl(string url)
		{
			if (this.currentServerData == null)
			{
				return;
			}
			this.currentServerData.SynchronisePictrueUrl(url);
		}

		public void RegisterOnPictrueUrlChange(Action onDownloadImageSuccess)
		{
			this.OnPictureURLChange = (Action)Delegate.Remove(this.OnPictureURLChange, onDownloadImageSuccess);
			this.OnPictureURLChange = (Action)Delegate.Combine(this.OnPictureURLChange, onDownloadImageSuccess);
		}

		public void UnregisterOnPictrueUrlChange(Action onDownloadImageSuccess)
		{
			this.OnPictureURLChange = (Action)Delegate.Remove(this.OnPictureURLChange, onDownloadImageSuccess);
		}

		private static ServerManager _instance;

		public Dictionary<string, ServerData> serverDatas = new Dictionary<string, ServerData>();

		public Dictionary<string, TopRunInfo> allTopRunDatas = new Dictionary<string, TopRunInfo>();

		public Dictionary<string, TopRun> globalDatas = new Dictionary<string, TopRun>();

		public Dictionary<string, TopRun> friendDatas = new Dictionary<string, TopRun>();

		public Dictionary<string, TopRun> vipDatas = new Dictionary<string, TopRun>();

		private ServerData currentServerData;

		public Action OnPlayerNameChange;

		public Action OnMedalsChange;

		public Action OnScoreChange;

		public Action OnRankIDChange;

		public Action OnCountryCodeChange;

		public Action OnPictureURLChange;
	}
}
