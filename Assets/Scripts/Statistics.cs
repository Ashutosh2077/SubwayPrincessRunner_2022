using System;
using UnityEngine;

public class Statistics
{
	public void AddStatChangedHandler(Stat stat, Statistics.StatChangedDelegate handler)
	{
		if (this._statChangedHandlers == null)
		{
			this._statChangedHandlers = new Statistics.StatChangedDelegate[this._values.Length];
		}
		this._statChangedHandlers[(int)stat] = (Statistics.StatChangedDelegate)Delegate.Combine(this._statChangedHandlers[(int)stat], handler);
	}

	public bool AnyListenerForStat(Stat stat)
	{
		return this._statChangedHandlers != null && this._statChangedHandlers[(int)stat] != null;
	}

	public void Clear()
	{
		foreach (object obj in Enum.GetValues(typeof(Stat)))
		{
			Stat stat = (Stat)((int)obj);
			this[stat] = 0;
		}
	}

	public void Copy(Statistics src)
	{
		foreach (object obj in Enum.GetValues(typeof(Stat)))
		{
			Stat stat = (Stat)((int)obj);
			this[stat] = src[stat];
		}
	}

	public static Statistics Parse(string s)
	{
		Statistics statistics = new Statistics();
		if (s.Length > 0)
		{
			char[] separator = new char[]
			{
				';'
			};
			string[] array = s.Split(separator);
			int i = 0;
			int num = array.Length;
			while (i < num)
			{
				int num2 = array[i].IndexOf('=');
				string text = array[i].Substring(0, num2);
				try
				{
					statistics._values[(int)Enum.Parse(typeof(Stat), text, true)] = int.Parse(array[i].Substring(num2 + 1));
				}
				catch (ArgumentException)
				{
					UnityEngine.Debug.LogWarning(string.Format("Unknown int stat \"{0}\"encountered while reading statistics: ", text));
				}
				i++;
			}
		}
		return statistics;
	}

	public void RemoveStatChangedHandler(Stat stat, Statistics.StatChangedDelegate handler)
	{
		if (this._statChangedHandlers != null)
		{
			this._statChangedHandlers[(int)stat] = (Statistics.StatChangedDelegate)Delegate.Remove(this._statChangedHandlers[(int)stat], handler);
		}
	}

	public override string ToString()
	{
		int num = 0;
		int[] values = this._values;
		for (int i = 0; i < values.Length; i++)
		{
			if (values[i] != 0)
			{
				num++;
			}
		}
		Stat[] array = (Stat[])Enum.GetValues(typeof(Stat));
		string[] array2 = new string[num];
		int num2 = 0;
		int j = 0;
		int num3 = array.Length;
		while (j < num3)
		{
			if (this[array[j]] != 0)
			{
				array2[num2] = string.Format("{0}={1}", array[j], this[array[j]]);
				num2++;
			}
			j++;
		}
		return string.Join(";", array2);
	}

	public int this[string stat]
	{
		get
		{
			return this._values[(int)Enum.Parse(typeof(Stat), stat, true)];
		}
		set
		{
			this[(Stat)((int)Enum.Parse(typeof(Stat), stat, true))] = value;
		}
	}

	public int this[TempStat tempStat]
	{
		get
		{
			return this._tempStatValues[(int)tempStat];
		}
		set
		{
			if (this._tempStatValues[(int)tempStat] != value)
			{
				this._tempStatValues[(int)tempStat] = value;
				this.dirty = true;
				if (this._tempStatChangedHandlers != null)
				{
					Statistics.TempStatChangedDelegate tempStatChangedDelegate = this._tempStatChangedHandlers[(int)tempStat];
					if (tempStatChangedDelegate != null)
					{
						tempStatChangedDelegate(tempStat, value);
					}
				}
			}
		}
	}

	public int this[Stat stat]
	{
		get
		{
			return this._values[(int)stat];
		}
		set
		{
			if (this._values[(int)stat] != value)
			{
				this._values[(int)stat] = value;
				this.dirty = true;
				if (this._statChangedHandlers != null)
				{
					Statistics.StatChangedDelegate statChangedDelegate = this._statChangedHandlers[(int)stat];
					if (statChangedDelegate != null)
					{
						statChangedDelegate(stat, value);
					}
				}
			}
		}
	}

	private Statistics.StatChangedDelegate[] _statChangedHandlers;

	private Statistics.TempStatChangedDelegate[] _tempStatChangedHandlers;

	private int[] _tempStatValues;

	private int[] _values = new int[Enum.GetNames(typeof(Stat)).Length];

	public bool dirty;

	public delegate void StatChangedDelegate(Stat stat, int newValue);

	public delegate void TempStatChangedDelegate(TempStat tempStat, int newValue);
}
