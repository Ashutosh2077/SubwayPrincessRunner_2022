using System;
using UnityEngine;

public class CelebrationClone : MonoBehaviour
{
	private void Awake()
	{
		if (this._targetObject != null)
		{
			CalcCircularPoint calcCircularPoint = new CalcCircularPoint(this._copyCount, this._axis, this._radius);
			Vector3 position = base.transform.position;
			Quaternion rotation = this._targetObject.transform.rotation;
			for (int i = 0; i < this._copyCount; i++)
			{
				Vector3 vector = position + calcCircularPoint.CalcCenterOffset(i);
				if (this._alignToCloner)
				{
					rotation = Quaternion.LookRotation(base.transform.up, position - vector);
				}
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this._targetObject, vector, rotation);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localScale = this._targetObject.transform.localScale;
			}
			if (this._destroyOriginal)
			{
				UnityEngine.Object.Destroy(this._targetObject);
				this._targetObject = null;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Vector3 position = base.transform.position;
		CalcCircularPoint calcCircularPoint = new CalcCircularPoint(this._copyCount, this._axis, this._radius);
		for (int i = 0; i < this._copyCount; i++)
		{
			Vector3 vector = position + calcCircularPoint.CalcCenterOffset(i);
			if (this._alignToCloner)
			{
				Gizmos.DrawRay(vector, base.transform.up * this._radius * 0.5f);
			}
			Gizmos.DrawSphere(vector, this._radius * 0.05f);
		}
	}

	[SerializeField]
	private GameObject _targetObject;

	[SerializeField]
	private int _copyCount = 4;

	[SerializeField]
	private bool _destroyOriginal = true;

	[SerializeField]
	private float _radius = 1f;

	[SerializeField]
	private CalcCircularPoint.Axis _axis = CalcCircularPoint.Axis.Z;

	[SerializeField]
	private bool _alignToCloner = true;

	private CalcCircularPoint calc;
}
