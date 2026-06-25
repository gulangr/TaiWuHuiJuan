using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006C4 RID: 1732
	public class LifeSkillMonitor : MonitorDataItemBase
	{
		// Token: 0x14000064 RID: 100
		// (add) Token: 0x0600524F RID: 21071 RVA: 0x0026167C File Offset: 0x0025F87C
		// (remove) Token: 0x06005250 RID: 21072 RVA: 0x002616B4 File Offset: 0x0025F8B4
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnGetQualificationGrowthType;

		// Token: 0x14000065 RID: 101
		// (add) Token: 0x06005251 RID: 21073 RVA: 0x002616EC File Offset: 0x0025F8EC
		// (remove) Token: 0x06005252 RID: 21074 RVA: 0x00261724 File Offset: 0x0025F924
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnQualificationsChangeEvent;

		// Token: 0x14000066 RID: 102
		// (add) Token: 0x06005253 RID: 21075 RVA: 0x0026175C File Offset: 0x0025F95C
		// (remove) Token: 0x06005254 RID: 21076 RVA: 0x00261794 File Offset: 0x0025F994
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int> OnCharQualificationsChangeEvent;

		// Token: 0x14000067 RID: 103
		// (add) Token: 0x06005255 RID: 21077 RVA: 0x002617CC File Offset: 0x0025F9CC
		// (remove) Token: 0x06005256 RID: 21078 RVA: 0x00261804 File Offset: 0x0025FA04
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnAttainmentChangeEvent;

		// Token: 0x14000068 RID: 104
		// (add) Token: 0x06005257 RID: 21079 RVA: 0x0026183C File Offset: 0x0025FA3C
		// (remove) Token: 0x06005258 RID: 21080 RVA: 0x00261874 File Offset: 0x0025FA74
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnLearnedSkillsChangeEvent;

		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x06005259 RID: 21081 RVA: 0x002618A9 File Offset: 0x0025FAA9
		public short ActualAge
		{
			get
			{
				return this._ageMonitor.ActualAge;
			}
		}

		// Token: 0x17000A26 RID: 2598
		// (get) Token: 0x0600525A RID: 21082 RVA: 0x002618B6 File Offset: 0x0025FAB6
		// (set) Token: 0x0600525B RID: 21083 RVA: 0x002618BE File Offset: 0x0025FABE
		public sbyte GrowthType { get; private set; }

		// Token: 0x17000A27 RID: 2599
		// (get) Token: 0x0600525C RID: 21084 RVA: 0x002618C7 File Offset: 0x0025FAC7
		public short[] Qualifications { get; }

		// Token: 0x17000A28 RID: 2600
		// (get) Token: 0x0600525D RID: 21085 RVA: 0x002618CF File Offset: 0x0025FACF
		public short[] Attainments { get; }

		// Token: 0x17000A29 RID: 2601
		// (get) Token: 0x0600525E RID: 21086 RVA: 0x002618D7 File Offset: 0x0025FAD7
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					29U,
					97U,
					96U,
					31U
				};
			}
		}

		// Token: 0x17000A2A RID: 2602
		// (get) Token: 0x0600525F RID: 21087 RVA: 0x00261902 File Offset: 0x0025FB02
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005260 RID: 21088 RVA: 0x00261905 File Offset: 0x0025FB05
		public LifeSkillMonitor()
		{
			this.Qualifications = new short[16];
			this.Attainments = new short[16];
			this.LearnedLifeSkills = new List<LifeSkillItem>();
		}

		// Token: 0x06005261 RID: 21089 RVA: 0x00261934 File Offset: 0x0025FB34
		protected override void MonitorData()
		{
			bool flag = this._ageMonitor == null;
			if (flag)
			{
				this._ageMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AgeHealthMonitor>(base.CharacterId, false);
			}
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				foreach (uint fieldId in this.RelativeFieldIds)
				{
					GameDataBridge.AddDataMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
				}
			}
		}

		// Token: 0x06005262 RID: 21090 RVA: 0x002619DC File Offset: 0x0025FBDC
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x06005263 RID: 21091 RVA: 0x00261A4C File Offset: 0x0025FC4C
		public override void OnDataInit()
		{
			Action onGetQualificationGrowthType = this.OnGetQualificationGrowthType;
			if (onGetQualificationGrowthType != null)
			{
				onGetQualificationGrowthType();
			}
			Action onQualificationsChangeEvent = this.OnQualificationsChangeEvent;
			if (onQualificationsChangeEvent != null)
			{
				onQualificationsChangeEvent();
			}
			Action<int> onCharQualificationsChangeEvent = this.OnCharQualificationsChangeEvent;
			if (onCharQualificationsChangeEvent != null)
			{
				onCharQualificationsChangeEvent(base.CharacterId);
			}
			Action onAttainmentChangeEvent = this.OnAttainmentChangeEvent;
			if (onAttainmentChangeEvent != null)
			{
				onAttainmentChangeEvent();
			}
			Action onLearnedSkillsChangeEvent = this.OnLearnedSkillsChangeEvent;
			if (onLearnedSkillsChangeEvent != null)
			{
				onLearnedSkillsChangeEvent();
			}
		}

		// Token: 0x06005264 RID: 21092 RVA: 0x00261ABC File Offset: 0x0025FCBC
		public unsafe override void InitFromDeadCharacter(DeadCharacter deadCharacter)
		{
			for (sbyte i = 0; i < 16; i += 1)
			{
				this.Qualifications[(int)i] = *(ref deadCharacter.BaseLifeSkillQualifications.Items.FixedElementField + (IntPtr)i * 2);
			}
			this.DataFlag = 2;
		}

		// Token: 0x06005265 RID: 21093 RVA: 0x00261B04 File Offset: 0x0025FD04
		protected override bool IsValidMonitor()
		{
			Action onGetQualificationGrowthType = this.OnGetQualificationGrowthType;
			bool flag = onGetQualificationGrowthType != null && onGetQualificationGrowthType.GetInvocationList().Length != 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				Action onQualificationsChangeEvent = this.OnQualificationsChangeEvent;
				bool flag2;
				if (onQualificationsChangeEvent == null || onQualificationsChangeEvent.GetInvocationList().Length == 0)
				{
					Action<int> onCharQualificationsChangeEvent = this.OnCharQualificationsChangeEvent;
					flag2 = (onCharQualificationsChangeEvent != null && onCharQualificationsChangeEvent.GetInvocationList().Length != 0);
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					result = true;
				}
				else
				{
					Action onAttainmentChangeEvent = this.OnAttainmentChangeEvent;
					bool flag4 = onAttainmentChangeEvent != null && onAttainmentChangeEvent.GetInvocationList().Length != 0;
					if (flag4)
					{
						result = true;
					}
					else
					{
						Action onLearnedSkillsChangeEvent = this.OnLearnedSkillsChangeEvent;
						bool flag5 = onLearnedSkillsChangeEvent != null && onLearnedSkillsChangeEvent.GetInvocationList().Length != 0;
						result = flag5;
					}
				}
			}
			return result;
		}

		// Token: 0x06005266 RID: 21094 RVA: 0x00261BAC File Offset: 0x0025FDAC
		public unsafe override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type > 0;
			if (!flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 29U;
				if (flag2)
				{
					List<LifeSkillItem> list = null;
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref list);
					this.LearnedLifeSkills.Clear();
					bool flag3 = list != null;
					if (flag3)
					{
						this.LearnedLifeSkills.AddRange(list);
					}
					bool init = base.Init;
					if (init)
					{
						Action onLearnedSkillsChangeEvent = this.OnLearnedSkillsChangeEvent;
						if (onLearnedSkillsChangeEvent != null)
						{
							onLearnedSkillsChangeEvent();
						}
					}
				}
				else
				{
					bool flag4 = wrapper.Notification.Uid.SubId1 == 97U;
					if (flag4)
					{
						LifeSkillShorts attainments = default(LifeSkillShorts);
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref attainments);
						bool changeFlag = false;
						for (sbyte i = 0; i < 16; i += 1)
						{
							changeFlag = (changeFlag || this.Attainments[(int)i] != *(ref attainments.Items.FixedElementField + (IntPtr)i * 2));
							this.Attainments[(int)i] = *(ref attainments.Items.FixedElementField + (IntPtr)i * 2);
						}
						bool flag5 = changeFlag && base.Init;
						if (flag5)
						{
							Action onAttainmentChangeEvent = this.OnAttainmentChangeEvent;
							if (onAttainmentChangeEvent != null)
							{
								onAttainmentChangeEvent();
							}
						}
					}
					else
					{
						bool flag6 = wrapper.Notification.Uid.SubId1 == 96U;
						if (flag6)
						{
							LifeSkillShorts qualifications = default(LifeSkillShorts);
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref qualifications);
							bool changeFlag2 = false;
							for (sbyte j = 0; j < 16; j += 1)
							{
								changeFlag2 = (changeFlag2 || this.Qualifications[(int)j] != *(ref qualifications.Items.FixedElementField + (IntPtr)j * 2));
								this.Qualifications[(int)j] = *(ref qualifications.Items.FixedElementField + (IntPtr)j * 2);
							}
							bool flag7 = changeFlag2 && base.Init;
							if (flag7)
							{
								Action onQualificationsChangeEvent = this.OnQualificationsChangeEvent;
								if (onQualificationsChangeEvent != null)
								{
									onQualificationsChangeEvent();
								}
								Action<int> onCharQualificationsChangeEvent = this.OnCharQualificationsChangeEvent;
								if (onCharQualificationsChangeEvent != null)
								{
									onCharQualificationsChangeEvent(base.CharacterId);
								}
							}
						}
						else
						{
							bool flag8 = wrapper.Notification.Uid.SubId1 == 31U;
							if (flag8)
							{
								sbyte type = -1;
								Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref type);
								this.GrowthType = type;
								bool flag9 = !base.Init;
								if (flag9)
								{
									this.DataFlag = 1;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06005267 RID: 21095 RVA: 0x00261E52 File Offset: 0x00260052
		public void AddActualAgeListener(Action listener)
		{
			this._ageMonitor.AddActualAgeListener(listener);
		}

		// Token: 0x06005268 RID: 21096 RVA: 0x00261E62 File Offset: 0x00260062
		public void RemoveActualAgeListener(Action listener)
		{
			this._ageMonitor.RemoveActualAgeListener(listener);
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005269 RID: 21097 RVA: 0x00261E79 File Offset: 0x00260079
		public void AddGrowthTypeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnGetQualificationGrowthType -= listener;
			this.OnGetQualificationGrowthType += listener;
		}

		// Token: 0x0600526A RID: 21098 RVA: 0x00261E9B File Offset: 0x0026009B
		public void RemoveGrowthTypeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnGetQualificationGrowthType -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600526B RID: 21099 RVA: 0x00261EBC File Offset: 0x002600BC
		public void AddQualificationsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnQualificationsChangeEvent -= listener;
			this.OnQualificationsChangeEvent += listener;
		}

		// Token: 0x0600526C RID: 21100 RVA: 0x00261EDE File Offset: 0x002600DE
		public void RemoveQualificationsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnQualificationsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600526D RID: 21101 RVA: 0x00261EFF File Offset: 0x002600FF
		public void AddQualificationsListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharQualificationsChangeEvent -= listener;
			this.OnCharQualificationsChangeEvent += listener;
		}

		// Token: 0x0600526E RID: 21102 RVA: 0x00261F21 File Offset: 0x00260121
		public void RemoveQualificationsListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharQualificationsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600526F RID: 21103 RVA: 0x00261F42 File Offset: 0x00260142
		public void AddAttainmentListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAttainmentChangeEvent -= listener;
			this.OnAttainmentChangeEvent += listener;
		}

		// Token: 0x06005270 RID: 21104 RVA: 0x00261F64 File Offset: 0x00260164
		public void RemoveAttainmentListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAttainmentChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005271 RID: 21105 RVA: 0x00261F85 File Offset: 0x00260185
		public void AddLearnedSkillsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnLearnedSkillsChangeEvent -= listener;
			this.OnLearnedSkillsChangeEvent += listener;
		}

		// Token: 0x06005272 RID: 21106 RVA: 0x00261FA7 File Offset: 0x002601A7
		public void RemoveLearnedSkillsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnLearnedSkillsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x040037F6 RID: 14326
		public readonly List<LifeSkillItem> LearnedLifeSkills;

		// Token: 0x040037F7 RID: 14327
		private AgeHealthMonitor _ageMonitor;
	}
}
