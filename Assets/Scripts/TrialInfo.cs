using System;
using UnityEngine;

[Serializable]
public class TrialInfo
{
	public TrialType type;

	public Characters.CharacterType characterType;

	public int characterThemeId;

	public Helmets.HelmType helmetType;

	public int days;

	public int aim;

	public int taskAim;

	public string icon;

	public AnimationClip idel;

	public AnimationClip alert;
}
