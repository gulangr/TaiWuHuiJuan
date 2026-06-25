using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004CB RID: 1227
	public class ClothingWeaveTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x1700075A RID: 1882
		// (get) Token: 0x0600421D RID: 16925 RVA: 0x002033F2 File Offset: 0x002015F2
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x0600421E RID: 16926 RVA: 0x002033F5 File Offset: 0x002015F5
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600421F RID: 16927 RVA: 0x002033F8 File Offset: 0x002015F8
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004220 RID: 16928 RVA: 0x0020341C File Offset: 0x0020161C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (EClothingWeaveType t = EClothingWeaveType.Invalid; t < EClothingWeaveType.Count; t += 1)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Third_ClothingWeaveType_{0}", (int)t))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x06004221 RID: 16929 RVA: 0x00203480 File Offset: 0x00201680
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			ClothingItem clothingConfig = Clothing.Instance[data.Key.TemplateId];
			sbyte weaveType = clothingConfig.WeaveType;
			foreach (int selectedSubType in selectedIndices)
			{
				bool flag = selectedSubType == (int)weaveType;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}
	}
}
