using System;
using System.Collections.Generic;
using GameData.Domains.Building;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

// Token: 0x02000416 RID: 1046
public class VillagerCraftPreviewPanel : Refers
{
	// Token: 0x1700065C RID: 1628
	// (get) Token: 0x06003E55 RID: 15957 RVA: 0x001F446F File Offset: 0x001F266F
	private TextMeshProUGUI ResultTip
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("ResultTip");
		}
	}

	// Token: 0x1700065D RID: 1629
	// (get) Token: 0x06003E56 RID: 15958 RVA: 0x001F447C File Offset: 0x001F267C
	private ItemScrollView PreviewItemScrollView
	{
		get
		{
			return base.CGet<ItemScrollView>("PreviewItemScrollView");
		}
	}

	// Token: 0x06003E57 RID: 15959 RVA: 0x001F4489 File Offset: 0x001F2689
	public void ShowTip(LanguageKey key)
	{
		this.ResultTip.text = LocalStringManager.Get(key).ColorReplace();
	}

	// Token: 0x06003E58 RID: 15960 RVA: 0x001F44A4 File Offset: 0x001F26A4
	public void SetProductionPool(ProductionPool productionPool, ProductionPool previewProductionPool, short itemSubType)
	{
		this._productionPool = productionPool;
		this._previewProductionPool = previewProductionPool;
		this._totalWeight = 0;
		this._previewTotalWeight = 0;
		List<ItemDisplayData> itemList = new List<ItemDisplayData>();
		bool flag = this._productionPool != null;
		if (flag)
		{
			foreach (KeyValuePair<Production, ProductionData> keyValuePair in this._productionPool.Productions)
			{
				Production production3;
				ProductionData productionData3;
				keyValuePair.Deconstruct(out production3, out productionData3);
				Production production = production3;
				ProductionData productionData = productionData3;
				short subType = ItemTemplateHelper.GetItemSubType(production.ItemType, production.TemplateId);
				bool flag2 = itemSubType >= 0 && itemSubType != subType;
				if (!flag2)
				{
					bool canProduce = productionData.CanProduce;
					if (canProduce)
					{
						this._totalWeight += productionData.Weight;
					}
					ItemDisplayData itemData = new ItemDisplayData(production.ItemType, production.TemplateId);
					itemList.Add(itemData);
				}
			}
		}
		bool flag3 = this._previewProductionPool != null;
		if (flag3)
		{
			foreach (KeyValuePair<Production, ProductionData> keyValuePair in this._previewProductionPool.Productions)
			{
				Production production3;
				ProductionData productionData3;
				keyValuePair.Deconstruct(out production3, out productionData3);
				Production production2 = production3;
				ProductionData productionData2 = productionData3;
				short subType2 = ItemTemplateHelper.GetItemSubType(production2.ItemType, production2.TemplateId);
				bool flag4 = itemSubType >= 0 && itemSubType != subType2;
				if (!flag4)
				{
					bool canProduce2 = productionData2.CanProduce;
					if (canProduce2)
					{
						this._previewTotalWeight += productionData2.Weight;
					}
				}
			}
		}
		itemList.Sort(delegate(ItemDisplayData itemDataA, ItemDisplayData itemDataB)
		{
			Production productionA = new Production(itemDataA.Key.ItemType, itemDataA.Key.TemplateId);
			ProductionPool previewProductionPool2 = this._previewProductionPool;
			ProductionData productionDataA = (previewProductionPool2 != null) ? previewProductionPool2.Productions[productionA] : this._productionPool.Productions[productionA];
			Production productionB = new Production(itemDataB.Key.ItemType, itemDataB.Key.TemplateId);
			ProductionPool previewProductionPool3 = this._previewProductionPool;
			ProductionData productionDataB = (previewProductionPool3 != null) ? previewProductionPool3.Productions[productionB] : this._productionPool.Productions[productionB];
			bool flag5 = productionDataA.CanProduce != productionDataB.CanProduce;
			int result;
			if (flag5)
			{
				result = productionDataB.CanProduce.CompareTo(productionDataA.CanProduce);
			}
			else
			{
				bool flag6 = productionDataA.Weight != productionDataB.Weight;
				if (flag6)
				{
					result = productionDataB.Weight.CompareTo(productionDataA.Weight);
				}
				else
				{
					sbyte gradeA = ItemTemplateHelper.GetGrade(itemDataA.Key.ItemType, itemDataA.Key.TemplateId);
					sbyte gradeB = ItemTemplateHelper.GetGrade(itemDataB.Key.ItemType, itemDataB.Key.TemplateId);
					bool flag7 = gradeA != gradeB;
					if (flag7)
					{
						result = gradeB.CompareTo(gradeA);
					}
					else
					{
						result = itemDataA.Key.TemplateId.CompareTo(itemDataB.Key.TemplateId);
					}
				}
			}
			return result;
		});
		this.PreviewItemScrollView.Init();
		this.PreviewItemScrollView.MySortAndFilter.SortEnabled = false;
		this.PreviewItemScrollView.SetItemList(ref itemList, true, "VillagerCraftPreviewPanel", true, new Action<ItemDisplayData, ItemView>(this.OnRenderItem));
	}

	// Token: 0x06003E59 RID: 15961 RVA: 0x001F46C4 File Offset: 0x001F28C4
	private void OnRenderItem(ItemDisplayData itemData, ItemView itemView)
	{
		TextMeshProUGUI attainmentText = itemView.transform.Find("Attainment").GetComponent<TextMeshProUGUI>();
		TextMeshProUGUI chanceLabel = itemView.transform.Find("Chance").GetComponent<TextMeshProUGUI>();
		Production production = new Production(itemData.Key.ItemType, itemData.Key.TemplateId);
		ProductionData productionData = this._productionPool.Productions[production];
		int weight = productionData.Weight;
		Transform upObj = itemView.transform.Find("Up");
		Transform downObj = itemView.transform.Find("Down");
		bool isUp = false;
		bool isDown = false;
		int totalWeight = this._totalWeight;
		bool canProduce = productionData.CanProduce;
		float realChance;
		int chance = this.CalcChance(weight, totalWeight, out realChance);
		bool flag = canProduce && this._previewProductionPool != null;
		if (flag)
		{
			ProductionData previewProductionData = this._previewProductionPool.Productions[production];
			int previewWeight = previewProductionData.Weight;
			float previewRealChance;
			chance = this.CalcChance(previewWeight, this._previewTotalWeight, out previewRealChance);
			isUp = (previewRealChance > realChance);
			isDown = (previewRealChance < realChance);
		}
		upObj.gameObject.SetActive(isUp);
		downObj.gameObject.SetActive(isDown);
		string chanceColor = isUp ? "brightblue" : (isDown ? "brightred" : "pinkyellow");
		string chanceText = string.Format("{0}%", chance).SetColor(chanceColor);
		chanceLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_VillagerCraftPreviewPanel_Chance, chanceText).ColorReplace();
		bool notMeet = chance == 0 || !canProduce;
		bool flag2 = notMeet;
		if (flag2)
		{
			itemView.SetItemNotCanSelectReason(LocalStringManager.Get(LanguageKey.LK_VillagerCraftPreviewPanel_CannotProduce).SetColor("brightred"));
			itemView.SetInteractable(false);
		}
		else
		{
			itemView.HideInteractionState();
		}
	}

	// Token: 0x06003E5A RID: 15962 RVA: 0x001F488C File Offset: 0x001F2A8C
	private int CalcChance(int weight, int totalWeight, out float realChance)
	{
		realChance = ((totalWeight == 0) ? 0f : ((float)weight / (float)totalWeight));
		int chance = (totalWeight == 0) ? 0 : (weight * 100 / totalWeight);
		float num = realChance;
		bool flag = num > 0f && num < 0.01f;
		if (flag)
		{
			chance = 1;
		}
		return chance;
	}

	// Token: 0x04002CFB RID: 11515
	private ProductionPool _productionPool;

	// Token: 0x04002CFC RID: 11516
	private ProductionPool _previewProductionPool;

	// Token: 0x04002CFD RID: 11517
	private int _totalWeight;

	// Token: 0x04002CFE RID: 11518
	private int _previewTotalWeight;
}
