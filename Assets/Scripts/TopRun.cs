using System;

[Serializable]
public class TopRun
{
	public TopRun()
	{
		this.rank = -1;
		this.highestScore = 0f;
	}

	public string userId;

	public float highestScore;

	public int rank;
}
