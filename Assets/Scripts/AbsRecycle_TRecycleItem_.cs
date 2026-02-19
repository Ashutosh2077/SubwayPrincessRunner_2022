using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsRecycle<TRecycleItem> : MonoBehaviour where TRecycleItem : AbsRecycleItem
{
	protected AbsRecycle()
	{
		this.max = 20;
	}

	public virtual void Awake()
	{
		if (this.origGo == null)
		{
			this.origGo = base.gameObject;
		}
		this.recycleItems = new List<TRecycleItem>();
	}

	public void Instantiate()
	{
		try
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.origGo);
			gameObject.transform.parent = base.transform;
			AbsRecycleItem absRecycleItem = this.NewRecycleItem(gameObject);
			this.recycleItems.Add((TRecycleItem)((object)absRecycleItem));
			gameObject.SetActive(false);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log(ex.Message);
		}
	}

	public abstract TRecycleItem NewRecycleItem(GameObject newGo);

	public virtual TRecycleItem Retain()
	{
		if (this.recycleItems.Count < this.max && this.origGo != null)
		{
			GameObject newGo = UnityEngine.Object.Instantiate<GameObject>(this.origGo);
			AbsRecycleItem absRecycleItem = this.NewRecycleItem(newGo);
			absRecycleItem.Retain();
			this.recycleItems.Add((TRecycleItem)((object)absRecycleItem));
			return absRecycleItem as TRecycleItem;
		}
		for (int i = 0; i < this.recycleItems.Count; i++)
		{
			TRecycleItem result = this.recycleItems[i];
			if (!result.IsUsing())
			{
				result.Retain();
				return result;
			}
		}
		return (TRecycleItem)((object)null);
	}

	public virtual void WarmUp()
	{
		base.StartCoroutine(this.WarmUpC());
	}

	public virtual IEnumerator WarmUpC()
	{
		while (this.recycleItems.Count < this.max)
		{
			if (this.origGo != null)
			{
				this.Instantiate();
				this.Instantiate();
				this.Instantiate();
				this.Instantiate();
				yield return null;
			}
		}
		yield break;
	}

	public GameObject origGo;

	public List<TRecycleItem> recycleItems;

	public int max;
}
