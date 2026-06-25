using System;
using Config;
using TMPro;

namespace UICommon
{
	// Token: 0x020005C4 RID: 1476
	public class UIItemCombatSkill : Refers
	{
		// Token: 0x0600461C RID: 17948 RVA: 0x0020DFFD File Offset: 0x0020C1FD
		private void Awake()
		{
			this.InitComponents();
		}

		// Token: 0x0600461D RID: 17949 RVA: 0x0020E008 File Offset: 0x0020C208
		private void InitComponents()
		{
			bool init = this._init;
			if (!init)
			{
				bool flag = this.Names.Contains("SkillName");
				if (flag)
				{
					this._nameLabel = base.CGet<TextMeshProUGUI>("SkillName");
				}
				bool flag2 = this.Names.Contains("SkillIcon");
				if (flag2)
				{
					this._icon = base.CGet<CImage>("SkillIcon");
				}
				bool flag3 = this.Names.Contains("MouseTip");
				if (flag3)
				{
					this._mouseTip = base.CGet<TooltipInvoker>("MouseTip");
				}
				bool flag4 = this.Names.Contains("Attainment");
				if (flag4)
				{
					this._attainmentLabel = base.CGet<TextMeshProUGUI>("Attainment");
				}
				bool flag5 = this.Names.Contains("Qualification");
				if (flag5)
				{
					this._qualificationLabel = base.CGet<TextMeshProUGUI>("Qualification");
				}
				this._init = true;
			}
		}

		// Token: 0x0600461E RID: 17950 RVA: 0x0020E0EC File Offset: 0x0020C2EC
		public void Refresh(sbyte templateId, short qualification = -32768, short attainment = -32768, ESkillIconType iconType = ESkillIconType.Sprite)
		{
			this.InitComponents();
			CombatSkillTypeItem config = CombatSkillType.Instance[templateId];
			bool flag = null != this._nameLabel;
			if (flag)
			{
				this._nameLabel.text = config.Name;
			}
			bool flag2 = null != this._icon;
			if (flag2)
			{
				bool flag3 = iconType == ESkillIconType.Sprite;
				if (flag3)
				{
					this._icon.SetSprite(config.DisplayIcon, false, null);
				}
				else
				{
					bool flag4 = iconType == ESkillIconType.Display;
					if (flag4)
					{
						this._icon.SetSprite(config.DisplayIcon, false, null);
					}
				}
			}
			bool flag5 = null != this._mouseTip;
			if (flag5)
			{
				this._mouseTip.Type = TipType.Simple;
				this._mouseTip.enabled = true;
				this._mouseTip.IsLanguageKey = false;
				this._mouseTip.PresetParam = new string[2];
				this._mouseTip.PresetParam[0] = config.Name;
				this._mouseTip.PresetParam[1] = config.Desc;
			}
			bool flag6 = qualification != short.MinValue;
			if (flag6)
			{
				this._qualificationLabel.text = string.Format("{0}", qualification);
			}
			bool flag7 = attainment != short.MinValue && this._attainmentLabel != null;
			if (flag7)
			{
				this._attainmentLabel.text = string.Format("{0}", attainment);
			}
		}

		// Token: 0x04003099 RID: 12441
		private bool _init;

		// Token: 0x0400309A RID: 12442
		private TextMeshProUGUI _nameLabel;

		// Token: 0x0400309B RID: 12443
		private CImage _icon;

		// Token: 0x0400309C RID: 12444
		private TextMeshProUGUI _attainmentLabel;

		// Token: 0x0400309D RID: 12445
		private TextMeshProUGUI _qualificationLabel;

		// Token: 0x0400309E RID: 12446
		private TooltipInvoker _mouseTip;
	}
}
