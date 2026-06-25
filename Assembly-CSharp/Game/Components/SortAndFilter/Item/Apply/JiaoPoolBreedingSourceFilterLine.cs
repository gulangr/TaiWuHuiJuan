using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item.Apply
{
	// Token: 0x02000DF4 RID: 3572
	internal sealed class JiaoPoolBreedingSourceFilterLine : FilterToggleGroupLineLogic<ITradeableContent>
	{
		// Token: 0x170012C2 RID: 4802
		// (get) Token: 0x0600AAA4 RID: 43684 RVA: 0x004E8EED File Offset: 0x004E70ED
		public override int Id
		{
			get
			{
				return 47;
			}
		}

		// Token: 0x170012C3 RID: 4803
		// (get) Token: 0x0600AAA5 RID: 43685 RVA: 0x004E8EF1 File Offset: 0x004E70F1
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AAA6 RID: 43686 RVA: 0x004E8EF4 File Offset: 0x004E70F4
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			return new List<FilterToggleConfig>
			{
				new FilterToggleConfig("ui9_btn_filter_misc", LanguageKey.LK_JiaoPool_OtherPool),
				new FilterToggleConfig("ui9_btn_filter_misc", LanguageKey.LK_Inventory),
				new FilterToggleConfig("ui9_btn_filter_misc", LanguageKey.LK_Warehouse),
				new FilterToggleConfig("ui9_btn_filter_misc", LanguageKey.LK_Treasury)
			};
		}

		// Token: 0x0600AAA7 RID: 43687 RVA: 0x004E8F72 File Offset: 0x004E7172
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0600AAA8 RID: 43688 RVA: 0x004E8F78 File Offset: 0x004E7178
		public override bool IsDataMatch(ITradeableContent data, LineState lineState)
		{
			ToggleKey toggleState = lineState.ToggleGroupState;
			ItemDisplayData itemDisplayData;
			bool flag;
			if (!toggleState.IsAll)
			{
				itemDisplayData = (data as ItemDisplayData);
				flag = (itemDisplayData == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				result = toggleState.IsAll;
			}
			else
			{
				int index = toggleState.Index;
				if (!true)
				{
				}
				bool flag3;
				switch (index)
				{
				case 0:
					flag3 = (itemDisplayData.ItemSourceType == 7);
					break;
				case 1:
					flag3 = (itemDisplayData.ItemSourceType == 1);
					break;
				case 2:
					flag3 = (itemDisplayData.ItemSourceType == 2);
					break;
				case 3:
					flag3 = (itemDisplayData.ItemSourceType == 3);
					break;
				default:
					flag3 = true;
					break;
				}
				if (!true)
				{
				}
				result = flag3;
			}
			return result;
		}
	}
}
