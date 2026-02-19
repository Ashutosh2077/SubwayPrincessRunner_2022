using System;
using UnityEngine;

public class UIWidgetResizer : MonoBehaviour
{
	public void AdjustAnchorBackgroundToResolution()
	{
		UIRoot uiroot = null;
		if (UIScreenController.Instance != null)
		{
			uiroot = UIScreenController.Instance.root;
		}
		if (uiroot == null)
		{
			uiroot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		}
		Vector2 vector = new Vector2((float)this._anchorBackground.width, (float)this._anchorBackground.height);
		vector.y = (float)(uiroot.manualHeight - 84);
		this._anchorBackground.width = Mathf.RoundToInt(vector.x);
		this._anchorBackground.height = Mathf.RoundToInt(vector.y);
	}

	private void OnEnable()
	{
		this.AdjustAnchorBackgroundToResolution();
	}

	[SerializeField]
	private UIWidget _anchorBackground;

	private const int _pixelsOutsideTheBackground = 84;
}
