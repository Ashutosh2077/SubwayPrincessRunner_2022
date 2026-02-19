using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class WheelSurfPopup : UIBaseScreen
{
	public override void GainFocus()
	{
		this.UpdateCoinUI();
	}

	public override void Init()
	{
		base.Init();
		LotteryButtonHelp lotteryButtonHelp = this.buttonHelp;
		lotteryButtonHelp.OnButtonClick = (Action)Delegate.Combine(lotteryButtonHelp.OnButtonClick, new Action(this.OnStartRoll));
		this.rewardItems_Selected = this.rewardItems_FrontUI;
		int i = 0;
		int num = this.rewardDisplayWeights.Length;
		while (i < num)
		{
			this.total += this.rewardDisplayWeights[i];
			i++;
		}
		this.probability_FrontUI = new double[this.rewardItems_FrontUI.Length];
		int j = 0;
		int num2 = this.rewardItems_FrontUI.Length;
		while (j < num2)
		{
			this.probability_FrontUI[j] = this.rewardItems_FrontUI[j].probability;
			j++;
		}
		this.probability_GameOverUI = new double[this.rewardItems_GameOverUI.Length];
		int k = 0;
		int num3 = this.rewardItems_GameOverUI.Length;
		while (k < num3)
		{
			this.probability_GameOverUI[k] = this.rewardItems_GameOverUI[k].probability;
			k++;
		}
		this.pointerDeltaAngel = 180f / (float)this.pointerFrequency;
		this.RefreshUI();
	}

	private void RefreshUI()
	{
		if (!this.willInitUI)
		{
			return;
		}
		int i = 0;
		int num = this.rewardUIs.Length;
		while (i < num)
		{
			this.rewardUIs[i].icon.spriteName = this.rewardItems_Selected[i].icon;
			this.rewardUIs[i].count.text = "X" + this.rewardItems_Selected[i].count;
			i++;
		}
	}

	private void RefreshLabel()
	{
		this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_TITLE);
		this.freeSpinLbl.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_BUTTON_SPIN_FREE);
		this.spinLbl.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_BUTTON_SPIN_NORMAL);
		this.luckySpr.spriteName = Strings.Get(LanguageKey.ATLAS_UI_LOTTERY_LUCK_SPRITE);
	}

	public override void Show()
	{
		base.Show();
		this.mask.enabled = false;
		this.UpdateCoinUI();
		if (WheelSurfPopup.screenUI == ScreenUI.GameOverUI)
		{
			this.rewardItems_Selected = this.rewardItems_GameOverUI;
			this.probability = this.probability_GameOverUI;
		}
		else
		{
			this.rewardItems_Selected = this.rewardItems_FrontUI;
			this.probability = this.probability_FrontUI;
		}
		this.RefreshUI();
		this.RefreshLabel();
		this.UpdateButton();
	}

	public override void Hide()
	{
		WheelSurfPopup.screenUI = ScreenUI.FrontUI;
		base.Hide();
	}

	private void UpdateButton()
	{
		this.buttonHelp.Reload(WheelSurfPopup.screenUI == ScreenUI.GameOverUI);
	}

	private void UpdateCoinUI()
	{
		this.coinLabel.text = PlayerInfo.Instance.amountOfCoins.ToString();
	}

	public void OnStartRoll()
	{
		this.UpdateCoinUI();
		GameStats.Instance.gameOverPlayLotteryCount++;
		this.OnStart();
		this.Roll();
	}

	private void OnStart()
	{
		this.mask.enabled = true;
		this.buttonHelp.GetComponent<TweenScale>().enabled = false;
	}

	private void OnEnd()
	{
		this.buttonHelp.GetComponent<TweenScale>().enabled = true;
		FreeRewardManager.Instance.SetFreeRewardType(this.rewardItems_Selected[this.currentId], delegate()
		{
			if (WheelSurfPopup.screenUI == ScreenUI.GameOverUI && GameStats.Instance.gameOverPlayLotteryCount >= 2)
			{
				UIScreenController.Instance.ClosePopup(null);
				return;
			}
			this.mask.enabled = false;
			this.UpdateButton();
			this.UpdateCoinUI();
		});
	}

	private void Roll()
	{
		if (WheelSurfPopup.screenUI == ScreenUI.GameOverUI && !PlayerPrefs.HasKey("NewPlayerFirstClickGameoverLottery"))
		{
			int num = UnityEngine.Random.Range(0, 100);
			if (num > 50)
			{
				this.currentId = this.bestGameoverRewards[0];
			}
			else
			{
				this.currentId = this.bestGameoverRewards[0];
			}
			PlayerPrefs.SetInt("NewPlayerFirstClickGameoverLottery", 1);
		}
		else
		{
			this.currentId = this.Get(this.probability);
		}
		this.PayReward(this.rewardItems_Selected[this.currentId]);
		AudioPlayer.Instance.PlaySound("lottery_sprit", true);
		this.GoTo(this.currentId);
	}

	public int Get(double[] prob)
	{
		int result = 0;
		int maxValue = (int)(prob.Sum() * 1000.0);
		System.Random random = WheelSurfPopup.rnd;
		double num = (double)random.Next(0, maxValue) / 1000.0;
		for (int i = 0; i < prob.Count<double>(); i++)
		{
			double num2 = prob.Take(i).Sum();
			double num3 = prob.Take(i + 1).Sum();
			if (num >= num2 && num < num3)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	private int Random(double[] probability)
	{
		AliasMethod aliasMethod = new AliasMethod(probability, new System.Random((int)DateTime.UtcNow.Ticks));
		return aliasMethod.next();
	}

	private void GoTo(int id)
	{
		int num = 0;
		for (int i = 0; i < id; i++)
		{
			num += this.rewardDisplayWeights[i];
		}
		int num2 = num + this.rewardDisplayWeights[id];
		int num3 = UnityEngine.Random.Range(num + 2, num2 - 2);
		this.Rotato(this.wheel, this.targetVelocity, (float)this.targetAngel - (float)((!this.isClockWise) ? -1 : 1) * ((float)num3 * 360f / (float)this.total), this.acc, this.dec, this.isClockWise);
	}

	public void Rotato(Transform target, float targetSpeed, float targetAngel, float acc, float dec, bool isClockWise)
	{
		base.StartCoroutine(this.Rotato_C(target, targetSpeed, targetAngel, acc, dec, isClockWise));
	}

	[ContextMenu("Check")]
	public void CheckDataIsAdapter()
	{
		float f = this.targetVelocity / this.acc;
		float f2 = this.targetVelocity / this.dec;
		float num = 0.5f * this.acc * Mathf.Pow(f, 2f);
		float num2 = 0.5f * this.dec * Mathf.Pow(f2, 2f);
		if ((float)this.targetAngel < num + num2 + 360f)
		{
			this.targetAngel = Mathf.CeilToInt((num + num2) / 360f) * 360 + 360;
		}
		else if (this.targetAngel % 360 != 0)
		{
			this.targetAngel = (this.targetAngel / 360 + 1) * 360;
		}
	}

	private IEnumerator Rotato_C(Transform target, float targetSpeed, float targetAngel, float acc, float dec, bool isClockWise)
	{
		targetAngel += target.transform.localEulerAngles.z;
		float accTime = targetSpeed / acc;
		float decTime = targetSpeed / dec;
		float accAngle = 0.5f * acc * Mathf.Pow(accTime, 2f);
		float decAngle = 0.5f * dec * Mathf.Pow(decTime, 2f);
		float constTime = (targetAngel - accAngle - decAngle) / targetSpeed;
		float totalTime = accTime + decTime + constTime;
		float total = 0f;
		float currentTime = 0f;
		float currentVectory = 0f;
		float deltaAngel = 0f;
		float pointerAngel = 0f;
		float movementTime = 0f;
		float currentPointerAngel = 0f;
		float lastVectory = 0f;
		Vector3 currentEuler;
		while (currentTime < totalTime)
		{
			movementTime = Time.deltaTime;
			lastVectory = currentVectory;
			if (currentTime + movementTime <= accTime)
			{
				currentVectory = this.CalcSpeed(currentVectory, 0f, targetSpeed, acc, movementTime);
				deltaAngel = Mathf.Abs((currentVectory + lastVectory) * movementTime * 0.5f);
			}
			else if (currentTime >= accTime + constTime)
			{
				currentVectory = this.CalcSpeed(currentVectory, 0f, targetSpeed, -dec, movementTime);
				deltaAngel = Mathf.Abs((currentVectory + lastVectory) * movementTime * 0.5f);
			}
			else
			{
				currentVectory = this.targetVelocity;
				deltaAngel = currentVectory * movementTime;
				if (lastVectory < this.targetVelocity)
				{
					deltaAngel -= (this.targetVelocity - lastVectory) * (accTime - currentTime) * 0.5f;
				}
				if (currentTime + movementTime > accTime + constTime)
				{
					currentVectory = this.CalcSpeed(currentVectory, 0f, targetSpeed, -dec, movementTime + currentTime - accTime - constTime);
					deltaAngel -= (this.targetVelocity - currentVectory) * (movementTime + currentTime - accTime - constTime);
				}
			}
			total += deltaAngel;
			currentEuler = target.localEulerAngles;
			if (isClockWise)
			{
				currentEuler.z -= deltaAngel;
			}
			else
			{
				currentEuler.z += deltaAngel;
			}
			target.localEulerAngles = currentEuler;
			currentPointerAngel = Mathf.Abs(currentVectory / this.targetVelocity) * (float)this.pointerTargetAngel * (float)((!isClockWise) ? -1 : 1);
			pointerAngel = this.ClampAngel(currentEuler.z) / this.pointerDeltaAngel * currentPointerAngel;
			currentEuler = this.pointer.localEulerAngles;
			currentEuler.z = pointerAngel;
			this.pointer.localEulerAngles = currentEuler;
			currentTime += movementTime;
			yield return null;
		}
		deltaAngel = currentVectory * (currentTime - totalTime) * 0.5f;
		total += deltaAngel;
		currentEuler = target.localEulerAngles;
		if (isClockWise)
		{
			currentEuler.z -= deltaAngel;
		}
		else
		{
			currentEuler.z += deltaAngel;
		}
		target.localEulerAngles = currentEuler;
		this.pointer.localEulerAngles = Vector3.zero;
		this.OnEnd();
		yield break;
	}

	private float CalcSpeed(float currentSpeed, float initSpeed, float maxSpeed, float acc, float time)
	{
		currentSpeed += acc * time;
		if (Mathf.Abs(currentSpeed - initSpeed) > Mathf.Abs(maxSpeed - initSpeed))
		{
			return maxSpeed;
		}
		return currentSpeed;
	}

	private float ClampAngel(float angel)
	{
		angel = Mathf.Repeat(angel + 360f, 360f);
		for (int i = 0; i < this.pointerFrequency; i++)
		{
			if (angel > (float)(2 * i) * this.pointerDeltaAngel && angel <= (float)(2 * i + 1) * this.pointerDeltaAngel)
			{
				return angel - (float)(2 * i) * this.pointerDeltaAngel;
			}
			if (angel > (float)(2 * i + 1) * this.pointerDeltaAngel && angel <= (float)(2 * i + 2) * this.pointerDeltaAngel)
			{
				return -angel + (float)(2 * i + 2) * this.pointerDeltaAngel;
			}
		}
		return 0f;
	}

	private void PayReward(WheelReward wr)
	{
		switch (wr.type)
		{
		case WheelRewardType.Coin:
			PlayerInfo.Instance.amountOfCoins += wr.count;
			break;
		case WheelRewardType.Key:
			PlayerInfo.Instance.amountOfKeys += wr.count;
			break;
		case WheelRewardType.Scorebooster:
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.scorebooster, wr.count);
			break;
		case WheelRewardType.Headstart:
			PlayerInfo.Instance.IncreaseUpgradeAmount(PropType.headstart2000, wr.count);
			break;
		case WheelRewardType.LeeSymbol:
			PlayerInfo.Instance.CollectSymbol(Characters.CharacterType.lee, wr.count);
			break;
		case WheelRewardType.TurtlefokSymbol:
			PlayerInfo.Instance.CollectSymbol(Characters.CharacterType.turtlefok, wr.count);
			break;
		}
	}

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private UILabel freeSpinLbl;

	[SerializeField]
	private UILabel spinLbl;

	[SerializeField]
	private UISprite luckySpr;

	[SerializeField]
	private UILabel coinLabel;

	[SerializeField]
	private UISprite mask;

	[SerializeField]
	private Transform pointer;

	[SerializeField]
	private Transform wheel;

	[SerializeField]
	private bool willInitUI;

	[SerializeField]
	private int[] rewardDisplayWeights;

	[SerializeField]
	private WheelReward[] rewardItems_FrontUI;

	[SerializeField]
	private WheelReward[] rewardItems_GameOverUI;

	[SerializeField]
	private RewardUI[] rewardUIs;

	[SerializeField]
	private int targetAngel;

	[SerializeField]
	private float targetVelocity;

	[SerializeField]
	private int pointerTargetAngel;

	[SerializeField]
	private int pointerFrequency;

	[SerializeField]
	private float acc;

	[SerializeField]
	private float dec;

	[SerializeField]
	private bool isClockWise;

	[SerializeField]
	private LotteryButtonHelp buttonHelp;

	[SerializeField]
	private int[] bestGameoverRewards = new int[2];

	public static ScreenUI screenUI = ScreenUI.FrontUI;

	private WheelReward[] rewardItems_Selected;

	private double[] probability_FrontUI;

	private double[] probability_GameOverUI;

	private double[] probability;

	private int currentId;

	private const int needKeys = 20;

	private float pointerDeltaAngel;

	private int total;

	private static System.Random rnd = new System.Random();
}
