using System;
using UnityEngine;

public class UIBasicButton : MonoBehaviour
{
	protected virtual void OnClick()
	{
		if (this.trigger == UIBasicButton.Trigger.OnClick)
		{
			this.Send();
		}
	}

	protected virtual void OnHover(bool isOver)
	{
		if ((isOver && this.trigger == UIBasicButton.Trigger.OnMouseOver) || (!isOver && this.trigger == UIBasicButton.Trigger.OnMouseOut))
		{
			this.Send();
		}
	}

	protected virtual void OnPress(bool isPressed)
	{
		if ((isPressed && this.trigger == UIBasicButton.Trigger.OnPress) || (!isPressed && this.trigger == UIBasicButton.Trigger.OnRelease))
		{
			this.Send();
		}
	}

	protected virtual void Send()
	{
	}

	public UIBasicButton.Trigger trigger;

	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease
	}
}
