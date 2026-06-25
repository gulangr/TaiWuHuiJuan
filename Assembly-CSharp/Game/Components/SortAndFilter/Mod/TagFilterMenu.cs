using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Mod
{
	// Token: 0x02000D1A RID: 3354
	public class TagFilterMenu : DetailedFilterMenuLogic<ModSortAndFilterData>
	{
		// Token: 0x17001196 RID: 4502
		// (get) Token: 0x0600A742 RID: 42818 RVA: 0x004DD6FC File Offset: 0x004DB8FC
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001197 RID: 4503
		// (get) Token: 0x0600A743 RID: 42819 RVA: 0x004DD6FF File Offset: 0x004DB8FF
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A744 RID: 42820 RVA: 0x004DD704 File Offset: 0x004DB904
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_Filter;
		}

		// Token: 0x0600A745 RID: 42821 RVA: 0x004DD720 File Offset: 0x004DB920
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			foreach (string tag in SteamManager.AllTagList)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(tag)));
			}
			return configs;
		}

		// Token: 0x0600A746 RID: 42822 RVA: 0x004DD78C File Offset: 0x004DB98C
		public override bool IsDataMatch(ModSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			int mask = 0;
			foreach (int selectedSubType in selectedIndices)
			{
				mask |= 1 << selectedSubType;
			}
			return (data.Tags & mask) != 0;
		}
	}
}
