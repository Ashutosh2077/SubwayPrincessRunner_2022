using System;

public struct IntMask
{
	public IntMask(int i)
	{
		this.mask = i;
	}

	public bool this[int bit]
	{
		get
		{
			return (this.mask & 1 << bit) != 0;
		}
		set
		{
			if (value)
			{
				this.mask |= 1 << bit;
			}
			else
			{
				this.mask &= ~(1 << bit);
			}
		}
	}

	public static implicit operator int(IntMask i)
	{
		return i.mask;
	}

	public static implicit operator IntMask(int i)
	{
		return new IntMask(i);
	}

	private int mask;
}
