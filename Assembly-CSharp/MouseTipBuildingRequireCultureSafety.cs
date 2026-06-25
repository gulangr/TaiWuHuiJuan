using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Organization.Display;
using TMPro;
using UnityEngine;

// Token: 0x020003FA RID: 1018
public class MouseTipBuildingRequireCultureSafety : MouseTipItem
{
	// Token: 0x17000634 RID: 1588
	// (get) Token: 0x06003D07 RID: 15623 RVA: 0x001EAE5F File Offset: 0x001E905F
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003D08 RID: 15624 RVA: 0x001EAE64 File Offset: 0x001E9064
	protected override void Init(ArgumentBox argsBox)
	{
		short templateId;
		argsBox.Get("TemplateId", out templateId);
		BuildingManageYieldTipsData tipsData;
		argsBox.Get<BuildingManageYieldTipsData>("ProduceData", out tipsData);
		BuildingBlockItem configData = BuildingBlock.Instance.GetItem(templateId);
		SettlementDisplayData[] settlements = (tipsData.SafetyOrCultureFactorSettlementsAndPickValue != null) ? tipsData.SafetyOrCultureFactorSettlementsAndPickValue.Values.ToArray<SettlementDisplayData>() : Array.Empty<SettlementDisplayData>();
		int cultureSafetyAddition;
		SharedMethods.PickSafetyOrCultureFactorSettlements(configData, settlements, out cultureSafetyAddition);
		this.titleName.text = LocalStringManager.Get(LanguageKey.LK_Building_Effect_Tips_Title);
		string postfix;
		bool isCulture;
		bool isPositive;
		MouseTipBuildingRequireCultureSafety.CalcIconName(configData, out postfix, out isCulture, out isPositive);
		string icon = string.Format("<SpName=mousetip_development_{0}>", isCulture ? 1 : 0);
		string ident = icon + LocalStringManager.Get(isCulture ? LanguageKey.LK_Culture : LanguageKey.LK_Safety);
		this.effectTips.text = ("<SpName=mousetip_buildingeffect_" + postfix + ">" + LocalStringManager.GetFormat(isPositive ? LanguageKey.LK_Building_Effect_Tips_TextF_1 : LanguageKey.LK_Building_Effect_Tips_TextF_2, LocalStringManager.Get(isCulture ? LanguageKey.LK_Culture : LanguageKey.LK_Safety))).ColorReplace();
		this.effectTips.gameObject.GetOrAddComponent<TMPTextSpriteHelper>().Parse();
		this.effectTipsFull.text = LocalStringManager.GetFormat(LanguageKey.LK_Building_Effect_Tips_Text_2, ident, LocalStringManager.Get(isPositive ? LanguageKey.LK_Building_Effect_Tips_Text_Highest : LanguageKey.LK_Building_Effect_Tips_Text_Lowest));
		this.effectTipsFull.gameObject.GetOrAddComponent<TMPTextSpriteHelper>().Parse();
		this.line3.text = string.Format("{0}%", cultureSafetyAddition + 100);
		this.line4.text = LocalStringManager.Get(LanguageKey.LK_Building_Effect_Tips_Text_4);
		WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
		bool flag = tipsData.SafetyOrCultureFactorSettlementsAndPickValue != null;
		if (flag)
		{
			CommonUtils.PrepareEnoughChildren(this.holder, this.settlementTemplate, tipsData.SafetyOrCultureFactorSettlementsAndPickValue.Count, null);
			int index = 0;
			foreach (KeyValuePair<int, SettlementDisplayData> pair in tipsData.SafetyOrCultureFactorSettlementsAndPickValue)
			{
				Refers instance = this.holder.GetChild(index).GetComponent<Refers>();
				int value = SharedMethods.CalcSafetyOrCultureFactorSettlementPickValue((short)(isCulture ? configData.RequireCulture : configData.RequireSafety), isCulture ? pair.Value.Culture : pair.Value.Safety);
				MapAreaItem areaConfig = MapArea.Instance.GetItem(pair.Value.AreaTemplateId);
				MapStateItem stateConfig = MapState.Instance.GetItem(areaConfig.StateID);
				instance.CGet<TextMeshProUGUI>("AreaName").text = string.Concat(new string[]
				{
					stateConfig.Name,
					"-",
					areaConfig.Name,
					"-",
					worldMapModel.GetSettlementName(pair.Value)
				});
				instance.CGet<TextMeshProUGUI>("LeftTitle").text = LocalStringManager.Get(isCulture ? LanguageKey.LK_Culture : LanguageKey.LK_Safety);
				TMP_Text tmp_Text = instance.CGet<TextMeshProUGUI>("LeftValue");
				string text;
				if (!isCulture)
				{
					SettlementDisplayData value2 = pair.Value;
					text = value2.Safety.ToString();
				}
				else
				{
					SettlementDisplayData value2 = pair.Value;
					text = value2.Culture.ToString();
				}
				tmp_Text.text = text;
				instance.CGet<TextMeshProUGUI>("RightTitle").text = LanguageKey.LK_Building_ResourceExpand_Influence.Tr();
				instance.CGet<TextMeshProUGUI>("RightValue").text = value.ToString();
				instance.CGet<CImage>("bg").gameObject.SetActive(index % 2 != 0);
				index++;
			}
		}
	}

	// Token: 0x06003D09 RID: 15625 RVA: 0x001EB220 File Offset: 0x001E9420
	internal static void CalcIconName(BuildingBlockItem configData, out string postfix, out bool isCulture, out bool isPositive)
	{
		postfix = "2_0";
		isCulture = false;
		isPositive = false;
		sbyte requireCulture = configData.RequireCulture;
		sbyte b = requireCulture;
		if (b <= 0)
		{
			if (b >= 0)
			{
				sbyte requireSafety = configData.RequireSafety;
				sbyte b2 = requireSafety;
				if (b2 <= 0)
				{
					if (b2 < 0)
					{
						postfix = "0_1";
					}
				}
				else
				{
					postfix = "0_0";
					isPositive = true;
				}
			}
			else
			{
				postfix = "1_1";
				isCulture = true;
			}
		}
		else
		{
			postfix = "1_0";
			isCulture = true;
			isPositive = true;
		}
	}

	// Token: 0x04002BBA RID: 11194
	[SerializeField]
	private TextMeshProUGUI titleName;

	// Token: 0x04002BBB RID: 11195
	[SerializeField]
	private TextMeshProUGUI effectTips;

	// Token: 0x04002BBC RID: 11196
	[SerializeField]
	private TextMeshProUGUI effectTipsFull;

	// Token: 0x04002BBD RID: 11197
	[SerializeField]
	private TextMeshProUGUI line3;

	// Token: 0x04002BBE RID: 11198
	[SerializeField]
	private TextMeshProUGUI line4;

	// Token: 0x04002BBF RID: 11199
	[SerializeField]
	private GameObject settlementTemplate;

	// Token: 0x04002BC0 RID: 11200
	[SerializeField]
	private RectTransform holder;
}
