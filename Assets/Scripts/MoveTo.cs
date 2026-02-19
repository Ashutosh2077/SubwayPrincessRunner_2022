using System;

public class MoveTo : Point
{
	public override void OnInit(PointsManager manager)
	{
	}

	public override void OnWholeEnd(PointsManager manager)
	{
	}

	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		manager.SetTransformToTarget();
	}

	public override bool OnUpdate(PointsManager manager)
	{
		manager.GoToNextPoint();
		return false;
	}

	public override void OnImility(PointsManager manager)
	{
		manager.SetTransformTo(base.transform);
	}
}
