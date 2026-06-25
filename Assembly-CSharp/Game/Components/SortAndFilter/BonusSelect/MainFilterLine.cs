using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E92 RID: 3730
	public class MainFilterLine : FilterToggleGroupLineLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x17001386 RID: 4998
		// (get) Token: 0x0600AD21 RID: 44321 RVA: 0x004F0C19 File Offset: 0x004EEE19
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AD22 RID: 44322 RVA: 0x004F0C1C File Offset: 0x004EEE1C
		public override bool IsDataMatch(SkillBreakBonusSelectableItem data, LineState lineState)
		{
			ToggleKey toggleState = lineState.ToggleGroupState;
			bool isAll = toggleState.IsAll;
			bool result;
			if (isAll)
			{
				result = true;
			}
			else
			{
				int index = toggleState.Index;
				if (!true)
				{
				}
				bool flag;
				switch (index)
				{
				case 0:
					flag = (data.Type == EBonusItemType.Exp);
					break;
				case 1:
					flag = (data.Type == EBonusItemType.Character);
					break;
				case 2:
					flag = (data.Type == EBonusItemType.Book);
					break;
				case 3:
					flag = (data.Type == EBonusItemType.Medicine);
					break;
				case 4:
					flag = (data.Type == EBonusItemType.Material);
					break;
				case 5:
					flag = (data.Type == EBonusItemType.Food);
					break;
				case 6:
					flag = (data.Type == EBonusItemType.BloodDew);
					break;
				default:
					flag = true;
					break;
				}
				if (!true)
				{
				}
				result = flag;
			}
			return result;
		}

		// Token: 0x0600AD23 RID: 44323 RVA: 0x004F0CD4 File Offset: 0x004EEED4
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			return new List<FilterToggleConfig>
			{
				new FilterToggleConfig("ui9_btn_filter_experience", StringKey.CreateKey("LK_Skill_Break_BonusSelect_TypeFilter_0")),
				new FilterToggleConfig("ui9_btn_filter_relationship", StringKey.CreateKey("LK_Skill_Break_BonusSelect_TypeFilter_1")),
				new FilterToggleConfig("ui9_btn_filter_book", StringKey.CreateKey("LK_Skill_Break_BonusSelect_TypeFilter_2")),
				new FilterToggleConfig("ui9_btn_filter_medicine", StringKey.CreateKey("LK_Skill_Break_BonusSelect_TypeFilter_3")),
				new FilterToggleConfig("ui9_btn_filter_material", StringKey.CreateKey("LK_Skill_Break_BonusSelect_TypeFilter_4")),
				new FilterToggleConfig("ui9_btn_filter_food", StringKey.CreateKey("LK_Skill_Break_BonusSelect_TypeFilter_5")),
				new FilterToggleConfig("ui9_btn_filter_blooddew", StringKey.CreateKey("LK_Skill_Break_BonusSelect_TypeFilter_6"))
			};
		}

		// Token: 0x0600AD24 RID: 44324 RVA: 0x004F0DA8 File Offset: 0x004EEFA8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x17001387 RID: 4999
		// (get) Token: 0x0600AD25 RID: 44325 RVA: 0x004F0DBB File Offset: 0x004EEFBB
		protected override int Level
		{
			get
			{
				return 0;
			}
		}
	}
}
