using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	public Character()
	{
		this.IsGrounded = new Variable<bool>(false);
		this.subwayColliders = new HashSet<Collider>();
		this.underpassColliders = new HashSet<Collider>();
		this.walls = new Wall[2];
		this.onewayColliders = new HashSet<Collider>();
		this.speedupColliders = new HashSet<Collider>();
		this.squeezeCollider = new VariableBool();
		this.superShoesJumpApexRatio = 0.5f;
		this.trainJumpSampleLength = 10f;
		this.verticalSpeed_jumpTolerance = -30f;
	}

	public event Character.OnChangeTrackDelegate OnChangeTrack;

	public event Character.OnCriticalHitDelegate OnCriticalHit;

	public event Character.OnHangtimeDelegate OnHangtime;

	public event Character.OnHitByTrainDelegate OnHitByTrain;

	public event Character.OnFallIntoWaterDelegate OnFallIntoWater;

	public event Character.OnJumpDelegate OnJump;

	public event Character.OnJumpIfHitByTrainDelegate OnJumpIfHitByTrain;

	public event Character.OnJumpOverTrainDelegate OnJumpOverTrain;

	public event Character.OnLandingDelegate OnLanding;

	public event Character.OnPassedObstacleDelegate OnPassedObstacle;

	public event Character.OnRollDelegate OnRoll;

	public event Character.OnRollGuardDelegate OnRollGuard;

	public event Character.OnStumbleDelegate OnStumble;

	public event Character.OnTutorialMoveBackToCheckPointDelegate OnTutorialMoveBackToCheckPoint;

	public event Character.OnTutorialStartFromCheckPointDelegate OnTutorialStartFromCheckPoint;

	public event Character.OnHandleWaterBoardDelegate OnHandleWaterBoard;

	public void ApplyGravity()
	{
		if (this.verticalSpeed < 0f && this.characterController.isGrounded)
		{
			if (this.startedJumpFromGround && this.trainJump && this.IsRunningOnGround())
			{
				this.NotifyOnJumpOverTrain();
			}
			if (this.running.currentRunPosition != Running.RunPositions.air)
			{
				this.startedJumpFromGround = false;
			}
			this.verticalSpeed = 0f;
			this.IsGrounded.Value = true;
			if (this.isJumping || this.isFalling)
			{
				this.isJumping = false;
				this.isJumpingWithSuperShoes = false;
				this.isFalling = false;
				this.IsGrounded.Value = true;
				this.NotifyOnLanding();
			}
		}
		else if (this.startedJumpFromGround && this.trainJumpSampleZ < this.z)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(base.transform.position, -Vector3.up), out raycastHit) && (raycastHit.collider.CompareTag("HitMovingTrain") || raycastHit.collider.CompareTag("HitTrain")))
			{
				this.trainJump = true;
			}
			this.trainJumpSampleZ += this.trainJumpSampleLength;
		}
		if (this.isStairing)
		{
			this.verticalSpeed -= 1.5f * this.gravity * Time.deltaTime;
		}
		else
		{
			this.verticalSpeed -= this.gravity * Time.deltaTime;
		}
		if (!this.characterController.isGrounded && !this.isFalling && this.verticalSpeed < this.verticalFallSpeedLimit && !this.isRolling && !this.isStairing)
		{
			this.isFalling = true;
			this.NotifyOnHangtime();
			this.IsGrounded.Value = false;
		}
	}

	public float CalculateJumpVerticalSpeed()
	{
		return this.CalculateJumpVerticalSpeed(this.jumpHeight);
	}

	public float CalculateJumpVerticalSpeed(float jumpHeight)
	{
		return Mathf.Sqrt(2f * jumpHeight * this.gravity);
	}

	public void ChangeTrack(int movement, float duration)
	{
		TasksManager.Instance.PlayerDidThis(TaskTarget.StayInOneLane, (int)(Time.time - this.sameLaneTimeStamp), -1);
		TasksManager.Instance.RemoveProgressForThis(TaskTarget.StayInOneLane);
		this.sameLaneTimeStamp = Time.time;
		if (this.trackMovement != movement)
		{
			this.ForceChangeTrack(movement, duration, false);
		}
		else
		{
			this.trackMovementNext = movement;
		}
	}

	private IEnumerator ChangeTrackCoroutine(int move, float duration, bool force = false)
	{
		if (this.isInOneway)
		{
			if (this.trackIndexTarget > this.onewayId)
			{
				this.HandleStumble(Character.StumbleType.Side, Character.StumbleHorizontalHit.Right, Character.StumbleVerticalHit.Middle, "side");
			}
			else if (this.trackIndexTarget < this.onewayId)
			{
				this.HandleStumble(Character.StumbleType.Side, Character.StumbleHorizontalHit.Left, Character.StumbleVerticalHit.Middle, "side");
			}
			yield break;
		}
		this.trackMovement = move;
		this.trackMovementNext = 0;
		int newTrackIndex = this.trackIndexTarget + move;
		float trackChangeIndexDistance = Mathf.Abs((float)newTrackIndex - this.trackIndexPosition);
		float trackIndexPositionBegin = this.trackIndexPosition;
		float startX = this.x;
		float endX = this.trackController.GetTrackX(newTrackIndex);
		float dir = Mathf.Sign((float)(newTrackIndex - this.trackIndexTarget));
		float startRotation = this.characterRotation;
		if (newTrackIndex >= 0)
		{
			if (newTrackIndex >= this.trackController.NumberOfTracks)
			{
				this.HandleStumble(Character.StumbleType.Side, Character.StumbleHorizontalHit.Right, Character.StumbleVerticalHit.Middle, "side");
			}
			else
			{
				if (!force && this.OnChangeTrack != null)
				{
					this.OnChangeTrack((move < 0) ? Character.OnChangeTrackDirection.Left : Character.OnChangeTrackDirection.Right);
				}
				this.trackIndexTarget = newTrackIndex;
				this.HandleWaterBoard();
				yield return base.StartCoroutine(myTween.To(trackChangeIndexDistance * duration, delegate(float t)
				{
					this.trackIndexPosition = Mathf.Lerp(trackIndexPositionBegin, (float)newTrackIndex, t);
					this.x = Mathf.Lerp(startX, endX, t);
					this.characterRotation = pMath.Bell(t) * dir * this.characterAngle + Mathf.Lerp(startRotation, 0f, t);
					this.characterRoot.localRotation = Quaternion.Euler(0f, this.characterRotation, 0f);
				}));
				this.trackIndex = newTrackIndex;
				this.trackMovement = 0;
				if (this.trackMovementNext != 0)
				{
					base.StartCoroutine(this.ChangeTrackCoroutine(this.trackMovementNext, duration, force));
				}
			}
		}
		else
		{
			this.HandleStumble(Character.StumbleType.Side, Character.StumbleHorizontalHit.Left, Character.StumbleVerticalHit.Middle, "side");
		}
		yield break;
	}

	public void CheckInAirJump()
	{
		if (this.characterController.isGrounded && this.inAirJump)
		{
			this.Jump();
			this.inAirJump = false;
		}
	}

	public void EndRoll()
	{
		if (this.characterController.enabled)
		{
			this.characterController.Move(Vector3.up * 2f);
		}
		this.squeezeCollider.Remove(this);
		if (this.characterController.enabled)
		{
			this.characterController.Move(Vector3.down * 2f);
		}
		this.isRolling = false;
	}

	public void ForceChangeTrack(int movement, float duration, bool force = false)
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.ChangeTrackCoroutine(movement, duration, force));
	}

	public void ForceLeaveSubway()
	{
		this.subwayColliders.Clear();
		this.isInsideSubway = false;
	}

	private Character.ImpactX GetImpactX(Collider collider)
	{
		Bounds bounds = this.characterCollider.bounds;
		Bounds bounds2 = collider.bounds;
		float num = Mathf.Max(bounds.min.x, bounds2.min.x);
		float num2 = Mathf.Min(bounds.max.x, bounds2.max.x);
		float num3 = (num + num2) * 0.5f;
		float num4 = num3 - bounds2.min.x;
		if ((double)num4 > (double)bounds2.size.x - (double)this.ColliderTrackWidth * 0.33)
		{
			return Character.ImpactX.Right;
		}
		if ((double)num4 < (double)this.ColliderTrackWidth * 0.33)
		{
			return Character.ImpactX.Left;
		}
		return Character.ImpactX.Middle;
	}

	private Character.ImpactY GetImpactY(Collider collider)
	{
		Bounds bounds = this.characterCollider.bounds;
		Bounds bounds2 = collider.bounds;
		float num = Mathf.Max(bounds.min.y, bounds2.min.y);
		float num2 = Mathf.Min(bounds.max.y, bounds2.max.y);
		float num3 = (num + num2) * 0.5f;
		float num4 = (num3 - bounds.min.y) / bounds.size.y;
		if (num4 < 0.33f)
		{
			return Character.ImpactY.Lower;
		}
		if (num4 < 0.66f)
		{
			return Character.ImpactY.Middle;
		}
		return Character.ImpactY.Upper;
	}

	private Character.ImpactZ GetImpactZ(Collider collider)
	{
		Vector3 position = base.transform.position;
		Bounds bounds = collider.bounds;
		if (position.z > bounds.max.z - ((bounds.max.z - bounds.min.z > 30f) ? this.stumbleCornerTolerance : ((bounds.max.z - bounds.min.z) * 0.5f)))
		{
			return Character.ImpactZ.After;
		}
		if (position.z < bounds.min.z + this.stumbleCornerTolerance)
		{
			return Character.ImpactZ.Before;
		}
		return Character.ImpactZ.Middle;
	}

	public float GetTrackX()
	{
		return this.trackController.GetPosition(this.trackController.GetTrackX(this.trackIndex), 0f).x;
	}

	private void HandleRevive()
	{
		base.StopAllCoroutines();
		this.StopStumble();
	}

	private void HandleStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		if (!this.game.IsInFlypackMode && !this.game.IsInSpringJumpMode && !this.game.IsInBoundJumpMode)
		{
			this.NotifyOnStumble(stumbleType, horizontalHit, verticalHit, colliderName);
			this.StartStumble();
		}
	}

	private void HitByTrainSequence()
	{
		if (this.helmet.IsActive)
		{
			this.NotifyOnJumpIfHitByTrain();
		}
		else
		{
			this.NotifyOnHitByTrain();
		}
	}

	private void FallIntoWater()
	{
		if (this.helmet.IsActive)
		{
			this.NotifyOnJumpIfHitByTrain();
		}
		else
		{
			this.NotifyOnFallIntoWater();
		}
	}

	public void DelegateIsInGame(bool isInGame)
	{
		if (!isInGame)
		{
			base.StopAllCoroutines();
			this.immuneToCriticalHit = false;
			this.characterController.enabled = true;
			this.stopColliding = false;
		}
	}

	public void DelegateSqueeze(bool squeeze)
	{
		if (squeeze)
		{
			this.characterController.height = 4f;
			this.characterController.center = new Vector3(0f, 2f, this.characterControllerCenter.z);
			this.characterCollider.height = 4f;
			this.characterCollider.center = new Vector3(0f, 4f, this.characterColliderCenter.z);
		}
		else
		{
			this.characterController.center = this.characterControllerCenter;
			this.characterController.height = this.characterControllerHeight;
			this.characterCollider.center = this.characterColliderCenter;
			this.characterCollider.height = this.characterColliderHeight;
		}
	}

	public void Initialize()
	{
		this.layers = Layers.Instance;
		this.game = Game.Instance;
		this.game.IsInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(this.game.IsInGame.OnChange, new Variable<bool>.OnChangeDelegate(this.DelegateIsInGame));
		this.squeezeCollider.OnChange = (VariableBool.OnChangeDelegate)Delegate.Combine(this.squeezeCollider.OnChange, new VariableBool.OnChangeDelegate(this.DelegateSqueeze));
		this.trackController = TrackController.Instance;
		this.characterController = Game.Charactercontroller;
		this.running = Running.Instance;
		this.revive = Revive.Instance;
		this.revive.OnRevive += this.HandleRevive;
		base.GetComponent<CharacterRendering>().Initialize();
		this.superShoes = SuperShoes.Instance;
		this.characterRoot = this.characterModel.transform;
		this.helmet = Helmet.Instance;
		this.helmet.OnJumpFromWater += this.OnJumpFromWater;
		this.characterCamera = CharacterCamera.Instance;
		this.boss = Boss.Instance;
		this.CharacterPickupParticleSystem = base.GetComponentInChildren<PickupParticles>();
		this.characterColliderTrigger = this.characterCollider.GetComponent<OnTriggerObject>();
		this.characterColliderTrigger.OnEnter = (OnTriggerObject.OnEnterDelegate)Delegate.Combine(this.characterColliderTrigger.OnEnter, new OnTriggerObject.OnEnterDelegate(this.OnCharacterColliderEnter));
		this.characterColliderTrigger.OnExit = (OnTriggerObject.OnExitDelegate)Delegate.Combine(this.characterColliderTrigger.OnExit, new OnTriggerObject.OnExitDelegate(this.OnCharacterColliderExit));
		this.characterControllerCenter = this.characterController.center;
		this.characterControllerHeight = this.characterController.height;
		this.characterColliderCenter = this.characterCollider.center;
		this.characterColliderHeight = this.characterCollider.height;
		this.OnLanding = (Character.OnLandingDelegate)Delegate.Combine(this.OnLanding, new Character.OnLandingDelegate(TasksManager.Instance.OnChangeIsCharacterOnGround));
	}

	private bool IsRunningOnGround()
	{
		return this.running.currentRunPosition == Running.RunPositions.ground;
	}

	public void Jump()
	{
		if (this.isInsideUnderpass)
		{
			return;
		}
		bool flag = !this.isJumping && this.verticalSpeed <= 0f && this.verticalSpeed > this.verticalSpeed_jumpTolerance;
		if (this.characterController.isGrounded || (flag && !this.running.transitionFromHeight))
		{
			this.isJumping = true;
			this.isFalling = false;
			this.IsGrounded.Value = false;
			if (this.isJumpingHigher)
			{
				this.isJumpingWithSuperShoes = true;
				Vector3 position = base.transform.position;
				Character.JumpCurveData value = default(Character.JumpCurveData);
				value.z_start = position.z;
				value.z_length = this.JumpLength(this.game.currentSpeed, this.jumpHeightSuperShoes) * this.superShoesJumpApexRatio;
				value.z_end = value.z_start + value.z_length;
				value.y_start = position.y;
				this.superShoesJump = new Character.JumpCurveData?(value);
				this.verticalSpeed = 0f;
			}
			else
			{
				this.verticalSpeed = this.CalculateJumpVerticalSpeed(this.jumpHeight);
				this.NotifyOnJump();
			}
			if (this.IsRunningOnGround())
			{
				this.startedJumpFromGround = true;
				this.trainJump = false;
				this.trainJumpSampleZ = this.z + this.trainJumpSampleLength;
			}
		}
		else if (this.verticalSpeed < 0f)
		{
			this.inAirJump = true;
		}
	}

	public float JumpLength(float speed, float jumpHeight)
	{
		return speed * 2f * this.CalculateJumpVerticalSpeed(jumpHeight) / this.gravity;
	}

	private void KillSwitchOn()
	{
		this.game.WillDie();
	}

	private IEnumerator MoveCharacterToPosition(float newX, float newZ, float time)
	{
		float oldX = this.x;
		float oldZ = this.z;
		float oldY = base.transform.position.y;
		this.game.ChangeState(null);
		this.immuneToCriticalHit = true;
		this.stopColliding = true;
		this.characterController.enabled = false;
		this.NotifyOnTutorialMoveBackToCheckPoint(time);
		yield return base.StartCoroutine(myTween.To(time, delegate(float t)
		{
			this.x = Mathf.SmoothStep(oldX, newX, t);
			this.z = Mathf.SmoothStep(oldZ, newZ, t);
			this.characterController.transform.position = this.trackController.GetPosition(this.x, this.z) + Vector3.up * Mathf.SmoothStep(oldY, 0f, Mathf.Sin(t * 90f));
			this.characterCamera.UpdatePosition(this.characterController.transform.position, Quaternion.identity, Time.deltaTime, false);
		}));
		this.immuneToCriticalHit = false;
		this.characterController.enabled = true;
		this.NotifyOnTutorialStartFromCheckPoint();
		this.stopColliding = false;
		this.ResetWalls();
		this.game.ChangeState(this.game.Running);
		yield break;
	}

	public void HandleWaterBoard()
	{
		if (this.OnHandleWaterBoard != null)
		{
			this.OnHandleWaterBoard(this.trackIndexTarget);
		}
	}

	public void MoveForward()
	{
		Vector3 position = base.transform.position;
		float num = this.z + this.game.currentSpeed * Time.deltaTime;
		Vector3 a = this.verticalSpeed * Time.deltaTime * Vector3.up;
		Vector3 position2 = this.trackController.GetPosition(this.x, num);
		Vector3 b = new Vector3(position.x, 0f, position.z);
		if (this.superShoesJump != null)
		{
			Character.JumpCurveData value = this.superShoesJump.Value;
			if (this.z < value.z_end)
			{
				float num2 = this.superSneakersJumpCurve.Evaluate((num - value.z_start) / value.z_length) * this.jumpHeightSuperShoes + value.y_start;
				float d = num2 - position.y;
				a = Vector3.up * d;
			}
			else
			{
				this.superShoesJump = null;
				this.verticalSpeed = 0f;
				a = Vector3.zero;
			}
		}
		if (this.isStairing && !this.isJumping)
		{
			a = Vector3.down * 100f;
		}
		Vector3 b2 = position2 - b;
		if (this.characterController.enabled)
		{
			this.characterController.Move(a + b2);
		}
		else
		{
			this.characterController.transform.position += b2;
		}
		this.z = base.transform.position.z;
		if (this.characterController.isGrounded)
		{
			this.lastGroundedY = position.y;
		}
	}

	public void MoveWithGravity()
	{
		if (this.characterController.enabled)
		{
			this.verticalSpeed -= this.gravity * Time.deltaTime;
			if (this.verticalSpeed > 0f)
			{
				this.verticalSpeed = 0f;
			}
			Vector3 motion = this.verticalSpeed * Time.deltaTime * Vector3.up;
			this.characterController.Move(motion);
		}
	}

	private void NotifyCriticalHit()
	{
		Character.CriticalHitType type = Character.CriticalHitType.None;
		if (this.OnCriticalHit == null)
		{
			return;
		}
		string text = this.lastHitTag;
		if (text != null && text != null)
		{
			if (!(text == "HitTrain"))
			{
				if (!(text == "HitBarrier"))
				{
					if (!(text == "HitMovingTrain"))
					{
						if (text == "FallIntoWater")
						{
							type = Character.CriticalHitType.FallIntoWater;
						}
					}
					else
					{
						type = Character.CriticalHitType.MovingTrain;
					}
				}
				else
				{
					type = Character.CriticalHitType.Barrier;
				}
			}
			else
			{
				type = Character.CriticalHitType.Train;
			}
		}
		this.OnCriticalHit(type);
	}

	private void NotifyOnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (this.OnChangeTrack != null)
		{
			this.OnChangeTrack(direction);
		}
	}

	private void NotifyOnCriticalHit(Character.CriticalHitType type)
	{
		if (this.OnCriticalHit != null)
		{
			this.OnCriticalHit(type);
		}
	}

	private void NotifyOnHangtime()
	{
		if (this.OnHangtime != null)
		{
			this.OnHangtime();
		}
	}

	private void NotifyOnFallIntoWater()
	{
		if (this.OnFallIntoWater != null)
		{
			this.OnFallIntoWater();
		}
	}

	private void NotifyOnHitByTrain()
	{
		if (this.OnHitByTrain != null)
		{
			this.OnHitByTrain();
		}
	}

	private void NotifyOnJump()
	{
		if (this.OnJump != null)
		{
			this.OnJump();
		}
	}

	private void NotifyOnJumpIfHitByTrain()
	{
		if (this.OnJumpIfHitByTrain != null)
		{
			this.OnJumpIfHitByTrain();
		}
	}

	private void NotifyOnJumpOverTrain()
	{
		if (this.OnJumpOverTrain != null)
		{
			this.OnJumpOverTrain();
		}
	}

	private void NotifyOnLanding()
	{
		if (this.OnLanding != null)
		{
			this.OnLanding(base.transform);
		}
	}

	private void NotifyOnRoll()
	{
		if (this.OnRoll != null)
		{
			this.OnRoll();
		}
	}

	private void NotifyOnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		if (this.OnStumble != null)
		{
			this.OnStumble(stumbleType, horizontalHit, verticalHit, colliderName);
		}
	}

	private void NotifyOnTutorialMoveBackToCheckPoint(float duration)
	{
		if (this.OnTutorialMoveBackToCheckPoint != null)
		{
			this.OnTutorialMoveBackToCheckPoint(duration);
		}
	}

	private void NotifyOnTutorialStartFromCheckPoint()
	{
		if (this.OnTutorialStartFromCheckPoint != null)
		{
			this.OnTutorialStartFromCheckPoint();
		}
	}

	public void NotifyPickup(IPickup pickup)
	{
		if (pickup != null)
		{
			pickup.NotifyPickup(this.CharacterPickupParticleSystem);
		}
	}

	private Character.ObstacleType ObstacleTypeByTag(string tag)
	{
		if (tag != null && tag != null)
		{
			if (tag == "JumpTrain")
			{
				return Character.ObstacleType.JumpTrain;
			}
			if (tag == "RollBarrier")
			{
				return Character.ObstacleType.RollBarrier;
			}
			if (tag == "JumpBarrier")
			{
				return Character.ObstacleType.JumpBarrier;
			}
			if (tag == "JumpHighBarrier")
			{
				return Character.ObstacleType.JumpHighBarrier;
			}
		}
		return Character.ObstacleType.None;
	}

	private void OnCharacterColliderEnter(Collider collider)
	{
		if (this.game.IsInGame.Value)
		{
			if (collider.CompareTag("Underpass"))
			{
				this.underpassColliders.Add(collider);
				this.isInsideUnderpass = (this.underpassColliders.Count > 0);
			}
			else if (collider.CompareTag("Subway"))
			{
				this.subwayColliders.Add(collider);
				this.isInsideSubway = (this.subwayColliders.Count > 0);
				this.subwayMaxY = collider.bounds.max.y - 3f;
			}
			else if (collider.CompareTag("OneWay") && !this.game.IsInFlypackMode)
			{
				this.onewayColliders.Add(collider);
				this.isInOneway = (this.onewayColliders.Count > 0);
				this.onewayId = Mathf.FloorToInt(collider.bounds.center.x / 20f) + 1;
			}
			else if (collider.CompareTag("Speedup"))
			{
				this.speedupColliders.Add(collider);
				this.isInspeedup = (this.speedupColliders.Count > 0);
				this.game.StartSpeedUp();
			}
			else if (!this.stopColliding && collider.gameObject.layer != this.layers.KeepOnHelmet)
			{
				IPickup componentInChildren = collider.GetComponentInChildren<IPickup>();
				if (componentInChildren != null)
				{
					this.NotifyPickup(componentInChildren);
					return;
				}
				ITouchByCharacter componentInChildren2 = collider.GetComponentInChildren<ITouchByCharacter>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.BeTouched();
					return;
				}
				Wall componentInChildren3 = collider.GetComponentInChildren<Wall>();
				if (componentInChildren3 != null)
				{
					if (componentInChildren3.swipeDir == SwipeDir.Left)
					{
						this.walls[0] = componentInChildren3;
					}
					else
					{
						this.walls[1] = componentInChildren3;
					}
					componentInChildren3.OnTrigger();
					return;
				}
				if (collider.gameObject.layer == this.layers.Default)
				{
					if (collider.isTrigger && this.characterController.isGrounded)
					{
						this.IsGrounded.Value = true;
					}
					if (collider.isTrigger)
					{
						Character.ObstacleType obstacleType = this.ObstacleTypeByTag(collider.tag);
						if (obstacleType != Character.ObstacleType.None)
						{
							this.lastObstacleTriggerType = obstacleType;
							this.lastObstacleTriggerTrackIndex = this.trackIndex;
						}
					}
				}
				else if (collider.isTrigger)
				{
					if ("bush" == collider.name)
					{
						this.HandleStumble(Character.StumbleType.Bush, Character.StumbleHorizontalHit.Center, Character.StumbleVerticalHit.Lower, collider.name);
					}
					else if ("water" == collider.name)
					{
						this.lastHitTag = collider.tag;
						this.FallIntoWater();
						this.NotifyCriticalHit();
					}
					else
					{
						this.HandleStumble(Character.StumbleType.Normal, Character.StumbleHorizontalHit.Center, Character.StumbleVerticalHit.Middle, collider.name);
					}
				}
				else
				{
					this.lastHitTag = collider.tag;
					if (!this.game.IsInFlypackMode)
					{
						if (this.lastHitTag == "HitLeftBoundSide")
						{
							this.ChangeTrack(1, 0.2f);
							this.forceToOneway = true;
							return;
						}
						if (this.lastHitTag == "HitRightBoundSide")
						{
							this.ChangeTrack(-1, 0.2f);
							this.forceToOneway = true;
							return;
						}
					}
					Character.ImpactX impactX = this.GetImpactX(collider);
					Character.ImpactY impactY = this.GetImpactY(collider);
					Character.ImpactZ impactZ = this.GetImpactZ(collider);
					float num = (collider.bounds.min.x + collider.bounds.max.x) / 2f;
					float num2 = base.transform.position.x;
					int num3;
					if (num2 < num)
					{
						num3 = 1;
					}
					else if (num2 > num)
					{
						num3 = -1;
					}
					else
					{
						num3 = 0;
					}
					bool flag = num3 == 0 || this.trackMovement == num3;
					bool flag2 = this.characterCollider.bounds.center.z < collider.bounds.min.z;
					bool flag3 = impactZ == Character.ImpactZ.Before && !flag2 && flag;
					if (impactZ == Character.ImpactZ.Middle || flag3)
					{
						if (this.trackMovement != 0)
						{
							float duration = 0.5f;
							if (this.trackController.IsRunningOnTutorialTrack)
							{
								duration = 0.2f;
							}
							this.ChangeTrack(-this.trackMovement, duration);
						}
						if (impactX != Character.ImpactX.Left)
						{
							if (impactX == Character.ImpactX.Right)
							{
								this.HandleStumble(Character.StumbleType.Normal, Character.StumbleHorizontalHit.Right, Character.StumbleVerticalHit.Middle, collider.name);
							}
						}
						else
						{
							this.HandleStumble(Character.StumbleType.Normal, Character.StumbleHorizontalHit.Left, Character.StumbleVerticalHit.Middle, collider.name);
						}
					}
					else if (impactX == Character.ImpactX.Middle || this.trackMovement == 0)
					{
						if (impactZ == Character.ImpactZ.Before)
						{
							if (impactY == Character.ImpactY.Lower)
							{
								this.verticalSpeed = this.CalculateJumpVerticalSpeed(8f);
								this.HandleStumble(Character.StumbleType.Normal, Character.StumbleHorizontalHit.Center, Character.StumbleVerticalHit.Lower, collider.name);
							}
							else if (collider.gameObject.CompareTag("HitMovingTrain"))
							{
								this.HitByTrainSequence();
								this.NotifyCriticalHit();
							}
							else if (impactY == Character.ImpactY.Middle)
							{
								this.HandleStumble(Character.StumbleType.Normal, Character.StumbleHorizontalHit.Center, Character.StumbleVerticalHit.Middle, collider.name);
								this.NotifyCriticalHit();
							}
							else
							{
								this.HandleStumble(Character.StumbleType.Normal, Character.StumbleHorizontalHit.Center, Character.StumbleVerticalHit.Upper, collider.name);
								this.NotifyCriticalHit();
							}
						}
					}
					else
					{
						if (impactZ == Character.ImpactZ.Before && flag)
						{
							if (collider.gameObject.CompareTag("HitMovingTrain"))
							{
								this.HitByTrainSequence();
								this.NotifyCriticalHit();
							}
							else if (collider.gameObject.layer == this.layers.HitBounceOnly)
							{
								this.HandleStumble(Character.StumbleType.Normal, Character.StumbleHorizontalHit.Center, Character.StumbleVerticalHit.Lower, collider.name);
							}
							else
							{
								this.ForceChangeTrack(-this.trackMovement, 0.5f, false);
							}
						}
						else if (collider.gameObject.layer == this.layers.HitBounceOnly)
						{
							this.ForceChangeTrack(-this.trackMovement, 0.5f, false);
						}
						if (impactX == Character.ImpactX.Left)
						{
							this.HandleStumble(Character.StumbleType.Normal, Character.StumbleHorizontalHit.LeftCorner, Character.StumbleVerticalHit.Middle, collider.name);
						}
						else if (impactX == Character.ImpactX.Right)
						{
							this.HandleStumble(Character.StumbleType.Normal, Character.StumbleHorizontalHit.RightCorner, Character.StumbleVerticalHit.Middle, collider.name);
						}
					}
				}
			}
		}
	}

	private void OnCharacterColliderExit(Collider collider)
	{
		if (collider.CompareTag("Underpass"))
		{
			if (this.underpassColliders.Contains(collider))
			{
				this.underpassColliders.Remove(collider);
				this.isInsideUnderpass = (this.underpassColliders.Count > 0);
			}
		}
		else if (collider.CompareTag("Subway"))
		{
			if (this.subwayColliders.Contains(collider))
			{
				this.subwayColliders.Remove(collider);
				this.isInsideSubway = (this.subwayColliders.Count > 0);
			}
		}
		else if (collider.CompareTag("OneWay"))
		{
			if (this.onewayColliders.Contains(collider))
			{
				this.onewayColliders.Remove(collider);
				this.isInOneway = (this.onewayColliders.Count > 0);
				this.forceToOneway = this.isInOneway;
			}
		}
		else if (collider.CompareTag("Speedup"))
		{
			if (this.speedupColliders.Contains(collider))
			{
				this.speedupColliders.Remove(collider);
				this.isInspeedup = (this.speedupColliders.Count > 0);
			}
		}
		else
		{
			Character.ObstacleType obstacleType = this.ObstacleTypeByTag(collider.tag);
			if (obstacleType == this.lastObstacleTriggerType && this.lastObstacleTriggerTrackIndex == this.trackIndex && this.OnPassedObstacle != null)
			{
				this.OnPassedObstacle(obstacleType);
			}
			if (obstacleType == Character.ObstacleType.RollBarrier && this.OnRollGuard != null)
			{
				this.OnRollGuard();
			}
		}
	}

	public void Restart()
	{
		this.trackIndex = this.initialTrackIndex;
		this.trackIndexTarget = this.initialTrackIndex;
		this.x = this.trackController.GetTrackX(this.trackIndex);
		this.trackIndexPosition = (float)this.trackIndex;
		this.z = 0f;
		this.trackMovement = 0;
		this.trackMovementNext = 0;
		this.squeezeCollider.Clear();
		this.characterController.Move(-5f * Vector3.up);
		this.verticalSpeed = 0f;
		this.superShoesJump = null;
		this.jumpHeight = this.jumpHeightNormal;
		this.inAirJump = false;
		this.isJumping = false;
		this.isJumpingWithSuperShoes = false;
		this.isRolling = false;
		this.IsGrounded.Value = false;
		this.lastGroundedY = 0f;
		this.boss.Restart(true);
		this.StartStumble();
		this.startedJumpFromGround = false;
		this.sameLaneTimeStamp = Time.time;
		this.subwayColliders.Clear();
		this.underpassColliders.Clear();
		this.walls[0] = null;
		this.walls[1] = null;
		this.onewayColliders.Clear();
		this.speedupColliders.Clear();
		this.isInsideSubway = false;
		this.isInsideUnderpass = false;
		this.isInOneway = false;
		this.forceToOneway = false;
		this.isInspeedup = false;
		this.onewayId = -1;
	}

	public void Roll()
	{
		if (!this.isRolling)
		{
			if (this.superShoesJump != null)
			{
				this.superShoesJump = null;
			}
			this.squeezeCollider.Add(this);
			if (!this.running.transitionFromHeight)
			{
				this.verticalSpeed = -this.CalculateJumpVerticalSpeed(this.jumpHeight);
			}
			else
			{
				this.verticalSpeed = -250f;
			}
			this.isRolling = true;
			this.NotifyOnRoll();
		}
	}

	private void OnJumpFromWater()
	{
		this.IsJumping = true;
		this.IsFalling = false;
		this.verticalSpeed = this.CalculateJumpVerticalSpeed(60f);
	}

	public void SetBackToCheckPoint(float zoomTime)
	{
		float lastCheckPoint = this.trackController.GetLastCheckPoint(this.z);
		this.trackIndex = this.initialTrackIndex;
		this.trackIndexTarget = this.initialTrackIndex;
		float trackX = this.trackController.GetTrackX(this.trackIndex);
		this.trackIndexPosition = (float)this.trackIndex;
		this.trackMovement = 0;
		this.trackMovementNext = 0;
		base.StartCoroutine(this.MoveCharacterToPosition(trackX, lastCheckPoint, zoomTime));
	}

	public void SetFrontToCheckPoint()
	{
		TrackPiece.TrackCheckPoint nextCheckPoint = this.trackController.GetNextCheckPoint(this.z);
		if (nextCheckPoint == null)
		{
			return;
		}
		this.trackIndex = this.initialTrackIndex;
		this.trackIndexTarget = this.initialTrackIndex;
		this.trackIndexPosition = (float)this.trackIndex;
		this.trackMovement = 0;
		this.trackMovementNext = 0;
		this.x = this.trackController.GetTrackX(this.trackIndex);
		this.z = nextCheckPoint.Z;
		base.transform.position = this.trackController.GetPosition(this.x, this.z) + Vector3.up * nextCheckPoint.y;
		this.characterCamera.Reset(base.transform.position, Quaternion.identity, true);
	}

	private void StartStumble()
	{
		this.isStumbling = true;
		if (!this.trackController.IsRunningOnTutorialTrack)
		{
			this.boss.CatchUp();
		}
		this.boss.StartCoroutine(this.StumbleDecay());
	}

	public void StopStumble()
	{
		this.boss.ResetCatchUp();
		this.isStumbling = false;
	}

	private IEnumerator StumbleDecay()
	{
		yield return new WaitForSeconds(this.stumbleDecayTime);
		this.StopStumble();
		yield break;
	}

	public Wall CanWallWithSwipeDir(SwipeDir dir)
	{
		if (!this.characterController.isGrounded)
		{
			return null;
		}
		Wall wall;
		if (dir == SwipeDir.Left)
		{
			wall = this.walls[0];
		}
		else
		{
			wall = this.walls[1];
		}
		if (wall != null && wall.Bounds.Contains(base.transform.position))
		{
			return wall;
		}
		return null;
	}

	public void WallEndWithSwipeDir(SwipeDir dir)
	{
		if (dir == SwipeDir.Left)
		{
			this.walls[0] = null;
		}
		this.walls[1] = null;
	}

	public void ResetWalls()
	{
		if (this.walls[0] != null)
		{
			this.walls[0].Collider.enabled = true;
		}
		this.walls[0] = null;
		if (this.walls[1] != null)
		{
			this.walls[1].Collider.enabled = true;
		}
		this.walls[1] = null;
	}

	public void ResetOneway()
	{
		this.onewayColliders.Clear();
		this.isInOneway = false;
		this.forceToOneway = false;
	}

	public void Update()
	{
		Vector3 position = base.transform.position;
		if (position.y < -200f)
		{
			position.y = -199f;
			base.transform.position = position;
		}
	}

	public static Character Instance
	{
		get
		{
			if (Character.instance == null)
			{
				Character.instance = (UnityEngine.Object.FindObjectOfType(typeof(Character)) as Character);
			}
			return Character.instance;
		}
	}

	public int TrackIndexTarget
	{
		get
		{
			return this.trackIndexTarget;
		}
	}

	public bool IsAboveGround
	{
		get
		{
			return base.transform.position.y > 20f;
		}
	}

	public bool IsFalling
	{
		get
		{
			return this.isFalling;
		}
		set
		{
			this.isFalling = value;
		}
	}

	public bool isStairing
	{
		get
		{
			return this.running.inStair;
		}
	}

	public bool IsInsideSubway
	{
		get
		{
			return this.isInsideSubway;
		}
	}

	public bool IsJumpingHigher
	{
		get
		{
			return this.isJumpingHigher;
		}
		set
		{
			this.isJumpingHigher = value;
		}
	}

	public bool InSuperShoesJump
	{
		get
		{
			return this.isJumpingHigher || this.superShoesJump != null || this.isJumpingWithSuperShoes;
		}
	}

	public bool IsInspeedup
	{
		get
		{
			return this.isInspeedup;
		}
	}

	public bool IsInsideUnderpass
	{
		get
		{
			return this.isInsideUnderpass;
		}
	}

	public bool IsJumping
	{
		get
		{
			return this.isJumping;
		}
		set
		{
			this.isJumping = value;
		}
	}

	public bool IsRolling
	{
		get
		{
			return this.isRolling;
		}
	}

	public bool IsStumbling
	{
		get
		{
			return this.isStumbling;
		}
		set
		{
			this.isStumbling = value;
		}
	}

	public int TrackIndex
	{
		get
		{
			return this.trackIndex;
		}
	}

	public Transform characterRoot;

	public CapsuleCollider characterCollider;

	public OnTriggerObject coinMagnetCollider;

	public OnTriggerObject coinMagnetLongCollider;

	[SerializeField]
	private float characterAngle = 45f;

	public ParticleSystem helmetCrashParticleSystem;

	public PickupParticles CharacterPickupParticleSystem;

	public float ColliderTrackWidth = 17f;

	[HideInInspector]
	public CharacterController characterController;

	[HideInInspector]
	public OnTriggerObject characterColliderTrigger;

	[HideInInspector]
	public CharacterModel characterModel;

	[HideInInspector]
	public CharacterCamera characterCamera;

	[HideInInspector]
	public Helmet helmet;

	[HideInInspector]
	public SuperShoes superShoes;

	[HideInInspector]
	public Running running;

	[HideInInspector]
	public bool immuneToCriticalHit;

	public int trackIndex;

	public float x;

	public float z;

	public float verticalSpeed;

	public float lastGroundedY;

	public float subwayMaxY;

	public float underpassMaxY;

	private float jumpHeight;

	public float gravity = 200f;

	public float jumpHeightNormal = 20f;

	public float jumpHeightSuperShoes = 40f;

	public float verticalFallSpeedLimit = -1f;

	public float stumbleCornerTolerance = 15f;

	public float stumbleDecayTime = 5f;

	public bool inAirJump;

	public Variable<bool> IsGrounded;

	[SerializeField]
	private AnimationCurve superSneakersJumpCurve;

	[SerializeField]
	private float superShoesJumpApexRatio;

	[HideInInspector]
	public bool stopColliding;

	public float sameLaneTimeStamp;

	private Vector3 characterColliderCenter;

	private float characterColliderHeight;

	private Vector3 characterControllerCenter;

	private float characterControllerHeight;

	private float characterRotation;

	private Game game;

	private Boss boss;

	private int initialTrackIndex = 1;

	private static Character instance;

	private bool isFalling;

	private bool isJumping;

	private bool isJumpingWithSuperShoes;

	private bool isJumpingHigher;

	private bool isRolling;

	private bool isStumbling;

	private string lastHitTag;

	private int onewayId;

	private int lastObstacleTriggerTrackIndex;

	private Character.ObstacleType lastObstacleTriggerType;

	private float lastZ;

	private Layers layers;

	private Revive revive;

	private VariableBool squeezeCollider;

	private bool startedJumpFromGround;

	private HashSet<Collider> subwayColliders;

	private bool isInsideSubway;

	private HashSet<Collider> underpassColliders;

	private bool isInsideUnderpass;

	private Wall[] walls;

	private HashSet<Collider> onewayColliders;

	private bool isInOneway;

	private HashSet<Collider> speedupColliders;

	private bool isInspeedup;

	[HideInInspector]
	public bool forceToOneway;

	private Character.JumpCurveData? superShoesJump;

	private TrackController trackController;

	public float trackIndexPosition;

	private int trackIndexTarget;

	private int trackMovement;

	private int trackMovementNext;

	private bool trainJump;

	private float trainJumpSampleLength;

	private float trainJumpSampleZ;

	private float verticalSpeed_jumpTolerance;

	public enum CriticalHitType
	{
		Train,
		Barrier,
		MovingTrain,
		FallIntoWater,
		None
	}

	private enum ImpactX
	{
		Left,
		Middle,
		Right
	}

	private enum ImpactY
	{
		Upper,
		Middle,
		Lower
	}

	private enum ImpactZ
	{
		Before,
		Middle,
		After
	}

	public enum ObstacleType
	{
		JumpHighBarrier,
		JumpTrain,
		RollBarrier,
		JumpBarrier,
		None
	}

	public enum OnChangeTrackDirection
	{
		Left,
		Right
	}

	public delegate void OnChangeTrackDelegate(Character.OnChangeTrackDirection direction);

	public delegate void OnCriticalHitDelegate(Character.CriticalHitType type);

	public delegate void OnHangtimeDelegate();

	public delegate void OnHitByTrainDelegate();

	public delegate void OnFallIntoWaterDelegate();

	public delegate void OnJumpDelegate();

	public delegate void OnJumpIfHitByTrainDelegate();

	public delegate void OnJumpOverTrainDelegate();

	public delegate void OnLandingDelegate(Transform characterTransform);

	public delegate void OnPassedObstacleDelegate(Character.ObstacleType type);

	public delegate void OnRollDelegate();

	public delegate void OnRollGuardDelegate();

	public delegate void OnStumbleDelegate(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName);

	public delegate void OnTutorialMoveBackToCheckPointDelegate(float duration);

	public delegate void OnTutorialStartFromCheckPointDelegate();

	public delegate void OnHandleWaterBoardDelegate(int x);

	public enum StumbleHorizontalHit
	{
		Left,
		LeftCorner,
		Center,
		RightCorner,
		Right
	}

	public enum StumbleType
	{
		Normal,
		Bush,
		Side
	}

	public enum StumbleVerticalHit
	{
		Upper,
		Middle,
		Lower
	}

	private struct JumpCurveData
	{
		public float z_start;

		public float z_length;

		public float z_end;

		public float y_start;
	}
}
