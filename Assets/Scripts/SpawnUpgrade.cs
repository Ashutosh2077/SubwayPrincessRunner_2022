using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUpgrade : Randomizer
{
	public override void PerformRandomizer(List<GameObject> objects)
	{
		SpawnUpgradeManager.Instance.PerformSelection(this, objects);
	}

	public GameObject doubleScoreMultiplier;

	public GameObject flypackPickup;

	public GameObject superShoes;

	public GameObject magnetBooster;

	public GameObject mysteryBox;

	public GameObject gem;
}
