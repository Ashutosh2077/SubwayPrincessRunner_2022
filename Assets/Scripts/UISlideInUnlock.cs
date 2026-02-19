using System;
using UnityEngine;

public class UISlideInUnlock : UISlideIn
{
	public void SetupSlideInUnlock(string message)
	{
		this.youUnlockLbl.text = Strings.Get(LanguageKey.UI_TOP_TIP_YOU_UNLOCK);
		base.gameObject.SetActive(true);
		this.UnlockName.text = message;
		this.SlideIn(null);
	}

	public UILabel youUnlockLbl;

	public UILabel UnlockName;
}
