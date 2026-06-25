using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CF9 RID: 3321
	public class OrganizationFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x1700117E RID: 4478
		// (get) Token: 0x0600A6B0 RID: 42672 RVA: 0x004D8247 File Offset: 0x004D6447
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x0600A6B1 RID: 42673 RVA: 0x004D824A File Offset: 0x004D644A
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_5;
		}

		// Token: 0x0600A6B2 RID: 42674 RVA: 0x004D8258 File Offset: 0x004D6458
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i <= 6; i++)
			{
				configs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Character_Third_Organization_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x0600A6B3 RID: 42675 RVA: 0x004D82B4 File Offset: 0x004D64B4
		public override bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices)
		{
			CharacterDisplayDataForGeneralScrollList generalData = base.GetGeneralData(data);
			bool flag = generalData == null;
			return !flag && selectedIndices.Any((int index) => OrganizationFilterMenu<TData>.IsOrganizationMatch(generalData.OrgInfo.OrgTemplateId, (OrganizationMenuOption)index));
		}

		// Token: 0x0600A6B4 RID: 42676 RVA: 0x004D82FC File Offset: 0x004D64FC
		private static bool IsOrganizationMatch(sbyte orgTemplateId, OrganizationMenuOption option)
		{
			OrganizationMenuOption matchedOption = OrganizationFilterMenu<TData>.GetOrganizationOption(orgTemplateId);
			return matchedOption == option;
		}

		// Token: 0x0600A6B5 RID: 42677 RVA: 0x004D831C File Offset: 0x004D651C
		private static OrganizationMenuOption GetOrganizationOption(sbyte orgTemplateId)
		{
			bool flag = orgTemplateId == 16;
			OrganizationMenuOption result;
			if (flag)
			{
				result = OrganizationMenuOption.TaiwuVillage;
			}
			else
			{
				OrganizationItem organizationConfig = Organization.Instance[orgTemplateId];
				bool isSect = organizationConfig.IsSect;
				if (isSect)
				{
					result = OrganizationMenuOption.Sect;
				}
				else
				{
					bool flag2 = CommonUtils.IterCityOrganizationKeys().Contains(orgTemplateId);
					if (flag2)
					{
						result = OrganizationMenuOption.City;
					}
					else
					{
						bool flag3 = orgTemplateId == 37;
						if (flag3)
						{
							result = OrganizationMenuOption.Town;
						}
						else
						{
							bool flag4 = orgTemplateId == 38;
							if (flag4)
							{
								result = OrganizationMenuOption.WalledTown;
							}
							else
							{
								bool flag5 = orgTemplateId == 36;
								if (flag5)
								{
									result = OrganizationMenuOption.Village;
								}
								else
								{
									result = OrganizationMenuOption.Other;
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
