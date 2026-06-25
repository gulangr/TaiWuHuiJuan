using System;
using System.Collections.Generic;
using Config;
using GameData.GameDataBridge;

namespace Game.Views.MouseTips
{
	// Token: 0x0200082E RID: 2094
	public abstract class CommonTipDynamicRuntime : CommonTipBaseRuntime
	{
		// Token: 0x0600666E RID: 26222 RVA: 0x002EBA12 File Offset: 0x002E9C12
		protected CommonTipDynamicRuntime(CommonTipItem configLine) : base(configLine)
		{
		}

		// Token: 0x17000C57 RID: 3159
		// (get) Token: 0x0600666F RID: 26223 RVA: 0x002EBA1D File Offset: 0x002E9C1D
		protected int ListenerId
		{
			get
			{
				return this._listenerId;
			}
		}

		// Token: 0x06006670 RID: 26224 RVA: 0x002EBA28 File Offset: 0x002E9C28
		protected sealed override void OnAttached()
		{
			bool flag = this._listenerId != 0;
			if (!flag)
			{
				this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameDataInternal));
				this.BindMonitor();
				this.OnAttachedWithListener();
			}
		}

		// Token: 0x06006671 RID: 26225 RVA: 0x002EBA6C File Offset: 0x002E9C6C
		protected sealed override void OnDetaching()
		{
			bool flag = this._listenerId == 0;
			if (!flag)
			{
				this.UnbindMonitor();
				GameDataBridge.UnregisterListener(this._listenerId);
				this._listenerId = 0;
				this.OnDetachedWithListener();
			}
		}

		// Token: 0x06006672 RID: 26226 RVA: 0x002EBAAA File Offset: 0x002E9CAA
		private void OnNotifyGameDataInternal(List<NotificationWrapper> notifications)
		{
			this.HandleNotifyGameData(notifications);
			base.RefreshOwner();
		}

		// Token: 0x06006673 RID: 26227 RVA: 0x002EBABC File Offset: 0x002E9CBC
		protected virtual void BindMonitor()
		{
		}

		// Token: 0x06006674 RID: 26228 RVA: 0x002EBABF File Offset: 0x002E9CBF
		protected virtual void UnbindMonitor()
		{
		}

		// Token: 0x06006675 RID: 26229 RVA: 0x002EBAC2 File Offset: 0x002E9CC2
		protected virtual void HandleNotifyGameData(List<NotificationWrapper> notifications)
		{
		}

		// Token: 0x06006676 RID: 26230 RVA: 0x002EBAC5 File Offset: 0x002E9CC5
		protected virtual void OnAttachedWithListener()
		{
		}

		// Token: 0x06006677 RID: 26231 RVA: 0x002EBAC8 File Offset: 0x002E9CC8
		protected virtual void OnDetachedWithListener()
		{
		}

		// Token: 0x040047B8 RID: 18360
		private int _listenerId;
	}
}
