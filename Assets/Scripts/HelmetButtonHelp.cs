using System;
using UnityEngine;

public class HelmetButtonHelp : MonoBehaviour
{
	private void Awake()
	{
		this.helmet = Helmet.Instance;
		this.state = HelmetButtonHelp.State.None;
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Remove(instance.onPowerupAmountChanged, new Action(this.UpdateLabels));
		this.helmet.OnEndHelmet -= this.OnEndHelmet;
		this.helmet.OnSwitchToHelmet -= this.OnSwitchHelmet;
		GameStats instance2 = GameStats.Instance;
		instance2.OnHelmetInCooling = (Action<float>)Delegate.Remove(instance2.OnHelmetInCooling, new Action<float>(this.OnHelmetInCooling));
	}

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onPowerupAmountChanged = (Action)Delegate.Combine(instance.onPowerupAmountChanged, new Action(this.UpdateLabels));
		this.UpdateLabels();
		this.helmet.OnEndHelmet += this.OnEndHelmet;
		this.helmet.OnSwitchToHelmet += this.OnSwitchHelmet;
		GameStats instance2 = GameStats.Instance;
		instance2.OnHelmetInCooling = (Action<float>)Delegate.Combine(instance2.OnHelmetInCooling, new Action<float>(this.OnHelmetInCooling));
	}

	public void Init()
	{
		this.animatingState = HelmetButtonHelp.AnimatingState.OffScreen;
		this.target.localPosition = this.offScreen;
	}

	public void Show()
	{
		if (!PlayerInfo.Instance.tutorialCompleted || PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet) <= 0)
		{
			base.gameObject.SetActive(false);
			this.IsActive = false;
			return;
		}
		base.gameObject.SetActive(true);
		this.IsActive = true;
		int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet);
		if (upgradeAmount > 0)
		{
			if (this.animatingState == HelmetButtonHelp.AnimatingState.OffScreen)
			{
				this._current = 0f;
			}
			else if (this.animatingState == HelmetButtonHelp.AnimatingState.AnimatingOut)
			{
				this._current = this._duration - this._current;
			}
			this.animatingState = HelmetButtonHelp.AnimatingState.AnimatingIn;
			this.SetState(HelmetButtonHelp.State.Normal);
		}
	}

	private void UpdateLabels()
	{
		int upgradeAmount = PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet);
		this.amountLbl.text = upgradeAmount.ToString();
		if (upgradeAmount == 0)
		{
			if (this.animatingState == HelmetButtonHelp.AnimatingState.OnScreen)
			{
				this._current = 0f;
			}
			else if (this.animatingState == HelmetButtonHelp.AnimatingState.AnimatingIn)
			{
				this._current = this._duration - this._current;
			}
			this.animatingState = HelmetButtonHelp.AnimatingState.AnimatingOut;
		}
	}

	private void OnEndHelmet()
	{
		this.SetState(HelmetButtonHelp.State.ColdDown);
	}

	private void OnSwitchHelmet(GameObject helmet)
	{
		this.SetState(HelmetButtonHelp.State.Using);
	}

	private void OnHelmetInCooling(float rate)
	{
		this.coldDownSprite.fillAmount = rate;
		if (rate <= 0.001f)
		{
			this.SetState(HelmetButtonHelp.State.Normal);
		}
	}

	private void OnClick()
	{
		if (!Game.Instance.IsInRunningMode || Game.Instance.isDead)
		{
			return;
		}
		if (PlayerInfo.Instance.GetUpgradeAmount(PropType.helmet) > 0 && !Game.Instance.Attachment.IsActive(Game.Instance.Attachment.Helmet))
		{
			Game.Instance.Attachment.Add(Game.Instance.Attachment.Helmet);
		}
	}

	private void SetState(HelmetButtonHelp.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		if (newState != HelmetButtonHelp.State.Normal)
		{
			if (newState != HelmetButtonHelp.State.Using)
			{
				if (newState == HelmetButtonHelp.State.ColdDown)
				{
					this.coldDownSprite.enabled = true;
					this.coldDownSprite.fillAmount = 1f;
					this.tr.enabled = false;
				}
			}
			else
			{
				this.coldDownSprite.enabled = true;
				this.coldDownSprite.fillAmount = 1f;
				this.tr.enabled = true;
				this.tr.PlayForward();
			}
		}
		else
		{
			this.coldDownSprite.enabled = false;
			this.tr.enabled = false;
		}
		this.state = newState;
	}

	private void Update()
	{
		if (this.animatingState == HelmetButtonHelp.AnimatingState.AnimatingIn)
		{
			this._current += Time.deltaTime;
			this.target.localPosition = Vector3.Lerp(this.offScreen, this.onScreen, this._current / this._duration);
			if (this._current >= this._duration)
			{
				this.animatingState = HelmetButtonHelp.AnimatingState.OnScreen;
				this.target.localPosition = this.onScreen;
			}
		}
		else if (this.animatingState == HelmetButtonHelp.AnimatingState.AnimatingOut)
		{
			this._current += Time.deltaTime;
			this.target.localPosition = Vector3.Lerp(this.onScreen, this.offScreen, this._current / this._duration);
			if (this._current >= this._duration)
			{
				this.animatingState = HelmetButtonHelp.AnimatingState.OffScreen;
				this.target.localPosition = this.offScreen;
			}
		}
	}

	public bool IsActive { get; private set; }

	private HelmetButtonHelp.State state;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private Vector3 onScreen;

	[SerializeField]
	private Vector3 offScreen;

	[SerializeField]
	private float _duration = 0.5f;

	[SerializeField]
	private UILabel amountLbl;

	[SerializeField]
	private TweenRotation tr;

	[SerializeField]
	private UISprite coldDownSprite;

	private HelmetButtonHelp.AnimatingState animatingState = HelmetButtonHelp.AnimatingState.OffScreen;

	private float _current;

	private Helmet helmet;

	public enum State
	{
		Normal,
		Using,
		ColdDown,
		None
	}

	public enum AnimatingState
	{
		_notset,
		OnScreen,
		OffScreen,
		AnimatingIn,
		AnimatingOut
	}
}
