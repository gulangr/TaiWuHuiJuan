using System;
using Config;
using FrameWork;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002BF RID: 703
public class MouseTipMaterial : MouseTipItem
{
	// Token: 0x170004AE RID: 1198
	// (get) Token: 0x06002AD7 RID: 10967 RVA: 0x00148978 File Offset: 0x00146B78
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002AD8 RID: 10968 RVA: 0x0014897C File Offset: 0x00146B7C
	protected override void Init(ArgumentBox argsBox)
	{
		base.Init(argsBox);
		ItemDisplayData itemData;
		argsBox.Get<ItemDisplayData>("ItemData", out itemData);
		bool templateDataOnly;
		argsBox.Get("TemplateDataOnly", out templateDataOnly);
		this._itemData = itemData;
		MaterialItem configData = Config.Material.Instance[itemData.Key.TemplateId];
		base.CGet<TextMeshProUGUI>("Name").text = configData.Name;
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("GradeName").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		base.CGet<TextMeshProUGUI>("Grade").text = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - configData.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
		base.CGet<TextMeshProUGUI>("Value").text = (templateDataOnly ? configData.BaseValue.ToString() : itemData.Value.ToString());
		base.CGet<GameObject>("Material").SetActive(!templateDataOnly);
		base.CGet<CImage>("ItemIcon").SetSprite(configData.Icon, false, null);
		base.SetItemDesc(configData.Desc, itemData.LoveTokenDataItem);
		base.CGet<TextMeshProUGUI>("SubType").text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", configData.ItemSubType));
		base.CGet<TextMeshProUGUI>("Weight").text = NumberFormatUtils.FormatItemWeight(itemData.Weight);
		this.InitItemDisableFunctionList(itemData);
		base.RefreshDisableFunction();
		bool showEatingInfo = !templateDataOnly && CommonUtils.CanItemEat(itemData.Key.ItemType, itemData.Key.TemplateId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		GameObject eatEffect = base.CGet<GameObject>("EatEffect");
		Refers eatingRefers = eatEffect.GetComponent<Refers>();
		short eatingTime;
		bool showEatingTime = argsBox.Get("EatingTime", out eatingTime);
		eatingRefers.CGet<GameObject>("EatingTimeTips").SetActive(showEatingTime);
		bool flag = showEatingTime;
		if (flag)
		{
			eatingRefers.CGet<TextMeshProUGUI>("EatingTime").text = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_Eating_Time, eatingTime).ColorReplace();
		}
		eatEffect.SetActive(showEatingInfo);
		bool flag2 = showEatingInfo;
		if (flag2)
		{
			RectTransform addPropertyHolder = eatingRefers.CGet<RectTransform>("AddPropertyHolder");
			RectTransform addPropertyHolderSpecial = eatingRefers.CGet<RectTransform>("AddPropertyHolderSpecial");
			RectTransform curAddPropertyHolder = addPropertyHolder;
			RectTransform otherAddPropertyHolder = addPropertyHolderSpecial;
			TipsAddProperty addPropertyPrefab = eatingRefers.CGet<TipsAddProperty>("AddPropertyVertical");
			int recoverHealthSlot = this.GetEffectSlot(configData, EMedicineEffectType.RecoverHealth);
			bool flag3 = recoverHealthSlot > 0;
			if (flag3)
			{
				eatingRefers.CGet<TextMeshProUGUI>("EatHealHealthValue").text = string.Format("+{0}", this.GetEffectValue(configData, recoverHealthSlot));
				eatingRefers.CGet<GameObject>("EatHealHealth").SetActive(true);
			}
			else
			{
				eatingRefers.CGet<GameObject>("EatHealHealth").SetActive(false);
			}
			int detoxPoisonSlot = this.GetEffectSlot(configData, EMedicineEffectType.DetoxPoison);
			bool flag4 = detoxPoisonSlot > 0;
			if (flag4)
			{
				sbyte poisonType = this.GetEffectSubtype(configData, detoxPoisonSlot).PoisonType();
				PoisonItem poisonConfig = Poison.Instance[poisonType];
				string maxPoisonLevelTips = LocalStringManager.GetFormat(LanguageKey.LK_Poison_Max_Level_Tips, LocalStringManager.Get(string.Format("LK_Poison_Level_{0}", this.GetEffectThreshold(configData, detoxPoisonSlot))));
				short effectValue = this.GetEffectValue(configData, detoxPoisonSlot);
				eatingRefers.CGet<CImage>("EatHealPoisonIcon").SetSprite(poisonConfig.TipsIcon, false, null);
				eatingRefers.CGet<TextMeshProUGUI>("EatHealPoisonName").text = poisonConfig.Name + maxPoisonLevelTips;
				eatingRefers.CGet<TextMeshProUGUI>("EatHealPoisonValue").text = ((effectValue < 0) ? string.Format("{0}", effectValue) : string.Format("-{0}%", effectValue));
				eatingRefers.CGet<GameObject>("EatHealPoison").SetActive(true);
			}
			else
			{
				eatingRefers.CGet<GameObject>("EatHealPoison").SetActive(false);
			}
			int recoverOuterInjurySlot = this.GetEffectSlot(configData, EMedicineEffectType.RecoverOuterInjury);
			eatingRefers.CGet<GameObject>("EatHealOuterInjury").SetActive(false);
			int recoverInnerInjurySlot = this.GetEffectSlot(configData, EMedicineEffectType.RecoverInnerInjury);
			eatingRefers.CGet<GameObject>("EatHealInnerInjury").SetActive(false);
			int changeDisorderOfQiSlot = this.GetEffectSlot(configData, EMedicineEffectType.ChangeDisorderOfQi);
			bool flag5 = changeDisorderOfQiSlot > 0;
			if (flag5)
			{
				eatingRefers.CGet<TextMeshProUGUI>("EatHealHealthValue").text = string.Format("+{0}", this.GetEffectValue(configData, recoverHealthSlot));
				eatingRefers.CGet<GameObject>("EatHealQiDisorder").SetActive(true);
			}
			else
			{
				eatingRefers.CGet<TextMeshProUGUI>("EatHealQiDisorderValue").text = ((int)(this.GetEffectValue(configData, changeDisorderOfQiSlot) / 10)).ToString();
				eatingRefers.CGet<GameObject>("EatHealQiDisorder").SetActive(false);
			}
			bool flag6 = configData.BaseMaxHealthDelta > 0;
			if (flag6)
			{
				eatingRefers.CGet<TextMeshProUGUI>("MaxHealthValue").text = configData.BaseMaxHealthDelta.ToString();
				eatingRefers.CGet<GameObject>("MaxHealth").SetActive(true);
			}
			else
			{
				eatingRefers.CGet<GameObject>("MaxHealth").SetActive(false);
			}
			int index = 0;
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 6, (int)configData.HitRateStrength, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 7, (int)configData.HitRateTechnique, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 8, (int)configData.HitRateSpeed, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 9, (int)configData.HitRateMind, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 10, (int)configData.PenetrateOfOuter, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 11, (int)configData.PenetrateOfInner, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 12, (int)configData.AvoidRateStrength, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 13, (int)configData.AvoidRateTechnique, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 14, (int)configData.AvoidRateSpeed, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 15, (int)configData.AvoidRateMind, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 16, (int)configData.PenetrateResistOfOuter, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 17, (int)configData.PenetrateResistOfInner, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 18, (int)configData.RecoveryOfStance, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 19, (int)configData.RecoveryOfBreath, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 20, (int)configData.MoveSpeed, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 21, (int)configData.RecoveryOfFlaw, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 22, (int)configData.CastSpeed, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 23, (int)configData.RecoveryOfBlockedAcupoint, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 24, (int)configData.WeaponSwitchSpeed, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 25, (int)configData.AttackSpeed, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 26, (int)configData.InnerRatio, false, false, false, true, false, false);
			index += MouseTip_Util.AppendAddProperty(curAddPropertyHolder, addPropertyPrefab, index, 27, (int)configData.RecoveryOfQiDisorder, false, false, false, true, false, false);
			eatingRefers.CGet<GameObject>("AddPropertySpace").SetActive(index > 0);
			curAddPropertyHolder.gameObject.SetActive(index > 0);
			for (int i = index; i < curAddPropertyHolder.childCount; i++)
			{
				curAddPropertyHolder.GetChild(i).gameObject.SetActive(false);
			}
			otherAddPropertyHolder.gameObject.SetActive(false);
			eatingRefers.CGet<TextMeshProUGUI>("Duration").text = configData.Duration.ToString();
		}
		base.RefreshPoisons(configData.InnatePoisons, itemData);
		RectTransform poisonResistHolder = eatingRefers.CGet<RectTransform>("PoisonResistHolder");
		bool showPoisonResist = configData.ResistOfHotPoison != 0 || configData.ResistOfGloomyPoison != 0 || configData.ResistOfColdPoison != 0 || configData.ResistOfRedPoison != 0 || configData.ResistOfRottenPoison != 0 || configData.ResistOfIllusoryPoison != 0;
		poisonResistHolder.gameObject.SetActive(showPoisonResist);
		bool flag7 = showPoisonResist;
		if (flag7)
		{
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(0).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfHotPoison, (int)configData.ResistOfHotPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(1).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfGloomyPoison, (int)configData.ResistOfGloomyPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(2).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfRedPoison, (int)configData.ResistOfRedPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(3).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfColdPoison, (int)configData.ResistOfColdPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(4).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfRottenPoison, (int)configData.ResistOfRottenPoison);
			MouseTip_Util.UpdatePoisonResistRefers(poisonResistHolder.GetChild(5).GetComponent<Refers>(), ECharacterPropertyDisplayType.ResistOfIllusoryPoison, (int)configData.ResistOfIllusoryPoison);
		}
		bool canUseInCombat = configData.ConsumedFeatureMedals >= 0;
		base.CGet<GameObject>("UseInCombat").SetActive(canUseInCombat);
		bool flag8 = canUseInCombat;
		if (flag8)
		{
			base.CGet<TextMeshProUGUI>("CostWisdom").text = string.Format("x{0}", configData.ConsumedFeatureMedals);
		}
		bool hasRefiningEffect = configData.RefiningEffect > -1;
		base.CGet<GameObject>("Refine").SetActive(hasRefiningEffect);
		bool flag9 = hasRefiningEffect;
		if (flag9)
		{
			RefiningEffectItem refiningEffectConfig = RefiningEffect.Instance[configData.RefiningEffect];
			RectTransform refiningEffectHolder = base.CGet<RectTransform>("RefiningEffectHolder");
			TipsRefiningEffect effectPrefab = refiningEffectHolder.GetChild(0).GetComponent<TipsRefiningEffect>();
			int matIndex = 0;
			for (sbyte itemType = 0; itemType <= 2; itemType += 1)
			{
				sbyte propertyType = 0;
				sbyte propertyValue = 0;
				switch (itemType)
				{
				case 0:
					propertyType = (sbyte)refiningEffectConfig.WeaponType;
					propertyValue = refiningEffectConfig.WeaponBonusValues[(int)configData.Grade];
					break;
				case 1:
					propertyType = (sbyte)refiningEffectConfig.ArmorType;
					propertyValue = refiningEffectConfig.ArmorBonusValues[(int)configData.Grade];
					break;
				case 2:
					propertyType = (sbyte)refiningEffectConfig.AccessoryType;
					propertyValue = refiningEffectConfig.AccessoryBonusValues[(int)configData.Grade];
					break;
				}
				bool flag10 = matIndex < refiningEffectHolder.childCount;
				TipsRefiningEffect effectUi;
				if (flag10)
				{
					effectUi = refiningEffectHolder.GetChild(matIndex).GetComponent<TipsRefiningEffect>();
				}
				else
				{
					effectUi = Object.Instantiate<TipsRefiningEffect>(effectPrefab);
					effectUi.transform.SetParent(refiningEffectHolder);
					effectUi.transform.localScale = Vector3.one;
				}
				effectUi.gameObject.SetActive(true);
				bool isPercent = itemType != 2;
				effectUi.SetData(-1, itemType, propertyType, (int)propertyValue, isPercent, true);
				matIndex++;
			}
			for (int j = matIndex; j < refiningEffectHolder.childCount; j++)
			{
				refiningEffectHolder.GetChild(j).gameObject.SetActive(false);
			}
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(refiningEffectHolder);
			});
		}
		base.RefreshHoldCount();
		base.RefreshHotkeyDisplayLockItem();
		base.ForceRebuildLayout(2U, null);
	}

	// Token: 0x06002AD9 RID: 10969 RVA: 0x0014953C File Offset: 0x0014773C
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		MaterialItem configData = Config.Material.Instance[itemDisplayData.Key.TemplateId];
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

	// Token: 0x06002ADA RID: 10970 RVA: 0x001495D4 File Offset: 0x001477D4
	private int GetEffectSlot(MaterialItem configData, EMedicineEffectType type)
	{
		bool flag = configData.PrimaryEffectType == type;
		int result;
		if (flag)
		{
			result = 1;
		}
		else
		{
			bool flag2 = configData.SecondaryEffectType == type;
			if (flag2)
			{
				result = 2;
			}
			else
			{
				result = -1;
			}
		}
		return result;
	}

	// Token: 0x06002ADB RID: 10971 RVA: 0x0014960C File Offset: 0x0014780C
	private short GetEffectValue(MaterialItem configData, int slot)
	{
		return (slot == 1) ? configData.PrimaryEffectValue : configData.SecondaryEffectValue;
	}

	// Token: 0x06002ADC RID: 10972 RVA: 0x00149630 File Offset: 0x00147830
	private EMedicineEffectSubType GetEffectSubtype(MaterialItem configData, int slot)
	{
		return (slot == 1) ? configData.PrimaryEffectSubType : configData.SecondaryEffectSubType;
	}

	// Token: 0x06002ADD RID: 10973 RVA: 0x00149654 File Offset: 0x00147854
	private short GetEffectThreshold(MaterialItem configData, int slot)
	{
		return (slot == 1) ? configData.PrimaryEffectThresholdValue : configData.SecondaryEffectThresholdValue;
	}

	// Token: 0x06002ADE RID: 10974 RVA: 0x00149678 File Offset: 0x00147878
	private sbyte GetEffectRecoverTimes(MaterialItem configData, int slot)
	{
		return (slot == 1) ? configData.PrimaryInjuryRecoveryTimes : configData.SecondaryInjuryRecoveryTimes;
	}
}
