using System;
using UnityEngine;

public class RenderersActivator : BaseO
{
	protected override void Awake()
	{
		this.renderers = base.GetComponentsInChildren<Renderer>();
		base.Awake();
		this.OnDeactivate();
	}

	public override void OnActivate()
	{
		int i = 0;
		int num = this.renderers.Length;
		while (i < num)
		{
			if (this.renderers[i] != null)
			{
				this.renderers[i].enabled = true;
			}
			i++;
		}
	}

	public override void OnDeactivate()
	{
		int i = 0;
		int num = this.renderers.Length;
		while (i < num)
		{
			if (this.renderers[i] != null)
			{
				this.renderers[i].enabled = false;
			}
			i++;
		}
	}

	private Renderer[] renderers;
}
