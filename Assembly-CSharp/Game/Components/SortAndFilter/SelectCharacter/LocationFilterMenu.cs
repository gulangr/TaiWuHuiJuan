using System;
using System.Collections.Generic;
using Config;
using Game.Views.Select;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CF0 RID: 3312
	public class LocationFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x17001176 RID: 4470
		// (get) Token: 0x0600A68D RID: 42637 RVA: 0x004D79AD File Offset: 0x004D5BAD
		public override int Id
		{
			get
			{
				return 16;
			}
		}

		// Token: 0x0600A68E RID: 42638 RVA: 0x004D79B1 File Offset: 0x004D5BB1
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_MerchantInfo_Location;
		}

		// Token: 0x0600A68F RID: 42639 RVA: 0x004D79C0 File Offset: 0x004D5BC0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> res = new List<FilterDropdownItemConfig>();
			foreach (MapStateItem item in ((IEnumerable<MapStateItem>)MapState.Instance))
			{
				res.Add(new FilterDropdownItemConfig
				{
					Text = item.Name
				});
			}
			return res;
		}

		// Token: 0x0600A690 RID: 42640 RVA: 0x004D7A38 File Offset: 0x004D5C38
		public override bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices)
		{
			BountyCharacterDataAdapter wrapper = data as BountyCharacterDataAdapter;
			bool flag = wrapper != null;
			if (flag)
			{
				foreach (int index in selectedIndices)
				{
					bool flag2 = (int)wrapper.Data.FullBlockName.stateTemplateId == index;
					if (flag2)
					{
						return true;
					}
				}
			}
			else
			{
				DirectSamsaraMotherCharacterDataAdapter directSamsaraWrapper = data as DirectSamsaraMotherCharacterDataAdapter;
				bool flag3 = directSamsaraWrapper != null;
				if (flag3)
				{
					foreach (int index2 in selectedIndices)
					{
						bool flag4 = (int)directSamsaraWrapper.Data.FullBlockName.stateTemplateId == index2;
						if (flag4)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
