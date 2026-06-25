using System;
using CharacterDataMonitor;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005D7 RID: 1495
	public class CharacterHealth : CharacterUIElement
	{
		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x060046BD RID: 18109 RVA: 0x0021279B File Offset: 0x0021099B
		private AgeHealthMonitor Item
		{
			get
			{
				return this.MonitorDataItem as AgeHealthMonitor;
			}
		}

		// Token: 0x170008EC RID: 2284
		// (get) Token: 0x060046BE RID: 18110 RVA: 0x002127A8 File Offset: 0x002109A8
		public CharacterHealthBar CharacterHealthBar
		{
			get
			{
				return this._characterHealthBar;
			}
		}

		// Token: 0x170008ED RID: 2285
		// (get) Token: 0x060046BF RID: 18111 RVA: 0x002127B0 File Offset: 0x002109B0
		// (set) Token: 0x060046C0 RID: 18112 RVA: 0x002127B8 File Offset: 0x002109B8
		public bool GearMateMode { get; set; }

		// Token: 0x060046C1 RID: 18113 RVA: 0x002127C1 File Offset: 0x002109C1
		public CharacterHealth(CharacterHealthBar healthBar)
		{
			this._characterHealthBar = healthBar;
		}

		// Token: 0x060046C2 RID: 18114 RVA: 0x002127D4 File Offset: 0x002109D4
		internal override void BindEvent()
		{
			this.Item.AddOnHealthChangeEventListener(new Action(this.FillElement));
			SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DisorderOfQiMonitor>(this.Item.CharacterId, this.IsDead).AddDisorderOfQiListener(new Action(this.Item.Refresh));
			bool init = this.Item.Init;
			if (init)
			{
				this.FillElement();
			}
		}

		// Token: 0x060046C3 RID: 18115 RVA: 0x00212844 File Offset: 0x00210A44
		public override void UnbindEvent()
		{
			this.Item.RemoveOnHealthChangeEventListener(new Action(this.FillElement));
			SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DisorderOfQiMonitor>(this.Item.CharacterId, this.IsDead).RemoveDisorderOfQiListener(new Action(this.Item.Refresh));
		}

		// Token: 0x060046C4 RID: 18116 RVA: 0x002128A0 File Offset: 0x00210AA0
		public override void FillElement()
		{
			bool flag = null == this._characterHealthBar;
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				bool gearMateMode = this.GearMateMode;
				if (gearMateMode)
				{
					this._characterHealthBar.Refresh(this.Item.LeftMaxHealth, this.Item.LeftMaxHealth, this.Item.HealthRecovery, base.CharacterId);
				}
				else
				{
					this._characterHealthBar.Refresh(this.Item.Health, this.Item.LeftMaxHealth, this.Item.HealthRecovery, base.CharacterId);
				}
				Action onFillHealthChange = this.OnFillHealthChange;
				if (onFillHealthChange != null)
				{
					onFillHealthChange();
				}
			}
		}

		// Token: 0x060046C5 RID: 18117 RVA: 0x00212950 File Offset: 0x00210B50
		public override void ResetToEmpty()
		{
			bool flag = null == this._characterHealthBar;
			if (flag)
			{
				bool flag2 = this.MonitorDataItem != null;
				if (flag2)
				{
					this.UnbindEvent();
					this.MonitorDataItem = null;
				}
			}
			else
			{
				this._characterHealthBar.Refresh(-1, -1, 0, -1);
				Action onFillHealthChange = this.OnFillHealthChange;
				if (onFillHealthChange != null)
				{
					onFillHealthChange();
				}
			}
		}

		// Token: 0x060046C6 RID: 18118 RVA: 0x002129B0 File Offset: 0x00210BB0
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AgeHealthMonitor>(charId, this.IsDead);
		}

		// Token: 0x060046C7 RID: 18119 RVA: 0x002129D3 File Offset: 0x00210BD3
		public void SetGetHealthStringFunc(Func<short[], int, string> func)
		{
			this._characterHealthBar.GetHealthString = func;
		}

		// Token: 0x060046C8 RID: 18120 RVA: 0x002129E4 File Offset: 0x00210BE4
		public bool CheckMayDead()
		{
			bool flag = this.Item.LeftMaxHealth <= 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				int healthIntValue = CommonUtils.Health2Age(this.Item.Health);
				int leftHealthIntValue = CommonUtils.Health2Age(this.Item.LeftMaxHealth);
				result = ((float)healthIntValue <= (float)leftHealthIntValue * 0.25f);
			}
			return result;
		}

		// Token: 0x040030FB RID: 12539
		private readonly CharacterHealthBar _characterHealthBar;

		// Token: 0x040030FC RID: 12540
		public Action OnFillHealthChange;
	}
}
