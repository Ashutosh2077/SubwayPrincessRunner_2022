using System;

public class Wait : Point
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
		manager.Wait = true;
		manager.TargetAnim.Stop();
	}

	public override bool OnUpdate(PointsManager manager)
	{
		return false;
	}

	public override void OnImility(PointsManager manager)
	{
	}
}
