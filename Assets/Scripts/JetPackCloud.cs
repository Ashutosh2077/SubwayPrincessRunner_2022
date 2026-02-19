using System;
using UnityEngine;

public class JetPackCloud : MonoBehaviour
{
	private void Awake()
	{
		this.material = base.gameObject.GetComponent<Renderer>().material;
		this.material.mainTextureOffset = new Vector2(this.startOffset, 0f);
	}

	private void Update()
	{
		float x = (this.material.mainTextureOffset.x + Time.deltaTime * this.scrollSpeed) % 1f;
		this.material.mainTextureOffset = new Vector2(x, 0f);
	}

	private Material material;

	public float scrollSpeed = 0.5f;

	public float startOffset;
}
