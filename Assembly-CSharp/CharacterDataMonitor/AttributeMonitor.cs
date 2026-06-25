using System;
using System.Collections.Generic;
using System.Diagnostics;
using FrameWork;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006B6 RID: 1718
	public class AttributeMonitor : MonitorDataItemBase
	{
		// Token: 0x14000028 RID: 40
		// (add) Token: 0x0600506B RID: 20587 RVA: 0x00259304 File Offset: 0x00257504
		// (remove) Token: 0x0600506C RID: 20588 RVA: 0x0025933C File Offset: 0x0025753C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<sbyte> OnMainAttributeChangeEvent;

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x0600506D RID: 20589 RVA: 0x00259374 File Offset: 0x00257574
		// (remove) Token: 0x0600506E RID: 20590 RVA: 0x002593AC File Offset: 0x002575AC
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int, sbyte> OnCharMainAttributeChangeEvent;

		// Token: 0x1400002A RID: 42
		// (add) Token: 0x0600506F RID: 20591 RVA: 0x002593E4 File Offset: 0x002575E4
		// (remove) Token: 0x06005070 RID: 20592 RVA: 0x0025941C File Offset: 0x0025761C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<sbyte> OnAtkHitValuesChangeEvent;

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x06005071 RID: 20593 RVA: 0x00259454 File Offset: 0x00257654
		// (remove) Token: 0x06005072 RID: 20594 RVA: 0x0025948C File Offset: 0x0025768C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int, sbyte> OnCharAtkHitValuesChangeEvent;

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x06005073 RID: 20595 RVA: 0x002594C4 File Offset: 0x002576C4
		// (remove) Token: 0x06005074 RID: 20596 RVA: 0x002594FC File Offset: 0x002576FC
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnAtkPenetrabilityChangeEvent;

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x06005075 RID: 20597 RVA: 0x00259534 File Offset: 0x00257734
		// (remove) Token: 0x06005076 RID: 20598 RVA: 0x0025956C File Offset: 0x0025776C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int> OnCharAtkPenetrabilityChangeEvent;

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x06005077 RID: 20599 RVA: 0x002595A4 File Offset: 0x002577A4
		// (remove) Token: 0x06005078 RID: 20600 RVA: 0x002595DC File Offset: 0x002577DC
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<sbyte> OnDefHitValuesChangeEvent;

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x06005079 RID: 20601 RVA: 0x00259614 File Offset: 0x00257814
		// (remove) Token: 0x0600507A RID: 20602 RVA: 0x0025964C File Offset: 0x0025784C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int, sbyte> OnCharDefHitValuesChangeEvent;

		// Token: 0x14000030 RID: 48
		// (add) Token: 0x0600507B RID: 20603 RVA: 0x00259684 File Offset: 0x00257884
		// (remove) Token: 0x0600507C RID: 20604 RVA: 0x002596BC File Offset: 0x002578BC
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnDefPenetrabilityChangeEvent;

		// Token: 0x14000031 RID: 49
		// (add) Token: 0x0600507D RID: 20605 RVA: 0x002596F4 File Offset: 0x002578F4
		// (remove) Token: 0x0600507E RID: 20606 RVA: 0x0025972C File Offset: 0x0025792C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int> OnCharDefPenetrabilityChangeEvent;

		// Token: 0x170009D6 RID: 2518
		// (get) Token: 0x0600507F RID: 20607 RVA: 0x00259761 File Offset: 0x00257961
		public short[] CurMainAttribute { get; }

		// Token: 0x170009D7 RID: 2519
		// (get) Token: 0x06005080 RID: 20608 RVA: 0x00259769 File Offset: 0x00257969
		public short[] MaxMainAttribute { get; }

		// Token: 0x170009D8 RID: 2520
		// (get) Token: 0x06005081 RID: 20609 RVA: 0x00259771 File Offset: 0x00257971
		public int[] AtkHitValues { get; }

		// Token: 0x170009D9 RID: 2521
		// (get) Token: 0x06005082 RID: 20610 RVA: 0x00259779 File Offset: 0x00257979
		// (set) Token: 0x06005083 RID: 20611 RVA: 0x00259781 File Offset: 0x00257981
		public OuterAndInnerInts AtkPenetrability { get; private set; }

		// Token: 0x170009DA RID: 2522
		// (get) Token: 0x06005084 RID: 20612 RVA: 0x0025978A File Offset: 0x0025798A
		public int[] DefHitValues { get; }

		// Token: 0x170009DB RID: 2523
		// (get) Token: 0x06005085 RID: 20613 RVA: 0x00259792 File Offset: 0x00257992
		// (set) Token: 0x06005086 RID: 20614 RVA: 0x0025979A File Offset: 0x0025799A
		public OuterAndInnerInts DefPenetrability { get; private set; }

		// Token: 0x170009DC RID: 2524
		// (get) Token: 0x06005087 RID: 20615 RVA: 0x002597A3 File Offset: 0x002579A3
		public sealed override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					43U,
					79U,
					80U,
					81U,
					82U,
					83U
				};
			}
		}

		// Token: 0x170009DD RID: 2525
		// (get) Token: 0x06005088 RID: 20616 RVA: 0x002597E0 File Offset: 0x002579E0
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005089 RID: 20617 RVA: 0x002597E4 File Offset: 0x002579E4
		public AttributeMonitor()
		{
			this.CurMainAttribute = new short[6];
			this.MaxMainAttribute = new short[6];
			this.AtkHitValues = new int[4];
			this.AtkPenetrability = default(OuterAndInnerInts);
			this.DefHitValues = new int[4];
			this.DefPenetrability = default(OuterAndInnerInts);
			this._fieldListenManager = new MonitorPerFieldListenManager();
			this._fieldListenManager.InitMonitorFuncs(new Action<uint>(this.AddMonitorByFieldId), new Action<uint>(this.RemoveMonitorByFieldId));
			this._fieldListenManager.InitStateMap(this.RelativeFieldIds);
		}

		// Token: 0x0600508A RID: 20618 RVA: 0x0025988A File Offset: 0x00257A8A
		protected override void MonitorData()
		{
		}

		// Token: 0x0600508B RID: 20619 RVA: 0x00259890 File Offset: 0x00257A90
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
			this._fieldListenManager.ClearStateMap();
		}

		// Token: 0x0600508C RID: 20620 RVA: 0x0025990C File Offset: 0x00257B0C
		public override void OnDataInit()
		{
			Action<sbyte> onMainAttributeChangeEvent = this.OnMainAttributeChangeEvent;
			if (onMainAttributeChangeEvent != null)
			{
				onMainAttributeChangeEvent(6);
			}
			Action<int, sbyte> onCharMainAttributeChangeEvent = this.OnCharMainAttributeChangeEvent;
			if (onCharMainAttributeChangeEvent != null)
			{
				onCharMainAttributeChangeEvent(base.CharacterId, 6);
			}
			Action<sbyte> onAtkHitValuesChangeEvent = this.OnAtkHitValuesChangeEvent;
			if (onAtkHitValuesChangeEvent != null)
			{
				onAtkHitValuesChangeEvent(4);
			}
			Action<int, sbyte> onCharAtkHitValuesChangeEvent = this.OnCharAtkHitValuesChangeEvent;
			if (onCharAtkHitValuesChangeEvent != null)
			{
				onCharAtkHitValuesChangeEvent(base.CharacterId, 4);
			}
			Action onAtkPenetrabilityChangeEvent = this.OnAtkPenetrabilityChangeEvent;
			if (onAtkPenetrabilityChangeEvent != null)
			{
				onAtkPenetrabilityChangeEvent();
			}
			Action<int> onCharAtkPenetrabilityChangeEvent = this.OnCharAtkPenetrabilityChangeEvent;
			if (onCharAtkPenetrabilityChangeEvent != null)
			{
				onCharAtkPenetrabilityChangeEvent(base.CharacterId);
			}
			Action<sbyte> onDefHitValuesChangeEvent = this.OnDefHitValuesChangeEvent;
			if (onDefHitValuesChangeEvent != null)
			{
				onDefHitValuesChangeEvent(4);
			}
			Action<int, sbyte> onCharDefHitValuesChangeEvent = this.OnCharDefHitValuesChangeEvent;
			if (onCharDefHitValuesChangeEvent != null)
			{
				onCharDefHitValuesChangeEvent(base.CharacterId, 4);
			}
			Action onDefPenetrabilityChangeEvent = this.OnDefPenetrabilityChangeEvent;
			if (onDefPenetrabilityChangeEvent != null)
			{
				onDefPenetrabilityChangeEvent();
			}
			Action<int> onCharDefPenetrabilityChangeEvent = this.OnCharDefPenetrabilityChangeEvent;
			if (onCharDefPenetrabilityChangeEvent != null)
			{
				onCharDefPenetrabilityChangeEvent(base.CharacterId);
			}
		}

		// Token: 0x0600508D RID: 20621 RVA: 0x002599F4 File Offset: 0x00257BF4
		public unsafe override void InitFromDeadCharacter(DeadCharacter deadCharacter)
		{
			for (sbyte i = 0; i < 6; i += 1)
			{
				this.CurMainAttribute[(int)i] = *(ref deadCharacter.BaseMainAttributes.Items.FixedElementField + (IntPtr)i * 2);
			}
			this.DataFlag = 2;
		}

		// Token: 0x0600508E RID: 20622 RVA: 0x00259A3C File Offset: 0x00257C3C
		protected override bool IsValidMonitor()
		{
			Action<sbyte> onMainAttributeChangeEvent = this.OnMainAttributeChangeEvent;
			bool flag;
			if (onMainAttributeChangeEvent == null || onMainAttributeChangeEvent.GetInvocationList().Length == 0)
			{
				Action<int, sbyte> onCharMainAttributeChangeEvent = this.OnCharMainAttributeChangeEvent;
				flag = (onCharMainAttributeChangeEvent != null && onCharMainAttributeChangeEvent.GetInvocationList().Length != 0);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				Action<sbyte> onAtkHitValuesChangeEvent = this.OnAtkHitValuesChangeEvent;
				bool flag3;
				if (onAtkHitValuesChangeEvent == null || onAtkHitValuesChangeEvent.GetInvocationList().Length == 0)
				{
					Action<int, sbyte> onCharAtkHitValuesChangeEvent = this.OnCharAtkHitValuesChangeEvent;
					flag3 = (onCharAtkHitValuesChangeEvent != null && onCharAtkHitValuesChangeEvent.GetInvocationList().Length != 0);
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
					Action onAtkPenetrabilityChangeEvent = this.OnAtkPenetrabilityChangeEvent;
					bool flag5;
					if (onAtkPenetrabilityChangeEvent == null || onAtkPenetrabilityChangeEvent.GetInvocationList().Length == 0)
					{
						Action<int> onCharAtkPenetrabilityChangeEvent = this.OnCharAtkPenetrabilityChangeEvent;
						flag5 = (onCharAtkPenetrabilityChangeEvent != null && onCharAtkPenetrabilityChangeEvent.GetInvocationList().Length != 0);
					}
					else
					{
						flag5 = true;
					}
					bool flag6 = flag5;
					if (flag6)
					{
						result = true;
					}
					else
					{
						Action<sbyte> onDefHitValuesChangeEvent = this.OnDefHitValuesChangeEvent;
						bool flag7;
						if (onDefHitValuesChangeEvent == null || onDefHitValuesChangeEvent.GetInvocationList().Length == 0)
						{
							Action<int, sbyte> onCharDefHitValuesChangeEvent = this.OnCharDefHitValuesChangeEvent;
							flag7 = (onCharDefHitValuesChangeEvent != null && onCharDefHitValuesChangeEvent.GetInvocationList().Length != 0);
						}
						else
						{
							flag7 = true;
						}
						bool flag8 = flag7;
						if (flag8)
						{
							result = true;
						}
						else
						{
							Action onDefPenetrabilityChangeEvent = this.OnDefPenetrabilityChangeEvent;
							bool flag9;
							if (onDefPenetrabilityChangeEvent == null || onDefPenetrabilityChangeEvent.GetInvocationList().Length == 0)
							{
								Action<int> onCharDefPenetrabilityChangeEvent = this.OnCharDefPenetrabilityChangeEvent;
								flag9 = (onCharDefPenetrabilityChangeEvent != null && onCharDefPenetrabilityChangeEvent.GetInvocationList().Length != 0);
							}
							else
							{
								flag9 = true;
							}
							bool flag10 = flag9;
							result = flag10;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600508F RID: 20623 RVA: 0x00259B78 File Offset: 0x00257D78
		public unsafe override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type > 0;
			if (!flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 43U;
				if (flag2)
				{
					MainAttributes mainAttributes = default(MainAttributes);
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref mainAttributes);
					for (sbyte i = 0; i < 6; i += 1)
					{
						bool changeFlag = this.CurMainAttribute[(int)i] != *(ref mainAttributes.Items.FixedElementField + (IntPtr)i * 2);
						this.CurMainAttribute[(int)i] = *(ref mainAttributes.Items.FixedElementField + (IntPtr)i * 2);
						bool flag3 = base.Init && changeFlag;
						if (flag3)
						{
							Action<sbyte> onMainAttributeChangeEvent = this.OnMainAttributeChangeEvent;
							if (onMainAttributeChangeEvent != null)
							{
								onMainAttributeChangeEvent(i);
							}
							Action<int, sbyte> onCharMainAttributeChangeEvent = this.OnCharMainAttributeChangeEvent;
							if (onCharMainAttributeChangeEvent != null)
							{
								onCharMainAttributeChangeEvent(base.CharacterId, i);
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
					bool flag5 = wrapper.Notification.Uid.SubId1 == 79U;
					if (flag5)
					{
						MainAttributes mainAttributes2 = default(MainAttributes);
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref mainAttributes2);
						for (sbyte j = 0; j < 6; j += 1)
						{
							bool changeFlag2 = this.MaxMainAttribute[(int)j] != *(ref mainAttributes2.Items.FixedElementField + (IntPtr)j * 2);
							this.MaxMainAttribute[(int)j] = *(ref mainAttributes2.Items.FixedElementField + (IntPtr)j * 2);
							bool flag6 = base.Init && changeFlag2;
							if (flag6)
							{
								Action<sbyte> onMainAttributeChangeEvent2 = this.OnMainAttributeChangeEvent;
								if (onMainAttributeChangeEvent2 != null)
								{
									onMainAttributeChangeEvent2(j);
								}
								Action<int, sbyte> onCharMainAttributeChangeEvent2 = this.OnCharMainAttributeChangeEvent;
								if (onCharMainAttributeChangeEvent2 != null)
								{
									onCharMainAttributeChangeEvent2(base.CharacterId, j);
								}
							}
						}
						bool flag7 = !base.Init;
						if (flag7)
						{
							this.DataFlag = 1;
						}
					}
					else
					{
						bool flag8 = wrapper.Notification.Uid.SubId1 == 80U;
						if (flag8)
						{
							HitOrAvoidInts values = default(HitOrAvoidInts);
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref values);
							for (sbyte k = 0; k < 4; k += 1)
							{
								bool changeFlag3 = this.AtkHitValues[(int)k] != *(ref values.Items.FixedElementField + (IntPtr)k * 4);
								this.AtkHitValues[(int)k] = *(ref values.Items.FixedElementField + (IntPtr)k * 4);
								bool flag9 = base.Init && changeFlag3;
								if (flag9)
								{
									Action<sbyte> onAtkHitValuesChangeEvent = this.OnAtkHitValuesChangeEvent;
									if (onAtkHitValuesChangeEvent != null)
									{
										onAtkHitValuesChangeEvent(k);
									}
									Action<int, sbyte> onCharAtkHitValuesChangeEvent = this.OnCharAtkHitValuesChangeEvent;
									if (onCharAtkHitValuesChangeEvent != null)
									{
										onCharAtkHitValuesChangeEvent(base.CharacterId, k);
									}
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
							bool flag11 = wrapper.Notification.Uid.SubId1 == 81U;
							if (flag11)
							{
								OuterAndInnerInts intValues = default(OuterAndInnerInts);
								Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref intValues);
								bool changeFlag4 = this.AtkPenetrability.Outer != intValues.Outer || this.AtkPenetrability.Inner != intValues.Inner;
								this.AtkPenetrability = intValues;
								bool flag12 = base.Init && changeFlag4;
								if (flag12)
								{
									Action onAtkPenetrabilityChangeEvent = this.OnAtkPenetrabilityChangeEvent;
									if (onAtkPenetrabilityChangeEvent != null)
									{
										onAtkPenetrabilityChangeEvent();
									}
									Action<int> onCharAtkPenetrabilityChangeEvent = this.OnCharAtkPenetrabilityChangeEvent;
									if (onCharAtkPenetrabilityChangeEvent != null)
									{
										onCharAtkPenetrabilityChangeEvent(base.CharacterId);
									}
								}
								bool flag13 = !base.Init;
								if (flag13)
								{
									this.DataFlag = 1;
								}
							}
							else
							{
								bool flag14 = wrapper.Notification.Uid.SubId1 == 82U;
								if (flag14)
								{
									HitOrAvoidInts values2 = default(HitOrAvoidInts);
									Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref values2);
									for (sbyte l = 0; l < 4; l += 1)
									{
										bool changeFlag5 = this.DefHitValues[(int)l] != *(ref values2.Items.FixedElementField + (IntPtr)l * 4);
										this.DefHitValues[(int)l] = *(ref values2.Items.FixedElementField + (IntPtr)l * 4);
										bool flag15 = base.Init && changeFlag5;
										if (flag15)
										{
											Action<sbyte> onDefHitValuesChangeEvent = this.OnDefHitValuesChangeEvent;
											if (onDefHitValuesChangeEvent != null)
											{
												onDefHitValuesChangeEvent(l);
											}
											Action<int, sbyte> onCharDefHitValuesChangeEvent = this.OnCharDefHitValuesChangeEvent;
											if (onCharDefHitValuesChangeEvent != null)
											{
												onCharDefHitValuesChangeEvent(base.CharacterId, l);
											}
										}
									}
									bool flag16 = !base.Init;
									if (flag16)
									{
										this.DataFlag = 1;
									}
								}
								else
								{
									bool flag17 = wrapper.Notification.Uid.SubId1 == 83U;
									if (flag17)
									{
										OuterAndInnerInts intValues2 = default(OuterAndInnerInts);
										Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref intValues2);
										bool changeFlag6 = this.DefPenetrability.Outer != intValues2.Outer || this.DefPenetrability.Inner != intValues2.Inner;
										this.DefPenetrability = intValues2;
										bool flag18 = base.Init && changeFlag6;
										if (flag18)
										{
											Action onDefPenetrabilityChangeEvent = this.OnDefPenetrabilityChangeEvent;
											if (onDefPenetrabilityChangeEvent != null)
											{
												onDefPenetrabilityChangeEvent();
											}
											Action<int> onCharDefPenetrabilityChangeEvent = this.OnCharDefPenetrabilityChangeEvent;
											if (onCharDefPenetrabilityChangeEvent != null)
											{
												onCharDefPenetrabilityChangeEvent(base.CharacterId);
											}
										}
										bool flag19 = !base.Init;
										if (flag19)
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

		// Token: 0x06005090 RID: 20624 RVA: 0x0025A107 File Offset: 0x00258307
		public void AddMainAttributeListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnMainAttributeChangeEvent -= listener;
			this.OnMainAttributeChangeEvent += listener;
			this.OnAddRemoveListenerForFieldId(43U);
			this.OnAddRemoveListenerForFieldId(79U);
		}

		// Token: 0x06005091 RID: 20625 RVA: 0x0025A13B File Offset: 0x0025833B
		public void RemoveMainAttributeListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnMainAttributeChangeEvent -= listener;
			this.OnAddRemoveListenerForFieldId(43U);
			this.OnAddRemoveListenerForFieldId(79U);
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005092 RID: 20626 RVA: 0x0025A16E File Offset: 0x0025836E
		public void AddMainAttributeListener(Action<int, sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharMainAttributeChangeEvent -= listener;
			this.OnCharMainAttributeChangeEvent += listener;
			this.OnAddRemoveListenerForFieldId(43U);
			this.OnAddRemoveListenerForFieldId(79U);
		}

		// Token: 0x06005093 RID: 20627 RVA: 0x0025A1A2 File Offset: 0x002583A2
		public void RemoveMainAttributeListener(Action<int, sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharMainAttributeChangeEvent -= listener;
			this.OnAddRemoveListenerForFieldId(43U);
			this.OnAddRemoveListenerForFieldId(79U);
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005094 RID: 20628 RVA: 0x0025A1D5 File Offset: 0x002583D5
		public void AddAtkHitValuesListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAtkHitValuesChangeEvent -= listener;
			this.OnAtkHitValuesChangeEvent += listener;
			this.OnAddRemoveListenerForFieldId(80U);
		}

		// Token: 0x06005095 RID: 20629 RVA: 0x0025A200 File Offset: 0x00258400
		public void RemoveAtkHitValuesListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAtkHitValuesChangeEvent -= listener;
			this.OnAddRemoveListenerForFieldId(80U);
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005096 RID: 20630 RVA: 0x0025A22A File Offset: 0x0025842A
		public void AddAtkHitValuesListener(Action<int, sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharAtkHitValuesChangeEvent -= listener;
			this.OnCharAtkHitValuesChangeEvent += listener;
			this.OnAddRemoveListenerForFieldId(80U);
		}

		// Token: 0x06005097 RID: 20631 RVA: 0x0025A255 File Offset: 0x00258455
		public void RemoveAtkHitValuesListener(Action<int, sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharAtkHitValuesChangeEvent -= listener;
			this.OnAddRemoveListenerForFieldId(80U);
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005098 RID: 20632 RVA: 0x0025A27F File Offset: 0x0025847F
		public void AddAtkPenetrabilityListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAtkPenetrabilityChangeEvent -= listener;
			this.OnAtkPenetrabilityChangeEvent += listener;
			this.OnAddRemoveListenerForFieldId(81U);
		}

		// Token: 0x06005099 RID: 20633 RVA: 0x0025A2AA File Offset: 0x002584AA
		public void RemoveAtkPenetrabilityListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAtkPenetrabilityChangeEvent -= listener;
			this.OnAddRemoveListenerForFieldId(81U);
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600509A RID: 20634 RVA: 0x0025A2D4 File Offset: 0x002584D4
		public void AddAtkPenetrabilityListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharAtkPenetrabilityChangeEvent -= listener;
			this.OnCharAtkPenetrabilityChangeEvent += listener;
			this.OnAddRemoveListenerForFieldId(81U);
		}

		// Token: 0x0600509B RID: 20635 RVA: 0x0025A2FF File Offset: 0x002584FF
		public void RemoveAtkPenetrabilityListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharAtkPenetrabilityChangeEvent -= listener;
			this.OnAddRemoveListenerForFieldId(81U);
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600509C RID: 20636 RVA: 0x0025A329 File Offset: 0x00258529
		public void AddDefHitValuesListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnDefHitValuesChangeEvent -= listener;
			this.OnDefHitValuesChangeEvent += listener;
			this.OnAddRemoveListenerForFieldId(82U);
		}

		// Token: 0x0600509D RID: 20637 RVA: 0x0025A354 File Offset: 0x00258554
		public void RemoveDefHitValuesListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnDefHitValuesChangeEvent -= listener;
			this.OnAddRemoveListenerForFieldId(82U);
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600509E RID: 20638 RVA: 0x0025A37E File Offset: 0x0025857E
		public void AddDefHitValuesListener(Action<int, sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharDefHitValuesChangeEvent -= listener;
			this.OnCharDefHitValuesChangeEvent += listener;
			this.OnAddRemoveListenerForFieldId(82U);
		}

		// Token: 0x0600509F RID: 20639 RVA: 0x0025A3A9 File Offset: 0x002585A9
		public void RemoveDefHitValuesListener(Action<int, sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharDefHitValuesChangeEvent -= listener;
			this.OnAddRemoveListenerForFieldId(82U);
			base.OnChangeEventRemoved();
		}

		// Token: 0x060050A0 RID: 20640 RVA: 0x0025A3D3 File Offset: 0x002585D3
		public void AddDefPenetrabilityListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnDefPenetrabilityChangeEvent -= listener;
			this.OnDefPenetrabilityChangeEvent += listener;
			this.OnAddRemoveListenerForFieldId(83U);
		}

		// Token: 0x060050A1 RID: 20641 RVA: 0x0025A3FE File Offset: 0x002585FE
		public void RemoveDefPenetrabilityListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnDefPenetrabilityChangeEvent -= listener;
			this.OnAddRemoveListenerForFieldId(83U);
			base.OnChangeEventRemoved();
		}

		// Token: 0x060050A2 RID: 20642 RVA: 0x0025A428 File Offset: 0x00258628
		public void AddDefPenetrabilityListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharDefPenetrabilityChangeEvent -= listener;
			this.OnCharDefPenetrabilityChangeEvent += listener;
			this.OnAddRemoveListenerForFieldId(83U);
		}

		// Token: 0x060050A3 RID: 20643 RVA: 0x0025A453 File Offset: 0x00258653
		public void RemoveDefPenetrabilityListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharDefPenetrabilityChangeEvent -= listener;
			this.OnAddRemoveListenerForFieldId(83U);
			base.OnChangeEventRemoved();
		}

		// Token: 0x060050A4 RID: 20644 RVA: 0x0025A480 File Offset: 0x00258680
		private void OnAddRemoveListenerForFieldId(uint filedId)
		{
			List<MulticastDelegate> list = EasyPool.Get<List<MulticastDelegate>>();
			this._fieldListenManager.OnAddRemoveListener(filedId, this.GetActionsForField(filedId, list));
			EasyPool.Free<List<MulticastDelegate>>(list);
		}

		// Token: 0x060050A5 RID: 20645 RVA: 0x0025A4B0 File Offset: 0x002586B0
		private List<MulticastDelegate> GetActionsForField(uint fieldId, List<MulticastDelegate> list)
		{
			list.Clear();
			if (fieldId != 43U)
			{
				switch (fieldId)
				{
				case 79U:
					break;
				case 80U:
					list.Add(this.OnAtkHitValuesChangeEvent);
					list.Add(this.OnCharAtkHitValuesChangeEvent);
					return list;
				case 81U:
					list.Add(this.OnAtkPenetrabilityChangeEvent);
					list.Add(this.OnCharAtkPenetrabilityChangeEvent);
					return list;
				case 82U:
					list.Add(this.OnDefHitValuesChangeEvent);
					list.Add(this.OnCharDefHitValuesChangeEvent);
					return list;
				case 83U:
					list.Add(this.OnDefPenetrabilityChangeEvent);
					list.Add(this.OnCharDefPenetrabilityChangeEvent);
					return list;
				default:
					throw new ArgumentOutOfRangeException("fieldId", fieldId, null);
				}
			}
			list.Add(this.OnMainAttributeChangeEvent);
			list.Add(this.OnCharMainAttributeChangeEvent);
			return list;
		}

		// Token: 0x060050A6 RID: 20646 RVA: 0x0025A598 File Offset: 0x00258798
		private void AddMonitorByFieldId(uint fieldId)
		{
			GameDataBridge.AddDataMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
		}

		// Token: 0x060050A7 RID: 20647 RVA: 0x0025A5BA File Offset: 0x002587BA
		private void RemoveMonitorByFieldId(uint fieldId)
		{
			GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
		}

		// Token: 0x04003778 RID: 14200
		private MonitorPerFieldListenManager _fieldListenManager;
	}
}
