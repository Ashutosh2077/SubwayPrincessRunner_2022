using System;
using Network;
using UnityEngine;

public class TopRunCell : MonoBehaviour
{
	public void SetData(RankScreen.RankPopupType type, TopRun topRun)
	{
		this._type = type;
		this._data = topRun;
		this._info = ServerManager.Instance.GetTopRunInfo(topRun);
		this.UpdateUI();
	}

	public void UpdateUI()
	{
		if (this._data == null)
		{
			UnityEngine.Object.Destroy(base.gameObject, 0.1f);
			return;
		}
		if (this._data.rank <= 3)
		{
			this.rankSpr.enabled = true;
			this.rankLbl.enabled = false;
			if (this._data.rank == 1)
			{
				this.rankSpr.spriteName = "Rank_icon_place_first";
			}
			if (this._data.rank == 2)
			{
				this.rankSpr.spriteName = "Rank_icon_place_second";
			}
			if (this._data.rank == 3)
			{
				this.rankSpr.spriteName = "Rank_icon_place_third";
			}
		}
		else
		{
			this.rankSpr.enabled = false;
			this.rankLbl.enabled = true;
			this.rankLbl.text = this._data.rank.ToString();
		}
		this.scroeLbl.text = this._data.highestScore.ToString();
		this.RefreshPlayerName();
		this.RefreshVIP();
		this.RefreshCoutryCode();
		this.RefreshImage();
	}

	public void RefreshPlayerName()
	{
		if (this.playerNamelbl != null)
		{
			this.playerNamelbl.text = this._info.playerName;
		}
	}

	public void RefreshVIP()
	{
		if (this.vipTip != null)
		{
			this.vipTip.spriteName = ((!"yes".Equals(this._info.isVip)) ? string.Empty : "rank_vip");
		}
	}

	public void RefreshCoutryCode()
	{
		if (this.countrySpr != null)
		{
			this.countrySpr.spriteName = this._info.countryCode + "@2x";
		}
	}

	public void RefreshImage()
	{
		if (this.headTxt != null)
		{
			this.headTxt.mainTexture = ImageManager.Instance.GetTexture(this._info.pictureUrl);
		}
	}

	[SerializeField]
	private UISprite rankSpr;

	[SerializeField]
	private UILabel rankLbl;

	[SerializeField]
	private UITexture headTxt;

	[SerializeField]
	private UILabel playerNamelbl;

	[SerializeField]
	private UILabel scroeLbl;

	[SerializeField]
	private UISprite countrySpr;

	[SerializeField]
	private UISprite vipTip;

	private RankScreen.RankPopupType _type;

	private TopRun _data;

	private TopRunInfo _info;

	private const string goldSpriteName = "Rank_icon_place_first";

	private const string sliverSpriteName = "Rank_icon_place_second";

	private const string bronzeSpriteName = "Rank_icon_place_third";
}
