using System;
using System.Collections.Generic;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F2D RID: 3885
	public class InjuryDynamic : MonoBehaviour
	{
		// Token: 0x17001437 RID: 5175
		// (get) Token: 0x0600B2CB RID: 45771 RVA: 0x00516694 File Offset: 0x00514894
		public Injury Injury
		{
			get
			{
				return this.injuryView;
			}
		}

		// Token: 0x17001438 RID: 5176
		// (get) Token: 0x0600B2CC RID: 45772 RVA: 0x0051669C File Offset: 0x0051489C
		// (set) Token: 0x0600B2CD RID: 45773 RVA: 0x005166A4 File Offset: 0x005148A4
		public int CharacterId
		{
			get
			{
				return this._characterId;
			}
			set
			{
				bool flag = this._characterId == value;
				if (!flag)
				{
					this.UnbindMonitor();
					this._characterId = value;
					this._requestPending = false;
					bool flag2 = this._characterId >= 0;
					if (flag2)
					{
						this.BindMonitor();
						this.RequestDisplayData();
					}
				}
			}
		}

		// Token: 0x0600B2CE RID: 45774 RVA: 0x005166F6 File Offset: 0x005148F6
		public void SetExternalListenerId(int listenerId)
		{
			this._listenerId = listenerId;
			this._useExternalListener = true;
		}

		// Token: 0x0600B2CF RID: 45775 RVA: 0x00516708 File Offset: 0x00514908
		public void InitWithListenerId(int characterId)
		{
			bool flag = this._listenerId == 0;
			if (!flag)
			{
				this.UnbindMonitor();
				this._characterId = characterId;
				this._requestPending = false;
				bool flag2 = this._characterId >= 0;
				if (flag2)
				{
					this.BindMonitor();
					this.RequestDisplayData();
				}
			}
		}

		// Token: 0x0600B2D0 RID: 45776 RVA: 0x0051675C File Offset: 0x0051495C
		private void OnEnable()
		{
			bool flag = !this._useExternalListener;
			if (flag)
			{
				this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
			}
			bool flag2 = this._characterId >= 0;
			if (flag2)
			{
				this.BindMonitor();
				this.RequestDisplayData();
			}
		}

		// Token: 0x0600B2D1 RID: 45777 RVA: 0x005167B0 File Offset: 0x005149B0
		private void OnDisable()
		{
			this.UnbindMonitor();
			bool flag = !this._useExternalListener && this._listenerId != 0;
			if (flag)
			{
				GameDataBridge.UnregisterListener(this._listenerId);
				this._listenerId = 0;
			}
			this._requestPending = false;
		}

		// Token: 0x0600B2D2 RID: 45778 RVA: 0x005167FC File Offset: 0x005149FC
		private void Update()
		{
			bool flag = !this._useExternalListener;
			if (flag)
			{
				this.ProcessPendingRequest();
			}
		}

		// Token: 0x0600B2D3 RID: 45779 RVA: 0x00516820 File Offset: 0x00514A20
		public void ProcessPendingRequest()
		{
			bool flag = !this._requestPending || this._listenerId == 0 || this._characterId < 0;
			if (!flag)
			{
				bool flag2 = this._lastRequestFrame == Time.frameCount;
				if (!flag2)
				{
					this._requestPending = false;
					this._lastRequestFrame = Time.frameCount;
					CharacterDomainMethod.Call.GetCharacterInjuryDisplayData(this._listenerId, this._characterId);
				}
			}
		}

		// Token: 0x0600B2D4 RID: 45780 RVA: 0x00516888 File Offset: 0x00514A88
		public void Refresh()
		{
			bool flag = this._characterId >= 0;
			if (flag)
			{
				this.RequestDisplayData();
			}
		}

		// Token: 0x0600B2D5 RID: 45781 RVA: 0x005168B0 File Offset: 0x00514AB0
		private void BindMonitor()
		{
			bool flag = this._characterId < 0 || this._listenerId == 0;
			if (!flag)
			{
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 26U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 19U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 21U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 22U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 23U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 24U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 25U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 44U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 93U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 43U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 79U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 58U);
			}
		}

		// Token: 0x0600B2D6 RID: 45782 RVA: 0x005169F0 File Offset: 0x00514BF0
		public void UnbindMonitor()
		{
			bool flag = this._characterId < 0 || this._listenerId == 0;
			if (!flag)
			{
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 26U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 19U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 21U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 22U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 23U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 24U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 25U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 44U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 93U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 43U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 79U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 58U);
			}
		}

		// Token: 0x0600B2D7 RID: 45783 RVA: 0x00516B30 File Offset: 0x00514D30
		private void RequestDisplayData()
		{
			bool flag = this._characterId < 0 || this._listenerId == 0;
			if (!flag)
			{
				this._requestPending = true;
			}
		}

		// Token: 0x0600B2D8 RID: 45784 RVA: 0x00516B60 File Offset: 0x00514D60
		public void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 1;
				if (flag)
				{
					bool flag2 = notification.DomainId == 4 && notification.MethodId == 201;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._displayData);
						Injury injury = this.injuryView;
						if (injury != null)
						{
							injury.Set(this._displayData, true);
						}
					}
				}
				else
				{
					bool flag3 = notification.Type == 0;
					if (flag3)
					{
						DataUid uid = notification.Uid;
						bool flag4 = uid.DomainId == 4 && uid.DataId == 0 && uid.SubId0 == (ulong)((long)this._characterId);
						if (flag4)
						{
							this.RequestDisplayData();
						}
					}
				}
			}
		}

		// Token: 0x04008AB7 RID: 35511
		[SerializeField]
		private Injury injuryView;

		// Token: 0x04008AB8 RID: 35512
		private int _listenerId;

		// Token: 0x04008AB9 RID: 35513
		private int _characterId = -1;

		// Token: 0x04008ABA RID: 35514
		private CharacterInjuryDisplayData _displayData = new CharacterInjuryDisplayData();

		// Token: 0x04008ABB RID: 35515
		private bool _requestPending;

		// Token: 0x04008ABC RID: 35516
		private int _lastRequestFrame = -1;

		// Token: 0x04008ABD RID: 35517
		private bool _useExternalListener;
	}
}
