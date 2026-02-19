using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kiloo.Common
{
	public static class FileUtil
	{
		public static bool ArraysAreEqual<T>(T[] a, T[] b)
		{
			if (a == null)
			{
				return b == null;
			}
			if (b == null)
			{
				return false;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			for (int i = 0; i < a.Length; i++)
			{
				if (!object.Equals(a[i], b[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static FileInfo[] GetFilesForPath(string path, bool slotsOnly = false)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
			if (directoryInfo.Exists)
			{
				string fileName = Path.GetFileName(path);
				FileInfo[] array = directoryInfo.GetFiles(fileName + (slotsOnly ? ".*" : "*"));
				if (array.Length > 0)
				{
					array = Array.FindAll<FileInfo>(array, delegate(FileInfo file)
					{
						string name = file.Name;
						return (!slotsOnly && name.Equals(fileName, StringComparison.OrdinalIgnoreCase)) || FileUtil.GetSlotForPath(name) >= 0;
					});
					Array.Sort<FileInfo>(array, (FileInfo a, FileInfo b) => b.LastWriteTimeUtc.CompareTo(a.LastWriteTimeUtc));
					return array;
				}
			}
			return new FileInfo[0];
		}

		public static DateTime GetLastWriteTimeUtc(FileInfo fileInfo)
		{
			return fileInfo.LastWriteTimeUtc;
		}

		private static int GetMostRecentSlot(string path, bool fixTimestamps)
		{
			FileInfo[] filesForPath = FileUtil.GetFilesForPath(path, false);
			int num = filesForPath.Length;
			if (num <= 0)
			{
				return -1;
			}
			FileInfo fileInfo = filesForPath[0];
			if (fixTimestamps)
			{
				DateTime utcNow = DateTime.UtcNow;
				if (fileInfo.LastWriteTimeUtc > utcNow)
				{
					TimeSpan timeSpan = fileInfo.LastWriteTimeUtc - utcNow;
					timeSpan += TimeSpan.FromMinutes(1.0);
					for (int i = 0; i < num; i++)
					{
						filesForPath[i].LastWriteTimeUtc = filesForPath[i].LastWriteTimeUtc - timeSpan;
					}
				}
			}
			return FileUtil.GetSlotForPath(fileInfo.Name);
		}

		private static int GetSlotForPath(string path)
		{
			string extension = Path.GetExtension(path);
			int result;
			if (!string.IsNullOrEmpty(extension) && int.TryParse(extension.Substring(1), out result))
			{
				return result;
			}
			return -1;
		}

		public static byte[] Load(string path, string secret, string alternatePath, bool useLastModifiedPathIfNoSlots = false)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			byte[] secretBytes = (secret == null) ? null : Encoding.UTF8.GetBytes(secret);
			bool flag = false;
			byte[] array = null;
			useLastModifiedPathIfNoSlots = (useLastModifiedPathIfNoSlots && alternatePath != null);
			foreach (FileInfo fileInfo in FileUtil.GetFilesForPath(path, useLastModifiedPathIfNoSlots))
			{
				array = FileUtil.TryLoadFile(fileInfo.FullName, secretBytes);
				if (array != null)
				{
					break;
				}
				flag = true;
			}
			if (array == null && alternatePath != null)
			{
				foreach (FileInfo fileInfo2 in FileUtil.GetFilesForPath(alternatePath, useLastModifiedPathIfNoSlots))
				{
					array = FileUtil.TryLoadFile(fileInfo2.FullName, secretBytes);
					if (array != null)
					{
						break;
					}
					flag = true;
				}
			}
			if (array == null && useLastModifiedPathIfNoSlots)
			{
				FileInfo[] array2 = new FileInfo[]
				{
					new FileInfo(path),
					new FileInfo(alternatePath)
				};
				array2 = Array.FindAll<FileInfo>(array2, (FileInfo file) => file.Exists);
				Array.Sort<FileInfo>(array2, (FileInfo a, FileInfo b) => b.LastWriteTimeUtc.CompareTo(a.LastWriteTimeUtc));
				foreach (FileInfo fileInfo3 in array2)
				{
					array = FileUtil.TryLoadFile(fileInfo3.FullName, secretBytes);
					if (array != null)
					{
						break;
					}
					flag = true;
				}
			}
			if (array != null)
			{
				return array;
			}
			if (flag)
			{
				throw new IOException("File is corrupt along with all redundant files");
			}
			throw new FileNotFoundException();
		}

		public static byte[] ReadAllBytes(string filename)
		{
			FileInfo fileInfo = new FileInfo(filename);
			byte[] array = new byte[fileInfo.Length];
			FileStream fileStream = fileInfo.OpenRead();
			fileStream.ReadFully(array);
			fileStream.Close();
			return array;
		}

		public static Dictionary<E, int> ReadEnumIntDictionary<E>(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			Dictionary<E, int> dictionary = new Dictionary<E, int>(num);
			Type typeFromHandle = typeof(E);
			for (int i = 0; i < num; i++)
			{
				string value = reader.ReadString();
				int value2 = reader.ReadInt32();
				E key = (E)((object)Enum.Parse(typeFromHandle, value, true));
				dictionary[key] = value2;
			}
			return dictionary;
		}

		public static Dictionary<E, string> ReadEnumStringDictionary<E>(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			Dictionary<E, string> dictionary = new Dictionary<E, string>(num);
			Type typeFromHandle = typeof(E);
			for (int i = 0; i < num; i++)
			{
				string value = reader.ReadString();
				string value2 = reader.ReadString();
				if (Enum.IsDefined(typeFromHandle, value))
				{
					E key = (E)((object)Enum.Parse(typeFromHandle, value, true));
					dictionary[key] = value2;
				}
			}
			return dictionary;
		}

		public static void ReadFully(this Stream s, byte[] buffer)
		{
			s.ReadFully(buffer, 0, buffer.Length);
		}

		public static void ReadFully(this Stream s, byte[] buffer, int offset, int count)
		{
			int i = count;
			int num = 0;
			while (i > 0)
			{
				int num2 = s.Read(buffer, num, i);
				if (num2 == 0)
				{
					throw new EndOfStreamException();
				}
				num += num2;
				i -= num2;
			}
		}

		public static Dictionary<string, string> ReadStringStringDictionary(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			Dictionary<string, string> dictionary = new Dictionary<string, string>(num);
			for (int i = 0; i < num; i++)
			{
				string key = reader.ReadString();
				string value = reader.ReadString();
				dictionary[key] = value;
			}
			return dictionary;
		}

		public static void Save(string path, byte[] data, string secret, int offset = 0, int length = -1, int redundancy = 0, int slots = 0, string alternatePath = "", int alternateRedundancy = 0, int alternateSlots = 0)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (length == -1)
			{
				length = data.Length;
				if (offset > 0)
				{
					length -= offset;
				}
			}
			if (length < 0 || offset + length > data.Length)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			byte[] array;
			if (string.IsNullOrEmpty(secret))
			{
				array = data;
			}
			else
			{
				byte[] bytes = Encoding.UTF8.GetBytes(secret);
				array = new byte[bytes.Length + length];
				Array.Copy(bytes, array, bytes.Length);
				Array.Copy(data, offset, array, bytes.Length, length);
			}
			byte[] array2 = CheckEncoding.ComputeSHA1(array);
			int num = 1 + redundancy;
			if (alternatePath != null)
			{
				num += 1 + alternateRedundancy;
			}
			string[] array3 = new string[num];
			int num2 = 0;
			if (redundancy > 0 || slots > 0)
			{
				int num3 = FileUtil.GetMostRecentSlot(path, true);
				for (int i = 0; i <= redundancy; i++)
				{
					num3++;
					if (num3 >= slots)
					{
						num3 = 0;
					}
					array3[num2++] = path + "." + num3;
				}
			}
			else
			{
				array3[num2++] = path;
			}
			if (alternatePath != null)
			{
				if (alternateRedundancy > 0 || alternateSlots > 0)
				{
					int num4 = FileUtil.GetMostRecentSlot(alternatePath, true);
					for (int j = 0; j <= alternateRedundancy; j++)
					{
						num4++;
						if (num4 >= alternateSlots)
						{
							num4 = 0;
						}
						array3[num2++] = alternatePath + "." + num4;
					}
				}
				else
				{
					array3[num2++] = alternatePath;
				}
			}
			bool flag = false;
			for (int k = 0; k < array3.Length; k++)
			{
				try
				{
					using (FileStream fileStream = new FileStream(array3[k], FileMode.Create))
					{
						BinaryWriter binaryWriter = new BinaryWriter(fileStream);
						binaryWriter.Write(array2.Length);
						binaryWriter.Write(array2);
						binaryWriter.Write(length);
						binaryWriter.Write(data, offset, length);
						fileStream.Close();
						flag = true;
					}
				}
				catch (Exception ex)
				{
				}
			}
			if (!flag)
			{
				throw new IOException("All attempts to write failed");
			}
		}

		public static void SetLastWriteTimeUtc(FileInfo fileInfo, DateTime time)
		{
			fileInfo.LastWriteTimeUtc = time;
		}

		private static byte[] TryLoadFile(string path, byte[] secretBytes)
		{
			try
			{
				byte[] b;
				byte[] array;
				using (FileStream fileStream = new FileStream(path, FileMode.Open))
				{
					BinaryReader binaryReader = new BinaryReader(fileStream);
					int count = binaryReader.ReadInt32();
					b = binaryReader.ReadBytes(count);
					int count2 = binaryReader.ReadInt32();
					array = binaryReader.ReadBytes(count2);
					binaryReader.Close();
				}
				byte[] array2;
				if (secretBytes != null)
				{
					array2 = new byte[secretBytes.Length + array.Length];
					Array.Copy(secretBytes, array2, secretBytes.Length);
					Array.Copy(array, 0, array2, secretBytes.Length, array.Length);
				}
				else
				{
					array2 = array;
				}
				if (FileUtil.ArraysAreEqual<byte>(CheckEncoding.ComputeSHA1(array2), b))
				{
					return array;
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		public static void WriteAllBytes(string filename, byte[] bytes)
		{
			Stream stream = new FileStream(filename, FileMode.Create);
			stream.Write(bytes, 0, bytes.Length);
			stream.Close();
		}

		public static void WriteEnumIntDictionary<E>(BinaryWriter writer, IDictionary<E, int> dict)
		{
			writer.Write(dict.Count);
			foreach (KeyValuePair<E, int> keyValuePair in dict)
			{
				string name = Enum.GetName(typeof(E), keyValuePair.Key);
				if (name == null)
				{
					throw new IOException("Key is not enum constant");
				}
				writer.Write(name);
				writer.Write(keyValuePair.Value);
			}
		}

		public static void WriteEnumStringDictionary<E>(BinaryWriter writer, IDictionary<E, string> dict)
		{
			writer.Write(dict.Count);
			foreach (KeyValuePair<E, string> keyValuePair in dict)
			{
				string name = Enum.GetName(typeof(E), keyValuePair.Key);
				if (name == null)
				{
					throw new IOException("Key is not enum constant");
				}
				writer.Write(name);
				writer.Write(keyValuePair.Value);
			}
		}

		public static void WriteStringStringDictionary(BinaryWriter writer, IDictionary<string, string> dict)
		{
			writer.Write(dict.Count);
			foreach (KeyValuePair<string, string> keyValuePair in dict)
			{
				writer.Write(keyValuePair.Key);
				writer.Write(keyValuePair.Value);
			}
		}
	}
}
