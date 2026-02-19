using System;
using System.Collections;
using UnityEngine;

public class SuperShoes : ICharacterAttachment
{
	public SuperShoes()
	{
		this.character = Character.Instance;
		this.characterRendering = CharacterRendering.Instance;
		this.characterModel = this.characterRendering.CharacterModel;
		this.longMagnet = this.character.GetComponentInChildren<LongMagnet>();
	}

	public event SuperShoes.OnSwitchToSuperShoesDelegate OnSwitchToSuperShoes;

	public event SuperShoes.SuperShoesOnStopDelegate SuperShoesOnStop;

	public IEnumerator Begain()
	{
		this.Prepare();
		while (this.Powerup.timeLeft > 0f && this.Stop == StopFlag.DONT_STOP)
		{
			if (!this.Paused)
			{
				this.timeActiveInARow += Time.deltaTime;
			}
			yield return null;
		}
		this.End();
		yield break;
	}

	private void Prepare()
	{
		GameStats.Instance.pickedUpPowerups++;
		this.Powerup = GameStats.Instance.RegisterPowerup(PropType.supershoes);
		this.character.IsJumpingHigher = true;
		this.Paused = false;
		if (this.character.IsStumbling)
		{
			this.character.StopStumble();
		}
		this.characterModel.meshSuperShoes.enabled = true;
		this.IsActive = true;
		this.longMagnet.Activate();
		if (this.OnSwitchToSuperShoes != null)
		{
			this.OnSwitchToSuperShoes();
		}
		this.Stop = StopFlag.DONT_STOP;
	}

	private void End()
	{
		this.timeActiveInARow = 0f;
		this.characterModel.meshSuperShoes.enabled = false;
		this.IsActive = false;
		if (!this.longMagnet.ShouldBeActive)
		{
			this.longMagnet.Deactivate();
		}
		if (this.Powerup.timeLeft <= 0f)
		{
			AudioPlayer.Instance.PlaySound("leyou_Hr_powerDown", true);
		}
		this.character.IsJumpingHigher = false;
		if (this.SuperShoesOnStop != null)
		{
			this.SuperShoesOnStop();
		}
	}

	public void Reset()
	{
		this.Paused = false;
		this.character.IsJumpingHigher = false;
	}

	public void StopUse()
	{
		this.IsActive = false;
		this.characterModel.meshSuperShoes.enabled = false;
	}

	public void Pause()
	{
		this.Paused = true;
	}

	public void Resume()
	{
		this.Paused = false;
	}

	public static SuperShoes Instance
	{
		get
		{
			if (SuperShoes.instance == null)
			{
				SuperShoes.instance = new SuperShoes();
			}
			return SuperShoes.instance;
		}
	}

	public bool ShouldPauseInFlypack
	{
		get
		{
			return true;
		}
	}

	public IEnumerator Current { get; set; }

	public bool Paused { get; set; }

	public StopFlag Stop { get; set; }

	public bool IsActive { get; private set; }

	private Character character;

	private CharacterModel characterModel;

	private CharacterRendering characterRendering;

	private static SuperShoes instance;

	private LongMagnet longMagnet;

	private ActiveProp Powerup;

	private float timeActiveInARow;

	public delegate void OnSwitchToSuperShoesDelegate();

	public delegate void SuperShoesOnStopDelegate();
}
