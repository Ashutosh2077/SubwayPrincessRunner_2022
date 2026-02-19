using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	public void CatchPlayer(Animation characterAnimation)
	{
		if (!this.game.IsInFlypackMode && !this.game.IsInSpringJumpMode && !this.game.IsInBoundJumpMode)
		{
			int num = this.debugCatchAnimationToPlay;
			this.audioSource.Stop();
			base.StopAllCoroutines();
			this.caught = true;
			List<int> list = new List<int>(this.caughtSets.Length);
			if (this.debugCatchAnimationToPlay < 0 || this.debugCatchAnimationToPlay >= this.caughtSets.Length)
			{
				int i = 0;
				int num2 = this.caughtSets.Length;
				while (i < num2)
				{
					if (characterAnimation[this.caughtSets[i].death.name].enabled)
					{
						list.Add(i);
					}
					i++;
				}
			}
			if (list.Count <= 0)
			{
				this.ShowEnemies(false);
				return;
			}
			num = UnityEngine.Random.Range(0, list.Count);
			this.guardAnimation.CrossFade(this.caughtSets[num].guard.name, 0.2f);
			float num3 = this.caughtSets[num].catchAvatarAnimationPlayOffset / 25f;
			if (this.OnCatchPlayer != null)
			{
				this.OnCatchPlayer(this.caughtSets[num].avatar.name, num3, this.caughtSets[num].waitTimeBeforeScreen);
			}
			base.StartCoroutine(myTween.To(num3, delegate(float t)
			{
				for (int j = 0; j < this.enemies.Length; j++)
				{
					Vector3 position = this.character.transform.position;
					this.enemies[j].position = Vector3.Lerp(this.enemies[j].position, position, t);
				}
			}));
		}
	}

	public void CatchUp()
	{
		this.CatchUp(this.catchUpDuration);
	}

	public void CatchUp(float duration)
	{
		if (!this.closeToCharacter)
		{
			float distanceFrom = this.distanceToCharacter;
			this.ShowEnemies(true);
			base.StopAllCoroutines();
			this.guardAnimation.Play(this.ReturnRandomAnimations(this.defaultAnimations.catchupGuard));
			this.guardAnimation.PlayQueued(this.ReturnRandomAnimations(this.defaultAnimations.runGuard));
			this.audioSource.timeSamples = UnityEngine.Random.Range(0, this.audioSource.timeSamples);
			this.audioSource.Play();
			this.audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.05f);
			base.StartCoroutine(myTween.To(duration, delegate(float t)
			{
				this.distanceToCharacter = Mathf.SmoothStep(distanceFrom, this.distanceToCharacterMin, t);
			}));
			base.StartCoroutine(myTween.To(duration, delegate(float t)
			{
				this.audioSource.volume = Mathf.SmoothStep(0f, this.guardProximityLoopVolume, t);
			}));
			this.closeToCharacter = true;
		}
	}

	private void HandleOnPauseChange(bool pause)
	{
		if (pause)
		{
			if (this.audioSource.isPlaying)
			{
				this.audioSource.Pause();
			}
			this.isPaused = true;
		}
		else
		{
			if (this.isPaused)
			{
				this.audioSource.Play();
			}
			this.isPaused = false;
		}
	}

	public void HitByTrainSequence()
	{
		this.audioSource.Stop();
		base.StartCoroutine(this.HitByTrainSequenceCoroutine());
	}

	public void FallIntoWater()
	{
		this.audioSource.Stop();
		base.StartCoroutine(this.FallIntoWaterCoroutine());
	}

	public IEnumerator HitByTrainSequenceCoroutine()
	{
		GameStats.Instance.guardHitScreen++;
		float catchUpTime = 0.2f;
		yield return base.StartCoroutine(myTween.To(catchUpTime, delegate(float t)
		{
			for (int i = 0; i < this.enemies.Length; i++)
			{
				this.enemies[i].position = Vector3.Lerp(this.enemies[i].position, this.character.transform.position, t);
			}
		}));
		yield return new WaitForSeconds(0.4f);
		this.guardAnimation.Play("Guard_death_moving");
		yield break;
	}

	public IEnumerator FallIntoWaterCoroutine()
	{
		GameStats.Instance.guardFallWater++;
		float catchUpTime = 0.5f;
		Vector3 endPos = this.characterTransform.position - Vector3.forward * 5f;
		yield return base.StartCoroutine(myTween.To(catchUpTime, delegate(float t)
		{
			for (int i = 0; i < this.enemies.Length; i++)
			{
				this.enemies[i].position = Vector3.Lerp(this.enemies[i].position, endPos, t);
			}
		}));
		this.guardAnimation.Play("FallIntoWater");
		yield break;
	}

	public void Initialize()
	{
		this.game = Game.Instance;
		this.characterController = Game.Charactercontroller;
		this.character = Character.Instance;
		this.characterRendering = CharacterRendering.Instance;
		this.characterTransform = this.character.transform;
		this.enemyRenderers = base.gameObject.GetComponentsInChildren<Renderer>();
		this.enemiesStartPos = new Vector3[this.enemies.Length];
		this.audioSource = base.GetComponent<AudioSource>();
		for (int i = 0; i < this.enemies.Length; i++)
		{
			this.enemiesStartPos[i] = this.enemies[i].localPosition;
		}
		this.x = new SmoothDampFloat(0f, this.xSmoothTime);
		this.audioSource.volume = this.guardProximityLoopVolume;
		this.game.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Combine(this.game.OnPauseChange, new Game.OnPauseChangeDelegate(this.HandleOnPauseChange));
		int j = 0;
		int num = this.caughtSets.Length;
		while (j < num)
		{
			this.SetupAvatarAnimationsStates(this.characterRendering.characterAnimation, this.caughtSets[j].avatar);
			this.SetupGuardAnimationsStates(this.guardAnimation, this.caughtSets[j].guard);
			j++;
		}
		List<AnimationClip> list = new List<AnimationClip>();
		foreach (object obj in this.guardAnimation)
		{
			AnimationState animationState = (AnimationState)obj;
			list.Add(animationState.clip);
		}
		this.InitializeClips(this.guardAnimation, list, this.defaultAnimations.jumpGuard);
		this.InitializeClips(this.guardAnimation, list, this.defaultAnimations.runGuard);
		this.InitializeClips(this.guardAnimation, list, this.defaultAnimations.introGuard);
		this.InitializeClips(this.guardAnimation, list, this.defaultAnimations.dodgeRigthGuard);
		this.InitializeClips(this.guardAnimation, list, this.defaultAnimations.dodgeLeftGuard);
		this.InitializeClips(this.guardAnimation, list, this.defaultAnimations.rollGuard);
		this.InitializeClips(this.guardAnimation, list, this.defaultAnimations.catchupGuard);
	}

	private void InitializeClips(Animation animationComponent, List<AnimationClip> addedClips, AnimationClip[] clips)
	{
		foreach (AnimationClip animationClip in clips)
		{
			if (!addedClips.Contains(animationClip))
			{
				addedClips.Add(animationClip);
				animationComponent.AddClip(animationClip, animationClip.name);
			}
		}
	}

	public void Jump(float delay)
	{
		if (this.distanceToCharacter <= this.distanceToCharacterMin)
		{
			TasksManager.Instance.PlayerDidThis(TaskTarget.GuardJump, 1, -1);
		}
		base.StartCoroutine(this.JumpCoroutine(delay));
	}

	private void JumpAnimation()
	{
		string animation = this.ReturnRandomAnimations(this.defaultAnimations.jumpGuard);
		this.guardAnimation.Play(animation);
		string animation2 = this.ReturnRandomAnimations(this.defaultAnimations.runGuard);
		this.guardAnimation.CrossFadeQueued(animation2, 0.2f);
	}

	private IEnumerator JumpCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.JumpAnimation();
		this.verticalSpeed = this.character.CalculateJumpVerticalSpeed() * 0.7f;
		yield break;
	}

	public void LateUpdate()
	{
		this.x.Target = this.characterTransform.position.x;
		this.x.Update();
		if (this.character.isStairing)
		{
			this.y = base.transform.position.y;
			this.lastGroundedSmooth = this.character.running.GetStairYAtZ(this.character.z - this.distanceToCharacter);
			if (this.y > this.lastGroundedSmooth)
			{
				this.verticalSpeed -= this.gravity * Time.deltaTime;
			}
			this.y += this.verticalSpeed * Time.deltaTime;
			this.y = Mathf.Max(this.y, this.lastGroundedSmooth);
		}
		else
		{
			this.lastGroundedSmooth = Mathf.SmoothDamp(this.lastGroundedSmooth, this.character.lastGroundedY, ref this.lastGroundedVelocity, this.lastGroundedSmoothTime);
			if (this.y > this.lastGroundedSmooth)
			{
				this.verticalSpeed -= this.gravity * Time.deltaTime;
			}
			if (float.IsNaN(this.lastGroundedSmooth))
			{
				this.lastGroundedSmooth = 0f;
			}
			this.y += this.verticalSpeed * Time.deltaTime;
			this.y = Mathf.Max(this.y, this.lastGroundedSmooth);
		}
		Vector3 position = this.characterTransform.position - Vector3.forward * this.distanceToCharacter;
		position.y = this.y;
		position.x = this.x.Value;
		base.transform.position = position;
	}

	public void MuteProximityLoop()
	{
		this.audioSource.Stop();
	}

	private void OnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (!this.caught)
		{
			if (this.characterController.isGrounded)
			{
				string text = (direction == Character.OnChangeTrackDirection.Left) ? this.ReturnRandomAnimations(this.defaultAnimations.dodgeLeftGuard) : this.ReturnRandomAnimations(this.defaultAnimations.dodgeRigthGuard);
				this.guardAnimation[text].speed = this.game.NormalizedGameSpeed;
				this.guardAnimation.CrossFade(text, 0.1f);
			}
			if (!this.character.IsJumping)
			{
				string animation = this.ReturnRandomAnimations(this.defaultAnimations.runGuard);
				this.guardAnimation.CrossFadeQueued(animation, 0.1f);
			}
		}
	}

	public void OnDisable()
	{
		this.character.OnJump -= this.OnJump;
		this.character.OnRollGuard -= this.OnRoll;
		this.character.OnRoll -= this.OnRollNoAnimation;
		this.character.OnChangeTrack += this.OnChangeTrack;
	}

	public void OnEnable()
	{
		this.lastGroundedSmooth = this.character.lastGroundedY;
		this.lastGroundedVelocity = 0f;
		this.y = this.character.lastGroundedY;
		this.x.Value = this.character.transform.position.x;
		this.distanceToCharacter = this.distanceToCharacterMin;
		this.closeToCharacter = true;
		this.verticalSpeed = 0f;
		this.character.OnJump += this.OnJump;
		this.character.OnRollGuard += this.OnRoll;
		this.character.OnRoll += this.OnRollNoAnimation;
		this.character.OnChangeTrack += this.OnChangeTrack;
	}

	private void OnJump()
	{
		this.Jump(this.distanceToCharacter / this.game.currentSpeed);
	}

	private void OnRoll()
	{
		string text = this.ReturnRandomAnimations(this.defaultAnimations.rollGuard);
		this.guardAnimation[text].time = 0f;
		this.guardAnimation[text].speed = 1f;
		this.guardAnimation.CrossFade(text, 0.1f);
		string animation = this.ReturnRandomAnimations(this.defaultAnimations.runGuard);
		this.guardAnimation.CrossFadeQueued(animation, 0.2f);
	}

	private void OnRollNoAnimation()
	{
		base.StartCoroutine(this.RollCoroutine(this.distanceToCharacter / this.game.currentSpeed));
	}

	public void PlayIntro()
	{
		this.game.topMenu.AddEnemyOnStopEvent(new Action(this.Run));
	}

	public void Run()
	{
		this.closeToCharacter = false;
		this.CatchUp();
		this.game.topMenu.RemoveEnemyOnStopEvent(new Action(this.Run));
	}

	public void ResetCatchUp()
	{
		this.ResetCatchUp(this.resetCatchUpDuration);
	}

	public void ResetCatchUp(float duration)
	{
		base.StartCoroutine(this.ResetCatchUpCoroutine(duration));
	}

	public IEnumerator ResetCatchUpCoroutine(float duration)
	{
		if (!this.closeToCharacter)
		{
			yield break;
		}
		float distanceFrom = this.distanceToCharacter;
		this.closeToCharacter = false;
		base.StartCoroutine(myTween.To(duration, delegate(float t)
		{
			this.distanceToCharacter = Mathf.SmoothStep(distanceFrom, this.distanceToCharacterMax, t);
		}));
		yield return base.StartCoroutine(myTween.To(duration * 2f, delegate(float t)
		{
			this.audioSource.volume = Mathf.SmoothStep(this.guardProximityLoopVolume, 0f, t);
		}));
		this.audioSource.Stop();
		if (!this.game.isDead)
		{
			this.ShowEnemies(false);
		}
		yield break;
	}

	public void ResetModelRootPosition()
	{
		for (int i = 0; i < this.enemies.Length; i++)
		{
			this.enemies[i].localPosition = this.enemiesStartPos[i];
			this.enemies[i].localRotation = Quaternion.identity;
		}
	}

	public void Restart(bool closeToCharacter)
	{
		base.StopAllCoroutines();
		this.closeToCharacter = closeToCharacter;
		this.distanceToCharacter = (closeToCharacter ? this.distanceToCharacterMin : this.distanceToCharacterMax);
	}

	private string ReturnRandomAnimations(AnimationClip[] guardAnimations)
	{
		string result = string.Empty;
		if (guardAnimations != null)
		{
			result = guardAnimations[UnityEngine.Random.Range(0, guardAnimations.Length)].name;
		}
		return result;
	}

	private IEnumerator RollCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.verticalSpeed = -this.character.CalculateJumpVerticalSpeed();
		yield break;
	}

	private void SetupAvatarAnimationsStates(Animation animation, AnimationClip animationClip)
	{
		if (animation.GetClip(animationClip.name) == null)
		{
			animation.AddClip(animationClip, animationClip.name);
		}
		animation[animationClip.name].enabled = false;
		animation[animationClip.name].layer = 4;
	}

	private void SetupGuardAnimationsStates(Animation animation, AnimationClip animationClip)
	{
		if (animation.GetClip(animationClip.name) == null)
		{
			animation.AddClip(animationClip, animationClip.name);
		}
	}

	public void ShowEnemies(bool vis)
	{
		this.isShowing = vis;
		this.caught = false;
		int i = 0;
		int num = this.enemyRenderers.Length;
		while (i < num)
		{
			this.enemyRenderers[i].gameObject.SetActive(vis);
			i++;
		}
	}

	public static Boss Instance
	{
		get
		{
			if (Boss.instance == null)
			{
				Boss.instance = (UnityEngine.Object.FindObjectOfType(typeof(Boss)) as Boss);
			}
			return Boss.instance;
		}
	}

	public Boss.DefaultAnimations defaultAnimations;

	public float distanceToCharacterMin = 10f;

	public float distanceToCharacterMax = 50f;

	public float catchUpDuration = 0.7f;

	public float resetCatchUpDuration = 1.5f;

	public float lastGroundedSmoothTime = 0.3f;

	public float xSmoothTime = 0.1f;

	public float gravity = 200f;

	public bool isShowing;

	public Animation guardAnimation;

	public Boss.CatchAnimationSet[] caughtSets;

	public int debugCatchAnimationToPlay = -1;

	public Transform[] enemies;

	public Boss.OnCatchPlayerDelegate OnCatchPlayer;

	public float guardProximityLoopVolume = 0.9f;

	private AudioSource audioSource;

	private bool caught;

	private Character character;

	private CharacterController characterController;

	private CharacterRendering characterRendering;

	private Transform characterTransform;

	private bool closeToCharacter;

	private float distanceToCharacter;

	private Vector3[] enemiesStartPos;

	private Renderer[] enemyRenderers;

	private Game game;

	private static Boss instance;

	private bool isPaused = true;

	private float lastGroundedSmooth;

	private float lastGroundedVelocity;

	private float verticalSpeed;

	private SmoothDampFloat x;

	private float y;

	[Serializable]
	public class CatchAnimationSet
	{
		public AnimationClip death;

		public AnimationClip avatar;

		public AnimationClip guard;

		public float catchAvatarAnimationPlayOffset;

		public float waitTimeBeforeScreen;
	}

	[Serializable]
	public class DefaultAnimations
	{
		public AnimationClip[] introGuard;

		public AnimationClip[] runGuard;

		public AnimationClip[] jumpGuard;

		public AnimationClip[] dodgeLeftGuard;

		public AnimationClip[] dodgeRigthGuard;

		public AnimationClip[] rollGuard;

		public AnimationClip[] catchupGuard;
	}

	public delegate void OnCatchPlayerDelegate(string currentChartacterCatch, float catchUpTime, float waitTimeBeforeScreen);
}
