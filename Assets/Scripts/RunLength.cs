using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Condition/RunLength")]
[Serializable]
public class RunLength : ICondition
{
	public override bool IfMeet()
	{
		return false;
	}

	public int target;
}
