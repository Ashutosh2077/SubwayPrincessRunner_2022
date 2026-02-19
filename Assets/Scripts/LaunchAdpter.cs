using System;
using UnityEngine;

public class LaunchAdpter : MonoBehaviour
{
	private void Start()
	{
		float num = (float)this.texture.height / (float)this.texture.width;
		if (UIBaseScreen.IsOutOfProportion())
		{
			this.texture.height = this.root.activeHeight;
			this.texture.width = (int)((float)this.texture.height / num);
		}
		else
		{
			this.texture.width = (int)((float)this.root.activeHeight / (float)Screen.height * (float)Screen.width);
			this.texture.height = (int)((float)this.texture.width * num);
		}
	}

	[SerializeField]
	private UIWidget texture;

	[SerializeField]
	private UIRoot root;
}
