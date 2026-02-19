using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomizerOne : Randomizer
{
	public override void PerformRandomizer(List<GameObject> objects)
	{
		int num = UnityEngine.Random.Range(0, base.transform.childCount);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			if (i == num)
			{
				objects.Add(gameObject);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}
}
