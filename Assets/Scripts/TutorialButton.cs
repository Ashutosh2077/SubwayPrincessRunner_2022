using System;
using UnityEngine;

public class TutorialButton : MonoBehaviour
{
	private void OnClick()
	{
		if (this.buttonAction == TutorialButton.ButtonAction.Ok)
		{
			if (this.tutorialType == TutorialButton.TutorialPopupType.Tasks1)
			{
				UIScreenController.Instance.QueuePopup("Task_popup");
			}
			else if (this.tutorialType == TutorialButton.TutorialPopupType.Tasks2)
			{
				UIScreenController.Instance.QueuePopup("Task_popup");
			}
			else if (this.tutorialType == TutorialButton.TutorialPopupType.Facebook)
			{
				UIScreenController.Instance.PushScreen("FriendsUI");
			}
			else if (this.tutorialType == TutorialButton.TutorialPopupType.CollectFromFriends)
			{
				UIScreenController.Instance.PushScreen("FriendsUI");
			}
			else if (this.tutorialType == TutorialButton.TutorialPopupType.Helmets)
			{
				UIScreenController.Instance.QueuePopup("HelmetPopup");
				PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.helmet, 3);
			}
			else if (this.tutorialType != TutorialButton.TutorialPopupType.ChangeLog)
			{
				if (this.tutorialType == TutorialButton.TutorialPopupType.ChangeLogEndGame)
				{
					UIScreenController.Instance.QueuePopup("Task_popup");
				}
				else
				{
					UnityEngine.Debug.LogError("tutorialType was not defined in " + base.gameObject.name, base.gameObject);
				}
			}
		}
		this.SendFlurryEvent();
		UIScreenController.Instance.ClosePopup(null);
	}

	private void SendFlurryEvent()
	{
	}

	[SerializeField]
	private TutorialButton.ButtonAction buttonAction;

	private const int NUMBER_OF_FREE_HOVERBOARDS = 3;

	public TutorialButton.TutorialPopupType tutorialType;

	public enum ButtonAction
	{
		Ok,
		Cancel
	}

	public enum TutorialPopupType
	{
		_notSet,
		Tasks1,
		Tasks2,
		Facebook,
		CollectFromFriends,
		Helmets,
		ChangeLog,
		ChangeLogEndGame
	}
}
