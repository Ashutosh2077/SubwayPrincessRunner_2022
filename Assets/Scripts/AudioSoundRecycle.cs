using System;
using UnityEngine;

public class AudioSoundRecycle : AbsRecycle<AudioRecycleItem>
{
	public override AudioRecycleItem NewRecycleItem(GameObject newGo)
	{
		return new AudioRecycleItem
		{
			audio = newGo.GetComponent<AudioSource>()
		};
	}

	public virtual void Pause()
	{
		for (int i = 0; i < this.recycleItems.Count; i++)
		{
			this.recycleItems[i].audio.Pause();
		}
	}

	public virtual void Stop()
	{
		for (int i = 0; i < this.recycleItems.Count; i++)
		{
			this.recycleItems[i].audio.Stop();
		}
	}

	public override void WarmUp()
	{
		while (this.recycleItems.Count < this.max)
		{
			if (this.origGo != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.origGo);
				gameObject.transform.parent = base.transform;
				AbsRecycleItem absRecycleItem = this.NewRecycleItem(gameObject);
				this.recycleItems.Add((AudioRecycleItem)absRecycleItem);
			}
		}
	}
}
