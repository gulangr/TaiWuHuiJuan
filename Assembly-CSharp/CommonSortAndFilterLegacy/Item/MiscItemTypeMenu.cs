using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200054B RID: 1355
	internal class MiscItemTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007FF RID: 2047
		// (get) Token: 0x060043C4 RID: 17348 RVA: 0x0020814B File Offset: 0x0020634B
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060043C5 RID: 17349 RVA: 0x00208150 File Offset: 0x00206350
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x060043C6 RID: 17350 RVA: 0x00208174 File Offset: 0x00206374
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> menuConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (MiscItemTypeMenu.EMenuKeys t = MiscItemTypeMenu.EMenuKeys.ImportantItem; t < MiscItemTypeMenu.EMenuKeys.Count; t++)
			{
				StringKey key = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Third_MiscItem_{0}", (int)t));
				menuConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = key
				});
			}
			return menuConfigs;
		}

		// Token: 0x060043C7 RID: 17351 RVA: 0x002081DC File Offset: 0x002063DC
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			MiscItem miscItem = Misc.Instance[data.Key.TemplateId];
			bool flag = selectedIndices == null || selectedIndices.Count == 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						switch (enumerator.Current)
						{
						case 0:
						{
							bool flag2 = MiscItemTypeMenu.IsImportItem(miscItem);
							if (flag2)
							{
								return true;
							}
							break;
						}
						case 1:
						{
							bool flag3 = MiscItemTypeMenu.IsSwordFragments(miscItem);
							if (flag3)
							{
								return true;
							}
							break;
						}
						case 2:
						{
							bool flag4 = MiscItemTypeMenu.IsItem(miscItem);
							if (flag4)
							{
								return true;
							}
							break;
						}
						case 3:
						{
							bool flag5 = MiscItemTypeMenu.IsBloodDew(miscItem);
							if (flag5)
							{
								return true;
							}
							break;
						}
						case 4:
						{
							bool flag6 = MiscItemTypeMenu.IsMountainSeed(miscItem);
							if (flag6)
							{
								return true;
							}
							break;
						}
						case 5:
						{
							bool flag7 = !MiscItemTypeMenu.IsImportItem(miscItem) && !MiscItemTypeMenu.IsSwordFragments(miscItem) && !MiscItemTypeMenu.IsBloodDew(miscItem) && !MiscItemTypeMenu.IsMountainSeed(miscItem) && !MiscItemTypeMenu.IsItem(miscItem);
							if (flag7)
							{
								return true;
							}
							break;
						}
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x17000800 RID: 2048
		// (get) Token: 0x060043C8 RID: 17352 RVA: 0x00208328 File Offset: 0x00206528
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060043C9 RID: 17353 RVA: 0x0020832C File Offset: 0x0020652C
		private static bool IsImportItem(MiscItem miscItem)
		{
			short templateId = miscItem.TemplateId;
			return templateId == 265 || templateId == 349 || templateId == 275 || templateId == 364 || templateId == 370;
		}

		// Token: 0x060043CA RID: 17354 RVA: 0x00208378 File Offset: 0x00206578
		private static bool IsSwordFragments(MiscItem miscItem)
		{
			return miscItem.GroupId == 229;
		}

		// Token: 0x060043CB RID: 17355 RVA: 0x00208398 File Offset: 0x00206598
		private static bool IsBloodDew(MiscItem miscItem)
		{
			return miscItem.GroupId == 9;
		}

		// Token: 0x060043CC RID: 17356 RVA: 0x002083B4 File Offset: 0x002065B4
		private static bool IsMountainSeed(MiscItem miscItem)
		{
			return miscItem.GroupId == 316;
		}

		// Token: 0x060043CD RID: 17357 RVA: 0x002083D4 File Offset: 0x002065D4
		private static bool IsItem(MiscItem miscItem)
		{
			short templateId = miscItem.TemplateId;
			return templateId == 18 || templateId == 264 || miscItem.GroupId == 82;
		}

		// Token: 0x02001940 RID: 6464
		private enum EMenuKeys
		{
			// Token: 0x0400B19A RID: 45466
			ImportantItem,
			// Token: 0x0400B19B RID: 45467
			SwordFragments,
			// Token: 0x0400B19C RID: 45468
			Item,
			// Token: 0x0400B19D RID: 45469
			BloodDew,
			// Token: 0x0400B19E RID: 45470
			MountainSeed,
			// Token: 0x0400B19F RID: 45471
			Other,
			// Token: 0x0400B1A0 RID: 45472
			Count
		}
	}
}
