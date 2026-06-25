using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Taiwu;
using UISkillBreakPlate;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B7C RID: 2940
	public static class PracticeSkillPlatePageUtils
	{
		// Token: 0x0600914A RID: 37194 RVA: 0x0043B8E0 File Offset: 0x00439AE0
		public static void RefreshOtherToggles(CombatSkillDisplayData skillData, int charId, bool isTaiwu, GameData.Domains.Taiwu.SkillBreakPlate breakPlate, CToggleGroupObsolete otherPageToggleGroup)
		{
			bool flag = skillData == null;
			if (!flag)
			{
				bool broken = CombatSkillStateHelper.IsBrokenOut(skillData.ActivationState);
				for (byte i = 1; i <= 5; i += 1)
				{
					byte directIndex = CombatSkillStateHelper.GetNormalPageInternalIndex(0, i);
					byte reverseIndex = CombatSkillStateHelper.GetNormalPageInternalIndex(1, i);
					bool isDirectRead = CombatSkillStateHelper.IsPageRead(skillData.ReadingState, directIndex);
					bool isReverseRead = CombatSkillStateHelper.IsPageRead(skillData.ReadingState, reverseIndex);
					bool canCancel = !broken && breakPlate == null;
					bool isFailed = breakPlate != null && breakPlate.Failed;
					sbyte pageActiveDirection = CombatSkillStateHelper.GetPageActiveDirection(skillData.ActivationState, i);
					bool canSwitchToDirect = pageActiveDirection == 1;
					bool canSwitchToReverse = pageActiveDirection == 0;
					bool directInteractable = isTaiwu && !skillData.Revoked && isDirectRead && (canCancel || canSwitchToDirect || isFailed);
					bool reverseInteractable = isTaiwu && !skillData.Revoked && isReverseRead && (canCancel || canSwitchToReverse || isFailed);
					bool showDirectPage = !skillData.Revoked && isDirectRead;
					bool showReversePage = !skillData.Revoked && isReverseRead;
					PracticeSlice directPageSlice = otherPageToggleGroup.Get((int)directIndex).GetComponent<PracticeSlice>();
					PracticeSlice reversePageSlice = otherPageToggleGroup.Get((int)reverseIndex).GetComponent<PracticeSlice>();
					directPageSlice.SetPageShow(showDirectPage);
					directPageSlice.SetNameBgActive(showDirectPage);
					directPageSlice.SetInteractable(directInteractable);
					TooltipInvoker directTip = directPageSlice.GetPageTip();
					TooltipInvoker directTipNoPage = directPageSlice.GetNoPageTip();
					reversePageSlice.SetPageShow(showReversePage);
					reversePageSlice.SetNameBgActive(showReversePage);
					reversePageSlice.SetInteractable(reverseInteractable);
					TooltipInvoker reverseTip = reversePageSlice.GetPageTip();
					TooltipInvoker reverseTipNoPage = reversePageSlice.GetNoPageTip();
					PracticeSkillPlatePageUtils.RefreshOtherPageTips((int)(i - 1), directTip, skillData.TemplateId, true, true, showDirectPage, true, otherPageToggleGroup);
					PracticeSkillPlatePageUtils.RefreshOtherPageTips((int)(i - 1), directTipNoPage, skillData.TemplateId, true, true, showDirectPage, true, otherPageToggleGroup);
					PracticeSkillPlatePageUtils.RefreshOtherPageTips((int)(i - 1), reverseTip, skillData.TemplateId, false, true, showReversePage, true, otherPageToggleGroup);
					PracticeSkillPlatePageUtils.RefreshOtherPageTips((int)(i - 1), reverseTipNoPage, skillData.TemplateId, false, true, showReversePage, true, otherPageToggleGroup);
				}
			}
		}

		// Token: 0x0600914B RID: 37195 RVA: 0x0043BAC8 File Offset: 0x00439CC8
		public static ushort GetSelectedPage(CToggleGroupObsolete outlineToggleGroup, CToggleGroupObsolete otherPageToggleGroup)
		{
			List<CToggleObsolete> togList = otherPageToggleGroup.GetAllActive();
			ushort selectedPage = 0;
			CToggleObsolete outlineActiveToggle = outlineToggleGroup.GetActive();
			bool flag = outlineActiveToggle;
			if (flag)
			{
				selectedPage = CombatSkillStateHelper.SetPageRead(selectedPage, (byte)outlineActiveToggle.Key);
			}
			foreach (CToggleObsolete toggle in togList)
			{
				selectedPage = CombatSkillStateHelper.SetPageRead(selectedPage, (byte)toggle.Key);
			}
			return selectedPage;
		}

		// Token: 0x0600914C RID: 37196 RVA: 0x0043BB54 File Offset: 0x00439D54
		public static void OpenBreakPlate(bool isReview, short skillTemplateId, string qualificationTypeName, string qualificationTypeIcon, string qualificationRequireValue, ushort selectedPage, Action onDeActive = null, SkillBreakPlateIndex? autoOpenBonusCoordinate = null)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("SkillId", skillTemplateId);
			argBox.Set("SelectedPage", selectedPage);
			argBox.Set("IsReview", isReview);
			argBox.Set("QualificationTypeName", qualificationTypeName);
			argBox.Set("QualificationTypeIcon", qualificationTypeIcon);
			argBox.Set("QualificationRequireValue", qualificationRequireValue);
			bool flag = autoOpenBonusCoordinate != null;
			if (flag)
			{
				argBox.Set<SkillBreakPlateIndex>("AutoOpenBonusCoordinate", autoOpenBonusCoordinate.Value);
			}
			UIElement skillBreakPlate = UIElement.SkillBreakPlate;
			skillBreakPlate.OnDeActive = (Action)Delegate.Combine(skillBreakPlate.OnDeActive, onDeActive);
			UIElement.SkillBreakPlate.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.SkillBreakPlate, true);
		}

		// Token: 0x0600914D RID: 37197 RVA: 0x0043BC10 File Offset: 0x00439E10
		public unsafe static ValueTuple<string, string, string> CalcQualificationInfo(short skillTemplateId, CombatSkillShorts combatSkillQualifications, LifeSkillShorts lifeSkillQualifications)
		{
			CombatSkillItem configData = CombatSkill.Instance[skillTemplateId];
			SkillGradeDataItem skillGradeCfg = SkillGradeData.Instance[configData.Grade];
			int requireQualification = (int)skillGradeCfg.PracticeQualificationRequirement;
			short currQualification = *combatSkillQualifications[(int)configData.Type];
			bool isCombatSkillQualification = true;
			sbyte lifeSkillType;
			short lifeSkillQualification = GameData.Domains.Taiwu.SharedMethods.GetQualificationWithSectApprovalBonus(configData.SectId, currQualification, lifeSkillQualifications, out lifeSkillType);
			LifeSkillTypeItem lifeSkillConfig = null;
			bool flag = lifeSkillQualification > currQualification;
			if (flag)
			{
				currQualification = lifeSkillQualification;
				isCombatSkillQualification = false;
				lifeSkillConfig = Config.LifeSkillType.Instance[lifeSkillType];
			}
			CombatSkillTypeItem combatSkillTypeItem = Config.CombatSkillType.Instance[configData.Type];
			string qualificationTypeName = isCombatSkillQualification ? combatSkillTypeItem.Name : lifeSkillConfig.Name;
			string qualificationTypeIcon = isCombatSkillQualification ? ("sp_18_iconwuxuezhanshi_" + configData.Type.ToString()) : ("mousetip_jiyi_" + lifeSkillConfig.TemplateId.ToString());
			string currQualificationText = currQualification.ToString().SetColor(((int)currQualification > requireQualification) ? "brightblue" : "brightred");
			string qualificationRequireValue = string.Format("{0}/{1}", currQualificationText, requireQualification).ColorReplace();
			return new ValueTuple<string, string, string>(qualificationTypeName, qualificationTypeIcon, qualificationRequireValue);
		}

		// Token: 0x0600914E RID: 37198 RVA: 0x0043BD2C File Offset: 0x00439F2C
		public static void RefreshOtherPageTips(int index, TooltipInvoker tip, short skillId, bool isDirect, bool needNotReadLine = false, bool isRead = false, bool needNotActiveLine = false, CToggleGroupObsolete toggleGroup = null)
		{
			tip.Type = TipType.GeneralLines;
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			PracticeSkillPlatePageUtils.<>c__DisplayClass5_0 CS$<>8__locals1;
			CS$<>8__locals1.tipParam = tip.RuntimeParam;
			CS$<>8__locals1.lineCount = 0;
			string otherPageName = LocalStringManager.Get(string.Format("LK_CombatSkill_{0}_Page_{1}", isDirect ? "Direct" : "Reverse", index)).SetColor(isDirect ? "brightblue" : "brightred");
			string titleName = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Tip_OtherPage_Title, otherPageName);
			CS$<>8__locals1.tipParam.Set("Title", titleName);
			GeneralLineData effectTitle = new GeneralLineData
			{
				Type = 1,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Tip_OtherPage_EffectTitle)
				}
			};
			PracticeSkillPlatePageUtils.<RefreshOtherPageTips>g__AddNode|5_0(effectTitle, ref CS$<>8__locals1);
			int pageIndex = isDirect ? (index + 5) : (index + 5 + 5);
			List<string> descList = EasyPool.Get<List<string>>();
			PageEffectHelper.GenerateNormalPageEffects(skillId, pageIndex - 5, descList);
			foreach (string desc in descList)
			{
				GeneralLineData effectLine = new GeneralLineData
				{
					Type = 5,
					Args = new List<string>
					{
						desc
					},
					ExtraArgs = new List<object>
					{
						24
					}
				};
				PracticeSkillPlatePageUtils.<RefreshOtherPageTips>g__AddNode|5_0(effectLine, ref CS$<>8__locals1);
			}
			bool flag = needNotReadLine || needNotActiveLine;
			if (flag)
			{
				GeneralLineData space = new GeneralLineData
				{
					Type = 4,
					PreferredHeight = 10f
				};
				PracticeSkillPlatePageUtils.<RefreshOtherPageTips>g__AddNode|5_0(space, ref CS$<>8__locals1);
			}
			if (needNotReadLine)
			{
				bool flag2 = !isRead;
				if (flag2)
				{
					GeneralLineData notReadLine = new GeneralLineData
					{
						Type = 3,
						Args = new List<string>
						{
							LocalStringManager.Get(LanguageKey.LK_Skill_Break_Tip_OtherPage_Desc_NotRead)
						}
					};
					PracticeSkillPlatePageUtils.<RefreshOtherPageTips>g__AddNode|5_0(notReadLine, ref CS$<>8__locals1);
				}
			}
			if (needNotActiveLine)
			{
				bool isActivated = toggleGroup.Get(pageIndex).isOn;
				bool flag3 = !isActivated;
				if (flag3)
				{
					GeneralLineData notActiveLine = new GeneralLineData
					{
						Type = 3,
						Args = new List<string>
						{
							LocalStringManager.Get(LanguageKey.LK_Skill_Break_Tip_OtherPage_Desc_NotActive)
						}
					};
					PracticeSkillPlatePageUtils.<RefreshOtherPageTips>g__AddNode|5_0(notActiveLine, ref CS$<>8__locals1);
				}
			}
			CS$<>8__locals1.tipParam.Set("LineCount", CS$<>8__locals1.lineCount);
			tip.Refresh(false, -1);
			EasyPool.Free<List<string>>(descList);
		}

		// Token: 0x0600914F RID: 37199 RVA: 0x0043BFA8 File Offset: 0x0043A1A8
		public static void RefreshOutlinePageTip(TooltipInvoker tip, sbyte index, bool needNotReadLine = false, bool isRead = false)
		{
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			PracticeSkillPlatePageUtils.<>c__DisplayClass6_0 CS$<>8__locals1;
			CS$<>8__locals1.tipParam = tip.RuntimeParam;
			tip.Type = TipType.GeneralLines;
			string outlineName = LocalStringManager.Get(string.Format("LK_CombatSkill_First_Page_Type_{0}", index)).SetColor(PracticeSkillPlatePageUtils.OutlinePageColorMap[(int)index]);
			CS$<>8__locals1.tipParam.Set("Title", LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Tip_Outline_Title, outlineName));
			CS$<>8__locals1.lineCount = 0;
			string outlineDesc = SkillBreakOutlineEffect.Instance[index].Desc.ColorReplace();
			GeneralLineData descLine = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					outlineDesc
				}
			};
			PracticeSkillPlatePageUtils.<RefreshOutlinePageTip>g__AddNode|6_0(descLine, ref CS$<>8__locals1);
			bool flag = needNotReadLine && !isRead;
			if (flag)
			{
				GeneralLineData notReadLine = new GeneralLineData
				{
					Type = 11,
					Args = new List<string>
					{
						LocalStringManager.Get(LanguageKey.LK_Skill_Break_Tip_Outline_Desc_NotRead)
					}
				};
				PracticeSkillPlatePageUtils.<RefreshOutlinePageTip>g__AddNode|6_0(notReadLine, ref CS$<>8__locals1);
			}
			CS$<>8__locals1.tipParam.Set("LineCount", CS$<>8__locals1.lineCount);
			CS$<>8__locals1.tipParam.Set("DisableRaycastTarget", true);
		}

		// Token: 0x06009150 RID: 37200 RVA: 0x0043C0E4 File Offset: 0x0043A2E4
		public static void RefreshOutlinePageTipLegacy(TooltipInvoker tip, short skillTemplateId, sbyte index, bool showPage)
		{
			tip.Type = TipType.Simple;
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CombatSkill_First_Page_Tips_Title));
			if (showPage)
			{
				TutorialChapterModel tutorialChapterModel = SingletonObject.getInstance<TutorialChapterModel>();
				bool flag = tutorialChapterModel.InGuiding && tutorialChapterModel.TutorialChapterIndex == 5;
				if (flag)
				{
					bool flag2 = skillTemplateId == 716;
					if (flag2)
					{
						skillTemplateId = 709;
					}
				}
				SkillBreakGridListItem breakGridConfig = SkillBreakGridList.Instance[skillTemplateId];
				StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
				List<BreakGrid> gridList = (index == 0) ? breakGridConfig.BreakGridListJust : ((index == 1) ? breakGridConfig.BreakGridListKind : ((index == 2) ? breakGridConfig.BreakGridListEven : ((index == 3) ? breakGridConfig.BreakGridListRebel : breakGridConfig.BreakGridListEgoistic)));
				strBuilder.Clear();
				for (int i = 0; i < gridList.Count; i++)
				{
					BreakGrid grid = gridList[i];
					SkillBreakPlateGridBonusTypeItem bonusTypeData = SkillBreakPlateGridBonusType.Instance[grid.BonusType];
					bool flag3 = i > 0;
					if (flag3)
					{
						strBuilder.Append("\n");
					}
					strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Dot_Symbol));
					strBuilder.Append(string.Format("{0}x{1}{2}", bonusTypeData.Name, grid.GridCount, LocalStringManager.Get(LanguageKey.LK_Colon_Symbol)).SetColor("pinkyellow"));
					for (int j = 0; j < bonusTypeData.CharacterPropertyBonusList.Length; j++)
					{
						PropertyAndValue bonus = bonusTypeData.CharacterPropertyBonusList[j];
						short displayTypeId = CharacterPropertyReferenced.Instance[bonus.PropertyId].DisplayType;
						CharacterPropertyDisplayItem propertyData = CharacterPropertyDisplay.Instance[displayTypeId];
						string valueStr = (bonus.Value >= 0) ? string.Format("+{0}", bonus.Value) : bonus.Value.ToString();
						bool isPercent = propertyData.IsPercent;
						if (isPercent)
						{
							valueStr += "%";
						}
						bool flag4 = j > 0;
						if (flag4)
						{
							strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol_2));
						}
						strBuilder.Append((propertyData.Name + valueStr).SetColor("brightblue"));
					}
					for (int k = 0; k < bonusTypeData.CombatSkillPropertyBonusList.Length; k++)
					{
						PropertyAndValue bonus2 = bonusTypeData.CombatSkillPropertyBonusList[k];
						CombatSkillPropertyItem propertyData2 = CombatSkillProperty.Instance[(int)bonus2.PropertyId];
						string valueStr2 = (bonus2.Value >= 0) ? string.Format("+{0}", bonus2.Value) : bonus2.Value.ToString();
						bool isPercent2 = propertyData2.IsPercent;
						if (isPercent2)
						{
							valueStr2 += "%";
						}
						bool flag5 = k > 0;
						if (flag5)
						{
							strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol_2));
						}
						strBuilder.Append((propertyData2.Name + valueStr2).SetColor("brightblue"));
					}
				}
				tip.RuntimeParam.Set("arg1", strBuilder.ToString());
				EasyPool.Free<StringBuilder>(strBuilder);
			}
		}

		// Token: 0x06009152 RID: 37202 RVA: 0x0043C498 File Offset: 0x0043A698
		[CompilerGenerated]
		internal static void <RefreshOtherPageTips>g__AddNode|5_0(GeneralLineData lineData, ref PracticeSkillPlatePageUtils.<>c__DisplayClass5_0 A_1)
		{
			ArgumentBox tipParam = A_1.tipParam;
			string format = "LineData{0}";
			int num = A_1.lineCount + 1;
			A_1.lineCount = num;
			tipParam.SetObject(string.Format(format, num), lineData);
		}

		// Token: 0x06009153 RID: 37203 RVA: 0x0043C4D4 File Offset: 0x0043A6D4
		[CompilerGenerated]
		internal static void <RefreshOutlinePageTip>g__AddNode|6_0(GeneralLineData lineData, ref PracticeSkillPlatePageUtils.<>c__DisplayClass6_0 A_1)
		{
			ArgumentBox tipParam = A_1.tipParam;
			string format = "LineData{0}";
			int num = A_1.lineCount + 1;
			A_1.lineCount = num;
			tipParam.SetObject(string.Format(format, num), lineData);
		}

		// Token: 0x04006FF8 RID: 28664
		public static readonly Dictionary<int, string> OutlinePageColorMap = new Dictionary<int, string>
		{
			{
				0,
				"BehaviorType_Just"
			},
			{
				1,
				"BehaviorType_Kind"
			},
			{
				2,
				"BehaviorType_Even"
			},
			{
				3,
				"BehaviorType_Rebel"
			},
			{
				4,
				"BehaviorType_Egoistic"
			}
		};
	}
}
