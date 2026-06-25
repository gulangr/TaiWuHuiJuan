using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E36 RID: 3638
	public class CharacterMenuItemFiveElementsMenu : DetailedFilterMenuLogic<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x17001319 RID: 4889
		// (get) Token: 0x0600ABB2 RID: 43954 RVA: 0x004EC3C4 File Offset: 0x004EA5C4
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700131A RID: 4890
		// (get) Token: 0x0600ABB3 RID: 43955 RVA: 0x004EC3C7 File Offset: 0x004EA5C7
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600ABB4 RID: 43956 RVA: 0x004EC3CC File Offset: 0x004EA5CC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_3;
		}

		// Token: 0x0600ABB5 RID: 43957 RVA: 0x004EC3E8 File Offset: 0x004EA5E8
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

		// Token: 0x0600ABB6 RID: 43958 RVA: 0x004EC460 File Offset: 0x004EA660
		public override bool IsDataMatch(CombatSkillDisplayDataCharacterMenuListItem data, IReadOnlyCollection<int> selectedIndices)
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

		// Token: 0x04008521 RID: 34081
		private readonly List<sbyte> _fiveElementsOptions = new List<sbyte>();
	}
}
