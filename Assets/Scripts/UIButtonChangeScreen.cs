using System;
using UnityEngine;

[AddComponentMenu("GUI/Interaction/Change Screen Button")]
public class UIButtonChangeScreen : UIBasicButton
{
	protected override void Send()
	{
		if (this.useSend)
		{
			if (this.ScreenNameToOpen == "PauseUI")
			{
				Game.Instance.wasButtonClicked = true;
			}
			if (base.enabled && base.gameObject.activeInHierarchy)
			{
				if (string.IsNullOrEmpty(this.ScreenNameToOpen) && (this.screenChangeType == UIButtonChangeScreen.ScreenChangeType.PushScreen || this.screenChangeType == UIButtonChangeScreen.ScreenChangeType.SwitchScreen || this.screenChangeType == UIButtonChangeScreen.ScreenChangeType.QueuePopup))
				{
					UnityEngine.Debug.LogError(base.name + " tried to send an empty Change Screen message");
				}
				UIScreenController instance = UIScreenController.Instance;
				if (instance == null)
				{
					return;
				}
				if (this.screenChangeType == UIButtonChangeScreen.ScreenChangeType.PushScreen)
				{
					instance.PushScreen(this.ScreenNameToOpen);
				}
				else if (this.screenChangeType == UIButtonChangeScreen.ScreenChangeType.SwitchScreen)
				{
					instance.SwitchScreen(this.ScreenNameToOpen);
				}
				else if (this.screenChangeType == UIButtonChangeScreen.ScreenChangeType.QueuePopup)
				{
					instance.QueuePopup(this.ScreenNameToOpen);
				}
				else if (this.screenChangeType == UIButtonChangeScreen.ScreenChangeType.PushPopup)
				{
					instance.PushPopup(this.ScreenNameToOpen);
				}
				if (this.screenChangeType == UIButtonChangeScreen.ScreenChangeType.ClosePopup)
				{
					instance.ClosePopup(this.ScreenNameToOpen);
				}
			}
		}
	}

	public UIButtonChangeScreen.ScreenChangeType screenChangeType;

	public string ScreenNameToOpen;

	private bool useSend = true;

	public enum ScreenChangeType
	{
		PushScreen,
		SwitchScreen,
		None,
		QueuePopup,
		ClosePopup,
		PushPopup
	}
}
