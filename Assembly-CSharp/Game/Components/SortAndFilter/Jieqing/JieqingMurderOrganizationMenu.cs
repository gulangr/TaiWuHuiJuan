using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Jieqing
{
	// Token: 0x02000D26 RID: 3366
	public class JieqingMurderOrganizationMenu : DynamicDetailedFilterMenuLogic<CharacterDisplayData>
	{
		// Token: 0x170011A0 RID: 4512
		// (get) Token: 0x0600A771 RID: 42865 RVA: 0x004DEFF4 File Offset: 0x004DD1F4
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170011A1 RID: 4513
		// (get) Token: 0x0600A772 RID: 42866 RVA: 0x004DEFF7 File Offset: 0x004DD1F7
		public override int Id
		{
			get
			{
				return EJieqingMurderMenuId.Organization.ToInt();
			}
		}

		// Token: 0x0600A773 RID: 42867 RVA: 0x004DF004 File Offset: 0x004DD204
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Prison_0;
		}

		// Token: 0x0600A774 RID: 42868 RVA: 0x004DF020 File Offset: 0x004DD220
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return null;
		}

		// Token: 0x0600A775 RID: 42869 RVA: 0x004DF034 File Offset: 0x004DD234
		public override List<FilterDropdownItemConfig> GetDynamicMenuConfigs(IEnumerable<CharacterDisplayData> dataList)
		{
			this._organizationOptions.Clear();
			HashSet<string> nameSet = new HashSet<string>();
			foreach (CharacterDisplayData prison in dataList)
			{
				nameSet.Add(JieqingMurderOrganizationMenu.GetOrgName((short)prison.OrgInfo.OrgTemplateId));
			}
			this._organizationOptions.AddRange(nameSet);
			this._organizationOptions.Sort();
			return (from name in this._organizationOptions
			select new FilterDropdownItemConfig
			{
				Text = StringKey.CreateDirect(name)
			}).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600A776 RID: 42870 RVA: 0x004DF0F0 File Offset: 0x004DD2F0
		public static string GetOrgName(short orgTemplateId)
		{
			return Organization.Instance[(int)orgTemplateId].Name;
		}

		// Token: 0x0600A777 RID: 42871 RVA: 0x004DF114 File Offset: 0x004DD314
		public override bool IsDataMatch(CharacterDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this._organizationOptions[index] == JieqingMurderOrganizationMenu.GetOrgName((short)data.OrgInfo.OrgTemplateId));
		}

		// Token: 0x0400836E RID: 33646
		private readonly List<string> _organizationOptions = new List<string>();
	}
}
