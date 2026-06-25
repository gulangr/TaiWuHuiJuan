using System;
using System.Collections.Generic;
using Config;
using Game.Views.Select;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CED RID: 3309
	public class PunishmentTypeFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x17001173 RID: 4467
		// (get) Token: 0x0600A67E RID: 42622 RVA: 0x004D7671 File Offset: 0x004D5871
		public override int Id
		{
			get
			{
				return 13;
			}
		}

		// Token: 0x0600A67F RID: 42623 RVA: 0x004D7675 File Offset: 0x004D5875
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_PunishmentType;
		}

		// Token: 0x0600A680 RID: 42624 RVA: 0x004D7684 File Offset: 0x004D5884
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> res = new List<FilterDropdownItemConfig>();
			foreach (PunishmentTypeItem item in ((IEnumerable<PunishmentTypeItem>)PunishmentType.Instance))
			{
				res.Add(new FilterDropdownItemConfig
				{
					Text = item.Name
				});
			}
			return res;
		}

		// Token: 0x0600A681 RID: 42625 RVA: 0x004D76FC File Offset: 0x004D58FC
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
					bool flag2 = (int)wrapper.Data.SettlementBounty.PunishmentType == index;
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
