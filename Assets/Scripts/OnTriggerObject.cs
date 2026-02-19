using System;
using UnityEngine;

public class OnTriggerObject : MonoBehaviour
{
	public void OnTriggerEnter(Collider collider)
	{
		if (this.OnEnter != null)
		{
			this.OnEnter(collider);
		}
	}

	public void OnTriggerExit(Collider collider)
	{
		if (this.OnExit != null)
		{
			this.OnExit(collider);
		}
	}

	public OnTriggerObject.OnEnterDelegate OnEnter;

	public OnTriggerObject.OnExitDelegate OnExit;

	public delegate void OnEnterDelegate(Collider collider);

	public delegate void OnExitDelegate(Collider collider);
}
