using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.CombatSkill;

namespace CommonSortAndFilterLegacy.CombatSkill
{
	// Token: 0x02000576 RID: 1398
	public class FiveElementsMenu : StaticDetailedFilterMenuBase<CombatSkillDisplayData>
	{
		// Token: 0x1700083C RID: 2108
		// (get) Token: 0x06004492 RID: 17554 RVA: 0x0020A1DC File Offset: 0x002083DC
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x1700083D RID: 2109
		// (get) Token: 0x06004493 RID: 17555 RVA: 0x0020A1DF File Offset: 0x002083DF
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06004494 RID: 17556 RVA: 0x0020A1E4 File Offset: 0x002083E4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_3);
		}

		// Token: 0x06004495 RID: 17557 RVA: 0x0020A208 File Offset: 0x00208408
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			this._fiveElementsOptions.Clear();
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (sbyte fiveElement = 0; fiveElement < 6; fiveElement += 1)
			{
				this._fiveElementsOptions.Add(fiveElement);
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", fiveElement))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x06004496 RID: 17558 RVA: 0x0020A288 File Offset: 0x00208488
		public override bool IsDataMatch(CombatSkillDisplayData data, IReadOnlyCollection<int> selectedIndices)
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

		// Token: 0x0400301A RID: 12314
		private readonly List<sbyte> _fiveElementsOptions = new List<sbyte>();
	}
}
