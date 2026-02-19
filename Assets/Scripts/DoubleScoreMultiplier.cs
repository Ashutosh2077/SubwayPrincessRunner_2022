using System;
using System.Collections;

public class DoubleScoreMultiplier : ICharacterAttachment
{
	public IEnumerator Begain()
	{
		this.Prepare();
		while (this.Powerup.timeLeft > 0f && this.Stop == StopFlag.DONT_STOP)
		{
			yield return null;
		}
		this.End();
		yield break;
	}

	private void Prepare()
	{
		GameStats.Instance.pickedUpPowerups++;
		this.Paused = false;
		this.Stop = StopFlag.DONT_STOP;
		this.Powerup = GameStats.Instance.RegisterPowerup(PropType.doubleMultiplier);
		PlayerInfo.Instance.doubleScore = true;
	}

	private void End()
	{
		PlayerInfo.Instance.doubleScore = false;
		if (this.Powerup.timeLeft <= 0f)
		{
			AudioPlayer.Instance.PlaySound("leyou_Hr_powerDown", true);
		}
	}

	public void Reset()
	{
		this.Paused = false;
		PlayerInfo.Instance.doubleScore = false;
	}

	public void Pause()
	{
		this.Paused = true;
	}

	public void Resume()
	{
		this.Paused = false;
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

	public static DoubleScoreMultiplier Instance
	{
		get
		{
			if (DoubleScoreMultiplier.instance == null)
			{
				DoubleScoreMultiplier.instance = new DoubleScoreMultiplier();
			}
			return DoubleScoreMultiplier.instance;
		}
	}

	private static DoubleScoreMultiplier instance;

	private ActiveProp Powerup;
}
