using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Condition/ScoreMultiplier")]
[Serializable]
public class ScoreMultiplier : ICondition
{
	public override bool IfMeet()
	{
		return false;
	}

	public int target;
}
