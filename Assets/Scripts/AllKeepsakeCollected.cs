using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Condition/AllKeepsakeCollected")]
[Serializable]
public class AllKeepsakeCollected : ICondition
{
	public override bool IfMeet()
	{
		return false;
	}

	public string city;
}
