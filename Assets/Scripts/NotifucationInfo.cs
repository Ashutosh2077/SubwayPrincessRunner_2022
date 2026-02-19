using System;

public class NotifucationInfo
{
	public NotifucationInfo()
	{
		this.updateNotificationValue = null;
		this.value = false;
	}

	public void SetValue()
	{
		if (this.updateNotificationValue != null)
		{
			this.value = this.updateNotificationValue();
		}
	}

	public Func<bool> updateNotificationValue;

	public bool value;
}
