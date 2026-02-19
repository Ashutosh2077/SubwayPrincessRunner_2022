using System;
using UnityEngine;

public class FollowZ : MonoBehaviour
{
	private void Awake()
	{
		this.mTrans = base.transform;
		this.mTrans.rotation = Quaternion.identity;
		this.initPos = this.mTrans.position;
		base.enabled = false;
	}

	private void LateUpdate()
	{
		if (this.target.position.z >= this.mTrans.position.z)
		{
			this.mTrans.position = new Vector3(this.mTrans.position.x, this.mTrans.position.y, this.target.position.z + this.zOffset);
		}
	}

	private void OnDisable()
	{
		this.mTrans.position = this.initPos;
	}

	[SerializeField]
	private Transform target;

	[SerializeField]
	private float zOffset;

	[SerializeField]
	private float translationTime;

	private Transform mTrans;

	private Vector3 initPos;
}
