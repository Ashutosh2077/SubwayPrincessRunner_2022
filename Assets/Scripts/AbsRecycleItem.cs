using System;

[Serializable]
public abstract class AbsRecycleItem
{
	public abstract bool IsUsing();

	public abstract void Release();

	public abstract void Retain();
}
