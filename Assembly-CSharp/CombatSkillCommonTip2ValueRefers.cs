using System;
using TMPro;
using UnityEngine;

// Token: 0x02000265 RID: 613
public class CombatSkillCommonTip2ValueRefers : Refers
{
	// Token: 0x060028C7 RID: 10439 RVA: 0x0012DE79 File Offset: 0x0012C079
	public void DefaultShow()
	{
		base.CGet<GameObject>("Element1").SetActive(false);
		base.CGet<GameObject>("Element2").SetActive(false);
	}

	// Token: 0x060028C8 RID: 10440 RVA: 0x0012DEA0 File Offset: 0x0012C0A0
	public void ShowElement(CombatSkillCommonTip2ValueRefers.ElementType elementType, string languageKey, string iconName, string valueText)
	{
		TextLanguage tip = base.CGet<TextLanguage>("Tips");
		tip.Key = languageKey;
		tip.SetLanguage();
		this.ShowElement(elementType, iconName, valueText);
	}

	// Token: 0x060028C9 RID: 10441 RVA: 0x0012DED4 File Offset: 0x0012C0D4
	public void ShowElementWithoutIcon(CombatSkillCommonTip2ValueRefers.ElementType elementType, string languageKey, string valueText)
	{
		TextLanguage tip = base.CGet<TextLanguage>("Tips");
		tip.Key = languageKey;
		tip.SetLanguage();
		this.ShowElement(elementType, "", valueText);
	}

	// Token: 0x060028CA RID: 10442 RVA: 0x0012DF0C File Offset: 0x0012C10C
	private void ShowElement(CombatSkillCommonTip2ValueRefers.ElementType elementType, string iconName, string valueText)
	{
		if (elementType != CombatSkillCommonTip2ValueRefers.ElementType.One)
		{
			if (elementType == CombatSkillCommonTip2ValueRefers.ElementType.Two)
			{
				base.CGet<GameObject>("Element2").SetActive(true);
				base.CGet<TextMeshProUGUI>("Value2").text = valueText;
				base.CGet<CImage>("Icon2").SetSprite(iconName, false, null);
				base.CGet<CImage>("Icon2").gameObject.SetActive(iconName != "");
			}
		}
		else
		{
			base.CGet<GameObject>("Element1").SetActive(true);
			base.CGet<TextMeshProUGUI>("Value1").text = valueText;
			base.CGet<CImage>("Icon1").SetSprite(iconName, false, null);
			base.CGet<CImage>("Icon1").gameObject.SetActive(iconName != "");
		}
	}

	// Token: 0x060028CB RID: 10443 RVA: 0x0012DFE4 File Offset: 0x0012C1E4
	public void SetValue(CombatSkillCommonTip2ValueRefers.ElementType elementType, string valueText)
	{
		if (elementType != CombatSkillCommonTip2ValueRefers.ElementType.One)
		{
			if (elementType == CombatSkillCommonTip2ValueRefers.ElementType.Two)
			{
				base.CGet<TextMeshProUGUI>("Value2").text = valueText;
			}
		}
		else
		{
			base.CGet<TextMeshProUGUI>("Value1").text = valueText;
		}
	}

	// Token: 0x020015EB RID: 5611
	public enum ElementType
	{
		// Token: 0x0400A66B RID: 42603
		One,
		// Token: 0x0400A66C RID: 42604
		Two
	}
}
