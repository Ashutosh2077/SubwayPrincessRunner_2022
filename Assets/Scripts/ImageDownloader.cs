using System;
using System.Collections;
using UnityEngine;

public class ImageDownloader
{
	public ImageDownloader(string url, Action<bool, ImageDownloader> onCompleteAction, float timeout, object obj)
	{
		this.url = url;
		this._timeout = timeout;
		this.cookie = obj;
		this.onDownloadComplete = (Action<bool, ImageDownloader>)Delegate.Combine(this.onDownloadComplete, onCompleteAction);
	}

	public event Action<bool, ImageDownloader> onDownloadComplete;

	public IEnumerator Download()
	{
		float _startedDownloading = Time.realtimeSinceStartup;
		if (this.url != null)
		{
			WWW www = new WWW(this.url);
			while (!www.isDone)
			{
				if (this._timeout > 0f && Time.realtimeSinceStartup - _startedDownloading >= this._timeout)
				{
					this.DownloadComplete(false);
					yield break;
				}
				yield return null;
			}
			if (www.error != null)
			{
				this.DownloadComplete(false);
			}
			else
			{
				this.image = www.texture;
				if (this.image != null && (this.image.width != 8 || this.image.height != 8))
				{
					ImageManager.Instance.Add(this.url, this.image);
					this.DownloadComplete(true);
				}
				else
				{
					this.DownloadComplete(false);
				}
			}
		}
		yield break;
	}

	private void DownloadComplete(bool succes)
	{
		if (this.onDownloadComplete != null)
		{
			this.onDownloadComplete(succes, this);
		}
	}

	public object cookie { get; private set; }

	public Texture2D image { get; private set; }

	public string url { get; private set; }

	private float _timeout;
}
