using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Randomizer : MonoBehaviour
{
	public void InitializeRandomizer()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	public abstract void PerformRandomizer(List<GameObject> objects);
}
