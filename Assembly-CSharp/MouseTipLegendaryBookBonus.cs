using System;
using System.Collections.Generic;
using System.Text;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A7 RID: 679
public class MouseTipLegendaryBookBonus : MouseTipBase
{
	// Token: 0x1700049E RID: 1182
	// (get) Token: 0x06002A5F RID: 10847 RVA: 0x0014433E File Offset: 0x0014253E
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A60 RID: 10848 RVA: 0x00144344 File Offset: 0x00142544
	protected override void Init(ArgumentBox argsBox)
	{
		sbyte skillType;
		argsBox.Get("SkillType", out skillType);
		short bonusType;
		argsBox.Get("BonusType", out bonusType);
		short slotType;
		argsBox.Get("SlotType", out slotType);
		bool showExpCost;
		argsBox.Get("ShowExpCost", out showExpCost);
		int needExp;
		argsBox.Get("NeedExp", out needExp);
		int currExp;
		argsBox.Get("CurrExp", out currExp);
		StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
		base.CGet<TextMeshProUGUI>("Title").text = CombatSkillType.Instance[skillType].Name;
		LegendaryBookPropertyBonusTypeItem bonusConfig = LegendaryBookPropertyBonusType.Instance[bonusType];
		RectTransform addPropertyHolder = base.CGet<RectTransform>("AddPropertyHolder");
		int index = 0;
		for (int i = 0; i < bonusConfig.PropertyBonusList.Length; i++)
		{
			PropertyAndValue addProperty = bonusConfig.PropertyBonusList[i];
			index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, addProperty.PropertyId, (int)addProperty.Value, false, MouseTipLegendaryBookBonus.ShowPercentPropertyTypes.Contains(addProperty.PropertyId), false, true, false, false);
		}
		for (int j = index; j < addPropertyHolder.childCount; j++)
		{
			addPropertyHolder.GetChild(j).gameObject.SetActive(false);
		}
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
		});
		base.CGet<GameObject>("CostExp").SetActive(showExpCost);
		bool flag = showExpCost;
		if (flag)
		{
			strBuilder.Clear();
			strBuilder.Append(currExp.ToString().SetColor((needExp <= currExp) ? "brightblue" : "brightred"));
			strBuilder.Append("/");
			strBuilder.Append(needExp);
			base.CGet<TextMeshProUGUI>("Exp").text = strBuilder.ToString();
		}
		base.CGet<GameObject>("UnlockEffect").SetActive(slotType >= 0);
		bool flag2 = slotType >= 0;
		if (flag2)
		{
			LegendaryBookSlotItem slotConfig = LegendaryBookSlot.Instance[slotType];
			strBuilder.Clear();
			strBuilder.Append("<indent=15px>");
			strBuilder.Append(slotConfig.Name);
			strBuilder.Append("</indent>");
			strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
			strBuilder.Append(slotConfig.Desc.ColorReplace());
			base.CGet<TextMeshProUGUI>("EffectDesc").text = strBuilder.ToString();
		}
		EasyPool.Free<StringBuilder>(strBuilder);
	}

	// Token: 0x04001EAD RID: 7853
	private const string EffectNameLayoutBegin = "<indent=15px>";

	// Token: 0x04001EAE RID: 7854
	private const string EffectNameLayoutEnd = "</indent>";

	// Token: 0x04001EAF RID: 7855
	private static readonly List<short> ShowPercentPropertyTypes = new List<short>
	{
		6,
		7,
		8,
		9,
		10,
		11,
		12,
		13,
		14,
		15,
		16,
		17
	};
}
