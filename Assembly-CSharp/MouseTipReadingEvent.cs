using System;
using FrameWork;
using TMPro;

// Token: 0x020002C9 RID: 713
public class MouseTipReadingEvent : MouseTipBase
{
	// Token: 0x170004B5 RID: 1205
	// (get) Token: 0x06002B0E RID: 11022 RVA: 0x0014E3CE File Offset: 0x0014C5CE
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B0F RID: 11023 RVA: 0x0014E3D4 File Offset: 0x0014C5D4
	protected override void Init(ArgumentBox argsBox)
	{
		TextMeshProUGUI textTitle = base.CGet<TextMeshProUGUI>("Title");
		TextMeshProUGUI textDesc = base.CGet<TextMeshProUGUI>("Desc");
		TextMeshProUGUI subHeading = base.CGet<TextMeshProUGUI>("SubHeading");
		TextMeshProUGUI combatCount = base.CGet<TextMeshProUGUI>("CombatText");
		TextMeshProUGUI lifeSkillCount = base.CGet<TextMeshProUGUI>("LifeSkillText");
		string title;
		argsBox.Get("Title", out title);
		string content;
		argsBox.Get("Content", out content);
		string subHeadingText;
		argsBox.Get("SubHeadingText", out subHeadingText);
		string combatCountText;
		argsBox.Get("CombatCountText", out combatCountText);
		string lifeSkillCountText;
		argsBox.Get("LifeSkillCountText", out lifeSkillCountText);
		textTitle.text = title;
		textDesc.text = content.ColorReplace();
		subHeading.text = subHeadingText;
		combatCount.text = combatCountText;
		lifeSkillCount.text = lifeSkillCountText;
		textDesc.GetComponent<TMPTextSpriteHelper>().Parse();
	}
}
