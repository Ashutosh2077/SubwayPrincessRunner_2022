using System;
using UnityEngine;

public class UIButtonGame : UIBasicButton
{
	protected override void Send()
	{
		if (this.messageType == UIButtonGame.GameMessage.StartNewRun)
		{
			if (Game.Instance != null)
			{
				Game.Instance.StartNewRun(false);
				UIScreenController.Instance.PushScreen("IngameUI");
				SaveMeManager.ResetSaveMeForNewRun();
			}
			else
			{
				UIScreenController.Instance.PushScreen("GameoverUI");
				UnityEngine.Debug.LogError("UIButtonGame:Send");
			}
		}
		else if (this.messageType == UIButtonGame.GameMessage.RestartFromPause)
		{
		}
	}

	public UIButtonGame.GameMessage messageType;

	public enum GameMessage
	{
		_notSet,
		StartNewRun,
		RestartFromPause
	}
}
