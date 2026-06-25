using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Villager
{
	// Token: 0x02000463 RID: 1123
	public class GenderMenu<T> : StaticDetailedFilterMenuBase<T> where T : IVillagerSortAndFilterData
	{
		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x06004081 RID: 16513 RVA: 0x001FFA9E File Offset: 0x001FDC9E
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004082 RID: 16514 RVA: 0x001FFAA4 File Offset: 0x001FDCA4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_0)
			};
		}

		// Token: 0x06004083 RID: 16515 RVA: 0x001FFAD0 File Offset: 0x001FDCD0
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from tuple in this._genderList
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = StringKey.CreateKey(tuple.Item2)
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004084 RID: 16516 RVA: 0x001FFB14 File Offset: 0x001FDD14
		public override bool IsDataMatch(T data, IReadOnlyCollection<int> selectedIndices)
		{
			sbyte dataGender = data.Gender;
			return selectedIndices.Any((int index) => this._genderList[index].Item1 == dataGender);
		}

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x06004085 RID: 16517 RVA: 0x001FFB58 File Offset: 0x001FDD58
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x04002E12 RID: 11794
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
