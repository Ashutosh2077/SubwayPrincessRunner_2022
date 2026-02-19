using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
	public class SecondManager : MonoBehaviour
	{
		public static SecondManager Instance
		{
			get
			{
				if (SecondManager._instance == null)
				{
					GameObject gameObject = new GameObject("SecondManager");
					SecondManager._instance = gameObject.AddComponent<SecondManager>();
				}
				return SecondManager._instance;
			}
		}

		private void Awake()
		{
			if (SecondManager._instance == null)
			{
				SecondManager._instance = this;
			}
			this._interval = this._min_interval;
			this._guestLoginIn = new SecondManager.LoginIn(PlatFormType.guest);
			this._facebookLoginIn = new SecondManager.LoginIn(PlatFormType.facebook);
			this._currentPlatType = PlatFormType.guest;
		}

		public void RequestGuestUserID()
		{
			this.ResetTime();
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return;
			}
			Dictionary<string, string> userLoginDict = NetworkConnect.Instance.GetUserLoginDict(PlatFormType.guest, null);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetUniqueUid, userLoginDict, new Action<string>(this._guestLoginIn.GetUserIDListener));
		}

		public void RequestFacebookUserID(string platId)
		{
			this._facebookLoginIn.platId = platId;
			this._currentPlatType = PlatFormType.facebook;
			this.RequestFacebookUserID();
		}

		private void RequestFacebookUserID()
		{
			this.ResetTime();
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return;
			}
			Dictionary<string, string> userLoginDict = NetworkConnect.Instance.GetUserLoginDict(PlatFormType.facebook, this._facebookLoginIn.platId);
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetUniqueUid, userLoginDict, new Action<string>(this._facebookLoginIn.GetUserIDListener));
		}

		private void ResetTime()
		{
			this._factor = 0;
			base.enabled = true;
		}

		private void Update()
		{
			if (this._factor < this._interval)
			{
				this._factor++;
			}
			else
			{
				this._factor = 0;
				if (this._interval < this._max_interval)
				{
					this._interval *= 2;
				}
				else
				{
					this._interval = this._max_interval;
				}
				if (this._currentPlatType == PlatFormType.guest && this._guestLoginIn.hasInited)
				{
					base.enabled = false;
					this._interval = this._min_interval;
					ServerManager.Instance.RequestUserInformation(this._currentPlatType, this._guestLoginIn.userId, this);
					return;
				}
				if (this._currentPlatType == PlatFormType.facebook && this._facebookLoginIn.hasInited)
				{
					base.enabled = false;
					this._interval = this._min_interval;
					ServerManager.Instance.RequestUserInformation(this._currentPlatType, this._facebookLoginIn.userId, this);
					return;
				}
				if (this._currentPlatType == PlatFormType.guest)
				{
					this.RequestGuestUserID();
					this._interval = this._min_interval;
					return;
				}
				if (this._currentPlatType == PlatFormType.facebook)
				{
					this.RequestFacebookUserID();
					this._interval = this._min_interval;
					return;
				}
			}
		}

		public PlatFormType platType
		{
			get
			{
				return this._currentPlatType;
			}
		}

		public string userId
		{
			get
			{
				if (this._currentPlatType == PlatFormType.facebook)
				{
					return this._facebookLoginIn.userId;
				}
				return this._guestLoginIn.userId;
			}
		}

		public bool hasInited
		{
			get
			{
				if (this._currentPlatType == PlatFormType.facebook)
				{
					return this._facebookLoginIn.hasInited;
				}
				return this._guestLoginIn.hasInited;
			}
		}

		public bool facebook
		{
			get
			{
				return this._currentPlatType == PlatFormType.facebook && this._facebookLoginIn.hasInited;
			}
		}

		private static SecondManager _instance;

		public int _min_interval = 300;

		public int _max_interval = 60000;

		private int _factor;

		private int _interval;

		private SecondManager.LoginIn _guestLoginIn;

		private SecondManager.LoginIn _facebookLoginIn;

		private PlatFormType _currentPlatType;

		public class LoginIn
		{
			public LoginIn(PlatFormType type)
			{
				this._platType = type;
			}

			public void GetUserIDListener(string s)
			{
				UnityEngine.Debug.Log("GetUserIDListener: " + s);
				if (string.IsNullOrEmpty(s) || !s.Contains("status"))
				{
					return;
				}
				IDictionary<string, object> dictionary = RiseJson.Deserialize(s) as IDictionary<string, object>;
				if (dictionary == null || !dictionary.ContainsKey("status"))
				{
					return;
				}
				int num = (int)((long)dictionary["status"]);
				if (num == -1 || num == 0)
				{
					if (dictionary.ContainsKey("msg"))
					{
						UnityEngine.Debug.Log(dictionary["msg"]);
					}
					return;
				}
				if (!dictionary.ContainsKey("data"))
				{
					return;
				}
				this.userId = (string)dictionary["data"];
				this.hasInited = true;
			}

			public string platId { get; set; }

			public bool hasInited { get; private set; }

			public string userId { get; private set; }

			private PlatFormType _platType;
		}
	}
}
