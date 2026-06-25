using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D8C RID: 3468
	public class FoodFilterLine : FilterToggleGroupLineLogic<ITradeableContent>
	{
		// Token: 0x1700124C RID: 4684
		// (get) Token: 0x0600A8EF RID: 43247 RVA: 0x004E22A1 File Offset: 0x004E04A1
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700124D RID: 4685
		// (get) Token: 0x0600A8F0 RID: 43248 RVA: 0x004E22A4 File Offset: 0x004E04A4
		protected override bool IndividualLine
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600A8F1 RID: 43249 RVA: 0x004E22A8 File Offset: 0x004E04A8
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			return new List<FilterToggleConfig>
			{
				new FilterToggleConfig("ui9_btn_filter_vegan", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Food_0),
				new FilterToggleConfig("ui9_btn_filter_meat", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Food_1),
				new FilterToggleConfig("ui9_btn_filter_tea", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Food_2),
				new FilterToggleConfig("ui9_btn_filter_wine", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Food_3)
			};
		}

		// Token: 0x0600A8F2 RID: 43250 RVA: 0x004E232C File Offset: 0x004E052C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x1700124E RID: 4686
		// (get) Token: 0x0600A8F3 RID: 43251 RVA: 0x004E233F File Offset: 0x004E053F
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A8F4 RID: 43252 RVA: 0x004E2344 File Offset: 0x004E0544
		public override bool IsDataMatch(ITradeableContent data, LineState lineState)
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
				short subType = ItemTemplateHelper.GetItemSubType(data.RealKey.ItemType, data.RealKey.TemplateId);
				int index = toggleState.Index;
				if (!true)
				{
				}
				bool flag;
				switch (index)
				{
				case 0:
					flag = (subType == 700);
					break;
				case 1:
					flag = (subType == 701);
					break;
				case 2:
					flag = (subType == 900);
					break;
				case 3:
					flag = (subType == 901);
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
	}
}
