using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubScene
{
	public SubScene()
	{
		this.trackPieces = new Dictionary<TrackPieceType, List<TrackPiece>>();
		this.LastZ = 0f;
	}

	public void AddToDict(TrackPiece tp)
	{
		if (!this.trackPieces.ContainsKey(tp.trackPieceType))
		{
			List<TrackPiece> list = new List<TrackPiece>();
			list.Add(tp);
			this.trackPieces.Add(tp.trackPieceType, list);
		}
		else
		{
			List<TrackPiece> list2 = this.trackPieces[tp.trackPieceType];
			int num = 0;
			int count = list2.Count;
			while (list2[num].zMinimum < tp.zMinimum)
			{
				num++;
				if (num == count)
				{
					break;
				}
			}
			list2.Insert(num, tp);
		}
	}

	public void Restart()
	{
		Vector3 position = Vector3.zero;
		foreach (KeyValuePair<TrackPieceType, List<TrackPiece>> keyValuePair in this.trackPieces)
		{
			List<TrackPiece> value = keyValuePair.Value;
			int i = 0;
			int count = value.Count;
			while (i < count)
			{
				position = value[i].transform.position;
				position.y = -1000f;
				value[i].transform.position = position;
				i++;
			}
		}
		this.LastZ = 0f;
	}

	public Dictionary<TrackPieceType, List<TrackPiece>> TrackPieces
	{
		get
		{
			return this.trackPieces;
		}
	}

	public float LastZ { get; set; }

	public string mainMusic;

	public int minLength;

	public int maxLength;

	public bool allowCityEnd;

	public bool allowFlypack;

	public int order;

	private Dictionary<TrackPieceType, List<TrackPiece>> trackPieces;
}
