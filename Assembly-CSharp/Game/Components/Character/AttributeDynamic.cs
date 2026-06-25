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
	// Token: 0x02000F0F RID: 3855
	public class AttributeDynamic : MonoBehaviour
	{
		// Token: 0x1700142C RID: 5164
		// (get) Token: 0x0600B1B9 RID: 45497 RVA: 0x0050F16A File Offset: 0x0050D36A
		public Attribute Attribue
		{
			get
			{
				return this.attributeView;
			}
		}

		// Token: 0x1700142D RID: 5165
		// (get) Token: 0x0600B1BA RID: 45498 RVA: 0x0050F172 File Offset: 0x0050D372
		// (set) Token: 0x0600B1BB RID: 45499 RVA: 0x0050F17C File Offset: 0x0050D37C
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
					else
					{
						Attribute attribute = this.attributeView;
						if (attribute != null)
						{
							attribute.SetEmpty();
						}
					}
				}
			}
		}

		// Token: 0x0600B1BC RID: 45500 RVA: 0x0050F1E4 File Offset: 0x0050D3E4
		public void SetExternalListenerId(int listenerId)
		{
			this._listenerId = listenerId;
			this._useExternalListener = true;
		}

		// Token: 0x0600B1BD RID: 45501 RVA: 0x0050F1F8 File Offset: 0x0050D3F8
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
				else
				{
					Attribute attribute = this.attributeView;
					if (attribute != null)
					{
						attribute.SetEmpty();
					}
				}
			}
		}

		// Token: 0x0600B1BE RID: 45502 RVA: 0x0050F260 File Offset: 0x0050D460
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

		// Token: 0x0600B1BF RID: 45503 RVA: 0x0050F2B4 File Offset: 0x0050D4B4
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

		// Token: 0x0600B1C0 RID: 45504 RVA: 0x0050F300 File Offset: 0x0050D500
		private void Update()
		{
			bool flag = !this._useExternalListener;
			if (flag)
			{
				this.ProcessPendingRequest();
			}
		}

		// Token: 0x0600B1C1 RID: 45505 RVA: 0x0050F324 File Offset: 0x0050D524
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
					CharacterDomainMethod.Call.GetCharacterAttributeDisplayData(this._listenerId, this._characterId);
				}
			}
		}

		// Token: 0x0600B1C2 RID: 45506 RVA: 0x0050F38C File Offset: 0x0050D58C
		public void Refresh()
		{
			bool flag = this._characterId >= 0;
			if (flag)
			{
				this.RequestDisplayData();
			}
		}

		// Token: 0x0600B1C3 RID: 45507 RVA: 0x0050F3B4 File Offset: 0x0050D5B4
		private void BindMonitor()
		{
			bool flag = this._characterId < 0 || this._listenerId == 0;
			if (!flag)
			{
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 43U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 79U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 80U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 81U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 82U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 83U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 84U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 85U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 86U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 87U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 88U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 89U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 90U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 91U);
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 92U);
			}
		}

		// Token: 0x0600B1C4 RID: 45508 RVA: 0x0050F53C File Offset: 0x0050D73C
		public void UnbindMonitor()
		{
			bool flag = this._characterId < 0 || this._listenerId == 0;
			if (!flag)
			{
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 43U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 79U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 80U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 81U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 82U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 83U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 84U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 85U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 86U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 87U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 88U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 89U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 90U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 91U);
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._characterId), 92U);
			}
		}

		// Token: 0x0600B1C5 RID: 45509 RVA: 0x0050F6C4 File Offset: 0x0050D8C4
		private void RequestDisplayData()
		{
			bool flag = this._characterId < 0 || this._listenerId == 0;
			if (!flag)
			{
				this._requestPending = true;
			}
		}

		// Token: 0x0600B1C6 RID: 45510 RVA: 0x0050F6F4 File Offset: 0x0050D8F4
		public void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 1;
				if (flag)
				{
					bool flag2 = notification.DomainId == 4 && notification.MethodId == 54;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._displayData);
						Attribute attribute = this.attributeView;
						if (attribute != null)
						{
							attribute.Set(this._displayData);
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

		// Token: 0x040089B2 RID: 35250
		[SerializeField]
		private Attribute attributeView;

		// Token: 0x040089B3 RID: 35251
		private int _listenerId;

		// Token: 0x040089B4 RID: 35252
		private int _characterId = -1;

		// Token: 0x040089B5 RID: 35253
		private CharacterAttributeDisplayData _displayData = new CharacterAttributeDisplayData();

		// Token: 0x040089B6 RID: 35254
		private bool _requestPending;

		// Token: 0x040089B7 RID: 35255
		private int _lastRequestFrame = -1;

		// Token: 0x040089B8 RID: 35256
		private bool _useExternalListener;
	}
}
