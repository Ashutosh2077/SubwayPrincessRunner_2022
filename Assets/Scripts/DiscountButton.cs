using System;
using UnityEngine;

public class DiscountButton : MonoBehaviour
{
	public virtual void Awake()
	{
		this.originalFillGraySpriteName = this.fillGraySprite.spriteName;
		this.originalFillGrayScale = new Vector3((float)this.fillGraySprite.width, (float)this.fillGraySprite.height, 1f);
		this.originalContentPosition = this.content.localPosition;
		this.originalDescriptionPosition = this.description.cachedTransform.localPosition;
		this.originalPricePosition = this.price.cachedTransform.localPosition;
		this.originalDescriptionColor = this.description.color;
		this.vipTip.SetActive(false);
	}

	private void Common(int productIndex)
	{
		this.icon.spriteName = InAppData.inAppData[productIndex].iconName;
		this.price.text = InAppData.inAppData[productIndex].price;
		if (PlayerInfo.Instance.hasSubscribed && this.vipTip != null && InAppData.inAppData[productIndex].type == InAppData.DataType.Key)
		{
			this.vipTip.SetActive(true);
		}
	}

	protected virtual void ShowNoDiscount(int productIndex, string backupDescription = "")
	{
		this.fillGraySprite.width = Mathf.RoundToInt(this.originalFillGrayScale.x);
		this.fillGraySprite.height = Mathf.RoundToInt(this.originalFillGrayScale.y);
		this.content.localPosition = this.originalContentPosition;
		this.fillGraySprite.spriteName = this.originalFillGraySpriteName;
		this.price.transform.localPosition = this.originalPricePosition;
		if (string.IsNullOrEmpty(InAppData.inAppData[productIndex].description))
		{
			this.description.text = backupDescription;
		}
		else
		{
			this.description.text = InAppData.inAppData[productIndex].description;
		}
		this.description.color = Color.white;
		this.description.cachedTransform.localPosition = this.originalDescriptionPosition;
		this.description.color = this.originalDescriptionColor;
		this.Common(productIndex);
	}

	[SerializeField]
	protected UISprite icon;

	[SerializeField]
	protected UILabel description;

	[SerializeField]
	protected UILabel price;

	[SerializeField]
	protected UISprite fillGraySprite;

	[SerializeField]
	protected Transform content;

	[SerializeField]
	protected GameObject vipTip;

	protected Vector3 originalContentPosition;

	protected Color32 originalDescriptionColor;

	protected Vector3 originalDescriptionPosition;

	protected Vector3 originalFillGrayScale;

	protected string originalFillGraySpriteName;

	protected Vector3 originalPricePosition;
}
