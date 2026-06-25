using System;
using FrameWork;
using TMPro;

// Token: 0x02000280 RID: 640
public class MouseTipCombatBlockAttack : MouseTipBase
{
	// Token: 0x0600294F RID: 10575 RVA: 0x00134BC8 File Offset: 0x00132DC8
	protected override void Init(ArgumentBox argsBox)
	{
		TextMeshProUGUI textTitle = base.CGet<TextMeshProUGUI>("Title");
		TextMeshProUGUI textDesc = base.CGet<TextMeshProUGUI>("Desc1");
		TextMeshProUGUI textDesc2 = base.CGet<TextMeshProUGUI>("Desc2");
		TextMeshProUGUI textDesc3 = base.CGet<TextMeshProUGUI>("Desc9");
		string title;
		argsBox.Get("arg0", out title);
		string desc;
		argsBox.Get("arg1", out desc);
		string desc2;
		argsBox.Get("arg2", out desc2);
		textTitle.SetText(title, true);
		textDesc.SetText(desc, true);
		textDesc2.SetText(desc2, true);
		textDesc3.SetText(LocalStringManager.Get(LanguageKey.LK_Combat_Block_Attack_Tips6).ColorReplace(), true);
		textDesc3.GetComponent<TMPTextSpriteHelper>().Parse();
	}
}
