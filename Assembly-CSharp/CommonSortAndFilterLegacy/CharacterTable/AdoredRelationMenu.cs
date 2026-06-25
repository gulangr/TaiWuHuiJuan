using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x0200058B RID: 1419
	public class AdoredRelationMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x17000852 RID: 2130
		// (get) Token: 0x060044D8 RID: 17624 RVA: 0x0020AC28 File Offset: 0x00208E28
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060044D9 RID: 17625 RVA: 0x0020AC2C File Offset: 0x00208E2C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_3)
			};
		}

		// Token: 0x060044DA RID: 17626 RVA: 0x0020AC58 File Offset: 0x00208E58
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

		// Token: 0x060044DB RID: 17627 RVA: 0x0020ACBC File Offset: 0x00208EBC
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
							bool flag2 = (relationFromTaiwu & 16384) != 0;
							if (flag2)
							{
								return true;
							}
							break;
						}
						case 1:
						{
							bool flag3 = (relationToTaiwu & 16384) != 0;
							if (flag3)
							{
								return true;
							}
							break;
						}
						case 2:
						{
							bool flag4 = (relationFromTaiwu & 16384) != 0 && (relationToTaiwu & 16384) != 0;
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

		// Token: 0x17000853 RID: 2131
		// (get) Token: 0x060044DC RID: 17628 RVA: 0x0020ADB8 File Offset: 0x00208FB8
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17000854 RID: 2132
		// (get) Token: 0x060044DD RID: 17629 RVA: 0x0020ADBB File Offset: 0x00208FBB
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(2, 0));
			}
		}
	}
}
