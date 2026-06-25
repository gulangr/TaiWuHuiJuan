using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CF8 RID: 3320
	public class EnemyRelationFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x1700117C RID: 4476
		// (get) Token: 0x0600A6AA RID: 42666 RVA: 0x004D809F File Offset: 0x004D629F
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x0600A6AB RID: 42667 RVA: 0x004D80A2 File Offset: 0x004D62A2
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_4;
		}

		// Token: 0x0600A6AC RID: 42668 RVA: 0x004D80B0 File Offset: 0x004D62B0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < 3; i++)
			{
				configs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Character_Third_EnemyRelation_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x0600A6AD RID: 42669 RVA: 0x004D810C File Offset: 0x004D630C
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
									bool flag4 = (relationFromTaiwu & 32768) > 0;
									if (flag4)
									{
										return true;
									}
									break;
								}
								case 1:
								{
									bool flag5 = (relationToTaiwu & 32768) > 0;
									if (flag5)
									{
										return true;
									}
									break;
								}
								case 2:
								{
									bool flag6 = (relationFromTaiwu & 32768) != 0 && (relationToTaiwu & 32768) > 0;
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

		// Token: 0x1700117D RID: 4477
		// (get) Token: 0x0600A6AE RID: 42670 RVA: 0x004D8230 File Offset: 0x004D6430
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(3, 1));
			}
		}
	}
}
