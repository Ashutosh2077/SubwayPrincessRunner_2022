using System;
using UnityEngine;

public class PointConstraint : MonoBehaviour
{
	private void Awake()
	{
		this.transformCached = base.transform;
	}

	private void LateUpdate()
	{
		this.transformCached.position = new Vector3(this.master.position.x, 0f, this.master.position.z);
		this.transformCached.localPosition = new Vector3(this.transformCached.localPosition.x, 0f, this.transformCached.localPosition.z);
	}

	public Transform master;

	private Transform transformCached;
}
