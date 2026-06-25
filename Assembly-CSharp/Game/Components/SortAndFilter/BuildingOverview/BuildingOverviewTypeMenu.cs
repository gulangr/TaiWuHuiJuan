using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Components.SortAndFilter.BuildingOverview
{
	// Token: 0x02000E7B RID: 3707
	public class BuildingOverviewTypeMenu : DetailedFilterMenuLogic<BuildingOverviewSortData>
	{
		// Token: 0x17001372 RID: 4978
		// (get) Token: 0x0600ACC2 RID: 44226 RVA: 0x004EF590 File Offset: 0x004ED790
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001373 RID: 4979
		// (get) Token: 0x0600ACC3 RID: 44227 RVA: 0x004EF593 File Offset: 0x004ED793
		public override int Id
		{
			get
			{
				return EBuildingOverviewMenuId.BuildingOverviewType.ToInt();
			}
		}

		// Token: 0x0600ACC4 RID: 44228 RVA: 0x004EF5A0 File Offset: 0x004ED7A0
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_Building_Func_Type;
		}

		// Token: 0x0600ACC5 RID: 44229 RVA: 0x004EF5BC File Offset: 0x004ED7BC
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Earn_SilverMoney),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Earn_Prestige),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Recruit_Talent),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Harvest_Materials),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Harvest_Item),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Improve_Reading_Efficiency),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Increase_Probability_Of_QualificationGrowth),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Improve_Skills),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Assist_Breaking_Through),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Assist_Manufacture),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Other)
			};
		}

		// Token: 0x0600ACC6 RID: 44230 RVA: 0x004EF6C8 File Offset: 0x004ED8C8
		public override bool IsDataMatch(BuildingOverviewSortData data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = selectedIndices == null || selectedIndices.Count == 0;
			return flag || selectedIndices.Contains((int)data.Building.FuncType);
		}
	}
}
