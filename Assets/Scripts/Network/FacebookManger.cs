using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Network
{
	public class FacebookManger : MonoBehaviour
	{
		public static FacebookManger Instance
		{
			get
			{
				if (FacebookManger._instance == null)
				{
					GameObject gameObject = new GameObject("FacebookManger");
					FacebookManger._instance = gameObject.AddComponent<FacebookManger>();
				}
				return FacebookManger._instance;
			}
		}

		public void URL()
		{
			Application.OpenURL("https://www.facebook.com/Subway-Princess-Runner-226987164727227/");
		}

		public void LoginFacebook()
		{
			if (!RiseSdk.Instance.IsLogin())
			{
				RiseSdkListener.OnSNSEvent -= this.OnLoginFacebook;
				RiseSdkListener.OnSNSEvent += this.OnLoginFacebook;
				RiseSdk.Instance.Login();
			}
			else
			{
				this.OnLoginFacebook(RiseSdk.SnsEventType.LoginSuccess, 0);
			}
		}

		private void OnLoginFacebook(RiseSdk.SnsEventType type, int id)
		{
			RiseSdkListener.OnSNSEvent -= this.OnLoginFacebook;
			this.facebookHasLogin = (type == RiseSdk.SnsEventType.LoginSuccess);
			if (this.OnFacebookLoginResult != null)
			{
				this.OnFacebookLoginResult(this.facebookHasLogin);
			}
			UnityEngine.Debug.LogError("Has Facebook Login:" + this.facebookHasLogin);
			if (this.facebookHasLogin)
			{
				PlayerInfo.Instance.hasFacebookLogin = true;
				this.GetFriends();
				this.Me();
				if (this.me != null && !string.IsNullOrEmpty(this.me.id))
				{
					SecondManager.Instance.RequestFacebookUserID(this.me.id);
				}
			}
			else
			{
				SecondManager.Instance.RequestGuestUserID();
			}
		}

		public void GetFriends()
		{
			if (!RiseSdk.Instance.IsLogin())
			{
				return;
			}
			string friends = RiseSdk.Instance.GetFriends();
			if (string.IsNullOrEmpty(friends))
			{
				return;
			}
			UnityEngine.Debug.Log(friends);
			List<object> list = RiseJson.Deserialize(friends) as List<object>;
			if (list == null || list.Count == 0)
			{
				return;
			}
			this.friendsList = new List<FaceBookPerson>(list.Count);
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
				FaceBookPerson faceBookPerson = new FaceBookPerson();
				try
				{
					object obj;
					if (dictionary.TryGetValue("id", out obj))
					{
						faceBookPerson.id = (string)obj;
					}
					if (dictionary.TryGetValue("name", out obj))
					{
						faceBookPerson.name = (string)obj;
					}
					if (dictionary.TryGetValue("picture", out obj))
					{
						faceBookPerson.picture = (string)obj;
					}
					this.friendsList.Add(faceBookPerson);
				}
				catch
				{
				}
				i++;
			}
		}

		public void Me()
		{
			string text = RiseSdk.Instance.Me();
			UnityEngine.Debug.LogError("Facebook Me:" + text);
			if (!string.IsNullOrEmpty(text) && text.Length > 5)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)RiseJson.Deserialize(text);
				this.me = new FaceBookPerson();
				if (dictionary.ContainsKey("id"))
				{
					this.me.id = dictionary["id"].ToString();
				}
				if (dictionary.ContainsKey("name"))
				{
					this.me.name = dictionary["name"].ToString();
				}
				if (dictionary.ContainsKey("picture"))
				{
					this.me.picture = dictionary["picture"].ToString();
				}
			}
			if (ImageManager.Instance.ContainsKey(this.me.picture))
			{
				return;
			}
			ImageDownloader imageDownloader = new ImageDownloader(this.me.picture, new Action<bool, ImageDownloader>(this.OnMeComplete), 60f, null);
			base.StartCoroutine(imageDownloader.Download());
		}

		private void OnMeComplete(bool result, ImageDownloader loader)
		{
		}

		public string FriendsIds()
		{
			if (this.friendsList == null || this.friendsList.Count <= 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (this.me != null)
			{
				stringBuilder.Append(this.me.id);
				stringBuilder.Append(",");
			}
			int i = 0;
			int count = this.friendsList.Count;
			while (i < count)
			{
				stringBuilder.Append(this.friendsList[i].id);
				if (i != count - 1)
				{
					stringBuilder.Append(",");
				}
				i++;
			}
			return stringBuilder.ToString();
		}

		public string GetNameAccrodingID(string id)
		{
			if (this.friendsList == null || this.friendsList.Count <= 0)
			{
				return null;
			}
			int i = 0;
			int count = this.friendsList.Count;
			while (i < count)
			{
				if (this.friendsList[i].id.Equals(id))
				{
					return this.friendsList[i].name;
				}
				i++;
			}
			return null;
		}

		public string GetPictureAccrodingID(string id)
		{
			if (this.friendsList == null || this.friendsList.Count <= 0)
			{
				return null;
			}
			int i = 0;
			int count = this.friendsList.Count;
			while (i < count)
			{
				if (this.friendsList[i].id.Equals(id))
				{
					return this.friendsList[i].picture;
				}
				i++;
			}
			return null;
		}

		private static FacebookManger _instance;

		public bool facebookHasLogin;

		public Action<bool> OnFacebookLoginResult;

		public List<FaceBookPerson> friendsList;

		public FaceBookPerson me;
	}
}
