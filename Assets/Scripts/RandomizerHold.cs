using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomizerHold : Randomizer
{
	public static void Initialize()
	{
		RandomizerHold.startIndex = UnityEngine.Random.Range(0, RandomizerHold.randomIndices.Length);
	}

	public override void PerformRandomizer(List<GameObject> objects)
	{
		int num = Mathf.FloorToInt(base.transform.position.z / RandomizerHold.distance) + RandomizerHold.startIndex;
		int num2 = RandomizerHold.randomIndices[(RandomizerHold.startIndex + num) % RandomizerHold.randomIndices.Length];
		for (int i = 0; i < this.children.Length; i++)
		{
			if (i == num2)
			{
				objects.Add(this.children[i]);
			}
			else
			{
				this.children[i].SetActive(false);
			}
		}
	}

	[SerializeField]
	private GameObject[] children;

	private static int startIndex = 0;

	private static float distance = 3000f;

	private static int[] randomIndices = new int[]
	{
		0,
		1,
		2,
		3,
		0,
		4,
		5,
		1,
		0,
		2,
		4,
		1,
		3,
		2,
		0,
		5,
		1,
		0,
		3,
		1,
		3
	};
}
