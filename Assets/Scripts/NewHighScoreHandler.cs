using System;
using System.Collections;
using UnityEngine;

public class NewHighScoreHandler : MonoBehaviour
{
	private void Awake()
	{
		this._Title.alpha = 0f;
		this._highScoreLbl.alpha = 0f;
		base.StartCoroutine(this.SetUI());
	}

	private IEnumerator SetUI()
	{
		this._Title.text = Strings.Get(LanguageKey.BRAG_CELEBRATION_NEW_HIGHSCORE);
		this._highScoreLbl.text = PlayerInfo.Instance.highestScore.ToString();
		this._highScoreLbl.alpha = 1f;
		this._Title.alpha = 1f;
		yield return new WaitForSeconds(1f);
		yield break;
	}

	[SerializeField]
	private UILabel _Title;

	[SerializeField]
	private UILabel _highScoreLbl;
}
