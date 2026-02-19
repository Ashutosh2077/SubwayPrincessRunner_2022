using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
	public void Awake()
	{
		this.coins = new List<TrackObject>();
		this.GetCoins();
	}

	private IEnumerator CleanUpCoins()
	{
		while (base.enabled)
		{
			int numDeletes = (this.coins.Count > this.numDeletedPerCleanup) ? this.numDeletedPerCleanup : this.coins.Count;
			int lastIndex = Mathf.Max(0, this.coins.Count - (numDeletes + 1));
			for (int i = this.coins.Count - 1; i >= lastIndex; i--)
			{
				UnityEngine.Object.Destroy(this.coins[i].gameObject);
				this.coins.RemoveAt(i);
			}
			yield return new WaitForSeconds(this.cleanupIntervalInSeconds);
		}
		yield break;
	}

	public TrackObject GetCoin(string from)
	{
		TrackObject trackObject;
		if (this.coins.Count > 0)
		{
			trackObject = this.coins[0];
		}
		else
		{
			trackObject = this.MakeNewCoin(this.coins.Count);
		}
		this.coins.Remove(trackObject);
		Transform transform = trackObject.transform;
		if (!transform.gameObject.activeInHierarchy)
		{
			transform.gameObject.SetActive(true);
		}
		this.numberOfActiveCoins++;
		this.numberOfActiveCoins_high = Mathf.Max(this.numberOfActiveCoins_high, this.numberOfActiveCoins);
		return trackObject;
	}

	private void GetCoins()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			TrackObject component = transform.GetComponent<TrackObject>();
			if (component != null)
			{
				this.coins.Add(component);
			}
		}
	}

	private TrackObject MakeNewCoin(int coinIndex)
	{
		Vector3 position = this.spawnPoint + this.spawnSpacing * (float)coinIndex;
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.coinPrefab, position, Quaternion.identity);
		gameObject.transform.parent = base.transform;
		TrackObject component = gameObject.GetComponent<TrackObject>();
		this.coins.Add(component);
		return component;
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.CleanUpCoins());
	}

	public void Put(List<TrackObject> coins)
	{
		int i = 0;
		int count = coins.Count;
		while (i < count)
		{
			this.Put(coins[i]);
			i++;
		}
	}

	public void Put(TrackObject coin)
	{
		coin.transform.parent = base.transform;
		Vector3 position = coin.transform.position;
		position.y = -1000f;
		coin.transform.position = position;
		this.coins.Add(coin);
		this.numberOfActiveCoins--;
	}

	private void Update()
	{
		int i = 0;
		int count = this.activeRotatePickups.Count;
		while (i < count)
		{
			if (this.activeRotatePickups[i].enabled)
			{
				if (this.activeRotatePickups[i].Z + 50f < Character.Instance.z)
				{
					this.activeRotatePickups[i].enabled = false;
				}
				else
				{
					this.activeRotatePickups[i].PhasedRotate();
				}
			}
			i++;
		}
	}

	public void AddActiveRotatePickups(PickupRotate rotate)
	{
		if (!this.activeRotatePickups.Contains(rotate))
		{
			this.activeRotatePickups.Add(rotate);
		}
	}

	public void RemoveActiveRotatePickups(PickupRotate rotate)
	{
		if (this.activeRotatePickups.Contains(rotate))
		{
			this.activeRotatePickups.Remove(rotate);
		}
	}

	public static CoinPool Instance
	{
		get
		{
			if (CoinPool.instance == null)
			{
				CoinPool.instance = (UnityEngine.Object.FindObjectOfType(typeof(CoinPool)) as CoinPool);
			}
			return CoinPool.instance;
		}
	}

	public static string FindPath(Transform child)
	{
		string text = string.Empty;
		Transform parent = child.parent;
		while (parent != null)
		{
			text = parent.name + '/' + text;
			parent = parent.parent;
		}
		return text;
	}

	public GameObject coinPrefab;

	public int numDeletedPerCleanup = 4;

	public float cleanupIntervalInSeconds = 5f;

	private List<PickupRotate> activeRotatePickups = new List<PickupRotate>();

	private List<TrackObject> coins;

	private static CoinPool instance;

	private int numberOfActiveCoins;

	private int numberOfActiveCoins_high;

	private Vector3 spawnPoint = -1000f * Vector3.up;

	private Vector3 spawnSpacing = -20f * Vector3.right;
}
