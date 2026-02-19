using System;
using UnityEngine;

public class RenderQueueModifier : MonoBehaviour
{
	private void LateUpdate()
	{
		if (this.m_target != null && this.m_target.drawCall != null)
		{
			int num = this.m_target.drawCall.renderQueue + ((this.m_type == RenderQueueModifier.RenderType.FRONT) ? 2 : -2);
			if (this._lastQueue != num)
			{
				this._lastQueue = num;
				int i = 0;
				int num2 = this._renderers.Length;
				while (i < num2)
				{
					this._renderers[i].material.renderQueue = this._lastQueue;
					i++;
				}
			}
		}
	}

	private void Start()
	{
		this._renderers = base.GetComponentsInChildren<Renderer>();
	}

	private int _lastQueue;

	private Renderer[] _renderers;

	public UIWidget m_target;

	public RenderQueueModifier.RenderType m_type;

	public enum RenderType
	{
		FRONT,
		BACK
	}
}
