using System;
using UnityEngine;

internal class CharacterRenderingEffects : MonoBehaviour
{
	public void ActivateSnow()
	{
		if (!this._snowActive)
		{
			this._currentSnow.Play();
			this._snowActive = true;
		}
	}

	private void Awake()
	{
		if (DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.Low && this._showSnow)
		{
			this._snow.gameObject.SetActive(false);
			this._currentSnow = this._snowLow;
		}
		else if (DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.High && this._showSnow)
		{
			this._snowLow.gameObject.SetActive(false);
			this._currentSnow = this._snow;
		}
		else
		{
			this._snow.gameObject.SetActive(false);
			this._snowLow.gameObject.SetActive(false);
			this._currentSnow = this._snowLow;
		}
		this.emissionModule = this._currentSnow.emission;
		this._originalEmissionRate = this.emissionModule.rateOverTimeMultiplier;
		if (this._showStars)
		{
			this._stars.gameObject.SetActive(true);
			this._starsOriginalPosition = this._stars.position;
		}
		else
		{
			this._stars.gameObject.SetActive(false);
		}
	}

	public void DeactivateSnow()
	{
		if (this._snowActive)
		{
			this._snowActive = false;
			this._currentSnow.Stop();
		}
	}

	public void Initialize(CharacterModel characterModel)
	{
		this._game = Game.Instance;
		this._game.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(this._game.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(this.DeactivateSnow));
		this._game.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(this._game.OnIntroRun, new Game.OnIntroRunDelegate(this.ActivateSnow));
		this._character = Character.Instance;
		this.flypackParticleStart.Target = characterModel.flypackCloudPosition;
		this.flypackParticleIng.Target = characterModel.flypackCloudPosition;
		this.flypackParticleEnd.Target = characterModel.flypackCloudPosition;
		this.speedupFollow.Target = characterModel.transform;
		this.SetupSnow(characterModel.transform);
	}

	public void SetStartParticlesActive()
	{
		this.flypackParticleStart.gameObject.SetActive(true);
		this.flypackParticleStart.enabled = true;
		this.flypackParticleIng.gameObject.SetActive(false);
		this.flypackParticleEnd.gameObject.SetActive(false);
	}

	public void SetIngParticlesActive()
	{
		this.flypackParticleIng.gameObject.SetActive(true);
		this.flypackParticleIng.enabled = true;
		this.flypackParticleStart.gameObject.SetActive(false);
		this.flypackParticleEnd.gameObject.SetActive(false);
	}

	public void SetEndParticlesActive()
	{
		this.flypackParticleEnd.gameObject.SetActive(true);
		this.flypackParticleEnd.enabled = true;
		this.flypackParticleStart.gameObject.SetActive(false);
		this.flypackParticleIng.gameObject.SetActive(false);
	}

	public void SetSpeedupActive()
	{
		this.speedup.SetActive(true);
		this.speedupFollow.gameObject.SetActive(true);
		this.speedupFollow.enabled = true;
	}

	public void SetSpeedupDeactive()
	{
		this.speedupFollow.gameObject.SetActive(false);
		this.speedup.SetActive(false);
		this.speedupFollow.enabled = false;
	}

	private void SetupSnow(Transform root)
	{
		this._snowTransform = this._currentSnow.gameObject.transform;
		this._snowOriginalPosition = this._snowTransform.position;
	}

	private void Update()
	{
		if (this._snowActive)
		{
			this._snowTransform.position = new Vector3(this._snowOriginalPosition.x, this._snowOriginalPosition.y, this._character.z + this._zOffset);
			if (this._character.IsInsideSubway && this.emissionModule.rateOverTimeMultiplier != 0f)
			{
				this.emissionModule.rateOverTimeMultiplier = 0f;
			}
			else if (!this._character.IsInsideSubway && this.emissionModule.rateOverTimeMultiplier != this._originalEmissionRate)
			{
				this.emissionModule.rateOverTimeMultiplier = this._originalEmissionRate;
			}
		}
		if (this._showStars)
		{
			this._stars.position = new Vector3(this._starsOriginalPosition.x, this._starsOriginalPosition.y, this._character.z + this._starsOriginalPosition.z);
		}
	}

	public GameObject FlypackParticles
	{
		get
		{
			return this.flypackParticles;
		}
	}

	[SerializeField]
	private GameObject flypackParticles;

	[SerializeField]
	private ParticleFollow flypackParticleStart;

	[SerializeField]
	private ParticleFollow flypackParticleIng;

	[SerializeField]
	private ParticleFollow flypackParticleEnd;

	[SerializeField]
	private GameObject speedup;

	[SerializeField]
	private ParticleFollow speedupFollow;

	[SerializeField]
	private bool _showSnow;

	[SerializeField]
	private ParticleSystem _snow;

	[SerializeField]
	private ParticleSystem _snowLow;

	[SerializeField]
	private bool _showStars;

	[SerializeField]
	private Transform _stars;

	[SerializeField]
	private float _zOffset;

	private Character _character;

	private ParticleSystem _currentSnow;

	private ParticleSystem.EmissionModule emissionModule;

	private Game _game;

	private float _originalEmissionRate;

	private bool _snowActive;

	private Vector3 _snowOriginalPosition;

	private Transform _snowTransform;

	private Vector3 _starsOriginalPosition;
}
