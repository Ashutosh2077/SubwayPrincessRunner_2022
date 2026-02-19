using System;
using UnityEngine;

public class UIScrollClick : MonoBehaviour
{
	private void Awake()
	{
		if (this.target == null)
		{
			return;
		}
		this.scollClick = this.target.GetComponent<IScrollClick>();
	}

	private void OnClick()
	{
		if (this.scollClick == null)
		{
			return;
		}
		Vector2 pos = UICamera.currentTouch.pos;
		this.scollClick.ScrollClicked(pos);
	}

	[SerializeField]
	private GameObject target;

	private IScrollClick scollClick;
}
