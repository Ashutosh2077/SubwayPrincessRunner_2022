using System;
using System.Collections;
using UnityEngine;

public class UIHelmetUpdateLabel : MonoBehaviour
{
	private IEnumerator CountUpAndFlashCoroutine()
	{
		if (this._countUpCoroutineRunning)
		{
			yield break;
		}
		this._countUpCoroutineRunning = true;
		int targetAmount;
		for (;;)
		{
			targetAmount = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet);
			if (this._lastAmountSet >= targetAmount)
			{
				break;
			}
			this._lastAmountSet++;
			this.amountLabel.text = this._lastAmountSet.ToString();
			this.SpawnScrollingLabel();
			this.blinkSprite.enabled = true;
			float aniFactor = 0f;
			while (aniFactor < 1f)
			{
				aniFactor = Mathf.Clamp01(aniFactor + 4f * Time.deltaTime);
				this.blinkSprite.color = new Color(1f, 1f, 1f, 1f - aniFactor);
				yield return null;
			}
			this.blinkSprite.enabled = false;
			yield return new WaitForSeconds(0.1f);
		}
		if (this._lastAmountSet != targetAmount)
		{
			this.amountLabel.text = targetAmount.ToString();
			this._lastAmountSet = targetAmount;
		}
		this._countUpCoroutineRunning = false;
		yield break;
		yield break;
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Remove(instance.onPowerupAmountChanged, new Action(this.UpdateLabels));
		this._countUpCoroutineRunning = false;
	}

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Combine(instance.onPowerupAmountChanged, new Action(this.UpdateLabels));
		this._lastAmountSet = -1;
		this.UpdateLabels();
		this.blinkSprite.enabled = false;
	}

	private void SpawnScrollingLabel()
	{
		ScrollingTextLabel component = NGUITools.AddChild(base.gameObject, this.scrollingLabelPrefab).GetComponent<ScrollingTextLabel>();
		if (component != null)
		{
			component.StartScrolling("+1", UIHelmetUpdateLabel.SCROLL_TEXT_STARTOFFSET, UIHelmetUpdateLabel.SCROLL_TEXT_ENDOFFSET, 1f, 0.5f, true);
		}
		else
		{
			UnityEngine.Debug.LogError("No ScrollingTextLabel component on scrollingLabelPrefab");
		}
	}

	private void UpdateLabels()
	{
		int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet);
		if (this._lastAmountSet == -1)
		{
			this.amountLabel.text = upgradeAmount.ToString();
			this._lastAmountSet = upgradeAmount;
		}
		else if (this._lastAmountSet < upgradeAmount)
		{
			if (!this._countUpCoroutineRunning)
			{
				base.StartCoroutine(this.CountUpAndFlashCoroutine());
			}
		}
		else
		{
			this.amountLabel.text = upgradeAmount.ToString();
			this._lastAmountSet = upgradeAmount;
		}
	}

	private bool _countUpCoroutineRunning;

	private int _lastAmountSet = -1;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UISprite blinkSprite;

	private static readonly Vector3 SCROLL_TEXT_ENDOFFSET = new Vector3(0f, 20f, 0f);

	private static readonly Vector3 SCROLL_TEXT_STARTOFFSET = new Vector3(0f, -20f, 0f);

	[SerializeField]
	private GameObject scrollingLabelPrefab;
}
