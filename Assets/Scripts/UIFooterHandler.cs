using System;
using UnityEngine;

public class UIFooterHandler : MonoBehaviour
{
	private void OnEnable()
	{
		this.UpdateTips();
		PurchaseHandler.Instance.AddOnUpgradePurchase(new Action(this.UpdateTips));
		PlayerInfo instance = PlayerInfo.Instance;
		instance.OnHelmUnlocked = (Action<Helmets.HelmType>)Delegate.Combine(instance.OnHelmUnlocked, new Action<Helmets.HelmType>(this.UpdateTipsBY));
	}

	private void OnDisable()
	{
		PurchaseHandler.Instance.RemoveOnUpgradePurchase(new Action(this.UpdateTips));
		PlayerInfo instance = PlayerInfo.Instance;
		instance.OnHelmUnlocked = (Action<Helmets.HelmType>)Delegate.Remove(instance.OnHelmUnlocked, new Action<Helmets.HelmType>(this.UpdateTipsBY));
	}

	private void UpdateTipsBY(Helmets.HelmType obj)
	{
		this.UpdateTips();
	}

	public void InitButtonType()
	{
		this.helm.SetFill(false);
		this.character.SetFill(false);
		this.upgrade.SetFill(false);
		this.store.SetFill(false);
	}

	private void UpdateTips()
	{
		this.tips[0].enabled = PlayerInfo.Instance.CanUnlockCharacter();
		this.tips[1].enabled = PlayerInfo.Instance.CanUnlockHelm();
		this.tips[2].enabled = PlayerInfo.Instance.CanIncreasePowerup();
	}

	public void OnButtonClick(int selected)
	{
		this.InitButtonType();
		switch (selected)
		{
		case 1:
			this.character.SetFill(true);
			break;
		case 2:
			this.helm.SetFill(true);
			break;
		case 3:
			this.upgrade.SetFill(true);
			break;
		case 4:
			this.store.SetFill(true);
			break;
		default:
			UnityEngine.Debug.Log("No button was selected in the footer?", this);
			break;
		}
	}

	public FootItem helm;

	public FootItem character;

	public FootItem upgrade;

	public FootItem store;

	[SerializeField]
	private UISprite[] tips = new UISprite[3];
}
