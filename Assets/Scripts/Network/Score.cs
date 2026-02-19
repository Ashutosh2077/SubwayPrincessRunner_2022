using System;
using UnityEngine;

namespace Network
{
	public class Score
	{
		public Score()
		{
			this.rule = new SynchroniseRule(60f);
			this._score = PlayerPrefs.GetInt("Network_Score", 0);
			this._scoreTmp = this._score;
			this._rankKey = ServeTimeUpdate.Instance.RankWeekString;
		}

		public void GetScore()
		{
			if (!ServeTimeUpdate.Instance.ServerTimeValid())
			{
				return;
			}
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			this._rankKey = ServeTimeUpdate.Instance.RankWeekString;
			ScoreRankRequest.Instance.GetUserScore(this._rankKey, new Action<int, object>(this.OnGetScoreCallback));
			this.rule.Request();
		}

		private void OnGetScoreCallback(int status, object obj)
		{
			if (status == -1 || status == 0)
			{
				return;
			}
			this.score = (int)((long)obj);
			this._scoreTmp = this._score;
			this.rule.hasGotInitValue = true;
		}

		public void UploadScore(int score)
		{
			if (!ServeTimeUpdate.Instance.ServerTimeValid())
			{
				return;
			}
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			if (!ServeTimeUpdate.Instance.RankWeekString.Equals(this._rankKey))
			{
				this.score = 0;
				this._scoreTmp = 0;
				this._rankKey = ServeTimeUpdate.Instance.RankWeekString;
			}
			if (this._scoreTmp >= score)
			{
				return;
			}
			this._scoreTmp = score;
			ScoreRankRequest.Instance.UploadScore(this._rankKey, (float)score, new Action<int, object>(this.OnUploadScoreCallback));
		}

		private void OnUploadScoreCallback(int status, object obj)
		{
			if (status == -1 || status == 0)
			{
				return;
			}
			this.rule.hasGotInitValue = true;
			this.score = this._scoreTmp;
			RankID rankID_Week = ServerManager.Instance.RankID_Week;
			if (rankID_Week != null)
			{
				rankID_Week.GetRankID();
			}
		}

		public void Synchronise(int score)
		{
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			this.rule.hasGotInitValue = true;
			this.score = score;
		}

		public int score
		{
			get
			{
				return this._score;
			}
			private set
			{
				if (this._score == value)
				{
					return;
				}
				this._score = value;
				PlayerPrefs.SetInt("Network_Score", this._score);
				if (this.onValueChange != null)
				{
					this.onValueChange();
				}
			}
		}

		public SynchroniseRule rule { get; private set; }

		private int _score;

		private int _scoreTmp;

		private string _rankKey;

		public Action onValueChange;
	}
}
