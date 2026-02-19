using System;

public class WaitUntil : Point
{
	public override void OnEnd(PointsManager manager)
	{
	}

	public override void OnImility(PointsManager manager)
	{
	}

	public override void OnInit(PointsManager manager)
	{
	}

	public override void OnStart(PointsManager manager)
	{
		this.uiscreen = UIScreenController.Instance;
	}

	public override bool OnUpdate(PointsManager manager)
	{
		if ("FrontUI".Equals(this.uiscreen.GetTopScreenName()) && this.uiscreen.IsPopupQueueEmpty())
		{
			manager.GoToNextPoint();
		}
		return false;
	}

	public override void OnWholeEnd(PointsManager manager)
	{
	}

	private UIScreenController uiscreen;
}
