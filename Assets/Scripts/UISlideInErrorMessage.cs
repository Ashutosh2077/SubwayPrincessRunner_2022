using System;
using UnityEngine;

public class UISlideInErrorMessage : UISlideIn
{
	public void SetupErrorMessage(string message)
	{
		base.gameObject.SetActive(true);
		this.messageLabel.text = message;
		this.SlideIn(null);
	}

	[SerializeField]
	private UILabel messageLabel;
}
