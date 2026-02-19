using System;
using System.Collections;
using UnityEngine;

public class RankLoadingPopup : UIBaseScreen
{
	public override void Init()
	{
		base.Init();
		UIEventListener uieventListener = UIEventListener.Get(this.bgBtn);
		uieventListener.onPress = new UIEventListener.BoolDelegate(this.OnPress);
		this.ta = this.tipLbl.GetComponent<TweenAlpha>();
		this.ta.enabled = false;
	}

	public void OnPress(GameObject go, bool isPressed)
	{
		if (isPressed)
		{
			UIScreenController.Instance.ClosePopup(null);
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKeyDown(KeyCode.Home))
		{
			this.OnPress(base.gameObject, true);
		}
	}

	public override void Show()
	{
		base.Show();
		this.RefreshLable();
		base.StartCoroutine(this.PlayTween());
	}

	private void RefreshLable()
	{
		this.tipLbl.text = Strings.Get(LanguageKey.UI_POPUP_RANK_LOADING_CONTENT);
	}

	private IEnumerator PlayTween()
	{
		this.tipLbl.enabled = true;
		this.tipLbl.alpha = 0f;
		this.ta.ResetToBeginning2();
		this.ta.PlayForward();
		yield return new WaitForSeconds(this.ta.delay + 4f);
		this.ta.Stop();
		this.tipLbl.enabled = false;
		yield break;
	}

	public override void Hide()
	{
		base.Hide();
		this.ta.enabled = false;
		this.tipLbl.enabled = false;
	}

	[SerializeField]
	private GameObject bgBtn;

	[SerializeField]
	private UILabel tipLbl;

	private TweenAlpha ta;
}
