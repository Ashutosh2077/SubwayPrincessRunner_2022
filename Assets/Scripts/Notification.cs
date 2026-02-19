using System;
using UnityEngine;

public class Notification : MonoBehaviour
{
	public int[] GetIds()
	{
		return this.ids;
	}

	public void SetNotification(int id, bool value)
	{
		if (id < 0 || id >= this.dots.Length)
		{
			return;
		}
		this.dots[id].enabled = value;
	}

	[SerializeField]
	private UISprite[] dots;

	[SerializeField]
	private int[] ids;
}
