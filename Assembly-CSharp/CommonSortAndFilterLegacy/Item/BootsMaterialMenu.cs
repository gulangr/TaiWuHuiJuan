using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004B7 RID: 1207
	public class BootsMaterialMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x060041CD RID: 16845 RVA: 0x0020299E File Offset: 0x00200B9E
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x060041CE RID: 16846 RVA: 0x002029A1 File Offset: 0x00200BA1
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060041CF RID: 16847 RVA: 0x002029A4 File Offset: 0x00200BA4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Material);
		}

		// Token: 0x060041D0 RID: 16848 RVA: 0x002029C8 File Offset: 0x00200BC8
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

		// Token: 0x060041D1 RID: 16849 RVA: 0x00202A64 File Offset: 0x00200C64
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			ArmorItem armorConfig = Armor.Instance[data.Key.TemplateId];
			sbyte resourceType = armorConfig.ResourceType;
			int resourceTypeIndex = this._resourceTypeOptions.IndexOf(resourceType);
			return selectedIndices.Any((int selectedIndex) => selectedIndex == resourceTypeIndex);
		}

		// Token: 0x04002EB3 RID: 11955
		private readonly List<sbyte> _resourceTypeOptions = new List<sbyte>
		{
			4,
			1,
			3,
			2
		};
	}
}
