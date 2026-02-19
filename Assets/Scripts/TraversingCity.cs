using System;
using System.Collections;
using UnityEngine;

public class TraversingCity : CharacterState
{
	private void Awake()
	{
		this.character = Character.Instance;
		this.characterRendering = CharacterRendering.Instance;
		this.characterTransform = this.character.transform;
		this.game = Game.Instance;
		this.characterCamera = CharacterCamera.Instance;
		this.trackController = TrackController.Instance;
		this.progress = 0f;
		this.ao = null;
	}

	public override IEnumerator Begin()
	{
		this.progress = 0f;
		this.ao = null;
		FadeData fadeData = this.cameraCulling.SetToAndThenBack();
		float factor = 0f;
		while (factor < fadeData.fadeInDuration)
		{
			factor += Time.deltaTime;
			yield return null;
		}
		factor = 0f;
		this.trackController.ChangeToNextCity(true);
		this.trackController.SetCharacterPosition(this.character);
		this.position = this.characterTransform.position;
		factor += Time.deltaTime;
		yield return null;
		this.characterCamera.Reset(this.position, Quaternion.identity, true);
		factor += Time.deltaTime;
		yield return null;
		this.game.LayTrackChunks();
		this.characterRendering.ActivateSnow(this.trackController.GetCurrentCity().allowSnow);
		factor += Time.deltaTime;
		yield return null;
		while (factor < fadeData.onDuration - 0.1f)
		{
			factor += Time.deltaTime;
			yield return null;
		}
		this.game.ChangeState(this.game.Running);
		yield break;
	}

	public static TraversingCity Instance
	{
		get
		{
			if (TraversingCity.instance == null)
			{
				TraversingCity.instance = (UnityEngine.Object.FindObjectOfType(typeof(TraversingCity)) as TraversingCity);
			}
			return TraversingCity.instance;
		}
	}

	[SerializeField]
	private Running.RunPositions currentRunPosition;

	private Character character;

	private CharacterRendering characterRendering;

	private Transform characterTransform;

	private CharacterController characterController;

	private Game game;

	private CharacterCamera characterCamera;

	[SerializeField]
	private CameraCulling cameraCulling;

	private TrackController trackController;

	private Vector3 position;

	private AsyncOperation ao;

	private float progress;

	private string nextCitySceneName;

	private static TraversingCity instance;
}
