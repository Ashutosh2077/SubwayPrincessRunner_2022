using System;
using System.Collections.Generic;
using UnityEngine;

public class TrackPiece : MonoBehaviour
{
	public void Awake()
	{
		this.objects = base.GetComponentsInChildren<TrackObject>(true);
		if (!this.zMaximumActive)
		{
			this.zMaximum = float.MaxValue;
		}
		Track component = base.transform.root.GetComponent<Track>();
		component.AddToChunks(this);
		Randomizer[] componentsInChildren = base.GetComponentsInChildren<Randomizer>(true);
		int i = 0;
		int num = componentsInChildren.Length;
		while (i < num)
		{
			componentsInChildren[i].InitializeRandomizer();
			i++;
		}
		if (this.CheckPoints != null && this.CheckPoints.Count > 0)
		{
			this.CheckPoints.Sort((TrackPiece.TrackCheckPoint x, TrackPiece.TrackCheckPoint y) => x.Z.CompareTo(y.Z));
		}
		if (!this.hasSorted)
		{
			Array.Sort<TrackObject>(this.objects, (TrackObject to1, TrackObject to2) => to1.transform.position.z.CompareTo(to2.transform.position.z));
			this.hasSorted = true;
		}
	}

	public void Deactivate()
	{
		int i = 0;
		int num = this.objects.Length;
		while (i < num)
		{
			this.objects[i].Deactivate();
			i++;
		}
	}

	public void DeactivateObstacles(float maxZ)
	{
		foreach (object obj in base.transform)
		{
			Transform target = (Transform)obj;
			this.DeactiveObstaclesRecursive(target, maxZ);
		}
	}

	private void DeactiveObstaclesRecursive(Transform target, float maxZ)
	{
		float num = (!(target.GetComponent<Collider>() == null)) ? target.GetComponent<Collider>().bounds.min.z : target.transform.position.z;
		if (target.GetComponent<FlagObject>() == null)
		{
			foreach (object obj in target)
			{
				Transform target2 = (Transform)obj;
				this.DeactiveObstaclesRecursive(target2, maxZ);
			}
		}
		else if (num < maxZ && target.gameObject.layer != 16)
		{
			Vector3 localPosition = target.localPosition;
			if (!this.hiddenObstacles.ContainsKey(target))
			{
				this.hiddenObstacles.Add(target, localPosition);
			}
			target.localPosition = new Vector3(localPosition.x, -1000f, localPosition.z);
		}
	}

	private void DrawCheckPointGizmos()
	{
		if (this.CheckPoints == null || this.CheckPoints.Count <= 0)
		{
			return;
		}
		int i = 0;
		int count = this.CheckPoints.Count;
		while (i < count)
		{
			Vector3 position = base.transform.position;
			position.z = this.CheckPoints[i].Z;
			Gizmos.DrawSphere(position + Vector3.up * 5f, 5f);
			i++;
		}
	}

	public float GetLastCheckPoint(float characterZ)
	{
		int index = 0;
		for (int i = this.CheckPoints.Count - 1; i > 0; i--)
		{
			if (characterZ >= this.CheckPoints[i].Z)
			{
				index = i;
				break;
			}
		}
		return this.CheckPoints[index].Z;
	}

	public TrackPiece.TrackCheckPoint GetNextCheckPoint(float characterZ)
	{
		TrackPiece.TrackCheckPoint trackCheckPoint = null;
		for (int i = 0; i < this.CheckPoints.Count; i++)
		{
			if (characterZ <= this.CheckPoints[i].Z + base.transform.position.z)
			{
				trackCheckPoint = new TrackPiece.TrackCheckPoint();
				trackCheckPoint.y = this.CheckPoints[i].y;
				trackCheckPoint.Z = this.CheckPoints[i].Z + base.transform.position.z;
				break;
			}
		}
		return trackCheckPoint;
	}

	public void OnDrawGizmos()
	{
		this.DrawCheckPointGizmos();
	}

	public void RestoreHiddenObstacles()
	{
		foreach (KeyValuePair<Transform, Vector3> keyValuePair in this.hiddenObstacles)
		{
			if (keyValuePair.Key != null)
			{
				keyValuePair.Key.localPosition = keyValuePair.Value;
			}
		}
		this.hiddenObstacles.Clear();
	}

	public int subscene;

	public TrackPieceType trackPieceType;

	public float zSize = 40f;

	public int probability = 1;

	public float zMinimum;

	public bool zMaximumActive;

	public float zMaximum;

	public List<TrackPiece.TrackCheckPoint> CheckPoints;

	public TrackObject[] objects;

	public bool hasSorted;

	private Dictionary<Transform, Vector3> hiddenObstacles = new Dictionary<Transform, Vector3>();

	[Serializable]
	public class TrackCheckPoint
	{
		public float y;

		public float Z;
	}
}
