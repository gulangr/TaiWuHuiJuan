using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips
{
	// Token: 0x0200087E RID: 2174
	public class TooltipCombatSkill : MouseTipBase
	{
		// Token: 0x17000C7F RID: 3199
		// (get) Token: 0x06006870 RID: 26736 RVA: 0x002FBF59 File Offset: 0x002FA159
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06006871 RID: 26737 RVA: 0x002FBF5C File Offset: 0x002FA15C
		protected override void Init(ArgumentBox argsBox)
		{
			this.Refresh(argsBox);
		}

		// Token: 0x06006872 RID: 26738 RVA: 0x002FBF68 File Offset: 0x002FA168
		public override void Refresh(ArgumentBox argsBox)
		{
			argsBox.Get("CombatSkillId", out this._combatSkillTemplateId);
			argsBox.Get("CharId", out this._charId);
			argsBox.Get("ShowOnlyTemplateInfo", out this._showOnlyTemplateInfo);
			argsBox.Get("IsSimple", out this._isSimple);
			this._combatSkillDisplayData = null;
			this._configData = CombatSkill.Instance[this._combatSkillTemplateId];
			GlobalDomainMethod.Call.InvokeGuidingTrigger(260);
			sbyte equipType = this._configData.EquipType;
			bool flag = equipType == 1 || equipType == 2 || equipType == 3;
			if (flag)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(259);
			}
			this.NeedWaitData = !this._showOnlyTemplateInfo;
			this.RefreshConfigOnlyInfo();
			bool flag2 = !this._showOnlyTemplateInfo;
			if (flag2)
			{
				CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataOnce(this, this._charId, this._combatSkillTemplateId, new AsyncMethodCallbackDelegate(this.HandlerMethodGetCombatSkillDisplayDataOnce));
			}
			else
			{
				this.Element.ShowAfterRefresh();
			}
		}

		// Token: 0x06006873 RID: 26739 RVA: 0x002FC06C File Offset: 0x002FA26C
		private void HandlerMethodGetCombatSkillDisplayDataOnce(int offset, RawDataPool pool)
		{
			bool flag = this == null || this._configData == null;
			if (!flag)
			{
				Serializer.Deserialize(pool, offset, ref this._combatSkillDisplayData);
				this.RefreshDataDependentInfo();
				this.Element.ShowAfterRefresh();
			}
		}

		// Token: 0x06006874 RID: 26740 RVA: 0x002FC0B8 File Offset: 0x002FA2B8
		private void RefreshConfigOnlyInfo()
		{
			bool flag = this._configData == null;
			if (!flag)
			{
				this.nameText.text = this._configData.Name.SetColor(Colors.Instance.GradeColors[(int)this._configData.Grade]);
				string gradeName = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", this._configData.Grade));
				string gradeNum = LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - this._configData.Grade)));
				string gradeString = LanguageKey.LK_CombatSkill_Grade_Format.TrFormat(gradeName, gradeNum, LanguageKey.LK_Grade.Tr());
				this.gradeText.text = gradeString.SetColor(Colors.Instance.GradeColors[(int)this._configData.Grade]);
				this.skillIcon.SetSprite(this._configData.Icon, false, null);
				this.skillIcon.SetColor(CommonUtils.GetFiveElementColor((int)this._configData.FiveElements));
				this.frame.SetSprite(TooltipCombatSkill.FramePaths[(int)this._configData.EquipType] + this._configData.Grade.ToString(), false, null);
				this.fiveElementsFrame.gameObject.SetActive(false);
				OrganizationItem organizationConfig = Organization.Instance[this._configData.SectId];
				string sectName = organizationConfig.Name;
				this.sectText.text = sectName;
				this.sectIcon.SetSprite(organizationConfig.MousetipIcon, false, null);
				string equipTypeName = this.GetEquipTypeName((int)this._configData.EquipType);
				this.equipTypeText.text = equipTypeName;
				int suffix = TooltipCombatSkill.EquipTypeToZhenqiIconSuffix.GetValueOrDefault((int)this._configData.EquipType, 0);
				this.equipTypeIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_mousetip_zhenqi_", suffix), false, null);
				CombatSkillTypeItem typeConfig = Config.CombatSkillType.Instance[this._configData.Type];
				this.typeText.text = typeConfig.Name;
				this.typeIcon.SetSprite(string.Format("{0}{1}", "mousetip_gongfa_", this._configData.Type), false, null);
				string fiveElementsName = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", this._configData.FiveElements));
				this.fiveElementsText.text = fiveElementsName;
				this.fiveElementsIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_elements_big_", this._configData.FiveElements), false, null);
				this.gradeBack.SetSprite(string.Format("{0}{1}", "ui9_mousetip_base_level_", this._configData.Grade), false, null);
				MouseTip_Util.SetMultiLineAutoHeightText(this.descText, this._configData.Desc);
				this.gridCountArea.SetActive(this._configData.EquipType == 0);
				bool isNeigong = this._configData.EquipType == 0;
				this.loopingArea.SetActive(isNeigong);
				bool flag2 = isNeigong;
				if (flag2)
				{
					this.RefreshLoopingConfigOnly();
				}
				this.RefreshCombatInfo();
				bool showOnlyTemplateInfo = this._showOnlyTemplateInfo;
				if (showOnlyTemplateInfo)
				{
					this.gridCountText.text = this._configData.GridCost.ToString();
					this.RefreshGridCountFromConfig();
				}
				this.RefreshHitInfo();
				this.RefreshCastNeedConfigOnly();
				this.RefreshEquipEffectConfigOnly();
				bool isAgileOrDefense = this._configData.EquipType == 2 || this._configData.EquipType == 3;
				bool flag3 = isAgileOrDefense;
				if (flag3)
				{
					this.RefreshCastEffectConfigOnly();
				}
				else
				{
					this.castEffectArea.SetActive(false);
				}
				this.RefreshDirectionEffectConfigOnly();
				this.defenseSpecialArea.SetActive(false);
				this.taiJiQuanLeveragingRow.SetActive(false);
				this.shuiHuoYingQiGongReduceDamageRow.SetActive(false);
				this.bodyStrongArea.SetActive(false);
				this.legendaryBookArea.SetActive(false);
				this.annotationArea.SetActive(false);
				this.legendaryBookAnnotationArea.SetActive(false);
				this.jumpChargeAnnotationArea.SetActive(false);
				this.moveIntervalAnnotationArea.SetActive(false);
				this.innerOuterRatioAnnotationArea.SetActive(false);
				this.skillSlotAnnotationArea.gameObject.SetActive(false);
				this.loopingAnnotationArea.gameObject.SetActive(false);
				this.UpdateRightMainAreaDisplay();
				this.HideAllSpecialBack();
				this.detailRightArea.SetActive(false);
				this.RefreshDetailMode();
				this.effectAreaLine.gameObject.SetActive((this.effectArea.transform as RectTransform).rect.height > 0f);
			}
		}

		// Token: 0x06006875 RID: 26741 RVA: 0x002FC580 File Offset: 0x002FA780
		private void RefreshDataDependentInfo()
		{
			bool flag = this._configData == null;
			if (!flag)
			{
				bool flag2 = this._configData.EquipType == 0;
				if (flag2)
				{
					this.RefreshGridCountFromDisplayData();
					this.RefreshLoopingFromDisplayData();
				}
				this.RefreshCombatInfo();
				this.RefreshEquipEffect();
				this.RefreshCastEffect();
				this.RefreshDefenseSpecialEffect();
				this.RefreshCombatEffect();
				this.RefreshSpecialEffect();
				this.RefreshHitInfo();
				this.RefreshCastNeed();
				this.RefreshBodyStrong();
				this.RefreshAnnotation();
				this.RefreshLegendaryBook();
				this.RefreshLegendaryBookAnnotation();
				this.RefreshDetailMode();
				this.effectAreaLine.gameObject.SetActive((this.effectArea.transform as RectTransform).rect.height > 0f);
			}
		}

		// Token: 0x06006876 RID: 26742 RVA: 0x002FC654 File Offset: 0x002FA854
		private bool IsDetailMode()
		{
			return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		}

		// Token: 0x06006877 RID: 26743 RVA: 0x002FC680 File Offset: 0x002FA880
		private void RefreshRootLayout(ICollection<RectTransform> additionalRebuildTargets = null)
		{
			RectTransform rootRt = base.GetComponent<RectTransform>();
			RectTransform mainRt = this.mainArea.GetComponent<RectTransform>();
			LayoutRebuilder.ForceRebuildLayoutImmediate(mainRt);
			bool flag = additionalRebuildTargets != null;
			if (flag)
			{
				foreach (RectTransform target in additionalRebuildTargets)
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(target);
				}
			}
			Vector2 mainSize = mainRt.rect.size;
			float width = mainSize.x;
			float height = mainSize.y;
			bool activeSelf = this.detailRightArea.activeSelf;
			if (activeSelf)
			{
				RectTransform rightRt = this.detailRightArea.GetComponent<RectTransform>();
				LayoutRebuilder.ForceRebuildLayoutImmediate(rightRt);
				float rightWidth = 0f;
				for (int i = 0; i < rightRt.childCount; i++)
				{
					Transform child = rightRt.GetChild(i);
					rightWidth += (child.gameObject.activeSelf ? child.GetComponent<RectTransform>().rect.width : 0f);
					bool flag2 = i > 0;
					if (flag2)
					{
						float spacing = rightRt.GetComponent<HorizontalLayoutGroup>().spacing;
						rightWidth += spacing;
					}
				}
				rightRt.SetWidth(rightWidth);
				Vector2 rightSize = rightRt.rect.size;
				width += rightSize.x;
				bool flag3 = rightSize.y > height;
				if (flag3)
				{
					height = rightSize.y;
				}
				rightRt.anchoredPosition = new Vector2(mainSize.x, 0f);
			}
			mainRt.anchoredPosition = Vector2.zero;
			rootRt.sizeDelta = new Vector2(width, height);
			this._lastMainSize = mainRt.rect.size;
		}

		// Token: 0x06006878 RID: 26744 RVA: 0x002FC84C File Offset: 0x002FAA4C
		private void RefreshDetailMode()
		{
			bool detailMode = this.IsDetailMode();
			this.RefreshGridCount(detailMode);
			this.RefreshLoopingDetailMode(detailMode);
			this.RefreshDistanceInfo(this._configData.EquipType == 1);
			this.RefreshAttackPropertyInfo(this._configData.EquipType == 1);
			this.RefreshEquipEffect();
			this.RefreshCastEffect();
			bool flag = this._configData.EquipType == 1 && this._combatSkillDisplayData != null;
			if (flag)
			{
				this.RefreshHitInfo();
			}
			this.RefreshCastNeed();
			this.RefreshDefenseSpecialEffect();
			this.RefreshRequirementDetail();
			this.detailRightArea.SetActive(detailMode);
			this.RefreshPressTip(detailMode);
			this.RefreshRootLayout(null);
			base.SetAllowOverlapLayout(detailMode);
		}

		// Token: 0x06006879 RID: 26745 RVA: 0x002FC908 File Offset: 0x002FAB08
		private void RefreshPressTip(bool detailMode)
		{
			if (detailMode)
			{
				this.pressTipLeftText.text = LanguageKey.LK_Release_Key_Tips.Tr();
				this.pressTipRightText.text = LanguageKey.LK_Back_To_Simple_Tips.Tr();
			}
			else
			{
				this.pressTipLeftText.text = LanguageKey.LK_Press_Tip.Tr();
				this.pressTipRightText.text = LanguageKey.LK_Show_Detail_Tips.Tr();
			}
		}

		// Token: 0x0600687A RID: 26746 RVA: 0x002FC97C File Offset: 0x002FAB7C
		private void RefreshGridCount(bool detailMode)
		{
			bool showOnlyTemplateInfo = this._showOnlyTemplateInfo;
			if (showOnlyTemplateInfo)
			{
				this.gridCountText.text = this._configData.GridCost.ToString();
			}
			else
			{
				bool flag = this._combatSkillDisplayData != null;
				if (flag)
				{
					if (detailMode)
					{
						sbyte baseGrid = this._configData.GridCost;
						sbyte extraGrid = this._combatSkillDisplayData.GridCount - baseGrid;
						bool flag2 = extraGrid != 0;
						if (flag2)
						{
							string sign = (extraGrid > 0) ? "+" : "";
							this.gridCountText.text = string.Format("{0}{1}{2}", baseGrid, sign, extraGrid);
						}
						else
						{
							this.gridCountText.text = baseGrid.ToString();
						}
					}
					else
					{
						this.gridCountText.text = this._combatSkillDisplayData.GridCount.ToString();
					}
				}
			}
		}

		// Token: 0x0600687B RID: 26747 RVA: 0x002FCA68 File Offset: 0x002FAC68
		private void Update()
		{
			bool flag = this._configData == null;
			if (!flag)
			{
				bool hasStick = this.HasStick;
				if (!hasStick)
				{
					bool altDown = this.IsDetailMode();
					bool flag2 = altDown != this._lastAltDown;
					if (flag2)
					{
						this._lastAltDown = altDown;
						this.RefreshDetailMode();
					}
					RectTransform mainRt = this.mainArea.GetComponent<RectTransform>();
					Vector2 mainSize = mainRt.rect.size;
					bool flag3 = mainSize != this._lastMainSize;
					if (flag3)
					{
						this.RefreshRootLayout(null);
					}
				}
			}
		}

		// Token: 0x0600687C RID: 26748 RVA: 0x002FCAF4 File Offset: 0x002FACF4
		private string GetEquipTypeName(int equipType)
		{
			return LocalStringManager.Get(string.Format("LK_CombatSkill_EquipType_{0}", equipType));
		}

		// Token: 0x0600687D RID: 26749 RVA: 0x002FCB1C File Offset: 0x002FAD1C
		private void RefreshGridCountFromConfig()
		{
			bool flag = this._configData.EquipType != 0;
			if (!flag)
			{
				this.attackGridItem.Set(1, this._configData.SpecificGrids[0]);
				this.agileGridItem.Set(2, this._configData.SpecificGrids[1]);
				this.defenceGridItem.Set(3, this._configData.SpecificGrids[2]);
				this.specialGridItem.Set(4, this._configData.SpecificGrids[3]);
				this.genericGridItem.Set(5, this._configData.GenericGrid);
			}
		}

		// Token: 0x0600687E RID: 26750 RVA: 0x002FCBC4 File Offset: 0x002FADC4
		private void RefreshGridCountFromDisplayData()
		{
			bool flag = this._combatSkillDisplayData == null;
			if (!flag)
			{
				this.attackGridItem.Set(1, this._combatSkillDisplayData.SpecificGrids[0], this.GetPropertyIsInSpecialBreaks(49, false));
				this.agileGridItem.Set(2, this._combatSkillDisplayData.SpecificGrids[1], this.GetPropertyIsInSpecialBreaks(50, false));
				this.defenceGridItem.Set(3, this._combatSkillDisplayData.SpecificGrids[2], this.GetPropertyIsInSpecialBreaks(51, false));
				this.specialGridItem.Set(4, this._combatSkillDisplayData.SpecificGrids[3], this.GetPropertyIsInSpecialBreaks(52, false));
				this.genericGridItem.Set(5, this._combatSkillDisplayData.GenericGrid, this.GetPropertyIsInSpecialBreaks(9, false));
			}
		}

		// Token: 0x0600687F RID: 26751 RVA: 0x002FCC94 File Offset: 0x002FAE94
		private void RefreshCombatInfo()
		{
			bool isAttackSkill = this._configData.EquipType == 1;
			this.RefreshDistanceInfo(isAttackSkill);
			this.RefreshPowerInfo();
			this.RefreshHitPartsInfo(isAttackSkill);
			this.RefreshPoisonInfo(isAttackSkill);
			this.RefreshAttackPropertyInfo(isAttackSkill);
			this.RefreshAtkEffectInfo(isAttackSkill);
			bool showFight = this.fightDistanceArea.gameObject.activeSelf || this.fightPowerArea.gameObject.activeSelf || this.fightHitPartsArea.gameObject.activeSelf || this.fightPoisonArea.gameObject.activeSelf;
			this.fightArea.SetActive(showFight);
		}

		// Token: 0x06006880 RID: 26752 RVA: 0x002FCD38 File Offset: 0x002FAF38
		private void RefreshDistanceInfo(bool isAttackSkill)
		{
			this.fightDistanceArea.gameObject.SetActive(isAttackSkill);
			bool flag = !isAttackSkill;
			if (!flag)
			{
				short baseDistance = this._configData.DistanceAdditionWhenCast;
				short nearDistance = (this._combatSkillDisplayData != null) ? this._combatSkillDisplayData.AddAttackDistanceForward : baseDistance;
				short farDistance = (this._combatSkillDisplayData != null) ? this._combatSkillDisplayData.AddAttackDistanceBackward : baseDistance;
				this.fightDistanceTitleText.text = LanguageKey.LK_CombatSkill_Range_Tip.Tr();
				bool detailMode = this.IsDetailMode();
				this.fightNearDistanceText.text = TooltipCombatSkill.FormatAttackDistanceWithDetail(nearDistance, baseDistance, detailMode);
				this.fightFarDistanceText.text = TooltipCombatSkill.FormatAttackDistanceWithDetail(farDistance, baseDistance, detailMode);
				bool flag2 = this.distanceSpecialBack;
				if (flag2)
				{
					this.distanceSpecialBack.enabled = this.GetPropertyIsInSpecialBreaks(34, false);
				}
			}
		}

		// Token: 0x06006881 RID: 26753 RVA: 0x002FCE0C File Offset: 0x002FB00C
		private void RefreshPowerInfo()
		{
			short power = (this._combatSkillDisplayData != null) ? this._combatSkillDisplayData.Power : 100;
			string valueText = string.Format("{0}%", power).SetColor("brightblue");
			this.fightPowerArea.gameObject.SetActive(true);
			this.fightPowerTitleText.text = LanguageKey.LK_CombatSkill_Practice_Broken_Power.Tr();
			this.fightPowerValueText.text = valueText.ColorReplace();
			bool flag = this.powerSpecialBack;
			if (flag)
			{
				this.powerSpecialBack.enabled = this.GetPropertyIsInSpecialBreaks(0, false);
			}
		}

		// Token: 0x06006882 RID: 26754 RVA: 0x002FCEAC File Offset: 0x002FB0AC
		private void RefreshHitPartsInfo(bool isAttackSkill)
		{
			bool flag = !isAttackSkill;
			if (flag)
			{
				this.fightHitPartsArea.gameObject.SetActive(false);
			}
			else
			{
				int[] hitPartWeights = this.GetHitPartWeights();
				int hitPartCount = MouseTipConstant.HitPartNames.GetLength(0);
				int totalWeight = 0;
				int maxWeight = 0;
				for (int i = 0; i < hitPartCount; i++)
				{
					int configIndex = (int)CommonUtils.ConvertShowIndexToConfigIndex((sbyte)i);
					bool flag2 = configIndex < 0 || configIndex >= hitPartWeights.Length;
					if (!flag2)
					{
						int weight = hitPartWeights[configIndex];
						bool flag3 = weight <= 0;
						if (!flag3)
						{
							totalWeight += weight;
							bool flag4 = weight > maxWeight;
							if (flag4)
							{
								maxWeight = weight;
							}
						}
					}
				}
				bool hasHitPartValue = totalWeight > 0;
				this.fightHitPartsArea.gameObject.SetActive(hasHitPartValue);
				bool flag5 = !hasHitPartValue;
				if (!flag5)
				{
					this.fightHitPartsTitleText.text = LanguageKey.LK_CombatSkill_Tip_Fight_HitPart.Tr();
					CommonUtils.PrepareEnoughChildren(this.fightHitPartContainer, this.fightHitPartTemplateItem.gameObject, hitPartCount, null);
					bool[] hitPartSpecialBacks = this.CalInjuryPartAtkRatesBySpecialBreak();
					for (int j = 0; j < hitPartCount; j++)
					{
						int configIndex2 = (int)CommonUtils.ConvertShowIndexToConfigIndex((sbyte)j);
						int weight2 = (configIndex2 >= 0 && configIndex2 < hitPartWeights.Length) ? hitPartWeights[configIndex2] : 0;
						int showInt = (weight2 > 0) ? Math.Max(1, (int)((double)weight2 * 1000.0 / (double)totalWeight + 0.5)) : 0;
						string content = (showInt > 0) ? string.Format("{0}.{1}%", showInt / 10, showInt % 10) : "0%";
						string colorName = (weight2 <= 0) ? "grey" : ((weight2 == maxWeight) ? "orange" : "brightyellow");
						TooltipCombatSkillHitPartItem item = this.fightHitPartContainer.GetChild(j).GetComponent<TooltipCombatSkillHitPartItem>();
						bool showBack = configIndex2 >= 0 && configIndex2 < hitPartSpecialBacks.Length && hitPartSpecialBacks[configIndex2];
						item.Set(string.Format("{0}{1}", "ui9_icon_mousetip_bodyparts_0_", j), content, Colors.Instance[colorName], j < hitPartCount - 1, showBack);
					}
				}
			}
		}

		// Token: 0x06006883 RID: 26755 RVA: 0x002FD0D4 File Offset: 0x002FB2D4
		private void RefreshPoisonInfo(bool isAttackSkill)
		{
			this.EnsurePoisonRowsInitialized();
			bool flag = !isAttackSkill;
			if (flag)
			{
				this.fightPoisonArea.gameObject.SetActive(false);
				this.fightPoisonTemplateItem.gameObject.SetActive(false);
				this.SetPoisonRowCount(0);
			}
			else
			{
				List<ValueTuple<sbyte, short, sbyte>> displayedPoisons = this.GetDisplayedPoisons();
				bool hasPoison = displayedPoisons.Count > 0;
				this.fightPoisonArea.gameObject.SetActive(hasPoison);
				this.fightPoisonTemplateItem.gameObject.SetActive(hasPoison);
				this.SetPoisonRowCount(displayedPoisons.Count);
				for (int i = 0; i < displayedPoisons.Count; i++)
				{
					TooltipCombatSkillPoisonItem row = this._poisonRows[i];
					ValueTuple<sbyte, short, sbyte> valueTuple = displayedPoisons[i];
					sbyte type = valueTuple.Item1;
					short value = valueTuple.Item2;
					sbyte level = valueTuple.Item3;
					int levelIconIndex = TooltipCombatSkill.GetPoisonLevelIconIndex(level);
					row.Set(Poison.Instance[type].Name, value.ToString(), string.Format("{0}{1}", "ui9_icon_poison_0_", type), string.Format("{0}{1}", "ui9_icon_venom_", levelIconIndex), (int)level);
				}
				LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
			}
		}

		// Token: 0x06006884 RID: 26756 RVA: 0x002FD214 File Offset: 0x002FB414
		private int[] GetHitPartWeights()
		{
			CombatSkillDisplayData combatSkillDisplayData = this._combatSkillDisplayData;
			bool flag = ((combatSkillDisplayData != null) ? combatSkillDisplayData.BodyPartWeights : null) != null && this._combatSkillDisplayData.BodyPartWeights.Count > 0;
			int[] result;
			if (flag)
			{
				int[] weights = new int[this._combatSkillDisplayData.BodyPartWeights.Count];
				for (int i = 0; i < weights.Length; i++)
				{
					weights[i] = this._combatSkillDisplayData.BodyPartWeights[i];
				}
				result = weights;
			}
			else
			{
				int[] configWeights = new int[this._configData.InjuryPartAtkRateDistribution.Length];
				for (int j = 0; j < configWeights.Length; j++)
				{
					configWeights[j] = (int)this._configData.InjuryPartAtkRateDistribution[j];
				}
				result = configWeights;
			}
			return result;
		}

		// Token: 0x06006885 RID: 26757 RVA: 0x002FD2DC File Offset: 0x002FB4DC
		[return: TupleElementNames(new string[]
		{
			"type",
			"value",
			"level"
		})]
		private List<ValueTuple<sbyte, short, sbyte>> GetDisplayedPoisons()
		{
			List<ValueTuple<sbyte, short, sbyte>> result = new List<ValueTuple<sbyte, short, sbyte>>();
			PoisonsAndLevels poisons = (this._combatSkillDisplayData != null) ? this._combatSkillDisplayData.Poisons : this._configData.Poisons;
			bool flag = !poisons.IsNonZero();
			List<ValueTuple<sbyte, short, sbyte>> result2;
			if (flag)
			{
				result2 = result;
			}
			else
			{
				for (sbyte order = 0; order < 6; order += 1)
				{
					sbyte type = PoisonType.GetTypeBySortingOrder(order);
					short value = poisons.GetValue((int)type);
					bool flag2 = value <= 0;
					if (!flag2)
					{
						result.Add(new ValueTuple<sbyte, short, sbyte>(type, value, poisons.GetLevel((int)type)));
					}
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x06006886 RID: 26758 RVA: 0x002FD380 File Offset: 0x002FB580
		private void EnsurePoisonRowsInitialized()
		{
			bool flag = this._poisonRows.Count == 0;
			if (flag)
			{
				this._poisonRows.Add(this.fightPoisonTemplateItem);
			}
		}

		// Token: 0x06006887 RID: 26759 RVA: 0x002FD3B4 File Offset: 0x002FB5B4
		private void SetPoisonRowCount(int count)
		{
			this.EnsurePoisonRowsInitialized();
			CommonUtils.PrepareEnoughChildren(this.fightPoisonContainer, this.fightPoisonTemplateItem.gameObject, count, null);
			while (this._poisonRows.Count < count)
			{
				this._poisonRows.Add(this.fightPoisonContainer.GetChild(this._poisonRows.Count).GetComponent<TooltipCombatSkillPoisonItem>());
			}
			for (int i = 0; i < this._poisonRows.Count; i++)
			{
				bool shouldShow = i < count;
				bool flag = i == 0;
				if (flag)
				{
					shouldShow &= this.fightPoisonTemplateItem.gameObject.activeSelf;
				}
				this._poisonRows[i].gameObject.SetActive(shouldShow);
			}
		}

		// Token: 0x06006888 RID: 26760 RVA: 0x002FD480 File Offset: 0x002FB680
		private static string FormatAttackDistance(short value)
		{
			float distance = (float)Math.Min(value, 120) / 10f;
			string sign = (value >= 0) ? "+" : string.Empty;
			return string.Format("{0}{1:f1}", sign, distance);
		}

		// Token: 0x06006889 RID: 26761 RVA: 0x002FD4C4 File Offset: 0x002FB6C4
		private static string FormatAttackDistanceWithDetail(short currentValue, short baseValue, bool detailMode)
		{
			bool flag = !detailMode;
			string result2;
			if (flag)
			{
				result2 = TooltipCombatSkill.FormatAttackDistance(currentValue);
			}
			else
			{
				short extraValue = currentValue - baseValue;
				bool flag2 = extraValue == 0;
				if (flag2)
				{
					result2 = TooltipCombatSkill.FormatAttackDistance(baseValue);
				}
				else
				{
					float extraDistance = (float)Math.Min(extraValue, 120) / 10f;
					string extraStr = (extraValue > 0) ? string.Format("+{0:f1}", extraDistance) : string.Format("{0:f1}", extraDistance);
					string result = TooltipCombatSkill.FormatAttackDistance(baseValue) + "<color=#brightblue>" + extraStr + "</color>";
					result2 = result.ColorReplace();
				}
			}
			return result2;
		}

		// Token: 0x0600688A RID: 26762 RVA: 0x002FD55C File Offset: 0x002FB75C
		public static int GetPoisonLevelIconIndex(sbyte level)
		{
			return (int)(level - 1);
		}

		// Token: 0x0600688B RID: 26763 RVA: 0x002FD574 File Offset: 0x002FB774
		private void RefreshLoopingConfigOnly()
		{
			this.obtainedNeiliText.text = string.Format("-/{0}", this._configData.TotalObtainableNeili);
			this.RefreshFiveElementTransferWhileLooping(this._configData.DestTypeWhileLooping, this._configData.TransferTypeWhileLooping);
		}

		// Token: 0x0600688C RID: 26764 RVA: 0x002FD5C8 File Offset: 0x002FB7C8
		private void RefreshLoopingFromDisplayData()
		{
			bool flag = this._combatSkillDisplayData == null;
			if (!flag)
			{
				this.RefreshFiveElementTransferWhileLooping(this._combatSkillDisplayData.FiveElementDestTypeWhileLooping, this._combatSkillDisplayData.FiveElementTransferTypeWhileLooping);
			}
		}

		// Token: 0x0600688D RID: 26765 RVA: 0x002FD604 File Offset: 0x002FB804
		private void RefreshLoopingDetailMode(bool detailMode)
		{
			bool flag = this._configData.EquipType != 0;
			if (!flag)
			{
				bool showOnlyTemplateInfo = this._showOnlyTemplateInfo;
				if (showOnlyTemplateInfo)
				{
					this.obtainedNeiliText.text = string.Format("-/{0}", this._configData.TotalObtainableNeili);
				}
				else
				{
					bool flag2 = this._combatSkillDisplayData == null;
					if (!flag2)
					{
						this._strBuilder.Clear();
						short obtained = this._combatSkillDisplayData.ObtainedNeili;
						short max = this._combatSkillDisplayData.MaxObtainableNeili;
						string obtainedColor = (obtained >= max) ? "brightred" : "brightblue";
						this._strBuilder.Append(string.Format("<color=#{0}>{1}</color>", obtainedColor, obtained));
						this._strBuilder.Append("/");
						if (detailMode)
						{
							short baseMaxNeili = this._configData.TotalObtainableNeili;
							short extraMaxNeili = max - baseMaxNeili;
							this._strBuilder.Append(baseMaxNeili);
							this._strBuilder.Append(string.Format("<color=#brightblue>+{0}</color>", extraMaxNeili));
						}
						else
						{
							this._strBuilder.Append(max);
						}
						this.obtainedNeiliText.text = this._strBuilder.ToString().ColorReplace();
					}
				}
			}
		}

		// Token: 0x0600688E RID: 26766 RVA: 0x002FD758 File Offset: 0x002FB958
		private void RefreshFiveElementTransferWhileLooping(sbyte destType, sbyte transferType)
		{
			bool hasTransfer = destType >= 0;
			this.fiveElementTransferWhileLoopingArea.SetActive(hasTransfer);
			bool flag = !hasTransfer;
			if (!flag)
			{
				if (!true)
				{
				}
				sbyte b;
				switch (transferType)
				{
				case 0:
					b = FiveElementsType.Countered[(int)destType];
					break;
				case 1:
					b = FiveElementsType.Countering[(int)destType];
					break;
				case 2:
					b = FiveElementsType.Produced[(int)destType];
					break;
				default:
					b = FiveElementsType.Producing[(int)destType];
					break;
				}
				if (!true)
				{
				}
				sbyte srcType = b;
				this.srcFiveElementsWhileLoopingIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_five_elements_", srcType), false, null);
				this.srcFiveElementsWhileLoopingText.text = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", srcType)).SetColor(CommonUtils.GetFiveElementColor((int)srcType));
				this.destFiveElementsWhileLoopingIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_five_elements_", destType), false, null);
				this.destFiveElementsWhileLoopingText.text = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", destType)).SetColor(CommonUtils.GetFiveElementColor((int)destType));
			}
		}

		// Token: 0x0600688F RID: 26767 RVA: 0x002FD86C File Offset: 0x002FBA6C
		private void RefreshEquipEffect()
		{
			bool showOnlyTemplateInfo = this._showOnlyTemplateInfo;
			if (showOnlyTemplateInfo)
			{
				this.RefreshEquipEffectConfigOnly();
			}
			else
			{
				this.EnsureEquipEffectRowsInitialized();
				List<ValueTuple<short, int, int, bool, bool>> equipProperties = this.GetEquipEffectProperties();
				equipProperties = (from p in equipProperties
				orderby p.Item5
				select p).ToList<ValueTuple<short, int, int, bool, bool>>();
				bool hasEquipEffect = equipProperties.Count > 0;
				CombatSkillDisplayData combatSkillDisplayData = this._combatSkillDisplayData;
				List<ValueTuple<short, int>> neiliData = (combatSkillDisplayData != null) ? combatSkillDisplayData.NeiliAllocationAddProperty : null;
				bool hasNeili = neiliData != null && neiliData.Count > 0;
				this.equipEffectArea.SetActive(hasEquipEffect);
				this.equipEffectTemplateItem.gameObject.SetActive(hasEquipEffect);
				this.SetEquipEffectRowCount(equipProperties.Count);
				bool detailMode = this.IsDetailMode();
				for (int i = 0; i < equipProperties.Count; i++)
				{
					TooltipCombatSkillEffectItem row = this._equipEffectRows[i];
					ValueTuple<short, int, int, bool, bool> valueTuple = equipProperties[i];
					short propertyId = valueTuple.Item1;
					int baseValue = valueTuple.Item2;
					int extraValue = valueTuple.Item3;
					bool isPercent = valueTuple.Item4;
					bool hasSpecialBreak = valueTuple.Item5;
					this.SetEquipEffectItem(row, propertyId, baseValue, extraValue, isPercent, detailMode, i, hasSpecialBreak);
				}
				this.neiliAllocationArea.SetActive(hasNeili);
				this.neiliAllocationTemplateItem.gameObject.SetActive(hasNeili);
				this.SetNeiliAllocationRowCount(hasNeili ? neiliData.Count : 0);
				bool flag = hasNeili;
				if (flag)
				{
					for (int j = 0; j < neiliData.Count; j++)
					{
						TooltipCombatSkillEffectItem row2 = this._neiliAllocationRows[j];
						ValueTuple<short, int> valueTuple2 = neiliData[j];
						short propertyId2 = valueTuple2.Item1;
						int bonus = valueTuple2.Item2;
						this.SetNeiliAllocationItem(row2, propertyId2, bonus, j);
					}
				}
				LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
			}
		}

		// Token: 0x06006890 RID: 26768 RVA: 0x002FDA38 File Offset: 0x002FBC38
		private bool RefreshNeiliAllocationFromConfig()
		{
			List<ValueTuple<short, int>> neiliProperties = this._configData.CalcDefaultNeiliAllocationBonus();
			bool hasNeili = neiliProperties.Count > 0;
			this.neiliAllocationArea.SetActive(hasNeili);
			this.neiliAllocationTemplateItem.gameObject.SetActive(hasNeili);
			this.SetNeiliAllocationRowCount(hasNeili ? neiliProperties.Count : 0);
			bool flag = hasNeili;
			if (flag)
			{
				for (int i = 0; i < neiliProperties.Count; i++)
				{
					TooltipCombatSkillEffectItem row = this._neiliAllocationRows[i];
					ValueTuple<short, int> valueTuple = neiliProperties[i];
					short propertyId = valueTuple.Item1;
					int bonus = valueTuple.Item2;
					this.SetNeiliAllocationItem(row, propertyId, bonus, i);
				}
			}
			return hasNeili;
		}

		// Token: 0x06006891 RID: 26769 RVA: 0x002FDAE8 File Offset: 0x002FBCE8
		[return: TupleElementNames(new string[]
		{
			"propertyId",
			"baseValue",
			"extraValue",
			"isPercent",
			"hasSpecialBreak"
		})]
		private List<ValueTuple<short, int, int, bool, bool>> GetEquipEffectProperties()
		{
			List<ValueTuple<short, int, int, bool, bool>> result = new List<ValueTuple<short, int, int, bool, bool>>();
			bool flag = this._configData == null;
			List<ValueTuple<short, int, int, bool, bool>> result2;
			if (flag)
			{
				result2 = result;
			}
			else
			{
				List<PropertyAndValue> equipAddPropertyList = new List<PropertyAndValue>(this._configData.PropertyAddList);
				int poisonNum = 0;
				short allPoisonValue = 0;
				List<PropertyAndValue> poisons = new List<PropertyAndValue>();
				for (int i = 0; i < equipAddPropertyList.Count; i++)
				{
					PropertyAndValue addProperty = equipAddPropertyList[i];
					CharacterPropertyDisplayItem characterPropertyDisplayItem = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[addProperty.PropertyId].DisplayType];
					ECharacterPropertyDisplayType type = characterPropertyDisplayItem.Type;
					bool flag2 = type == ECharacterPropertyDisplayType.ResistOfHotPoison || type == ECharacterPropertyDisplayType.ResistOfColdPoison || type == ECharacterPropertyDisplayType.ResistOfGloomyPoison || type == ECharacterPropertyDisplayType.ResistOfRedPoison || type == ECharacterPropertyDisplayType.ResistOfRottenPoison || type == ECharacterPropertyDisplayType.ResistOfIllusoryPoison;
					if (flag2)
					{
						poisonNum++;
						poisons.Add(addProperty);
						allPoisonValue = addProperty.Value;
					}
				}
				bool allPoison = poisonNum >= 6;
				bool flag3 = allPoison;
				if (flag3)
				{
					for (int j = 0; j < poisons.Count; j++)
					{
						equipAddPropertyList.Remove(poisons[j]);
					}
					PropertyAndValue allPoisonPropertyAndValue = new PropertyAndValue(10000, allPoisonValue);
					equipAddPropertyList.Add(allPoisonPropertyAndValue);
				}
				Dictionary<short, ValueTuple<int, int>> mergedProperties = new Dictionary<short, ValueTuple<int, int>>();
				int displayPower = this.GetDisplayDataPower(this._combatSkillDisplayData);
				foreach (PropertyAndValue property in equipAddPropertyList)
				{
					int baseValue = (int)property.Value * displayPower / 100;
					mergedProperties[property.PropertyId] = new ValueTuple<int, int>(baseValue, 0);
				}
				bool flag4 = this._combatSkillDisplayData != null && this._combatSkillDisplayData.BreakAddProperty != null;
				if (flag4)
				{
					foreach (ValueTuple<short, short, bool> addProperty2 in this._combatSkillDisplayData.BreakAddProperty)
					{
						bool flag5 = (int)addProperty2.Item1 >= CharacterPropertyReferenced.Instance.Count || addProperty2.Item2 == 0;
						if (!flag5)
						{
							bool flag6 = mergedProperties.ContainsKey(addProperty2.Item1);
							if (flag6)
							{
								ValueTuple<int, int> current = mergedProperties[addProperty2.Item1];
								bool item = addProperty2.Item3;
								if (item)
								{
									mergedProperties[addProperty2.Item1] = new ValueTuple<int, int>(current.Item1, current.Item2 + (int)addProperty2.Item2);
								}
								else
								{
									mergedProperties[addProperty2.Item1] = new ValueTuple<int, int>(current.Item1 + (int)addProperty2.Item2, current.Item2);
								}
							}
							else
							{
								bool item2 = addProperty2.Item3;
								if (item2)
								{
									mergedProperties[addProperty2.Item1] = new ValueTuple<int, int>(0, (int)addProperty2.Item2);
								}
								else
								{
									mergedProperties[addProperty2.Item1] = new ValueTuple<int, int>((int)addProperty2.Item2, 0);
								}
							}
						}
					}
				}
				foreach (KeyValuePair<short, ValueTuple<int, int>> kvp in mergedProperties)
				{
					short propertyId = kvp.Key;
					int baseValue2 = kvp.Value.Item1;
					int extraValue = kvp.Value.Item2;
					bool flag7 = propertyId == 10000;
					if (flag7)
					{
						result.Add(new ValueTuple<short, int, int, bool, bool>(propertyId, baseValue2, extraValue, false, false));
					}
					else
					{
						bool isPercent = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[propertyId].DisplayType].IsPercent;
						bool hasSpecialBreak = this.GetPropertyIsInSpecialBreaks(propertyId, true);
						result.Add(new ValueTuple<short, int, int, bool, bool>(propertyId, baseValue2, extraValue, isPercent, hasSpecialBreak));
					}
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x06006892 RID: 26770 RVA: 0x002FDF0C File Offset: 0x002FC10C
		private void SetEquipEffectItem(TooltipCombatSkillEffectItem item, short propertyId, int baseValue, int extraValue, bool isPercent, bool detailMode, int index, bool showSpecialBack = false)
		{
			bool flag = propertyId == 10000;
			string iconSpriteName;
			string title;
			if (flag)
			{
				iconSpriteName = "mousetip_duxing_big_all";
				title = LocalStringManager.Get(LanguageKey.LK_CombatSkill_AllPoison);
			}
			else
			{
				CharacterPropertyReferencedItem propertyReferencedItem = CharacterPropertyReferenced.Instance[propertyId];
				CharacterPropertyDisplayItem propertyDisplayItem = CharacterPropertyDisplay.Instance[propertyReferencedItem.DisplayType];
				iconSpriteName = propertyDisplayItem.TipsIcon;
				title = LocalStringManager.Get(propertyDisplayItem.Name);
			}
			int totalValue = baseValue + extraValue;
			bool flag2 = detailMode && extraValue != 0;
			string valueText;
			if (flag2)
			{
				string formattedBase = this.FormatPropertyValue(baseValue, isPercent);
				string extraSign = (extraValue >= 0) ? "+" : string.Empty;
				string formattedExtra = isPercent ? string.Format("{0}{1}%", extraSign, extraValue) : string.Format("{0}{1}", extraSign, extraValue);
				valueText = formattedBase + "<color=#brightblue>" + formattedExtra + "</color>";
				valueText = valueText.ColorReplace();
			}
			else
			{
				valueText = this.FormatPropertyValue(totalValue, isPercent);
			}
			item.Set(iconSpriteName, title, valueText, showSpecialBack);
			item.SetDividerVisible(index % 2 == 0);
		}

		// Token: 0x06006893 RID: 26771 RVA: 0x002FE01C File Offset: 0x002FC21C
		private string FormatPropertyValue(int value, bool isPercent)
		{
			string result;
			if (isPercent)
			{
				result = ((value > 0) ? string.Format("+{0}%", value) : string.Format("{0}%", value));
			}
			else
			{
				result = ((value > 0) ? string.Format("+{0}", value) : value.ToString());
			}
			return result;
		}

		// Token: 0x06006894 RID: 26772 RVA: 0x002FE07C File Offset: 0x002FC27C
		private void EnsureEquipEffectRowsInitialized()
		{
			bool flag = this._equipEffectRows.Count == 0;
			if (flag)
			{
				this._equipEffectRows.Add(this.equipEffectTemplateItem);
			}
		}

		// Token: 0x06006895 RID: 26773 RVA: 0x002FE0B0 File Offset: 0x002FC2B0
		private void SetEquipEffectRowCount(int count)
		{
			this.EnsureEquipEffectRowsInitialized();
			CommonUtils.PrepareEnoughChildren(this.equipEffectContainer, this.equipEffectTemplateItem.gameObject, count, null);
			while (this._equipEffectRows.Count < count)
			{
				this._equipEffectRows.Add(this.equipEffectContainer.GetChild(this._equipEffectRows.Count).GetComponent<TooltipCombatSkillEffectItem>());
			}
			for (int i = 0; i < this._equipEffectRows.Count; i++)
			{
				bool shouldShow = i < count;
				bool flag = i == 0;
				if (flag)
				{
					shouldShow &= this.equipEffectTemplateItem.gameObject.activeSelf;
				}
				this._equipEffectRows[i].gameObject.SetActive(shouldShow);
			}
		}

		// Token: 0x06006896 RID: 26774 RVA: 0x002FE17C File Offset: 0x002FC37C
		private void SetNeiliAllocationItem(TooltipCombatSkillEffectItem item, short propertyId, int bonus, int index)
		{
			CharacterPropertyReferencedItem referenced = CharacterPropertyReferenced.Instance[propertyId];
			CharacterPropertyDisplayItem display = CharacterPropertyDisplay.Instance[referenced.DisplayType];
			string icon = display.TipsBigIcon;
			string title = LocalStringManager.Get(display.Name);
			string value = (bonus > 0) ? string.Format("+{0}", bonus) : bonus.ToString();
			item.Set(icon, title, value);
			item.SetDividerVisible(index % 2 == 0);
		}

		// Token: 0x06006897 RID: 26775 RVA: 0x002FE1F4 File Offset: 0x002FC3F4
		private void EnsureNeiliAllocationRowsInitialized()
		{
			bool flag = this._neiliAllocationRows.Count == 0;
			if (flag)
			{
				this._neiliAllocationRows.Add(this.neiliAllocationTemplateItem);
			}
		}

		// Token: 0x06006898 RID: 26776 RVA: 0x002FE228 File Offset: 0x002FC428
		private void SetNeiliAllocationRowCount(int count)
		{
			this.EnsureNeiliAllocationRowsInitialized();
			CommonUtils.PrepareEnoughChildren(this.neiliAllocationContainer, this.neiliAllocationTemplateItem.gameObject, count, null);
			while (this._neiliAllocationRows.Count < count)
			{
				this._neiliAllocationRows.Add(this.neiliAllocationContainer.GetChild(this._neiliAllocationRows.Count).GetComponent<TooltipCombatSkillEffectItem>());
			}
			for (int i = 0; i < this._neiliAllocationRows.Count; i++)
			{
				bool shouldShow = i < count;
				bool flag = i == 0;
				if (flag)
				{
					shouldShow &= this.neiliAllocationTemplateItem.gameObject.activeSelf;
				}
				this._neiliAllocationRows[i].gameObject.SetActive(shouldShow);
			}
		}

		// Token: 0x06006899 RID: 26777 RVA: 0x002FE2F4 File Offset: 0x002FC4F4
		private int GetDisplayDataPower(CombatSkillDisplayData displayData)
		{
			bool flag = displayData == null;
			int result;
			if (flag)
			{
				result = 100;
			}
			else
			{
				result = (int)displayData.RequirementsPower;
			}
			return result;
		}

		// Token: 0x0600689A RID: 26778 RVA: 0x002FE31C File Offset: 0x002FC51C
		private void RefreshEquipEffectConfigOnly()
		{
			this.EnsureEquipEffectRowsInitialized();
			List<PropertyAndValue> equipAddPropertyList = new List<PropertyAndValue>(this._configData.PropertyAddList);
			int poisonNum = 0;
			short allPoisonValue = 0;
			List<PropertyAndValue> poisons = new List<PropertyAndValue>();
			for (int i = 0; i < equipAddPropertyList.Count; i++)
			{
				PropertyAndValue addProperty = equipAddPropertyList[i];
				CharacterPropertyDisplayItem characterPropertyDisplayItem = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[addProperty.PropertyId].DisplayType];
				ECharacterPropertyDisplayType type = characterPropertyDisplayItem.Type;
				bool flag = type == ECharacterPropertyDisplayType.ResistOfHotPoison || type == ECharacterPropertyDisplayType.ResistOfColdPoison || type == ECharacterPropertyDisplayType.ResistOfGloomyPoison || type == ECharacterPropertyDisplayType.ResistOfRedPoison || type == ECharacterPropertyDisplayType.ResistOfRottenPoison || type == ECharacterPropertyDisplayType.ResistOfIllusoryPoison;
				if (flag)
				{
					poisonNum++;
					poisons.Add(addProperty);
					allPoisonValue = addProperty.Value;
				}
			}
			bool allPoison = poisonNum >= 6;
			bool flag2 = allPoison;
			if (flag2)
			{
				for (int j = 0; j < poisons.Count; j++)
				{
					equipAddPropertyList.Remove(poisons[j]);
				}
				PropertyAndValue allPoisonPropertyAndValue = new PropertyAndValue(10000, allPoisonValue);
				equipAddPropertyList.Add(allPoisonPropertyAndValue);
			}
			bool hasEquipEffect = equipAddPropertyList.Count > 0;
			this.equipEffectArea.SetActive(hasEquipEffect);
			this.equipEffectTemplateItem.gameObject.SetActive(hasEquipEffect);
			this.SetEquipEffectRowCount(equipAddPropertyList.Count);
			int displayPower = 100;
			for (int k = 0; k < equipAddPropertyList.Count; k++)
			{
				TooltipCombatSkillEffectItem row = this._equipEffectRows[k];
				PropertyAndValue addProperty2 = equipAddPropertyList[k];
				int baseValue = (int)addProperty2.Value * displayPower / 100;
				bool isPercent = addProperty2.PropertyId != 10000 && CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[addProperty2.PropertyId].DisplayType].IsPercent;
				this.SetEquipEffectItem(row, addProperty2.PropertyId, baseValue, 0, isPercent, false, k, false);
			}
			bool hasNeili = this.RefreshNeiliAllocationFromConfig();
			bool flag3 = !hasEquipEffect;
			if (flag3)
			{
				this.equipEffectArea.SetActive(hasNeili);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
		}

		// Token: 0x0600689B RID: 26779 RVA: 0x002FE544 File Offset: 0x002FC744
		private void RefreshDirectionEffectConfigOnly()
		{
			this.directionArea.SetActive(false);
			this.rightDirectionArea.SetActive(true);
			this.rightDirectEffectArea.SetActive(true);
			this.rightReverseEffectArea.SetActive(true);
			this.rightDirectEffectTitleText.text = LanguageKey.LK_CombatSkill_Direct_Effect.Tr();
			this.rightReverseEffectTitleText.text = LanguageKey.LK_CombatSkill_Reverse_Effect.Tr();
			this.rightDirectEffectIcon.SetSprite("ui9_icon_combat_skill_effect_direction_0", false, null);
			this.rightReverseEffectIcon.SetSprite("ui9_icon_combat_skill_effect_direction_1", false, null);
			Color directColor = Colors.Instance["brightblue"];
			Color reverseColor = Colors.Instance["brightred"];
			this.rightDirectEffectIcon.SetColor(directColor);
			this.rightDirectEffectDescText.color = directColor;
			this.rightReverseEffectIcon.SetColor(reverseColor);
			this.rightReverseEffectDescText.color = reverseColor;
			string directDesc = CommonUtils.GetSpecialEffectDesc(this._configData.DirectEffectID);
			string reverseDesc = CommonUtils.GetSpecialEffectDesc(this._configData.ReverseEffectID);
			this.rightDirectEffectDescText.text = "     " + directDesc;
			this.rightReverseEffectDescText.text = "     " + reverseDesc;
		}

		// Token: 0x0600689C RID: 26780 RVA: 0x002FE67C File Offset: 0x002FC87C
		private void RefreshCastEffectConfigOnly()
		{
			this.EnsureCastEffectRowsInitialized();
			sbyte equipType = this._configData.EquipType;
			bool flag = equipType != 2 && equipType != 3;
			if (flag)
			{
				this.castEffectArea.SetActive(false);
				this.castEffectTemplateItem.gameObject.SetActive(false);
				this.SetCastEffectRowCount(0);
			}
			else
			{
				int[] hitTypeOrder = new int[]
				{
					2,
					1,
					0,
					3
				};
				List<ValueTuple<short, int, int, bool, bool>> castProperties = new List<ValueTuple<short, int, int, bool, bool>>();
				bool flag2 = equipType == 2;
				if (flag2)
				{
					short addMoveSpeed = this._configData.AddMoveSpeedOnCast * GlobalConfig.Instance.AgileSkillBaseAddSpeed / 100;
					bool flag3 = addMoveSpeed != 0;
					if (flag3)
					{
						castProperties.Add(new ValueTuple<short, int, int, bool, bool>(20, (int)addMoveSpeed, 0, false, false));
					}
					bool flag4 = this._configData.AddPercentMoveSpeedOnCast != 0;
					if (flag4)
					{
						castProperties.Add(new ValueTuple<short, int, int, bool, bool>(20, (int)this._configData.AddPercentMoveSpeedOnCast, 0, true, false));
					}
					short addHitBase = GlobalConfig.Instance.AgileSkillBaseAddHit;
					foreach (int hitType in hitTypeOrder)
					{
						bool flag5 = this._configData.AddHitOnCast[hitType] == 0;
						if (!flag5)
						{
							if (!true)
							{
							}
							short num;
							switch (hitType)
							{
							case 0:
								num = 6;
								break;
							case 1:
								num = 7;
								break;
							case 2:
								num = 8;
								break;
							case 3:
								num = 9;
								break;
							default:
								num = 0;
								break;
							}
							if (!true)
							{
							}
							short propertyId = num;
							short value = this._configData.AddHitOnCast[hitType] * addHitBase / 100;
							castProperties.Add(new ValueTuple<short, int, int, bool, bool>(propertyId, (int)value, 0, false, false));
						}
					}
					bool flag6 = this._configData.MoveCdBonus != 0;
					if (flag6)
					{
						castProperties.Add(new ValueTuple<short, int, int, bool, bool>(-1, (int)(-(int)this._configData.MoveCdBonus), 0, true, false));
					}
					bool flag7 = this._configData.JumpPrepareFrame > 0;
					if (flag7)
					{
						int jumpSpeed = CFormula.CalcJumpSpeed(0);
						bool flag8 = jumpSpeed > 0;
						if (flag8)
						{
							castProperties.Add(new ValueTuple<short, int, int, bool, bool>(-2, this._configData.JumpPrepareFrame, jumpSpeed, false, false));
						}
					}
				}
				else
				{
					bool flag9 = this._configData.AddOuterPenetrateResistOnCast != 0;
					if (flag9)
					{
						castProperties.Add(new ValueTuple<short, int, int, bool, bool>(16, (int)this._configData.AddOuterPenetrateResistOnCast, 0, false, false));
					}
					bool flag10 = this._configData.AddInnerPenetrateResistOnCast != 0;
					if (flag10)
					{
						castProperties.Add(new ValueTuple<short, int, int, bool, bool>(17, (int)this._configData.AddInnerPenetrateResistOnCast, 0, false, false));
					}
					foreach (int hitType2 in hitTypeOrder)
					{
						bool flag11 = this._configData.AddAvoidOnCast[hitType2] == 0;
						if (!flag11)
						{
							if (!true)
							{
							}
							short num;
							switch (hitType2)
							{
							case 0:
								num = 12;
								break;
							case 1:
								num = 13;
								break;
							case 2:
								num = 14;
								break;
							case 3:
								num = 15;
								break;
							default:
								num = 0;
								break;
							}
							if (!true)
							{
							}
							short propertyId2 = num;
							castProperties.Add(new ValueTuple<short, int, int, bool, bool>(propertyId2, (int)this._configData.AddAvoidOnCast[hitType2], 0, false, false));
						}
					}
				}
				bool hasCastEffect = castProperties.Count > 0;
				this.castEffectArea.SetActive(hasCastEffect);
				this.castEffectTemplateItem.gameObject.SetActive(hasCastEffect);
				this.SetCastEffectRowCount(castProperties.Count);
				for (int i = 0; i < castProperties.Count; i++)
				{
					TooltipCombatSkillCastEffectItem row = this._castEffectRows[i];
					ValueTuple<short, int, int, bool, bool> valueTuple = castProperties[i];
					short propertyId3 = valueTuple.Item1;
					int baseValue = valueTuple.Item2;
					int extraValue = valueTuple.Item3;
					bool isPercent = valueTuple.Item4;
					bool hasSpecialBreak = valueTuple.Item5;
					this.SetCastEffectItem(row, propertyId3, baseValue, extraValue, isPercent, false, i, hasSpecialBreak);
				}
				LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
			}
		}

		// Token: 0x0600689D RID: 26781 RVA: 0x002FEA54 File Offset: 0x002FCC54
		private void RefreshCastEffect()
		{
			bool showOnlyTemplateInfo = this._showOnlyTemplateInfo;
			if (showOnlyTemplateInfo)
			{
				this.RefreshCastEffectConfigOnly();
			}
			else
			{
				this.EnsureCastEffectRowsInitialized();
				List<ValueTuple<short, int, int, bool, bool>> castProperties = this.GetCastEffectProperties();
				castProperties = (from p in castProperties
				orderby p.Item5
				select p).ToList<ValueTuple<short, int, int, bool, bool>>();
				bool hasCastEffect = castProperties.Count > 0;
				this.castEffectArea.SetActive(hasCastEffect);
				this.castEffectTemplateItem.gameObject.SetActive(hasCastEffect);
				this.SetCastEffectRowCount(castProperties.Count);
				bool detailMode = this.IsDetailMode();
				for (int i = 0; i < castProperties.Count; i++)
				{
					TooltipCombatSkillCastEffectItem row = this._castEffectRows[i];
					ValueTuple<short, int, int, bool, bool> valueTuple = castProperties[i];
					short propertyId = valueTuple.Item1;
					int baseValue = valueTuple.Item2;
					int extraValue = valueTuple.Item3;
					bool isPercent = valueTuple.Item4;
					bool hasSpecialBreak = valueTuple.Item5;
					this.SetCastEffectItem(row, propertyId, baseValue, extraValue, isPercent, detailMode, i, hasSpecialBreak);
				}
				LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
			}
		}

		// Token: 0x0600689E RID: 26782 RVA: 0x002FEB6C File Offset: 0x002FCD6C
		[return: TupleElementNames(new string[]
		{
			"propertyId",
			"baseValue",
			"extraValue",
			"isPercent",
			"hasSpecialBreak"
		})]
		private List<ValueTuple<short, int, int, bool, bool>> GetCastEffectProperties()
		{
			List<ValueTuple<short, int, int, bool, bool>> result = new List<ValueTuple<short, int, int, bool, bool>>();
			bool flag = this._configData == null || this._combatSkillDisplayData == null;
			List<ValueTuple<short, int, int, bool, bool>> result2;
			if (flag)
			{
				result2 = result;
			}
			else
			{
				sbyte equipType = this._configData.EquipType;
				HashSet<short> displayedPropertyIds = new HashSet<short>();
				bool flag2 = equipType == 2;
				if (flag2)
				{
					bool flag3 = this._combatSkillDisplayData.AddMoveSpeed != 0;
					if (flag3)
					{
						result.Add(new ValueTuple<short, int, int, bool, bool>(20, (int)this._combatSkillDisplayData.AddMoveSpeed, 0, false, this.GetPropertyIsInSpecialBreaks(11, false)));
						displayedPropertyIds.Add(20);
					}
					bool flag4 = this._combatSkillDisplayData.AddPercentMoveSpeed != 0;
					if (flag4)
					{
						result.Add(new ValueTuple<short, int, int, bool, bool>(20, (int)this._combatSkillDisplayData.AddPercentMoveSpeed, 0, true, false));
					}
					bool flag5 = this._combatSkillDisplayData.AddHitSpeed != 0;
					if (flag5)
					{
						result.Add(new ValueTuple<short, int, int, bool, bool>(8, this._combatSkillDisplayData.AddHitSpeed, 0, false, this.GetPropertyIsInSpecialBreaks(14, false)));
						displayedPropertyIds.Add(8);
					}
					bool flag6 = this._combatSkillDisplayData.AddHitTechnique != 0;
					if (flag6)
					{
						result.Add(new ValueTuple<short, int, int, bool, bool>(7, this._combatSkillDisplayData.AddHitTechnique, 0, false, this.GetPropertyIsInSpecialBreaks(13, false)));
						displayedPropertyIds.Add(7);
					}
					bool flag7 = this._combatSkillDisplayData.AddHitStrength != 0;
					if (flag7)
					{
						result.Add(new ValueTuple<short, int, int, bool, bool>(6, this._combatSkillDisplayData.AddHitStrength, 0, false, this.GetPropertyIsInSpecialBreaks(12, false)));
						displayedPropertyIds.Add(6);
					}
					bool flag8 = this._combatSkillDisplayData.AddHitMind != 0;
					if (flag8)
					{
						result.Add(new ValueTuple<short, int, int, bool, bool>(9, this._combatSkillDisplayData.AddHitMind, 0, false, this.GetPropertyIsInSpecialBreaks(15, false)));
						displayedPropertyIds.Add(9);
					}
					bool flag9 = this._configData.MoveCdBonus != 0;
					if (flag9)
					{
						result.Add(new ValueTuple<short, int, int, bool, bool>(-1, (int)(-(int)this._configData.MoveCdBonus), 0, true, false));
					}
					bool flag10 = this._configData.JumpPrepareFrame > 0;
					if (flag10)
					{
						int jumpSpeed = (this._combatSkillDisplayData != null) ? this._combatSkillDisplayData.JumpSpeed : CFormula.CalcJumpSpeed(0);
						bool flag11 = jumpSpeed > 0;
						if (flag11)
						{
							result.Add(new ValueTuple<short, int, int, bool, bool>(-2, this._configData.JumpPrepareFrame, jumpSpeed, false, false));
						}
					}
				}
				else
				{
					bool flag12 = equipType == 3;
					if (flag12)
					{
						bool flag13 = this._combatSkillDisplayData.AddOuterDef != 0;
						if (flag13)
						{
							result.Add(new ValueTuple<short, int, int, bool, bool>(16, this._combatSkillDisplayData.AddOuterDef, 0, false, this.GetPropertyIsInSpecialBreaks(18, false)));
							displayedPropertyIds.Add(16);
						}
						bool flag14 = this._combatSkillDisplayData.AddInnerDef != 0;
						if (flag14)
						{
							result.Add(new ValueTuple<short, int, int, bool, bool>(17, this._combatSkillDisplayData.AddInnerDef, 0, false, this.GetPropertyIsInSpecialBreaks(19, false)));
							displayedPropertyIds.Add(17);
						}
						bool flag15 = this._combatSkillDisplayData.AddAvoidSpeed != 0;
						if (flag15)
						{
							result.Add(new ValueTuple<short, int, int, bool, bool>(14, this._combatSkillDisplayData.AddAvoidSpeed, 0, false, this.GetPropertyIsInSpecialBreaks(22, false)));
							displayedPropertyIds.Add(14);
						}
						bool flag16 = this._combatSkillDisplayData.AddAvoidTechnique != 0;
						if (flag16)
						{
							result.Add(new ValueTuple<short, int, int, bool, bool>(13, this._combatSkillDisplayData.AddAvoidTechnique, 0, false, this.GetPropertyIsInSpecialBreaks(21, false)));
							displayedPropertyIds.Add(13);
						}
						bool flag17 = this._combatSkillDisplayData.AddAvoidStrength != 0;
						if (flag17)
						{
							result.Add(new ValueTuple<short, int, int, bool, bool>(12, this._combatSkillDisplayData.AddAvoidStrength, 0, false, this.GetPropertyIsInSpecialBreaks(20, false)));
							displayedPropertyIds.Add(12);
						}
						bool flag18 = this._combatSkillDisplayData.AddAvoidMind != 0;
						if (flag18)
						{
							result.Add(new ValueTuple<short, int, int, bool, bool>(15, this._combatSkillDisplayData.AddAvoidMind, 0, false, this.GetPropertyIsInSpecialBreaks(23, false)));
							displayedPropertyIds.Add(15);
						}
					}
				}
				bool flag19 = this._combatSkillDisplayData.BreakAddProperty != null;
				if (flag19)
				{
					foreach (ValueTuple<short, short, bool> breakProperty in this._combatSkillDisplayData.BreakAddProperty)
					{
						bool flag20 = !breakProperty.Item3;
						if (!flag20)
						{
							bool flag21 = breakProperty.Item2 == 0;
							if (!flag21)
							{
								bool flag22 = (int)breakProperty.Item1 < CharacterPropertyReferenced.Instance.Count;
								bool isPercent;
								bool isEquipEffect;
								short propertyId;
								if (flag22)
								{
									CharacterPropertyDisplayItem propertyConfig = CharacterPropertyDisplay.Instance[breakProperty.Item1];
									bool isDisplaySpecially = propertyConfig.IsDisplaySpecially;
									if (isDisplaySpecially)
									{
										continue;
									}
									isPercent = propertyConfig.IsPercent;
									bool isInverse = propertyConfig.IsInverse;
									isEquipEffect = true;
									propertyId = breakProperty.Item1;
								}
								else
								{
									short combatSkillPropertyId = (short)((int)breakProperty.Item1 - CharacterPropertyReferenced.Instance.Count);
									CombatSkillPropertyItem propertyConfig2 = CombatSkillProperty.Instance[(int)combatSkillPropertyId];
									bool isDisplaySpecially2 = propertyConfig2.IsDisplaySpecially;
									if (isDisplaySpecially2)
									{
										continue;
									}
									isPercent = propertyConfig2.IsPercent;
									bool isInverse = propertyConfig2.IsInverse;
									isEquipEffect = false;
									propertyId = breakProperty.Item1;
								}
								bool flag23 = isEquipEffect;
								if (!flag23)
								{
									bool flag24 = displayedPropertyIds.Contains(propertyId) && propertyId != 20;
									if (!flag24)
									{
										result.Add(new ValueTuple<short, int, int, bool, bool>(propertyId, (int)breakProperty.Item2, 0, isPercent, true));
									}
								}
							}
						}
					}
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600689F RID: 26783 RVA: 0x002FF114 File Offset: 0x002FD314
		private void SetCastEffectItem(TooltipCombatSkillCastEffectItem item, short propertyId, int baseValue, int extraValue, bool isPercent, bool detailMode, int index, bool showSpecialBack = false)
		{
			bool flag = propertyId == -1;
			string iconSpriteName;
			string title;
			string valueText;
			if (flag)
			{
				iconSpriteName = "ui9_icon_mousetip_big_movementinterval";
				title = LanguageKey.LK_CombatSkill_MoveInterval.Tr();
				valueText = string.Format("<color=#{0}>-{1}%</color>", "brightblue", baseValue).ColorReplace();
			}
			else
			{
				bool flag2 = propertyId == -2;
				if (flag2)
				{
					iconSpriteName = "ui9_icon_mousetip_big_jumpdistance";
					title = LanguageKey.LK_CombatSkill_JumpCharge.Tr();
					valueText = ((float)baseValue / (float)extraValue / 60f).ToString("F2") + "s";
				}
				else
				{
					bool flag3 = (int)propertyId < CharacterPropertyReferenced.Instance.Count;
					if (flag3)
					{
						CharacterPropertyReferencedItem propertyReferencedItem = CharacterPropertyReferenced.Instance[propertyId];
						CharacterPropertyDisplayItem propertyDisplayItem = CharacterPropertyDisplay.Instance[propertyReferencedItem.DisplayType];
						iconSpriteName = propertyDisplayItem.TipsBigIcon;
						title = LocalStringManager.Get(propertyDisplayItem.Name);
						int totalValue = baseValue + extraValue;
						if (detailMode)
						{
							string formattedBase = this.FormatPropertyValue(baseValue, isPercent);
							string extraSign = (extraValue >= 0) ? "+" : string.Empty;
							string formattedExtra = isPercent ? string.Format("{0}{1}%", extraSign, extraValue) : string.Format("{0}{1}", extraSign, extraValue);
							valueText = (formattedBase + "<color=#brightblue>" + formattedExtra + "</color>").ColorReplace();
						}
						else
						{
							valueText = this.FormatPropertyValue(totalValue, isPercent);
						}
					}
					else
					{
						short combatSkillPropertyId = (short)((int)propertyId - CharacterPropertyReferenced.Instance.Count);
						CombatSkillPropertyItem propertyConfig = CombatSkillProperty.Instance[(int)combatSkillPropertyId];
						iconSpriteName = propertyConfig.TipsIcon;
						title = LocalStringManager.Get(propertyConfig.Name);
						int totalValue2 = baseValue + extraValue;
						if (detailMode)
						{
							string formattedBase2 = this.FormatPropertyValue(baseValue, isPercent);
							string extraSign2 = (extraValue >= 0) ? "+" : string.Empty;
							string formattedExtra2 = isPercent ? string.Format("{0}{1}%", extraSign2, extraValue) : string.Format("{0}{1}", extraSign2, extraValue);
							valueText = (formattedBase2 + "<color=#brightblue>" + formattedExtra2 + "</color>").ColorReplace();
						}
						else
						{
							valueText = this.FormatPropertyValue(totalValue2, isPercent);
						}
					}
				}
			}
			item.Set(iconSpriteName, title, valueText, showSpecialBack);
		}

		// Token: 0x060068A0 RID: 26784 RVA: 0x002FF340 File Offset: 0x002FD540
		private void EnsureCastEffectRowsInitialized()
		{
			bool flag = this._castEffectRows.Count == 0;
			if (flag)
			{
				this._castEffectRows.Add(this.castEffectTemplateItem);
			}
		}

		// Token: 0x060068A1 RID: 26785 RVA: 0x002FF374 File Offset: 0x002FD574
		private void SetCastEffectRowCount(int count)
		{
			this.EnsureCastEffectRowsInitialized();
			CommonUtils.PrepareEnoughChildren(this.castEffectContainer, this.castEffectTemplateItem.gameObject, count, null);
			while (this._castEffectRows.Count < count)
			{
				this._castEffectRows.Add(this.castEffectContainer.GetChild(this._castEffectRows.Count).GetComponent<TooltipCombatSkillCastEffectItem>());
			}
			for (int i = 0; i < this._castEffectRows.Count; i++)
			{
				bool shouldShow = i < count;
				bool flag = i == 0;
				if (flag)
				{
					shouldShow &= this.castEffectTemplateItem.gameObject.activeSelf;
				}
				this._castEffectRows[i].gameObject.SetActive(shouldShow);
			}
		}

		// Token: 0x060068A2 RID: 26786 RVA: 0x002FF440 File Offset: 0x002FD640
		private void RefreshDefenseSpecialEffect()
		{
			bool flag = this._showOnlyTemplateInfo || this._configData.EquipType != 3;
			if (flag)
			{
				this.defenseSpecialArea.SetActive(false);
				this.effectDurationRow.SetActive(false);
				this.defenseSpecialTemplateItem.gameObject.SetActive(false);
				this.SetDefenseSpecialRowCount(0);
			}
			else
			{
				short effectDuration = (this._combatSkillDisplayData != null) ? this._combatSkillDisplayData.EffectDuration : this._configData.ContinuousFrames;
				this.effectDurationText.text = ((float)effectDuration / 60f).ToString("f1");
				this.effectDurationRow.SetActive(true);
				bool flag2 = this.effectDurationSpecialBack;
				if (flag2)
				{
					this.effectDurationSpecialBack.enabled = this.GetPropertyIsInSpecialBreaks(28, false);
				}
				List<ValueTuple<string, string, string, string, bool>> items = this.GetDefenseSpecialItems();
				bool hasItems = items.Count > 0;
				this.defenseSpecialArea.SetActive(true);
				this.defenseSpecialTemplateItem.gameObject.SetActive(hasItems);
				this.SetDefenseSpecialRowCount(items.Count);
				bool detailMode = this.IsDetailMode();
				for (int i = 0; i < items.Count; i++)
				{
					TooltipCombatSkillDefenseSpecialItem row = this._defenseSpecialRows[i];
					ValueTuple<string, string, string, string, bool> valueTuple = items[i];
					string icon = valueTuple.Item1;
					string title = valueTuple.Item2;
					string value = valueTuple.Item3;
					string desc = valueTuple.Item4;
					bool showSpecialBack = valueTuple.Item5;
					row.Set(icon, title, value, desc, detailMode, showSpecialBack);
				}
				LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
			}
		}

		// Token: 0x060068A3 RID: 26787 RVA: 0x002FF5E0 File Offset: 0x002FD7E0
		[return: TupleElementNames(new string[]
		{
			"icon",
			"title",
			"value",
			"desc",
			"showSpecialBack"
		})]
		private List<ValueTuple<string, string, string, string, bool>> GetDefenseSpecialItems()
		{
			List<ValueTuple<string, string, string, string, bool>> result = new List<ValueTuple<string, string, string, string, bool>>();
			bool detailMode = this.IsDetailMode();
			bool hasFightBack = this._configData.FightBackDamage > 0;
			bool flag = hasFightBack;
			if (flag)
			{
				bool flag2 = this._combatSkillDisplayData != null;
				int fightbackPower;
				string fightbackValue;
				if (flag2)
				{
					fightbackPower = this._combatSkillDisplayData.FightbackPower;
					int baseFightBackPower = (int)(GlobalConfig.Instance.DefendSkillBaseFightBackPower * this._configData.FightBackDamage / 100);
					bool flag3 = detailMode && fightbackPower != baseFightBackPower;
					if (flag3)
					{
						int extra = fightbackPower - baseFightBackPower;
						string extraSign = (extra > 0) ? "+" : string.Empty;
						fightbackValue = string.Format("{0}%<color=#{1}>{2}{3}%</color>", new object[]
						{
							baseFightBackPower,
							"brightblue",
							extraSign,
							extra
						}).ColorReplace();
					}
					else
					{
						fightbackValue = string.Format("{0}%", fightbackPower);
					}
				}
				else
				{
					fightbackPower = (int)(GlobalConfig.Instance.DefendSkillBaseFightBackPower * this._configData.FightBackDamage / 100);
					fightbackValue = string.Format("{0}%", fightbackPower);
				}
				string fightbackTypeStr = this.GetFightbackTypeStr();
				string fightbackDesc = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_FightBack_Desc, fightbackTypeStr, fightbackPower).ColorReplace();
				result.Add(new ValueTuple<string, string, string, string, bool>("ui9_icon_mousetip_big_counterattack", LanguageKey.LK_CombatSkill_FightBack.Tr(), fightbackValue, fightbackDesc, this.GetPropertyIsInSpecialBreaks(24, false)));
			}
			bool hasBounce = this._configData.BounceRateOfOuterInjury > 0 || this._configData.BounceRateOfInnerInjury > 0;
			bool flag4 = hasBounce;
			if (flag4)
			{
				bool flag5 = this._combatSkillDisplayData != null;
				int bouncePowerOuter;
				int bouncePowerInner;
				short bounceDistance;
				if (flag5)
				{
					bouncePowerOuter = this._combatSkillDisplayData.BouncePowerOuter;
					bouncePowerInner = this._combatSkillDisplayData.BouncePowerInner;
					bounceDistance = this._combatSkillDisplayData.BounceDistance;
				}
				else
				{
					bouncePowerOuter = (int)(this._configData.BounceRateOfOuterInjury * GlobalConfig.Instance.DefendSkillBaseBouncePower / 100);
					bouncePowerInner = (int)(this._configData.BounceRateOfInnerInjury * GlobalConfig.Instance.DefendSkillBaseBouncePower / 100);
					bounceDistance = this._configData.BounceDistance;
				}
				int baseBounceOuter = (int)(this._configData.BounceRateOfOuterInjury * GlobalConfig.Instance.DefendSkillBaseBouncePower / 100);
				int baseBounceInner = (int)(this._configData.BounceRateOfInnerInjury * GlobalConfig.Instance.DefendSkillBaseBouncePower / 100);
				bool flag6 = detailMode;
				string bounceValue;
				if (flag6)
				{
					int extraOuter = bouncePowerOuter - baseBounceOuter;
					int extraInner = bouncePowerInner - baseBounceInner;
					bounceValue = this.FormatBounceDetailValue(baseBounceOuter, extraOuter, baseBounceInner, extraInner);
				}
				else
				{
					bounceValue = bouncePowerOuter.ToString().SetColor("outterinjury") + "、" + bouncePowerInner.ToString().SetColor("innerinjury");
				}
				float distanceMin = 2f;
				float distanceMax = (float)bounceDistance / 10f;
				string bounceRangeText = string.Format("{0}-{1:f1}", distanceMin, distanceMax);
				string bounceDesc = this.BuildBounceDescText(this._configData.BounceRateOfOuterInjury > 0, bouncePowerOuter, this._configData.BounceRateOfInnerInjury > 0, bouncePowerInner, bounceRangeText);
				bool showBounceSpecialBack = this.GetPropertyIsInSpecialBreaks(27, false) || this.GetPropertyIsInSpecialBreaks(25, false) || this.GetPropertyIsInSpecialBreaks(26, false);
				result.Add(new ValueTuple<string, string, string, string, bool>("ui9_icon_mousetip_big_counterattack", LanguageKey.LK_CombatSkill_Bounce.Tr(), bounceValue, bounceDesc, showBounceSpecialBack));
			}
			bool flag7 = this._combatSkillDisplayData != null;
			if (flag7)
			{
				byte directPageIndex = CombatSkillStateHelper.GetPageInternalIndex(-1, 0, 2);
				byte reversePageIndex = CombatSkillStateHelper.GetPageInternalIndex(-1, 1, 2);
				bool isDirectPageActive = CombatSkillStateHelper.IsPageActive(this._combatSkillDisplayData.ActivationState, directPageIndex);
				bool isReversePageActive = CombatSkillStateHelper.IsPageActive(this._combatSkillDisplayData.ActivationState, reversePageIndex);
				bool flag8 = isDirectPageActive || isReversePageActive;
				if (flag8)
				{
					string pageEffectDesc = LocalStringManager.Get(isDirectPageActive ? LanguageKey.LK_CombatSkill_Tip_PageEffectDesc_Direct : LanguageKey.LK_CombatSkill_Tip_PageEffectDesc_Reverse);
					result.Add(new ValueTuple<string, string, string, string, bool>("ui9_icon_mousetip_big_damagereduction", LanguageKey.LK_CombatSkill_Defense_DirectDamageReduce.Tr(), "20%", pageEffectDesc, false));
				}
			}
			return result;
		}

		// Token: 0x060068A4 RID: 26788 RVA: 0x002FF9C4 File Offset: 0x002FDBC4
		private string GetFightbackTypeStr()
		{
			short[] addAvoid = this._configData.AddAvoidOnCast;
			this._strBuilder.Clear();
			bool first = true;
			bool flag = addAvoid[0] > 0;
			if (flag)
			{
				bool flag2 = !first;
				if (flag2)
				{
					this._strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
				}
				this._strBuilder.Append(LocalStringManager.Get(CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[12].DisplayType].Name));
				first = false;
			}
			bool flag3 = addAvoid[1] > 0;
			if (flag3)
			{
				bool flag4 = !first;
				if (flag4)
				{
					this._strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
				}
				this._strBuilder.Append(LocalStringManager.Get(CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[13].DisplayType].Name));
				first = false;
			}
			bool flag5 = addAvoid[2] > 0;
			if (flag5)
			{
				bool flag6 = !first;
				if (flag6)
				{
					this._strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
				}
				this._strBuilder.Append(LocalStringManager.Get(CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[14].DisplayType].Name));
			}
			return this._strBuilder.ToString();
		}

		// Token: 0x060068A5 RID: 26789 RVA: 0x002FFB1C File Offset: 0x002FDD1C
		private string FormatBounceDetailValue(int baseOuter, int extraOuter, int baseInner, int extraInner)
		{
			string extraOuterSign = (extraOuter > 0) ? "+" : string.Empty;
			string extraInnerSign = (extraInner > 0) ? "+" : string.Empty;
			string outerStr = (extraOuter != 0) ? string.Format("{0}<color=#{1}>{2}{3}</color>", new object[]
			{
				baseOuter.ToString().SetColor("outterinjury"),
				"brightblue",
				extraOuterSign,
				extraOuter
			}) : baseOuter.ToString().SetColor("outterinjury");
			string innerStr = (extraInner != 0) ? string.Format("{0}<color=#{1}>{2}{3}</color>", new object[]
			{
				baseInner.ToString().SetColor("innerinjury"),
				"brightblue",
				extraInnerSign,
				extraInner
			}) : baseInner.ToString().SetColor("innerinjury");
			return (outerStr + "、" + innerStr).ColorReplace();
		}

		// Token: 0x060068A6 RID: 26790 RVA: 0x002FFC08 File Offset: 0x002FDE08
		private string BuildBounceDescText(bool hasOuter, int outerPower, bool hasInner, int innerPower, string rangeText)
		{
			this._strBuilder.Clear();
			if (hasOuter)
			{
				this._strBuilder.Append(LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_Bounce_Outer, outerPower).ColorReplace());
			}
			if (hasInner)
			{
				if (hasOuter)
				{
					this._strBuilder.Append("  ");
				}
				this._strBuilder.Append(LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_Bounce_Inner, innerPower).ColorReplace());
			}
			this._strBuilder.Append("\n");
			this._strBuilder.Append(LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_Bounce_Range, rangeText).ColorReplace());
			return this._strBuilder.ToString();
		}

		// Token: 0x060068A7 RID: 26791 RVA: 0x002FFCC8 File Offset: 0x002FDEC8
		private void EnsureDefenseSpecialRowsInitialized()
		{
			bool flag = this._defenseSpecialRows.Count == 0;
			if (flag)
			{
				this._defenseSpecialRows.Add(this.defenseSpecialTemplateItem);
			}
		}

		// Token: 0x060068A8 RID: 26792 RVA: 0x002FFCFC File Offset: 0x002FDEFC
		private void SetDefenseSpecialRowCount(int count)
		{
			this.EnsureDefenseSpecialRowsInitialized();
			CommonUtils.PrepareEnoughChildren(this.defenseSpecialContainer, this.defenseSpecialTemplateItem.gameObject, count, null);
			while (this._defenseSpecialRows.Count < count)
			{
				this._defenseSpecialRows.Add(this.defenseSpecialContainer.GetChild(this._defenseSpecialRows.Count).GetComponent<TooltipCombatSkillDefenseSpecialItem>());
			}
			for (int i = 0; i < this._defenseSpecialRows.Count; i++)
			{
				bool shouldShow = i < count;
				bool flag = i == 0;
				if (flag)
				{
					shouldShow &= this.defenseSpecialTemplateItem.gameObject.activeSelf;
				}
				this._defenseSpecialRows[i].gameObject.SetActive(shouldShow);
			}
		}

		// Token: 0x060068A9 RID: 26793 RVA: 0x002FFDC8 File Offset: 0x002FDFC8
		private void RefreshCombatEffect()
		{
			CombatSkillDisplayData combatSkillDisplayData = this._combatSkillDisplayData;
			List<CombatSkillEffectData> effectData = (combatSkillDisplayData != null) ? combatSkillDisplayData.EffectData : null;
			bool flag;
			if (effectData != null)
			{
				flag = effectData.Exists((CombatSkillEffectData d) => d.Type == ECombatSkillEffectType.TaiJiQuanLeveragingValue);
			}
			else
			{
				flag = false;
			}
			bool hasTaiJi = flag;
			this.taiJiQuanLeveragingRow.SetActive(hasTaiJi);
			bool flag2 = hasTaiJi;
			if (flag2)
			{
				CombatSkillEffectData data = effectData.Find((CombatSkillEffectData d) => d.Type == ECombatSkillEffectType.TaiJiQuanLeveragingValue);
				this.taiJiQuanLeveragingValueText.text = data.Value.ToString();
			}
			bool flag3;
			if (effectData != null)
			{
				flag3 = effectData.Exists((CombatSkillEffectData d) => d.Type == ECombatSkillEffectType.ShuiHuoYingQiGongReduceDamage);
			}
			else
			{
				flag3 = false;
			}
			bool hasShuiHuo = flag3;
			this.shuiHuoYingQiGongReduceDamageRow.SetActive(hasShuiHuo);
			bool flag4 = hasShuiHuo;
			if (flag4)
			{
				CombatSkillEffectData data2 = effectData.Find((CombatSkillEffectData d) => d.Type == ECombatSkillEffectType.ShuiHuoYingQiGongReduceDamage);
				this.shuiHuoYingQiGongReduceDamageValueText.text = string.Format("{0}%", data2.Value);
			}
		}

		// Token: 0x060068AA RID: 26794 RVA: 0x002FFEF8 File Offset: 0x002FE0F8
		private void RefreshSpecialEffect()
		{
			bool showOnlyTemplateInfo = this._showOnlyTemplateInfo;
			if (!showOnlyTemplateInfo)
			{
				CombatSkillDisplayData combatSkillDisplayData = this._combatSkillDisplayData;
				sbyte? b = (combatSkillDisplayData != null) ? new sbyte?(combatSkillDisplayData.EffectType) : null;
				int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
				int num2 = -1;
				bool broken = !(num.GetValueOrDefault() == num2 & num != null);
				this.directEffectTitleText.text = LanguageKey.LK_CombatSkill_Direct_Effect.Tr();
				this.directEffectIcon.SetSprite("ui9_icon_combat_skill_effect_direction_0", false, null);
				this.reverseEffectTitleText.text = LanguageKey.LK_CombatSkill_Reverse_Effect.Tr();
				this.reverseEffectIcon.SetSprite("ui9_icon_combat_skill_effect_direction_1", false, null);
				Color grayColor = Colors.Instance["grey"];
				Color directNormalColor = Colors.Instance["brightblue"];
				Color reverseNormalColor = Colors.Instance["brightred"];
				Color titleNormalColor = Colors.Instance["brightyellow"];
				bool flag = broken;
				if (flag)
				{
					bool isDirect = this._combatSkillDisplayData.EffectType == 0;
					this.directionArea.SetActive(true);
					this.directEffectTitle.SetActive(isDirect);
					this.directEffectDesc.SetActive(isDirect);
					this.reverseEffectTitle.SetActive(!isDirect);
					this.reverseEffectDesc.SetActive(!isDirect);
					this.directEffectIcon.SetColor(isDirect ? directNormalColor : grayColor);
					this.directEffectDescText.color = (isDirect ? directNormalColor : grayColor);
					this.reverseEffectIcon.SetColor((!isDirect) ? reverseNormalColor : grayColor);
					this.reverseEffectDescText.color = ((!isDirect) ? reverseNormalColor : grayColor);
					string directDesc = isDirect ? CommonUtils.GetSpecialEffectDesc(this._combatSkillDisplayData) : CommonUtils.GetSpecialEffectDesc(this._configData.DirectEffectID);
					string reverseDesc = (!isDirect) ? CommonUtils.GetSpecialEffectDesc(this._combatSkillDisplayData) : CommonUtils.GetSpecialEffectDesc(this._configData.ReverseEffectID);
					this.directEffectDescText.text = "     " + directDesc;
					this.reverseEffectDescText.text = "     " + reverseDesc;
					this.rightDirectionArea.SetActive(true);
					this.UpdateRightMainAreaDisplay();
					bool flag2 = isDirect;
					if (flag2)
					{
						this.rightReverseEffectArea.SetActive(true);
						this.rightDirectEffectArea.SetActive(false);
						this.rightReverseEffectTitleText.text = LanguageKey.LK_CombatSkill_Reverse_Effect.Tr();
						this.rightReverseEffectIcon.SetSprite("ui9_icon_combat_skill_effect_direction_1", false, null);
						this.rightReverseEffectIcon.SetColor(grayColor);
						this.rightReverseEffectDescText.color = grayColor;
						this.rightReverseEffectDescText.text = "     " + CommonUtils.GetSpecialEffectDesc(this._configData.ReverseEffectID);
					}
					else
					{
						this.rightDirectEffectArea.SetActive(true);
						this.rightReverseEffectArea.SetActive(false);
						this.rightDirectEffectTitleText.text = LanguageKey.LK_CombatSkill_Direct_Effect.Tr();
						this.rightDirectEffectIcon.SetSprite("ui9_icon_combat_skill_effect_direction_0", false, null);
						this.rightDirectEffectIcon.SetColor(grayColor);
						this.rightDirectEffectDescText.color = grayColor;
						this.rightDirectEffectDescText.text = "     " + CommonUtils.GetSpecialEffectDesc(this._configData.DirectEffectID);
					}
				}
				else
				{
					this.directionArea.SetActive(false);
					this.rightDirectionArea.SetActive(true);
					this.rightDirectEffectArea.SetActive(true);
					this.rightReverseEffectArea.SetActive(true);
					this.rightDirectEffectTitleText.text = LanguageKey.LK_CombatSkill_Direct_Effect.Tr();
					this.rightReverseEffectTitleText.text = LanguageKey.LK_CombatSkill_Reverse_Effect.Tr();
					this.rightDirectEffectIcon.SetSprite("ui9_icon_combat_skill_effect_direction_0", false, null);
					this.rightReverseEffectIcon.SetSprite("ui9_icon_combat_skill_effect_direction_1", false, null);
					this.rightDirectEffectIcon.SetColor(grayColor);
					this.rightReverseEffectIcon.SetColor(grayColor);
					this.rightDirectEffectDescText.color = grayColor;
					this.rightReverseEffectDescText.color = grayColor;
					string directDesc2 = CommonUtils.GetSpecialEffectDesc(this._configData.DirectEffectID);
					string reverseDesc2 = CommonUtils.GetSpecialEffectDesc(this._configData.ReverseEffectID);
					this.rightDirectEffectDescText.text = "     " + directDesc2;
					this.rightReverseEffectDescText.text = "     " + reverseDesc2;
					this.UpdateRightMainAreaDisplay();
				}
			}
		}

		// Token: 0x060068AB RID: 26795 RVA: 0x00300384 File Offset: 0x002FE584
		private void RefreshAttackPropertyInfo(bool isAttackSkill)
		{
			this.attackPropertyArea.SetActive(isAttackSkill);
			bool flag = !isAttackSkill;
			if (!flag)
			{
				this.attackPropertyTitleText.text = LanguageKey.LK_CombatSkill_Attack_Property.Tr();
				bool flag2 = this._combatSkillDisplayData != null;
				int outerPenetrate;
				int innerPenetrate;
				int baseInnerPenetrate;
				int baseOuterPenetrate;
				if (flag2)
				{
					outerPenetrate = 100 + this._combatSkillDisplayData.PenetrateValueOuter;
					innerPenetrate = 100 + this._combatSkillDisplayData.PenetrateValueInner;
					int totalPenetrate = (int)this._configData.Penetrate;
					baseInnerPenetrate = totalPenetrate * (int)this._combatSkillDisplayData.CurrInnerRatio / 100;
					baseOuterPenetrate = totalPenetrate - baseInnerPenetrate;
				}
				else
				{
					int totalPenetrate2 = (int)this._configData.Penetrate;
					baseInnerPenetrate = totalPenetrate2 * (int)this._configData.BaseInnerRatio / 100;
					baseOuterPenetrate = totalPenetrate2 - baseInnerPenetrate;
					outerPenetrate = 100 + baseOuterPenetrate;
					innerPenetrate = 100 + baseInnerPenetrate;
				}
				bool detailMode = this.IsDetailMode();
				this.outerPenetrateItem.Set("ui9_icon_attribute_attack_big_0", LanguageKey.LK_Penetrate_Outer.Tr(), this.FormatPenetrateValue(outerPenetrate, 100 + baseOuterPenetrate, detailMode));
				this.innerPenetrateItem.Set("ui9_icon_attribute_attack_big_1", LanguageKey.LK_Penetrate_Inner.Tr(), this.FormatPenetrateValue(innerPenetrate, 100 + baseInnerPenetrate, detailMode));
				bool flag3 = this.penetrateSpecialBack;
				if (flag3)
				{
					bool showPenetrateSpecialBack = this.GetPropertyIsInSpecialBreaks(29, false) || this.GetPropertyIsInSpecialBreaks(72, false);
					this.penetrateSpecialBack.enabled = showPenetrateSpecialBack;
				}
			}
		}

		// Token: 0x060068AC RID: 26796 RVA: 0x003004DC File Offset: 0x002FE6DC
		private string FormatPenetrateValue(int currentValue, int baseValue, bool detailMode)
		{
			bool flag = detailMode && currentValue != baseValue;
			string result;
			if (flag)
			{
				int extraValue = currentValue - baseValue;
				string extraSign = (extraValue >= 0) ? "+" : string.Empty;
				result = string.Format("{0}%<color=#{1}>{2}{3}%</color>", new object[]
				{
					baseValue,
					"brightblue",
					extraSign,
					extraValue
				}).ColorReplace();
			}
			else
			{
				result = string.Format("{0}%", currentValue);
			}
			return result;
		}

		// Token: 0x060068AD RID: 26797 RVA: 0x0030055C File Offset: 0x002FE75C
		private void RefreshAtkEffectInfo(bool isAttackSkill)
		{
			bool flag = !isAttackSkill;
			if (flag)
			{
				this.atkFlawText.gameObject.SetActive(false);
				this.atkAcupointText.gameObject.SetActive(false);
			}
			else
			{
				bool hasFlaw = this._configData.HasAtkFlawEffect;
				bool hasAcupoint = this._configData.HasAtkAcupointEffect;
				this.atkFlawText.gameObject.SetActive(hasFlaw);
				this.atkAcupointText.gameObject.SetActive(hasAcupoint);
				bool flag2 = hasFlaw;
				if (flag2)
				{
					int power = Mathf.CeilToInt((float)CommonUtils.CalcSumMax2HitDistribution((int)this._configData.TemplateId) / 10f);
					this.atkFlawText.text = LocalStringManager.GetFormat(LanguageKey.LK_AtkFlaw_Dynamic_Tips, LocalStringManager.Get(string.Format("LK_Num_{0}", power))).ColorReplace();
				}
				bool flag3 = hasAcupoint;
				if (flag3)
				{
					int power2 = Mathf.CeilToInt((float)CommonUtils.CalcSumMax2HitDistribution((int)this._configData.TemplateId) / 10f);
					this.atkAcupointText.text = LocalStringManager.GetFormat(LanguageKey.LK_AtkAcupoint_Dynamic_Tips, LocalStringManager.Get(string.Format("LK_Num_{0}", power2))).ColorReplace();
				}
			}
		}

		// Token: 0x060068AE RID: 26798 RVA: 0x00300688 File Offset: 0x002FE888
		private void RefreshHitInfo()
		{
			bool isAttack = this._configData.EquipType == 1;
			this.hitInfoArea.SetActive(isAttack && !this._isSimple);
			bool flag = !isAttack;
			if (!flag)
			{
				this.hitInfoTitleText.text = LanguageKey.LK_CombatSkill_HitProperty.Tr();
				bool isMindHit = CombatSkillEquipType.IsMindHitSkill(this._combatSkillTemplateId);
				bool detailMode = this.IsDetailMode();
				this.EnsureHitInfoRowsInitialized();
				int rowCount = TooltipCombatSkill.HitTypeOrder.Length;
				TooltipCombatSkill.EnsureRowCount<TooltipCombatSkillHitInfoItem>(this._hitInfoRows, this.hitInfoTemplateItem, this.hitInfoContainer, rowCount, null);
				CombatSkillDisplayData combatSkillDisplayData = this._combatSkillDisplayData;
				HitOrAvoidInts hitDistribution = (combatSkillDisplayData != null) ? combatSkillDisplayData.HitDistribution : default(HitOrAvoidInts);
				int totalHit = (int)this._configData.TotalHit;
				int cumulativeDistribution = 0;
				CImage[] hitChartBacks = new CImage[]
				{
					this.hitChartBack0,
					this.hitChartBack1,
					this.hitChartBack2,
					this.hitChartBack3
				};
				for (int i = 0; i < rowCount; i++)
				{
					int hitType = TooltipCombatSkill.HitTypeOrder[i];
					TooltipCombatSkillHitInfoItem row = this._hitInfoRows[i];
					bool flag2 = this._combatSkillDisplayData != null;
					int distribution;
					int hitValue;
					int baseHitValue;
					if (flag2)
					{
						distribution = hitDistribution[hitType];
						int baseDistribution = (int)this._configData.PerHitDamageRateDistribution[hitType];
						hitValue = this.GetHitValueByType(hitType);
						baseHitValue = totalHit * baseDistribution / 100;
					}
					else
					{
						distribution = (int)this._configData.PerHitDamageRateDistribution[hitType];
						hitValue = 100 + totalHit * distribution / 100;
						baseHitValue = hitValue;
					}
					bool isMindHitType = hitType == 3;
					cumulativeDistribution += distribution;
					float fillAmount = (float)cumulativeDistribution / 100f;
					hitChartBacks[i].fillAmount = fillAmount;
					bool flag3 = distribution == 0;
					if (flag3)
					{
						row.gameObject.SetActive(false);
					}
					else
					{
						int showDistribution = distribution / 10;
						string distributionNumberStr = LocalStringManager.Get(string.Format("LK_Num_{0}", showDistribution));
						string chengShu = LanguageKey.LK_CombatSkill_HitInfo_Title_Format.TrFormat(distributionNumberStr);
						string title = this.GetHitTypeName(hitType);
						if (!true)
						{
						}
						string text;
						switch (hitType)
						{
						case 0:
							text = "forceresistance";
							break;
						case 1:
							text = "accuracyparry";
							break;
						case 2:
							text = "dexteritydodge";
							break;
						case 3:
							text = "brightyellow";
							break;
						default:
							text = null;
							break;
						}
						if (!true)
						{
						}
						string titleColor = text;
						string coloredTitle = string.IsNullOrEmpty(titleColor) ? title : title.SetColor(titleColor);
						bool flag4 = detailMode && this._combatSkillDisplayData != null;
						string ratioText;
						if (flag4)
						{
							int extraHitValue = hitValue - baseHitValue;
							bool flag5 = extraHitValue != 0;
							if (flag5)
							{
								string sign = (extraHitValue > 0) ? "+" : "";
								ratioText = string.Format("{0}%\n<color=#{1}>{2}{3}%</color>", new object[]
								{
									baseHitValue,
									"brightblue",
									sign,
									extraHitValue
								}).ColorReplace();
							}
							else
							{
								ratioText = string.Format("{0}%", hitValue);
							}
						}
						else
						{
							ratioText = string.Format("{0}%", hitValue);
						}
						string iconSpriteName = string.Format("{0}{1}", "ui9_icon_mousetip_hit_chart_", i);
						bool showHitSpecialBack = this.GetPropertyIsInSpecialBreaks(30, false) || this.GetPropertyIsInSpecialBreaks(73, false);
						if (!true)
						{
						}
						short num;
						switch (hitType)
						{
						case 0:
							num = 31;
							break;
						case 1:
							num = 32;
							break;
						case 2:
							num = 33;
							break;
						default:
							num = -1;
							break;
						}
						if (!true)
						{
						}
						short distributionPropertyId = num;
						bool showDistSpecialBack = distributionPropertyId >= 0 && this.GetPropertyIsInSpecialBreaks(distributionPropertyId, false);
						row.Set(iconSpriteName, coloredTitle, ratioText, chengShu, showHitSpecialBack || showDistSpecialBack);
						row.gameObject.SetActive(true);
						RectTransform rowRect = row.GetComponent<RectTransform>();
						RectTransform parentRect = this.hitInfoContainer.GetComponent<RectTransform>();
						float widthRatio = (float)distribution / 100f;
						float xPos = (float)(cumulativeDistribution - distribution) / 100f * parentRect.rect.width;
						rowRect.anchoredPosition = new Vector2(xPos, rowRect.anchoredPosition.y);
						rowRect.sizeDelta = new Vector2(parentRect.rect.width * widthRatio, rowRect.sizeDelta.y);
					}
				}
			}
		}

		// Token: 0x060068AF RID: 26799 RVA: 0x00300AD0 File Offset: 0x002FECD0
		private int GetHitValueByType(int hitType)
		{
			bool flag = this._combatSkillDisplayData == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				if (!true)
				{
				}
				int num;
				switch (hitType)
				{
				case 0:
					num = 100 + this._combatSkillDisplayData.HitValueStrength;
					break;
				case 1:
					num = 100 + this._combatSkillDisplayData.HitValueTechnique;
					break;
				case 2:
					num = 100 + this._combatSkillDisplayData.HitValueSpeed;
					break;
				case 3:
					num = 100 + this._combatSkillDisplayData.HitValueMind;
					break;
				default:
					num = 0;
					break;
				}
				if (!true)
				{
				}
				result = num;
			}
			return result;
		}

		// Token: 0x060068B0 RID: 26800 RVA: 0x00300B5C File Offset: 0x002FED5C
		private string GetHitTypeName(int hitType)
		{
			if (!true)
			{
			}
			int num;
			switch (hitType)
			{
			case 0:
				num = 6;
				break;
			case 1:
				num = 7;
				break;
			case 2:
				num = 8;
				break;
			case 3:
				num = 9;
				break;
			default:
				num = 0;
				break;
			}
			if (!true)
			{
			}
			int propertyType = num;
			CharacterPropertyDisplayItem displayItem = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[propertyType].DisplayType];
			return LocalStringManager.Get(displayItem.Name);
		}

		// Token: 0x060068B1 RID: 26801 RVA: 0x00300BCC File Offset: 0x002FEDCC
		private void EnsureHitInfoRowsInitialized()
		{
			bool flag = this._hitInfoRows.Count == 0;
			if (flag)
			{
				this._hitInfoRows.Add(this.hitInfoTemplateItem);
			}
		}

		// Token: 0x060068B2 RID: 26802 RVA: 0x00300C00 File Offset: 0x002FEE00
		private void RefreshCastNeedConfigOnly()
		{
			bool showCastNeed = this._configData.EquipType != 0 && this._configData.EquipType != 4;
			this.castNeedArea.SetActive(showCastNeed && !this._isSimple);
			bool flag = !showCastNeed;
			if (!flag)
			{
				this.RefreshBodyPartNeed();
				this.RefreshBreathStanceCostConfigOnly();
				this.RefreshMobilityCostConfigOnly();
				this.RefreshCostTrickConfigOnly();
				this.RefreshWeaponDurabilityCostConfigOnly();
			}
		}

		// Token: 0x060068B3 RID: 26803 RVA: 0x00300C78 File Offset: 0x002FEE78
		private void RefreshCastNeed()
		{
			bool flag = this._showOnlyTemplateInfo || this._combatSkillDisplayData == null;
			if (flag)
			{
				this.RefreshCastNeedConfigOnly();
			}
			else
			{
				bool showCastNeed = this._configData.EquipType != 0 && this._configData.EquipType != 4;
				bool flag2 = !showCastNeed;
				if (!flag2)
				{
					this.RefreshBreathStanceCostFromData();
					this.RefreshMobilityCostFromData();
					this.RefreshCostTrickFromData();
					this.RefreshWeaponDurabilityCostFromData();
				}
			}
		}

		// Token: 0x060068B4 RID: 26804 RVA: 0x00300CF0 File Offset: 0x002FEEF0
		private void RefreshBodyPartNeed()
		{
			List<sbyte> needBodyPartTypes = this._configData.NeedBodyPartTypes;
			bool hasBodyPart = needBodyPartTypes != null && needBodyPartTypes.Count > 0;
			this.bodyPartArea.SetActive(hasBodyPart);
			bool flag = !hasBodyPart;
			if (!flag)
			{
				int groupCount = needBodyPartTypes.Count;
				bool flag2 = this._bodyPartGroups.Count == 0;
				if (flag2)
				{
					this._bodyPartGroups.Add(this.bodyPartGroupTemplate);
					this._bodyPartGroupItems.Add(new List<TooltipCombatSkillBodyPartItem>());
				}
				CommonUtils.PrepareEnoughChildren(this.bodyPartGroupContainer, this.bodyPartGroupTemplate.gameObject, groupCount, null);
				while (this._bodyPartGroups.Count < groupCount)
				{
					this._bodyPartGroups.Add(this.bodyPartGroupContainer.GetChild(this._bodyPartGroups.Count).GetComponent<TooltipCombatSkillBodyPartGroup>());
					this._bodyPartGroupItems.Add(new List<TooltipCombatSkillBodyPartItem>());
				}
				for (int g = 0; g < groupCount; g++)
				{
					sbyte needId = needBodyPartTypes[g];
					List<int> parts = TooltipCombatSkill.GetNeedParts(needId);
					TooltipCombatSkillBodyPartGroup group = this._bodyPartGroups[g];
					group.SetPipeDividerVisible(g < groupCount - 1);
					CommonUtils.PrepareEnoughChildren(group.ItemContainer, group.ItemTemplate.gameObject, parts.Count, null);
					List<TooltipCombatSkillBodyPartItem> items = this._bodyPartGroupItems[g];
					while (items.Count < parts.Count)
					{
						items.Add(group.ItemContainer.GetChild(items.Count).GetComponent<TooltipCombatSkillBodyPartItem>());
					}
					bool useSlash = needId == 4 || needId == 6;
					for (int i = 0; i < parts.Count; i++)
					{
						bool showSlash = useSlash && i < parts.Count - 1;
						items[i].Set(string.Format("{0}{1}", "ui9_icon_mousetip_bodyparts_0_", parts[i]), LocalStringManager.Get(MouseTipConstant.HitPartNames[parts[i], 1]), showSlash);
						items[i].gameObject.SetActive(true);
					}
					for (int j = parts.Count; j < items.Count; j++)
					{
						items[j].gameObject.SetActive(false);
					}
					group.gameObject.SetActive(true);
				}
				for (int g2 = groupCount; g2 < this._bodyPartGroups.Count; g2++)
				{
					this._bodyPartGroups[g2].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x060068B5 RID: 26805 RVA: 0x00300FC8 File Offset: 0x002FF1C8
		private static List<int> GetNeedParts(sbyte needId)
		{
			if (!true)
			{
			}
			List<int> result;
			switch (needId)
			{
			case 0:
				result = new List<int>
				{
					0
				};
				break;
			case 1:
				result = new List<int>
				{
					1
				};
				break;
			case 2:
				result = new List<int>
				{
					2
				};
				break;
			case 3:
				result = new List<int>
				{
					3,
					4
				};
				break;
			case 4:
				result = new List<int>
				{
					3,
					4
				};
				break;
			case 5:
				result = new List<int>
				{
					5,
					6
				};
				break;
			case 6:
				result = new List<int>
				{
					5,
					6
				};
				break;
			default:
				result = new List<int>();
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060068B6 RID: 26806 RVA: 0x003010A8 File Offset: 0x002FF2A8
		private void RefreshBreathStanceCostConfigOnly()
		{
			bool show = this._configData.BreathStanceTotalCost > 0;
			this.breathStanceCostArea.SetActive(show);
			bool flag = !show;
			if (!flag)
			{
				int totalCost = (int)this._configData.BreathStanceTotalCost;
				int innerRatio = (int)this._configData.BaseInnerRatio;
				int costBreath = totalCost * innerRatio / 100;
				int costStance = totalCost - costBreath;
				this.breathCostText.text = LanguageKey.LK_CombatSkill_Breath.Tr() + string.Format("{0}%", costBreath);
				this.stanceCostText.text = LanguageKey.LK_CombatSkill_Stance.Tr() + string.Format("{0}%", costStance);
			}
		}

		// Token: 0x060068B7 RID: 26807 RVA: 0x0030115C File Offset: 0x002FF35C
		private void RefreshBreathStanceCostFromData()
		{
			bool flag = this._combatSkillDisplayData == null;
			if (!flag)
			{
				bool show = this._configData.BreathStanceTotalCost > 0 || this._combatSkillDisplayData.CostBreath > 0 || this._combatSkillDisplayData.CostStance > 0;
				this.breathStanceCostArea.SetActive(show);
				bool flag2 = !show;
				if (!flag2)
				{
					bool detailMode = this.IsDetailMode();
					sbyte costBreath = this._combatSkillDisplayData.CostBreath;
					sbyte costStance = this._combatSkillDisplayData.CostStance;
					string breathVal = this.FormatCostWithDetail((int)costBreath, (int)(this._configData.BreathStanceTotalCost * this._configData.BaseInnerRatio / 100), detailMode);
					this.breathCostText.text = LanguageKey.LK_CombatSkill_Breath.Tr() + this.ApplyCostFontType(breathVal, this._combatSkillDisplayData.CostBreathFontType);
					string stanceVal = this.FormatCostWithDetail((int)costStance, (int)(this._configData.BreathStanceTotalCost * (100 - this._configData.BaseInnerRatio) / 100), detailMode);
					this.stanceCostText.text = LanguageKey.LK_CombatSkill_Stance.Tr() + this.ApplyCostFontType(stanceVal, this._combatSkillDisplayData.CostStanceFontType);
					bool flag3 = this.breathStanceSpecialBack;
					if (flag3)
					{
						this.breathStanceSpecialBack.enabled = this.GetPropertyIsInSpecialBreaks(3, false);
					}
				}
			}
		}

		// Token: 0x060068B8 RID: 26808 RVA: 0x003012B4 File Offset: 0x002FF4B4
		private void RefreshMobilityCostConfigOnly()
		{
			bool show = this._configData.MobilityCost > 0;
			this.mobilityCostArea.SetActive(show);
			bool flag = show;
			if (flag)
			{
				this.mobilityCostText.text = string.Format("{0}%", this._configData.MobilityCost);
			}
			this.mobilityCostDetailArea.SetActive(false);
		}

		// Token: 0x060068B9 RID: 26809 RVA: 0x00301318 File Offset: 0x002FF518
		private void RefreshMobilityCostFromData()
		{
			bool flag = this._combatSkillDisplayData == null;
			if (!flag)
			{
				bool show = this._combatSkillDisplayData.CostMobility > 0;
				this.mobilityCostArea.SetActive(show);
				bool detailMode = this.IsDetailMode();
				bool flag2 = show;
				if (flag2)
				{
					string val = this.FormatCostWithDetail((int)this._combatSkillDisplayData.CostMobility, (int)this._configData.MobilityCost, detailMode);
					this.mobilityCostText.text = this.ApplyCostFontType(val, this._combatSkillDisplayData.CostMobilityFontType);
				}
				bool flag3 = this.mobilityCostSpecialBack;
				if (flag3)
				{
					this.mobilityCostSpecialBack.enabled = this.GetPropertyIsInSpecialBreaks(10, false);
				}
				bool hasPerFrame = this._configData.MobilityReduceSpeed != 0;
				bool hasByMove = this._configData.MoveCostMobility != 0;
				bool showDetail = detailMode && show && this._configData.EquipType == 2 && (hasPerFrame || hasByMove);
				this.mobilityCostDetailArea.SetActive(showDetail);
				bool flag4 = showDetail;
				if (flag4)
				{
					this.mobilityCostPerFrameText.transform.parent.gameObject.SetActive(hasPerFrame);
					bool flag5 = hasPerFrame;
					if (flag5)
					{
						this.mobilityCostPerFrameText.text = this.FormatCostMobility(this._configData.MobilityReduceSpeed) + "%";
					}
					this.mobilityCostByMoveText.text = this.FormatCostMobility(this._configData.MoveCostMobility) + "%";
				}
			}
		}

		// Token: 0x060068BA RID: 26810 RVA: 0x0030148C File Offset: 0x002FF68C
		private void RefreshCostTrickConfigOnly()
		{
			List<NeedTrick> costTrickList = this._configData.TrickCost;
			bool show = costTrickList != null && costTrickList.Count > 0;
			this.costTrickArea.SetActive(show);
			bool flag = !show;
			if (!flag)
			{
				this.EnsureCostTrickRowsInitialized();
				TooltipCombatSkill.EnsureRowCount<TooltipCombatSkillCostTrickItem>(this._costTrickRows, this.costTrickTemplateItem, this.costTrickContainer, costTrickList.Count, null);
				for (int i = 0; i < costTrickList.Count; i++)
				{
					NeedTrick needTrick = costTrickList[i];
					TrickTypeItem trickConfig = Config.TrickType.Instance[needTrick.TrickType];
					this._costTrickRows[i].Set(trickConfig.Name, trickConfig.FontColor, string.Format("×{0}", needTrick.NeedCount));
					this._costTrickRows[i].gameObject.SetActive(true);
				}
				for (int j = costTrickList.Count; j < this._costTrickRows.Count; j++)
				{
					this._costTrickRows[j].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x060068BB RID: 26811 RVA: 0x003015CC File Offset: 0x002FF7CC
		private void RefreshCostTrickFromData()
		{
			bool flag = this._combatSkillDisplayData == null;
			if (!flag)
			{
				List<NeedTrick> costTrickList = this._combatSkillDisplayData.CostTricks;
				bool show = costTrickList != null && costTrickList.Count > 0;
				this.costTrickArea.SetActive(show);
				bool flag2 = !show;
				if (!flag2)
				{
					this.EnsureCostTrickRowsInitialized();
					TooltipCombatSkill.EnsureRowCount<TooltipCombatSkillCostTrickItem>(this._costTrickRows, this.costTrickTemplateItem, this.costTrickContainer, costTrickList.Count, null);
					bool detailMode = this.IsDetailMode();
					bool showTrickSpecialBack = false;
					for (int i = 0; i < costTrickList.Count; i++)
					{
						NeedTrick needTrick = costTrickList[i];
						TrickTypeItem trickConfig = Config.TrickType.Instance[needTrick.TrickType];
						int baseCount = (int)((this._configData.TrickCost != null && i < this._configData.TrickCost.Count) ? this._configData.TrickCost[i].NeedCount : needTrick.NeedCount);
						string countText = (detailMode && (int)needTrick.NeedCount != baseCount) ? string.Format("{0}+<color=#{1}>{2}</color>", baseCount, "brightblue", (int)needTrick.NeedCount - baseCount).ColorReplace() : string.Format("×{0}", needTrick.NeedCount);
						sbyte fontType = (this._combatSkillDisplayData.CostTricksFontType != null && i < this._combatSkillDisplayData.CostTricksFontType.Count) ? this._combatSkillDisplayData.CostTricksFontType[i] : 0;
						countText = this.ApplyCostFontType(countText, fontType);
						this._costTrickRows[i].Set(trickConfig.Name, trickConfig.FontColor, countText);
						this._costTrickRows[i].gameObject.SetActive(true);
						bool propertyIsInSpecialBreaks = this.GetPropertyIsInSpecialBreaks((short)(53 + needTrick.TrickType), false);
						if (propertyIsInSpecialBreaks)
						{
							showTrickSpecialBack = true;
						}
					}
					bool flag3 = this.costTrickAreaSpecialBack;
					if (flag3)
					{
						this.costTrickAreaSpecialBack.enabled = showTrickSpecialBack;
					}
					for (int j = costTrickList.Count; j < this._costTrickRows.Count; j++)
					{
						this._costTrickRows[j].gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x060068BC RID: 26812 RVA: 0x0030182C File Offset: 0x002FFA2C
		private void RefreshWeaponDurabilityCostConfigOnly()
		{
			bool show = this._configData.WeaponDurableCost > 0;
			this.weaponDurabilityCostArea.SetActive(show);
			bool flag = show;
			if (flag)
			{
				this.weaponDurabilityCostText.text = this._configData.WeaponDurableCost.ToString();
			}
		}

		// Token: 0x060068BD RID: 26813 RVA: 0x00301878 File Offset: 0x002FFA78
		private void RefreshWeaponDurabilityCostFromData()
		{
			bool show = this._configData.WeaponDurableCost > 0;
			this.weaponDurabilityCostArea.SetActive(show);
			bool flag = show;
			if (flag)
			{
				TMP_Text tmp_Text = this.weaponDurabilityCostText;
				string text = this._configData.WeaponDurableCost.ToString();
				CombatSkillDisplayData combatSkillDisplayData = this._combatSkillDisplayData;
				tmp_Text.text = this.ApplyCostFontType(text, (combatSkillDisplayData != null) ? combatSkillDisplayData.CostWeaponDurabilityFontType : 0);
			}
		}

		// Token: 0x060068BE RID: 26814 RVA: 0x003018E0 File Offset: 0x002FFAE0
		private void EnsureCostTrickRowsInitialized()
		{
			bool flag = this._costTrickRows.Count == 0;
			if (flag)
			{
				this._costTrickRows.Add(this.costTrickTemplateItem);
			}
		}

		// Token: 0x060068BF RID: 26815 RVA: 0x00301914 File Offset: 0x002FFB14
		private string FormatCostWithDetail(int currentValue, int baseValue, bool detailMode)
		{
			bool flag = detailMode && currentValue != baseValue;
			string result;
			if (flag)
			{
				int extra = currentValue - baseValue;
				string sign = (extra > 0) ? "+" : "";
				result = string.Format("{0}%<color=#{1}>{2}{3}%</color>", new object[]
				{
					baseValue,
					"brightblue",
					sign,
					extra
				}).ColorReplace();
			}
			else
			{
				result = string.Format("{0}%", currentValue);
			}
			return result;
		}

		// Token: 0x060068C0 RID: 26816 RVA: 0x00301994 File Offset: 0x002FFB94
		private string ApplyCostFontType(string text, sbyte fontType)
		{
			bool flag = fontType == 1;
			string result;
			if (flag)
			{
				result = text.SetColor("brightblue");
			}
			else
			{
				bool flag2 = fontType == 2;
				if (flag2)
				{
					result = text.SetColor("brightred");
				}
				else
				{
					result = text;
				}
			}
			return result;
		}

		// Token: 0x060068C1 RID: 26817 RVA: 0x003019D4 File Offset: 0x002FFBD4
		private string FormatCostMobility(int costMobilityValue)
		{
			bool flag = costMobilityValue <= 0;
			string result;
			if (flag)
			{
				result = "0";
			}
			else
			{
				result = ((float)costMobilityValue * 100f / (float)MoveSpecialConstants.MaxMobility).ToString("F2");
			}
			return result;
		}

		// Token: 0x060068C2 RID: 26818 RVA: 0x00301A18 File Offset: 0x002FFC18
		private string FormatJumpSpeed(int prepareFrame, int speed)
		{
			bool flag = prepareFrame <= 0 || speed <= 0;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_JumpPrepareFrame, ((float)prepareFrame / (float)speed / 60f).ToString("F2"));
			}
			return result;
		}

		// Token: 0x060068C3 RID: 26819 RVA: 0x00301A68 File Offset: 0x002FFC68
		private void RefreshBodyStrong()
		{
			bool flag = this._showOnlyTemplateInfo || this._combatSkillDisplayData == null;
			if (flag)
			{
				this.bodyStrongArea.SetActive(false);
				this.UpdateRightMainAreaDisplay();
			}
			else
			{
				int[] outer = this._configData.OuterDamageSteps;
				int[] inner = this._configData.InnerDamageSteps;
				CombatSkillDamageStepBonusDisplayData bonus = this._combatSkillDisplayData.DamageStepBonus;
				int totalItems = outer.Length;
				List<bool> bodyPartActive = this._combatSkillDisplayData.BodyPartDamageStepActive;
				int neededCount = 0;
				for (int i = 0; i < totalItems; i++)
				{
					int configIndex = (int)CommonUtils.ConvertShowIndexToConfigIndex((sbyte)i);
					int outValue = outer[configIndex];
					int innerValue = inner[configIndex];
					bool flag2 = bonus.OuterInjuryStepBonus != 0;
					if (flag2)
					{
						outValue *= bonus.OuterInjuryStepBonus;
					}
					bool flag3 = bonus.InnerInjuryStepBonus != 0;
					if (flag3)
					{
						innerValue *= bonus.InnerInjuryStepBonus;
					}
					bool flag4 = outValue > 0 || innerValue > 0;
					if (flag4)
					{
						neededCount++;
					}
				}
				int fatalValue = this._configData.FatalDamageStep;
				bool flag5 = bonus.FatalStepBonus != 0;
				if (flag5)
				{
					fatalValue *= bonus.FatalStepBonus;
				}
				bool flag6 = fatalValue > 0;
				if (flag6)
				{
					neededCount++;
				}
				int mindValue = this._configData.MindDamageStep;
				bool flag7 = bonus.MindStepBonus != 0;
				if (flag7)
				{
					mindValue *= bonus.MindStepBonus;
				}
				bool flag8 = mindValue > 0;
				if (flag8)
				{
					neededCount++;
				}
				bool hasAny = neededCount > 0;
				this.bodyStrongArea.SetActive(hasAny);
				this.UpdateRightMainAreaDisplay();
				bool flag9 = !hasAny;
				if (!flag9)
				{
					this.bodyStrongTitleText.text = LocalStringManager.Get(LanguageKey.LK_CombatSkill_Injury_Fatal).Replace(LocalStringManager.Get(MouseTipConstant.OuterInjuryInfos[0, 1]), "").Trim();
					this.EnsureBodyStrongRowsInitialized();
					TooltipCombatSkill.EnsureRowCount<TooltipCombatSkillBodyStrongItem>(this._bodyStrongRows, this.bodyStrongTemplateItem, this.bodyStrongContainer, neededCount, null);
					int rowIndex = 0;
					for (int j = 0; j < totalItems; j++)
					{
						int configIndex2 = (int)CommonUtils.ConvertShowIndexToConfigIndex((sbyte)j);
						int outValue2 = outer[configIndex2];
						int innerValue2 = inner[configIndex2];
						bool flag10 = bonus.OuterInjuryStepBonus != 0;
						if (flag10)
						{
							outValue2 *= bonus.OuterInjuryStepBonus;
						}
						bool flag11 = bonus.InnerInjuryStepBonus != 0;
						if (flag11)
						{
							innerValue2 *= bonus.InnerInjuryStepBonus;
						}
						bool flag12 = outValue2 <= 0 && innerValue2 <= 0;
						if (!flag12)
						{
							string title = LocalStringManager.Get(MouseTipConstant.OuterInjuryInfos[j, 1]);
							bool isActive = bodyPartActive != null && j < bodyPartActive.Count && bodyPartActive[j];
							this._bodyStrongRows[rowIndex].Set(title, MouseTipConstant.OuterInjuryInfos[j, 2], string.Format("+{0}", outValue2), outValue2 > 0, MouseTipConstant.InnerInjuryInfos[j, 2], string.Format("+{0}", innerValue2), innerValue2 > 0, isActive);
							this._bodyStrongRows[rowIndex].gameObject.SetActive(true);
							rowIndex++;
						}
					}
					bool flag13 = fatalValue > 0;
					if (flag13)
					{
						bool fatalActive = bodyPartActive != null && 7 < bodyPartActive.Count && bodyPartActive[7];
						this._bodyStrongRows[rowIndex].Set(LanguageKey.LK_CombatSkill_Injury_Fatal.Tr(), "mousetip_zhongchuang_0", string.Format("+{0}", fatalValue), true, "", "", false, fatalActive);
						this._bodyStrongRows[rowIndex].gameObject.SetActive(true);
						rowIndex++;
					}
					bool flag14 = mindValue > 0;
					if (flag14)
					{
						bool mindActive = bodyPartActive != null && 8 < bodyPartActive.Count && bodyPartActive[8];
						this._bodyStrongRows[rowIndex].Set(LanguageKey.LK_CombatSkill_Injury_Mind.Tr(), "mousetip_dongxin_0", string.Format("+{0}", mindValue), true, "", "", false, mindActive);
						this._bodyStrongRows[rowIndex].gameObject.SetActive(true);
						rowIndex++;
					}
					for (int k = rowIndex; k < this._bodyStrongRows.Count; k++)
					{
						this._bodyStrongRows[k].gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x060068C4 RID: 26820 RVA: 0x00301F10 File Offset: 0x00300110
		private void EnsureBodyStrongRowsInitialized()
		{
			bool flag = this._bodyStrongRows.Count == 0;
			if (flag)
			{
				this._bodyStrongRows.Add(this.bodyStrongTemplateItem);
			}
		}

		// Token: 0x060068C5 RID: 26821 RVA: 0x00301F44 File Offset: 0x00300144
		private void RefreshRequirementDetail()
		{
			bool isConfigOnly = this._showOnlyTemplateInfo || this._combatSkillDisplayData == null;
			bool flag = isConfigOnly;
			List<ValueTuple<int, int, int>> requirements;
			if (flag)
			{
				requirements = this.GetRequirementsFromConfig();
			}
			else
			{
				requirements = this._combatSkillDisplayData.Requirements;
			}
			bool hasRequirements = requirements != null && requirements.Count > 0;
			bool shouldShow = isConfigOnly ? (hasRequirements && this.IsDetailMode()) : hasRequirements;
			this.requirementArea.SetActive(shouldShow);
			this.requirementAnnotationArea.SetActive(shouldShow && !PlayerCastBossSkills.Ids.Contains(this._configData.TemplateId));
			this.UpdateRightMainAreaDisplay();
			bool flag2 = !shouldShow;
			if (!flag2)
			{
				this.requirementTitleText.text = LanguageKey.LK_CombatSkill_Requirement_Title.Tr();
				string valueStr = (isConfigOnly ? "100%" : string.Format("{0}%", this._combatSkillDisplayData.RequirementsPower)).SetColor("lowwarning");
				string maxValueStr = isConfigOnly ? "100%" : string.Format("{0}%", this._combatSkillDisplayData.MaxPower);
				string maxTip = LanguageKey.LK_CombatSkill_Max_Power_Tips.Tr();
				string rightStr = LanguageKey.LK_Brackets_Fix.TrFormat(maxTip + maxValueStr).SetColor("brightyellow");
				this.requirementPowerText.text = valueStr + rightStr;
				bool flag3 = this.maxPowerSpecialBack;
				if (flag3)
				{
					this.maxPowerSpecialBack.enabled = this.GetPropertyIsInSpecialBreaks(1, false);
				}
				bool flag4 = requirements == null;
				if (!flag4)
				{
					int needGrayType = TooltipCombatSkill.GenerateNeedGrayType(requirements);
					CommonUtils.PrepareExtraItemInfo config = new CommonUtils.PrepareExtraItemInfo
					{
						ExtraItemCount = 1,
						TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
					};
					TooltipCombatSkill.EnsureRowCount<TooltipCombatSkillRequirementItem>(this._requirementRows, this.requirementTemplateItem, this.requirementContainer, requirements.Count, new CommonUtils.PrepareExtraItemInfo?(config));
					bool showReqSpecialBack = this.GetPropertyIsInSpecialBreaks(48, false);
					for (int i = 0; i < requirements.Count; i++)
					{
						ValueTuple<int, int, int> valueTuple = requirements[i];
						int type = valueTuple.Item1;
						int required = valueTuple.Item2;
						int actual = valueTuple.Item3;
						int displayType = (int)((type >= 0 && type < CharacterPropertyReferenced.Instance.Count) ? CharacterPropertyReferenced.Instance[type].DisplayType : -1);
						string iconSprite = (displayType >= 0 && displayType < CharacterPropertyDisplay.Instance.Count) ? CharacterPropertyDisplay.Instance[displayType].TipsIcon : "";
						string name = (displayType >= 0 && displayType < CharacterPropertyDisplay.Instance.Count) ? CharacterPropertyDisplay.Instance[displayType].ShortName : "";
						bool needGray = type == needGrayType;
						this._requirementRows[i].Set(iconSprite, name, actual, required, needGray, showReqSpecialBack);
						this._requirementRows[i].gameObject.SetActive(true);
					}
					for (int j = requirements.Count; j < this._requirementRows.Count; j++)
					{
						this._requirementRows[j].gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x060068C6 RID: 26822 RVA: 0x00302270 File Offset: 0x00300470
		public static int GenerateNeedGrayType([TupleElementNames(new string[]
		{
			"type",
			"required",
			"actual"
		})] List<ValueTuple<int, int, int>> requirements)
		{
			bool flag = requirements == null || requirements.Count == 0;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				bool flag2 = requirements.Any(([TupleElementNames(new string[]
				{
					"type",
					"required",
					"actual"
				})] ValueTuple<int, int, int> t) => t.Item3 < 0);
				if (flag2)
				{
					result = -1;
				}
				else
				{
					result = (from t in requirements
					select new ValueTuple<int, float>(t.Item1, (float)t.Item3 / (float)t.Item2) into t
					orderby t.Item2
					select t).First<ValueTuple<int, float>>().Item1;
				}
			}
			return result;
		}

		// Token: 0x060068C7 RID: 26823 RVA: 0x0030231C File Offset: 0x0030051C
		private void UpdateRightMainAreaDisplay()
		{
			bool shouldShow = this.bodyStrongArea.activeSelf || this.requirementArea.activeSelf || this.rightDirectionArea.activeSelf;
			this.rightMainArea.SetActive(shouldShow);
		}

		// Token: 0x060068C8 RID: 26824 RVA: 0x00302360 File Offset: 0x00300560
		[return: TupleElementNames(new string[]
		{
			"type",
			"required",
			"actual"
		})]
		private List<ValueTuple<int, int, int>> GetRequirementsFromConfig()
		{
			List<ValueTuple<int, int, int>> result = new List<ValueTuple<int, int, int>>();
			CombatSkillItem configData = this._configData;
			bool flag = ((configData != null) ? configData.UsingRequirement : null) == null;
			List<ValueTuple<int, int, int>> result2;
			if (flag)
			{
				result2 = result;
			}
			else
			{
				foreach (PropertyAndValue req in this._configData.UsingRequirement)
				{
					result.Add(new ValueTuple<int, int, int>((int)req.PropertyId, (int)req.Value, -1));
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x060068C9 RID: 26825 RVA: 0x003023FC File Offset: 0x003005FC
		private void RefreshAnnotation()
		{
			bool flag = this._showOnlyTemplateInfo || this._configData == null;
			if (!flag)
			{
				this.annotationArea.SetActive(true);
				this.annotationTitleText.text = LanguageKey.LK_CombatSkill_Annotation_Title.Tr();
				bool flag2 = this._configData.EquipType == 2;
				if (flag2)
				{
					bool hasJumpCharge = this._configData.JumpPrepareFrame > 0;
					this.jumpChargeAnnotationArea.SetActive(hasJumpCharge);
					bool flag3 = hasJumpCharge;
					if (flag3)
					{
						int jumpSpeed = (this._combatSkillDisplayData != null) ? this._combatSkillDisplayData.JumpSpeed : CFormula.CalcJumpSpeed(0);
						bool flag4 = jumpSpeed > 0;
						if (flag4)
						{
							this.jumpChargeAnnotationTitleText.text = LanguageKey.LK_CombatSkill_JumpCharge.Tr();
							string fullText = LanguageKey.LK_CombatSkill_JumpPrepareFrame.TrFormat(this.FormatJumpSpeed(this._configData.JumpPrepareFrame, jumpSpeed));
							this.jumpChargeAnnotationContentText.text = fullText;
						}
						else
						{
							this.jumpChargeAnnotationArea.SetActive(false);
						}
					}
					bool hasMoveInterval = this._configData.MoveCdBonus != 0;
					this.moveIntervalAnnotationArea.SetActive(hasMoveInterval);
					bool flag5 = hasMoveInterval;
					if (flag5)
					{
						this.moveIntervalAnnotationTitleText.text = LanguageKey.LK_CombatSkill_MoveInterval.Tr();
						this.moveIntervalAnnotationContentText.text = LanguageKey.LK_CombatSkill_MoveInterval_Desc.Tr();
					}
				}
				else
				{
					this.moveIntervalAnnotationArea.SetActive(false);
					this.jumpChargeAnnotationArea.SetActive(false);
				}
				this.skillSlotAnnotationArea.gameObject.SetActive(this._configData.EquipType == 0);
				this.loopingAnnotationArea.gameObject.SetActive(this._configData.EquipType == 0);
				bool flag6 = this._configData.EquipType == 0;
				if (flag6)
				{
					this.loopingAnnotationNeiliText.text = LanguageKey.LK_TooltipCombatSkill_Annotation_Looping_1.TrFormat(this._configData.TotalObtainableNeili);
					bool flag7 = this._combatSkillDisplayData != null;
					if (flag7)
					{
						sbyte destType = this._combatSkillDisplayData.FiveElementDestTypeWhileLooping;
						sbyte transferType = this._combatSkillDisplayData.FiveElementTransferTypeWhileLooping;
						bool haveTransfer = destType >= 0;
						this.loopingAnnotationTransferDirectionText.gameObject.SetActive(haveTransfer);
						bool flag8 = haveTransfer;
						if (flag8)
						{
							if (!true)
							{
							}
							sbyte b;
							switch (transferType)
							{
							case 0:
								b = FiveElementsType.Countered[(int)destType];
								break;
							case 1:
								b = FiveElementsType.Countering[(int)destType];
								break;
							case 2:
								b = FiveElementsType.Produced[(int)destType];
								break;
							default:
								b = FiveElementsType.Producing[(int)destType];
								break;
							}
							if (!true)
							{
							}
							sbyte srcType = b;
							string srcString = TooltipCombatSkill.<RefreshAnnotation>g__GetIconString|283_0("ui9_icon_mousetip_elements_" + srcType.ToString(), LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", srcType)));
							string destString = TooltipCombatSkill.<RefreshAnnotation>g__GetIconString|283_0("ui9_icon_mousetip_elements_" + destType.ToString(), LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", destType)));
							this.loopingAnnotationTransferDirectionText.text = LanguageKey.LK_TooltipCombatSkill_Annotation_Looping_3.TrFormat(srcString, destString);
							this.loopingAnnotationTransferDirectionText.GetComponent<TMPTextSpriteHelper>().Parse();
						}
					}
					else
					{
						this.loopingAnnotationTransferDirectionText.gameObject.SetActive(false);
					}
				}
				this.RefreshInnerOuterRatioAnnotation();
			}
		}

		// Token: 0x060068CA RID: 26826 RVA: 0x00302744 File Offset: 0x00300944
		private void RefreshInnerOuterRatioAnnotation()
		{
			bool isActiveSkill = this._configData.EquipType == 1 || this._configData.EquipType == 2 || this._configData.EquipType == 3;
			this.innerOuterRatioAnnotationArea.SetActive(isActiveSkill);
			bool flag = !isActiveSkill;
			if (!flag)
			{
				CombatSkillDisplayData combatSkillDisplayData = this._combatSkillDisplayData;
				sbyte currInnerRatio = (combatSkillDisplayData != null) ? combatSkillDisplayData.CurrInnerRatio : this._configData.BaseInnerRatio;
				int outerRatio = (int)(100 - currInnerRatio);
				this.innerRatioText.text = string.Format("{0}%", currInnerRatio);
				this.outerRatioText.text = string.Format("{0}%", outerRatio);
				float ratio = (float)currInnerRatio / 100f;
				this.innerRatioFillImage.fillAmount = ratio;
				this.outerRatioFillImage.fillAmount = 1f - ratio;
				float barWidth = this.innerOuterRatioBar.rect.width;
				this.innerOuterRatioHandle.anchoredPosition = new Vector2((1f - ratio) * barWidth, this.innerOuterRatioHandle.anchoredPosition.y);
			}
		}

		// Token: 0x060068CB RID: 26827 RVA: 0x00302860 File Offset: 0x00300A60
		private void RefreshLegendaryBook()
		{
			bool isTaiwu = this._charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			bool flag = this._showOnlyTemplateInfo || this._combatSkillDisplayData == null || !isTaiwu || this._combatSkillDisplayData.LegendaryBookSlotIds == null || this._combatSkillDisplayData.LegendaryBookSlotIds.Count == 0;
			if (flag)
			{
				this.legendaryBookArea.SetActive(false);
				this.legendaryBookTemplateItem.gameObject.SetActive(false);
				this.SetLegendaryBookRowCount(0);
			}
			else
			{
				List<short> slotIds = this._combatSkillDisplayData.LegendaryBookSlotIds;
				this.legendaryBookArea.SetActive(true);
				this.legendaryBookTemplateItem.gameObject.SetActive(true);
				this.legendaryBookTitleText.text = LanguageKey.LK_CombatSkill_SpecialEffect_Title.Tr();
				this.SetLegendaryBookRowCount(slotIds.Count);
				for (int i = 0; i < slotIds.Count; i++)
				{
					LegendaryBookSlotItem slotConfig = LegendaryBookSlot.Instance[slotIds[i]];
					this._legendaryBookRows[i].Set(LanguageKey.LK_CombatSkill_LegendaryBook_Title.Tr(), slotConfig.Name);
					this._legendaryBookRows[i].gameObject.SetActive(true);
				}
				for (int j = slotIds.Count; j < this._legendaryBookRows.Count; j++)
				{
					this._legendaryBookRows[j].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x060068CC RID: 26828 RVA: 0x003029DC File Offset: 0x00300BDC
		private void SetLegendaryBookRowCount(int count)
		{
			bool flag = this._legendaryBookRows.Count == 0;
			if (flag)
			{
				this._legendaryBookRows.Add(this.legendaryBookTemplateItem);
			}
			CommonUtils.PrepareEnoughChildren(this.legendaryBookContainer, this.legendaryBookTemplateItem.gameObject, count, null);
			while (this._legendaryBookRows.Count < count)
			{
				this._legendaryBookRows.Add(this.legendaryBookContainer.GetChild(this._legendaryBookRows.Count).GetComponent<TooltipCombatSkillTextItem>());
			}
		}

		// Token: 0x060068CD RID: 26829 RVA: 0x00302A6C File Offset: 0x00300C6C
		private void RefreshLegendaryBookAnnotation()
		{
			bool isTaiwu = this._charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			bool flag = this._showOnlyTemplateInfo || this._combatSkillDisplayData == null || !isTaiwu || this._combatSkillDisplayData.LegendaryBookSlotIds == null || this._combatSkillDisplayData.LegendaryBookSlotIds.Count == 0;
			if (flag)
			{
				this.legendaryBookAnnotationArea.SetActive(false);
				this.legendaryBookAnnotationTemplateItem.gameObject.SetActive(false);
				this.SetLegendaryBookAnnotationRowCount(0);
			}
			else
			{
				List<short> slotIds = this._combatSkillDisplayData.LegendaryBookSlotIds;
				this.legendaryBookAnnotationArea.SetActive(true);
				this.legendaryBookAnnotationTemplateItem.gameObject.SetActive(true);
				this.SetLegendaryBookAnnotationRowCount(slotIds.Count);
				for (int i = 0; i < slotIds.Count; i++)
				{
					LegendaryBookSlotItem slotConfig = LegendaryBookSlot.Instance[slotIds[i]];
					this._legendaryBookAnnotationRows[i].Set(slotConfig.Name, slotConfig.Desc.ColorReplace());
					this._legendaryBookAnnotationRows[i].gameObject.SetActive(true);
				}
				for (int j = slotIds.Count; j < this._legendaryBookAnnotationRows.Count; j++)
				{
					this._legendaryBookAnnotationRows[j].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x060068CE RID: 26830 RVA: 0x00302BD4 File Offset: 0x00300DD4
		private void SetLegendaryBookAnnotationRowCount(int count)
		{
			bool flag = this._legendaryBookAnnotationRows.Count == 0;
			if (flag)
			{
				this._legendaryBookAnnotationRows.Add(this.legendaryBookAnnotationTemplateItem);
			}
			CommonUtils.PrepareEnoughChildren(this.legendaryBookAnnotationContainer, this.legendaryBookAnnotationTemplateItem.gameObject, count, null);
			while (this._legendaryBookAnnotationRows.Count < count)
			{
				this._legendaryBookAnnotationRows.Add(this.legendaryBookAnnotationContainer.GetChild(this._legendaryBookAnnotationRows.Count).GetComponent<TooltipCombatSkillTextItem>());
			}
		}

		// Token: 0x060068CF RID: 26831 RVA: 0x00302C64 File Offset: 0x00300E64
		public static void EnsureRowCount<T>(List<T> rows, T template, Transform container, int count, CommonUtils.PrepareExtraItemInfo? config = null) where T : Component
		{
			rows.Clear();
			CommonUtils.PrepareEnoughChildren(container, template.gameObject, count, config);
			for (int i = 0; i < count; i++)
			{
				rows.Add(container.GetChild(i + ((config != null) ? config.GetValueOrDefault().ExtraItemCount : 0)).GetComponent<T>());
			}
		}

		// Token: 0x060068D0 RID: 26832 RVA: 0x00302CCC File Offset: 0x00300ECC
		private bool GetPropertyIsInSpecialBreaks(short propertyId, bool isEquipEffect)
		{
			bool flag = this._combatSkillDisplayData == null || this._combatSkillDisplayData.BreakAddProperty == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < this._combatSkillDisplayData.BreakAddProperty.Count; i++)
				{
					ValueTuple<short, short, bool> addProperty = this._combatSkillDisplayData.BreakAddProperty[i];
					bool flag2 = (int)addProperty.Item1 < CharacterPropertyReferenced.Instance.Count && isEquipEffect;
					if (flag2)
					{
						bool flag3 = addProperty.Item1 == propertyId && addProperty.Item3;
						if (flag3)
						{
							return true;
						}
					}
					else
					{
						bool flag4 = (int)addProperty.Item1 >= CharacterPropertyReferenced.Instance.Count && !isEquipEffect;
						if (flag4)
						{
							bool flag5 = (int)addProperty.Item1 - CharacterPropertyReferenced.Instance.Count == (int)propertyId && addProperty.Item3;
							if (flag5)
							{
								return true;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060068D1 RID: 26833 RVA: 0x00302DC0 File Offset: 0x00300FC0
		private bool[] CalInjuryPartAtkRatesBySpecialBreak()
		{
			bool[] showBacks = new bool[7];
			bool propertyIsInSpecialBreaks = this.GetPropertyIsInSpecialBreaks(35, false);
			if (propertyIsInSpecialBreaks)
			{
				showBacks[0] = true;
			}
			else
			{
				bool propertyIsInSpecialBreaks2 = this.GetPropertyIsInSpecialBreaks(36, false);
				if (propertyIsInSpecialBreaks2)
				{
					showBacks[1] = true;
				}
				else
				{
					bool propertyIsInSpecialBreaks3 = this.GetPropertyIsInSpecialBreaks(37, false);
					if (propertyIsInSpecialBreaks3)
					{
						showBacks[2] = true;
					}
					else
					{
						bool propertyIsInSpecialBreaks4 = this.GetPropertyIsInSpecialBreaks(38, false);
						if (propertyIsInSpecialBreaks4)
						{
							showBacks[3] = true;
							showBacks[4] = true;
						}
						else
						{
							bool propertyIsInSpecialBreaks5 = this.GetPropertyIsInSpecialBreaks(39, false);
							if (propertyIsInSpecialBreaks5)
							{
								showBacks[5] = true;
								showBacks[6] = true;
							}
						}
					}
				}
			}
			return showBacks;
		}

		// Token: 0x060068D2 RID: 26834 RVA: 0x00302E50 File Offset: 0x00301050
		private void HideAllSpecialBack()
		{
			bool flag = this.powerSpecialBack;
			if (flag)
			{
				this.powerSpecialBack.enabled = false;
			}
			bool flag2 = this.maxPowerSpecialBack;
			if (flag2)
			{
				this.maxPowerSpecialBack.enabled = false;
			}
			bool flag3 = this.distanceSpecialBack;
			if (flag3)
			{
				this.distanceSpecialBack.enabled = false;
			}
			bool flag4 = this.penetrateSpecialBack;
			if (flag4)
			{
				this.penetrateSpecialBack.enabled = false;
			}
			bool flag5 = this.breathStanceSpecialBack;
			if (flag5)
			{
				this.breathStanceSpecialBack.enabled = false;
			}
			bool flag6 = this.mobilityCostSpecialBack;
			if (flag6)
			{
				this.mobilityCostSpecialBack.enabled = false;
			}
			bool flag7 = this.costTrickAreaSpecialBack;
			if (flag7)
			{
				this.costTrickAreaSpecialBack.enabled = false;
			}
			bool flag8 = this.effectDurationSpecialBack;
			if (flag8)
			{
				this.effectDurationSpecialBack.enabled = false;
			}
		}

		// Token: 0x060068D5 RID: 26837 RVA: 0x00303088 File Offset: 0x00301288
		[CompilerGenerated]
		internal static string <RefreshAnnotation>g__GetIconString|283_0(string iconName, string content)
		{
			return TMPTextSpriteHelper.GetStringWithTextSpriteTag(iconName) + content;
		}

		// Token: 0x04004A26 RID: 18982
		private short _combatSkillTemplateId;

		// Token: 0x04004A27 RID: 18983
		private int _charId;

		// Token: 0x04004A28 RID: 18984
		private CombatSkillItem _configData;

		// Token: 0x04004A29 RID: 18985
		private CombatSkillDisplayData _combatSkillDisplayData;

		// Token: 0x04004A2A RID: 18986
		private bool _showOnlyTemplateInfo;

		// Token: 0x04004A2B RID: 18987
		private bool _isSimple;

		// Token: 0x04004A2C RID: 18988
		private bool _lastAltDown;

		// Token: 0x04004A2D RID: 18989
		private Vector2 _lastMainSize;

		// Token: 0x04004A2E RID: 18990
		private readonly StringBuilder _strBuilder = new StringBuilder();

		// Token: 0x04004A2F RID: 18991
		private readonly List<TooltipCombatSkillPoisonItem> _poisonRows = new List<TooltipCombatSkillPoisonItem>();

		// Token: 0x04004A30 RID: 18992
		private readonly List<TooltipCombatSkillEffectItem> _equipEffectRows = new List<TooltipCombatSkillEffectItem>();

		// Token: 0x04004A31 RID: 18993
		private readonly List<TooltipCombatSkillCastEffectItem> _castEffectRows = new List<TooltipCombatSkillCastEffectItem>();

		// Token: 0x04004A32 RID: 18994
		private readonly List<TooltipCombatSkillDefenseSpecialItem> _defenseSpecialRows = new List<TooltipCombatSkillDefenseSpecialItem>();

		// Token: 0x04004A33 RID: 18995
		private readonly List<TooltipCombatSkillHitInfoItem> _hitInfoRows = new List<TooltipCombatSkillHitInfoItem>();

		// Token: 0x04004A34 RID: 18996
		private readonly List<TooltipCombatSkillBodyStrongItem> _bodyStrongRows = new List<TooltipCombatSkillBodyStrongItem>();

		// Token: 0x04004A35 RID: 18997
		private readonly List<TooltipCombatSkillRequirementItem> _requirementRows = new List<TooltipCombatSkillRequirementItem>();

		// Token: 0x04004A36 RID: 18998
		private readonly List<TooltipCombatSkillCostTrickItem> _costTrickRows = new List<TooltipCombatSkillCostTrickItem>();

		// Token: 0x04004A37 RID: 18999
		private readonly List<TooltipCombatSkillEffectItem> _neiliAllocationRows = new List<TooltipCombatSkillEffectItem>();

		// Token: 0x04004A38 RID: 19000
		private static readonly int[] HitTypeOrder = new int[]
		{
			0,
			1,
			2,
			3
		};

		// Token: 0x04004A39 RID: 19001
		private static readonly string[] FramePaths = new string[]
		{
			"ui9_icon_combat_skill_type_neigong_",
			"ui9_icon_combat_skill_type_attack_",
			"ui9_icon_combat_skill_type_agile_",
			"ui9_icon_combat_skill_type_defense_",
			"ui9_icon_combat_skill_type_assist_"
		};

		// Token: 0x04004A3A RID: 19002
		public static readonly Dictionary<int, int> EquipTypeToZhenqiIconSuffix = new Dictionary<int, int>
		{
			{
				0,
				5
			},
			{
				1,
				0
			},
			{
				2,
				1
			},
			{
				3,
				2
			},
			{
				4,
				3
			}
		};

		// Token: 0x04004A3B RID: 19003
		[Header("基础信息")]
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04004A3C RID: 19004
		[SerializeField]
		private TextMeshProUGUI gradeText;

		// Token: 0x04004A3D RID: 19005
		[SerializeField]
		private CImage skillIcon;

		// Token: 0x04004A3E RID: 19006
		[SerializeField]
		private CImage frame;

		// Token: 0x04004A3F RID: 19007
		[SerializeField]
		private CImage fiveElementsFrame;

		// Token: 0x04004A40 RID: 19008
		[SerializeField]
		private CImage sectIcon;

		// Token: 0x04004A41 RID: 19009
		[SerializeField]
		private TextMeshProUGUI sectText;

		// Token: 0x04004A42 RID: 19010
		[SerializeField]
		private CImage equipTypeIcon;

		// Token: 0x04004A43 RID: 19011
		[SerializeField]
		private TextMeshProUGUI equipTypeText;

		// Token: 0x04004A44 RID: 19012
		[SerializeField]
		private CImage typeIcon;

		// Token: 0x04004A45 RID: 19013
		[SerializeField]
		private TextMeshProUGUI typeText;

		// Token: 0x04004A46 RID: 19014
		[SerializeField]
		private TextMeshProUGUI gridCountText;

		// Token: 0x04004A47 RID: 19015
		[SerializeField]
		private CImage fiveElementsIcon;

		// Token: 0x04004A48 RID: 19016
		[SerializeField]
		private TextMeshProUGUI fiveElementsText;

		// Token: 0x04004A49 RID: 19017
		[SerializeField]
		private CImage gradeBack;

		// Token: 0x04004A4A RID: 19018
		[SerializeField]
		private TextMeshProUGUI descText;

		// Token: 0x04004A4B RID: 19019
		[Header("内功格数")]
		[SerializeField]
		private GameObject gridCountArea;

		// Token: 0x04004A4C RID: 19020
		[SerializeField]
		private TooltipCombatSkillGridItem attackGridItem;

		// Token: 0x04004A4D RID: 19021
		[SerializeField]
		private TooltipCombatSkillGridItem agileGridItem;

		// Token: 0x04004A4E RID: 19022
		[SerializeField]
		private TooltipCombatSkillGridItem defenceGridItem;

		// Token: 0x04004A4F RID: 19023
		[SerializeField]
		private TooltipCombatSkillGridItem specialGridItem;

		// Token: 0x04004A50 RID: 19024
		[SerializeField]
		private TooltipCombatSkillGridItem genericGridItem;

		// Token: 0x04004A51 RID: 19025
		[Header("周天运转")]
		[SerializeField]
		private GameObject loopingArea;

		// Token: 0x04004A52 RID: 19026
		[SerializeField]
		private TextMeshProUGUI obtainedNeiliText;

		// Token: 0x04004A53 RID: 19027
		[SerializeField]
		private GameObject fiveElementTransferWhileLoopingArea;

		// Token: 0x04004A54 RID: 19028
		[SerializeField]
		private CImage srcFiveElementsWhileLoopingIcon;

		// Token: 0x04004A55 RID: 19029
		[SerializeField]
		private TextMeshProUGUI srcFiveElementsWhileLoopingText;

		// Token: 0x04004A56 RID: 19030
		[SerializeField]
		private CImage destFiveElementsWhileLoopingIcon;

		// Token: 0x04004A57 RID: 19031
		[SerializeField]
		private TextMeshProUGUI destFiveElementsWhileLoopingText;

		// Token: 0x04004A58 RID: 19032
		[Header("战斗属性")]
		[SerializeField]
		private GameObject fightArea;

		// Token: 0x04004A59 RID: 19033
		[SerializeField]
		private GameObject fightDistanceArea;

		// Token: 0x04004A5A RID: 19034
		[SerializeField]
		private TextMeshProUGUI fightDistanceTitleText;

		// Token: 0x04004A5B RID: 19035
		[SerializeField]
		private TextMeshProUGUI fightNearDistanceText;

		// Token: 0x04004A5C RID: 19036
		[SerializeField]
		private TextMeshProUGUI fightFarDistanceText;

		// Token: 0x04004A5D RID: 19037
		[SerializeField]
		private GameObject fightPowerArea;

		// Token: 0x04004A5E RID: 19038
		[SerializeField]
		private TextMeshProUGUI fightPowerTitleText;

		// Token: 0x04004A5F RID: 19039
		[SerializeField]
		private TextMeshProUGUI fightPowerValueText;

		// Token: 0x04004A60 RID: 19040
		[SerializeField]
		private GameObject fightHitPartsArea;

		// Token: 0x04004A61 RID: 19041
		[SerializeField]
		private TextMeshProUGUI fightHitPartsTitleText;

		// Token: 0x04004A62 RID: 19042
		[SerializeField]
		private Transform fightHitPartContainer;

		// Token: 0x04004A63 RID: 19043
		[SerializeField]
		private TooltipCombatSkillHitPartItem fightHitPartTemplateItem;

		// Token: 0x04004A64 RID: 19044
		[SerializeField]
		private GameObject fightPoisonArea;

		// Token: 0x04004A65 RID: 19045
		[SerializeField]
		private Transform fightPoisonContainer;

		// Token: 0x04004A66 RID: 19046
		[SerializeField]
		private TooltipCombatSkillPoisonItem fightPoisonTemplateItem;

		// Token: 0x04004A67 RID: 19047
		[Header("属性容器")]
		[SerializeField]
		private GameObject effectArea;

		// Token: 0x04004A68 RID: 19048
		[SerializeField]
		private CImage effectAreaLine;

		// Token: 0x04004A69 RID: 19049
		[Header("攻击属性")]
		[SerializeField]
		private GameObject attackPropertyArea;

		// Token: 0x04004A6A RID: 19050
		[SerializeField]
		private TextMeshProUGUI attackPropertyTitleText;

		// Token: 0x04004A6B RID: 19051
		[SerializeField]
		private TooltipCombatSkillAttackPropertyItem outerPenetrateItem;

		// Token: 0x04004A6C RID: 19052
		[SerializeField]
		private TooltipCombatSkillAttackPropertyItem innerPenetrateItem;

		// Token: 0x04004A6D RID: 19053
		[SerializeField]
		private TextMeshProUGUI atkFlawText;

		// Token: 0x04004A6E RID: 19054
		[SerializeField]
		private TextMeshProUGUI atkAcupointText;

		// Token: 0x04004A6F RID: 19055
		[Header("运功效果")]
		[SerializeField]
		private GameObject equipEffectArea;

		// Token: 0x04004A70 RID: 19056
		[SerializeField]
		private Transform equipEffectContainer;

		// Token: 0x04004A71 RID: 19057
		[SerializeField]
		private TooltipCombatSkillEffectItem equipEffectTemplateItem;

		// Token: 0x04004A72 RID: 19058
		[Header("真气加成")]
		[SerializeField]
		private GameObject neiliAllocationArea;

		// Token: 0x04004A73 RID: 19059
		[SerializeField]
		private Transform neiliAllocationContainer;

		// Token: 0x04004A74 RID: 19060
		[SerializeField]
		private TooltipCombatSkillEffectItem neiliAllocationTemplateItem;

		// Token: 0x04004A75 RID: 19061
		[Header("施展效果")]
		[SerializeField]
		private GameObject castEffectArea;

		// Token: 0x04004A76 RID: 19062
		[SerializeField]
		private Transform castEffectContainer;

		// Token: 0x04004A77 RID: 19063
		[SerializeField]
		private TooltipCombatSkillCastEffectItem castEffectTemplateItem;

		// Token: 0x04004A78 RID: 19064
		[Header("护体特殊施展效果")]
		[SerializeField]
		private GameObject defenseSpecialArea;

		// Token: 0x04004A79 RID: 19065
		[SerializeField]
		private GameObject effectDurationRow;

		// Token: 0x04004A7A RID: 19066
		[SerializeField]
		private TextMeshProUGUI effectDurationText;

		// Token: 0x04004A7B RID: 19067
		[SerializeField]
		private Transform defenseSpecialContainer;

		// Token: 0x04004A7C RID: 19068
		[SerializeField]
		private TooltipCombatSkillDefenseSpecialItem defenseSpecialTemplateItem;

		// Token: 0x04004A7D RID: 19069
		[Header("特殊效果")]
		[SerializeField]
		private GameObject taiJiQuanLeveragingRow;

		// Token: 0x04004A7E RID: 19070
		[SerializeField]
		private TextMeshProUGUI taiJiQuanLeveragingValueText;

		// Token: 0x04004A7F RID: 19071
		[SerializeField]
		private GameObject shuiHuoYingQiGongReduceDamageRow;

		// Token: 0x04004A80 RID: 19072
		[SerializeField]
		private TextMeshProUGUI shuiHuoYingQiGongReduceDamageValueText;

		// Token: 0x04004A81 RID: 19073
		[Header("正逆练特效")]
		[SerializeField]
		private GameObject directionArea;

		// Token: 0x04004A82 RID: 19074
		[SerializeField]
		private GameObject directEffectTitle;

		// Token: 0x04004A83 RID: 19075
		[SerializeField]
		private TextMeshProUGUI directEffectTitleText;

		// Token: 0x04004A84 RID: 19076
		[SerializeField]
		private CImage directEffectIcon;

		// Token: 0x04004A85 RID: 19077
		[SerializeField]
		private GameObject directEffectDesc;

		// Token: 0x04004A86 RID: 19078
		[SerializeField]
		private TextMeshProUGUI directEffectDescText;

		// Token: 0x04004A87 RID: 19079
		[SerializeField]
		private GameObject reverseEffectTitle;

		// Token: 0x04004A88 RID: 19080
		[SerializeField]
		private TextMeshProUGUI reverseEffectTitleText;

		// Token: 0x04004A89 RID: 19081
		[SerializeField]
		private CImage reverseEffectIcon;

		// Token: 0x04004A8A RID: 19082
		[SerializeField]
		private GameObject reverseEffectDesc;

		// Token: 0x04004A8B RID: 19083
		[SerializeField]
		private TextMeshProUGUI reverseEffectDescText;

		// Token: 0x04004A8C RID: 19084
		[Header("命中构成")]
		[SerializeField]
		private GameObject hitInfoArea;

		// Token: 0x04004A8D RID: 19085
		[SerializeField]
		private TextMeshProUGUI hitInfoTitleText;

		// Token: 0x04004A8E RID: 19086
		[SerializeField]
		private CImage hitChartBack0;

		// Token: 0x04004A8F RID: 19087
		[SerializeField]
		private CImage hitChartBack1;

		// Token: 0x04004A90 RID: 19088
		[SerializeField]
		private CImage hitChartBack2;

		// Token: 0x04004A91 RID: 19089
		[SerializeField]
		private CImage hitChartBack3;

		// Token: 0x04004A92 RID: 19090
		[SerializeField]
		private Transform hitInfoContainer;

		// Token: 0x04004A93 RID: 19091
		[SerializeField]
		private TooltipCombatSkillHitInfoItem hitInfoTemplateItem;

		// Token: 0x04004A94 RID: 19092
		[Header("施展需要")]
		[SerializeField]
		private GameObject castNeedArea;

		// Token: 0x04004A95 RID: 19093
		[SerializeField]
		private TextMeshProUGUI castNeedTitleText;

		// Token: 0x04004A96 RID: 19094
		[SerializeField]
		private GameObject bodyPartArea;

		// Token: 0x04004A97 RID: 19095
		[SerializeField]
		private Transform bodyPartGroupContainer;

		// Token: 0x04004A98 RID: 19096
		[SerializeField]
		private TooltipCombatSkillBodyPartGroup bodyPartGroupTemplate;

		// Token: 0x04004A99 RID: 19097
		private readonly List<TooltipCombatSkillBodyPartGroup> _bodyPartGroups = new List<TooltipCombatSkillBodyPartGroup>();

		// Token: 0x04004A9A RID: 19098
		private readonly List<List<TooltipCombatSkillBodyPartItem>> _bodyPartGroupItems = new List<List<TooltipCombatSkillBodyPartItem>>();

		// Token: 0x04004A9B RID: 19099
		[SerializeField]
		private GameObject breathStanceCostArea;

		// Token: 0x04004A9C RID: 19100
		[SerializeField]
		private CImage breathCostIcon;

		// Token: 0x04004A9D RID: 19101
		[SerializeField]
		private TextMeshProUGUI breathCostText;

		// Token: 0x04004A9E RID: 19102
		[SerializeField]
		private CImage stanceCostIcon;

		// Token: 0x04004A9F RID: 19103
		[SerializeField]
		private TextMeshProUGUI stanceCostText;

		// Token: 0x04004AA0 RID: 19104
		[SerializeField]
		private GameObject mobilityCostArea;

		// Token: 0x04004AA1 RID: 19105
		[SerializeField]
		private TextMeshProUGUI mobilityCostText;

		// Token: 0x04004AA2 RID: 19106
		[SerializeField]
		private GameObject mobilityCostDetailArea;

		// Token: 0x04004AA3 RID: 19107
		[SerializeField]
		private TextMeshProUGUI mobilityCostPerFrameText;

		// Token: 0x04004AA4 RID: 19108
		[SerializeField]
		private TextMeshProUGUI mobilityCostByMoveText;

		// Token: 0x04004AA5 RID: 19109
		[SerializeField]
		private GameObject costTrickArea;

		// Token: 0x04004AA6 RID: 19110
		[SerializeField]
		private Transform costTrickContainer;

		// Token: 0x04004AA7 RID: 19111
		[SerializeField]
		private TooltipCombatSkillCostTrickItem costTrickTemplateItem;

		// Token: 0x04004AA8 RID: 19112
		[SerializeField]
		private GameObject weaponDurabilityCostArea;

		// Token: 0x04004AA9 RID: 19113
		[SerializeField]
		private TextMeshProUGUI weaponDurabilityCostText;

		// Token: 0x04004AAA RID: 19114
		[Header("特殊效果")]
		[SerializeField]
		private GameObject legendaryBookArea;

		// Token: 0x04004AAB RID: 19115
		[SerializeField]
		private TextMeshProUGUI legendaryBookTitleText;

		// Token: 0x04004AAC RID: 19116
		[SerializeField]
		private Transform legendaryBookContainer;

		// Token: 0x04004AAD RID: 19117
		[SerializeField]
		private TooltipCombatSkillTextItem legendaryBookTemplateItem;

		// Token: 0x04004AAE RID: 19118
		private readonly List<TooltipCombatSkillTextItem> _legendaryBookRows = new List<TooltipCombatSkillTextItem>();

		// Token: 0x04004AAF RID: 19119
		private readonly List<TooltipCombatSkillTextItem> _legendaryBookAnnotationRows = new List<TooltipCombatSkillTextItem>();

		// Token: 0x04004AB0 RID: 19120
		[Header("身心强健")]
		[SerializeField]
		private GameObject bodyStrongArea;

		// Token: 0x04004AB1 RID: 19121
		[SerializeField]
		private TextMeshProUGUI bodyStrongTitleText;

		// Token: 0x04004AB2 RID: 19122
		[SerializeField]
		private Transform bodyStrongContainer;

		// Token: 0x04004AB3 RID: 19123
		[SerializeField]
		private TooltipCombatSkillBodyStrongItem bodyStrongTemplateItem;

		// Token: 0x04004AB4 RID: 19124
		[Header("发挥需求")]
		[SerializeField]
		private GameObject requirementArea;

		// Token: 0x04004AB5 RID: 19125
		[SerializeField]
		private TextMeshProUGUI requirementTitleText;

		// Token: 0x04004AB6 RID: 19126
		[SerializeField]
		private TextMeshProUGUI requirementPowerText;

		// Token: 0x04004AB7 RID: 19127
		[SerializeField]
		private Transform requirementContainer;

		// Token: 0x04004AB8 RID: 19128
		[SerializeField]
		private TooltipCombatSkillRequirementItem requirementTemplateItem;

		// Token: 0x04004AB9 RID: 19129
		[Header("右侧正逆练")]
		[SerializeField]
		private GameObject rightDirectionArea;

		// Token: 0x04004ABA RID: 19130
		[SerializeField]
		private GameObject rightDirectEffectArea;

		// Token: 0x04004ABB RID: 19131
		[SerializeField]
		private TextMeshProUGUI rightDirectEffectTitleText;

		// Token: 0x04004ABC RID: 19132
		[SerializeField]
		private CImage rightDirectEffectIcon;

		// Token: 0x04004ABD RID: 19133
		[SerializeField]
		private TextMeshProUGUI rightDirectEffectDescText;

		// Token: 0x04004ABE RID: 19134
		[SerializeField]
		private GameObject rightReverseEffectArea;

		// Token: 0x04004ABF RID: 19135
		[SerializeField]
		private TextMeshProUGUI rightReverseEffectTitleText;

		// Token: 0x04004AC0 RID: 19136
		[SerializeField]
		private CImage rightReverseEffectIcon;

		// Token: 0x04004AC1 RID: 19137
		[SerializeField]
		private TextMeshProUGUI rightReverseEffectDescText;

		// Token: 0x04004AC2 RID: 19138
		[Header("注解")]
		[SerializeField]
		private TextMeshProUGUI annotationTitleText;

		// Token: 0x04004AC3 RID: 19139
		[Header("功法栏位注解")]
		[SerializeField]
		private RectTransform skillSlotAnnotationArea;

		// Token: 0x04004AC4 RID: 19140
		[Header("周天运转注解")]
		[SerializeField]
		private RectTransform loopingAnnotationArea;

		// Token: 0x04004AC5 RID: 19141
		[SerializeField]
		private TextMeshProUGUI loopingAnnotationNeiliText;

		// Token: 0x04004AC6 RID: 19142
		[SerializeField]
		private TextMeshProUGUI loopingAnnotationTransferText;

		// Token: 0x04004AC7 RID: 19143
		[SerializeField]
		private TextMeshProUGUI loopingAnnotationTransferDirectionText;

		// Token: 0x04004AC8 RID: 19144
		[Header("跳跃蓄力注解")]
		[SerializeField]
		private GameObject jumpChargeAnnotationArea;

		// Token: 0x04004AC9 RID: 19145
		[SerializeField]
		private TextMeshProUGUI jumpChargeAnnotationTitleText;

		// Token: 0x04004ACA RID: 19146
		[SerializeField]
		private TextMeshProUGUI jumpChargeAnnotationContentText;

		// Token: 0x04004ACB RID: 19147
		[Header("移动间隔注解")]
		[SerializeField]
		private GameObject moveIntervalAnnotationArea;

		// Token: 0x04004ACC RID: 19148
		[SerializeField]
		private TextMeshProUGUI moveIntervalAnnotationTitleText;

		// Token: 0x04004ACD RID: 19149
		[SerializeField]
		private TextMeshProUGUI moveIntervalAnnotationContentText;

		// Token: 0x04004ACE RID: 19150
		[Header("发挥需求注解")]
		[SerializeField]
		private GameObject requirementAnnotationArea;

		// Token: 0x04004ACF RID: 19151
		[Header("词条效果注解")]
		[SerializeField]
		private GameObject legendaryBookAnnotationArea;

		// Token: 0x04004AD0 RID: 19152
		[SerializeField]
		private Transform legendaryBookAnnotationContainer;

		// Token: 0x04004AD1 RID: 19153
		[SerializeField]
		private TooltipCombatSkillTextItem legendaryBookAnnotationTemplateItem;

		// Token: 0x04004AD2 RID: 19154
		[Header("内外功比例注解")]
		[SerializeField]
		private GameObject innerOuterRatioAnnotationArea;

		// Token: 0x04004AD3 RID: 19155
		[SerializeField]
		private RectTransform innerOuterRatioBar;

		// Token: 0x04004AD4 RID: 19156
		[SerializeField]
		private CImage innerRatioFillImage;

		// Token: 0x04004AD5 RID: 19157
		[SerializeField]
		private CImage outerRatioFillImage;

		// Token: 0x04004AD6 RID: 19158
		[SerializeField]
		private RectTransform innerOuterRatioHandle;

		// Token: 0x04004AD7 RID: 19159
		[SerializeField]
		private TextMeshProUGUI outerRatioText;

		// Token: 0x04004AD8 RID: 19160
		[SerializeField]
		private TextMeshProUGUI innerRatioText;

		// Token: 0x04004AD9 RID: 19161
		[Header("根节点布局")]
		[SerializeField]
		private GameObject mainArea;

		// Token: 0x04004ADA RID: 19162
		[Header("详细模式右侧")]
		[SerializeField]
		private GameObject detailRightArea;

		// Token: 0x04004ADB RID: 19163
		[SerializeField]
		private GameObject rightMainArea;

		// Token: 0x04004ADC RID: 19164
		[SerializeField]
		private GameObject annotationArea;

		// Token: 0x04004ADD RID: 19165
		[Header("SpecialBack - 峨眉突破高亮")]
		[SerializeField]
		private CImage powerSpecialBack;

		// Token: 0x04004ADE RID: 19166
		[SerializeField]
		private CImage maxPowerSpecialBack;

		// Token: 0x04004ADF RID: 19167
		[SerializeField]
		private CImage distanceSpecialBack;

		// Token: 0x04004AE0 RID: 19168
		[SerializeField]
		private CImage penetrateSpecialBack;

		// Token: 0x04004AE1 RID: 19169
		[SerializeField]
		private CImage breathStanceSpecialBack;

		// Token: 0x04004AE2 RID: 19170
		[SerializeField]
		private CImage mobilityCostSpecialBack;

		// Token: 0x04004AE3 RID: 19171
		[SerializeField]
		private CImage costTrickAreaSpecialBack;

		// Token: 0x04004AE4 RID: 19172
		[SerializeField]
		private CImage effectDurationSpecialBack;

		// Token: 0x04004AE5 RID: 19173
		[Header("展开收缩提示")]
		[SerializeField]
		private TextMeshProUGUI pressTipLeftText;

		// Token: 0x04004AE6 RID: 19174
		[SerializeField]
		private TextMeshProUGUI pressTipRightText;
	}
}
