using System;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class VipScrollView : MonoBehaviour
{
	public void Hide()
	{
		this.isShow = false;
	}

	public void Show()
	{
		if (this.isShow)
		{
			return;
		}
		this.isShow = true;
		if (this.result != PullListResult.Success || this.pullListTime + 60f < RealTimeTracker.time)
		{
			this.result = PullListResult.None;
			this.pullListTime = RealTimeTracker.time;
			UIScreenController.Instance.PushPopup("RankLoadingPopup");
			this.StopCloseLoadingCoroutine();
			this.closeLoadingCoroutine = base.StartCoroutine(DelayInvoke.start(delegate
			{
				this.CloseLoadingPopup();
			}, 30f));
			ScoreRankRequest.Instance.GetRankList("vipRankKey", 50, RankOrder.Desc, new Action<int, object>(this.PullVipsListListener));
			return;
		}
		this.FillTable();
	}

	private void StopCloseLoadingCoroutine()
	{
		if (this.closeLoadingCoroutine != null)
		{
			base.StopCoroutine(this.closeLoadingCoroutine);
			this.closeLoadingCoroutine = null;
		}
	}

	public void PullVipsListListener(int status, object obj)
	{
		this.StopCloseLoadingCoroutine();
		if (UIScreenController.Instance.isShowingPopup)
		{
			UIScreenController.Instance.CloseAllPopups();
		}
		if (status == -1)
		{
			this.result = PullListResult.NetError;
			if (this.isShow)
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
			}
			return;
		}
		if (status == 0)
		{
			this.result = PullListResult.NoData;
			if (this.isShow)
			{
				UISliderInController.Instance.OnDataErrorPickedUp();
			}
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		if (this.topRunList == null)
		{
			this.topRunList = new List<TopRun>();
		}
		this.topRunList.Clear();
		int num = 0;
		List<string> list = new List<string>(dictionary.Count);
		IEnumerator<KeyValuePair<string, object>> enumerator = dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ServerManager instance = ServerManager.Instance;
			KeyValuePair<string, object> keyValuePair = enumerator.Current;
			TopRun topRun = instance.GetVipTopRun(keyValuePair.Key);
			if (topRun == null)
			{
				TopRun topRun2 = new TopRun();
				TopRun topRun3 = topRun2;
				KeyValuePair<string, object> keyValuePair2 = enumerator.Current;
				topRun3.userId = keyValuePair2.Key;
				topRun = topRun2;
				ServerManager.Instance.AddVipTopRun(topRun);
			}
			TopRun topRun4 = topRun;
			KeyValuePair<string, object> keyValuePair3 = enumerator.Current;
			topRun4.highestScore = (float)((int)((long)keyValuePair3.Value));
			topRun.rank = num + 1;
			this.topRunList.Add(topRun);
			if (!SecondManager.Instance.facebook)
			{
				goto IL_162;
			}
			KeyValuePair<string, object> keyValuePair4 = enumerator.Current;
			if (!keyValuePair4.Key.Equals(SecondManager.Instance.userId))
			{
				goto IL_162;
			}
			IL_16F:
			num++;
			continue;
			IL_162:
			list.Add(topRun.userId);
			goto IL_16F;
		}
		this.result = PullListResult.Success;
		if (this.isShow)
		{
			UISliderInController.Instance.OnGetSuccessPickedUp();
		}
		if (list.Count > 0)
		{
			string[] value = list.ToArray();
			StringKeyValueRequest.Instance.GetStringData(string.Join(",", value), "subscription", new Action<int, object>(this.GetSubscriptionDataCallback));
			StringKeyValueRequest.Instance.GetStringData(string.Join(",", value), "playerName", new Action<int, object>(this.GetPlayerNameDataCallback));
			StringKeyValueRequest.Instance.GetStringData(string.Join(",", value), "countryCode", new Action<int, object>(this.GetCountryCodeDataCallback));
			StringKeyValueRequest.Instance.GetStringData(string.Join(",", value), "pictureUrl", new Action<int, object>(this.GePictureUrlDataCallback));
		}
		this.FillTable();
	}

	private void GetSubscriptionDataCallback(int status, object obj)
	{
		if (status == -1 || status == 0)
		{
			this.result = PullListResult.NoData;
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int count = this.topRunList.Count;
		string text = string.Empty;
		string isVip = string.Empty;
		for (int i = 0; i < count; i++)
		{
			text = this.topRunList[i].userId;
			if (dictionary.ContainsKey(text))
			{
				if (!(dictionary[text] is bool))
				{
					isVip = (string)dictionary[text];
					TopRunInfo topRunInfo = ServerManager.Instance.GetTopRunInfo(text);
					topRunInfo.isVip = isVip;
					if (this._cacheCells[i] != null)
					{
						this._cacheCells[i].RefreshVIP();
					}
				}
			}
		}
	}

	private void GetCountryCodeDataCallback(int status, object obj)
	{
		if (status == -1 || status == 0)
		{
			this.result = PullListResult.NoData;
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int count = this.topRunList.Count;
		string countryCode = string.Empty;
		string text = string.Empty;
		for (int i = 0; i < count; i++)
		{
			text = this.topRunList[i].userId;
			if (dictionary.ContainsKey(text))
			{
				if (!(dictionary[text] is bool))
				{
					countryCode = (string)dictionary[this.topRunList[i].userId];
					TopRunInfo topRunInfo = ServerManager.Instance.GetTopRunInfo(text);
					topRunInfo.countryCode = countryCode;
					if (this._cacheCells[i] != null)
					{
						this._cacheCells[i].RefreshCoutryCode();
					}
				}
			}
		}
	}

	private void GetPlayerNameDataCallback(int status, object obj)
	{
		if (status == -1 || status == 0)
		{
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int count = this.topRunList.Count;
		string playerName = string.Empty;
		string text = string.Empty;
		for (int i = 0; i < count; i++)
		{
			text = this.topRunList[i].userId;
			if (dictionary.ContainsKey(text))
			{
				if (!(dictionary[text] is bool))
				{
					playerName = (string)dictionary[this.topRunList[i].userId];
					TopRunInfo topRunInfo = ServerManager.Instance.GetTopRunInfo(text);
					topRunInfo.playerName = playerName;
					if (this._cacheCells[i] != null)
					{
						this._cacheCells[i].RefreshPlayerName();
					}
				}
			}
		}
	}

	private void GePictureUrlDataCallback(int status, object obj)
	{
		if (status == -1 || status == 0)
		{
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int count = this.topRunList.Count;
		string pictureUrl = string.Empty;
		string text = string.Empty;
		for (int i = 0; i < count; i++)
		{
			text = this.topRunList[i].userId;
			if (dictionary.ContainsKey(text))
			{
				if (!(dictionary[text] is bool))
				{
					pictureUrl = (string)dictionary[this.topRunList[i].userId];
					TopRunInfo topRunInfo = ServerManager.Instance.GetTopRunInfo(text);
					topRunInfo.pictureUrl = pictureUrl;
					if (ImageManager.Instance.ContainsKey(topRunInfo.pictureUrl))
					{
						if (this._cacheCells[i] != null)
						{
							this._cacheCells[i].RefreshImage();
						}
					}
					else
					{
						ImageDownloader imageDownloader = new ImageDownloader(topRunInfo.pictureUrl, new Action<bool, ImageDownloader>(this.OnComplete), 60f, i);
						NetworkRequest.Instance.StartCoroutine(imageDownloader.Download());
					}
				}
			}
		}
	}

	private void OnComplete(bool result, ImageDownloader loader)
	{
		if (result)
		{
			int index = (int)loader.cookie;
			if (this._cacheCells[index] != null)
			{
				this._cacheCells[index].RefreshImage();
			}
		}
	}

	private void CloseLoadingPopup()
	{
		UIScreenController.Instance.ClosePopup(null);
	}

	public void FillTable()
	{
		if (this.topRunList == null || this.topRunList.Count <= 0)
		{
			return;
		}
		if (this._cacheCells == null)
		{
			this._cacheCells = new List<TopRunCell>();
		}
		this._cacheCells.Clear();
		if (this._cacheRankGoes == null)
		{
			this._cacheRankGoes = new List<GameObject>();
		}
		int num = 0;
		int num2 = 0;
		while (this.topRunList.Count > num)
		{
			TopRun topRun = this.topRunList[num];
			if (this._cacheRankGoes.Count > num && this._cacheRankGoes[num] != null)
			{
				this._cacheRankGoes[num].SetActive(true);
				this._cacheCells.Add(this._cacheRankGoes[num].GetComponent<TopRunCell>());
			}
			else
			{
				TopRunCell topRunCell = this.InstantCell<TopRunCell>(this.rankcellPrefab, this.table.gameObject);
				this._cacheRankGoes.Add(topRunCell.gameObject);
				this._cacheCells.Add(topRunCell);
			}
			this._cacheCells[num2].SetData(RankScreen.RankPopupType.VIP, topRun);
			num++;
			num2++;
		}
		this.table.Reposition();
	}

	private TopRunCell InstantCell<T>(GameObject prefab, GameObject parent) where T : TopRunCell
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
		gameObject.transform.parent = parent.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		return gameObject.GetComponent<T>();
	}

	public void ClearTableList()
	{
		if (this._cacheCells == null || this._cacheCells.Count < 1)
		{
			return;
		}
		for (int i = this._cacheCells.Count - 1; i >= 0; i--)
		{
			UnityEngine.Object.Destroy(this._cacheCells[i].gameObject);
		}
		this._cacheCells.Clear();
	}

	[SerializeField]
	private GameObject rankcellPrefab;

	[SerializeField]
	private UITable table;

	private List<TopRunCell> _cacheCells;

	private List<GameObject> _cacheRankGoes;

	private PullListResult result = PullListResult.None;

	private float pullListTime;

	private List<TopRun> topRunList;

	private bool isShow;

	private Coroutine closeLoadingCoroutine;
}
