using System;
using UnityEngine;

public class Layers
{
	private Layers()
	{
	}

	private static int FindLayer(string name)
	{
		int num = LayerMask.NameToLayer(name);
		if (num == -1)
		{
			UnityEngine.Debug.LogError("Could not find layer '" + name + "'.");
		}
		return num;
	}

	public static void Init()
	{
		Layers._instance = new Layers();
	}

	public static Layers Instance
	{
		get
		{
			if (Layers._instance == null)
			{
				Layers.Init();
			}
			return Layers._instance;
		}
	}

	public readonly int Default = Layers.FindLayer("Default");

	public readonly int HitBounceOnly = Layers.FindLayer("HitBounceOnly");

	public readonly int KeepOnHelmet = Layers.FindLayer("KeepOnHelmet");

	public readonly int _3DGUI = Layers.FindLayer("3DGUI");

	public readonly int CELEBRATION = Layers.FindLayer("CELEBRATION");

	public readonly int _2DGUI = Layers.FindLayer("2DGUI");

	public readonly int Character = Layers.FindLayer("Character");

	public readonly int LoadScreenLayer = Layers.FindLayer("LoadScreenLayer");

	private static Layers _instance;
}
