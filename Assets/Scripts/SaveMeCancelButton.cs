using System;
using UnityEngine;

public class SaveMeCancelButton : MonoBehaviour
{
	public static void CloseSaveMe()
	{
		UIScreenController.Instance.ClosePopup(null);
		Revive.Instance.SendSkipRevive();
		SaveMeManager.ResetSaveMeForNewRun();
	}

	private void OnClick()
	{
		if (this.saveMePopup != null && this.saveMePopup.getAnimationTimeLeft() < this.saveMePopup.getAnimationDuration() - 1f)
		{
			SaveMeCancelButton.CloseSaveMe();
		}
	}

	public SaveMePopup saveMePopup;
}
