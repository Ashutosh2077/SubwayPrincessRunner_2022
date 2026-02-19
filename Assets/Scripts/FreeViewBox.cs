using System;
using UnityEngine;

public class FreeViewBox : MonoBehaviour
{
	private void Start()
	{
		this.anim = base.GetComponent<Animation>();
		this.game = Game.Instance;
		this.originPos = base.transform.position;
		base.transform.position = Vector3.up * 1000f;
		this.game.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(this.game.OnIntroRun, new Game.OnIntroRunDelegate(this.OnIntroRun));
		this.time = this.wait;
		this.StopParticleSystem();
	}

	private void Update()
	{
		if (this.isShow)
		{
			return;
		}
		if (LoadScene.finished && this.game.IsInTopMenu.Value && "FrontUI".Equals(UIScreenController.Instance.GetTopScreenName()) && UIScreenController.Instance.IsPopupQueueEmpty())
		{
			if (this.time > 0f)
			{
				this.time -= Time.deltaTime;
			}
			else
			{
				this.isShow = true;
				base.transform.position = this.originPos;
				this.anim.Play("Box_fall");
				this.anim.CrossFadeQueued("Prop_box_Alert");
			}
		}
	}

	public void PlayParticleSystem()
	{
		this.particlesHelp.Play();
		AudioPlayer.Instance.PlaySound("prop_box_fall_sfx", true);
	}

	public void StopParticleSystem()
	{
		this.particlesHelp.Stop();
	}

	private void OnIntroRun()
	{
		this.isShow = false;
		this.anim.Stop();
		base.transform.position = Vector3.up * 1000f;
		this.time = this.wait;
		this.StopParticleSystem();
	}

	public void OnMouseUpAsButton()
	{
		if ("FrontUI".Equals(UIScreenController.Instance.GetTopScreenName()) && UIScreenController.Instance.IsPopupQueueEmpty())
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "into_game_box", 0, null);
			UIScreenController.Instance.PushPopup("WatchVideoPopup");
		}
	}

	[SerializeField]
	private float wait;

	[SerializeField]
	private PaticlesHelper particlesHelp;

	private Animation anim;

	private Game game;

	private Vector3 originPos;

	private bool isShow;

	private float time;
}
