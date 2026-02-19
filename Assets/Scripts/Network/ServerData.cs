using System;
using System.Collections;

namespace Network
{
	public class ServerData
	{
		public ServerData()
		{
			this.playerName = new PlayerName();
			this.facebookID = new FacebookID();
			this.countryCode = new CountryCode();
			this.subscription = new Subscription();
			this.medals = new Medals();
			this.score_week = new Score();
			this.rankID_week = new RankID();
			this.pictureUrl = new PictureUrl();
		}

		public IEnumerator RequestGuest()
		{
			this.RequestMedals();
			yield return null;
			this.RequestScore();
			yield return null;
			this.RequestRankID();
			yield break;
		}

		public IEnumerator RequestFacebook()
		{
			this.UploadPlayerName_Force(FacebookManger.Instance.me.name);
			yield return null;
			this.RequestMedals();
			yield return null;
			this.RequestScore();
			yield return null;
			this.RequestRankID();
			yield return null;
			this.UploadFacebookID(FacebookManger.Instance.me.id);
			yield return null;
			this.UploadPictrueUrl(FacebookManger.Instance.me.picture);
			yield break;
		}

		public bool CanUploadPlayerName()
		{
			return !SecondManager.Instance.facebook && SecondManager.Instance.hasInited && this.playerName.rule.hasGotInitValue;
		}

		public void RequestPlayerName()
		{
			if (this.playerName.rule.Check())
			{
				this.playerName.GetStringValue();
			}
		}

		public void SynchronisePlayerName(string playerName)
		{
			this.playerName.Synchronise(playerName);
		}

		public void UploadPlayerName_Strict(string playerName)
		{
			this.playerName.UploadKeyValue_Strict(playerName);
		}

		public void UploadPlayerName_Force(string playerName)
		{
			this.playerName.UploadKeyValue_Force(playerName);
		}

		public void RegisterOnPlayerNameChange(Action onPlayerNameChange)
		{
			PlayerName playerName = this.playerName;
			playerName.onValueChange = (Action)Delegate.Remove(playerName.onValueChange, onPlayerNameChange);
			PlayerName playerName2 = this.playerName;
			playerName2.onValueChange = (Action)Delegate.Combine(playerName2.onValueChange, onPlayerNameChange);
		}

		public void UnregisterOnPlayerNameChange(Action onPlayerNameChange)
		{
			PlayerName playerName = this.playerName;
			playerName.onValueChange = (Action)Delegate.Remove(playerName.onValueChange, onPlayerNameChange);
		}

		public void UploadFacebookID(string facebookID)
		{
			this.facebookID.Synchronise(facebookID);
			this.facebookID.UploadKeyValue_Force(facebookID);
		}

		public void RequestMedals()
		{
			if (this.medals.rule.Check())
			{
				this.medals.GetJsonData();
			}
		}

		public void RegisterOnMedalsChange(Action onMedalsChange)
		{
			Medals medals = this.medals;
			medals.onValueChange = (Action)Delegate.Remove(medals.onValueChange, onMedalsChange);
			Medals medals2 = this.medals;
			medals2.onValueChange = (Action)Delegate.Combine(medals2.onValueChange, onMedalsChange);
		}

		public void UnregisterOnMedalsChange(Action onMedalsChange)
		{
			Medals medals = this.medals;
			medals.onValueChange = (Action)Delegate.Remove(medals.onValueChange, onMedalsChange);
		}

		public void RequestScore()
		{
			if (this.score_week.rule.Check())
			{
				this.score_week.GetScore();
			}
		}

		public void UploadScore(int score)
		{
			this.score_week.UploadScore(score);
		}

		public void SynchroniseScore(int score)
		{
			this.score_week.Synchronise(score);
		}

		public void RegisterOnScoreChange(Action onScoreChange)
		{
			Score score = this.score_week;
			score.onValueChange = (Action)Delegate.Remove(score.onValueChange, onScoreChange);
			Score score2 = this.score_week;
			score2.onValueChange = (Action)Delegate.Combine(score2.onValueChange, onScoreChange);
		}

		public void UnregisterOnScoreChange(Action onScoreChange)
		{
			Score score = this.score_week;
			score.onValueChange = (Action)Delegate.Remove(score.onValueChange, onScoreChange);
		}

		public void RequestRankID()
		{
			if (this.rankID_week.rule.Check())
			{
				this.rankID_week.GetRankID();
			}
		}

		public void SynchroniseRankID(int rankId)
		{
			this.rankID_week.Synchronise(rankId);
		}

		public void RegisterOnRankIDChange(Action onRankIDChange)
		{
			RankID rankID = this.rankID_week;
			rankID.onValueChange = (Action)Delegate.Remove(rankID.onValueChange, onRankIDChange);
			RankID rankID2 = this.rankID_week;
			rankID2.onValueChange = (Action)Delegate.Combine(rankID2.onValueChange, onRankIDChange);
		}

		public void UnregisterOnRankIDChange(Action onRankIDChange)
		{
			RankID rankID = this.rankID_week;
			rankID.onValueChange = (Action)Delegate.Remove(rankID.onValueChange, onRankIDChange);
		}

		public void RequestCountryCode()
		{
			if (this.countryCode.rule.Check())
			{
				this.countryCode.GetStringValue();
			}
		}

		public void SynchroniseCountryCode(string countryCode)
		{
			this.countryCode.Synchronise(countryCode);
		}

		public void RegisterOnCountryCodeChange(Action onCountryCodeChange)
		{
			CountryCode countryCode = this.countryCode;
			countryCode.onValueChange = (Action)Delegate.Remove(countryCode.onValueChange, onCountryCodeChange);
			CountryCode countryCode2 = this.countryCode;
			countryCode2.onValueChange = (Action)Delegate.Combine(countryCode2.onValueChange, onCountryCodeChange);
		}

		public void UnregisterOnCountryCodeChange(Action onCountryCodeChange)
		{
			CountryCode countryCode = this.countryCode;
			countryCode.onValueChange = (Action)Delegate.Remove(countryCode.onValueChange, onCountryCodeChange);
		}

		public void RequestSubscription()
		{
			if (CheckSubscription.isSubscriptionActive)
			{
				this.UploadSubscription((!PlayerInfo.Instance.hasSubscribed) ? "no" : "yes");
			}
			else if (this.subscription.rule.Check())
			{
				this.subscription.GetStringValue();
			}
		}

		public void UploadSubscription(string value)
		{
			this.subscription.UploadKeyValue_Force(value);
		}

		public void RequestPictrueUrl()
		{
			if (this.pictureUrl.rule.Check())
			{
				this.pictureUrl.GetStringValue();
			}
		}

		public void UploadPictrueUrl(string url)
		{
			this.pictureUrl.UploadKeyValue_Force(url);
		}

		public void SynchronisePictrueUrl(string url)
		{
			this.pictureUrl.Synchronise(url);
		}

		public void RegisterOnPictrueUrlChange(Action onDownloadImageSuccess)
		{
			PictureUrl pictureUrl = this.pictureUrl;
			pictureUrl.onDownloadImageSuccess = (Action)Delegate.Remove(pictureUrl.onDownloadImageSuccess, onDownloadImageSuccess);
			PictureUrl pictureUrl2 = this.pictureUrl;
			pictureUrl2.onDownloadImageSuccess = (Action)Delegate.Combine(pictureUrl2.onDownloadImageSuccess, onDownloadImageSuccess);
		}

		public void UnregisterOnPictrueUrlChange(Action onDownloadImageSuccess)
		{
			PictureUrl pictureUrl = this.pictureUrl;
			pictureUrl.onDownloadImageSuccess = (Action)Delegate.Remove(pictureUrl.onDownloadImageSuccess, onDownloadImageSuccess);
		}

		public PlayerName playerName;

		public FacebookID facebookID;

		public CountryCode countryCode;

		public Subscription subscription;

		public Medals medals;

		public Score score_week;

		public RankID rankID_week;

		public PictureUrl pictureUrl;
	}
}
