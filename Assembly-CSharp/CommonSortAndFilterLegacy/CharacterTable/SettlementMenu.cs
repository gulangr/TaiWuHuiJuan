using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Character;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x02000590 RID: 1424
	public class SettlementMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x1700085F RID: 2143
		// (get) Token: 0x060044FB RID: 17659 RVA: 0x0020B39D File Offset: 0x0020959D
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060044FC RID: 17660 RVA: 0x0020B3A0 File Offset: 0x002095A0
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CharacterTableFilter_Settlement_Title)
			};
		}

		// Token: 0x060044FD RID: 17661 RVA: 0x0020B3CC File Offset: 0x002095CC
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < Enum.GetValues(typeof(ESettlementMenuOption)).Length; i++)
			{
				configs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(string.Format("LK_CharacterTableFilter_Settlement_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x060044FE RID: 17662 RVA: 0x0020B440 File Offset: 0x00209640
		public override bool IsDataMatch(CharacterTableSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => SettlementMenu.IsSettlementTypeMatch(data, (ESettlementMenuOption)index));
		}

		// Token: 0x060044FF RID: 17663 RVA: 0x0020B474 File Offset: 0x00209674
		private static bool IsSettlementTypeMatch(CharacterTableSortAndFilterData data, ESettlementMenuOption option)
		{
			OrganizationInfo orgInfo = data.Data.GetOrg(92);
			OrganizationItem config = Organization.Instance[orgInfo.OrgTemplateId];
			if (!true)
			{
			}
			bool result;
			switch (option)
			{
			case ESettlementMenuOption.Taiwu:
				result = (config.SettlementType == EOrganizationSettlementType.TaiwuVillage);
				break;
			case ESettlementMenuOption.Sect:
				result = (config.SettlementType == EOrganizationSettlementType.Sect);
				break;
			case ESettlementMenuOption.City:
				result = (config.SettlementType == EOrganizationSettlementType.City);
				break;
			case ESettlementMenuOption.Town:
				result = (config.SettlementType == EOrganizationSettlementType.Town);
				break;
			case ESettlementMenuOption.WalledTown:
				result = (config.SettlementType == EOrganizationSettlementType.WalledTown);
				break;
			case ESettlementMenuOption.Village:
				result = (config.SettlementType == EOrganizationSettlementType.Village);
				break;
			case ESettlementMenuOption.Other:
				result = (config.SettlementType == EOrganizationSettlementType.Invalid);
				break;
			default:
				result = false;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x06004500 RID: 17664 RVA: 0x0020B52A File Offset: 0x0020972A
		public override int Id
		{
			get
			{
				return 8;
			}
		}
	}
}
