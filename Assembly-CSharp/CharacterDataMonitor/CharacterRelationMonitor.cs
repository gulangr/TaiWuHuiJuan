using System;
using System.Diagnostics;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006BA RID: 1722
	public class CharacterRelationMonitor : MonitorDataItemBase
	{
		// Token: 0x1400003E RID: 62
		// (add) Token: 0x06005113 RID: 20755 RVA: 0x0025C804 File Offset: 0x0025AA04
		// (remove) Token: 0x06005114 RID: 20756 RVA: 0x0025C83C File Offset: 0x0025AA3C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnRelationDataChangedEvent;

		// Token: 0x170009F1 RID: 2545
		// (get) Token: 0x06005115 RID: 20757 RVA: 0x0025C871 File Offset: 0x0025AA71
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06005116 RID: 20758 RVA: 0x0025C874 File Offset: 0x0025AA74
		protected override void MonitorData()
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			base.Character.CallMethod<int>(129, taiwuCharId);
		}

		// Token: 0x06005117 RID: 20759 RVA: 0x0025C89F File Offset: 0x0025AA9F
		public override void UnMonitorData()
		{
		}

		// Token: 0x06005118 RID: 20760 RVA: 0x0025C8A4 File Offset: 0x0025AAA4
		public override void Refresh()
		{
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				this.DataFlag = 1;
				int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				base.Character.CallMethod<int>(129, taiwuCharId);
			}
		}

		// Token: 0x06005119 RID: 20761 RVA: 0x0025C8E7 File Offset: 0x0025AAE7
		public override void OnDataInit()
		{
			Action onRelationDataChangedEvent = this.OnRelationDataChangedEvent;
			if (onRelationDataChangedEvent != null)
			{
				onRelationDataChangedEvent();
			}
		}

		// Token: 0x0600511A RID: 20762 RVA: 0x0025C8FC File Offset: 0x0025AAFC
		protected override bool IsValidMonitor()
		{
			Action onRelationDataChangedEvent = this.OnRelationDataChangedEvent;
			return onRelationDataChangedEvent != null && onRelationDataChangedEvent.GetInvocationList().Length != 0;
		}

		// Token: 0x0600511B RID: 20763 RVA: 0x0025C930 File Offset: 0x0025AB30
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type == 1;
			if (flag)
			{
				bool flag2 = wrapper.Notification.MethodId == 129;
				if (flag2)
				{
					ValueTuple<ushort, ushort> tuple = new ValueTuple<ushort, ushort>(0, 0);
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref tuple);
					bool changeFlag = this.RelationBitFlag != tuple.Item2;
					this.RelationBitFlag = tuple.Item2;
					bool flag3 = base.Init && changeFlag;
					if (flag3)
					{
						Action onRelationDataChangedEvent = this.OnRelationDataChangedEvent;
						if (onRelationDataChangedEvent != null)
						{
							onRelationDataChangedEvent();
						}
					}
					bool flag4 = !base.Init;
					if (flag4)
					{
						this.DataFlag = 1;
					}
				}
			}
		}

		// Token: 0x0600511C RID: 20764 RVA: 0x0025C9E3 File Offset: 0x0025ABE3
		public void AddRelationShipListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnRelationDataChangedEvent -= listener;
			this.OnRelationDataChangedEvent += listener;
		}

		// Token: 0x0600511D RID: 20765 RVA: 0x0025CA05 File Offset: 0x0025AC05
		public void RemoveRelationShipListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnRelationDataChangedEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0400379C RID: 14236
		public ushort RelationBitFlag;
	}
}
