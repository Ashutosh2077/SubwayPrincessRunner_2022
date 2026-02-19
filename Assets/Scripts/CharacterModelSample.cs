using System;
using UnityEngine;

public class CharacterModelSample : MonoBehaviour, ICharacterModel
{
	public Transform BoneHead
	{
		get
		{
			return this._boneHead;
		}
	}

	public Transform BoneHelmet
	{
		get
		{
			return this._boneHelmet;
		}
	}

	public Transform BoneRightHand
	{
		get
		{
			return this._boneRightHand;
		}
	}

	[SerializeField]
	private Transform _boneHead;

	[SerializeField]
	private Transform _boneRightHand;

	[SerializeField]
	private Transform _boneHelmet;

	public CharacterCustomization.CustomControl[] Customs;
}
