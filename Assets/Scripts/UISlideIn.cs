using System;
using System.Collections;
using UnityEngine;

public class UISlideIn : MonoBehaviour
{
	private IEnumerator InvokeDidShowCallback(float waitTime)
	{
		float startPoint = 0f;
		while (startPoint < waitTime)
		{
			startPoint += RealTimeTracker.deltaTime;
			yield return null;
		}
		Action handler = this._onDidShowCallback;
		if (handler != null)
		{
			handler();
		}
		yield break;
	}

	public IEnumerator PreloadSlideIn()
	{
		base.gameObject.SetActive(true);
		yield return null;
		yield return null;
		base.gameObject.SetActive(false);
		yield break;
	}

	protected virtual void ReadyForNewMessage()
	{
		base.gameObject.SetActive(false);
		UISliderInController.Instance.ReadyForNextSlide();
	}

	public void SetupSlideIn()
	{
		base.gameObject.SetActive(true);
		this.SlideIn(null);
	}

	protected virtual void SlideIn(Action onDidShowCallback)
	{
		this._onDidShowCallback = onDidShowCallback;
		SpringPosition.Begin(base.gameObject, this.posOn, 10f).ignoreTimeScale = true;
		this._slideOutTimer = 3f;
		this._readyForNextTimer = 1f;
		base.StartCoroutine("InvokeDidShowCallback", 0.5f);
		this._triggerSlideOut = true;
	}

	protected virtual void SlideOut()
	{
		SpringPosition.Begin(base.gameObject, this.posOff, 10f).ignoreTimeScale = true;
		this._triggerReadyForNext = true;
	}

	protected virtual void Start()
	{
		base.transform.localPosition = this.posOff;
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (this._triggerReadyForNext || this._triggerSlideOut)
		{
			float deltaTime = RealTimeTracker.deltaTime;
			if (this._triggerSlideOut)
			{
				this._slideOutTimer -= deltaTime;
				if (this._slideOutTimer <= 0f)
				{
					this.SlideOut();
					this._triggerSlideOut = false;
				}
			}
			if (this._triggerReadyForNext)
			{
				this._readyForNextTimer -= deltaTime;
				if (this._readyForNextTimer <= 0f)
				{
					this._triggerReadyForNext = false;
					this.ReadyForNewMessage();
				}
			}
		}
	}

	private Action _onDidShowCallback;

	private float _readyForNextTimer = 1f;

	private float _slideOutTimer = 3f;

	private bool _triggerReadyForNext;

	private bool _triggerSlideOut;

	protected Vector3 posOff = new Vector3(0f, 250f, 0f);

	protected Vector3 posOn = new Vector3(0f, 0f, 0f);
}
