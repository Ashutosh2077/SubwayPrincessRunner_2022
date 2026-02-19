using System;
using System.Collections.Generic;
using UnityEngine;

public class RaftRendering : MonoBehaviour
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

	private string[] GetNamesAddAnimationClips(AnimationPair[] pairs, Animation avaterAnimation, Animation raftAnimation, List<AnimationClip> addedClipsList)
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
			if (raftAnimation != null && animationPair.helmetAnimationClip != null)
			{
				this.AddClipsToAnimationComp(raftAnimation, animationPair.helmetAnimationClip, null);
			}
			if (characterAnimationClip == null)
			{
				throw new Exception("character cannot be null.");
			}
			array[i] = characterAnimationClip.name;
		}
		return array;
	}

	public void Initialize(Animation avatarAnimation, Animation raftAnimation, List<AnimationClip> addedClipsList)
	{
		CharacterRendering instance = CharacterRendering.Instance;
		if (this.jumpAnimations.Length != this.hangtimeAnimations.Length)
		{
			UnityEngine.Debug.LogError("Lists of raft jump and hangtime animations does not have the same length", this);
		}
		instance.animations.RUN = this.GetNamesAddAnimationClips(this.runAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.LAND = this.GetNamesAddAnimationClips(this.landAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.JUMP = this.GetNamesAddAnimationClips(this.jumpAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.HANGTIME = this.GetNamesAddAnimationClips(this.hangtimeAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.ROLL = this.GetNamesAddAnimationClips(this.rollAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.DODGE_LEFT = this.GetNamesAddAnimationClips(this.dodgeLeftAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.DODGE_RIGHT = this.GetNamesAddAnimationClips(this.dodgeRightAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.GRIND = this.GetNamesAddAnimationClips(this.grindAnimations, avatarAnimation, raftAnimation, addedClipsList);
		this.GetNamesAddAnimationClips(this.grindLandAnimations, avatarAnimation, raftAnimation, addedClipsList);
		instance.animations.GET_ON = this.GetNamesAddAnimationClips(this.getOnRaftAnimations, avatarAnimation, raftAnimation, addedClipsList);
		if (raftAnimation != null)
		{
			raftAnimation.AddClip(this.defaultAnimation, this.defaultAnimation.name);
		}
		if (this.defaultAnimation != null)
		{
			instance.animations.DEFAULT_ANIMATION = this.defaultAnimation.name;
		}
	}

	public AnimationClip defaultAnimation;

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

	public AnimationPair[] getOnRaftAnimations;
}
