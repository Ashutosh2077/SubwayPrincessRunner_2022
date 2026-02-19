using System;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
	private void Awake()
	{
		NotificationsObserver instance = NotificationsObserver.Instance;
		instance.RegisterUpdateNotificationValue(NotificationType.AchiementFinished, new Func<bool>(PlayerInfo.Instance.GetAllAchievementAward));
		instance.RegisterUpdateNotificationValue(NotificationType.DialyLandingReward, new Func<bool>(PlayerInfo.Instance.DailyLandingPayOut));
		instance.RegisterUpdateNotificationValue(NotificationType.CharacterCanUnlock, new Func<bool>(this.CanShowCharacterTip));
		instance.RegisterUpdateNotificationValue(NotificationType.TopRunUp, new Func<bool>(this.TopRunHasUp));
		instance.RegisterUpdateNotificationValue(NotificationType.UpgradeCanBuy, new Func<bool>(this.UpgradeCanBuy));
		instance.RegisterUpdateNotificationValue(NotificationType.Lucky, new Func<bool>(this.Lucky));
	}

	public bool CanShowCharacterTip()
	{
		return PlayerInfo.Instance.CanUnlockCharacter() || PlayerInfo.Instance.CanUnlockHelm() || PlayerInfo.Instance.CanIncreasePowerup();
	}

	private bool TopRunHasUp()
	{
		return false;
	}

	private bool UpgradeCanBuy()
	{
		return false;
	}

	private bool Lucky()
	{
		return PlayerInfo.Instance.CheckIfLotteryCanWatchFreeView() && PlayerInfo.Instance.lotteryWatchViewRemainCount == 3;
	}
}
