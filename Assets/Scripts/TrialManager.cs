using System;
using UnityEngine;

public class TrialManager : MonoBehaviour
{
	public bool IsTestChar { get; set; }

	public bool IsTestHelm { get; set; }

	public bool preUseTryRole { get; set; }

	public bool nothingElse { get; set; }

	public static TrialManager Instance
	{
		get
		{
			if (TrialManager._instance == null)
			{
				TrialManager._instance = (UnityEngine.Object.FindObjectOfType(typeof(TrialManager)) as TrialManager);
			}
			return TrialManager._instance;
		}
	}

	private void Awake()
	{
		this.begainDateTime = new DateTime(this.year, this.month, this.day, 0, 0, 0, DateTimeKind.Utc);
		this.Check();
	}

	private void Check()
	{
		int num = PlayerInfo.Instance.currentTrialIndex;
		if (num >= 3)
		{
			num = -1;
		}
		if (num == -1)
		{
			this.Next();
		}
		else
		{
			this.currentTrialInfo = this.infos[num];
			if (this.begainDateTime.AddDays((double)PlayerInfo.Instance.totalTrialDays) < DateTime.UtcNow || !this.CheckTrialValidly())
			{
				this.Next();
			}
		}
	}

	private void Next()
	{
		int num = 0;
		if (this.isContinue)
		{
			bool flag;
			do
			{
				this.currentTrialInfo = this.infos[PlayerInfo.Instance.NextTrial()];
				flag = this.CheckTrialValidly();
				num++;
			}
			while (!flag && num <= 3);
			if (flag)
			{
				PlayerInfo.Instance.totalTrialDays = (DateTime.UtcNow.Date - this.begainDateTime.Date).Days + this.currentTrialInfo.days;
			}
		}
		else
		{
			bool flag;
			bool flag2;
			do
			{
				this.currentTrialInfo = this.infos[PlayerInfo.Instance.NextTrial()];
				PlayerInfo.Instance.totalTrialDays += this.currentTrialInfo.days;
				flag2 = (this.begainDateTime.AddDays((double)PlayerInfo.Instance.totalTrialDays) < DateTime.UtcNow);
				flag = this.CheckTrialValidly();
				if (!flag)
				{
					num++;
				}
			}
			while ((flag2 || !flag) && num <= 3);
		}
		if (num > 3)
		{
			this.nothingElse = true;
			this.currentTrialInfo = null;
			PlayerInfo.Instance.currentTrialIndex = -1;
		}
	}

	public TrialInfo SelectValidlyTrialInfo()
	{
		if (this.CheckTrialValidly())
		{
			return this.currentTrialInfo;
		}
		int i = 0;
		int num = this.infos.Length;
		while (i < num)
		{
			if (this.CheckTrialValidly(this.infos[i]))
			{
				return this.infos[i];
			}
			i++;
		}
		return null;
	}

	private bool CheckTrialValidly(TrialInfo info)
	{
		if (info == null)
		{
			return false;
		}
		if (info.type != TrialType.Character)
		{
			return info.type == TrialType.Helmet && !HelmetManager.Instance.isHelmetUnlocked(info.helmetType);
		}
		if (info.characterThemeId == 0)
		{
			return !PlayerInfo.Instance.IsCollectionComplete(info.characterType);
		}
		return !PlayerInfo.Instance.IsThemeUnlockedForCharacter(info.characterType, info.characterThemeId);
	}

	public bool CheckTrialValidly()
	{
		return this.CheckTrialValidly(this.currentTrialInfo);
	}

	public bool IsCurrentCharacterTrial(Characters.CharacterType characterType, int themeId)
	{
		return this.currentTrialInfo != null && (this.currentTrialInfo.type == TrialType.Character && this.currentTrialInfo.characterType == characterType) && this.currentTrialInfo.characterThemeId == themeId;
	}

	public bool HasTrialCharacter(Characters.CharacterType characterType)
	{
		int i = 0;
		int num = this.infos.Length;
		while (i < num)
		{
			TrialInfo trialInfo = this.infos[i];
			if (trialInfo.type == TrialType.Character && trialInfo.characterType == characterType)
			{
				return true;
			}
			i++;
		}
		return false;
	}

	public bool IsCurrentHelmetTrial(Helmets.HelmType helmetType)
	{
		return this.currentTrialInfo != null && this.currentTrialInfo.type == TrialType.Helmet && this.currentTrialInfo.helmetType == helmetType;
	}

	public bool HasHelmetTrial(Helmets.HelmType helmetType)
	{
		int i = 0;
		int num = this.infos.Length;
		while (i < num)
		{
			TrialInfo trialInfo = this.infos[i];
			if (trialInfo.type == TrialType.Helmet && trialInfo.helmetType == helmetType)
			{
				return true;
			}
			i++;
		}
		return false;
	}

	public void End()
	{
		if (this.IsTestChar)
		{
			this.IsTestChar = false;
			this.preUseTryRole = true;
			CharacterScreenManager.Instance.SelectCharacter((Characters.CharacterType)this.preCharacter, this.preCharacterSkin);
			PlayerInfo.Instance.SaveIfDirty();
		}
		if (this.IsTestHelm)
		{
			this.IsTestHelm = false;
			this.preUseTryRole = true;
			PlayerInfo.Instance.currentHelmet = (Helmets.HelmType)this.preHelmet;
			PlayerInfo.Instance.SaveIfDirty();
		}
	}

	public void Begin()
	{
		if (this.currentTrialInfo == null)
		{
			return;
		}
		if (this.currentTrialInfo.type == TrialType.Character)
		{
			this.IsTestChar = true;
			this.preCharacter = PlayerInfo.Instance.currentCharacter;
			this.preCharacterSkin = PlayerInfo.Instance.currentThemeIndex;
			CharacterScreenManager.Instance.SelectCharacter(this.currentTrialInfo.characterType, this.currentTrialInfo.characterThemeId);
		}
		else if (this.currentTrialInfo.type == TrialType.Helmet)
		{
			this.IsTestHelm = true;
			this.preHelmet = (int)PlayerInfo.Instance.currentHelmet;
			PlayerInfo.Instance.currentHelmet = this.currentTrialInfo.helmetType;
		}
	}

	public bool IsInTest()
	{
		return this.IsTestChar || this.IsTestHelm;
	}

	public bool CheckOnMainScreen()
	{
		return !this.preUseTryRole && this.currentTrialInfo != null;
	}

	[SerializeField]
	private TrialInfo[] infos;

	[SerializeField]
	private int year;

	[SerializeField]
	private int month;

	[SerializeField]
	private int day;

	[SerializeField]
	private bool isContinue;

	public DateTime begainDateTime;

	[HideInInspector]
	public TrialInfo currentTrialInfo;

	private int preCharacter;

	private int preCharacterSkin;

	private int preHelmet;

	public const int NUMBER_OF_TRIALS = 3;

	private static TrialManager _instance;
}
