using System;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
	public void Awake()
	{
		this.trackSpacing = (this.trackRight.position - this.trackLeft.position).magnitude / (float)(this.numberOfTracks - 1);
		this.city = Resources.Load<City>("City/" + this.cityName);
		if (this.city == null)
		{
			UnityEngine.Debug.LogError(this.cityName + " is not exit in Resources/City fold.");
			UnityEngine.Debug.Break();
		}
		TrackController.Instance.AddTrack(this.cityName, this);
		int i = 0;
		int num = this.city.subScenes.Length;
		while (i < num)
		{
			SubScene subScene = this.city.subScenes[i];
			if (subScene.minLength == -1)
			{
				subScene.minLength = int.MaxValue;
			}
			if (subScene.maxLength == -1)
			{
				subScene.maxLength = int.MaxValue;
			}
			this.subScenes.Add(subScene.order, subScene);
			i++;
		}
		this.Restart();
	}

	public void AddToChunks(TrackPiece newPiece)
	{
		if (!this.subScenes.ContainsKey(newPiece.subscene))
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				newPiece.subscene,
				" is not exit in ",
				this.cityName,
				"'s subScenes."
			}));
			UnityEngine.Debug.Break();
		}
		SubScene subScene = this.subScenes[newPiece.subscene];
		subScene.AddToDict(newPiece);
	}

	public void Restart()
	{
		foreach (KeyValuePair<int, SubScene> keyValuePair in this.subScenes)
		{
			keyValuePair.Value.Restart();
		}
		this.currentSubSceneId = 0;
	}

	public void SetTrackPosition()
	{
		base.transform.position = new Vector3(500f * (float)this.city.order, 0f, 0f);
	}

	public int GetSubsceneId(bool force)
	{
		if (force)
		{
			int num = (this.currentSubSceneId + 1) % this.subScenes.Count;
			if (this.subScenes.ContainsKey(num))
			{
				return num;
			}
		}
		return this.currentSubSceneId;
	}

	public bool AllowFlypack()
	{
		return this.CurrentSubScene.allowFlypack;
	}

	public void ChangeBackgroundMusic()
	{
		AudioPlayer.Instance.PlayMusic(this.CurrentSubScene.mainMusic, 0.5f, 0.5f, 0.5f);
	}

	public int ChangeToNextSubScene(int subOrder)
	{
		if (this.subScenes.ContainsKey(subOrder))
		{
			this.currentSubSceneId = (subOrder + 1) % this.subScenes.Count;
		}
		return this.currentSubSceneId;
	}

	public SubScene GetSubSceneByName(int subOrder)
	{
		if (!this.subScenes.ContainsKey(subOrder))
		{
			UnityEngine.Debug.LogError(this.cityName + " does not contain " + subOrder);
			UnityEngine.Debug.Break();
			return this.subScenes[this.currentSubSceneId];
		}
		return this.subScenes[subOrder];
	}

	public SubScene CurrentSubScene
	{
		get
		{
			return this.GetSubSceneByName(this.currentSubSceneId);
		}
	}

	public Vector3 GetPosition(float x, float z)
	{
		return Vector3.forward * z + this.trackLeft.position + x * Vector3.right;
	}

	public float GetTrackX(int trackIndex)
	{
		return (this.trackLeft.position + this.trackRight.position).x * 0.5f + this.trackSpacing * (float)trackIndex;
	}

	public SubScene DefaultSubscene
	{
		get
		{
			return this.GetSubSceneByName(0);
		}
	}

	public City City
	{
		get
		{
			return this.city;
		}
	}

	public string cityName;

	public Transform trackLeft;

	public Transform trackRight;

	public int numberOfTracks = 3;

	public float cleanUpDistance = 2000f;

	public float aheadDistance = 700f;

	private const float deltaX = 500f;

	private City city;

	private float trackSpacing;

	public Dictionary<int, SubScene> subScenes = new Dictionary<int, SubScene>();

	private int currentSubSceneId;
}
