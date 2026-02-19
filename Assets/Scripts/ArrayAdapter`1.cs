using System;
using System.Collections.Generic;

public class ArrayAdapter<T> : ListAdapter
{
	public ArrayAdapter(List<T> list)
	{
		this._dataList = list;
	}

	public virtual void AddItem(T element)
	{
		this._dataList.Add(element);
	}

	public virtual void AddMany(IEnumerable<T> list)
	{
		this._dataList.AddRange(list);
	}

	public virtual void Clear()
	{
		this._dataList.Clear();
	}

	public override int GetCount()
	{
		return this._dataList.Count;
	}

	public virtual T GetItem(int index)
	{
		return this._dataList[index];
	}

	public virtual void Insert(int index, T element)
	{
		this._dataList.Insert(index, element);
	}

	public virtual void RemoveItem(T element)
	{
		if (this._dataList.Count > 0)
		{
			this._dataList.Remove(element);
		}
	}

	public virtual void RemoveItemAt(int index)
	{
		if (this._dataList.Count > 0)
		{
			this._dataList.RemoveAt(index);
		}
	}

	public List<T> _dataList;
}
