using System;
using UnityEngine;

public class PauseScreen : UIBaseScreen
{
	private void OnEnable()
	{
		SettingsPopup.IsInPause = true;
	}

	private void ShowNativeAd()
	{
		if (!PlayerInfo.Instance.hasSubscribed)
		{
			float num = (float)RiseSdk.Instance.GetScreenWidth() / (float)RiseSdk.Instance.GetScreenHeight();
			if (Mathf.Abs(num - 0.5625f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
			else if (Mathf.Abs(num - 0.6667f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 103, 33, "config2-3");
			}
			else if (Mathf.Abs(num - 0.75f) < 0.01f)
			{
				RiseSdk.Instance.ShowNativeAd("loading", 157, 33, "config3-4");
			}
			else
			{
				RiseSdk.Instance.ShowNativeAd("loading", 37, 33, "config9-16");
			}
			this.taskList.transform.localPosition = Vector3.up * 400f;
		}
		else
		{
			this.taskList.transform.localPosition = Vector3.up * 600f;
		}
	}

	private void OnDisable()
	{
		SettingsPopup.IsInPause = false;
		RiseSdk.Instance.CloseNativeAd("loading");
	}

	public override void Show()
	{
		base.Show();
		if (Game.Instance != null)
		{
			Game.Instance.TriggerPause(true);
			int magnitude = (int)(Time.time - Character.Instance.sameLaneTimeStamp);
			Character.Instance.sameLaneTimeStamp = Time.time;
			TasksManager.Instance.PlayerDidThis(TaskTarget.StayInOneLane, magnitude, -1);
			GC.Collect();
		}
		RiseSdk.Instance.enableBackHomeAd(false, "custom", 20000);
		if (PlayerInfo.Instance.rawMultiplier == 30)
		{
			this.taskList.gameObject.SetActive(false);
		}
		else
		{
			this.taskList.Show();
		}
		this.ShowNativeAd();
	}

	public override void Hide()
	{
		base.Hide();
		if (!PlayerInfo.Instance.hasSubscribed)
		{
			RiseSdk.Instance.enableBackHomeAd(true, "custom", 20000);
		}
	}

	public override void GainFocus()
	{
		this.ShowNativeAd();
	}

	public override void LooseFocus()
	{
		RiseSdk.Instance.CloseNativeAd("loading");
	}

	public void OnMenuClick()
	{
		UIScreenController.Instance.GoToMainMenuFromGame(base.gameObject);
	}

	[SerializeField]
	private TaskList taskList;
}
