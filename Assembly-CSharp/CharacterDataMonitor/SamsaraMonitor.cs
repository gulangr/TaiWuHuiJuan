using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.GameDataBridge;
using GameData.Serializer;

namespace CharacterDataMonitor
{
	// Token: 0x020006C9 RID: 1737
	[Obsolete("静态监听已经废弃，改为调方法拉取")]
	public class SamsaraMonitor : MonitorDataItemBase
	{
		// Token: 0x1400006C RID: 108
		// (add) Token: 0x060052AC RID: 21164 RVA: 0x00262978 File Offset: 0x00260B78
		// (remove) Token: 0x060052AD RID: 21165 RVA: 0x002629B0 File Offset: 0x00260BB0
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnSamsaraChangeEvent;

		// Token: 0x1400006D RID: 109
		// (add) Token: 0x060052AE RID: 21166 RVA: 0x002629E8 File Offset: 0x00260BE8
		// (remove) Token: 0x060052AF RID: 21167 RVA: 0x00262A20 File Offset: 0x00260C20
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int> OnCharSamsaraChangeEvent;

		// Token: 0x17000A3B RID: 2619
		// (get) Token: 0x060052B0 RID: 21168 RVA: 0x00262A55 File Offset: 0x00260C55
		public DeadCharacter[] PreLifeCharacters { get; }

		// Token: 0x17000A3C RID: 2620
		// (get) Token: 0x060052B1 RID: 21169 RVA: 0x00262A5D File Offset: 0x00260C5D
		// (set) Token: 0x060052B2 RID: 21170 RVA: 0x00262A65 File Offset: 0x00260C65
		public sbyte Count { get; private set; }

		// Token: 0x17000A3D RID: 2621
		// (get) Token: 0x060052B3 RID: 21171 RVA: 0x00262A6E File Offset: 0x00260C6E
		public bool IsRanchenzi
		{
			get
			{
				return this._templateId == 916;
			}
		}

		// Token: 0x17000A3E RID: 2622
		// (get) Token: 0x060052B4 RID: 21172 RVA: 0x00262A7D File Offset: 0x00260C7D
		protected override bool IsPureFieldMonitor
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000A3F RID: 2623
		// (get) Token: 0x060052B5 RID: 21173 RVA: 0x00262A80 File Offset: 0x00260C80
		public override List<uint> RelativeFieldIds
		{
			get
			{
				return new List<uint>
				{
					1U
				};
			}
		}

		// Token: 0x060052B6 RID: 21174 RVA: 0x00262A8F File Offset: 0x00260C8F
		public SamsaraMonitor()
		{
			this.PreLifeCharacters = new DeadCharacter[9];
			this.PreexistenceCharIds = default(PreexistenceCharIds);
		}

		// Token: 0x060052B7 RID: 21175 RVA: 0x00262AB4 File Offset: 0x00260CB4
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

		// Token: 0x060052B8 RID: 21176 RVA: 0x00262B38 File Offset: 0x00260D38
		public override void Refresh()
		{
			bool isDead = base.Character.IsDead;
			if (!isDead)
			{
				base.Character.CallMethod(55);
			}
		}

		// Token: 0x060052B9 RID: 21177 RVA: 0x00262B68 File Offset: 0x00260D68
		public override void UnMonitorData()
		{
			foreach (uint fieldId in this.RelativeFieldIds)
			{
				GameDataBridge.AddDataUnMonitor(base.ListenerId, base.DomainId, base.DataId, base.CharId, fieldId);
			}
		}

		// Token: 0x060052BA RID: 21178 RVA: 0x00262BD8 File Offset: 0x00260DD8
		public override void OnDataInit()
		{
			Action onSamsaraChangeEvent = this.OnSamsaraChangeEvent;
			if (onSamsaraChangeEvent != null)
			{
				onSamsaraChangeEvent();
			}
			Action<int> onCharSamsaraChangeEvent = this.OnCharSamsaraChangeEvent;
			if (onCharSamsaraChangeEvent != null)
			{
				onCharSamsaraChangeEvent(base.CharacterId);
			}
		}

		// Token: 0x060052BB RID: 21179 RVA: 0x00262C08 File Offset: 0x00260E08
		protected override bool IsValidMonitor()
		{
			Action onSamsaraChangeEvent = this.OnSamsaraChangeEvent;
			bool result;
			if (onSamsaraChangeEvent == null || onSamsaraChangeEvent.GetInvocationList().Length == 0)
			{
				Action<int> onCharSamsaraChangeEvent = this.OnCharSamsaraChangeEvent;
				result = (onCharSamsaraChangeEvent != null && onCharSamsaraChangeEvent.GetInvocationList().Length != 0);
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060052BC RID: 21180 RVA: 0x00262C4C File Offset: 0x00260E4C
		public override void OnNotifyData(NotificationWrapper wrapper)
		{
			bool flag = wrapper.Notification.Type == 0;
			if (flag)
			{
				bool flag2 = wrapper.Notification.Uid.SubId1 == 1U;
				if (flag2)
				{
					this._templateId = -1;
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref this._templateId);
					bool isRanchenzi = this.IsRanchenzi;
					if (isRanchenzi)
					{
						bool flag3 = !base.Init;
						if (flag3)
						{
							this.DataFlag = 2;
						}
						this.OnDataInit();
					}
					else
					{
						base.Character.CallMethod(55);
					}
				}
			}
			else
			{
				bool flag4 = wrapper.Notification.MethodId == 55;
				if (flag4)
				{
					CharacterSamsaraData characterSamsaraData = null;
					Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref characterSamsaraData);
					for (int i = 0; i < 9; i++)
					{
						this.PreLifeCharacters[i] = null;
					}
					List<DeadCharacter> deadCharacters = characterSamsaraData.DeadCharacters;
					this.Count = (sbyte)((deadCharacters != null) ? deadCharacters.Count : 0);
					this.PreexistenceCharIds = characterSamsaraData.PreexistenceCharIds;
					for (int j = 0; j < (int)this.Count; j++)
					{
						int index = PreexistenceCharIds.Positions[j];
						this.PreLifeCharacters[index] = characterSamsaraData.DeadCharacters[j];
					}
					bool init = base.Init;
					if (init)
					{
						Action onSamsaraChangeEvent = this.OnSamsaraChangeEvent;
						if (onSamsaraChangeEvent != null)
						{
							onSamsaraChangeEvent();
						}
						Action<int> onCharSamsaraChangeEvent = this.OnCharSamsaraChangeEvent;
						if (onCharSamsaraChangeEvent != null)
						{
							onCharSamsaraChangeEvent(base.CharacterId);
						}
					}
					bool flag5 = !base.Init;
					if (flag5)
					{
						this.DataFlag = 1;
					}
				}
			}
		}

		// Token: 0x060052BD RID: 21181 RVA: 0x00262DF4 File Offset: 0x00260FF4
		public void AddSamsaraListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnSamsaraChangeEvent -= listener;
			this.OnSamsaraChangeEvent += listener;
		}

		// Token: 0x060052BE RID: 21182 RVA: 0x00262E16 File Offset: 0x00261016
		public void RemoveSamsaraListener(Action listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnSamsaraChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x060052BF RID: 21183 RVA: 0x00262E37 File Offset: 0x00261037
		public void AddSamsaraListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharSamsaraChangeEvent -= listener;
			this.OnCharSamsaraChangeEvent += listener;
		}

		// Token: 0x060052C0 RID: 21184 RVA: 0x00262E59 File Offset: 0x00261059
		public void RemoveSamsaraListener(Action<int> listener)
		{
			if (listener == null)
			{
				throw new Exception("null listener is not supported!");
			}
			this.OnCharSamsaraChangeEvent -= listener;
			base.OnChangeEventRemoved();
		}

		// Token: 0x04003807 RID: 14343
		public PreexistenceCharIds PreexistenceCharIds;

		// Token: 0x04003809 RID: 14345
		private short _templateId;
	}
}
