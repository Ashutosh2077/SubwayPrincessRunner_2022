using System;
using UnityEngine;

public class CharacterPopup : UIBaseScreen
{
	public override void Init()
	{
		base.Init();
		this.uimodelController = UIModelController.Instance;
	}

	protected virtual void OnDisable()
	{
		if (UIScreenController.Instance != null && !UIScreenController.Instance.stoppingFromEditor)
		{
			this.uimodelController.ClearTutorialPopup();
		}
	}

	protected void SetCharacter(Characters.CharacterType character, Vector2? offsetFromAnchor, Vector3? rotationOffset, float scale = 19f, bool posing = false, AnimationClip pose = null)
	{
		uimodelController.ClearTutorialPopup();
		if (characterModel == null)
		{
			Vector3 localPosition = (offsetFromAnchor.HasValue ? ((Vector3)offsetFromAnchor.Value) : new Vector3(35f, 23f, 50f));
			localPosition.z = 50f;
			characterModel = NGUITools.AddChild(uimodelController.TutorialPopupAnchor, uimodelController.ModelPrefab).GetComponent<CharacterModel>();
			Utility.SetLayerRecursively(characterModel.transform, uimodelController.TutorialPopupAnchor.layer);
			Transform transform = characterModel.transform;
			transform.localPosition = localPosition;
			transform.localScale = new Vector3(scale, scale, scale);
			transform.localEulerAngles = (posing ? rotationOffset.Value : new Vector3(50f, 200f, 0f));
			characterModel.HideBlobShadow();
		}
		Characters.Model model = Characters.characterData[character];
		string modelName = model.modelName;
		characterModel.ChangeCharacterModel(modelName, 0);
		characterModel.HideAllPowerups();
		if (posing)
		{
			Animation animation = characterModel.GetAnimation();
			animation.AddClip(pose, pose.name);
			animation.Play(pose.name);
		}
		else
		{
			characterModel.StartIdlePopupAnimations();
		}
		CharacterScaler component = uimodelController.TutorialPopupAnchor.GetComponent<CharacterScaler>();
		if (component != null)
		{
			component.lookAtCamera = true;
		}
	}

	private CharacterModel characterModel;

	protected UIModelController uimodelController;
}
