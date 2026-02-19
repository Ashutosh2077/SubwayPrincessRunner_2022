using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class ScoreRankRequest
	{
		public static ScoreRankRequest Instance
		{
			get
			{
				if (ScoreRankRequest._instance == null)
				{
					ScoreRankRequest._instance = new ScoreRankRequest();
				}
				return ScoreRankRequest._instance;
			}
		}

		public void UploadScore(string key, float score, Action<int, object> handle = null)
		{
			ScoreRankRequest.RankData @object = new ScoreRankRequest.RankData(SecondManager.Instance.userId, key, handle);
			Dictionary<string, string> submitScoreDict = NetworkConnect.Instance.GetSubmitScoreDict(SecondManager.Instance.userId, key, score, 0);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.UploadScore, submitScoreDict, new Action<string>(@object.UploadListener));
		}

		public void GetUserScore(string key, Action<int, object> handle = null)
		{
			ScoreRankRequest.RankData @object = new ScoreRankRequest.RankData(SecondManager.Instance.userId, key, handle);
			Dictionary<string, string> requestUserScoreDict = NetworkConnect.Instance.GetRequestUserScoreDict(SecondManager.Instance.userId, key);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetUserScore, requestUserScoreDict, new Action<string>(@object.GetListener));
		}

		public void GetUserRankIdx(string key, Action<int, object> handle = null)
		{
			ScoreRankRequest.RankData @object = new ScoreRankRequest.RankData(SecondManager.Instance.userId, key, handle);
			Dictionary<string, string> requestUserRankIdxDict = NetworkConnect.Instance.GetRequestUserRankIdxDict(SecondManager.Instance.userId, key);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetUserRankIdx, requestUserRankIdxDict, new Action<string>(@object.GetListener));
		}

		public void GetRankList(string key, int rankCount, RankOrder order = RankOrder.Desc, Action<int, object> handle = null)
		{
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			if (!ServeTimeUpdate.Instance.ServerTimeValid())
			{
				return;
			}
			ScoreRankRequest.RankData @object = new ScoreRankRequest.RankData(SecondManager.Instance.userId, key, handle);
			Dictionary<string, string> requestRankListDict = NetworkConnect.Instance.GetRequestRankListDict(SecondManager.Instance.userId, key, order, rankCount);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetRankList, requestRankListDict, new Action<string>(@object.GetListener));
		}

		public void GetRequestRankListAroundUser(string key, int start, int end, RankOrder order = RankOrder.Desc, Action<int, object> handle = null)
		{
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			if (!ServeTimeUpdate.Instance.ServerTimeValid())
			{
				return;
			}
			ScoreRankRequest.RankData @object = new ScoreRankRequest.RankData(SecondManager.Instance.userId, key, handle);
			Dictionary<string, string> requestRankListAroundUserDict = NetworkConnect.Instance.GetRequestRankListAroundUserDict(SecondManager.Instance.userId, key, start, end, order);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetDynamicRankList, requestRankListAroundUserDict, new Action<string>(@object.GetListener));
		}

		public void GetFriendRankList(string key, PlatFormType platType, string platIds, RankOrder order = RankOrder.Desc, Action<int, object> handle = null)
		{
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			if (!ServeTimeUpdate.Instance.ServerTimeValid())
			{
				return;
			}
			ScoreRankRequest.RankData @object = new ScoreRankRequest.RankData(SecondManager.Instance.userId, key, handle);
			Dictionary<string, string> requestFriendRankListDict = NetworkConnect.Instance.GetRequestFriendRankListDict(SecondManager.Instance.userId, key, order, platIds, platType);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetFriendRankList, requestFriendRankListDict, new Action<string>(@object.GetListener));
		}

		private static ScoreRankRequest _instance;

		public class RankData
		{
			public RankData(string userId, string key, Action<int, object> onSuccess)
			{
				this.userId = userId;
				this.key = key;
				this.onRespondSuccessed = onSuccess;
			}

			public void UploadListener(string s)
			{
				UnityEngine.Debug.Log("UploadScoreListener:" + s);
				if (string.IsNullOrEmpty(s) || !s.Contains("status"))
				{
					if (this.onRespondSuccessed != null)
					{
						this.onRespondSuccessed(-1, "error message");
					}
					return;
				}
				IDictionary<string, object> dictionary = RiseJson.Deserialize(s) as IDictionary<string, object>;
				if ((int)((long)dictionary["status"]) == 0)
				{
					if (this.onRespondSuccessed != null)
					{
						this.onRespondSuccessed(0, dictionary["msg"]);
					}
					return;
				}
				if (this.onRespondSuccessed != null)
				{
					this.onRespondSuccessed(1, null);
				}
			}

			public void GetListener(string s)
			{
				UnityEngine.Debug.Log("GetListener:" + s);
				if (string.IsNullOrEmpty(s) || !s.Contains("status"))
				{
					if (this.onRespondSuccessed != null)
					{
						this.onRespondSuccessed(-1, "error message");
					}
					return;
				}
				IDictionary<string, object> dictionary = RiseJson.Deserialize(s) as IDictionary<string, object>;
				if ((int)((long)dictionary["status"]) == 0)
				{
					if (this.onRespondSuccessed != null)
					{
						this.onRespondSuccessed(0, dictionary["msg"]);
					}
					return;
				}
				if (!dictionary.ContainsKey("data"))
				{
					if (this.onRespondSuccessed != null)
					{
						this.onRespondSuccessed(-1, "no data");
					}
					return;
				}
				if (this.onRespondSuccessed != null)
				{
					this.onRespondSuccessed(1, dictionary["data"]);
				}
			}

			public string userId;

			public string key;

			public Action<int, object> onRespondSuccessed;
		}
	}
}
