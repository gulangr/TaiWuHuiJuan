using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D31 RID: 3377
	public class CombatSkillSectMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170011B1 RID: 4529
		// (get) Token: 0x0600A7A1 RID: 42913 RVA: 0x004DF659 File Offset: 0x004DD859
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170011B2 RID: 4530
		// (get) Token: 0x0600A7A2 RID: 42914 RVA: 0x004DF65C File Offset: 0x004DD85C
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A7A3 RID: 42915 RVA: 0x004DF660 File Offset: 0x004DD860
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Sect;
		}

		// Token: 0x0600A7A4 RID: 42916 RVA: 0x004DF67C File Offset: 0x004DD87C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return (from organizationConfig in Organization.Instance
			where organizationConfig.IsSect || organizationConfig.TemplateId == 0
			select new FilterDropdownItemConfig
			{
				Text = organizationConfig.Name
			}).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600A7A5 RID: 42917 RVA: 0x004DF6E0 File Offset: 0x004DD8E0
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			SkillBookItem bookConfig = SkillBook.Instance[data.Key.TemplateId];
			short combatSkillId = bookConfig.CombatSkillTemplateId;
			bool flag = combatSkillId < 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				CombatSkillItem combatSkillConfig = CombatSkill.Instance[combatSkillId];
				sbyte sectId = combatSkillConfig.SectId;
				result = (from index in selectedIndices
				select Organization.Instance[index]).Any((OrganizationItem organizationConfig) => organizationConfig.TemplateId == sectId);
			}
			return result;
		}
	}
}
