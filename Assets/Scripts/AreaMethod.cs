using System;

public class AreaMethod
{
	public AreaMethod(double[] prob, Random random)
	{
		if (prob == null || this.rand == null)
		{
			throw new NullReferenceException();
		}
		if (prob.Length == 0)
		{
			throw new ArgumentOutOfRangeException("Probability vector must be nonempty.");
		}
		this.length = prob.Length;
		this.probability = new double[this.length];
		this.probability[0] = prob[0];
		for (int i = 1; i < this.length; i++)
		{
			this.probability[i] = this.probability[i - 1] + prob[i];
		}
	}

	public int next()
	{
		double num = this.rand.NextDouble();
		int result = 0;
		int i = 0;
		int num2 = this.length - 1;
		while (i < num2)
		{
			int num3 = (i + num2) / 2;
			if (this.probability[num3] < num)
			{
				i = num3 + 1;
				if (this.probability[i] > num)
				{
					result = i;
					break;
				}
			}
			else
			{
				if (this.probability[num3] <= num)
				{
					result = num3 + 1;
					break;
				}
				num2 = num3 - 1;
				if (this.probability[num2] <= num)
				{
					result = num3;
					break;
				}
			}
		}
		return result;
	}

	private double[] probability;

	private int length;

	private Random rand;

	private int index;
}
