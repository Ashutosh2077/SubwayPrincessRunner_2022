using System;
using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
	private void Start()
	{
		this.game = Game.Instance;
		this.helmet = Helmet.Instance;
		if (this.game != null && !this.hasInited)
		{
			this.character = this.game.character;
			this.trackController = this.game.trackController;
			this.hasInited = true;
		}
		if (PlayerInfo.Instance.tutorialCompleted)
		{
			base.enabled = false;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (!this.character.stopColliding && collider.gameObject.layer == Layers.Instance.Character)
		{
			if (this.displayText)
			{
				UISliderInController.Instance.QueueMessage(Strings.Get(this.text));
			}
			if (this.displayMesh)
			{
				base.StartCoroutine(this.ShowMesh());
			}
			this.helmet.isAllowed = this.allowHelmet;
			if (this.endTutorial)
			{
				PlayerPrefs.SetInt("IsNewPlayerStatus", 1);
				PlayerPrefs.SetInt("NewPlayerFirstSaveStatus", 1);
				this.trackController.IsRunningOnTutorialTrack = false;
				PlayerInfo.Instance.tutorialCompleted = true;
				Game.Instance.show20sAd = false;
			}
			base.GetComponent<Collider>().enabled = false;
		}
	}

	private IEnumerator ShowMesh()
	{
		this.Mesh.transform.rotation = Quaternion.AngleAxis(this.meshDir, new Vector3(0f, 0f, 1f)) * Quaternion.Euler(0f, 180f, 0f);
		this.Mesh.SetActive(true);
		Vector3 pos = new Vector3(0f, 0f, 20f);
		yield return base.StartCoroutine(myTween.To(this.time, delegate(float t)
		{
			this.Mesh.transform.localPosition = Vector3.Lerp(pos - this.Mesh.transform.up * 5f, pos + this.Mesh.transform.up * 5f, t);
			this.Mesh.GetComponent<Renderer>().material.mainTextureOffset = Vector2.Lerp(Vector2.zero, new Vector2(0f, -0.035f), t);
			if (!this.game.IsInGame.Value)
			{
				this.Mesh.transform.localPosition = new Vector3(1000f, 1000f, 0f);
			}
		}));
		this.Mesh.SetActive(false);
		yield break;
	}

	private void Update()
	{
		if (PlayerInfo.Instance.tutorialCompleted)
		{
			base.enabled = false;
		}
	}

	private GameObject Mesh
	{
		get
		{
			if (this._mesh == null)
			{
				this._mesh = Camera.main.transform.Find("Arrow").gameObject;
			}
			return this._mesh;
		}
	}

	[SerializeField]
	private bool displayText;

	[SerializeField]
	private string text;

	[SerializeField]
	private bool displayMesh;

	[SerializeField]
	private float meshDir;

	[SerializeField]
	private float time = 1f;

	[SerializeField]
	private bool allowHelmet;

	[SerializeField]
	private bool endTutorial;

	private GameObject _mesh;

	private Character character;

	private Game game;

	private Helmet helmet;

	private bool hasInited;

	private TrackController trackController;
}
