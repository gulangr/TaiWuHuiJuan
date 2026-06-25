using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Main.Reading
{
	// Token: 0x02000971 RID: 2417
	public class ReadingSkillBookPagesDisplay : MonoBehaviour
	{
		// Token: 0x06007391 RID: 29585 RVA: 0x0035A853 File Offset: 0x00358A53
		private void OnEnable()
		{
			this._animFoldFlag = false;
			this.Clear();
		}

		// Token: 0x06007392 RID: 29586 RVA: 0x0035A864 File Offset: 0x00358A64
		public void UpdatePages(SkillBookPageDisplayData readingPageDisplayData, ReadingBookStrategies strategies, IntList strategyExpireTime, int[] extraProgresses, bool isBookRead, bool onInit = false, bool clearPages = true, List<SkillBookPageDisplayData> refDataList = null, Action<int> onClickPage = null, bool tutorialMode = false)
		{
			RectTransform pageSupplyRoot = this.pageSupply;
			RectTransform pageSupplyTemplate = this.pageSupplyState;
			List<ValueTuple<int, sbyte>> suppliedIndexList = new List<ValueTuple<int, sbyte>>();
			byte i = 0;
			while ((int)i < this.pageCount)
			{
				sbyte state = readingPageDisplayData.State[(int)i];
				bool flag = refDataList != null;
				if (flag)
				{
					foreach (SkillBookPageDisplayData refPageDisplayData in refDataList)
					{
						bool flag2 = !refPageDisplayData.State.CheckIndex((int)i) || readingPageDisplayData.ItemKey.TemplateId != refPageDisplayData.ItemKey.TemplateId;
						if (!flag2)
						{
							sbyte refState = refPageDisplayData.State[(int)i];
							bool hasSupply = refState < state;
							bool flag3 = readingPageDisplayData.IsCombatBook && refPageDisplayData.Type[(int)i] != readingPageDisplayData.Type[(int)i];
							if (flag3)
							{
								hasSupply = false;
							}
							bool flag4 = hasSupply;
							if (flag4)
							{
								state = refState;
							}
							this._curSuppliedStateList.Add(state);
						}
					}
					bool flag5 = state < readingPageDisplayData.State[(int)i];
					if (flag5)
					{
						suppliedIndexList.Add(new ValueTuple<int, sbyte>((int)i, state));
					}
				}
				this.SetPage(i, state, readingPageDisplayData, strategies, strategyExpireTime, (extraProgresses == null) ? 0 : extraProgresses[(int)i], isBookRead, !clearPages, onClickPage, tutorialMode);
				i += 1;
			}
			for (int j = 0; j < this.pageCount; j++)
			{
				ReadingPagePrefab page = this.pageList[j];
				for (int k = 0; k < page.combatSkillBookNodes.Count; k++)
				{
					page.combatSkillBookNodes[k].SetActive(readingPageDisplayData.IsCombatBook);
				}
			}
			this._lastSuppliedStateList.Clear();
			this._lastSuppliedStateList.AddRange(this._curSuppliedStateList);
			this._curSuppliedStateList.Clear();
			for (int l = 0; l < pageSupplyRoot.childCount; l++)
			{
				pageSupplyRoot.GetChild(l).gameObject.SetActive(false);
			}
			int totalMergedCount = 0;
			int curMergedCount = 0;
			Transform pageSupplyTrans = null;
			for (int m = 0; m < suppliedIndexList.Count; m++)
			{
				int pageIndex = suppliedIndexList[m].Item1;
				sbyte pageState = suppliedIndexList[m].Item2;
				bool flag6 = m > 0;
				if (flag6)
				{
					int lastPageIndex = suppliedIndexList[m - 1].Item1;
					sbyte lastPageState = suppliedIndexList[m - 1].Item2;
					bool flag7 = pageIndex - lastPageIndex == 1 && pageState == lastPageState;
					if (flag7)
					{
						totalMergedCount++;
						curMergedCount++;
					}
					else
					{
						pageSupplyTrans = null;
					}
				}
				bool flag8 = !pageSupplyTrans;
				if (flag8)
				{
					curMergedCount = 0;
					int childIndex = m - totalMergedCount;
					pageSupplyTrans = ((childIndex < pageSupplyRoot.childCount) ? pageSupplyRoot.GetChild(childIndex) : Object.Instantiate<RectTransform>(pageSupplyTemplate, pageSupplyRoot));
				}
				pageSupplyTrans.gameObject.SetActive(true);
				RectTransform pageSupplyRectTrans = pageSupplyTrans.GetComponent<RectTransform>();
				ReadingPageSupplyState state2 = pageSupplyRectTrans.GetComponent<ReadingPageSupplyState>();
				Vector3 pos = Vector3.zero;
				int realCount = curMergedCount + 1;
				for (int n = 0; n < realCount; n++)
				{
					ReadingPagePrefab page2 = this.pageList[pageIndex - n];
					pos += page2.transform.localPosition;
				}
				pos /= (float)realCount;
				pageSupplyRectTrans.localPosition = pageSupplyRectTrans.localPosition.SetY(pos.y).SetX(0f);
				pageSupplyRectTrans.sizeDelta = pageSupplyRectTrans.sizeDelta.SetY(pageSupplyTemplate.sizeDelta.y * (float)(curMergedCount + 1));
				state2.SetSupplySprite(pageState);
			}
		}

		// Token: 0x06007393 RID: 29587 RVA: 0x0035AC4C File Offset: 0x00358E4C
		public void SetPage(byte pageIndex, sbyte pageState, SkillBookPageDisplayData displayData, ReadingBookStrategies strategies, IntList strategyExpireTime, int extraProgress, bool isBookRead, bool playAnimation, Action<int> onClickPage = null, bool tutorialMode = false)
		{
			ReadingPagePrefab page = this.pageList[(int)pageIndex];
			int progress = Mathf.Clamp((int)displayData.ReadingProgress[(int)pageIndex], 0, 100);
			bool isSkipped = strategies.GetSkipPage(pageIndex);
			bool flag = this._pageProgressValues != null && (int)pageIndex < this._pageProgressValues.Length;
			if (flag)
			{
				this._pageProgressValues[(int)pageIndex] = progress;
			}
			bool flag2 = this._pagePreviewProgressValues != null && (int)pageIndex < this._pagePreviewProgressValues.Length;
			if (flag2)
			{
				this._pagePreviewProgressValues[(int)pageIndex] = (isBookRead ? 0 : Mathf.Clamp(progress + Math.Max(0, Math.Min(100 - progress, extraProgress)), 0, 100));
			}
			bool flag3 = this._pageRereadPreviewValues != null && (int)pageIndex < this._pageRereadPreviewValues.Length;
			if (flag3)
			{
				this._pageRereadPreviewValues[(int)pageIndex] = (isBookRead ? Mathf.Clamp(extraProgress, 0, 100) : 0);
			}
			bool flag4 = this._baseSkipPages != null && (int)pageIndex < this._baseSkipPages.Length;
			if (flag4)
			{
				this._baseSkipPages[(int)pageIndex] = isSkipped;
			}
			ReadingSkillBookPagesDisplay.SetPageStateDisplay(page, pageState);
			this.ApplyPageProgressDisplay(pageIndex, isSkipped, playAnimation, null, null);
			for (int i = 0; i < page.numPageList.Count; i++)
			{
				ReadingDisplayHelper.SetNumPageText(displayData.IsCombatBook, pageIndex, page.numPageList[i]);
			}
			for (int j = 0; j < page.numPageTypeList.Count; j++)
			{
				bool isDirect = displayData.Type[(int)pageIndex] == 0;
				ReadingDisplayHelper.SetNumPageTypeText(pageIndex, displayData.Type[(int)pageIndex], isDirect, page.numPageTypeList[j]);
			}
			CButton pageButton = page.GetComponent<CButton>();
			pageButton.ClearAndAddListener(delegate
			{
				bool flag8 = onClickPage != null;
				if (flag8)
				{
					onClickPage((int)pageIndex);
				}
			});
			page.GetComponent<PointerTrigger>().enabled = (onClickPage != null);
			int currentDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
			for (int k = 0; k < 3; k++)
			{
				ReadingStrategySlot strategySlot = page.strategySlotList[k];
				ReadingBriefStrategy strategyBrief = page.strategyBriefList[k];
				DisableStyleRoot grey = strategySlot.GetComponent<DisableStyleRoot>();
				sbyte strategy = strategies.GetPageStrategy(pageIndex, k);
				strategySlot.SetStrategyEnable(strategy >= 0);
				TooltipInvoker mouseTipDisplayer = strategySlot.GetComponent<TooltipInvoker>();
				grey.SetStyleEffect(tutorialMode && k < 2, false);
				strategySlot.leftTimeHolder.SetActive(strategy >= 0);
				strategyBrief.SetStrategy(strategy >= 0);
				strategyBrief.SetMonth(strategy >= 0);
				bool flag5 = strategy >= 0;
				if (flag5)
				{
					ReadingStrategyItem strategyCfg = ReadingStrategy.Instance[strategy];
					strategySlot.strategyName.SetText(strategyCfg.Name, true);
					strategyBrief.strategy.SetText(strategyCfg.Name[0].ToString(), true);
					strategyBrief.SetTip(strategyCfg.Name, strategyCfg.Desc);
					mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("arg0", strategyCfg.Name).Set("arg1", strategyCfg.Desc);
					mouseTipDisplayer.enabled = true;
					int expireTime = -1;
					bool flag6 = strategyExpireTime.Items != null && strategyExpireTime.Items.Count > 0;
					if (flag6)
					{
						expireTime = strategyExpireTime.Items[(int)(pageIndex * 3) + k];
					}
					strategySlot.leftTime.text = (expireTime - currentDate).ToString();
					strategyBrief.monthText.text = (expireTime - currentDate).ToString();
				}
				else
				{
					strategyBrief.ClearTip();
					bool flag7 = mouseTipDisplayer.RuntimeParam != null;
					if (flag7)
					{
						EasyPool.Free<ArgumentBox>(mouseTipDisplayer.RuntimeParam);
						mouseTipDisplayer.RuntimeParam = null;
					}
					mouseTipDisplayer.enabled = false;
				}
			}
		}

		// Token: 0x06007394 RID: 29588 RVA: 0x0035B0AC File Offset: 0x003592AC
		public void FocusStrategySlot(byte pageIndex, int strategyIndex)
		{
			bool flag = this._curPage != null;
			if (flag)
			{
				bool flag2 = this._curStrategySlot >= 0;
				if (flag2)
				{
					this._curPage.strategySlotList[this._curStrategySlot].highlight.SetActive(false);
				}
				bool flag3 = this._curPage.UserInt != (int)pageIndex;
				if (flag3)
				{
					this._curPage.eventHighlight2.SetActive(false);
				}
			}
			this._curPage = this.pageList[(int)pageIndex];
			this._curStrategySlot = strategyIndex;
			this._curPage.eventHighlight2.SetActive(true);
			bool flag4 = this._curStrategySlot >= 0;
			if (flag4)
			{
				this._curPage.strategySlotList[this._curStrategySlot].highlight.SetActive(true);
			}
		}

		// Token: 0x06007395 RID: 29589 RVA: 0x0035B184 File Offset: 0x00359384
		public void Clear()
		{
			bool flag = this._pageProgressValues == null || this._pageProgressValues.Length != this.pageCount;
			if (flag)
			{
				this._pageProgressValues = new int[this.pageCount];
			}
			bool flag2 = this._pagePreviewProgressValues == null || this._pagePreviewProgressValues.Length != this.pageCount;
			if (flag2)
			{
				this._pagePreviewProgressValues = new int[this.pageCount];
			}
			bool flag3 = this._pageRereadPreviewValues == null || this._pageRereadPreviewValues.Length != this.pageCount;
			if (flag3)
			{
				this._pageRereadPreviewValues = new int[this.pageCount];
			}
			bool flag4 = this._baseSkipPages == null || this._baseSkipPages.Length != this.pageCount;
			if (flag4)
			{
				this._baseSkipPages = new bool[this.pageCount];
			}
			this._curPage = null;
			this._curStrategySlot = -1;
			this._hasPreview = false;
			for (int i = 0; i < this.pageCount; i++)
			{
				ReadingPagePrefab page = this.pageList[i];
				bool flag5 = this._baseSkipPages != null && i < this._baseSkipPages.Length;
				if (flag5)
				{
					this._baseSkipPages[i] = false;
				}
				bool flag6 = this._pageProgressValues != null && i < this._pageProgressValues.Length;
				if (flag6)
				{
					this._pageProgressValues[i] = 0;
				}
				bool flag7 = this._pagePreviewProgressValues != null && i < this._pagePreviewProgressValues.Length;
				if (flag7)
				{
					this._pagePreviewProgressValues[i] = 0;
				}
				bool flag8 = this._pageRereadPreviewValues != null && i < this._pageRereadPreviewValues.Length;
				if (flag8)
				{
					this._pageRereadPreviewValues[i] = 0;
				}
				page.pageStateIcon.gameObject.SetActive(false);
				page.pageStateText.gameObject.SetActive(false);
				page.pageStateText.SetText(string.Empty, true);
				page.progressBar.ResetDisplay();
				page.progressText.SetText("0%", true);
				page.highlight.SetActive(false);
				page.eventHighlight1.SetActive(false);
				page.eventHighlight2.SetActive(false);
				for (int j = 0; j < 3; j++)
				{
					ReadingStrategySlot strategySlot = page.strategySlotList[j];
					strategySlot.ResetStrategyState();
					strategySlot.highlight.SetActive(false);
					bool flag9 = strategySlot.previewMarker != null;
					if (flag9)
					{
						strategySlot.previewMarker.SetActive(false);
					}
					bool flag10 = strategySlot.previewMarkerText != null;
					if (flag10)
					{
						strategySlot.previewMarkerText.SetText(string.Empty, true);
					}
					bool flag11 = strategySlot.clearMarker != null;
					if (flag11)
					{
						strategySlot.clearMarker.gameObject.SetActive(false);
					}
					ReadingBriefStrategy strategyBrief = page.strategyBriefList[j];
					strategyBrief.SetStrategy(false);
				}
			}
		}

		// Token: 0x06007396 RID: 29590 RVA: 0x0035B490 File Offset: 0x00359690
		public void ApplyStrategyPreview(in ReadingSkillBookPagesDisplay.ReadingStrategyPreview preview)
		{
			this.ClearStrategyPreview();
			bool flag = (int)preview.PageIndex >= this.pageCount;
			if (!flag)
			{
				this._hasPreview = true;
				bool previewSkipPage = preview.PreviewSkipPage;
				if (previewSkipPage)
				{
					this.SetPreviewSkip(preview.PageIndex, true);
				}
				bool flag2 = preview.PreviewProgressList != null;
				if (flag2)
				{
					foreach (ReadingSkillBookPagesDisplay.PreviewPageProgress item in preview.PreviewProgressList)
					{
						bool flag3 = (int)item.PageIndex >= this.pageCount;
						if (!flag3)
						{
							int value = Math.Clamp(item.ProgressValue, 0, 100);
							bool flag4 = preview.PreviewSkipPage && item.PageIndex == preview.PageIndex;
							if (!flag4)
							{
								this.ApplyPageProgressDisplay(item.PageIndex, false, false, new int?(value), new int?(0));
							}
						}
					}
				}
				ReadingPagePrefab page = this.pageList[(int)preview.PageIndex];
				for (int i = 0; i < 3; i++)
				{
					ReadingStrategySlot slot = page.strategySlotList[i];
					ReadingSkillBookPagesDisplay.PreviewMarkerType marker = ReadingSkillBookPagesDisplay.PreviewMarkerType.None;
					bool flag5 = preview.SlotMarkers != null && i < preview.SlotMarkers.Length;
					if (flag5)
					{
						marker = preview.SlotMarkers[i];
					}
					bool flag6 = slot.previewMarker != null;
					if (flag6)
					{
						slot.previewMarker.SetActive(marker > ReadingSkillBookPagesDisplay.PreviewMarkerType.None);
					}
					bool flag7 = slot.previewMarkerText != null;
					if (flag7)
					{
						switch (marker)
						{
						case ReadingSkillBookPagesDisplay.PreviewMarkerType.Repeat:
							slot.previewMarkerText.SetText(LocalStringManager.Get(LanguageKey.LK_ReadingEvent_Preview_Repeat), true);
							slot.clearMarker.gameObject.SetActive(false);
							break;
						case ReadingSkillBookPagesDisplay.PreviewMarkerType.Clear:
							slot.previewMarkerText.SetText(LocalStringManager.Get(LanguageKey.LK_ReadingEvent_Preview_Clear), true);
							slot.clearMarker.gameObject.SetActive(true);
							slot.previewMarker.SetActive(false);
							break;
						case ReadingSkillBookPagesDisplay.PreviewMarkerType.EffectMultiplier:
						{
							bool flag8 = preview.EffectMultiplier > 1;
							if (flag8)
							{
								slot.previewMarkerText.SetText(LocalStringManager.GetFormat(LanguageKey.LK_ReadingEvent_Preview_EffectMultiplier, preview.EffectMultiplier), true);
							}
							else
							{
								slot.previewMarkerText.SetText(string.Empty, true);
							}
							slot.clearMarker.gameObject.SetActive(false);
							break;
						}
						default:
							slot.previewMarkerText.SetText(string.Empty, true);
							break;
						}
					}
				}
			}
		}

		// Token: 0x06007397 RID: 29591 RVA: 0x0035B748 File Offset: 0x00359948
		public void ClearStrategyPreview()
		{
			bool flag = !this._hasPreview;
			if (!flag)
			{
				this._hasPreview = false;
				byte i = 0;
				while ((int)i < this.pageCount)
				{
					this.ApplyPageProgressDisplay(i, this._baseSkipPages[(int)i], false, null, null);
					for (int j = 0; j < 3; j++)
					{
						ReadingStrategySlot slot = this.pageList[(int)i].strategySlotList[j];
						bool flag2 = slot.previewMarker != null;
						if (flag2)
						{
							slot.previewMarker.SetActive(false);
						}
						bool flag3 = slot.previewMarkerText != null;
						if (flag3)
						{
							slot.previewMarkerText.SetText(string.Empty, true);
						}
						bool flag4 = slot.clearMarker != null;
						if (flag4)
						{
							slot.clearMarker.gameObject.SetActive(false);
						}
					}
					i += 1;
				}
			}
		}

		// Token: 0x06007398 RID: 29592 RVA: 0x0035B852 File Offset: 0x00359A52
		private void SetPreviewSkip(byte pageIndex, bool isSkipped)
		{
			this.ApplyPageProgressDisplay(pageIndex, isSkipped, false, new int?(0), new int?(0));
		}

		// Token: 0x06007399 RID: 29593 RVA: 0x0035B86C File Offset: 0x00359A6C
		private void ApplyPageProgressDisplay(byte pageIndex, bool isSkipped, bool animateCurrent, int? previewProgressOverride = null, int? rereadPreviewOverride = null)
		{
			ReadingPagePrefab page = this.pageList[(int)pageIndex];
			int currentProgress = this._pageProgressValues[(int)pageIndex];
			int previewProgress = previewProgressOverride ?? this._pagePreviewProgressValues[(int)pageIndex];
			int rereadPreview = rereadPreviewOverride ?? this._pageRereadPreviewValues[(int)pageIndex];
			page.progressBar.SetDisplay((float)currentProgress / 100f, (float)previewProgress / 100f, (float)rereadPreview / 100f, isSkipped, animateCurrent);
			page.progressText.SetText(ReadingSkillBookPagesDisplay.GetPageProgressText(currentProgress, previewProgress, rereadPreview, isSkipped), true);
		}

		// Token: 0x0600739A RID: 29594 RVA: 0x0035B90C File Offset: 0x00359B0C
		private static void SetPageStateDisplay(ReadingPagePrefab page, sbyte pageState)
		{
			Sprite iconSprite;
			LanguageKey stateTextKey;
			bool hasDisplay = ReadingSkillBookPagesDisplay.TryGetPageStateDisplay(page, pageState, out iconSprite, out stateTextKey);
			page.pageStateIcon.gameObject.SetActive(hasDisplay);
			page.pageStateText.gameObject.SetActive(hasDisplay);
			bool flag = !hasDisplay;
			if (flag)
			{
				page.pageStateText.SetText(string.Empty, true);
			}
			else
			{
				page.pageStateIcon.sprite = iconSprite;
				page.pageStateText.SetText(LocalStringManager.Get(stateTextKey), true);
			}
		}

		// Token: 0x0600739B RID: 29595 RVA: 0x0035B988 File Offset: 0x00359B88
		private static bool TryGetPageStateDisplay(ReadingPagePrefab page, sbyte pageState, out Sprite iconSprite, out LanguageKey stateTextKey)
		{
			bool result;
			switch (pageState)
			{
			case 0:
				iconSprite = page.completePageStateSprite;
				stateTextKey = LanguageKey.LK_Book_Page_State_Complete_No_Symbol;
				result = true;
				break;
			case 1:
				iconSprite = page.incompletePageStateSprite;
				stateTextKey = LanguageKey.LK_Book_Page_State_Incomplete_No_Symbol;
				result = true;
				break;
			case 2:
				iconSprite = page.lostPageStateSprite;
				stateTextKey = LanguageKey.LK_Book_Page_State_Lost_No_Symbol;
				result = true;
				break;
			default:
				iconSprite = null;
				stateTextKey = LanguageKey.EventEditor_Error_DuplicateGroupKey;
				result = false;
				break;
			}
			return result;
		}

		// Token: 0x0600739C RID: 29596 RVA: 0x0035B9F4 File Offset: 0x00359BF4
		private static string GetPageProgressText(int currentProgress, int previewProgress, int rereadPreview, bool isSkipped)
		{
			string result;
			if (isSkipped)
			{
				result = LocalStringManager.Get(LanguageKey.LK_Reading_SkipPage);
			}
			else
			{
				int delta = (previewProgress > currentProgress) ? (previewProgress - currentProgress) : rereadPreview;
				string currentText = string.Format("{0}%", currentProgress);
				bool flag = delta <= 0;
				if (flag)
				{
					result = currentText;
				}
				else
				{
					result = currentText + string.Format("+{0}%", delta).SetColor("brightblue");
				}
			}
			return result;
		}

		// Token: 0x04005611 RID: 22033
		[SerializeField]
		private List<ReadingPagePrefab> pageList;

		// Token: 0x04005612 RID: 22034
		[SerializeField]
		private RectTransform back;

		// Token: 0x04005613 RID: 22035
		[SerializeField]
		private RectTransform pageSupply;

		// Token: 0x04005614 RID: 22036
		[SerializeField]
		private RectTransform pageSupplyState;

		// Token: 0x04005615 RID: 22037
		public int pageCount;

		// Token: 0x04005616 RID: 22038
		private ReadingPagePrefab _curPage;

		// Token: 0x04005617 RID: 22039
		private int _curStrategySlot;

		// Token: 0x04005618 RID: 22040
		private int[] _pageProgressValues;

		// Token: 0x04005619 RID: 22041
		private int[] _pagePreviewProgressValues;

		// Token: 0x0400561A RID: 22042
		private int[] _pageRereadPreviewValues;

		// Token: 0x0400561B RID: 22043
		private bool[] _baseSkipPages;

		// Token: 0x0400561C RID: 22044
		private bool _hasPreview;

		// Token: 0x0400561D RID: 22045
		private bool _animFoldFlag = false;

		// Token: 0x0400561E RID: 22046
		private readonly List<sbyte> _lastSuppliedStateList = new List<sbyte>();

		// Token: 0x0400561F RID: 22047
		private readonly List<sbyte> _curSuppliedStateList = new List<sbyte>();

		// Token: 0x02001E7B RID: 7803
		public struct PreviewPageProgress
		{
			// Token: 0x0400C9F0 RID: 51696
			public byte PageIndex;

			// Token: 0x0400C9F1 RID: 51697
			public int ProgressValue;
		}

		// Token: 0x02001E7C RID: 7804
		public enum PreviewMarkerType
		{
			// Token: 0x0400C9F3 RID: 51699
			None,
			// Token: 0x0400C9F4 RID: 51700
			Repeat,
			// Token: 0x0400C9F5 RID: 51701
			Clear,
			// Token: 0x0400C9F6 RID: 51702
			EffectMultiplier
		}

		// Token: 0x02001E7D RID: 7805
		public struct ReadingStrategyPreview
		{
			// Token: 0x0400C9F7 RID: 51703
			public byte PageIndex;

			// Token: 0x0400C9F8 RID: 51704
			public bool PreviewSkipPage;

			// Token: 0x0400C9F9 RID: 51705
			public int EffectMultiplier;

			// Token: 0x0400C9FA RID: 51706
			public ReadingSkillBookPagesDisplay.PreviewMarkerType[] SlotMarkers;

			// Token: 0x0400C9FB RID: 51707
			public List<ReadingSkillBookPagesDisplay.PreviewPageProgress> PreviewProgressList;
		}
	}
}
