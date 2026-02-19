using System;
using System.Collections;
using UnityEngine;

public class CoinMagnet : ICharacterAttachment
{
	public CoinMagnet()
	{
		this.character = Character.Instance;
		this.characterController = this.character.characterController;
		this.coinEFX = this.character.CharacterPickupParticleSystem.CoinEFX.transform;
		this.characterRendering = CharacterRendering.Instance;
		this.characterModel = this.characterRendering.CharacterModel;
		this.characterAnimation = this.characterRendering.characterAnimation;
		this.coinMagnetCollider = this.character.coinMagnetCollider;
		this.characterAnimation["hold_magnet"].AddMixingTransform(this.characterRendering.CharacterModel.shoulderTransform);
		this.characterAnimation["hold_magnet"].layer = 3;
		this.characterAnimation["hold_magnet"].weight = 0.9f;
		this.characterAnimation["hold_magnet"].enabled = false;
		this.game = Game.Instance;
		this.pullSpeed = HelmetModelPreviewFactory.Instance.pullSpeed;
	}

	public IEnumerator Begain()
	{
		this.Before();
		while (this.Powerup.timeLeft > 0f && this.Stop == StopFlag.DONT_STOP)
		{
			this.coinEFX.position = this.characterModel.meshCoinMagnet.transform.position;
			yield return null;
		}
		this.After();
		yield break;
	}

	private void Before()
	{
		GameStats.Instance.pickedUpPowerups++;
		this.Powerup = GameStats.Instance.RegisterPowerup(PropType.coinmagnet);
		this.Paused = false;
		if (this.character.IsStumbling)
		{
			this.character.StopStumble();
		}
		AudioPlayer.Instance.PlaySound("bmx_magnet_on", true);
		if (!Helmet.Instance.IsActive || !Helmet.Instance.UseMagent)
		{
			this.characterModel.meshCoinMagnet.enabled = true;
			this.characterAnimation["hold_magnet"].enabled = true;
			this.characterAnimation.Play("hold_magnet");
			this.coinMagnetCollider.OnEnter = new OnTriggerObject.OnEnterDelegate(this.CoinTriggerHit);
			this.coinMagnetCollider.GetComponent<Collider>().enabled = true;
		}
		this.IsActive = true;
		this.Stop = StopFlag.DONT_STOP;
	}

	private void After()
	{
		if (!Helmet.Instance.IsActive || !Helmet.Instance.UseMagent)
		{
			this.coinMagnetCollider.GetComponent<Collider>().enabled = false;
			this.characterModel.meshCoinMagnet.enabled = false;
			this.coinEFX.localPosition = PickupParticles.coinEfxOffset;
			this.characterAnimation["hold_magnet"].enabled = false;
			if (this.Powerup.timeLeft <= 0f)
			{
				AudioPlayer.Instance.PlaySound("bmx_magnet_off", true);
			}
		}
		this.IsActive = false;
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

	public void Reset()
	{
		this.Paused = false;
	}

	public void Pause()
	{
		this.Paused = true;
	}

	public void Resume()
	{
		this.Paused = false;
	}

	public static CoinMagnet Instance
	{
		get
		{
			if (CoinMagnet.instance == null)
			{
				CoinMagnet.instance = new CoinMagnet();
			}
			return CoinMagnet.instance;
		}
	}

	public IEnumerator Current { get; set; }

	public bool Paused { get; set; }

	public bool ShouldPauseInFlypack
	{
		get
		{
			return false;
		}
	}

	public StopFlag Stop { get; set; }

	public bool IsActive { get; private set; }

	public float pullSpeed = 200f;

	private ActiveProp Powerup;

	private Character character;

	private Animation characterAnimation;

	private CharacterController characterController;

	private CharacterModel characterModel;

	private CharacterRendering characterRendering;

	private Transform coinEFX;

	private OnTriggerObject coinMagnetCollider;

	private Game game;

	private static CoinMagnet instance;
}
