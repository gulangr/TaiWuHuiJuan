using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CF7 RID: 3319
	public class AdoreRelationFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x1700117A RID: 4474
		// (get) Token: 0x0600A6A4 RID: 42660 RVA: 0x004D7EF9 File Offset: 0x004D60F9
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x0600A6A5 RID: 42661 RVA: 0x004D7EFC File Offset: 0x004D60FC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_3;
		}

		// Token: 0x0600A6A6 RID: 42662 RVA: 0x004D7F08 File Offset: 0x004D6108
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < 3; i++)
			{
				configs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Character_Third_AdoredRelation_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x0600A6A7 RID: 42663 RVA: 0x004D7F64 File Offset: 0x004D6164
		public override bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices)
		{
			CharacterDisplayDataForGeneralScrollList generalData = base.GetGeneralData(data);
			bool flag = generalData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ushort relationFromTaiwu = generalData.RelationFromTaiwu;
				ushort relationToTaiwu = generalData.RelationToTaiwu;
				bool flag2 = selectedIndices == null || selectedIndices.Count == 0;
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = !generalData.IsInteractedWithTaiwu;
					if (flag3)
					{
						result = false;
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
									bool flag4 = (relationFromTaiwu & 16384) > 0;
									if (flag4)
									{
										return true;
									}
									break;
								}
								case 1:
								{
									bool flag5 = (relationToTaiwu & 16384) > 0;
									if (flag5)
									{
										return true;
									}
									break;
								}
								case 2:
								{
									bool flag6 = (relationFromTaiwu & 16384) != 0 && (relationToTaiwu & 16384) > 0;
									if (flag6)
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
				}
			}
			return result;
		}

		// Token: 0x1700117B RID: 4475
		// (get) Token: 0x0600A6A8 RID: 42664 RVA: 0x004D8088 File Offset: 0x004D6288
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(3, 0));
			}
		}
	}
}
