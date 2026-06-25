using System;
using FrameWork;
using TMPro;

// Token: 0x02000298 RID: 664
public class MouseTipExtraProfessionSkill : MouseTipBase
{
	// Token: 0x17000495 RID: 1173
	// (get) Token: 0x06002A0A RID: 10762 RVA: 0x0013F20B File Offset: 0x0013D40B
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A0B RID: 10763 RVA: 0x0013F210 File Offset: 0x0013D410
	protected override void Init(ArgumentBox argsBox)
	{
		base.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_ExtraProfessionSkill_Title).ColorReplace();
		base.CGet<TextMeshProUGUI>("Desc").text = LocalStringManager.Get(LanguageKey.LK_ExtraProfessionSkill_Desc).ColorReplace();
		base.CGet<TextMeshProUGUI>("FuncDesc").text = LocalStringManager.Get(LanguageKey.LK_ExtraProfessionSkill_FuncDesc).ColorReplace();
	}
}
