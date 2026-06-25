using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E25 RID: 3621
	public class FiveElementsMenu : DetailedFilterMenuLogic<IFilterableCombatSkill>
	{
		// Token: 0x170012FE RID: 4862
		// (get) Token: 0x0600AB6C RID: 43884 RVA: 0x004EBAC4 File Offset: 0x004E9CC4
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012FF RID: 4863
		// (get) Token: 0x0600AB6D RID: 43885 RVA: 0x004EBAC7 File Offset: 0x004E9CC7
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600AB6E RID: 43886 RVA: 0x004EBACC File Offset: 0x004E9CCC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_3;
		}

		// Token: 0x0600AB6F RID: 43887 RVA: 0x004EBAE8 File Offset: 0x004E9CE8
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			this._fiveElementsOptions.Clear();
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			for (sbyte fiveElement = 0; fiveElement < 6; fiveElement += 1)
			{
				this._fiveElementsOptions.Add(fiveElement);
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", fiveElement))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AB70 RID: 43888 RVA: 0x004EBB60 File Offset: 0x004E9D60
		public override bool IsDataMatch(IFilterableCombatSkill data, IReadOnlyCollection<int> selectedIndices)
		{
			CombatSkillItem combatSkillConfig = CombatSkill.Instance[data.TemplateId];
			sbyte fiveElement = combatSkillConfig.FiveElements;
			foreach (int selectionIndex in selectedIndices)
			{
				sbyte selectionFiveElement = this._fiveElementsOptions[selectionIndex];
				bool flag = selectionFiveElement == fiveElement;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400850F RID: 34063
		private readonly List<sbyte> _fiveElementsOptions = new List<sbyte>();
	}
}
