using System;
using System.Collections.Generic;
using System.Diagnostics;
using Config;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006B5 RID: 1717
	public class AgeHealthMonitor : MonitorDataItemBase
	{
		// Token: 0x1400001F RID: 31
		// (add) Token: 0x0600502B RID: 20523 RVA: 0x00258210 File Offset: 0x00256410
		// (remove) Token: 0x0600502C RID: 20524 RVA: 0x00258248 File Offset: 0x00256448
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnTemplateIdChangeEvent;

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x0600502D RID: 20525 RVA: 0x00258280 File Offset: 0x00256480
		// (remove) Token: 0x0600502E RID: 20526 RVA: 0x002582B8 File Offset: 0x002564B8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnDisplayAgeChangeEvent;

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x0600502F RID: 20527 RVA: 0x002582F0 File Offset: 0x002564F0
		// (remove) Token: 0x06005030 RID: 20528 RVA: 0x00258328 File Offset: 0x00256528
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnHealthChangeEvent;

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x06005031 RID: 20529 RVA: 0x00258360 File Offset: 0x00256560
		// (remove) Token: 0x06005032 RID: 20530 RVA: 0x00258398 File Offset: 0x00256598
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int> OnCharHealthChangeEvent;

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x06005033 RID: 20531 RVA: 0x002583D0 File Offset: 0x002565D0
		// (remove) Token: 0x06005034 RID: 20532 RVA: 0x00258408 File Offset: 0x00256608
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnActualAgeChangeEvent;

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x06005035 RID: 20533 RVA: 0x00258440 File Offset: 0x00256640
		// (remove) Token: 0x06005036 RID: 20534 RVA: 0x00258478 File Offset: 0x00256678
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int> OnCharActualAgeChangeEvent;

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x06005037 RID: 20535 RVA: 0x002584B0 File Offset: 0x002566B0
		// (remove) Token: 0x06005038 RID: 20536 RVA: 0x002584E8 File Offset: 0x002566E8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnCreatingTypeChangeEvent;

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x06005039 RID: 20537 RVA: 0x00258520 File Offset: 0x00256720
		// (remove) Token: 0x0600503A RID: 20538 RVA: 0x00258558 File Offset: 0x00256758
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnPhysiologicalAgeChangeEvent;

		// Token: 0x14000027 RID: 39
		// (add) Token: 0x0600503B RID: 20539 RVA: 0x00258590 File Offset: 0x00256790
		// (remove) Token: 0x0600503C RID: 20540 RVA: 0x002585C8 File Offset: 0x002567C8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnPhysiologicalAgeAffectorChangeEvent;

		// Token: 0x170009CA RID: 2506
		// (get) Token: 0x0600503D RID: 20541 RVA: 0x002585FD File Offset: 0x002567FD
		// (set) Token: 0x0600503E RID: 20542 RVA: 0x00258605 File Offset: 0x00256805
		public short TemplateId { get; private set; }

		// Token: 0x170009CB RID: 2507
		// (get) Token: 0x0600503F RID: 20543 RVA: 0x0025860E File Offset: 0x0025680E
		// (set) Token: 0x06005040 RID: 20544 RVA: 0x00258616 File Offset: 0x00256816
		public short DisplayAge { get; private set; }

		// Token: 0x170009CC RID: 2508
		// (get) Token: 0x06005041 RID: 20545 RVA: 0x0025861F File Offset: 0x0025681F
		// (set) Token: 0x06005042 RID: 20546 RVA: 0x00258627 File Offset: 0x00256827
		public short PhysiologicalAge { get; private set; }

		// Token: 0x170009CD RID: 2509
		// (get) Token: 0x06005043 RID: 20547 RVA: 0x00258630 File Offset: 0x00256830
		// (set) Token: 0x06005044 RID: 20548 RVA: 0x00258638 File Offset: 0x00256838
		public short ActualAge { get; private set; }

		// Token: 0x170009CE RID: 2510
		// (get) Token: 0x06005045 RID: 20549 RVA: 0x00258641 File Offset: 0x00256841
		// (set) Token: 0x06005046 RID: 20550 RVA: 0x00258649 File Offset: 0x00256849
		public sbyte BirthMonth { get; private set; }

		// Token: 0x170009CF RID: 2511
		// (get) Token: 0x06005047 RID: 20551 RVA: 0x00258652 File Offset: 0x00256852
		// (set) Token: 0x06005048 RID: 20552 RVA: 0x0025865A File Offset: 0x0025685A
		public short Health { get; private set; }

		// Token: 0x170009D0 RID: 2512
		// (get) Token: 0x06005049 RID: 20553 RVA: 0x00258663 File Offset: 0x00256863
		// (set) Token: 0x0600504A RID: 20554 RVA: 0x0025866B File Offset: 0x0025686B
		public short LeftMaxHealth { get; private set; }

		// Token: 0x170009D1 RID: 2513
		// (get) Token: 0x0600504B RID: 20555 RVA: 0x00258674 File Offset: 0x00256874
		// (set) Token: 0x0600504C RID: 20556 RVA: 0x0025867C File Offset: 0x0025687C
		public short HealthRecovery { get; private set; }

		// Token: 0x170009D2 RID: 2514
		// (get) Token: 0x0600504D RID: 20557 RVA: 0x00258685 File Offset: 0x00256885
		// (set) Token: 0x0600504E RID: 20558 RVA: 0x0025868D File Offset: 0x0025688D
		public byte CreatingType { get; private set; }

		// Token: 0x170009D3 RID: 2515
		// (get) Token: 0x0600504F RID: 20559 RVA: 0x00258696 File Offset: 0x00256896
		// (set) Token: 0x06005050 RID: 20560 RVA: 0x0025869E File Offset: 0x0025689E
		public AgeAffector AgeAffector { get; private set; }

		// Token: 0x170009D4 RID: 2516
		// (get) Token: 0x06005051 RID: 20561 RVA: 0x002586A7 File Offset: 0x002568A7
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170009D5 RID: 2517
		// (get) Token: 0x06005052 RID: 20562 RVA: 0x002586AA File Offset: 0x002568AA
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					1U,
					2U,
					4U,
					75U,
					19U,
					5U
				};
			}
		}

		// Token: 0x06005053 RID: 20563 RVA: 0x002586E4 File Offset: 0x002568E4
		protected override void MonitorData()
		{
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				base.Character.CallMethod(97);
				base.Character.CallMethod(41);
				base.Character.CallMethod(178);
				base.Character.CallMethod(96);
				base.Character.CallMethod(179);
				foreach (uint fieldId in this.RelativeFieldIds)
				{
					GameDataBridge.AddDataMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
				}
			}
		}

		// Token: 0x06005054 RID: 20564 RVA: 0x002587B4 File Offset: 0x002569B4
		public override void Refresh()
		{
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				this.DataFlag = 1;
				base.Character.CallMethod(97);
				base.Character.CallMethod(41);
				base.Character.CallMethod(178);
				base.Character.CallMethod(96);
				base.Character.CallMethod(179);
			}
		}

		// Token: 0x06005055 RID: 20565 RVA: 0x00258828 File Offset: 0x00256A28
		public override void OnDataInit()
		{
			Action onTemplateIdChangeEvent = this.OnTemplateIdChangeEvent;
			if (onTemplateIdChangeEvent != null)
			{
				onTemplateIdChangeEvent();
			}
			Action onActualAgeChangeEvent = this.OnActualAgeChangeEvent;
			if (onActualAgeChangeEvent != null)
			{
				onActualAgeChangeEvent();
			}
			Action<int> onCharActualAgeChangeEvent = this.OnCharActualAgeChangeEvent;
			if (onCharActualAgeChangeEvent != null)
			{
				onCharActualAgeChangeEvent(base.CharacterId);
			}
			Action onDisplayAgeChangeEvent = this.OnDisplayAgeChangeEvent;
			if (onDisplayAgeChangeEvent != null)
			{
				onDisplayAgeChangeEvent();
			}
			Action onPhysiologicalAgeChangeEvent = this.OnPhysiologicalAgeChangeEvent;
			if (onPhysiologicalAgeChangeEvent != null)
			{
				onPhysiologicalAgeChangeEvent();
			}
			Action onPhysiologicalAgeAffectorChangeEvent = this.OnPhysiologicalAgeAffectorChangeEvent;
			if (onPhysiologicalAgeAffectorChangeEvent != null)
			{
				onPhysiologicalAgeAffectorChangeEvent();
			}
			Action onHealthChangeEvent = this.OnHealthChangeEvent;
			if (onHealthChangeEvent != null)
			{
				onHealthChangeEvent();
			}
			Action<int> onCharHealthChangeEvent = this.OnCharHealthChangeEvent;
			if (onCharHealthChangeEvent != null)
			{
				onCharHealthChangeEvent(base.CharacterId);
			}
			Action onCreatingTypeChangeEvent = this.OnCreatingTypeChangeEvent;
			if (onCreatingTypeChangeEvent != null)
			{
				onCreatingTypeChangeEvent();
			}
		}

		// Token: 0x06005056 RID: 20566 RVA: 0x002588E4 File Offset: 0x00256AE4
		protected override bool IsValidMonitor()
		{
			Action onTemplateIdChangeEvent = this.OnTemplateIdChangeEvent;
			bool flag = onTemplateIdChangeEvent != null && onTemplateIdChangeEvent.GetInvocationList().Length != 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				Action onActualAgeChangeEvent = this.OnActualAgeChangeEvent;
				bool flag2;
				if (onActualAgeChangeEvent == null || onActualAgeChangeEvent.GetInvocationList().Length == 0)
				{
					Action<int> onCharActualAgeChangeEvent = this.OnCharActualAgeChangeEvent;
					flag2 = (onCharActualAgeChangeEvent != null && onCharActualAgeChangeEvent.GetInvocationList().Length != 0);
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
					Action onDisplayAgeChangeEvent = this.OnDisplayAgeChangeEvent;
					bool flag4 = onDisplayAgeChangeEvent != null && onDisplayAgeChangeEvent.GetInvocationList().Length != 0;
					if (flag4)
					{
						result = true;
					}
					else
					{
						Action onPhysiologicalAgeChangeEvent = this.OnPhysiologicalAgeChangeEvent;
						bool flag5 = onPhysiologicalAgeChangeEvent != null && onPhysiologicalAgeChangeEvent.GetInvocationList().Length != 0;
						if (flag5)
						{
							result = true;
						}
						else
						{
							Action onPhysiologicalAgeAffectorChangeEvent = this.OnPhysiologicalAgeAffectorChangeEvent;
							bool flag6 = onPhysiologicalAgeAffectorChangeEvent != null && onPhysiologicalAgeAffectorChangeEvent.GetInvocationList().Length != 0;
							if (flag6)
							{
								result = true;
							}
							else
							{
								Action onHealthChangeEvent = this.OnHealthChangeEvent;
								bool flag7;
								if (onHealthChangeEvent == null || onHealthChangeEvent.GetInvocationList().Length == 0)
								{
									Action<int> onCharHealthChangeEvent = this.OnCharHealthChangeEvent;
									flag7 = (onCharHealthChangeEvent != null && onCharHealthChangeEvent.GetInvocationList().Length != 0);
								}
								else
								{
									flag7 = true;
								}
								bool flag8 = flag7;
								result = flag8;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06005057 RID: 20567 RVA: 0x002589F0 File Offset: 0x00256BF0
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x06005058 RID: 20568 RVA: 0x00258A60 File Offset: 0x00256C60
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type == 0;
			if (flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 1U;
				if (flag2)
				{
					short templateId = 0;
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref templateId);
					bool changeFlag = templateId != this.TemplateId;
					this.TemplateId = templateId;
					bool flag3 = base.Init && changeFlag;
					if (flag3)
					{
						Action onTemplateIdChangeEvent = this.OnTemplateIdChangeEvent;
						if (onTemplateIdChangeEvent != null)
						{
							onTemplateIdChangeEvent();
						}
					}
				}
				else
				{
					bool flag4 = wrapper.Notification.Uid.SubId1 == 4U;
					if (flag4)
					{
						short age = 0;
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref age);
						bool changeFlag2 = age != this.ActualAge;
						this.ActualAge = age;
						bool flag5 = base.Init && changeFlag2;
						if (flag5)
						{
							Action onActualAgeChangeEvent = this.OnActualAgeChangeEvent;
							if (onActualAgeChangeEvent != null)
							{
								onActualAgeChangeEvent();
							}
							Action<int> onCharActualAgeChangeEvent = this.OnCharActualAgeChangeEvent;
							if (onCharActualAgeChangeEvent != null)
							{
								onCharActualAgeChangeEvent(base.CharacterId);
							}
						}
					}
					else
					{
						bool flag6 = wrapper.Notification.Uid.SubId1 == 19U;
						if (flag6)
						{
							short health = 0;
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref health);
							bool changeFlag3 = health != this.Health;
							this.Health = health;
							bool flag7 = base.Init && changeFlag3;
							if (flag7)
							{
								Action onHealthChangeEvent = this.OnHealthChangeEvent;
								if (onHealthChangeEvent != null)
								{
									onHealthChangeEvent();
								}
								Action<int> onCharHealthChangeEvent = this.OnCharHealthChangeEvent;
								if (onCharHealthChangeEvent != null)
								{
									onCharHealthChangeEvent(base.CharacterId);
								}
							}
						}
						else
						{
							bool flag8 = wrapper.Notification.Uid.SubId1 == 5U;
							if (flag8)
							{
								sbyte birthMonth = -1;
								Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref birthMonth);
								bool changeFlag4 = birthMonth != this.BirthMonth;
								this.BirthMonth = birthMonth;
								bool flag9 = base.Init && changeFlag4;
								if (flag9)
								{
									Action onDisplayAgeChangeEvent = this.OnDisplayAgeChangeEvent;
									if (onDisplayAgeChangeEvent != null)
									{
										onDisplayAgeChangeEvent();
									}
								}
								bool flag10 = !base.Init;
								if (flag10)
								{
									this.DataFlag = 1;
								}
							}
							else
							{
								bool flag11 = wrapper.Notification.Uid.SubId1 == 2U;
								if (flag11)
								{
									byte creatingType = 0;
									Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref creatingType);
									bool changed = this.CreatingType != creatingType;
									this.CreatingType = creatingType;
									bool flag12 = base.Init && changed;
									if (flag12)
									{
										Action onCreatingTypeChangeEvent = this.OnCreatingTypeChangeEvent;
										if (onCreatingTypeChangeEvent != null)
										{
											onCreatingTypeChangeEvent();
										}
									}
								}
								else
								{
									bool flag13 = wrapper.Notification.Uid.SubId1 == 75U;
									if (flag13)
									{
										short physiologicalAge = 0;
										Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref physiologicalAge);
										bool changed2 = this.PhysiologicalAge != physiologicalAge;
										this.PhysiologicalAge = physiologicalAge;
										bool flag14 = base.Init && changed2;
										if (flag14)
										{
											Action onPhysiologicalAgeChangeEvent = this.OnPhysiologicalAgeChangeEvent;
											if (onPhysiologicalAgeChangeEvent != null)
											{
												onPhysiologicalAgeChangeEvent();
											}
											base.Character.CallMethod(179);
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag15 = wrapper.Notification.Type == 1;
				if (flag15)
				{
					bool flag16 = wrapper.Notification.MethodId == 41;
					if (flag16)
					{
						short age2 = -1;
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref age2);
						bool changeFlag5 = age2 != this.DisplayAge;
						this.DisplayAge = age2;
						bool flag17 = base.Init && changeFlag5;
						if (flag17)
						{
							Action onDisplayAgeChangeEvent2 = this.OnDisplayAgeChangeEvent;
							if (onDisplayAgeChangeEvent2 != null)
							{
								onDisplayAgeChangeEvent2();
							}
						}
					}
					else
					{
						bool flag18 = wrapper.Notification.MethodId == 96;
						if (flag18)
						{
							short health2 = 0;
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref health2);
							bool changeFlag6 = health2 != this.LeftMaxHealth;
							this.LeftMaxHealth = health2;
							bool flag19 = base.Init && changeFlag6;
							if (flag19)
							{
								Action onHealthChangeEvent2 = this.OnHealthChangeEvent;
								if (onHealthChangeEvent2 != null)
								{
									onHealthChangeEvent2();
								}
								Action<int> onCharHealthChangeEvent2 = this.OnCharHealthChangeEvent;
								if (onCharHealthChangeEvent2 != null)
								{
									onCharHealthChangeEvent2(base.CharacterId);
								}
							}
						}
						else
						{
							bool flag20 = wrapper.Notification.MethodId == 97;
							if (flag20)
							{
								short changeValue = 0;
								Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref changeValue);
								bool changeFlag7 = changeValue != this.HealthRecovery;
								this.HealthRecovery = changeValue;
								bool flag21 = base.Init && changeFlag7;
								if (flag21)
								{
									Action onHealthChangeEvent3 = this.OnHealthChangeEvent;
									if (onHealthChangeEvent3 != null)
									{
										onHealthChangeEvent3();
									}
									Action<int> onCharHealthChangeEvent3 = this.OnCharHealthChangeEvent;
									if (onCharHealthChangeEvent3 != null)
									{
										onCharHealthChangeEvent3(base.CharacterId);
									}
								}
							}
							else
							{
								bool flag22 = wrapper.Notification.MethodId == 178;
								if (flag22)
								{
									short age3 = -1;
									Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref age3);
									bool changeFlag8 = age3 != this.PhysiologicalAge;
									this.PhysiologicalAge = age3;
									bool flag23 = base.Init && changeFlag8;
									if (flag23)
									{
										Action onPhysiologicalAgeChangeEvent2 = this.OnPhysiologicalAgeChangeEvent;
										if (onPhysiologicalAgeChangeEvent2 != null)
										{
											onPhysiologicalAgeChangeEvent2();
										}
										base.Character.CallMethod(179);
									}
								}
								else
								{
									bool flag24 = wrapper.Notification.MethodId == 179;
									if (flag24)
									{
										byte affector = 0;
										Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref affector);
										bool changeFlag9 = affector != (byte)this.AgeAffector;
										this.AgeAffector = (AgeAffector)affector;
										bool flag25 = base.Init && changeFlag9;
										if (flag25)
										{
											Action onPhysiologicalAgeAffectorChangeEvent = this.OnPhysiologicalAgeAffectorChangeEvent;
											if (onPhysiologicalAgeAffectorChangeEvent != null)
											{
												onPhysiologicalAgeAffectorChangeEvent();
											}
											base.Character.CallMethod(179);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06005059 RID: 20569 RVA: 0x00259048 File Offset: 0x00257248
		public override void InitFromDeadCharacter(DeadCharacter deadCharacter)
		{
			this.ActualAge = deadCharacter.GetActualAge();
			this.PhysiologicalAge = (this.DisplayAge = this.ActualAge);
			this.BirthMonth = SingletonObject.getInstance<TimeManager>().GetMonthInYear(deadCharacter.BirthDate);
			this.HealthRecovery = 0;
			this.DataFlag = 2;
			this.TemplateId = deadCharacter.TemplateId;
			this.CreatingType = Config.Character.Instance[this.TemplateId].CreatingType;
		}

		// Token: 0x0600505A RID: 20570 RVA: 0x002590C9 File Offset: 0x002572C9
		public void AddTemplateIdListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnTemplateIdChangeEvent -= listener;
			this.OnTemplateIdChangeEvent += listener;
		}

		// Token: 0x0600505B RID: 20571 RVA: 0x002590EB File Offset: 0x002572EB
		public void RemoveTemplateIdListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnTemplateIdChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600505C RID: 20572 RVA: 0x0025910C File Offset: 0x0025730C
		public void AddActualAgeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnActualAgeChangeEvent -= listener;
			this.OnActualAgeChangeEvent += listener;
		}

		// Token: 0x0600505D RID: 20573 RVA: 0x0025912E File Offset: 0x0025732E
		public void RemoveActualAgeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnActualAgeChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600505E RID: 20574 RVA: 0x0025914F File Offset: 0x0025734F
		public void AddActualAgeListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharActualAgeChangeEvent -= listener;
			this.OnCharActualAgeChangeEvent += listener;
		}

		// Token: 0x0600505F RID: 20575 RVA: 0x00259171 File Offset: 0x00257371
		public void RemoveActualAgeListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharActualAgeChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005060 RID: 20576 RVA: 0x00259192 File Offset: 0x00257392
		public void AddOnAgeChangeEventListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnDisplayAgeChangeEvent -= listener;
			this.OnDisplayAgeChangeEvent += listener;
		}

		// Token: 0x06005061 RID: 20577 RVA: 0x002591B4 File Offset: 0x002573B4
		public void RemoveOnAgeChangeEventListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnDisplayAgeChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005062 RID: 20578 RVA: 0x002591D5 File Offset: 0x002573D5
		public void AddOnPhysiologicalAgeChangeEventListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnPhysiologicalAgeChangeEvent -= listener;
			this.OnPhysiologicalAgeChangeEvent += listener;
			this.OnPhysiologicalAgeAffectorChangeEvent -= listener;
			this.OnPhysiologicalAgeAffectorChangeEvent += listener;
		}

		// Token: 0x06005063 RID: 20579 RVA: 0x00259207 File Offset: 0x00257407
		public void RemoveOnPhysiologicalAgeChangeEventListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnPhysiologicalAgeChangeEvent -= listener;
			this.OnPhysiologicalAgeAffectorChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005064 RID: 20580 RVA: 0x00259230 File Offset: 0x00257430
		public void AddOnHealthChangeEventListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnHealthChangeEvent -= listener;
			this.OnHealthChangeEvent += listener;
		}

		// Token: 0x06005065 RID: 20581 RVA: 0x00259252 File Offset: 0x00257452
		public void RemoveOnHealthChangeEventListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnHealthChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005066 RID: 20582 RVA: 0x00259273 File Offset: 0x00257473
		public void AddOnHealthChangeEventListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharHealthChangeEvent -= listener;
			this.OnCharHealthChangeEvent += listener;
		}

		// Token: 0x06005067 RID: 20583 RVA: 0x00259295 File Offset: 0x00257495
		public void RemoveOnHealthChangeEventListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharHealthChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005068 RID: 20584 RVA: 0x002592B6 File Offset: 0x002574B6
		public void AddOnCreatingTypeChangeEventListener(Action listener)
		{
			this.OnCreatingTypeChangeEvent -= listener;
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCreatingTypeChangeEvent += listener;
		}

		// Token: 0x06005069 RID: 20585 RVA: 0x002592D8 File Offset: 0x002574D8
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
