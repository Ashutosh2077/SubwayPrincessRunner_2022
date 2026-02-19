using System;
using System.Collections.Generic;
using UnityEngine;

public class RecodeManager
{
	public static RecodeManager Instance
	{
		get
		{
			if (RecodeManager._instance == null)
			{
				RecodeManager._instance = new RecodeManager();
			}
			return RecodeManager._instance;
		}
	}

	public void SendRecode(string recode)
	{
		this.result = RecodeManager.RecodeResult.None;
		this.goods = null;
		IvyApp.Instance.Recode(recode, new Action<string>(this.RecodeListener));
	}

	private void RecodeListener(string text)
	{
		UnityEngine.Debug.Log("RecodeListener:::" + text);
		this.result = RecodeManager.RecodeResult.None;
		if (string.IsNullOrEmpty(text))
		{
			UISliderInController.Instance.OnRecodeStatusPickedUp(false);
			return;
		}
		IDictionary<string, object> dictionary = RiseJson.Deserialize(text) as IDictionary<string, object>;
		if (dictionary == null || dictionary.Count <= 0 || !dictionary.ContainsKey("status") || !dictionary.ContainsKey("data"))
		{
			UISliderInController.Instance.OnRecodeStatusPickedUp(false);
			return;
		}
		int num = (int)((long)dictionary["status"]);
		if (num == 200)
		{
			IList<object> list = dictionary["data"] as IList<object>;
			this.goods = new RecodeManager.Good[list.Count];
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				this.goods[i] = new RecodeManager.Good();
				dictionary = (list[i] as IDictionary<string, object>);
				if (dictionary.ContainsKey("goods_type"))
				{
					this.goods[i]._type = (string)dictionary["goods_type"];
				}
				if (dictionary.ContainsKey("goods_id"))
				{
					this.goods[i]._id = (string)dictionary["goods_id"];
				}
				if (dictionary.ContainsKey("goods_num"))
				{
					this.goods[i]._num = (string)dictionary["goods_num"];
				}
				i++;
			}
			this.result = RecodeManager.RecodeResult.Success;
			UISliderInController.Instance.OnRecodeStatusPickedUp(true);
			UIScreenController.Instance.PushPopup("RedeemPopup");
		}
		else if (num == 201)
		{
			UISliderInController.Instance.OnRecodeStatusPickedUp(false);
			this.result = RecodeManager.RecodeResult.OutTime;
		}
		else if (num == 202)
		{
			UISliderInController.Instance.OnRecodeStatusPickedUp(false);
			this.result = RecodeManager.RecodeResult.Invalid;
		}
		else if (num == 203)
		{
			UISliderInController.Instance.OnRecodeStatusPickedUp(false);
			this.result = RecodeManager.RecodeResult.HadUsed;
		}
	}

	public RecodeManager.Good[] GetRecodes()
	{
		if (this.result == RecodeManager.RecodeResult.Success)
		{
			return this.goods;
		}
		return null;
	}

	public void GetRecodeGoods()
	{
		if (this.result != RecodeManager.RecodeResult.Success)
		{
			return;
		}
		int i = 0;
		int num = this.goods.Length;
		while (i < num)
		{
			RecodeManager.Good good = this.goods[i];
			int num2;
			if (int.TryParse(good._num, out num2))
			{
				string id = good._id;
				if (id != null)
				{
					if (!(id == "1"))
					{
						if (!(id == "2"))
						{
							if (!(id == "3"))
							{
								if (!(id == "4"))
								{
									if (!(id == "5"))
									{
										if (!(id == "6"))
										{
										}
									}
								}
							}
						}
						else
						{
							PlayerInfo.Instance.amountOfKeys += num2;
						}
					}
					else
					{
						PlayerInfo.Instance.amountOfCoins += num2;
					}
				}
			}
			i++;
		}
	}

	private static RecodeManager _instance;

	private RecodeManager.RecodeResult result = RecodeManager.RecodeResult.None;

	private RecodeManager.Good[] goods;

	[Serializable]
	public class RecodeData
	{
		public RecodeManager.Good[] data;
	}

	[Serializable]
	public class Good
	{
		public string _type;

		public string _id;

		public string _num;
	}

	public enum RecodeResult
	{
		Success,
		OutTime,
		Invalid,
		HadUsed,
		None
	}
}
