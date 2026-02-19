using System;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsObserver
{
	private NotificationsObserver()
	{
		if (NotificationsObserver._instance == null)
		{
			NotificationsObserver._instance = this;
			this.notifications = new List<Notification>();
			this.notificationData = new Dictionary<NotificationType, NotifucationInfo>();
			int i = 0;
			int num = 6;
			while (i < num)
			{
				NotificationType key = (NotificationType)i;
				NotifucationInfo value = new NotifucationInfo();
				this.notificationData.Add(key, value);
				i++;
			}
		}
		else
		{
			UnityEngine.Debug.LogError("There is more than one NotificationsObserver in the scene!");
		}
	}

	public void RegisterUpdateNotificationValue(NotificationType type, Func<bool> func)
	{
		if (!this.notificationData.ContainsKey(type))
		{
			return;
		}
		this.notificationData[type].updateNotificationValue = func;
	}

	public void RegisterNotificationAction(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		Notification component = go.GetComponent<Notification>();
		if (component == null)
		{
			return;
		}
		this.notifications.Add(component);
	}

	public void NotifyNotificationDataChange(NotificationType type)
	{
		this.RefreshNotificationData(type);
		this.RefreshNotificationListShow(type);
	}

	public void NotifyNotificationDataChange(NotificationType type, bool value)
	{
		this.SetNotificationInfoValue(type, value);
		this.RefreshNotificationListShow(type);
	}

	public void NotifyNotificationDataChange()
	{
		int i = 0;
		int num = 6;
		while (i < num)
		{
			NotificationType type = (NotificationType)i;
			this.RefreshNotificationData(type);
			i++;
		}
		this.RefreshNotificationListShow();
	}

	private void RefreshNotificationListShow()
	{
		if (this.notifications == null || this.notifications.Count <= 0)
		{
			return;
		}
		int i = 0;
		int count = this.notifications.Count;
		while (i < count)
		{
			this.RefreshOneNotificationShow(this.notifications[i]);
			i++;
		}
	}

	private void RefreshNotificationListShow(NotificationType type)
	{
		if (this.notifications == null || this.notifications.Count <= 0)
		{
			return;
		}
		int i = 0;
		int count = this.notifications.Count;
		while (i < count)
		{
			this.RefreshOneNotificationShow(type, this.notifications[i]);
			i++;
		}
	}

	private void RefreshOneNotificationShow(Notification notificaiton)
	{
		int[] ids = notificaiton.GetIds();
		for (int i = 0; i < ids.Length; i++)
		{
			this.SetNotificationID(notificaiton, ids[i], i);
		}
	}

	private void RefreshOneNotificationShow(NotificationType type, Notification notificaiton)
	{
		int[] ids = notificaiton.GetIds();
		int i = 0;
		int num = ids.Length;
		while (i < num)
		{
			int num2 = 1 << (int)type;
			if ((ids[i] & num2) != 0)
			{
				this.SetNotificationID(notificaiton, ids[i], i);
			}
			i++;
		}
	}

	private void SetNotificationID(Notification notificaiton, int id, int order)
	{
		bool flag = false;
		int num = 1;
		for (int i = 0; i < 6; i++)
		{
			if ((id & num) != 0)
			{
				flag = (flag || this.GetValueByIndex(i));
			}
			num <<= 1;
		}
		notificaiton.SetNotification(order, flag);
	}

	private bool GetValueByIndex(int index)
	{
		return this.GetValueByNotificationType((NotificationType)index);
	}

	private bool GetValueByNotificationType(NotificationType type)
	{
		return this.notificationData.ContainsKey(type) && this.notificationData[type].value;
	}

	private void RefreshNotificationData(NotificationType type)
	{
		if (!this.notificationData.ContainsKey(type))
		{
			return;
		}
		this.notificationData[type].SetValue();
	}

	private void SetNotificationInfoValue(NotificationType type, bool value)
	{
		if (!this.notificationData.ContainsKey(type))
		{
			return;
		}
		this.notificationData[type].value = value;
	}

	public static NotificationsObserver Instance
	{
		get
		{
			if (NotificationsObserver._instance == null)
			{
				NotificationsObserver._instance = new NotificationsObserver();
			}
			return NotificationsObserver._instance;
		}
	}

	private static NotificationsObserver _instance;

	private Dictionary<NotificationType, NotifucationInfo> notificationData;

	public List<Notification> notifications;
}
