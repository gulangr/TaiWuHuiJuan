using System;
using CharacterDataMonitor;
using TMPro;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005D5 RID: 1493
	public class CharacterGender : CharacterUIElement
	{
		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x060046AD RID: 18093 RVA: 0x00212229 File Offset: 0x00210429
		private BasicInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as BasicInfoMonitor;
			}
		}

		// Token: 0x060046AE RID: 18094 RVA: 0x00212238 File Offset: 0x00210438
		public CharacterGender(CImage icon, TextMeshProUGUI label, TooltipInvoker mouseTip = null)
		{
			bool flag = null == icon && null == label;
			if (flag)
			{
				throw new Exception("can not handle CharacterGender for null elements");
			}
			this._infoItem = new InfoItem(icon, null, label, null, mouseTip);
			this._infoItem.SetIcon(string.Empty);
			this._infoItem.SetInfoValue(string.Empty);
			bool flag2 = null != mouseTip;
			if (flag2)
			{
				mouseTip.enabled = true;
				mouseTip.Type = TipType.Simple;
				mouseTip.IsLanguageKey = false;
				mouseTip.PresetParam = new string[]
				{
					LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Gender),
					LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Gender_TipContent)
				};
			}
		}

		// Token: 0x060046AF RID: 18095 RVA: 0x002122EC File Offset: 0x002104EC
		public CharacterGender(Refers refers)
		{
			bool flag = null == refers;
			if (flag)
			{
				throw new Exception("can not handle CharacterGender for null Refers");
			}
			this._infoItem = new InfoItem(refers);
			this._infoItem.SetIcon(string.Empty);
			this._infoItem.SetInfoName(LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Gender));
			this._infoItem.SetInfoValue(string.Empty);
			TooltipInvoker mouseTip = this._infoItem.GetMouseTip();
			bool flag2 = null != mouseTip;
			if (flag2)
			{
				mouseTip.enabled = true;
				mouseTip.Type = TipType.Simple;
				mouseTip.IsLanguageKey = false;
				mouseTip.PresetParam = new string[]
				{
					LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Gender),
					LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Gender_TipContent)
				};
			}
		}

		// Token: 0x060046B0 RID: 18096 RVA: 0x002123B0 File Offset: 0x002105B0
		internal override void BindEvent()
		{
			this.Item.AddGenderListener(new Action(this.FillElement));
		}

		// Token: 0x060046B1 RID: 18097 RVA: 0x002123CC File Offset: 0x002105CC
		public override void UnbindEvent()
		{
			this.Item.RemoveGenderListener(new Action(this.FillElement));
		}

		// Token: 0x060046B2 RID: 18098 RVA: 0x002123E8 File Offset: 0x002105E8
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
					CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(this.Item.Gender, this.Item.NameRelatedData.CharTemplateId);
					this._infoItem.SetIcon(CommonUtils.GetGenderIcon(displayGender));
					this._infoItem.SetInfoValue(CommonUtils.GetGenderString(displayGender));
				}
			}
		}

		// Token: 0x060046B3 RID: 18099 RVA: 0x00212478 File Offset: 0x00210678
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

		// Token: 0x060046B4 RID: 18100 RVA: 0x002124DC File Offset: 0x002106DC
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x040030F7 RID: 12535
		private readonly InfoItem _infoItem;
	}
}
