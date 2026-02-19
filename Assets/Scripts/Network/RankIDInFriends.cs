using System;

namespace Network
{
	public class RankIDInFriends
	{
		public RankIDInFriends()
		{
			this._rankId = -1;
		}

		public void Synchronise(int rankId)
		{
			this.rankID = rankId;
		}

		public int rankID
		{
			get
			{
				return this._rankId;
			}
			set
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

		private int _rankId;

		public Action onValueChange;
	}
}
