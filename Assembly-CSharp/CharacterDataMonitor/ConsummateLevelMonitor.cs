using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006BC RID: 1724
	public class ConsummateLevelMonitor : MonitorDataItemBase
	{
		// Token: 0x14000045 RID: 69
		// (add) Token: 0x06005149 RID: 20809 RVA: 0x0025D514 File Offset: 0x0025B714
		// (remove) Token: 0x0600514A RID: 20810 RVA: 0x0025D54C File Offset: 0x0025B74C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnConsummateLevelChangeEvent;

		// Token: 0x14000046 RID: 70
		// (add) Token: 0x0600514B RID: 20811 RVA: 0x0025D584 File Offset: 0x0025B784
		// (remove) Token: 0x0600514C RID: 20812 RVA: 0x0025D5BC File Offset: 0x0025B7BC
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnCreatingTypeChangeEvent;

		// Token: 0x170009F9 RID: 2553
		// (get) Token: 0x0600514D RID: 20813 RVA: 0x0025D5F1 File Offset: 0x0025B7F1
		// (set) Token: 0x0600514E RID: 20814 RVA: 0x0025D5F9 File Offset: 0x0025B7F9
		public sbyte Level { get; private set; }

		// Token: 0x170009FA RID: 2554
		// (get) Token: 0x0600514F RID: 20815 RVA: 0x0025D602 File Offset: 0x0025B802
		// (set) Token: 0x06005150 RID: 20816 RVA: 0x0025D60A File Offset: 0x0025B80A
		public byte CreatingType { get; private set; }

		// Token: 0x170009FB RID: 2555
		// (get) Token: 0x06005151 RID: 20817 RVA: 0x0025D613 File Offset: 0x0025B813
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009FC RID: 2556
		// (get) Token: 0x06005152 RID: 20818 RVA: 0x0025D616 File Offset: 0x0025B816
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					2U,
					28U
				};
			}
		}

		// Token: 0x06005153 RID: 20819 RVA: 0x0025D630 File Offset: 0x0025B830
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

		// Token: 0x06005154 RID: 20820 RVA: 0x0025D6B4 File Offset: 0x0025B8B4
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x06005155 RID: 20821 RVA: 0x0025D724 File Offset: 0x0025B924
		public override void OnDataInit()
		{
			Action onConsummateLevelChangeEvent = this.OnConsummateLevelChangeEvent;
			if (onConsummateLevelChangeEvent != null)
			{
				onConsummateLevelChangeEvent();
			}
		}

		// Token: 0x06005156 RID: 20822 RVA: 0x0025D73C File Offset: 0x0025B93C
		protected override bool IsValidMonitor()
		{
			Action onConsummateLevelChangeEvent = this.OnConsummateLevelChangeEvent;
			return onConsummateLevelChangeEvent != null && onConsummateLevelChangeEvent.GetInvocationList().Length != 0;
		}

		// Token: 0x06005157 RID: 20823 RVA: 0x0025D764 File Offset: 0x0025B964
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type == 0;
			if (flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 28U;
				if (flag2)
				{
					sbyte level = 0;
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref level);
					bool changeFlag = level != this.Level;
					this.Level = level;
					bool flag3 = changeFlag && base.Init;
					if (flag3)
					{
						Action onConsummateLevelChangeEvent = this.OnConsummateLevelChangeEvent;
						if (onConsummateLevelChangeEvent != null)
						{
							onConsummateLevelChangeEvent();
						}
					}
					else
					{
						bool flag4 = !base.Init;
						if (flag4)
						{
							this.DataFlag = 1;
						}
						else
						{
							this.OnDataInit();
						}
					}
				}
				else
				{
					bool flag5 = wrapper.Notification.Uid.SubId1 == 2U;
					if (flag5)
					{
						byte creatingType = byte.MaxValue;
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref creatingType);
						bool changeFlag2 = creatingType != this.CreatingType;
						this.CreatingType = creatingType;
						bool flag6 = changeFlag2 && base.Init;
						if (flag6)
						{
							Action onCreatingTypeChangeEvent = this.OnCreatingTypeChangeEvent;
							if (onCreatingTypeChangeEvent != null)
							{
								onCreatingTypeChangeEvent();
							}
						}
					}
				}
			}
		}

		// Token: 0x06005158 RID: 20824 RVA: 0x0025D898 File Offset: 0x0025BA98
		public void AddConsummateLevelListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnConsummateLevelChangeEvent -= listener;
			this.OnConsummateLevelChangeEvent += listener;
		}

		// Token: 0x06005159 RID: 20825 RVA: 0x0025D8BA File Offset: 0x0025BABA
		public void RemoveConsummateLevelListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnConsummateLevelChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600515A RID: 20826 RVA: 0x0025D8DB File Offset: 0x0025BADB
		public void AddOnCreatingTypeChangeEventListener(Action listener)
		{
			this.OnCreatingTypeChangeEvent -= listener;
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCreatingTypeChangeEvent += listener;
		}

		// Token: 0x0600515B RID: 20827 RVA: 0x0025D8FD File Offset: 0x0025BAFD
		public void RemoveOnCreatingTypeChangeEventListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCreatingTypeChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}
	}
}
