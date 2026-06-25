using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004C0 RID: 1216
	public class BracersMaterialMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x060041EF RID: 16879 RVA: 0x00202DC6 File Offset: 0x00200FC6
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x060041F0 RID: 16880 RVA: 0x00202DC9 File Offset: 0x00200FC9
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060041F1 RID: 16881 RVA: 0x00202DCC File Offset: 0x00200FCC
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Material);
		}

		// Token: 0x060041F2 RID: 16882 RVA: 0x00202DF0 File Offset: 0x00200FF0
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

		// Token: 0x060041F3 RID: 16883 RVA: 0x00202E8C File Offset: 0x0020108C
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			ArmorItem armorConfig = Armor.Instance[data.Key.TemplateId];
			sbyte resourceType = armorConfig.ResourceType;
			int resourceTypeIndex = this._resourceTypeOptions.IndexOf(resourceType);
			return selectedIndices.Any((int selectedIndex) => selectedIndex == resourceTypeIndex);
		}

		// Token: 0x04002EC2 RID: 11970
		private readonly List<sbyte> _resourceTypeOptions = new List<sbyte>
		{
			4,
			1,
			3,
			2
		};
	}
}
