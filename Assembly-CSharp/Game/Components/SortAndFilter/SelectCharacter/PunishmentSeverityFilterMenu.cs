using System;
using System.Collections.Generic;
using Config;
using Game.Views.Select;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CEE RID: 3310
	public class PunishmentSeverityFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x17001174 RID: 4468
		// (get) Token: 0x0600A683 RID: 42627 RVA: 0x004D7789 File Offset: 0x004D5989
		public override int Id
		{
			get
			{
				return 14;
			}
		}

		// Token: 0x0600A684 RID: 42628 RVA: 0x004D778D File Offset: 0x004D598D
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_PunishmentSeverity;
		}

		// Token: 0x0600A685 RID: 42629 RVA: 0x004D779C File Offset: 0x004D599C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> res = new List<FilterDropdownItemConfig>();
			foreach (PunishmentSeverityItem item in ((IEnumerable<PunishmentSeverityItem>)PunishmentSeverity.Instance))
			{
				res.Add(new FilterDropdownItemConfig
				{
					Text = item.Name
				});
			}
			return res;
		}

		// Token: 0x0600A686 RID: 42630 RVA: 0x004D7814 File Offset: 0x004D5A14
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
					bool flag2 = (int)wrapper.Data.SettlementBounty.PunishmentSeverity == index;
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
