using System;
using System.Collections;
using UnityEngine;

public class UIPanelStaticDelayer : MonoBehaviour
{
	private void Awake()
	{
		this._panel = base.GetComponent<UIPanel>();
		if (this._panel == null)
		{
			UnityEngine.Debug.LogWarning("UIPanelStaticDelayer is not set on a UIPanel");
		}
		else if (this.framesToWait < 0)
		{
			UnityEngine.Debug.LogWarning("UIPanelStaticDelayer.framesToWait can not be less than 0");
		}
		else
		{
			this.OrderStaticDelay();
			this.inited = true;
		}
	}

	private void OnEnable()
	{
		if (this.refreshOnEnable && this.inited)
		{
			this.OrderStaticDelay();
		}
	}

	private void OrderStaticDelay()
	{
		if (this._panel.widgetsAreStatic)
		{
			this._panel.widgetsAreStatic = false;
		}
		base.StartCoroutine(this.SetStaticDelayed(this.framesToWait, this._panel));
	}

	public IEnumerator SetStaticDelayed(int delayFrames, UIPanel panel)
	{
		int num = 0;
		while (num < delayFrames)
		{
			panel.Refresh();
			num++;
			yield return null;
		}
		panel.widgetsAreStatic = true;
		yield break;
	}

	private UIPanel _panel;

	[SerializeField]
	private int framesToWait = 2;

	private bool inited;

	[SerializeField]
	private bool refreshOnEnable;
}
