using System;
using UnityEngine;

[Serializable]
public abstract class ICondition : ScriptableObject
{
	public abstract bool IfMeet();
}
