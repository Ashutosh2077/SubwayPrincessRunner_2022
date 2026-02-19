using System;
using System.Collections;
using UnityEngine;

public class KickedInFly : BaseO, ITouchByCharacter
{
	protected override void Awake()
	{
		this.originLPos = base.transform.localPosition;
		this.originLQuat = base.transform.localRotation;
		this.col = base.GetComponent<Collider>();
		base.Awake();
	}

	public override void OnActivate()
	{
		base.enabled = true;
		base.transform.localPosition = this.originLPos;
		base.transform.localRotation = this.originLQuat;
		this.col.enabled = true;
	}

	public bool BeTouched()
	{
		this.col.enabled = false;
		AudioPlayer.Instance.PlaySound(this.kickedClip, true);
		base.StartCoroutine(this.BeKickedOff_C());
		return true;
	}

	private IEnumerator BeKickedOff_C()
	{
		if (base.GetComponent<MovingO>() != null)
		{
			base.GetComponent<MovingO>().enabled = false;
		}
		float startZ = base.transform.position.z;
		float startX = base.transform.position.x;
		Quaternion quat = base.transform.rotation;
		Quaternion to = base.transform.rotation * Quaternion.Euler(-30f, 0f, 90f);
		float rate = 0f;
		float z = startZ;
		while (rate < 1f)
		{
			z = Mathf.Lerp(startZ, 2000f, rate);
			base.transform.position = new Vector3(Mathf.Lerp(startX / z, 0.05f, rate) * z, Mathf.Lerp(0f, 0.2f, rate) * z, z);
			base.transform.rotation = Quaternion.Lerp(quat, to, rate);
			rate += Time.deltaTime / 1.6f;
			yield return null;
		}
		this.OnDeactivate();
		yield break;
	}

	public string kickedClip;

	private Vector3 originLPos;

	private Quaternion originLQuat;

	private Collider col;
}
