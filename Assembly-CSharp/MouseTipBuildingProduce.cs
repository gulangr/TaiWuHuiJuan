using System;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Building;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;

// Token: 0x02000276 RID: 630
public class MouseTipBuildingProduce : MouseTipBase
{
	// Token: 0x17000481 RID: 1153
	// (get) Token: 0x06002925 RID: 10533 RVA: 0x00132A8C File Offset: 0x00130C8C
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002926 RID: 10534 RVA: 0x00132A90 File Offset: 0x00130C90
	protected override void Init(ArgumentBox argsBox)
	{
		argsBox.Get<BuildingManageYieldTipsData>("ProduceData", out this._data);
		BuildingBlockItem buildingBlockItem = BuildingBlock.Instance[this._data.BuildingProduceDependencyData.TemplateId];
		ShopEventItem shopEventItem = ShopEvent.Instance[buildingBlockItem.SuccesEvent[0]];
		Refers line = base.CGet<Refers>("Line1");
		Refers line2 = base.CGet<Refers>("Line2");
		Refers lineProductivity = base.CGet<Refers>("LineProductivity");
		Refers lineCulture = base.CGet<Refers>("LineCulture");
		Refers lineEfficiency = base.CGet<Refers>("LineEfficiency");
		Refers lineDifficult = base.CGet<Refers>("LineDifficult");
		Refers lineRandomFactor = base.CGet<Refers>("LineRandomFactor");
		Refers lineExtra = base.CGet<Refers>("LineExtra");
		bool produceMoneyAuthority = SharedMethods.IsBuildingProduceMoneyAuthority(buildingBlockItem, shopEventItem);
		line.gameObject.SetActive(produceMoneyAuthority);
		bool flag = produceMoneyAuthority;
		if (flag)
		{
			ResourceTypeItem resourceTypeItem = ResourceType.Instance[this._data.ProduceResourceType];
			line.CGet<TextMeshProUGUI>("Count").text = resourceTypeItem.Name.SetColor("pinkyellow");
			line.CGet<CImage>("Icon").SetSprite(resourceTypeItem.Icon, false, null);
		}
		line2.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(produceMoneyAuthority ? LanguageKey.LK_Building_PredictProduct_Tips_Text_1 : LanguageKey.LK_Building_PredictProduct_Tips_Text_14);
		line2.CGet<TextMeshProUGUI>("Count").text = MouseTipBuildingProduce.CalcProduct(this._data, produceMoneyAuthority);
		string text4 = this._data.BuildingProduceDependencyData.ProductivityFactor.ToString() + "%";
		lineProductivity.CGet<TextMeshProUGUI>("Count").text = text4.SetColor("pinkyellow");
		string text5 = this._data.BuildingProduceDependencyData.TotalAttainmentFactor.ToString() + "%";
		lineEfficiency.CGet<TextMeshProUGUI>("Count").text = text5.SetColor("pinkyellow");
		int gainResourcePercentFactor = this._data.BuildingProduceDependencyData.GainResourcePercentFactor;
		string resourceStr = (gainResourcePercentFactor >= 100) ? (gainResourcePercentFactor.ToString() + "%").SetColor("brightblue") : (gainResourcePercentFactor.ToString() + "%").SetColor("brightred");
		lineDifficult.CGet<TextMeshProUGUI>("Count").text = resourceStr;
		lineRandomFactor.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.GetFormat(LanguageKey.LK_Building_PredictProduct_Tips_Text_18, string.Concat(new string[]
		{
			"  ",
			((int)this._data.BuildingProduceDependencyData.RandomFactorLowerLimit).ToString(),
			"%~",
			((int)this._data.BuildingProduceDependencyData.RandomFactorUpperLimit).ToString(),
			"%"
		}));
		lineCulture.gameObject.SetActive(SharedMethods.BuildingRequireSafetyOrCulture(buildingBlockItem));
		bool flag2 = SharedMethods.BuildingRequireSafetyOrCulture(buildingBlockItem);
		if (flag2)
		{
			int factor = this._data.BuildingProduceDependencyData.SafetyCultureFactor;
			lineCulture.CGet<TextMeshProUGUI>("Count").text = (factor.ToString() + "%").SetColor("pinkyellow");
			lineCulture.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get((buildingBlockItem.RequireCulture != 0) ? LanguageKey.LK_Building_PredictProduct_Tips_Text_10 : LanguageKey.LK_Building_PredictProduct_Tips_Text_11);
		}
		TextMeshProUGUI leaderOwnDesc = base.CGet<TextMeshProUGUI>("LeaderOwnDesc");
		leaderOwnDesc.transform.parent.gameObject.SetActive(produceMoneyAuthority);
		bool flag3 = produceMoneyAuthority;
		if (flag3)
		{
			leaderOwnDesc.SetText(LocalStringManager.GetFormat(LanguageKey.LK_Building_SharedPencent_Tip, GlobalConfig.Instance.ShopBuildingSharePencent[0], GlobalConfig.Instance.ShopBuildingSharePencent[1]).ColorReplace(), true);
		}
		sbyte resourceType = SharedMethods.GetBuildingResourceGoodsOrExchangeResourceGoodsType(buildingBlockItem, shopEventItem);
		BuildingDomainMethod.AsyncCall.GetTaiwuVillageResourceBlockEffect(this, (resourceType == 6) ? EBuildingScaleEffect.BuildingMoneyIncomeBonus : EBuildingScaleEffect.BuildingAuthorityIncomeBonus, delegate(int offset, RawDataPool dataPool)
		{
			int bonus = 0;
			Serializer.Deserialize(dataPool, offset, ref bonus);
			lineExtra.gameObject.SetActive(bonus > 0);
			bool flag4 = bonus > 0;
			if (flag4)
			{
				lineExtra.CGet<TextMeshProUGUI>("Title").SetText(LocalStringManager.GetFormat(LanguageKey.LK_Building_PredictProduct_Tips_Text_19, (resourceType == 6) ? BuildingBlock.Instance[17].Name : BuildingBlock.Instance[16].Name, 100 * bonus), true);
			}
		});
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x00132E90 File Offset: 0x00131090
	public static string CalcProduct(BuildingManageYieldTipsData data, bool produceMoneyAuthority)
	{
		StringBuilder sb = new StringBuilder();
		string result;
		if (produceMoneyAuthority)
		{
			string text2 = data.ManageProduceValuationMin.ToString() + "~" + data.ManageProduceValuationMax.ToString();
			sb.Clear();
			sb.Append(text2.SetColor("pinkyellow"));
			text2 = sb.ToString();
			result = text2;
		}
		else
		{
			sb.Clear();
			sb.Append(string.Format("{0}%", data.ManageProduceValuationMin).SetColor("pinkyellow")).Append("~").Append(string.Format("{0}%", data.ManageProduceValuationMax).SetColor("pinkyellow"));
			string text3 = sb.ToString();
			result = text3;
		}
		return result;
	}

	// Token: 0x04001DFA RID: 7674
	private BuildingManageYieldTipsData _data;
}
