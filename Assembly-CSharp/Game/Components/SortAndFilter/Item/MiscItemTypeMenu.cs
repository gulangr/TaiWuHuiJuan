using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DCB RID: 3531
	public class MiscItemTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700128B RID: 4747
		// (get) Token: 0x0600A9CE RID: 43470 RVA: 0x004E678C File Offset: 0x004E498C
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700128C RID: 4748
		// (get) Token: 0x0600A9CF RID: 43471 RVA: 0x004E678F File Offset: 0x004E498F
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A9D0 RID: 43472 RVA: 0x004E6794 File Offset: 0x004E4994
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A9D1 RID: 43473 RVA: 0x004E67B0 File Offset: 0x004E49B0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (MiscItemTypeMenu.EMenuKeys t = MiscItemTypeMenu.EMenuKeys.ImportantItem; t < MiscItemTypeMenu.EMenuKeys.Count; t++)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Third_MiscItem_{0}", (int)t))));
			}
			return configs;
		}

		// Token: 0x0600A9D2 RID: 43474 RVA: 0x004E67FC File Offset: 0x004E49FC
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
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

		// Token: 0x0600A9D3 RID: 43475 RVA: 0x004E6948 File Offset: 0x004E4B48
		private static bool IsImportItem(MiscItem miscItem)
		{
			short templateId = miscItem.TemplateId;
			return templateId == 265 || templateId == 349 || templateId == 275 || templateId == 364 || templateId == 370;
		}

		// Token: 0x0600A9D4 RID: 43476 RVA: 0x004E6994 File Offset: 0x004E4B94
		private static bool IsSwordFragments(MiscItem miscItem)
		{
			return miscItem.GroupId == 229;
		}

		// Token: 0x0600A9D5 RID: 43477 RVA: 0x004E69B4 File Offset: 0x004E4BB4
		private static bool IsBloodDew(MiscItem miscItem)
		{
			return miscItem.GroupId == 9;
		}

		// Token: 0x0600A9D6 RID: 43478 RVA: 0x004E69D0 File Offset: 0x004E4BD0
		private static bool IsMountainSeed(MiscItem miscItem)
		{
			return miscItem.GroupId == 316;
		}

		// Token: 0x0600A9D7 RID: 43479 RVA: 0x004E69F0 File Offset: 0x004E4BF0
		private static bool IsItem(MiscItem miscItem)
		{
			short templateId = miscItem.TemplateId;
			return templateId == 18 || templateId == 264 || miscItem.GroupId == 82;
		}

		// Token: 0x02002495 RID: 9365
		private enum EMenuKeys
		{
			// Token: 0x0400E4D1 RID: 58577
			ImportantItem,
			// Token: 0x0400E4D2 RID: 58578
			SwordFragments,
			// Token: 0x0400E4D3 RID: 58579
			Item,
			// Token: 0x0400E4D4 RID: 58580
			BloodDew,
			// Token: 0x0400E4D5 RID: 58581
			MountainSeed,
			// Token: 0x0400E4D6 RID: 58582
			Other,
			// Token: 0x0400E4D7 RID: 58583
			Count
		}
	}
}
