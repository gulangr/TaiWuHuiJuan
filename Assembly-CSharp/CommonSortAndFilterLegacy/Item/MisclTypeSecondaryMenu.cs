using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000556 RID: 1366
	public class MisclTypeSecondaryMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000810 RID: 2064
		// (get) Token: 0x0600440B RID: 17419 RVA: 0x00208C93 File Offset: 0x00206E93
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000811 RID: 2065
		// (get) Token: 0x0600440C RID: 17420 RVA: 0x00208C96 File Offset: 0x00206E96
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600440D RID: 17421 RVA: 0x00208C9C File Offset: 0x00206E9C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x0600440E RID: 17422 RVA: 0x00208CC0 File Offset: 0x00206EC0
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (EMiscFilterKey t = EMiscFilterKey.Resource; t < EMiscFilterKey.Count; t += 1)
			{
				StringKey key = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Misc_{0}", (int)t));
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = key
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600440F RID: 17423 RVA: 0x00208D2C File Offset: 0x00206F2C
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			sbyte itemType = data.Key.ItemType;
			bool flag = itemType != 12 && itemType != 11;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				EMiscFilterKey itemIndexKey = this.GetToggleIndexKey(data.Key.ItemType, data.Key.TemplateId);
				foreach (int item in selectedIndices)
				{
					bool flag2 = item == (int)itemIndexKey;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06004410 RID: 17424 RVA: 0x00208DD0 File Offset: 0x00206FD0
		private EMiscFilterKey GetToggleIndexKey(sbyte itemType, short templateId)
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
				switch (miscConfig.FilterType)
				{
				case EMiscFilterType.Resource:
					result = EMiscFilterKey.Resource;
					break;
				case EMiscFilterType.Item:
					result = EMiscFilterKey.Item;
					break;
				case EMiscFilterType.Collection:
					result = EMiscFilterKey.Collection;
					break;
				case EMiscFilterType.KeyItem:
					result = EMiscFilterKey.KeyItem;
					break;
				case EMiscFilterType.LegendaryBook:
					result = EMiscFilterKey.LegendaryBook;
					break;
				case EMiscFilterType.WesternPresent:
					result = EMiscFilterKey.WesternPresent;
					break;
				case EMiscFilterType.Misc:
					result = EMiscFilterKey.Misc;
					break;
				default:
					result = EMiscFilterKey.Count;
					break;
				}
			}
			return result;
		}
	}
}
