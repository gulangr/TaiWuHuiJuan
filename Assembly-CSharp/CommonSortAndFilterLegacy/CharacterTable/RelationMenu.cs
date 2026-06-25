using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x0200058A RID: 1418
	public class RelationMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x17000850 RID: 2128
		// (get) Token: 0x060044D0 RID: 17616 RVA: 0x0020AA68 File Offset: 0x00208C68
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060044D1 RID: 17617 RVA: 0x0020AA6C File Offset: 0x00208C6C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_2)
			};
		}

		// Token: 0x060044D2 RID: 17618 RVA: 0x0020AA98 File Offset: 0x00208C98
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

		// Token: 0x060044D3 RID: 17619 RVA: 0x0020AAFC File Offset: 0x00208CFC
		public override bool IsDataMatch(CharacterTableSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			ushort relationFromTaiwu = (ushort)data.Data.GetInt(105);
			ushort relationToTaiwu = (ushort)data.Data.GetInt(104);
			bool sameFactor = data.Data.GetInt(106) > 0;
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
					RelationMenu.EMenuOptions option = (RelationMenu.EMenuOptions)index;
					bool flag2 = option == RelationMenu.EMenuOptions.Faction;
					if (flag2)
					{
						bool flag3 = sameFactor;
						if (flag3)
						{
							return true;
						}
					}
					else
					{
						IEnumerable<ushort> relationTypes = RelationMenu.GetRelationTypesForOption(option);
						bool flag4 = RelationMenu.CheckRelationMatchAtLeastOne(relationToTaiwu, relationFromTaiwu, relationTypes);
						if (flag4)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060044D4 RID: 17620 RVA: 0x0020ABD4 File Offset: 0x00208DD4
		private static bool CheckRelationMatchAtLeastOne(ushort relationToTaiwu, ushort relationFromTaiwu, IEnumerable<ushort> relationTypes)
		{
			return relationTypes.Any((ushort relationType) => (relationToTaiwu & relationType) != 0 || (relationFromTaiwu & relationType) > 0);
		}

		// Token: 0x060044D5 RID: 17621 RVA: 0x0020AC0C File Offset: 0x00208E0C
		private static IEnumerable<ushort> GetRelationTypesForOption(RelationMenu.EMenuOptions option)
		{
			switch (option)
			{
			case RelationMenu.EMenuOptions.Adored:
				yield return 16384;
				break;
			case RelationMenu.EMenuOptions.Enemy:
				yield return 32768;
				break;
			case RelationMenu.EMenuOptions.Friend:
				yield return 8192;
				break;
			case RelationMenu.EMenuOptions.SwornBrotherOrSister:
				yield return 512;
				break;
			case RelationMenu.EMenuOptions.HusbandOrWife:
				yield return 1024;
				break;
			case RelationMenu.EMenuOptions.Child:
				yield return 2;
				yield return 128;
				yield return 16;
				break;
			case RelationMenu.EMenuOptions.Faction:
				throw new ArgumentOutOfRangeException("option");
			case RelationMenu.EMenuOptions.Parent:
				yield return 1;
				yield return 64;
				yield return 8;
				break;
			case RelationMenu.EMenuOptions.MentorOrMentee:
				yield return 2048;
				yield return 4096;
				break;
			case RelationMenu.EMenuOptions.BrotherOrSister:
				yield return 4;
				yield return 256;
				yield return 32;
				break;
			}
			yield break;
		}

		// Token: 0x17000851 RID: 2129
		// (get) Token: 0x060044D6 RID: 17622 RVA: 0x0020AC1C File Offset: 0x00208E1C
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x02001960 RID: 6496
		public enum EMenuOptions
		{
			// Token: 0x0400B227 RID: 45607
			Adored,
			// Token: 0x0400B228 RID: 45608
			Enemy,
			// Token: 0x0400B229 RID: 45609
			Friend,
			// Token: 0x0400B22A RID: 45610
			SwornBrotherOrSister,
			// Token: 0x0400B22B RID: 45611
			HusbandOrWife,
			// Token: 0x0400B22C RID: 45612
			Child,
			// Token: 0x0400B22D RID: 45613
			Faction,
			// Token: 0x0400B22E RID: 45614
			Parent,
			// Token: 0x0400B22F RID: 45615
			MentorOrMentee,
			// Token: 0x0400B230 RID: 45616
			BrotherOrSister,
			// Token: 0x0400B231 RID: 45617
			Count
		}
	}
}
