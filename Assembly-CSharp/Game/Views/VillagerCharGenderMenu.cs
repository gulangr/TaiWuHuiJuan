using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using GameData.Domains.Character.Display;

namespace Game.Views
{
	// Token: 0x02000719 RID: 1817
	public class VillagerCharGenderMenu : DetailedFilterMenuLogic<VillagerCharDisplayData>
	{
		// Token: 0x17000A76 RID: 2678
		// (get) Token: 0x06005699 RID: 22169 RVA: 0x00281CFC File Offset: 0x0027FEFC
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17000A77 RID: 2679
		// (get) Token: 0x0600569A RID: 22170 RVA: 0x00281CFF File Offset: 0x0027FEFF
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600569B RID: 22171 RVA: 0x00281D04 File Offset: 0x0027FF04
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_0;
		}

		// Token: 0x0600569C RID: 22172 RVA: 0x00281D20 File Offset: 0x0027FF20
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			this._genders.Clear();
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			this._genders.Add(0);
			dropdownConfigs.Add(new FilterDropdownItemConfig
			{
				Text = LanguageKey.LK_Gender_Woman.Tr()
			});
			this._genders.Add(1);
			dropdownConfigs.Add(new FilterDropdownItemConfig
			{
				Text = LanguageKey.LK_Gender_Man.Tr()
			});
			return dropdownConfigs;
		}

		// Token: 0x0600569D RID: 22173 RVA: 0x00281DAC File Offset: 0x0027FFAC
		public override bool IsDataMatch(VillagerCharDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectionIndex in selectedIndices)
			{
				sbyte selectionFiveElement = this._genders[selectionIndex];
				bool flag = selectionFiveElement == data.Gender;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04003B2A RID: 15146
		private readonly List<sbyte> _genders = new List<sbyte>();
	}
}
