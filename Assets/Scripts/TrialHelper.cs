using System;
using UnityEngine;

public class TrialHelper : MonoBehaviour
{
	private void OnEnable()
	{
		if (TrialManager.Instance.nothingElse || TrialManager.Instance.currentTrialInfo == null)
		{
			this.isActive = false;
			base.gameObject.SetActive(false);
		}
		else if ((TrialManager.Instance.begainDateTime.AddDays((double)PlayerInfo.Instance.totalTrialDays) - DateTime.UtcNow).Ticks < 0L)
		{
			this.isActive = false;
			base.gameObject.SetActive(false);
			TrialManager.Instance.currentTrialInfo = null;
		}
		else
		{
			this.isActive = true;
			base.gameObject.SetActive(true);
			this.tryIcon.spriteName = TrialManager.Instance.currentTrialInfo.icon;
			this.tryIcon.MakePixelPerfect();
		}
	}

	private void Update()
	{
		if (!this.isActive)
		{
			return;
		}
		if (TrialManager.Instance.nothingElse || TrialManager.Instance.currentTrialInfo == null)
		{
			this.isActive = false;
			base.gameObject.SetActive(false);
			TrialManager.Instance.currentTrialInfo = null;
			return;
		}
		if ((TrialManager.Instance.begainDateTime.AddDays((double)PlayerInfo.Instance.totalTrialDays) - DateTime.UtcNow).Ticks < 0L)
		{
			this.isActive = false;
			base.gameObject.SetActive(false);
			TrialManager.Instance.currentTrialInfo = null;
		}
		else
		{
			this.isActive = true;
			base.gameObject.SetActive(true);
		}
	}

	[SerializeField]
	private UISprite tryIcon;

	private bool isActive;
}
