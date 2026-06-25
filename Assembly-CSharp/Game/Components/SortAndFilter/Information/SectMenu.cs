using System;
using System.Collections.Generic;
using Config;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E0A RID: 3594
	public class SectMenu : DetailedFilterMenuLogic<InformationSortAndFilterData>
	{
		// Token: 0x170012E2 RID: 4834
		// (get) Token: 0x0600AB00 RID: 43776 RVA: 0x004E9DE4 File Offset: 0x004E7FE4
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012E3 RID: 4835
		// (get) Token: 0x0600AB01 RID: 43777 RVA: 0x004E9DE7 File Offset: 0x004E7FE7
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AB02 RID: 43778 RVA: 0x004E9DEC File Offset: 0x004E7FEC
		public override StringKey GetMenuBarLabel()
		{
			return InformationType.DefValue.Sect.Name;
		}

		// Token: 0x0600AB03 RID: 43779 RVA: 0x004E9E10 File Offset: 0x004E8010
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			this._options.Clear();
			foreach (OrganizationItem organization in ((IEnumerable<OrganizationItem>)Organization.Instance))
			{
				bool flag = organization.SettlementType != EOrganizationSettlementType.Sect;
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

		// Token: 0x0600AB04 RID: 43780 RVA: 0x004E9EBC File Offset: 0x004E80BC
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

		// Token: 0x040084DC RID: 34012
		private readonly List<sbyte> _options = new List<sbyte>();
	}
}
