using System;
using UnityEngine;

[RequireComponent(typeof(UIRoot))]
public class NGUIScaler : MonoBehaviour
{
	private void Awake()
	{
		this.UIRoot = base.gameObject.GetComponent<UIRoot>();
		if (UIBaseScreen.IsOutOfProportion())
		{
			if ((float)Screen.height > 1280f)
			{
				float num = (float)Screen.width / 720f;
				num = (float)Screen.height / num;
				this.UIRoot.manualHeight = (int)num;
			}
		}
		else
		{
			this.UIRoot.manualHeight = 1280;
		}
	}

	private UIRoot UIRoot;
}
