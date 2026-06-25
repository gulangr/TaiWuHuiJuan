using System;
using System.Collections.Generic;
using Config;
using Game.Components.Character;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B71 RID: 2929
	public class EatInfoItemView : MonoBehaviour
	{
		// Token: 0x060090C9 RID: 37065 RVA: 0x00437AD8 File Offset: 0x00435CD8
		public void Setup(ItemDisplayData itemData, short duration, int curCharaId, int addPercent)
		{
			this._curCharaId = curCharaId;
			IItemConfig itemConfig = itemData.Key.GetConfig();
			this.SetupItemIcon(itemData.Key, itemConfig.Icon);
			this.gradBg.SetSprite("ui9_back_charactermenu_01_specialtitle_base_" + itemConfig.Grade.ToString(), false, null);
			this.titleTxt.text = itemConfig.Name;
			this.durationTxt.text = duration.ToString();
			for (int i = this.contentLayout.childCount - 1; i >= 0; i--)
			{
				Object.Destroy(this.contentLayout.GetChild(i).gameObject);
			}
			FoodItem foodConfig = itemConfig as FoodItem;
			bool flag = foodConfig != null;
			if (flag)
			{
				this.SetupForFood(foodConfig);
			}
			else
			{
				MedicineItem medConfig = itemConfig as MedicineItem;
				bool flag2 = medConfig != null;
				if (flag2)
				{
					this.SetupForMed(medConfig, itemData);
				}
				else
				{
					TeaWineItem teaWineConfig = itemConfig as TeaWineItem;
					bool flag3 = teaWineConfig != null;
					if (flag3)
					{
						this.SetupForTeaWine(teaWineConfig, addPercent);
					}
					else
					{
						MaterialItem materialConfig = itemConfig as MaterialItem;
						bool flag4 = materialConfig != null;
						if (!flag4)
						{
							throw new Exception(string.Format("未兼容该类型服食物：{0}", itemConfig.GetType()));
						}
						this.SetupForMaterial(materialConfig);
					}
				}
			}
		}

		// Token: 0x060090CA RID: 37066 RVA: 0x00437C28 File Offset: 0x00435E28
		private void SetupItemIcon(ItemKey itemKey, string icon)
		{
			bool isWug = EatingItems.IsWug(itemKey);
			bool flag = this.wugSkeleton != null;
			if (flag)
			{
				this.wugSkeleton.gameObject.SetActive(isWug);
				bool flag2 = isWug;
				if (flag2)
				{
					MedicineItem wugConfig = Medicine.Instance[itemKey.TemplateId];
					SkeletonDataAsset dataAsset = this.wugSkeletonDataAssets[(int)wugConfig.WugGrowthType];
					CommonUtils.SetSkeletonDataAsset(this.wugSkeleton, dataAsset, "default", "animation", true);
					string slotOrAttachmentName = InjuryEatItem.WugSkeletonSlotOrAttachmentNames[(int)wugConfig.WugGrowthType];
					string slotName = string.Format("images/{0}", slotOrAttachmentName);
					string attachmentName = string.Format("images/{0}_{1}", slotOrAttachmentName, (int)(wugConfig.WugType + 1));
					this.wugSkeleton.Skeleton.FindSlot(slotName).Attachment = this.wugSkeleton.Skeleton.GetAttachment(slotName, attachmentName);
					this.itemIcon.gameObject.SetActive(false);
					return;
				}
			}
			this.itemIcon.gameObject.SetActive(true);
			this.itemIcon.SetSprite(icon, false, null);
		}

		// Token: 0x060090CB RID: 37067 RVA: 0x00437D40 File Offset: 0x00435F40
		private void SetupForTeaWine(TeaWineItem teaWineConfig, int addPercent)
		{
			List<ValueTuple<string, string, int>> cellDataList2 = this.GetMainPropDataTeaWine(teaWineConfig, addPercent);
			bool flag = cellDataList2 != null && cellDataList2.Count > 0;
			if (flag)
			{
				this.AddMainPropContent(cellDataList2);
			}
		}

		// Token: 0x060090CC RID: 37068 RVA: 0x00437D74 File Offset: 0x00435F74
		private void SetupForMaterial(MaterialItem materialConfig)
		{
			List<ValueTuple<string, string, int>> cellDataList = this.GetMainPropDataMaterial(materialConfig);
			bool flag = cellDataList != null && cellDataList.Count > 0;
			if (flag)
			{
				this.AddMainPropContent(cellDataList);
			}
			bool flag2 = materialConfig.BaseMaxHealthDelta > 0;
			if (flag2)
			{
				this.AddDescContent(LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_Health_Instant, materialConfig.BaseMaxHealthDelta.ToString()).ColorReplace());
			}
			this.RefreshMaterialEatEffect(materialConfig, 1);
			this.RefreshMaterialEatEffect(materialConfig, 2);
		}

		// Token: 0x060090CD RID: 37069 RVA: 0x00437DE8 File Offset: 0x00435FE8
		private void RefreshMaterialEatEffect(MaterialItem config, int slot)
		{
			EMedicineEffectType effectType = (slot == 1) ? config.PrimaryEffectType : config.SecondaryEffectType;
			bool flag = effectType == EMedicineEffectType.Invalid;
			if (!flag)
			{
				short effectValue = (slot == 1) ? config.PrimaryEffectValue : config.SecondaryEffectValue;
				short threshold = (slot == 1) ? config.PrimaryEffectThresholdValue : config.SecondaryEffectThresholdValue;
				sbyte recoveryTimes = (slot == 1) ? config.PrimaryInjuryRecoveryTimes : config.SecondaryInjuryRecoveryTimes;
				EMedicineEffectSubType subType = (slot == 1) ? config.PrimaryEffectSubType : config.SecondaryEffectSubType;
				switch (effectType)
				{
				case EMedicineEffectType.RecoverOuterInjury:
					this.RefreshMaterialInjury(true, (int)effectValue, recoveryTimes, threshold);
					break;
				case EMedicineEffectType.RecoverInnerInjury:
					this.RefreshMaterialInjury(false, (int)effectValue, recoveryTimes, threshold);
					break;
				case EMedicineEffectType.RecoverHealth:
					this.AddDescContent(LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_Health_Instant, effectValue.ToString()).ColorReplace());
					break;
				case EMedicineEffectType.ChangeDisorderOfQi:
					this.AddDescContent(LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_QiDisorder_Instant, ((int)(effectValue / 10)).ToString()).ColorReplace());
					break;
				case EMedicineEffectType.DetoxPoison:
					this.RefreshMaterialDetoxPoison((int)effectValue, threshold, subType);
					break;
				}
			}
		}

		// Token: 0x060090CE RID: 37070 RVA: 0x00437EF4 File Offset: 0x004360F4
		private void RefreshMaterialInjury(bool isOuter, int effectValue, sbyte recoveryTimes, short threshold)
		{
			LanguageKey tipKey = isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Instant_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Instant_Inner;
			this.AddDescContent(LocalStringManager.GetFormat(tipKey, recoveryTimes, effectValue, threshold).ColorReplace());
		}

		// Token: 0x060090CF RID: 37071 RVA: 0x00437F38 File Offset: 0x00436138
		private void RefreshMaterialDetoxPoison(int effectValue, short threshold, EMedicineEffectSubType subType)
		{
			sbyte poisonType = subType.PoisonType();
			PoisonItem poisonConfig = Poison.Instance[poisonType];
			string maxPoisonLevelTips = LocalStringManager.GetFormat(LanguageKey.LK_Poison_Max_Level_Tips, LocalStringManager.Get(string.Format("LK_Poison_Level_{0}", threshold)));
			string typeName = poisonConfig.Name.SetColor(poisonConfig.FontColor);
			string content = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_HealPoison, typeName, maxPoisonLevelTips, effectValue.ToString()).ColorReplace();
			this.AddDescContent(content);
		}

		// Token: 0x060090D0 RID: 37072 RVA: 0x00437FB0 File Offset: 0x004361B0
		private void SetupForFood(FoodItem foodConfig)
		{
			List<ValueTuple<string, string, int>> cellDataList = this.GetMainPropRegenData(foodConfig.MainAttributesRegen);
			bool flag = cellDataList != null && cellDataList.Count > 0;
			if (flag)
			{
				this.AddMainPropRegenContent(cellDataList);
			}
			List<ValueTuple<string, string, int>> cellDataList2 = this.GetMainPropDataFood(foodConfig);
			bool flag2 = cellDataList2 != null && cellDataList2.Count > 0;
			if (flag2)
			{
				this.AddMainPropContent(cellDataList2);
			}
		}

		// Token: 0x060090D1 RID: 37073 RVA: 0x00438010 File Offset: 0x00436210
		private void SetupForMed(MedicineItem medConfig, ItemDisplayData itemData)
		{
			List<ValueTuple<string, string, int>> cellDataList2 = this.GetMainPropDataMed(medConfig);
			bool flag = cellDataList2 != null && cellDataList2.Count > 0;
			if (flag)
			{
				this.AddMainPropContent(cellDataList2);
			}
			this.RefreshEatEffect(medConfig, itemData);
			this.RefreshSelfPoisons(medConfig, itemData);
			bool isWugKing = EatingItems.IsWugKing(itemData.Key);
			bool flag2 = isWugKing;
			if (flag2)
			{
				this.RefreshWugKing(itemData.Key);
			}
			bool isWug = EatingItems.IsWug(itemData.Key);
			bool flag3 = isWug && !isWugKing;
			if (flag3)
			{
				this.AddDescContent(medConfig.SpecialEffectDesc.ColorReplace());
				string killTip = TipWugKingEffect.GetKillWugTip(medConfig);
				bool flag4 = !string.IsNullOrEmpty(killTip);
				if (flag4)
				{
					this.AddDescContent(killTip);
				}
			}
		}

		// Token: 0x060090D2 RID: 37074 RVA: 0x004380C8 File Offset: 0x004362C8
		private void RefreshEatEffect(MedicineItem configData, ItemDisplayData itemData)
		{
			bool isPoisonEffect = configData.EffectType == EMedicineEffectType.ApplyPoison;
			bool showEatEffect = configData.HasNormalEatingEffect && !isPoisonEffect;
			bool hasDamageStep = configData.DamageStepBonus > 0;
			bool flag = !showEatEffect;
			if (!flag)
			{
				int effectValue = itemData.MedicineEffectValue;
				bool isInstant = configData.InstantAffect;
				bool isMonthly = configData.Duration != 0;
				switch (configData.EffectType)
				{
				case EMedicineEffectType.RecoverOuterInjury:
				case EMedicineEffectType.RecoverInnerInjury:
				{
					bool isOuter = configData.EffectType == EMedicineEffectType.RecoverOuterInjury;
					this.RefreshInjury(isInstant, isMonthly, isOuter, effectValue, configData);
					break;
				}
				case EMedicineEffectType.RecoverHealth:
					this.RefreshHealth(isInstant, isMonthly, effectValue, configData);
					break;
				case EMedicineEffectType.ChangeDisorderOfQi:
					this.RefreshQiDisorder(isInstant, isMonthly, effectValue, configData);
					break;
				case EMedicineEffectType.DetoxPoison:
					this.RefreshDetoxPoison(isInstant, isMonthly, effectValue, configData);
					break;
				}
			}
		}

		// Token: 0x060090D3 RID: 37075 RVA: 0x00438198 File Offset: 0x00436398
		private void RefreshInjury(bool isInstant, bool isMonthly, bool isOuter, int effectValue, MedicineItem configData)
		{
			if (isInstant)
			{
				LanguageKey eatHealInjuryInstantTipKey = isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Instant_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Instant_Inner;
				string content = LocalStringManager.GetFormat(eatHealInjuryInstantTipKey, configData.InjuryRecoveryTimes, effectValue, configData.EffectThresholdValue).ColorReplace();
				string title = (isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Instant_PreTitle_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Instant_PreTitle_Inner).Tr().ColorReplace();
				this.AddDescContent(content);
			}
			if (isMonthly)
			{
				LanguageKey eatHealInjuryMonthlyTipKey = isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_Inner;
				string content2 = LocalStringManager.GetFormat(eatHealInjuryMonthlyTipKey, configData.InjuryRecoveryTimes, effectValue, configData.EffectThresholdValue).ColorReplace();
				string title2 = (isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_PreTitle_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_PreTitle_Inner).Tr().ColorReplace();
				this.AddDescContent(content2);
			}
			LanguageKey eatInjuryMonthlyDamageStepTipKey = isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_DamageStep_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_DamageStep_Inner;
			string step = LocalStringManager.GetFormat(eatInjuryMonthlyDamageStepTipKey, configData.DamageStepBonus).ColorReplace();
			this.AddDescContent(step);
		}

		// Token: 0x060090D4 RID: 37076 RVA: 0x004382A8 File Offset: 0x004364A8
		private void RefreshHealth(bool isInstant, bool isMonthly, int effectValue, MedicineItem configData)
		{
			string valueText = configData.EffectIsPercentage ? string.Format("{0}%", effectValue) : string.Format("{0}", effectValue);
			if (isInstant)
			{
				string content = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_Health_Instant, valueText).ColorReplace();
				this.AddDescContent(content);
			}
			if (isMonthly)
			{
				string content2 = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_Health_Monthly, valueText).ColorReplace();
				this.AddDescContent(content2);
			}
			string step = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_Health_Monthly_DamageStep, configData.DamageStepBonus).ColorReplace();
			this.AddDescContent(step);
		}

		// Token: 0x060090D5 RID: 37077 RVA: 0x0043834C File Offset: 0x0043654C
		private void RefreshQiDisorder(bool isInstant, bool isMonthly, int effectValue, MedicineItem configData)
		{
			string valueText = configData.EffectIsPercentage ? string.Format("{0}%", effectValue) : string.Format("{0}", effectValue / 10);
			if (isInstant)
			{
				string content = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_QiDisorder_Instant, valueText).ColorReplace();
				this.AddDescContent(content);
			}
			if (isMonthly)
			{
				string content2 = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_QiDisorder_Monthly, valueText).ColorReplace();
				this.AddDescContent(content2);
			}
			string step = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_QiDisorder_Monthly_DamageStep, configData.DamageStepBonus).ColorReplace();
			this.AddDescContent(step);
		}

		// Token: 0x060090D6 RID: 37078 RVA: 0x004383F4 File Offset: 0x004365F4
		private void RefreshDetoxPoison(bool isInstant, bool isMonthly, int effectValue, MedicineItem configData)
		{
			sbyte poisonType = configData.PoisonType;
			PoisonItem poisonConfig = Poison.Instance[poisonType];
			string maxPoisonLevelTips = LocalStringManager.GetFormat(LanguageKey.LK_Poison_Max_Level_Tips, LocalStringManager.Get(string.Format("LK_Poison_Level_{0}", configData.EffectThresholdValue)));
			string valueText = configData.EffectIsPercentage ? string.Format("{0}%", effectValue) : string.Format("{0}", effectValue);
			string typeName = poisonConfig.Name.SetColor(poisonConfig.FontColor);
			int num = isInstant ? 6573 : 6574;
			string content = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_HealPoison, typeName, maxPoisonLevelTips, valueText).ColorReplace();
			this.AddDescContent(content);
		}

		// Token: 0x060090D7 RID: 37079 RVA: 0x004384B0 File Offset: 0x004366B0
		private void AddDescContent(string content)
		{
			EatInfoItemDescContentTemplate gridContent = Object.Instantiate<EatInfoItemDescContentTemplate>(this.descContentTemplate, this.contentLayout);
			gridContent.gameObject.SetActive(true);
			gridContent.Setup(LanguageKey.LK_CharacterMenu_EatDetail_Other_Title.Tr(), content);
		}

		// Token: 0x060090D8 RID: 37080 RVA: 0x004384F0 File Offset: 0x004366F0
		private void AddDescContent(string content, string title)
		{
			EatInfoItemDescContentTemplate gridContent = Object.Instantiate<EatInfoItemDescContentTemplate>(this.descContentTemplate, this.contentLayout);
			gridContent.gameObject.SetActive(true);
			gridContent.Setup(title, content);
		}

		// Token: 0x060090D9 RID: 37081 RVA: 0x00438526 File Offset: 0x00436726
		private void AddMainPropRegenContent(List<ValueTuple<string, string, int>> cellDataList)
		{
			this.AddMainPropContent(cellDataList);
		}

		// Token: 0x060090DA RID: 37082 RVA: 0x00438533 File Offset: 0x00436733
		private void AddMainPropContent(List<ValueTuple<string, string, int>> cellDataList)
		{
			this.AddGridContent(LanguageKey.LK_CharacterMenu_EatDetail_PropBuff_Title, cellDataList);
		}

		// Token: 0x060090DB RID: 37083 RVA: 0x00438544 File Offset: 0x00436744
		private void AddGridContent(LanguageKey titleKey, List<ValueTuple<string, string, int>> cellDataList)
		{
			EatInfoItemGridContentTemplate gridContent = Object.Instantiate<EatInfoItemGridContentTemplate>(this.gridContentTemplate, this.contentLayout);
			gridContent.gameObject.SetActive(true);
			gridContent.Setup(titleKey.Tr(), cellDataList);
		}

		// Token: 0x060090DC RID: 37084 RVA: 0x00438580 File Offset: 0x00436780
		protected unsafe void RefreshSelfPoisons(MedicineItem configData, ItemDisplayData itemData)
		{
			PoisonsAndLevels innatePoisons = default(PoisonsAndLevels);
			innatePoisons.Initialize();
			bool flag = configData.EffectType == EMedicineEffectType.ApplyPoison;
			if (flag)
			{
				sbyte type = configData.PoisonType;
				*(ref innatePoisons.Values.FixedElementField + (IntPtr)type * 2) = (short)((sbyte)itemData.MedicineEffectValue);
				*(ref innatePoisons.Levels.FixedElementField + type) = (sbyte)configData.EffectThresholdValue;
			}
			bool flag2 = !innatePoisons.IsNonZero();
			if (!flag2)
			{
				List<ValueTuple<string, string, int>> dataList = new List<ValueTuple<string, string, int>>();
				for (sbyte order = 0; order < 6; order += 1)
				{
					sbyte type2 = PoisonType.GetTypeBySortingOrder(order);
					PoisonItem poisonTypeConfig = Poison.Instance[type2];
					short innatePoisonValue = *(ref innatePoisons.Values.FixedElementField + (IntPtr)type2 * 2);
					sbyte innatePoisonLevel = *(ref innatePoisons.Levels.FixedElementField + type2);
					bool show = innatePoisonValue > 0;
					bool flag3 = show;
					if (flag3)
					{
						dataList.Add(new ValueTuple<string, string, int>(poisonTypeConfig.Name, "ui9_back_poison_big_7_" + type2.ToString(), (int)innatePoisonValue));
					}
				}
				this.AddGridContent(LanguageKey.LK_PoisonInjury_Tip_Title, dataList);
			}
		}

		// Token: 0x060090DD RID: 37085 RVA: 0x004386A0 File Offset: 0x004368A0
		public void RefreshWugKing(ItemKey itemKey)
		{
			bool isWugKing = EatingItems.IsWugKing(itemKey);
			base.gameObject.SetActive(isWugKing);
			bool flag = !isWugKing;
			if (!flag)
			{
				EatInfoItemWugKingContentTemplate wugContent = Object.Instantiate<EatInfoItemWugKingContentTemplate>(this.wugKingContentTemplate, this.contentLayout);
				wugContent.Setup(LanguageKey.LK_CharacterMenu_EatDetail_WugKingEffect.Tr(), itemKey, this._curCharaId);
			}
		}

		// Token: 0x060090DE RID: 37086 RVA: 0x004386F8 File Offset: 0x004368F8
		private unsafe List<ValueTuple<string, string, int>> GetMainPropRegenData(MainAttributes mainAttributesRegen)
		{
			List<ValueTuple<string, string, int>> dataList = new List<ValueTuple<string, string, int>>();
			for (sbyte i = 0; i < 6; i += 1)
			{
				short value = *(ref mainAttributesRegen.Items.FixedElementField + (IntPtr)i * 2);
				bool flag = value == 0;
				if (!flag)
				{
					CharacterPropertyDisplayItem propRefer = CharacterPropertyDisplay.Instance[(short)i];
					dataList.Add(new ValueTuple<string, string, int>(Attribute.GetMainAttributeName(i), Attribute.GetMainAttributeIcon(i), (int)value));
				}
			}
			return dataList;
		}

		// Token: 0x060090DF RID: 37087 RVA: 0x0043876C File Offset: 0x0043696C
		private List<ValueTuple<string, string, int>> GetMainPropDataFood(FoodItem config)
		{
			List<ValueTuple<string, string, int>> dataList = new List<ValueTuple<string, string, int>>();
			this.AddCellData(dataList, ECharacterPropertyReferencedType.Strength, (int)config.Strength, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.Dexterity, (int)config.Dexterity, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.Concentration, (int)config.Concentration, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.Vitality, (int)config.Vitality, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.Energy, (int)config.Energy, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.Intelligence, (int)config.Intelligence, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateStrength, (int)config.HitRateStrength, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateTechnique, (int)config.HitRateTechnique, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateSpeed, (int)config.HitRateSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateMind, (int)config.HitRateMind, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateOfOuter, (int)config.PenetrateOfOuter, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateOfInner, (int)config.PenetrateOfInner, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateStrength, (int)config.AvoidRateStrength, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateTechnique, (int)config.AvoidRateTechnique, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateSpeed, (int)config.AvoidRateSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateMind, (int)config.AvoidRateMind, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateResistOfOuter, (int)config.PenetrateResistOfOuter, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateResistOfInner, (int)config.PenetrateResistOfInner, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfStance, (int)config.RecoveryOfStance, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfBreath, (int)config.RecoveryOfBreath, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.MoveSpeed, (int)config.MoveSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfFlaw, (int)config.RecoveryOfFlaw, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.CastSpeed, (int)config.CastSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfBlockedAcupoint, (int)config.RecoveryOfBlockedAcupoint, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.WeaponSwitchSpeed, (int)config.WeaponSwitchSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AttackSpeed, (int)config.AttackSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.InnerRatio, (int)config.InnerRatio, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfQiDisorder, (int)config.RecoveryOfQiDisorder, 0);
			return dataList;
		}

		// Token: 0x060090E0 RID: 37088 RVA: 0x00438958 File Offset: 0x00436B58
		private List<ValueTuple<string, string, int>> GetMainPropDataMed(MedicineItem config)
		{
			List<ValueTuple<string, string, int>> dataList = new List<ValueTuple<string, string, int>>();
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateStrength, (int)config.HitRateStrength, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateTechnique, (int)config.HitRateTechnique, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateSpeed, (int)config.HitRateSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateMind, (int)config.HitRateMind, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateOfOuter, (int)config.PenetrateOfOuter, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateOfInner, (int)config.PenetrateOfInner, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateStrength, (int)config.AvoidRateStrength, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateTechnique, (int)config.AvoidRateTechnique, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateSpeed, (int)config.AvoidRateSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateMind, (int)config.AvoidRateMind, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateResistOfOuter, (int)config.PenetrateResistOfOuter, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateResistOfInner, (int)config.PenetrateResistOfInner, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfStance, (int)config.RecoveryOfStance, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfBreath, (int)config.RecoveryOfBreath, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.MoveSpeed, (int)config.MoveSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfFlaw, (int)config.RecoveryOfFlaw, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.CastSpeed, (int)config.CastSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfBlockedAcupoint, (int)config.RecoveryOfBlockedAcupoint, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.WeaponSwitchSpeed, (int)config.WeaponSwitchSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AttackSpeed, (int)config.AttackSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.InnerRatio, (int)config.InnerRatio, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfQiDisorder, (int)config.RecoveryOfQiDisorder, 0);
			return dataList;
		}

		// Token: 0x060090E1 RID: 37089 RVA: 0x00438AE4 File Offset: 0x00436CE4
		private List<ValueTuple<string, string, int>> GetMainPropDataTeaWine(TeaWineItem config, int addPercent)
		{
			List<ValueTuple<string, string, int>> dataList = new List<ValueTuple<string, string, int>>();
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateStrength, (int)config.HitRateStrength, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateTechnique, (int)config.HitRateTechnique, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateSpeed, (int)config.HitRateSpeed, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateMind, (int)config.HitRateMind, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateOfOuter, (int)config.PenetrateOfOuter, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateOfInner, (int)config.PenetrateOfInner, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateStrength, (int)config.AvoidRateStrength, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateTechnique, (int)config.AvoidRateTechnique, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateSpeed, (int)config.AvoidRateSpeed, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateMind, (int)config.AvoidRateMind, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateResistOfOuter, (int)config.PenetrateResistOfOuter, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateResistOfInner, (int)config.PenetrateResistOfInner, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.InnerRatio, (int)config.InnerRatio, addPercent);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfQiDisorder, (int)config.RecoveryOfQiDisorder, addPercent);
			return dataList;
		}

		// Token: 0x060090E2 RID: 37090 RVA: 0x00438BE8 File Offset: 0x00436DE8
		private List<ValueTuple<string, string, int>> GetMainPropDataMaterial(MaterialItem config)
		{
			List<ValueTuple<string, string, int>> dataList = new List<ValueTuple<string, string, int>>();
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateStrength, (int)config.HitRateStrength, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateTechnique, (int)config.HitRateTechnique, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateSpeed, (int)config.HitRateSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.HitRateMind, (int)config.HitRateMind, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateOfOuter, (int)config.PenetrateOfOuter, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateOfInner, (int)config.PenetrateOfInner, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateStrength, (int)config.AvoidRateStrength, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateTechnique, (int)config.AvoidRateTechnique, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateSpeed, (int)config.AvoidRateSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AvoidRateMind, (int)config.AvoidRateMind, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateResistOfOuter, (int)config.PenetrateResistOfOuter, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.PenetrateResistOfInner, (int)config.PenetrateResistOfInner, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfStance, (int)config.RecoveryOfStance, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfBreath, (int)config.RecoveryOfBreath, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.MoveSpeed, (int)config.MoveSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfFlaw, (int)config.RecoveryOfFlaw, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.CastSpeed, (int)config.CastSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfBlockedAcupoint, (int)config.RecoveryOfBlockedAcupoint, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.WeaponSwitchSpeed, (int)config.WeaponSwitchSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.AttackSpeed, (int)config.AttackSpeed, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.InnerRatio, (int)config.InnerRatio, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.RecoveryOfQiDisorder, (int)config.RecoveryOfQiDisorder, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.ResistOfHotPoison, (int)config.ResistOfHotPoison, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.ResistOfGloomyPoison, (int)config.ResistOfGloomyPoison, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.ResistOfRedPoison, (int)config.ResistOfRedPoison, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.ResistOfColdPoison, (int)config.ResistOfColdPoison, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.ResistOfRottenPoison, (int)config.ResistOfRottenPoison, 0);
			this.AddCellData(dataList, ECharacterPropertyReferencedType.ResistOfIllusoryPoison, (int)config.ResistOfIllusoryPoison, 0);
			return dataList;
		}

		// Token: 0x060090E3 RID: 37091 RVA: 0x00438DDC File Offset: 0x00436FDC
		private void AddCellData(List<ValueTuple<string, string, int>> dataList, ECharacterPropertyReferencedType type, int value, int percent = 0)
		{
			bool flag = value == 0;
			if (!flag)
			{
				bool flag2 = percent > 0 && value > 0;
				if (flag2)
				{
					value = value * (100 + percent) / 100;
				}
				string nameStr;
				string iconName;
				CommonUtils.GetCharacterPropReferenceDisplayInfo((short)type, out nameStr, out iconName);
				dataList.Add(new ValueTuple<string, string, int>(nameStr, iconName, value));
			}
		}

		// Token: 0x04006F81 RID: 28545
		[SerializeField]
		private CImage itemIcon;

		// Token: 0x04006F82 RID: 28546
		[SerializeField]
		private SkeletonGraphic wugSkeleton;

		// Token: 0x04006F83 RID: 28547
		[SerializeField]
		private SkeletonDataAsset[] wugSkeletonDataAssets;

		// Token: 0x04006F84 RID: 28548
		[SerializeField]
		private CImage gradBg;

		// Token: 0x04006F85 RID: 28549
		[SerializeField]
		private TextMeshProUGUI titleTxt;

		// Token: 0x04006F86 RID: 28550
		[SerializeField]
		private TextMeshProUGUI durationTxt;

		// Token: 0x04006F87 RID: 28551
		[SerializeField]
		private RectTransform contentLayout;

		// Token: 0x04006F88 RID: 28552
		[SerializeField]
		private EatInfoItemGridContentTemplate gridContentTemplate;

		// Token: 0x04006F89 RID: 28553
		[SerializeField]
		private EatInfoItemDescContentTemplate descContentTemplate;

		// Token: 0x04006F8A RID: 28554
		[SerializeField]
		private EatInfoItemWugKingContentTemplate wugKingContentTemplate;

		// Token: 0x04006F8B RID: 28555
		private int _curCharaId;
	}
}
