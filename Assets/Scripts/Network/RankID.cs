using System;

namespace Network
{
	public class RankID
	{
		public RankID()
		{
			this.rule = new SynchroniseRule(60f);
			this._rankId = -1;
		}

		public void Reset()
		{
			this.rule.Reset();
			this._rankId = -1;
		}

		public void GetRankID()
		{
			if (!ServeTimeUpdate.Instance.ServerTimeValid())
			{
				return;
			}
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			ScoreRankRequest.Instance.GetUserRankIdx(ServeTimeUpdate.Instance.RankWeekString, new Action<int, object>(this.OnGetRankIDCallback));
			this.rule.Request();
		}

		private void OnGetRankIDCallback(int status, object obj)
		{
			if (status == -1 || status == 0)
			{
				return;
			}
			this.rule.hasGotInitValue = true;
			this.rankID = (int)((long)obj);
		}

		public void Synchronise(int rankId)
		{
			if (!SecondManager.Instance.hasInited)
			{
				return;
			}
			this.rule.hasGotInitValue = true;
			this.rankID = rankId;
		}

		public int rankID
		{
			get
			{
				return this._rankId;
			}
			private set
			{
				if (this._rankId == value)
				{
					return;
				}
				this._rankId = value;
				if (this.onValueChange != null)
				{
					this.onValueChange();
				}
			}
		}

		public SynchroniseRule rule { get; private set; }

		private int _rankId;

		public Action onValueChange;
	}
}
