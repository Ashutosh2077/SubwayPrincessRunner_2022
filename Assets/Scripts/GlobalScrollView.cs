using System;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class GlobalScrollView : MonoBehaviour
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
			ScoreRankRequest.Instance.GetRankList(ServeTimeUpdate.Instance.RankWeekString, 50, RankOrder.Desc, new Action<int, object>(this.PullGlobalListListener));
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

	public void PullGlobalListListener(int status, object obj)
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
			TopRun topRun = instance.GetGlobalTopRun(keyValuePair.Key);
			if (topRun == null)
			{
				TopRun topRun2 = new TopRun();
				TopRun topRun3 = topRun2;
				KeyValuePair<string, object> keyValuePair2 = enumerator.Current;
				topRun3.userId = keyValuePair2.Key;
				topRun = topRun2;
				ServerManager.Instance.AddGlobalTopRun(topRun);
			}
			TopRun topRun4 = topRun;
			KeyValuePair<string, object> keyValuePair3 = enumerator.Current;
			topRun4.highestScore = (float)((long)keyValuePair3.Value);
			topRun.rank = num + 1;
			this.topRunList.Add(topRun);
			KeyValuePair<string, object> keyValuePair4 = enumerator.Current;
			if (keyValuePair4.Key.Equals(SecondManager.Instance.userId))
			{
				ServerManager.Instance.SynchroniseScore((int)topRun.highestScore);
				ServerManager.Instance.SynchroniseRankID(topRun.rank);
				ServerManager.Instance.ResetMyTopRunInfo();
			}
			else
			{
				list.Add(topRun.userId);
			}
			num++;
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
			StringKeyValueRequest.Instance.GetStringData(string.Join(",", value), "countryCode", new Action<int, object>(this.GetCountryCodeDataCallback));
			StringKeyValueRequest.Instance.GetStringData(string.Join(",", value), "playerName", new Action<int, object>(this.GetPlayerNameDataCallback));
			StringKeyValueRequest.Instance.GetStringData(string.Join(",", list.ToArray()), "pictureUrl", new Action<int, object>(this.GePictureUrlDataCallback));
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
		string text = string.Empty;
		string countryCode = string.Empty;
		for (int i = 0; i < count; i++)
		{
			text = this.topRunList[i].userId;
			if (dictionary.ContainsKey(text))
			{
				if (!(dictionary[text] is bool))
				{
					countryCode = (string)dictionary[text];
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
			this.result = PullListResult.NoData;
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int count = this.topRunList.Count;
		string text = string.Empty;
		string playerName = string.Empty;
		for (int i = 0; i < count; i++)
		{
			text = this.topRunList[i].userId;
			if (dictionary.ContainsKey(text))
			{
				if (!(dictionary[this.topRunList[i].userId] is bool))
				{
					playerName = (string)dictionary[text];
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
			this.result = PullListResult.NoData;
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
					pictureUrl = (string)dictionary[text];
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
		int num = 0;
		int num2 = 0;
		while (this.topRunList.Count > num)
		{
			TopRun topRun = this.topRunList[num];
			if (this._cacheRankCell[num] != null)
			{
				this._cacheRankCell[num].SetActive(true);
				this._cacheCells.Add(this._cacheRankCell[num].GetComponent<TopRunCell>());
			}
			else
			{
				TopRunCell topRunCell = this.InstantCell<TopRunCell>(this.rankcellPrefab, this.table.gameObject);
				this._cacheRankCell[num] = topRunCell.gameObject;
				this._cacheCells.Add(topRunCell);
			}
			this._cacheCells[num2].SetData(RankScreen.RankPopupType.HighScore, topRun);
			num++;
			num2++;
		}
		if (this.topRunList.Count > 0)
		{
			TopRunTipCell topRunTipCell;
			if (this._cacheRankTipGo[0] == null)
			{
				topRunTipCell = this.InstantCell<TopRunTipCell>(this.rankTipPrefab, this.table.gameObject);
				this._cacheRankTipGo[0] = topRunTipCell.gameObject;
				topRunTipCell.SetData(new TopRun
				{
					rank = -1
				});
				topRunTipCell.transform.SetSiblingIndex(0);
			}
			else
			{
				this._cacheRankTipGo[0].SetActive(true);
				topRunTipCell = this._cacheRankTipGo[0].GetComponent<TopRunTipCell>();
			}
			this._cacheTipCells.Add(topRunTipCell);
		}
		if (this.topRunList.Count > 3)
		{
			TopRunTipCell topRunTipCell2;
			if (this._cacheRankTipGo[1] == null)
			{
				topRunTipCell2 = this.InstantCell<TopRunTipCell>(this.rankTipPrefab, this.table.gameObject);
				this._cacheRankTipGo[1] = topRunTipCell2.gameObject;
				topRunTipCell2.SetData(new TopRun
				{
					rank = -2
				});
				topRunTipCell2.transform.SetSiblingIndex(4);
			}
			else
			{
				this._cacheRankTipGo[1].SetActive(true);
				topRunTipCell2 = this._cacheRankTipGo[1].GetComponent<TopRunTipCell>();
			}
			this._cacheTipCells.Add(topRunTipCell2);
		}
		if (this.topRunList.Count > 10)
		{
			TopRunTipCell topRunTipCell3;
			if (this._cacheRankTipGo[2] == null)
			{
				topRunTipCell3 = this.InstantCell<TopRunTipCell>(this.rankTipPrefab, this.table.gameObject);
				this._cacheRankTipGo[2] = topRunTipCell3.gameObject;
				topRunTipCell3.SetData(new TopRun
				{
					rank = -3
				});
				topRunTipCell3.transform.SetSiblingIndex(12);
			}
			else
			{
				this._cacheRankTipGo[2].SetActive(true);
				topRunTipCell3 = this._cacheRankTipGo[2].GetComponent<TopRunTipCell>();
			}
			this._cacheTipCells.Add(topRunTipCell3);
		}
		this.table.Reposition();
	}

	private T InstantCell<T>(GameObject prefab, GameObject parent)
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
	private GameObject rankTipPrefab;

	[SerializeField]
	private GameObject rankcellPrefab;

	[SerializeField]
	private UITable table;

	private List<TopRunCell> _cacheCells = new List<TopRunCell>(50);

	private List<TopRunTipCell> _cacheTipCells = new List<TopRunTipCell>(3);

	private GameObject[] _cacheRankCell = new GameObject[50];

	private GameObject[] _cacheRankTipGo = new GameObject[3];

	private PullListResult result = PullListResult.None;

	private float pullListTime;

	private List<TopRun> topRunList;

	private bool isShow;

	private Coroutine closeLoadingCoroutine;
}
