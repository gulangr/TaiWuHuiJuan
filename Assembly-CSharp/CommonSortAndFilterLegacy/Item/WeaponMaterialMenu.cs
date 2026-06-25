using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004E8 RID: 1256
	public class WeaponMaterialMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x0600428B RID: 17035 RVA: 0x002044FD File Offset: 0x002026FD
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x0600428C RID: 17036 RVA: 0x00204500 File Offset: 0x00202700
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600428D RID: 17037 RVA: 0x00204504 File Offset: 0x00202704
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Material);
		}

		// Token: 0x0600428E RID: 17038 RVA: 0x00204528 File Offset: 0x00202728
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

		// Token: 0x0600428F RID: 17039 RVA: 0x002045C4 File Offset: 0x002027C4
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			WeaponItem weaponConfig = Weapon.Instance[data.Key.TemplateId];
			sbyte resourceType = weaponConfig.ResourceType;
			int resourceTypeIndex = this._resourceTypeOptions.IndexOf(resourceType);
			return selectedIndices.Any((int selectedIndex) => selectedIndex == resourceTypeIndex);
		}

		// Token: 0x04002F05 RID: 12037
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
