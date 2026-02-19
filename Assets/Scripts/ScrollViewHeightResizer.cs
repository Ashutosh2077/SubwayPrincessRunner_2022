using System;
using System.Collections;
using UnityEngine;

public class ScrollViewHeightResizer : MonoBehaviour
{
	private void AddColliders()
	{
		Vector3 zero = Vector3.zero;
		zero.z = this._zDepth;
		if (!this.isCalculated)
		{
			if (this._addTopCollider)
			{
				GameObject gameObject = NGUITools.AddChild(base.gameObject.transform.gameObject);
				gameObject.AddComponent<BoxCollider>().center = zero;
				gameObject.name = "TopScrollBlocker";
				float num = (float)Screen.height - (this._scrollCollider.transform.localPosition.y + this._scrollCollider.transform.localScale.y / 2f);
				float y = (float)Screen.height - num / 2f;
				float x = (float)Screen.width;
				float x2 = 0f;
				float z = base.gameObject.transform.localPosition.z;
				gameObject.transform.localPosition = new Vector3(x2, y, z);
				gameObject.transform.localScale = new Vector3(x, num, 1f);
			}
			if (this._addBottomcollider)
			{
				GameObject gameObject2 = NGUITools.AddChild(base.gameObject.transform.gameObject);
				gameObject2.AddComponent<BoxCollider>().center = zero;
				gameObject2.name = "BottomScrollBlocker";
				float num2 = this._scrollCollider.transform.localPosition.y - this._scrollCollider.transform.localScale.y / 2f;
				float y2 = num2 / 2f;
				float x3 = (float)Screen.width;
				float x4 = 0f;
				float z2 = base.gameObject.transform.localPosition.z;
				gameObject2.transform.localPosition = new Vector3(x4, y2, z2);
				gameObject2.transform.localScale = new Vector3(x3, num2, 1f);
			}
		}
	}

	public void RearrangeWidgets()
	{
		UIRoot uiroot = null;
		if (UIScreenController.Instance != null)
		{
			uiroot = UIScreenController.Instance.root;
		}
		if (uiroot == null)
		{
			UnityEngine.Debug.LogWarning("Root is not set in the UIScreenController");
		}
		UIScrollBar component = this._scrollBar.GetComponent<UIScrollBar>();
		float num = (float)uiroot.manualHeight - this._staticObjectsHeight;
		Vector4 baseClipRegion = this._scrollPanel.baseClipRegion;
		baseClipRegion.w = num;
		this._scrollPanel.baseClipRegion = baseClipRegion;
		Vector3 localPosition = this._scrollPanel.transform.localPosition;
		localPosition.y = this._scrollPanel.clipOffset.y * -1f;
		this._scrollPanel.transform.localPosition = localPosition;
		Vector3 localPosition2 = this._grid.transform.localPosition;
		localPosition2.y = 0f;
		this._grid.transform.localPosition = localPosition2;
		float y = (float)(uiroot.manualHeight / 2) + this._centerOffset;
		float y2 = (float)uiroot.manualHeight - this._staticObjectsHeight;
		Vector3 localPosition3 = this._scrollCollider.transform.localPosition;
		localPosition3.y = y;
		this._scrollCollider.transform.localPosition = localPosition3;
		Vector3 localScale = this._scrollCollider.transform.localScale;
		localScale.y = y2;
		this._scrollCollider.transform.localScale = localScale;
		UIWidget backgroundWidget = component.backgroundWidget;
		UIWidget foregroundWidget = component.foregroundWidget;
		int num2 = Mathf.RoundToInt(num);
		backgroundWidget.height = num2;
		foregroundWidget.height = num2;
		Vector3 localPosition4 = this._scrollBar.transform.localPosition;
		localPosition4.y = (float)(num2 / 2);
		this._scrollBar.transform.localPosition = localPosition4;
		base.StartCoroutine(this.RepositionscrollView(1));
		this.AddColliders();
		this.isCalculated = true;
	}

	private IEnumerator RepositionscrollView(int frames)
	{
		int index__0 = 0;
		while (index__0 < frames)
		{
			index__0++;
			yield return null;
		}
		UIScrollView dragPn__ = this._scrollPanel.GetComponent<UIScrollView>();
		if (dragPn__ != null)
		{
		}
		yield break;
	}

	private void Start()
	{
		this.rearrange = false;
		if (this._scrollBar == null)
		{
			UnityEngine.Debug.LogError("ScrollBar not set in ScrollViewHeightResizer");
		}
		if (this._scrollCollider == null)
		{
			UnityEngine.Debug.LogError("ScrollCollider not set in ScrollViewHeightResizer");
		}
		if (this._scrollPanel == null)
		{
			UnityEngine.Debug.LogError("ScrollPanel not set in ScrollViewHeightResizer");
		}
		if (this._grid == null)
		{
			UnityEngine.Debug.LogError("Grid not set in ScrollViewHeightResizer");
		}
		if (this._grid != null)
		{
		}
		this.RearrangeWidgets();
	}

	private void Update()
	{
		if (this.rearrange)
		{
			UnityEngine.Debug.Log("ScrollViewResizer update (rearrange)" + base.gameObject.name, this);
			this.RearrangeWidgets();
			this.rearrange = false;
		}
	}

	public Vector4 clipping
	{
		get
		{
			if (!this.isCalculated)
			{
				this.RearrangeWidgets();
			}
			return this._scrollPanel.finalClipRegion;
		}
	}

	public float Height
	{
		get
		{
			return this._staticObjectsHeight;
		}
		set
		{
			this._staticObjectsHeight = value;
		}
	}

	public Vector3 scrollPanelPosition
	{
		get
		{
			if (!this.isCalculated)
			{
				this.RearrangeWidgets();
			}
			return this._scrollPanel.transform.localPosition;
		}
	}

	[SerializeField]
	private GameObject _scrollBar;

	[SerializeField]
	private GameObject _scrollCollider;

	[SerializeField]
	private UIPanel _scrollPanel;

	[SerializeField]
	private GameObject _grid;

	[SerializeField]
	private float _staticObjectsHeight;

	[SerializeField]
	private float _centerOffset;

	[SerializeField]
	private bool _addTopCollider;

	[SerializeField]
	private bool _addBottomcollider;

	[SerializeField]
	private float _zDepth;

	[SerializeField]
	private bool rearrange;

	private bool isCalculated;
}
