using System;
using FrameWork;
using TMPro;

// Token: 0x020002A2 RID: 674
public class MouseTipGiveUpLegendaryBook : MouseTipBase
{
	// Token: 0x17000499 RID: 1177
	// (get) Token: 0x06002A2F RID: 10799 RVA: 0x00141D93 File Offset: 0x0013FF93
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A30 RID: 10800 RVA: 0x00141D96 File Offset: 0x0013FF96
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x06002A31 RID: 10801 RVA: 0x00141DA4 File Offset: 0x0013FFA4
	public override void Refresh(ArgumentBox argBox)
	{
		TextMeshProUGUI desc = base.CGet<TextMeshProUGUI>("Desc1");
		TextMeshProUGUI title = base.CGet<TextMeshProUGUI>("Title1");
		TextMeshProUGUI sub1Text = base.CGet<TextMeshProUGUI>("Sub1Text");
		TextMeshProUGUI sub2Text = base.CGet<TextMeshProUGUI>("Sub2Text");
		TextMeshProUGUI sub3Text = base.CGet<TextMeshProUGUI>("Sub3Text");
		TextMeshProUGUI desc2 = base.CGet<TextMeshProUGUI>("Desc2");
		TextMeshProUGUI desc3 = base.CGet<TextMeshProUGUI>("Desc3");
		desc.text = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_GiveUp_Tips_Desc1).ColorReplace();
		title.text = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_GiveUp_Tips_SubTitle1).ColorReplace();
		sub1Text.text = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_GiveUp_Tips_Corpse1).ColorReplace();
		sub2Text.text = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_GiveUp_Tips_Corpse2).ColorReplace();
		sub3Text.text = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_GiveUp_Tips_Corpse3).ColorReplace();
		sub1Text.GetComponent<TMPTextSpriteHelper>().Parse();
		sub2Text.GetComponent<TMPTextSpriteHelper>().Parse();
		sub3Text.GetComponent<TMPTextSpriteHelper>().Parse();
		desc2.text = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_GiveUp_Tips_Desc2).ColorReplace();
		desc3.text = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_GiveUp_Tips_Desc3).ColorReplace();
	}
}
