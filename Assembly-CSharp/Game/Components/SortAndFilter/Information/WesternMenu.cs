using System;
using System.Collections.Generic;
using Config;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E0C RID: 3596
	public class WesternMenu : DetailedFilterMenuLogic<InformationSortAndFilterData>
	{
		// Token: 0x170012E6 RID: 4838
		// (get) Token: 0x0600AB0C RID: 43788 RVA: 0x004EA078 File Offset: 0x004E8278
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012E7 RID: 4839
		// (get) Token: 0x0600AB0D RID: 43789 RVA: 0x004EA07B File Offset: 0x004E827B
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x0600AB0E RID: 43790 RVA: 0x004EA080 File Offset: 0x004E8280
		public override StringKey GetMenuBarLabel()
		{
			return InformationType.DefValue.Western.Name;
		}

		// Token: 0x0600AB0F RID: 43791 RVA: 0x004EA0A4 File Offset: 0x004E82A4
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			this._options.Clear();
			foreach (WesternRegionItem region in ((IEnumerable<WesternRegionItem>)WesternRegion.Instance))
			{
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = region.Name
				});
				this._options.Add(region.TemplateId);
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AB10 RID: 43792 RVA: 0x004EA13C File Offset: 0x004E833C
		public override bool IsDataMatch(InformationSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectionIndex in selectedIndices)
			{
				bool flag = this._options[selectionIndex] == data.WesternRegionTemplateId;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040084DE RID: 34014
		private readonly List<short> _options = new List<short>();
	}
}
