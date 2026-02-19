using System;
using UnityEngine;

namespace Network
{
	public class ServeTimeUpdate : MonoBehaviour
	{
		public static ServeTimeUpdate Instance
		{
			get
			{
				if (ServeTimeUpdate._instance == null)
				{
					ServeTimeUpdate._instance = Utils.FindObject<ServeTimeUpdate>();
				}
				if (ServeTimeUpdate._instance == null)
				{
					ServeTimeUpdate._instance = new GameObject("ServeTimeUpdate").AddComponent<ServeTimeUpdate>();
				}
				return ServeTimeUpdate._instance;
			}
		}

		private void Awake()
		{
			if (ServeTimeUpdate._instance == null)
			{
				ServeTimeUpdate._instance = this;
			}
		}

		private void Start()
		{
			this.RequestServerTime();
		}

		public void RequestServerTime()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return;
			}
			this.IsActive = false;
			NetworkRequest.Instance.Request(NetworkConnect.RequestCommand.GetServerTime, null, new Action<string>(this.GetServerTimeListener));
		}

		private void GetServerTimeListener(string s)
		{
			UnityEngine.Debug.Log("GetServerTimeListener:" + s);
			this.IsActive = true;
			long num;
			if (!string.IsNullOrEmpty(s) && long.TryParse(s, out num))
			{
				if (this._serverTime >= num)
				{
					return;
				}
				this._serverTime = num;
				PlayerInfo.Instance.lastLotteryFreeViewDateTime = DateManager.TranslateServeTicksToDateTime(num);
				this._time = RealTimeTracker.time;
				this._deltaTime = DateManager.CalcDeltaTime(num);
				this.RankWeekString = DateManager.CalcWeekRankString(num);
				this._interval *= 3;
				if (this._requestDatasAfter)
				{
					ServerManager.Instance.RequestUserInformation(SecondManager.Instance.platType, SecondManager.Instance.userId, this);
				}
			}
		}

		private bool CheckForNewCalc()
		{
			return (double)this._time + this._deltaTime < (double)RealTimeTracker.time;
		}

		public bool ServerTimeValid()
		{
			return this._serverTime != -1L;
		}

		public bool ChechCanRequestDatas()
		{
			bool flag = this.ServerTimeValid();
			this._requestDatasAfter = !flag;
			return flag;
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
				if (!this.IsActive)
				{
					this.IsActive = true;
				}
				else if (this.CheckForNewCalc())
				{
					this.RequestServerTime();
				}
			}
		}

		public string RankWeekString { get; private set; }

		public long ServerTime
		{
			get
			{
				return this._serverTime;
			}
		}

		public float time
		{
			get
			{
				return this._time;
			}
		}

		public bool IsActive { get; set; }

		public int _interval = 60;

		public int _max_interval = 60000;

		private int _factor;

		private double _deltaTime = 60.0;

		private float _time = -1f;

		private long _serverTime = -1L;

		private bool _requestDatasAfter;

		private static ServeTimeUpdate _instance;
	}
}
