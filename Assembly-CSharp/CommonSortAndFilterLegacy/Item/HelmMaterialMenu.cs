using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004D3 RID: 1235
	public class HelmMaterialMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000765 RID: 1893
		// (get) Token: 0x0600423C RID: 16956 RVA: 0x00203B56 File Offset: 0x00201D56
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000766 RID: 1894
		// (get) Token: 0x0600423D RID: 16957 RVA: 0x00203B59 File Offset: 0x00201D59
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600423E RID: 16958 RVA: 0x00203B5C File Offset: 0x00201D5C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Material);
		}

		// Token: 0x0600423F RID: 16959 RVA: 0x00203B80 File Offset: 0x00201D80
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			foreach (sbyte resourceType in this._resourceTypeOptions)
			{
				ResourceTypeItem resourceTypeConfig = ResourceType.Instance[resourceType];
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = resourceTypeConfig.Icon,
					Text = resourceTypeConfig.Name
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x06004240 RID: 16960 RVA: 0x00203C1C File Offset: 0x00201E1C
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			ArmorItem armorConfig = Armor.Instance[data.Key.TemplateId];
			sbyte resourceType = armorConfig.ResourceType;
			int resourceTypeIndex = this._resourceTypeOptions.IndexOf(resourceType);
			return selectedIndices.Any((int selectedIndex) => selectedIndex == resourceTypeIndex);
		}

		// Token: 0x04002EDC RID: 11996
		private readonly List<sbyte> _resourceTypeOptions = new List<sbyte>
		{
			4,
			1,
			3,
			2,
			5
		};
	}
}
