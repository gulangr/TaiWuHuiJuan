using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CEA RID: 3306
	public class RoleArrangementWorkFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x1700116F RID: 4463
		// (get) Token: 0x0600A66D RID: 42605 RVA: 0x004D7241 File Offset: 0x004D5441
		public override int Id
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x0600A66E RID: 42606 RVA: 0x004D7245 File Offset: 0x004D5445
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_6;
		}

		// Token: 0x0600A66F RID: 42607 RVA: 0x004D7254 File Offset: 0x004D5454
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < 8; i++)
			{
				configs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Villager_Third_RoleArrangement_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x0600A670 RID: 42608 RVA: 0x004D72B0 File Offset: 0x004D54B0
		public override bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices)
		{
			IVillagerSelectCharacterData villagerData = data as IVillagerSelectCharacterData;
			bool flag = villagerData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte workType = villagerData.WorkType;
				int arrangementTemplateId = villagerData.ArrangementTemplateId;
				bool isBuyOperation = villagerData.IsBuyOperation;
				foreach (int index in selectedIndices)
				{
					bool flag2 = RoleArrangementWorkFilterMenu<TData>.IsRoleArrangementMatch(index, workType, arrangementTemplateId, isBuyOperation);
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600A671 RID: 42609 RVA: 0x004D734C File Offset: 0x004D554C
		private static bool IsRoleArrangementMatch(int index, sbyte workType, int arrangementTemplateId, bool isBuyOperation)
		{
			if (!true)
			{
			}
			bool result;
			switch (index)
			{
			case 0:
				result = (workType == 10);
				break;
			case 1:
				result = (workType == 14);
				break;
			case 2:
				result = (arrangementTemplateId == 6);
				break;
			case 3:
				result = (arrangementTemplateId == 8 && isBuyOperation);
				break;
			case 4:
				result = (arrangementTemplateId == 8 && !isBuyOperation);
				break;
			case 5:
				result = (arrangementTemplateId == 11);
				break;
			case 6:
				result = (arrangementTemplateId == 13);
				break;
			case 7:
				result = (arrangementTemplateId == 15);
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

		// Token: 0x17001170 RID: 4464
		// (get) Token: 0x0600A672 RID: 42610 RVA: 0x004D73D9 File Offset: 0x004D55D9
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(8, 2));
			}
		}
	}
}
