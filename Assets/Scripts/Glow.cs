using System;
using System.Collections;
using UnityEngine;

public class Glow : MonoBehaviour
{
	private void Reset()
	{
		if (this.meshRenderer == null)
		{
			this.meshRenderer = base.GetComponentInChildren<MeshRenderer>();
		}
		if (this.anim == null)
		{
			this.anim = base.GetComponentInChildren<Animation>();
		}
	}

	public void Awake()
	{
		Transform parent = base.transform.parent;
		Transform transform = (!(parent != null)) ? null : parent.parent;
		if (DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.Low && (parent.gameObject.name.Contains("coin") || (transform != null && transform.gameObject.name.Contains("coin"))))
		{
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform2 = (Transform)obj;
					UnityEngine.Object.Destroy(transform2.gameObject);
				}
			}
			finally
			{
			}
			if (this.meshRenderer == null)
			{
				this.meshRenderer = base.GetComponentInChildren<MeshRenderer>();
			}
			if (this.anim == null)
			{
				this.anim = base.GetComponentInChildren<Animation>();
			}
			if (this.meshRenderer != null)
			{
				this.meshRenderer.enabled = false;
				base.enabled = false;
				UnityEngine.Object.Destroy(this.meshRenderer);
				this.meshRenderer = null;
			}
		}
		else
		{
			if (this.meshRenderer == null)
			{
				this.meshRenderer = base.GetComponentInChildren<MeshRenderer>();
			}
			if (this.anim == null)
			{
				this.anim = base.GetComponentInChildren<Animation>();
			}
		}
	}

	public void SetVisible(bool visible)
	{
		if (this.anim != null)
		{
			if (visible)
			{
				this.anim.Play();
			}
			else
			{
				this.anim.Stop();
			}
		}
		if (this.meshRenderer != null)
		{
			this.meshRenderer.enabled = visible;
			base.enabled = visible;
		}
	}

	public MeshRenderer meshRenderer;

	[SerializeField]
	private Animation anim;
}
