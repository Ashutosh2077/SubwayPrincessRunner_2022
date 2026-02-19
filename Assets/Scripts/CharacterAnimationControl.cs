using System;
using UnityEngine;

public class CharacterAnimationControl : MonoBehaviour
{
	private void Awake()
	{
		this.animationComponent = base.gameObject.GetComponent<Animation>();
	}

	public void Play(string name)
	{
		this.animationComponent.Play(name);
	}

	public void Speed(string name, float speed)
	{
		this.animationComponent[name].speed = speed;
	}

	private Animation animationComponent;
}
