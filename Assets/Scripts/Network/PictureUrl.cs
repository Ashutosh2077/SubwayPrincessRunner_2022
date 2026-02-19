using System;
using UnityEngine;

namespace Network
{
	public class PictureUrl : StringKeyValue
	{
		public PictureUrl()
		{
			this._key = "pictureUrl";
			this.onValueChange = (Action)Delegate.Combine(this.onValueChange, new Action(this.DownloadImage));
			this._playerPrefsKey = "Network_PictureUrl";
			this._value = PlayerPrefs.GetString(this._playerPrefsKey, string.Empty);
			if (string.IsNullOrEmpty(this._value))
			{
				base.GetStringValue();
			}
		}

		protected override void UploadValueBeforeGetValue()
		{
		}

		protected void DownloadImage()
		{
			if (ImageManager.Instance.ContainsKey(this._value))
			{
				return;
			}
			ImageDownloader imageDownloader = new ImageDownloader(this._value, new Action<bool, ImageDownloader>(this.OnComplete), 60f, null);
			NetworkRequest.Instance.StartCoroutine(imageDownloader.Download());
		}

		private void OnComplete(bool result, ImageDownloader loader)
		{
			UnityEngine.Debug.Log("Download Result:" + result);
			if (result && this.onDownloadImageSuccess != null)
			{
				this.onDownloadImageSuccess();
			}
		}

		public Texture2D Image
		{
			get
			{
				return ImageManager.Instance.GetTexture(this._value);
			}
		}

		public Action onDownloadImageSuccess;
	}
}
