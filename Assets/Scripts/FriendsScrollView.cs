using System;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class FriendsScrollView : MonoBehaviour
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
		if (!SecondManager.Instance.facebook)
		{
			return;
		}
		if (this.result == PullListResult.Success && this.pullListTime + 60f >= RealTimeTracker.time)
		{
			this.FillTable();
			return;
		}
		this.result = PullListResult.None;
		this.pullListTime = RealTimeTracker.time;
		UIScreenController.Instance.PushPopup("RankLoadingPopup");
		this.StopCloseLoadingCoroutine();
		this.closeLoadingCoroutine = base.StartCoroutine(DelayInvoke.start(delegate
		{
			this.CloseLoadingPopup();
		}, 30f));
		string value = FacebookManger.Instance.FriendsIds();
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		ScoreRankRequest.Instance.GetFriendRankList("facebookRankKey", PlatFormType.facebook, FacebookManger.Instance.FriendsIds(), RankOrder.Desc, new Action<int, object>(this.PullFriendsListListener));
	}

	private void StopCloseLoadingCoroutine()
	{
		if (this.closeLoadingCoroutine != null)
		{
			base.StopCoroutine(this.closeLoadingCoroutine);
			this.closeLoadingCoroutine = null;
		}
	}

	public void PullFriendsListListener(int status, object obj)
	{
		StopCloseLoadingCoroutine();
		if (UIScreenController.Instance.isShowingPopup)
		{
			UIScreenController.Instance.CloseAllPopups();
		}
		switch (status)
		{
			case -1:
				result = PullListResult.NetError;
				if (isShow)
				{
					UISliderInController.Instance.OnNetErrorPickedUp();
				}
				return;
			case 0:
				result = PullListResult.NoData;
				if (isShow)
				{
					UISliderInController.Instance.OnDataErrorPickedUp();
				}
				return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		if (topRunList == null)
		{
			topRunList = new List<TopRun>();
		}
		topRunList.Clear();
		int num = 0;
		List<string> list = new List<string>(dictionary.Count);
		IEnumerator<KeyValuePair<string, object>> enumerator = dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!(enumerator.Current.Value is bool))
			{
				TopRun topRun = ServerManager.Instance.GetFriendTopRun(enumerator.Current.Key);
				if (topRun == null)
				{
					TopRun topRun2 = new TopRun();
					topRun2.userId = enumerator.Current.Key;
					topRun = topRun2;
					ServerManager.Instance.AddFriendTopRun(topRun);
				}
				topRun.highestScore = (int)(long)enumerator.Current.Value;
				topRun.rank = num + 1;
				topRunList.Add(topRun);
				list.Add(topRun.userId);
				num++;
			}
		}
		result = PullListResult.Success;
		if (isShow)
		{
			UISliderInController.Instance.OnGetSuccessPickedUp();
		}
		if (list.Count > 0)
		{
			string[] value = list.ToArray();
			StringKeyValueRequest.Instance.GetStringData(string.Join(",", value), "facebookID", GetFacebookIDDataCallback);
			StringKeyValueRequest.Instance.GetStringData(string.Join(",", value), "subscription", GetSubscriptionDataCallback);
		}
		FillTable();
	}


	private void GetSubscriptionDataCallback(int status, object obj)
	{
		if (status == -1 || status == 0)
		{
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int count = this.topRunList.Count;
		string isVip = string.Empty;
		string text = string.Empty;
		for (int i = 0; i < count; i++)
		{
			text = this.topRunList[i].userId;
			if (dictionary.ContainsKey(text))
			{
				if (!(dictionary[text] is bool))
				{
					isVip = (string)dictionary[this.topRunList[i].userId];
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

	private void GetFacebookIDDataCallback(int status, object obj)
	{
		if (status == -1 || status == 0)
		{
			return;
		}
		IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
		int count = this.topRunList.Count;
		string id = string.Empty;
		for (int i = 0; i < count; i++)
		{
			string userId = this.topRunList[i].userId;
			if (dictionary.ContainsKey(userId))
			{
				if (!(dictionary[userId] is bool))
				{
					id = (string)dictionary[this.topRunList[i].userId];
					TopRunInfo topRunInfo = ServerManager.Instance.GetTopRunInfo(userId);
					topRunInfo.facebookName = FacebookManger.Instance.GetNameAccrodingID(id);
					topRunInfo.pictureUrl = FacebookManger.Instance.GetPictureAccrodingID(id);
					if (this._cacheCells[i] != null)
					{
						this._cacheCells[i].RefreshPlayerName();
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
			this._cacheCells[num2].SetData(RankScreen.RankPopupType.Friends, topRun);
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
