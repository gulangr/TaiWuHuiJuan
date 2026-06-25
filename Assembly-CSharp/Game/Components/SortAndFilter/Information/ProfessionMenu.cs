using System;
using System.Collections.Generic;
using Config;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E0E RID: 3598
	public class ProfessionMenu : DetailedFilterMenuLogic<InformationSortAndFilterData>
	{
		// Token: 0x170012EA RID: 4842
		// (get) Token: 0x0600AB18 RID: 43800 RVA: 0x004EA330 File Offset: 0x004E8530
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012EB RID: 4843
		// (get) Token: 0x0600AB19 RID: 43801 RVA: 0x004EA333 File Offset: 0x004E8533
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x0600AB1A RID: 43802 RVA: 0x004EA338 File Offset: 0x004E8538
		public override StringKey GetMenuBarLabel()
		{
			return InformationType.DefValue.Profession.Name;
		}

		// Token: 0x0600AB1B RID: 43803 RVA: 0x004EA35C File Offset: 0x004E855C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			this._options.Clear();
			foreach (ProfessionItem profession in ((IEnumerable<ProfessionItem>)Profession.Instance))
			{
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = profession.Name
				});
				this._options.Add(profession.TemplateId);
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AB1C RID: 43804 RVA: 0x004EA3F4 File Offset: 0x004E85F4
		public override bool IsDataMatch(InformationSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectionIndex in selectedIndices)
			{
				bool flag = this._options[selectionIndex] == data.Profession;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040084E0 RID: 34016
		private readonly List<int> _options = new List<int>();
	}
}
