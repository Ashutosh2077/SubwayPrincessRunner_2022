using System;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewOptimize : UIScrollView
{
	private void Start()
	{
		UIScrollView component = base.GetComponent<UIScrollView>();
		if (component != null)
		{
			UIScrollView uiscrollView = component;
			uiscrollView.onDragStarted = (UIScrollView.OnDragNotification)Delegate.Combine(uiscrollView.onDragStarted, new UIScrollView.OnDragNotification(this.OnDragStart));
			UIScrollView uiscrollView2 = component;
			uiscrollView2.onDragFinished = (UIScrollView.OnDragNotification)Delegate.Combine(uiscrollView2.onDragFinished, new UIScrollView.OnDragNotification(this.OnDragFinished));
		}
		this.originPanelY = this.panelTrans.localPosition.y;
	}

	private void OnDragFinished()
	{
		base.enabled = false;
	}

	private void OnDragStart()
	{
		base.enabled = true;
	}

	private void RefreshUI()
	{
		float num = this.panelTrans.localPosition.y - this.originPanelY;
		int num2 = -1;
		float num3 = 0f;
		while (num3 < num)
		{
			num2++;
			if (num2 == 0 || num2 == 4 || num2 == 12)
			{
				num3 += (float)this.tipHeight;
			}
			else
			{
				num3 += (float)this.cellHeight;
			}
		}
		int i = num2;
		float num4 = num3;
		while (i < num2 + this.initCount)
		{
			i++;
			if (i == 0 || i == 4 || i == 12)
			{
				num4 += (float)this.tipHeight;
			}
			else
			{
				num4 += (float)this.cellHeight;
			}
		}
		if (this.lastStart != num2)
		{
			while (this.lastStart < num2)
			{
				Transform transform = this.children[0];
				this.children.RemoveAt(0);
				this.children.Add(transform);
				transform.localPosition = new Vector3(0f, num4, 0f);
			}
		}
		this.lastStart = num2;
	}

	[SerializeField]
	private int tipHeight = 110;

	[SerializeField]
	private int cellHeight = 100;

	[SerializeField]
	private int initCount = 10;

	[SerializeField]
	private List<Transform> children;

	[SerializeField]
	private Transform panelTrans;

	private int lastStart;

	private float originPanelY;
}
