using System;
using System.Collections;
using UnityEngine;

public class Raft : ICharacterAttachment
{
	public Raft()
	{
		this.character = Character.Instance;
		this.characterModel = this.character.characterModel;
		this.roftRoot = this.characterModel.BoneHelmet.gameObject;
	}

	public event Raft.OnEndRaftDelegate _OnEndRaft;

	public event Raft.OnSwitchToRoftDelegate OnSwitchToRoft;

	private void OnEndRaft()
	{
		if (this._OnEndRaft != null)
		{
			this._OnEndRaft();
		}
	}

	public IEnumerator Begain()
	{
		if (this.isAllowed)
		{
			TasksManager.Instance.PlayerDidThis(TaskTarget.Raft, 1, -1);
			this.Paused = false;
			if (this.character.IsStumbling)
			{
				this.character.StopStumble();
			}
			this.IsActive = true;
			this.roftRoot.SetActive(true);
			this.characterModel.SetRaft(this.raft);
			if (this.OnSwitchToRoft != null)
			{
				this.OnSwitchToRoft(this.raft);
			}
			this.character.immuneToCriticalHit = true;
			this.Stop = StopFlag.DONT_STOP;
			while (this.Stop == StopFlag.DONT_STOP)
			{
				yield return null;
			}
			this.IsActive = false;
			this.characterModel.RemoveRaft();
			this.character.immuneToCriticalHit = false;
			this.OnEndRaft();
			if (this.Stop == StopFlag.STOP)
			{
				this.IsActive = false;
			}
			this.character.IsJumping = true;
			this.character.IsFalling = false;
			this.character.verticalSpeed = this.character.CalculateJumpVerticalSpeed(10f);
		}
		yield break;
	}

	public void Pause()
	{
		this.Paused = true;
		this.roftRoot.SetActive(false);
	}

	public void Reset()
	{
		this.character.immuneToCriticalHit = true;
		this.character.characterController.enabled = true;
		this.character.characterCollider.enabled = true;
		this.roftRoot.SetActive(true);
	}

	public void Resume()
	{
		this.Paused = false;
		this.roftRoot.SetActive(true);
	}

	public static Raft Instance
	{
		get
		{
			if (Raft.instance == null)
			{
				Raft.instance = new Raft();
			}
			return Raft.instance;
		}
	}

	public bool ShouldPauseInFlypack
	{
		get
		{
			return true;
		}
	}

	public IEnumerator Current { get; set; }

	public bool Paused { get; set; }

	public StopFlag Stop { get; set; }

	public bool IsActive { get; private set; }

	private static Raft instance;

	public bool isAllowed = true;

	public GameObject raft;

	private Character character;

	private CharacterModel characterModel;

	private GameObject roftRoot;

	public delegate void OnEndRaftDelegate();

	public delegate void OnSwitchToRoftDelegate(GameObject roft);
}
