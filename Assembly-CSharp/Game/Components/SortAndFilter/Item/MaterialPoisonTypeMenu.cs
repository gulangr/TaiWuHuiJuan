using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DB5 RID: 3509
	public class MaterialPoisonTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001270 RID: 4720
		// (get) Token: 0x0600A97E RID: 43390 RVA: 0x004E5BEB File Offset: 0x004E3DEB
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001271 RID: 4721
		// (get) Token: 0x0600A97F RID: 43391 RVA: 0x004E5BEE File Offset: 0x004E3DEE
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A980 RID: 43392 RVA: 0x004E5BF4 File Offset: 0x004E3DF4
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_PoisonType;
		}

		// Token: 0x0600A981 RID: 43393 RVA: 0x004E5C10 File Offset: 0x004E3E10
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (sbyte p = 0; p < 6; p += 1)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_Poison_Name_{0}", p))));
			}
			return configs;
		}

		// Token: 0x0600A982 RID: 43394 RVA: 0x004E5C60 File Offset: 0x004E3E60
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			MaterialItem materialConfig = Material.Instance[data.Key.TemplateId];
			PoisonsAndLevels poisonType = materialConfig.InnatePoisons;
			foreach (int index in selectedIndices)
			{
				bool flag = poisonType.GetValue(index) > 0;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}
	}
}
