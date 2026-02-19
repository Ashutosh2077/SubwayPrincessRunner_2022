using System;

public interface IGameOverUI
{
	void Init(GameOverScreen screen);

	void Show();

	void Hide();

	void RefreshUI(bool showTryRole, int coins);

	void AfterTryCharacter();

	void AfterDoubleCoins(int coins);

	void AfterLotteryClick();

	bool FootAutoShow();
}
