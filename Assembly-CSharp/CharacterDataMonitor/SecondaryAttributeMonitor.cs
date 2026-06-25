using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006CB RID: 1739
	public class SecondaryAttributeMonitor : MonitorDataItemBase
	{
		// Token: 0x1400006E RID: 110
		// (add) Token: 0x060052C2 RID: 21186 RVA: 0x00262E84 File Offset: 0x00261084
		// (remove) Token: 0x060052C3 RID: 21187 RVA: 0x00262EBC File Offset: 0x002610BC
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<sbyte> OnAttributeChangeEvent;

		// Token: 0x17000A40 RID: 2624
		// (get) Token: 0x060052C4 RID: 21188 RVA: 0x00262EF1 File Offset: 0x002610F1
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A41 RID: 2625
		// (get) Token: 0x060052C5 RID: 21189 RVA: 0x00262EF4 File Offset: 0x002610F4
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					84U,
					85U,
					86U,
					87U,
					88U,
					89U,
					90U,
					91U,
					92U
				};
			}
		}

		// Token: 0x060052C6 RID: 21190 RVA: 0x00262F57 File Offset: 0x00261157
		public SecondaryAttributeMonitor()
		{
			this.Attributes = new short[10];
		}

		// Token: 0x060052C7 RID: 21191 RVA: 0x00262F70 File Offset: 0x00261170
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

		// Token: 0x060052C8 RID: 21192 RVA: 0x00262FF4 File Offset: 0x002611F4
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x060052C9 RID: 21193 RVA: 0x00263064 File Offset: 0x00261264
		public override void OnDataInit()
		{
			Action<sbyte> onAttributeChangeEvent = this.OnAttributeChangeEvent;
			if (onAttributeChangeEvent != null)
			{
				onAttributeChangeEvent(10);
			}
		}

		// Token: 0x060052CA RID: 21194 RVA: 0x0026307C File Offset: 0x0026127C
		protected override bool IsValidMonitor()
		{
			Action<sbyte> onAttributeChangeEvent = this.OnAttributeChangeEvent;
			return onAttributeChangeEvent != null && onAttributeChangeEvent.GetInvocationList().Length != 0;
		}

		// Token: 0x060052CB RID: 21195 RVA: 0x002630A4 File Offset: 0x002612A4
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type > 0;
			if (!flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 84U;
				if (flag2)
				{
					OuterAndInnerShorts shorts = default(OuterAndInnerShorts);
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref shorts);
					bool outerChange = this.Attributes[0] != shorts.Outer;
					bool innerChange = this.Attributes[1] != shorts.Inner;
					this.Attributes[0] = shorts.Outer;
					this.Attributes[1] = shorts.Inner;
					bool init = base.Init;
					if (init)
					{
						bool flag3 = outerChange;
						if (flag3)
						{
							Action<sbyte> onAttributeChangeEvent = this.OnAttributeChangeEvent;
							if (onAttributeChangeEvent != null)
							{
								onAttributeChangeEvent(0);
							}
						}
						bool flag4 = innerChange;
						if (flag4)
						{
							Action<sbyte> onAttributeChangeEvent2 = this.OnAttributeChangeEvent;
							if (onAttributeChangeEvent2 != null)
							{
								onAttributeChangeEvent2(1);
							}
						}
					}
				}
				else
				{
					bool flag5 = wrapper.Notification.Uid.SubId1 == 85U;
					if (flag5)
					{
						short value = -1;
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value);
						bool changeFlag = this.Attributes[2] != value;
						this.Attributes[2] = value;
						bool flag6 = base.Init && changeFlag;
						if (flag6)
						{
							Action<sbyte> onAttributeChangeEvent3 = this.OnAttributeChangeEvent;
							if (onAttributeChangeEvent3 != null)
							{
								onAttributeChangeEvent3(2);
							}
						}
					}
					else
					{
						bool flag7 = wrapper.Notification.Uid.SubId1 == 86U;
						if (flag7)
						{
							short value2 = -1;
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value2);
							bool changeFlag2 = this.Attributes[3] != value2;
							this.Attributes[3] = value2;
							bool flag8 = base.Init && changeFlag2;
							if (flag8)
							{
								Action<sbyte> onAttributeChangeEvent4 = this.OnAttributeChangeEvent;
								if (onAttributeChangeEvent4 != null)
								{
									onAttributeChangeEvent4(3);
								}
							}
						}
						else
						{
							bool flag9 = wrapper.Notification.Uid.SubId1 == 87U;
							if (flag9)
							{
								short value3 = -1;
								Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value3);
								bool changeFlag3 = this.Attributes[4] != value3;
								this.Attributes[4] = value3;
								bool flag10 = base.Init && changeFlag3;
								if (flag10)
								{
									Action<sbyte> onAttributeChangeEvent5 = this.OnAttributeChangeEvent;
									if (onAttributeChangeEvent5 != null)
									{
										onAttributeChangeEvent5(4);
									}
								}
							}
							else
							{
								bool flag11 = wrapper.Notification.Uid.SubId1 == 88U;
								if (flag11)
								{
									short value4 = -1;
									Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value4);
									bool changeFlag4 = this.Attributes[5] != value4;
									this.Attributes[5] = value4;
									bool flag12 = base.Init && changeFlag4;
									if (flag12)
									{
										Action<sbyte> onAttributeChangeEvent6 = this.OnAttributeChangeEvent;
										if (onAttributeChangeEvent6 != null)
										{
											onAttributeChangeEvent6(5);
										}
									}
								}
								else
								{
									bool flag13 = wrapper.Notification.Uid.SubId1 == 89U;
									if (flag13)
									{
										short value5 = -1;
										Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value5);
										bool changeFlag5 = this.Attributes[6] != value5;
										this.Attributes[6] = value5;
										bool flag14 = base.Init && changeFlag5;
										if (flag14)
										{
											Action<sbyte> onAttributeChangeEvent7 = this.OnAttributeChangeEvent;
											if (onAttributeChangeEvent7 != null)
											{
												onAttributeChangeEvent7(6);
											}
										}
									}
									else
									{
										bool flag15 = wrapper.Notification.Uid.SubId1 == 90U;
										if (flag15)
										{
											short value6 = -1;
											Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value6);
											bool changeFlag6 = this.Attributes[7] != value6;
											this.Attributes[7] = value6;
											bool flag16 = base.Init && changeFlag6;
											if (flag16)
											{
												Action<sbyte> onAttributeChangeEvent8 = this.OnAttributeChangeEvent;
												if (onAttributeChangeEvent8 != null)
												{
													onAttributeChangeEvent8(7);
												}
											}
										}
										else
										{
											bool flag17 = wrapper.Notification.Uid.SubId1 == 91U;
											if (flag17)
											{
												short value7 = -1;
												Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value7);
												bool changeFlag7 = this.Attributes[8] != value7;
												this.Attributes[8] = value7;
												bool flag18 = base.Init && changeFlag7;
												if (flag18)
												{
													Action<sbyte> onAttributeChangeEvent9 = this.OnAttributeChangeEvent;
													if (onAttributeChangeEvent9 != null)
													{
														onAttributeChangeEvent9(8);
													}
												}
											}
											else
											{
												bool flag19 = wrapper.Notification.Uid.SubId1 == 92U;
												if (flag19)
												{
													short value8 = -1;
													Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value8);
													bool changeFlag8 = this.Attributes[9] != value8;
													this.Attributes[9] = value8;
													bool flag20 = base.Init && changeFlag8;
													if (flag20)
													{
														Action<sbyte> onAttributeChangeEvent10 = this.OnAttributeChangeEvent;
														if (onAttributeChangeEvent10 != null)
														{
															onAttributeChangeEvent10(9);
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
			}
		}

		// Token: 0x060052CC RID: 21196 RVA: 0x00263578 File Offset: 0x00261778
		public void AddAttributeListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAttributeChangeEvent -= listener;
			this.OnAttributeChangeEvent += listener;
		}

		// Token: 0x060052CD RID: 21197 RVA: 0x0026359A File Offset: 0x0026179A
		public void RemoveAttributeListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnAttributeChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x04003816 RID: 14358
		public short[] Attributes;
	}
}
