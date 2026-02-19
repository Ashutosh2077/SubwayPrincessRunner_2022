using System;
using System.Collections.Generic;
using UnityEngine;

public class HelmetModelPreviewFactory : MonoBehaviour
{
	private void AddCustomModel(GameObject prefab, Transform root)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
		gameObject.transform.parent = root;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.layer = Layers.Instance._3DGUI;
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		int i = 0;
		int num = componentsInChildren.Length;
		while (i < num)
		{
			componentsInChildren[i].gameObject.layer = Layers.Instance._3DGUI;
			i++;
		}
	}

	public void Awake()
	{
		int i = 0;
		int num = this.helmets.Length;
		while (i < num)
		{
			this.name2setup.Add(this.helmets[i].name, this.helmets[i]);
			i++;
		}
		int j = 0;
		int num2 = this.HelmetSelector.Length;
		while (j < num2)
		{
			if (this.helmetThatMatchHelmType.ContainsKey(this.HelmetSelector[j].helmType))
			{
				throw new Exception("There are more helmets assigned to the helmet selection");
			}
			this.helmetThatMatchHelmType.Add(this.HelmetSelector[j].helmType, this.HelmetSelector[j].helmetPrefab);
			j++;
		}
		this.helmetManager = HelmetManager.Instance;
	}

	public HelmetModelPreviewFactory.HelmetSelection GetHelmetSelection(Helmets.HelmType helm)
	{
		int i = 0;
		int num = this.HelmetSelector.Length;
		while (i < num)
		{
			if (this.HelmetSelector[i].helmType == helm)
			{
				return this.HelmetSelector[i];
			}
			i++;
		}
		return null;
	}

	public HelmetModelPreviewFactory.HelmetSelection GetHelmetSelection(int index)
	{
		if (index < 0 || index >= this.HelmetSelector.Length)
		{
			return null;
		}
		return this.HelmetSelector[index];
	}

	public GameObject GetHelmet()
	{
		GameObject result;
		if (this.helmetThatMatchHelmType.TryGetValue(this.GetCurrentEquippedHelmet(), out result))
		{
			return result;
		}
		return null;
	}

	public Helmets.HelmType GetCurrentEquippedHelmet()
	{
		return this.helmetManager.CurrentlyEquippedHelmet();
	}

	public void ChangeHelmet(Helmets.HelmType helmType, GameObject helmetGO, Animation characterAnimation, bool updateAnimation)
	{
		string helmModelName = Helmets.helmData[helmType].helmModelName;
		this.characterModel = characterAnimation.transform.parent.GetComponent<CharacterModel>();
		if (this.intialScale == new Vector3(-1f, -1f, -1f))
		{
			this.intialScale = this.characterModel.transform.localScale;
		}
		HelmetModelPreviewFactory.HelmetModelMenuData helmetModelMenuData;
		if (this.name2setup.TryGetValue(helmModelName, out helmetModelMenuData))
		{
			string helmName;
			if (helmetGO != null)
			{
				helmName = helmetGO.name;
			}
			else
			{
				helmName = "helmet";
			}
			helmetGO = this.characterModel.SetNewHelmet(helmetGO, helmetModelMenuData.helmetPrefab, helmName);
			if (updateAnimation)
			{
				if (characterAnimation[helmetModelMenuData.hangtimeClip.name] == null)
				{
					characterAnimation.AddClip(helmetModelMenuData.hangtimeClip, helmetModelMenuData.hangtimeClip.name);
					characterAnimation[helmetModelMenuData.hangtimeClip.name].wrapMode = WrapMode.Once;
					if (characterAnimation[helmetModelMenuData.runClip.name] == null)
					{
						characterAnimation.AddClip(helmetModelMenuData.runClip, helmetModelMenuData.runClip.name);
					}
				}
				characterAnimation.Play(helmetModelMenuData.hangtimeClip.name);
				characterAnimation.CrossFadeQueued(helmetModelMenuData.runClip.name, 0.2f);
			}
		}
		else
		{
			UnityEngine.Debug.LogError("could not find sample character model for '" + helmModelName + "'.");
		}
	}

	public Quaternion GetHelmetDefaultRotation(string name)
	{
		HelmetModelPreviewFactory.HelmetModelMenuData helmetModelMenuData;
		if (this.name2setup.TryGetValue(name, out helmetModelMenuData))
		{
			return Quaternion.Euler(helmetModelMenuData.eulerAngles) * Quaternion.Euler(0f, 180f, 0f);
		}
		return Quaternion.Euler(194f, 110.5f, 180f);
	}

	public GameObject GetHelmetModelForScroll(string name, Helmets.HelmType helmType)
	{
		HelmetModelPreviewFactory.HelmetModelMenuData helmetModelMenuData;
		if (this.name2setup.TryGetValue(name, out helmetModelMenuData))
		{
			GameObject gameObject = new GameObject("Helmet: " + name);
			GameObject gameObject2;
			if (helmetModelMenuData.helmetPrefabInScroll != null)
			{
				gameObject2 = UnityEngine.Object.Instantiate<GameObject>(helmetModelMenuData.helmetPrefabInScroll);
			}
			else
			{
				gameObject2 = UnityEngine.Object.Instantiate<GameObject>(helmetModelMenuData.helmetPrefab);
			}
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localRotation = Quaternion.Euler(helmetModelMenuData.eulerAngles) * Quaternion.Euler(0f, 180f, 0f);
			Vector3 localPosition = gameObject2.transform.localPosition;
			gameObject2.transform.localPosition = localPosition + new Vector3(0f, helmetModelMenuData.menuYPos, 0f);
			return gameObject;
		}
		return null;
	}

	public static HelmetModelPreviewFactory Instance
	{
		get
		{
			if (HelmetModelPreviewFactory.instance == null)
			{
				HelmetModelPreviewFactory.instance = (UnityEngine.Object.FindObjectOfType(typeof(HelmetModelPreviewFactory)) as HelmetModelPreviewFactory);
			}
			return HelmetModelPreviewFactory.instance;
		}
	}

	[SerializeField]
	private HelmetModelPreviewFactory.HelmetSelection[] HelmetSelector;

	[SerializeField]
	private HelmetModelPreviewFactory.HelmetModelMenuData[] helmets;

	public float pullSpeed = 200f;

	public float cooldownDistance = 50f;

	public float slowMotionDistance = 90f;

	public float slowDownToScale = 0.3f;

	public float WaitForParticlesDelay;

	public float RemoveObstaclesDistance = 250f;

	private CharacterModel characterModel;

	private HelmetManager helmetManager;

	private static HelmetModelPreviewFactory instance;

	private Vector3 intialScale = new Vector3(-1f, -1f, -1f);

	private Dictionary<string, HelmetModelPreviewFactory.HelmetModelMenuData> name2setup = new Dictionary<string, HelmetModelPreviewFactory.HelmetModelMenuData>();

	private Dictionary<Helmets.HelmType, GameObject> helmetThatMatchHelmType = new Dictionary<Helmets.HelmType, GameObject>();

	[Serializable]
	public class HelmetSelection
	{
		public Helmets.HelmType helmType;

		public GameObject helmetPrefab;
	}

	[Serializable]
	public class HelmetModelMenuData
	{
		public string name;

		public Vector3 eulerAngles;

		public float menuYPos;

		public AnimationClip hangtimeClip;

		public AnimationClip runClip;

		public GameObject helmetPrefab;

		public GameObject helmetPrefabInScroll;
	}
}
