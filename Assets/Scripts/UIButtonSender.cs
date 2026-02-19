using System;
using UnityEngine;

public class UIButtonSender : MonoBehaviour
{
	protected virtual void OnClick()
	{
		if (this.trigger == UIButtonSender.Trigger.OnClick)
		{
			this.Send();
		}
	}

	protected virtual void OnHover(bool isOver)
	{
		if ((isOver && this.trigger == UIButtonSender.Trigger.OnMouseOver) || (!isOver && this.trigger == UIButtonSender.Trigger.OnMouseOut))
		{
			this.Send();
		}
	}

	protected virtual void OnPress(bool isPressed)
	{
		if ((isPressed && this.trigger == UIButtonSender.Trigger.OnPress) || (!isPressed && this.trigger == UIButtonSender.Trigger.OnPress))
		{
			this.Send(isPressed);
		}
	}

	protected virtual void Send()
	{
	}

	protected virtual void Send(bool isPressed)
	{
	}

	public UIButtonSender.Trigger trigger;

	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease
	}
}
