using System;
using Spine;
using Spine.Unity;
using UnityEngine;

// Token: 0x0200025A RID: 602
public sealed class YuanshanBox : MonoBehaviour
{
	// Token: 0x060027A3 RID: 10147 RVA: 0x001242F8 File Offset: 0x001224F8
	public void SetSelectable(bool selectable)
	{
		this.Selector.interactable = selectable;
		this.Locked = !selectable;
	}

	// Token: 0x060027A4 RID: 10148 RVA: 0x00124320 File Offset: 0x00122520
	public void CloseBox()
	{
		this.SetSelectable(false);
		bool flag = !this.IsTrueBox;
		if (!flag)
		{
			AudioManager.Instance.PlaySound(this.Parent.GameStart, false, false);
			this.SkeletonGraphic.AnimationState.SetAnimation(0, this.Close, false);
			new YuanshanBox.CallBack(new Action(this.SwapCallBack), this.SkeletonGraphic.AnimationState);
		}
	}

	// Token: 0x060027A5 RID: 10149 RVA: 0x00124394 File Offset: 0x00122594
	public void OpenBox(string _)
	{
		bool locked = this.Locked;
		if (!locked)
		{
			this.Parent.OnBoxSelected(this.IsTrueBox);
		}
	}

	// Token: 0x060027A6 RID: 10150 RVA: 0x001243C0 File Offset: 0x001225C0
	public void ShowBone(bool isTrueBox)
	{
		bool isTrueBox2 = this.IsTrueBox;
		if (isTrueBox2)
		{
			AudioManager.Instance.PlaySound(isTrueBox ? this.Parent.GetBoxCorrect : this.Parent.GetBoxIncorrect, false, false);
			this.SkeletonGraphic.AnimationState.SetAnimation(0, this.Open, false);
			this._trueBoxSelected = isTrueBox;
			new YuanshanBox.CallBack(new Action(this.UnlockReplayCallBack), this.SkeletonGraphic.AnimationState);
		}
	}

	// Token: 0x060027A7 RID: 10151 RVA: 0x0012443F File Offset: 0x0012263F
	private void SwapCallBack()
	{
		this.Parent.StartSwap(UI_Yuanshan.EAnimPhase.Swap);
	}

	// Token: 0x060027A8 RID: 10152 RVA: 0x00124450 File Offset: 0x00122650
	private void UnlockReplayCallBack()
	{
		bool trueBoxSelected = this._trueBoxSelected;
		if (trueBoxSelected)
		{
			this._trueBoxSelected = false;
			this.Parent.Stage += 1U;
			bool flag = this.Parent.Stage < UI_Yuanshan.Finish;
			if (flag)
			{
				this.Parent.ShowEffect(this.Parent.EffectCorrect);
			}
		}
		else
		{
			this.Parent.ShowEffect(this.Parent.EffectIncorrect);
		}
		this.SkeletonGraphic.AnimationState.SetAnimation(0, this.Opened, true);
	}

	// Token: 0x04001CE6 RID: 7398
	public bool IsTrueBox;

	// Token: 0x04001CE7 RID: 7399
	public SkeletonGraphic SkeletonGraphic;

	// Token: 0x04001CE8 RID: 7400
	public UI_Yuanshan Parent;

	// Token: 0x04001CE9 RID: 7401
	public RectTransform Self;

	// Token: 0x04001CEA RID: 7402
	public CButtonObsolete Selector;

	// Token: 0x04001CEB RID: 7403
	public string Open;

	// Token: 0x04001CEC RID: 7404
	public string Close;

	// Token: 0x04001CED RID: 7405
	public string Opened;

	// Token: 0x04001CEE RID: 7406
	public string Closed;

	// Token: 0x04001CEF RID: 7407
	private bool _trueBoxSelected;

	// Token: 0x04001CF0 RID: 7408
	[NonSerialized]
	public bool Locked;

	// Token: 0x020015A7 RID: 5543
	private sealed class CallBack
	{
		// Token: 0x0600CF97 RID: 53143 RVA: 0x005A33E0 File Offset: 0x005A15E0
		public CallBack(Action callBack, Spine.AnimationState state)
		{
			YuanshanBox.CallBack <>4__this = this;
			bool flag = callBack == null;
			if (!flag)
			{
				this._callback = delegate(TrackEntry <p0>)
				{
					callBack();
					state.Complete -= <>4__this._callback;
				};
				state.Complete += this._callback;
			}
		}

		// Token: 0x0400A57D RID: 42365
		private readonly Spine.AnimationState.TrackEntryDelegate _callback;
	}
}
