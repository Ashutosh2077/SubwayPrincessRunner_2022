using System;
using UnityEngine;

public class DailyLandingHelp : MonoBehaviour
{
	public void Init(int dayId)
	{
		this.dayIndex = dayId;
		this.award = DailyLandingAwards.GetDailyLandingAwardByID(dayId);
		if (this.award == null)
		{
			return;
		}
		if (this.award.type != DailyLandingAward.DailyLandingRewardType.Chest)
		{
			this.rewardLbl.text = this.award.Amount.ToString();
		}
		else
		{
			this.rewardLbl.enabled = false;
		}
	}

	public void Refresh()
	{
		bool flag;
		int num = Mathf.Clamp(PlayerInfo.Instance.GetDailyLandingDaysInRow(out flag), 0, DailyLandingAwards.awards.Length);
		if (flag)
		{
			num--;
		}
		if (this.dayIndex - 1 == num)
		{
			this.dayLbl.text = Strings.Get(LanguageKey.DAILY_CHALLENGE_TODAY_2);
			this.dayLbl.color = UIPosScalesAndNGUIAtlas.Instance.dayLblActive;
			this.rewardLbl.color = UIPosScalesAndNGUIAtlas.Instance.rewardLblActive;
			this.checkMskSpr.enabled = flag;
			this.backgroundInactive.enabled = false;
			this.backgroundActive.enabled = true;
		}
		else if (this.dayIndex - 1 < num)
		{
			this.dayLbl.text = string.Format(Strings.Get(LanguageKey.DAILY_CHALLENGE_DAY), this.dayIndex);
			this.dayLbl.color = UIPosScalesAndNGUIAtlas.Instance.dayLblActive;
			this.rewardLbl.color = UIPosScalesAndNGUIAtlas.Instance.rewardLblActive;
			this.checkMskSpr.enabled = true;
			this.backgroundInactive.enabled = false;
			this.backgroundActive.enabled = true;
		}
		else
		{
			this.dayLbl.text = string.Format(Strings.Get(LanguageKey.DAILY_CHALLENGE_DAY), this.dayIndex);
			this.dayLbl.color = UIPosScalesAndNGUIAtlas.Instance.dayLblInactive;
			this.rewardLbl.color = UIPosScalesAndNGUIAtlas.Instance.rewardLblInactive;
			this.checkMskSpr.enabled = false;
			this.backgroundInactive.enabled = true;
			this.backgroundActive.enabled = false;
		}
	}

	[SerializeField]
	private int dayIndex;

	[SerializeField]
	private UILabel dayLbl;

	[SerializeField]
	private UILabel rewardLbl;

	[SerializeField]
	private UISprite checkMskSpr;

	[SerializeField]
	private UISprite backgroundInactive;

	[SerializeField]
	private UISprite backgroundActive;

	[SerializeField]
	private UISprite rewardIconSpr;

	private DailyLandingAward award;
}
