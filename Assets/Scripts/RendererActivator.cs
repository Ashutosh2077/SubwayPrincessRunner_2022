using System;
using UnityEngine;

public class RendererActivator : BaseO
{
	protected override void Awake()
	{
		this.render = base.GetComponent<Renderer>();
		base.Awake();
		this.OnDeactivate();
	}

	public override void OnActivate()
	{
		this.render.enabled = true;
	}

	public override void OnDeactivate()
	{
		this.render.enabled = false;
	}

	private Renderer render;
}
