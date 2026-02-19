using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingO : BaseO
{
	protected override void Awake()
	{
		this.game = Game.Instance;
		if (this.game != null)
		{
			if (this.game.awakeDone)
			{
				this.Init();
			}
			if (base.transform.childCount == 0)
			{
				UnityEngine.Debug.Log("No train child");
			}
			MovingO.characterController = this.game.character.characterController;
			this.child = base.transform.GetChild(0);
			this.Collider = base.GetComponent<BoxCollider>();
			base.enabled = false;
		}
		this.curTrans = base.transform;
		base.Awake();
	}

	protected virtual void Init()
	{
	}

	public override void OnActivate()
	{
		base.enabled = true;
		if (!MovingO.activeMovings.Contains(this))
		{
			MovingO.activeMovings.Add(this);
		}
		this.autoPilot = false;
		this.child.localPosition = new Vector3(0f, 0f, this.Distance * this.speed);
	}

	public override void OnDeactivate()
	{
		if (MovingO.activeMovings.Contains(this))
		{
			MovingO.activeMovings.Remove(this);
		}
		base.enabled = false;
		this.child.transform.localPosition = -200f * Vector3.up;
	}

	protected virtual void Update()
	{
		if (this.game != null)
		{
			if (this.autoPilot)
			{
				return;
			}
			this.position = new Vector3(0f, 0f, this.Distance * this.Speed);
			this.child.localPosition = this.position;
		}
	}

	public static void ActivateAutoPilot()
	{
		int i = 0;
		int count = MovingO.activeMovings.Count;
		while (i < count)
		{
			if (MovingO.activeMovings[i].Collider.bounds.min.z - MovingO.characterController.transform.position.z < MovingO.autoPilotActivationDistance)
			{
				MovingO.activeMovings[i].autoPilot = true;
			}
			i++;
		}
	}

	public void OnDrawGizmos()
	{
		if (this.child != null)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(this.child.position, base.transform.position);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position, 5f);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(this.child.position, 5f);
		}
	}

	protected virtual float Distance { get; private set; }

	protected virtual float Speed { get; private set; }

	private static List<MovingO> activeMovings = new List<MovingO>();

	protected static CharacterController characterController;

	public static float autoPilotActivationDistance = 200f;

	public float speed = 1f;

	protected Game game;

	protected bool autoPilot;

	protected BoxCollider Collider;

	protected Transform child;

	protected Transform curTrans;

	private Vector3 position;
}
