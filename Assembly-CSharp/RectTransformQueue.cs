using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// Token: 0x0200008F RID: 143
public class RectTransformQueue : MonoBehaviour
{
	// Token: 0x06000512 RID: 1298 RVA: 0x00022DC4 File Offset: 0x00020FC4
	private void OnEnable()
	{
		bool flag = this._activeChildren == null;
		if (flag)
		{
			this._activeChildren = new List<RectTransformQueue.ChildDisplayInfo>();
		}
		bool flag2 = this._enqueueCacheList == null;
		if (flag2)
		{
			this._enqueueCacheList = new List<RectTransformQueue.ChildDisplayInfo>();
		}
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x00022E04 File Offset: 0x00021004
	private void Update()
	{
		bool flag = !this.ActiveState;
		if (!flag)
		{
			this._timer += Time.deltaTime;
			bool flag2 = this._timer >= this.UpdateFrequency;
			if (flag2)
			{
				this._timer = 0f;
				this.AddDisplayInfo();
				this.Move();
			}
		}
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x00022E64 File Offset: 0x00021064
	private void AddDisplayInfo()
	{
		bool flag = this._enqueueCacheList.Count <= 0;
		if (!flag)
		{
			RectTransformQueue.ChildDisplayInfo info = this._enqueueCacheList[0];
			info.Target.pivot = Vector2.zero;
			info.Target.anchorMin = Vector2.zero;
			info.Target.anchorMax = Vector2.zero;
			bool flag2 = this.Direction == RectTransformQueue.FlowDirection.BottomToTop;
			if (flag2)
			{
				info.Target.anchoredPosition = Vector2.down * info.Target.rect.height * 0.5f;
			}
			else
			{
				bool flag3 = this.Direction == RectTransformQueue.FlowDirection.TopToBottom;
				if (flag3)
				{
					info.Target.anchoredPosition = Vector2.up * info.Target.rect.height * 0.5f;
				}
			}
			info.StartShowTime = Time.time;
			info.Target.gameObject.SetActive(true);
			Action<RectTransform> showChildAction = this.ShowChildAction;
			if (showChildAction != null)
			{
				showChildAction(info.Target);
			}
			this._activeChildren.Add(info);
			this._enqueueCacheList.RemoveAt(0);
			bool flag4 = this._activeChildren.Count > (int)this.MaxDisplayCount;
			if (flag4)
			{
				this._activeChildren[0].QuickRemove();
			}
		}
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x00022FD0 File Offset: 0x000211D0
	private void Move()
	{
		float yPos = 0f;
		for (int i = this._activeChildren.Count - 1; i >= 0; i--)
		{
			RectTransformQueue.ChildDisplayInfo info = this._activeChildren[i];
			float height = info.Target.rect.height;
			bool flag = this.Direction == RectTransformQueue.FlowDirection.BottomToTop;
			if (flag)
			{
				bool flag2 = info.Target.anchoredPosition.y < yPos;
				if (flag2)
				{
					info.StartMoveTo(yPos);
				}
				else
				{
					info.CheckExistTime();
				}
				yPos += height;
			}
			else
			{
				bool flag3 = this.Direction == RectTransformQueue.FlowDirection.TopToBottom;
				if (flag3)
				{
					bool flag4 = info.Target.anchoredPosition.y > yPos;
					if (flag4)
					{
						info.StartMoveTo(yPos);
					}
					else
					{
						info.CheckExistTime();
					}
					yPos -= height;
				}
			}
		}
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x000230B4 File Offset: 0x000212B4
	private void RemoveTarget(RectTransform target)
	{
		bool flag = this.HideChildAction != null;
		if (flag)
		{
			this.HideChildAction(target);
		}
		else
		{
			Object.Destroy(target);
		}
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x000230E8 File Offset: 0x000212E8
	public void Enqueue(RectTransform rectTrans, float maxExistTime = 4f)
	{
		rectTrans.gameObject.SetActive(false);
		RectTransformQueue.ChildDisplayInfo info = new RectTransformQueue.ChildDisplayInfo
		{
			Target = rectTrans,
			MaxExistTime = maxExistTime,
			Controller = this
		};
		info.Target.SetParent(base.transform, false);
		this._enqueueCacheList.Add(info);
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x00023144 File Offset: 0x00021344
	public void Remove(RectTransform rectTrans, bool showingOnly)
	{
		bool foundFlag = false;
		foreach (RectTransformQueue.ChildDisplayInfo info in this._activeChildren)
		{
			bool flag = info.Target == rectTrans;
			if (flag)
			{
				foundFlag = true;
				this._activeChildren.Remove(info);
				break;
			}
		}
		bool flag2 = !showingOnly && !foundFlag;
		if (flag2)
		{
			foreach (RectTransformQueue.ChildDisplayInfo info2 in this._enqueueCacheList)
			{
				bool flag3 = info2.Target == rectTrans;
				if (flag3)
				{
					foundFlag = true;
					this._enqueueCacheList.Remove(info2);
					break;
				}
			}
		}
		bool flag4 = foundFlag;
		if (flag4)
		{
			this.RemoveTarget(rectTrans);
		}
	}

	// Token: 0x04000416 RID: 1046
	public bool ActiveState;

	// Token: 0x04000417 RID: 1047
	public float UpdateFrequency = 0.5f;

	// Token: 0x04000418 RID: 1048
	public float AnimationTime = 0.3f;

	// Token: 0x04000419 RID: 1049
	public sbyte MaxDisplayCount = 8;

	// Token: 0x0400041A RID: 1050
	public RectTransformQueue.FlowDirection Direction;

	// Token: 0x0400041B RID: 1051
	public Action<RectTransform> HideChildAction;

	// Token: 0x0400041C RID: 1052
	public Action<RectTransform> ShowChildAction;

	// Token: 0x0400041D RID: 1053
	private List<RectTransformQueue.ChildDisplayInfo> _activeChildren;

	// Token: 0x0400041E RID: 1054
	private List<RectTransformQueue.ChildDisplayInfo> _enqueueCacheList;

	// Token: 0x0400041F RID: 1055
	private float _timer;

	// Token: 0x02001101 RID: 4353
	public enum FlowDirection
	{
		// Token: 0x04009521 RID: 38177
		TopToBottom,
		// Token: 0x04009522 RID: 38178
		BottomToTop
	}

	// Token: 0x02001102 RID: 4354
	private struct ChildDisplayInfo
	{
		// Token: 0x0600C119 RID: 49433 RVA: 0x0056D180 File Offset: 0x0056B380
		public void StartMoveTo(float moveTarget)
		{
			this.MoveTo = moveTarget;
			this.MoveFrom = this.Target.anchoredPosition.y;
			bool flag = this._tweener != null;
			if (flag)
			{
				this._tweener.Complete(true);
			}
			this._tweener = DOVirtual.Float(0f, 1f, this.Controller.AnimationTime, new TweenCallback<float>(this.StepMove)).OnComplete(new TweenCallback(this.CheckExistTime)).SetAutoKill(true);
		}

		// Token: 0x0600C11A RID: 49434 RVA: 0x0056D21C File Offset: 0x0056B41C
		public void QuickRemove()
		{
			Tweener tweener = this._tweener;
			if (tweener != null)
			{
				tweener.Kill(false);
			}
			this.Controller.Remove(this.Target, true);
		}

		// Token: 0x0600C11B RID: 49435 RVA: 0x0056D248 File Offset: 0x0056B448
		private void StepMove(float stepValue)
		{
			Vector2 pos = this.Target.anchoredPosition;
			pos.x = 0f;
			pos.y = Mathf.Lerp(this.MoveFrom, this.MoveTo, stepValue);
			this.Target.anchoredPosition = pos;
		}

		// Token: 0x0600C11C RID: 49436 RVA: 0x0056D294 File Offset: 0x0056B494
		public void CheckExistTime()
		{
			bool flag = Time.time - this.StartShowTime >= this.MaxExistTime;
			if (flag)
			{
				this.Controller.Remove(this.Target, true);
			}
		}

		// Token: 0x04009523 RID: 38179
		public RectTransform Target;

		// Token: 0x04009524 RID: 38180
		public float MaxExistTime;

		// Token: 0x04009525 RID: 38181
		public float StartShowTime;

		// Token: 0x04009526 RID: 38182
		public float MoveFrom;

		// Token: 0x04009527 RID: 38183
		public float MoveTo;

		// Token: 0x04009528 RID: 38184
		public RectTransformQueue Controller;

		// Token: 0x04009529 RID: 38185
		private Tweener _tweener;
	}
}
