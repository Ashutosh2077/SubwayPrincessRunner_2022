using System;
using UnityEngine;

public interface ICharacterModel
{
	Transform BoneHead { get; }

	Transform BoneHelmet { get; }

	Transform BoneRightHand { get; }
}
