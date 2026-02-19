using System;
using System.Security.Cryptography;
using System.Text;

public static class CheckEncoding
{
	public static byte[] ComputeSHA1(byte[] data)
	{
		byte[] result;
		using (SHA1 sha = SHA1.Create())
		{
			result = sha.ComputeHash(data);
		}
		return result;
	}

	public static byte[] ComputeSHA1(string data, Encoding encoding)
	{
		if (encoding == null)
		{
			encoding = Encoding.UTF8;
		}
		return CheckEncoding.ComputeSHA1(encoding.GetBytes(data));
	}
}
