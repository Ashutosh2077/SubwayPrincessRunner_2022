using System;
using UnityEngine;

public class CustomizeControl : MonoBehaviour
{
	private void Awake()
	{
		this.anim = base.GetComponentInParent<Animation>();
		this.eyeAnima = base.transform.GetChild(0).GetComponent<AvatarEyeAnimation>();
	}

	private void OnEnable()
	{
		if (!this.adjustAnimation)
		{
			this.ResetTransform();
			return;
		}
		this.anim.AddClip(this.animClip, this.animClip.name);
		this.anim.clip = this.animClip;
		if (this.totalFrame <= 0)
		{
			this.anim[this.animClip.name].time = 0f;
		}
		else
		{
			this.anim[this.animClip.name].time = (float)this.frame / (float)this.totalFrame;
		}
		this.anim[this.animClip.name].speed = 0f;
		this.anim.Play();
	}

	private void ResetTransform()
	{
		if (this.clipdata == null)
		{
			return;
		}
		Transform transform = this.anim.transform;
		for (int i = 0; i < this.clipdata.datas.Length; i++)
		{
			Transform transform2 = transform.Find(this.clipdata.datas[i].path);
			ClipData clipData = this.clipdata.datas[i];
			string propertyName = clipData.propertyName;
			switch (propertyName)
			{
			case "m_LocalRotation.x":
			{
				Quaternion localRotation = transform2.localRotation;
				localRotation.x = clipData.startValue;
				transform2.localRotation = localRotation;
				break;
			}
			case "m_LocalRotation.y":
			{
				Quaternion localRotation = transform2.localRotation;
				localRotation.y = clipData.startValue;
				transform2.localRotation = localRotation;
				break;
			}
			case "m_LocalRotation.z":
			{
				Quaternion localRotation = transform2.localRotation;
				localRotation.z = clipData.startValue;
				transform2.localRotation = localRotation;
				break;
			}
			case "m_LocalRotation.w":
			{
				Quaternion localRotation = transform2.localRotation;
				localRotation.w = clipData.startValue;
				transform2.localRotation = localRotation;
				break;
			}
			case "m_LocalPosition.x":
			{
				Vector3 localPosition = transform2.localPosition;
				localPosition.x = clipData.startValue;
				transform2.localPosition = localPosition;
				break;
			}
			case "m_LocalPosition.y":
			{
				Vector3 localPosition = transform2.localPosition;
				localPosition.y = clipData.startValue;
				transform2.localPosition = localPosition;
				break;
			}
			case "m_LocalPosition.z":
			{
				Vector3 localPosition = transform2.localPosition;
				localPosition.z = clipData.startValue;
				transform2.localPosition = localPosition;
				break;
			}
			case "m_LocalScale.x":
			{
				Vector3 localScale = transform2.localScale;
				localScale.x = clipData.startValue;
				transform2.localScale = localScale;
				break;
			}
			case "m_LocalScale.y":
			{
				Vector3 localScale = transform2.localScale;
				localScale.y = clipData.startValue;
				transform2.localScale = localScale;
				break;
			}
			case "m_LocalScale.z":
			{
				Vector3 localScale = transform2.localScale;
				localScale.z = clipData.startValue;
				transform2.localScale = localScale;
				break;
			}
			}
		}
	}

	private void OnDisable()
	{
		if (!this.adjustAnimation)
		{
			return;
		}
		this.anim.RemoveClip(this.animClip);
	}

	public void ChangeSkin(int CustomIndx)
	{
		if (this.Skins != null)
		{
			for (int i = 0; i < this.Skins.Length; i++)
			{
				this.Skins[i].gameObject.SetActive(false);
			}
			if (CustomIndx < this.Skins.Length)
			{
				this.Skins[CustomIndx].gameObject.SetActive(true);
				this.body.sharedMaterial = this.Skins[CustomIndx].sharedMaterial;
				if (this.eyeAnima)
				{
					this.eyeAnima.ChangeMat(this.Skins[CustomIndx].sharedMaterial);
				}
			}
		}
	}

	public SkinnedMeshRenderer body;

	public SkinnedMeshRenderer[] Skins;

	public AnimationClipData clipdata;

	public bool adjustAnimation;

	public AnimationClip animClip;

	public int frame;

	public int totalFrame;

	private Animation anim;

	private AvatarEyeAnimation eyeAnima;
}
