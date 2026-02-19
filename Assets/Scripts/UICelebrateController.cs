using System;
using UnityEngine;

public class UICelebrateController : MonoBehaviour
{
	private void Awake()
	{
		if (UICelebrateController.Instance == null)
		{
			UICelebrateController.Instance = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public static UICelebrateController Instance;
}
