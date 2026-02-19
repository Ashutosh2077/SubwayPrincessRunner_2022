using System;

public class SynchroniseRule
{
	public SynchroniseRule(float interval)
	{
		this._interval = interval;
		this.hasGotInitValue = false;
		this._waitingForRespond = false;
		this._time = -1f;
	}

	public bool Check()
	{
		return !this.hasGotInitValue && (!this._waitingForRespond || RealTimeTracker.time > this._time);
	}

	public void Request()
	{
		this._waitingForRespond = true;
		this._time = RealTimeTracker.time + this._interval;
	}

	public void Respond()
	{
		this._waitingForRespond = false;
	}

	public void Reset()
	{
		this.hasGotInitValue = false;
		this._waitingForRespond = false;
		this._time = -1f;
	}

	public bool hasGotInitValue { get; set; }

	private bool _waitingForRespond;

	private float _time;

	private float _interval;
}
