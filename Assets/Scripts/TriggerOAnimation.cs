using System;
using UnityEngine;

public class TriggerOAnimation : TriggerO
{
	public override void TriggerOnEnter(Collider collider)
	{
		if (collider.gameObject.layer == Layers.Instance.Character)
		{
			if (string.IsNullOrEmpty(this.triggerClip) || this.anim[this.triggerClip] == null)
			{
				UnityEngine.Debug.Log("triggerClip is null Or " + base.name + "'s animation has not triggerClip.");
			}
			this.anim.CrossFade(this.triggerClip, 0.2f);
			if (this.triggerAudio.Clip != null)
			{
				AudioPlayer.Instance.PlaySound(this.triggerAudio.Clip.name, true);
			}
		}
	}

	[SerializeField]
	private string triggerClip;

	[SerializeField]
	private AudioClipInfo triggerAudio;
}
