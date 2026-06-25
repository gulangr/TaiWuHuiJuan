using System;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x0200028E RID: 654
public class MouseTipDemonSlayer : MouseTipBase
{
	// Token: 0x1700048E RID: 1166
	// (get) Token: 0x060029DB RID: 10715 RVA: 0x0013D684 File Offset: 0x0013B884
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060029DC RID: 10716 RVA: 0x0013D688 File Offset: 0x0013B888
	protected override void Init(ArgumentBox argsBox)
	{
		string helpTip = LocalStringManager.Get(LanguageKey.LK_SectShaolinDemonSlayer_HelpTips);
		string[] tips = helpTip.Split(';', StringSplitOptions.None);
		RectTransform tipsHolder = base.CGet<RectTransform>("TipsHolder");
		Refers[] tipsRefer = tipsHolder.GetComponentsInChildren<Refers>(true);
		for (int i = 0; i < tips.Length; i++)
		{
			string tip = tips[i];
			Refers refer = tipsRefer[i];
			refer.gameObject.SetActive(true);
			refer.CGet<TextMeshProUGUI>("TipText").text = tip.ColorReplace();
		}
	}
}
