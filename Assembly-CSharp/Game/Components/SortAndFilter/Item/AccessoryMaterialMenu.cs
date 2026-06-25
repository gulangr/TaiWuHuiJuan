using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D6A RID: 3434
	public class AccessoryMaterialMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700120D RID: 4621
		// (get) Token: 0x0600A849 RID: 43081 RVA: 0x004E05F7 File Offset: 0x004DE7F7
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700120E RID: 4622
		// (get) Token: 0x0600A84A RID: 43082 RVA: 0x004E05FA File Offset: 0x004DE7FA
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A84B RID: 43083 RVA: 0x004E0600 File Offset: 0x004DE800
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Material;
		}

		// Token: 0x0600A84C RID: 43084 RVA: 0x004E061C File Offset: 0x004DE81C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			foreach (sbyte resourceType in this._resourceTypeOptions)
			{
				ResourceTypeItem resourceTypeConfig = ResourceType.Instance[resourceType];
				configs.Add(new FilterDropdownItemConfig
				{
					Text = resourceTypeConfig.Name
				});
			}
			return configs;
		}

		// Token: 0x0600A84D RID: 43085 RVA: 0x004E06AC File Offset: 0x004DE8AC
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			AccessoryItem accessoryConfig = Accessory.Instance[data.Key.TemplateId];
			bool flag = accessoryConfig == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte resourceType = accessoryConfig.ResourceType;
				int resourceTypeIndex = this._resourceTypeOptions.IndexOf(resourceType);
				result = selectedIndices.Any((int index) => index == resourceTypeIndex);
			}
			return result;
		}

		// Token: 0x040083E4 RID: 33764
		private readonly List<sbyte> _resourceTypeOptions = new List<sbyte>
		{
			4,
			1,
			3,
			2
		};
	}
}
