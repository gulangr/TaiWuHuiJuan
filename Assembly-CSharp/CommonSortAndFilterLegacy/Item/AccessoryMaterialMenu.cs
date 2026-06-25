using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004AE RID: 1198
	public class AccessoryMaterialMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x060041AB RID: 16811 RVA: 0x00202576 File Offset: 0x00200776
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000721 RID: 1825
		// (get) Token: 0x060041AC RID: 16812 RVA: 0x00202579 File Offset: 0x00200779
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060041AD RID: 16813 RVA: 0x0020257C File Offset: 0x0020077C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Material);
		}

		// Token: 0x060041AE RID: 16814 RVA: 0x002025A0 File Offset: 0x002007A0
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

		// Token: 0x060041AF RID: 16815 RVA: 0x0020263C File Offset: 0x0020083C
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			ArmorItem armorConfig = Armor.Instance[data.Key.TemplateId];
			sbyte resourceType = armorConfig.ResourceType;
			int resourceTypeIndex = this._resourceTypeOptions.IndexOf(resourceType);
			return selectedIndices.Any((int selectedIndex) => selectedIndex == resourceTypeIndex);
		}

		// Token: 0x04002EA4 RID: 11940
		private readonly List<sbyte> _resourceTypeOptions = new List<sbyte>
		{
			4,
			1,
			3,
			2
		};
	}
}
