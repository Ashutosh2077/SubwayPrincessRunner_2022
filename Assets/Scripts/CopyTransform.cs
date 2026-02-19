using System;
using UnityEngine;

public class CopyTransform : MonoBehaviour
{
	private void LateUpdate()
	{
		base.transform.localPosition = this.transformToCopy.localPosition;
		base.transform.localRotation = this.transformToCopy.localRotation;
	}

	[SerializeField]
	private Transform transformToCopy;
}
