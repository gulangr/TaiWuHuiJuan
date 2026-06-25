using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CF6 RID: 3318
	public class RelationFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x17001179 RID: 4473
		// (get) Token: 0x0600A69E RID: 42654 RVA: 0x004D7D3D File Offset: 0x004D5F3D
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x0600A69F RID: 42655 RVA: 0x004D7D40 File Offset: 0x004D5F40
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_2;
		}

		// Token: 0x0600A6A0 RID: 42656 RVA: 0x004D7D5C File Offset: 0x004D5F5C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i <= 9; i++)
			{
				configs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Character_Third_Relation_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x0600A6A1 RID: 42657 RVA: 0x004D7DBC File Offset: 0x004D5FBC
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
				bool isSameFaction = generalData.IsSameFactionWithTaiwu;
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
						Func<ushort, bool> <>9__0;
						foreach (int index in selectedIndices)
						{
							RelationFilterMenu<TData>.RelationMenuOption option = (RelationFilterMenu<TData>.RelationMenuOption)index;
							bool flag4 = option == RelationFilterMenu<TData>.RelationMenuOption.Faction;
							if (flag4)
							{
								bool flag5 = isSameFaction;
								if (flag5)
								{
									return true;
								}
							}
							else
							{
								IEnumerable<ushort> relationTypes = RelationFilterMenu<TData>.GetRelationTypesForOption(option);
								IEnumerable<ushort> source = relationTypes;
								Func<ushort, bool> predicate;
								if ((predicate = <>9__0) == null)
								{
									predicate = (<>9__0 = ((ushort relationType) => (relationToTaiwu & relationType) != 0 || (relationFromTaiwu & relationType) > 0));
								}
								bool flag6 = source.Any(predicate);
								if (flag6)
								{
									return true;
								}
							}
						}
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x0600A6A2 RID: 42658 RVA: 0x004D7EE0 File Offset: 0x004D60E0
		private static IEnumerable<ushort> GetRelationTypesForOption(RelationFilterMenu<TData>.RelationMenuOption option)
		{
			switch (option)
			{
			case RelationFilterMenu<TData>.RelationMenuOption.Adored:
				yield return 16384;
				break;
			case RelationFilterMenu<TData>.RelationMenuOption.Enemy:
				yield return 32768;
				break;
			case RelationFilterMenu<TData>.RelationMenuOption.Friend:
				yield return 8192;
				break;
			case RelationFilterMenu<TData>.RelationMenuOption.SwornBrotherOrSister:
				yield return 512;
				break;
			case RelationFilterMenu<TData>.RelationMenuOption.HusbandOrWife:
				yield return 1024;
				break;
			case RelationFilterMenu<TData>.RelationMenuOption.Child:
				yield return 2;
				yield return 128;
				yield return 16;
				break;
			case RelationFilterMenu<TData>.RelationMenuOption.Parent:
				yield return 1;
				yield return 64;
				yield return 8;
				break;
			case RelationFilterMenu<TData>.RelationMenuOption.MentorOrMentee:
				yield return 2048;
				yield return 4096;
				break;
			case RelationFilterMenu<TData>.RelationMenuOption.BrotherOrSister:
				yield return 4;
				yield return 256;
				yield return 32;
				break;
			}
			yield break;
		}

		// Token: 0x0200242C RID: 9260
		public enum RelationMenuOption
		{
			// Token: 0x0400E1EE RID: 57838
			Adored,
			// Token: 0x0400E1EF RID: 57839
			Enemy,
			// Token: 0x0400E1F0 RID: 57840
			Friend,
			// Token: 0x0400E1F1 RID: 57841
			SwornBrotherOrSister,
			// Token: 0x0400E1F2 RID: 57842
			HusbandOrWife,
			// Token: 0x0400E1F3 RID: 57843
			Child,
			// Token: 0x0400E1F4 RID: 57844
			Faction,
			// Token: 0x0400E1F5 RID: 57845
			Parent,
			// Token: 0x0400E1F6 RID: 57846
			MentorOrMentee,
			// Token: 0x0400E1F7 RID: 57847
			BrotherOrSister
		}
	}
}
