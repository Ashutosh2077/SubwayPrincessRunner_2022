using System;
using UnityEngine;

public class SetParticleZVelo : MonoBehaviour
{
	private void Awake()
	{
		this._particleSystem = base.gameObject.GetComponent<ParticleSystem>();
		if (this._particleSystem == null)
		{
			base.enabled = false;
			UnityEngine.Object.Destroy(this);
			return;
		}
		this.mainModule = this._particleSystem.main;
		this._game = Game.Instance;
	}

	private void Update()
	{
		base.transform.rotation = Quaternion.identity;
		this.mainModule.startSpeed = this._game.currentSpeed * this._speedMultiplier;
	}

	private Game _game;

	private ParticleSystem _particleSystem;

	private ParticleSystem.MainModule mainModule;

	[SerializeField]
	private float _speedMultiplier = 1f;
}
