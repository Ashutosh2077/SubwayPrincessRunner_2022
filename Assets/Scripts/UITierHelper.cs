using System;
using UnityEngine;

public class UITierHelper : MonoBehaviour
{
	public bool ResetTiers()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		this.SetupTiers(this._type);
		Upgrade upgrade = Upgrades.upgrades[this._type];
		return PlayerInfo.Instance.GetCurrentTier(this._type) >= upgrade.numberOfTiers - 1;
	}

	public void SetupTiers(PropType type)
	{
		this._type = type;
		Upgrade upgrade = Upgrades.upgrades[type];
		int numberOfTiers = upgrade.numberOfTiers;
		int currentTier = PlayerInfo.Instance.GetCurrentTier(type);
		for (int i = 0; i < numberOfTiers - 1; i++)
		{
			UISprite uisprite = NGUITools.AddSprite(base.gameObject, this.usedAtlas, string.Format(UIPosScalesAndNGUIAtlas.Instance.slotFormat, 1));
			uisprite.name = "slot" + (i + 1);
			uisprite.pivot = UIWidget.Pivot.BottomLeft;
			uisprite.cachedTransform.localPosition = new Vector3((float)(35 * i), 0f, 0f);
			uisprite.depth = 12;
			uisprite.MakePixelPerfect();
		}
		for (int j = 0; j < currentTier; j++)
		{
			UISprite uisprite2 = NGUITools.AddSprite(base.gameObject, this.usedAtlas, string.Format(UIPosScalesAndNGUIAtlas.Instance.slotFormat, 2));
			uisprite2.name = "ActiveSlot" + (j + 1);
			uisprite2.pivot = UIWidget.Pivot.BottomLeft;
			uisprite2.transform.localPosition = new Vector3((float)(35 * j), 0f, 0f);
			uisprite2.depth = 12;
			uisprite2.MakePixelPerfect();
		}
	}

	private PropType _type;

	public UIAtlas usedAtlas;
}
