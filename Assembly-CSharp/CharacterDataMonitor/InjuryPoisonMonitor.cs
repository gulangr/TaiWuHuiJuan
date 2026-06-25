using System;
using System.Collections.Generic;
using System.Diagnostics;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;

namespace CharacterDataMonitor
{
	// Token: 0x020006C3 RID: 1731
	public class InjuryPoisonMonitor : MonitorDataItemBase
	{
		// Token: 0x14000060 RID: 96
		// (add) Token: 0x0600522F RID: 21039 RVA: 0x00260DD8 File Offset: 0x0025EFD8
		// (remove) Token: 0x06005230 RID: 21040 RVA: 0x00260E10 File Offset: 0x0025F010
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnInjuriesChangeEvent;

		// Token: 0x14000061 RID: 97
		// (add) Token: 0x06005231 RID: 21041 RVA: 0x00260E48 File Offset: 0x0025F048
		// (remove) Token: 0x06005232 RID: 21042 RVA: 0x00260E80 File Offset: 0x0025F080
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int> OnCharInjuriesChangeEvent;

		// Token: 0x14000062 RID: 98
		// (add) Token: 0x06005233 RID: 21043 RVA: 0x00260EB8 File Offset: 0x0025F0B8
		// (remove) Token: 0x06005234 RID: 21044 RVA: 0x00260EF0 File Offset: 0x0025F0F0
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<sbyte> OnPoisonsChangeEvent;

		// Token: 0x14000063 RID: 99
		// (add) Token: 0x06005235 RID: 21045 RVA: 0x00260F28 File Offset: 0x0025F128
		// (remove) Token: 0x06005236 RID: 21046 RVA: 0x00260F60 File Offset: 0x0025F160
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<sbyte> OnPoisonResistsChangeEvent;

		// Token: 0x17000A23 RID: 2595
		// (get) Token: 0x06005237 RID: 21047 RVA: 0x00260F95 File Offset: 0x0025F195
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A24 RID: 2596
		// (get) Token: 0x06005238 RID: 21048 RVA: 0x00260F98 File Offset: 0x0025F198
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					1U,
					44U,
					93U,
					26U
				};
			}
		}

		// Token: 0x06005239 RID: 21049 RVA: 0x00260FC2 File Offset: 0x0025F1C2
		public InjuryPoisonMonitor()
		{
			this.Poisons = new int[6];
			this.PoisonResists = new int[6];
		}

		// Token: 0x0600523A RID: 21050 RVA: 0x00260FE4 File Offset: 0x0025F1E4
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

		// Token: 0x0600523B RID: 21051 RVA: 0x00261068 File Offset: 0x0025F268
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x0600523C RID: 21052 RVA: 0x002610D8 File Offset: 0x0025F2D8
		public override void OnDataInit()
		{
			Action onInjuriesChangeEvent = this.OnInjuriesChangeEvent;
			if (onInjuriesChangeEvent != null)
			{
				onInjuriesChangeEvent();
			}
			Action<int> onCharInjuriesChangeEvent = this.OnCharInjuriesChangeEvent;
			if (onCharInjuriesChangeEvent != null)
			{
				onCharInjuriesChangeEvent(base.CharacterId);
			}
			Action<sbyte> onPoisonsChangeEvent = this.OnPoisonsChangeEvent;
			if (onPoisonsChangeEvent != null)
			{
				onPoisonsChangeEvent(6);
			}
			Action<sbyte> onPoisonResistsChangeEvent = this.OnPoisonResistsChangeEvent;
			if (onPoisonResistsChangeEvent != null)
			{
				onPoisonResistsChangeEvent(6);
			}
		}

		// Token: 0x0600523D RID: 21053 RVA: 0x00261138 File Offset: 0x0025F338
		protected override bool IsValidMonitor()
		{
			Action onInjuriesChangeEvent = this.OnInjuriesChangeEvent;
			bool flag;
			if (onInjuriesChangeEvent == null || onInjuriesChangeEvent.GetInvocationList().Length == 0)
			{
				Action<int> onCharInjuriesChangeEvent = this.OnCharInjuriesChangeEvent;
				flag = (onCharInjuriesChangeEvent != null && onCharInjuriesChangeEvent.GetInvocationList().Length != 0);
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
				Action<sbyte> onPoisonsChangeEvent = this.OnPoisonsChangeEvent;
				bool flag3 = onPoisonsChangeEvent != null && onPoisonsChangeEvent.GetInvocationList().Length != 0;
				if (flag3)
				{
					result = true;
				}
				else
				{
					Action<sbyte> onPoisonResistsChangeEvent = this.OnPoisonResistsChangeEvent;
					bool flag4 = onPoisonResistsChangeEvent != null && onPoisonResistsChangeEvent.GetInvocationList().Length != 0;
					result = flag4;
				}
			}
			return result;
		}

		// Token: 0x0600523E RID: 21054 RVA: 0x002611C0 File Offset: 0x0025F3C0
		public unsafe override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type > 0;
			if (!flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 1U;
				if (flag2)
				{
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref this.TemplateId);
					CharacterItem character = Config.Character.Instance[this.TemplateId];
					this.ImmunePoisonConfig = character.PoisonImmunities;
				}
				else
				{
					bool flag3 = wrapper.Notification.Uid.SubId1 == 44U;
					if (flag3)
					{
						PoisonInts poisons = default(PoisonInts);
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref poisons);
						for (sbyte i = 0; i < 6; i += 1)
						{
							bool changeFlag = this.Poisons[(int)i] != *(ref poisons.Items.FixedElementField + (IntPtr)i * 4);
							this.Poisons[(int)i] = *(ref poisons.Items.FixedElementField + (IntPtr)i * 4);
							bool flag4 = base.Init && changeFlag;
							if (flag4)
							{
								Action<sbyte> onPoisonsChangeEvent = this.OnPoisonsChangeEvent;
								if (onPoisonsChangeEvent != null)
								{
									onPoisonsChangeEvent(i);
								}
							}
						}
					}
					else
					{
						bool flag5 = wrapper.Notification.Uid.SubId1 == 93U;
						if (flag5)
						{
							PoisonInts poisonsResists = default(PoisonInts);
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref poisonsResists);
							for (sbyte j = 0; j < 6; j += 1)
							{
								bool changeFlag2 = this.PoisonResists[(int)j] != *(ref poisonsResists.Items.FixedElementField + (IntPtr)j * 4);
								this.PoisonResists[(int)j] = *(ref poisonsResists.Items.FixedElementField + (IntPtr)j * 4);
								bool flag6 = base.Init && changeFlag2;
								if (flag6)
								{
									Action<sbyte> onPoisonResistsChangeEvent = this.OnPoisonResistsChangeEvent;
									if (onPoisonResistsChangeEvent != null)
									{
										onPoisonResistsChangeEvent(j);
									}
								}
							}
						}
						else
						{
							bool flag7 = wrapper.Notification.Uid.SubId1 == 26U;
							if (flag7)
							{
								Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref this.Injuries);
								bool init = base.Init;
								if (init)
								{
									Action onInjuriesChangeEvent = this.OnInjuriesChangeEvent;
									if (onInjuriesChangeEvent != null)
									{
										onInjuriesChangeEvent();
									}
									Action<int> onCharInjuriesChangeEvent = this.OnCharInjuriesChangeEvent;
									if (onCharInjuriesChangeEvent != null)
									{
										onCharInjuriesChangeEvent(base.CharacterId);
									}
								}
								bool flag8 = !base.Init;
								if (flag8)
								{
									ExtraDomainMethod.AsyncCall.GetPoisonImmunities(null, base.CharacterId, delegate(int offset, RawDataPool dataPool)
									{
										Serializer.Deserialize(dataPool, offset, ref this.ImmunePoisonExtra);
										this.DataFlag = 1;
										this.OnDataInit();
									});
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600523F RID: 21055 RVA: 0x00261459 File Offset: 0x0025F659
		public override void Refresh()
		{
			ExtraDomainMethod.AsyncCall.GetPoisonImmunities(null, base.CharacterId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this.ImmunePoisonExtra);
				base.Refresh();
			});
		}

		// Token: 0x06005240 RID: 21056 RVA: 0x00261475 File Offset: 0x0025F675
		public void AddInjuriesListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnInjuriesChangeEvent -= listener;
			this.OnInjuriesChangeEvent += listener;
		}

		// Token: 0x06005241 RID: 21057 RVA: 0x00261497 File Offset: 0x0025F697
		public void RemoveInjuriesListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnInjuriesChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005242 RID: 21058 RVA: 0x002614B8 File Offset: 0x0025F6B8
		public void ClearInjuriesListener()
		{
			this.OnInjuriesChangeEvent = null;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005243 RID: 21059 RVA: 0x002614C9 File Offset: 0x0025F6C9
		public void AddInjuriesListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharInjuriesChangeEvent -= listener;
			this.OnCharInjuriesChangeEvent += listener;
		}

		// Token: 0x06005244 RID: 21060 RVA: 0x002614EB File Offset: 0x0025F6EB
		public void RemoveInjuriesListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharInjuriesChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005245 RID: 21061 RVA: 0x0026150C File Offset: 0x0025F70C
		public void AddPoisonsListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnPoisonsChangeEvent -= listener;
			this.OnPoisonsChangeEvent += listener;
		}

		// Token: 0x06005246 RID: 21062 RVA: 0x0026152E File Offset: 0x0025F72E
		public void RemovePoisonsListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnPoisonsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005247 RID: 21063 RVA: 0x0026154F File Offset: 0x0025F74F
		public void ClearPoisonsListener()
		{
			this.OnPoisonsChangeEvent = null;
			base.OnChangeEventRemoved();
		}

		// Token: 0x06005248 RID: 21064 RVA: 0x00261560 File Offset: 0x0025F760
		public void AddPoisonResistsListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnPoisonResistsChangeEvent -= listener;
			this.OnPoisonResistsChangeEvent += listener;
		}

		// Token: 0x06005249 RID: 21065 RVA: 0x00261582 File Offset: 0x0025F782
		public void RemovePoisonResistsListener(Action<sbyte> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnPoisonResistsChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600524A RID: 21066 RVA: 0x002615A3 File Offset: 0x0025F7A3
		public void ClearPoisonResistsListener()
		{
			this.OnPoisonResistsChangeEvent = null;
			base.OnChangeEventRemoved();
		}

		// Token: 0x0600524B RID: 21067 RVA: 0x002615B4 File Offset: 0x0025F7B4
		public bool IsImmune(sbyte poisonType)
		{
			CharacterItem characterCfg = Config.Character.Instance.GetItem(this.TemplateId);
			return (this.ImmunePoisonConfig != null && this.ImmunePoisonConfig[(int)poisonType]) || GameData.Domains.Character.SharedMethods.HasPoisonImmunity(poisonType, characterCfg, ref this.PoisonResists, this.ImmunePoisonExtra);
		}

		// Token: 0x0600524C RID: 21068 RVA: 0x00261604 File Offset: 0x0025F804
		public bool IsBornImmune(sbyte poisonType)
		{
			CharacterItem characterCfg = Config.Character.Instance.GetItem(this.TemplateId);
			return characterCfg.PoisonImmunities[(int)poisonType] || BitOperation.GetBit(this.ImmunePoisonExtra, (int)poisonType);
		}

		// Token: 0x040037E8 RID: 14312
		public Injuries Injuries;

		// Token: 0x040037E9 RID: 14313
		public int[] Poisons;

		// Token: 0x040037EA RID: 14314
		public int[] PoisonResists;

		// Token: 0x040037EB RID: 14315
		public bool[] ImmunePoisonConfig;

		// Token: 0x040037EC RID: 14316
		public byte ImmunePoisonExtra;

		// Token: 0x040037ED RID: 14317
		public short TemplateId;
	}
}
