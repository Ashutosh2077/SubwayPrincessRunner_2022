using System;
using UnityEngine;

public class SpeedStripes : MonoBehaviour
{
	private void Activate()
	{
		this.Activate(this._fadeInTime);
	}

	private void Activate(float time)
	{
		int i = 0;
		int num = this._renderes.Length;
		while (i < num)
		{
			this._renderes[i].enabled = true;
			i++;
		}
		base.StartCoroutine(myTween.To(time, delegate(float t)
		{
			this._material.SetColor(Shaders.Instance.MainColor, Color.Lerp(Color.black, this._originalColor, t));
		}));
	}

	private void Start()
	{
		Flypack.Instance.OnDeactivateTurboHeadstart += this.OnStopTuboHeadstart;
		Flypack instance = Flypack.Instance;
		instance.OnStart = (Flypack.OnStartDelegate)Delegate.Combine(instance.OnStart, new Flypack.OnStartDelegate(this.OnStartFlypack));
		instance.OnStop = (Flypack.OnStopDelegate)Delegate.Combine(instance.OnStop, new Flypack.OnStopDelegate(this.OnStopFlypack));
		Helmet.Instance.OnSpeedStart += this.Activate;
		Helmet.Instance.OnSpeedEnd += this.Deactivate;
		Game instance2 = Game.Instance;
		instance2.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(instance2.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(this.Reset));
		this._material = new Material(this._OriginalMaterial);
		this._originalColor = this._material.GetColor(Shaders.Instance.MainColor);
		this._material.SetColor(Shaders.Instance.MainColor, Color.black);
		int i = 0;
		int num = this._renderes.Length;
		while (i < num)
		{
			this._renderes[i].material = this._material;
			this._renderes[i].enabled = false;
			i++;
		}
	}

	private void Deactivate()
	{
		this.Deactivate(this._fadeInTime);
	}

	private void Deactivate(float time)
	{
		base.StartCoroutine(myTween.To(time, delegate(float t)
		{
			this._material.SetColor(Shaders.Instance.MainColor, Color.Lerp(this._originalColor, Color.black, t));
			if (t == 1f)
			{
				int i = 0;
				int num = this._renderes.Length;
				while (i < num)
				{
					this._renderes[i].enabled = false;
					i++;
				}
			}
		}));
	}

	private void OnStartFlypack(bool isHeadstart)
	{
		if (!isHeadstart)
		{
			this.Deactivate(this._fadeInTime);
		}
		else
		{
			this.Activate(this._fadeInTime);
		}
	}

	private void OnStopFlypack()
	{
		this.Deactivate(this._fadeInTime);
	}

	private void OnStopTuboHeadstart()
	{
		if (!Helmet.Instance.IsActive)
		{
			this.Deactivate(this._fadeInTime);
		}
	}

	private void Reset()
	{
		base.StopAllCoroutines();
		this._material.SetColor(Shaders.Instance.MainColor, Color.black);
		int i = 0;
		int num = this._renderes.Length;
		while (i < num)
		{
			this._renderes[i].enabled = false;
			i++;
		}
	}

	[SerializeField]
	private float _fadeInTime = 0.5f;

	[SerializeField]
	private Renderer[] _renderes;

	[SerializeField]
	private Material _OriginalMaterial;

	private Material _material;

	private Color _originalColor;
}
