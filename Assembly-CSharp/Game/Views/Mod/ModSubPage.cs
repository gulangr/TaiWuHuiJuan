using System;
using UnityEngine;

namespace Game.Views.Mod
{
	// Token: 0x020008CD RID: 2253
	public class ModSubPage : MonoBehaviour
	{
		// Token: 0x06006B72 RID: 27506 RVA: 0x00319814 File Offset: 0x00317A14
		public virtual void Init(ViewMod parentView)
		{
			this.ParentView = parentView;
		}

		// Token: 0x06006B73 RID: 27507 RVA: 0x0031981E File Offset: 0x00317A1E
		public virtual void Refresh()
		{
		}

		// Token: 0x06006B74 RID: 27508 RVA: 0x00319824 File Offset: 0x00317A24
		public virtual bool QuickHide()
		{
			return false;
		}

		// Token: 0x06006B75 RID: 27509 RVA: 0x00319837 File Offset: 0x00317A37
		public virtual void TryChangeTab(Action onSuccess, Action onFailed)
		{
			if (onSuccess != null)
			{
				onSuccess();
			}
		}

		// Token: 0x04004DF6 RID: 19958
		protected ViewMod ParentView;
	}
}
