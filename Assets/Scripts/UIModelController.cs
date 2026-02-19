using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModelController : MonoBehaviour
{
	private GameObject _ActivateModel(Characters.CharacterType characterName, int modelIndex, UIModelController.ModelScreen screen)
	{
		if (this._cachedActiveModel != null && !this.isShownInCelebrate)
		{
			this.ClearModels();
		}
		else
		{
			this.isShownInCelebrate = false;
		}
		Characters.Model model = Characters.characterData[characterName];
		string modelName = model.modelName;
		this._currentActivatedScreenModel = screen;
		if (screen == UIModelController.ModelScreen.Character)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ModelPrefab);
			gameObject.transform.parent = this.CharacterAnchor.transform;
			gameObject.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.characterScreenCharacterModelLocalPos;
			Utility.SetLayerRecursively(gameObject.transform, this.CharacterAnchor.layer);
			gameObject.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.characterScreenCharacterModelLocalScl;
			gameObject.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.characterScreenCharacterModelLocalRot);
			CharacterModel component = gameObject.GetComponent<CharacterModel>();
			component.ChangeCharacterModel(modelName, modelIndex);
			component.HideAllPowerups();
			component.StartIdleAnimations();
			this._cachedActiveModel = component;
			return gameObject;
		}
		if (screen == UIModelController.ModelScreen.TrialRolePopup)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.ModelPrefab);
			gameObject2.transform.parent = this.TutorialPopupAnchor.transform;
			gameObject2.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.tryCharacterModelLocalPos;
			Utility.SetLayerRecursively(gameObject2.transform, this.TutorialPopupAnchor.layer);
			gameObject2.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.tryCharacterModelLocalScl;
			gameObject2.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.tryCharacterModelLocalRot);
			CharacterModel component2 = gameObject2.GetComponent<CharacterModel>();
			component2.ChangeCharacterModel(modelName, modelIndex);
			component2.HideAllPowerups();
			component2.StartIdleAnimations();
			this._cachedActiveModel = component2;
			return gameObject2;
		}
		if (screen == UIModelController.ModelScreen.TrialHelmetPopup)
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.ModelPrefab);
			gameObject3.transform.parent = this.TutorialPopupAnchor.transform;
			gameObject3.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.tryCharacterModelLocalPos;
			gameObject3.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.tryCharacterModelLocalScl;
			gameObject3.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.tryCharacterModelLocalRot);
			CharacterModel component3 = gameObject3.GetComponent<CharacterModel>();
			component3.ChangeCharacterModel(modelName, modelIndex);
			component3.HideAllPowerups();
			component3.StartTryAnimation();
			GameObject helmetRoot = component3.GetHelmetRoot();
			HelmetModelPreviewFactory.Instance.ChangeHelmet(this._currentTryHelmType, helmetRoot, component3.GetAnimation(), true);
			Utility.SetLayerRecursively(gameObject3.transform, this.TutorialPopupAnchor.layer);
			this._cachedActiveModel = component3;
			return gameObject3;
		}
		if (screen == UIModelController.ModelScreen.GameOver)
		{
			return null;
		}
		if (screen == UIModelController.ModelScreen.Helms)
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(this.ModelPrefab);
			gameObject4.transform.parent = this.CharacterAnchor.transform;
			gameObject4.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.helmetScreenCharacterModelLocalPos;
			Utility.SetLayerRecursively(gameObject4.transform, this.CharacterAnchor.layer);
			gameObject4.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.helmetScreenCharacterModelLocalScl;
			gameObject4.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.helmetScreenCharacterModelLocalRot);
			CharacterModel component4 = gameObject4.GetComponent<CharacterModel>();
			component4.ChangeCharacterModel(modelName, modelIndex);
			component4.HideAllPowerups();
			this._cachedActiveModel = component4;
			return gameObject4;
		}
		if (screen == UIModelController.ModelScreen.CelebrationCharacterUnlock)
		{
			GameObject gameObject5 = UnityEngine.Object.Instantiate<GameObject>(this.ModelPrefab);
			gameObject5.transform.parent = this.TutorialPopupAnchor.transform;
			gameObject5.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.celebrationCharacterUnlockCharacterModelLocalPos;
			Utility.SetLayerRecursively(gameObject5.transform, this.TutorialPopupAnchor.layer);
			gameObject5.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.celebrationCharacterUnlockCharacterModelLocalScl;
			gameObject5.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.celebrationCharacterUnlockCharacterModelLocalRot);
			CharacterModel component5 = gameObject5.GetComponent<CharacterModel>();
			component5.ChangeCharacterModel(modelName, modelIndex);
			component5.HideAllPowerups();
			component5.StartIdleAnimations();
			this._isCelebrationCharacterScreen = true;
			return gameObject5;
		}
		if (screen == UIModelController.ModelScreen.CelebrationHelmUnlock)
		{
			GameObject gameObject6 = UnityEngine.Object.Instantiate<GameObject>(this.ModelPrefab);
			gameObject6.transform.parent = this.TutorialPopupAnchor.transform;
			gameObject6.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.celebrationHelmUnlockCharacterModelLocalPos;
			gameObject6.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.celebrationHelmUnlockCharacterModelLocalScl;
			gameObject6.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.celebrationHelmUnlockCharacterModelLocalRot);
			CharacterModel component6 = gameObject6.GetComponent<CharacterModel>();
			component6.ChangeCharacterModel(modelName, modelIndex);
			component6.HideAllPowerups();
			GameObject helmetRoot2 = component6.GetHelmetRoot();
			HelmetModelPreviewFactory.Instance.ChangeHelmet(this._currentCelebrateHelmType, helmetRoot2, component6.GetAnimation(), true);
			Utility.SetLayerRecursively(gameObject6.transform, this.TutorialPopupAnchor.layer);
			this._isCelebrationCharacterScreen = false;
			base.StartCoroutine("AnimateUnlockBackground", gameObject6);
			return gameObject6;
		}
		if (screen == UIModelController.ModelScreen.CelebrationHighScore)
		{
			GameObject gameObject7 = UnityEngine.Object.Instantiate<GameObject>(this.ModelPrefab);
			gameObject7.transform.parent = this.TutorialPopupAnchor.transform;
			gameObject7.transform.localPosition = UIPosScalesAndNGUIAtlas.Instance.celebrationHighScoreCharacterModelLocalPos;
			gameObject7.transform.localScale = UIPosScalesAndNGUIAtlas.Instance.celebrationHighScoreCharacterModelLocalScl;
			gameObject7.transform.localRotation = Quaternion.Euler(UIPosScalesAndNGUIAtlas.Instance.celebrationHighScoreCharacterModelLocalRot);
			CharacterModel component7 = gameObject7.GetComponent<CharacterModel>();
			component7.ChangeCharacterModel(modelName, modelIndex);
			component7.HideAllPowerups();
			Utility.SetLayerRecursively(gameObject7.transform, this.TutorialPopupAnchor.layer);
			this._isCelebrationCharacterScreen = false;
			base.StartCoroutine("AnimateRunningCharacterForBragCelebration", gameObject7);
			return gameObject7;
		}
		return null;
	}

	public void ActivateCelebrationHelmWithStripes(Helmets.HelmType helmType, UIModelController.ModelScreen screen)
	{
		this.isShownInCelebrate = true;
		this._currentCelebrateHelmType = helmType;
		if (screen == UIModelController.ModelScreen.CelebrationHighScore)
		{
			this._ActivateModel((Characters.CharacterType)PlayerInfo.Instance.currentCharacter, PlayerInfo.Instance.currentThemeIndex, screen);
		}
		else
		{
			this._ActivateModel(this.currentCharacterModelShown, PlayerInfo.Instance.currentThemeIndex, screen);
		}
	}

	public void ActivateGameOverModel()
	{
		this._ActivateModel((Characters.CharacterType)PlayerInfo.Instance.currentCharacter, PlayerInfo.Instance.currentThemeIndex, UIModelController.ModelScreen.GameOver);
	}

	public void ActivateTrailRoleModel(Characters.CharacterType characterType, int themeIndex)
	{
		this._currentCharacterShownModel = characterType;
		this._currentCThemeShownIndex = themeIndex;
		this._ActivateModel(characterType, themeIndex, UIModelController.ModelScreen.TrialRolePopup);
	}

	public void ActivateTrailHelmetModel(Helmets.HelmType helmType)
	{
		this._currentCharacterShownModel = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		this._currentCThemeShownIndex = PlayerInfo.Instance.currentThemeIndex;
		this._currentTryHelmType = helmType;
		this._ActivateModel(this._currentCharacterShownModel, this._currentCThemeShownIndex, UIModelController.ModelScreen.TrialHelmetPopup);
	}

	public void ActivateHelmetModel()
	{
		this._currentCharacterShownModel = (Characters.CharacterType)PlayerInfo.Instance.currentCharacter;
		this._currentCThemeShownIndex = PlayerInfo.Instance.currentThemeIndex;
		this._ActivateModel(this._currentCharacterShownModel, this._currentCThemeShownIndex, UIModelController.ModelScreen.Helms);
	}

	private IEnumerator AnimateRunningCharacterForBragCelebration(GameObject model)
	{
		Animation _charAnim = model.GetComponentInChildren<Animation>();
		for (int i = 0; i < this.highScoreClips.Count; i++)
		{
			if (_charAnim[this.highScoreClips[i].name] == null)
			{
				_charAnim.AddClip(this.highScoreClips[i], this.highScoreClips[i].name);
			}
		}
		AnimationClip select = null;
		this.isAnimationPlaying = true;
		while (this.isAnimationPlaying)
		{
			List<AnimationClip> clips = this.highScoreClips.FindAll((AnimationClip a) => a != select);
			select = clips[UnityEngine.Random.Range(0, clips.Count)];
			float time = 0f;
			float waitTime = select.length;
			_charAnim.Play(select.name);
			while (time < waitTime)
			{
				time += Time.deltaTime;
				yield return null;
			}
		}
		yield break;
	}

	private IEnumerator AnimateUnlockBackground(GameObject go)
	{
		Vector3 charWithHelmPosition = new Vector3(0f, -100f, 0f);
		Vector3 charPosition = new Vector3(-5f, -110f, 0f);
		int index = 0;
		Vector3 currentRotation = Vector3.zero;
		Vector3 currentOffset = Vector3.zero;
		Vector3 currentBackgroundRotationOffset = Vector3.zero;
		Vector3 currentCharPosition = Vector3.zero;
		if (this._currentActivatedScreenModel == UIModelController.ModelScreen.CelebrationHighScore)
		{
			currentBackgroundRotationOffset = new Vector3(0f, 0f, 350f);
			this.UpdateCelebrationRotation(go, new Vector3(5f, 210f, 0f), charWithHelmPosition, new Vector3(0f, 90f, 100f), currentBackgroundRotationOffset);
			yield break;
		}
		for (;;)
		{
			int value = index % 3;
			if (value == 0)
			{
				if (this._isCelebrationCharacterScreen)
				{
					currentRotation = new Vector3(0f, 130f, 0f);
					currentOffset = new Vector3(0f, 0f, 180f);
					currentBackgroundRotationOffset = new Vector3(35f, 20f, 0f);
					currentCharPosition = charPosition;
				}
				else
				{
					currentRotation = new Vector3(27f, 205f, 16f);
					currentOffset = new Vector3(45f, 100f, 130f);
					currentCharPosition = charWithHelmPosition;
				}
			}
			else if (value == 1)
			{
				if (this._isCelebrationCharacterScreen)
				{
					currentRotation = new Vector3(0f, 180f, 0f);
					currentOffset = new Vector3(0f, 0f, 200f);
					currentBackgroundRotationOffset = new Vector3(35f, 0f, 0f);
					currentCharPosition = charPosition;
				}
				else
				{
					currentRotation = new Vector3(350f, 90f, 350f);
					currentOffset = new Vector3(0f, 90f, 170f);
					currentCharPosition = charWithHelmPosition;
				}
			}
			else if (this._isCelebrationCharacterScreen)
			{
				currentRotation = new Vector3(0f, 240f, 0f);
				currentOffset = new Vector3(5f, 0f, 200f);
				currentBackgroundRotationOffset = new Vector3(35f, -30f, 0f);
				currentCharPosition = charPosition;
			}
			else
			{
				currentRotation = new Vector3(10f, 210f, 0f);
				currentOffset = new Vector3(0f, 100f, 120f);
				currentCharPosition = charWithHelmPosition;
			}
			this.UpdateCelebrationRotation(go, currentRotation, currentCharPosition, currentOffset, currentBackgroundRotationOffset);
			index++;
			yield return new WaitForSeconds(2.5f);
		}
	}

	public void ClearModels()
	{
		if (this._cachedActiveModel != null)
		{
			this._cachedActiveModel = null;
		}
		foreach (object obj in this.CharacterAnchor.transform)
		{
			Transform transform = (Transform)obj;
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		foreach (object obj2 in this.GameOverAnchor.transform)
		{
			Transform transform = (Transform)obj2;
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		foreach (object obj3 in this.TutorialPopupAnchor.transform)
		{
			Transform transform = (Transform)obj3;
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		foreach (object obj4 in this.CelebrationPopupAnchor.transform)
		{
			Transform transform = (Transform)obj4;
			UnityEngine.Object.Destroy(transform.gameObject);
		}
	}

	public void ClearTutorialPopup()
	{
		foreach (object obj in this.TutorialPopupAnchor.transform)
		{
			Transform transform = (Transform)obj;
			UnityEngine.Object.Destroy(transform.gameObject);
		}
	}

	public void DeactivateCelebrationPopupModels()
	{
		base.StopCoroutine("AnimateUnlockBackground");
		this.isAnimationPlaying = false;
		base.StopCoroutine("AnimateRunningCharacterForBragCelebration");
		this.isShownInCelebrate = false;
		foreach (object obj in this.TutorialPopupAnchor.transform)
		{
			Transform transform = (Transform)obj;
			UnityEngine.Object.Destroy(transform.gameObject);
		}
	}

	public void ActivateTutorialPopup(bool active)
	{
		NGUITools.SetActive(this.TutorialPopupAnchor, active);
		this._PauseAnimations(!active, this.TutorialPopupAnchor.transform);
	}

	private void _PauseAnimations(bool pause, Transform trans)
	{
		IEnumerator enumerator = trans.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform trans2 = (Transform)obj;
				this._PauseAnimations(pause, trans2);
			}
		}
		finally
		{
		}
		CharacterModel component = trans.GetComponent<CharacterModel>();
		if (component != null)
		{
			if (pause)
			{
				if (this._currentActivatedScreenModel == UIModelController.ModelScreen.TrialHelmetPopup)
				{
					component.StopTryAnimations();
				}
				else
				{
					component.StopIdleAnimations();
				}
			}
			else if (this._currentActivatedScreenModel == UIModelController.ModelScreen.TrialHelmetPopup)
			{
				component.StartTryAnimation();
			}
			else
			{
				component.StartIdleAnimations();
			}
		}
	}

	public void SelectCharacterForPlay(Characters.CharacterType characterType, int themeIndex)
	{
		PlayerInfo.Instance.currentCharacter = (int)characterType;
		PlayerInfo.Instance.currentThemeIndex = themeIndex;
		if (Game.Instance != null)
		{
			Game.Instance.Character.characterModel.ChangeCharacterOfPlayByPlayerInfo();
		}
	}

	public void SelectCurrentHelmShown(Helmets.HelmType currentlyShownHelm)
	{
		PlayerInfo.Instance.currentHelmet = currentlyShownHelm;
	}

	public void SelectThemeForCurrentCharacterModel(int themeIndex)
	{
		this._currentCThemeShownIndex = themeIndex;
		this.SelectCharacterForPlay(this.currentCharacterModelShown, this.currentCThemeShownIndex);
	}

	public GameObject ShowCharacterInCelebration(Characters.CharacterType charType, int themeIndex, UIModelController.ModelScreen screen)
	{
		this.isShownInCelebrate = true;
		return this._ActivateModel(charType, themeIndex, screen);
	}

	public void ShowHelmetMenuModel(Helmets.HelmType currentHelmShown, bool updateAnimation)
	{
		this._currentHelmetShown = currentHelmShown;
		GameObject helmetRoot = this._cachedActiveModel.GetHelmetRoot();
		HelmetModelPreviewFactory.Instance.ChangeHelmet(currentHelmShown, helmetRoot, this._cachedActiveModel.GetAnimation(), updateAnimation);
	}

	public void ShowMenuCharacterModel(Characters.CharacterType charType, int themeIndex)
	{
		if (this._currentCharacterShownModel != charType || this._currentCThemeShownIndex != themeIndex || this._cachedActiveModel == null)
		{
			this._currentCharacterShownModel = charType;
			this._currentCThemeShownIndex = themeIndex;
			this._ChangeCharacterModel(charType, themeIndex);
		}
	}

	private void _ChangeCharacterModel(Characters.CharacterType characterName, int themeIndex)
	{
		if (this._cachedActiveModel != null)
		{
			Characters.Model model = Characters.characterData[characterName];
			string modelName = model.modelName;
			this._cachedActiveModel.ChangeCharacterModel(modelName, themeIndex);
			this._cachedActiveModel.HideAllPowerups();
			this._cachedActiveModel.StartIdleAnimations();
		}
		else
		{
			this._ActivateModel(characterName, themeIndex, UIModelController.ModelScreen.Character);
		}
	}

	private void UpdateCelebrationRotation(GameObject go, Vector3 charRotation, Vector3 charPosition, Vector3 charPositionOffset, Vector3 backgroundRotationOffset)
	{
		if (go != null)
		{
			go.transform.localRotation = Quaternion.Euler(charRotation);
			go.transform.localPosition = charPosition + charPositionOffset;
		}
	}

	public Helmets.HelmType currentHelmetShown
	{
		get
		{
			return this._currentHelmetShown;
		}
	}

	public Characters.CharacterType currentCharacterModelShown
	{
		get
		{
			return this._currentCharacterShownModel;
		}
	}

	public int currentCThemeShownIndex
	{
		get
		{
			return this._currentCThemeShownIndex;
		}
	}

	public static UIModelController Instance
	{
		get
		{
			if (UIModelController._instance == null)
			{
				UIModelController._instance = (UnityEngine.Object.FindObjectOfType(typeof(UIModelController)) as UIModelController);
			}
			return UIModelController._instance;
		}
	}

	public GameObject CharacterAnchor;

	public GameObject GameOverAnchor;

	public GameObject CelebrationPopupAnchor;

	public GameObject TutorialPopupAnchor;

	public GameObject PauseScreenAnchor;

	public GameObject ModelPrefab;

	private CharacterModel _cachedActiveModel;

	private UIModelController.ModelScreen _currentActivatedScreenModel;

	private Helmets.HelmType _currentHelmetShown;

	private Characters.CharacterType _currentCharacterShownModel;

	private int _currentCThemeShownIndex;

	private static UIModelController _instance;

	private bool _isCelebrationCharacterScreen;

	private Helmets.HelmType _currentCelebrateHelmType;

	private Helmets.HelmType _currentTryHelmType;

	private bool isShownInCelebrate;

	public List<AnimationClip> highScoreClips;

	private bool isAnimationPlaying = true;

	public enum ModelScreen
	{
		Character,
		GameOver,
		TrialRolePopup,
		TrialHelmetPopup,
		Popup,
		Helms,
		CelebrationCharacterUnlock,
		CelebrationHelmUnlock,
		CelebrationHighScore,
		DuelResultPopup,
		CelebrationTopRun
	}
}
