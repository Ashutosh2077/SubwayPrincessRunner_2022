using System;

[Serializable]
public class ClipData
{
	public ClipData(float value, string path, string propertyName)
	{
		this.startValue = value;
		this.path = path;
		this.propertyName = propertyName;
	}

	public float startValue;

	public string path;

	public string propertyName;
}
