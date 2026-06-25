using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006BB RID: 1723
	public class CombatSkillMonitor : MonitorDataItemBase
	{
		// Token: 0x1400003F RID: 63
		// (add) Token: 0x0600511F RID: 20767 RVA: 0x0025CA30 File Offset: 0x0025AC30
		// (remove) Token: 0x06005120 RID: 20768 RVA: 0x0025CA68 File Offset: 0x0025AC68
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnLearnedSkillsChangeEvent;

		// Token: 0x14000040 RID: 64
		// (add) Token: 0x06005121 RID: 20769 RVA: 0x0025CAA0 File Offset: 0x0025ACA0
		// (remove) Token: 0x06005122 RID: 20770 RVA: 0x0025CAD8 File Offset: 0x0025ACD8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnGetQualificationGrowthType;

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x06005123 RID: 20771 RVA: 0x0025CB10 File Offset: 0x0025AD10
		// (remove) Token: 0x06005124 RID: 20772 RVA: 0x0025CB48 File Offset: 0x0025AD48
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnQualificationsChangeEvent;

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x06005125 RID: 20773 RVA: 0x0025CB80 File Offset: 0x0025AD80
		// (remove) Token: 0x06005126 RID: 20774 RVA: 0x0025CBB8 File Offset: 0x0025ADB8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int> OnCharQualificationsChangeEvent;

		// Token: 0x14000043 RID: 67
		// (add) Token: 0x06005127 RID: 20775 RVA: 0x0025CBF0 File Offset: 0x0025ADF0
		// (remove) Token: 0x06005128 RID: 20776 RVA: 0x0025CC28 File Offset: 0x0025AE28
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnAttainmentChangeEvent;

		// Token: 0x14000044 RID: 68
		// (add) Token: 0x06005129 RID: 20777 RVA: 0x0025CC60 File Offset: 0x0025AE60
		// (remove) Token: 0x0600512A RID: 20778 RVA: 0x0025CC98 File Offset: 0x0025AE98
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnAttainmentPanelsChangeEvent;

		// Token: 0x170009F2 RID: 2546
		// (get) Token: 0x0600512B RID: 20779 RVA: 0x0025CCCD File Offset: 0x0025AECD
		public short ActualAge
		{
			get
			{
				return this._ageMonitor.ActualAge;
			}
		}

		// Token: 0x170009F3 RID: 2547
		// (get) Token: 0x0600512C RID: 20780 RVA: 0x0025CCDA File Offset: 0x0025AEDA
		// (set) Token: 0x0600512D RID: 20781 RVA: 0x0025CCE2 File Offset: 0x0025AEE2
		public short[] AttainmentPanels { get; private set; }

		// Token: 0x170009F4 RID: 2548
		// (get) Token: 0x0600512E RID: 20782 RVA: 0x0025CCEB File Offset: 0x0025AEEB
		// (set) Token: 0x0600512F RID: 20783 RVA: 0x0025CCF3 File Offset: 0x0025AEF3
		public sbyte GrowthType { get; private set; }

		// Token: 0x170009F5 RID: 2549
		// (get) Token: 0x06005130 RID: 20784 RVA: 0x0025CCFC File Offset: 0x0025AEFC
		public short[] Qualifications { get; }

		// Token: 0x170009F6 RID: 2550
		// (get) Token: 0x06005131 RID: 20785 RVA: 0x0025CD04 File Offset: 0x0025AF04
		public short[] Attainments { get; }

		// Token: 0x170009F7 RID: 2551
		// (get) Token: 0x06005132 RID: 20786 RVA: 0x0025CD0C File Offset: 0x0025AF0C
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009F8 RID: 2552
		// (get) Token: 0x06005133 RID: 20787 RVA: 0x0025CD0F File Offset: 0x0025AF0F
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					59U,
					99U,
					98U,
					61U,
					33U
				};
			}
		}

		// Token: 0x06005134 RID: 20788 RVA: 0x0025CD43 File Offset: 0x0025AF43
		public CombatSkillMonitor()
		{
			this.LearnedCombatSkills = new List<short>();
			this.Qualifications = new short[14];
			this.Attainments = new short[14];
		}

		// Token: 0x06005135 RID: 20789 RVA: 0x0025CD74 File Offset: 0x0025AF74
		protected override void MonitorData()
		{
			if (this._ageMonitor == null)
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

		// Token: 0x06005136 RID: 20790 RVA: 0x0025CE14 File Offset: 0x0025B014
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
			AgeHealthMonitor ageMonitor = this._ageMonitor;
			if (ageMonitor != null)
			{
				ageMonitor.RemoveActualAgeListener(new Action(this.OnDataInit));
			}
		}

		// Token: 0x06005137 RID: 20791 RVA: 0x0025CEA4 File Offset: 0x0025B0A4
		public override void OnDataInit()
		{
			Action onLearnedSkillsChangeEvent = this.OnLearnedSkillsChangeEvent;
			if (onLearnedSkillsChangeEvent != null)
			{
				onLearnedSkillsChangeEvent();
			}
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
			Action onAttainmentPanelsChangeEvent = this.OnAttainmentPanelsChangeEvent;
			if (onAttainmentPanelsChangeEvent != null)
			{
				onAttainmentPanelsChangeEvent();
			}
		}

		// Token: 0x06005138 RID: 20792 RVA: 0x0025CF24 File Offset: 0x0025B124
		public unsafe override void InitFromDeadCharacter(DeadCharacter deadCharacter)
		{
			for (sbyte i = 0; i < 14; i += 1)
			{
				this.Qualifications[(int)i] = *(ref deadCharacter.BaseCombatSkillQualifications.Items.FixedElementField + (IntPtr)i * 2);
			}
			this.DataFlag = 2;
		}

		// Token: 0x06005139 RID: 20793 RVA: 0x0025CF6C File Offset: 0x0025B16C
		protected override bool IsValidMonitor()
		{
			Action onLearnedSkillsChangeEvent = this.OnLearnedSkillsChangeEvent;
			bool flag = onLearnedSkillsChangeEvent != null && onLearnedSkillsChangeEvent.GetInvocationList().Length != 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				Action onGetQualificationGrowthType = this.OnGetQualificationGrowthType;
				bool flag2 = onGetQualificationGrowthType != null && onGetQualificationGrowthType.GetInvocationList().Length != 0;
				if (flag2)
				{
					result = true;
				}
				else
				{
					Action onQualificationsChangeEvent = this.OnQualificationsChangeEvent;
					bool flag3;
					if (onQualificationsChangeEvent == null || onQualificationsChangeEvent.GetInvocationList().Length == 0)
					{
						Action<int> onCharQualificationsChangeEvent = this.OnCharQualificationsChangeEvent;
						flag3 = (onCharQualificationsChangeEvent != null && onCharQualificationsChangeEvent.GetInvocationList().Length != 0);
					}
					else
					{
						flag3 = true;
					}
					bool flag4 = flag3;
					if (flag4)
					{
						result = true;
					}
					else
					{
						Action onAttainmentChangeEvent = this.OnAttainmentChangeEvent;
						bool flag5 = onAttainmentChangeEvent != null && onAttainmentChangeEvent.GetInvocationList().Length != 0;
						if (flag5)
						{
							result = true;
						}
						else
						{
							Action onAttainmentPanelsChangeEvent = this.OnAttainmentPanelsChangeEvent;
							bool flag6 = onAttainmentPanelsChangeEvent != null && onAttainmentPanelsChangeEvent.GetInvocationList().Length != 0;
							result = flag6;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600513A RID: 20794 RVA: 0x0025D038 File Offset: 0x0025B238
		public unsafe override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type > 0;
			if (!flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 59U;
				if (flag2)
				{
					List<short> learnedSkills = new List<short>();
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref learnedSkills);
					this.LearnedCombatSkills.Clear();
					this.LearnedCombatSkills.AddRange(learnedSkills);
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
					bool flag3 = wrapper.Notification.Uid.SubId1 == 99U;
					if (flag3)
					{
						CombatSkillShorts attainments = default(CombatSkillShorts);
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref attainments);
						bool changeFlag = false;
						for (sbyte i = 0; i < 14; i += 1)
						{
							changeFlag = (changeFlag || this.Attainments[(int)i] != *(ref attainments.Items.FixedElementField + (IntPtr)i * 2));
							this.Attainments[(int)i] = *(ref attainments.Items.FixedElementField + (IntPtr)i * 2);
						}
						bool flag4 = changeFlag && base.Init;
						if (flag4)
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
						bool flag5 = wrapper.Notification.Uid.SubId1 == 98U;
						if (flag5)
						{
							CombatSkillShorts qualifications = default(CombatSkillShorts);
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref qualifications);
							bool changeFlag2 = false;
							for (sbyte j = 0; j < 14; j += 1)
							{
								changeFlag2 = (changeFlag2 || this.Qualifications[(int)j] != *(ref qualifications.Items.FixedElementField + (IntPtr)j * 2));
								this.Qualifications[(int)j] = *(ref qualifications.Items.FixedElementField + (IntPtr)j * 2);
							}
							bool flag6 = changeFlag2 && base.Init;
							if (flag6)
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
							bool flag7 = wrapper.Notification.Uid.SubId1 == 61U;
							if (flag7)
							{
								short[] panelsData = null;
								Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref panelsData);
								bool changeFlag3 = this.AttainmentPanels == null || !CombatSkillAttainmentPanelsHelper.EqualAll(this.AttainmentPanels, panelsData);
								this.AttainmentPanels = panelsData;
								bool flag8 = changeFlag3 && base.Init;
								if (flag8)
								{
									Action onAttainmentPanelsChangeEvent = this.OnAttainmentPanelsChangeEvent;
									if (onAttainmentPanelsChangeEvent != null)
									{
										onAttainmentPanelsChangeEvent();
									}
								}
							}
							else
							{
								bool flag9 = wrapper.Notification.Uid.SubId1 == 33U;
								if (flag9)
								{
									sbyte type = -1;
									Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref type);
									this.GrowthType = type;
									bool flag10 = !base.Init;
									if (flag10)
									{
										this.DataFlag = 1;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600513B RID: 20795 RVA: 0x0025D360 File Offset: 0x0025B560
		public void AddActualAgeListener(Action listener)
		{
			this._ageMonitor.AddActualAgeListener(listener);
		}

		// Token: 0x0600513C RID: 20796 RVA: 0x0025D370 File Offset: 0x0025B570
		public void RemoveActualAgeListener(Action listener)
		{
			this._ageMonitor.RemoveActualAgeListener(listener);
		}

		// Token: 0x0600513D RID: 20797 RVA: 0x0025D380 File Offset: 0x0025B580
		public void AddGrowthTypeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnGetQualificationGrowthType -= listener;
			this.OnGetQualificationGrowthType += listener;
		}

		// Token: 0x0600513E RID: 20798 RVA: 0x0025D3A2 File Offset: 0x0025B5A2
		public void RemoveGrowthTypeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnGetQualificationGrowthType -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600513F RID: 20799 RVA: 0x0025D3C3 File Offset: 0x0025B5C3
		public void AddQualificationsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnQualificationsChangeEvent -= listener;
			this.OnQualificationsChangeEvent += listener;
		}

		// Token: 0x06005140 RID: 20800 RVA: 0x0025D3E5 File Offset: 0x0025B5E5
		public void RemoveQualificationsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnQualificationsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005141 RID: 20801 RVA: 0x0025D406 File Offset: 0x0025B606
		public void AddQualificationsListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharQualificationsChangeEvent -= listener;
			this.OnCharQualificationsChangeEvent += listener;
		}

		// Token: 0x06005142 RID: 20802 RVA: 0x0025D428 File Offset: 0x0025B628
		public void RemoveQualificationsListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharQualificationsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005143 RID: 20803 RVA: 0x0025D449 File Offset: 0x0025B649
		public void AddAttainmentListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAttainmentChangeEvent -= listener;
			this.OnAttainmentChangeEvent += listener;
		}

		// Token: 0x06005144 RID: 20804 RVA: 0x0025D46B File Offset: 0x0025B66B
		public void RemoveAttainmentListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAttainmentChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005145 RID: 20805 RVA: 0x0025D48C File Offset: 0x0025B68C
		public void AddLearnedSkillsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnLearnedSkillsChangeEvent -= listener;
			this.OnLearnedSkillsChangeEvent += listener;
		}

		// Token: 0x06005146 RID: 20806 RVA: 0x0025D4AE File Offset: 0x0025B6AE
		public void RemoveLearnedSkillsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnLearnedSkillsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005147 RID: 20807 RVA: 0x0025D4CF File Offset: 0x0025B6CF
		public void AddAttainmentPanelsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAttainmentPanelsChangeEvent -= listener;
			this.OnAttainmentPanelsChangeEvent += listener;
		}

		// Token: 0x06005148 RID: 20808 RVA: 0x0025D4F1 File Offset: 0x0025B6F1
		public void RemoveAttainmentPanelsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAttainmentPanelsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x040037A3 RID: 14243
		public readonly List<short> LearnedCombatSkills;

		// Token: 0x040037A8 RID: 14248
		private AgeHealthMonitor _ageMonitor;
	}
}
