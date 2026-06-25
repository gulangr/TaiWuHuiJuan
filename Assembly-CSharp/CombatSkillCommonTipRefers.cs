using System;
using TMPro;
using UnityEngine;

// Token: 0x02000266 RID: 614
public class CombatSkillCommonTipRefers : Refers
{
	// Token: 0x060028CD RID: 10445 RVA: 0x0012E034 File Offset: 0x0012C234
	public void SetData(string languageText, string iconName, string valueText, bool showBack)
	{
		base.CGet<TextMeshProUGUI>("Value").text = valueText;
		base.CGet<CImage>("Icon").SetSprite(iconName, false, null);
		TextMeshProUGUI tip = base.CGet<TextMeshProUGUI>("Tips");
		tip.text = languageText;
		base.CGet<GameObject>("SpecialBack").SetActive(showBack);
	}

	// Token: 0x060028CE RID: 10446 RVA: 0x0012E08F File Offset: 0x0012C28F
	public void SetValue(string valueText)
	{
		base.CGet<TextMeshProUGUI>("Value").text = valueText;
	}
}
