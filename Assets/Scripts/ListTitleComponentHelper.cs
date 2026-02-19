using System;
using UnityEngine;

public class ListTitleComponentHelper : MonoBehaviour
{
	public void Setup(string text, string descripe)
	{
		this.bigLabel.text = text;
		this.smallLabel.text = descripe;
	}

	[SerializeField]
	private UILabel bigLabel;

	[SerializeField]
	private UILabel smallLabel;
}
