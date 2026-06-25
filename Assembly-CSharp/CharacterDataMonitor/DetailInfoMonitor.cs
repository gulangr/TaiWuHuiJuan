using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006BD RID: 1725
	public class DetailInfoMonitor : MonitorDataItemBase
	{
		// Token: 0x14000047 RID: 71
		// (add) Token: 0x0600515D RID: 20829 RVA: 0x0025D928 File Offset: 0x0025BB28
		// (remove) Token: 0x0600515E RID: 20830 RVA: 0x0025D960 File Offset: 0x0025BB60
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnOrganizationInfoChangeEvent;

		// Token: 0x14000048 RID: 72
		// (add) Token: 0x0600515F RID: 20831 RVA: 0x0025D998 File Offset: 0x0025BB98
		// (remove) Token: 0x06005160 RID: 20832 RVA: 0x0025D9D0 File Offset: 0x0025BBD0
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnFameTypeChangeEvent;

		// Token: 0x14000049 RID: 73
		// (add) Token: 0x06005161 RID: 20833 RVA: 0x0025DA08 File Offset: 0x0025BC08
		// (remove) Token: 0x06005162 RID: 20834 RVA: 0x0025DA40 File Offset: 0x0025BC40
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnAttractionChangeEvent;

		// Token: 0x1400004A RID: 74
		// (add) Token: 0x06005163 RID: 20835 RVA: 0x0025DA78 File Offset: 0x0025BC78
		// (remove) Token: 0x06005164 RID: 20836 RVA: 0x0025DAB0 File Offset: 0x0025BCB0
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnHappinessChangeEvent;

		// Token: 0x1400004B RID: 75
		// (add) Token: 0x06005165 RID: 20837 RVA: 0x0025DAE8 File Offset: 0x0025BCE8
		// (remove) Token: 0x06005166 RID: 20838 RVA: 0x0025DB20 File Offset: 0x0025BD20
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnBehaviorChangeEvent;

		// Token: 0x1400004C RID: 76
		// (add) Token: 0x06005167 RID: 20839 RVA: 0x0025DB58 File Offset: 0x0025BD58
		// (remove) Token: 0x06005168 RID: 20840 RVA: 0x0025DB90 File Offset: 0x0025BD90
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnSamsaraChangeEvent;

		// Token: 0x170009FD RID: 2557
		// (get) Token: 0x06005169 RID: 20841 RVA: 0x0025DBC5 File Offset: 0x0025BDC5
		// (set) Token: 0x0600516A RID: 20842 RVA: 0x0025DBCD File Offset: 0x0025BDCD
		public OrganizationInfo OrganizationInfo { get; private set; }

		// Token: 0x170009FE RID: 2558
		// (get) Token: 0x0600516B RID: 20843 RVA: 0x0025DBD6 File Offset: 0x0025BDD6
		// (set) Token: 0x0600516C RID: 20844 RVA: 0x0025DBDE File Offset: 0x0025BDDE
		public bool IsReclusiveChar { get; private set; }

		// Token: 0x170009FF RID: 2559
		// (get) Token: 0x0600516D RID: 20845 RVA: 0x0025DBE7 File Offset: 0x0025BDE7
		// (set) Token: 0x0600516E RID: 20846 RVA: 0x0025DBEF File Offset: 0x0025BDEF
		[Obsolete("use FameType to refresh view")]
		public sbyte Fame { get; private set; }

		// Token: 0x17000A00 RID: 2560
		// (get) Token: 0x0600516F RID: 20847 RVA: 0x0025DBF8 File Offset: 0x0025BDF8
		// (set) Token: 0x06005170 RID: 20848 RVA: 0x0025DC00 File Offset: 0x0025BE00
		public sbyte FameType { get; private set; }

		// Token: 0x17000A01 RID: 2561
		// (get) Token: 0x06005171 RID: 20849 RVA: 0x0025DC09 File Offset: 0x0025BE09
		// (set) Token: 0x06005172 RID: 20850 RVA: 0x0025DC11 File Offset: 0x0025BE11
		public short Attraction { get; private set; }

		// Token: 0x17000A02 RID: 2562
		// (get) Token: 0x06005173 RID: 20851 RVA: 0x0025DC1C File Offset: 0x0025BE1C
		// (set) Token: 0x06005174 RID: 20852 RVA: 0x0025DC59 File Offset: 0x0025BE59
		public sbyte Happiness
		{
			get
			{
				bool flag = base.Character.CombatCharacter != null;
				sbyte happiness;
				if (flag)
				{
					happiness = base.Character.CombatCharacter.Happiness;
				}
				else
				{
					happiness = this._happiness;
				}
				return happiness;
			}
			private set
			{
				this._happiness = value;
			}
		}

		// Token: 0x17000A03 RID: 2563
		// (get) Token: 0x06005175 RID: 20853 RVA: 0x0025DC62 File Offset: 0x0025BE62
		// (set) Token: 0x06005176 RID: 20854 RVA: 0x0025DC6A File Offset: 0x0025BE6A
		public short Behavior { get; private set; }

		// Token: 0x17000A04 RID: 2564
		// (get) Token: 0x06005177 RID: 20855 RVA: 0x0025DC73 File Offset: 0x0025BE73
		// (set) Token: 0x06005178 RID: 20856 RVA: 0x0025DC7B File Offset: 0x0025BE7B
		public PreexistenceCharIds PreexistenceCharIds { get; private set; }

		// Token: 0x17000A05 RID: 2565
		// (get) Token: 0x06005179 RID: 20857 RVA: 0x0025DC84 File Offset: 0x0025BE84
		// (set) Token: 0x0600517A RID: 20858 RVA: 0x0025DC8C File Offset: 0x0025BE8C
		public List<FameActionRecord> FameActionRecords { get; private set; }

		// Token: 0x17000A06 RID: 2566
		// (get) Token: 0x0600517B RID: 20859 RVA: 0x0025DC98 File Offset: 0x0025BE98
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					8U,
					76U,
					78U,
					6U,
					77U,
					63U,
					41U
				};
			}
		}

		// Token: 0x17000A07 RID: 2567
		// (get) Token: 0x0600517C RID: 20860 RVA: 0x0025DCE7 File Offset: 0x0025BEE7
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600517D RID: 20861 RVA: 0x0025DCEC File Offset: 0x0025BEEC
		public DetailInfoMonitor()
		{
			this.OrganizationInfo = default(OrganizationInfo);
			this.PreexistenceCharIds = default(PreexistenceCharIds);
			this.FameActionRecords = null;
		}

		// Token: 0x0600517E RID: 20862 RVA: 0x0025DD2C File Offset: 0x0025BF2C
		protected override void MonitorData()
		{
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				foreach (uint fieldId in this.RelativeFieldIds)
				{
					GameDataBridge.AddDataMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
				}
				base.Character.CallMethod(101);
				base.Character.CallMethod(116);
			}
		}

		// Token: 0x0600517F RID: 20863 RVA: 0x0025DDCC File Offset: 0x0025BFCC
		public override void OnDataInit()
		{
			Action onOrganizationInfoChangeEvent = this.OnOrganizationInfoChangeEvent;
			if (onOrganizationInfoChangeEvent != null)
			{
				onOrganizationInfoChangeEvent();
			}
			Action onFameTypeChangeEvent = this.OnFameTypeChangeEvent;
			if (onFameTypeChangeEvent != null)
			{
				onFameTypeChangeEvent();
			}
			Action onAttractionChangeEvent = this.OnAttractionChangeEvent;
			if (onAttractionChangeEvent != null)
			{
				onAttractionChangeEvent();
			}
			Action onHappinessChangeEvent = this.OnHappinessChangeEvent;
			if (onHappinessChangeEvent != null)
			{
				onHappinessChangeEvent();
			}
			Action onBehaviorChangeEvent = this.OnBehaviorChangeEvent;
			if (onBehaviorChangeEvent != null)
			{
				onBehaviorChangeEvent();
			}
			Action onSamsaraChangeEvent = this.OnSamsaraChangeEvent;
			if (onSamsaraChangeEvent != null)
			{
				onSamsaraChangeEvent();
			}
		}

		// Token: 0x06005180 RID: 20864 RVA: 0x0025DE48 File Offset: 0x0025C048
		public override void InitFromDeadCharacter(DeadCharacter deadCharacter)
		{
			this.OrganizationInfo = deadCharacter.OrganizationInfo;
			this.FameType = deadCharacter.FameType;
			this.Attraction = deadCharacter.Attraction;
			this.Happiness = deadCharacter.Happiness;
			this.Behavior = deadCharacter.Morality;
			this.PreexistenceCharIds = deadCharacter.PreexistenceCharIds;
			this.DataFlag = 2;
		}

		// Token: 0x06005181 RID: 20865 RVA: 0x0025DEAC File Offset: 0x0025C0AC
		protected override bool IsValidMonitor()
		{
			Action onOrganizationInfoChangeEvent = this.OnOrganizationInfoChangeEvent;
			bool flag = onOrganizationInfoChangeEvent != null && onOrganizationInfoChangeEvent.GetInvocationList().Length != 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				Action onFameTypeChangeEvent = this.OnFameTypeChangeEvent;
				bool flag2 = onFameTypeChangeEvent != null && onFameTypeChangeEvent.GetInvocationList().Length != 0;
				if (flag2)
				{
					result = true;
				}
				else
				{
					Action onAttractionChangeEvent = this.OnAttractionChangeEvent;
					bool flag3 = onAttractionChangeEvent != null && onAttractionChangeEvent.GetInvocationList().Length != 0;
					if (flag3)
					{
						result = true;
					}
					else
					{
						Action onHappinessChangeEvent = this.OnHappinessChangeEvent;
						bool flag4 = onHappinessChangeEvent != null && onHappinessChangeEvent.GetInvocationList().Length != 0;
						if (flag4)
						{
							result = true;
						}
						else
						{
							Action onBehaviorChangeEvent = this.OnBehaviorChangeEvent;
							bool flag5 = onBehaviorChangeEvent != null && onBehaviorChangeEvent.GetInvocationList().Length != 0;
							if (flag5)
							{
								result = true;
							}
							else
							{
								Action onSamsaraChangeEvent = this.OnSamsaraChangeEvent;
								bool flag6 = onSamsaraChangeEvent != null && onSamsaraChangeEvent.GetInvocationList().Length != 0;
								result = flag6;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06005182 RID: 20866 RVA: 0x0025DF80 File Offset: 0x0025C180
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
			this.DataFlag = 0;
		}

		// Token: 0x06005183 RID: 20867 RVA: 0x0025DFF8 File Offset: 0x0025C1F8
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type == 1;
			if (flag)
			{
				bool flag2 = wrapper.Notification.MethodId == 101;
				if (flag2)
				{
					sbyte fameType = -1;
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref fameType);
					bool changeFlag = fameType != this.FameType;
					this.FameType = fameType;
					bool flag3 = base.Init && changeFlag;
					if (flag3)
					{
						Action onFameTypeChangeEvent = this.OnFameTypeChangeEvent;
						if (onFameTypeChangeEvent != null)
						{
							onFameTypeChangeEvent();
						}
					}
				}
				else
				{
					bool flag4 = wrapper.Notification.MethodId == 116;
					if (flag4)
					{
						bool result = false;
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref result);
						bool changeFlag2 = this.IsReclusiveChar != result;
						this.IsReclusiveChar = result;
						bool flag5 = base.Init && changeFlag2;
						if (flag5)
						{
							Action onOrganizationInfoChangeEvent = this.OnOrganizationInfoChangeEvent;
							if (onOrganizationInfoChangeEvent != null)
							{
								onOrganizationInfoChangeEvent();
							}
						}
					}
				}
			}
			else
			{
				bool flag6 = wrapper.Notification.Uid.SubId1 == 8U;
				if (flag6)
				{
					OrganizationInfo orgInfo = default(OrganizationInfo);
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref orgInfo);
					bool changeFlag3 = this.OrganizationInfo.Equals(orgInfo);
					this.OrganizationInfo = orgInfo;
					bool flag7 = base.Init && changeFlag3;
					if (flag7)
					{
						Action onOrganizationInfoChangeEvent2 = this.OnOrganizationInfoChangeEvent;
						if (onOrganizationInfoChangeEvent2 != null)
						{
							onOrganizationInfoChangeEvent2();
						}
					}
				}
				else
				{
					bool flag8 = wrapper.Notification.Uid.SubId1 == 76U;
					if (flag8)
					{
						sbyte fame = -1;
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref fame);
						bool changeFlag4 = fame != this.Fame;
						this.Fame = fame;
						bool flag9 = changeFlag4;
						if (flag9)
						{
							base.Character.CallMethod(101);
						}
					}
					else
					{
						bool flag10 = wrapper.Notification.Uid.SubId1 == 41U;
						if (flag10)
						{
							List<FameActionRecord> fameActionRecords = null;
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref fameActionRecords);
							bool changeFlag5 = false;
							bool flag11 = this.FameActionRecords != null;
							if (flag11)
							{
								changeFlag5 = !this.FameActionRecords.Equals(fameActionRecords);
							}
							else
							{
								bool flag12 = fameActionRecords != null;
								if (flag12)
								{
									changeFlag5 = !fameActionRecords.Equals(this.FameActionRecords);
								}
							}
							this.FameActionRecords = fameActionRecords;
							bool flag13 = base.Init && changeFlag5;
							if (flag13)
							{
								Action onFameTypeChangeEvent2 = this.OnFameTypeChangeEvent;
								if (onFameTypeChangeEvent2 != null)
								{
									onFameTypeChangeEvent2();
								}
							}
						}
						else
						{
							bool flag14 = wrapper.Notification.Uid.SubId1 == 78U;
							if (flag14)
							{
								short attraction = 0;
								Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref attraction);
								bool changeFlag6 = attraction != this.Attraction;
								this.Attraction = attraction;
								bool flag15 = base.Init && changeFlag6;
								if (flag15)
								{
									Action onAttractionChangeEvent = this.OnAttractionChangeEvent;
									if (onAttractionChangeEvent != null)
									{
										onAttractionChangeEvent();
									}
								}
							}
							else
							{
								bool flag16 = wrapper.Notification.Uid.SubId1 == 6U;
								if (flag16)
								{
									sbyte happiness = -1;
									Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref happiness);
									bool changeFlag7 = happiness != this.Happiness;
									this.Happiness = happiness;
									bool flag17 = base.Init && changeFlag7;
									if (flag17)
									{
										Action onHappinessChangeEvent = this.OnHappinessChangeEvent;
										if (onHappinessChangeEvent != null)
										{
											onHappinessChangeEvent();
										}
									}
								}
								else
								{
									bool flag18 = wrapper.Notification.Uid.SubId1 == 77U;
									if (flag18)
									{
										short behavior = -1;
										Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref behavior);
										bool changeFlag8 = behavior != this.Behavior;
										this.Behavior = behavior;
										bool flag19 = base.Init && changeFlag8;
										if (flag19)
										{
											Action onBehaviorChangeEvent = this.OnBehaviorChangeEvent;
											if (onBehaviorChangeEvent != null)
											{
												onBehaviorChangeEvent();
											}
										}
									}
									else
									{
										bool flag20 = wrapper.Notification.Uid.SubId1 == 63U;
										if (flag20)
										{
											PreexistenceCharIds preexistenceCharIds = default(PreexistenceCharIds);
											Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref preexistenceCharIds);
											this.PreexistenceCharIds = preexistenceCharIds;
											bool init = base.Init;
											if (init)
											{
												Action onSamsaraChangeEvent = this.OnSamsaraChangeEvent;
												if (onSamsaraChangeEvent != null)
												{
													onSamsaraChangeEvent();
												}
											}
											bool flag21 = !base.Init;
											if (flag21)
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
			}
		}

		// Token: 0x06005184 RID: 20868 RVA: 0x0025E464 File Offset: 0x0025C664
		public unsafe int[] GetCharacterSamsaraInfo(ref int[] array)
		{
			bool flag = array == null;
			if (flag)
			{
				array = new int[9];
			}
			for (int i = 0; i < 9; i++)
			{
				array[i] = -1;
			}
			PreexistenceCharIds preexistenceCharIds = this.PreexistenceCharIds;
			for (int j = 0; j < this.PreexistenceCharIds.Count; j++)
			{
				int posIndex = PreexistenceCharIds.Positions[j];
				array[posIndex] = *(ref preexistenceCharIds.CharIds.FixedElementField + (IntPtr)j * 4);
			}
			return array;
		}

		// Token: 0x06005185 RID: 20869 RVA: 0x0025E4ED File Offset: 0x0025C6ED
		public void SetToCombatCharacterHappiness(sbyte happiness)
		{
			this.Happiness = happiness;
			Action onHappinessChangeEvent = this.OnHappinessChangeEvent;
			if (onHappinessChangeEvent != null)
			{
				onHappinessChangeEvent();
			}
		}

		// Token: 0x06005186 RID: 20870 RVA: 0x0025E50A File Offset: 0x0025C70A
		public void AddOnOrganizationInfoListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnOrganizationInfoChangeEvent -= listener;
			this.OnOrganizationInfoChangeEvent += listener;
		}

		// Token: 0x06005187 RID: 20871 RVA: 0x0025E52C File Offset: 0x0025C72C
		public void RemoveOnOrganizationInfoListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnOrganizationInfoChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005188 RID: 20872 RVA: 0x0025E54D File Offset: 0x0025C74D
		public void AddOnFameTypeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnFameTypeChangeEvent -= listener;
			this.OnFameTypeChangeEvent += listener;
		}

		// Token: 0x06005189 RID: 20873 RVA: 0x0025E56F File Offset: 0x0025C76F
		public void RemoveFameTypeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnFameTypeChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600518A RID: 20874 RVA: 0x0025E590 File Offset: 0x0025C790
		public void AddAttractionListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAttractionChangeEvent -= listener;
			this.OnAttractionChangeEvent += listener;
		}

		// Token: 0x0600518B RID: 20875 RVA: 0x0025E5B2 File Offset: 0x0025C7B2
		public void RemoveAttractionListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAttractionChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600518C RID: 20876 RVA: 0x0025E5D3 File Offset: 0x0025C7D3
		public void AddOnHappinessListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnHappinessChangeEvent -= listener;
			this.OnHappinessChangeEvent += listener;
		}

		// Token: 0x0600518D RID: 20877 RVA: 0x0025E5F5 File Offset: 0x0025C7F5
		public void RemoveOnHappinessListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnHappinessChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600518E RID: 20878 RVA: 0x0025E616 File Offset: 0x0025C816
		public void AddOnBehaviorListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnBehaviorChangeEvent -= listener;
			this.OnBehaviorChangeEvent += listener;
		}

		// Token: 0x0600518F RID: 20879 RVA: 0x0025E638 File Offset: 0x0025C838
		public void RemoveOnBehaviorListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnBehaviorChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005190 RID: 20880 RVA: 0x0025E659 File Offset: 0x0025C859
		public void AddOnSamsaraListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnSamsaraChangeEvent -= listener;
			this.OnSamsaraChangeEvent += listener;
		}

		// Token: 0x06005191 RID: 20881 RVA: 0x0025E67B File Offset: 0x0025C87B
		public void RemoveOnSamsaraListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnSamsaraChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x040037B8 RID: 14264
		private sbyte _happiness;
	}
}
