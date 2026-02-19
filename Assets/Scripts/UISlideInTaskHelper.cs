using System;
using UnityEngine;

public class UISlideInTaskHelper : UISlideIn
{
	public void SetupSlideInTask(string message)
	{
		base.gameObject.SetActive(true);
		this.line1.text = message;
		this.line2.text = Strings.Get(LanguageKey.UI_TOP_TIP_MISSION_COMPLETE);
		this.SlideIn(null);
	}

	public void SetupSlideInAchievement(string message)
	{
		base.gameObject.SetActive(true);
		this.line1.text = message;
		this.line2.text = Strings.Get(LanguageKey.UI_TOP_TIP_ACHIEVEMENT_COMPLETE);
		this.SlideIn(null);
	}

	public UILabel line1;

	public UILabel line2;
}
