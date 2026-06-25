using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CFC RID: 3324
	public class RoleArrangementFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x17001182 RID: 4482
		// (get) Token: 0x0600A6C2 RID: 42690 RVA: 0x004D8661 File Offset: 0x004D6861
		public override int Id
		{
			get
			{
				return 9;
			}
		}

		// Token: 0x0600A6C3 RID: 42691 RVA: 0x004D8665 File Offset: 0x004D6865
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_VillagerRole_Title;
		}

		// Token: 0x0600A6C4 RID: 42692 RVA: 0x004D8674 File Offset: 0x004D6874
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> list = new List<FilterDropdownItemConfig>();
			foreach (VillagerRoleItem item in ((IEnumerable<VillagerRoleItem>)VillagerRole.Instance))
			{
				OrganizationMemberItem orgMemberCfg = OrganizationMember.Instance[item.OrganizationMember];
				list.Add(new FilterDropdownItemConfig
				{
					Text = orgMemberCfg.GradeName
				});
			}
			return list;
		}

		// Token: 0x0600A6C5 RID: 42693 RVA: 0x004D8700 File Offset: 0x004D6900
		public override bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices)
		{
			IVillagerSelectCharacterData villagerData = data as IVillagerSelectCharacterData;
			bool flag = villagerData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int roleId = villagerData.RoleTemplateId;
				List<VillagerRoleItem> roleCfgList = VillagerRole.Instance.ToList<VillagerRoleItem>();
				foreach (int index in selectedIndices)
				{
					bool flag2 = index >= 0 && index < roleCfgList.Count;
					if (flag2)
					{
						bool flag3 = (int)roleCfgList[index].TemplateId == roleId;
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
