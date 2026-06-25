using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using Game.Components.Character;
using Game.Components.Common;
using Game.Views.Bottom;
using Game.Views.MouseTips.Common;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x0200089E RID: 2206
	public class TooltipCost : MouseTipBase
	{
		// Token: 0x17000C89 RID: 3209
		// (get) Token: 0x06006978 RID: 27000 RVA: 0x00307EE2 File Offset: 0x003060E2
		private TipType ActiveCostTipType
		{
			get
			{
				return this.isLoop ? TipType.ActiveLoopCost : TipType.ActiveReadCost;
			}
		}

		// Token: 0x17000C8A RID: 3210
		// (get) Token: 0x06006979 RID: 27001 RVA: 0x00307EF8 File Offset: 0x003060F8
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600697A RID: 27002 RVA: 0x00307EFB File Offset: 0x003060FB
		private new void OnEnable()
		{
			SingletonObject.getInstance<TooltipManager>().OnShowingTips += this.OnManagerShowingTips;
		}

		// Token: 0x0600697B RID: 27003 RVA: 0x00307F14 File Offset: 0x00306114
		private new void OnDisable()
		{
			SingletonObject.getInstance<TooltipManager>().OnShowingTips -= this.OnManagerShowingTips;
		}

		// Token: 0x0600697C RID: 27004 RVA: 0x00307F30 File Offset: 0x00306130
		private void OnManagerShowingTips(TipType type, int _)
		{
			bool flag = type != this.ActiveCostTipType;
			if (!flag)
			{
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
				{
					bool flag2 = this;
					if (flag2)
					{
						this.Refresh();
					}
				});
			}
		}

		// Token: 0x0600697D RID: 27005 RVA: 0x00307F68 File Offset: 0x00306168
		public override bool CanShowWithArgumentBox(ArgumentBox argumentBox)
		{
			Game.Views.Bottom.ReadAndLoop source;
			return argumentBox != null && argumentBox.Get<Game.Views.Bottom.ReadAndLoop>("Source", out source) && source;
		}

		// Token: 0x0600697E RID: 27006 RVA: 0x00307F90 File Offset: 0x00306190
		protected override void Init(ArgumentBox argsBox)
		{
			bool flag = this.detailPanel == null;
			if (flag)
			{
				Transform transform = base.transform.Find("DetailPanel");
				this.detailPanel = ((transform != null) ? transform.gameObject : null);
			}
			bool flag2 = this.detailPanel != null;
			if (flag2)
			{
				this.detailPanel.SetActive(false);
			}
			this._isDetail = false;
			this._operationHintsInitialized = false;
			this._cachedTipsIsLoop = null;
			this.Refresh(argsBox);
		}

		// Token: 0x0600697F RID: 27007 RVA: 0x0030800F File Offset: 0x0030620F
		public override void Refresh()
		{
			this.ApplyView();
		}

		// Token: 0x06006980 RID: 27008 RVA: 0x00308018 File Offset: 0x00306218
		private void Update()
		{
			this.UpdateDetail();
			this.UpdateEncyclopediaHotkeyVisibility();
		}

		// Token: 0x06006981 RID: 27009 RVA: 0x0030802C File Offset: 0x0030622C
		public override void Refresh(ArgumentBox argBox)
		{
			Game.Views.Bottom.ReadAndLoop source;
			bool flag = argBox != null && argBox.Get<Game.Views.Bottom.ReadAndLoop>("Source", out source) && source;
			if (flag)
			{
				this._source = source;
			}
			this.ApplyView();
		}

		// Token: 0x06006982 RID: 27010 RVA: 0x00308068 File Offset: 0x00306268
		private void ApplyView()
		{
			TooltipCost.ViewContext ctx;
			bool flag = !this.TryBuildViewContext(out ctx);
			if (!flag)
			{
				bool flag2 = this.headerLabel != null;
				if (flag2)
				{
					this.headerLabel.SetText(ctx.Title.ColorReplace(), true);
				}
				bool flag3 = this.descLabel1 != null;
				if (flag3)
				{
					this.descLabel1.SetText(ctx.Desc1.ColorReplace(), true);
				}
				bool flag4 = this.descLabel2 != null;
				if (flag4)
				{
					this.descLabel2.SetText(ctx.Desc2.ColorReplace(), true);
				}
				this.RefreshState(ctx);
				this.RefreshCost(ctx);
				this.RefreshWarnings(ctx.Warnings);
				this.RefreshDetailContent(ctx);
				this.RefreshAnnotation(ctx.Title, ctx.Annotation);
				this.ApplyDetailVisibility();
				this.RefreshTipsIfNeeded();
				this.EnsureOperationHints();
			}
		}

		// Token: 0x06006983 RID: 27011 RVA: 0x00308154 File Offset: 0x00306354
		private bool TryBuildViewContext(out TooltipCost.ViewContext ctx)
		{
			ctx = default(TooltipCost.ViewContext);
			bool flag = this._source == null || this._source.Data == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ReadAndLoopDisplayData data = this._source.Data;
				ctx = (this.isLoop ? TooltipCost.BuildLoopContext(data, this._source.LoopingSkillDisplayData, this._source.CachedNeiliType, this._source.ExtraNeiliAllocationProgress) : TooltipCost.BuildReadContext(data, this._source.BookPagesInfo));
				result = true;
			}
			return result;
		}

		// Token: 0x06006984 RID: 27012 RVA: 0x003081EC File Offset: 0x003063EC
		private unsafe static TooltipCost.ViewContext BuildLoopContext(ReadAndLoopDisplayData data, CombatSkillDisplayData loopingSkillDisplayData, sbyte neiliType, int[] extraNeiliAllocationProgress)
		{
			short progress = data.ActiveLoopingProgress;
			int stageIndex = Mathf.Min((int)(progress / 10), GlobalConfig.Instance.ActiveLoopProgressAffectedEfficiency.Length - 1);
			short maxProgress = GlobalConfig.Instance.MaxActiveNeigongLoopingProgress;
			short concentrationCost = GlobalConfig.Instance.ActiveNeigongLoopingAttributeCost;
			short haveConcentration = *data.MainAttributes[2];
			bool concentrationEnough = haveConcentration >= concentrationCost;
			short timeCost = GlobalConfig.Instance.ActiveNeigongLoopingTimeCost;
			int haveTime = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			bool timeEnough = haveTime >= (int)timeCost;
			return new TooltipCost.ViewContext
			{
				Title = LanguageKey.LK_ActiveLoop_Tip_Title.Tr(),
				Desc1 = LanguageKey.LK_ActiveLoop_Tip_Desc1.Tr(),
				Desc2 = LanguageKey.LK_LoopingProgressStatus_Tips_Desc.Tr(),
				StateStageIndex = stageIndex,
				StateProgressFill = TooltipCost.CalcStageProgressFill((int)progress, (int)maxProgress),
				StateStatus = LanguageKey.LK_WorkState.Tr(),
				StateEfficiencyTitle = LanguageKey.LK_Efficiency.Tr(),
				StateEfficiency = TooltipCost.FormatEfficiencyText((int)GlobalConfig.Instance.ActiveLoopProgressAffectedEfficiency[stageIndex], stageIndex),
				StateProgressTitle = LanguageKey.LK_Combat_RawCreate_MouseTips_05.Tr(),
				StateProgressText = TooltipCost.FormatProgressText((int)progress),
				Cost1Icon = Attribute.GetMainAttributeIcon(2),
				Cost1Title = Attribute.GetMainAttributeName(2),
				Cost1Value = TooltipCost.FormatAttributeCost((int)haveConcentration, (int)concentrationCost, concentrationEnough),
				Cost2Icon = "ui9_icon_event_action_point_0",
				Cost2Title = LanguageKey.LK_Active_Tip_TimeCost.Tr().Split('{', StringSplitOptions.None)[0].TrimEnd(new char[]
				{
					'：',
					':'
				}),
				Cost2Value = TooltipCost.FormatTimeCost(haveTime, (int)timeCost, timeEnough),
				Warnings = TooltipCost.BuildWarnings(!concentrationEnough, LanguageKey.LK_ActiveLoop_Tip_NotEnough1.Tr(), !timeEnough, LanguageKey.LK_ActiveLoop_Tip_NotEnough2.Tr(), data.LoopingId == -1, LanguageKey.LK_ActiveLoop_Tip_NotEnough3.Tr(), progress >= maxProgress, LanguageKey.LK_ActiveReadLoop_Disable_Max.Tr()),
				Annotation = TooltipCost.BuildLoopAnnotation(),
				DetailTitle = LanguageKey.LK_MouseTipLoopProgress_Title.Tr(),
				DetailRows = TooltipCost.BuildLoopDetailRows(data, loopingSkillDisplayData, neiliType, extraNeiliAllocationProgress)
			};
		}

		// Token: 0x06006985 RID: 27013 RVA: 0x00308414 File Offset: 0x00306614
		private unsafe static TooltipCost.ViewContext BuildReadContext(ReadAndLoopDisplayData data, SkillBookPageDisplayData bookPagesInfo)
		{
			short progress = data.ActiveReadingProgress;
			int stageIndex = Mathf.Min((int)(progress / 10), GlobalConfig.Instance.ActiveReadProgressAffectedEfficiency.Length - 1);
			short maxProgress = GlobalConfig.Instance.MaxActiveReadingProgress;
			short costIntelligence = GlobalConfig.Instance.ActiveReadingAttributeCost;
			short haveIntelligence = *data.MainAttributes[5];
			bool intelligenceEnough = haveIntelligence >= costIntelligence;
			int haveTime = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			short readTimeCost = GlobalConfig.Instance.ActiveReadingTimeCost;
			bool timeEnough = haveTime >= (int)readTimeCost;
			return new TooltipCost.ViewContext
			{
				Title = LanguageKey.LK_ActiveRead_Tip_Title.Tr(),
				Desc1 = LanguageKey.LK_ActiveRead_Tip_Desc1.Tr(),
				Desc2 = LanguageKey.LK_ReadingProgressStatus_Tips_Desc.Tr(),
				StateStageIndex = stageIndex,
				StateProgressFill = TooltipCost.CalcStageProgressFill((int)progress, (int)maxProgress),
				StateStatus = LanguageKey.LK_WorkState.Tr(),
				StateEfficiencyTitle = LanguageKey.LK_Efficiency.Tr(),
				StateEfficiency = TooltipCost.FormatEfficiencyText((int)GlobalConfig.Instance.ActiveReadProgressAffectedEfficiency[stageIndex], stageIndex),
				StateProgressTitle = LanguageKey.LK_Combat_RawCreate_MouseTips_05.Tr(),
				StateProgressText = TooltipCost.FormatProgressText((int)progress),
				Cost1Icon = Attribute.GetMainAttributeIcon(5),
				Cost1Title = Attribute.GetMainAttributeName(5),
				Cost1Value = TooltipCost.FormatAttributeCost((int)haveIntelligence, (int)costIntelligence, intelligenceEnough),
				Cost2Icon = "ui9_icon_event_action_point_0",
				Cost2Title = LanguageKey.LK_Active_Tip_TimeCost.Tr().Split('{', StringSplitOptions.None)[0].TrimEnd(new char[]
				{
					'：',
					':'
				}),
				Cost2Value = TooltipCost.FormatTimeCost(haveTime, (int)readTimeCost, timeEnough),
				Warnings = TooltipCost.BuildWarnings(!intelligenceEnough, LanguageKey.LK_ActiveRead_Tip_NotEnough1.Tr(), !timeEnough, LanguageKey.LK_ActiveRead_Tip_NotEnough2.Tr(), !data.BookKey.IsValid(), LanguageKey.LK_ActiveRead_Tip_NotEnough3.Tr(), progress >= maxProgress, LanguageKey.LK_ActiveReadLoop_Disable_Max.Tr()),
				Annotation = TooltipCost.BuildReadAnnotation(),
				DetailTitle = LanguageKey.LK_MouseTipReadProgress_Title.Tr(),
				DetailRows = TooltipCost.BuildReadDetailRows(data, bookPagesInfo)
			};
		}

		// Token: 0x06006986 RID: 27014 RVA: 0x00308640 File Offset: 0x00306840
		private static List<TooltipCost.DetailRow> BuildLoopDetailRows(ReadAndLoopDisplayData data, CombatSkillDisplayData loopingSkillDisplayData, sbyte neiliType, int[] extraNeiliAllocationProgress)
		{
			List<TooltipCost.DetailRow> rows = new List<TooltipCost.DetailRow>(6);
			bool flag = data.LoopingId < 0;
			List<TooltipCost.DetailRow> result;
			if (flag)
			{
				result = rows;
			}
			else
			{
				CombatSkillItem skillCfg = CombatSkill.Instance[data.LoopingId];
				string skillIcon = ((skillCfg != null) ? skillCfg.Icon : null) ?? string.Empty;
				bool flag2 = loopingSkillDisplayData != null;
				string neiliValue;
				if (flag2)
				{
					short obtained = loopingSkillDisplayData.ObtainedNeili;
					short maxNeili = loopingSkillDisplayData.MaxObtainableNeili;
					string color = (obtained >= maxNeili) ? "brightblue" : "brightred";
					neiliValue = string.Format("<color=#{0}>{1}</color>/<color=#{2}>{3}</color>", new object[]
					{
						color,
						obtained,
						"pinkyellow",
						maxNeili
					});
				}
				else
				{
					neiliValue = "-";
				}
				rows.Add(TooltipCost.DetailRow.FromFormatKey(LanguageKey.LK_MouseTipLoopProgress_Neili, neiliValue, skillIcon));
				bool flag3 = neiliType >= 0 && loopingSkillDisplayData != null;
				if (flag3)
				{
					NeiliTypeItem neiliTypeConfig = NeiliType.Instance[neiliType];
					string color2 = (neiliTypeConfig.ColorType != 2 && (loopingSkillDisplayData.FiveElementDestTypeWhileLooping == -1 || loopingSkillDisplayData.FiveElementDestTypeWhileLooping == (sbyte)neiliTypeConfig.FiveElements)) ? "brightblue" : "brightred";
					string fiveElementValue = string.Concat(new string[]
					{
						"<color=#",
						color2,
						">",
						neiliTypeConfig.Name,
						"</color>"
					});
					string fiveElementIcon = string.Format("{0}{1}", "ui9_icon_five_elements_", neiliTypeConfig.FiveElements);
					rows.Add(TooltipCost.DetailRow.FromFormatKey(LanguageKey.LK_MouseTipLoopProgress_FiveElements, fiveElementValue, fiveElementIcon));
				}
				else
				{
					rows.Add(TooltipCost.DetailRow.FromFormatKey(LanguageKey.LK_MouseTipLoopProgress_FiveElements, "-", string.Empty));
				}
				int maxAllocationProgress = LoopingCommonUtils.GetNeiliAllocationMaxProgress();
				int i = 0;
				while (i < 4 && rows.Count < 6)
				{
					string allocationKey = string.Format("LK_MouseTipLoopProgress_NeiliAllocation_{0}", i);
					string allocationIcon = string.Format("{0}{1}", "ui9_icon_mousetip_kungfu_", i + 1);
					bool flag4 = extraNeiliAllocationProgress != null;
					if (flag4)
					{
						int allocationProgress = extraNeiliAllocationProgress[i];
						string color3 = (allocationProgress >= maxAllocationProgress) ? "brightblue" : "brightred";
						int extraNeiliAllocation = LoopingCommonUtils.CalcExtraNeiliAllocationFromProgress(allocationProgress);
						string allocationValue = string.Format("<color=#{0}>{1}</color>/<color=#{2}>{3}</color>", new object[]
						{
							color3,
							extraNeiliAllocation,
							"pinkyellow",
							GlobalConfig.Instance.MaxExtraNeiliAllocation
						});
						rows.Add(TooltipCost.DetailRow.FromFormatTemplate(LocalStringManager.Get(allocationKey), allocationValue, allocationIcon));
					}
					else
					{
						rows.Add(TooltipCost.DetailRow.FromFormatTemplate(LocalStringManager.Get(allocationKey), "-", allocationIcon));
					}
					i++;
				}
				result = rows;
			}
			return result;
		}

		// Token: 0x06006987 RID: 27015 RVA: 0x003088F8 File Offset: 0x00306AF8
		private static List<TooltipCost.DetailRow> BuildReadDetailRows(ReadAndLoopDisplayData data, SkillBookPageDisplayData bookPagesInfo)
		{
			List<TooltipCost.DetailRow> rows = new List<TooltipCost.DetailRow>(6);
			bool flag = !data.BookKey.IsValid() || bookPagesInfo == null;
			List<TooltipCost.DetailRow> result;
			if (flag)
			{
				result = rows;
			}
			else
			{
				bool isCombatBook = bookPagesInfo.IsCombatBook;
				int pageCount = isCombatBook ? 6 : 5;
				SkillBookItem skillBook = SkillBook.Instance.GetItem(data.BookKey.TemplateId);
				int i = 0;
				while (i < pageCount && rows.Count < 6)
				{
					bool flag2 = i >= bookPagesInfo.ReadingProgress.Length;
					if (flag2)
					{
						break;
					}
					bool flag3 = isCombatBook;
					string pageTitle;
					string icon;
					if (flag3)
					{
						pageTitle = ((i == 0) ? LanguageKey.LK_CombatSkill_Book_First_Page.Tr() : LocalStringManager.Get(string.Format("LK_Book_Page_Index_{0}", i - 1)));
						icon = ((i == 0) ? string.Format("mousetip_directory_{0}", bookPagesInfo.Type[i]) : ((bookPagesInfo.Type[i] == 0) ? "mousetip_zhengni_0" : "mousetip_zhengni_1"));
					}
					else
					{
						pageTitle = LocalStringManager.Get(string.Format("LK_Book_Page_Index_{0}", i));
						icon = string.Empty;
					}
					bool isFinished = bookPagesInfo.ReadingProgress[i] == 100;
					string color = isFinished ? "brightblue" : "brightred";
					string value = string.Format("<color=#{0}>{1}%</color>", color, bookPagesInfo.ReadingProgress[i]);
					bool flag4 = isFinished && skillBook != null;
					if (flag4)
					{
						short expPerPage = SkillGradeData.Instance[skillBook.Grade].ReadingExpGainPerPage;
						value += string.Format(" (     +{0})", expPerPage);
					}
					rows.Add(new TooltipCost.DetailRow(icon, pageTitle, value));
					i++;
				}
				result = rows;
			}
			return result;
		}

		// Token: 0x06006988 RID: 27016 RVA: 0x00308AC0 File Offset: 0x00306CC0
		private void RefreshTipsIfNeeded()
		{
			bool flag2;
			if (!(this.tipsHotkeyDisplay == null))
			{
				bool? cachedTipsIsLoop = this._cachedTipsIsLoop;
				bool flag = this.isLoop;
				flag2 = (cachedTipsIsLoop.GetValueOrDefault() == flag & cachedTipsIsLoop != null);
			}
			else
			{
				flag2 = true;
			}
			bool flag3 = flag2;
			if (!flag3)
			{
				this._cachedTipsIsLoop = new bool?(this.isLoop);
				Transform tipsArea = this.tipsHotkeyDisplay.transform.parent;
				this.tipsHotkeyDisplay.gameObject.SetActive(true);
				bool flag4 = tipsArea != null;
				if (flag4)
				{
					tipsArea.gameObject.SetActive(true);
				}
				this.tipsHotkeyDisplay.Refresh(this.isLoop ? EHotKeyDisplayType.AttentivelyTips : EHotKeyDisplayType.DedicatedTips);
			}
		}

		// Token: 0x06006989 RID: 27017 RVA: 0x00308B6C File Offset: 0x00306D6C
		private void EnsureOperationHints()
		{
			bool flag = this.operationArea == null || this._operationHintsInitialized;
			if (!flag)
			{
				this.operationArea.SetLockItemActive(false);
				this._encyclopediaUiOpen = UIManager.Instance.IsFocusElement(UIElement.Encyclopedia);
				this.operationArea.RefreshHotkeyDisplayViewEncyclopedia(false);
				this.operationArea.ShowHotkeyDisplayDetail(true);
				this.RefreshDetailHotkey();
				this._operationHintsInitialized = true;
			}
		}

		// Token: 0x0600698A RID: 27018 RVA: 0x00308BE4 File Offset: 0x00306DE4
		private void UpdateEncyclopediaHotkeyVisibility()
		{
			bool flag = this.operationArea == null || !this._operationHintsInitialized;
			if (!flag)
			{
				bool isOpen = UIManager.Instance.IsFocusElement(UIElement.Encyclopedia);
				bool flag2 = isOpen == this._encyclopediaUiOpen;
				if (!flag2)
				{
					this._encyclopediaUiOpen = isOpen;
					this.operationArea.RefreshHotkeyDisplayViewEncyclopedia(true);
				}
			}
		}

		// Token: 0x0600698B RID: 27019 RVA: 0x00308C48 File Offset: 0x00306E48
		private void UpdateDetail()
		{
			bool hasStick = this.HasStick;
			if (!hasStick)
			{
				bool altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
				bool flag = altDown == this._isDetail;
				if (!flag)
				{
					this._isDetail = altDown;
					this.ApplyDetailVisibility();
					this.RefreshDetailHotkey();
				}
			}
		}

		// Token: 0x0600698C RID: 27020 RVA: 0x00308CA0 File Offset: 0x00306EA0
		private void ApplyDetailVisibility()
		{
			bool flag = this.detailPanel != null && this.detailPanel.activeSelf != this._isDetail;
			if (flag)
			{
				this.detailPanel.SetActive(this._isDetail);
			}
		}

		// Token: 0x0600698D RID: 27021 RVA: 0x00308CEC File Offset: 0x00306EEC
		private void RefreshDetailHotkey()
		{
			bool flag = this.operationArea == null;
			if (!flag)
			{
				bool isDetail = this._isDetail;
				if (isDetail)
				{
					this.operationArea.RefreshCancelDetail();
				}
				else
				{
					this.operationArea.RefreshPressToDetail();
				}
			}
		}

		// Token: 0x0600698E RID: 27022 RVA: 0x00308D30 File Offset: 0x00306F30
		private void RefreshState(TooltipCost.ViewContext ctx)
		{
			bool flag = this.stateTypeName != null;
			if (flag)
			{
				this.stateTypeName.SetText(ctx.StateStatus, true);
			}
			bool flag2 = this.stateTypeIcon != null;
			if (flag2)
			{
				bool flag3 = ctx.StateStageIndex >= 0;
				if (flag3)
				{
					this.stateTypeIcon.SetSprite(string.Format("{0}{1}", "ui9_btn_bottom_read_and_loop_state_", ctx.StateStageIndex), false, null);
					this.stateTypeIcon.SetEnabled(true);
				}
				else
				{
					this.stateTypeIcon.SetEnabled(false);
				}
			}
			bool flag4 = this.stateTypeProgressFill != null;
			if (flag4)
			{
				this.stateTypeProgressFill.fillAmount = ctx.StateProgressFill;
			}
			TooltipItemProperty tooltipItemProperty = this.efficiencyItem;
			if (tooltipItemProperty != null)
			{
				tooltipItemProperty.Set(null, ctx.StateEfficiencyTitle, ctx.StateEfficiency, true);
			}
			TooltipItemProperty tooltipItemProperty2 = this.progressItem;
			if (tooltipItemProperty2 != null)
			{
				tooltipItemProperty2.Set(null, ctx.StateProgressTitle, ctx.StateProgressText, true);
			}
		}

		// Token: 0x0600698F RID: 27023 RVA: 0x00308E2C File Offset: 0x0030702C
		private void RefreshCost(TooltipCost.ViewContext ctx)
		{
			TooltipItemProperty tooltipItemProperty = this.costItem1;
			if (tooltipItemProperty != null)
			{
				tooltipItemProperty.Set(ctx.Cost1Icon, ctx.Cost1Title, ctx.Cost1Value, false);
			}
			TooltipItemProperty tooltipItemProperty2 = this.costItem2;
			if (tooltipItemProperty2 != null)
			{
				tooltipItemProperty2.Set(ctx.Cost2Icon, ctx.Cost2Title, ctx.Cost2Value, false);
			}
		}

		// Token: 0x06006990 RID: 27024 RVA: 0x00308E84 File Offset: 0x00307084
		private void RefreshWarnings(string warnings)
		{
			bool flag = this.warningItem == null;
			if (!flag)
			{
				bool hasWarnings = !string.IsNullOrEmpty(warnings);
				this.warningItem.gameObject.SetActive(hasWarnings);
				Transform warningArea = this.warningItem.transform.parent;
				bool flag2 = warningArea != null;
				if (flag2)
				{
					warningArea.gameObject.SetActive(hasWarnings);
				}
				bool flag3 = hasWarnings;
				if (flag3)
				{
					this.warningItem.SetValue(warnings);
				}
			}
		}

		// Token: 0x06006991 RID: 27025 RVA: 0x00308F00 File Offset: 0x00307100
		private void RefreshDetailContent(TooltipCost.ViewContext ctx)
		{
			List<TooltipCost.DetailRow> detailRows = ctx.DetailRows;
			int count = (detailRows != null) ? detailRows.Count : 0;
			bool flag = this.detaiMainAreaGo != null;
			if (flag)
			{
				this.detaiMainAreaGo.SetActive(count != 0);
			}
			bool flag2 = this.progressTitle != null;
			if (flag2)
			{
				this.progressTitle.SetText(ctx.DetailTitle.ColorReplace(), true);
			}
			bool flag3 = this.progressList == null || this.progressList.Length == 0;
			if (!flag3)
			{
				for (int i = 0; i < this.progressList.Length; i++)
				{
					TooltipItemProperty item = this.progressList[i];
					bool flag4 = item == null;
					if (!flag4)
					{
						bool flag5 = ctx.DetailRows != null && i < count;
						if (flag5)
						{
							TooltipCost.DetailRow row = ctx.DetailRows[i];
							item.gameObject.SetActive(true);
							item.Set(row.Icon, row.Title, row.Value, false);
						}
						else
						{
							item.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x06006992 RID: 27026 RVA: 0x0030902C File Offset: 0x0030722C
		private void RefreshAnnotation(string headerTitle, string annotation)
		{
			bool flag = this.annotationItem == null;
			if (!flag)
			{
				bool hasAnnotation = !string.IsNullOrEmpty(annotation);
				this.annotationItem.gameObject.SetActive(hasAnnotation);
				bool flag2 = !hasAnnotation;
				if (!flag2)
				{
					this.annotationItem.Set(null, headerTitle, annotation, false);
				}
			}
		}

		// Token: 0x06006993 RID: 27027 RVA: 0x00309082 File Offset: 0x00307282
		private static float CalcStageProgressFill(int progress, int maxProgress)
		{
			return (progress < maxProgress) ? ((float)(progress % 10) * 0.1f) : 1f;
		}

		// Token: 0x06006994 RID: 27028 RVA: 0x0030909C File Offset: 0x0030729C
		private static string FormatAttributeCost(int haveValue, int costValue, bool enough)
		{
			string haveColor = enough ? "brightblue" : "brightred";
			return string.Format("<color=#{0}>{1}</color>/{2}", haveColor, haveValue, costValue);
		}

		// Token: 0x06006995 RID: 27029 RVA: 0x003090D5 File Offset: 0x003072D5
		private static string FormatTimeCost(int haveTime, int timeCost, bool enough)
		{
			return LanguageKey.LK_Make_Resource_Require_Meet.TrFormat(string.Format("<color=#{0}>{1}</color>", enough ? "brightblue" : "brightred", haveTime), timeCost);
		}

		// Token: 0x06006996 RID: 27030 RVA: 0x00309108 File Offset: 0x00307308
		private static string BuildWarnings(bool show1, string text1, bool show2, string text2, bool show3, string text3, bool showMax, string textMax)
		{
			StringBuilder sb = new StringBuilder();
			TooltipCost.AppendWarningLine(sb, show1, text1);
			TooltipCost.AppendWarningLine(sb, show2, text2);
			TooltipCost.AppendWarningLine(sb, show3, text3);
			TooltipCost.AppendWarningLine(sb, showMax, textMax);
			return sb.ToString();
		}

		// Token: 0x06006997 RID: 27031 RVA: 0x00309150 File Offset: 0x00307350
		private static void AppendWarningLine(StringBuilder sb, bool show, string text)
		{
			bool flag = !show || string.IsNullOrEmpty(text);
			if (!flag)
			{
				bool flag2 = sb.Length > 0;
				if (flag2)
				{
					sb.Append('\n');
				}
				string formatted = text;
				bool flag3 = formatted.IndexOf("brightred", StringComparison.OrdinalIgnoreCase) < 0;
				if (flag3)
				{
					formatted = "<color=#brightred>" + formatted + "</color>";
				}
				sb.Append(formatted.ColorReplace());
			}
		}

		// Token: 0x06006998 RID: 27032 RVA: 0x003091BC File Offset: 0x003073BC
		private static string FormatEfficiencyText(int efficiency, int stageIndex)
		{
			if (!true)
			{
			}
			string text;
			switch (stageIndex)
			{
			case 0:
				text = "brightblue";
				break;
			case 1:
				text = "pinkyellow";
				break;
			case 2:
				text = "brightred";
				break;
			default:
				text = "grey";
				break;
			}
			if (!true)
			{
			}
			string color = text;
			return string.Format("<color=#{0}>{1}%</color>", color, efficiency);
		}

		// Token: 0x06006999 RID: 27033 RVA: 0x00309220 File Offset: 0x00307420
		private static string FormatProgressText(int progress)
		{
			int withinStage = progress % 10;
			return string.Format("{0}/10", withinStage);
		}

		// Token: 0x0600699A RID: 27034 RVA: 0x00309248 File Offset: 0x00307448
		private static string BuildLoopAnnotation()
		{
			string dot = LanguageKey.LK_Dot_Symbol.Tr();
			return string.Concat(new string[]
			{
				dot,
				LanguageKey.LK_ActiveLoop_Tip_Desc2.Tr(),
				"\n",
				dot,
				LanguageKey.LK_ActiveLoop_Tip_Desc4.Tr()
			});
		}

		// Token: 0x0600699B RID: 27035 RVA: 0x0030929C File Offset: 0x0030749C
		private static string BuildReadAnnotation()
		{
			string dot = LanguageKey.LK_Dot_Symbol.Tr();
			return string.Concat(new string[]
			{
				dot,
				LanguageKey.LK_ActiveRead_Tip_Desc2.Tr(),
				"\n",
				dot,
				LanguageKey.LK_ActiveRead_Tip_Desc3.Tr(),
				"\n",
				dot,
				LanguageKey.LK_ActiveRead_Tip_Desc4.Tr(),
				"\n",
				dot,
				LanguageKey.LK_ActiveRead_Tip_Desc6.Tr()
			});
		}

		// Token: 0x0600699C RID: 27036 RVA: 0x00309324 File Offset: 0x00307524
		private static string GetLocalizedFormatTitle(string formatTemplate)
		{
			int placeholderIndex = formatTemplate.IndexOf('{');
			bool flag = placeholderIndex <= 0;
			string result;
			if (flag)
			{
				result = formatTemplate;
			}
			else
			{
				result = formatTemplate.Substring(0, placeholderIndex).TrimEnd(new char[]
				{
					'：',
					':',
					' '
				});
			}
			return result;
		}

		// Token: 0x04004BC4 RID: 19396
		[Header("模式")]
		[SerializeField]
		private bool isLoop;

		// Token: 0x04004BC5 RID: 19397
		[Header("标题内容")]
		[SerializeField]
		private TextMeshProUGUI headerLabel;

		// Token: 0x04004BC6 RID: 19398
		[SerializeField]
		private TextMeshProUGUI descLabel1;

		// Token: 0x04004BC7 RID: 19399
		[SerializeField]
		private TextMeshProUGUI descLabel2;

		// Token: 0x04004BC8 RID: 19400
		[Header("状态")]
		[SerializeField]
		private TextMeshProUGUI stateTypeName;

		// Token: 0x04004BC9 RID: 19401
		[SerializeField]
		private CImage stateTypeIcon;

		// Token: 0x04004BCA RID: 19402
		[SerializeField]
		private CImage stateTypeProgressFill;

		// Token: 0x04004BCB RID: 19403
		[SerializeField]
		private TooltipItemProperty efficiencyItem;

		// Token: 0x04004BCC RID: 19404
		[SerializeField]
		private TooltipItemProperty progressItem;

		// Token: 0x04004BCD RID: 19405
		[Header("消耗")]
		[SerializeField]
		private TooltipItemProperty costItem1;

		// Token: 0x04004BCE RID: 19406
		[SerializeField]
		private TooltipItemProperty costItem2;

		// Token: 0x04004BCF RID: 19407
		[Header("警告")]
		[SerializeField]
		private TooltipItemProperty warningItem;

		// Token: 0x04004BD0 RID: 19408
		[Header("提示")]
		[SerializeField]
		private Game.Components.Common.HotkeyDisplay tipsHotkeyDisplay;

		// Token: 0x04004BD1 RID: 19409
		[Header("操作提示")]
		[SerializeField]
		private TooltipOperationArea operationArea;

		// Token: 0x04004BD2 RID: 19410
		[Header("详细信息")]
		[SerializeField]
		private GameObject detaiMainAreaGo;

		// Token: 0x04004BD3 RID: 19411
		[SerializeField]
		private GameObject detailPanel;

		// Token: 0x04004BD4 RID: 19412
		[SerializeField]
		private TextMeshProUGUI progressTitle;

		// Token: 0x04004BD5 RID: 19413
		[SerializeField]
		private TooltipItemProperty[] progressList;

		// Token: 0x04004BD6 RID: 19414
		[Header("注解")]
		[SerializeField]
		private TooltipItemProperty annotationItem;

		// Token: 0x04004BD7 RID: 19415
		private bool _isDetail;

		// Token: 0x04004BD8 RID: 19416
		private Game.Views.Bottom.ReadAndLoop _source;

		// Token: 0x04004BD9 RID: 19417
		private bool _operationHintsInitialized;

		// Token: 0x04004BDA RID: 19418
		private bool _encyclopediaUiOpen;

		// Token: 0x04004BDB RID: 19419
		private bool? _cachedTipsIsLoop;

		// Token: 0x02001D98 RID: 7576
		public static class Args
		{
			// Token: 0x0400C6A7 RID: 50855
			public const string Source = "Source";
		}

		// Token: 0x02001D99 RID: 7577
		private struct ViewContext
		{
			// Token: 0x0400C6A8 RID: 50856
			public string Title;

			// Token: 0x0400C6A9 RID: 50857
			public string Desc1;

			// Token: 0x0400C6AA RID: 50858
			public string Desc2;

			// Token: 0x0400C6AB RID: 50859
			public int StateStageIndex;

			// Token: 0x0400C6AC RID: 50860
			public float StateProgressFill;

			// Token: 0x0400C6AD RID: 50861
			public string StateStatus;

			// Token: 0x0400C6AE RID: 50862
			public string StateEfficiencyTitle;

			// Token: 0x0400C6AF RID: 50863
			public string StateEfficiency;

			// Token: 0x0400C6B0 RID: 50864
			public string StateProgressTitle;

			// Token: 0x0400C6B1 RID: 50865
			public string StateProgressText;

			// Token: 0x0400C6B2 RID: 50866
			public string Cost1Icon;

			// Token: 0x0400C6B3 RID: 50867
			public string Cost1Title;

			// Token: 0x0400C6B4 RID: 50868
			public string Cost1Value;

			// Token: 0x0400C6B5 RID: 50869
			public string Cost2Icon;

			// Token: 0x0400C6B6 RID: 50870
			public string Cost2Title;

			// Token: 0x0400C6B7 RID: 50871
			public string Cost2Value;

			// Token: 0x0400C6B8 RID: 50872
			public string Warnings;

			// Token: 0x0400C6B9 RID: 50873
			public string Annotation;

			// Token: 0x0400C6BA RID: 50874
			public string DetailTitle;

			// Token: 0x0400C6BB RID: 50875
			public List<TooltipCost.DetailRow> DetailRows;
		}

		// Token: 0x02001D9A RID: 7578
		private readonly struct DetailRow
		{
			// Token: 0x0600EDAE RID: 60846 RVA: 0x00609B14 File Offset: 0x00607D14
			public DetailRow(string icon, string title, string value)
			{
				this.Icon = icon;
				this.Title = title;
				this.Value = value;
			}

			// Token: 0x0600EDAF RID: 60847 RVA: 0x00609B2C File Offset: 0x00607D2C
			public static TooltipCost.DetailRow FromFormatKey(LanguageKey formatKey, string valueContent, string icon = "")
			{
				return TooltipCost.DetailRow.FromFormatTemplate(LocalStringManager.Get(formatKey), valueContent, icon);
			}

			// Token: 0x0600EDB0 RID: 60848 RVA: 0x00609B3B File Offset: 0x00607D3B
			public static TooltipCost.DetailRow FromFormatTemplate(string formatTemplate, string valueContent, string icon = "")
			{
				return new TooltipCost.DetailRow(icon, TooltipCost.GetLocalizedFormatTitle(formatTemplate), valueContent);
			}

			// Token: 0x0400C6BC RID: 50876
			public readonly string Icon;

			// Token: 0x0400C6BD RID: 50877
			public readonly string Title;

			// Token: 0x0400C6BE RID: 50878
			public readonly string Value;
		}
	}
}
