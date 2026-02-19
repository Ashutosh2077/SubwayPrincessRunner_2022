using System;
using System.Text;

public class CelebrationReward
{
	public void PopulateFromString(string rewardAsString)
	{
		this.ResetToDefaultValues();
		char[] separator = new char[]
		{
			','
		};
		string[] array = rewardAsString.Split(separator);
		try
		{
			if (array.Length >= 11)
			{
				this.CelebrationRewardOrigin = (CelebrationRewardOrigin)((int)Enum.Parse(typeof(CelebrationRewardOrigin), array[0]));
				this.rewardType = (CelebrationRewardType)((int)Enum.Parse(typeof(CelebrationRewardType), array[1]));
				this.amount = int.Parse(array[2]);
				this.characterType = (Characters.CharacterType)((int)Enum.Parse(typeof(Characters.CharacterType), array[3]));
				this.characterThemeIndex = int.Parse(array[4]);
				this.helmType = (Helmets.HelmType)((int)Enum.Parse(typeof(Helmets.HelmType), array[5]));
				this.powerupType = (PropType)((int)Enum.Parse(typeof(PropType), array[7]));
				this.Uid = long.Parse(array[8]);
				this.rank = int.Parse(array[9]);
				this.score = int.Parse(array[10]);
			}
		}
		catch
		{
			PlayerInfo.Instance.pendingRewards.Clear();
			PlayerInfo.Instance.SaveIfDirty();
		}
	}

	private void ResetToDefaultValues()
	{
		this.CelebrationRewardOrigin = CelebrationRewardOrigin.Notset;
		this.rewardType = CelebrationRewardType._notset;
		this.amount = 0;
		this.characterType = Characters.CharacterType.slick;
		this.characterThemeIndex = 0;
		this.helmType = Helmets.HelmType.normal;
		this.powerupType = PropType._notset;
		this.Uid = 0L;
		this.rank = 1;
		this.score = 0;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.CelebrationRewardOrigin).Append(",");
		stringBuilder.Append(this.rewardType).Append(",");
		stringBuilder.Append(this.amount).Append(",");
		stringBuilder.Append(this.characterType).Append(",");
		stringBuilder.Append(this.characterThemeIndex).Append(",");
		stringBuilder.Append(this.helmType).Append(",");
		stringBuilder.Append("helmPower").Append(",");
		stringBuilder.Append(this.powerupType).Append(",");
		stringBuilder.Append(this.Uid).Append(",");
		stringBuilder.Append(this.rank).Append(",");
		stringBuilder.Append(this.score).Append(",");
		return stringBuilder.ToString();
	}

	public bool Find(CelebrationReward cr)
	{
		return this.Uid == cr.Uid;
	}

	public CelebrationRewardOrigin CelebrationRewardOrigin;

	public CelebrationRewardType rewardType;

	public int amount;

	public Characters.CharacterType characterType;

	public int characterThemeIndex;

	public Helmets.HelmType helmType;

	public PropType powerupType;

	public long Uid;

	public int rank = 1;

	public int score;
}
