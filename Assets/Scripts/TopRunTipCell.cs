using System;
using UnityEngine;

public class TopRunTipCell : MonoBehaviour
{
	public void SetData(TopRun run)
	{
		this.run = run;
		this.UpdateUI();
	}

	public void UpdateUI()
	{
		LanguageKey key = LanguageKey.TOPRUN_RANK0_TITLE;
		int rank = this.run.rank;
		if (rank != -1)
		{
			if (rank != -2)
			{
				if (rank == -3)
				{
					key = LanguageKey.TOPRUN_RANK2_TITLE;
					this.title.color = this.copperColor;
					for (int i = 0; i < this.cupIcons.Length; i++)
					{
						this.cupIcons[i].spriteName = "AX_cup_Copper";
					}
				}
			}
			else
			{
				key = LanguageKey.TOPRUN_RANK1_TITLE;
				this.title.color = this.silveryColor;
				for (int j = 0; j < this.cupIcons.Length; j++)
				{
					this.cupIcons[j].spriteName = "AX_cup_Silver";
				}
			}
		}
		else
		{
			key = LanguageKey.TOPRUN_RANK0_TITLE;
			this.title.color = this.goldColor;
			for (int k = 0; k < this.cupIcons.Length; k++)
			{
				this.cupIcons[k].spriteName = "AX_cup_Gold";
			}
		}
		this.title.text = Strings.Get(key);
	}

	private TopRun run;

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private Color goldColor;

	[SerializeField]
	private Color silveryColor;

	[SerializeField]
	private Color copperColor;

	[SerializeField]
	private UISprite[] cupIcons = new UISprite[4];
}
