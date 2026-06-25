using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D3D RID: 3389
	public class WeaponSubTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170011B7 RID: 4535
		// (get) Token: 0x0600A7B2 RID: 42930 RVA: 0x004DF8BB File Offset: 0x004DDABB
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170011B8 RID: 4536
		// (get) Token: 0x0600A7B3 RID: 42931 RVA: 0x004DF8BE File Offset: 0x004DDABE
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A7B4 RID: 42932 RVA: 0x004DF8C4 File Offset: 0x004DDAC4
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A7B5 RID: 42933 RVA: 0x004DF8E0 File Offset: 0x004DDAE0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < 16; i++)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", i))));
			}
			return configs;
		}

		// Token: 0x0600A7B6 RID: 42934 RVA: 0x004DF930 File Offset: 0x004DDB30
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
				short weaponSubType = weaponConfig.ItemSubType;
				foreach (int selectedSubType in selectedIndices)
				{
					bool flag2 = selectedSubType == (int)weaponSubType;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}
	}
}
