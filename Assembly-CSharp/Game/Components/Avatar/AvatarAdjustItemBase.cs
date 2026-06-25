using System;
using System.Collections.Generic;
using DG.Tweening;
using GameData.Domains.Character.AvatarSystem;
using UnityEngine;

namespace Game.Components.Avatar
{
	// Token: 0x02000F71 RID: 3953
	[RequireComponent(typeof(Refers))]
	public abstract class AvatarAdjustItemBase : MonoBehaviour
	{
		// Token: 0x17001494 RID: 5268
		// (get) Token: 0x0600B54A RID: 46410 RVA: 0x0052A368 File Offset: 0x00528568
		public bool ColorLocked
		{
			get
			{
				return this.GetColorLockState();
			}
		}

		// Token: 0x17001495 RID: 5269
		// (get) Token: 0x0600B54B RID: 46411 RVA: 0x0052A370 File Offset: 0x00528570
		private int ColorIndex
		{
			get
			{
				return this.GetColorIndex();
			}
		}

		// Token: 0x17001496 RID: 5270
		// (get) Token: 0x0600B54C RID: 46412 RVA: 0x0052A378 File Offset: 0x00528578
		public AvatarData Data
		{
			get
			{
				return this.Controller.AvatarData;
			}
		}

		// Token: 0x17001497 RID: 5271
		// (get) Token: 0x0600B54D RID: 46413 RVA: 0x0052A385 File Offset: 0x00528585
		public float Size
		{
			get
			{
				return this.GetSize();
			}
		}

		// Token: 0x0600B54E RID: 46414 RVA: 0x0052A390 File Offset: 0x00528590
		protected virtual void Awake()
		{
			this.RectTransform = base.GetComponent<RectTransform>();
			this.Refers.CTryGet<RectTransform>("AdjustBody", out this._adjustBody);
			CButtonObsolete topTitle;
			bool flag = this.Refers.CTryGet<CButtonObsolete>("TopTitle", out topTitle) && null != topTitle;
			if (flag)
			{
				topTitle.ClearAndAddListener(new Action(this.OnTitleClick));
			}
			bool flag2 = this.Refers.CTryGet<InfinityScrollLegacy>("ColorScroll", out this._scroll);
			if (flag2)
			{
				CToggleGroupObsolete cToggleGroup = this._scroll.GetComponent<CToggleGroupObsolete>();
				bool flag3 = null != cToggleGroup;
				if (flag3)
				{
					this._scroll.SetTogGroup(cToggleGroup, false, false);
				}
			}
			IdSwitch idSwitch;
			bool flag4 = this.Refers.CTryGet<IdSwitch>("IDSwitch", out idSwitch) && null != idSwitch;
			if (flag4)
			{
				idSwitch.OnValueChanged = new Action<int>(this.SetId);
			}
		}

		// Token: 0x0600B54F RID: 46415 RVA: 0x0052A474 File Offset: 0x00528674
		protected virtual void Start()
		{
			bool flag = null != this.Controller;
			if (flag)
			{
				this.OnStateChange(false);
			}
		}

		// Token: 0x0600B550 RID: 46416 RVA: 0x0052A49A File Offset: 0x0052869A
		public void SetController(AvatarAdjustController controller)
		{
			this.Controller = controller;
			this.BindArgUpdate();
			this.SetColorPrefab();
		}

		// Token: 0x0600B551 RID: 46417 RVA: 0x0052A4B4 File Offset: 0x005286B4
		private bool GetColorLockState()
		{
			bool flag = this.Refers.Names.Contains("LockSwitch");
			return flag && this.Refers.CGet<CToggleObsolete>("LockSwitch").isOn;
		}

		// Token: 0x0600B552 RID: 46418 RVA: 0x0052A4F8 File Offset: 0x005286F8
		private void OnTitleClick()
		{
			this.Closed = !this.Closed;
			this.OnStateChange(true);
			bool flag = !this.Closed;
			if (flag)
			{
				this.Controller.OnItemOpen(this);
			}
		}

		// Token: 0x0600B553 RID: 46419 RVA: 0x0052A538 File Offset: 0x00528738
		private float GetSize()
		{
			bool flag = null == this.RectTransform;
			if (flag)
			{
				this.RectTransform = base.GetComponent<RectTransform>();
			}
			return this.RectTransform.rect.height;
		}

		// Token: 0x0600B554 RID: 46420 RVA: 0x0052A57C File Offset: 0x0052877C
		private void OnStateChange(bool anim = true)
		{
			bool closed = this.Closed;
			if (closed)
			{
				this.OnClose(anim);
			}
			else
			{
				this.OnOpen(anim);
			}
		}

		// Token: 0x0600B555 RID: 46421 RVA: 0x0052A5A8 File Offset: 0x005287A8
		public virtual void OnOpen(bool anim)
		{
			bool flag = null == this.RectTransform;
			if (flag)
			{
				this.RectTransform = base.GetComponent<RectTransform>();
			}
			CommonUtils.TryKillTween(this._animTweener, false);
			bool flag2 = this._scroll != null;
			if (flag2)
			{
				this._scroll.gameObject.SetActive(true);
			}
			if (anim)
			{
				float hDiff = this.SwitchSize.y - this.SwitchSize.x;
				this._animTweener = DOVirtual.Float(0f, 1f, 0.3f, delegate(float stepValue)
				{
					this.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.SwitchSize.x + hDiff * stepValue);
					this.Refers.CGet<RectTransform>("AdjustBody").localScale = new Vector3(1f, stepValue, 1f);
					AvatarAdjustController controller2 = this.Controller;
					if (controller2 != null)
					{
						controller2.MarkLayoutDirty();
					}
				}).SetAutoKill(true);
				RectTransform arrow;
				bool flag3 = this.Refers.CTryGet<RectTransform>("Arrow", out arrow) && arrow;
				if (flag3)
				{
					arrow.localRotation = Quaternion.Euler(0f, 0f, 0f);
				}
			}
			else
			{
				this.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.SwitchSize.y);
				bool flag4 = null != this._adjustBody;
				if (flag4)
				{
					this._adjustBody.localScale = Vector3.one;
				}
				AvatarAdjustController controller = this.Controller;
				if (controller != null)
				{
					controller.MarkLayoutDirty();
				}
			}
		}

		// Token: 0x0600B556 RID: 46422 RVA: 0x0052A6F4 File Offset: 0x005288F4
		private void OnClose(bool anim)
		{
			CommonUtils.TryKillTween(this._animTweener, false);
			if (anim)
			{
				float hDiff = this.SwitchSize.y - this.SwitchSize.x;
				this._animTweener = DOVirtual.Float(1f, 0f, 0.3f, delegate(float stepValue)
				{
					this.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.SwitchSize.x + hDiff * stepValue);
					bool flag3 = null != this._adjustBody;
					if (flag3)
					{
						this._adjustBody.localScale = new Vector3(1f, stepValue, 1f);
					}
					AvatarAdjustController controller2 = this.Controller;
					if (controller2 != null)
					{
						controller2.MarkLayoutDirty();
					}
				}).OnComplete(delegate
				{
					bool flag3 = this._scroll != null;
					if (flag3)
					{
						this._scroll.gameObject.SetActive(false);
					}
				}).SetAutoKill(true);
				this.Refers.CGet<RectTransform>("Arrow").localRotation = Quaternion.Euler(0f, 0f, 90f);
			}
			else
			{
				this.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.SwitchSize.x);
				bool flag = null != this._adjustBody;
				if (flag)
				{
					this._adjustBody.localScale = Vector3.zero;
				}
				bool flag2 = this._scroll != null;
				if (flag2)
				{
					this._scroll.gameObject.SetActive(false);
				}
				AvatarAdjustController controller = this.Controller;
				if (controller != null)
				{
					controller.MarkLayoutDirty();
				}
			}
		}

		// Token: 0x0600B557 RID: 46423 RVA: 0x0052A81B File Offset: 0x00528A1B
		public void Close(bool anim = true)
		{
			this.Closed = true;
			this.OnClose(anim);
		}

		// Token: 0x0600B558 RID: 46424 RVA: 0x0052A82D File Offset: 0x00528A2D
		public virtual void BindArgUpdate()
		{
		}

		// Token: 0x0600B559 RID: 46425 RVA: 0x0052A830 File Offset: 0x00528A30
		public virtual void SetColorPrefab()
		{
			Refers color;
			bool flag = this.Refers.CTryGet<Refers>("ColorPrefab", out color);
			if (flag)
			{
				bool flag2 = color != null;
				if (flag2)
				{
					this.OnColorPrefabRender(this.ColorIndex, color);
				}
			}
		}

		// Token: 0x0600B55A RID: 46426 RVA: 0x0052A870 File Offset: 0x00528A70
		protected virtual int GetColorIndex()
		{
			return 0;
		}

		// Token: 0x0600B55B RID: 46427 RVA: 0x0052A884 File Offset: 0x00528A84
		protected void OnColorPrefabRender(int index, Refers refer)
		{
			refer.CGet<CImage>("Color").color = this._colors[index].Item2;
			refer.CGet<CToggleObsolete>("Toggle").onValueChanged.RemoveAllListeners();
			refer.CGet<CToggleObsolete>("Toggle").isOn = (index == this.ColorIndex);
			refer.CGet<CToggleObsolete>("Toggle").onValueChanged.AddListener(delegate(bool val)
			{
				if (val)
				{
					bool colorLocked = this.ColorLocked;
					if (colorLocked)
					{
						this.Controller.SetLockGroupColor(index);
					}
					else
					{
						this.SetColorIndex(index);
					}
					this._scroll.ReRender();
				}
			});
		}

		// Token: 0x0600B55C RID: 46428 RVA: 0x0052A924 File Offset: 0x00528B24
		public virtual void SetColorIndex(int index)
		{
		}

		// Token: 0x0600B55D RID: 46429 RVA: 0x0052A927 File Offset: 0x00528B27
		protected void UpdateColorScroll(InfinityScrollLegacy scroll, List<ValueTuple<byte, Color>> colors)
		{
			this._scroll = scroll;
			this._scroll.OnItemRender = new Action<int, Refers>(this.OnColorPrefabRender);
			this._colors = colors;
			scroll.SetDataCount(colors.Count);
		}

		// Token: 0x0600B55E RID: 46430 RVA: 0x0052A95C File Offset: 0x00528B5C
		protected virtual void SetId(int newIndex)
		{
		}

		// Token: 0x0600B55F RID: 46431 RVA: 0x0052A960 File Offset: 0x00528B60
		public void RegisterOnArgUpdateListener(Action action)
		{
			bool flag = this.OnArgsUpdate == null;
			if (flag)
			{
				this.OnArgsUpdate = action;
			}
			else
			{
				this.OnArgsUpdate = (Action)Delegate.Remove(this.OnArgsUpdate, action);
				this.OnArgsUpdate = (Action)Delegate.Combine(this.OnArgsUpdate, action);
			}
		}

		// Token: 0x0600B560 RID: 46432 RVA: 0x0052A9B4 File Offset: 0x00528BB4
		public void UnRegisterOnArgUpdateListener(Action action)
		{
			bool flag = this.OnArgsUpdate != null;
			if (flag)
			{
				this.OnArgsUpdate = (Action)Delegate.Remove(this.OnArgsUpdate, action);
			}
		}

		// Token: 0x0600B561 RID: 46433 RVA: 0x0052A9E6 File Offset: 0x00528BE6
		public virtual void OnQuickAdjustTriggered(int delta)
		{
		}

		// Token: 0x04008D37 RID: 36151
		public bool Closed;

		// Token: 0x04008D38 RID: 36152
		public Refers Refers;

		// Token: 0x04008D39 RID: 36153
		[HideInInspector]
		public RectTransform RectTransform;

		// Token: 0x04008D3A RID: 36154
		private RectTransform _adjustBody;

		// Token: 0x04008D3B RID: 36155
		public Vector2 SwitchSize;

		// Token: 0x04008D3C RID: 36156
		private List<ValueTuple<byte, Color>> _colors;

		// Token: 0x04008D3D RID: 36157
		private InfinityScrollLegacy _scroll;

		// Token: 0x04008D3E RID: 36158
		private Tweener _animTweener;

		// Token: 0x04008D3F RID: 36159
		protected Action OnArgsUpdate;

		// Token: 0x04008D40 RID: 36160
		protected AvatarAdjustController Controller;

		// Token: 0x04008D41 RID: 36161
		public CToggleObsolete ShaveBaldToggle;

		// Token: 0x04008D42 RID: 36162
		protected DisableStyleRoot _shaveBaldDisableStyleRoot;
	}
}
