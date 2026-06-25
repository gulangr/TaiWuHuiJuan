using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D45 RID: 3397
	public abstract class ArmorMaterialMenuBase : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170011CC RID: 4556
		// (get) Token: 0x0600A7DC RID: 42972 RVA: 0x004DFEC0 File Offset: 0x004DE0C0
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600A7DD RID: 42973 RVA: 0x004DFEC4 File Offset: 0x004DE0C4
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Material;
		}

		// Token: 0x0600A7DE RID: 42974 RVA: 0x004DFEE0 File Offset: 0x004DE0E0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			foreach (sbyte resourceType in this.ResourceTypeOptions)
			{
				ResourceTypeItem resourceTypeConfig = ResourceType.Instance[resourceType];
				configs.Add(new FilterDropdownItemConfig
				{
					Text = resourceTypeConfig.Name
				});
			}
			return configs;
		}

		// Token: 0x0600A7DF RID: 42975 RVA: 0x004DFF70 File Offset: 0x004DE170
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			ArmorItem armorConfig = Armor.Instance[data.Key.TemplateId];
			bool flag = armorConfig == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte resourceType = armorConfig.ResourceType;
				int resourceTypeIndex = this.ResourceTypeOptions.IndexOf(resourceType);
				result = selectedIndices.Any((int index) => index == resourceTypeIndex);
			}
			return result;
		}

		// Token: 0x040083A5 RID: 33701
		protected readonly List<sbyte> ResourceTypeOptions = new List<sbyte>
		{
			4,
			1,
			3,
			2
		};
	}
}
