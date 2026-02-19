using System;
using UnityEngine;

public class InitMaterials : MonoBehaviour
{
	private void Start()
	{
		InitAssets.Instance.NotifyInitMaterials(base.gameObject.GetComponentsInChildren<Renderer>(true));
	}
}
