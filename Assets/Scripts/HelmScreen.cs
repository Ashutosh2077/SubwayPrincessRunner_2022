using System;
using System.Collections.Generic;
using UnityEngine;

public class HelmScreen : UIBaseScreen, IScrollClick
{
	private T ElementToCenterOn<T>(List<T> elements, Helmets.HelmType helm)
	{
		T result = default(T);
		int num = 0;
		int i = 0;
		int count = this.helmList.Count;
		while (i < count)
		{
			if (helm == this.helmList[i].Key)
			{
				break;
			}
			num++;
			i++;
		}
		if (num < elements.Count)
		{
			result = elements[num];
		}
		return result;
	}

	public override void Hide()
	{
		base.Hide();
		UIModelController.Instance.ClearModels();
	}

	public override void Init()
	{
		base.Init();
		this.selectBtn.InitButton(this);
		this._cellWidth = this.scrollGrid.cellWidth;
		this._centerer = this.scrollGrid.GetComponent<CenterOnChild>();
		this.InitHelms();
		this._hasInited = true;
		base.InitializeCoinbox(true, true, true, 0f, 0f, 0f);
	}

	private void InitHelms()
	{
		this.helmList = new List<KeyValuePair<Helmets.HelmType, Helmets.Helm>>();
		foreach (Helmets.HelmType helmType in Helmets.helmData.Keys)
		{
			Helmets.Helm value = Helmets.helmData[helmType];
			if (HelmetManager.Instance.isHelmetUnlocked(helmType) || HelmetManager.Instance.isHelmActive(helmType))
			{
				this.helmList.Add(new KeyValuePair<Helmets.HelmType, Helmets.Helm>(helmType, value));
			}
		}
		int num = 0;
		int i = 0;
		int count = this.helmList.Count;
		while (i < count)
		{
			HelmScreen.HelmState helmState = new HelmScreen.HelmState();
			helmState.type = this.helmList[i].Key;
			helmState.helmRoot = HelmetModelPreviewFactory.Instance.GetHelmetModelForScroll(this.helmList[i].Value.helmModelName, this.helmList[i].Key);
			if (helmState.helmRoot == null)
			{
				UnityEngine.Debug.LogError("Helm: '" + this.helmList[i].Value.helmModelName + "' not found.");
			}
			else
			{
				helmState.helmRoot.name = string.Format("{0:000}{1}", num, this.helmList[i].Value.name);
				helmState.helmTransform = helmState.helmRoot.transform.GetChild(0);
				helmState.defaultRotation = HelmetModelPreviewFactory.Instance.GetHelmetDefaultRotation(this.helmList[i].Value.helmModelName);
				this.scrollHelms.Add(helmState);
				Transform transform = helmState.helmRoot.transform;
				transform.parent = this.scrollAnchor.transform;
				transform.localPosition = new Vector3((float)num * this._cellWidth, 0f, 50f);
				transform.localScale = Vector3.one * 7f;
				transform.localEulerAngles = new Vector3(53f, 183f, 360f);
				GameObject gameObject = NGUITools.AddChild(this.scrollGrid.gameObject, this.dummyObject);
				this.helmIndices.Add(gameObject.AddComponent<OverlayIndex>());
				this.helmIndices[num].index = num;
				gameObject.name = string.Format("{0:000}{1}", num, this.helmList[i].Key.ToString());
				num++;
			}
			i++;
		}
		Utility.SetLayerRecursively(this.scrollAnchor.transform, 20);
		Helmets.HelmType currentHelmet = PlayerInfo.Instance.currentHelmet;
		OverlayIndex overlayIndex = this.ElementToCenterOn<OverlayIndex>(this.helmIndices, currentHelmet);
		this._centerer.CenterOnTransform(overlayIndex.transform, true);
		this._currentHelmShown = currentHelmet;
		float num2 = Mathf.Abs(this.scrollAnchor.transform.localPosition.x);
		for (int j = 0; j < this.scrollHelms.Count; j++)
		{
			float num3 = Mathf.Abs(num2 - (float)j * this._cellWidth);
			float num4 = 1.5f * this._cellWidth;
			float d = Mathf.SmoothStep(this.maxScale, this.minScale, num3 / num4);
			this.scrollHelms[j].helmRoot.transform.localScale = Vector3.one * d;
		}
	}

	private void RefreshCurrentHelmAndUI()
	{
		UIModelController.Instance.ActivateHelmetModel();
		this.ShowHelmetInMenu(true);
	}

	public void ScrollClicked(Vector2 pos)
	{
		if (this._centerer.CenterOnClosestChildAtPosition(pos) && HelmetManager.Instance.isHelmetUnlocked(this._currentHelmShown))
		{
			this.SelectCurrentHelmShown();
			NGUITools.PlaySound(this.selectSound);
		}
	}

	private void SelectCurrentHelmShown()
	{
		PlayerInfo.Instance.currentHelmet = this._currentHelmShown;
		this.UpdateButtons();
	}

	public override void Show()
	{
		base.Show();
		this.RefreshCurrentHelmAndUI();
	}

	private void Update()
	{
		if (this._hasInited && this.helmList != null)
		{
			if (this._centerer.centeredObject != null)
			{
				int index = this._centerer.centeredObject.GetComponent<OverlayIndex>().index;
				Transform helmTransform = this.scrollHelms[index].helmTransform;
				helmTransform.Rotate(Vector3.up, 30f * Time.deltaTime);
				if (this.scrollHelms[index].type != this._currentHelmShown)
				{
					this._currentHelmShown = this.scrollHelms[index].type;
					this.UpdateDisplayedModelWithTheme(this._currentHelmShown);
				}
				for (int i = 0; i < this.scrollHelms.Count; i++)
				{
					if (i != index)
					{
						HelmScreen.HelmState helmState = this.scrollHelms[i];
						Quaternion b = helmState.helmTransform.parent.rotation * helmState.defaultRotation;
						helmState.helmTransform.rotation = Quaternion.Lerp(helmState.helmTransform.rotation, b, 10f * Time.deltaTime);
					}
				}
			}
			for (int j = 0; j < this.scrollHelms.Count; j++)
			{
				float num = Mathf.Abs(Mathf.Abs(this.scrollAnchor.transform.localPosition.x) - (float)j * this._cellWidth);
				float num2 = 1.5f * this._cellWidth;
				float d = Mathf.SmoothStep(this.maxScale, this.minScale, num / num2);
				this.scrollHelms[j].helmRoot.transform.localScale = Vector3.one * d;
			}
			if (UIScreenController.Instance.isShowingPopup)
			{
				if (!this._popupActive)
				{
					this._popupActive = true;
				}
			}
			else if (this._popupActive)
			{
				this._popupActive = false;
			}
			if (this._popupActive)
			{
				if (this._modelsEnabled)
				{
					this.scrollAnchor.SetActive(false);
					this._modelsEnabled = false;
				}
			}
			else if (!this._modelsEnabled)
			{
				this.scrollAnchor.SetActive(true);
				this.UpdateDisplayedModelWithTheme(this._currentHelmShown);
				this._modelsEnabled = true;
			}
		}
	}

	private void UpdateDisplayedModelWithTheme(Helmets.HelmType helmType)
	{
		this.ShowHelmetInMenu(true);
	}

	public void ShowHelmetInMenu(bool updateAnimation)
	{
		UIModelController.Instance.ShowHelmetMenuModel(this._currentHelmShown, updateAnimation);
		this.UpdateNamesAndButtons();
	}

	private void UpdateNamesAndButtons()
	{
		this.UpdateNames();
		this.UpdateButtons();
	}

	private void UpdateNames()
	{
		Helmets.Helm helm = Helmets.helmData[this._currentHelmShown];
		this.nameLabel.text = Strings.Get(helm.name);
		this.nameLabel.gameObject.SetActive(true);
		this.skill1Label.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL1_H);
		this.skill2Label.text = Strings.Get(LanguageKey.UI_POPUP_TRY_HOVERBOARD_SKILL2_H);
		this.skill1Go.SetActive(helm.useMutiplier);
		this.skill2Go.SetActive(helm.useMagent);
	}

	private void UpdateButtons()
	{
		this.selectBtn.UpdateSelectState(this._currentHelmShown);
	}

	[SerializeField]
	private GameObject scrollAnchor;

	[SerializeField]
	private UIGrid scrollGrid;

	[SerializeField]
	private UIPanel scrollPanel;

	[SerializeField]
	private GameObject dummyObject;

	[SerializeField]
	private GameObject skill1Go;

	[SerializeField]
	private UILabel skill1Label;

	[SerializeField]
	private GameObject skill2Go;

	[SerializeField]
	private UILabel skill2Label;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private AudioClip selectSound;

	[SerializeField]
	private HelmetSelectButton selectBtn;

	[SerializeField]
	private float minScale;

	[SerializeField]
	private float maxScale;

	public bool resetHelmAnimation;

	private float _cellWidth;

	private CenterOnChild _centerer;

	private Helmets.HelmType _currentHelmShown;

	private bool _hasInited;

	private bool _modelsEnabled;

	private bool _popupActive;

	private List<OverlayIndex> helmIndices = new List<OverlayIndex>();

	private List<KeyValuePair<Helmets.HelmType, Helmets.Helm>> helmList;

	private List<HelmScreen.HelmState> scrollHelms = new List<HelmScreen.HelmState>();

	private class HelmState
	{
		public Helmets.HelmType type;

		public Quaternion defaultRotation;

		public GameObject helmRoot;

		public Transform helmTransform;
	}
}
