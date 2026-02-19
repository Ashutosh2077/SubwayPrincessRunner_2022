using System;
using System.Text;
using UnityEngine;

public static class StringUtility
{
	public static int GetNextKeyValuePair(string text, int startIndex, out string key, out string value)
	{
		int num = StringUtility.NonEscapedIndexOf(text, startIndex, '[');
		if (num >= 0)
		{
			int num2 = StringUtility.NonEscapedIndexOf(text, num + 1, ']');
			if (num2 >= 0)
			{
				key = text.Substring(num + 1, num2 - num - 1).Trim();
				int num3 = StringUtility.NonEscapedIndexOf(text, num2 + 1, '[');
				if (num3 < 0)
				{
					num3 = text.Length;
				}
				value = text.Substring(num2 + 1, num3 - num2 - 1).Trim();
				return num3;
			}
		}
		key = null;
		value = null;
		return -1;
	}

	public static int NonEscapedIndexOf(string text, int startIndex, char ch)
	{
		int num = text.IndexOf(ch, startIndex);
		if (num == 0)
		{
			return num;
		}
		if (num > 0 && text[num - 1] != '\\')
		{
			return num;
		}
		return -1;
	}

	public static string ToHexString(byte[] bytes, bool lowerCase = false)
	{
		int num = (lowerCase ? 97 : 65) - 10;
		char[] array = new char[bytes.Length * 2];
		for (int i = 0; i < bytes.Length; i++)
		{
			byte b = (byte)(bytes[i] >> 4);
			array[i * 2] = ((b > 9) ? ((char)((int)b + num)) : ((char)(b + 48)));
			b = (byte) (bytes[i] & 15);
			array[i * 2 + 1] = ((b > 9) ? ((char)((int)b + num)) : ((char)(b + 48)));
		}
		return new string(array);
	}

	public static string Color32ToString(Color32 clr)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(clr.a);
		stringBuilder.Append(",");
		stringBuilder.Append(clr.b);
		stringBuilder.Append(",");
		stringBuilder.Append(clr.g);
		stringBuilder.Append(",");
		stringBuilder.Append(clr.r);
		return stringBuilder.ToString();
	}

	public static Color32 StringToColor32(string str)
	{
		Color32 result = default(Color32);
		string[] array = str.Split(new char[]
		{
			','
		});
		if (array != null && array.Length == 4)
		{
			result.a = byte.Parse(array[0]);
			result.b = byte.Parse(array[1]);
			result.g = byte.Parse(array[2]);
			result.r = byte.Parse(array[3]);
		}
		return result;
	}
}
