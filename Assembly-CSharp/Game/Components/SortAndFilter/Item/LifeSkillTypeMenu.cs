using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D32 RID: 3378
	public class LifeSkillTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170011B3 RID: 4531
		// (get) Token: 0x0600A7A7 RID: 42919 RVA: 0x004DF77D File Offset: 0x004DD97D
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170011B4 RID: 4532
		// (get) Token: 0x0600A7A8 RID: 42920 RVA: 0x004DF780 File Offset: 0x004DD980
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A7A9 RID: 42921 RVA: 0x004DF784 File Offset: 0x004DD984
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A7AA RID: 42922 RVA: 0x004DF7A0 File Offset: 0x004DD9A0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return (from config in LifeSkillType.Instance
			select new FilterDropdownItemConfig
			{
				Text = config.Name
			}).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600A7AB RID: 42923 RVA: 0x004DF7E0 File Offset: 0x004DD9E0
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			SkillBookItem bookConfig = SkillBook.Instance[data.Key.TemplateId];
			sbyte lifeSkillType = bookConfig.LifeSkillType;
			foreach (int index in selectedIndices)
			{
				bool flag = lifeSkillType == LifeSkillType.Instance[index].TemplateId;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}
	}
}
