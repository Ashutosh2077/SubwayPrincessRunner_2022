using System;
using UnityEngine;

[RequireComponent(typeof(TrackObject))]
public class BaseO : MonoBehaviour
{
	protected virtual void Awake()
	{
		TrackObject component = base.GetComponent<TrackObject>();
		component.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(component.OnActivate, new TrackObject.OnActivateDelegate(this.OnActivate));
		component.OnDeactivate = (TrackObject.OnDeactivateDelegate)Delegate.Combine(component.OnDeactivate, new TrackObject.OnDeactivateDelegate(this.OnDeactivate));
	}

	public virtual void OnActivate()
	{
	}

	public virtual void OnDeactivate()
	{
	}
}
