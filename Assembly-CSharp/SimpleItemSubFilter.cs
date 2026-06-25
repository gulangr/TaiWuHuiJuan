using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200035B RID: 859
public class SimpleItemSubFilter : Refers
{
	// Token: 0x1700056E RID: 1390
	// (get) Token: 0x060031FA RID: 12794 RVA: 0x0018AB8C File Offset: 0x00188D8C
	private int ConfigIndex
	{
		get
		{
			return (int)this._mainFilterType;
		}
	}

	// Token: 0x1700056F RID: 1391
	// (get) Token: 0x060031FB RID: 12795 RVA: 0x0018AB94 File Offset: 0x00188D94
	private SimpleItemSubFilter.SubTypeFilterConfig SelectedSubFilterConfig
	{
		get
		{
			return this._subTypeFilterConfigs[this.ConfigIndex][this._selectedSubTypeIndex];
		}
	}

	// Token: 0x17000570 RID: 1392
	// (get) Token: 0x060031FC RID: 12796 RVA: 0x0018ABB0 File Offset: 0x00188DB0
	public Enum SelectedSubType
	{
		get
		{
			bool flag = this._mainFilterType == SimpleItemMainFilter.ItemFilterType.All;
			Enum result;
			if (flag)
			{
				result = SimpleItemSubFilter.InvalidEnum.Invalid;
			}
			else
			{
				result = this.SelectedSubFilterConfig.SubFilterType;
			}
			return result;
		}
	}

	// Token: 0x060031FD RID: 12797 RVA: 0x0018ABE3 File Offset: 0x00188DE3
	public void Init()
	{
		this.InitRefers();
	}

	// Token: 0x060031FE RID: 12798 RVA: 0x0018ABED File Offset: 0x00188DED
	public void Refresh(SimpleItemMainFilter.ItemFilterType mainFilterType)
	{
		this._mainFilterType = mainFilterType;
		this.GenerateSubToggles();
	}

	// Token: 0x060031FF RID: 12799 RVA: 0x0018ABFE File Offset: 0x00188DFE
	public void SetCallback(Action onSelectedTypeChanged)
	{
		this._onSelectedTypeChanged = onSelectedTypeChanged;
	}

	// Token: 0x06003200 RID: 12800 RVA: 0x0018AC08 File Offset: 0x00188E08
	public bool IsItemMatch(sbyte itemType, short templateId)
	{
		switch (itemType)
		{
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
			return this.IsWeaponItemMatch(itemType, templateId);
		case 5:
			return this.IsMaterialItemMatch(templateId);
		case 6:
			return this.IsCraftToolItemMatch(templateId);
		case 7:
		case 9:
			return this.IsFoolItemMatch(itemType, templateId);
		case 8:
			return this.IsMedicineItemMatch(templateId);
		case 10:
			return this.IsBookItemMatch(templateId);
		case 12:
			return this.IsMiscItemMatch(templateId);
		}
		return false;
	}

	// Token: 0x06003201 RID: 12801 RVA: 0x0018ACA4 File Offset: 0x00188EA4
	private bool IsFoolItemMatch(sbyte itemType, short templateId)
	{
		SimpleItemSubFilter.FoodWineFilterType selectedFilterType = (SimpleItemSubFilter.FoodWineFilterType)this.SelectedSubFilterConfig.SubFilterType;
		bool flag = selectedFilterType == SimpleItemSubFilter.FoodWineFilterType.All;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			if (!true)
			{
			}
			SimpleItemSubFilter.FoodWineFilterType foodWineFilterType;
			if (itemType != 7)
			{
				if (itemType != 9)
				{
					throw new ArgumentOutOfRangeException();
				}
				foodWineFilterType = SimpleItemSubFilter.GetTeaWineItemFilterType(templateId);
			}
			else
			{
				foodWineFilterType = SimpleItemSubFilter.GetFoodItemFilterType(templateId);
			}
			if (!true)
			{
			}
			SimpleItemSubFilter.FoodWineFilterType itemFilterType = foodWineFilterType;
			result = (selectedFilterType == itemFilterType);
		}
		return result;
	}

	// Token: 0x06003202 RID: 12802 RVA: 0x0018AD0C File Offset: 0x00188F0C
	private bool IsMedicineItemMatch(short templateId)
	{
		SimpleItemSubFilter.MedicineFilterType selectedFilterType = (SimpleItemSubFilter.MedicineFilterType)this.SelectedSubFilterConfig.SubFilterType;
		bool flag = selectedFilterType == SimpleItemSubFilter.MedicineFilterType.All;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			SimpleItemSubFilter.MedicineFilterType itemFilterType = SimpleItemSubFilter.GetMedicineItemFilterType(templateId);
			result = (selectedFilterType == itemFilterType);
		}
		return result;
	}

	// Token: 0x06003203 RID: 12803 RVA: 0x0018AD48 File Offset: 0x00188F48
	private bool IsWeaponItemMatch(sbyte itemType, short templateId)
	{
		SimpleItemSubFilter.EquipFilterType selectedFilterType = (SimpleItemSubFilter.EquipFilterType)this.SelectedSubFilterConfig.SubFilterType;
		bool result;
		switch (selectedFilterType)
		{
		case SimpleItemSubFilter.EquipFilterType.All:
			result = (itemType == 0 || itemType == 2 || itemType == 3 || SimpleItemSubFilter.IsArmorMatch(itemType, templateId, selectedFilterType) || SimpleItemSubFilter.IsCarrierMatch(itemType, templateId));
			break;
		case SimpleItemSubFilter.EquipFilterType.Weapon:
			result = (itemType == 0);
			break;
		case SimpleItemSubFilter.EquipFilterType.Helm:
		case SimpleItemSubFilter.EquipFilterType.Torso:
		case SimpleItemSubFilter.EquipFilterType.Bracers:
		case SimpleItemSubFilter.EquipFilterType.Boots:
			result = SimpleItemSubFilter.IsArmorMatch(itemType, templateId, selectedFilterType);
			break;
		case SimpleItemSubFilter.EquipFilterType.Accessory:
			result = (itemType == 2);
			break;
		case SimpleItemSubFilter.EquipFilterType.Clothing:
			result = (itemType == 3);
			break;
		case SimpleItemSubFilter.EquipFilterType.Carrier:
			result = SimpleItemSubFilter.IsCarrierMatch(itemType, templateId);
			break;
		case SimpleItemSubFilter.EquipFilterType.Other:
			result = true;
			break;
		default:
			result = false;
			break;
		}
		return result;
	}

	// Token: 0x06003204 RID: 12804 RVA: 0x0018ADF4 File Offset: 0x00188FF4
	private static bool IsCarrierMatch(sbyte itemType, short templateId)
	{
		bool flag = itemType != 4;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			CarrierItem config = Carrier.Instance[templateId];
			short subType = config.ItemSubType;
			result = (subType != 401 && subType != 402 && subType != 403);
		}
		return result;
	}

	// Token: 0x06003205 RID: 12805 RVA: 0x0018AE48 File Offset: 0x00189048
	private static bool IsArmorMatch(sbyte itemType, short templateId, SimpleItemSubFilter.EquipFilterType selectedFilterType)
	{
		bool flag = itemType != 1;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			SimpleItemSubFilter.EquipFilterType filterType = SimpleItemSubFilter.GetArmorItemFilterType(templateId);
			result = (filterType == selectedFilterType);
		}
		return result;
	}

	// Token: 0x06003206 RID: 12806 RVA: 0x0018AE74 File Offset: 0x00189074
	private bool IsBookItemMatch(short templateId)
	{
		SimpleItemSubFilter.BookFilterType selectedFilterType = (SimpleItemSubFilter.BookFilterType)this.SelectedSubFilterConfig.SubFilterType;
		bool flag = selectedFilterType == SimpleItemSubFilter.BookFilterType.All;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			SkillBookItem config = SkillBook.Instance[templateId];
			short itemSubType = config.ItemSubType;
			if (!true)
			{
			}
			SimpleItemSubFilter.BookFilterType bookFilterType;
			if (itemSubType != 1000)
			{
				if (itemSubType != 1001)
				{
					bookFilterType = SimpleItemSubFilter.BookFilterType.Other;
				}
				else
				{
					bookFilterType = SimpleItemSubFilter.BookFilterType.CombatSkill;
				}
			}
			else
			{
				bookFilterType = SimpleItemSubFilter.BookFilterType.LifeSkill;
			}
			if (!true)
			{
			}
			SimpleItemSubFilter.BookFilterType itemFilterType = bookFilterType;
			result = (selectedFilterType == itemFilterType);
		}
		return result;
	}

	// Token: 0x06003207 RID: 12807 RVA: 0x0018AEF0 File Offset: 0x001890F0
	private bool IsCraftToolItemMatch(short templateId)
	{
		SimpleItemSubFilter.CraftToolFilterType selectedFilterType = (SimpleItemSubFilter.CraftToolFilterType)this.SelectedSubFilterConfig.SubFilterType;
		bool flag = selectedFilterType == SimpleItemSubFilter.CraftToolFilterType.All;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			CraftToolItem config = CraftTool.Instance[templateId];
			List<sbyte> needSkillTypes = config.RequiredLifeSkillTypes;
			result = needSkillTypes.Any((sbyte skillType) => selectedFilterType == this.GetCraftToolFilterType(skillType));
		}
		return result;
	}

	// Token: 0x06003208 RID: 12808 RVA: 0x0018AF60 File Offset: 0x00189160
	private bool IsMaterialItemMatch(short templateId)
	{
		SimpleItemSubFilter.MaterialFilterType selectedFilterType = (SimpleItemSubFilter.MaterialFilterType)this.SelectedSubFilterConfig.SubFilterType;
		MaterialItem config = Config.Material.Instance[templateId];
		bool result;
		switch (selectedFilterType)
		{
		case SimpleItemSubFilter.MaterialFilterType.All:
		{
			short itemSubType = config.ItemSubType;
			result = (itemSubType == 500 || itemSubType == 501 || itemSubType == 502 || itemSubType == 503 || itemSubType == 504 || itemSubType == 505 || itemSubType == 506);
			break;
		}
		case SimpleItemSubFilter.MaterialFilterType.Food:
			result = (config.ItemSubType == 500);
			break;
		case SimpleItemSubFilter.MaterialFilterType.Wood:
			result = (config.ItemSubType == 501);
			break;
		case SimpleItemSubFilter.MaterialFilterType.Metal:
			result = (config.ItemSubType == 502);
			break;
		case SimpleItemSubFilter.MaterialFilterType.Jade:
			result = (config.ItemSubType == 503);
			break;
		case SimpleItemSubFilter.MaterialFilterType.Fabric:
			result = (config.ItemSubType == 504);
			break;
		case SimpleItemSubFilter.MaterialFilterType.Medicine:
			result = (config.ItemSubType == 505);
			break;
		case SimpleItemSubFilter.MaterialFilterType.Poison:
			result = (config.ItemSubType == 506);
			break;
		case SimpleItemSubFilter.MaterialFilterType.Other:
			result = true;
			break;
		default:
			result = false;
			break;
		}
		return result;
	}

	// Token: 0x06003209 RID: 12809 RVA: 0x0018B098 File Offset: 0x00189298
	private bool IsMiscItemMatch(short templateId)
	{
		SimpleItemSubFilter.MiscFilterType selectedFilterType = (SimpleItemSubFilter.MiscFilterType)this.SelectedSubFilterConfig.SubFilterType;
		MiscItem config = Misc.Instance[templateId];
		bool result;
		switch (selectedFilterType)
		{
		case SimpleItemSubFilter.MiscFilterType.All:
			result = (SimpleItemSubFilter.IsSweepNet(templateId) || SimpleItemSubFilter.IsNeedle(templateId) || SimpleItemSubFilter.IsBloodDew(templateId) || SimpleItemSubFilter.IsBuildingCore(config) || SimpleItemSubFilter.IsRope(config) || SimpleItemSubFilter.IsCricketJar(config));
			break;
		case SimpleItemSubFilter.MiscFilterType.BuildingCore:
			result = SimpleItemSubFilter.IsBuildingCore(config);
			break;
		case SimpleItemSubFilter.MiscFilterType.Rope:
			result = SimpleItemSubFilter.IsRope(config);
			break;
		case SimpleItemSubFilter.MiscFilterType.Needle:
			result = SimpleItemSubFilter.IsNeedle(templateId);
			break;
		case SimpleItemSubFilter.MiscFilterType.SweepNet:
			result = SimpleItemSubFilter.IsSweepNet(templateId);
			break;
		case SimpleItemSubFilter.MiscFilterType.BloodDew:
			result = SimpleItemSubFilter.IsBloodDew(templateId);
			break;
		case SimpleItemSubFilter.MiscFilterType.Jar:
			result = SimpleItemSubFilter.IsCricketJar(config);
			break;
		case SimpleItemSubFilter.MiscFilterType.Other:
			result = true;
			break;
		default:
			result = false;
			break;
		}
		return result;
	}

	// Token: 0x0600320A RID: 12810 RVA: 0x0018B16C File Offset: 0x0018936C
	private static bool IsCricketJar(MiscItem config)
	{
		return config.ItemSubType == 1201;
	}

	// Token: 0x0600320B RID: 12811 RVA: 0x0018B18C File Offset: 0x0018938C
	private static bool IsRope(MiscItem config)
	{
		return config.ItemSubType == 1206;
	}

	// Token: 0x0600320C RID: 12812 RVA: 0x0018B1AC File Offset: 0x001893AC
	private static bool IsBuildingCore(MiscItem config)
	{
		return config.ItemSubType == 1205;
	}

	// Token: 0x0600320D RID: 12813 RVA: 0x0018B1CC File Offset: 0x001893CC
	private static bool IsBloodDew(short templateId)
	{
		return templateId == 9 || templateId == 10 || templateId == 11 || templateId == 12 || templateId == 13 || templateId == 14 || templateId == 15 || templateId == 16 || templateId == 17;
	}

	// Token: 0x0600320E RID: 12814 RVA: 0x0018B210 File Offset: 0x00189410
	private static bool IsNeedle(short templateId)
	{
		return templateId == 264;
	}

	// Token: 0x0600320F RID: 12815 RVA: 0x0018B22C File Offset: 0x0018942C
	private static bool IsSweepNet(short templateId)
	{
		return templateId == 18;
	}

	// Token: 0x06003210 RID: 12816 RVA: 0x0018B244 File Offset: 0x00189444
	private SimpleItemSubFilter.CraftToolFilterType GetCraftToolFilterType(sbyte lifeSkillType)
	{
		switch (lifeSkillType)
		{
		case 6:
			return SimpleItemSubFilter.CraftToolFilterType.Forging;
		case 7:
			return SimpleItemSubFilter.CraftToolFilterType.Woodworking;
		case 8:
			return SimpleItemSubFilter.CraftToolFilterType.Medicine;
		case 10:
			return SimpleItemSubFilter.CraftToolFilterType.Weaving;
		case 11:
			return SimpleItemSubFilter.CraftToolFilterType.Jade;
		case 14:
			return SimpleItemSubFilter.CraftToolFilterType.Cooking;
		}
		return SimpleItemSubFilter.CraftToolFilterType.Other;
	}

	// Token: 0x06003211 RID: 12817 RVA: 0x0018B2A4 File Offset: 0x001894A4
	private static SimpleItemSubFilter.FoodWineFilterType GetFoodItemFilterType(short templateId)
	{
		FoodItem config = Food.Instance[templateId];
		short itemSubType = config.ItemSubType;
		short num = itemSubType;
		SimpleItemSubFilter.FoodWineFilterType result;
		if (num != 700)
		{
			if (num != 701)
			{
				result = SimpleItemSubFilter.FoodWineFilterType.Other;
			}
			else
			{
				result = SimpleItemSubFilter.FoodWineFilterType.Meat;
			}
		}
		else
		{
			result = SimpleItemSubFilter.FoodWineFilterType.Vegetarian;
		}
		return result;
	}

	// Token: 0x06003212 RID: 12818 RVA: 0x0018B2E8 File Offset: 0x001894E8
	private static SimpleItemSubFilter.FoodWineFilterType GetTeaWineItemFilterType(short templateId)
	{
		TeaWineItem config = TeaWine.Instance[templateId];
		short itemSubType = config.ItemSubType;
		short num = itemSubType;
		SimpleItemSubFilter.FoodWineFilterType result;
		if (num != 900)
		{
			if (num != 901)
			{
				result = SimpleItemSubFilter.FoodWineFilterType.Other;
			}
			else
			{
				result = SimpleItemSubFilter.FoodWineFilterType.Wine;
			}
		}
		else
		{
			result = SimpleItemSubFilter.FoodWineFilterType.Tea;
		}
		return result;
	}

	// Token: 0x06003213 RID: 12819 RVA: 0x0018B32C File Offset: 0x0018952C
	private static SimpleItemSubFilter.MedicineFilterType GetMedicineItemFilterType(short templateId)
	{
		MedicineItem config = Medicine.Instance[templateId];
		bool flag = config.BuffAndOtherMedicine == 1;
		SimpleItemSubFilter.MedicineFilterType result;
		if (flag)
		{
			result = SimpleItemSubFilter.MedicineFilterType.Buff;
		}
		else
		{
			bool flag2 = config.BuffAndOtherMedicine == 2;
			if (flag2)
			{
				result = SimpleItemSubFilter.MedicineFilterType.Other;
			}
			else
			{
				switch (config.EffectType)
				{
				case EMedicineEffectType.RecoverOuterInjury:
					return SimpleItemSubFilter.MedicineFilterType.Outer;
				case EMedicineEffectType.RecoverInnerInjury:
					return SimpleItemSubFilter.MedicineFilterType.Inner;
				case EMedicineEffectType.RecoverHealth:
					return SimpleItemSubFilter.MedicineFilterType.Health;
				case EMedicineEffectType.ChangeDisorderOfQi:
					return SimpleItemSubFilter.MedicineFilterType.HealDisorder;
				case EMedicineEffectType.DetoxPoison:
					return SimpleItemSubFilter.MedicineFilterType.DeToxic;
				case EMedicineEffectType.ApplyPoison:
					return SimpleItemSubFilter.MedicineFilterType.Poison;
				}
				result = SimpleItemSubFilter.MedicineFilterType.Other;
			}
		}
		return result;
	}

	// Token: 0x06003214 RID: 12820 RVA: 0x0018B3B8 File Offset: 0x001895B8
	private static SimpleItemSubFilter.EquipFilterType GetArmorItemFilterType(short templateId)
	{
		ArmorItem config = Armor.Instance[templateId];
		SimpleItemSubFilter.EquipFilterType result;
		switch (config.ItemSubType)
		{
		case 100:
			result = SimpleItemSubFilter.EquipFilterType.Helm;
			break;
		case 101:
			result = SimpleItemSubFilter.EquipFilterType.Torso;
			break;
		case 102:
			result = SimpleItemSubFilter.EquipFilterType.Bracers;
			break;
		case 103:
			result = SimpleItemSubFilter.EquipFilterType.Boots;
			break;
		default:
			result = SimpleItemSubFilter.EquipFilterType.Other;
			break;
		}
		return result;
	}

	// Token: 0x06003215 RID: 12821 RVA: 0x0018B414 File Offset: 0x00189614
	private void GenerateSubToggles()
	{
		bool flag = this._mainFilterType == SimpleItemMainFilter.ItemFilterType.All;
		if (flag)
		{
			this.PrepareAndSetupToggles(0);
		}
		else
		{
			SimpleItemSubFilter.SubTypeFilterConfig[] subConfig = this._subTypeFilterConfigs[(int)this._mainFilterType];
			this.PrepareAndSetupToggles(subConfig.Length);
		}
	}

	// Token: 0x06003216 RID: 12822 RVA: 0x0018B454 File Offset: 0x00189654
	private void PrepareAndSetupToggles(int count)
	{
		CToggleObsolete template = this._toggleTemplate;
		Transform parent = base.transform;
		ToggleGroup toggleGroup = base.GetComponent<ToggleGroup>();
		int childCount = parent.childCount;
		for (int i = count; i < childCount; i++)
		{
			parent.GetChild(i).gameObject.SetActive(false);
		}
		for (int j = 0; j < count; j++)
		{
			CToggleObsolete toggle = (j < childCount) ? parent.GetChild(j).GetComponent<CToggleObsolete>() : Object.Instantiate<CToggleObsolete>(template, parent);
			toggle.group = toggleGroup;
			toggle.gameObject.SetActive(true);
			SimpleItemSubFilter.SubTypeFilterConfig subConfig = this._subTypeFilterConfigs[(int)this._mainFilterType][j];
			Refers refers = toggle.GetComponent<Refers>();
			GameObject selected = refers.CGet<GameObject>("Selected");
			TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
			label.text = LocalStringManager.Get(subConfig.LanguageKey);
			int ii = j;
			toggle.onValueChanged.RemoveAllListeners();
			toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				selected.SetActive(isOn);
				if (isOn)
				{
					this._selectedSubTypeIndex = ii;
					Action onSelectedTypeChanged = this._onSelectedTypeChanged;
					if (onSelectedTypeChanged != null)
					{
						onSelectedTypeChanged();
					}
				}
			});
		}
		CToggleObsolete toggle2 = parent.GetChild(0).GetComponent<CToggleObsolete>();
		bool isOn2 = toggle2.isOn;
		if (isOn2)
		{
			Toggle.ToggleEvent onValueChanged = toggle2.onValueChanged;
			if (onValueChanged != null)
			{
				onValueChanged.Invoke(true);
			}
		}
		else
		{
			toggle2.isOn = true;
		}
	}

	// Token: 0x06003217 RID: 12823 RVA: 0x0018B5CD File Offset: 0x001897CD
	private void InitRefers()
	{
		this._toggleTemplate = base.CGet<CToggleObsolete>("ToggleTemplate");
	}

	// Token: 0x040024A8 RID: 9384
	private Action _onSelectedTypeChanged;

	// Token: 0x040024A9 RID: 9385
	private SimpleItemMainFilter.ItemFilterType _mainFilterType;

	// Token: 0x040024AA RID: 9386
	private int _selectedSubTypeIndex;

	// Token: 0x040024AB RID: 9387
	private readonly SimpleItemSubFilter.SubTypeFilterConfig[][] _subTypeFilterConfigs = new SimpleItemSubFilter.SubTypeFilterConfig[][]
	{
		default(SimpleItemSubFilter.SubTypeFilterConfig[]),
		new SimpleItemSubFilter.SubTypeFilterConfig[]
		{
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Filter_Type_All, SimpleItemSubFilter.FoodWineFilterType.All),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Food_Vegetarian, SimpleItemSubFilter.FoodWineFilterType.Vegetarian),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Food_Meat, SimpleItemSubFilter.FoodWineFilterType.Meat),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Food_Tea, SimpleItemSubFilter.FoodWineFilterType.Tea),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Food_Wine, SimpleItemSubFilter.FoodWineFilterType.Wine)
		},
		new SimpleItemSubFilter.SubTypeFilterConfig[]
		{
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Filter_Type_All, SimpleItemSubFilter.MedicineFilterType.All),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_MedicineItem_Filter_Type_Outer_Short, SimpleItemSubFilter.MedicineFilterType.Outer),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_MedicineItem_Filter_Type_Inner_Short, SimpleItemSubFilter.MedicineFilterType.Inner),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_MedicineItem_Filter_Type_DeToxic_Short, SimpleItemSubFilter.MedicineFilterType.DeToxic),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_MedicineItem_Filter_Type_Poison_Short, SimpleItemSubFilter.MedicineFilterType.Poison),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_MedicineItem_Filter_Type_HealDisorder_Short, SimpleItemSubFilter.MedicineFilterType.HealDisorder),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_MedicineItem_Filter_Type_Health_Short, SimpleItemSubFilter.MedicineFilterType.Health),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_MedicineItem_Filter_Type_Buff_Short, SimpleItemSubFilter.MedicineFilterType.Buff),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_MedicineItem_Filter_Type_Other_Short, SimpleItemSubFilter.MedicineFilterType.Other)
		},
		new SimpleItemSubFilter.SubTypeFilterConfig[]
		{
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Filter_Type_All, SimpleItemSubFilter.EquipFilterType.All),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Equip_Slot_Name_Short_Weapon, SimpleItemSubFilter.EquipFilterType.Weapon),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Equip_Slot_Name_Short_Helm, SimpleItemSubFilter.EquipFilterType.Helm),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Equip_Slot_Name_Short_Torso, SimpleItemSubFilter.EquipFilterType.Torso),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Equip_Slot_Name_Short_Bracers, SimpleItemSubFilter.EquipFilterType.Bracers),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Equip_Slot_Name_Short_Boots, SimpleItemSubFilter.EquipFilterType.Boots),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Equip_Slot_Name_Short_Accessory, SimpleItemSubFilter.EquipFilterType.Accessory),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Equip_Slot_Name_Short_Clothing, SimpleItemSubFilter.EquipFilterType.Clothing),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Equip_Slot_Name_Short_Carrier, SimpleItemSubFilter.EquipFilterType.Carrier)
		},
		new SimpleItemSubFilter.SubTypeFilterConfig[]
		{
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Filter_Type_All, SimpleItemSubFilter.BookFilterType.All),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Book_CombatSkill, SimpleItemSubFilter.BookFilterType.CombatSkill),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Book_LifeSkill, SimpleItemSubFilter.BookFilterType.LifeSkill)
		},
		new SimpleItemSubFilter.SubTypeFilterConfig[]
		{
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Filter_Type_All, SimpleItemSubFilter.CraftToolFilterType.All),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_CraftTool_Weaving, SimpleItemSubFilter.CraftToolFilterType.Weaving),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_CraftTool_Woodworking, SimpleItemSubFilter.CraftToolFilterType.Woodworking),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_CraftTool_Jade, SimpleItemSubFilter.CraftToolFilterType.Jade),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_CraftTool_Forging, SimpleItemSubFilter.CraftToolFilterType.Forging),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_CraftTool_Medicine, SimpleItemSubFilter.CraftToolFilterType.Medicine),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_CraftTool_Cooking, SimpleItemSubFilter.CraftToolFilterType.Cooking)
		},
		new SimpleItemSubFilter.SubTypeFilterConfig[]
		{
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Filter_Type_All, SimpleItemSubFilter.MaterialFilterType.All),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Material_Food, SimpleItemSubFilter.MaterialFilterType.Food),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Material_Wood, SimpleItemSubFilter.MaterialFilterType.Wood),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Material_Metal, SimpleItemSubFilter.MaterialFilterType.Metal),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Material_Jade, SimpleItemSubFilter.MaterialFilterType.Jade),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Material_Fabric, SimpleItemSubFilter.MaterialFilterType.Fabric),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Material_Medicine, SimpleItemSubFilter.MaterialFilterType.Medicine),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Material_Poison, SimpleItemSubFilter.MaterialFilterType.Poison)
		},
		new SimpleItemSubFilter.SubTypeFilterConfig[]
		{
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Filter_Type_All, SimpleItemSubFilter.MiscFilterType.All),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Misc_BuildingCore, SimpleItemSubFilter.MiscFilterType.BuildingCore),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Misc_Rope, SimpleItemSubFilter.MiscFilterType.Rope),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Misc_Needle, SimpleItemSubFilter.MiscFilterType.Needle),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Misc_SweepNet, SimpleItemSubFilter.MiscFilterType.SweepNet),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Misc_BloodDew, SimpleItemSubFilter.MiscFilterType.BloodDew),
			new SimpleItemSubFilter.SubTypeFilterConfig(LanguageKey.LK_Item_Filter_SubType_Misc_Jar, SimpleItemSubFilter.MiscFilterType.Jar)
		}
	};

	// Token: 0x040024AC RID: 9388
	private CToggleObsolete _toggleTemplate;

	// Token: 0x0200170E RID: 5902
	public enum InvalidEnum
	{
		// Token: 0x0400AA2E RID: 43566
		Invalid = -1
	}

	// Token: 0x0200170F RID: 5903
	public enum FoodWineFilterType
	{
		// Token: 0x0400AA30 RID: 43568
		All,
		// Token: 0x0400AA31 RID: 43569
		Vegetarian,
		// Token: 0x0400AA32 RID: 43570
		Meat,
		// Token: 0x0400AA33 RID: 43571
		Tea,
		// Token: 0x0400AA34 RID: 43572
		Wine,
		// Token: 0x0400AA35 RID: 43573
		Other
	}

	// Token: 0x02001710 RID: 5904
	public enum MedicineFilterType
	{
		// Token: 0x0400AA37 RID: 43575
		All,
		// Token: 0x0400AA38 RID: 43576
		Outer,
		// Token: 0x0400AA39 RID: 43577
		Inner,
		// Token: 0x0400AA3A RID: 43578
		DeToxic,
		// Token: 0x0400AA3B RID: 43579
		Poison,
		// Token: 0x0400AA3C RID: 43580
		HealDisorder,
		// Token: 0x0400AA3D RID: 43581
		Health,
		// Token: 0x0400AA3E RID: 43582
		Buff,
		// Token: 0x0400AA3F RID: 43583
		Other
	}

	// Token: 0x02001711 RID: 5905
	public enum EquipFilterType
	{
		// Token: 0x0400AA41 RID: 43585
		All,
		// Token: 0x0400AA42 RID: 43586
		Weapon,
		// Token: 0x0400AA43 RID: 43587
		Helm,
		// Token: 0x0400AA44 RID: 43588
		Torso,
		// Token: 0x0400AA45 RID: 43589
		Bracers,
		// Token: 0x0400AA46 RID: 43590
		Boots,
		// Token: 0x0400AA47 RID: 43591
		Accessory,
		// Token: 0x0400AA48 RID: 43592
		Clothing,
		// Token: 0x0400AA49 RID: 43593
		Carrier,
		// Token: 0x0400AA4A RID: 43594
		Other
	}

	// Token: 0x02001712 RID: 5906
	public enum BookFilterType
	{
		// Token: 0x0400AA4C RID: 43596
		All,
		// Token: 0x0400AA4D RID: 43597
		CombatSkill,
		// Token: 0x0400AA4E RID: 43598
		LifeSkill,
		// Token: 0x0400AA4F RID: 43599
		Other
	}

	// Token: 0x02001713 RID: 5907
	public enum CraftToolFilterType
	{
		// Token: 0x0400AA51 RID: 43601
		All,
		// Token: 0x0400AA52 RID: 43602
		Weaving,
		// Token: 0x0400AA53 RID: 43603
		Woodworking,
		// Token: 0x0400AA54 RID: 43604
		Jade,
		// Token: 0x0400AA55 RID: 43605
		Forging,
		// Token: 0x0400AA56 RID: 43606
		Medicine,
		// Token: 0x0400AA57 RID: 43607
		Cooking,
		// Token: 0x0400AA58 RID: 43608
		Other
	}

	// Token: 0x02001714 RID: 5908
	public enum MaterialFilterType
	{
		// Token: 0x0400AA5A RID: 43610
		All,
		// Token: 0x0400AA5B RID: 43611
		Food,
		// Token: 0x0400AA5C RID: 43612
		Wood,
		// Token: 0x0400AA5D RID: 43613
		Metal,
		// Token: 0x0400AA5E RID: 43614
		Jade,
		// Token: 0x0400AA5F RID: 43615
		Fabric,
		// Token: 0x0400AA60 RID: 43616
		Medicine,
		// Token: 0x0400AA61 RID: 43617
		Poison,
		// Token: 0x0400AA62 RID: 43618
		Other
	}

	// Token: 0x02001715 RID: 5909
	public enum MiscFilterType
	{
		// Token: 0x0400AA64 RID: 43620
		All,
		// Token: 0x0400AA65 RID: 43621
		BuildingCore,
		// Token: 0x0400AA66 RID: 43622
		Rope,
		// Token: 0x0400AA67 RID: 43623
		Needle,
		// Token: 0x0400AA68 RID: 43624
		SweepNet,
		// Token: 0x0400AA69 RID: 43625
		BloodDew,
		// Token: 0x0400AA6A RID: 43626
		Jar,
		// Token: 0x0400AA6B RID: 43627
		Other
	}

	// Token: 0x02001716 RID: 5910
	private struct SubTypeFilterConfig
	{
		// Token: 0x0600D317 RID: 54039 RVA: 0x005B1067 File Offset: 0x005AF267
		public SubTypeFilterConfig(LanguageKey languageKey, Enum subFilterType)
		{
			this.LanguageKey = languageKey;
			this.SubFilterType = subFilterType;
		}

		// Token: 0x0400AA6C RID: 43628
		public readonly LanguageKey LanguageKey;

		// Token: 0x0400AA6D RID: 43629
		public readonly Enum SubFilterType;
	}
}
