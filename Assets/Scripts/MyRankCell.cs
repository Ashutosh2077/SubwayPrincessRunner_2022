using System;
using Network;
using UnityEngine;

public class MyRankCell : MonoBehaviour
{
	private void Awake()
	{
		this.rankLbl.text = "--";
		this.scoreLbl.text = "-----";
	}

	private void OnEnable()
	{
		ServerManager.Instance.RegisterOnPictrueUrlChange(new Action(this.RefreshHead));
		ServerManager.Instance.RequestPictrueUrl();
		ServerManager.Instance.RegisterOnScoreChange(new Action(this.RefreshScore));
		ServerManager.Instance.RequestScore();
		ServerManager.Instance.RegisterOnRankIDChange(new Action(this.RefreshRankID));
		ServerManager.Instance.RequestRankID();
		ServerManager.Instance.RegisterOnCountryCodeChange(new Action(this.RefreshCountryCode));
		ServerManager.Instance.RequestCountryCode();
		ServerManager.Instance.ResetMyTopRunInfo();
	}

	private void OnDisable()
	{
		ServerManager.Instance.UnregisterOnPictrueUrlChange(new Action(this.RefreshHead));
		ServerManager.Instance.UnregisterOnScoreChange(new Action(this.RefreshScore));
		ServerManager.Instance.UnregisterOnRankIDChange(new Action(this.RefreshRankID));
		ServerManager.Instance.UnregisterOnCountryCodeChange(new Action(this.RefreshCountryCode));
	}

	public void UpdateUI()
	{
		this.RefreshScore();
		this.RefreshCountryCode();
		this.RefreshSubscription();
		this.RefreshRankID();
		this.RefreshHead();
	}

	private void RefreshRankID()
	{
		RankID rankID_Week = ServerManager.Instance.RankID_Week;
		int num = -1;
		if (rankID_Week != null)
		{
			num = rankID_Week.rankID;
		}
		if (num == -1)
		{
			this.rankLbl.text = "--";
			return;
		}
		this.rankLbl.text = num.ToString();
	}

	private void RefreshScore()
	{
		Score score_Week = ServerManager.Instance.Score_Week;
		if (score_Week != null)
		{
			this.scoreLbl.text = score_Week.score.ToString();
		}
	}

	private void RefreshCountryCode()
	{
		CountryCode countryCode = ServerManager.Instance.CountryCode;
		if (countryCode != null)
		{
			this.coutrySpr.spriteName = countryCode.Value + "@2x";
		}
	}

	private void RefreshSubscription()
	{
		this.vipSpr.spriteName = ((!PlayerInfo.Instance.hasSubscribed) ? string.Empty : "rank_vip");
	}

	private void RefreshHead()
	{
		PictureUrl pictureUrl = ServerManager.Instance.PictureUrl;
		if (pictureUrl != null)
		{
			this.headTxt.mainTexture = pictureUrl.Image;
		}
	}

	[SerializeField]
	private UILabel rankLbl;

	[SerializeField]
	private UILabel scoreLbl;

	[SerializeField]
	private UITexture headTxt;

	[SerializeField]
	private UISprite vipSpr;

	[SerializeField]
	private UISprite coutrySpr;
}
