using System;

[Serializable]
public class TopRunInfo
{
	public TopRunInfo()
	{
		this.playerName = "----";
		this.facebookName = "----";
		this.countryCode = "NotSet";
		this.isVip = "no";
		this.pictureUrl = null;
	}

	public string playerName;

	public string facebookName;

	public string countryCode;

	public string isVip;

	public string pictureUrl;
}
