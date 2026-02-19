using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStats
{
	private void _AddScoreForPickup(bool wasPowerup)
	{
		int num = PlayerInfo.Instance.scoreMultiplier * 30;
		this.score += num;
		TasksManager.Instance.PlayerDidThis(TaskTarget.Score, num, -1);
		TasksManager.Instance.PlayerDidThis(TaskTarget.NoCoinsWithoutScore, num, -1);
		TasksManager.Instance.PlayerDidThis(TaskTarget.NoRollsWithoutScore, num, -1);
		TasksManager.Instance.PlayerDidThis(TaskTarget.NoJumpsWithoutScore, num, -1);
		if (!wasPowerup)
		{
			TasksManager.Instance.PlayerDidThis(TaskTarget.NoPowerUpsWithoutScore, num, -1);
		}
	}

	public void AddScoreForPickup(PropType type)
	{
		switch (type)
		{
		case PropType.chest:
		case PropType.letters:
			this._AddScoreForPickup(false);
			break;
		case PropType.flypack:
		case PropType.supershoes:
		case PropType.coinmagnet:
		case PropType.doubleMultiplier:
			this._AddScoreForPickup(true);
			break;
		}
	}

	public void CalculateScore()
	{
		if (this._metersLastUsedForScore < this.meters)
		{
			this._meterScore = this.meters - this._metersLastUsedForScore;
			this._metersLastUsedForScore = this.meters;
			int num = (int)(this._meterScore * (float)PlayerInfo.Instance.scoreMultiplier);
			num += this.ConsumeAmountNeededToAdjustScoreBy(4) * PlayerInfo.Instance.scoreMultiplier;
			this.score += num;
			TasksManager.Instance.PlayerDidThis(TaskTarget.Score, num, -1);
			TasksManager.Instance.PlayerDidThis(TaskTarget.NoCoinsWithoutScore, num, -1);
			TasksManager.Instance.PlayerDidThis(TaskTarget.NoRollsWithoutScore, num, -1);
			TasksManager.Instance.PlayerDidThis(TaskTarget.NoJumpsWithoutScore, num, -1);
			if (this._listOfActivePowerups.Count == 0 || (this._listOfActivePowerups.Count == 1 && this._listOfActivePowerups[0].type == PropType.helmet))
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.NoPowerUpsWithoutScore, num, -1);
			}
			if (this.scoreBooster5Activated)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.ScoreWithScorebooster, num, -1);
			}
			Action onScoreChanged = this.OnScoreChanged;
			if (onScoreChanged != null)
			{
				onScoreChanged();
			}
		}
	}

	public void ClearPowerups()
	{
		this._listOfActivePowerups.Clear();
		TasksManager.Instance.RemoveProgressForThis(TaskTarget.ActivePowerups);
	}

	public static int CoinToScoreConversion(int coins)
	{
		return coins * 2 * PlayerInfo.Instance.rawMultiplier;
	}

	private int ConsumeAmountNeededToAdjustScoreBy(int amount)
	{
		if (amount > 0)
		{
			if (amount > this._amountLeftToConsume)
			{
				amount = this._amountLeftToConsume;
			}
			if (this._amountLeftToConsume > 0)
			{
				this._amountLeftToConsume -= amount;
				return amount;
			}
		}
		return 0;
	}

	public List<ActiveProp> GetActivePowerups()
	{
		return this._listOfActivePowerups;
	}

	public void IncreaseAmountNeededToAdjustScoreBy(int amount)
	{
		this._amountLeftToConsume += amount;
	}

	public void RemoveHoverHelmPowerup()
	{
		for (int i = this._listOfActivePowerups.Count - 1; i >= 0; i--)
		{
			if (this._listOfActivePowerups[i].type == PropType.helmet)
			{
				this._listOfActivePowerups[i].timeLeft = 0f;
			}
		}
	}

	private void ReportOneOfEachPowerupIfApplicable(PropType powerupType)
	{
		if (TasksManager.Instance.IsTaskTargetActive(TaskTarget.OneOfEachPowerup) && this._taskEachPowerupPickupStatus.ContainsKey(powerupType) && !this._taskEachPowerupPickupStatus[powerupType])
		{
			this._taskEachPowerupPickupStatus[powerupType] = true;
			TasksManager.Instance.PlayerDidThis(TaskTarget.OneOfEachPowerup, 1, -1);
		}
	}

	public void Reset()
	{
		this.duration = 0f;
		this.ResetScore();
		this.ResetAmountNeededToAdjustScoreBy();
		this.coins = 0;
		this.coinsCoinMagnet = 0;
		this.coinsWithHelmet = 0;
		this.coinsWithFlypack = 0;
		this.allCoinsInFlypack = 0;
		this.scoreBooster5Activated = false;
		this.scoreBooster10Activated = false;
		this.meters = 0f;
		this.metersRunLeftTrack = 0f;
		this.metersRunCenterTrack = 0f;
		this.metersRunRightTrack = 0f;
		this.metersRunGround = 0f;
		this.metersRunTrain = 0f;
		this.metersRunStation = 0f;
		this.metersFly = 0f;
		this.grindedTrains = 0;
		this.jumps = 0;
		this.jumpsOverTrains = 0;
		this.rolls = 0;
		this.rollsLeftTrack = 0;
		this.rollsCenterTrack = 0;
		this.rollsRightTrack = 0;
		this.trackChanges = 0;
		this.dodgeBarrier = 0;
		this.jumpBarrier = 0;
		this.jumpHighBarrier = 0;
		this.fallIntoWater = 0;
		this.trainHit = 0;
		this.guardHitScreen = 0;
		this.guardFallWater = 0;
		this.barrierHit = 0;
		this.flypackPickups = 0;
		this.superShoesPickups = 0;
		this.coinMagnetsPickups = 0;
		this.chestPickups = 0;
		this.pickedUpPowerups = 0;
		this.doubleMultiplierPickups = 0;
		this.gameOverPlayLotteryCount = 0;
		Characters.Model model = Characters.characterData[(Characters.CharacterType)PlayerInfo.Instance.currentCharacter];
		this.reviveCount = model.freeReviveCount;
	}

	public void ResetAmountNeededToAdjustScoreBy()
	{
		this._amountLeftToConsume = 0;
	}

	public void ResetScore()
	{
		this.score = 0;
		this._metersLastUsedForScore = 0f;
		this._meterScore = 0f;
	}

	public ActiveProp RegisterPowerup(PropType type)
	{
		ActiveProp activeProp = new ActiveProp();
		activeProp.type = type;
		activeProp.timeActivated = Time.time;
		activeProp.timeLeft = PlayerInfo.Instance.GetPowerupDuration(type);
		if (type == PropType.headstart2000 || type == PropType.headstart500 || type == PropType.headstartLong || type == PropType.headstart)
		{
			activeProp.timeLeft = 0f;
		}
		for (int i = this._listOfActivePowerups.Count - 1; i >= 0; i--)
		{
			if (this._listOfActivePowerups[i].type == activeProp.type)
			{
				this._listOfActivePowerups.RemoveAt(i);
			}
		}
		this.AddScoreForPickup(type);
		this._listOfActivePowerups.Add(activeProp);
		TasksManager.Instance.RemoveProgressForThis(TaskTarget.ActivePowerups);
		TasksManager.Instance.PlayerDidThis(TaskTarget.ActivePowerups, this._listOfActivePowerups.Count, -1);
		return activeProp;
	}

	public void UpdatePowerupTimes(float deltaTime)
	{
		if (!this.pausePowerups)
		{
			for (int i = this._listOfActivePowerups.Count - 1; i >= 0; i--)
			{
				if ((!Game.Instance.IsInFlypackMode && !Game.Instance.IsInSpringJumpMode && !Game.Instance.IsInBoundJumpMode) || (this._listOfActivePowerups[i].type != PropType.helmet && this._listOfActivePowerups[i].type != PropType.supershoes))
				{
					ActiveProp activeProp = this._listOfActivePowerups[i];
					activeProp.timeLeft -= deltaTime;
					if (this._listOfActivePowerups[i].timeLeft < 0f && (!Game.Instance.IsInFlypackMode || this._listOfActivePowerups[i].type != PropType.flypack))
					{
						if (this._listOfActivePowerups[i].type == PropType.helmet)
						{
							float num = Helmet.Instance.WaitForParticlesDelay + PlayerInfo.Instance.GetHelmCoolDown();
							if (this._listOfActivePowerups[i].timeLeft > -num)
							{
								if (this.OnHelmetInCooling != null)
								{
									this.OnHelmetInCooling(this._listOfActivePowerups[i].timeLeft / num + 1f);
								}
								goto IL_188;
							}
							Helmet.Instance.HardReset();
						}
						this._listOfActivePowerups.RemoveAt(i);
						TasksManager.Instance.RemoveProgressForThis(TaskTarget.ActivePowerups);
						TasksManager.Instance.PlayerDidThis(TaskTarget.ActivePowerups, this._listOfActivePowerups.Count, -1);
					}
				}
				IL_188:;
			}
		}
	}

	public int allCoinsInFlypack
	{
		get
		{
			return this._allCoinsInFlypack;
		}
		set
		{
			this._allCoinsInFlypack = value;
		}
	}

	public int barrierHit
	{
		get
		{
			return this._barrierHit;
		}
		set
		{
			this._barrierHit = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CrashBarriers, 1, -1);
			}
		}
	}

	public int coinMagnetsPickups
	{
		get
		{
			return this._coinMagnetsPickups;
		}
		set
		{
			this._coinMagnetsPickups = value;
			if (value == 0)
			{
				if (this._taskEachPowerupPickupStatus.ContainsKey(PropType.coinmagnet))
				{
					this._taskEachPowerupPickupStatus[PropType.coinmagnet] = false;
				}
				else
				{
					this._taskEachPowerupPickupStatus.Add(PropType.coinmagnet, true);
				}
			}
			else
			{
				this.ReportOneOfEachPowerupIfApplicable(PropType.coinmagnet);
				TasksManager.Instance.PlayerDidThis(TaskTarget.Magnets, 1, -1);
			}
		}
	}

	public int coins
	{
		get
		{
			return Utils.EncryptDecryptXORValue(this._xoredNumberOfCoins);
		}
		set
		{
			this._xoredNumberOfCoins = Utils.EncryptDecryptXORValue(value);
			Action onCoinsChanged = this.OnCoinsChanged;
			if (onCoinsChanged != null)
			{
				onCoinsChanged();
			}
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoin, PlayerInfo.Instance.hasDoubleCoins ? 2 : 1, -1);
				TasksManager.Instance.RemoveProgressForThis(TaskTarget.NoCoinsWithoutScore);
			}
		}
	}

	public int coinsCoinMagnet
	{
		get
		{
			return this._coinsCoinMagnet;
		}
		set
		{
			this._coinsCoinMagnet = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CoinsWithMagnet, 1, -1);
			}
		}
	}

	public int coinsCollectedOnCenterTrack
	{
		get
		{
			return this._coinsCollectedOnCenterTrack;
		}
		set
		{
			this._coinsCollectedOnCenterTrack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CollectCoinsCenterLane, 1, -1);
			}
		}
	}

	public int coinsCollectedOnLeftTrack
	{
		get
		{
			return this._coinCollectedOnLeftTrack;
		}
		set
		{
			this._coinCollectedOnLeftTrack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CollectCoinsLeftLane, 1, -1);
			}
		}
	}

	public int coinsCollectedOnRightTrack
	{
		get
		{
			return this._coinsCollectedOnRightTrack;
		}
		set
		{
			this._coinsCollectedOnRightTrack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CollectCoinsRightLane, 1, -1);
			}
		}
	}

	public int coinsInAir
	{
		get
		{
			return this._coinsInAir;
		}
		set
		{
			this._coinsInAir = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.EarnCoinWithoutTouchingGround, 1, -1);
			}
		}
	}

	public int coinsNotTouchingGround
	{
		get
		{
			return this._coinsNotTouchingGround;
		}
		set
		{
			this._coinsNotTouchingGround = value;
		}
	}

	public int coinsWithFlypack
	{
		get
		{
			return this._allCoinsInFlypack;
		}
		set
		{
			this._allCoinsInFlypack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CoinsWithJetpack, 1, -1);
			}
		}
	}

	public int coinsWithSpringJump
	{
		get
		{
			return this._coinsWithSpringJump;
		}
		set
		{
			this._coinsWithSpringJump = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CollectCoinsWithPowerJumper, 1, -1);
			}
		}
	}

	public int coinsWithHelmet
	{
		get
		{
			return this._coinsWithHelmet;
		}
		set
		{
			if (value != this._coinsWithHelmet)
			{
				this._coinsWithHelmet = value;
				if (this.OnCoinsWithHelmetChanged != null)
				{
					this.OnCoinsWithHelmetChanged();
				}
			}
		}
	}

	public int reviveCount
	{
		get
		{
			return this._reviveCount;
		}
		set
		{
			this._reviveCount = value;
		}
	}

	public int dodgeBarrier
	{
		get
		{
			return this._dodgeBarrier;
		}
		set
		{
			this._dodgeBarrier = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.RollUnderBarriers, 1, -1);
				TasksManager.Instance.PlayerDidThis(TaskTarget.DodgeBarriers, 1, -1);
			}
		}
	}

	public int doubleMultiplierPickups
	{
		get
		{
			return this._doubleMultiplierPickups;
		}
		set
		{
			this._doubleMultiplierPickups = value;
			if (value == 0)
			{
				if (this._taskEachPowerupPickupStatus.ContainsKey(PropType.doubleMultiplier))
				{
					this._taskEachPowerupPickupStatus[PropType.doubleMultiplier] = false;
				}
				else
				{
					this._taskEachPowerupPickupStatus.Add(PropType.doubleMultiplier, true);
				}
			}
			else
			{
				this.ReportOneOfEachPowerupIfApplicable(PropType.doubleMultiplier);
				TasksManager.Instance.PlayerDidThis(TaskTarget.DoubleMultiplier, 1, -1);
			}
		}
	}

	public int grindedTrains
	{
		get
		{
			return this._grindedTrains;
		}
		set
		{
			this._grindedTrains = value;
		}
	}

	public int guardHitScreen
	{
		get
		{
			return this._guardHitScreen;
		}
		set
		{
			this._guardHitScreen = value;
		}
	}

	public int guardFallWater
	{
		get
		{
			return this._guardFallWater;
		}
		set
		{
			this._guardFallWater = value;
		}
	}

	public static GameStats Instance
	{
		get
		{
			if (GameStats.instance == null)
			{
				GameStats.instance = new GameStats();
			}
			return GameStats.instance;
		}
	}

	public int flypackPickups
	{
		get
		{
			return this._flypackPickups;
		}
		set
		{
			this._flypackPickups = value;
			if (value == 0)
			{
				if (this._taskEachPowerupPickupStatus.ContainsKey(PropType.flypack))
				{
					this._taskEachPowerupPickupStatus[PropType.flypack] = false;
				}
				else
				{
					this._taskEachPowerupPickupStatus.Add(PropType.flypack, true);
				}
			}
			else
			{
				this.ReportOneOfEachPowerupIfApplicable(PropType.flypack);
				TasksManager.Instance.PlayerDidThis(TaskTarget.Jetpack, 1, -1);
			}
		}
	}

	public int jumpBarrier
	{
		get
		{
			return this._jumpBarrier;
		}
		set
		{
			this._jumpBarrier = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.JumpBarriers, 1, -1);
				TasksManager.Instance.PlayerDidThis(TaskTarget.DodgeBarriers, 1, -1);
			}
		}
	}

	public int jumpHighBarrier
	{
		get
		{
			return this._jumpHighBarrier;
		}
		set
		{
			this._jumpHighBarrier = value;
		}
	}

	public int jumps
	{
		get
		{
			return this._jumps;
		}
		set
		{
			this._jumps = value;
			Action onJumpsChanged = this.OnJumpsChanged;
			if (onJumpsChanged != null)
			{
				onJumpsChanged();
			}
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.Jump, 1, -1);
				TasksManager.Instance.RemoveProgressForThis(TaskTarget.NoJumpsWithoutScore);
			}
		}
	}

	public int jumpsOverTrains
	{
		get
		{
			return this._jumpsOverTrains;
		}
		set
		{
			this._jumpsOverTrains = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.JumpTrain, 1, -1);
			}
		}
	}

	public Dictionary<PropType, bool> TaskEachPowerupPickupStatus
	{
		get
		{
			return this._taskEachPowerupPickupStatus;
		}
	}

	public int movingTrainHit
	{
		get
		{
			return this._movingTrainHit;
		}
		set
		{
			this._movingTrainHit = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.DieToTrain, 1, -1);
			}
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CrashTrains, 1, -1);
			}
		}
	}

	public int fallIntoWater
	{
		get
		{
			return this._fallIntoWater;
		}
		set
		{
			this._fallIntoWater = value;
		}
	}

	public int gameOverPlayLotteryCount
	{
		get
		{
			return this._gameOverPlayLotteryCount;
		}
		set
		{
			this._gameOverPlayLotteryCount = value;
			if (value >= 2 && this.OnGameOverPlayLotteryCountIncreased != null)
			{
				this.OnGameOverPlayLotteryCountIncreased();
			}
		}
	}

	public int chestPickups
	{
		get
		{
			return this._chestPickups;
		}
		set
		{
			this._chestPickups = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.Chestes, 1, -1);
			}
		}
	}

	public bool PAUSEPOWERUPS
	{
		get
		{
			return this.pausePowerups;
		}
		set
		{
			this.pausePowerups = value;
		}
	}

	public int pickedUpPowerups
	{
		get
		{
			return this._pickedUpPowerups;
		}
		set
		{
			this._pickedUpPowerups = value;
			if (value != 0)
			{
				Statistics stats= PlayerInfo.Instance.stats;
				(stats )[Stat.PickupPowerup] = stats[Stat.PickupPowerup] + 1;
				TasksManager.Instance.PlayerDidThis(TaskTarget.Powerups, 1, -1);
				TasksManager.Instance.RemoveProgressForThis(TaskTarget.NoPowerUpsWithoutScore);
			}
		}
	}

	public int powerJumperPickups
	{
		get
		{
			return this._powerJumperPickups;
		}
		set
		{
			this._powerJumperPickups = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.PickUpPowerJumpers, 1, -1);
				TasksManager.Instance.PlayerDidThis(TaskTarget.PickUpPowerJumpersSingleRun, 1, -1);
			}
		}
	}

	public int rolls
	{
		get
		{
			return this._rolls;
		}
		set
		{
			this._rolls = value;
			Action onRollsChanged = this.OnRollsChanged;
			if (onRollsChanged != null)
			{
				onRollsChanged();
			}
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.Roll, 1, -1);
				TasksManager.Instance.RemoveProgressForThis(TaskTarget.NoRollsWithoutScore);
			}
		}
	}

	public int rollsCenterTrack
	{
		get
		{
			return this._rollsCenterTrack;
		}
		set
		{
			this._rollsCenterTrack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.RollCenter, 1, -1);
			}
		}
	}

	public int rollsLeftTrack
	{
		get
		{
			return this._rollsLeftTrack;
		}
		set
		{
			this._rollsLeftTrack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.RollLeft, 1, -1);
			}
		}
	}

	public int rollsRightTrack
	{
		get
		{
			return this._rollsRightTrack;
		}
		set
		{
			this._rollsRightTrack = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.RollRight, 1, -1);
			}
		}
	}

	public int saveMeSymbolPickup
	{
		get
		{
			return this._saveMeSymbolPickup;
		}
		set
		{
			this._saveMeSymbolPickup = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.PickupKeys, 1, -1);
			}
		}
	}

	public int score
	{
		get
		{
			int num = ~(this._score ^ 7365);
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			this._score = (~value ^ 7365);
		}
	}

	public bool scoreBooster5Activated
	{
		get
		{
			return this._scoreBooster5Activated;
		}
		set
		{
			this._scoreBooster5Activated = value;
			PlayerInfo.Instance.TriggerOnScoreMultiplierChanged();
		}
	}

	public bool scoreBooster10Activated
	{
		get
		{
			return this._scoreBooster10Activated;
		}
		set
		{
			this._scoreBooster10Activated = value;
			PlayerInfo.Instance.TriggerOnScoreMultiplierChanged();
		}
	}

	public int superChestPickups
	{
		get
		{
			return this._superChestPickups;
		}
		set
		{
			this._superChestPickups = value;
		}
	}

	public int superShoesPickups
	{
		get
		{
			return this._superShoesPickups;
		}
		set
		{
			this._superShoesPickups = value;
			if (value == 0)
			{
				if (this._taskEachPowerupPickupStatus.ContainsKey(PropType.supershoes))
				{
					this._taskEachPowerupPickupStatus[PropType.supershoes] = false;
				}
				else
				{
					this._taskEachPowerupPickupStatus.Add(PropType.supershoes, true);
				}
			}
			else
			{
				this.ReportOneOfEachPowerupIfApplicable(PropType.supershoes);
				TasksManager.Instance.PlayerDidThis(TaskTarget.SuperSneakers, 1, -1);
			}
		}
	}

	public int trainHit
	{
		get
		{
			return this._trainHit;
		}
		set
		{
			this._trainHit = value;
			if (value != 0)
			{
				TasksManager.Instance.PlayerDidThis(TaskTarget.CrashTrains, 1, -1);
			}
		}
	}

	public float duration;

	public Action OnCoinsChanged;

	public Action OnGameOverPlayLotteryCountIncreased;

	public Action OnScoreChanged;

	public Action OnJumpsChanged;

	public Action OnRollsChanged;

	public Action OnCoinsWithHelmetChanged;

	public Action<float> OnHelmetInCooling;

	public float meters;

	public float metersRunLeftTrack;

	public float metersRunCenterTrack;

	public float metersRunRightTrack;

	public float metersFly;

	public float metersRunGround;

	public float metersRunTrain;

	public float metersRunStation;

	public int trackChanges;

	private int _reviveCount;

	private int _allCoinsInFlypack;

	private int _amountLeftToConsume;

	private int _barrierHit;

	private int _coinCollectedOnLeftTrack;

	private int _coinMagnetsPickups;

	private int _coinsCoinMagnet;

	private int _coinsCollectedOnCenterTrack;

	private int _coinsCollectedOnRightTrack;

	private int _coinsInAir;

	private int _coinsNotTouchingGround;

	private int _coinsWithSpringJump;

	private int _coinsWithHelmet;

	private int _dodgeBarrier;

	private int _doubleMultiplierPickups;

	private int _grindedTrains;

	private int _guardHitScreen;

	private int _guardFallWater;

	private int _flypackPickups;

	private int _jumpBarrier;

	private int _jumpHighBarrier;

	private int _jumps;

	private int _jumpsOverTrains;

	private List<ActiveProp> _listOfActivePowerups = new List<ActiveProp>();

	private float _meterScore;

	private float _metersLastUsedForScore;

	private Dictionary<PropType, bool> _taskEachPowerupPickupStatus = new Dictionary<PropType, bool>();

	private int _movingTrainHit;

	private int _fallIntoWater;

	private int _chestPickups;

	private int _pickedUpPowerups;

	private int _powerJumperPickups;

	private int _rolls;

	private int _rollsCenterTrack;

	private int _rollsLeftTrack;

	private int _rollsRightTrack;

	private int _saveMeSymbolPickup;

	private int _score;

	private bool _scoreBooster5Activated;

	private bool _scoreBooster10Activated;

	private int _superChestPickups;

	private int _superShoesPickups;

	private int _trainHit;

	private int _xoredNumberOfCoins;

	private static GameStats instance;

	private bool pausePowerups;

	private int _gameOverPlayLotteryCount;

	public delegate void CoinsChangedIngame();
}
