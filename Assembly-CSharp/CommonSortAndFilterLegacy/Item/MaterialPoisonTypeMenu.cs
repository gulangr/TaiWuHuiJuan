using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000537 RID: 1335
	internal class MaterialPoisonTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007E7 RID: 2023
		// (get) Token: 0x06004382 RID: 17282 RVA: 0x002073CF File Offset: 0x002055CF
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007E8 RID: 2024
		// (get) Token: 0x06004383 RID: 17283 RVA: 0x002073D2 File Offset: 0x002055D2
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004384 RID: 17284 RVA: 0x002073D8 File Offset: 0x002055D8
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			MaterialItem materialConfig = Material.Instance[data.Key.TemplateId];
			PoisonsAndLevels poisonType = materialConfig.InnatePoisons;
			foreach (int index in selectedIndices)
			{
				bool flag = poisonType.GetValue(index) > 0;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004385 RID: 17285 RVA: 0x0020745C File Offset: 0x0020565C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_PoisonType);
		}

		// Token: 0x06004386 RID: 17286 RVA: 0x00207480 File Offset: 0x00205680
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (sbyte p = 0; p < 6; p += 1)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(string.Format("LK_Poison_Name_{0}", p))
				});
			}
			return dropdownConfigs;
		}
	}
}
