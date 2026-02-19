using System;
using System.Collections;
using UnityEngine;

public class Revive : MonoBehaviour
{
	public event Revive.OnReviveDelegate OnRevive;

	public event Revive.OnSwitchToRunningDelegate OnSwitchToRunning;

	private void Awake()
	{
		this.game = Game.Instance;
		this.character = Character.Instance;
		this.trackController = TrackController.Instance;
	}

	private IEnumerator ReviveNow()
	{
		this.reviveParticle.gameObject.SetActive(true);
		this.reviveParticle.Play();
		AudioPlayer.Instance.PlaySound("leyou_deat combo", 0.5f, 0.5f, 1.5f);
		float timeLeft = this.WaitForParticlesDelay;
		while (timeLeft > 0f)
		{
			timeLeft -= Time.deltaTime;
			yield return null;
		}
		this.trackController.LayEmptyPieces(this.character.z, this.RemoveObstaclesDistance * Game.Instance.NormalizedGameSpeed);
		if (this.OnRevive != null)
		{
			this.OnRevive();
		}
		this.game.Revive();
		if (this.OnSwitchToRunning != null)
		{
			this.OnSwitchToRunning();
		}
		yield return null;
		this.character.IsJumping = true;
		this.character.IsFalling = false;
		this.character.verticalSpeed = this.character.CalculateJumpVerticalSpeed((Game.Instance.HitType != Character.CriticalHitType.FallIntoWater) ? 15f : 0f);
		if ("IngameUI".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			(UIScreenController.Instance.GetScreenFromCache("IngameUI") as IngameScreen).CountDown();
		}
		this.reviveParticle.gameObject.SetActive(false);
		yield break;
	}

	public void SendRevive()
	{
		base.StartCoroutine(this.ReviveNow());
	}

	public void SendSkipRevive()
	{
		base.StartCoroutine(this.game.SkipRevive());
	}

	public static Revive Instance
	{
		get
		{
			if (Revive.instance == null)
			{
				Revive.instance = Utils.FindObject<Revive>();
			}
			return Revive.instance;
		}
	}

	public float WaitForParticlesDelay = 0.144f;

	public float RemoveObstaclesDistance = 250f;

	[OptionalField]
	[SerializeField]
	private ParticleSystem reviveParticle;

	private Character character;

	private Game game;

	private static Revive instance;

	private TrackController trackController;

	public delegate void OnReviveDelegate();

	public delegate void OnSwitchToRunningDelegate();
}
