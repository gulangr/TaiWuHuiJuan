using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x020005A1 RID: 1441
	public class OrganizationMenu<T> : StaticDetailedFilterMenuBase<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x17000896 RID: 2198
		// (get) Token: 0x06004564 RID: 17764 RVA: 0x0020C0DE File Offset: 0x0020A2DE
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004565 RID: 17765 RVA: 0x0020C0E4 File Offset: 0x0020A2E4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_5)
			};
		}

		// Token: 0x06004566 RID: 17766 RVA: 0x0020C110 File Offset: 0x0020A310
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < 7; i++)
			{
				configs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Character_Third_Organization_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x06004567 RID: 17767 RVA: 0x0020C174 File Offset: 0x0020A374
		public override bool IsDataMatch(T data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => OrganizationMenu<T>.IsOrganizationMatch(data.Organization.OrgTemplateId, (EOrganizationMenuOption)index));
		}

		// Token: 0x06004568 RID: 17768 RVA: 0x0020C1A8 File Offset: 0x0020A3A8
		private static bool IsOrganizationMatch(sbyte dataOrganization, EOrganizationMenuOption index)
		{
			EOrganizationMenuOption dataOption = OrganizationMenu<T>.GetDataMatchedOption(dataOrganization);
			return dataOption == index;
		}

		// Token: 0x06004569 RID: 17769 RVA: 0x0020C1C8 File Offset: 0x0020A3C8
		private static EOrganizationMenuOption GetDataMatchedOption(sbyte dataOrganization)
		{
			bool flag = dataOrganization == 16;
			EOrganizationMenuOption result;
			if (flag)
			{
				result = EOrganizationMenuOption.TaiwuVillage;
			}
			else
			{
				OrganizationItem organizationConfig = Organization.Instance[dataOrganization];
				bool isSect = organizationConfig.IsSect;
				if (isSect)
				{
					result = EOrganizationMenuOption.Sect;
				}
				else
				{
					bool flag2 = CommonUtils.IterCityOrganizationKeys().Contains(dataOrganization);
					if (flag2)
					{
						result = EOrganizationMenuOption.City;
					}
					else
					{
						bool flag3 = dataOrganization == 37;
						if (flag3)
						{
							result = EOrganizationMenuOption.Town;
						}
						else
						{
							bool flag4 = dataOrganization == 38;
							if (flag4)
							{
								result = EOrganizationMenuOption.WalledTown;
							}
							else
							{
								bool flag5 = dataOrganization == 36;
								if (flag5)
								{
									result = EOrganizationMenuOption.Village;
								}
								else
								{
									result = EOrganizationMenuOption.Other;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x17000897 RID: 2199
		// (get) Token: 0x0600456A RID: 17770 RVA: 0x0020C244 File Offset: 0x0020A444
		public override int Id
		{
			get
			{
				return 5;
			}
		}
	}
}
