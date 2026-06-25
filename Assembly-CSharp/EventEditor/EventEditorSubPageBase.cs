using System;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x0200064D RID: 1613
	public abstract class EventEditorSubPageBase : MonoBehaviour
	{
		// Token: 0x06004CD6 RID: 19670
		protected abstract void InternalInit();

		// Token: 0x06004CD7 RID: 19671
		public abstract void Show();

		// Token: 0x06004CD8 RID: 19672
		public abstract void Hide();

		// Token: 0x04003542 RID: 13634
		protected OperateStack OperateStack;
	}
}
