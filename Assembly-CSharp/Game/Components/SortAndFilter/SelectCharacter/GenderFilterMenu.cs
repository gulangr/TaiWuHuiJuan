using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CF4 RID: 3316
	public class GenderFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x17001177 RID: 4471
		// (get) Token: 0x0600A694 RID: 42644 RVA: 0x004D7B64 File Offset: 0x004D5D64
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A695 RID: 42645 RVA: 0x004D7B68 File Offset: 0x004D5D68
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_0;
		}

		// Token: 0x0600A696 RID: 42646 RVA: 0x004D7B84 File Offset: 0x004D5D84
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

		// Token: 0x0600A697 RID: 42647 RVA: 0x004D7BE0 File Offset: 0x004D5DE0
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
				foreach (int index in selectedIndices)
				{
					int num = index;
					int num2 = num;
					if (num2 != 0)
					{
						if (num2 == 1)
						{
							bool flag2 = generalData.Gender == 0;
							if (flag2)
							{
								return true;
							}
						}
					}
					else
					{
						bool flag3 = generalData.Gender == 1;
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
