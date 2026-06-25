using System;
using System.Collections.Generic;
using Config;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E0B RID: 3595
	public class LifeSkillMenu : DetailedFilterMenuLogic<InformationSortAndFilterData>
	{
		// Token: 0x170012E4 RID: 4836
		// (get) Token: 0x0600AB06 RID: 43782 RVA: 0x004E9F38 File Offset: 0x004E8138
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012E5 RID: 4837
		// (get) Token: 0x0600AB07 RID: 43783 RVA: 0x004E9F3B File Offset: 0x004E813B
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600AB08 RID: 43784 RVA: 0x004E9F40 File Offset: 0x004E8140
		public override StringKey GetMenuBarLabel()
		{
			return InformationType.DefValue.LifeSkill.Name;
		}

		// Token: 0x0600AB09 RID: 43785 RVA: 0x004E9F64 File Offset: 0x004E8164
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			this._options.Clear();
			foreach (LifeSkillTypeItem lifeSkill in ((IEnumerable<LifeSkillTypeItem>)LifeSkillType.Instance))
			{
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = lifeSkill.Name
				});
				this._options.Add(lifeSkill.TemplateId);
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AB0A RID: 43786 RVA: 0x004E9FFC File Offset: 0x004E81FC
		public override bool IsDataMatch(InformationSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectionIndex in selectedIndices)
			{
				bool flag = this._options[selectionIndex] == data.LifeSkillType;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040084DD RID: 34013
		private readonly List<sbyte> _options = new List<sbyte>();
	}
}
