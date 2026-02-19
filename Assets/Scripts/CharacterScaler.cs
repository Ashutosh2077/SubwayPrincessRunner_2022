using System;
using UnityEngine;

public class CharacterScaler : MonoBehaviour
{
	private void PositionCharacter()
	{
		float posX = this._posX;
		float posY = this._posY;
		Transform transform = base.transform;
		Vector3 localPosition = transform.localPosition;
		localPosition.x = posX;
		localPosition.y = posY;
		transform.localPosition = localPosition;
	}

	private void RotateCharacter()
	{
		if (this._camera != null)
		{
			Vector3 zero = Vector3.zero;
			float num = (float)this._root.manualHeight - 720f;
			float num2 = (this._scaleMultiplierForRotation != 0f) ? (num / this._scaleMultiplierForRotation + 5f) : 0f;
			zero.x = -num2;
			zero.y = 0f;
			zero.z = 0f;
			base.gameObject.transform.localRotation = Quaternion.Euler(zero);
		}
		else
		{
			UnityEngine.Debug.LogWarning("Camera not set in the CharacterModel prefab");
		}
	}

	private void ScaleCharacter()
	{
		float num = (float)this._root.manualHeight;
		Transform transform = base.transform;
		Vector3 localPosition = transform.localPosition;
		localPosition.z = ((this._scaleDelta != 0f) ? (num - this._scaleDelta) : 0f);
		transform.transform.localPosition = localPosition;
	}

	private void SetScreenRelatedSettings()
	{
		if (this._anchorType == CharacterScaler.ScaleAnchorType.CharacterAnchor)
		{
			this._posX = 0f;
			this._posY = 30f;
			this._scaleMultiplierForRotation = 56f;
			this._scaleDelta = 900f;
		}
		else if (this._anchorType == CharacterScaler.ScaleAnchorType.GameOverAnchor)
		{
			this._posX = -70f;
			this._posY = (float)(this._root.manualHeight / 2) - 230f;
			this._scaleMultiplierForRotation = 32f;
			this._scaleDelta = 720f;
		}
		else if (this._anchorType == CharacterScaler.ScaleAnchorType.TutorialPupupAnchor)
		{
			this._posX = 0f;
			this._posY = -50f;
			this._scaleMultiplierForRotation = 0f;
			this._scaleDelta = 600f;
		}
		else if (this._anchorType == CharacterScaler.ScaleAnchorType.CelebrationPopupAnchor)
		{
			this._posX = 0f;
			this._posY = 0f;
			this._scaleMultiplierForRotation = 0f;
			this._scaleDelta = (float)this._root.manualHeight;
		}
	}

	private void Start()
	{
		if (this._camera == null)
		{
			this._camera = NGUITools.FindInParents<Camera>(base.gameObject);
		}
		this._root = UIScreenController.Instance.root;
		this.SetScreenRelatedSettings();
		if (this._root == null)
		{
			UnityEngine.Debug.LogWarning("Root not set in the UIScreenController prefab");
		}
		this.ScaleCharacter();
		this.PositionCharacter();
		this.RotateCharacter();
	}

	private void Update()
	{
		if (this.lookAtCamera)
		{
			this.RotateCharacter();
			this.lookAtCamera = false;
		}
	}

	[SerializeField]
	public bool lookAtCamera;

	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private CharacterScaler.ScaleAnchorType _anchorType;

	private float _posX = 90f;

	private float _posY = 225f;

	private UIRoot _root;

	private float _scaleDelta = 450f;

	private float _scaleMultiplierForRotation = 56f;

	public enum ScaleAnchorType
	{
		CharacterAnchor,
		GameOverAnchor,
		TutorialPupupAnchor,
		CelebrationPopupAnchor
	}
}
