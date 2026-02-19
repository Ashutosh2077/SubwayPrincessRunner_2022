using System;
using UnityEngine;

public class ScrollPanelPixelPerfect : MonoBehaviour
{
	private void Start()
	{
		this._transform = base.transform;
		float num = (float)UIRoot.list[0].activeHeight;
		float num2 = (float)Screen.height;
		this._pixelFactor = num / num2;
		this.dragPanel = base.GetComponent<UIScrollView>();
	}

	private void Update()
	{
		float y = this._transform.localPosition.y;
		float num = Mathf.Round(y * this._pixelFactor) / this._pixelFactor;
		num = y - num;
		this.dragPanel.MoveRelative(new Vector3(0f, num, 0f));
	}

	private float _pixelFactor = 1f;

	private Transform _transform;

	private UIScrollView dragPanel;
}
