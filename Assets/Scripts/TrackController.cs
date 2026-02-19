using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackController : MonoBehaviour
{
	public static TrackController Instance
	{
		get
		{
			if (TrackController.instance == null)
			{
				TrackController.instance = UnityEngine.Object.FindObjectOfType<TrackController>();
			}
			return TrackController.instance;
		}
	}

	private void Awake()
	{
		this.trackPieces = new TrackPieceController();
		this.flypack = Flypack.Instance;
		this.trackDict = new Dictionary<string, Track>();
		this.lastPortalZ = (this.lastCityZ = (this.lastSubsceneZ = 0f));
		this.lastFlypackSpawnZ = 0f;
		this.willChangeMainMusic = false;
	}

	public void AddTrack(string city, Track track)
	{
		if (!this.trackDict.ContainsKey(city))
		{
			this.trackDict.Add(city, track);
		}
	}

	public void RemoveTrack(string city)
	{
		if (this.trackDict.ContainsKey(city))
		{
			this.trackDict.Remove(city);
		}
	}

	public void SetStartCity()
	{
		Track x = null;
		if (GlobalInit.Instance.debug)
		{
			x = this.trackDict[GlobalInit.Instance.cityScenename];
		}
		else
		{
			foreach (KeyValuePair<string, Track> keyValuePair in this.trackDict)
			{
				if (keyValuePair.Value.City.order == 0)
				{
					x = keyValuePair.Value;
					break;
				}
			}
		}
		if (x != null)
		{
			this.currentTrack = x;
			Shader.SetGlobalVector("_Distort", this.currentTrack.City.distort);
			this.trackPieces.SetTrackPieces(this.currentTrack.DefaultSubscene);
			this.willChangeMainMusic = false;
			this.currentTrack.ChangeBackgroundMusic();
		}
		else
		{
			UnityEngine.Debug.LogError("The Cities's orders do not have 0!!!");
			UnityEngine.Debug.Break();
		}
	}

	public void ChangeToNextCity(bool isContinue)
	{
		int num = (this.currentTrack.City.order + 1) % this.trackDict.Count;
		Track track = null;
		foreach (KeyValuePair<string, Track> keyValuePair in this.trackDict)
		{
			if (keyValuePair.Value.City.order == num)
			{
				track = keyValuePair.Value;
				break;
			}
		}
		if (track != null)
		{
			this.ChangeCity(track, isContinue);
		}
		else
		{
			UnityEngine.Debug.LogError(num + " has not exit!!!");
			UnityEngine.Debug.Break();
		}
	}

	public void ChangeCity(Track track, bool isContinue)
	{
		RandomizerHold.Initialize();
		if (this.currentTrack != null)
		{
			this.currentTrack.Restart();
		}
		this.currentTrack = track;
		Shader.SetGlobalVector("_Distort", this.currentTrack.City.distort);
		if (!isContinue)
		{
			this.trackPieceZ = 180f;
			this.lastPortalZ = 180f;
			this.lastCityZ = 180f;
			this.lastSubsceneZ = 180f;
			this.currentTrack.SetTrackPosition();
		}
		else
		{
			this.lastPortalZ = (this.lastSubsceneZ = (this.lastCityZ = this.trackPieceZ));
			this.trackPieceZ += 180f;
		}
		base.StopAllCoroutines();
		int i = 0;
		int count = this.activeTrackPieces.Count;
		while (i < count)
		{
			this.activeTrackPieces[i].Deactivate();
			i++;
		}
		this.activeTrackPieces.Clear();
		this.trackPieces.SetTrackPieces(this.currentTrack.DefaultSubscene);
		this.trackPieces.Initialize(0f);
	}

	private void ChangeSubscene(int subId)
	{
		SubScene subSceneByName = this.currentTrack.GetSubSceneByName(subId);
		this.lastSubsceneZ = this.trackPieceZ;
		this.trackPieces.SetTrackPieces(subSceneByName);
		this.trackPieces.Initialize(0f);
		this.willChangeMainMusic = true;
	}

	public SubScene GetCurrentSubScene()
	{
		if (this.currentTrack != null)
		{
			return this.currentTrack.CurrentSubScene;
		}
		return null;
	}

	public City GetCurrentCity()
	{
		if (this.currentTrack != null)
		{
			return this.currentTrack.City;
		}
		return null;
	}

	public void ChangeBackgroundMusic(float characterZ)
	{
		if (this.willChangeMainMusic && characterZ >= this.lastSubsceneZ - 360f)
		{
			this.willChangeMainMusic = false;
			this.currentTrack.ChangeBackgroundMusic();
		}
	}

	public float LayJetpackPieces(float characterZ, float flyLength)
	{
		this.LayTracksUpTo(characterZ, flyLength, true, false);
		float result = this.trackPieceZ - characterZ;
		this.LayTrackPiece(this.trackPieces.GetPieceBySpecialTrackPieceType(TrackPieceType.Jetpacklanding));
		return result;
	}

	public void LayTransitionPieces(float characterZ)
	{
		this.LayTracksUpTo(characterZ, this.currentTrack.aheadDistance, false, true);
	}

	public void LayTransitionEndPeice()
	{
		this.LayTrackPiece(this.trackPieces.GetPieceBySpecialTrackPieceType(TrackPieceType.TransitionEnd));
	}

	public void LayTrackPieces(float characterZ)
	{
		this.LayTracksUpTo(characterZ, this.currentTrack.aheadDistance, false, false);
	}

	public void LayTracksUpTo(float characterZ, float trackAheadDistance, bool isJetpack, bool isTransition)
	{
		if (this.trackPieces.CanDeliver())
		{
			float num = characterZ + trackAheadDistance;
			if (this.trackPieceZ < num)
			{
				this.CleanupTrackPieces(characterZ);
			}
			int num2 = 0;
			while (this.trackPieceZ < num)
			{
				TrackPiece trackPiece;
				if (this.firstTrackPiece && !PlayerInfo.Instance.tutorialCompleted)
				{
					trackPiece = this.trackPieces.GetPieceBySpecialTrackPieceType(TrackPieceType.Tutorial);
					this.firstTrackPiece = false;
					if (trackPiece.CheckPoints.Count > 0)
					{
						this.IsRunningOnTutorialTrack = true;
					}
				}
				else if (isJetpack)
				{
					trackPiece = this.trackPieces.GetJetPakPiece(num2);
					num2++;
				}
				else if (isTransition)
				{
					trackPiece = this.trackPieces.GetTransitionPiece(num2);
					num2++;
				}
				else
				{
					bool changeEnvEnable = false;
					bool changeCityEnable = false;
					bool forceChangeEnv = false;
					bool forceChangeCity = false;
					SubScene currentSubScene = this.GetCurrentSubScene();
					if (this.trackPieceZ - this.lastSubsceneZ > (float)currentSubScene.minLength)
					{
						changeEnvEnable = true;
					}
					if (this.trackPieceZ - this.lastSubsceneZ > (float)currentSubScene.maxLength)
					{
						forceChangeEnv = true;
						changeEnvEnable = true;
					}
					if (this.trackPieceZ - this.lastFlypackSpawnZ < this.currentTrack.aheadDistance * 1.1f)
					{
						forceChangeEnv = false;
						changeEnvEnable = false;
					}
					if (this.currentTrack.CurrentSubScene.allowCityEnd && this.trackPieceZ - this.lastCityZ > (float)this.currentTrack.City.minLength)
					{
						if (this.trackPieceZ - this.lastPortalZ > (float)this.currentTrack.City.minIntervalLength)
						{
							changeCityEnable = true;
						}
						if (this.trackPieceZ - this.lastPortalZ > (float)this.currentTrack.City.maxIntervalLength)
						{
							changeCityEnable = true;
							forceChangeCity = true;
							forceChangeEnv = false;
							changeEnvEnable = false;
						}
					}
					this.trackPieces.MoveForward(currentSubScene.LastZ, changeEnvEnable, forceChangeEnv, changeCityEnable, forceChangeCity);
					trackPiece = this.trackPieces.GetRandomActive();
					int num3 = 0;
					while (this.activeTrackPieces.Contains(trackPiece) && num3 < 500)
					{
						trackPiece = this.trackPieces.GetRandomActive();
						num3++;
					}
				}
				this.LayTrackPiece(trackPiece);
			}
		}
	}

	private void LayTrackPiece(TrackPiece TrackPiece)
	{
		base.StartCoroutine(this.LayTrackPieceAsync(TrackPiece));
	}

	private IEnumerator LayTrackPieceAsync(TrackPiece trackPiece)
	{
		trackPiece.gameObject.transform.position = Vector3.forward * this.trackPieceZ;
		this.trackPieceZ += trackPiece.zSize;
		SubScene sub = this.GetCurrentSubScene();
		sub.LastZ += trackPiece.zSize;
		this.activeTrackPieces.Add(trackPiece);
		if (trackPiece.trackPieceType == TrackPieceType.SubsceneTransition)
		{
			this.ChangeSubscene(this.currentTrack.ChangeToNextSubScene(trackPiece.subscene));
		}
		if (trackPiece.trackPieceType == TrackPieceType.CityTransition)
		{
			this.lastPortalZ = this.trackPieceZ;
		}
		if (trackPiece.trackPieceType == TrackPieceType.Jetpacklanding)
		{
			this.flypack.PlacePickups(trackPiece.transform.position.z);
		}
		trackPiece.RestoreHiddenObstacles();
		yield return base.StartCoroutine(this.PerformRecursiveRandomizer(trackPiece.gameObject, true));
		if (!trackPiece.hasSorted)
		{
			Array.Sort<TrackObject>(trackPiece.objects, (TrackObject to1, TrackObject to2) => to1.transform.position.z.CompareTo(to2.transform.position.z));
			trackPiece.hasSorted = true;
		}
		foreach (TrackObject o in trackPiece.objects)
		{
			if (o.gameObject.activeInHierarchy)
			{
				o.Activate();
				yield return null;
			}
		}
		yield break;
	}

	private IEnumerator PerformRecursiveRandomizer(GameObject parent, bool sortSpawnUpgrades = true)
	{
		List<TrackController.GameObjectWrapper> objectsToActivate = new List<TrackController.GameObjectWrapper>();
		List<TrackController.SelectorWrapper> spawnPoints = new List<TrackController.SelectorWrapper>();
		List<GameObject> objectsToVisit = new List<GameObject>();
		objectsToVisit.Add(parent);
		while (objectsToVisit.Count > 0)
		{
			GameObject gameObject = objectsToVisit[0];
			objectsToVisit.RemoveAt(0);
			if (sortSpawnUpgrades)
			{
				SpawnUpgrade component = gameObject.GetComponent<SpawnUpgrade>();
				if (component != null)
				{
					spawnPoints.Add(new TrackController.SelectorWrapper(component, gameObject));
					continue;
				}
			}
			SelectorOffset component2 = gameObject.GetComponent<SelectorOffset>();
			if (component2 != null)
			{
				component2.ChooseRandomOffset();
			}
			objectsToActivate.Add(new TrackController.GameObjectWrapper(gameObject));
			Randomizer component3 = gameObject.GetComponent<Randomizer>();
			if (component3 != null)
			{
				component3.PerformRandomizer(objectsToVisit);
			}
			else
			{
				int j = 0;
				Transform transform = gameObject.transform;
				while (j < transform.childCount)
				{
					GameObject gameObject2 = transform.GetChild(j).gameObject;
					objectsToVisit.Add(gameObject2);
					j++;
				}
			}
		}
		List<TrackController.GameObjectWrapper> lowPriority = (from gow in objectsToActivate
		where this.IsLowPriority(gow.GameObject)
		select gow).ToList<TrackController.GameObjectWrapper>();
		objectsToActivate = (from gow in objectsToActivate
		where !this.IsLowPriority(gow.GameObject)
		select gow).ToList<TrackController.GameObjectWrapper>();
		objectsToActivate.Sort((TrackController.GameObjectWrapper x, TrackController.GameObjectWrapper y) => x.Z.CompareTo(y.Z));
		lowPriority.Sort((TrackController.GameObjectWrapper x, TrackController.GameObjectWrapper y) => x.Z.CompareTo(y.Z));
		objectsToActivate.AddRange(lowPriority);
		int i = 0;
		foreach (TrackController.GameObjectWrapper gow2 in objectsToActivate)
		{
			if (gow2.GameObject != null)
			{
				gow2.GameObject.SetActive(true);
			}
			i++;
			if (i == 4)
			{
				yield return null;
				i = 0;
			}
		}
		if (spawnPoints.Count > 0)
		{
			spawnPoints.Sort((TrackController.SelectorWrapper x, TrackController.SelectorWrapper y) => x.Z.CompareTo(y.Z));
			objectsToVisit.Clear();
			foreach (TrackController.SelectorWrapper selectorWrapper in spawnPoints)
			{
				selectorWrapper.Selector.PerformRandomizer(objectsToVisit);
			}
			foreach (GameObject gameObject3 in objectsToVisit)
			{
				gameObject3.SetActive(true);
			}
		}
		yield break;
	}

	public Vector3 GetPosition(float x, float z)
	{
		return this.currentTrack.GetPosition(x, z);
	}

	public float GetTrackX(int trackIndex)
	{
		return this.currentTrack.GetTrackX(trackIndex);
	}

	private bool IsLowPriority(GameObject g)
	{
		return g.layer != 16;
	}

	public void CleanupTrackPieces(float characterZ)
	{
		float num = characterZ - this.currentTrack.cleanUpDistance;
		int i = 0;
		int count = this.activeTrackPieces.Count;
		while (i < count)
		{
			if (this.activeTrackPieces[i].transform.position.z + this.activeTrackPieces[i].zSize < num)
			{
				this.trackPiecesForDeactivation.Add(this.activeTrackPieces[i]);
			}
			i++;
		}
		int j = 0;
		int count2 = this.trackPiecesForDeactivation.Count;
		while (j < count2)
		{
			if (this.trackPiecesForDeactivation[j].trackPieceType != TrackPieceType.Tutorial)
			{
				this.trackPiecesForDeactivation[j].Deactivate();
			}
			this.activeTrackPieces.Remove(this.trackPiecesForDeactivation[j]);
			j++;
		}
		this.trackPiecesForDeactivation.Clear();
	}

	public void DeactivateTrackPieces()
	{
		base.StopAllCoroutines();
		int i = 0;
		int count = this.activeTrackPieces.Count;
		while (i < count)
		{
			this.activeTrackPieces[i].Deactivate();
			i++;
		}
	}

	public void LayEmptyPieces(float characterZ, float removeDistance)
	{
		this.RemovePieceObstacles(characterZ + removeDistance);
	}

	public void RemovePieceObstacles(float removeDistance)
	{
		int i = 0;
		int count = this.activeTrackPieces.Count;
		while (i < count)
		{
			this.activeTrackPieces[i].DeactivateObstacles(removeDistance);
			i++;
		}
	}

	public float GetLastCheckPoint(float characterZ)
	{
		int i = 0;
		int count = this.activeTrackPieces.Count;
		while (i < count)
		{
			TrackPiece trackPiece = this.activeTrackPieces[i];
			if (this.IsRunningOnTutorialTrack && trackPiece.trackPieceType == TrackPieceType.Tutorial)
			{
				return trackPiece.GetLastCheckPoint(characterZ);
			}
			i++;
		}
		UnityEngine.Debug.Log("No checkpoints in track");
		return 0f;
	}

	public TrackPiece.TrackCheckPoint GetNextCheckPoint(float characterZ)
	{
		TrackPiece.TrackCheckPoint trackCheckPoint = null;
		int i = 0;
		int count = this.activeTrackPieces.Count;
		while (i < count)
		{
			TrackPiece trackPiece = this.activeTrackPieces[i];
			if (trackPiece.CheckPoints != null && trackPiece.CheckPoints.Count != 0)
			{
				trackCheckPoint = trackPiece.GetNextCheckPoint(characterZ);
				if (trackCheckPoint != null)
				{
					break;
				}
			}
			i++;
		}
		return trackCheckPoint;
	}

	public void SetCharacterPosition(Character character)
	{
		character.z = this.trackPieceZ + 250f;
		character.transform.position = this.GetPosition(character.x, character.z);
	}

	public void Restart()
	{
		RandomizerHold.Initialize();
		if (this.currentTrack != null)
		{
			this.currentTrack.Restart();
		}
		this.trackPieceZ = 0f;
		this.lastPortalZ = 0f;
		this.lastCityZ = 0f;
		this.lastSubsceneZ = 0f;
		this.lastFlypackSpawnZ = 0f;
		this.willChangeMainMusic = false;
		this.SetStartCity();
		this.trackPieces.Initialize(0f);
		int i = 0;
		int count = this.activeTrackPieces.Count;
		while (i < count)
		{
			this.activeTrackPieces[i].Deactivate();
			i++;
		}
		this.activeTrackPieces.Clear();
		this.firstTrackPiece = true;
	}

	public bool AllowFlypack()
	{
		return this.currentTrack.AllowFlypack();
	}

	public float LastFlypackSpawnZ
	{
		set
		{
			this.lastFlypackSpawnZ = value;
		}
	}

	public int NumberOfTracks
	{
		get
		{
			return this.currentTrack.numberOfTracks;
		}
	}

	public bool IsRunningOnTutorialTrack { get; set; }

	public Dictionary<string, Track> trackDict;

	private Track currentTrack;

	private List<TrackPiece> activeTrackPieces = new List<TrackPiece>(5);

	private List<TrackPiece> trackPiecesForDeactivation = new List<TrackPiece>(5);

	private bool firstTrackPiece = true;

	private float lastCityZ;

	private float lastSubsceneZ;

	private float lastPortalZ;

	private float trackPieceZ;

	private float lastFlypackSpawnZ;

	private bool willChangeMainMusic;

	private TrackPieceController trackPieces;

	private Flypack flypack;

	private static TrackController instance;

	private struct GameObjectWrapper
	{
		public GameObjectWrapper(GameObject gameObject)
		{
			this.GameObject = gameObject;
			this.z = gameObject.transform.position.z;
		}

		public float Z
		{
			get
			{
				return this.z;
			}
		}

		public GameObject GameObject;

		public float z;
	}

	private struct SelectorWrapper
	{
		public SelectorWrapper(Randomizer selector, GameObject gameObject)
		{
			this.Selector = selector;
			this.GameObject = gameObject;
			this.z = gameObject.transform.position.z;
		}

		public float Z
		{
			get
			{
				return this.z;
			}
		}

		public GameObject GameObject;

		public float z;

		public Randomizer Selector;
	}
}
