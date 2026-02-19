using System;
using System.Text;

public class CSVReader
{
	public CSVReader(byte[] buffers)
	{
		this.mBuffers = buffers;
		this.mOffset = 0;
	}

	private bool canRead
	{
		get
		{
			return this.mBuffers != null && this.mOffset < this.mBuffers.Length;
		}
	}

	public BetterList<string> ReadLine()
	{
		this.mTemp.Clear();
		int num = this.mBuffers.Length;
		string text = string.Empty;
		bool flag = false;
		int num2 = 0;
		while (this.canRead)
		{
			if (flag)
			{
				string text2 = this.ReadLine(false);
				if (text2 == null)
				{
					return null;
				}
				text2 = text2.Replace("\\n", "\n");
				text = text + "\n" + text2;
			}
			else
			{
				string text3 = this.ReadLine(true);
				if (text3 == null)
				{
					return null;
				}
				text = text3.Replace("\\n", "\n");
				num2 = 0;
			}
			int i = num2;
			int length = text.Length;
			while (i < length)
			{
				int num3 = (int)text[i];
				if (num3 == 44 && !flag)
				{
					this.mTemp.Add(text.Substring(num2, i - num2));
					num2 = i + 1;
				}
				if (num3 == 34)
				{
					if (flag)
					{
						if (i + 1 >= length)
						{
							this.mTemp.Add(text.Substring(num2, i - num2).Replace("\"\"", "\""));
							return this.mTemp;
						}
						if (text[i + 1] != '"')
						{
							flag = false;
							this.mTemp.Add(text.Substring(num2, i - num2).Replace("\"\"", "\""));
							if (text[i + 1] == ',')
							{
								i++;
								num2 = i + 1;
							}
						}
						else
						{
							i++;
						}
					}
					else
					{
						num2 = i + 1;
						flag = true;
					}
				}
				i++;
			}
			if (num2 < text.Length)
			{
				if (!flag)
				{
					this.mTemp.Add(text.Substring(num2, text.Length - num2));
					return this.mTemp;
				}
			}
		}
		return null;
	}

	private string ReadLine(int index, int count)
	{
		return Encoding.UTF8.GetString(this.mBuffers, index, count);
	}

	public string ReadLine (bool skipEmptyLines)
	{
		int max = mBuffers.Length;

		// Skip empty characters
		if (skipEmptyLines)
		{
			while (mOffset < max && mBuffers[mOffset] < 32) ++mOffset;
		}

		int end = mOffset;

		if (end < max)
		{
			for (; ; )
			{
				if (end < max)
				{
					int ch = mBuffers[end++];
					if (ch != '\n' && ch != '\r') continue;
				}
				else ++end;

				string line = ReadLine(mOffset, end - mOffset - 1);
				mOffset = end;
				return line;
			}
		}
		mOffset = max;
		return null;
	}



	private BetterList<string> mTemp = new BetterList<string>();

	private byte[] mBuffers;

	private int mOffset;
}
