using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006CC RID: 1740
	public class SkillQualificationMonitor : MonitorDataItemBase
	{
		// Token: 0x1400006F RID: 111
		// (add) Token: 0x060052CE RID: 21198 RVA: 0x002635BC File Offset: 0x002617BC
		// (remove) Token: 0x060052CF RID: 21199 RVA: 0x002635F4 File Offset: 0x002617F4
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnSkillQualificationBonusChangeEvent;

		// Token: 0x17000A42 RID: 2626
		// (get) Token: 0x060052D0 RID: 21200 RVA: 0x00263629 File Offset: 0x00261829
		// (set) Token: 0x060052D1 RID: 21201 RVA: 0x00263631 File Offset: 0x00261831
		public List<SkillQualificationBonus> BonusesList { get; private set; }

		// Token: 0x17000A43 RID: 2627
		// (get) Token: 0x060052D2 RID: 21202 RVA: 0x0026363A File Offset: 0x0026183A
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A44 RID: 2628
		// (get) Token: 0x060052D3 RID: 21203 RVA: 0x0026363D File Offset: 0x0026183D
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					62U
				};
			}
		}

		// Token: 0x060052D4 RID: 21204 RVA: 0x0026364D File Offset: 0x0026184D
		public SkillQualificationMonitor()
		{
			this.BonusesList = new List<SkillQualificationBonus>();
		}

		// Token: 0x060052D5 RID: 21205 RVA: 0x00263664 File Offset: 0x00261864
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

		// Token: 0x060052D6 RID: 21206 RVA: 0x002636E8 File Offset: 0x002618E8
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x060052D7 RID: 21207 RVA: 0x00263758 File Offset: 0x00261958
		public override void OnDataInit()
		{
			Action onSkillQualificationBonusChangeEvent = this.OnSkillQualificationBonusChangeEvent;
			if (onSkillQualificationBonusChangeEvent != null)
			{
				onSkillQualificationBonusChangeEvent();
			}
		}

		// Token: 0x060052D8 RID: 21208 RVA: 0x00263770 File Offset: 0x00261970
		protected override bool IsValidMonitor()
		{
			Action onSkillQualificationBonusChangeEvent = this.OnSkillQualificationBonusChangeEvent;
			return onSkillQualificationBonusChangeEvent != null && onSkillQualificationBonusChangeEvent.GetInvocationList().Length != 0;
		}

		// Token: 0x060052D9 RID: 21209 RVA: 0x00263798 File Offset: 0x00261998
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type > 0;
			if (!flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 62U;
				if (flag2)
				{
					List<SkillQualificationBonus> bonusList = null;
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref bonusList);
					this.BonusesList = (bonusList ?? new List<SkillQualificationBonus>());
					bool flag3 = !base.Init;
					if (flag3)
					{
						this.DataFlag = 1;
					}
					else
					{
						Action onSkillQualificationBonusChangeEvent = this.OnSkillQualificationBonusChangeEvent;
						if (onSkillQualificationBonusChangeEvent != null)
						{
							onSkillQualificationBonusChangeEvent();
						}
					}
				}
			}
		}

		// Token: 0x060052DA RID: 21210 RVA: 0x00263828 File Offset: 0x00261A28
		public void AddQualificationBonusListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnSkillQualificationBonusChangeEvent -= listener;
			this.OnSkillQualificationBonusChangeEvent += listener;
		}

		// Token: 0x060052DB RID: 21211 RVA: 0x0026384A File Offset: 0x00261A4A
		public void RemoveQualificationBonusListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnSkillQualificationBonusChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}
	}
}
