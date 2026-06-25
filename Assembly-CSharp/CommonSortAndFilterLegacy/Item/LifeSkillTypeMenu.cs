using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000495 RID: 1173
	public class LifeSkillTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000706 RID: 1798
		// (get) Token: 0x06004154 RID: 16724 RVA: 0x0020142F File Offset: 0x001FF62F
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000707 RID: 1799
		// (get) Token: 0x06004155 RID: 16725 RVA: 0x00201432 File Offset: 0x001FF632
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004156 RID: 16726 RVA: 0x00201438 File Offset: 0x001FF638
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004157 RID: 16727 RVA: 0x0020145C File Offset: 0x001FF65C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from combatSkillTypeConfig in LifeSkillType.Instance
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = combatSkillTypeConfig.Icon,
				Text = combatSkillTypeConfig.Name
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004158 RID: 16728 RVA: 0x0020149C File Offset: 0x001FF69C
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			SkillBookItem bookConfig = SkillBook.Instance[data.Key.TemplateId];
			sbyte lifeSkillType = bookConfig.LifeSkillType;
			return (from index in selectedIndices
			select LifeSkillType.Instance[index]).Any((LifeSkillTypeItem lifeSkillTypeConfig) => lifeSkillType == lifeSkillTypeConfig.TemplateId);
		}
	}
}
