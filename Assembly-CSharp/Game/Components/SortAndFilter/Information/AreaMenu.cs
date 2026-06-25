using System;
using System.Collections.Generic;
using Config;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E09 RID: 3593
	public class AreaMenu : DetailedFilterMenuLogic<InformationSortAndFilterData>
	{
		// Token: 0x170012E0 RID: 4832
		// (get) Token: 0x0600AAFA RID: 43770 RVA: 0x004E9C90 File Offset: 0x004E7E90
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012E1 RID: 4833
		// (get) Token: 0x0600AAFB RID: 43771 RVA: 0x004E9C93 File Offset: 0x004E7E93
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AAFC RID: 43772 RVA: 0x004E9C98 File Offset: 0x004E7E98
		public override StringKey GetMenuBarLabel()
		{
			return InformationType.DefValue.Area.Name;
		}

		// Token: 0x0600AAFD RID: 43773 RVA: 0x004E9CBC File Offset: 0x004E7EBC
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			this._options.Clear();
			foreach (OrganizationItem organization in ((IEnumerable<OrganizationItem>)Organization.Instance))
			{
				bool flag = organization.SettlementType != EOrganizationSettlementType.City;
				if (!flag)
				{
					dropdownConfigs.Add(new FilterDropdownItemConfig
					{
						Text = organization.Name
					});
					this._options.Add(organization.TemplateId);
				}
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AAFE RID: 43774 RVA: 0x004E9D68 File Offset: 0x004E7F68
		public override bool IsDataMatch(InformationSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectionIndex in selectedIndices)
			{
				bool flag = this._options[selectionIndex] == data.OrganizationTemplateId;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040084DB RID: 34011
		private readonly List<sbyte> _options = new List<sbyte>();
	}
}
