using System;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
	private void Awake()
	{
		if (this.target == null)
		{
			this.target = new GameObject("Points Target").transform;
		}
		PointsManager.hasShowedModels = new List<string>();
	}

	private void Start()
	{
		if (this.targetAnim == null)
		{
			this.targetAnim = base.GetComponent<PointAnimationCtrl>();
		}
		if (this.targetAnim != null && this.targetAnim.anim == null && this.thing != null)
		{
			this.targetAnim.anim = Character.Instance.characterModel.characterAnimation;
		}
		PointsManager._isApplicationPaused = false;
	}

	private void SetPointStart()
	{
		this.currentPoint = this.list.CurrentPoint;
		if (this.currentPoint != null)
		{
			this.temp = this.currentPoint.transform.position;
			this.temp.y = 0f;
			this.target.position = this.temp;
			this.temp -= this.trans.position;
			this.temp.y = 0f;
			if (this.temp == Vector3.zero)
			{
				this.target.forward = this.currentPoint.transform.forward;
			}
			else
			{
				this.target.forward = this.temp.normalized;
			}
			if (!this.currentPoint.move)
			{
				this.trans.position = this.currentPoint.transform.position;
				this.trans.rotation = this.currentPoint.transform.rotation;
			}
			this.currentPoint.OnStart(this);
		}
	}

	public void Play()
	{
		PointsManager._isApplicationPaused = false;
		PointsManager.hasShowedModels.Clear();
		if (this.model != null && this.thing != null)
		{
			UnityEngine.Object.Destroy(this.thing.gameObject, 0.1f);
			this.thing = null;
		}
		if (this.characterController != null)
		{
			this.characterController.enabled = true;
		}
		base.enabled = true;
		if (this.targetAnim != null && this.targetAnim.anim != null)
		{
			int i = 0;
			int count = this.list.Count;
			while (i < count)
			{
				this.list[i].OnInit(this);
				i++;
			}
		}
		this.list.Play();
		this.SetPointStart();
	}

	public void BreakLoop()
	{
		if (this.currentPoint != null)
		{
			this.currentPoint.OnEnd(this);
		}
		int num = this.list.OnBreak();
		if (num == -1)
		{
			this.OnStop();
			return;
		}
		this.SetPointStart();
	}

	public void OnStop()
	{
		int i = 0;
		int count = this.list.Count;
		while (i < count)
		{
			this.list[i].OnWholeEnd(this);
			i++;
		}
		if (this.onStop != null)
		{
			this.onStop();
		}
		if (base.transform.childCount > 0)
		{
			int j = 0;
			int childCount = base.transform.childCount;
			while (j < childCount)
			{
				Transform child = base.transform.GetChild(j);
				if (child != null)
				{
					UnityEngine.Object.Destroy(child.gameObject, 0.1f);
				}
				j++;
			}
			this.thing = null;
			if (this.targetAnim != null)
			{
				this.targetAnim.anim = null;
			}
			this.refrence = null;
		}
		if (this.thing == null && this.characterController != null)
		{
			this.characterController.enabled = false;
		}
		this.currentPoint = null;
		base.enabled = false;
	}

	public void GoToNextPoint()
	{
		this.currentPoint.OnEnd(this);
		if (this.list.Next() == -1)
		{
			this.OnStop();
			return;
		}
		this.SetPointStart();
	}

	private void Update()
	{
		if (PointsManager._isApplicationPaused)
		{
			return;
		}
		if (this.currentPoint != null)
		{
			bool flag = this.currentPoint.OnUpdate(this);
			if (flag)
			{
				if (this.characterController != null && this.characterController.enabled)
				{
					this.velocity.y = -10f * Time.deltaTime;
					this.characterController.Move(this.velocity);
				}
				else
				{
					this.velocity.y = 0f;
					this.trans.position += this.velocity;
				}
			}
			this.velocity = Vector3.zero;
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			PointsManager._isApplicationPaused = true;
		}
		else
		{
			PointsManager._isApplicationPaused = false;
			GC.Collect();
		}
	}

	public void Check()
	{
		this.temp = this.target.position - this.trans.position;
		this.temp.y = 0f;
		if (this.temp.magnitude < this.pointToPointThreshold || Vector3.Dot(this.temp, this.target.forward) < 0f)
		{
			this.GoToNextPoint();
		}
	}

	public void InitializeCharacterModel(Transform point, string name, int themeId)
	{
		if (this.thing != null)
		{
			return;
		}
		CharacterModelSample characterModelSample = CharacterModelSampleFactory.Instance.BuildCharacterModelSample(name, themeId);
		this.thing = characterModelSample.transform;
		this.thing.parent = base.transform;
		this.targetAnim.anim = this.thing.GetComponentInChildren<Animation>();
		this.thing.localPosition = Vector3.zero;
		this.thing.localRotation = Quaternion.identity;
		this.refrence = characterModelSample.BoneHead;
		PointsManager.hasShowedModels.Add(name);
		int i = 0;
		int count = this.list.Count;
		while (i < count)
		{
			this.list[i].OnInit(this);
			i++;
		}
	}

	public void InitializedModel(Vector3 localPos, Vector3 localRotation)
	{
		this.thing = UnityEngine.Object.Instantiate<GameObject>(this.model).transform;
		this.thing.parent = base.transform;
		this.thing.localPosition = localPos;
		this.thing.localRotation = Quaternion.Euler(localRotation);
		int i = 0;
		int count = this.list.Count;
		while (i < count)
		{
			this.list[i].OnInit(this);
			i++;
		}
	}

	public void PlaySound(AudioClipInfo clip, bool loop)
	{
		if (this.audioSource == null || clip == null || clip.Clip == null)
		{
			return;
		}
		this.audioSource.clip = clip.Clip;
		this.audioSource.loop = loop;
		this.audioSource.volume = UnityEngine.Random.Range(clip.minVolume, clip.maxVolume);
		this.audioSource.pitch = UnityEngine.Random.Range(clip.minPitch, clip.maxPitch);
		this.audioSource.rolloffMode = clip.Rollof;
		this.audioSource.Play();
	}

	public void StopSound()
	{
		if (this.audioSource == null)
		{
			return;
		}
		this.audioSource.Stop();
	}

	public float GetDistanceToTarget()
	{
		return Vector3.Distance(this.target.position, this.trans.position);
	}

	public void SetTransformToTarget()
	{
		this.SetTranformPQ(this.trans, this.target);
	}

	public void SetTransformTo(Transform trans)
	{
		this.SetTranformPQ(this.trans, trans);
	}

	private void SetTranformPQ(Transform trans, Transform target)
	{
		trans.position = target.position;
		trans.rotation = target.rotation;
	}

	public void SetPositionAccordingRefrence(Vector3 pos)
	{
		this.trans.position = this.refrence.localPosition + pos;
		this.refrence.rotation = this.trans.rotation;
	}

	public void Move(float delta)
	{
		this.velocity = this.target.forward * delta;
	}

	public void RotatoToTarget()
	{
		this.trans.rotation = Quaternion.Lerp(this.trans.rotation, this.target.rotation, Time.deltaTime * 10f);
	}

	public void RotatoToPoint()
	{
		this.trans.rotation = Quaternion.Lerp(this.trans.rotation, this.currentPoint.transform.rotation, Time.deltaTime * 10f);
	}

	public PointAnimationCtrl TargetAnim
	{
		get
		{
			return this.targetAnim;
		}
	}

	public Transform Refrence
	{
		get
		{
			return this.refrence;
		}
	}

	public bool Wait
	{
		get
		{
			return this.wait;
		}
		set
		{
			if (value)
			{
				this.wait = value;
			}
			else
			{
				if (this.list.CurrentId == -1)
				{
					this.wait = value;
					this.OnStop();
					return;
				}
				if (this.wait != value)
				{
					this.wait = value;
					this.GoToNextPoint();
				}
				else
				{
					this.list.WaitNext(this);
					this.SetPointStart();
				}
			}
		}
	}

	[SerializeField]
	private PointList list;

	[SerializeField]
	private GameObject model;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private PointAnimationCtrl targetAnim;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private Transform thing;

	[SerializeField]
	private CharacterController characterController;

	[SerializeField]
	private Transform trans;

	private bool wait;

	private Transform refrence;

	private float pointToPointThreshold = 2f;

	private Point currentPoint;

	private Vector3 velocity;

	public Action onStop;

	private Vector3 temp;

	public static List<string> hasShowedModels;

	private static bool _isApplicationPaused;
}
