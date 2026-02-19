using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	private void Awake()
	{
		this.slider.value = 0f;
	}

	private void OnEnable()
	{
		this.loadingLbl.text = Strings.Get(LanguageKey.START_APP_LOADING);
	}

	private void Start()
	{
		base.StartCoroutine(this.LoadSceneAsync());
	}

	private IEnumerator LoadSceneAsync()
	{
		float ratio = 1f;
		if (GlobalInit.Instance.debug)
		{
			ratio = 0.7f;
		}
		else
		{
			ratio = 0.5f;
		}
		SceneManager.LoadScene("Merge", LoadSceneMode.Additive);
		this.slider.value = ratio * 0.5f;
		yield return null;
		if (GlobalInit.Instance.debug)
		{
			this.ao = SceneManager.LoadSceneAsync(GlobalInit.Instance.cityScenename, LoadSceneMode.Additive);
			this.ao.allowSceneActivation = false;
			this.aoList.Add(this.ao);
		}
		else
		{
			int j = 0;
			int num = GlobalInit.Instance.citiesScenename.Length;
			while (j < num)
			{
				this.ao = SceneManager.LoadSceneAsync(GlobalInit.Instance.citiesScenename[j], LoadSceneMode.Additive);
				this.ao.allowSceneActivation = false;
				this.aoList.Add(this.ao);
				j++;
			}
		}
		float curValue = this.slider.value;
		float remainProgress = 1f - curValue;
		int len = this.aoList.Count;
		for (int i = 0; i < len; i++)
		{
			this.aoList[i].allowSceneActivation = true;
			while (!this.aoList[i].isDone)
			{
				this.slider.value = curValue + remainProgress * ((float)i + this.aoList[i].progress) / (float)len;
				yield return null;
			}
		}
		LoadScene.finished = true;
		yield return null;
		UnityEngine.Object.Destroy(base.gameObject);
		Resources.UnloadAsset(this.background.mainTexture);
		Resources.UnloadUnusedAssets();
		yield break;
	}

	private IEnumerator inProgressing()
	{
		float ratio = 1f;
		if (GlobalInit.Instance.debug)
		{
			ratio = 0.7f;
		}
		else
		{
			ratio = 0.5f;
		}
		while (!this.ao.isDone)
		{
			this.progress = 0f;
			if (this.ao != null)
			{
				this.progress = this.ao.progress;
			}
			this.slider.value = this.Progress * ratio;
			yield return null;
		}
		if (GlobalInit.Instance.debug)
		{
			this.ao = SceneManager.LoadSceneAsync(GlobalInit.Instance.cityScenename, LoadSceneMode.Additive);
			while (!this.ao.isDone)
			{
				this.progress = 0f;
				if (this.ao != null)
				{
					this.progress = this.ao.progress;
				}
				this.slider.value = this.progress * (1f - ratio) + ratio;
				yield return null;
			}
		}
		else
		{
			int i = 0;
			int max = GlobalInit.Instance.citiesScenename.Length;
			while (i < max)
			{
				this.ao = SceneManager.LoadSceneAsync(GlobalInit.Instance.citiesScenename[i], LoadSceneMode.Additive);
				while (!this.ao.isDone)
				{
					this.progress = 0f;
					if (this.ao != null)
					{
						this.progress = this.ao.progress;
					}
					this.slider.value = (this.progress + (float)i) * ratio / (float)max + (1f - ratio);
					yield return null;
				}
				i++;
			}
		}
		LoadScene.finished = true;
		yield return null;
		UnityEngine.Object.Destroy(base.gameObject);
		Resources.UnloadAsset(this.background.mainTexture);
		Resources.UnloadUnusedAssets();
		yield break;
	}

	private float Progress
	{
		get
		{
			float num = this.progress * 0.5f;
			if (this.progress > 0.6f)
			{
				num += 1.25f * this.progress - 0.75f;
			}
			return num;
		}
	}

	[SerializeField]
	private UILabel loadingLbl;

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private UITexture background;

	private AsyncOperation ao;

	public static bool finished;

	private float progress;

	private List<AsyncOperation> aoList = new List<AsyncOperation>();
}
