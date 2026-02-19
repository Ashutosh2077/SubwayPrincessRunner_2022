using System;
using UnityEngine;

public class RightHandCollider : MonoBehaviour
{
	public event RightHandCollider.OnHitDelegate OnHi5Pressed;

	public event RightHandCollider.OnHitDelegate OnHi5Released;

	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	public void Enable()
	{
		base.gameObject.SetActive(true);
		base.transform.parent = this._characterModel.BoneRightHand;
		base.transform.localPosition = new Vector3(0.8f, 0f, 0f);
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
	}

	private void OnMouseDown()
	{
		if (this.OnHi5Pressed != null)
		{
			this.OnHi5Pressed();
		}
	}

	private void OnMouseUp()
	{
		if (this.OnHi5Released != null)
		{
			this.OnHi5Released();
		}
	}

	[SerializeField]
	private CharacterModel _characterModel;

	public delegate void OnHitDelegate();
}
