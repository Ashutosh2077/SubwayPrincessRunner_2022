using System;
using System.Collections;
using UnityEngine;

public class Helmet : ICharacterAttachment
{
	public Helmet()
	{
		this.character = Character.Instance;
		this.characterModel = this.character.characterModel;
		this.characterController = this.character.characterController;
		this.characterAnimation = CharacterRendering.Instance.characterAnimation;
		this.coinMagnetCollider = this.character.coinMagnetCollider;
		this.coinEFX = this.character.CharacterPickupParticleSystem.CoinEFX.transform;
		this.helmetRoot = this.characterModel.BoneHelmet.gameObject;
		this.trackController = TrackController.Instance;
		this.game = Game.Instance;
		this.cooldownDistance = HelmetModelPreviewFactory.Instance.cooldownDistance;
		this.slowMotionDistance = HelmetModelPreviewFactory.Instance.slowMotionDistance;
		this.slowDownToScale = HelmetModelPreviewFactory.Instance.slowDownToScale;
		this.WaitForParticlesDelay = HelmetModelPreviewFactory.Instance.WaitForParticlesDelay;
		this.RemoveObstaclesDistance = HelmetModelPreviewFactory.Instance.RemoveObstaclesDistance;
	}

	public event Helmet.OnEndHelmetDelegate OnEndHelmet;

	public event Helmet.OnHelmetJumpDelegate OnJump;

	public event Helmet.OnJumpFromWaterDelegate OnJumpFromWater;

	public event Helmet.OnRunDelegate OnRun;

	public event Helmet.OnSpeedEndDelegate OnSpeedEnd;

	public event Helmet.OnSpeedStartDelegate OnSpeedStart;

	public event Helmet.OnSwitchToHelmetDelegate OnSwitchToHelmet;

	private void _OnEndHelmet()
	{
		if (this.OnEndHelmet != null)
		{
			this.OnEndHelmet();
		}
	}

	private void Prepare()
	{
		PlayerInfo.Instance.UseUpgrade(PropType.helmet);
		TasksManager.Instance.PlayerDidThis(TaskTarget.Helmet, 1, -1);
		this.Powerup = GameStats.Instance.RegisterPowerup(PropType.helmet);
		this.useMagnet = (this.useMutiplier = false);
		this.Paused = false;
		if (this.character.IsStumbling)
		{
			this.character.StopStumble();
		}
		this.IsActive = true;
		this.helmetRoot.SetActive(true);
		this.helm = this.characterModel.AddHelmetModel();
		if (this.OnSwitchToHelmet != null)
		{
			this.OnSwitchToHelmet(this.helm);
		}
		this.character.CharacterPickupParticleSystem.PickedupPowerUp();
		this.character.immuneToCriticalHit = true;
		this.SetupAbility();
		this.Stop = StopFlag.DONT_STOP;
	}

	private void End()
	{
		this.IsActive = false;
		this.character.immuneToCriticalHit = false;
		this.isInCooldown = true;
		if (this.useMagnet)
		{
			this.useMagnet = false;
			if (!CoinMagnet.Instance.IsActive)
			{
				this.coinMagnetCollider.GetComponent<Collider>().enabled = false;
				this.characterModel.meshCoinMagnet.enabled = false;
				this.coinEFX.localPosition = PickupParticles.coinEfxOffset;
				this.characterAnimation["hold_magnet"].enabled = false;
			}
		}
		if (this.useMutiplier)
		{
			this.useMutiplier = false;
			GameStats.Instance.scoreBooster10Activated = false;
		}
		this._OnEndHelmet();
	}

	private void DontStopUntil()
	{
		if (this.Stop == StopFlag.DONT_STOP)
		{
			TasksManager.Instance.PlayerDidThis(TaskTarget.HelmetExpire, 1, -1);
			AudioPlayer.Instance.PlaySound("leyou_Hr_powerDown", true);
			if (this.character.IsFalling || this.character.IsJumping)
			{
				if (Game.Instance.HitType == Character.CriticalHitType.FallIntoWater)
				{
					if (this.OnJumpFromWater != null)
					{
						this.OnJumpFromWater();
					}
				}
				else if (this.OnJump != null)
				{
					this.OnJump();
				}
			}
			else if (this.OnRun != null)
			{
				this.OnRun();
			}
			UnityEngine.Object.Destroy(this.helm);
		}
	}

	private void IfExplose()
	{
		this.character.helmetCrashParticleSystem.gameObject.SetActive(true);
		this.character.helmetCrashParticleSystem.Play();
		AudioPlayer.Instance.PlaySound("leyou_Hr_H_crash", true);
		if (Game.Instance.HitType == Character.CriticalHitType.FallIntoWater)
		{
			if (this.OnJumpFromWater != null)
			{
				this.OnJumpFromWater();
			}
		}
		else if (this.OnJump != null)
		{
			this.OnJump();
		}
		UnityEngine.Object.Destroy(this.helm);
	}

	public IEnumerator Begain()
	{
		if (this.isAllowed && !this.isInCooldown)
		{
			this.Prepare();
			while (this.Powerup.timeLeft > 0f && this.Stop == StopFlag.DONT_STOP)
			{
				if (this.useMagnet)
				{
					this.coinEFX.position = this.characterModel.meshCoinMagnet.transform.position;
				}
				yield return null;
			}
			this.End();
			this.DontStopUntil();
			if (this.Stop != StopFlag.STOP)
			{
				this.HardReset();
				yield break;
			}
			this.IfExplose();
			float timeLeft = this.WaitForParticlesDelay;
			while (timeLeft > 0f)
			{
				timeLeft -= Time.deltaTime;
				yield return null;
			}
			this.trackController.LayEmptyPieces(this.character.z, this.RemoveObstaclesDistance * Game.Instance.NormalizedGameSpeed);
			this.character.IsJumping = true;
			this.character.IsFalling = false;
			this.character.verticalSpeed = this.character.CalculateJumpVerticalSpeed(10f);
			float newSlowMotionDistance = this.slowMotionDistance * Game.Instance.NormalizedGameSpeed;
			float newCoolDownDist = this.cooldownDistance * Game.Instance.NormalizedGameSpeed;
			float distanceLeft = newSlowMotionDistance;
			bool didStopCooldown = false;
			while (distanceLeft > 0f)
			{
				distanceLeft -= Game.Instance.currentLevelSpeed * Time.deltaTime;
				newCoolDownDist -= Game.Instance.currentLevelSpeed * Time.deltaTime;
				if (newCoolDownDist < 0f && !didStopCooldown)
				{
					didStopCooldown = true;
				}
				yield return null;
			}
			this.character.helmetCrashParticleSystem.gameObject.SetActive(false);
		}
		yield break;
	}

	public void HardReset()
	{
		if (this.useMagnet)
		{
			this.useMagnet = false;
			this.coinMagnetCollider.GetComponent<Collider>().enabled = false;
			this.characterModel.meshCoinMagnet.enabled = false;
			this.coinEFX.localPosition = PickupParticles.coinEfxOffset;
			this.characterAnimation["hold_magnet"].enabled = false;
		}
		if (this.useMutiplier)
		{
			this.useMutiplier = false;
			GameStats.Instance.scoreBooster10Activated = false;
		}
		this.Paused = false;
		this.IsActive = false;
		this.isInCooldown = false;
		this.isAllowed = true;
	}

	public void Pause()
	{
		this.Paused = true;
		this.helmetRoot.SetActive(false);
	}

	public void Reset()
	{
		this.character.immuneToCriticalHit = true;
		this.character.characterController.enabled = true;
		this.character.characterCollider.enabled = true;
		this.helmetRoot.SetActive(true);
		Time.timeScale = 1f;
		this.character.helmetCrashParticleSystem.gameObject.SetActive(false);
	}

	public void Resume()
	{
		this.Paused = false;
		this.helmetRoot.SetActive(true);
	}

	public void SetupAbility()
	{
		Helmets.Helm helm = Helmets.helmData[PlayerInfo.Instance.currentHelmet];
		if (helm.useMagent)
		{
			this.useMagnet = true;
			if (!CoinMagnet.Instance.IsActive)
			{
				AudioPlayer.Instance.PlaySound("bmx_magnet_on", true);
				this.characterModel.meshCoinMagnet.enabled = true;
				this.characterAnimation["hold_magnet"].enabled = true;
				this.characterAnimation.Play("hold_magnet");
				this.coinMagnetCollider.GetComponent<Collider>().enabled = true;
				this.coinMagnetCollider.OnEnter = new OnTriggerObject.OnEnterDelegate(this.CoinTriggerHit);
			}
		}
		if (helm.useMutiplier)
		{
			this.useMutiplier = true;
			GameStats.Instance.scoreBooster10Activated = true;
		}
	}

	public void CoinTriggerHit(Collider collider)
	{
		Coin component = collider.GetComponent<Coin>();
		Glow componentInChildren = collider.GetComponentInChildren<Glow>();
		if (component != null)
		{
			float num = 70f;
			if ((this.character.transform.position.y >= num && component.transform.position.y >= num) || (this.character.transform.position.y < num && this.character.transform.position.y >= 0f && component.transform.position.y < num && component.transform.position.y >= 0f) || (this.character.transform.position.y < 0f && component.transform.position.y < 0f))
			{
				component.GetComponent<Collider>().enabled = false;
				CoroutineC.Instance.StartCoroutineC(this.Pull(component, componentInChildren));
			}
		}
	}

	private IEnumerator Pull(Coin coin, Glow glow)
	{
		Vector3 coinPosition = coin.transform.position;
		Vector3 vector = coinPosition - this.characterController.transform.position;
		if (glow == null)
		{
			yield return CoroutineC.Instance.StartCoroutineC(myTween.To(vector.magnitude / (this.pullSpeed * this.game.NormalizedGameSpeed), delegate(float t)
			{
				coin.transform.position = Vector3.Lerp(coinPosition, this.characterModel.meshCoinMagnet.transform.position, t * t);
			}));
		}
		else
		{
			Vector3 glowPosition = glow.transform.position;
			yield return CoroutineC.Instance.StartCoroutineC(myTween.To(vector.magnitude / (this.pullSpeed * this.game.NormalizedGameSpeed), delegate(float t)
			{
				coin.transform.position = Vector3.Lerp(coinPosition, this.characterModel.meshCoinMagnet.transform.position, t * t);
				glow.transform.position = Vector3.Lerp(glowPosition, this.characterModel.meshCoinMagnet.transform.position, t * t);
			}));
		}
		IPickup pickup = coin.GetComponent<IPickup>();
		this.character.NotifyPickup(pickup);
		GameStats instance = GameStats.Instance;
		instance.coinsCoinMagnet++;
		yield break;
	}

	public static Helmet Instance
	{
		get
		{
			if (Helmet.instance == null)
			{
				Helmet.instance = new Helmet();
			}
			return Helmet.instance;
		}
	}

	public bool ShouldPauseInFlypack
	{
		get
		{
			return true;
		}
	}

	public bool UseMagent
	{
		get
		{
			return this.useMagnet;
		}
	}

	public IEnumerator Current { get; set; }

	public bool Paused { get; set; }

	public StopFlag Stop { get; set; }

	public bool IsActive { get; private set; }

	public float cooldownDistance = 50f;

	public float slowMotionDistance = 90f;

	public float slowDownToScale = 0.3f;

	public float WaitForParticlesDelay = 0.5f;

	public float RemoveObstaclesDistance = 250f;

	public float pullSpeed = 200f;

	private bool isInCooldown;

	public bool isAllowed = true;

	private ActiveProp Powerup;

	private Game game;

	private Character character;

	private CharacterModel characterModel;

	private Animation characterAnimation;

	private CharacterController characterController;

	private OnTriggerObject coinMagnetCollider;

	private Transform coinEFX;

	private GameObject helmetRoot;

	private static Helmet instance;

	private TrackController trackController;

	private GameObject helm;

	private bool useMagnet;

	private bool useMutiplier;

	public delegate void OnEndHelmetDelegate();

	public delegate void OnHelmetJumpDelegate();

	public delegate void OnJumpFromWaterDelegate();

	public delegate void OnRunDelegate();

	public delegate void OnSpeedEndDelegate();

	public delegate void OnSpeedStartDelegate();

	public delegate void OnSwitchToHelmetDelegate(GameObject helmet);
}
