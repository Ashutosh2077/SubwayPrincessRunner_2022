using System;
using Network;
using UnityEngine;

public class HighestScoreHelper : MonoBehaviour
{
	private void Awake()
	{
		this.Init();
		this.NewGame();
	}

	public void AnimateIn()
	{
		if (!this._passedHighScore)
		{
			this.currentAnimationState = HighestScoreHelper.AnimatingState.AnimatingIn;
		}
	}

	public void AnimateOut()
	{
		if (this._gameRunning)
		{
			this.currentAnimationState = HighestScoreHelper.AnimatingState.AnimatingOut;
		}
	}

	public void GameOver()
	{
		this._gameRunning = false;
		this._cachedTransform.localPosition = this._resetPosition;
		this.background.alpha = this._backgroundAlphaDefault;
		this.points.alpha = this._pointsAlphaDefault;
		this.scoreType.alpha = this._scoreTypeAlphaDefault;
		this.currentAnimationState = HighestScoreHelper.AnimatingState.OffScreen;
	}

	private void Init()
	{
		this._backgroundAlphaDefault = this.background.alpha;
		this._pointsAlphaDefault = this.points.alpha;
		this._scoreTypeAlphaDefault = this.scoreType.alpha;
		this._cachedTransform = base.transform;
		this.inited = true;
		Game.Instance.OnGameStarted = (Action)Delegate.Combine(Game.Instance.OnGameStarted, new Action(this.NewGame));
	}

	private int CalculateGapToFriendsScore(int scoreTo, int currentScore)
	{
		return scoreTo - currentScore;
	}

	public void NewGame()
	{
		GameStats.Instance.Reset();
		this.GameOver();
		this._gameRunning = true;
		if (SecondManager.Instance.facebook)
		{
			PictureUrl pictureUrl = ServerManager.Instance.PictureUrl;
			if (pictureUrl != null)
			{
				this.head.mainTexture = pictureUrl.Image;
			}
		}
		else
		{
			this.head.mainTexture = null;
		}
		if (PlayerInfo.Instance.tutorialCompleted)
		{
			this.SetNewHighScore();
			this.AnimateIn();
		}
	}

	private bool CheckNewHighScore()
	{
		if (this._HighestScore <= GameStats.Instance.score)
		{
			return false;
		}
		this.points.color = UIPosScalesAndNGUIAtlas.Instance.ingameHighScorePointsColor;
		this.points.text = this._HighestScore.ToString();
		this.scoreType.color = UIPosScalesAndNGUIAtlas.Instance.ingameHighScoreScoreTxtColor;
		this.scoreType.text = Strings.Get(LanguageKey.INGAME_UI_HIGHSCORE);
		return true;
	}

	public void SetNewHighScore()
	{
		if (this._gameRunning)
		{
			this._HighestScore = PlayerInfo.Instance.highestScore;
			this._passedHighScore = false;
			if (this.CheckNewHighScore())
			{
				this.AnimateIn();
			}
		}
	}

	private void Update()
	{
		if (!this.inited)
		{
			this.Init();
		}
		HighestScoreHelper.AnimatingState animatingState = this.currentAnimationState;
		if (animatingState != HighestScoreHelper.AnimatingState.AnimatingIn)
		{
			if (animatingState == HighestScoreHelper.AnimatingState.AnimatingOut)
			{
				this._current += Time.deltaTime;
				float t = this._current / this._duration;
				this.vector = Vector3.Lerp(this._inPosition, this._resetPosition, this._current / this._duration);
				this.background.alpha = Mathf.Lerp(this._backgroundAlphaDefault, 0f, t);
				this.points.alpha = Mathf.Lerp(this._pointsAlphaDefault, 0f, t);
				this.scoreType.alpha = Mathf.Lerp(this._scoreTypeAlphaDefault, 0f, t);
				if (this._current >= this._duration)
				{
					this._current = 0f;
					this._cachedTransform.localPosition = this._resetPosition;
					this.background.alpha = this._backgroundAlphaDefault;
					this.points.alpha = this._pointsAlphaDefault;
					this.scoreType.alpha = this._scoreTypeAlphaDefault;
					this.currentAnimationState = HighestScoreHelper.AnimatingState.OffScreen;
				}
				else
				{
					this._cachedTransform.localPosition = this.vector;
				}
			}
		}
		else
		{
			this._current += Time.deltaTime;
			this.vector = Vector3.Lerp(this._resetPosition, this._inPosition, this._current / this._duration);
			this._cachedTransform.localPosition = this.vector;
			if (this._current >= this._duration)
			{
				this._current = 0f;
				this.currentAnimationState = HighestScoreHelper.AnimatingState.OnScreen;
			}
		}
		if (this.currentAnimationState != HighestScoreHelper.AnimatingState.AnimatingOut && this.currentAnimationState != HighestScoreHelper.AnimatingState.OffScreen && this.currentAnimationState != HighestScoreHelper.AnimatingState._notset && this._gameRunning && !this._passedHighScore && PlayerInfo.Instance.tutorialCompleted && !this._passedHighScore)
		{
			if (this._HighestScore >= GameStats.Instance.score)
			{
				this.points.text = this.CalculateGapToFriendsScore(this._HighestScore, GameStats.Instance.score).ToString();
			}
			else
			{
				this._passedHighScore = true;
				this.points.text = "0";
				this.AnimateOut();
			}
		}
	}

	public UISprite background;

	public UILabel points;

	public UILabel scoreType;

	public HighestScoreHelper.AnimatingState currentAnimationState;

	public UITexture head;

	private int _HighestScore;

	private bool _passedHighScore;

	private float _duration = 0.5f;

	private float _backgroundAlphaDefault;

	private Transform _cachedTransform;

	private float _current;

	private bool _gameRunning;

	private float _pointsAlphaDefault;

	[SerializeField]
	private Vector3 _inPosition = new Vector3(0f, -240f, 0f);

	[SerializeField]
	private Vector3 _resetPosition = new Vector3(150f, -240f, 0f);

	private float _scoreTypeAlphaDefault;

	private bool inited;

	private Vector3 vector;

	public enum AnimatingState
	{
		_notset,
		OnScreen,
		OffScreen,
		AnimatingIn,
		AnimatingOut
	}
}
