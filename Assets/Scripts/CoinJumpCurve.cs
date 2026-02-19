using System;
using System.Collections.Generic;
using UnityEngine;

public class CoinJumpCurve : BaseO
{
	protected override void Awake()
	{
		this.game = Game.Instance;
		this.character = Character.Instance;
		if (CoinJumpCurve.coinPool == null)
		{
			CoinJumpCurve.coinPool = CoinPool.Instance;
		}
		base.Awake();
	}

	private Vector3 CalcJumpCurve(float ratio)
	{
		return this.CalcJumpCurve(ratio, this.game.currentLevelSpeed);
	}

	private Vector3 CalcJumpCurve(float ratio, float speed)
	{
		float d = this.character.JumpLength(speed, this.JumpHeight);
		return base.transform.position + base.transform.forward * d * (ratio - this.curveOffset) + base.transform.up * this.NormalizedJumpCurve(ratio) * this.JumpHeight;
	}

	private void DrawCurve(float speed, Color color)
	{
		Gizmos.color = color;
		Vector3 from = this.CalcJumpCurve(this.beginRatio, speed);
		for (int i = 0; i < this.previewSteps; i++)
		{
			Vector3 vector = this.CalcJumpCurve((this.endRatio - this.beginRatio) * (float)i / (float)(this.previewSteps - 1) + this.beginRatio, speed);
			Gizmos.DrawLine(from, vector);
			from = vector;
		}
	}

	private float InvertedSpeed(float z)
	{
		return this.NormalizedJumpCurve(z) / Mathf.Sqrt(1f + Mathf.Pow(-8f * z + 4f, 2f));
	}

	private float NormalizedJumpCurve(float z)
	{
		return 4f * z * (1f - z);
	}

	public override void OnActivate()
	{
		if (this.activation == 1)
		{
			UnityEngine.Debug.Log("CoinJumpCurve has been activate twice. " + Utils.GetLongName(base.transform));
			UnityEngine.Debug.Break();
		}
		this.activation++;
		float num = this.character.JumpLength(this.game.currentLevelSpeed, this.JumpHeight);
		for (float num2 = this.beginRatio * num; num2 < this.endRatio * num; num2 += this.coinSpacing)
		{
			TrackObject coin = CoinJumpCurve.coinPool.GetCoin("CoinJumpCurve");
			coin.transform.parent = base.transform;
			coin.transform.position = this.CalcJumpCurve(num2 / num);
			coin.transform.localScale = Vector3.one;
			coin.Activate();
			this.coins.Add(coin);
		}
		this.game.OnSpeedChanged += this.PositionCoins;
	}

	public override void OnDeactivate()
	{
		this.game.OnSpeedChanged -= this.PositionCoins;
		int i = 0;
		int count = this.coins.Count;
		while (i < count)
		{
			this.coins[i].Deactivate();
			i++;
		}
		this.activation--;
		CoinJumpCurve.coinPool.Put(this.coins);
		this.coins.Clear();
	}

	private void PositionCoins(float forSpeed)
	{
		for (int i = 0; i < this.coins.Count; i++)
		{
			float ratio = (float)i / (float)(this.coins.Count - 1);
			this.coins[i].transform.position = this.CalcJumpCurve(ratio, forSpeed);
		}
	}

	private float JumpHeight
	{
		get
		{
			return this.superSneakers ? this.character.jumpHeightSuperShoes : this.character.jumpHeightNormal;
		}
	}

	public float speed = 100f;

	public float curveOffset;

	public float coinSpacing = 15f;

	public float beginRatio;

	public float endRatio = 1f;

	public bool superSneakers;

	private int activation;

	private Character character;

	private static CoinPool coinPool;

	private List<TrackObject> coins = new List<TrackObject>();

	private Game game;

	private int previewSteps = 10;
}
