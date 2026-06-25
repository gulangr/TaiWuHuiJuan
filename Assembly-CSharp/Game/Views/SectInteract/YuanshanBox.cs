using System;
using FrameWork.UISystem.UIElements;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009BD RID: 2493
	public sealed class YuanshanBox : MonoBehaviour
	{
		// Token: 0x060078DB RID: 30939 RVA: 0x003836CE File Offset: 0x003818CE
		private void Awake()
		{
			base.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				this.OpenBox(string.Empty);
			});
		}

		// Token: 0x060078DC RID: 30940 RVA: 0x003836EC File Offset: 0x003818EC
		public void SetSelectable(bool selectable)
		{
			this.Selector.interactable = selectable;
			this.Locked = !selectable;
		}

		// Token: 0x060078DD RID: 30941 RVA: 0x00383714 File Offset: 0x00381914
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

		// Token: 0x060078DE RID: 30942 RVA: 0x00383788 File Offset: 0x00381988
		public void OpenBox(string _)
		{
			bool locked = this.Locked;
			if (!locked)
			{
				this.Parent.OnBoxSelected(this.IsTrueBox);
			}
		}

		// Token: 0x060078DF RID: 30943 RVA: 0x003837B4 File Offset: 0x003819B4
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
			this.Hover.gameObject.SetActive(false);
		}

		// Token: 0x060078E0 RID: 30944 RVA: 0x00383845 File Offset: 0x00381A45
		private void SwapCallBack()
		{
			this.Parent.StartSwap(ViewYuanshanMiniGame.EAnimPhase.Swap);
		}

		// Token: 0x060078E1 RID: 30945 RVA: 0x00383854 File Offset: 0x00381A54
		private void UnlockReplayCallBack()
		{
			bool trueBoxSelected = this._trueBoxSelected;
			if (trueBoxSelected)
			{
				this._trueBoxSelected = false;
				this.Parent.Stage += 1U;
				bool flag = this.Parent.Stage < ViewYuanshanMiniGame.Finish;
				if (flag)
				{
					this.Parent.SetCanStart(true);
					base.StartCoroutine(this.Parent.ShowEffectCoroutine(this.Parent.LevelEffList[(int)this.Parent.Stage], null));
				}
			}
			else
			{
				this.Parent.SetCanStart(true);
			}
			this.SkeletonGraphic.AnimationState.SetAnimation(0, this.Opened, true);
		}

		// Token: 0x04005B7D RID: 23421
		public bool IsTrueBox;

		// Token: 0x04005B7E RID: 23422
		public SkeletonGraphic SkeletonGraphic;

		// Token: 0x04005B7F RID: 23423
		public ViewYuanshanMiniGame Parent;

		// Token: 0x04005B80 RID: 23424
		public RectTransform Self;

		// Token: 0x04005B81 RID: 23425
		public CButton Selector;

		// Token: 0x04005B82 RID: 23426
		public PointerTrigger PointerTrigger;

		// Token: 0x04005B83 RID: 23427
		public Transform Hover;

		// Token: 0x04005B84 RID: 23428
		public string Open;

		// Token: 0x04005B85 RID: 23429
		public string Close;

		// Token: 0x04005B86 RID: 23430
		public string Opened;

		// Token: 0x04005B87 RID: 23431
		public string Closed;

		// Token: 0x04005B88 RID: 23432
		private bool _trueBoxSelected;

		// Token: 0x04005B89 RID: 23433
		[NonSerialized]
		public bool Locked;

		// Token: 0x02001F08 RID: 7944
		private sealed class CallBack
		{
			// Token: 0x0600F2A3 RID: 62115 RVA: 0x0061AFAC File Offset: 0x006191AC
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

			// Token: 0x0400CC16 RID: 52246
			private readonly Spine.AnimationState.TrackEntryDelegate _callback;
		}
	}
}
