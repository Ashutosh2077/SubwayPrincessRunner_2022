using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPowerupHandler : MonoBehaviour
{
	public int getNumberOfSpaces(int i)
	{
		if (i % 2 == 0)
		{
			return this._screenOffset / 4 * Mathf.CeilToInt((float)i / 2f + 1f);
		}
		return this._screenOffset / 4 * Mathf.CeilToInt((float)i / 2f);
	}

	public float getSliderHights(int i, int slotPosition)
	{
		return (float)Mathf.FloorToInt((float)(i / 2)) * this._powerupSlots[slotPosition].getSliderHeight();
	}

	private void Update()
	{
		List<ActiveProp> activePowerups = GameStats.Instance.GetActivePowerups();
		int i = 0;
		for (int j = activePowerups.Count - 1; j >= 0; j--)
		{
			if (this._powerupSlots.Count < activePowerups.Count)
			{
				int count = this._powerupSlots.Count;
				GameObject gameObject = NGUITools.AddChild(base.gameObject, this.PowerupPrefab);
				gameObject.transform.localPosition = this._offScreenPosition;
				this._powerupSlots.Add(gameObject.GetComponent<UIPowerupHelper>());
				if (count % 2 == 0)
				{
					this._powerupSlots[count].Anchor.side = UIAnchor.Side.BottomLeft;
				}
				else
				{
					this._powerupSlots[count].Anchor.side = UIAnchor.Side.BottomRight;
				}
			}
			this._powerupSlots[i].SetPowerupSlot(activePowerups[j]);
			if (!this._powerupSlots[i].gameObject.activeInHierarchy)
			{
				this._powerupSlots[i].gameObject.SetActive(true);
			}
			float x = (float)(this._screenOffset + this._powerupSlots[i].getHalfIconBGWidth());
			if (this._powerupSlots[i].Anchor.side == UIAnchor.Side.BottomRight)
			{
				x = (float)(this._powerupSlots[i].getSliderWidth() * -1 - this._screenOffset);
			}
			float y = (float)this.getNumberOfSpaces(j) + this.getSliderHights(j, i) + (float)this._bottomOffset;
			this._powerupSlots[i].setContainerPosition(x, y, 0f);
			i++;
		}
		while (i < this._powerupSlots.Count)
		{
			if (this._powerupSlots[i].gameObject.activeInHierarchy)
			{
				this._powerupSlots[i].HidePowerupSlot();
				this._powerupSlots[i].transform.localPosition = this._offScreenPosition;
				this._powerupSlots[i].gameObject.SetActive(false);
			}
			i++;
		}
	}

	private int _bottomOffset = 5;

	private Vector3 _offScreenPosition = new Vector3(0f, -1000f, 0f);

	private List<UIPowerupHelper> _powerupSlots = new List<UIPowerupHelper>();

	private int _screenOffset = 26;

	public GameObject PowerupPrefab;
}
