using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x02000594 RID: 1428
	public class AdoredRelationMenu<T> : StaticDetailedFilterMenuBase<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x17000869 RID: 2153
		// (get) Token: 0x06004519 RID: 17689 RVA: 0x0020B879 File Offset: 0x00209A79
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600451A RID: 17690 RVA: 0x0020B87C File Offset: 0x00209A7C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_3)
			};
		}

		// Token: 0x0600451B RID: 17691 RVA: 0x0020B8A8 File Offset: 0x00209AA8
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

		// Token: 0x0600451C RID: 17692 RVA: 0x0020B90C File Offset: 0x00209B0C
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

		// Token: 0x1700086A RID: 2154
		// (get) Token: 0x0600451D RID: 17693 RVA: 0x0020BA08 File Offset: 0x00209C08
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700086B RID: 2155
		// (get) Token: 0x0600451E RID: 17694 RVA: 0x0020BA0B File Offset: 0x00209C0B
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(2, 0));
			}
		}
	}
}
