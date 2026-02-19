using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Condition/Keepsake")]
[Serializable]
public class Keepsake : ScriptableObject
{
	public bool IfMeet()
	{
		return this.condition.IfMeet();
	}

	public string sakeName;

	public string iconName;

	public string city;

	public ICondition condition;

	public string tip;
}
