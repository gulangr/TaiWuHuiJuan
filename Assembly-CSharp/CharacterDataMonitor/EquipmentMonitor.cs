using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006C1 RID: 1729
	public class EquipmentMonitor : MonitorDataItemBase
	{
		// Token: 0x1400005C RID: 92
		// (add) Token: 0x06005207 RID: 20999 RVA: 0x002602DC File Offset: 0x0025E4DC
		// (remove) Token: 0x06005208 RID: 21000 RVA: 0x00260314 File Offset: 0x0025E514
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<sbyte> OnEquipChangeEvent;

		// Token: 0x1400005D RID: 93
		// (add) Token: 0x06005209 RID: 21001 RVA: 0x0026034C File Offset: 0x0025E54C
		// (remove) Token: 0x0600520A RID: 21002 RVA: 0x00260384 File Offset: 0x0025E584
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnEquipmentLoadChangeEvent;

		// Token: 0x17000A1C RID: 2588
		// (get) Token: 0x0600520B RID: 21003 RVA: 0x002603B9 File Offset: 0x0025E5B9
		// (set) Token: 0x0600520C RID: 21004 RVA: 0x002603C1 File Offset: 0x0025E5C1
		public int CurEquipmentLoad { get; private set; }

		// Token: 0x17000A1D RID: 2589
		// (get) Token: 0x0600520D RID: 21005 RVA: 0x002603CA File Offset: 0x0025E5CA
		// (set) Token: 0x0600520E RID: 21006 RVA: 0x002603D2 File Offset: 0x0025E5D2
		public int MaxEquipmentLoad { get; private set; }

		// Token: 0x17000A1E RID: 2590
		// (get) Token: 0x0600520F RID: 21007 RVA: 0x002603DB File Offset: 0x0025E5DB
		public ItemKey[] Equipment { get; }

		// Token: 0x17000A1F RID: 2591
		// (get) Token: 0x06005210 RID: 21008 RVA: 0x002603E3 File Offset: 0x0025E5E3
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					56U,
					106U,
					105U
				};
			}
		}

		// Token: 0x17000A20 RID: 2592
		// (get) Token: 0x06005211 RID: 21009 RVA: 0x00260405 File Offset: 0x0025E605
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005212 RID: 21010 RVA: 0x00260408 File Offset: 0x0025E608
		public EquipmentMonitor()
		{
			this.Equipment = new ItemKey[17];
		}

		// Token: 0x06005213 RID: 21011 RVA: 0x00260420 File Offset: 0x0025E620
		protected override void MonitorData()
		{
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				foreach (uint fieldId in this.RelativeFieldIds)
				{
					GameDataBridge.AddDataMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
				}
			}
		}

		// Token: 0x06005214 RID: 21012 RVA: 0x002604A4 File Offset: 0x0025E6A4
		public override void OnDataInit()
		{
			Action<sbyte> onEquipChangeEvent = this.OnEquipChangeEvent;
			if (onEquipChangeEvent != null)
			{
				onEquipChangeEvent(17);
			}
			Action onEquipmentLoadChangeEvent = this.OnEquipmentLoadChangeEvent;
			if (onEquipmentLoadChangeEvent != null)
			{
				onEquipmentLoadChangeEvent();
			}
		}

		// Token: 0x06005215 RID: 21013 RVA: 0x002604D0 File Offset: 0x0025E6D0
		protected override bool IsValidMonitor()
		{
			Action<sbyte> onEquipChangeEvent = this.OnEquipChangeEvent;
			return onEquipChangeEvent != null && onEquipChangeEvent.GetInvocationList().Length != 0;
		}

		// Token: 0x06005216 RID: 21014 RVA: 0x002604F8 File Offset: 0x0025E6F8
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x06005217 RID: 21015 RVA: 0x00260568 File Offset: 0x0025E768
		public sbyte GetEquipmentSlot(ItemKey itemKey)
		{
			return EquipmentSlotHelper.GetEquipmentSlot(itemKey, this.Equipment);
		}

		// Token: 0x06005218 RID: 21016 RVA: 0x00260578 File Offset: 0x0025E778
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type > 0;
			if (!flag)
			{
				uint subId = wrapper.Notification.Uid.SubId1;
				uint num = subId;
				if (num != 56U)
				{
					if (num != 105U)
					{
						if (num == 106U)
						{
							int curLoad = 0;
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref curLoad);
							bool changeFlag = curLoad != this.CurEquipmentLoad;
							this.CurEquipmentLoad = curLoad;
							bool flag2 = base.Init && changeFlag;
							if (flag2)
							{
								Action onEquipmentLoadChangeEvent = this.OnEquipmentLoadChangeEvent;
								if (onEquipmentLoadChangeEvent != null)
								{
									onEquipmentLoadChangeEvent();
								}
							}
						}
					}
					else
					{
						int maxLoad = 0;
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref maxLoad);
						bool changeFlag2 = maxLoad != this.MaxEquipmentLoad;
						this.MaxEquipmentLoad = maxLoad;
						bool flag3 = base.Init && changeFlag2;
						if (flag3)
						{
							Action onEquipmentLoadChangeEvent2 = this.OnEquipmentLoadChangeEvent;
							if (onEquipmentLoadChangeEvent2 != null)
							{
								onEquipmentLoadChangeEvent2();
							}
						}
						bool flag4 = !base.Init;
						if (flag4)
						{
							this.DataFlag = 1;
						}
					}
				}
				else
				{
					ItemKey[] newEquipments = new ItemKey[17];
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref newEquipments);
					for (sbyte i = 0; i < 17; i += 1)
					{
						bool changeFlag3 = this.Equipment[(int)i].Id != newEquipments[(int)i].Id;
						this.Equipment[(int)i] = newEquipments[(int)i];
						bool flag5 = changeFlag3 && base.Init;
						if (flag5)
						{
							Action<sbyte> onEquipChangeEvent = this.OnEquipChangeEvent;
							if (onEquipChangeEvent != null)
							{
								onEquipChangeEvent(i);
							}
						}
					}
				}
			}
		}

		// Token: 0x06005219 RID: 21017 RVA: 0x0026073B File Offset: 0x0025E93B
		public void AddEquipmentChangeListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnEquipChangeEvent -= listener;
			this.OnEquipChangeEvent += listener;
		}

		// Token: 0x0600521A RID: 21018 RVA: 0x0026075D File Offset: 0x0025E95D
		public void RemoveEquipmentChangeListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnEquipChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600521B RID: 21019 RVA: 0x0026077E File Offset: 0x0025E97E
		public void AddEquipmentLoadChangeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnEquipmentLoadChangeEvent -= listener;
			this.OnEquipmentLoadChangeEvent += listener;
		}

		// Token: 0x0600521C RID: 21020 RVA: 0x002607A0 File Offset: 0x0025E9A0
		public void RemoveEquipmentLoadChangeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnEquipmentLoadChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}
	}
}
