using System;
using System.Collections.Generic;
using GameData.Domains.Building;

namespace Game.Components.SortAndFilter.Recruit
{
	// Token: 0x02000D08 RID: 3336
	public class GenderFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : BuildingRecruitCharacterData
	{
		// Token: 0x17001188 RID: 4488
		// (get) Token: 0x0600A6F9 RID: 42745 RVA: 0x004DAD4C File Offset: 0x004D8F4C
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A6FA RID: 42746 RVA: 0x004DAD50 File Offset: 0x004D8F50
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_0;
		}

		// Token: 0x0600A6FB RID: 42747 RVA: 0x004DAD6C File Offset: 0x004D8F6C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_Gender_Man
				},
				new FilterDropdownItemConfig
				{
					Text = LanguageKey.LK_Gender_Woman
				}
			};
		}

		// Token: 0x0600A6FC RID: 42748 RVA: 0x004DADC8 File Offset: 0x004D8FC8
		public override bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices)
		{
			BuildingRecruitCharacterData generalData = base.GetGeneralData(data);
			bool flag = generalData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (int index in selectedIndices)
				{
					int num = index;
					int num2 = num;
					if (num2 != 0)
					{
						if (num2 == 1)
						{
							bool flag2 = generalData.CharacterData.Gender == 0;
							if (flag2)
							{
								return true;
							}
						}
					}
					else
					{
						bool flag3 = generalData.CharacterData.Gender == 1;
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
	}
}
