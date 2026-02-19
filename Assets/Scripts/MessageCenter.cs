using System;
using UnityEngine;

public class MessageCenter : MonoBehaviour
{
	private void Awake()
	{
		MessageCenter._instance = this;
	}

	public static MessageCenter Instance
	{
		get
		{
			if (MessageCenter._instance == null)
			{
				UnityEngine.Debug.Log("Instance requested before being instantiated");
				MessageCenter._instance = (UnityEngine.Object.FindObjectOfType(typeof(MessageCenter)) as MessageCenter);
				if (MessageCenter._instance == null)
				{
					UnityEngine.Debug.LogError("MessageCenter not found in the scene.");
				}
			}
			return MessageCenter._instance;
		}
	}

	public static bool IsInstanced
	{
		get
		{
			return MessageCenter._instance != null;
		}
	}

	private static MessageCenter _instance;
}
