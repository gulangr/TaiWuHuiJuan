using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DC3 RID: 3523
	public abstract class MaterialCommonHardnessMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x0600A9AD RID: 43437 RVA: 0x004E5FF8 File Offset: 0x004E41F8
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness;
		}

		// Token: 0x17001280 RID: 4736
		// (get) Token: 0x0600A9AE RID: 43438 RVA: 0x004E6014 File Offset: 0x004E4214
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600A9AF RID: 43439 RVA: 0x004E6018 File Offset: 0x004E4218
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			foreach (EMaterialFilterHardness hardness in this.GenerateMaterialHardnessList())
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Material_FilterHardness_{0}", (int)hardness))));
			}
			return configs;
		}

		// Token: 0x0600A9B0 RID: 43440 RVA: 0x004E6090 File Offset: 0x004E4290
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Key.ItemType != 5;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				MaterialItem materialConfig = Material.Instance[data.Key.TemplateId];
				foreach (int index in selectedIndices)
				{
					EMaterialFilterHardness hardness = this.GenerateMaterialHardnessList().ElementAt(index);
					bool flag2 = materialConfig.FilterHardness == hardness;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600A9B1 RID: 43441
		protected abstract IEnumerable<EMaterialFilterHardness> GenerateMaterialHardnessList();
	}
}
