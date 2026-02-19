using System;
using UnityEngine;

public class Mirror : BaseO
{
	protected override void Awake()
	{
		this.children = new Transform[base.transform.childCount];
		for (int i = 0; i < base.transform.childCount; i++)
		{
			this.children[i] = base.transform.GetChild(i);
		}
		base.Awake();
	}

	public override void OnActivate()
	{
		int num = UnityEngine.Random.Range(0, 2) * 2 - 1;
		for (int i = 0; i < this.children.Length; i++)
		{
			Vector3 localPosition = this.children[i].localPosition;
			localPosition.x *= (float)num;
			this.children[i].localPosition = localPosition;
		}
	}

	private Transform[] children;
}
