using System;
using System.Collections.Generic;
using Config;
using Game.Components.SortAndFilter;
using GameData.Domains.Character.Display;

namespace Game.Views
{
	// Token: 0x0200071A RID: 1818
	public class VillagerCharBehaviorTypeMenu : DetailedFilterMenuLogic<VillagerCharDisplayData>
	{
		// Token: 0x17000A78 RID: 2680
		// (get) Token: 0x0600569F RID: 22175 RVA: 0x00281E2C File Offset: 0x0028002C
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17000A79 RID: 2681
		// (get) Token: 0x060056A0 RID: 22176 RVA: 0x00281E2F File Offset: 0x0028002F
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x060056A1 RID: 22177 RVA: 0x00281E34 File Offset: 0x00280034
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_1;
		}

		// Token: 0x060056A2 RID: 22178 RVA: 0x00281E50 File Offset: 0x00280050
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			this._cachedData.Clear();
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			sbyte i = 0;
			while ((int)i < BehaviorType.Instance.Count)
			{
				this._cachedData.Add(i);
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = BehaviorType.Instance[(short)i].Name
				});
				i += 1;
			}
			return dropdownConfigs;
		}

		// Token: 0x060056A3 RID: 22179 RVA: 0x00281ECC File Offset: 0x002800CC
		public override bool IsDataMatch(VillagerCharDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectionIndex in selectedIndices)
			{
				sbyte selectionFiveElement = this._cachedData[selectionIndex];
				bool flag = selectionFiveElement == data.BehaviorType;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04003B2B RID: 15147
		private readonly List<sbyte> _cachedData = new List<sbyte>();
	}
}
