using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x0200059A RID: 1434
	public class EnemyRelationMenu<T> : StaticDetailedFilterMenuBase<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x0600453D RID: 17725 RVA: 0x0020BCEE File Offset: 0x00209EEE
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600453E RID: 17726 RVA: 0x0020BCF4 File Offset: 0x00209EF4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_4)
			};
		}

		// Token: 0x0600453F RID: 17727 RVA: 0x0020BD20 File Offset: 0x00209F20
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

		// Token: 0x06004540 RID: 17728 RVA: 0x0020BD84 File Offset: 0x00209F84
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
							bool flag2 = (relationFromTaiwu & 32768) > 0;
							if (flag2)
							{
								return true;
							}
							break;
						}
						case 1:
						{
							bool flag3 = (relationToTaiwu & 32768) > 0;
							if (flag3)
							{
								return true;
							}
							break;
						}
						case 2:
						{
							bool flag4 = (relationFromTaiwu & 32768) != 0 && (relationToTaiwu & 32768) > 0;
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

		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x06004541 RID: 17729 RVA: 0x0020BE80 File Offset: 0x0020A080
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000880 RID: 2176
		// (get) Token: 0x06004542 RID: 17730 RVA: 0x0020BE83 File Offset: 0x0020A083
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(2, 1));
			}
		}
	}
}
