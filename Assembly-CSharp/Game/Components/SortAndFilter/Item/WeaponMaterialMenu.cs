using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D3E RID: 3390
	public class WeaponMaterialMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170011B9 RID: 4537
		// (get) Token: 0x0600A7B8 RID: 42936 RVA: 0x004DF9C1 File Offset: 0x004DDBC1
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170011BA RID: 4538
		// (get) Token: 0x0600A7B9 RID: 42937 RVA: 0x004DF9C4 File Offset: 0x004DDBC4
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A7BA RID: 42938 RVA: 0x004DF9C8 File Offset: 0x004DDBC8
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Material;
		}

		// Token: 0x0600A7BB RID: 42939 RVA: 0x004DF9E4 File Offset: 0x004DDBE4
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

		// Token: 0x0600A7BC RID: 42940 RVA: 0x004DFA74 File Offset: 0x004DDC74
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			WeaponItem weaponConfig = Weapon.Instance[data.Key.TemplateId];
			bool flag = weaponConfig == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte resourceType = weaponConfig.ResourceType;
				int resourceTypeIndex = this._resourceTypeOptions.IndexOf(resourceType);
				foreach (int index in selectedIndices)
				{
					bool flag2 = index == resourceTypeIndex;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x040083A3 RID: 33699
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
