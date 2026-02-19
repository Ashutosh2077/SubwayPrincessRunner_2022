using System;
using UnityEngine;

public class TrackObject : MonoBehaviour
{
	public void Activate()
	{
		if (this.OnActivate != null)
		{
			this.OnActivate();
		}
	}

	public void Deactivate()
	{
		if (this.OnDeactivate != null)
		{
			this.OnDeactivate();
		}
	}

	public TrackObject.OnActivateDelegate OnActivate;

	public TrackObject.OnDeactivateDelegate OnDeactivate;

	public delegate void OnActivateDelegate();

	public delegate void OnDeactivateDelegate();
}
