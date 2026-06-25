using System;
using Config;
using FrameWork;
using TMPro;

// Token: 0x02000279 RID: 633
public class MouseTipCaravanOperation : MouseTipBase
{
	// Token: 0x06002932 RID: 10546 RVA: 0x001332C4 File Offset: 0x001314C4
	protected override void Init(ArgumentBox argsBox)
	{
		bool isInvest;
		argsBox.Get("IsInvest", out isInvest);
		sbyte resourceType;
		argsBox.Get("ResourceType", out resourceType);
		int resourceNeed;
		argsBox.Get("ResourceNeed", out resourceNeed);
		int resourceAmount;
		argsBox.Get("ResourceAmount", out resourceAmount);
		string tip;
		argsBox.Get("Tip", out tip);
		LanguageKey title = isInvest ? LanguageKey.LK_MerchantInfo_Invest : LanguageKey.LK_MerchantInfo_Protect;
		base.CGet<TextMeshProUGUI>("Title").SetText(LocalStringManager.Get(title), true);
		LanguageKey desc = isInvest ? LanguageKey.LK_MerchantInfo_Invest_Tip_Desc : LanguageKey.LK_MerchantInfo_Protect_Tip_Desc;
		base.CGet<TextMeshProUGUI>("Desc").SetText(LocalStringManager.Get(desc).ColorReplace(), true);
		LanguageKey shortTitle = isInvest ? LanguageKey.LK_MerchantInfo_Invest_Short : LanguageKey.LK_MerchantInfo_Protect_Short;
		string costStr = LocalStringManager.Get(LanguageKey.LK_Cost);
		string subTitle = LocalStringManager.GetFormat(LanguageKey.LK_SurroundWithChineseSquareBrackets, LocalStringManager.Get(shortTitle) + costStr);
		base.CGet<TextMeshProUGUI>("SubTitle").SetText(subTitle, true);
		ResourceTypeItem resourceTypeConfig = ResourceType.Instance[resourceType];
		base.CGet<CImage>("Icon").SetSprite(resourceTypeConfig.Icon, false, null);
		string color = (resourceAmount >= resourceNeed) ? "brightblue" : "brightred";
		string colonSymbol = LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
		string resourceNeedStr = CommonUtils.GetDisplayStringForNum(resourceNeed, 100000).SetColor(color);
		string resourceAmountStr = CommonUtils.GetDisplayStringForNum(resourceAmount, 100000).SetColor("pinkyellow");
		string str = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_CaravanResourceCost, new object[]
		{
			costStr,
			resourceTypeConfig.Name,
			colonSymbol,
			resourceNeedStr,
			resourceAmountStr
		});
		base.CGet<TextMeshProUGUI>("Cost").SetText(str, true);
		TextMeshProUGUI tipText = base.CGet<TextMeshProUGUI>("Tip");
		tipText.gameObject.SetActive(!tip.IsNullOrEmpty());
		tipText.SetText(tip, true);
	}
}
