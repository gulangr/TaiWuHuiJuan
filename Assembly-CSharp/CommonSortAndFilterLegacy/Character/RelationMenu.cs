using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x020005A8 RID: 1448
	public class RelationMenu<T> : StaticDetailedFilterMenuBase<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x170008AC RID: 2220
		// (get) Token: 0x0600458B RID: 17803 RVA: 0x0020C56B File Offset: 0x0020A76B
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600458C RID: 17804 RVA: 0x0020C570 File Offset: 0x0020A770
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_2)
			};
		}

		// Token: 0x0600458D RID: 17805 RVA: 0x0020C59C File Offset: 0x0020A79C
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

		// Token: 0x0600458E RID: 17806 RVA: 0x0020C600 File Offset: 0x0020A800
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

		// Token: 0x0600458F RID: 17807 RVA: 0x0020C6CC File Offset: 0x0020A8CC
		private static bool CheckRelationMatchAtLeastOne(ushort relationToTaiwu, ushort relationFromTaiwu, IEnumerable<ushort> relationTypes)
		{
			return relationTypes.Any((ushort relationType) => (relationToTaiwu & relationType) != 0 || (relationFromTaiwu & relationType) > 0);
		}

		// Token: 0x06004590 RID: 17808 RVA: 0x0020C704 File Offset: 0x0020A904
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

		// Token: 0x170008AD RID: 2221
		// (get) Token: 0x06004591 RID: 17809 RVA: 0x0020C714 File Offset: 0x0020A914
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x02001976 RID: 6518
		public enum EMenuOptions
		{
			// Token: 0x0400B261 RID: 45665
			Adored,
			// Token: 0x0400B262 RID: 45666
			Enemy,
			// Token: 0x0400B263 RID: 45667
			Friend,
			// Token: 0x0400B264 RID: 45668
			SwornBrotherOrSister,
			// Token: 0x0400B265 RID: 45669
			HusbandOrWife,
			// Token: 0x0400B266 RID: 45670
			Child,
			// Token: 0x0400B267 RID: 45671
			Faction,
			// Token: 0x0400B268 RID: 45672
			Parent,
			// Token: 0x0400B269 RID: 45673
			MentorOrMentee,
			// Token: 0x0400B26A RID: 45674
			BrotherOrSister,
			// Token: 0x0400B26B RID: 45675
			Count
		}
	}
}
