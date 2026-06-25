using System;
using System.Collections.Generic;
using GameData.Domains.Character;
using GameData.GameDataBridge;

namespace CharacterDataMonitor
{
	// Token: 0x020006C5 RID: 1733
	public abstract class MonitorDataItemBase
	{
		// Token: 0x17000A2B RID: 2603
		// (get) Token: 0x06005273 RID: 21107 RVA: 0x00261FC8 File Offset: 0x002601C8
		// (set) Token: 0x06005274 RID: 21108 RVA: 0x00261FD0 File Offset: 0x002601D0
		public CharacterMonitor Character
		{
			get
			{
				return this._character;
			}
			set
			{
				this._character = value;
				bool flag = !value.IsDead && this._character.CharacterId >= 0;
				if (flag)
				{
					this.MonitorData();
				}
			}
		}

		// Token: 0x17000A2C RID: 2604
		// (get) Token: 0x06005275 RID: 21109 RVA: 0x0026200C File Offset: 0x0026020C
		public bool IsTaiwu
		{
			get
			{
				return this.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000A2D RID: 2605
		// (get) Token: 0x06005276 RID: 21110 RVA: 0x00262020 File Offset: 0x00260220
		protected virtual bool IsPureFieldMonitor
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000A2E RID: 2606
		// (get) Token: 0x06005277 RID: 21111 RVA: 0x00262023 File Offset: 0x00260223
		protected ushort DomainId
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000A2F RID: 2607
		// (get) Token: 0x06005278 RID: 21112 RVA: 0x00262026 File Offset: 0x00260226
		protected ushort DataId
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000A30 RID: 2608
		// (get) Token: 0x06005279 RID: 21113 RVA: 0x00262029 File Offset: 0x00260229
		public bool Init
		{
			get
			{
				return this.DataFlag == 2;
			}
		}

		// Token: 0x17000A31 RID: 2609
		// (get) Token: 0x0600527A RID: 21114 RVA: 0x00262034 File Offset: 0x00260234
		public int CharacterId
		{
			get
			{
				return this.Character.CharacterId;
			}
		}

		// Token: 0x17000A32 RID: 2610
		// (get) Token: 0x0600527B RID: 21115 RVA: 0x00262041 File Offset: 0x00260241
		public ulong CharId
		{
			get
			{
				return (ulong)((long)this.CharacterId);
			}
		}

		// Token: 0x17000A33 RID: 2611
		// (get) Token: 0x0600527C RID: 21116 RVA: 0x0026204A File Offset: 0x0026024A
		protected int ListenerId
		{
			get
			{
				return this.Character.ListenerId;
			}
		}

		// Token: 0x17000A34 RID: 2612
		// (get) Token: 0x0600527D RID: 21117 RVA: 0x00262057 File Offset: 0x00260257
		public virtual List<uint> RelativeFieldIds { get; } = null;

		// Token: 0x0600527E RID: 21118
		protected abstract void MonitorData();

		// Token: 0x0600527F RID: 21119
		public abstract void UnMonitorData();

		// Token: 0x06005280 RID: 21120
		public abstract void OnDataInit();

		// Token: 0x06005281 RID: 21121
		protected abstract bool IsValidMonitor();

		// Token: 0x06005282 RID: 21122
		public abstract void OnNotifyData(NotificationWrapper wrapper);

		// Token: 0x06005283 RID: 21123 RVA: 0x0026205F File Offset: 0x0026025F
		public virtual void InitFromDeadCharacter(DeadCharacter deadCharacter)
		{
		}

		// Token: 0x06005284 RID: 21124 RVA: 0x00262064 File Offset: 0x00260264
		public virtual void Refresh()
		{
			bool isPureFieldMonitor = this.IsPureFieldMonitor;
			if (isPureFieldMonitor)
			{
				bool init = this.Init;
				if (init)
				{
					this.OnDataInit();
				}
			}
		}

		// Token: 0x06005285 RID: 21125 RVA: 0x00262090 File Offset: 0x00260290
		protected void OnChangeEventRemoved()
		{
			bool flag = !this.IsValidMonitor();
			if (flag)
			{
				this.Character.RemoveMonitorDataItem(base.GetType());
			}
		}

		// Token: 0x06005286 RID: 21126 RVA: 0x002620BF File Offset: 0x002602BF
		public void OnAlive()
		{
			this.UnMonitorData();
			this.MonitorData();
		}

		// Token: 0x040037F8 RID: 14328
		private CharacterMonitor _character;

		// Token: 0x040037F9 RID: 14329
		public sbyte DataFlag = 0;
	}
}
