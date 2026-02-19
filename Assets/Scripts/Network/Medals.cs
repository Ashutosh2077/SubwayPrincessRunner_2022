using System;
using System.Collections.Generic;

namespace Network
{
	public class Medals : KeyJsonValue
	{
		public Medals()
		{
			this._key = "medals";
			this._gold = 0;
			this._sliver = 0;
			this._copper = 0;
			this._rankTag = "201904";
		}

		public override void Reset()
		{
			base.Reset();
			this._gold = 0;
			this._sliver = 0;
			this._copper = 0;
			this._rankTag = "201904";
		}

		protected override void Parse(object obj)
		{
			if (obj == null)
			{
				this._goldTmp = PlayerInfo.Instance.TopRunData.hasGoldMedal;
				this._sliverTmp = PlayerInfo.Instance.TopRunData.hasSliverMedal;
				this._copperTmp = PlayerInfo.Instance.TopRunData.hasBronzeMedal;
				this._rankTagTmp = ServeTimeUpdate.Instance.RankWeekString;
				base.UploadJson(string.Format("{{\"g\":{0},\"s\":{1},\"c\":{2},\"w\":\"{3}\"}}", new object[]
				{
					this._goldTmp,
					this._sliverTmp,
					this._copperTmp,
					this._rankTagTmp
				}));
				return;
			}
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary.ContainsKey("g"))
			{
				this._gold = (int)((long)dictionary["g"]);
			}
			if (dictionary.ContainsKey("s"))
			{
				this._sliver = (int)((long)dictionary["s"]);
			}
			if (dictionary.ContainsKey("c"))
			{
				this._copper = (int)((long)dictionary["c"]);
			}
			if (dictionary.ContainsKey("w"))
			{
				this._rankTag = (string)dictionary["w"];
			}
			if (!this._rankTag.Equals(ServeTimeUpdate.Instance.RankWeekString))
			{
				ScoreRankRequest.Instance.GetUserRankIdx(this._rankTag, new Action<int, object>(this.GetUserRankIdxHandle));
			}
		}

		private void GetUserRankIdxHandle(int status, object obj)
		{
			if (status == -1)
			{
				return;
			}
			if (status == 0)
			{
				this._rankTagTmp = ServeTimeUpdate.Instance.RankWeekString;
				base.UploadJson(string.Format("{{\"g\":{0},\"s\":{1},\"c\":{2},\"w\":\"{3}\"}}", new object[]
				{
					this._gold,
					this._sliver,
					this._copper,
					this._rankTagTmp
				}));
				return;
			}
			this._goldTmp = this._gold;
			this._sliverTmp = this._sliver;
			this._copperTmp = this._copper;
			int num = (int)((long)obj);
			if (num <= 3)
			{
				this._goldTmp++;
			}
			else if (num <= 10)
			{
				this._sliverTmp++;
			}
			else if (num <= 50)
			{
				this._copperTmp++;
			}
			this._rankTagTmp = ServeTimeUpdate.Instance.RankWeekString;
			base.UploadJson(string.Format("{{\"g\":{0},\"s\":{1},\"c\":{2},\"w\":\"{3}\"}}", new object[]
			{
				this._goldTmp,
				this._sliverTmp,
				this._copperTmp,
				this._rankTagTmp
			}));
		}

		protected override void UploadCallback(int status, object obj)
		{
			if (status == -1)
			{
				return;
			}
			if (status == 0)
			{
				return;
			}
			this._gold = this._goldTmp;
			this._sliver = this._sliverTmp;
			this._copper = this._copperTmp;
			this._rankTag = this._rankTagTmp;
			this._rule.hasGotInitValue = true;
		}

		protected override void OnValueChange()
		{
			PlayerInfo.Instance.TopRunData.hasGoldMedal = this._gold;
			PlayerInfo.Instance.TopRunData.hasSliverMedal = this._sliver;
			PlayerInfo.Instance.TopRunData.hasBronzeMedal = this._copper;
			PlayerInfo.Instance.TopRunData.weekstring = this._rankTag;
			base.OnValueChange();
		}

		protected override string ToJson()
		{
			return string.Format("{{\"g\":{0},\"s\":{1},\"c\":{2},\"w\":\"{3}\"}}", new object[]
			{
				this._gold,
				this._sliver,
				this._copper,
				this._rankTag
			});
		}

		private const string Format = "{{\"g\":{0},\"s\":{1},\"c\":{2},\"w\":\"{3}\"}}";

		public int _gold;

		public int _sliver;

		public int _copper;

		private string _rankTag;

		private int _goldTmp;

		private int _sliverTmp;

		private int _copperTmp;

		private string _rankTagTmp;
	}
}
