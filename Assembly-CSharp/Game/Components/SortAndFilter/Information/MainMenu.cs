using System;
using System.Collections.Generic;
using Config;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E01 RID: 3585
	public class MainMenu : DetailedFilterMenuLogic<InformationSortAndFilterData>
	{
		// Token: 0x170012D5 RID: 4821
		// (get) Token: 0x0600AAE2 RID: 43746 RVA: 0x004E9A80 File Offset: 0x004E7C80
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012D6 RID: 4822
		// (get) Token: 0x0600AAE3 RID: 43747 RVA: 0x004E9A83 File Offset: 0x004E7C83
		public override int Id
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x0600AAE4 RID: 43748 RVA: 0x004E9A88 File Offset: 0x004E7C88
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_Filter;
		}

		// Token: 0x0600AAE5 RID: 43749 RVA: 0x004E9AA4 File Offset: 0x004E7CA4
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			this._options.Clear();
			foreach (InformationTypeItem config in ((IEnumerable<InformationTypeItem>)InformationType.Instance))
			{
				bool inUse = config.InUse;
				if (inUse)
				{
					dropdownConfigs.Add(new FilterDropdownItemConfig
					{
						Text = config.Name
					});
					this._options.Add(config.TemplateId);
				}
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AAE6 RID: 43750 RVA: 0x004E9B48 File Offset: 0x004E7D48
		public override bool IsDataMatch(InformationSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectionIndex in selectedIndices)
			{
				bool flag = this._options[selectionIndex] == Information.Instance[data.TemplateId].Type;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040084DA RID: 34010
		private readonly List<sbyte> _options = new List<sbyte>();
	}
}
