using System;
using System.Collections.Generic;
using System.Linq;
using Game.Views.SettlementPrison;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Prison
{
	// Token: 0x02000D12 RID: 3346
	public class OrganizationMenu : DynamicDetailedFilterMenuLogic<CharacterDisplayDataForSettlementPrisoner>
	{
		// Token: 0x1700118F RID: 4495
		// (get) Token: 0x0600A71D RID: 42781 RVA: 0x004DBD68 File Offset: 0x004D9F68
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001190 RID: 4496
		// (get) Token: 0x0600A71E RID: 42782 RVA: 0x004DBD6B File Offset: 0x004D9F6B
		public override int Id
		{
			get
			{
				return EMainMenuId.Organization.ToInt();
			}
		}

		// Token: 0x0600A71F RID: 42783 RVA: 0x004DBD78 File Offset: 0x004D9F78
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Prison_0;
		}

		// Token: 0x0600A720 RID: 42784 RVA: 0x004DBD94 File Offset: 0x004D9F94
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return null;
		}

		// Token: 0x0600A721 RID: 42785 RVA: 0x004DBDA8 File Offset: 0x004D9FA8
		public override List<FilterDropdownItemConfig> GetDynamicMenuConfigs(IEnumerable<CharacterDisplayDataForSettlementPrisoner> dataList)
		{
			this._organizationOptions.Clear();
			HashSet<string> nameSet = new HashSet<string>();
			foreach (CharacterDisplayDataForSettlementPrisoner prison in dataList)
			{
				nameSet.Add(ViewSettlementPrison.GetOrgName((short)prison.KidnapCharDisplayData.OrganizationInfo.OrgTemplateId, prison.RandomNameId));
			}
			this._organizationOptions.AddRange(nameSet);
			this._organizationOptions.Sort();
			return (from name in this._organizationOptions
			select new FilterDropdownItemConfig
			{
				Text = StringKey.CreateDirect(name)
			}).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600A722 RID: 42786 RVA: 0x004DBE70 File Offset: 0x004DA070
		public override bool IsDataMatch(CharacterDisplayDataForSettlementPrisoner data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this._organizationOptions[index] == ViewSettlementPrison.GetOrgName((short)data.KidnapCharDisplayData.OrganizationInfo.OrgTemplateId, data.RandomNameId));
		}

		// Token: 0x04008357 RID: 33623
		private readonly List<string> _organizationOptions = new List<string>();
	}
}
