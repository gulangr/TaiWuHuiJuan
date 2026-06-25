using System;
using Config;
using TMPro;

namespace UICommon
{
	// Token: 0x020005C5 RID: 1477
	public class UIItemLifeSkill : Refers
	{
		// Token: 0x06004620 RID: 17952 RVA: 0x0020E25E File Offset: 0x0020C45E
		private void Awake()
		{
			this.InitComponents();
		}

		// Token: 0x06004621 RID: 17953 RVA: 0x0020E268 File Offset: 0x0020C468
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

		// Token: 0x06004622 RID: 17954 RVA: 0x0020E34C File Offset: 0x0020C54C
		public void Refresh(sbyte templateId, short qualification = -32768, short attainment = -32768, ESkillIconType iconType = ESkillIconType.Sprite)
		{
			this.InitComponents();
			LifeSkillTypeItem config = LifeSkillType.Instance[templateId];
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

		// Token: 0x0400309F RID: 12447
		private bool _init;

		// Token: 0x040030A0 RID: 12448
		private TextMeshProUGUI _nameLabel;

		// Token: 0x040030A1 RID: 12449
		private CImage _icon;

		// Token: 0x040030A2 RID: 12450
		private TextMeshProUGUI _attainmentLabel;

		// Token: 0x040030A3 RID: 12451
		private TextMeshProUGUI _qualificationLabel;

		// Token: 0x040030A4 RID: 12452
		private TooltipInvoker _mouseTip;
	}
}
