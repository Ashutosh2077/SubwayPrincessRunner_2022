using System;
using UnityEngine;

public class UIScale : MonoBehaviour
{
	private void Start()
	{
		if (DeviceInfo.Instance.formFactor == DeviceInfo.FormFactor.iPad)
		{
			base.transform.localScale = new Vector3(this.iPadSize.x, this.iPadSize.y, base.transform.localScale.z);
		}
	}

	private Vector2 iPadSize = new Vector2(390f, 520f);
}
