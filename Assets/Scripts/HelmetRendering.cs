using System;
using System.Collections.Generic;
using UnityEngine;

public class HelmetRendering : MonoBehaviour
{
	private void AddClipsToAnimationComp(Animation animation, AnimationClip clip, List<AnimationClip> addedClipsList)
	{
		if (clip != null)
		{
			if (addedClipsList != null)
			{
				if (!addedClipsList.Contains(clip))
				{
					addedClipsList.Add(clip);
					animation.AddClip(clip, clip.name);
				}
			}
			else
			{
				animation.AddClip(clip, clip.name);
			}
		}
	}

	private string[] GetNamesAddAnimationClips(AnimationPair[] pairs, Animation avaterAnimation, Animation helmetAnimation, List<AnimationClip> addedClipsList)
	{
		string[] array = new string[pairs.Length];
		for (int i = 0; i < array.Length; i++)
		{
			AnimationPair animationPair = pairs[i];
			AnimationClip characterAnimationClip = animationPair.characterAnimationClip;
			if (avaterAnimation != null)
			{
				this.AddClipsToAnimationComp(avaterAnimation, characterAnimationClip, addedClipsList);
			}
			if (helmetAnimation != null && animationPair.helmetAnimationClip != null)
			{
				this.AddClipsToAnimationComp(helmetAnimation, animationPair.helmetAnimationClip, null);
			}
			if (characterAnimationClip == null)
			{
				throw new Exception("character cannot be null.");
			}
			array[i] = characterAnimationClip.name;
		}
		return array;
	}

	public void Initialize(Animation avatarAnimation, Animation helmetAnimation, HelmetRendering.AnimationList defaultAbilityAnimationLists, List<AnimationClip> addedClipsList)
	{
		CharacterRendering instance = CharacterRendering.Instance;
		if (this.jumpAnimations.Length != this.hangtimeAnimations.Length)
		{
			UnityEngine.Debug.LogError("Lists of helmet jumps and helmet hangtime animations does not have the same length", this);
		}
		instance.animations.RUN = this.GetNamesAddAnimationClips((this.runAnimations.Length == 0) ? defaultAbilityAnimationLists.runAnimations : this.runAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.SUPERRUN = this.GetNamesAddAnimationClips((this.superRunAnimations.Length == 0) ? defaultAbilityAnimationLists.superRunAnimations : this.superRunAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.LAND = this.GetNamesAddAnimationClips((this.landAnimations.Length == 0) ? defaultAbilityAnimationLists.landAnimations : this.landAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.JUMP = this.GetNamesAddAnimationClips((this.jumpAnimations.Length == 0) ? defaultAbilityAnimationLists.jumpAnimations : this.jumpAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.HANGTIME = this.GetNamesAddAnimationClips((this.hangtimeAnimations.Length == 0) ? defaultAbilityAnimationLists.hangtimeAnimations : this.hangtimeAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.ROLL = this.GetNamesAddAnimationClips((this.rollAnimations.Length == 0) ? defaultAbilityAnimationLists.rollAnimations : this.rollAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.DODGE_LEFT = this.GetNamesAddAnimationClips((this.dodgeLeftAnimations.Length == 0) ? defaultAbilityAnimationLists.dodgeLeftAnimations : this.dodgeLeftAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.DODGE_RIGHT = this.GetNamesAddAnimationClips((this.dodgeRightAnimations.Length == 0) ? defaultAbilityAnimationLists.dodgeRightAnimations : this.dodgeRightAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.GRIND = this.GetNamesAddAnimationClips((this.grindAnimations.Length == 0) ? defaultAbilityAnimationLists.grindAnimations : this.grindAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		this.GetNamesAddAnimationClips((this.grindLandAnimations.Length == 0) ? defaultAbilityAnimationLists.grindLandAnimations : this.grindLandAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		instance.animations.GET_ON = this.GetNamesAddAnimationClips((this.getOnHelmAnimations.Length == 0) ? defaultAbilityAnimationLists.getOnHelmAnimations : this.getOnHelmAnimations, avatarAnimation, helmetAnimation, addedClipsList);
		if (helmetAnimation != null)
		{
			helmetAnimation.AddClip(this.defaultHelmetAnimation, this.defaultHelmetAnimation.name);
		}
		if (this.defaultHelmetAnimation != null)
		{
			instance.animations.DEFAULT_ANIMATION = this.defaultHelmetAnimation.name;
		}
	}

	[OptionalField]
	public AnimationClip defaultHelmetAnimation;

	public AnimationPair[] runAnimations;

	public AnimationPair[] superRunAnimations;

	public AnimationPair[] landAnimations;

	public AnimationPair[] jumpAnimations;

	public AnimationPair[] hangtimeAnimations;

	public AnimationPair[] rollAnimations;

	public AnimationPair[] dodgeLeftAnimations;

	public AnimationPair[] dodgeRightAnimations;

	public AnimationPair[] grindAnimations;

	public AnimationPair[] grindLandAnimations;

	public AnimationPair[] getOnHelmAnimations;

	[Serializable]
	public class AnimationList
	{
		public AnimationClip defaultHelmetAnimation;

		public AnimationPair[] runAnimations;

		public AnimationPair[] superRunAnimations;

		public AnimationPair[] landAnimations;

		public AnimationPair[] jumpAnimations;

		public AnimationPair[] hangtimeAnimations;

		public AnimationPair[] rollAnimations;

		public AnimationPair[] dodgeLeftAnimations;

		public AnimationPair[] dodgeRightAnimations;

		public AnimationPair[] grindAnimations;

		public AnimationPair[] grindLandAnimations;

		public AnimationPair[] getOnHelmAnimations;
	}
}
