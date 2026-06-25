using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Character.Display;

namespace CommonSortAndFilterLegacy.Prison
{
	// Token: 0x02000474 RID: 1140
	public class OrganizationMenu : DynamicDetailedFilterMenuBase<CharacterDisplayDataForSettlementPrisoner>
	{
		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x060040D4 RID: 16596 RVA: 0x002003AC File Offset: 0x001FE5AC
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060040D5 RID: 16597 RVA: 0x002003B0 File Offset: 0x001FE5B0
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Prison_0));
		}

		// Token: 0x060040D6 RID: 16598 RVA: 0x002003D4 File Offset: 0x001FE5D4
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetDynamicMenuConfigs(List<CharacterDisplayDataForSettlementPrisoner> dataList)
		{
			this._organizationOptions.Clear();
			HashSet<string> nameSet = new HashSet<string>();
			foreach (CharacterDisplayDataForSettlementPrisoner prison in dataList)
			{
				nameSet.Add(OrganizationMenu.GetOrgName((short)prison.KidnapCharDisplayData.OrganizationInfo.OrgTemplateId, prison.RandomNameId));
			}
			this._organizationOptions.AddRange(nameSet);
			this._organizationOptions.Sort();
			return (from name in this._organizationOptions
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = StringKey.CreateDirect(name)
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x060040D7 RID: 16599 RVA: 0x002004A0 File Offset: 0x001FE6A0
		public override bool IsDataMatch(CharacterDisplayDataForSettlementPrisoner data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this._organizationOptions[index] == OrganizationMenu.GetOrgName((short)data.KidnapCharDisplayData.OrganizationInfo.OrgTemplateId, data.RandomNameId));
		}

		// Token: 0x060040D8 RID: 16600 RVA: 0x002004D8 File Offset: 0x001FE6D8
		private static string GetOrgName(short orgTemplateId, short randomNameId)
		{
			return (randomNameId >= 0) ? LocalTownNames.Instance.TownNameCore[(int)randomNameId].Name : Organization.Instance[(int)orgTemplateId].Name;
		}

		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x060040D9 RID: 16601 RVA: 0x00200511 File Offset: 0x001FE711
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x04002E39 RID: 11833
		private readonly List<string> _organizationOptions = new List<string>();
	}
}
