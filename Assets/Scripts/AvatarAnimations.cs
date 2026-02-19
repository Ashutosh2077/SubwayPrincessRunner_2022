using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAnimations : MonoBehaviour
{
	private Animation FindAnimationInParent(GameObject current)
	{
		Animation component = current.GetComponent<Animation>();
		if (component != null)
		{
			return component;
		}
		if (current.transform.parent != null)
		{
			return this.FindAnimationInParent(current.transform.parent.gameObject);
		}
		return null;
	}

	private IEnumerator Play()
	{
		CharacterModel model = base.transform.parent.parent.parent.parent.GetComponent<CharacterModel>();
		if (model.PlayBall.gameObject.activeSelf)
		{
			model.PlayBall.gameObject.SetActive(false);
		}
		if (model.Flower.gameObject.activeSelf)
		{
			model.Flower.gameObject.SetActive(false);
		}
		if (model.Heart.gameObject.activeSelf)
		{
			model.Heart.gameObject.SetActive(false);
		}
		if (this.useStartClip)
		{
			this.Target.Play(this.startClipName);
			if (this.startClipName.Equals("SW_B_01_00_alert") || this.startClipName.Equals("SW_B_01_00_idle") || this.startClipName.Equals("SW_B_01_00_idle_2"))
			{
				model.PlayBall.gameObject.SetActive(true);
				model.PlayBall[this.startClipName + "_q"].time = 0f;
				model.PlayBall.Play(this.startClipName + "_q");
			}
			if (this.startClipName.Equals("SW_B_03_00_idle2"))
			{
				model.Flower.gameObject.SetActive(true);
				model.Flower[this.startClipName + "_hua"].time = 0f;
				model.Flower.Play(this.startClipName + "_hua");
			}
			if (this.startClipName.Equals("SW_B_03_00_idle"))
			{
				model.Heart.gameObject.SetActive(true);
				model.Heart[this.startClipName + "_heart"].time = 0f;
				model.Heart.Play(this.startClipName + "_heart");
			}
			this.nextTime = this.startClip.length;
			while (this.nextTime > 0f)
			{
				this.nextTime -= Time.deltaTime;
				yield return null;
			}
			this.useStartClip = false;
			if (model.PlayBall.gameObject.activeSelf)
			{
				model.PlayBall.gameObject.SetActive(false);
			}
			if (model.Flower.gameObject.activeSelf)
			{
				model.Flower.gameObject.SetActive(false);
			}
			if (model.Heart.gameObject.activeSelf)
			{
				model.Heart.gameObject.SetActive(false);
			}
		}
		if (this.eyeAnimation && !this.eyeAnimation.IsAnimating())
		{
			this.eyeAnimation.StartAnimatingEyes();
		}
		if (!this.usePopupIdles && !this.useUnlockIdles)
		{
			this.Target.Play(this.breathName);
			if (this.breathName.Equals("SW_B_01_00_alert") || this.breathName.Equals("SW_B_01_00_idle") || this.breathName.Equals("SW_B_01_00_idle_2"))
			{
				model.PlayBall.gameObject.SetActive(true);
				model.PlayBall[this.breathName + "_q"].time = 0f;
				model.PlayBall.Play(this.breathName + "_q");
			}
			if (this.breathName.Equals("SW_B_03_00_idle2"))
			{
				model.Flower.gameObject.SetActive(true);
				model.Flower[this.breathName + "_hua"].time = 0f;
				model.Flower.Play(this.breathName + "_hua");
			}
			if (this.breathName.Equals("SW_B_03_00_idle"))
			{
				model.Heart.gameObject.SetActive(true);
				model.Heart[this.breathName + "_heart"].time = 0f;
				model.Heart.Play(this.breathName + "_heart");
			}
			this.nextTime = this.Breath.length;
			while (this.nextTime > 0f)
			{
				this.nextTime -= Time.deltaTime;
				yield return null;
			}
			if (model.PlayBall.gameObject.activeSelf)
			{
				model.PlayBall.gameObject.SetActive(false);
			}
			if (model.Flower.gameObject.activeSelf)
			{
				model.Flower.gameObject.SetActive(false);
			}
			if (model.Heart.gameObject.activeSelf)
			{
				model.Heart.gameObject.SetActive(false);
			}
		}
		while (this.PlayIdleAnimations)
		{
			List<AnimationClip> possibleClips;
			if (this.usePopupIdles)
			{
				possibleClips = this.IdlesPopup.FindAll(new Predicate<AnimationClip>(this.IsNotNull));
			}
			else if (this.useUnlockIdles)
			{
				possibleClips = this.Idles.FindAll(new Predicate<AnimationClip>(this.IsNotNull));
			}
			else
			{
				possibleClips = this.Idles.FindAll(new Predicate<AnimationClip>(this.IsNotNull));
			}
			AnimationClip selectedClip = null;
			if (possibleClips.Count > 0)
			{
				selectedClip = possibleClips[UnityEngine.Random.Range(0, possibleClips.Count)];
			}
			string aniName = selectedClip.name;
			this.Target.Play(aniName);
			this.nextTime = selectedClip.length;
			if (aniName.Equals("SW_B_01_00_alert") || aniName.Equals("SW_B_01_00_idle") || aniName.Equals("SW_B_01_00_idle_2"))
			{
				model.PlayBall.gameObject.SetActive(true);
				model.PlayBall[aniName + "_q"].time = 0f;
				model.PlayBall.Play(aniName + "_q");
			}
			if (aniName.Equals("SW_B_03_00_idle2"))
			{
				model.Flower.gameObject.SetActive(true);
				model.Flower[aniName + "_hua"].time = 0f;
				model.Flower.Play(aniName + "_hua");
			}
			if (aniName.Equals("SW_B_03_00_idle"))
			{
				model.Heart.gameObject.SetActive(true);
				model.Heart[aniName + "_heart"].time = 0f;
				model.Heart.Play(aniName + "_heart");
			}
			while (this.nextTime > 0f)
			{
				this.nextTime -= Time.deltaTime;
				yield return null;
			}
			if (model.PlayBall.gameObject.activeSelf)
			{
				model.PlayBall.gameObject.SetActive(false);
			}
			if (model.Flower.gameObject.activeSelf)
			{
				model.Flower.gameObject.SetActive(false);
			}
			if (model.Heart.gameObject.activeSelf)
			{
				model.Heart.gameObject.SetActive(false);
			}
			if (!this.usePopupIdles && !this.useUnlockIdles)
			{
				this.Target.Play(this.breathName);
				if (this.breathName.Equals("SW_B_01_00_alert") || this.breathName.Equals("SW_B_01_00_idle") || this.breathName.Equals("SW_B_01_00_idle_2"))
				{
					model.PlayBall.gameObject.SetActive(true);
					model.PlayBall[this.breathName + "_q"].time = 0f;
					model.PlayBall.Play(this.breathName + "_q");
				}
				if (this.breathName.Equals("SW_B_03_00_idle2"))
				{
					model.Flower.gameObject.SetActive(true);
					model.Flower[this.breathName + "_hua"].time = 0f;
					model.Flower.Play(this.breathName + "_hua");
				}
				if (this.breathName.Equals("SW_B_03_00_idle"))
				{
					model.Heart.gameObject.SetActive(true);
					model.Heart[this.breathName + "_heart"].time = 0f;
					model.Heart.Play(this.breathName + "_heart");
				}
				this.nextTime = this.Breath.length;
				while (this.nextTime > 0f)
				{
					this.nextTime -= Time.deltaTime;
					yield return null;
				}
				if (model.PlayBall.gameObject.activeSelf)
				{
					model.PlayBall.gameObject.SetActive(false);
				}
				if (model.Flower.gameObject.activeSelf)
				{
					model.Flower.gameObject.SetActive(false);
				}
				if (model.Heart.gameObject.activeSelf)
				{
					model.Heart.gameObject.SetActive(false);
				}
			}
		}
		this.routine = null;
		yield break;
	}

	private void Awake()
	{
		this.eyeAnimation = base.GetComponent<AvatarEyeAnimation>();
		this.Target = this.FindAnimationInParent(base.gameObject);
		this.breathName = ((!(this.Breath == null)) ? this.Breath.name : string.Empty);
		this.startClipName = ((!(this.startClip == null)) ? this.startClip.name : string.Empty);
		this.IdlesPopupName = new List<string>(this.IdlesPopup.Count);
		int i = 0;
		int count = this.IdlesPopup.Count;
		while (i < count)
		{
			this.IdlesPopupName.Add((!(this.IdlesPopup[i] == null)) ? this.IdlesPopup[i].name : string.Empty);
			i++;
		}
		this.IdlesName = new List<string>(this.Idles.Count);
		int j = 0;
		int count2 = this.Idles.Count;
		while (j < count2)
		{
			this.IdlesName.Add((!(this.Idles[j] == null)) ? this.Idles[j].name : string.Empty);
			j++;
		}
		this.UnlockPopupName = new List<string>(this.UnlockPopup.Count);
		int k = 0;
		int count3 = this.UnlockPopup.Count;
		while (k < count3)
		{
			this.UnlockPopupName.Add((!(this.UnlockPopup[k] == null)) ? this.UnlockPopup[k].name : string.Empty);
			k++;
		}
		if (this.Target == null)
		{
			UnityEngine.Debug.LogError("AvatarAnimations: No animation component for avatar animations", this);
		}
		else if (this.PlayIdleAnimations)
		{
			if (this.usePopupIdles)
			{
				this.StartIdlePopupAnimations();
			}
			else if (this.useUnlockIdles)
			{
				this.StartUnlockAnimations();
			}
			else
			{
				this.StartIdleAnimations();
			}
		}
	}

	public void StartIdleAnimations()
	{
		this.PlayIdleAnimations = true;
		this.Paused = false;
		this.usePopupIdles = false;
		this.useUnlockIdles = false;
		this.Target.AddClip(this.Breath, this.breathName);
		if (this.startClip != null)
		{
			this.useStartClip = true;
			this.Target.AddClip(this.startClip, this.startClipName);
		}
		else
		{
			this.useStartClip = false;
		}
		int i = 0;
		int count = this.Idles.Count;
		while (i < count)
		{
			this.Target.AddClip(this.Idles[i], this.IdlesName[i]);
			i++;
		}
		if (this.eyeAnimation && !this.eyeAnimation.IsAnimating())
		{
			this.eyeAnimation.StartAnimatingEyes();
		}
		this.routine = this.Play();
		this.routine.MoveNext();
	}

	public void StartIdlePopupAnimations()
	{
		this.PlayIdleAnimations = true;
		this.Paused = false;
		this.usePopupIdles = true;
		this.useUnlockIdles = false;
		this.useStartClip = false;
		this.Target.AddClip(this.Breath, this.breathName);
		int i = 0;
		int count = this.IdlesPopup.Count;
		while (i < count)
		{
			this.Target.AddClip(this.IdlesPopup[i], this.IdlesPopupName[i]);
			i++;
		}
		if (this.eyeAnimation && !this.eyeAnimation.IsAnimating())
		{
			this.eyeAnimation.StartAnimatingEyes();
		}
		this.routine = this.Play();
		this.routine.MoveNext();
	}

	public void StartUnlockAnimations()
	{
		this.PlayIdleAnimations = true;
		this.Paused = false;
		this.usePopupIdles = false;
		this.useUnlockIdles = true;
		this.useStartClip = false;
		int i = 0;
		int count = this.UnlockPopup.Count;
		while (i < count)
		{
			this.Target.AddClip(this.UnlockPopup[i], this.UnlockPopupName[i]);
			i++;
		}
		if (this.eyeAnimation && !this.eyeAnimation.IsAnimating())
		{
			this.eyeAnimation.StartAnimatingEyes();
		}
		this.routine = this.Play();
		this.routine.MoveNext();
	}

	public void StopIdleAnimations()
	{
		this.PlayIdleAnimations = false;
		if (this.Target[this.breathName] != null)
		{
			this.Target.RemoveClip(this.Breath);
		}
		if (this.usePopupIdles)
		{
			int i = 0;
			int count = this.IdlesPopup.Count;
			while (i < count)
			{
				foreach (object obj in this.Target)
				{
					AnimationState animationState = (AnimationState)obj;
					if (animationState.clip == this.IdlesPopup[i])
					{
						this.Target.RemoveClip(this.IdlesPopup[i]);
					}
				}
				i++;
			}
		}
		else if (this.useUnlockIdles)
		{
			int j = 0;
			int count2 = this.UnlockPopup.Count;
			while (j < count2)
			{
				foreach (object obj2 in this.Target)
				{
					AnimationState animationState2 = (AnimationState)obj2;
					if (animationState2.clip == this.UnlockPopup[j])
					{
						this.Target.RemoveClip(this.UnlockPopup[j]);
					}
				}
				j++;
			}
		}
		else
		{
			if (this.startClip != null && this.Target[this.startClipName] != null)
			{
				this.Target.RemoveClip(this.startClip);
			}
			int k = 0;
			int count3 = this.Idles.Count;
			while (k < count3)
			{
				foreach (object obj3 in this.Target)
				{
					AnimationState animationState3 = (AnimationState)obj3;
					if (animationState3.clip == this.Idles[k])
					{
						this.Target.RemoveClip(this.Idles[k]);
					}
				}
				k++;
			}
		}
		if (this.eyeAnimation && this.eyeAnimation.IsAnimating())
		{
			this.eyeAnimation.StopAnimatingEyes();
		}
		this.routine = null;
	}

	private void Update()
	{
		if (this.PlayIdleAnimations && this.routine != null && !this.Paused)
		{
			this.routine.MoveNext();
			if (this.useUnlockIdles && this.Target != null)
			{
				this.Target.transform.Rotate(0f, 50f * Time.deltaTime, 0f);
			}
		}
	}

	private bool IsNotNull(AnimationClip a)
	{
		return a != null;
	}

	public Animation Target;

	public bool PlayIdleAnimations;

	public AnimationClip Breath;

	public AnimationClip startClip;

	public List<AnimationClip> Idles;

	public List<AnimationClip> IdlesPopup;

	public List<AnimationClip> UnlockPopup;

	public bool Paused;

	private IEnumerator routine;

	private float nextTime;

	private bool usePopupIdles;

	private bool useStartClip;

	private bool useUnlockIdles;

	private AvatarEyeAnimation eyeAnimation;

	private string breathName;

	private string startClipName;

	private List<string> IdlesName;

	private List<string> IdlesPopupName;

	private List<string> UnlockPopupName;
}
