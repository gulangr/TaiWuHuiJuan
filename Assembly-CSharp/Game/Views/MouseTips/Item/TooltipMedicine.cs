using System;
using System.Collections.Generic;
using System.Linq;
using CharacterDataMonitor;
using Config;
using FrameWork;
using Game.Views.MouseTips.Common;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x020008A7 RID: 2215
	public class TooltipMedicine : TooltipItemBase
	{
		// Token: 0x17000C93 RID: 3219
		// (get) Token: 0x060069EF RID: 27119 RVA: 0x0030D582 File Offset: 0x0030B782
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060069F0 RID: 27120 RVA: 0x0030D588 File Offset: 0x0030B788
		protected unsafe override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			bool flag = !argsBox.Get("CharId", out this._charId);
			if (flag)
			{
				this._charId = -1;
			}
			this._itemKey = this._itemData.RealKey;
			this.configData = Medicine.Instance[this._itemKey.TemplateId];
			base.Init(argsBox);
			this.InnatePoisons.Initialize();
			bool flag2 = this.configData.EffectType == EMedicineEffectType.ApplyPoison;
			if (flag2)
			{
				sbyte type = this.configData.PoisonType;
				*(ref this.InnatePoisons.Values.FixedElementField + (IntPtr)type * 2) = this.configData.EffectValue;
				*(ref this.InnatePoisons.Levels.FixedElementField + type) = (sbyte)this.configData.EffectThresholdValue;
			}
			this._isWug = EatingItems.IsWug(this._itemData.Key);
			this._isWugKing = EatingItems.IsWugKing(this._itemData.Key);
			bool hasDamageStep = this.configData.DamageStepBonus > 0;
			this.DisableDetail = (!this.HasSelfPoison && !this.HasAttachedPoison && !hasDamageStep && !this._isWugKing);
			base.PostInit();
			this.Refresh();
		}

		// Token: 0x060069F1 RID: 27121 RVA: 0x0030D6E9 File Offset: 0x0030B8E9
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x060069F2 RID: 27122 RVA: 0x0030D6FC File Offset: 0x0030B8FC
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshEatEffect();
			this.RefreshTime();
			this.RefreshAddProperty();
			this.RefreshCombat();
			this.RefreshCostAttribute();
			this.RefreshWugKing();
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
			CanvasGroup canvasGroup = base.gameObject.GetOrAddComponent<CanvasGroup>();
			canvasGroup.alpha = 0f;
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(3U, delegate
			{
				canvasGroup.alpha = 1f;
			});
		}

		// Token: 0x060069F3 RID: 27123 RVA: 0x0030D78D File Offset: 0x0030B98D
		protected override void OnDisable()
		{
			base.OnDisable();
			EatingItemMonitor eatingItemMonitor = this._eatingItemMonitor;
			if (eatingItemMonitor != null)
			{
				eatingItemMonitor.RemoveEatingItemListener(new Action(this.RefreshWugKingEffectState));
			}
			this._eatingItemMonitor = null;
		}

		// Token: 0x060069F4 RID: 27124 RVA: 0x0030D7BC File Offset: 0x0030B9BC
		protected override void RefreshSpecialArea()
		{
			bool isWug = this._isWug;
			if (isWug)
			{
				this.specialArea.RefreshWug((int)this.EatingTime, TooltipMedicine.GetKillWugTip(this.configData));
			}
			else
			{
				base.RefreshSpecialArea();
			}
		}

		// Token: 0x060069F5 RID: 27125 RVA: 0x0030D7FC File Offset: 0x0030B9FC
		private void RefreshEatEffect()
		{
			bool isPoisonEffect = this.configData.EffectType == EMedicineEffectType.ApplyPoison;
			bool showEatEffect = this.configData.HasNormalEatingEffect && !isPoisonEffect;
			this.rootEatArea.SetActive(showEatEffect);
			bool hasDamageStep = this.configData.DamageStepBonus > 0;
			this.propertyDetailDamageStep.gameObject.SetActive(hasDamageStep);
			this.propertyDamageStep.gameObject.SetActive(hasDamageStep);
			bool flag = !showEatEffect;
			if (!flag)
			{
				int effectValue = this._templateDataOnly ? ((int)this.configData.EffectValue) : this._itemData.MedicineEffectValue;
				bool isInstant = this.configData.InstantAffect;
				bool isMonthly = this.configData.Duration != 0;
				this.textEatTitle.text = (isMonthly ? LanguageKey.LK_ItemTips_Medicine_Eat_Effect_Title.Tr() : LanguageKey.LK_ItemTips_Medicine_Use_Effect_Title.Tr());
				switch (this.configData.EffectType)
				{
				case EMedicineEffectType.RecoverOuterInjury:
				case EMedicineEffectType.RecoverInnerInjury:
				{
					bool isOuter = this.configData.EffectType == EMedicineEffectType.RecoverOuterInjury;
					this.RefreshInjury(isInstant, isMonthly, isOuter, effectValue);
					break;
				}
				case EMedicineEffectType.RecoverHealth:
					this.RefreshHealth(isInstant, isMonthly, effectValue);
					break;
				case EMedicineEffectType.ChangeDisorderOfQi:
					this.RefreshQiDisorder(isInstant, isMonthly, effectValue);
					break;
				case EMedicineEffectType.DetoxPoison:
					this.RefreshDetoxPoison(isInstant, isMonthly, effectValue);
					break;
				}
				EMedicineEffectType effectType = this.configData.EffectType;
				bool isHealInjury = effectType == EMedicineEffectType.RecoverInnerInjury || effectType <= EMedicineEffectType.RecoverOuterInjury;
				bool isDetoxPoison = this.configData.EffectType == EMedicineEffectType.DetoxPoison;
				bool isShowTitle = isHealInjury || isDetoxPoison;
				this.propertyHealTitle.gameObject.SetActive(isShowTitle);
				this.rootSubHealInjury.SetActive(isHealInjury);
				this.rootSubDetoxPoison.SetActive(isDetoxPoison);
				effectType = this.configData.EffectType;
				bool isShowContent = effectType == EMedicineEffectType.RecoverHealth || effectType == EMedicineEffectType.ChangeDisorderOfQi;
				this.propertyHealContent.gameObject.SetActive(isShowContent);
			}
		}

		// Token: 0x060069F6 RID: 27126 RVA: 0x0030D9F4 File Offset: 0x0030BBF4
		private void RefreshInjury(bool isInstant, bool isMonthly, bool isOuter, int effectValue)
		{
			if (isInstant)
			{
				LanguageKey eatHealInjuryInstantTipKey = isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Instant_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Instant_Inner;
				string content = LocalStringManager.GetFormat(eatHealInjuryInstantTipKey, this.configData.InjuryRecoveryTimes, effectValue, this.configData.EffectThresholdValue).ColorReplace();
				this.propertySubHealInjuryContent.SetValue(content);
				string title = (isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Instant_PreTitle_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Instant_PreTitle_Inner).Tr().ColorReplace();
				this.propertyHealTitle.SetValue(title);
			}
			if (isMonthly)
			{
				LanguageKey eatHealInjuryMonthlyTipKey = isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_Inner;
				string content2 = LocalStringManager.GetFormat(eatHealInjuryMonthlyTipKey, this.configData.InjuryRecoveryTimes, effectValue, this.configData.EffectThresholdValue).ColorReplace();
				this.propertySubHealInjuryContent.SetValue(content2);
				string title2 = (isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_PreTitle_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_PreTitle_Inner).Tr().ColorReplace();
				this.propertyHealTitle.SetValue(title2);
			}
			LanguageKey eatInjuryMonthlyDamageStepTipKey = isOuter ? LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_DamageStep_Outer : LanguageKey.LK_ItemTips_MedicineEffect_Injury_Monthly_DamageStep_Inner;
			string step = LocalStringManager.GetFormat(eatInjuryMonthlyDamageStepTipKey, this.configData.DamageStepBonus).ColorReplace();
			this.propertyDamageStep.SetValue(step);
		}

		// Token: 0x060069F7 RID: 27127 RVA: 0x0030DB44 File Offset: 0x0030BD44
		private void RefreshHealth(bool isInstant, bool isMonthly, int effectValue)
		{
			string valueText = this.configData.EffectIsPercentage ? string.Format("{0}%", effectValue) : string.Format("{0}", effectValue);
			if (isInstant)
			{
				string content = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_Health_Instant, valueText).ColorReplace();
				this.propertyHealContent.SetValue(content);
			}
			if (isMonthly)
			{
				string content2 = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_Health_Monthly, valueText).ColorReplace();
				this.propertyHealContent.SetValue(content2);
			}
			string step = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_Health_Monthly_DamageStep, this.configData.DamageStepBonus).ColorReplace();
			this.propertyDamageStep.SetValue(step);
		}

		// Token: 0x060069F8 RID: 27128 RVA: 0x0030DC00 File Offset: 0x0030BE00
		private void RefreshQiDisorder(bool isInstant, bool isMonthly, int effectValue)
		{
			string valueText = this.configData.EffectIsPercentage ? string.Format("{0}%", effectValue) : string.Format("{0}", effectValue / 10);
			if (isInstant)
			{
				string content = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_QiDisorder_Instant, valueText).ColorReplace();
				this.propertyHealContent.SetValue(content);
			}
			if (isMonthly)
			{
				string content2 = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_QiDisorder_Monthly, valueText).ColorReplace();
				this.propertyHealContent.SetValue(content2);
			}
			string step = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_QiDisorder_Monthly_DamageStep, this.configData.DamageStepBonus).ColorReplace();
			this.propertyDamageStep.SetValue(step);
		}

		// Token: 0x060069F9 RID: 27129 RVA: 0x0030DCBC File Offset: 0x0030BEBC
		private void RefreshDetoxPoison(bool isInstant, bool isMonthly, int effectValue)
		{
			sbyte poisonType = this.configData.PoisonType;
			PoisonItem poisonConfig = Poison.Instance[poisonType];
			string maxPoisonLevelTips = LocalStringManager.GetFormat(LanguageKey.LK_Poison_Max_Level_Tips, LocalStringManager.Get(string.Format("LK_Poison_Level_{0}", this.configData.EffectThresholdValue)));
			string valueText = this.configData.EffectIsPercentage ? string.Format("{0}%", effectValue) : string.Format("{0}", effectValue);
			string typeName = poisonConfig.Name.SetColor(poisonConfig.FontColor);
			string typeIcon = TooltipItemPoisonArea.GetPoisonTypeIcon(poisonType);
			LanguageKey titleKey = isInstant ? LanguageKey.LK_ItemTips_MedicineEffect_HealPoison_Instant_PreTitle : LanguageKey.LK_ItemTips_MedicineEffect_HealPoison_Monthly_PreTitle;
			string title = titleKey.TrFormat(typeIcon, typeName);
			this.propertyHealTitle.SetValue(title);
			short poisonLevel = this.configData.EffectThresholdValue;
			int categoryIconIndex = TooltipCombatSkill.GetPoisonLevelIconIndex((sbyte)poisonLevel);
			string levelIcon = TooltipItemPoisonArea.GetPoisonLevelIcon(categoryIconIndex);
			string value = effectValue.ToString().SetColor("specialyellow");
			this.propertySubDetoxPoison.Set(typeName, value, false, typeIcon, levelIcon, (int)poisonLevel);
			string content = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_MedicineEffect_HealPoison, typeName, maxPoisonLevelTips, valueText).ColorReplace();
			this.propertySubDetoxPoisonContent.SetValue(content);
		}

		// Token: 0x060069FA RID: 27130 RVA: 0x0030DDF0 File Offset: 0x0030BFF0
		private void RefreshTime()
		{
			this.rootTime.SetActive(this.configData.Duration > 0);
			this.propertyTime.SetValue(this.configData.Duration.ToString().SetColor("brightyellow"));
		}

		// Token: 0x060069FB RID: 27131 RVA: 0x0030DE40 File Offset: 0x0030C040
		private void RefreshAddProperty()
		{
			bool isWug = this._isWug;
			if (isWug)
			{
				this.layoutAddProperty.gameObject.SetActive(false);
			}
			else
			{
				List<TooltipItemProperty> list = this.layoutAddProperty.GetComponentsInChildren<TooltipItemProperty>(true).ToList<TooltipItemProperty>();
				int index = 0;
				for (ECharacterPropertyReferencedType type = ECharacterPropertyReferencedType.HitRateStrength; type <= ECharacterPropertyReferencedType.ResistOfIllusoryPoison; type++)
				{
					int baseValue = this.GetAddPropertyBaseValue(type);
					TooltipUtil.AppendAddProperty(ref index, list, (short)type, baseValue, baseValue, this.IsDetail, false, this.configData.EffectIsPercentage, false, true, true, false, false);
				}
				for (int i = index; i < list.Count; i++)
				{
					list[i].gameObject.SetActive(false);
				}
				this.layoutAddProperty.gameObject.SetActive(index > 0);
			}
		}

		// Token: 0x060069FC RID: 27132 RVA: 0x0030DF10 File Offset: 0x0030C110
		private int GetAddPropertyBaseValue(ECharacterPropertyReferencedType type)
		{
			if (!true)
			{
			}
			short result;
			switch (type)
			{
			case ECharacterPropertyReferencedType.HitRateStrength:
				result = this.configData.HitRateStrength;
				break;
			case ECharacterPropertyReferencedType.HitRateTechnique:
				result = this.configData.HitRateTechnique;
				break;
			case ECharacterPropertyReferencedType.HitRateSpeed:
				result = this.configData.HitRateSpeed;
				break;
			case ECharacterPropertyReferencedType.HitRateMind:
				result = this.configData.HitRateMind;
				break;
			case ECharacterPropertyReferencedType.PenetrateOfOuter:
				result = this.configData.PenetrateOfOuter;
				break;
			case ECharacterPropertyReferencedType.PenetrateOfInner:
				result = this.configData.PenetrateOfInner;
				break;
			case ECharacterPropertyReferencedType.AvoidRateStrength:
				result = this.configData.AvoidRateStrength;
				break;
			case ECharacterPropertyReferencedType.AvoidRateTechnique:
				result = this.configData.AvoidRateTechnique;
				break;
			case ECharacterPropertyReferencedType.AvoidRateSpeed:
				result = this.configData.AvoidRateSpeed;
				break;
			case ECharacterPropertyReferencedType.AvoidRateMind:
				result = this.configData.AvoidRateMind;
				break;
			case ECharacterPropertyReferencedType.PenetrateResistOfOuter:
				result = this.configData.PenetrateResistOfOuter;
				break;
			case ECharacterPropertyReferencedType.PenetrateResistOfInner:
				result = this.configData.PenetrateResistOfInner;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfStance:
				result = this.configData.RecoveryOfStance;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfBreath:
				result = this.configData.RecoveryOfBreath;
				break;
			case ECharacterPropertyReferencedType.MoveSpeed:
				result = this.configData.MoveSpeed;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfFlaw:
				result = this.configData.RecoveryOfFlaw;
				break;
			case ECharacterPropertyReferencedType.CastSpeed:
				result = this.configData.CastSpeed;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfBlockedAcupoint:
				result = this.configData.RecoveryOfBlockedAcupoint;
				break;
			case ECharacterPropertyReferencedType.WeaponSwitchSpeed:
				result = this.configData.WeaponSwitchSpeed;
				break;
			case ECharacterPropertyReferencedType.AttackSpeed:
				result = this.configData.AttackSpeed;
				break;
			case ECharacterPropertyReferencedType.InnerRatio:
				result = this.configData.InnerRatio;
				break;
			case ECharacterPropertyReferencedType.RecoveryOfQiDisorder:
				result = this.configData.RecoveryOfQiDisorder;
				break;
			case ECharacterPropertyReferencedType.ResistOfHotPoison:
				result = this.configData.ResistOfHotPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfGloomyPoison:
				result = this.configData.ResistOfGloomyPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfColdPoison:
				result = this.configData.ResistOfColdPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfRedPoison:
				result = this.configData.ResistOfRedPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfRottenPoison:
				result = this.configData.ResistOfRottenPoison;
				break;
			case ECharacterPropertyReferencedType.ResistOfIllusoryPoison:
				result = this.configData.ResistOfIllusoryPoison;
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return (int)result;
		}

		// Token: 0x060069FD RID: 27133 RVA: 0x0030E180 File Offset: 0x0030C380
		private void RefreshCostAttribute()
		{
			bool hasCost = this.configData.RequiredMainAttributeType >= 0;
			this.rootCostAttribute.SetActive(hasCost);
			bool flag = hasCost;
			if (flag)
			{
				ECharacterPropertyDisplayType type = ECharacterPropertyDisplayType.Strength + (int)this.configData.RequiredMainAttributeType;
				CharacterPropertyDisplayItem propertyConfig = CharacterPropertyDisplay.Instance[type.ToInt()];
				string title = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipMedicine_CostProperty, propertyConfig.Name);
				this.propertyCostAttribute.Set(propertyConfig.TipsIcon, title, this.configData.RequiredMainAttributeValue.ToString().SetColor("brightred"), true);
			}
		}

		// Token: 0x060069FE RID: 27134 RVA: 0x0030E21C File Offset: 0x0030C41C
		private void RefreshCombat()
		{
			bool isShow = this.configData.ConsumedFeatureMedals >= 0;
			this.rootCombat.SetActive(isShow);
			bool flag = isShow;
			if (flag)
			{
				this.propertyCost.SetValue(this.configData.ConsumedFeatureMedals.ToString().SetColor("brightyellow"));
			}
		}

		// Token: 0x060069FF RID: 27135 RVA: 0x0030E274 File Offset: 0x0030C474
		private void RefreshWugKing()
		{
			this.rootWugKing.SetActive(this._isWugKing);
			this.rootWugKingDetail.SetActive(this._isWugKing);
			bool flag = !this._isWugKing;
			if (!flag)
			{
				WugKingItem wugKingConfig = WugKing.Instance.First((WugKingItem w) => w.WugMedicine == this.configData.TemplateId);
				this.propertyPreviewWugKing.Set("", this.configData.Name, this.configData.SpecialEffectDesc, true);
				short growingGoodWugId = wugKingConfig.GrowingGoodWugs.First<short>();
				MedicineItem growingGoodWugConfig = Medicine.Instance[growingGoodWugId];
				string growingGoodWugTitle = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_WugKing_Effect_GrowingGood, growingGoodWugConfig.Name);
				this.propertyPreviewWugGrowingGood.Set("", growingGoodWugTitle, wugKingConfig.GrowingGoodEffectDesc, true);
				short growingBadWugId = wugKingConfig.GrowingBadWugs.First<short>();
				MedicineItem growingBadWugConfig = Medicine.Instance[growingBadWugId];
				string growingBadWugTitle = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_WugKing_Effect_GrowingBad, growingBadWugConfig.Name);
				this.propertyPreviewWugGrowingBad.Set("", growingBadWugTitle, wugKingConfig.GrowingBadEffectDesc, true);
				MedicineItem grownWugConfig = Medicine.Instance[wugKingConfig.GrownWug];
				string grownWugTitle = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_WugKing_Effect_Grown, grownWugConfig.Name);
				this.propertyPreviewWugGrown.Set("", grownWugTitle, wugKingConfig.GrownEffectDesc, true);
				EatingItemMonitor eatingItemMonitor = this._eatingItemMonitor;
				if (eatingItemMonitor != null)
				{
					eatingItemMonitor.RemoveEatingItemListener(new Action(this.RefreshWugKingEffectState));
				}
				this._eatingItemMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EatingItemMonitor>(this._charId, false);
				bool init = this._eatingItemMonitor.Init;
				if (init)
				{
					this.RefreshWugKingEffectState();
				}
				else
				{
					this._eatingItemMonitor.AddEatingItemListener(new Action(this.RefreshWugKingEffectState));
				}
			}
		}

		// Token: 0x06006A00 RID: 27136 RVA: 0x0030E42C File Offset: 0x0030C62C
		public static string GetKillWugTip(MedicineItem wugMedicineConfig)
		{
			sbyte killWugPoisonType = -1;
			bool flag = wugMedicineConfig.ItemSubType == 802 && wugMedicineConfig.WugGrowthType != 5;
			if (flag)
			{
				Medicine.Instance.Iterate(delegate(MedicineItem m)
				{
					bool flag3 = m.ItemSubType == 801 && m.DetoxWugType == wugMedicineConfig.WugType;
					bool result2;
					if (flag3)
					{
						killWugPoisonType = m.PoisonType;
						result2 = false;
					}
					else
					{
						result2 = true;
					}
					return result2;
				});
			}
			bool flag2 = killWugPoisonType < 0;
			string result;
			if (flag2)
			{
				result = LocalStringManager.Get(LanguageKey.LK_ItemTips_Wug_Cannot_Kill);
			}
			else
			{
				PoisonItem poisonConfig = Poison.Instance[killWugPoisonType];
				result = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_Wug_Can_Kill, "<SpName=" + poisonConfig.TipsIcon + ">" + poisonConfig.Name.SetColor(poisonConfig.FontColor));
			}
			return result;
		}

		// Token: 0x06006A01 RID: 27137 RVA: 0x0030E4F8 File Offset: 0x0030C6F8
		private void RefreshWugKingEffectState()
		{
			bool flag = this._charId >= 0 && this.IsShowEatingTime;
			if (flag)
			{
				WugKingItem wugKingConfig = WugKing.Instance.First((WugKingItem w) => w.WugMedicine == this.configData.TemplateId);
				bool hasWugKing = this._eatingItemMonitor.EatingItemList.Any((ValueTuple<ItemKey, short> x) => EatingItems.IsWugKing(x.Item1));
				bool hasGrowingGood = hasWugKing && this._eatingItemMonitor.EatingItemList.Any((ValueTuple<ItemKey, short> x) => EatingItems.IsWug(x.Item1) && wugKingConfig.GrowingGoodWugs.Contains(x.Item1.TemplateId));
				DisableStyleRoot styleRoot = this.propertyPreviewWugGrowingGood.StyleRoot;
				if (styleRoot != null)
				{
					styleRoot.SetInteractable(hasGrowingGood);
				}
				bool hasGrowingBad = hasWugKing && this._eatingItemMonitor.EatingItemList.Any((ValueTuple<ItemKey, short> x) => EatingItems.IsWug(x.Item1) && wugKingConfig.GrowingBadWugs.Contains(x.Item1.TemplateId));
				DisableStyleRoot styleRoot2 = this.propertyPreviewWugGrowingBad.StyleRoot;
				if (styleRoot2 != null)
				{
					styleRoot2.SetInteractable(hasGrowingGood);
				}
				bool hasGrown = hasWugKing && this._eatingItemMonitor.EatingItemList.Any((ValueTuple<ItemKey, short> x) => EatingItems.IsWug(x.Item1) && wugKingConfig.GrownWug == x.Item1.TemplateId);
				DisableStyleRoot styleRoot3 = this.propertyPreviewWugGrown.StyleRoot;
				if (styleRoot3 != null)
				{
					styleRoot3.SetInteractable(hasGrowingGood);
				}
				DisableStyleRoot styleRoot4 = this.propertyPreviewWugKing.StyleRoot;
				if (styleRoot4 != null)
				{
					styleRoot4.SetInteractable(hasWugKing && !hasGrowingGood && !hasGrowingBad && !hasGrown);
				}
				bool flag2 = hasGrowingGood;
				if (flag2)
				{
					this.propertyWugKing.SetValue(wugKingConfig.GrowingGoodEffectDesc);
				}
				else
				{
					bool flag3 = hasGrowingBad;
					if (flag3)
					{
						this.propertyWugKing.SetValue(wugKingConfig.GrowingBadEffectDesc);
					}
					else
					{
						bool flag4 = hasGrown;
						if (flag4)
						{
							this.propertyWugKing.SetValue(wugKingConfig.GrownEffectDesc);
						}
						else
						{
							bool flag5 = hasWugKing;
							if (flag5)
							{
								this.propertyWugKing.SetValue(this.configData.SpecialEffectDesc);
							}
						}
					}
				}
			}
			else
			{
				DisableStyleRoot styleRoot5 = this.propertyPreviewWugGrowingGood.StyleRoot;
				if (styleRoot5 != null)
				{
					styleRoot5.SetInteractable(false);
				}
				DisableStyleRoot styleRoot6 = this.propertyPreviewWugGrowingBad.StyleRoot;
				if (styleRoot6 != null)
				{
					styleRoot6.SetInteractable(false);
				}
				DisableStyleRoot styleRoot7 = this.propertyPreviewWugGrown.StyleRoot;
				if (styleRoot7 != null)
				{
					styleRoot7.SetInteractable(false);
				}
				DisableStyleRoot styleRoot8 = this.propertyPreviewWugKing.StyleRoot;
				if (styleRoot8 != null)
				{
					styleRoot8.SetInteractable(true);
				}
				this.propertyWugKing.SetValue(this.configData.SpecialEffectDesc);
			}
		}

		// Token: 0x06006A02 RID: 27138 RVA: 0x0030E74C File Offset: 0x0030C94C
		protected override void InitItemDisableFunctionList()
		{
			base.InitItemDisableFunctionList();
			bool flag = !this.configData.Repairable;
			if (flag)
			{
				this._disableFunctionList.Add(ItemFunction.Repairable);
			}
			bool flag2 = !this.configData.Transferable;
			if (flag2)
			{
				this._disableFunctionList.Add(ItemFunction.Transferable);
			}
			bool flag3 = !this.configData.Poisonable;
			if (flag3)
			{
				this._disableFunctionList.Add(ItemFunction.Poisonable);
			}
			bool flag4 = !this.configData.Refinable;
			if (flag4)
			{
				this._disableFunctionList.Add(ItemFunction.Refinable);
			}
		}

		// Token: 0x04004C4D RID: 19533
		[Header("服食效果")]
		[SerializeField]
		private GameObject rootEatArea;

		// Token: 0x04004C4E RID: 19534
		[SerializeField]
		private TextMeshProUGUI textEatTitle;

		// Token: 0x04004C4F RID: 19535
		[SerializeField]
		private TooltipItemProperty propertyHealTitle;

		// Token: 0x04004C50 RID: 19536
		[SerializeField]
		private TooltipItemProperty propertyHealContent;

		// Token: 0x04004C51 RID: 19537
		[Header("疗伤")]
		[SerializeField]
		private GameObject rootSubHealInjury;

		// Token: 0x04004C52 RID: 19538
		[SerializeField]
		private TooltipItemProperty propertySubHealInjuryContent;

		// Token: 0x04004C53 RID: 19539
		[Header("解毒")]
		[SerializeField]
		private GameObject rootSubDetoxPoison;

		// Token: 0x04004C54 RID: 19540
		[SerializeField]
		private TooltipItemPropertyPoison propertySubDetoxPoison;

		// Token: 0x04004C55 RID: 19541
		[SerializeField]
		private TooltipItemProperty propertySubDetoxPoisonContent;

		// Token: 0x04004C56 RID: 19542
		[Header("其他")]
		[SerializeField]
		private TooltipItemProperty propertyDamageStep;

		// Token: 0x04004C57 RID: 19543
		[SerializeField]
		private Transform layoutAddProperty;

		// Token: 0x04004C58 RID: 19544
		[SerializeField]
		private GameObject rootTime;

		// Token: 0x04004C59 RID: 19545
		[SerializeField]
		private TooltipItemProperty propertyTime;

		// Token: 0x04004C5A RID: 19546
		[Header("消耗属性")]
		[SerializeField]
		private GameObject rootCostAttribute;

		// Token: 0x04004C5B RID: 19547
		[SerializeField]
		private TooltipItemProperty propertyCostAttribute;

		// Token: 0x04004C5C RID: 19548
		[Header("战斗使用")]
		[SerializeField]
		private GameObject rootCombat;

		// Token: 0x04004C5D RID: 19549
		[SerializeField]
		private TooltipItemProperty propertyCost;

		// Token: 0x04004C5E RID: 19550
		[Header("详细模式")]
		[SerializeField]
		private TooltipItemProperty propertyDetailDamageStep;

		// Token: 0x04004C5F RID: 19551
		[Header("王蛊效果")]
		[SerializeField]
		private GameObject rootWugKing;

		// Token: 0x04004C60 RID: 19552
		[SerializeField]
		private TooltipItemProperty propertyWugKing;

		// Token: 0x04004C61 RID: 19553
		[Header("王蛊详细")]
		[SerializeField]
		private GameObject rootWugKingDetail;

		// Token: 0x04004C62 RID: 19554
		[SerializeField]
		private TooltipItemProperty propertyPreviewWugKing;

		// Token: 0x04004C63 RID: 19555
		[SerializeField]
		private TooltipItemProperty propertyPreviewWugGrowingGood;

		// Token: 0x04004C64 RID: 19556
		[SerializeField]
		private TooltipItemProperty propertyPreviewWugGrowingBad;

		// Token: 0x04004C65 RID: 19557
		[SerializeField]
		private TooltipItemProperty propertyPreviewWugGrown;

		// Token: 0x04004C66 RID: 19558
		private MedicineItem configData;

		// Token: 0x04004C67 RID: 19559
		private bool _isWug;

		// Token: 0x04004C68 RID: 19560
		private bool _isWugKing;

		// Token: 0x04004C69 RID: 19561
		private EatingItemMonitor _eatingItemMonitor;
	}
}
