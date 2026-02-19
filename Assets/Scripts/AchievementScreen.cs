using System;
using System.Collections;
using UnityEngine;

public class AchievementScreen : UIBaseScreen
{
	public override void Init()
	{
		base.Init();
		this.InitCell();
	}

	private void InitCell()
	{
		int num = 0;
		this.cells = new AchievementCell[Achievements.NUMBER_OF_ACHIEVEMENTS];
		for (int i = 0; i < Achievements.NUMBER_OF_ACHIEVEMENTS; i++)
		{
			GameObject gameObject = NGUITools.AddChild(this.table.gameObject, this.achievementCellPrefab);
			gameObject.name = string.Format("{0:D3}", num);
			AchievementCell component = gameObject.GetComponent<AchievementCell>();
			if (component == null)
			{
				UnityEngine.Debug.LogError("The achievementCellPrefab has no class - AchievementCell.");
			}
			component.Init(i, false);
			this.cells[i] = component;
			num++;
		}
		base.StartCoroutine(this.Reposition());
	}

	private IEnumerator Reposition()
	{
		yield return null;
		yield return null;
		this.table.Reposition();
		yield break;
	}

	private void RefreshOrder()
	{
		int[] array = new int[Achievements.NUMBER_OF_ACHIEVEMENTS];
		int num = 0;
		for (int i = 0; i < this.cells.Length; i++)
		{
			int state = this.cells[i].GetState();
			if (state == 1)
			{
				array[num++] = this.cells[i].GetIndex();
			}
		}
		for (int j = 0; j < this.cells.Length; j++)
		{
			int state = this.cells[j].GetState();
			if (state == 2)
			{
				array[num++] = this.cells[j].GetIndex();
			}
		}
		for (int k = 0; k < this.cells.Length; k++)
		{
			int state = this.cells[k].GetState();
			if (state == 3)
			{
				array[num++] = this.cells[k].GetIndex();
			}
		}
		for (int l = 0; l < array.Length; l++)
		{
			this.cells[l].Init(array[l], true);
			this.cells[l].RefreshUI();
		}
	}

	public override void Show()
	{
		base.Show();
		this.RefreshOrder();
		this.RefreshLabel();
	}

	private void RefreshLabel()
	{
		this.titleLbl.text = Strings.Get(LanguageKey.UI_POPUP_ACHIEVEMENT_TITLE);
	}

	[SerializeField]
	private UILabel titleLbl;

	[SerializeField]
	private GameObject achievementCellPrefab;

	[SerializeField]
	private UITable table;

	private AchievementCell[] cells;
}
