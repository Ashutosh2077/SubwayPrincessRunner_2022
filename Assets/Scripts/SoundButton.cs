using System;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
	private void Awake()
	{
		this.SetActive();
	}

	private void OnEnable()
	{
		this.musicLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_MUSIC);
		this.onLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_MUSIC_OPEN);
		this.offLbl.text = Strings.Get(LanguageKey.UI_POPUP_SETTING_MUSIC_CLOSE);
	}

	private void SetActive()
	{
		this.on.SetActive(Settings.optionSound);
		this.off.SetActive(!Settings.optionSound);
	}

	public void ClickON()
	{
		if (!Settings.optionSound)
		{
			Settings.optionSound = true;
		}
		this.SetActive();
	}

	public void ClickOFF()
	{
		if (Settings.optionSound)
		{
			Settings.optionSound = false;
		}
		this.SetActive();
	}

	[SerializeField]
	private UILabel musicLbl;

	[SerializeField]
	private UILabel onLbl;

	[SerializeField]
	private UILabel offLbl;

	[SerializeField]
	private GameObject on;

	[SerializeField]
	private GameObject off;
}
