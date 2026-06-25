using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000499 RID: 1177
	public class CombatSkillTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x06004161 RID: 16737 RVA: 0x0020156B File Offset: 0x001FF76B
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x06004162 RID: 16738 RVA: 0x0020156E File Offset: 0x001FF76E
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004163 RID: 16739 RVA: 0x00201574 File Offset: 0x001FF774
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004164 RID: 16740 RVA: 0x00201598 File Offset: 0x001FF798
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from combatSkillTypeConfig in CombatSkillType.Instance
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = combatSkillTypeConfig.Icon,
				Text = combatSkillTypeConfig.Name
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004165 RID: 16741 RVA: 0x002015D8 File Offset: 0x001FF7D8
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			SkillBookItem bookConfig = SkillBook.Instance[data.Key.TemplateId];
			sbyte combatSkillType = bookConfig.CombatSkillType;
			return (from index in selectedIndices
			select CombatSkillType.Instance[index]).Any((CombatSkillTypeItem combatSkillTypeConfig) => combatSkillType == combatSkillTypeConfig.TemplateId);
		}
	}
}
