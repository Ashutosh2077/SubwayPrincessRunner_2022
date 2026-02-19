using System;
using UnityEngine;

public abstract class Point : MonoBehaviour
{
	public abstract void OnStart(PointsManager manager);

	public abstract void OnEnd(PointsManager manager);

	public abstract bool OnUpdate(PointsManager manager);

	public abstract void OnImility(PointsManager manager);

	public abstract void OnInit(PointsManager manager);

	public abstract void OnWholeEnd(PointsManager manager);

	public bool move;

	public int group;
}
