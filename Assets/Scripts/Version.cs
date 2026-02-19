using System;
using UnityEngine;

public class Version : MonoBehaviour
{
	private void Awake()
	{
		base.StartCoroutine(DelayInvoke.start(delegate
		{
			this.Check();
		}, 3f));
		this.child.SetActive(false);
	}

	private void Check()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			this.CancelUpdate();
		}
		else
		{
			this.CheckUpdate();
		}
	}

	private void CancelUpdate()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void CheckUpdate()
	{
		string remoteConfigString = RiseSdk.Instance.GetRemoteConfigString("data");
		if (string.IsNullOrEmpty(remoteConfigString))
		{
			this.CancelUpdate();
		}
		else if (Application.version.Equals(remoteConfigString))
		{
			if (PlayerInfo.Instance.updateFromLastApp)
			{
				this.GetReward();
			}
			PlayerInfo.Instance.updateFromLastApp = false;
			this.CancelUpdate();
		}
		else
		{
			PlayerInfo.Instance.updateFromLastApp = true;
			this.child.SetActive(true);
		}
	}

	public void ShowUpdate()
	{
		NewUpdatePopup.ShowUpdate = true;
		UIScreenController.Instance.PushPopup("NewUpdatePopup");
	}

	public void GetReward()
	{
		NewUpdatePopup.ShowUpdate = false;
		UIScreenController.Instance.PushPopup("NewUpdatePopup");
	}

	[SerializeField]
	private GameObject child;
}
