using System;
using UnityEngine;

public class CelebrationPopupLabelTemplate : MonoBehaviour
{
	private string _GetCoinsLabel(int amount)
	{
		return string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_COINS_AMOUNT), string.Format("{0:#,###0}", amount));
	}

	private string _GetKeysLabel(int amount)
	{
		if (amount > 1)
		{
			return string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_KEYS_AMOUNT), amount.ToString());
		}
		return string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_KEY_AMOUNT), amount.ToString());
	}

	private string _GetPowerupLabel(PropType type, int amount)
	{
		Upgrade upgrade = Upgrades.upgrades[type];
		return string.Concat(new object[]
		{
			string.Empty,
			amount,
			Strings.Get(LanguageKey.UI_CELEBRATION_GE),
			Strings.Get(upgrade.GetName())
		});
	}

	public void Init(int backgroundDepth)
	{
		base.gameObject.GetComponent<UIPanel>().depth = backgroundDepth + 1;
	}

	private void OnEnable()
	{
		this.normalStateGO.SetActive(false);
	}

	public void SetDoubleCions(int amount)
	{
		this.normalStateGO.SetActive(true);
		this.bigLabel.text = this._GetCoinsLabel(amount);
		this.subLabel.text = string.Empty;
	}

	public void SetupDoubleKeys(int amount)
	{
		this.normalStateGO.SetActive(true);
		this.bigLabel.text = this._GetKeysLabel(amount);
		this.subLabel.text = string.Empty;
	}

	public void SetDoublePowerup(PropType powerup, int amount)
	{
		this.normalStateGO.SetActive(true);
		this.bigLabel.text = this._GetPowerupLabel(powerup, amount);
		Upgrade upgrade = Upgrades.upgrades[powerup];
		this.subLabel.text = Strings.Get(upgrade.mysteryBoxDescription);
		this.ResetSubLabelPosition();
	}

	public void SetupDoubleSymbol(Characters.CharacterType characterType, int amount)
	{
		this.normalStateGO.SetActive(true);
		Characters.Model model = Characters.characterData[characterType];
		this.bigLabel.text = amount * 2 + Strings.Get(LanguageKey.UI_CELEBRATION_GE) + Strings.Get(model.symbolName);
		int num = model.Price - PlayerInfo.Instance.GetCollectedSymbols(characterType) - amount;
		if (num > 0)
		{
			this.subLabel.text = string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_COLLECT_MORE_TOKENS), num, Strings.Get(model.name));
		}
		else
		{
			this.subLabel.text = string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_YOU_UNLOCKED_TROPHY), Strings.Get(model.name));
		}
	}

	public void SetupCharacter(string charName)
	{
		this.normalStateGO.SetActive(true);
		this.Alpha = 0f;
		this.bigLabel.text = string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_CHARACTER_UNLOCK), charName);
		this.subLabel.text = string.Empty;
	}

	public void SetupCoins()
	{
		this.normalStateGO.SetActive(true);
		this.Alpha = 0f;
		this.bigLabel.text = string.Empty;
		this.subLabel.text = string.Empty;
	}

	public void SetupEventSpecialHelm(string helmName)
	{
		this.normalStateGO.SetActive(true);
		this.Alpha = 0f;
		this.bigLabel.text = string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_EVENT_BOARD_TRYOUT), helmName);
		this.subLabel.text = string.Empty;
	}

	public void SetupKeys(int amount)
	{
		this.normalStateGO.SetActive(true);
		this.Alpha = 0f;
		this.bigLabel.text = this._GetKeysLabel(amount);
		this.subLabel.text = string.Empty;
	}

	public void SetupPowerup(PropType powerup, int amount)
	{
		this.normalStateGO.SetActive(true);
		this.Alpha = 0f;
		this.bigLabel.text = this._GetPowerupLabel(powerup, amount);
		Upgrade upgrade = Upgrades.upgrades[powerup];
		this.subLabel.text = Strings.Get(upgrade.mysteryBoxDescription);
		this.ResetSubLabelPosition();
	}

	public void SetupSymbol(Characters.CharacterType characterType, int amount)
	{
		this.normalStateGO.SetActive(true);
		this.Alpha = 0f;
		Characters.Model model = Characters.characterData[characterType];
		this.bigLabel.text = amount + Strings.Get(LanguageKey.UI_CELEBRATION_GE) + Strings.Get(model.symbolName);
		int num = model.Price - PlayerInfo.Instance.GetCollectedSymbols(characterType) - amount;
		if (num > 0)
		{
			this.subLabel.text = string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_COLLECT_MORE_TOKENS), num, Strings.Get(model.name));
		}
		else
		{
			this.subLabel.text = string.Format(Strings.Get(LanguageKey.CELEBRATION_POPUP_YOU_UNLOCKED_TROPHY), Strings.Get(model.name));
		}
	}

	public void UpdateCoins(int amount)
	{
		this.normalStateGO.SetActive(true);
		this.bigLabel.text = this._GetCoinsLabel(amount);
		this.subLabel.text = string.Empty;
		this.ResetSubLabelPosition();
	}

	private void ResetSubLabelPosition()
	{
		this.subLabel.transform.localPosition = this.bigLabel.transform.localPosition - new Vector3(0f, (float)this.bigLabel.height * 0.5f + (float)this.subLabel.height * 0.5f + 5f, 0f);
	}

	public float Alpha
	{
		get
		{
			return this.bigLabel.alpha;
		}
		set
		{
			this.bigLabel.alpha = Mathf.Clamp01(value);
		}
	}

	public UILabel bigLabel;

	public UILabel subLabel;

	[SerializeField]
	private GameObject normalStateGO;
}
