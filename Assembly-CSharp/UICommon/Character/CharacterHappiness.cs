using System;
using CharacterDataMonitor;
using GameData.Domains.Character;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005D6 RID: 1494
	public class CharacterHappiness : CharacterUIElement
	{
		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x060046B5 RID: 18101 RVA: 0x002124FF File Offset: 0x002106FF
		private DetailInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as DetailInfoMonitor;
			}
		}

		// Token: 0x060046B6 RID: 18102 RVA: 0x0021250C File Offset: 0x0021070C
		public CharacterHappiness(Refers refers, bool tipType = false)
		{
			bool flag = null == refers;
			if (flag)
			{
				throw new Exception("Can not create CharacterHappiness from null Refers!");
			}
			this._tipType = tipType;
			this._infoItem = new InfoItem(refers);
			string infoName = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness);
			string infoDesc = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness_TipContent);
			this._infoItem.SetInfoName(infoName);
			this._infoItem.SetIcon(string.Empty);
			TooltipInvoker mouseTip = this._infoItem.GetMouseTip();
			bool flag2 = mouseTip != null;
			if (flag2)
			{
				mouseTip.Type = TipType.Simple;
				mouseTip.enabled = true;
				mouseTip.IsLanguageKey = false;
				mouseTip.PresetParam = new string[]
				{
					infoName,
					infoDesc
				};
			}
		}

		// Token: 0x060046B7 RID: 18103 RVA: 0x002125D1 File Offset: 0x002107D1
		internal override void BindEvent()
		{
			this.Item.AddOnHappinessListener(new Action(this.FillElement));
		}

		// Token: 0x060046B8 RID: 18104 RVA: 0x002125ED File Offset: 0x002107ED
		public override void UnbindEvent()
		{
			this.Item.RemoveOnHappinessListener(new Action(this.FillElement));
		}

		// Token: 0x060046B9 RID: 18105 RVA: 0x0021260C File Offset: 0x0021080C
		public override void FillElement()
		{
			bool flag = !this._infoItem.HasValidElement();
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				bool flag2 = this.Item == null || !this.Item.Init;
				if (!flag2)
				{
					sbyte happinessLevel = HappinessType.GetHappinessType(this.Item.Happiness);
					this._infoItem.SetIcon(this.UseNewIcon ? ("ui9_icon_happiness_big_" + happinessLevel.ToString()) : CommonUtils.GetHappinessIconLegacy(happinessLevel));
					this._infoItem.SetInfoValue(CommonUtils.GetHappinessString(happinessLevel));
					this._infoItem.SetInfoName(LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness));
					bool tipType = this._tipType;
					if (tipType)
					{
						this._infoItem.GetMouseTip().PresetParam[1] = CommonUtils.GetHappinessString(happinessLevel);
					}
				}
			}
		}

		// Token: 0x060046BA RID: 18106 RVA: 0x002126E4 File Offset: 0x002108E4
		public override void ResetToEmpty()
		{
			bool flag = !this._infoItem.HasValidElement();
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
				this._infoItem.SetIcon(string.Empty);
				this._infoItem.SetInfoValue(string.Empty);
			}
		}

		// Token: 0x060046BB RID: 18107 RVA: 0x00212748 File Offset: 0x00210948
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x060046BC RID: 18108 RVA: 0x0021276B File Offset: 0x0021096B
		public void SetNoData()
		{
			this._infoItem.SetIcon(CommonUtils.GetHappinessIconLegacy(-1));
			this._infoItem.SetInfoValue("-".SetColor("white"));
		}

		// Token: 0x040030F8 RID: 12536
		private bool _tipType = false;

		// Token: 0x040030F9 RID: 12537
		private readonly InfoItem _infoItem;

		// Token: 0x040030FA RID: 12538
		public bool UseNewIcon = false;
	}
}
