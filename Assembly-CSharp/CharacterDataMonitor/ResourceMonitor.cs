using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006C8 RID: 1736
	public class ResourceMonitor : MonitorDataItemBase
	{
		// Token: 0x1400006B RID: 107
		// (add) Token: 0x0600529F RID: 21151 RVA: 0x0026263C File Offset: 0x0026083C
		// (remove) Token: 0x060052A0 RID: 21152 RVA: 0x00262674 File Offset: 0x00260874
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<sbyte> OnResourceChangeEvent;

		// Token: 0x17000A38 RID: 2616
		// (get) Token: 0x060052A1 RID: 21153 RVA: 0x002626A9 File Offset: 0x002608A9
		public int[] Resources { get; }

		// Token: 0x17000A39 RID: 2617
		// (get) Token: 0x060052A2 RID: 21154 RVA: 0x002626B1 File Offset: 0x002608B1
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A3A RID: 2618
		// (get) Token: 0x060052A3 RID: 21155 RVA: 0x002626B4 File Offset: 0x002608B4
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					66U,
					34U
				};
			}
		}

		// Token: 0x060052A4 RID: 21156 RVA: 0x002626CD File Offset: 0x002608CD
		public ResourceMonitor()
		{
			this.Resources = new int[8];
		}

		// Token: 0x060052A5 RID: 21157 RVA: 0x002626E4 File Offset: 0x002608E4
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

		// Token: 0x060052A6 RID: 21158 RVA: 0x00262768 File Offset: 0x00260968
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x060052A7 RID: 21159 RVA: 0x002627D8 File Offset: 0x002609D8
		public override void OnDataInit()
		{
			Action<sbyte> onResourceChangeEvent = this.OnResourceChangeEvent;
			if (onResourceChangeEvent != null)
			{
				onResourceChangeEvent(8);
			}
		}

		// Token: 0x060052A8 RID: 21160 RVA: 0x002627F0 File Offset: 0x002609F0
		protected override bool IsValidMonitor()
		{
			Action<sbyte> onResourceChangeEvent = this.OnResourceChangeEvent;
			return onResourceChangeEvent != null && onResourceChangeEvent.GetInvocationList().Length != 0;
		}

		// Token: 0x060052A9 RID: 21161 RVA: 0x00262818 File Offset: 0x00260A18
		public unsafe override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type > 0;
			if (!flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 34U;
				if (flag2)
				{
					ResourceInts resources = default(ResourceInts);
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref resources);
					for (sbyte resourceType = 0; resourceType < 8; resourceType += 1)
					{
						int value = *(ref resources.Items.FixedElementField + (IntPtr)resourceType * 4);
						bool changeFlag = value != this.Resources[(int)resourceType];
						this.Resources[(int)resourceType] = value;
						bool flag3 = base.Init && changeFlag;
						if (flag3)
						{
							Action<sbyte> onResourceChangeEvent = this.OnResourceChangeEvent;
							if (onResourceChangeEvent != null)
							{
								onResourceChangeEvent(resourceType);
							}
						}
					}
					bool flag4 = !base.Init;
					if (flag4)
					{
						this.DataFlag = 1;
					}
				}
				else
				{
					bool flag5 = wrapper.Notification.Uid.SubId1 == 66U;
					if (flag5)
					{
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref this.Exp);
					}
				}
			}
		}

		// Token: 0x060052AA RID: 21162 RVA: 0x00262932 File Offset: 0x00260B32
		public void AddResourceListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnResourceChangeEvent -= listener;
			this.OnResourceChangeEvent += listener;
		}

		// Token: 0x060052AB RID: 21163 RVA: 0x00262954 File Offset: 0x00260B54
		public void RemoveResourceListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnResourceChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x04003803 RID: 14339
		public int Exp;
	}
}
