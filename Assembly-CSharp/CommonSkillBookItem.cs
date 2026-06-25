using System;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Utilities;
using TMPro;

// Token: 0x02000227 RID: 551
public class CommonSkillBookItem : CommonToggleState
{
	// Token: 0x06002316 RID: 8982 RVA: 0x00102C20 File Offset: 0x00100E20
	public void Refresh(GameData.Domains.Character.LifeSkillItem data, sbyte[] readingProgress)
	{
		Config.LifeSkillItem config = LifeSkill.Instance[data.SkillTemplateId];
		SkillBookItem skillBookConfig = SkillBook.Instance[config.SkillBookId];
		string grade = config.Grade.ToString();
		this.normalGradeIcon.SetSprite("ui_sp_base_goods_{0}".GetFormat(grade), false, null);
		this.hoverGradeIcon.SetSprite("ui_sp_base_goods_hover_{0}".GetFormat(grade), false, null);
		this.icon.SetSprite(skillBookConfig.Icon, false, null);
		this.bookName.text = config.Name.SetGradeColor((int)config.Grade);
		this.pageStates.Refresh(data, readingProgress);
		bool flag = this.mouseTip.enabled && readingProgress != null;
		if (flag)
		{
			this.mouseTip.Type = TipType.LifeSkillDetailReadProgress;
			this.mouseTip.RuntimeParam = new ArgumentBox().SetObject(MouseTipLifeSkillDetailReadProgress.ArgKeyBookConfig, skillBookConfig).SetObject(MouseTipLifeSkillDetailReadProgress.ArgKeyReadProgresses, readingProgress);
			bool showing = this.mouseTip.Showing;
			if (showing)
			{
				this.mouseTip.Refresh(false, -1);
			}
		}
	}

	// Token: 0x06002317 RID: 8983 RVA: 0x00102D40 File Offset: 0x00100F40
	public void Refresh(CombatSkillDisplayData data)
	{
		CombatSkillItem config = CombatSkill.Instance[data.TemplateId];
		string grade = config.Grade.ToString();
		this.normalGradeIcon.SetSprite("ui_sp_base_goods_{0}".GetFormat(grade), false, null);
		this.hoverGradeIcon.SetSprite("ui_sp_base_goods_hover_{0}".GetFormat(grade), false, null);
		this.icon.SetSprite(SkillBook.Instance[config.BookId].Icon, false, null);
		this.bookName.text = config.Name.SetGradeColor((int)config.Grade);
		this.pageStates.Refresh(data);
		bool enabled = this.mouseTip.enabled;
		if (enabled)
		{
			this.mouseTip.Type = TipType.CombatSkill;
			this.mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CombatSkillId", data.TemplateId).Set("CharId", data.CharId);
			bool showing = this.mouseTip.Showing;
			if (showing)
			{
				this.mouseTip.Refresh(false, -1);
			}
		}
	}

	// Token: 0x06002318 RID: 8984 RVA: 0x00102E58 File Offset: 0x00101058
	public override void OnStateChanged()
	{
		base.OnStateChanged();
		bool flag = base.CurrState == CommonToggleState.ToggleStates.Highlight;
		if (flag)
		{
			this.hover.SetActive(true);
		}
	}

	// Token: 0x04001AE6 RID: 6886
	public CImage normalGradeIcon;

	// Token: 0x04001AE7 RID: 6887
	public CImage hoverGradeIcon;

	// Token: 0x04001AE8 RID: 6888
	public CImage icon;

	// Token: 0x04001AE9 RID: 6889
	public TextMeshProUGUI bookName;

	// Token: 0x04001AEA RID: 6890
	public CommonPageStates pageStates;

	// Token: 0x04001AEB RID: 6891
	private const string NormalGradeIconPrefix = "ui_sp_base_goods_{0}";

	// Token: 0x04001AEC RID: 6892
	private const string HoverGradeIconPrefix = "ui_sp_base_goods_hover_{0}";
}
