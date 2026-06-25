using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;
using UICommon.Character.Elements;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005BB RID: 1467
	public class MainFilterLine : FilterToggleGroupLine<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008C7 RID: 2247
		// (get) Token: 0x060045E7 RID: 17895 RVA: 0x0020D76D File Offset: 0x0020B96D
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060045E8 RID: 17896 RVA: 0x0020D770 File Offset: 0x0020B970
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

		// Token: 0x060045E9 RID: 17897 RVA: 0x0020D828 File Offset: 0x0020BA28
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			List<FilterToggleConfig> list = new List<FilterToggleConfig>();
			for (EBonusItemType i = EBonusItemType.Exp; i <= EBonusItemType.BloodDew; i += 1)
			{
				list.Add(new FilterToggleConfig(ToggleTransitionIconSpriteNames.Default(), StringKey.CreateKey(string.Format("LK_Skill_Break_BonusSelect_TypeFilter_{0}", (int)i))));
			}
			return list;
		}

		// Token: 0x060045EA RID: 17898 RVA: 0x0020D880 File Offset: 0x0020BA80
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x170008C8 RID: 2248
		// (get) Token: 0x060045EB RID: 17899 RVA: 0x0020D893 File Offset: 0x0020BA93
		protected override int Level
		{
			get
			{
				return 0;
			}
		}
	}
}
