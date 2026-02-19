using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour, ICharacterModel
{
	public void Awake()
	{
		Characters.Model model = Characters.characterData[(Characters.CharacterType)PlayerInfo.Instance.currentCharacter];
		this.characterCustomization = base.gameObject.GetComponent<CharacterCustomization>();
		this.ChangeCharacterOfPlayByPlayerInfo();
		this.helmetRoot = this.meshHelmet.transform;
	}

	public void SetRaft(GameObject raft)
	{
		this.currentRaft = raft;
		raft.transform.parent = this.raftRoot.transform;
		raft.transform.localPosition = Vector3.zero;
		raft.transform.localRotation = Quaternion.identity;
		raft.transform.localScale = Vector3.one;
	}

	public void RemoveRaft()
	{
		this.currentRaft.transform.parent = null;
		this.currentRaft = null;
	}

	public GameObject SetNewHelmet(GameObject root, GameObject newHelm, string helmName)
	{
		if (this.currentHelmet != null)
		{
			UnityEngine.Object.Destroy(this.currentHelmet);
		}
		this.DeleteHelmModels();
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(newHelm);
		gameObject.transform.parent = root.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one * 1.2f;
		gameObject.name = helmName;
		Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>();
		int i = 0;
		int num = componentsInChildren.Length;
		while (i < num)
		{
			componentsInChildren[i].gameObject.layer = Layers.Instance._3DGUI;
			i++;
		}
		this.currentHelmet = gameObject;
		return gameObject;
	}

	public void AddModuleHelmetModel(GameObject modelPrefab, bool isMenu, Transform parent)
	{
		if (modelPrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(modelPrefab);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			this.currentModuleHelmModels.Add(gameObject);
			this.currentModuleHelmModelsRenderes.Add(gameObject.GetComponentInChildren<Renderer>());
			if (isMenu)
			{
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
		}
	}

	public void AddModuleHelmetMenuFX(GameObject modelPrefab, Transform parent)
	{
		if (modelPrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(modelPrefab);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			this.currentModuleHelmModels.Add(gameObject);
		}
	}

	public GameObject AddHelmetModel()
	{
		this.currentHelmet = UnityEngine.Object.Instantiate<GameObject>(HelmetModelPreviewFactory.Instance.GetHelmet());
		this.currentHelmet.transform.parent = this.helmetRoot;
		this.currentHelmet.transform.localPosition = Vector3.zero;
		this.currentHelmet.transform.localRotation = Quaternion.identity;
		this.currentHelmet.transform.localScale = Vector3.one * 1.2f;
		return this.currentHelmet;
	}

	public void DeleteHelmModels()
	{
		if (this.currentModuleHelmModels.Count > 0)
		{
			int i = 0;
			int count = this.currentModuleHelmModels.Count;
			while (i < count)
			{
				UnityEngine.Object.Destroy(this.currentModuleHelmModels[i]);
				i++;
			}
			this.currentModuleHelmModels.Clear();
			this.currentModuleHelmModelsRenderes.Clear();
		}
	}

	public void ChangeCharacterOfPlayByPlayerInfo()
	{
		Characters.Model model = Characters.characterData[(Characters.CharacterType)PlayerInfo.Instance.currentCharacter];
		this.ChangeCharacterModel(model.modelName, PlayerInfo.Instance.currentThemeIndex);
	}

	public void ChangeCharacterModel(string name, int themeIndex)
	{
		SkinnedMeshRenderer x = null;
		this.StopIdleAnimations();
		this.characterCustomization.Customize(name, themeIndex, ref x);
		if (x != null)
		{
			this.currentRender = x;
		}
		if (Character.Instance.superShoes != null && Character.Instance.superShoes.IsActive)
		{
			this.meshSuperShoes.enabled = true;
		}
	}

	public Animation GetAnimation()
	{
		if (this._animationComponent == null)
		{
			this._animationComponent = base.GetComponentInChildren<Animation>();
		}
		return this._animationComponent;
	}

	public GameObject GetHelmetRoot()
	{
		if (this.helmetRoot != null)
		{
			return this.helmetRoot.gameObject;
		}
		return null;
	}

	public void HideAllPowerups()
	{
		this.meshHelmet.enabled = false;
		int i = 0;
		int num = this.meshFlypack.Length;
		while (i < num)
		{
			this.meshFlypack[i].enabled = false;
			i++;
		}
		this.animFlypack.Stop();
		this.meshSuperShoes.enabled = false;
		this.meshCoinMagnet.enabled = false;
	}

	public void HideBlobShadow()
	{
		this.meshBlobShadow.enabled = false;
	}

	public void StartIdleAnimations()
	{
		if (this.currentRender != null)
		{
			AvatarAnimations component = this.currentRender.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StartIdleAnimations();
			}
		}
	}

	public void StartTryAnimation()
	{
		if (this.currentRender != null)
		{
			Animation animation = this.GetAnimation();
			TrialInfo currentTrialInfo = TrialManager.Instance.currentTrialInfo;
			if (currentTrialInfo != null && animation != null)
			{
				string name = currentTrialInfo.idel.name;
				if (animation.GetClip(name) == null)
				{
					animation.AddClip(currentTrialInfo.idel, name);
				}
				animation.Play(name);
				string name2 = currentTrialInfo.alert.name;
				if (animation.GetClip(name2) == null)
				{
					animation.AddClip(currentTrialInfo.alert, name2);
				}
				animation.CrossFadeQueued(name2, 0.1f);
			}
		}
	}

	public void StopTryAnimations()
	{
		if (this.currentRender != null)
		{
			Animation animation = this.GetAnimation();
			if (animation != null)
			{
				animation.Stop();
			}
		}
	}

	public void StartIdlePopupAnimations()
	{
		if (this.currentRender != null)
		{
			AvatarAnimations component = this.currentRender.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StartIdlePopupAnimations();
			}
		}
	}

	public void StopIdleAnimations()
	{
		if (this.currentRender != null)
		{
			AvatarAnimations component = this.currentRender.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StopIdleAnimations();
			}
		}
	}

	public Transform BoneHead
	{
		get
		{
			return this.headJoint;
		}
	}

	public Transform BoneHelmet
	{
		get
		{
			return (!(this.meshHelmet != null)) ? null : this.meshHelmet.transform;
		}
	}

	public Transform BoneRightHand
	{
		get
		{
			return this.rightHand;
		}
	}

	[OptionalField]
	public SkinnedMeshRenderer currentRender;

	[OptionalField]
	public SkinnedMeshRenderer meshSuperShoes;

	[OptionalField]
	public MeshRenderer meshHelmet;

	[OptionalField]
	public MeshRenderer meshCoinMagnet;

	[OptionalField]
	public Animation animFlypack;

	[OptionalField]
	public MeshRenderer[] meshFlypack;

	[OptionalField]
	public MeshRenderer meshBlobShadow;

	[OptionalField]
	public Animation characterAnimation;

	[OptionalField]
	public MeshRenderer shadow;

	[OptionalField]
	public Transform spineTransform;

	[OptionalField]
	public Transform shoulderTransform;

	[OptionalField]
	public Transform flypackCloudPosition;

	[OptionalField]
	public Transform leftFoot;

	[OptionalField]
	public Transform rightFoot;

	[OptionalField]
	public Transform headJoint;

	[OptionalField]
	public Transform rightHand;

	[HideInInspector]
	public GameObject currentRaft;

	[HideInInspector]
	public GameObject currentHelmet;

	public Animation PlayBall;

	public Animation Flower;

	public Animation Heart;

	[HideInInspector]
	public List<GameObject> currentModuleHelmModels = new List<GameObject>();

	[HideInInspector]
	public List<Renderer> currentModuleHelmModelsRenderes = new List<Renderer>();

	private Animation _animationComponent;

	private CharacterCustomization characterCustomization;

	private Transform helmetRoot;

	private Transform raftRoot;
}
