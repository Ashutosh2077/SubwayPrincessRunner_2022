using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageManager
{
	public static ImageManager Instance
	{
		get
		{
			if (ImageManager._instance == null)
			{
				ImageManager._instance = new ImageManager();
			}
			return ImageManager._instance;
		}
	}

	public void Add(string url, Texture2D image)
	{
		if (string.IsNullOrEmpty(url))
		{
			return;
		}
		if (!this.loaderDict.ContainsKey(url))
		{
			this.loaderDict.Add(url, image);
			this.Save(url, image, this.loaderDict.Count);
		}
	}

	private void Save(string url, Texture2D image, int number)
	{
		string text = Application.persistentDataPath + "/Images";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		text = text + "/texture2D." + number;
		byte[] array = image.EncodeToPNG();
		using (FileStream fileStream = new FileStream(text, FileMode.Create))
		{
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(url);
			binaryWriter.Write(array.Length);
			binaryWriter.Write(array);
			fileStream.Close();
		}
	}

	public void Load()
	{
		string path = Application.persistentDataPath + "/Images";
		if (!Directory.Exists(path))
		{
			return;
		}
		FileInfo[] files = new DirectoryInfo(path).GetFiles();
		if (files == null || files.Length <= 0)
		{
			return;
		}
		int i = 0;
		int num = files.Length;
		while (i < num)
		{
			using (FileStream fileStream = files[i].OpenRead())
			{
				BinaryReader binaryReader = new BinaryReader(fileStream);
				string key = binaryReader.ReadString();
				int count = binaryReader.ReadInt32();
				byte[] data = binaryReader.ReadBytes(count);
				fileStream.Close();
				Texture2D texture2D = new Texture2D(1, 1);
				texture2D.LoadImage(data);
				this.loaderDict.Add(key, texture2D);
			}
			i++;
		}
	}

	public Texture2D GetTexture(string url)
	{
		if (string.IsNullOrEmpty(url) || !this.loaderDict.ContainsKey(url))
		{
			return null;
		}
		return this.loaderDict[url];
	}

	public bool ContainsKey(string url)
	{
		return !string.IsNullOrEmpty(url) && this.loaderDict.ContainsKey(url);
	}

	private static ImageManager _instance;

	private Dictionary<string, Texture2D> loaderDict = new Dictionary<string, Texture2D>();
}
