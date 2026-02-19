using System;
using UnityEngine;

public class SaveMeButton : MonoBehaviour
{
	public void CenterUnit(Transform targetTransform, float aditionalOffset = 0f)
	{
		Vector3 zero = Vector3.zero;
		targetTransform.localPosition = zero;
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(targetTransform);
		zero.x = zero.x - bounds.extents.x / 2f + aditionalOffset;
		targetTransform.localPosition = zero;
	}

	public void WatchVideoOnClick()
	{
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
		RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
		RiseSdk.Instance.TrackEvent("click_video_saveme", "default,default");
		IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_saveme", 0, null);
		Game.Instance.wasButtonClicked = true;
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(3);
			}
			else
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
			}
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
	}

	public void OnClick()
	{
		Game.Instance.wasButtonClicked = true;
		this.cancleCollider.enabled = false;
		int numberOfKeysToSaveMe = SaveMeManager.GetNumberOfKeysToSaveMe();
		if (numberOfKeysToSaveMe <= PlayerInfo.Instance.amountOfKeys)
		{
			PlayerInfo.Instance.amountOfKeys -= numberOfKeysToSaveMe;
			SaveMeButton.ReviveGameSuc();
		}
		else
		{
			SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME = true;
			SaveMeManager.IS_PURCHASE_RUNNING_INGAME = true;
			PurchaseHandler.Instance.PurchaseKeysIfNeeded(numberOfKeysToSaveMe);
		}
	}

	public void OnFreeReviveClick()
	{
		UIScreenController instance = UIScreenController.Instance;
		IngameScreen ingameScreen = instance.GetScreenFromCache(instance.GetTopScreenName()) as IngameScreen;
		if (ingameScreen == null)
		{
			UnityEngine.Debug.LogError("IngameScreen == NULL");
		}
		else
		{
			ingameScreen.SetPauseButtonVisibility(true);
		}
		Revive.Instance.SendRevive();
		UIScreenController.Instance.ClosePopup(null);
		SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME = false;
		SaveMeManager.IS_PURCHASE_RUNNING_INGAME = false;
	}

	public static void ReviveGameSuc()
	{
		UIScreenController instance = UIScreenController.Instance;
		IngameScreen ingameScreen = instance.GetScreenFromCache(instance.GetTopScreenName()) as IngameScreen;
		if (ingameScreen == null)
		{
			UnityEngine.Debug.LogError("IngameScreen == NULL");
		}
		else
		{
			ingameScreen.SetPauseButtonVisibility(true);
		}
		Revive.Instance.SendRevive();
		SaveMeManager.IncrementNumberOfUsedKeys();
		UIScreenController.Instance.ClosePopup(null);
		SaveMeManager.IS_PURCHASE_MADE_FROM_INGAME = false;
		SaveMeManager.IS_PURCHASE_RUNNING_INGAME = false;
		TasksManager.Instance.PlayerDidThis(TaskTarget.SpendKeys, SaveMeManager.GetNumberOfKeysToSaveMe(), -1);
	}

	private void OnEnable()
	{
		this.watchLbl.text = Strings.Get(LanguageKey.UI_POPUP_SAVE_ME_BUTTON_FREE);
		this.freeLbl.text = Strings.Get(LanguageKey.UI_POPUP_SAVE_ME_BUTTON_FREE);
		this.saveMePrice.text = SaveMeManager.GetNumberOfKeysToSaveMe() + string.Empty;
		this.cancleCollider.enabled = true;
	}

	[SerializeField]
	private UILabel freeLbl;

	[SerializeField]
	private UILabel watchLbl;

	[SerializeField]
	private UILabel saveMePrice;

	public SaveMePopup saveMePopup;

	public BoxCollider cancleCollider;
}
