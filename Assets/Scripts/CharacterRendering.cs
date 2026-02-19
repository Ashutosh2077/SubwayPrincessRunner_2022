using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRendering : MonoBehaviour
{
	public event CharacterRendering.CharacterModelInitializedDelegate CharacterModelInitialized;

	private string GetHelmetAnimationName(string animationName)
	{
		if (this.helmetAnimation.GetClip(animationName) != null)
		{
			return animationName;
		}
		return this.animations.DEFAULT_ANIMATION;
	}

	private string GetRaftAnimationName(string animationName)
	{
		if (this.raftAnimation.GetClip(animationName) != null)
		{
			return animationName;
		}
		return this.animations.DEFAULT_ANIMATION;
	}

	private void SpringJumpOnFlyAheadStart()
	{
		this.effectInitRot = this.characterRenderingEffects.FlypackParticles.transform.rotation.eulerAngles;
		this.effectInitScale = this.characterRenderingEffects.FlypackParticles.transform.localScale;
	}

	public void Initialize()
	{
		this.InitializeCharacterModel();
		this.InitializeAnimations();
		this.waterBoards = new bool[3];
		this.isOnWaterBoard = false;
		this.game = Game.Instance;
		this.character = Character.Instance;
		this.character.characterModel = this.characterModel;
		this.characterController = Game.Charactercontroller;
		this.superShoes = SuperShoes.Instance;
		this.helmet = Helmet.Instance;
		this.raft = Raft.Instance;
		this.flypack = Flypack.Instance;
		this.springJump = SpringJump.Instance;
		this.boundJump = BoundJump.Instance;
		this.wallWalking = WallWalking.Instance;
		this.boss = Boss.Instance;
		this.revive = Revive.Instance;
		this.InitializeCharacterRenderingEffects();
		this.character.OnChangeTrack += this.OnChangeTrack;
		this.character.OnStumble += this.OnStumble;
		this.character.OnTutorialMoveBackToCheckPoint += this.OnTutorialMoveBackToCheckPoint;
		this.character.OnTutorialStartFromCheckPoint += this.OnTutorialStartFromCheckPoint;
		this.character.OnHitByTrain += this.OnHitByTrain;
		this.character.OnFallIntoWater += this.OnFallIntoWater;
		this.character.OnJump += this.OnJump;
		this.character.OnJumpIfHitByTrain += this.OnJump;
		this.character.OnRoll += this.OnRoll;
		this.character.OnLanding += this.OnLanding;
		this.character.OnHangtime += this.OnHangtime;
		this.character.IsGrounded.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(this.character.IsGrounded.OnChange, new Variable<bool>.OnChangeDelegate(this.OnChangeIsGrounded));
		this.character.OnHandleWaterBoard += this.OnHandleWaterBoard;
		WaterBoard.GetOnWaterBoard = (Action<int>)Delegate.Combine(WaterBoard.GetOnWaterBoard, new Action<int>(this.OnGetOnWaterBoard));
		WaterBoard.GetOffWaterBoard = (Action<int>)Delegate.Combine(WaterBoard.GetOffWaterBoard, new Action<int>(this.OnGetOffWaterBoard));
		this.speedup = SpeedUp.Instance;
		SpeedUp speedUp = this.speedup;
		speedUp.OnStart = (SpeedUp.OnStartDelegate)Delegate.Combine(speedUp.OnStart, new SpeedUp.OnStartDelegate(this.OnSpeedupStart));
		SpeedUp speedUp2 = this.speedup;
		speedUp2.OnStop = (SpeedUp.OnStopDelegate)Delegate.Combine(speedUp2.OnStop, new SpeedUp.OnStopDelegate(this.OnSpeedupStop));
		this.game.IsInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(this.game.IsInGame.OnChange, new Variable<bool>.OnChangeDelegate(this.IsInGame_OnChange));
		this.game.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(this.game.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(this.OnStageMenuSequence));
		this.game.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(this.game.OnIntroRun, new Game.OnIntroRunDelegate(this.OnIntroRun));
		this.game.OnTurboHeadstartInput += this.HandleOnTurboHeadstart;
		this.game.OnGameStarted = (Action)Delegate.Combine(this.game.OnGameStarted, new Action(this.OnGameStart));
		this.helmet.OnSwitchToHelmet += this.OnSwitchToHelmet;
		this.helmet.OnEndHelmet += this.OnSwitchToRunning;
		this.helmet.OnJump += this.OnJump;
		this.helmet.OnRun += this.OnRun;
		this.raft.OnSwitchToRoft += this.OnSwitchToRaft;
		this.raft._OnEndRaft += this.OnEndRaft;
		this.flypack.OnStart = (Flypack.OnStartDelegate)Delegate.Combine(this.flypack.OnStart, new Flypack.OnStartDelegate(this.OnSwitchToFlypack));
		this.flypack.OnStop = (Flypack.OnStopDelegate)Delegate.Combine(this.flypack.OnStop, new Flypack.OnStopDelegate(this.FlypackOnStop));
		this.flypack.OnFlyAheadStart = (Flypack.OnFlyAheadStartDelegate)Delegate.Combine(this.flypack.OnFlyAheadStart, new Flypack.OnFlyAheadStartDelegate(this.SpringJumpOnFlyAheadStart));
		this.flypack.OnFlyAheadUpdate = (Flypack.OnFlyAheadUpdateDelegate)Delegate.Combine(this.flypack.OnFlyAheadUpdate, new Flypack.OnFlyAheadUpdateDelegate(this.FlypackOnFlyAheadUpdate));
		this.flypack.OnFlyAheadEnd = (Flypack.OnFlyAheadEndDelegate)Delegate.Combine(this.flypack.OnFlyAheadEnd, new Flypack.OnFlyAheadEndDelegate(this.FlypackOnFlyAheadEnd));
		this.flypack.OnHidTurboHeadstartButtons += this.OnHidTurboHeadstartButtons;
		this.superShoes.OnSwitchToSuperShoes += this.OnSwitchToSuperShoes;
		this.superShoes.SuperShoesOnStop += this.SuperShoesOnStop;
		this.springJump.OnStart = (SpringJump.OnStartDelegate)Delegate.Combine(this.springJump.OnStart, new SpringJump.OnStartDelegate(this.SpringJumpOnStart));
		this.springJump.OnHangtime = (SpringJump.OnStartDelegate)Delegate.Combine(this.springJump.OnHangtime, new SpringJump.OnStartDelegate(this.SpringJumpOnHangtime));
		this.springJump.OnStop = (SpringJump.OnStopDelegate)Delegate.Combine(this.springJump.OnStop, new SpringJump.OnStopDelegate(this.SpringJumpOnStop));
		this.wallWalking.OnJumpAheadStart = (WallWalking.OnJumpAheadStartDelegate)Delegate.Combine(this.wallWalking.OnJumpAheadStart, new WallWalking.OnJumpAheadStartDelegate(this.WallWalkingOnJumpAheadStart));
		this.wallWalking.OnJumpAheadEnd = (WallWalking.OnJumpAheadEndDelegate)Delegate.Combine(this.wallWalking.OnJumpAheadEnd, new WallWalking.OnJumpAheadEndDelegate(this.WallWalkingOnJumpAheadEnd));
		this.wallWalking.OnLeaveWall = (WallWalking.OnLeaveWallDelegate)Delegate.Combine(this.wallWalking.OnLeaveWall, new WallWalking.OnLeaveWallDelegate(this.OnLeaveWallRun));
		this.boundJump.OnStart = (BoundJump.OnStartDelegate)Delegate.Combine(this.boundJump.OnStart, new BoundJump.OnStartDelegate(this.SpringJumpOnStart));
		this.boundJump.OnStop = (BoundJump.OnStopDelegate)Delegate.Combine(this.boundJump.OnStop, new BoundJump.OnStopDelegate(this.SpringJumpOnStop));
		this.boss.OnCatchPlayer = (Boss.OnCatchPlayerDelegate)Delegate.Combine(this.boss.OnCatchPlayer, new Boss.OnCatchPlayerDelegate(this.OnCatchPlayer));
		this.revive.OnRevive += this.OnRevive;
		this.revive.OnSwitchToRunning += this.OnSwitchToRunning;
		this.defaultHelmetRendering = HelmetModelPreviewFactory.Instance.GetHelmetSelection(0).helmetPrefab.GetComponent<HelmetRendering>();
		this.defaultHelmetAnimationList = new HelmetRendering.AnimationList();
		this.defaultHelmetAnimationList.runAnimations = this.defaultHelmetRendering.runAnimations;
		this.defaultHelmetAnimationList.superRunAnimations = this.defaultHelmetRendering.superRunAnimations;
		this.defaultHelmetAnimationList.landAnimations = this.defaultHelmetRendering.landAnimations;
		this.defaultHelmetAnimationList.jumpAnimations = this.defaultHelmetRendering.jumpAnimations;
		this.defaultHelmetAnimationList.hangtimeAnimations = this.defaultHelmetRendering.hangtimeAnimations;
		this.defaultHelmetAnimationList.rollAnimations = this.defaultHelmetRendering.rollAnimations;
		this.defaultHelmetAnimationList.dodgeLeftAnimations = this.defaultHelmetRendering.dodgeLeftAnimations;
		this.defaultHelmetAnimationList.dodgeRightAnimations = this.defaultHelmetRendering.dodgeRightAnimations;
		this.defaultHelmetAnimationList.grindAnimations = this.defaultHelmetRendering.grindAnimations;
		this.defaultHelmetAnimationList.grindLandAnimations = this.defaultHelmetRendering.grindLandAnimations;
		this.defaultHelmetAnimationList.getOnHelmAnimations = this.defaultHelmetRendering.getOnHelmAnimations;
	}

	private void InitializeAnimations()
	{
		this.animations = new CharacterRendering.Animations();
		this.animations.HIT_MID = this.InitializeClips(this.defaultAnimations.hitMid);
		this.animations.HIT_UPPER = this.InitializeClips(this.defaultAnimations.hitUpper);
		this.animations.HIT_LOWER = this.InitializeClips(this.defaultAnimations.hitLower);
		this.animations.HIT_MOVING = this.InitializeClips(this.defaultAnimations.hitMoving);
		this.animations.STUMBLE = this.InitializeClips(this.defaultAnimations.stumble);
		this.animations.STUMBLE_MIX = this.InitializeClips(this.defaultAnimations.stumbleMix);
		this.animations.STUMBLE_LEFT_SIDE = this.InitializeClips(this.defaultAnimations.stumbleLeftSide);
		this.animations.STUMBLE_RIGHT_SIDE = this.InitializeClips(this.defaultAnimations.stumbleRightSide);
		this.animations.STUMBLE_LEFT_CORNER = this.InitializeClips(this.defaultAnimations.stumbleLeftCorner);
		this.animations.STUMBLE_RIGHT_CORNER = this.InitializeClips(this.defaultAnimations.stumbleRightCorner);
		this.animations.FALLWATER = this.InitializeClips(this.defaultAnimations.fallWater);
		this.characterAnimation["caught"].layer = 4;
		this.characterAnimation["caught"].enabled = false;
		this.characterAnimation["stumble"].AddMixingTransform(this.characterModel.spineTransform);
		this.characterAnimation["stumble"].layer = 2;
		this.characterAnimation["stumble"].weight = 1f;
		this.characterAnimation["stumbleCornerLeft"].AddMixingTransform(this.characterModel.spineTransform);
		this.characterAnimation["stumbleCornerLeft"].layer = 2;
		this.characterAnimation["stumbleCornerLeft"].weight = 1f;
		this.characterAnimation["stumbleCornerRight"].AddMixingTransform(this.characterModel.spineTransform);
		this.characterAnimation["stumbleCornerRight"].layer = 2;
		this.characterAnimation["stumbleCornerRight"].weight = 1f;
	}

	public void InitializeCharacterModel()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.characterModelPrefab);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		this.characterModel = gameObject.GetComponent<CharacterModel>();
		this.characterAnimation = this.characterModel.characterAnimation;
	}

	private void InitializeCharacterRenderingEffects()
	{
		this.characterRenderingEffects = UnityEngine.Object.Instantiate<GameObject>(this.characterRenderingEffectsPrefab).GetComponent<CharacterRenderingEffects>();
		this.characterRenderingEffects.Initialize(this.characterModel);
	}

	private string[] InitializeClips(AnimationClip[] clips)
	{
		string[] array = new string[clips.Length];
		for (int i = 0; i < clips.Length; i++)
		{
			AnimationClip animationClip = clips[i];
			if (!this.addedAnimClipsNames.Contains(animationClip))
			{
				this.addedAnimClipsNames.Add(animationClip);
				this.characterAnimation.AddClip(animationClip, animationClip.name);
			}
			array[i] = animationClip.name;
		}
		return array;
	}

	private void IsInGame_OnChange(bool isInGame)
	{
		if (!isInGame)
		{
			this.FlypackOnStop();
			this.SpringJumpOnStop();
		}
	}

	private void HandleOnTurboHeadstart()
	{
		if (this.isRolling)
		{
			this.EndRollAnim();
		}
		base.StopAllCoroutines();
		base.StartCoroutine(myTween.To(1f, delegate(float t)
		{
			this.FlypackOnFlyAheadUpdate(t);
		}));
	}

	private void FlypackOnFlyAheadUpdate(float ratio)
	{
		float num = Mathf.Lerp(0f, 1f, this.jetpackParticleOffsetCurve.Evaluate(ratio));
		this.characterRenderingEffects.FlypackParticles.transform.rotation = Quaternion.Euler(this.effectInitRot - new Vector3(num, 0f, 0f));
		this.characterRenderingEffects.FlypackParticles.transform.localScale = this.effectInitScale + new Vector3(0f, 0f, num * 2f);
	}

	private void FlypackOnFlyAheadEnd()
	{
		this.characterRenderingEffects.SetIngParticlesActive();
	}

	private void OnHidTurboHeadstartButtons()
	{
		this.characterRenderingEffects.SetEndParticlesActive();
	}

	private void FlypackOnStop()
	{
		this.characterRenderingEffects.FlypackParticles.transform.localScale = this.effectInitScale;
		this.characterRenderingEffects.FlypackParticles.SetActive(false);
		int i = 0;
		int num = this.characterModel.meshFlypack.Length;
		while (i < num)
		{
			this.characterModel.meshFlypack[i].enabled = false;
			i++;
		}
		this.characterModel.animFlypack.Stop();
		if (this.helmet.IsActive)
		{
			this.OnSwitchToHelmet(this.currentHelmet);
		}
		else if (this.superShoes.IsActive)
		{
			this.OnSwitchToSuperShoes();
			this.OnHangtime();
		}
		else
		{
			this.OnSwitchToRunning();
			this.OnHangtime();
		}
	}

	private void OnGameStart()
	{
		string run = this.animations.Run;
		this.characterAnimation[run].speed = this.game.DefaultSpeedForAnimation;
		this.characterAnimation.CrossFade(run, 0.1f);
	}

	private void OnCatchPlayer(string currentCharacterCaught, float catchUpTime, float waitTimeBeforeScreen)
	{
		this.caught = this.characterAnimation[currentCharacterCaught];
		base.StartCoroutine(this.BegainCatchPlayerAnim(this.caught, catchUpTime));
	}

	private IEnumerator BegainCatchPlayerAnim(AnimationState caught, float delay)
	{
		this.caught.wrapMode = WrapMode.Once;
		this.caught.weight = 0f;
		this.caught.normalizedTime = 0f;
		this.caught.enabled = true;
		yield return new WaitForSeconds(delay);
		base.StartCoroutine(myTween.To(0.2f, delegate(float t)
		{
			caught.weight = Mathf.Lerp(0f, 1f, t);
		}));
		yield break;
	}

	private void OnChangeIsGrounded(bool isGrounded)
	{
		this.characterModel.shadow.enabled = isGrounded;
	}

	private void WallWalkingOnJumpAheadStart(SwipeDir dir)
	{
		string animation = this.animations.StartWallRunLeft;
		string animation2 = this.animations.WallRunLeft;
		if (dir == SwipeDir.Right)
		{
			animation = this.animations.StartWallRunRight;
			animation2 = this.animations.WallRunRight;
		}
		this.characterAnimation.CrossFade(animation, 0.1f);
		this.characterAnimation.CrossFadeQueued(animation2, 0.1f);
		if (this.helmet.IsActive && this.helmetAnimation != null)
		{
			this.helmetAnimation.CrossFade(animation, 0.1f);
			this.helmetAnimation.CrossFadeQueued(animation2, 0.1f);
		}
	}

	private void WallWalkingOnJumpAheadEnd(SwipeDir dir)
	{
	}

	private void OnStartWallRun(SwipeDir dir)
	{
		string jump = this.animations.Jump;
		string animation = this.animations.WallRunLeft;
		if (dir == SwipeDir.Right)
		{
			animation = this.animations.WallRunRight;
		}
		this.characterAnimation.CrossFade(jump, 0.1f);
		this.characterAnimation.CrossFadeQueued(animation, 0.1f);
		if (this.helmet.IsActive && this.helmetAnimation != null)
		{
			this.helmetAnimation.CrossFade(jump, 0.1f);
			this.helmetAnimation.CrossFadeQueued(animation, 0.1f);
		}
	}

	private void OnLeaveWallRun(SwipeDir dir)
	{
		string animation = this.animations.EndWallRunLeft;
		if (dir == SwipeDir.Right)
		{
			animation = this.animations.EndWallRunRight;
		}
		this.characterAnimation.CrossFade(animation, 0.1f);
		if (this.helmet.IsActive && this.helmetAnimation != null)
		{
			this.helmetAnimation.CrossFade(animation, 0.1f);
		}
	}

	private void OnGetOnWaterBoard(int x)
	{
		if (this.character.IsStumbling)
		{
			this.character.StopStumble();
		}
		this.waterBoards[x] = true;
		this.isOnWaterBoard = true;
		this.OnSwitchToWaterBoard();
	}

	private void OnHandleWaterBoard(int x)
	{
		if (this.waterBoards[x] && !this.isOnWaterBoard)
		{
			this.OnSwitchToWaterBoard();
			this.isOnWaterBoard = true;
			return;
		}
		if (!this.waterBoards[x] && this.isOnWaterBoard)
		{
			if (this.helmet.IsActive)
			{
				this.OnSwitchToHelmet(this.currentHelmet);
				this.OnRun();
			}
			else if (this.superShoes.IsActive)
			{
				this.OnSwitchToSuperShoes();
				this.OnRun();
			}
			else
			{
				this.OnSwitchToRunning();
				this.OnRun();
			}
			this.isOnWaterBoard = false;
		}
	}

	private void OnGetOffWaterBoard(int x)
	{
		this.waterBoards[x] = false;
		this.isOnWaterBoard = false;
		if (!this.helmet.IsActive)
		{
			if (this.superShoes.IsActive)
			{
				this.OnSwitchToSuperShoes();
				this.OnRun();
			}
			else
			{
				this.OnSwitchToRunning();
				this.OnRun();
			}
		}
	}

	private void OnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (this.characterController.isGrounded && !SpringJump.Instance.isActive)
		{
			string text = (direction == Character.OnChangeTrackDirection.Left) ? this.animations.DodgeLeft : this.animations.DodgeRight;
			this.characterAnimation[text].speed = this.game.DefaultSpeedForAnimation;
			this.characterAnimation.CrossFade(text, 0.02f);
			if (this.raft.IsActive && this.raftAnimation != null)
			{
				this.raftAnimation.CrossFade(this.GetRaftAnimationName(text), 0.02f);
			}
			else if (this.helmet.IsActive && this.helmetAnimation != null)
			{
				this.helmetAnimation.CrossFade(this.GetHelmetAnimationName(text), 0.02f);
			}
		}
		if (!this.character.IsJumping)
		{
			string run = this.animations.Run;
			this.characterAnimation.CrossFadeQueued(run, this.game.Attachment.IsActive(this.game.Attachment.Helmet) ? 0.4f : 0.02f);
			if (this.raft.IsActive && this.raftAnimation != null)
			{
				this.raftAnimation.CrossFadeQueued(this.GetRaftAnimationName(run), 0.02f);
			}
			else if (this.helmet.IsActive && this.helmetAnimation != null)
			{
				this.helmetAnimation.CrossFadeQueued(this.GetHelmetAnimationName(run), 0.02f);
			}
		}
	}

	private void OnHangtime()
	{
		if (!this.character.IsRolling)
		{
			if (!this.helmet.IsActive || this.hangtimeAnimation == null)
			{
				this.hangtimeAnimation = this.animations.Hangtime;
			}
			this.characterAnimation.CrossFade(this.hangtimeAnimation, 0.3f);
			if (this.raftAnimation != null)
			{
				this.raftAnimation.CrossFade(this.GetRaftAnimationName(this.hangtimeAnimation), 0.3f);
			}
			else if (this.helmetAnimation != null)
			{
				this.helmetAnimation.CrossFade(this.GetHelmetAnimationName(this.hangtimeAnimation), 0.3f);
			}
		}
	}

	private void OnHitByTrain()
	{
		this.characterAnimation.Play(this.animations.HitMoving);
		Vector3 currentPos = this.character.transform.position;
		Vector3 camPos = this.character.characterCamera.transform.position;
		base.StartCoroutine(myTween.To(0.5f, delegate(float t)
		{
			this.character.transform.position = Vector3.Lerp(currentPos, new Vector3(camPos.x, camPos.y - 33f, currentPos.z), t);
		}));
	}

	private void OnFallIntoWater()
	{
		this.characterAnimation.Play(this.animations.FallWater);
	}

	private void OnIntroRun()
	{
		this.waterBoards[0] = false;
		this.waterBoards[1] = false;
		this.waterBoards[2] = false;
		this.isOnWaterBoard = false;
		this.characterModel.transform.localPosition = Vector3.zero;
		this.characterModel.PlayBall.gameObject.SetActive(false);
		this.characterModel.Flower.gameObject.SetActive(false);
		this.characterModel.Heart.gameObject.SetActive(false);
		this.OnSwitchToRunning();
	}

	private void OnJump()
	{
		this.jumpAnimation = this.animations.Jump;
		this.hangtimeAnimation = this.animations.Hangtime;
		this.characterAnimation.CrossFade(this.jumpAnimation, 0.15f);
		if (this.raft.IsActive && this.raftAnimation != null)
		{
			this.raftAnimation.CrossFade(this.GetRaftAnimationName(this.jumpAnimation), 0.15f);
		}
		else if (this.helmet.IsActive && this.helmetAnimation != null)
		{
			this.helmetAnimation.CrossFade(this.GetHelmetAnimationName(this.jumpAnimation), 0.15f);
		}
	}

	private void OnLanding(Transform characterTransform)
	{
		string text = this.animations.Run;
		this.characterAnimation[text].speed = this.game.DefaultSpeedForAnimation;
		if (!this.character.IsRolling)
		{
			if (this.helmet.IsActive || this.raft.IsActive)
			{
				string text2;
				if (this.character.IsAboveGround)
				{
					text = this.animations.Grind;
					text2 = text + "_land";
				}
				else
				{
					text2 = this.animations.Land;
				}
				this.characterAnimation.CrossFade(text2, 0.1f);
				this.characterAnimation.CrossFadeQueued(text, 0.2f);
				if (this.raftAnimation != null)
				{
					this.helmetAnimation.CrossFade(this.GetRaftAnimationName(text2), 0.1f);
					this.helmetAnimation.CrossFadeQueued(this.GetHelmetAnimationName(text), 0.2f);
				}
				else if (this.helmetAnimation != null)
				{
					this.helmetAnimation.CrossFade(this.GetHelmetAnimationName(text2), 0.1f);
					this.helmetAnimation.CrossFadeQueued(this.GetHelmetAnimationName(text), 0.2f);
				}
			}
			else
			{
				string land = this.animations.Land;
				this.characterAnimation.CrossFade(land, 0.05f);
				this.characterAnimation.CrossFadeQueued(text, 0.1f);
			}
		}
	}

	private void OnRevive()
	{
		base.StopAllCoroutines();
		this.waterBoards[0] = false;
		this.waterBoards[1] = false;
		this.waterBoards[2] = false;
		this.isOnWaterBoard = false;
		if (this.caught != null)
		{
			this.caught.enabled = false;
		}
		this.OnJump();
		if (CoinMagnet.Instance.IsActive)
		{
			this.characterAnimation["hold_magnet"].enabled = true;
			this.characterAnimation.Play("hold_magnet");
		}
	}

	private void OnRoll()
	{
		base.StartCoroutine(this.OnRollPlayAnimation());
	}

	private IEnumerator OnRollPlayAnimation()
	{
		this.isRolling = true;
		string rollAnimation = this.animations.Roll;
		string runAnimation = this.animations.Run;
		this.characterAnimation[runAnimation].speed = this.game.DefaultSpeedForAnimation;
		this.characterAnimation.CrossFade(rollAnimation, 0.1f);
		if (this.raft.IsActive)
		{
			this.characterAnimation.CrossFadeQueued(runAnimation, 0.2f);
			if (this.raftAnimation != null)
			{
				this.raftAnimation.CrossFade(this.GetRaftAnimationName(rollAnimation), 0.1f);
				this.raftAnimation.CrossFadeQueued(this.GetRaftAnimationName(runAnimation), this.raft.IsActive ? 0.2f : 0f);
			}
		}
		else if (this.helmet.IsActive)
		{
			this.characterAnimation.CrossFadeQueued(runAnimation, 0.2f);
			if (this.helmetAnimation != null)
			{
				this.helmetAnimation.CrossFade(this.GetHelmetAnimationName(rollAnimation), 0.1f);
				this.helmetAnimation.CrossFadeQueued(this.GetHelmetAnimationName(runAnimation), this.helmet.IsActive ? 0.2f : 0f);
			}
		}
		else
		{
			this.characterAnimation.CrossFadeQueued(runAnimation, 0.1f);
		}
		float endTime = Time.time + this.characterAnimation[rollAnimation].length;
		while (Time.time < endTime && this.characterAnimation[rollAnimation].enabled && this.isRolling)
		{
			yield return null;
		}
		this.EndRollAnim();
		yield break;
	}

	private void EndRollAnim()
	{
		this.isRolling = false;
		this.character.EndRoll();
	}

	private void OnRun()
	{
		if (!this.character.IsFalling && !this.character.IsJumping)
		{
			string run = this.animations.Run;
			if (this.characterController.isGrounded)
			{
				this.characterAnimation[run].speed = this.game.DefaultSpeedForAnimation;
				this.characterAnimation.CrossFade(run);
				if (this.raftAnimation != null)
				{
					this.raftAnimation.CrossFade(this.GetRaftAnimationName(run));
				}
				else if (this.helmetAnimation != null)
				{
					this.helmetAnimation.CrossFade(this.GetHelmetAnimationName(run));
				}
			}
		}
	}

	private void OnStageMenuSequence()
	{
		if (this.characterAnimation != null)
		{
			if (this.caught != null)
			{
				this.caught.enabled = false;
			}
			this.characterAnimation.transform.localRotation = Quaternion.identity;
		}
	}

	private void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		if (this.character.IsStumbling && this.game.CharacterState != null && this.game.IsInRunningMode)
		{
			this.characterAnimation.Play(this.animations.HitLower);
			return;
		}
		if (stumbleType == Character.StumbleType.Bush || colliderName == "lightSignal" || colliderName == "powerbox")
		{
			this.characterAnimation.CrossFade(this.animations.StumbleMix, 0.05f);
			this.characterAnimation.CrossFadeQueued(this.animations.Run, 0.5f);
		}
		else if (stumbleType == Character.StumbleType.Side)
		{
			if (!this.game.Attachment.IsActive(this.game.Attachment.Helmet) && !this.game.IsInFlypackMode && !this.game.IsInSpringJumpMode && !this.game.IsInBoundJumpMode)
			{
				if (horizontalHit == Character.StumbleHorizontalHit.LeftCorner || horizontalHit == Character.StumbleHorizontalHit.Left)
				{
					this.characterAnimation.CrossFade(this.animations.StumbleLeftSide, 0.2f);
				}
				if (horizontalHit == Character.StumbleHorizontalHit.RightCorner || horizontalHit == Character.StumbleHorizontalHit.Right)
				{
					this.characterAnimation.CrossFade(this.animations.StumbleRightSide, 0.2f);
				}
			}
			if (!this.character.IsJumping)
			{
				this.characterAnimation.CrossFadeQueued(this.animations.Run, (!this.game.Attachment.IsActive(this.game.Attachment.Helmet)) ? 0.02f : 0.4f);
			}
		}
		else if (horizontalHit == Character.StumbleHorizontalHit.Center)
		{
			if (verticalHit == Character.StumbleVerticalHit.Lower)
			{
				if (this.game.Attachment.IsActive(this.game.Attachment.Helmet))
				{
					this.characterAnimation.CrossFade(this.animations.StumbleMix, 0.05f);
				}
				else
				{
					this.characterAnimation.CrossFade(this.animations.Stumble, 0.05f);
				}
				this.characterAnimation.CrossFadeQueued(this.animations.Run, 0.5f);
			}
			else if (verticalHit == Character.StumbleVerticalHit.Middle)
			{
				this.characterAnimation.CrossFade(this.animations.HitMid, 0.07f);
			}
			else if (verticalHit == Character.StumbleVerticalHit.Upper)
			{
				this.characterAnimation.CrossFade(this.animations.HitUpper, 0.07f);
			}
		}
		else
		{
			if (horizontalHit == Character.StumbleHorizontalHit.Left)
			{
				this.characterAnimation.Play(this.game.Attachment.IsActive(this.game.Attachment.Helmet) ? this.animations.StumbleLeftCorner : this.animations.StumbleLeftSide);
			}
			else if (horizontalHit == Character.StumbleHorizontalHit.LeftCorner)
			{
				this.characterAnimation.Play(this.animations.StumbleLeftCorner);
			}
			else if (horizontalHit == Character.StumbleHorizontalHit.Right)
			{
				this.characterAnimation.Play(this.game.Attachment.IsActive(this.game.Attachment.Helmet) ? this.animations.StumbleRightCorner : this.animations.StumbleRightSide);
			}
			else if (horizontalHit == Character.StumbleHorizontalHit.RightCorner)
			{
				this.characterAnimation.Play(this.animations.StumbleRightCorner);
			}
			this.characterAnimation.PlayQueued(this.animations.Run);
		}
	}

	private void OnSwitchToRaft(GameObject raft)
	{
		this.ToggleRaft(raft);
		string getOn = this.animations.GetOn;
		this.characterAnimation.CrossFade(getOn, 0.2f);
		string run = this.animations.Run;
		this.characterAnimation.CrossFadeQueued(run, 0.2f);
		if (this.raftAnimation != null)
		{
			this.raftAnimation.CrossFade(run, 0.2f);
		}
	}

	private void OnEndRaft()
	{
		if (this.helmet.IsActive)
		{
			this.OnSwitchToHelmet(this.currentHelmet);
		}
		else if (this.superShoes.IsActive)
		{
			this.OnSwitchToSuperShoes();
			this.OnJump();
		}
		else
		{
			this.OnSwitchToRunning();
			this.OnJump();
		}
	}

	private void OnSwitchToHelmet(GameObject helmet)
	{
		this.ToggleCustomHelmet(helmet);
		string getOn = this.animations.GetOn;
		this.characterAnimation.CrossFade(getOn, 0.1f);
		if (this.helmetAnimation != null)
		{
			this.helmetAnimation.CrossFade(this.GetHelmetAnimationName(getOn), 0.1f);
		}
		this.hangtimeAnimation = this.animations.Hangtime;
		if (!this.character.IsFalling && !this.character.IsJumping)
		{
			string run = this.animations.Run;
			this.characterAnimation.CrossFadeQueued(run, 0.2f);
			if (this.helmetAnimation != null)
			{
				this.helmetAnimation.CrossFadeQueued(this.GetHelmetAnimationName(run), 0.2f);
			}
		}
		else
		{
			this.characterAnimation.CrossFade(this.hangtimeAnimation, 0.2f);
			if (this.helmetAnimation != null)
			{
				this.helmetAnimation.CrossFadeQueued(this.GetHelmetAnimationName(this.hangtimeAnimation), 0.2f);
			}
		}
	}

	private void OnSwitchToPogostickJump()
	{
		this.animations.RUN = this.InitializeClips(this.pogostickClips.run);
		this.animations.DODGE_LEFT = this.InitializeClips(this.pogostickClips.dodgeLeft);
		this.animations.DODGE_RIGHT = this.InitializeClips(this.pogostickClips.dodgeRight);
		this.animations.HANGTIME = this.InitializeClips(this.pogostickClips.hangtime);
		this.characterAnimation.CrossFade(this.animations.Run);
	}

	private void OnSwitchToFlypack(bool isHeadStart)
	{
		this.animations.RUN = this.InitializeClips(this.jetpackAnimations.run);
		this.animations.DODGE_LEFT = this.InitializeClips(this.jetpackAnimations.dodgeLeft);
		this.animations.DODGE_RIGHT = this.InitializeClips(this.jetpackAnimations.dodgeRight);
		this.animations.TURBO_HEADSTART = this.InitializeClips(this.jetpackAnimations.turboHeadstart);
		int i = 0;
		int num = this.characterModel.meshFlypack.Length;
		while (i < num)
		{
			this.characterModel.meshFlypack[i].enabled = true;
			i++;
		}
		this.characterModel.animFlypack.Play();
		this.characterRenderingEffects.FlypackParticles.SetActive(true);
		this.characterRenderingEffects.SetStartParticlesActive();
		this.characterAnimation.CrossFade(this.animations.Run);
	}

	public void OnSwitchToRunning()
	{
		if (this.superShoes.IsActive)
		{
			this.animations.RUN = this.InitializeClips(this.SuperShoesAnimations.run);
		}
		else if (this.isOnWaterBoard)
		{
			this.animations.RUN = this.InitializeClips(this.WaterBoardAnimations.run);
		}
		else
		{
			this.animations.RUN = this.InitializeClips(this.defaultAnimations.run);
		}
		this.animations.START_WALLRUN_LEFT = this.InitializeClips(this.defaultAnimations.startWallRunLeft);
		this.animations.WALLRUN_LEFT = this.InitializeClips(this.defaultAnimations.wallRunLeft);
		this.animations.END_WALLRUN_LEFT = this.InitializeClips(this.defaultAnimations.endWallRunLeft);
		this.animations.START_WALLRUN_RIGHT = this.InitializeClips(this.defaultAnimations.startWallRunRight);
		this.animations.WALLRUN_RIGHT = this.InitializeClips(this.defaultAnimations.wallRunRight);
		this.animations.END_WALLRUN_RIGHT = this.InitializeClips(this.defaultAnimations.endWallRunRight);
		this.animations.LAND = this.InitializeClips(this.defaultAnimations.landing);
		this.animations.JUMP = this.InitializeClips(this.defaultAnimations.jump);
		this.animations.HANGTIME = this.InitializeClips(this.defaultAnimations.hangtime);
		this.animations.ROLL = this.InitializeClips(this.defaultAnimations.roll);
		this.animations.DODGE_LEFT = this.InitializeClips(this.defaultAnimations.dodgeLeft);
		this.animations.DODGE_RIGHT = this.InitializeClips(this.defaultAnimations.dodgeRight);
		if (this.currentHelmet != null)
		{
			this.ToggleCustomHelmet(null);
		}
	}

	private void OnSwitchToSuperShoes()
	{
		if (this.helmetRendering == null)
		{
			this.OnSwitchToRunning();
		}
	}

	private void OnSwitchToWaterBoard()
	{
		if (this.helmetRendering == null)
		{
			this.animations.RUN = this.InitializeClips(this.WaterBoardAnimations.run);
			this.characterAnimation.CrossFade(this.animations.Run, 0.1f);
		}
	}

	public void ActivateSnow(bool activate)
	{
		if (activate)
		{
			this.characterRenderingEffects.ActivateSnow();
		}
		else
		{
			this.characterRenderingEffects.DeactivateSnow();
		}
	}

	private void OnTutorialMoveBackToCheckPoint(float duration)
	{
		this.characterAnimation.CrossFade(this.animations.Run, duration);
	}

	private void OnTutorialStartFromCheckPoint()
	{
		this.characterAnimation.Play(this.animations.Run);
	}

	private void OnSpeedupStart()
	{
		this.characterRenderingEffects.SetSpeedupActive();
		string superRun = this.animations.SuperRun;
		if (this.helmet.IsActive)
		{
			this.characterAnimation.CrossFade(superRun, 0.1f);
			if (this.helmetAnimation != null)
			{
				this.helmetAnimation.CrossFade(this.GetHelmetAnimationName(superRun), 0.1f);
			}
		}
		else
		{
			this.characterAnimation.CrossFade(superRun, 0.1f);
		}
	}

	private void OnSpeedupStop()
	{
		this.characterRenderingEffects.SetSpeedupDeactive();
		string run = this.animations.Run;
		if (this.helmet.IsActive)
		{
			this.characterAnimation.CrossFade(run, 0.2f);
			if (this.helmetAnimation != null)
			{
				this.helmetAnimation.CrossFade(this.GetHelmetAnimationName(run), 0.2f);
			}
		}
		else
		{
			this.characterAnimation.CrossFade(run, 0.2f);
		}
	}

	private void OverrideRollTransition()
	{
		string run = this.animations.Run;
		if (this.helmet.IsActive)
		{
			this.characterAnimation.CrossFadeQueued(run, 0.2f);
			if (this.helmetAnimation != null)
			{
				this.helmetAnimation.CrossFadeQueued(this.GetHelmetAnimationName(run), this.helmet.IsActive ? 0.2f : 0f);
			}
		}
		else
		{
			this.characterAnimation.CrossFadeQueued(run, 0.1f);
		}
	}

	private void SpringJumpOnHangtime()
	{
		string hangtime = this.animations.Hangtime;
		this.characterAnimation.CrossFade(hangtime, 0.2f);
	}

	private void SpringJumpOnStart()
	{
		this.OnSwitchToPogostickJump();
	}

	private void SpringJumpOnStop()
	{
		if (this.helmet.IsActive)
		{
			this.OnSwitchToHelmet(this.currentHelmet);
		}
		else if (this.superShoes.IsActive)
		{
			this.OnSwitchToSuperShoes();
			this.OnHangtime();
		}
		else
		{
			this.OnSwitchToRunning();
			this.OnHangtime();
		}
	}

	private void SuperShoesOnStop()
	{
		if (this.helmetRendering == null)
		{
			this.OnSwitchToRunning();
			this.OnRun();
		}
	}

	private void Start()
	{
		if (this.CharacterModelInitialized != null)
		{
			this.CharacterModelInitialized(this.characterModel.BoneHelmet.gameObject);
		}
	}

	private void ToggleCustomHelmet(GameObject newHelmet)
	{
		if (this.currentHelmet != null && this.currentHelmet != newHelmet)
		{
			UnityEngine.Object.Destroy(this.currentHelmet);
		}
		this.currentHelmet = newHelmet;
		if (newHelmet != null)
		{
			this.characterModel.currentHelmet.gameObject.SetActive(true);
			this.helmetAnimation = newHelmet.GetComponent<Animation>();
			this.helmetRendering = newHelmet.GetComponent<HelmetRendering>();
			if (this.helmetRendering != null)
			{
				this.helmetRendering.Initialize(this.characterAnimation, this.helmetAnimation, this.defaultHelmetAnimationList, this.addedAnimClipsNames);
			}
			else
			{
				this.helmetRendering = this.defaultHelmetRendering;
				this.helmetRendering.Initialize(this.characterAnimation, this.helmetAnimation, this.defaultHelmetAnimationList, this.addedAnimClipsNames);
			}
		}
		else
		{
			this.helmetAnimation = null;
			this.helmetRendering = null;
		}
	}

	private void ToggleRaft(GameObject raft)
	{
		if (raft != null)
		{
			this.raftAnimation = raft.GetComponentInChildren<Animation>();
			this.raftRendering = raft.GetComponent<RaftRendering>();
			if (this.raftRendering != null)
			{
				this.raftRendering.Initialize(this.characterAnimation, this.helmetAnimation, this.addedAnimClipsNames);
			}
		}
		else
		{
			this.raftAnimation = null;
			this.raftRendering = null;
		}
	}

	public CharacterModel CharacterModel
	{
		get
		{
			return this.characterModel;
		}
	}

	public static CharacterRendering Instance
	{
		get
		{
			if (CharacterRendering.instance == null)
			{
				CharacterRendering.instance = Utils.FindObject<CharacterRendering>();
			}
			return CharacterRendering.instance;
		}
	}

	[SerializeField]
	private CharacterRendering.AnimationClipLists defaultAnimations;

	[SerializeField]
	private CharacterRendering.JetpackClips jetpackAnimations;

	[SerializeField]
	private CharacterRendering.SuperSneaksClips SuperShoesAnimations;

	[SerializeField]
	private CharacterRendering.PogostickClips pogostickClips;

	[SerializeField]
	private CharacterRendering.WaterBoardClips WaterBoardAnimations;

	[OptionalField]
	public Animation characterAnimation;

	[SerializeField]
	private AnimationCurve jetpackParticleOffsetCurve;

	public CharacterRendering.Animations animations;

	[SerializeField]
	private GameObject characterModelPrefab;

	[SerializeField]
	private GameObject characterRenderingEffectsPrefab;

	private List<AnimationClip> addedAnimClipsNames = new List<AnimationClip>();

	private AnimationState caught;

	private Character character;

	private CharacterController characterController;

	private CharacterModel characterModel;

	private CharacterRenderingEffects characterRenderingEffects;

	private GameObject currentHelmet;

	private HelmetRendering.AnimationList defaultHelmetAnimationList;

	private HelmetRendering defaultHelmetRendering;

	private RaftRendering reftRenderer;

	private Boss boss;

	private Game game;

	private string hangtimeAnimation;

	private Helmet helmet;

	private Raft raft;

	private Animation helmetAnimation;

	private HelmetRendering helmetRendering;

	private Animation raftAnimation;

	private RaftRendering raftRendering;

	private Vector3 effectInitRot = Vector3.zero;

	private Vector3 effectInitScale = Vector3.one;

	private static CharacterRendering instance;

	private bool isRolling;

	private Flypack flypack;

	private SpeedUp speedup;

	private string jumpAnimation;

	private SpringJump springJump;

	private BoundJump boundJump;

	private Revive revive;

	private SuperShoes superShoes;

	private WallWalking wallWalking;

	private bool[] waterBoards;

	private bool isOnWaterBoard;

	public delegate void CharacterModelInitializedDelegate(GameObject helmetRoot);

	[Serializable]
	public class AnimationClipLists
	{
		public AnimationClip[] run;

		public AnimationClip[] superRun;

		public AnimationClip[] startWallRunLeft;

		public AnimationClip[] wallRunLeft;

		public AnimationClip[] endWallRunLeft;

		public AnimationClip[] startWallRunRight;

		public AnimationClip[] wallRunRight;

		public AnimationClip[] endWallRunRight;

		public AnimationClip[] jump;

		public AnimationClip[] hangtime;

		public AnimationClip[] landing;

		public AnimationClip[] dodgeLeft;

		public AnimationClip[] dodgeRight;

		public AnimationClip[] roll;

		public AnimationClip[] hitMid;

		public AnimationClip[] hitUpper;

		public AnimationClip[] hitLower;

		public AnimationClip[] hitMoving;

		public AnimationClip[] fallWater;

		public AnimationClip[] stumble;

		public AnimationClip[] stumbleMix;

		public AnimationClip[] stumbleDeath;

		public AnimationClip[] stumbleLeftSide;

		public AnimationClip[] stumbleRightSide;

		public AnimationClip[] stumbleLeftCorner;

		public AnimationClip[] stumbleRightCorner;
	}

	[Serializable]
	public class Animations
	{
		private string GetRandomAnimationName(string[] animationsNames)
		{
			int num = UnityEngine.Random.Range(0, animationsNames.Length);
			return animationsNames[num];
		}

		private string[] GetRandomHoverJumps(string[] helmetJump, string[] helmetHangtime)
		{
			if (helmetJump.Length != helmetHangtime.Length)
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					"helmetJump.Length (",
					helmetJump.Length,
					") != (",
					helmetHangtime.Length,
					") helmetHangtime.Length"
				}));
			}
			int num = UnityEngine.Random.Range(0, Mathf.Min(helmetJump.Length, helmetHangtime.Length));
			return new string[]
			{
				helmetJump[num],
				helmetHangtime[num]
			};
		}

		public string DodgeLeft
		{
			get
			{
				return this.GetRandomAnimationName(this.DODGE_LEFT);
			}
		}

		public string DodgeRight
		{
			get
			{
				return this.GetRandomAnimationName(this.DODGE_RIGHT);
			}
		}

		public string GetOn
		{
			get
			{
				return this.GetRandomAnimationName(this.GET_ON);
			}
		}

		public string Grind
		{
			get
			{
				return this.GetRandomAnimationName(this.GRIND);
			}
		}

		public string Hangtime
		{
			get
			{
				return this.GetRandomAnimationName(this.HANGTIME);
			}
		}

		public string HitLower
		{
			get
			{
				return this.GetRandomAnimationName(this.HIT_LOWER);
			}
		}

		public string HitMid
		{
			get
			{
				return this.GetRandomAnimationName(this.HIT_MID);
			}
		}

		public string HitMoving
		{
			get
			{
				return this.GetRandomAnimationName(this.HIT_MOVING);
			}
		}

		public string FallWater
		{
			get
			{
				return this.GetRandomAnimationName(this.FALLWATER);
			}
		}

		public string HitUpper
		{
			get
			{
				return this.GetRandomAnimationName(this.HIT_UPPER);
			}
		}

		public string Jump
		{
			get
			{
				return this.GetRandomAnimationName(this.JUMP);
			}
		}

		public string Land
		{
			get
			{
				return this.GetRandomAnimationName(this.LAND);
			}
		}

		public string Roll
		{
			get
			{
				return this.GetRandomAnimationName(this.ROLL);
			}
		}

		public string Run
		{
			get
			{
				return this.GetRandomAnimationName(this.RUN);
			}
		}

		public string SuperRun
		{
			get
			{
				return this.GetRandomAnimationName(this.SUPERRUN);
			}
		}

		public string StartWallRunLeft
		{
			get
			{
				return this.GetRandomAnimationName(this.START_WALLRUN_LEFT);
			}
		}

		public string WallRunLeft
		{
			get
			{
				return this.GetRandomAnimationName(this.WALLRUN_LEFT);
			}
		}

		public string EndWallRunLeft
		{
			get
			{
				return this.GetRandomAnimationName(this.END_WALLRUN_LEFT);
			}
		}

		public string StartWallRunRight
		{
			get
			{
				return this.GetRandomAnimationName(this.START_WALLRUN_RIGHT);
			}
		}

		public string WallRunRight
		{
			get
			{
				return this.GetRandomAnimationName(this.WALLRUN_RIGHT);
			}
		}

		public string EndWallRunRight
		{
			get
			{
				return this.GetRandomAnimationName(this.END_WALLRUN_RIGHT);
			}
		}

		public string Stumble
		{
			get
			{
				return this.GetRandomAnimationName(this.STUMBLE);
			}
		}

		public string StumbleLeftCorner
		{
			get
			{
				return this.GetRandomAnimationName(this.STUMBLE_LEFT_CORNER);
			}
		}

		public string StumbleLeftSide
		{
			get
			{
				return this.GetRandomAnimationName(this.STUMBLE_LEFT_SIDE);
			}
		}

		public string StumbleMix
		{
			get
			{
				return this.GetRandomAnimationName(this.STUMBLE_MIX);
			}
		}

		public string StumbleRightCorner
		{
			get
			{
				return this.GetRandomAnimationName(this.STUMBLE_RIGHT_CORNER);
			}
		}

		public string StumbleRightSide
		{
			get
			{
				return this.GetRandomAnimationName(this.STUMBLE_RIGHT_SIDE);
			}
		}

		public string TurboHeadstart
		{
			get
			{
				return this.GetRandomAnimationName(this.TURBO_HEADSTART);
			}
		}

		public string[] RUN = new string[]
		{
			"run"
		};

		public string[] SUPERRUN = new string[]
		{
			"run"
		};

		public string[] START_WALLRUN_LEFT = new string[]
		{
			"startWallRun"
		};

		public string[] WALLRUN_LEFT = new string[]
		{
			"wallRun"
		};

		public string[] END_WALLRUN_LEFT = new string[]
		{
			"endWallRun"
		};

		public string[] START_WALLRUN_RIGHT = new string[]
		{
			"startWallRun"
		};

		public string[] WALLRUN_RIGHT = new string[]
		{
			"wallRun"
		};

		public string[] END_WALLRUN_RIGHT = new string[]
		{
			"endWallRun"
		};

		public string[] LAND = new string[]
		{
			"landing"
		};

		public string[] JUMP = new string[]
		{
			"jump",
			"jump2",
			"jump_salto"
		};

		public string[] HANGTIME = new string[]
		{
			"hangtime"
		};

		public string[] ROLL = new string[]
		{
			"roll"
		};

		public string[] DODGE_LEFT = new string[]
		{
			"dodgeLeft"
		};

		public string[] DODGE_RIGHT = new string[]
		{
			"dodgeRight"
		};

		public string[] TURBO_HEADSTART = new string[]
		{
			"dodgeRight"
		};

		public string[] HIT_MID = new string[]
		{
			"hitMid"
		};

		public string[] HIT_UPPER = new string[]
		{
			"hitUpper"
		};

		public string[] HIT_LOWER = new string[]
		{
			"hitLower"
		};

		public string[] HIT_MOVING = new string[]
		{
			"hitMoving"
		};

		public string[] FALLWATER = new string[]
		{
			"intoWater"
		};

		public string[] STUMBLE_MIX = new string[]
		{
			"stumble"
		};

		public string[] STUMBLE_LEFT_SIDE = new string[]
		{
			"stumbleLeftSide"
		};

		public string[] STUMBLE_RIGHT_SIDE = new string[]
		{
			"stumbleRightSide"
		};

		public string[] STUMBLE_LEFT_CORNER = new string[]
		{
			"stumbleLeftCorner"
		};

		public string[] STUMBLE_RIGHT_CORNER = new string[]
		{
			"stumbleRightCorner"
		};

		public string[] GRIND = new string[]
		{
			"run"
		};

		public string[] GET_ON = new string[]
		{
			"run"
		};

		public string[] STUMBLE = new string[]
		{
			"stumble"
		};

		public string DEFAULT_ANIMATION;
	}

	[Serializable]
	public class JetpackClips
	{
		public AnimationClip[] run;

		public AnimationClip[] dodgeLeft;

		public AnimationClip[] dodgeRight;

		public AnimationClip[] turboHeadstart;
	}

	[Serializable]
	public class PogostickClips
	{
		public AnimationClip[] run;

		public AnimationClip[] hangtime;

		public AnimationClip[] dodgeLeft;

		public AnimationClip[] dodgeRight;
	}

	[Serializable]
	public class SuperSneaksClips
	{
		public AnimationClip[] run;
	}

	[Serializable]
	public class WaterBoardClips
	{
		public AnimationClip[] run;
	}
}
