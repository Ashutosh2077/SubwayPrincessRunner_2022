using System;
using UnityEngine;

[ExecuteInEditMode]
public class UIShadowHelper<T> : MonoBehaviour where T : UIWidget
{
	private void Awake()
	{
		this._front = base.gameObject.GetComponent<T>();
		this._frontTransform = base.transform;
		if (this.shadow != null)
		{
			this._shadowTransform = this.shadow.cachedTransform;
			this.shadow.depth = this._front.depth - 1;
			this.shadow.gameObject.name = base.name + "Shadow";
		}
	}

	private void OnDisable()
	{
		if (this.shadow != null)
		{
			this.shadow.gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		if (this.shadow != null)
		{
			this.shadow.gameObject.SetActive(true);
		}
	}

	private void Start()
	{
		this.UpdateT();
	}

	private void Update()
	{
		if (this._front == this.shadow)
		{
			UnityEngine.Debug.LogError("front and shadow label are the same!", base.gameObject);
			this.shadow = (T)((object)null);
		}
		if (!this.updateDynamically)
		{
			return;
		}
		if (this._shadowTransform == null)
		{
			this._shadowTransform = this.shadow.cachedTransform;
		}
		this.UpdateT();
	}

	public void UpdateNow()
	{
		this.UpdateT();
	}

	private void UpdateT()
	{
		if (this.shadow != null)
		{
			this.withUpdate();
			if (this.shadow.depth != this._front.depth - 1)
			{
				this.shadow.depth = this._front.depth - 1;
			}
			this._shadowTransform.localPosition = this._frontTransform.localPosition + this.shadowOffset;
		}
	}

	protected virtual void withUpdate()
	{
	}

	public T shadow;

	public bool updateDynamically;

	public Vector3 shadowOffset;

	protected T _front;

	protected Transform _frontTransform;

	protected Transform _shadowTransform;
}
