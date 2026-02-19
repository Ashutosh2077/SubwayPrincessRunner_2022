using System;
using System.Collections.Generic;

public class VariableBool
{
	public void Add(object o)
	{
		if (!this.objects.Contains(o))
		{
			this.objects.Add(o);
		}
		this.UpdateValue();
	}

	public void Clear()
	{
		this.objects.Clear();
		this.UpdateValue();
	}

	public void FireOnChange()
	{
		this.NotifyOnChange();
	}

	private void NotifyOnChange()
	{
		if (this.OnChange != null)
		{
			this.OnChange(this.value);
		}
	}

	public void Remove(object o)
	{
		if (this.objects.Contains(o))
		{
			this.objects.Remove(o);
		}
		this.UpdateValue();
	}

	private void UpdateValue()
	{
		bool flag = this.objects.Count > 0;
		bool flag2 = flag != this.value;
		this.value = flag;
		if (flag2)
		{
			this.NotifyOnChange();
		}
	}

	public bool Value
	{
		get
		{
			return this.value;
		}
	}

	private HashSet<object> objects = new HashSet<object>();

	public VariableBool.OnChangeDelegate OnChange;

	private bool value;

	public delegate void OnChangeDelegate(bool value);
}
