using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200049D RID: 1181
	public abstract class CommonSkillBookReadStatusMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x0600416F RID: 16751 RVA: 0x00201789 File Offset: 0x001FF989
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004170 RID: 16752 RVA: 0x0020178C File Offset: 0x001FF98C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Reading);
		}

		// Token: 0x06004171 RID: 16753 RVA: 0x002017B0 File Offset: 0x001FF9B0
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_BookReasState_0
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_BookReasState_1
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_BookReasState_2
				}
			};
		}

		// Token: 0x06004172 RID: 16754 RVA: 0x00201844 File Offset: 0x001FFA44
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
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
							bool flag2 = CommonSkillBookReadStatusMenu.IsReadFinished(data);
							if (flag2)
							{
								return true;
							}
							break;
						}
						case 1:
						{
							bool flag3 = CommonSkillBookReadStatusMenu.IsReading(data);
							if (flag3)
							{
								return true;
							}
							break;
						}
						case 2:
						{
							bool flag4 = CommonSkillBookReadStatusMenu.IsUnread(data);
							if (flag4)
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

		// Token: 0x06004173 RID: 16755 RVA: 0x00201900 File Offset: 0x001FFB00
		private static bool IsReadFinished(ItemDisplayData data)
		{
			return data.IsReadingFinished;
		}

		// Token: 0x06004174 RID: 16756 RVA: 0x00201918 File Offset: 0x001FFB18
		private static bool IsReading(ItemDisplayData data)
		{
			return data.UsingType == ItemDisplayData.ItemUsingType.Reading;
		}

		// Token: 0x06004175 RID: 16757 RVA: 0x00201934 File Offset: 0x001FFB34
		private static bool IsUnread(ItemDisplayData data)
		{
			return !CommonSkillBookReadStatusMenu.IsReadFinished(data) && !CommonSkillBookReadStatusMenu.IsReading(data);
		}
	}
}
