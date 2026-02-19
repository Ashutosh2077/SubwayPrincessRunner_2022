using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
	private void OnChangeTrackDir(Character.OnChangeTrackDirection direction)
	{
		this.stats.trackChanges++;
	}

	private void OnCriticalHit(Character.CriticalHitType type)
	{
		switch (type)
		{
		case Character.CriticalHitType.Train:
			this.stats.trainHit++;
			break;
		case Character.CriticalHitType.Barrier:
			this.stats.barrierHit++;
			break;
		case Character.CriticalHitType.MovingTrain:
			this.stats.movingTrainHit++;
			break;
		case Character.CriticalHitType.FallIntoWater:
			this.stats.fallIntoWater++;
			break;
		}
	}

	private void OnJump()
	{
		this.stats.jumps++;
	}

	private void OnJumpOverTrain()
	{
		this.stats.jumpsOverTrains++;
	}

	private void OnPassedObstacle(Character.ObstacleType type)
	{
		switch (type)
		{
		case Character.ObstacleType.JumpHighBarrier:
			this.stats.jumpBarrier++;
			this.stats.jumpHighBarrier++;
			break;
		case Character.ObstacleType.JumpTrain:
			this.stats.jumpsOverTrains++;
			break;
		case Character.ObstacleType.RollBarrier:
			this.stats.dodgeBarrier++;
			break;
		case Character.ObstacleType.JumpBarrier:
			this.stats.jumpBarrier++;
			break;
		}
	}

	private void OnRoll()
	{
		this.stats.rolls++;
		if (this.character.TrackIndex == 0)
		{
			this.stats.rollsLeftTrack++;
		}
		if (this.character.TrackIndex == 1)
		{
			this.stats.rollsCenterTrack++;
		}
		if (this.character.TrackIndex == 2)
		{
			this.stats.rollsRightTrack++;
		}
	}

	private void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		switch (colliderName)
		{
		case "lightSignal":
			TasksManager.Instance.PlayerDidThis(TaskTarget.BumpLightSignal, 1, -1);
			return;
		case "bush":
		case "powerbox":
			TasksManager.Instance.PlayerDidThis(TaskTarget.BumpBush, 1, -1);
			return;
		case "side":
		case "collider":
			return;
		case "collider stumble":
			TasksManager.Instance.PlayerDidThis(TaskTarget.BumpTrain, 1, -1);
			return;
		case "blocker_jump":
		case "blocker_roll":
		case "blocker_standard":
			TasksManager.Instance.PlayerDidThis(TaskTarget.BumpBarrier, 1, -1);
			return;
		}
		TasksManager.Instance.PlayerDidThis(TaskTarget.BumpTrain, 1, -1);
	}

	public void Start()
	{
		this.stats = GameStats.Instance;
		this.character = base.GetComponent<Character>();
		this.character.OnChangeTrack += this.OnChangeTrackDir;
		this.character.OnJump += this.OnJump;
		this.character.OnRoll += this.OnRoll;
		this.character.OnPassedObstacle += this.OnPassedObstacle;
		this.character.OnJumpOverTrain += this.OnJumpOverTrain;
		this.character.OnCriticalHit += this.OnCriticalHit;
		this.character.OnStumble += this.OnStumble;
	}

	private Character character;

	private GameStats stats;
}
