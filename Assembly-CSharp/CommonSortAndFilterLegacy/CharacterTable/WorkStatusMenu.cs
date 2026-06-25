using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;
using GameData.Domains.Character.Display;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x0200058D RID: 1421
	public class WorkStatusMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x17000858 RID: 2136
		// (get) Token: 0x060044E6 RID: 17638 RVA: 0x0020AF7E File Offset: 0x0020917E
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060044E7 RID: 17639 RVA: 0x0020AF84 File Offset: 0x00209184
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_5)
			};
		}

		// Token: 0x060044E8 RID: 17640 RVA: 0x0020AFB0 File Offset: 0x002091B0
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < 5; i++)
			{
				configs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Villager_Third_WorkStatus_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x060044E9 RID: 17641 RVA: 0x0020B014 File Offset: 0x00209214
		public override bool IsDataMatch(CharacterTableSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => WorkStatusMenu.IsOptionMatch((EWorkStatusMenuOption)index, data));
		}

		// Token: 0x060044EA RID: 17642 RVA: 0x0020B048 File Offset: 0x00209248
		private static bool IsOptionMatch(EWorkStatusMenuOption option, CharacterTableSortAndFilterData data)
		{
			CharacterTableWorkData workData = data.Data.GetWork(99);
			if (!true)
			{
			}
			bool result;
			switch (option)
			{
			case EWorkStatusMenuOption.NoWork:
				result = (workData.WorkType == -1);
				break;
			case EWorkStatusMenuOption.ShopManage:
				result = (workData.WorkType == 1);
				break;
			case EWorkStatusMenuOption.RoleWork:
			{
				bool flag;
				if (workData.ArrangementTemplateId < 0)
				{
					sbyte workType = workData.WorkType;
					flag = (workType == 10 || workType == 14);
				}
				else
				{
					flag = true;
				}
				result = flag;
				break;
			}
			case EWorkStatusMenuOption.KeepGrave:
				result = (workData.WorkType == 12);
				break;
			case EWorkStatusMenuOption.Idle:
				result = (workData.WorkType == 13);
				break;
			default:
				result = false;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x17000859 RID: 2137
		// (get) Token: 0x060044EB RID: 17643 RVA: 0x0020B0EC File Offset: 0x002092EC
		public override int Id
		{
			get
			{
				return 5;
			}
		}
	}
}
