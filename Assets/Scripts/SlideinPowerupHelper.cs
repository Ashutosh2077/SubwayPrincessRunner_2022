using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideinPowerupHelper : MonoBehaviour
{
	public static event Action OnScoreBoostActivated;

	private IEnumerator AnimateColor(UISprite sprite, Color32 startValue, Color32 endValue, float speedFactor)
	{
		float animationLerpFactor = 0f;
		while (animationLerpFactor < 1f)
		{
			animationLerpFactor += Time.deltaTime * speedFactor;
			sprite.color = Color.Lerp(startValue, endValue, animationLerpFactor);
			yield return null;
		}
		yield break;
	}

	public void HidePowerup(bool instant, int index)
	{
		if (!this._hasInited)
		{
			this.InitHelper();
		}
		this.powerupButtons[index].Hide(instant, this.positionOff + Vector3.up * this.yOff * (float)index);
	}

	public void HidePowerups()
	{
		this.HidePowerups(false);
	}

	public void HidePowerups(bool instant)
	{
		Game.Instance.OnGameEnded = (Action)Delegate.Remove(Game.Instance.OnGameEnded, new Action(this.OnGameEnded));
		for (int i = 0; i < this.powerupButtons.Length; i++)
		{
			this.HidePowerup(instant, i);
		}
	}

	private void InitHelper()
	{
		for (int i = 0; i < this.powerupButtons.Length; i++)
		{
			this.powerupButtons[i].InitSlideinButton(this, i, this.types[i], this.positionOff + this.yOff * Vector3.up);
			PlayerInfo playerInfo = PlayerInfo.Instance;
			playerInfo.onPowerupAmountChanged = (Action)Delegate.Combine(playerInfo.onPowerupAmountChanged, new Action(this.powerupButtons[i].UpdateAmount));
		}
		this._hasInited = true;
	}

	private void OnDestroy()
	{
		for (int i = 0; i < this.powerupButtons.Length; i++)
		{
			PlayerInfo playerInfo = PlayerInfo.Instance;
			playerInfo.onPowerupAmountChanged = (Action)Delegate.Remove(playerInfo.onPowerupAmountChanged, new Action(this.powerupButtons[i].UpdateAmount));
		}
	}

	private void OnDisable()
	{
		UIScreenController uiscreenController = UIScreenController.Instance;
		if (uiscreenController == null)
		{
			return;
		}
		uiscreenController.OnChangedScreen = (Action<string>)Delegate.Remove(uiscreenController.OnChangedScreen, new Action<string>(this.ScreenDidChange));
	}

	private void OnEnable()
	{
		UIScreenController.Instance.OnChangedScreen = (Action<string>)Delegate.Combine(UIScreenController.Instance.OnChangedScreen, new Action<string>(this.ScreenDidChange));
	}

	private void OnGameEnded()
	{
		this.HidePowerups();
	}

	private void ScreenDidChange(string screenOrPopupName)
	{
		if (screenOrPopupName.Equals("SaveMePopup"))
		{
			this.HidePowerups();
		}
	}

	public void ShowPowerup(int index)
	{
		if (PlayerInfo.Instance.GetUpgradeAmount(this.types[index]) > 0)
		{
			this.powerupButtons[index].Show(this.positionOff + this.yOff * Vector3.up * (float)index, this.positionOn + this.yOff * Vector3.up * (float)index);
		}
	}

	public void ShowPowerups()
	{
		if (!this._hasInited)
		{
			this.InitHelper();
		}
		for (int i = 0; i < this.powerupButtons.Length; i++)
		{
			this.ShowPowerup(i);
		}
		Game.Instance.OnGameEnded = (Action)Delegate.Combine(Game.Instance.OnGameEnded, new Action(this.OnGameEnded));
		this.showDuration = 5f;
		base.enabled = true;
	}

	private void Update()
	{
		if (this.showDuration > 0f)
		{
			this.showDuration -= Time.deltaTime;
		}
		else
		{
			this.HidePowerups();
			base.enabled = false;
		}
	}

	public void SlideinPowerupClicked(int index)
	{
		if (!Game.Instance.isPaused && this.powerupButtons[index].GetComponent<Collider>().enabled)
		{
			if (this.types[index] == PropType.scorebooster)
			{
				GameStats.Instance.scoreBooster5Activated = true;
				TasksManager.Instance.PlayerDidThis(TaskTarget.ScoreBooster, 1, -1);
				if (SlideinPowerupHelper.OnScoreBoostActivated != null)
				{
					SlideinPowerupHelper.OnScoreBoostActivated();
				}
				this.HidePowerup(false, index);
			}
			else if (this.types[index] == PropType.headstart2000)
			{
				this._currentHeadstartGear++;
				int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PropType.headstart2000);
				if (upgradeAmount > 0)
				{
					this.HidePowerup(false, index);
					Game.Instance.Megaheadstart();
					TasksManager.Instance.PlayerDidThis(TaskTarget.Headstart, 1, -1);
				}
			}
			PlayerInfo.Instance.UseUpgrade(this.types[index]);
		}
	}

	private void Start()
	{
		if (!this._hasInited)
		{
			this.InitHelper();
		}
	}

	public static SlideinPowerupHelper Instance
	{
		get
		{
			if (SlideinPowerupHelper.instance == null)
			{
				SlideinPowerupHelper.instance = (UnityEngine.Object.FindObjectOfType(typeof(SlideinPowerupHelper)) as SlideinPowerupHelper);
			}
			return SlideinPowerupHelper.instance;
		}
	}

	[SerializeField]
	private SlideinPowerupButton[] powerupButtons;

	[SerializeField]
	private List<PropType> types;

	private int _currentHeadstartGear;

	private bool _hasInited;

	private static SlideinPowerupHelper instance;

	[SerializeField]
	private Vector3 positionOff = new Vector3(-100f, 255f, 0f);

	[SerializeField]
	private Vector3 positionOn = new Vector3(100f, 255f, 0f);

	[SerializeField]
	private float yOff = 200f;

	private float showDuration;
}
