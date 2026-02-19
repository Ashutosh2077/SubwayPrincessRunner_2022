using System;
using UnityEngine;

public class UICameraScreenClipping : MonoBehaviour
{
	public void CalculateClipping(bool popupSizedClip)
	{
		if (UIScreenController.Instance.root == null)
		{
			UnityEngine.Debug.LogError("UIRoot not set in UIScreenController");
		}
		if (this._cam == null)
		{
			this._cam = base.gameObject.GetComponent<Camera>();
		}
		Rect rect = this._cam.rect;
		if (!UIBaseScreen.IsOutOfProportion())
		{
			float num = (float)(Screen.width * 480 / Screen.height);
			float num2 = (popupSizedClip ? 265f : 300f) / num;
			float x = 0.5f - num2 / 2f;
			rect.x = x;
			rect.width = num2;
		}
		else if (popupSizedClip)
		{
			rect.x = 0.085f;
			rect.width = 0.83f;
		}
		else
		{
			rect.x = 0.05f;
			rect.width = 0.9f;
		}
		this._cam.rect = rect;
	}

	private void Start()
	{
		this._cam = base.gameObject.GetComponent<Camera>();
		if (this._cam == null)
		{
			UnityEngine.Debug.LogError("The UICameraScreenClipping script is not attached to a Camera");
		}
		else
		{
			this.CalculateClipping(false);
		}
	}

	private Camera _cam;

	private const int POPUP_WIDTH = 265;

	private const int SCREEN_WIDTH = 300;
}
