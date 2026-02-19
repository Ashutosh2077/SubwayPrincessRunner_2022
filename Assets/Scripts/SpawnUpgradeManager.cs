using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUpgradeManager
{
	public SpawnUpgradeManager()
	{
		float distancePerMeter = Game.Instance.distancePerMeter;
		Upgrade upgrade = Upgrades.upgrades[PropType.doubleMultiplier];
		this.doubleScoreMultiplier = new SpawnUpgradeManager.PickupType();
		this.doubleScoreMultiplier.spawnDistanceMin = (float)upgrade.minimumMeters * distancePerMeter;
		this.doubleScoreMultiplier.spawnProbability = upgrade.spawnProbability;
		this.doubleScoreMultiplier.ExtractGameObject = ((SpawnUpgrade spawnPoint) => spawnPoint.doubleScoreMultiplier);
		upgrade = Upgrades.upgrades[PropType.flypack];
		this.flypackPickup = new SpawnUpgradeManager.PickupType();
		this.flypackPickup.spawnDistanceMin = (float)upgrade.minimumMeters * distancePerMeter;
		this.flypackPickup.spawnProbability = upgrade.spawnProbability;
		this.flypackPickup.ExtractGameObject = ((SpawnUpgrade spawnPoint) => spawnPoint.flypackPickup);
		upgrade = Upgrades.upgrades[PropType.supershoes];
		this.superShoes = new SpawnUpgradeManager.PickupType();
		this.superShoes.spawnDistanceMin = (float)upgrade.minimumMeters * distancePerMeter;
		this.superShoes.spawnProbability = upgrade.spawnProbability;
		this.superShoes.ExtractGameObject = ((SpawnUpgrade spawnPoint) => spawnPoint.superShoes);
		upgrade = Upgrades.upgrades[PropType.coinmagnet];
		this.magnetBooster = new SpawnUpgradeManager.PickupType();
		this.magnetBooster.spawnDistanceMin = (float)upgrade.minimumMeters * distancePerMeter;
		this.magnetBooster.spawnProbability = upgrade.spawnProbability;
		this.magnetBooster.ExtractGameObject = ((SpawnUpgrade spawnPoint) => spawnPoint.magnetBooster);
		upgrade = Upgrades.upgrades[PropType.chest];
		this.mysteryBox = new SpawnUpgradeManager.PickupType();
		this.mysteryBox.spawnDistanceMin = (float)upgrade.minimumMeters * distancePerMeter;
		this.mysteryBox.spawnProbability = upgrade.spawnProbability;
		this.mysteryBox.ExtractGameObject = ((SpawnUpgrade spawnPoint) => spawnPoint.mysteryBox);
		upgrade = Upgrades.upgrades[PropType.gem];
		this.gem = new SpawnUpgradeManager.PickupType();
		this.gem.spawnDistanceMin = (float)upgrade.minimumMeters * distancePerMeter;
		this.gem.spawnProbability = upgrade.spawnProbability;
		this.gem.ExtractGameObject = ((SpawnUpgrade spawnPoint) => spawnPoint.gem);
		this.pickups = new List<SpawnUpgradeManager.PickupType>
		{
			this.doubleScoreMultiplier,
			this.flypackPickup,
			this.superShoes,
			this.magnetBooster,
			this.gem,
			this.mysteryBox
		};
		this.flypackSpawnProbability = this.flypackPickup.spawnProbability;
	}

	public bool CanSpawnPickup(float z)
	{
		return z > this.spawnZ;
	}

	private void CheckFlypackSpawnRate()
	{
		if (!TrackController.Instance.AllowFlypack())
		{
			if (this.flypackPickup.spawnProbability != 0)
			{
				this.flypackPickup.spawnProbability = 0;
			}
		}
		else if (this.flypackPickup.spawnProbability != this.flypackSpawnProbability)
		{
			this.flypackPickup.spawnProbability = this.flypackSpawnProbability;
		}
	}

	public void PerformSelection(SpawnUpgrade spawn, List<GameObject> objectsToVisit)
	{
		float z = spawn.transform.position.z;
		SpawnUpgradeManager.PickupType pickupType = null;
		this.CheckFlypackSpawnRate();
		if (this.CanSpawnPickup(z))
		{
			List<SpawnUpgradeManager.PickupType> list = this.pickups.FindAll((SpawnUpgradeManager.PickupType p) => p.spawnZ < z);
			if (list.Count > 0)
			{
				int[] array = new int[list.Count];
				int num = 0;
				for (int i = 0; i < list.Count; i++)
				{
					num += list[i].spawnProbability;
					array[i] = num;
				}
				int num2 = this.randomGen.Next(0, num + 1);
				for (int j = 0; j < array.Length; j++)
				{
					if (num2 <= array[j])
					{
						pickupType = list[j];
						pickupType.spawnZ = z + pickupType.spawnDistanceMin;
						break;
					}
				}
				this.SetNextSpawnPositionZ(z);
			}
		}
		if (pickupType == this.flypackPickup)
		{
			TrackController.Instance.LastFlypackSpawnZ = z;
		}
		for (int k = 0; k < this.pickups.Count; k++)
		{
			GameObject gameObject = this.pickups[k].ExtractGameObject(spawn);
			if (this.pickups[k] == pickupType)
			{
				objectsToVisit.Add(gameObject);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}

	public void Restart()
	{
		float distancePerMeter = Game.Instance.distancePerMeter;
		this.spawnZ = 250f * distancePerMeter;
		this.spawnSpacing = 300f * distancePerMeter;
		int i = 0;
		int count = this.pickups.Count;
		while (i < count)
		{
			this.pickups[i].spawnZ = float.MinValue;
			i++;
		}
	}

	public void SetNextSpawnPositionZ(float z)
	{
		this.spawnZ = z + this.spawnSpacing;
	}

	public static SpawnUpgradeManager Instance
	{
		get
		{
			if (SpawnUpgradeManager.instance == null)
			{
				SpawnUpgradeManager.instance = new SpawnUpgradeManager();
			}
			return SpawnUpgradeManager.instance;
		}
	}

	private SpawnUpgradeManager.PickupType doubleScoreMultiplier;

	private static SpawnUpgradeManager instance;

	private SpawnUpgradeManager.PickupType flypackPickup;

	private SpawnUpgradeManager.PickupType superShoes;

	private SpawnUpgradeManager.PickupType magnetBooster;

	private SpawnUpgradeManager.PickupType mysteryBox;

	private List<SpawnUpgradeManager.PickupType> pickups;

	private System.Random randomGen = new System.Random();

	private SpawnUpgradeManager.PickupType gem;

	private float spawnSpacing;

	private float spawnZ;

	private int flypackSpawnProbability;

	private class PickupType
	{
		public Func<SpawnUpgrade, GameObject> ExtractGameObject;

		public int spawnProbability;

		public float spawnDistanceMin;

		public float spawnZ;
	}
}
