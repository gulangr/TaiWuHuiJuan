using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x02000587 RID: 1415
	public class FallenMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x1700084A RID: 2122
		// (get) Token: 0x060044BD RID: 17597 RVA: 0x0020A744 File Offset: 0x00208944
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060044BE RID: 17598 RVA: 0x0020A748 File Offset: 0x00208948
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(StringKey.CreateKey(LanguageKey.LK_LegendaryBook_Button_Fallen));
		}

		// Token: 0x060044BF RID: 17599 RVA: 0x0020A76C File Offset: 0x0020896C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(LanguageKey.LK_LegendaryBook_Fallen_0_0)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(LanguageKey.LK_LegendaryBook_Fallen_0_1)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(LanguageKey.LK_LegendaryBook_Fallen_0_2)
				}
			};
		}

		// Token: 0x060044C0 RID: 17600 RVA: 0x0020A800 File Offset: 0x00208A00
		public override bool IsDataMatch(CharacterTableSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			int state = data.Data.GetInt(98);
			List<sbyte> states = this.GetSelectedStates(selectedIndices);
			return states.Contains((sbyte)state);
		}

		// Token: 0x060044C1 RID: 17601 RVA: 0x0020A830 File Offset: 0x00208A30
		private List<sbyte> GetSelectedStates(IReadOnlyCollection<int> selectedIndices)
		{
			List<sbyte> res = new List<sbyte>();
			foreach (int index in selectedIndices)
			{
				List<sbyte> list = res;
				if (!true)
				{
				}
				sbyte item;
				if (index != 0)
				{
					if (index != 1)
					{
						item = 3;
					}
					else
					{
						item = 2;
					}
				}
				else
				{
					item = 1;
				}
				if (!true)
				{
				}
				list.Add(item);
			}
			return res;
		}

		// Token: 0x1700084B RID: 2123
		// (get) Token: 0x060044C2 RID: 17602 RVA: 0x0020A8B0 File Offset: 0x00208AB0
		public override int Id
		{
			get
			{
				return 0;
			}
		}
	}
}
