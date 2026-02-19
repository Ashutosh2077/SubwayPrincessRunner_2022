using System;
using UnityEngine;

public class WaterBoard : BaseO, ITouchByCharacter
{
	protected override void Awake()
	{
		this.curTrans = base.transform;
		this.originLPos = this.target.localPosition;
		this.character = Character.Instance;
		this.BCollider = base.GetComponent<BoxCollider>();
		if (this.distance < 0f)
		{
			this.distance = 0f;
		}
		Vector3 center = this.BCollider.center;
		Vector3 size = this.BCollider.size;
		this.curTrans.localPosition = new Vector3(0f, 0f, this.distance - size.z * 0.5f);
		size.z -= this.distance;
		center.z = size.z * 0.5f;
		this.BCollider.size = size;
		this.BCollider.center = center;
		base.Awake();
		this.OnDeactivate();
	}

	public override void OnActivate()
	{
		this.beTouchedFirstUpdate = true;
		this.target.localPosition = this.originLPos;
		if (!this.end)
		{
			this.endZ = this.target.position.z + this.MoveZ;
		}
		else
		{
			this.endZ = this.end.position.z;
		}
	}

	public override void OnDeactivate()
	{
		base.enabled = false;
	}

	public bool BeTouched()
	{
		if (!this.beTouchedFirstUpdate)
		{
			return base.enabled;
		}
		this.beTouchedFirstUpdate = false;
		this.offset = this.target.position.z - this.character.z;
		this.x = ((this.target.position.x >= -10f) ? ((this.target.position.x <= 10f) ? 1 : 2) : 0);
		if (WaterBoard.GetOnWaterBoard != null)
		{
			WaterBoard.GetOnWaterBoard(this.x);
		}
		base.enabled = true;
		return true;
	}

	private void LateUpdate()
	{
		this.target.position = new Vector3(this.target.position.x, this.target.position.y, this.character.z + this.offset);
		if (this.target.position.z >= this.endZ)
		{
			this.OnEnd();
		}
	}

	private void OnEnd()
	{
		base.enabled = false;
		if (WaterBoard.GetOffWaterBoard != null)
		{
			WaterBoard.GetOffWaterBoard(this.x);
		}
	}

	[SerializeField]
	private float distance;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private float MoveZ;

	[SerializeField]
	private Transform end;

	private BoxCollider BCollider;

	private Vector3 originLPos;

	private Transform curTrans;

	private Character character;

	private float endZ;

	private float offset;

	private bool beTouchedFirstUpdate;

	private int x;

	public static Action<int> GetOnWaterBoard;

	public static Action<int> GetOffWaterBoard;
}
