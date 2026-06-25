using System;
using System.Collections.Generic;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Recruit
{
	// Token: 0x02000D04 RID: 3332
	public class TaiwuFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : BuildingRecruitCharacterData
	{
		// Token: 0x17001187 RID: 4487
		// (get) Token: 0x0600A6F2 RID: 42738 RVA: 0x004DAB93 File Offset: 0x004D8D93
		public override int Id
		{
			get
			{
				return 12;
			}
		}

		// Token: 0x0600A6F3 RID: 42739 RVA: 0x004DAB97 File Offset: 0x004D8D97
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_UI_Following_SubTitle_List;
		}

		// Token: 0x0600A6F4 RID: 42740 RVA: 0x004DABA4 File Offset: 0x004D8DA4
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Common_All)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Villager_WorkStatus_InTaiwuGroup)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Villager)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Main_6)
				}
			};
		}

		// Token: 0x0600A6F5 RID: 42741 RVA: 0x004DAC40 File Offset: 0x004D8E40
		public override bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices)
		{
			ITaiwuSelectCharacterData wrapper = data as ITaiwuSelectCharacterData;
			bool flag = wrapper == null;
			bool result;
			if (flag)
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
						case 1:
						{
							bool isTaiwuTeammate = wrapper.IsTaiwuTeammate;
							if (isTaiwuTeammate)
							{
								return true;
							}
							break;
						}
						case 2:
						{
							bool isSameFactionWithTaiwu = wrapper.GetGeneralScrollListData().IsSameFactionWithTaiwu;
							if (isSameFactionWithTaiwu)
							{
								return true;
							}
							break;
						}
						case 3:
						{
							bool flag2 = !wrapper.IsTaiwuTeammate && !wrapper.GetGeneralScrollListData().IsSameFactionWithTaiwu;
							if (flag2)
							{
								return true;
							}
							break;
						}
						default:
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}
	}
}
