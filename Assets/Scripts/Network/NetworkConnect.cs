using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class NetworkConnect
	{
		public static NetworkConnect Instance
		{
			get
			{
				if (NetworkConnect._instance == null)
				{
					NetworkConnect._instance = new NetworkConnect();
				}
				return NetworkConnect._instance;
			}
		}

		public string Url(NetworkConnect.RequestCommand cmd)
		{
			string text = "http://subwaysnow.17taptap.com";
			switch (cmd)
			{
			case NetworkConnect.RequestCommand.GetServerTime:
				text = "http://run.papermobi.com/index/gettime";
				break;
			case NetworkConnect.RequestCommand.GetUniqueUid:
				text += "/User/myid";
				break;
			case NetworkConnect.RequestCommand.UploadStringData:
				text += "/Appdata/commit";
				break;
			case NetworkConnect.RequestCommand.UploadJsonData:
				text += "/Appdata/hcommit";
				break;
			case NetworkConnect.RequestCommand.GetStringData:
				text += "/Appdata/get";
				break;
			case NetworkConnect.RequestCommand.GetJsonData:
				text += "/Appdata/hget";
				break;
			case NetworkConnect.RequestCommand.GetAllInnerJsonData:
				text += "/Appdata/hgetall";
				break;
			case NetworkConnect.RequestCommand.UploadScore:
				text += "/Appdata/setFixedLenRank";
				break;
			case NetworkConnect.RequestCommand.GetDynamicRankList:
				text += "/Appdata/getRankRange";
				break;
			case NetworkConnect.RequestCommand.GetRankList:
				text += "/Appdata/getRankList";
				break;
			case NetworkConnect.RequestCommand.GetUserScore:
				text += "/Appdata/getScore";
				break;
			case NetworkConnect.RequestCommand.GetUserRankIdx:
				text += "/Appdata/getLevel";
				break;
			case NetworkConnect.RequestCommand.UploadFile:
				text += "/Upload";
				break;
			case NetworkConnect.RequestCommand.GetFileUrl:
				text += "/Upload/getFile";
				break;
			case NetworkConnect.RequestCommand.GetFriendRankList:
				text += "/Appdata/getFriendsRank";
				break;
			}
			return text;
		}

		public string Uid()
		{
			string text = this.GetDeviceUid();
			if (text.Equals("tjb") || string.IsNullOrEmpty(text))
			{
				text = SystemInfo.deviceUniqueIdentifier;
				if (string.IsNullOrEmpty(text))
				{
					text = "uid" + DateTime.Now.Ticks + UnityEngine.Random.Range(1, 999);
				}
				text = text.ToLower();
				this.UpdateDeviceUid(text);
			}
			return text;
		}

		private string GetDeviceUid()
		{
			if (PlayerPrefs.HasKey("DeviceUid"))
			{
				return PlayerPrefs.GetString("DeviceUid");
			}
			PlayerPrefs.SetString("DeviceUid", "tjb");
			return "tjb";
		}

		private void UpdateDeviceUid(string uid)
		{
			PlayerPrefs.SetString("DeviceUid", uid);
		}

		public string AppId()
		{
			string empty = string.Empty;
			return "2213";
		}

		private string Stamp()
		{
			return ServeTimeUpdate.Instance.ServerTime.ToString();
		}

		private string Token(string userId)
		{
			return RiseSdk.CalculateMD5Hash(string.Concat(new object[]
			{
				userId,
				"gamesr",
				this.AppId(),
				this.Uid(),
				ServeTimeUpdate.Instance.ServerTime
			}));
		}

		private string Version()
		{
			string empty = string.Empty;
			return "1.0";
		}

		public Dictionary<string, string> GetUserLoginDict(PlatFormType platForm = PlatFormType.guest, string platUid = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["uuid"] = this.Uid();
			if (platForm != PlatFormType.guest && !string.IsNullOrEmpty(platUid))
			{
				dictionary["plat"] = platForm.ToString();
				dictionary["plat_id"] = platUid;
			}
			dictionary["version"] = this.Version();
			return dictionary;
		}

		public Dictionary<string, string> GetUploadStringDataDict(string userId, string key, string value)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["rk"] = key;
			dictionary["rv"] = value;
			dictionary["version"] = this.Version();
			dictionary["expire"] = "8640000";
			return dictionary;
		}

		public Dictionary<string, string> GetUploadJsonDataDict(string userId, string key, string json)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["rk"] = key;
			dictionary["json"] = json;
			dictionary["version"] = this.Version();
			dictionary["expire"] = "8640000";
			return dictionary;
		}

		public Dictionary<string, string> GetRequestStringDataDict(string userId, string key)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["rk"] = key;
			dictionary["version"] = this.Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestJsonDataDict(string userId, string key, string jsonKey)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["rk"] = key;
			dictionary["sk"] = jsonKey;
			dictionary["version"] = this.Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestAllJsonDataDict(string userId, string key)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["rk"] = key;
			dictionary["version"] = this.Version();
			return dictionary;
		}

		public Dictionary<string, string> GetSubmitScoreDict(string userId, string rankTag, float score, int expire = 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["rank_key"] = rankTag;
			dictionary["score"] = score.ToString();
			if (expire > 0)
			{
				dictionary["expire"] = expire.ToString();
			}
			dictionary["version"] = this.Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestRankListAroundUserDict(string userId, string rankTag, int myRankIdxUpCount, int myRankIdxDownCount, RankOrder order)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["rank_key"] = rankTag;
			dictionary["start"] = myRankIdxUpCount.ToString();
			dictionary["end"] = myRankIdxDownCount.ToString();
			Dictionary<string, string> dictionary2 = dictionary;
			string key = "order";
			int num = (int)order;
			dictionary2[key] = num.ToString();
			dictionary["version"] = this.Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestRankListDict(string userId, string rankTag, RankOrder order, int rankCount = 100)
		{
			if (rankCount < 1)
			{
				rankCount = 1;
			}
			else if (rankCount > 1000)
			{
				rankCount = 1000;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["rank_key"] = rankTag;
			Dictionary<string, string> dictionary2 = dictionary;
			string key = "order";
			int num = (int)order;
			dictionary2[key] = num.ToString();
			if (rankCount != 100)
			{
				dictionary["num"] = rankCount.ToString();
			}
			dictionary["version"] = this.Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestUserScoreDict(string userId, string rankTag)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["rank_key"] = rankTag;
			dictionary["version"] = this.Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestUserRankIdxDict(string userId, string rankTag)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["rank_key"] = rankTag;
			dictionary["version"] = this.Version();
			return dictionary;
		}

		public Dictionary<string, string> GetUploadUserFileDict(string userId, string fileName, int expireSeconds)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["file_name"] = fileName;
			dictionary["expire"] = expireSeconds.ToString();
			dictionary["version"] = this.Version();
			dictionary["expire"] = "8640000";
			return dictionary;
		}

		public Dictionary<string, string> GetRequestUserFileUrlDict(string userId, string fileName)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["file_name"] = fileName;
			dictionary["version"] = this.Version();
			return dictionary;
		}

		public Dictionary<string, string> GetRequestFriendRankListDict(string userId, string rankTag, RankOrder order, string platIds, PlatFormType platType)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["app_id"] = this.AppId();
			dictionary["user_id"] = userId;
			dictionary["uuid"] = this.Uid();
			dictionary["stamp"] = this.Stamp();
			dictionary["token"] = this.Token(userId);
			dictionary["rank_key"] = rankTag;
			Dictionary<string, string> dictionary2 = dictionary;
			string key = "order";
			int num = (int)order;
			dictionary2[key] = num.ToString();
			dictionary["fb_id"] = platIds;
			dictionary["plat"] = platType.ToString();
			dictionary["version"] = this.Version();
			return dictionary;
		}

		private static NetworkConnect _instance;

		private const string ServerUrl = "http://subwaysnow.17taptap.com";

		private const string serverTimeUrl = "http://run.papermobi.com";

		private const string expire = "8640000";

		public enum RequestCommand
		{
			GetServerTime,
			GetUniqueUid,
			UploadStringData,
			UploadJsonData,
			GetStringData,
			GetJsonData,
			GetAllInnerJsonData,
			UploadScore,
			GetDynamicRankList,
			GetRankList,
			GetUserScore,
			GetUserRankIdx,
			UploadFile,
			GetFileUrl,
			GetFriendRankList,
			GetCountryCode
		}
	}
}
