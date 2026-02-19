using System;

public class Variable<T> where T : IComparable
{
	public Variable(T initialValue)
	{
		this.value = initialValue;
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

	public T Value
	{
		get
		{
			return this.value;
		}
		set
		{
			bool flag = value.CompareTo(this.value) != 0;
			this.value = value;
			if (flag)
			{
				this.NotifyOnChange();
			}
		}
	}

	public Variable<T>.OnChangeDelegate OnChange;

	private T value;

	public delegate void OnChangeDelegate(T value);
}
