using System;
using System.Collections.Generic;
using Game.Views.Select;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CEF RID: 3311
	public class CaseStatusFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x17001175 RID: 4469
		// (get) Token: 0x0600A688 RID: 42632 RVA: 0x004D78A1 File Offset: 0x004D5AA1
		public override int Id
		{
			get
			{
				return 15;
			}
		}

		// Token: 0x0600A689 RID: 42633 RVA: 0x004D78A5 File Offset: 0x004D5AA5
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_HunterState;
		}

		// Token: 0x0600A68A RID: 42634 RVA: 0x004D78B4 File Offset: 0x004D5AB4
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(LanguageKey.LK_HunterState_NotInProgress),
				new FilterDropdownItemConfig(LanguageKey.LK_HunterState_Hunting),
				new FilterDropdownItemConfig(LanguageKey.LK_HunterState_Failed),
				new FilterDropdownItemConfig(LanguageKey.LK_HunterState_Escorting)
			};
		}

		// Token: 0x0600A68B RID: 42635 RVA: 0x004D7924 File Offset: 0x004D5B24
		public override bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices)
		{
			BountyCharacterDataAdapter wrapper = data as BountyCharacterDataAdapter;
			bool flag = wrapper == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (int index in selectedIndices)
				{
					bool flag2 = (int)wrapper.Data.HunterState == index;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}
	}
}
