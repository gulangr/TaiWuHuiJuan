using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D30 RID: 3376
	public class CombatSkillTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170011AF RID: 4527
		// (get) Token: 0x0600A79B RID: 42907 RVA: 0x004DF565 File Offset: 0x004DD765
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170011B0 RID: 4528
		// (get) Token: 0x0600A79C RID: 42908 RVA: 0x004DF568 File Offset: 0x004DD768
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A79D RID: 42909 RVA: 0x004DF56C File Offset: 0x004DD76C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A79E RID: 42910 RVA: 0x004DF588 File Offset: 0x004DD788
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return (from config in CombatSkillType.Instance
			select new FilterDropdownItemConfig
			{
				Text = config.Name
			}).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600A79F RID: 42911 RVA: 0x004DF5C8 File Offset: 0x004DD7C8
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			SkillBookItem bookConfig = SkillBook.Instance[data.Key.TemplateId];
			sbyte combatSkillType = bookConfig.CombatSkillType;
			foreach (int index in selectedIndices)
			{
				bool flag = combatSkillType == CombatSkillType.Instance[index].TemplateId;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}
	}
}
