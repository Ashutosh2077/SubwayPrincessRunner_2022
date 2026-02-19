using System;
using UnityEngine;

public class VisibleObject : MonoBehaviour
{
	public void OnBecameInvisible()
	{
		if (this.OnVisibleChange != null)
		{
			this.OnVisibleChange(false);
		}
	}

	public void OnBecameVisible()
	{
		if (this.OnVisibleChange != null)
		{
			this.OnVisibleChange(true);
		}
	}

	public VisibleObject.OnVisibleChangeDelegate OnVisibleChange;

	public delegate void OnVisibleChangeDelegate(bool isVisible);
}
