using System;
using UnityEngine;

public class UISlideInTopRunTip : UISlideIn
{
	public void SetupSlideTopRunTip(string tip)
	{
		base.gameObject.SetActive(true);
		this._tipLabel.text = tip;
		this.SlideIn(null);
	}

	[SerializeField]
	private UILabel _tipLabel;
}
