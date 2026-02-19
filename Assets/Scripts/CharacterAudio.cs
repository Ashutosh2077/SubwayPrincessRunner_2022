using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
	public void Start()
	{
		this.helmet = Helmet.Instance;
		this.character = base.GetComponent<Character>();
		this.character.OnChangeTrack += this.HandleOnChangeTrack;
		this.character.OnJump += this.HandleOnJump;
		this.character.OnRoll += this.HandleOnRoll;
		this.character.OnLanding += this.HandleOnLanding;
		this.character.OnStumble += this.HandleOnStumble;
		this.stumbleClips = new Dictionary<Character.StumbleType, string>();
		this.stumbleClips.Add(Character.StumbleType.Normal, "leyou_Hr_stumble");
		this.stumbleClips.Add(Character.StumbleType.Bush, "leyou_Hr_stumble_bush");
		this.stumbleClips.Add(Character.StumbleType.Side, "leyou_Hr_stumble_side");
		this.game = Game.Instance;
		this.game.OnTurboHeadstartInput += this.HandleOnTurboHeadstartInput;
		SlideinPowerupHelper.OnScoreBoostActivated += this.HandleOnScoreBoostActivated;
	}

	private void HandleOnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		AudioPlayer.Instance.PlaySound("leyou_Hr_run_dodge", base.transform.position);
	}

	private void HandleOnJump()
	{
		if (this.game.HasSuperShoes)
		{
			AudioPlayer.Instance.PlaySound("leyou_Hr_superSneakers_jump", true);
		}
		else
		{
			AudioPlayer.Instance.PlaySound("leyou_Hr_run_jump", true);
		}
	}

	private void HandleOnRoll()
	{
		AudioPlayer.Instance.PlaySound("leyou_Hr_run_roll", true);
	}

	private void HandleOnLanding(Transform characterTransform)
	{
		if (!this.character.IsRolling)
		{
			if (this.helmet.IsActive && this.character.IsAboveGround)
			{
				AudioPlayer.Instance.PlaySound("leyou_Hr_H_land", true);
			}
			else
			{
				AudioPlayer.Instance.PlaySound("leyou_Hr_landing", true);
			}
		}
	}

	private void HandleOnScoreBoostActivated()
	{
		AudioPlayer.Instance.PlaySound("leyou_Hr_powerUp", true);
	}

	private void HandleOnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		AudioPlayer.Instance.PlaySound(this.stumbleClips[stumbleType], true);
	}

	private void HandleOnTurboHeadstartInput()
	{
		AudioPlayer.Instance.PlaySound("leyou_Hr_turboheadstart", true);
	}

	private void FlypackOnStart(bool isHeadStart)
	{
		if (!isHeadStart)
		{
			AudioPlayer.Instance.PlaySound("leyou_Hr_powerUp", true);
		}
		AudioPlayer.Instance.PlaySound("leyou_Hr_flyPack_mainLOOP", 0.5f, 0.5f, 1f);
	}

	private void FlypackOnStop()
	{
		AudioPlayer.Instance.StopSound("leyou_Hr_flyPack_mainLOOP");
		AudioPlayer.Instance.PlaySound("leyou_Hr_powerDown", true);
	}

	private Character character;

	private Game game;

	private Helmet helmet;

	private Dictionary<Character.StumbleType, string> stumbleClips;
}
