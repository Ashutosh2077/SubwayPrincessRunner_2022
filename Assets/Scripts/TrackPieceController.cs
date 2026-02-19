using System;
using System.Collections.Generic;
using UnityEngine;

public class TrackPieceController
{
	public void SetTrackPieces(SubScene sub)
	{
		if (sub == null)
		{
			return;
		}
		this.trackPieces = sub.TrackPieces;
	}

	public bool CanDeliver()
	{
		return this.randomSpace.Count > 0;
	}

	public TrackPiece GetJetPakPiece(int index)
	{
		if ((float)index < 0f || index >= this.trackPieces[TrackPieceType.Jetpack].Count)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Illegal TrackChunk used in jetpack mode. Index=",
				index
			}));
			UnityEngine.Debug.Break();
			return null;
		}
		return this.trackPieces[TrackPieceType.Jetpack][index];
	}

	public TrackPiece GetTransitionPiece(int index)
	{
		if ((float)index < 0f || index >= this.trackPieces[TrackPieceType.Transition].Count)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Illegal TrackChunk used in Transition mode. Index=",
				index
			}));
			UnityEngine.Debug.Break();
			return null;
		}
		return this.trackPieces[TrackPieceType.Transition][index];
	}

	public TrackPiece GetPieceBySpecialTrackPieceType(TrackPieceType type)
	{
		this.activeTrackPieces.RemoveAll((TrackPiece piece) => piece.trackPieceType != TrackPieceType.Normal);
		this.activeTrackPieces.AddRange(this.trackPieces[type]);
		this.Recalculate(true);
		return this.GetRandomActive();
	}

	public TrackPiece GetRandomActive()
	{
		int index = UnityEngine.Random.Range(0, this.randomSpace.Count);
		int index2 = this.randomSpace[index];
		return this.activeTrackPieces[index2];
	}

	public void Initialize(float z)
	{
		this.activeTrackPieces.Clear();
		this.lastAddedIndex = -1;
		List<TrackPiece> list = this.trackPieces[TrackPieceType.Normal];
		for (int i = 0; i < list.Count; i++)
		{
			TrackPiece trackPiece = list[i];
			if (trackPiece.zMinimum <= z && z < trackPiece.zMaximum)
			{
				this.activeTrackPieces.Add(trackPiece);
				this.lastAddedIndex = i;
			}
		}
		this.Recalculate(false);
	}

	public void MoveForward(float z, bool changeEnvEnable, bool forceChangeEnv, bool changeCityEnable, bool forceChangeCity)
	{
		int num = 0;
		this.activeTrackPieces.RemoveAll((TrackPiece piece) => piece.trackPieceType != TrackPieceType.Normal);
		List<TrackPieceType> list = new List<TrackPieceType>(this.trackPieces.Keys);
		int i = 0;
		int count = this.trackPieces.Count;
		while (i < count)
		{
			TrackPieceType trackPieceType = list[i];
			if (trackPieceType != TrackPieceType.Tutorial && trackPieceType != TrackPieceType.Jetpack && trackPieceType != TrackPieceType.Jetpacklanding)
			{
				if (!forceChangeEnv || trackPieceType == TrackPieceType.SubsceneTransition)
				{
					if (!forceChangeCity || trackPieceType == TrackPieceType.CityTransition)
					{
						if (changeCityEnable || trackPieceType != TrackPieceType.CityTransition)
						{
							if (changeEnvEnable || trackPieceType != TrackPieceType.SubsceneTransition)
							{
								if (trackPieceType == TrackPieceType.Normal)
								{
									int j = this.lastAddedIndex + 1;
									int count2 = this.trackPieces[TrackPieceType.Normal].Count;
									while (j < count2)
									{
										TrackPiece trackPiece = this.trackPieces[TrackPieceType.Normal][j];
										if (trackPiece.zMinimum > z)
										{
											break;
										}
										this.activeTrackPieces.Add(trackPiece);
										num++;
										this.lastAddedIndex = j;
										j++;
									}
								}
								else
								{
									this.activeTrackPieces.AddRange(this.trackPieces[trackPieceType]);
								}
							}
						}
					}
				}
			}
			i++;
		}
		this.activeTrackPieces.RemoveAll((TrackPiece piece) => piece.trackPieceType == TrackPieceType.Normal && piece.zMaximum < z);
		this.Recalculate(forceChangeEnv || forceChangeCity);
	}

	private void Recalculate(bool onlySpecial)
	{
		this.randomSpace.Clear();
		for (int i = 0; i < this.activeTrackPieces.Count; i++)
		{
			TrackPiece trackPiece = this.activeTrackPieces[i];
			if (!onlySpecial || trackPiece.trackPieceType != TrackPieceType.Normal)
			{
				for (int j = 0; j < trackPiece.probability; j++)
				{
					this.randomSpace.Add(i);
				}
			}
		}
	}

	public Dictionary<TrackPieceType, List<TrackPiece>> TrackPieces
	{
		get
		{
			return this.trackPieces;
		}
	}

	private List<TrackPiece> activeTrackPieces = new List<TrackPiece>();

	private int lastAddedIndex = -1;

	private List<int> randomSpace = new List<int>();

	private Dictionary<TrackPieceType, List<TrackPiece>> trackPieces;
}
