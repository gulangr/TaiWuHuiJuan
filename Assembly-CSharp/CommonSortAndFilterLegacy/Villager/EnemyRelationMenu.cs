using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Villager
{
	// Token: 0x02000462 RID: 1122
	public class EnemyRelationMenu<T> : StaticDetailedFilterMenuBase<T> where T : IVillagerSortAndFilterData
	{
		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x0600407A RID: 16506 RVA: 0x001FF8F5 File Offset: 0x001FDAF5
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600407B RID: 16507 RVA: 0x001FF8F8 File Offset: 0x001FDAF8
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_4)
			};
		}

		// Token: 0x0600407C RID: 16508 RVA: 0x001FF924 File Offset: 0x001FDB24
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

		// Token: 0x0600407D RID: 16509 RVA: 0x001FF988 File Offset: 0x001FDB88
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

		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x0600407E RID: 16510 RVA: 0x001FFA84 File Offset: 0x001FDC84
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x0600407F RID: 16511 RVA: 0x001FFA87 File Offset: 0x001FDC87
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(2, 1));
			}
		}
	}
}
