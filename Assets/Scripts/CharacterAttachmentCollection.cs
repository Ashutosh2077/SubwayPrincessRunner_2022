using System;
using System.Collections.Generic;

public class CharacterAttachmentCollection
{
	public CharacterAttachmentCollection()
	{
		this.helmet = Helmet.Instance;
		this.superShoes = SuperShoes.Instance;
		this.coinMagnet = CoinMagnet.Instance;
		this.doubleScireMutiplier = DoubleScoreMultiplier.Instance;
	}

	public void Add(ICharacterAttachment modifier)
	{
		if (!this.modifiers.Contains(modifier))
		{
			this.modifiers.Add(modifier);
			modifier.Current = modifier.Begain();
		}
		else
		{
			modifier.Reset();
			modifier.Current = modifier.Begain();
		}
	}

	public bool IsActive(ICharacterAttachment modifier)
	{
		return this.modifiers.Contains(modifier);
	}

	public void Pause()
	{
		int i = 0;
		int count = this.modifiers.Count;
		while (i < count)
		{
			this.modifiers[i].Pause();
			i++;
		}
	}

	public void PauseInFlypackMode()
	{
		int i = 0;
		int count = this.modifiers.Count;
		while (i < count)
		{
			if (this.modifiers[i].ShouldPauseInFlypack)
			{
				this.modifiers[i].Pause();
			}
			i++;
		}
	}

	public void Reset()
	{
		int i = 0;
		int count = this.modifiers.Count;
		while (i < count)
		{
			this.modifiers[i].Reset();
			i++;
		}
		this.modifiers.Clear();
	}

	public void Resume()
	{
		int i = 0;
		int count = this.modifiers.Count;
		while (i < count)
		{
			this.modifiers[i].Resume();
			i++;
		}
	}

	public void Stop()
	{
		int i = 0;
		int count = this.modifiers.Count;
		while (i < count)
		{
			this.modifiers[i].Stop = StopFlag.STOP;
			i++;
		}
	}

	public void StopWithNoEnding()
	{
		int i = 0;
		int count = this.modifiers.Count;
		while (i < count)
		{
			this.modifiers[i].Stop = StopFlag.STOP_NO_ENDING;
			i++;
		}
	}

	public void Update()
	{
		if (this.modifiers.Count > 0)
		{
			this.deadModifiers.Clear();
			int i = 0;
			int count = this.modifiers.Count;
			while (i < count)
			{
				if (!this.modifiers[i].Paused && !this.modifiers[i].Current.MoveNext())
				{
					this.deadModifiers.Add(this.modifiers[i]);
				}
				i++;
			}
			if (this.deadModifiers.Count > 0)
			{
				int j = 0;
				int count2 = this.deadModifiers.Count;
				while (j < count2)
				{
					this.modifiers.Remove(this.deadModifiers[j]);
					j++;
				}
			}
		}
	}

	public CoinMagnet CoinMagnet
	{
		get
		{
			return this.coinMagnet;
		}
	}

	public DoubleScoreMultiplier DoubleScoreMultiplier
	{
		get
		{
			return this.doubleScireMutiplier;
		}
	}

	public Helmet Helmet
	{
		get
		{
			return this.helmet;
		}
	}

	public SuperShoes SuperShoes
	{
		get
		{
			return this.superShoes;
		}
	}

	private CoinMagnet coinMagnet;

	private List<ICharacterAttachment> deadModifiers = new List<ICharacterAttachment>();

	private DoubleScoreMultiplier doubleScireMutiplier;

	private Helmet helmet;

	private List<ICharacterAttachment> modifiers = new List<ICharacterAttachment>();

	private SuperShoes superShoes;
}
