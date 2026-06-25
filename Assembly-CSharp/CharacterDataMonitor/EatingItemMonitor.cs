using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006BF RID: 1727
	public class EatingItemMonitor : MonitorDataItemBase
	{
		// Token: 0x14000050 RID: 80
		// (add) Token: 0x060051AD RID: 20909 RVA: 0x0025ED78 File Offset: 0x0025CF78
		// (remove) Token: 0x060051AE RID: 20910 RVA: 0x0025EDB0 File Offset: 0x0025CFB0
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnEatingItemChangeEvent;

		// Token: 0x17000A0D RID: 2573
		// (get) Token: 0x060051AF RID: 20911 RVA: 0x0025EDE5 File Offset: 0x0025CFE5
		// (set) Token: 0x060051B0 RID: 20912 RVA: 0x0025EDED File Offset: 0x0025CFED
		public sbyte CanEatingMaxCount { get; private set; }

		// Token: 0x17000A0E RID: 2574
		// (get) Token: 0x060051B1 RID: 20913 RVA: 0x0025EDF6 File Offset: 0x0025CFF6
		public EatingItems EatingItems
		{
			get
			{
				return this._eatingItems;
			}
		}

		// Token: 0x17000A0F RID: 2575
		// (get) Token: 0x060051B2 RID: 20914 RVA: 0x0025EDFE File Offset: 0x0025CFFE
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000A10 RID: 2576
		// (get) Token: 0x060051B3 RID: 20915 RVA: 0x0025EE01 File Offset: 0x0025D001
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					58U
				};
			}
		}

		// Token: 0x060051B4 RID: 20916 RVA: 0x0025EE11 File Offset: 0x0025D011
		public bool CanEatMore()
		{
			return this.GetAvailableEatingSlotsCount() > 0;
		}

		// Token: 0x060051B5 RID: 20917 RVA: 0x0025EE1C File Offset: 0x0025D01C
		public int GetAvailableEatingSlotsCount()
		{
			return this._eatingItems.GetAvailableEatingSlotsCount(this.CanEatingMaxCount);
		}

		// Token: 0x060051B6 RID: 20918 RVA: 0x0025EE30 File Offset: 0x0025D030
		public override void Refresh()
		{
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				base.Character.CallMethod(33);
			}
		}

		// Token: 0x060051B7 RID: 20919 RVA: 0x0025EE60 File Offset: 0x0025D060
		protected override void MonitorData()
		{
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				foreach (uint fieldId in this.RelativeFieldIds)
				{
					GameDataBridge.AddDataMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
				}
				base.Character.CallMethod(33);
			}
		}

		// Token: 0x060051B8 RID: 20920 RVA: 0x0025EEF0 File Offset: 0x0025D0F0
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x060051B9 RID: 20921 RVA: 0x0025EF60 File Offset: 0x0025D160
		public override void OnDataInit()
		{
			Action onEatingItemChangeEvent = this.OnEatingItemChangeEvent;
			if (onEatingItemChangeEvent != null)
			{
				onEatingItemChangeEvent();
			}
		}

		// Token: 0x060051BA RID: 20922 RVA: 0x0025EF78 File Offset: 0x0025D178
		protected override bool IsValidMonitor()
		{
			Action onEatingItemChangeEvent = this.OnEatingItemChangeEvent;
			return onEatingItemChangeEvent != null && onEatingItemChangeEvent.GetInvocationList().Length != 0;
		}

		// Token: 0x060051BB RID: 20923 RVA: 0x0025EFA0 File Offset: 0x0025D1A0
		public unsafe static void DecodeEatingItems(EatingItems eatingItems, ref List<ValueTuple<ItemKey, short>> eatingItemList)
		{
			eatingItemList.Clear();
			for (int i = 0; i < 9; i++)
			{
				ItemKey itemKey = (ItemKey)(*(ref eatingItems.ItemKeys.FixedElementField + (IntPtr)i * 8));
				short duration = *(ref eatingItems.Durations.FixedElementField + (IntPtr)i * 2);
				eatingItemList.Add(new ValueTuple<ItemKey, short>(itemKey, duration));
			}
		}

		// Token: 0x060051BC RID: 20924 RVA: 0x0025F004 File Offset: 0x0025D204
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type == 0;
			if (flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 58U;
				if (flag2)
				{
					EatingItems eatingItems = default(EatingItems);
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref eatingItems);
					this._eatingItems = eatingItems;
					EatingItemMonitor.DecodeEatingItems(eatingItems, ref this.EatingItemList);
					Action onEatingItemChangeEvent = this.OnEatingItemChangeEvent;
					if (onEatingItemChangeEvent != null)
					{
						onEatingItemChangeEvent();
					}
				}
			}
			else
			{
				bool flag3 = wrapper.Notification.Type == 1;
				if (flag3)
				{
					bool flag4 = wrapper.Notification.MethodId == 33;
					if (flag4)
					{
						sbyte count = -1;
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref count);
						bool changeFlag = count != this.CanEatingMaxCount;
						this.CanEatingMaxCount = count;
						bool flag5 = base.Init && changeFlag;
						if (flag5)
						{
							Action onEatingItemChangeEvent2 = this.OnEatingItemChangeEvent;
							if (onEatingItemChangeEvent2 != null)
							{
								onEatingItemChangeEvent2();
							}
						}
						bool flag6 = !base.Init;
						if (flag6)
						{
							this.DataFlag = 1;
						}
					}
				}
			}
		}

		// Token: 0x060051BD RID: 20925 RVA: 0x0025F125 File Offset: 0x0025D325
		public void AddEatingItemListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnEatingItemChangeEvent -= listener;
			this.OnEatingItemChangeEvent += listener;
		}

		// Token: 0x060051BE RID: 20926 RVA: 0x0025F147 File Offset: 0x0025D347
		public void RemoveEatingItemListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnEatingItemChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x040037C4 RID: 14276
		public List<ValueTuple<ItemKey, short>> EatingItemList = new List<ValueTuple<ItemKey, short>>();

		// Token: 0x040037C5 RID: 14277
		private EatingItems _eatingItems;
	}
}
