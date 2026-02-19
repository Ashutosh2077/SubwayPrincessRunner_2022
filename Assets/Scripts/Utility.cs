using System;
using UnityEngine;

public static class Utility
{
	public static int CompareVersions(string leftVersion, string rightVersion)
	{
		char[] separator = new char[]
		{
			'.'
		};
		string[] array = leftVersion.Split(separator);
		char[] separator2 = new char[]
		{
			'.'
		};
		string[] array2 = rightVersion.Split(separator2);
		int num = 0;
		while (num < array.Length || num < array2.Length)
		{
			int num2 = (num < array.Length) ? int.Parse(array[num]) : 0;
			int num3 = (num < array2.Length) ? int.Parse(array2[num]) : 0;
			if (num2 != num3)
			{
				return num2 - num3;
			}
			num++;
		}
		return 0;
	}

	public static int NumberOfDigits(int number)
	{
		int num = 0;
		if (number == 0)
		{
			return 1;
		}
		while (number != 0)
		{
			number /= 10;
			num++;
		}
		return num;
	}

	public static void SetLayerRecursively(Transform t, int layer)
	{
		t.gameObject.layer = layer;
		foreach (object obj in t)
		{
			Transform t2 = (Transform)obj;
			Utility.SetLayerRecursively(t2, layer);
		}
	}
}
