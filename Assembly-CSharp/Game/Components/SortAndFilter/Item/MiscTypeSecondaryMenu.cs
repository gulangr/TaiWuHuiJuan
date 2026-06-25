using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DE8 RID: 3560
	public class MiscTypeSecondaryMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170012BB RID: 4795
		// (get) Token: 0x0600AA77 RID: 43639 RVA: 0x004E83CB File Offset: 0x004E65CB
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012BC RID: 4796
		// (get) Token: 0x0600AA78 RID: 43640 RVA: 0x004E83CE File Offset: 0x004E65CE
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AA79 RID: 43641 RVA: 0x004E83D4 File Offset: 0x004E65D4
		public static int GetSecondaryFilterIndex(EMiscFilterKey key)
		{
			int index = MiscTypeSecondaryMenu.VisibleMiscFilterKeys.IndexOf(key);
			return (index >= 0) ? index : int.MaxValue;
		}

		// Token: 0x0600AA7A RID: 43642 RVA: 0x004E8400 File Offset: 0x004E6600
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AA7B RID: 43643 RVA: 0x004E841C File Offset: 0x004E661C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			foreach (EMiscFilterKey key in MiscTypeSecondaryMenu.VisibleMiscFilterKeys)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Misc_{0}", (int)key))));
			}
			return configs;
		}

		// Token: 0x0600AA7C RID: 43644 RVA: 0x004E8498 File Offset: 0x004E6698
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectedIndex in selectedIndices)
			{
				bool flag = selectedIndex < 0 || selectedIndex >= MiscTypeSecondaryMenu.VisibleMiscFilterKeys.Count;
				if (!flag)
				{
					EMiscFilterKey selectedKey = MiscTypeSecondaryMenu.VisibleMiscFilterKeys[selectedIndex];
					bool flag2 = selectedKey == EMiscFilterKey.Jiao;
					if (flag2)
					{
						bool flag3 = data.JiaoLoongDisplayData != null;
						if (flag3)
						{
							return true;
						}
					}
					else
					{
						sbyte itemType = data.Key.ItemType;
						bool flag4 = itemType != 12 && itemType != 11;
						if (!flag4)
						{
							EMiscFilterKey itemIndexKey = MiscTypeSecondaryMenu.GetMiscFilterKey(data.Key.ItemType, data.Key.TemplateId);
							bool flag5 = selectedKey == itemIndexKey;
							if (flag5)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x0600AA7D RID: 43645 RVA: 0x004E8590 File Offset: 0x004E6790
		private static EMiscFilterKey GetMiscFilterKey(sbyte itemType, short templateId)
		{
			bool flag = itemType == 11;
			EMiscFilterKey result;
			if (flag)
			{
				result = EMiscFilterKey.Cricket;
			}
			else
			{
				MiscItem miscConfig = Misc.Instance[templateId];
				bool flag2 = MiscTypeSecondaryMenu.IsMiscOtherType(miscConfig);
				if (flag2)
				{
					result = EMiscFilterKey.Other;
				}
				else
				{
					EMiscFilterType filterType = miscConfig.FilterType;
					if (!true)
					{
					}
					EMiscFilterKey emiscFilterKey;
					switch (filterType)
					{
					case EMiscFilterType.Resource:
						emiscFilterKey = EMiscFilterKey.Resource;
						goto IL_94;
					case EMiscFilterType.Item:
						emiscFilterKey = EMiscFilterKey.Item;
						goto IL_94;
					case EMiscFilterType.Collection:
						emiscFilterKey = EMiscFilterKey.Collection;
						goto IL_94;
					case EMiscFilterType.KeyItem:
						emiscFilterKey = EMiscFilterKey.KeyItem;
						goto IL_94;
					case EMiscFilterType.LegendaryBook:
						emiscFilterKey = EMiscFilterKey.LegendaryBook;
						goto IL_94;
					case EMiscFilterType.WesternPresent:
						emiscFilterKey = EMiscFilterKey.WesternPresent;
						goto IL_94;
					case EMiscFilterType.Misc:
						emiscFilterKey = EMiscFilterKey.Misc;
						goto IL_94;
					case EMiscFilterType.Other:
						emiscFilterKey = EMiscFilterKey.Other;
						goto IL_94;
					}
					emiscFilterKey = EMiscFilterKey.Count;
					IL_94:
					if (!true)
					{
					}
					result = emiscFilterKey;
				}
			}
			return result;
		}

		// Token: 0x0600AA7E RID: 43646 RVA: 0x004E863C File Offset: 0x004E683C
		private static bool IsMiscOtherType(MiscItem miscItem)
		{
			short itemSubType = miscItem.ItemSubType;
			return itemSubType == 1204 || itemSubType == 1201 || miscItem.GroupId == 229 || miscItem.GroupId == 9 || miscItem.GroupId == 316 || MiscTypeSecondaryMenu.IsSpecial(miscItem);
		}

		// Token: 0x0600AA7F RID: 43647 RVA: 0x004E8694 File Offset: 0x004E6894
		private static bool IsSpecial(MiscItem miscItem)
		{
			bool flag = miscItem.FilterType != EMiscFilterType.Other;
			return !flag && (!MiscTypeSecondaryMenu.IsImportantItem(miscItem) && miscItem.GroupId != 229 && miscItem.GroupId != 9 && miscItem.GroupId != 316) && !MiscTypeSecondaryMenu.IsItem(miscItem);
		}

		// Token: 0x0600AA80 RID: 43648 RVA: 0x004E86F4 File Offset: 0x004E68F4
		private static bool IsImportantItem(MiscItem miscItem)
		{
			short templateId = miscItem.TemplateId;
			return templateId == 265 || templateId == 349 || templateId == 275 || templateId == 364 || templateId == 370;
		}

		// Token: 0x0600AA81 RID: 43649 RVA: 0x004E8740 File Offset: 0x004E6940
		private static bool IsItem(MiscItem miscItem)
		{
			short templateId = miscItem.TemplateId;
			return templateId == 18 || templateId == 264 || miscItem.GroupId == 82;
		}

		// Token: 0x040084C6 RID: 33990
		public const int InvalidSecondaryIndex = 2147483647;

		// Token: 0x040084C7 RID: 33991
		private static readonly List<EMiscFilterKey> VisibleMiscFilterKeys = new List<EMiscFilterKey>
		{
			EMiscFilterKey.Resource,
			EMiscFilterKey.KeyItem,
			EMiscFilterKey.Cricket,
			EMiscFilterKey.LegendaryBook,
			EMiscFilterKey.WesternPresent,
			EMiscFilterKey.Jiao,
			EMiscFilterKey.Other
		};
	}
}
