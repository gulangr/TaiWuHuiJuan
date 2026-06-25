using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DD1 RID: 3537
	public class MiscOtherTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001293 RID: 4755
		// (get) Token: 0x0600A9E9 RID: 43497 RVA: 0x004E6BB5 File Offset: 0x004E4DB5
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001294 RID: 4756
		// (get) Token: 0x0600A9EA RID: 43498 RVA: 0x004E6BB8 File Offset: 0x004E4DB8
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A9EB RID: 43499 RVA: 0x004E6BBC File Offset: 0x004E4DBC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A9EC RID: 43500 RVA: 0x004E6BD8 File Offset: 0x004E4DD8
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", 1204))),
				new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", 1201))),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Third_MiscItem_1")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Third_MiscItem_3")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Third_MiscItem_4")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Third_MiscItem_6"))
			};
		}

		// Token: 0x0600A9ED RID: 43501 RVA: 0x004E6C94 File Offset: 0x004E4E94
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Key.ItemType != 12;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				MiscItem miscItem = Misc.Instance[data.Key.TemplateId];
				using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						switch (enumerator.Current)
						{
						case 0:
						{
							bool flag2 = miscItem.ItemSubType == 1204;
							if (flag2)
							{
								return true;
							}
							break;
						}
						case 1:
						{
							bool flag3 = miscItem.ItemSubType == 1201;
							if (flag3)
							{
								return true;
							}
							break;
						}
						case 2:
						{
							bool flag4 = miscItem.GroupId == 229;
							if (flag4)
							{
								return true;
							}
							break;
						}
						case 3:
						{
							bool flag5 = miscItem.GroupId == 9;
							if (flag5)
							{
								return true;
							}
							break;
						}
						case 4:
						{
							bool flag6 = miscItem.GroupId == 316;
							if (flag6)
							{
								return true;
							}
							break;
						}
						case 5:
						{
							bool flag7 = MiscOtherTypeMenu.IsSpecial(miscItem);
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

		// Token: 0x0600A9EE RID: 43502 RVA: 0x004E6DD4 File Offset: 0x004E4FD4
		private static bool IsSpecial(MiscItem miscItem)
		{
			return !MiscOtherTypeMenu.IsImportantItem(miscItem) && miscItem.GroupId != 229 && miscItem.GroupId != 9 && miscItem.GroupId != 316 && !MiscOtherTypeMenu.IsItem(miscItem);
		}

		// Token: 0x0600A9EF RID: 43503 RVA: 0x004E6E20 File Offset: 0x004E5020
		private static bool IsImportantItem(MiscItem miscItem)
		{
			short templateId = miscItem.TemplateId;
			return templateId == 265 || templateId == 349 || templateId == 275 || templateId == 364 || templateId == 370;
		}

		// Token: 0x0600A9F0 RID: 43504 RVA: 0x004E6E6C File Offset: 0x004E506C
		private static bool IsItem(MiscItem miscItem)
		{
			short templateId = miscItem.TemplateId;
			return templateId == 18 || templateId == 264 || miscItem.GroupId == 82;
		}

		// Token: 0x0200249A RID: 9370
		private enum EMenuKeys
		{
			// Token: 0x0400E4E5 RID: 58597
			OtherInsect,
			// Token: 0x0400E4E6 RID: 58598
			CricketJar,
			// Token: 0x0400E4E7 RID: 58599
			SwordFragments,
			// Token: 0x0400E4E8 RID: 58600
			BloodDew,
			// Token: 0x0400E4E9 RID: 58601
			MountainSeed,
			// Token: 0x0400E4EA RID: 58602
			Special,
			// Token: 0x0400E4EB RID: 58603
			Count
		}
	}
}
