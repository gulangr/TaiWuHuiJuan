using System;
using Game.Views.Encyclopedia.Event;
using UnityEngine;

namespace Game.Views.Encyclopedia.Views
{
	// Token: 0x02000A73 RID: 2675
	internal class View : MonoBehaviour, IEventListener
	{
		// Token: 0x060083B7 RID: 33719 RVA: 0x003D4C9E File Offset: 0x003D2E9E
		public void Show()
		{
			base.gameObject.gameObject.SetActive(true);
			this.OnShow();
		}

		// Token: 0x060083B8 RID: 33720 RVA: 0x003D4CBA File Offset: 0x003D2EBA
		public void Hide()
		{
			base.gameObject.gameObject.SetActive(false);
			this.OnHide();
		}

		// Token: 0x060083B9 RID: 33721 RVA: 0x003D4CD6 File Offset: 0x003D2ED6
		protected virtual void OnShow()
		{
		}

		// Token: 0x060083BA RID: 33722 RVA: 0x003D4CD9 File Offset: 0x003D2ED9
		protected virtual void OnHide()
		{
		}

		// Token: 0x060083BB RID: 33723 RVA: 0x003D4CDC File Offset: 0x003D2EDC
		public virtual void InitButtonEvents()
		{
		}

		// Token: 0x060083BC RID: 33724 RVA: 0x003D4CDF File Offset: 0x003D2EDF
		public void HandleEvent(int eventId, IEventArgs args)
		{
			this.OnHandleEvent(eventId, args);
		}

		// Token: 0x060083BD RID: 33725 RVA: 0x003D4CEB File Offset: 0x003D2EEB
		protected virtual void OnHandleEvent(int eventId, IEventArgs args)
		{
		}
	}
}
