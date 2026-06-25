using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000492 RID: 1170
	public class BookTypeSecondaryMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x06004149 RID: 16713 RVA: 0x0020128B File Offset: 0x001FF48B
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x0600414A RID: 16714 RVA: 0x0020128E File Offset: 0x001FF48E
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600414B RID: 16715 RVA: 0x00201294 File Offset: 0x001FF494
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x0600414C RID: 16716 RVA: 0x002012B8 File Offset: 0x001FF4B8
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", 1001))
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", 1000))
				}
			};
		}

		// Token: 0x0600414D RID: 16717 RVA: 0x0020134C File Offset: 0x001FF54C
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectedSubType in selectedIndices)
			{
				EBookSubFilterKeys ebookSubFilterKeys = (EBookSubFilterKeys)selectedSubType;
				EBookSubFilterKeys ebookSubFilterKeys2 = ebookSubFilterKeys;
				if (ebookSubFilterKeys2 != EBookSubFilterKeys.CombatSkill)
				{
					if (ebookSubFilterKeys2 == EBookSubFilterKeys.LifeSkill)
					{
						bool flag = ItemFilterCommon.IsItemMatchItemSubType(data, 1000);
						if (flag)
						{
							return true;
						}
					}
				}
				else
				{
					bool flag2 = ItemFilterCommon.IsItemMatchItemSubType(data, 1001);
					if (flag2)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
