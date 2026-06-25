using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006C0 RID: 1728
	public class EquipCombatSkillMonitor : MonitorDataItemBase
	{
		// Token: 0x14000051 RID: 81
		// (add) Token: 0x060051C0 RID: 20928 RVA: 0x0025F17C File Offset: 0x0025D37C
		// (remove) Token: 0x060051C1 RID: 20929 RVA: 0x0025F1B4 File Offset: 0x0025D3B4
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnConsummateLevelChangeEvent;

		// Token: 0x14000052 RID: 82
		// (add) Token: 0x060051C2 RID: 20930 RVA: 0x0025F1EC File Offset: 0x0025D3EC
		// (remove) Token: 0x060051C3 RID: 20931 RVA: 0x0025F224 File Offset: 0x0025D424
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnMaxNeiliChangeEvent;

		// Token: 0x14000053 RID: 83
		// (add) Token: 0x060051C4 RID: 20932 RVA: 0x0025F25C File Offset: 0x0025D45C
		// (remove) Token: 0x060051C5 RID: 20933 RVA: 0x0025F294 File Offset: 0x0025D494
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnCurrNeiliChangeEvent;

		// Token: 0x14000054 RID: 84
		// (add) Token: 0x060051C6 RID: 20934 RVA: 0x0025F2CC File Offset: 0x0025D4CC
		// (remove) Token: 0x060051C7 RID: 20935 RVA: 0x0025F304 File Offset: 0x0025D504
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnNeiliTypeChangeEvent;

		// Token: 0x14000055 RID: 85
		// (add) Token: 0x060051C8 RID: 20936 RVA: 0x0025F33C File Offset: 0x0025D53C
		// (remove) Token: 0x060051C9 RID: 20937 RVA: 0x0025F374 File Offset: 0x0025D574
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnNeiliProportionOfFiveElementsChangeEvent;

		// Token: 0x14000056 RID: 86
		// (add) Token: 0x060051CA RID: 20938 RVA: 0x0025F3AC File Offset: 0x0025D5AC
		// (remove) Token: 0x060051CB RID: 20939 RVA: 0x0025F3E4 File Offset: 0x0025D5E4
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnNeiliAllocationChangeEvent;

		// Token: 0x14000057 RID: 87
		// (add) Token: 0x060051CC RID: 20940 RVA: 0x0025F41C File Offset: 0x0025D61C
		// (remove) Token: 0x060051CD RID: 20941 RVA: 0x0025F454 File Offset: 0x0025D654
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnBaseNeiliAllocationChangeEvent;

		// Token: 0x14000058 RID: 88
		// (add) Token: 0x060051CE RID: 20942 RVA: 0x0025F48C File Offset: 0x0025D68C
		// (remove) Token: 0x060051CF RID: 20943 RVA: 0x0025F4C4 File Offset: 0x0025D6C4
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnAllocatedNeiliEffectsChangeEvent;

		// Token: 0x14000059 RID: 89
		// (add) Token: 0x060051D0 RID: 20944 RVA: 0x0025F4FC File Offset: 0x0025D6FC
		// (remove) Token: 0x060051D1 RID: 20945 RVA: 0x0025F534 File Offset: 0x0025D734
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnLearnedCombatSkillsChangeEvent;

		// Token: 0x1400005A RID: 90
		// (add) Token: 0x060051D2 RID: 20946 RVA: 0x0025F56C File Offset: 0x0025D76C
		// (remove) Token: 0x060051D3 RID: 20947 RVA: 0x0025F5A4 File Offset: 0x0025D7A4
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnEquippedCombatSkillsChangeEvent;

		// Token: 0x1400005B RID: 91
		// (add) Token: 0x060051D4 RID: 20948 RVA: 0x0025F5DC File Offset: 0x0025D7DC
		// (remove) Token: 0x060051D5 RID: 20949 RVA: 0x0025F614 File Offset: 0x0025D814
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnBirthMonthChangeEvent;

		// Token: 0x17000A11 RID: 2577
		// (get) Token: 0x060051D6 RID: 20950 RVA: 0x0025F649 File Offset: 0x0025D849
		// (set) Token: 0x060051D7 RID: 20951 RVA: 0x0025F651 File Offset: 0x0025D851
		public sbyte ConsummateLevel { get; private set; }

		// Token: 0x17000A12 RID: 2578
		// (get) Token: 0x060051D8 RID: 20952 RVA: 0x0025F65A File Offset: 0x0025D85A
		// (set) Token: 0x060051D9 RID: 20953 RVA: 0x0025F662 File Offset: 0x0025D862
		public int MaxNeili { get; private set; }

		// Token: 0x17000A13 RID: 2579
		// (get) Token: 0x060051DA RID: 20954 RVA: 0x0025F66B File Offset: 0x0025D86B
		// (set) Token: 0x060051DB RID: 20955 RVA: 0x0025F673 File Offset: 0x0025D873
		public int CurrNeili { get; private set; }

		// Token: 0x17000A14 RID: 2580
		// (get) Token: 0x060051DC RID: 20956 RVA: 0x0025F67C File Offset: 0x0025D87C
		// (set) Token: 0x060051DD RID: 20957 RVA: 0x0025F684 File Offset: 0x0025D884
		public sbyte NeiliType { get; private set; }

		// Token: 0x17000A15 RID: 2581
		// (get) Token: 0x060051DE RID: 20958 RVA: 0x0025F68D File Offset: 0x0025D88D
		// (set) Token: 0x060051DF RID: 20959 RVA: 0x0025F695 File Offset: 0x0025D895
		public NeiliProportionOfFiveElements NeiliProportionOfFiveElements { get; private set; }

		// Token: 0x17000A16 RID: 2582
		// (get) Token: 0x060051E0 RID: 20960 RVA: 0x0025F69E File Offset: 0x0025D89E
		// (set) Token: 0x060051E1 RID: 20961 RVA: 0x0025F6A6 File Offset: 0x0025D8A6
		public NeiliAllocation NeiliAllocation { get; private set; }

		// Token: 0x17000A17 RID: 2583
		// (get) Token: 0x060051E2 RID: 20962 RVA: 0x0025F6AF File Offset: 0x0025D8AF
		// (set) Token: 0x060051E3 RID: 20963 RVA: 0x0025F6B7 File Offset: 0x0025D8B7
		public NeiliAllocation BaseNeiliAllocation { get; private set; }

		// Token: 0x17000A18 RID: 2584
		// (get) Token: 0x060051E4 RID: 20964 RVA: 0x0025F6C0 File Offset: 0x0025D8C0
		// (set) Token: 0x060051E5 RID: 20965 RVA: 0x0025F6C8 File Offset: 0x0025D8C8
		public NeiliAllocation AllocatedNeiliEffects { get; private set; }

		// Token: 0x17000A19 RID: 2585
		// (get) Token: 0x060051E6 RID: 20966 RVA: 0x0025F6D1 File Offset: 0x0025D8D1
		// (set) Token: 0x060051E7 RID: 20967 RVA: 0x0025F6D9 File Offset: 0x0025D8D9
		public sbyte BirthMonth { get; private set; }

		// Token: 0x060051E8 RID: 20968 RVA: 0x0025F6E4 File Offset: 0x0025D8E4
		private static short[] GenerateDefaultEquippedCombatSkills()
		{
			short[] array = new short[48];
			Array.Fill<short>(array, -1);
			return array;
		}

		// Token: 0x17000A1A RID: 2586
		// (get) Token: 0x060051E9 RID: 20969 RVA: 0x0025F707 File Offset: 0x0025D907
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A1B RID: 2587
		// (get) Token: 0x060051EA RID: 20970 RVA: 0x0025F70C File Offset: 0x0025D90C
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					28U,
					108U,
					45U,
					111U,
					109U,
					47U,
					114U,
					110U,
					59U,
					116U,
					5U
				};
			}
		}

		// Token: 0x060051EB RID: 20971 RVA: 0x0025F780 File Offset: 0x0025D980
		public EquipCombatSkillMonitor()
		{
			this.ConsummateLevel = 0;
			this.MaxNeili = 0;
			this.CurrNeili = 0;
		}

		// Token: 0x060051EC RID: 20972 RVA: 0x0025F7B8 File Offset: 0x0025D9B8
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

		// Token: 0x060051ED RID: 20973 RVA: 0x0025F83C File Offset: 0x0025DA3C
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x060051EE RID: 20974 RVA: 0x0025F8AC File Offset: 0x0025DAAC
		public override void OnDataInit()
		{
			Action onConsummateLevelChangeEvent = this.OnConsummateLevelChangeEvent;
			if (onConsummateLevelChangeEvent != null)
			{
				onConsummateLevelChangeEvent();
			}
			Action onMaxNeiliChangeEvent = this.OnMaxNeiliChangeEvent;
			if (onMaxNeiliChangeEvent != null)
			{
				onMaxNeiliChangeEvent();
			}
			Action onCurrNeiliChangeEvent = this.OnCurrNeiliChangeEvent;
			if (onCurrNeiliChangeEvent != null)
			{
				onCurrNeiliChangeEvent();
			}
			Action onNeiliTypeChangeEvent = this.OnNeiliTypeChangeEvent;
			if (onNeiliTypeChangeEvent != null)
			{
				onNeiliTypeChangeEvent();
			}
			Action onNeiliProportionOfFiveElementsChangeEvent = this.OnNeiliProportionOfFiveElementsChangeEvent;
			if (onNeiliProportionOfFiveElementsChangeEvent != null)
			{
				onNeiliProportionOfFiveElementsChangeEvent();
			}
			Action onNeiliAllocationChangeEvent = this.OnNeiliAllocationChangeEvent;
			if (onNeiliAllocationChangeEvent != null)
			{
				onNeiliAllocationChangeEvent();
			}
			Action onBaseNeiliAllocationChangeEvent = this.OnBaseNeiliAllocationChangeEvent;
			if (onBaseNeiliAllocationChangeEvent != null)
			{
				onBaseNeiliAllocationChangeEvent();
			}
			Action onAllocatedNeiliEffectsChangeEvent = this.OnAllocatedNeiliEffectsChangeEvent;
			if (onAllocatedNeiliEffectsChangeEvent != null)
			{
				onAllocatedNeiliEffectsChangeEvent();
			}
			Action onLearnedCombatSkillsChangeEvent = this.OnLearnedCombatSkillsChangeEvent;
			if (onLearnedCombatSkillsChangeEvent != null)
			{
				onLearnedCombatSkillsChangeEvent();
			}
			Action onEquippedCombatSkillsChangeEvent = this.OnEquippedCombatSkillsChangeEvent;
			if (onEquippedCombatSkillsChangeEvent != null)
			{
				onEquippedCombatSkillsChangeEvent();
			}
			Action onBirthMonthChangeEvent = this.OnBirthMonthChangeEvent;
			if (onBirthMonthChangeEvent != null)
			{
				onBirthMonthChangeEvent();
			}
		}

		// Token: 0x060051EF RID: 20975 RVA: 0x0025F980 File Offset: 0x0025DB80
		protected override bool IsValidMonitor()
		{
			Action onConsummateLevelChangeEvent = this.OnConsummateLevelChangeEvent;
			bool flag = onConsummateLevelChangeEvent != null && onConsummateLevelChangeEvent.GetInvocationList().Length != 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				Action onMaxNeiliChangeEvent = this.OnMaxNeiliChangeEvent;
				bool flag2 = onMaxNeiliChangeEvent != null && onMaxNeiliChangeEvent.GetInvocationList().Length != 0;
				if (flag2)
				{
					result = true;
				}
				else
				{
					Action onCurrNeiliChangeEvent = this.OnCurrNeiliChangeEvent;
					bool flag3 = onCurrNeiliChangeEvent != null && onCurrNeiliChangeEvent.GetInvocationList().Length != 0;
					if (flag3)
					{
						result = true;
					}
					else
					{
						Action onNeiliTypeChangeEvent = this.OnNeiliTypeChangeEvent;
						bool flag4 = onNeiliTypeChangeEvent != null && onNeiliTypeChangeEvent.GetInvocationList().Length != 0;
						if (flag4)
						{
							result = true;
						}
						else
						{
							Action onNeiliProportionOfFiveElementsChangeEvent = this.OnNeiliProportionOfFiveElementsChangeEvent;
							bool flag5 = onNeiliProportionOfFiveElementsChangeEvent != null && onNeiliProportionOfFiveElementsChangeEvent.GetInvocationList().Length != 0;
							if (flag5)
							{
								result = true;
							}
							else
							{
								Action onNeiliAllocationChangeEvent = this.OnNeiliAllocationChangeEvent;
								bool flag6 = onNeiliAllocationChangeEvent != null && onNeiliAllocationChangeEvent.GetInvocationList().Length != 0;
								if (flag6)
								{
									result = true;
								}
								else
								{
									Action onBaseNeiliAllocationChangeEvent = this.OnBaseNeiliAllocationChangeEvent;
									bool flag7 = onBaseNeiliAllocationChangeEvent != null && onBaseNeiliAllocationChangeEvent.GetInvocationList().Length != 0;
									if (flag7)
									{
										result = true;
									}
									else
									{
										Action onAllocatedNeiliEffectsChangeEvent = this.OnAllocatedNeiliEffectsChangeEvent;
										bool flag8 = onAllocatedNeiliEffectsChangeEvent != null && onAllocatedNeiliEffectsChangeEvent.GetInvocationList().Length != 0;
										if (flag8)
										{
											result = true;
										}
										else
										{
											Action onLearnedCombatSkillsChangeEvent = this.OnLearnedCombatSkillsChangeEvent;
											bool flag9 = onLearnedCombatSkillsChangeEvent != null && onLearnedCombatSkillsChangeEvent.GetInvocationList().Length != 0;
											if (flag9)
											{
												result = true;
											}
											else
											{
												Action onEquippedCombatSkillsChangeEvent = this.OnEquippedCombatSkillsChangeEvent;
												bool flag10 = onEquippedCombatSkillsChangeEvent != null && onEquippedCombatSkillsChangeEvent.GetInvocationList().Length != 0;
												if (flag10)
												{
													result = true;
												}
												else
												{
													Action onBirthMonthChangeEvent = this.OnBirthMonthChangeEvent;
													bool flag11 = onBirthMonthChangeEvent != null && onBirthMonthChangeEvent.GetInvocationList().Length != 0;
													result = flag11;
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
			return result;
		}

		// Token: 0x060051F0 RID: 20976 RVA: 0x0025FB04 File Offset: 0x0025DD04
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type > 0;
			if (!flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 28U;
				if (flag2)
				{
					sbyte value = -1;
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value);
					bool changeFlag = value != this.ConsummateLevel;
					this.ConsummateLevel = value;
					bool flag3 = base.Init && changeFlag;
					if (flag3)
					{
						Action onConsummateLevelChangeEvent = this.OnConsummateLevelChangeEvent;
						if (onConsummateLevelChangeEvent != null)
						{
							onConsummateLevelChangeEvent();
						}
					}
				}
				else
				{
					bool flag4 = wrapper.Notification.Uid.SubId1 == 108U;
					if (flag4)
					{
						int value2 = -1;
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value2);
						bool changeFlag2 = value2 != this.MaxNeili;
						this.MaxNeili = value2;
						bool flag5 = base.Init && changeFlag2;
						if (flag5)
						{
							Action onMaxNeiliChangeEvent = this.OnMaxNeiliChangeEvent;
							if (onMaxNeiliChangeEvent != null)
							{
								onMaxNeiliChangeEvent();
							}
						}
					}
					else
					{
						bool flag6 = wrapper.Notification.Uid.SubId1 == 45U;
						if (flag6)
						{
							int value3 = -1;
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value3);
							bool changeFlag3 = value3 != this.CurrNeili;
							this.CurrNeili = value3;
							bool flag7 = base.Init && changeFlag3;
							if (flag7)
							{
								Action onCurrNeiliChangeEvent = this.OnCurrNeiliChangeEvent;
								if (onCurrNeiliChangeEvent != null)
								{
									onCurrNeiliChangeEvent();
								}
							}
						}
						else
						{
							bool flag8 = wrapper.Notification.Uid.SubId1 == 111U;
							if (flag8)
							{
								sbyte value4 = -1;
								Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value4);
								bool changeFlag4 = value4 != this.NeiliType;
								this.NeiliType = value4;
								bool flag9 = base.Init && changeFlag4;
								if (flag9)
								{
									Action onNeiliTypeChangeEvent = this.OnNeiliTypeChangeEvent;
									if (onNeiliTypeChangeEvent != null)
									{
										onNeiliTypeChangeEvent();
									}
								}
							}
							else
							{
								bool flag10 = wrapper.Notification.Uid.SubId1 == 110U;
								if (flag10)
								{
									NeiliProportionOfFiveElements value5 = default(NeiliProportionOfFiveElements);
									Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value5);
									this.NeiliProportionOfFiveElements = value5;
									bool init = base.Init;
									if (init)
									{
										Action onNeiliProportionOfFiveElementsChangeEvent = this.OnNeiliProportionOfFiveElementsChangeEvent;
										if (onNeiliProportionOfFiveElementsChangeEvent != null)
										{
											onNeiliProportionOfFiveElementsChangeEvent();
										}
									}
								}
								else
								{
									bool flag11 = wrapper.Notification.Uid.SubId1 == 109U;
									if (flag11)
									{
										NeiliAllocation value6 = default(NeiliAllocation);
										Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value6);
										this.NeiliAllocation = value6;
										bool init2 = base.Init;
										if (init2)
										{
											Action onNeiliAllocationChangeEvent = this.OnNeiliAllocationChangeEvent;
											if (onNeiliAllocationChangeEvent != null)
											{
												onNeiliAllocationChangeEvent();
											}
										}
									}
									else
									{
										bool flag12 = wrapper.Notification.Uid.SubId1 == 47U;
										if (flag12)
										{
											NeiliAllocation value7 = default(NeiliAllocation);
											Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value7);
											this.BaseNeiliAllocation = value7;
											bool init3 = base.Init;
											if (init3)
											{
												Action onBaseNeiliAllocationChangeEvent = this.OnBaseNeiliAllocationChangeEvent;
												if (onBaseNeiliAllocationChangeEvent != null)
												{
													onBaseNeiliAllocationChangeEvent();
												}
											}
										}
										else
										{
											bool flag13 = wrapper.Notification.Uid.SubId1 == 114U;
											if (flag13)
											{
												NeiliAllocation value8 = default(NeiliAllocation);
												Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value8);
												this.AllocatedNeiliEffects = value8;
												bool init4 = base.Init;
												if (init4)
												{
													Action onAllocatedNeiliEffectsChangeEvent = this.OnAllocatedNeiliEffectsChangeEvent;
													if (onAllocatedNeiliEffectsChangeEvent != null)
													{
														onAllocatedNeiliEffectsChangeEvent();
													}
												}
											}
											else
											{
												bool flag14 = wrapper.Notification.Uid.SubId1 == 59U;
												if (flag14)
												{
													List<short> value9 = new List<short>();
													Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value9);
													this.LearnedCombatSkills.Clear();
													this.LearnedCombatSkills.AddRange(value9);
													bool init5 = base.Init;
													if (init5)
													{
														Action onLearnedCombatSkillsChangeEvent = this.OnLearnedCombatSkillsChangeEvent;
														if (onLearnedCombatSkillsChangeEvent != null)
														{
															onLearnedCombatSkillsChangeEvent();
														}
													}
												}
												else
												{
													bool flag15 = wrapper.Notification.Uid.SubId1 == 116U;
													if (flag15)
													{
														Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref this.EquippedCombatSkills);
														bool init6 = base.Init;
														if (init6)
														{
															Action onEquippedCombatSkillsChangeEvent = this.OnEquippedCombatSkillsChangeEvent;
															if (onEquippedCombatSkillsChangeEvent != null)
															{
																onEquippedCombatSkillsChangeEvent();
															}
														}
													}
													else
													{
														bool flag16 = wrapper.Notification.Uid.SubId1 == 5U;
														if (flag16)
														{
															sbyte value10 = -1;
															Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value10);
															bool changeFlag5 = value10 != this.BirthMonth;
															this.BirthMonth = value10;
															bool flag17 = base.Init && changeFlag5;
															if (flag17)
															{
																Action onBirthMonthChangeEvent = this.OnBirthMonthChangeEvent;
																if (onBirthMonthChangeEvent != null)
																{
																	onBirthMonthChangeEvent();
																}
															}
															bool flag18 = !base.Init;
															if (flag18)
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
					}
				}
			}
		}

		// Token: 0x060051F1 RID: 20977 RVA: 0x0025FFF9 File Offset: 0x0025E1F9
		public void AddConsummateLevelListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnConsummateLevelChangeEvent -= listener;
			this.OnConsummateLevelChangeEvent += listener;
		}

		// Token: 0x060051F2 RID: 20978 RVA: 0x0026001B File Offset: 0x0025E21B
		public void RemoveConsummateLevelListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnConsummateLevelChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060051F3 RID: 20979 RVA: 0x0026003C File Offset: 0x0025E23C
		public void AddMaxNeiliListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnMaxNeiliChangeEvent -= listener;
			this.OnMaxNeiliChangeEvent += listener;
		}

		// Token: 0x060051F4 RID: 20980 RVA: 0x0026005E File Offset: 0x0025E25E
		public void RemoveMaxNeiliListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnMaxNeiliChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060051F5 RID: 20981 RVA: 0x0026007F File Offset: 0x0025E27F
		public void AddCurrNeiliListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCurrNeiliChangeEvent -= listener;
			this.OnCurrNeiliChangeEvent += listener;
		}

		// Token: 0x060051F6 RID: 20982 RVA: 0x002600A1 File Offset: 0x0025E2A1
		public void RemoveCurrNeiliListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCurrNeiliChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060051F7 RID: 20983 RVA: 0x002600C2 File Offset: 0x0025E2C2
		public void AddNeiliTypeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnNeiliTypeChangeEvent -= listener;
			this.OnNeiliTypeChangeEvent += listener;
		}

		// Token: 0x060051F8 RID: 20984 RVA: 0x002600E4 File Offset: 0x0025E2E4
		public void RemoveNeiliTypeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnNeiliTypeChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060051F9 RID: 20985 RVA: 0x00260105 File Offset: 0x0025E305
		public void AddNeiliProportionOfFiveElementsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnNeiliProportionOfFiveElementsChangeEvent -= listener;
			this.OnNeiliProportionOfFiveElementsChangeEvent += listener;
		}

		// Token: 0x060051FA RID: 20986 RVA: 0x00260127 File Offset: 0x0025E327
		public void RemoveNeiliProportionOfFiveElementsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnNeiliProportionOfFiveElementsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060051FB RID: 20987 RVA: 0x00260148 File Offset: 0x0025E348
		public void AddNeiliAllocationListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnNeiliAllocationChangeEvent -= listener;
			this.OnNeiliAllocationChangeEvent += listener;
		}

		// Token: 0x060051FC RID: 20988 RVA: 0x0026016A File Offset: 0x0025E36A
		public void RemoveNeiliAllocationListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnNeiliAllocationChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060051FD RID: 20989 RVA: 0x0026018B File Offset: 0x0025E38B
		public void AddBaseNeiliAllocationListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnBaseNeiliAllocationChangeEvent -= listener;
			this.OnBaseNeiliAllocationChangeEvent += listener;
		}

		// Token: 0x060051FE RID: 20990 RVA: 0x002601AD File Offset: 0x0025E3AD
		public void RemoveBaseNeiliAllocationListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnBaseNeiliAllocationChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060051FF RID: 20991 RVA: 0x002601CE File Offset: 0x0025E3CE
		public void AddAllocatedNeiliEffectsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAllocatedNeiliEffectsChangeEvent -= listener;
			this.OnAllocatedNeiliEffectsChangeEvent += listener;
		}

		// Token: 0x06005200 RID: 20992 RVA: 0x002601F0 File Offset: 0x0025E3F0
		public void RemoveAllocatedNeiliEffectsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAllocatedNeiliEffectsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005201 RID: 20993 RVA: 0x00260211 File Offset: 0x0025E411
		public void AddLearnedCombatSkillsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnLearnedCombatSkillsChangeEvent -= listener;
			this.OnLearnedCombatSkillsChangeEvent += listener;
		}

		// Token: 0x06005202 RID: 20994 RVA: 0x00260233 File Offset: 0x0025E433
		public void RemoveLearnedCombatSkillsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnLearnedCombatSkillsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005203 RID: 20995 RVA: 0x00260254 File Offset: 0x0025E454
		public void AddEquippedCombatSkillsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnEquippedCombatSkillsChangeEvent -= listener;
			this.OnEquippedCombatSkillsChangeEvent += listener;
		}

		// Token: 0x06005204 RID: 20996 RVA: 0x00260276 File Offset: 0x0025E476
		public void RemoveEquippedCombatSkillsListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnEquippedCombatSkillsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005205 RID: 20997 RVA: 0x00260297 File Offset: 0x0025E497
		public void AddBirthMonthListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnBirthMonthChangeEvent -= listener;
			this.OnBirthMonthChangeEvent += listener;
		}

		// Token: 0x06005206 RID: 20998 RVA: 0x002602B9 File Offset: 0x0025E4B9
		public void RemoveBirthMonthListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnBirthMonthChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x040037DA RID: 14298
		public readonly List<short> LearnedCombatSkills = new List<short>();

		// Token: 0x040037DB RID: 14299
		public CombatSkillEquipment EquippedCombatSkills = new CombatSkillEquipment();
	}
}
