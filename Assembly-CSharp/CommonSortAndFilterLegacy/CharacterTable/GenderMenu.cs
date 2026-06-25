using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x02000588 RID: 1416
	public class GenderMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x1700084C RID: 2124
		// (get) Token: 0x060044C4 RID: 17604 RVA: 0x0020A8BC File Offset: 0x00208ABC
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060044C5 RID: 17605 RVA: 0x0020A8C0 File Offset: 0x00208AC0
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_0)
			};
		}

		// Token: 0x060044C6 RID: 17606 RVA: 0x0020A8EC File Offset: 0x00208AEC
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from tuple in this._genderList
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = StringKey.CreateKey(tuple.Item2)
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x060044C7 RID: 17607 RVA: 0x0020A930 File Offset: 0x00208B30
		public override bool IsDataMatch(CharacterTableSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			int dataGender = data.Data.GetInt(96);
			return selectedIndices.Any((int index) => (int)this._genderList[index].Item1 == dataGender);
		}

		// Token: 0x1700084D RID: 2125
		// (get) Token: 0x060044C8 RID: 17608 RVA: 0x0020A974 File Offset: 0x00208B74
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x04003050 RID: 12368
		[TupleElementNames(new string[]
		{
			"gender",
			"key"
		})]
		private readonly List<ValueTuple<sbyte, LanguageKey>> _genderList = new List<ValueTuple<sbyte, LanguageKey>>
		{
			new ValueTuple<sbyte, LanguageKey>(1, LanguageKey.LK_Gender_Man),
			new ValueTuple<sbyte, LanguageKey>(0, LanguageKey.LK_Gender_Woman)
		};
	}
}
