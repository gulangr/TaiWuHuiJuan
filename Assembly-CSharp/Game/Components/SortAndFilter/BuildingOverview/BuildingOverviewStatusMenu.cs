using System;
using System.Collections.Generic;
using Game.Views.Buildings.Migrate;

namespace Game.Components.SortAndFilter.BuildingOverview
{
	// Token: 0x02000E7C RID: 3708
	public class BuildingOverviewStatusMenu : DetailedFilterMenuLogic<BuildingOverviewSortData>
	{
		// Token: 0x17001374 RID: 4980
		// (get) Token: 0x0600ACC8 RID: 44232 RVA: 0x004EF70B File Offset: 0x004ED90B
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001375 RID: 4981
		// (get) Token: 0x0600ACC9 RID: 44233 RVA: 0x004EF70E File Offset: 0x004ED90E
		public override int Id
		{
			get
			{
				return EBuildingOverviewMenuId.BuildingOverviewStatus.ToInt();
			}
		}

		// Token: 0x0600ACCA RID: 44234 RVA: 0x004EF71C File Offset: 0x004ED91C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_Building_State_Type;
		}

		// Token: 0x0600ACCB RID: 44235 RVA: 0x004EF738 File Offset: 0x004ED938
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Can_Built),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Not_Built),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Already_Built),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Unlocked),
				new FilterDropdownItemConfig(LanguageKey.LK_Building_Locked)
			};
		}

		// Token: 0x0600ACCC RID: 44236 RVA: 0x004EF7C0 File Offset: 0x004ED9C0
		public override bool IsDataMatch(BuildingOverviewSortData data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = selectedIndices == null || selectedIndices.Count == 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (int index in selectedIndices)
				{
					if (!true)
					{
					}
					bool flag2;
					switch (index)
					{
					case 0:
						flag2 = data.Status.HasFlag(BuildingOverviewBuildingPrefab.EBuildingStatus.CanBuild);
						break;
					case 1:
						flag2 = data.Status.HasFlag(BuildingOverviewBuildingPrefab.EBuildingStatus.NotBuild);
						break;
					case 2:
						flag2 = data.Status.HasFlag(BuildingOverviewBuildingPrefab.EBuildingStatus.AlreadyBuild);
						break;
					case 3:
						flag2 = data.Status.HasFlag(BuildingOverviewBuildingPrefab.EBuildingStatus.Unlocked);
						break;
					case 4:
						flag2 = data.Status.HasFlag(BuildingOverviewBuildingPrefab.EBuildingStatus.Locked);
						break;
					default:
						flag2 = false;
						break;
					}
					if (!true)
					{
					}
					bool match = flag2;
					bool flag3 = match;
					if (flag3)
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
