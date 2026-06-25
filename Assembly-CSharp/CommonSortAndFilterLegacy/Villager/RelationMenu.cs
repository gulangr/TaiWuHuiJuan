using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Villager
{
	// Token: 0x02000469 RID: 1129
	public class RelationMenu<T> : StaticDetailedFilterMenuBase<T> where T : IVillagerSortAndFilterData
	{
		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x0600409C RID: 16540 RVA: 0x001FFBCA File Offset: 0x001FDDCA
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600409D RID: 16541 RVA: 0x001FFBD0 File Offset: 0x001FDDD0
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_2)
			};
		}

		// Token: 0x0600409E RID: 16542 RVA: 0x001FFBFC File Offset: 0x001FDDFC
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < 10; i++)
			{
				configs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Character_Third_Relation_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x0600409F RID: 16543 RVA: 0x001FFC60 File Offset: 0x001FDE60
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
				foreach (int index in selectedIndices)
				{
					RelationMenu<T>.EMenuOptions option = (RelationMenu<T>.EMenuOptions)index;
					bool flag2 = option == RelationMenu<T>.EMenuOptions.Faction;
					if (flag2)
					{
						bool isSameFactionWithTaiwu = data.IsSameFactionWithTaiwu;
						if (isSameFactionWithTaiwu)
						{
							return true;
						}
					}
					else
					{
						IEnumerable<ushort> relationTypes = RelationMenu<T>.GetRelationTypesForOption(option);
						bool flag3 = RelationMenu<T>.CheckRelationMatchAtLeastOne(relationToTaiwu, relationFromTaiwu, relationTypes);
						if (flag3)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060040A0 RID: 16544 RVA: 0x001FFD2C File Offset: 0x001FDF2C
		private static bool CheckRelationMatchAtLeastOne(ushort relationToTaiwu, ushort relationFromTaiwu, IEnumerable<ushort> relationTypes)
		{
			return relationTypes.Any((ushort relationType) => (relationToTaiwu & relationType) != 0 || (relationFromTaiwu & relationType) > 0);
		}

		// Token: 0x060040A1 RID: 16545 RVA: 0x001FFD64 File Offset: 0x001FDF64
		private static IEnumerable<ushort> GetRelationTypesForOption(RelationMenu<T>.EMenuOptions option)
		{
			switch (option)
			{
			case RelationMenu<T>.EMenuOptions.Adored:
				yield return 16384;
				break;
			case RelationMenu<T>.EMenuOptions.Enemy:
				yield return 32768;
				break;
			case RelationMenu<T>.EMenuOptions.Friend:
				yield return 8192;
				break;
			case RelationMenu<T>.EMenuOptions.SwornBrotherOrSister:
				yield return 512;
				break;
			case RelationMenu<T>.EMenuOptions.HusbandOrWife:
				yield return 1024;
				break;
			case RelationMenu<T>.EMenuOptions.Child:
				yield return 2;
				yield return 128;
				yield return 16;
				break;
			case RelationMenu<T>.EMenuOptions.Faction:
				throw new ArgumentOutOfRangeException("option");
			case RelationMenu<T>.EMenuOptions.Parent:
				yield return 1;
				yield return 64;
				yield return 8;
				break;
			case RelationMenu<T>.EMenuOptions.MentorOrMentee:
				yield return 2048;
				yield return 4096;
				break;
			case RelationMenu<T>.EMenuOptions.BrotherOrSister:
				yield return 4;
				yield return 256;
				yield return 32;
				break;
			}
			yield break;
		}

		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x060040A2 RID: 16546 RVA: 0x001FFD74 File Offset: 0x001FDF74
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x020018E9 RID: 6377
		public enum EMenuOptions
		{
			// Token: 0x0400B063 RID: 45155
			Adored,
			// Token: 0x0400B064 RID: 45156
			Enemy,
			// Token: 0x0400B065 RID: 45157
			Friend,
			// Token: 0x0400B066 RID: 45158
			SwornBrotherOrSister,
			// Token: 0x0400B067 RID: 45159
			HusbandOrWife,
			// Token: 0x0400B068 RID: 45160
			Child,
			// Token: 0x0400B069 RID: 45161
			Faction,
			// Token: 0x0400B06A RID: 45162
			Parent,
			// Token: 0x0400B06B RID: 45163
			MentorOrMentee,
			// Token: 0x0400B06C RID: 45164
			BrotherOrSister,
			// Token: 0x0400B06D RID: 45165
			Count
		}
	}
}
