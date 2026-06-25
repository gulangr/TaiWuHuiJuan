using System;
using System.Collections.Generic;
using System.Text;
using FrameWork;
using Game.Views.Building.BuildingManage;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Utilities;
using TMPro;

namespace Game.Views.Main.Reading
{
	// Token: 0x02000968 RID: 2408
	public static class ReadingDisplayHelper
	{
		// Token: 0x06007372 RID: 29554 RVA: 0x0035A008 File Offset: 0x00358208
		public static string GetDurabilityPreviewString(int currentValue, int maxValue, int costValue)
		{
			bool flag = maxValue == 0;
			string result2;
			if (flag)
			{
				result2 = "-";
			}
			else
			{
				StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
				string currentValueColor = (currentValue <= maxValue / 2) ? "brightred" : "pinkyellow";
				strBuilder.Clear();
				strBuilder.Append(currentValue.ToString().SetColor(currentValueColor));
				strBuilder.Append(((costValue > 0) ? string.Format("-{0}", costValue) : costValue.ToString()).SetColor("brightred"));
				strBuilder.Append(string.Format("/{0}", maxValue).SetColor("pinkyellow"));
				string result = strBuilder.ToString();
				EasyPool.Free<StringBuilder>(strBuilder);
				result2 = result;
			}
			return result2;
		}

		// Token: 0x06007373 RID: 29555 RVA: 0x0035A0C4 File Offset: 0x003582C4
		public static void UpdateReadingBookInfo(ReadingBookIntro refers, ReadingInspireHolder inspireHolder, ItemKey bookKey, ItemDisplayData displayData, ItemKey[] referenceBooks, bool strategyRefresh = false, Action changeBookEvent = null, bool isNeedRemoveBook = true)
		{
			ItemResourceButton itemView = refers.itemCard;
			itemView.gameObject.SetActive(bookKey.IsValid());
			refers.itemEmptyCard.SetActive(!bookKey.IsValid());
			refers.removeCurBookBtn.SetActive(bookKey.IsValid() && isNeedRemoveBook);
			bool flag = !bookKey.IsValid();
			if (flag)
			{
				refers.durability.text = "- / -".SetColor("lightgrey");
				refers.expGainText.text = "-".SetColor("lightgrey");
				bool flag2 = inspireHolder != null;
				if (flag2)
				{
					inspireHolder.inspireRatio.text = "- %".SetColor("lightgrey");
				}
				refers.infoRoot.SetActive(false);
				itemView.SetAsEmpty();
			}
			else
			{
				refers.durability.text = ReadingDisplayHelper.GetDurabilityPreviewString((int)displayData.Durability, (int)displayData.MaxDurability, 1);
				refers.expTitle.text = LocalStringManager.Get(LanguageKey.LK_ExpGain) + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
				refers.RefreshAllBookReadingData(bookKey, strategyRefresh);
				if (inspireHolder != null)
				{
					inspireHolder.RefreshAllBookReadingData(bookKey);
				}
				refers.infoRoot.SetActive(true);
				itemView.SetButtonFunc(displayData, (changeBookEvent == null) ? ItemResourceButton.ItemResourceButtonState.None : ItemResourceButton.ItemResourceButtonState.Change, null, changeBookEvent, null);
				TooltipInvoker tipDisplayer = itemView.GetComponent<TooltipInvoker>();
				tipDisplayer.Type = TipType.ReadingBook;
				tipDisplayer.RuntimeParam = new ArgumentBox().Set<ItemKey>("currentReadingBookKey", bookKey);
				tipDisplayer.RuntimeParam.SetObject("referenceBooks", referenceBooks);
			}
		}

		// Token: 0x06007374 RID: 29556 RVA: 0x0035A250 File Offset: 0x00358450
		public static ReadingSkillBookPagesDisplay UpdatePages(ReadingPages refers, ItemKey bookKey, SkillBookPageDisplayData displayData, ReadingBookStrategies strategies, IntList strategyExpireTime, int[] extraProgress, bool isBookRead, bool onInit = false, bool clearPages = true, List<SkillBookPageDisplayData> refDataList = null, Action<int> onClickPage = null, bool tutorialMode = false)
		{
			ReadingSkillBookPagesDisplay fivePages = refers.fivePages;
			ReadingSkillBookPagesDisplay sixPages = refers.sixPages;
			bool flag = !bookKey.IsValid();
			ReadingSkillBookPagesDisplay result;
			if (flag)
			{
				fivePages.gameObject.SetActive(false);
				sixPages.gameObject.SetActive(false);
				result = null;
			}
			else
			{
				bool isCombatSkillBook = ItemTemplateHelper.GetItemSubType(bookKey.ItemType, bookKey.TemplateId) == 1001;
				fivePages.gameObject.SetActive(!isCombatSkillBook);
				sixPages.gameObject.SetActive(isCombatSkillBook);
				ReadingSkillBookPagesDisplay curPages = isCombatSkillBook ? sixPages : fivePages;
				if (clearPages)
				{
					curPages.Clear();
				}
				curPages.UpdatePages(displayData, strategies, strategyExpireTime, extraProgress, isBookRead, onInit, clearPages, refDataList, onClickPage, tutorialMode);
				result = curPages;
			}
			return result;
		}

		// Token: 0x06007375 RID: 29557 RVA: 0x0035A30C File Offset: 0x0035850C
		public static void SetPageCompleteState(sbyte pageState, CImage pageImg)
		{
			bool flag = pageState == 0;
			if (flag)
			{
				pageImg.SetSprite("ui9_icon_reading_state_valid", false, null);
			}
			else
			{
				bool flag2 = 1 == pageState;
				if (flag2)
				{
					pageImg.SetSprite("ui9_icon_reading_state_invalid", false, null);
				}
				else
				{
					bool flag3 = 2 == pageState;
					if (flag3)
					{
						pageImg.SetSprite("ui9_icon_reading_state_lost", false, null);
					}
				}
			}
		}

		// Token: 0x06007376 RID: 29558 RVA: 0x0035A368 File Offset: 0x00358568
		public static void SetPageSkillType(byte pageIndex, sbyte pageType, bool isDirect, CImage pageImg)
		{
			bool flag = pageIndex == 0;
			if (flag)
			{
				pageImg.SetSprite(string.Format("ui9_icon_reading_first_page_type_{0}", pageType), false, null);
			}
			else
			{
				pageImg.SetSprite(isDirect ? string.Format("ui9_icon_reading_direct_page_type_{0}", (int)(pageIndex - 1)) : string.Format("ui9_icon_reading_reverse_page_type_{0}", (int)(pageIndex - 1)), false, null);
			}
		}

		// Token: 0x06007377 RID: 29559 RVA: 0x0035A3D0 File Offset: 0x003585D0
		public static void SetNumPageTypeText(byte pageIndex, sbyte pageType, bool isDirect, TextMeshProUGUI pageTxtMesh)
		{
			bool flag = pageIndex == 0;
			string stringKey;
			string colorKey;
			if (flag)
			{
				stringKey = string.Format("LK_CombatSkill_First_Page_Type_{0}", pageType);
				colorKey = ReadingDisplayHelper.FirstPageColorInfo[(int)pageType];
			}
			else
			{
				stringKey = (isDirect ? string.Format("LK_CombatSkill_Direct_Page_{0}", (int)(pageIndex - 1)) : string.Format("LK_CombatSkill_Reverse_Page_{0}", (int)(pageIndex - 1)));
				colorKey = (isDirect ? "96DCF9" : "E2773D");
			}
			pageTxtMesh.text = LocalStringManager.Get(stringKey).SetColor(colorKey);
		}

		// Token: 0x06007378 RID: 29560 RVA: 0x0035A460 File Offset: 0x00358660
		public static void SetNumPageText(bool isCombatBook, byte pageIndex, TextMeshProUGUI pageTxtMesh)
		{
			string stringKey;
			if (isCombatBook)
			{
				stringKey = ((pageIndex == 0) ? "LK_CombatSkill_Book_First_Page" : string.Format("LK_Book_Page_Index_{0}", (int)(pageIndex - 1)));
			}
			else
			{
				stringKey = string.Format("LK_Book_Page_Index_{0}", pageIndex);
			}
			pageTxtMesh.text = LocalStringManager.Get(stringKey);
		}

		// Token: 0x040055CA RID: 21962
		private static readonly string[] FirstPageColorInfo = new string[]
		{
			"FFEFB0",
			"1ABAC8",
			"E5E0D8",
			"B975FF",
			"FC542C"
		};
	}
}
