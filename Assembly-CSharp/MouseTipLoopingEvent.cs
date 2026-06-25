using System;
using FrameWork;
using TMPro;

// Token: 0x020002B7 RID: 695
public class MouseTipLoopingEvent : MouseTipBase
{
	// Token: 0x170004AA RID: 1194
	// (get) Token: 0x06002ABB RID: 10939 RVA: 0x001472F9 File Offset: 0x001454F9
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002ABC RID: 10940 RVA: 0x001472FC File Offset: 0x001454FC
	protected override void Init(ArgumentBox argsBox)
	{
		TextMeshProUGUI textTitle = base.CGet<TextMeshProUGUI>("Title");
		TextMeshProUGUI textDesc = base.CGet<TextMeshProUGUI>("Desc");
		TextMeshProUGUI subHeading = base.CGet<TextMeshProUGUI>("SubHeading");
		TextMeshProUGUI combatCount = base.CGet<TextMeshProUGUI>("CombatText");
		TextMeshProUGUI lifeSkillCount = base.CGet<TextMeshProUGUI>("LifeSkillText");
		string combatCountText;
		argsBox.Get("CombatCountText", out combatCountText);
		string lifeSkillCountText;
		argsBox.Get("LifeSkillCountText", out lifeSkillCountText);
		textTitle.text = LocalStringManager.Get(LanguageKey.LK_LoopingEvent_Tips_Title);
		textDesc.text = LocalStringManager.Get(LanguageKey.LK_LoopingEvent_Tips_Desc).ColorReplace();
		subHeading.text = LocalStringManager.Get(LanguageKey.LK_LoopingEvent_Tips_SubTitle);
		combatCount.text = LocalStringManager.Get(LanguageKey.LK_LoopingEvent_Tips_SubTitle_1) + combatCountText.ColorReplace() + "/1";
		lifeSkillCount.text = LocalStringManager.Get(LanguageKey.LK_LoopingEvent_Tips_SubTitle_2) + lifeSkillCountText.ColorReplace() + "/1";
		textDesc.GetComponent<TMPTextSpriteHelper>().Parse();
	}
}
