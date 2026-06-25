using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CFB RID: 3323
	public class WorkStatusFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x17001181 RID: 4481
		// (get) Token: 0x0600A6BD RID: 42685 RVA: 0x004D84D5 File Offset: 0x004D66D5
		public override int Id
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x0600A6BE RID: 42686 RVA: 0x004D84D8 File Offset: 0x004D66D8
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_5;
		}

		// Token: 0x0600A6BF RID: 42687 RVA: 0x004D84E4 File Offset: 0x004D66E4
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < 5; i++)
			{
				configs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Villager_Third_WorkStatus_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x0600A6C0 RID: 42688 RVA: 0x004D8540 File Offset: 0x004D6740
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
				foreach (int index in selectedIndices)
				{
					EWorkStatusMenuOption option = (EWorkStatusMenuOption)index;
					bool flag2 = option == EWorkStatusMenuOption.NoWork && workType == -1;
					if (flag2)
					{
						return true;
					}
					bool flag3 = option == EWorkStatusMenuOption.ShopManage && workType == 1;
					if (flag3)
					{
						return true;
					}
					bool flag4 = option == EWorkStatusMenuOption.KeepGrave && workType == 12;
					if (flag4)
					{
						return true;
					}
					bool flag5 = option == EWorkStatusMenuOption.Idle && workType == 13;
					if (flag5)
					{
						return true;
					}
					bool flag6 = option == EWorkStatusMenuOption.RoleWork;
					if (flag6)
					{
						bool flag7 = arrangementTemplateId >= 0 || workType == 10 || workType == 14;
						if (flag7)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}
	}
}
