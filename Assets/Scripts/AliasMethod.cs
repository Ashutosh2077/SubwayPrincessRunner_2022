using System;
using System.Collections.Generic;

public class AliasMethod
{
	public AliasMethod(double[] prob, Random rand)
	{
		if (prob == null || rand == null)
		{
			throw new NullReferenceException();
		}
		if (prob.Length == 0)
		{
			throw new ArgumentOutOfRangeException("Probability vector must be nonempty.");
		}
		this.rand = rand;
		this.length = prob.Length;
		this.probability = new double[this.length];
		this.alias = new int[this.length];
		double[] array = new double[this.length];
		Stack<int> stack = new Stack<int>();
		Stack<int> stack2 = new Stack<int>();
		for (int i = 0; i < this.length; i++)
		{
			array[i] = prob[i] * (double)this.length;
			if (array[i] < 1.0)
			{
				stack.Push(i);
			}
			else
			{
				stack2.Push(i);
			}
		}
		while (stack.Count > 0 && stack2.Count > 0)
		{
			int num = stack.Pop();
			int num2 = stack2.Pop();
			this.probability[num] = array[num];
			this.alias[num] = num2;
			array[num2] -= 1.0 - this.probability[num];
			if (array[num2] < 1.0)
			{
				stack.Push(num2);
			}
			else
			{
				stack2.Push(num2);
			}
		}
		while (stack.Count > 0)
		{
			this.probability[stack.Pop()] = 1.0;
		}
		while (stack2.Count > 0)
		{
			this.probability[stack2.Pop()] = 1.0;
		}
	}

	public int next()
	{
		int num = this.rand.Next(this.length);
		bool flag = this.rand.NextDouble() < this.probability[num];
		return (!flag) ? this.alias[num] : num;
	}

	private double[] probability;

	private int[] alias;

	private int length;

	private Random rand;
}
