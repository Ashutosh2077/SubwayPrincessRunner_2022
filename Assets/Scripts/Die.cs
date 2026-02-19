using System;
using System.Collections;
using UnityEngine;

public class Die : CharacterState
{
	private void Awake()
	{
		this.game = Game.Instance;
		this.character = Character.Instance;
		this.characterCamera = CharacterCamera.Instance;
	}

	public override IEnumerator Begin()
	{
		float time = 0f;
		this.skipRevive = false;
		if (this.game.HitType != Character.CriticalHitType.FallIntoWater)
		{
			while (!this.character.characterController.isGrounded && time < 1f)
			{
				this.character.MoveWithGravity();
				time += Time.deltaTime;
				this.characterCamera.UpdatePosition(this.character.transform.position, Quaternion.identity, Time.deltaTime, true);
				yield return null;
			}
		}
		else
		{
			while (time < 0.5f)
			{
				time += Time.deltaTime;
				this.characterCamera.UpdatePosition(this.character.transform.position, Quaternion.identity, Time.deltaTime, true);
				yield return null;
			}
		}
		time = 0f;
		while (time < 0.5f)
		{
			time += Time.deltaTime;
			this.characterCamera.UpdatePosition(this.character.transform.position, Quaternion.identity, Time.deltaTime, true);
			yield return null;
		}
		TasksManager.Instance.PlayerDidThis(TaskTarget.TimeDeath, Mathf.FloorToInt(this.game.GetDuration()), -1);
		bool isShowHelpMe = true;
		if (isShowHelpMe)
		{
			UIScreenController.Instance.QueuePopup("SaveMePopup");
		}
		time = 0f;
		while (time < this.waitTimeBeforeScreen)
		{
			if (Input.GetMouseButtonUp(0))
			{
				break;
			}
			if (UnityEngine.Input.touchCount > 0)
			{
				Touch touch = Input.touches[0];
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					break;
				}
			}
			if (this.skipRevive)
			{
				break;
			}
			time += Time.deltaTime;
			yield return null;
		}
		if (isShowHelpMe)
		{
			while (!this.skipRevive)
			{
				yield return null;
			}
		}
		base.StartCoroutine("DelayGameOverScreen");
		this.game.NormalPlayerRunDuration();
		yield break;
	}

	private IEnumerator DelayGameOverScreen()
	{
		if (!PlayerInfo.Instance.hasSubscribed && PlayerPrefs.GetInt("IsNewPlayerStatus") == 1 && Game.Instance.GetNextAdDuration() > 20f)
		{
			Game.Instance.showAdTime = Time.time;
			RiseSdk.Instance.TrackEvent("interstitial_endless", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_endless", 0, null);
			RiseSdk.Instance.ShowAd("passlevel");
			this.game.NewPlayerRunDuration();
			PlayerPrefs.SetInt("IsNewPlayerStatus", 2);
		}
		else if (Game.Instance.GetDuration() > 15f && !PlayerInfo.Instance.hasSubscribed && Game.Instance.GetNextAdDuration() > 20f)
		{
			Game.Instance.showAdTime = Time.time;
			RiseSdk.Instance.TrackEvent("interstitial_endless", "default,default");
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "interstitial_endless", 0, null);
			RiseSdk.Instance.ShowAd("passlevel");
		}
		yield return null;
		UIScreenController.Instance.GameOverTriggered();
		yield return null;
		this.game.StartTopMenu();
		yield break;
	}

	public override bool PauseActiveModifiers
	{
		get
		{
			return true;
		}
	}

	public bool SkipRevive
	{
		set
		{
			this.skipRevive = true;
		}
	}

	public static Die Instance
	{
		get
		{
			if (Die.instance == null)
			{
				Die.instance = (UnityEngine.Object.FindObjectOfType(typeof(Die)) as Die);
			}
			return Die.instance;
		}
	}

	private Game game;

	private Character character;

	private CharacterCamera characterCamera;

	[SerializeField]
	private float waitTimeBeforeScreen = 3f;

	private static Die instance;

	private bool skipRevive;
}
