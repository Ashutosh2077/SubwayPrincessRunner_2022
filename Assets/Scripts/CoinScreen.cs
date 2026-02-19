using System;
using System.Collections;
using UnityEngine;

public class CoinScreen : UIBaseScreen
{
	private IEnumerator RefreshTable()
	{
		yield return null;
		yield return null;
		this._table.Reposition();
		yield break;
	}

	private void FillTable()
	{
		IEnumerator enumerator = this._table.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				NGUITools.SetActive(transform.gameObject, false);
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
		finally
		{
		}
		int num = 0;
		GameObject gameObject = NGUITools.AddChild(this._table.gameObject, this.listTitleComponent);
		ListTitleComponentHelper component = gameObject.GetComponent<ListTitleComponentHelper>();
		gameObject.name = string.Format("{0:000}", num);
		component.Setup(Strings.Get(LanguageKey.COIN_SCREEN_STORE), Strings.Get(LanguageKey.COIN_SCREEN_STORE_DESCRIPTION));
		gameObject.GetComponent<UIDragScrollView>().scrollView = this._parentDragPanel;
		num++;
		for (int i = 0; i < InAppData.inAppData.Count; i++)
		{
			this.go = NGUITools.AddChild(this._table.gameObject, this.coinPrefab);
			this.go.name = string.Format("{0:000}", num);
			this.go.GetComponent<CoinButtonHelper>().Init(i);
			this.go.GetComponent<UIDragScrollView>().scrollView = this._parentDragPanel;
			NGUITools.AddWidgetCollider(this.go);
			num++;
		}
		this._table.Reposition();
	}

	public override void Init()
	{
		base.Init();
		this.FillTable();
		if (UIBaseScreen.IsOutOfProportion())
		{
			Vector3 localPosition = this._parentDragPanel.transform.parent.localPosition;
			localPosition.y = 0f;
			this._parentDragPanel.transform.parent.localPosition = localPosition;
		}
		base.InitializeCoinbox(false, true, true, 0f, 0f, 0f);
	}

	public void RefreshCurrencyEarners()
	{
		this.FillTable();
	}

	public override void Show()
	{
		base.Show();
		this.RefreshCurrencyEarners();
		base.StartCoroutine(this.RefreshTable());
	}

	public GameObject coinPrefab;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private UIScrollView _parentDragPanel;

	[SerializeField]
	private GameObject listTitleComponent;

	private GameObject go;
}
