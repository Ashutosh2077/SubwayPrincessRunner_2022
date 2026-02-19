using System;
using UnityEngine;

public class CenterOnChild : MonoBehaviour
{
	public bool CenterOnClosestChildAtPosition(Vector2 clickPositionOnScreen)
	{
		if (this.mDrag == null)
		{
			this.mDrag = NGUITools.FindInParents<UIScrollView>(base.gameObject);
			if (this.mDrag == null)
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					base.GetType(),
					" requires ",
					typeof(UIScrollView),
					" on a parent object in order to work"
				}), this);
				base.enabled = false;
				return false;
			}
			this.mDrag.onDragFinished = new UIScrollView.OnDragNotification(this.OnDragFinished);
		}
		if (this.mDrag.panel != null)
		{
			Vector4 finalClipRegion = this.mDrag.panel.finalClipRegion;
			Transform cachedTransform = this.mDrag.panel.cachedTransform;
			Vector3 position = cachedTransform.localPosition;
			position.x += finalClipRegion.x;
			position.y += finalClipRegion.y;
			position = cachedTransform.parent.TransformPoint(position);
			Vector3 a = clickPositionOnScreen;
			a.x -= (float)Screen.width * 0.5f;
			a *= (float)UIScreenController.Instance.root.manualWidth / (float)Screen.width;
			a.x += finalClipRegion.x;
			float num = float.MaxValue;
			Transform transform = null;
			Transform transform2 = base.transform;
			int i = 0;
			int childCount = transform2.childCount;
			while (i < childCount)
			{
				Transform child = transform2.GetChild(i);
				float num2 = Mathf.Abs(child.localPosition.x - a.x);
				if (num2 < num)
				{
					num = num2;
					transform = child;
				}
				i++;
			}
			if (transform != null)
			{
				if (this.mSelectedObject == transform.gameObject && this.mCenteredObject == transform.gameObject)
				{
					return true;
				}
				this.mCenteredObject = null;
				this.characterWasClicked = true;
				Vector3 b = cachedTransform.InverseTransformPoint(transform.position) - cachedTransform.InverseTransformPoint(position);
				if (!this.mDrag.canMoveHorizontally)
				{
					b.x = 0f;
				}
				if (!this.mDrag.canMoveVertically)
				{
					b.y = 0f;
				}
				b.z = 0f;
				SpringPanel.Begin(this.mDrag.gameObject, cachedTransform.localPosition - b, 8f).onFinished = new SpringPanel.OnFinished(this.OnStringFinished);
				if (this.mSelectedObject == transform.gameObject && this.mCenteredObject != transform.gameObject)
				{
					return true;
				}
				this.mSelectedObject = transform.gameObject;
			}
		}
		return false;
	}

	public void CenterOnTransform(Transform target, bool instant = false)
	{
		if (this.mDrag == null)
		{
			this.mDrag = NGUITools.FindInParents<UIScrollView>(base.gameObject);
			if (this.mDrag == null)
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					base.GetType(),
					" requires ",
					typeof(UIScrollView),
					" on a parent object in order to work"
				}), this);
				base.enabled = false;
				return;
			}
			this.mDrag.onDragFinished = new UIScrollView.OnDragNotification(this.OnDragFinished);
		}
		if (this.mDrag.panel != null)
		{
			Transform transform = base.transform;
			bool flag = false;
			int i = 0;
			int childCount = transform.childCount;
			while (i < childCount)
			{
				if (transform.GetChild(i) == target)
				{
					flag = true;
					break;
				}
				i++;
			}
			if (flag)
			{
				Vector4 finalClipRegion = this.mDrag.panel.finalClipRegion;
				Transform cachedTransform = this.mDrag.panel.cachedTransform;
				Vector3 position = cachedTransform.localPosition;
				position.x += finalClipRegion.x;
				position.y += finalClipRegion.y;
				position = cachedTransform.parent.TransformPoint(position);
				this.mDrag.currentMomentum = Vector3.zero;
				if (target != null)
				{
					this.mSelectedObject = target.gameObject;
					Vector3 b = cachedTransform.InverseTransformPoint(target.position) - cachedTransform.InverseTransformPoint(position);
					if (!this.mDrag.canMoveHorizontally)
					{
						b.x = 0f;
					}
					if (!this.mDrag.canMoveVertically)
					{
						b.y = 0f;
					}
					b.z = 0f;
					if (instant)
					{
						Vector3 localPosition = cachedTransform.localPosition - b;
						UIPanel panel = this.mDrag.panel;
						panel.clipOffset += new Vector2(b.x, b.y);
						cachedTransform.localPosition = localPosition;
						if (this.mDrag != null)
						{
							this.mDrag.UpdateScrollbars(false);
						}
					}
					else
					{
						SpringPanel.Begin(this.mDrag.gameObject, cachedTransform.localPosition - b, 8f).onFinished = new SpringPanel.OnFinished(this.OnStringFinished);
					}
				}
				else
				{
					this.mSelectedObject = null;
				}
			}
		}
	}

	public void CharacterFocusedFromClick()
	{
		this.characterWasClicked = false;
	}

	public void ClearCenterObject()
	{
		this.mSelectedObject = null;
	}

	private void OnDragFinished()
	{
		if (base.enabled)
		{
			this.Recenter();
		}
	}

	private void OnEnable()
	{
		this.Recenter();
	}

	private void OnStringFinished()
	{
		this.mCenteredObject = this.mSelectedObject;
	}

	public void Recenter()
	{
		if (this.mDrag == null)
		{
			this.mDrag = NGUITools.FindInParents<UIScrollView>(base.gameObject);
			if (this.mDrag == null)
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					base.GetType(),
					" requires ",
					typeof(UIScrollView),
					" on a parent object in order to work"
				}), this);
				base.enabled = false;
				return;
			}
			this.mDrag.onDragFinished = new UIScrollView.OnDragNotification(this.OnDragFinished);
		}
		if (this.mDrag.panel != null)
		{
			Vector4 finalClipRegion = this.mDrag.panel.finalClipRegion;
			Transform cachedTransform = this.mDrag.panel.cachedTransform;
			Vector3 vector = cachedTransform.localPosition;
			vector.x += finalClipRegion.x;
			vector.y += finalClipRegion.y;
			vector = cachedTransform.parent.TransformPoint(vector);
			Vector3 b = vector - this.mDrag.currentMomentum * (this.mDrag.momentumAmount * 0.1f);
			this.mDrag.currentMomentum = Vector3.zero;
			float num = float.MaxValue;
			Transform transform = null;
			Transform transform2 = base.transform;
			int i = 0;
			int childCount = transform2.childCount;
			while (i < childCount)
			{
				Transform child = transform2.GetChild(i);
				float num2 = Vector3.SqrMagnitude(child.position - b);
				if (num2 < num)
				{
					num = num2;
					transform = child;
				}
				i++;
			}
			if (transform != null)
			{
				this.mSelectedObject = transform.gameObject;
				Vector3 a = cachedTransform.InverseTransformPoint(transform.position);
				Vector3 b2 = cachedTransform.InverseTransformPoint(vector);
				Vector3 b3 = a - b2;
				if (!this.mDrag.canMoveHorizontally)
				{
					b3.x = 0f;
				}
				if (!this.mDrag.canMoveVertically)
				{
					b3.y = 0f;
				}
				b3.z = 0f;
				SpringPanel.Begin(this.mDrag.gameObject, cachedTransform.localPosition - b3, 8f).onFinished = new SpringPanel.OnFinished(this.OnStringFinished);
			}
			else
			{
				this.mSelectedObject = null;
			}
		}
	}

	public GameObject centeredObject
	{
		get
		{
			return this.mSelectedObject;
		}
	}

	[NonSerialized]
	public bool characterWasClicked;

	private GameObject mCenteredObject;

	private UIScrollView mDrag;

	private GameObject mSelectedObject;
}
