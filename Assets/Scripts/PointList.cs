using System;
using UnityEngine;

public class PointList : MonoBehaviour
{
	public void Play()
	{
		this.currentPointId = 0;
	}

	public int OnBreak()
	{
		if (this.currentPointId == -1)
		{
			return -1;
		}
		int num = this.currentPointId;
		while (num < this.Count && this.wayPoints[num].group != -1)
		{
			num++;
		}
		if (num >= this.Count)
		{
			return this.currentPointId = -1;
		}
		return this.currentPointId = num;
	}

	private void Reset()
	{
		this.wayPoints = new Point[base.transform.childCount];
		int i = 0;
		int childCount = base.transform.childCount;
		while (i < childCount)
		{
			this.wayPoints[i] = base.transform.GetChild(i).GetComponent<Point>();
			base.transform.GetChild(i).gameObject.name = string.Format("{0:00}", i);
			i++;
		}
	}

	private void OnDrawGizmos()
	{
		if (this.wayPoints == null)
		{
			return;
		}
		Gizmos.color = Color.blue;
		for (int i = 0; i < this.Count - 1; i++)
		{
			if (!(this.wayPoints[i] == null))
			{
				if (this.wayPoints[i].group == 0 || this.wayPoints[i].group == this.showgroup)
				{
					int num = i;
					do
					{
						num++;
					}
					while (num < this.Count && this.wayPoints[i] != null && this.wayPoints[num].group != 0 && this.wayPoints[num].group != this.showgroup);
					if (num < this.Count)
					{
						Gizmos.DrawLine(this.wayPoints[i].transform.position, this.wayPoints[num].transform.position);
					}
				}
			}
		}
		Gizmos.color = Color.red;
		foreach (Point point in this.wayPoints)
		{
			Gizmos.DrawSphere(point.transform.position, 1f);
		}
	}

	public int WaitNext(PointsManager manager)
	{
		if (this.currentPointId == -1)
		{
			return -1;
		}
		int num = this.currentPointId;
		while (num < this.Count && !(this.wayPoints[num] is Wait))
		{
			this.wayPoints[num].OnImility(manager);
			num++;
		}
		if (num >= this.Count - 1)
		{
			return -1;
		}
		return this.currentPointId = num + 1;
	}

	public int CalcNext()
	{
		if (this.currentPointId == -1)
		{
			return -1;
		}
		int num = this.currentPointId;
		int group = this.wayPoints[this.currentPointId].group;
		if (group != 0)
		{
			if (group % 2 == 0)
			{
				do
				{
					num++;
					num %= this.Count;
				}
				while (this.wayPoints[num].group != group);
				return num;
			}
			do
			{
				num++;
			}
			while (num < this.Count && this.wayPoints[num].group != group);
			if (num >= this.Count)
			{
				return -1;
			}
			return num;
		}
		else
		{
			num++;
			if (num >= this.Count)
			{
				return -1;
			}
			return num;
		}
	}

	public int Next()
	{
		return this.currentPointId = this.CalcNext();
	}

	public int CurrentId
	{
		get
		{
			return this.currentPointId;
		}
	}

	public Point CurrentPoint
	{
		get
		{
			return this.wayPoints[this.currentPointId];
		}
	}

	public Point this[int index]
	{
		get
		{
			if (this.wayPoints != null && index >= 0 && index < this.Count)
			{
				return this.wayPoints[index];
			}
			return null;
		}
	}

	public int Count
	{
		get
		{
			if (this.wayPoints == null)
			{
				return 0;
			}
			return this.wayPoints.Length;
		}
	}

	[SerializeField]
	private Point[] wayPoints;

	private int currentPointId;

	[SerializeField]
	private int showgroup;
}
