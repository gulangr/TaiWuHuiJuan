using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x0200058C RID: 1420
	public class EnemyRelationMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x17000855 RID: 2133
		// (get) Token: 0x060044DF RID: 17631 RVA: 0x0020ADD2 File Offset: 0x00208FD2
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060044E0 RID: 17632 RVA: 0x0020ADD8 File Offset: 0x00208FD8
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_4)
			};
		}

		// Token: 0x060044E1 RID: 17633 RVA: 0x0020AE04 File Offset: 0x00209004
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < 3; i++)
			{
				configs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Character_Third_EnemyRelation_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x060044E2 RID: 17634 RVA: 0x0020AE68 File Offset: 0x00209068
		public override bool IsDataMatch(CharacterTableSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			int relationFromTaiwu = data.Data.GetInt(105);
			int relationToTaiwu = data.Data.GetInt(104);
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
							bool flag2 = (relationFromTaiwu & 32768) != 0;
							if (flag2)
							{
								return true;
							}
							break;
						}
						case 1:
						{
							bool flag3 = (relationToTaiwu & 32768) != 0;
							if (flag3)
							{
								return true;
							}
							break;
						}
						case 2:
						{
							bool flag4 = (relationFromTaiwu & 32768) != 0 && (relationToTaiwu & 32768) != 0;
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

		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x060044E3 RID: 17635 RVA: 0x0020AF64 File Offset: 0x00209164
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x060044E4 RID: 17636 RVA: 0x0020AF67 File Offset: 0x00209167
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(2, 1));
			}
		}
	}
}
