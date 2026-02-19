using System;
using UnityEngine;

public class UIButtonSendCount : MonoBehaviour
{
	public void OnClick()
	{
		if (this.IdString.Length > 0)
		{
			for (int i = 0; i < this.IdString.Length; i++)
			{
				if (this.IdString != null)
				{
					string @event = this.IdString[i] + "_" + this.toPanelString;
					IvyApp.Instance.Statistics(string.Empty, string.Empty, @event, 0, null);
				}
			}
		}
		else
		{
			UnityEngine.Debug.LogError("Idstring is null!!");
		}
	}

	public string[] IdString = new string[2];

	public string toPanelString;
}
