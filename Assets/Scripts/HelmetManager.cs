using System;

internal class HelmetManager
{
	public event HelmetManager.OnHelmetChangeDelegate OnDisplayedHelmetChange;

	public bool isHelmActive(Helmets.HelmType helmType)
	{
		return Helmets.helmData[helmType].unlockType != Helmets.UnlockType.hiddenUntillUnlocked;
	}

	public bool isHelmetUnlocked(Helmets.HelmType helmType)
	{
		return Helmets.helmData[helmType].unlockType == Helmets.UnlockType.alwaysUnlocked || (PlayerInfo.Instance._helmetUnlockStatus.ContainsKey(helmType) && PlayerInfo.Instance._helmetUnlockStatus[helmType]);
	}

	public void ChangedDisplayedHelmet(Helmets.HelmType newHelm)
	{
		this.currentlyDisplayedHelmet = newHelm;
		if (this.OnDisplayedHelmetChange != null)
		{
			this.OnDisplayedHelmetChange(newHelm);
		}
	}

	public Helmets.HelmType CurrentlyDisplayedHelmet()
	{
		return this.currentlyDisplayedHelmet;
	}

	public Helmets.HelmType CurrentlyEquippedHelmet()
	{
		return PlayerInfo.Instance.currentHelmet;
	}

	public static HelmetManager Instance
	{
		get
		{
			if (HelmetManager.instance == null)
			{
				HelmetManager.instance = new HelmetManager();
			}
			return HelmetManager.instance;
		}
	}

	private Helmets.HelmType currentlyDisplayedHelmet;

	private static HelmetManager instance;

	public delegate void OnHelmetChangeDelegate(Helmets.HelmType helmet);
}
