using System;
using UnityEngine;

public class DeviceInfo
{
	private DeviceInfo()
	{
		if (Screen.height < 500)
		{
			this.formFactor = DeviceInfo.FormFactor.small;
		}
		else if (Screen.height < 900)
		{
			this.formFactor = DeviceInfo.FormFactor.medium;
		}
		else
		{
			this.formFactor = DeviceInfo.FormFactor.large;
		}
		if (this.isTablet())
		{
			this.formFactor = DeviceInfo.FormFactor.iPad;
		}
		if (Screen.height >= 500 && Screen.width > 320)
		{
			this.isHighres = true;
		}
		else
		{
			this.isHighres = false;
		}
		this.formFactor = DeviceInfo.FormFactor.small;
		this.isHighres = false;
		this.dpi = Screen.dpi;
		if (this.dpi <= 0f)
		{
			this.dpi = 300f;
		}
		if (this.isDeviceLowPerformance())
		{
			this.performanceLevel = DeviceInfo.PerformanceLevel.Low;
		}
	}

	private bool isDeviceLowPerformance()
	{
		int processorCount = SystemInfo.processorCount;
		string processorType = SystemInfo.processorType;
		int systemMemorySize = SystemInfo.systemMemorySize;
		int graphicsMemorySize = SystemInfo.graphicsMemorySize;
		if (processorCount >= 4)
		{
			return false;
		}
		if (processorType.Contains("rev"))
		{
			int num = processorType.IndexOf("rev");
			string text = processorType.Substring(num + 3).Trim();
			if (text.Contains(" "))
			{
				int length = text.IndexOf(" ");
				int num2;
				if (int.TryParse(text.Substring(0, length).Trim(), out num2))
				{
					bool flag = processorCount >= 2;
					bool flag2 = num2 >= 6;
					bool flag3 = systemMemorySize >= 512;
					bool flag4 = graphicsMemorySize >= 250;
					if (flag && flag2 && flag3 && flag4)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private bool isTablet()
	{
		float f = (Screen.dpi > 0f) ? ((float)Screen.width / Screen.dpi) : ((float)Screen.width);
		float f2 = (Screen.dpi > 0f) ? ((float)Screen.height / Screen.dpi) : ((float)Screen.height);
		double num = (double)Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
		return num >= 6.0;
	}

	public static DeviceInfo Instance
	{
		get
		{
			if (DeviceInfo._instance == null)
			{
				DeviceInfo._instance = new DeviceInfo();
			}
			return DeviceInfo._instance;
		}
	}

	public DeviceInfo.PerformanceLevel performanceLevel = DeviceInfo.PerformanceLevel.High;

	public string deviceModel = SystemInfo.deviceModel;

	public readonly float dpi;

	public readonly DeviceInfo.FormFactor formFactor;

	public readonly bool isHighres;

	private static DeviceInfo _instance;

	public enum FormFactor
	{
		iPhone,
		iPad,
		small,
		medium,
		large,
		iPhone5,
		iPhone6,
		iPhone6Plus
	}

	public enum PerformanceLevel
	{
		Low,
		Medium,
		High
	}
}
