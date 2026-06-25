using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200049A RID: 1178
	public class CombatSkillSectMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x06004167 RID: 16743 RVA: 0x00201651 File Offset: 0x001FF851
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x06004168 RID: 16744 RVA: 0x00201654 File Offset: 0x001FF854
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06004169 RID: 16745 RVA: 0x00201658 File Offset: 0x001FF858
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Sect);
		}

		// Token: 0x0600416A RID: 16746 RVA: 0x0020167C File Offset: 0x001FF87C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from organizationConfig in Organization.Instance
			where organizationConfig.IsSect || organizationConfig.TemplateId == 0
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = organizationConfig.Name
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x0600416B RID: 16747 RVA: 0x002016E0 File Offset: 0x001FF8E0
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
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
