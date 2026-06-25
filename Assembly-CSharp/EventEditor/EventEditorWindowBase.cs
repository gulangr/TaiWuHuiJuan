using System;
using DG.Tweening;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000652 RID: 1618
	public class EventEditorWindowBase : EventEditorSubPageBase
	{
		// Token: 0x06004D04 RID: 19716 RVA: 0x0024596C File Offset: 0x00243B6C
		public void Focus()
		{
			this.Show();
			base.transform.SetAsLastSibling();
		}

		// Token: 0x06004D05 RID: 19717 RVA: 0x00245984 File Offset: 0x00243B84
		public override void Show()
		{
			bool flag = !this.isShowing;
			if (flag)
			{
				base.transform.position = this.hidePos;
				base.transform.DOScale(Vector3.one, 0.3f);
				base.transform.DOLocalMove(Vector3.zero, 0.3f, false);
				this.isShowing = true;
			}
		}

		// Token: 0x06004D06 RID: 19718 RVA: 0x002459E8 File Offset: 0x00243BE8
		public override void Hide()
		{
			bool flag = this.isShowing;
			if (flag)
			{
				this.isShowing = false;
				base.transform.DOScale(Vector3.zero, 0.3f);
				base.transform.DOMove(this.hidePos, 0.3f, false);
			}
		}

		// Token: 0x06004D07 RID: 19719 RVA: 0x00245A37 File Offset: 0x00243C37
		protected override void InternalInit()
		{
			this.isShowing = false;
			base.transform.localScale = Vector3.zero;
			this.OperateStack = new OperateStack(64);
		}

		// Token: 0x06004D08 RID: 19720 RVA: 0x00245A5F File Offset: 0x00243C5F
		public void Undo()
		{
			OperateStack operateStack = this.OperateStack;
			if (operateStack != null)
			{
				operateStack.Undo();
			}
		}

		// Token: 0x06004D09 RID: 19721 RVA: 0x00245A74 File Offset: 0x00243C74
		public void Redo()
		{
			OperateStack operateStack = this.OperateStack;
			if (operateStack != null)
			{
				operateStack.Redo();
			}
		}

		// Token: 0x04003565 RID: 13669
		public bool isShowing = true;

		// Token: 0x04003566 RID: 13670
		public bool isFocus;

		// Token: 0x04003567 RID: 13671
		public Vector3 hidePos = Vector3.zero;
	}
}
