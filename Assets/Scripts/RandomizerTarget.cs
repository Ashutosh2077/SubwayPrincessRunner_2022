using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomizerTarget : Randomizer
{
	public override void PerformRandomizer(List<GameObject> objects)
	{
		int num = UnityEngine.Random.Range(0, this.Targets.Count);
		for (int i = 0; i < this.Targets.Count; i++)
		{
			if (i == num)
			{
				objects.Add(this.Targets[i]);
			}
			else
			{
				this.Targets[i].SetActive(false);
			}
		}
	}

	public List<GameObject> Targets;
}
