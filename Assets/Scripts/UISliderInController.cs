using System;
using System.Collections.Generic;
using UnityEngine;

public class UISliderInController : MonoBehaviour
{
	private void Awake()
	{
		if (UISliderInController.Instance == null)
		{
			UISliderInController.Instance = this;
		}
	}

	private void Start()
	{
		if (UIScreenController.Instance.curDeviceType == UIScreenController.DeviceType.iPhoneX)
		{
			UIAnchor component = base.transform.GetComponent<UIAnchor>();
			component.pixelOffset = new Vector2(0f, -132f);
		}
		else
		{
			UIScreenController.Instance.OnChangedScreen = (Action<string>)Delegate.Combine(UIScreenController.Instance.OnChangedScreen, new Action<string>(this.OnChangedScreen));
		}
		this.stopping = false;
		this.slideInActive = false;
		TasksManager.Instance.onTaskComplete = (TasksManager.TaskCompleteHandler)Delegate.Combine(TasksManager.Instance.onTaskComplete, new TasksManager.TaskCompleteHandler(this.OnTaskCompleted));
		TasksManager.Instance.onTaskSetComplete = (TasksManager.TaskSetCompleteHandler)Delegate.Combine(TasksManager.Instance.onTaskSetComplete, new TasksManager.TaskSetCompleteHandler(this.OnTaskSetCompleted));
		TasksManager.Instance.onAchievementComplete = (TasksManager.AchievementCompleteHandler)Delegate.Combine(TasksManager.Instance.onAchievementComplete, new TasksManager.AchievementCompleteHandler(this.OnAchievementCompleted));
		PlayerInfo.Instance.OnSymbolCollected = (Action<Characters.CharacterType>)Delegate.Combine(PlayerInfo.Instance.OnSymbolCollected, new Action<Characters.CharacterType>(this.OnSymbolPickUp));
		this.PreloadAllSlide();
	}

	private void OnChangedScreen(string screenName)
	{
		UIAnchor component = base.transform.GetComponent<UIAnchor>();
		if ("IngameUI".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			component.pixelOffset = new Vector2(0f, -UIScreenController.Instance.bannerHeight);
		}
		else
		{
			component.pixelOffset = Vector2.zero;
		}
	}

	private void PreloadAllSlide()
	{
		if (!this.hasPreloadAllSlide)
		{
			this.hasPreloadAllSlide = true;
			base.StartCoroutine(this.taskHelperSlide.PreloadSlideIn());
			base.StartCoroutine(this.taskSetHelperSlide.PreloadSlideIn());
			base.StartCoroutine(this.unlockSlide.PreloadSlideIn());
			base.StartCoroutine(this.errorMessageSlide.PreloadSlideIn());
			base.StartCoroutine(this.topRunTipSlide.PreloadSlideIn());
		}
	}

	private void OnTaskCompleted(string message)
	{
		this.QueueSlideIn(new UISliderInController.SlideIn(UISliderInController.SlideInType.Task, message));
	}

	private void OnAchievementCompleted(string message)
	{
		this.QueueSlideIn(new UISliderInController.SlideIn(UISliderInController.SlideInType.Achievement, message));
	}

	private void OnTaskSetCompleted()
	{
		TasksManager.Instance.PlayerDidThis(TaskTarget.ReachTaskSet, 1, -1);
		this.QueueSlideIn(new UISliderInController.SlideIn(UISliderInController.SlideInType.TaskSet));
	}

	private void OnSymbolPickUp(Characters.CharacterType type)
	{
		Characters.Model model = Characters.characterData[type];
		if (model.Price <= PlayerInfo.Instance.GetCollectedSymbols(type))
		{
			Statistics stats= PlayerInfo.Instance.stats;
			(stats )[Stat.OwnCharacters] = stats[Stat.OwnCharacters] + 1;
		}
	}

	public void OnNetErrorPickedUp()
	{
		if (!"GameoverUI".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			this.QueueSlideIn(new UISliderInController.SlideIn(UISliderInController.SlideInType.ErrorMessage, Strings.Get(LanguageKey.UI_TOP_TIP_NET_ERROR)));
		}
	}

	public void OnDataErrorPickedUp()
	{
		if (!"GameoverUI".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			this.QueueSlideIn(new UISliderInController.SlideIn(UISliderInController.SlideInType.ErrorMessage, Strings.Get(LanguageKey.UI_TOP_TIP_NET_ERROR)));
		}
	}

	public void OnGetSuccessPickedUp()
	{
		if (!"GameoverUI".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			this.QueueSlideIn(new UISliderInController.SlideIn(UISliderInController.SlideInType.ErrorMessage, Strings.Get(LanguageKey.UI_TOP_TIP_GOT_SUCCESSFULLY)));
		}
	}

	public void OnTrialFinished()
	{
		if (!"GameoverUI".Equals(UIScreenController.Instance.GetTopScreenName()))
		{
			this.QueueSlideIn(new UISliderInController.SlideIn(UISliderInController.SlideInType.ErrorMessage, Strings.Get(LanguageKey.UI_TOP_TIP_FILL_COMPLETED)));
		}
	}

	public void OnRecodeStatusPickedUp(bool success)
	{
		if (success)
		{
			this.QueueSlideIn(new UISliderInController.SlideIn(UISliderInController.SlideInType.TopRunTip, Strings.Get(LanguageKey.UI_TOP_TIP_REDEEM_SUCCESS)));
		}
		else
		{
			this.QueueSlideIn(new UISliderInController.SlideIn(UISliderInController.SlideInType.ErrorMessage, Strings.Get(LanguageKey.UI_TOP_TIP_REDEEM_CODE_ERROR)));
		}
	}

	private void ShowSlideIn()
	{
		if (this.queueSliderIn.Count > 0)
		{
			UISliderInController.SlideIn slideIn = this.queueSliderIn[0];
			this.queueSliderIn.RemoveAt(0);
			if (slideIn.type == UISliderInController.SlideInType.TopRunTip)
			{
				this.topRunTipSlide.SetupSlideTopRunTip(slideIn.payload);
			}
			else if (slideIn.type == UISliderInController.SlideInType.Task)
			{
				this.taskHelperSlide.SetupSlideInTask(slideIn.payload);
			}
			else if (slideIn.type == UISliderInController.SlideInType.Achievement)
			{
				this.taskHelperSlide.SetupSlideInAchievement(slideIn.payload);
			}
			else if (slideIn.type == UISliderInController.SlideInType.TaskSet)
			{
				this.taskSetHelperSlide.SetupSlideInTaskSet(PlayerInfo.Instance.rawMultiplier);
			}
			else if (slideIn.type == UISliderInController.SlideInType.Unlock)
			{
				this.unlockSlide.SetupSlideInUnlock(slideIn.payload);
			}
			else if (slideIn.type == UISliderInController.SlideInType.ErrorMessage)
			{
				this.errorMessageSlide.SetupErrorMessage(slideIn.payload);
			}
			this.slideInActive = true;
		}
	}

	public void QueueSlideIn(UISliderInController.SlideIn slideIn)
	{
		if (this.queueSliderIn != null && this.queueSliderIn.Count > 0)
		{
			UISliderInController.SlideIn slideIn2 = this.queueSliderIn[this.queueSliderIn.Count - 1];
			if (slideIn2.type == slideIn.type && slideIn2.payload == slideIn.payload && slideIn2.payloadInt == slideIn2.payloadInt)
			{
				return;
			}
		}
		this.queueSliderIn.Add(slideIn);
		if (!this.stopping && !this.slideInActive)
		{
			this.ShowSlideIn();
		}
	}

	public void ReadyForNextSlide()
	{
		this.slideInActive = false;
		if (!this.stopping && !this.slideInActive)
		{
			this.ShowSlideIn();
		}
	}

	public int NumberOfTaskSetSlideIns()
	{
		this.numOfTaskSetSlide = this.queueSliderIn.FindAll((UISliderInController.SlideIn c) => c.type == UISliderInController.SlideInType.TaskSet);
		return this.numOfTaskSetSlide.Count;
	}

	public void QueueMessage(string message)
	{
		this._QueueMessage(message);
	}

	private void _QueueMessage(string message)
	{
		this.queueMessage.Enqueue(message);
		if (!this.messageIsShowing)
		{
			this.ShowNextMessage();
		}
		if (!this.slideInActive)
		{
			this.ShowSlideIn();
		}
	}

	private void ShowNextMessage()
	{
		if (this.queueMessage.Count > 0)
		{
			string message = this.queueMessage.Dequeue();
			this.messageHelper.ShowMessage(message, false);
			this.messageIsShowing = true;
		}
	}

	public void ReadyForNextMessage()
	{
		this.messageIsShowing = false;
		this.ShowNextMessage();
	}

	private void OnDestroy()
	{
		TasksManager.Instance.onTaskComplete = (TasksManager.TaskCompleteHandler)Delegate.Remove(TasksManager.Instance.onTaskComplete, new TasksManager.TaskCompleteHandler(this.OnTaskCompleted));
		TasksManager.Instance.onTaskSetComplete = (TasksManager.TaskSetCompleteHandler)Delegate.Remove(TasksManager.Instance.onTaskSetComplete, new TasksManager.TaskSetCompleteHandler(this.OnTaskSetCompleted));
		PlayerInfo instance = PlayerInfo.Instance;
		PlayerInfo.Instance.OnSymbolCollected = (Action<Characters.CharacterType>)Delegate.Remove(PlayerInfo.Instance.OnSymbolCollected, new Action<Characters.CharacterType>(this.OnSymbolPickUp));
	}

	public bool Stop
	{
		get
		{
			return this.stopping;
		}
		set
		{
			this.stopping = value;
			if (!this.stopping)
			{
				this.ReadyForNextSlide();
			}
		}
	}

	public static UISliderInController Instance;

	[SerializeField]
	private UIMessageHelper messageHelper;

	[SerializeField]
	private UISlideInTaskHelper taskHelperSlide;

	public UISlideInTaskSetHelper taskSetHelperSlide;

	[SerializeField]
	private UISlideInUnlock unlockSlide;

	[SerializeField]
	private UISlideInErrorMessage errorMessageSlide;

	[SerializeField]
	private UISlideInTopRunTip topRunTipSlide;

	private bool slideInActive;

	private bool messageIsShowing;

	private bool stopping;

	private List<UISliderInController.SlideIn> numOfTaskSetSlide = new List<UISliderInController.SlideIn>();

	private List<UISliderInController.SlideIn> queueSliderIn = new List<UISliderInController.SlideIn>();

	private Queue<string> queueMessage = new Queue<string>();

	private bool hasPreloadAllSlide;

	public class SlideIn
	{
		public SlideIn(UISliderInController.SlideInType type)
		{
			this._payload = string.Empty;
			this._type = type;
		}

		public SlideIn(UISliderInController.SlideInType type, int PayLoadInt)
		{
			this._payload = string.Empty;
			this._type = type;
			this._payloadInt = PayLoadInt;
		}

		public SlideIn(UISliderInController.SlideInType type, string payload)
		{
			this._payloadInt = 0;
			this._type = type;
			this._payload = payload;
		}

		public string payload
		{
			get
			{
				return this._payload;
			}
		}

		public int payloadInt
		{
			get
			{
				return this._payloadInt;
			}
		}

		public UISliderInController.SlideInType type
		{
			get
			{
				return this._type;
			}
		}

		private string _payload;

		private int _payloadInt;

		private UISliderInController.SlideInType _type;
	}

	public enum SlideInType
	{
		TopRunTip,
		Task,
		TaskSet,
		Achievement,
		Unlock,
		ErrorMessage
	}
}
