using System;
using UnityEngine;

public class Create : Point
{
	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnImility(PointsManager manager)
	{
		this.Initialized(manager);
	}

	public override void OnInit(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		this.time = UnityEngine.Random.Range(this.minInterval, this.maxInterval);
	}

	public override bool OnUpdate(PointsManager manager)
	{
		if (this.time > 0f)
		{
			this.time -= Time.deltaTime;
		}
		else
		{
			this.Initialized(manager);
			manager.GoToNextPoint();
		}
		return false;
	}

	public override void OnWholeEnd(PointsManager manager)
	{
	}

	private void Initialized(PointsManager manager)
	{
		manager.InitializedModel(Vector3.zero, new Vector3(0f, 180f, 0f));
	}

	[SerializeField]
	private float minInterval;

	[SerializeField]
	private float maxInterval;

	private float time;
}
