using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAirCoinsManager : MonoBehaviour
{
	public void Awake()
	{
		this.flypack = Flypack.Instance;
		this.trackController = TrackController.Instance;
		this.coinPool = CoinPool.Instance;
	}

	private IEnumerator MoveCoins(float StartZ, float length, float height)
	{
		float z = StartZ;
		while (z < StartZ + length)
		{
			TrackObject coin = this.coinPool.GetCoin("InAirCoinsManager");
			coin.transform.position = Vector3.up * height + this.trackController.GetPosition(this.curve.Evaluate(z), z);
			coin.transform.localScale = Vector3.one;
			coin.Activate();
			z += this.coinDistance;
			this.coins.Add(coin);
			yield return null;
		}
		yield break;
	}

	public void ReleaseCoins()
	{
		int i = 0;
		int count = this.coins.Count;
		while (i < count)
		{
			this.coins[i].Deactivate();
			i++;
		}
		this.coinPool.Put(this.coins);
		this.coins.Clear();
	}

	public void Spawn(float startZ, float length, float height)
	{
		this.curve = new AnimationCurve();
		int num = 1;
		for (float num2 = startZ; num2 < startZ + length; num2 += this.flypack.characterChangeTrackLength + this.stayInTrackDistance)
		{
			this.curve.AddKey(new Keyframe(num2, this.trackController.GetTrackX(num)));
			this.curve.AddKey(new Keyframe(num2 + this.stayInTrackDistance, this.trackController.GetTrackX(num)));
			num = Mathf.Clamp(num + UnityEngine.Random.Range(-1, 2), 0, this.trackController.NumberOfTracks - 1);
			this.curve.AddKey(new Keyframe(num2 + this.stayInTrackDistance + this.flypack.characterChangeTrackLength, this.trackController.GetTrackX(num)));
		}
		base.StartCoroutine(this.MoveCoins(startZ, length, height));
	}

	public GameObject coinPrefab;

	public int numberOfCoins = 200;

	public float stayInTrackDistance = 60f;

	public float coinDistance = 30f;

	private CoinPool coinPool;

	private List<TrackObject> coins = new List<TrackObject>();

	private AnimationCurve curve;

	private Flypack flypack;

	private TrackController trackController;
}
