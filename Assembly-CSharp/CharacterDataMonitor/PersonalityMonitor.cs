using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006C7 RID: 1735
	public class PersonalityMonitor : MonitorDataItemBase
	{
		// Token: 0x14000069 RID: 105
		// (add) Token: 0x0600528E RID: 21134 RVA: 0x00262234 File Offset: 0x00260434
		// (remove) Token: 0x0600528F RID: 21135 RVA: 0x0026226C File Offset: 0x0026046C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<sbyte> OnPersonalityChangeEvent;

		// Token: 0x1400006A RID: 106
		// (add) Token: 0x06005290 RID: 21136 RVA: 0x002622A4 File Offset: 0x002604A4
		// (remove) Token: 0x06005291 RID: 21137 RVA: 0x002622DC File Offset: 0x002604DC
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int, sbyte> OnCharPersonalityChangeEvent;

		// Token: 0x17000A35 RID: 2613
		// (get) Token: 0x06005292 RID: 21138 RVA: 0x00262311 File Offset: 0x00260511
		public sbyte[] Personalities { get; }

		// Token: 0x17000A36 RID: 2614
		// (get) Token: 0x06005293 RID: 21139 RVA: 0x00262319 File Offset: 0x00260519
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					100U
				};
			}
		}

		// Token: 0x17000A37 RID: 2615
		// (get) Token: 0x06005294 RID: 21140 RVA: 0x00262329 File Offset: 0x00260529
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005295 RID: 21141 RVA: 0x0026232C File Offset: 0x0026052C
		public PersonalityMonitor()
		{
			this.Personalities = new sbyte[7];
		}

		// Token: 0x06005296 RID: 21142 RVA: 0x00262344 File Offset: 0x00260544
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

		// Token: 0x06005297 RID: 21143 RVA: 0x002623C8 File Offset: 0x002605C8
		public override void OnDataInit()
		{
			Action<sbyte> onPersonalityChangeEvent = this.OnPersonalityChangeEvent;
			if (onPersonalityChangeEvent != null)
			{
				onPersonalityChangeEvent(7);
			}
			Action<int, sbyte> onCharPersonalityChangeEvent = this.OnCharPersonalityChangeEvent;
			if (onCharPersonalityChangeEvent != null)
			{
				onCharPersonalityChangeEvent(base.CharacterId, 7);
			}
		}

		// Token: 0x06005298 RID: 21144 RVA: 0x002623F8 File Offset: 0x002605F8
		protected override bool IsValidMonitor()
		{
			Action<sbyte> onPersonalityChangeEvent = this.OnPersonalityChangeEvent;
			bool result;
			if (onPersonalityChangeEvent == null || onPersonalityChangeEvent.GetInvocationList().Length == 0)
			{
				Action<int, sbyte> onCharPersonalityChangeEvent = this.OnCharPersonalityChangeEvent;
				result = (onCharPersonalityChangeEvent != null && onCharPersonalityChangeEvent.GetInvocationList().Length != 0);
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06005299 RID: 21145 RVA: 0x0026243C File Offset: 0x0026063C
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x0600529A RID: 21146 RVA: 0x002624AC File Offset: 0x002606AC
		public unsafe override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type > 0;
			if (!flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 100U;
				if (flag2)
				{
					Personalities personalities = default(Personalities);
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref personalities);
					for (sbyte personality = 0; personality < 7; personality += 1)
					{
						bool changeFlag = this.Personalities[(int)personality] != *(ref personalities.Items.FixedElementField + personality);
						this.Personalities[(int)personality] = *(ref personalities.Items.FixedElementField + personality);
						bool flag3 = changeFlag && base.Init;
						if (flag3)
						{
							Action<sbyte> onPersonalityChangeEvent = this.OnPersonalityChangeEvent;
							if (onPersonalityChangeEvent != null)
							{
								onPersonalityChangeEvent(personality);
							}
							Action<int, sbyte> onCharPersonalityChangeEvent = this.OnCharPersonalityChangeEvent;
							if (onCharPersonalityChangeEvent != null)
							{
								onCharPersonalityChangeEvent(base.CharacterId, personality);
							}
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

		// Token: 0x0600529B RID: 21147 RVA: 0x002625B3 File Offset: 0x002607B3
		public void AddPersonalityListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnPersonalityChangeEvent -= listener;
			this.OnPersonalityChangeEvent += listener;
		}

		// Token: 0x0600529C RID: 21148 RVA: 0x002625D5 File Offset: 0x002607D5
		public void RemovePersonalityListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnPersonalityChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600529D RID: 21149 RVA: 0x002625F6 File Offset: 0x002607F6
		public void AddPersonalityListener(Action<int, sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharPersonalityChangeEvent -= listener;
			this.OnCharPersonalityChangeEvent += listener;
		}

		// Token: 0x0600529E RID: 21150 RVA: 0x00262618 File Offset: 0x00260818
		public void RemovePersonalityListener(Action<int, sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharPersonalityChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}
	}
}
