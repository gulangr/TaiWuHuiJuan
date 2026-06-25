using System;
using Config;
using FrameWork;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

// Token: 0x020002BC RID: 700
public class MouseTipMakingTool : MouseTipItem
{
	// Token: 0x170004AB RID: 1195
	// (get) Token: 0x06002AC7 RID: 10951 RVA: 0x00147B5B File Offset: 0x00145D5B
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x00147B60 File Offset: 0x00145D60
	protected override void Init(ArgumentBox argsBox)
	{
		base.Init(argsBox);
		ItemDisplayData itemData;
		argsBox.Get<ItemDisplayData>("ItemData", out itemData);
		bool templateDataOnly;
		argsBox.Get("TemplateDataOnly", out templateDataOnly);
		this._itemData = itemData;
		CraftToolItem configData = CraftTool.Instance[itemData.Key.TemplateId];
		TextMeshProUGUI currDurabilityYellow = base.CGet<TextMeshProUGUI>("CurrDurabilityYellow");
		TextMeshProUGUI currDurabilityRed = base.CGet<TextMeshProUGUI>("CurrDurabilityRed");
		base.CGet<TextMeshProUGUI>("Name").text = configData.Name;
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("GradeName").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		base.CGet<TextMeshProUGUI>("Grade").text = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - configData.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
		base.CGet<TextMeshProUGUI>("Value").text = (templateDataOnly ? configData.BaseValue.ToString() : itemData.Value.ToString());
		base.CGet<GameObject>("Durability").SetActive(!templateDataOnly);
		base.CGet<GameObject>("Material").SetActive(!templateDataOnly);
		base.CGet<CImage>("ItemIcon").SetSprite(configData.Icon, false, null);
		base.SetItemDesc(configData.Desc, itemData.LoveTokenDataItem);
		bool hasHalfDurability = itemData.Durability > itemData.MaxDurability / 2;
		base.CGet<TextMeshProUGUI>("SubType").text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", configData.ItemSubType));
		base.CGet<TextMeshProUGUI>("Weight").text = NumberFormatUtils.FormatItemWeight(itemData.Weight);
		currDurabilityYellow.gameObject.SetActive(hasHalfDurability);
		currDurabilityRed.gameObject.SetActive(!hasHalfDurability);
		(hasHalfDurability ? currDurabilityYellow : currDurabilityRed).text = itemData.Durability.ToString();
		base.CGet<TextMeshProUGUI>("MaxDurability").text = string.Format("/{0}", itemData.MaxDurability);
		RectTransform addPropertyHolder = base.CGet<RectTransform>("AddPropertyHolder");
		int index = 0;
		short attainment = UI_Make.GetToolAttainment(itemData.Key.TemplateId, -1);
		foreach (sbyte lifeSkillType in configData.RequiredLifeSkillTypes)
		{
			if (!true)
			{
			}
			ECharacterPropertyReferencedType echaracterPropertyReferencedType;
			switch (lifeSkillType)
			{
			case 6:
				echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentForging;
				break;
			case 7:
				echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentWoodworking;
				break;
			case 8:
				echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentMedicine;
				break;
			case 9:
				echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentToxicology;
				break;
			case 10:
				echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentWeaving;
				break;
			case 11:
				echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentJade;
				break;
			case 12:
			case 13:
				goto IL_2EA;
			case 14:
				echaracterPropertyReferencedType = ECharacterPropertyReferencedType.AttainmentCooking;
				break;
			default:
				goto IL_2EA;
			}
			if (!true)
			{
			}
			ECharacterPropertyReferencedType referencedType = echaracterPropertyReferencedType;
			index += MouseTip_Util.AppendAddProperty(addPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, (short)referencedType, (int)attainment, false, false, false, true, false, false);
			continue;
			IL_2EA:
			throw new ArgumentOutOfRangeException();
		}
		for (int i = index; i < addPropertyHolder.childCount; i++)
		{
			addPropertyHolder.GetChild(i).gameObject.SetActive(false);
		}
		this.InitItemDisableFunctionList(itemData);
		base.RefreshDisableFunction();
		base.RefreshHoldCount();
		base.RefreshHotkeyDisplayLockItem();
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x00147F08 File Offset: 0x00146108
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		CraftToolItem configData = CraftTool.Instance[itemDisplayData.Key.TemplateId];
		bool flag = !configData.Repairable;
		if (flag)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Repairable);
		}
		bool flag2 = !configData.Transferable;
		if (flag2)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Transferable);
		}
		bool flag3 = !configData.Poisonable;
		if (flag3)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Poisonable);
		}
		bool flag4 = !configData.Refinable;
		if (flag4)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Refinable);
		}
	}
}
