using System;
using UnityEngine;

public class AnimationEventDriver : MonoBehaviour
{
	public void PlayParticleSystem()
	{
		this.ps.Play();
	}

	[SerializeField]
	private ParticleSystem ps;
}
