using System;
using UnityEngine;

public class Settings : MonoBehaviour
{
	private void Awake()
	{
		Settings.LoadOptionsIfNeeded();
	}

	private static void LoadOptionsIfNeeded()
	{
		if (!Settings._optionsLoaded)
		{
			Settings._optionSound = (PlayerPrefs.GetInt("OPTION_SOUND", 1) != 0);
			AudioListener.volume = (Settings._optionSound ? 1f : 0f);
			Settings._optionsLoaded = true;
		}
	}

	public static bool optionSound
	{
		get
		{
			Settings.LoadOptionsIfNeeded();
			return Settings._optionSound;
		}
		set
		{
			Settings._optionSound = value;
			AudioListener.volume = (Settings._optionSound ? 1f : 0f);
			PlayerPrefs.SetInt("OPTION_SOUND", Settings._optionSound ? 1 : 0);
		}
	}

	private static bool _optionsLoaded;

	private static bool _optionSound;

	private const int OPTION_SOUND_DEFAULT = 1;

	private const string OPTION_SOUND_KEY = "OPTION_SOUND";
}
