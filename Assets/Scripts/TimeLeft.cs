using System;
using Network;
using UnityEngine;

public class TimeLeft : MonoBehaviour
{
	private void OnEnable()
	{
		this.frontLbl.text = Strings.Get(LanguageKey.UI_SCREEN_RANK_TIME);
		this.timeLbl.text = DateManager.TopRunRemainTimeToString(ServeTimeUpdate.Instance.ServerTime, ServeTimeUpdate.Instance.time);
	}

	private void OnDisable()
	{
		this.time = 0f;
	}

	private void Update()
	{
		if (this.time < 1f)
		{
			this.time += Time.deltaTime;
			return;
		}
		this.timeLbl.text = DateManager.TopRunRemainTimeToString(ServeTimeUpdate.Instance.ServerTime, ServeTimeUpdate.Instance.time);
		this.time = 0f;
	}

	[SerializeField]
	private UILabel frontLbl;

	[SerializeField]
	private UILabel timeLbl;

	[SerializeField]
	private GameObject timeLeft;

	private float time;
}
