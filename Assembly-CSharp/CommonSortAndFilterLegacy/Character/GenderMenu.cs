using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x0200059B RID: 1435
	public class GenderMenu<T> : StaticDetailedFilterMenuBase<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x17000881 RID: 2177
		// (get) Token: 0x06004544 RID: 17732 RVA: 0x0020BE9A File Offset: 0x0020A09A
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004545 RID: 17733 RVA: 0x0020BEA0 File Offset: 0x0020A0A0
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_0)
			};
		}

		// Token: 0x06004546 RID: 17734 RVA: 0x0020BECC File Offset: 0x0020A0CC
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from tuple in this._genderList
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = StringKey.CreateKey(tuple.Item2)
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004547 RID: 17735 RVA: 0x0020BF10 File Offset: 0x0020A110
		public override bool IsDataMatch(T data, IReadOnlyCollection<int> selectedIndices)
		{
			sbyte dataGender = data.Gender;
			return selectedIndices.Any((int index) => this._genderList[index].Item1 == dataGender);
		}

		// Token: 0x17000882 RID: 2178
		// (get) Token: 0x06004548 RID: 17736 RVA: 0x0020BF54 File Offset: 0x0020A154
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x04003054 RID: 12372
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
