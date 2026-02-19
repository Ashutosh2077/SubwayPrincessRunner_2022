using System;
using System.Collections.Generic;
using System.Text;
using Network;

public class IvyApp
{
	public static IvyApp Instance
	{
		get
		{
			if (IvyApp._instance == null)
			{
				IvyApp._instance = new IvyApp();
			}
			return IvyApp._instance;
		}
	}

	private string GetUrl(IvyApp.RequestCommand command)
	{
		if (command == IvyApp.RequestCommand.Statistics)
		{
			return this.trackEventUrl;
		}
		return this.url;
	}

	public void Recode(string recode, Action<string> listener)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("appid", NetworkConnect.Instance.AppId());
		dictionary.Add("uid", NetworkConnect.Instance.Uid());
		dictionary.Add("redcode", recode);
		IvyHttpHelper.Instance.MakeRequest(this.GetUrl(IvyApp.RequestCommand.Redcode), listener, dictionary);
	}

	public void Statistics(string event1, string event2, string event3, int version, Action<string> listener = null)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("appid", NetworkConnect.Instance.AppId());
		dictionary.Add("uid", NetworkConnect.Instance.Uid());
		if (!string.IsNullOrEmpty(event1))
		{
			dictionary.Add("event1", event1);
		}
		else if (!string.IsNullOrEmpty(event2))
		{
			dictionary.Add("event2", event2);
		}
		else if (!string.IsNullOrEmpty(event3))
		{
			dictionary.Add("event3", event3);
		}
		dictionary["version"] = this.get_uft8(RiseSdk.Instance.GetConfig(9));
		IvyHttpHelper.Instance.GetRequest(this.GetUrl(IvyApp.RequestCommand.Statistics), listener, dictionary);
	}

	public string get_uft8(string unicodeString)
	{
		UTF8Encoding utf8Encoding = new UTF8Encoding();
		byte[] bytes = utf8Encoding.GetBytes(unicodeString);
		return utf8Encoding.GetString(bytes);
	}

	private string trackEventUrl = "http://sanxiao.iibingo.com/api/Dot/OnEventVer";

	private string url = "http://sanxiao.iibingo.com/api/Redcode/authRedCode";

	private static IvyApp _instance;

	public enum RequestCommand
	{
		Statistics,
		Redcode,
		Other
	}
}
