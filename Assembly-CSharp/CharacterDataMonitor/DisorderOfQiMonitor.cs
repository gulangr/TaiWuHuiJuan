using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006BE RID: 1726
	public class DisorderOfQiMonitor : MonitorDataItemBase
	{
		// Token: 0x1400004D RID: 77
		// (add) Token: 0x06005192 RID: 20882 RVA: 0x0025E69C File Offset: 0x0025C89C
		// (remove) Token: 0x06005193 RID: 20883 RVA: 0x0025E6D4 File Offset: 0x0025C8D4
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnChangeOfMainAttributeChangeEvent;

		// Token: 0x1400004E RID: 78
		// (add) Token: 0x06005194 RID: 20884 RVA: 0x0025E70C File Offset: 0x0025C90C
		// (remove) Token: 0x06005195 RID: 20885 RVA: 0x0025E744 File Offset: 0x0025C944
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnChangeOfQiDisorderEvent;

		// Token: 0x1400004F RID: 79
		// (add) Token: 0x06005196 RID: 20886 RVA: 0x0025E77C File Offset: 0x0025C97C
		// (remove) Token: 0x06005197 RID: 20887 RVA: 0x0025E7B4 File Offset: 0x0025C9B4
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnDisorderOfQiChangeEvent;

		// Token: 0x17000A08 RID: 2568
		// (get) Token: 0x06005198 RID: 20888 RVA: 0x0025E7E9 File Offset: 0x0025C9E9
		// (set) Token: 0x06005199 RID: 20889 RVA: 0x0025E7F1 File Offset: 0x0025C9F1
		public short[] MainAttribute { get; private set; }

		// Token: 0x17000A09 RID: 2569
		// (get) Token: 0x0600519A RID: 20890 RVA: 0x0025E7FA File Offset: 0x0025C9FA
		// (set) Token: 0x0600519B RID: 20891 RVA: 0x0025E802 File Offset: 0x0025CA02
		public short DisorderOfQi { get; private set; }

		// Token: 0x17000A0A RID: 2570
		// (get) Token: 0x0600519C RID: 20892 RVA: 0x0025E80B File Offset: 0x0025CA0B
		// (set) Token: 0x0600519D RID: 20893 RVA: 0x0025E813 File Offset: 0x0025CA13
		public short ChangeOfQiDisorder { get; private set; }

		// Token: 0x17000A0B RID: 2571
		// (get) Token: 0x0600519E RID: 20894 RVA: 0x0025E81C File Offset: 0x0025CA1C
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000A0C RID: 2572
		// (get) Token: 0x0600519F RID: 20895 RVA: 0x0025E81F File Offset: 0x0025CA1F
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					21U,
					92U
				};
			}
		}

		// Token: 0x060051A0 RID: 20896 RVA: 0x0025E838 File Offset: 0x0025CA38
		public DisorderOfQiMonitor()
		{
			this.MainAttribute = new short[6];
		}

		// Token: 0x060051A1 RID: 20897 RVA: 0x0025E850 File Offset: 0x0025CA50
		public override void Refresh()
		{
			Action onDisorderOfQiChangeEvent = this.OnDisorderOfQiChangeEvent;
			if (onDisorderOfQiChangeEvent != null)
			{
				onDisorderOfQiChangeEvent();
			}
			bool isDead = base.Character.IsDead;
			if (isDead)
			{
				Action onChangeOfMainAttributeChangeEvent = this.OnChangeOfMainAttributeChangeEvent;
				if (onChangeOfMainAttributeChangeEvent != null)
				{
					onChangeOfMainAttributeChangeEvent();
				}
				Action onChangeOfQiDisorderEvent = this.OnChangeOfQiDisorderEvent;
				if (onChangeOfQiDisorderEvent != null)
				{
					onChangeOfQiDisorderEvent();
				}
			}
			else
			{
				this.DataFlag = 0;
				base.Character.CallMethod(42);
				base.Character.CallMethod(83);
			}
		}

		// Token: 0x060051A2 RID: 20898 RVA: 0x0025E8CC File Offset: 0x0025CACC
		protected override void MonitorData()
		{
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				foreach (uint fieldId in this.RelativeFieldIds)
				{
					GameDataBridge.AddDataMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
				}
				base.Character.CallMethod(42);
				base.Character.CallMethod(83);
			}
		}

		// Token: 0x060051A3 RID: 20899 RVA: 0x0025E96C File Offset: 0x0025CB6C
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x060051A4 RID: 20900 RVA: 0x0025E9DC File Offset: 0x0025CBDC
		public override void OnDataInit()
		{
			Action onChangeOfMainAttributeChangeEvent = this.OnChangeOfMainAttributeChangeEvent;
			if (onChangeOfMainAttributeChangeEvent != null)
			{
				onChangeOfMainAttributeChangeEvent();
			}
			Action onDisorderOfQiChangeEvent = this.OnDisorderOfQiChangeEvent;
			if (onDisorderOfQiChangeEvent != null)
			{
				onDisorderOfQiChangeEvent();
			}
			Action onChangeOfQiDisorderEvent = this.OnChangeOfQiDisorderEvent;
			if (onChangeOfQiDisorderEvent != null)
			{
				onChangeOfQiDisorderEvent();
			}
		}

		// Token: 0x060051A5 RID: 20901 RVA: 0x0025EA18 File Offset: 0x0025CC18
		protected override bool IsValidMonitor()
		{
			Action onChangeOfMainAttributeChangeEvent = this.OnChangeOfMainAttributeChangeEvent;
			bool flag = onChangeOfMainAttributeChangeEvent != null && onChangeOfMainAttributeChangeEvent.GetInvocationList().Length != 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				Action onDisorderOfQiChangeEvent = this.OnDisorderOfQiChangeEvent;
				bool flag2 = onDisorderOfQiChangeEvent != null && onDisorderOfQiChangeEvent.GetInvocationList().Length != 0;
				if (flag2)
				{
					result = true;
				}
				else
				{
					Action onChangeOfQiDisorderEvent = this.OnChangeOfQiDisorderEvent;
					bool flag3 = onChangeOfQiDisorderEvent != null && onChangeOfQiDisorderEvent.GetInvocationList().Length != 0;
					result = flag3;
				}
			}
			return result;
		}

		// Token: 0x060051A6 RID: 20902 RVA: 0x0025EA88 File Offset: 0x0025CC88
		public unsafe override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type == 0;
			if (flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 21U;
				if (flag2)
				{
					short value = -1;
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value);
					bool changeFlag = value != this.DisorderOfQi;
					this.DisorderOfQi = value;
					bool flag3 = base.Init && changeFlag;
					if (flag3)
					{
						Action onDisorderOfQiChangeEvent = this.OnDisorderOfQiChangeEvent;
						if (onDisorderOfQiChangeEvent != null)
						{
							onDisorderOfQiChangeEvent();
						}
					}
				}
				else
				{
					bool flag4 = wrapper.Notification.Uid.SubId1 == 92U;
					if (flag4)
					{
						base.Character.CallMethod(83);
					}
				}
			}
			else
			{
				bool flag5 = wrapper.Notification.Type == 1;
				if (flag5)
				{
					bool flag6 = wrapper.Notification.MethodId == 42;
					if (flag6)
					{
						MainAttributes mainAttribute = default(MainAttributes);
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref mainAttribute);
						bool changeFlag2 = !base.Init;
						for (sbyte i = 0; i < 6; i += 1)
						{
							changeFlag2 = (changeFlag2 || this.MainAttribute[(int)i] != *(ref mainAttribute.Items.FixedElementField + (IntPtr)i * 2));
							this.MainAttribute[(int)i] = *(ref mainAttribute.Items.FixedElementField + (IntPtr)i * 2);
						}
						bool flag7 = base.Init && changeFlag2;
						if (flag7)
						{
							Action onChangeOfMainAttributeChangeEvent = this.OnChangeOfMainAttributeChangeEvent;
							if (onChangeOfMainAttributeChangeEvent != null)
							{
								onChangeOfMainAttributeChangeEvent();
							}
						}
					}
					else
					{
						bool flag8 = wrapper.Notification.MethodId == 83;
						if (flag8)
						{
							short value2 = -1;
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref value2);
							bool changeFlag3 = value2 != this.ChangeOfQiDisorder;
							this.ChangeOfQiDisorder = value2;
							bool flag9 = base.Init && changeFlag3;
							if (flag9)
							{
								Action onChangeOfQiDisorderEvent = this.OnChangeOfQiDisorderEvent;
								if (onChangeOfQiDisorderEvent != null)
								{
									onChangeOfQiDisorderEvent();
								}
							}
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

		// Token: 0x060051A7 RID: 20903 RVA: 0x0025ECAE File Offset: 0x0025CEAE
		public void AddChangeOfMainAttributeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnChangeOfMainAttributeChangeEvent -= listener;
			this.OnChangeOfMainAttributeChangeEvent += listener;
		}

		// Token: 0x060051A8 RID: 20904 RVA: 0x0025ECD0 File Offset: 0x0025CED0
		public void RemoveChangeOfMainAttributeListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnChangeOfMainAttributeChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060051A9 RID: 20905 RVA: 0x0025ECF1 File Offset: 0x0025CEF1
		public void AddChangeOfQiDisorderListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnChangeOfQiDisorderEvent -= listener;
			this.OnChangeOfQiDisorderEvent += listener;
		}

		// Token: 0x060051AA RID: 20906 RVA: 0x0025ED13 File Offset: 0x0025CF13
		public void RemoveChangeOfQiDisorderListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnChangeOfQiDisorderEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060051AB RID: 20907 RVA: 0x0025ED34 File Offset: 0x0025CF34
		public void AddDisorderOfQiListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnDisorderOfQiChangeEvent -= listener;
			this.OnDisorderOfQiChangeEvent += listener;
		}

		// Token: 0x060051AC RID: 20908 RVA: 0x0025ED56 File Offset: 0x0025CF56
		public void RemoveDisorderOfQiListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnDisorderOfQiChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}
	}
}
