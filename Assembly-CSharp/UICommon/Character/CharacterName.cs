using System;
using CharacterDataMonitor;
using Config;
using GameData.Domains.World;
using TMPro;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005E0 RID: 1504
	public class CharacterName : CharacterUIElement
	{
		// Token: 0x170008F4 RID: 2292
		// (get) Token: 0x0600470C RID: 18188 RVA: 0x002145C3 File Offset: 0x002127C3
		private BasicInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as BasicInfoMonitor;
			}
		}

		// Token: 0x170008F5 RID: 2293
		// (get) Token: 0x0600470D RID: 18189 RVA: 0x002145D0 File Offset: 0x002127D0
		// (set) Token: 0x0600470E RID: 18190 RVA: 0x002145D8 File Offset: 0x002127D8
		public string Name { get; private set; }

		// Token: 0x0600470F RID: 18191 RVA: 0x002145E4 File Offset: 0x002127E4
		public CharacterName(TextMeshProUGUI label, TooltipInvoker mouseTip = null, TMPTextFadeAnimation animation = null)
		{
			bool flag = null == label;
			if (flag)
			{
				throw new Exception("can not handle CharacterName for null element");
			}
			this._infoItem = new InfoItem(null, null, label, animation, null);
			bool flag2 = null != mouseTip;
			if (flag2)
			{
				mouseTip.enabled = true;
				mouseTip.Type = TipType.Simple;
				mouseTip.IsLanguageKey = false;
				mouseTip.PresetParam = new string[]
				{
					LocalStringManager.Get(LanguageKey.LK_Char_Name),
					LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Char_Name_TipContent)
				};
			}
			this.Name = string.Empty;
			this._infoItem.SetInfoValue(this.Name);
		}

		// Token: 0x06004710 RID: 18192 RVA: 0x00214688 File Offset: 0x00212888
		public CharacterName(Refers refers)
		{
			bool flag = null == refers;
			if (flag)
			{
				throw new Exception("can not handle CharacterName for null refers");
			}
			TextMeshProUGUI valueLabel = null;
			bool flag2 = refers.Names.Contains("Name");
			if (flag2)
			{
				valueLabel = refers.CGet<TextMeshProUGUI>("Name");
			}
			TooltipInvoker mouseTip = null;
			bool flag3 = refers.Names.Contains("MouseTip");
			if (flag3)
			{
				mouseTip = refers.CGet<TooltipInvoker>("MouseTip");
			}
			TMPTextFadeAnimation animation = null;
			bool flag4 = refers.Names.Contains("Animation");
			if (flag4)
			{
				animation = refers.CGet<TMPTextFadeAnimation>("Animation");
			}
			this._infoItem = new InfoItem(null, null, valueLabel, animation, null);
			bool flag5 = null != mouseTip;
			if (flag5)
			{
				mouseTip.enabled = true;
				mouseTip.Type = TipType.Simple;
				mouseTip.IsLanguageKey = false;
				mouseTip.PresetParam = new string[]
				{
					LocalStringManager.Get(LanguageKey.LK_Char_Name),
					LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Char_Name_TipContent)
				};
			}
			this.Name = string.Empty;
			this._infoItem.SetInfoValue(this.Name);
		}

		// Token: 0x06004711 RID: 18193 RVA: 0x00214798 File Offset: 0x00212998
		internal override void BindEvent()
		{
			this.Item.AddNameDataListener(new Action(this.FillElement));
		}

		// Token: 0x06004712 RID: 18194 RVA: 0x002147B4 File Offset: 0x002129B4
		public override void UnbindEvent()
		{
			this.Item.RemoveNameDataListener(new Action(this.FillElement));
		}

		// Token: 0x06004713 RID: 18195 RVA: 0x002147D0 File Offset: 0x002129D0
		public override void FillElement()
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
				CharacterItem config = Character.Instance[this.Item.NameRelatedData.CharTemplateId];
				bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
				if (inGuiding)
				{
					bool flag3 = config.CreatingType != 1;
					if (flag3)
					{
						this.Name = config.Surname + config.GivenName;
						this._infoItem.SetInfoValue(this.Name);
						return;
					}
				}
				string nameResult = this.Anonymous ? (((config != null) ? config.AnonymousTitle : null) ?? ("Invalid " + this.Item.NameRelatedData.CharTemplateId.ToString())) : NameCenter.GetMonasticTitleOrDisplayName(ref this.Item.NameRelatedData, base.IsTaiwu, false);
				bool flag4 = config != null;
				if (flag4)
				{
					bool flag5 = this.Item.NameRelatedData.FullName.Type == 0 && SharedMethods.SmallVillageXiangshu((short)config.OrganizationInfo.OrgTemplateId, false);
					if (flag5)
					{
						nameResult = CommonUtils.GetXiangshuMinion0AnonymousTitle();
					}
				}
				this.Name = nameResult;
				this._infoItem.SetInfoValue(this.Name);
			}
		}

		// Token: 0x06004714 RID: 18196 RVA: 0x00214930 File Offset: 0x00212B30
		public override void ResetToEmpty()
		{
			this.Name = string.Empty;
			this._infoItem.SetInfoValue(this.Name);
		}

		// Token: 0x06004715 RID: 18197 RVA: 0x00214954 File Offset: 0x00212B54
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x0400311E RID: 12574
		private readonly InfoItem _infoItem;

		// Token: 0x04003120 RID: 12576
		public bool RealName;

		// Token: 0x04003121 RID: 12577
		public bool Anonymous;
	}
}
