using System;
using UnityEngine;

public class SyncPositionHelper : MonoBehaviour
{
	private void LateUpdate()
	{
		this.toTransform.position = new Vector3(this.fromTransform.position.x, this.toTransform.position.y, this.toTransform.position.z);
	}

	private void Start()
	{
		if (this.fromObject == null || this.toObject == null)
		{
			UnityEngine.Debug.LogError("SyncPositionHelper: Both from and to must be set. Disabling self", this);
			base.enabled = false;
		}
		else
		{
			this.fromTransform = this.fromObject.transform;
			this.toTransform = this.toObject.transform;
		}
	}

	public GameObject fromObject;

	private Transform fromTransform;

	public GameObject toObject;

	private Transform toTransform;
}
