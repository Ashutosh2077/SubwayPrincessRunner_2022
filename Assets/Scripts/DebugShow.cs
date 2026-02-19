using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugShow : MonoBehaviour
{
	public static void SetWillShowDebugInfoByGUI(bool value)
	{
		DebugShow.willShowDebugInfoByGUI = value;
	}

	public static void Log(string info)
	{
		if (!DebugShow.willShowDebugInfoByGUI)
		{
			return;
		}
		DebugShow.messages.Add(info);
	}

	private void OnGUI()
	{
		if (!DebugShow.willShowDebugInfoByGUI)
		{
			return;
		}
		GUI.color = Color.red;
		GUI.Label(this.startRect, "The Messages Below:");
		this.nextRect = this.startRect;
		foreach (string str in DebugShow.messages)
		{
			this.nextRect = new Rect(this.nextRect.xMin + this.offset.xMin, this.nextRect.yMax + this.offset.yMax, this.nextRect.width + this.offset.width, this.nextRect.height + this.offset.height);
			GUI.Label(this.nextRect, "Log:" + str);
		}
		this.nextRect = new Rect(100f, (float)(Screen.height - 100), 100f, 100f);
		if (GUI.Button(this.nextRect, "Clear"))
		{
			DebugShow.messages.Clear();
		}
	}

	private static List<string> messages = new List<string>();

	private static bool willShowDebugInfoByGUI;

	[SerializeField]
	private Rect startRect;

	[SerializeField]
	private Rect offset;

	private Rect nextRect;
}
