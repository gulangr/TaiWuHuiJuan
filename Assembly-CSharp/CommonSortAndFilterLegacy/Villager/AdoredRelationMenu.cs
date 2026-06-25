using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Villager
{
	// Token: 0x02000460 RID: 1120
	public class AdoredRelationMenu<T> : StaticDetailedFilterMenuBase<T> where T : IVillagerSortAndFilterData
	{
		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x0600406D RID: 16493 RVA: 0x001FF68F File Offset: 0x001FD88F
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600406E RID: 16494 RVA: 0x001FF694 File Offset: 0x001FD894
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_3)
			};
		}

		// Token: 0x0600406F RID: 16495 RVA: 0x001FF6C0 File Offset: 0x001FD8C0
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < 3; i++)
			{
				configs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Character_Third_AdoredRelation_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x06004070 RID: 16496 RVA: 0x001FF724 File Offset: 0x001FD924
		public override bool IsDataMatch(T data, IReadOnlyCollection<int> selectedIndices)
		{
			ushort relationFromTaiwu = data.RelationFromTaiwu;
			ushort relationToTaiwu = data.RelationToTaiwu;
			bool flag = selectedIndices == null || selectedIndices.Count == 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						switch (enumerator.Current)
						{
						case 0:
						{
							bool flag2 = (relationFromTaiwu & 16384) > 0;
							if (flag2)
							{
								return true;
							}
							break;
						}
						case 1:
						{
							bool flag3 = (relationToTaiwu & 16384) > 0;
							if (flag3)
							{
								return true;
							}
							break;
						}
						case 2:
						{
							bool flag4 = (relationFromTaiwu & 16384) != 0 && (relationToTaiwu & 16384) > 0;
							if (flag4)
							{
								return true;
							}
							break;
						}
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x06004071 RID: 16497 RVA: 0x001FF820 File Offset: 0x001FDA20
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x06004072 RID: 16498 RVA: 0x001FF823 File Offset: 0x001FDA23
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(2, 0));
			}
		}
	}
}
