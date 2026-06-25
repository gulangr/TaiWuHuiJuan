using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000547 RID: 1351
	public class PoisonTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007FB RID: 2043
		// (get) Token: 0x060043B8 RID: 17336 RVA: 0x00207FDE File Offset: 0x002061DE
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007FC RID: 2044
		// (get) Token: 0x060043B9 RID: 17337 RVA: 0x00207FE1 File Offset: 0x002061E1
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060043BA RID: 17338 RVA: 0x00207FE4 File Offset: 0x002061E4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x060043BB RID: 17339 RVA: 0x00208008 File Offset: 0x00206208
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i <= 5; i++)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(string.Format("LK_Poison_Name_{0}", i))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x060043BC RID: 17340 RVA: 0x00208070 File Offset: 0x00206270
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			MedicineItem medicineConfig = Medicine.Instance[data.Key.TemplateId];
			EMedicineEffectSubType effectSubType = medicineConfig.EffectSubType;
			foreach (int selectedSubType in selectedIndices)
			{
				bool flag = selectedSubType == (int)effectSubType;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}
	}
}
