using System;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x020002C6 RID: 710
public class MouseTipPrisonerResistance : MouseTipBase
{
	// Token: 0x06002AF6 RID: 10998 RVA: 0x0014B6D4 File Offset: 0x001498D4
	protected override void Init(ArgumentBox argsBox)
	{
		bool isPrivate;
		argsBox.Get("IsPrivate", out isPrivate);
		int resistance;
		argsBox.Get("Resistance", out resistance);
		int escapeRate;
		argsBox.Get("EscapeRate", out escapeRate);
		int ropeEffect;
		argsBox.Get("RopeEffect", out ropeEffect);
		bool completelyInfected;
		argsBox.Get("CompletelyInfected", out completelyInfected);
		bool owningBook;
		argsBox.Get("OwningBook", out owningBook);
		RectTransform descLayout = base.CGet<RectTransform>("DescLayout");
		TextMeshProUGUI[] descArray = descLayout.GetComponentsInChildren<TextMeshProUGUI>();
		for (int index = 0; index < descArray.Length; index++)
		{
			TextMeshProUGUI text = descArray[index];
			LanguageKey baseKey = isPrivate ? LanguageKey.LK_Kidnap_ResistanceTip_Desc_Private_0 : LanguageKey.LK_Kidnap_ResistanceTip_Desc_Organization_0;
			LanguageKey key = baseKey + index;
			text.text = LocalStringManager.Get(key).ColorReplace();
		}
		string resistanceColor = (resistance > 100) ? "brightred" : "brightblue";
		string resistanceText = resistance.ToString().SetColor(resistanceColor);
		base.CGet<TextMeshProUGUI>("Resistance").text = LocalStringManager.GetFormat(LanguageKey.LK_Kidnap_ResistanceTip_State_Resistance, resistanceText).ColorReplace();
		base.CGet<TextMeshProUGUI>("EscapeRate").text = LocalStringManager.GetFormat(LanguageKey.LK_Kidnap_ResistanceTip_State_EscapeRate, escapeRate).ColorReplace();
		base.CGet<TextMeshProUGUI>("RopeEffect").text = LocalStringManager.GetFormat(LanguageKey.LK_Kidnap_ResistanceTip_State_RopeEffect, ropeEffect).ColorReplace();
		base.CGet<GameObject>("SpecialLayout").SetActive(completelyInfected || owningBook);
		base.CGet<TextMeshProUGUI>("Special").text = (completelyInfected ? LanguageKey.LK_Kidnap_ResistanceTip_State_CompletelyInfected.Tr().ColorReplace() : LanguageKey.LK_Kidnap_ResistanceTip_State_OwningBook.Tr().ColorReplace());
	}
}
