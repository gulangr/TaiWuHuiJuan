using System;
using System.Collections.Generic;
using Game.Views.CharacterMenu;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Relationship
{
	// Token: 0x02000CE7 RID: 3303
	public class RelationshipSortController : SortController<CharacterDisplayDataForRelations>
	{
		// Token: 0x0600A667 RID: 42599 RVA: 0x004D6F77 File Offset: 0x004D5177
		public RelationshipSortController(Func<int, bool> isTaiwuFunc, Func<int, bool> isSpecialTeammateFunc)
		{
			this._isTaiwuFunc = isTaiwuFunc;
			this._isSpecialTeammateFunc = isSpecialTeammateFunc;
		}

		// Token: 0x0600A668 RID: 42600 RVA: 0x004D6F90 File Offset: 0x004D5190
		public override Comparison<CharacterDisplayDataForRelations> GenerateComparer(SortStateData sortData)
		{
			return (CharacterDisplayDataForRelations x, CharacterDisplayDataForRelations y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600A669 RID: 42601 RVA: 0x004D6FC4 File Offset: 0x004D51C4
		private int CompareData(CharacterDisplayDataForRelations x, CharacterDisplayDataForRelations y, SortStateData sortData)
		{
			bool flag = x == null && y == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = x == null;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					bool flag3 = y == null;
					if (flag3)
					{
						result = -1;
					}
					else
					{
						CharacterDisplayDataForGeneralScrollList main = x.Main;
						int xCharId = (main != null) ? main.CharacterId : -1;
						CharacterDisplayDataForGeneralScrollList main2 = y.Main;
						int yCharId = (main2 != null) ? main2.CharacterId : -1;
						Func<int, bool> isTaiwuFunc = this._isTaiwuFunc;
						bool xIsTaiwu = isTaiwuFunc != null && isTaiwuFunc(xCharId);
						Func<int, bool> isTaiwuFunc2 = this._isTaiwuFunc;
						bool yIsTaiwu = isTaiwuFunc2 != null && isTaiwuFunc2(yCharId);
						bool flag4 = xIsTaiwu != yIsTaiwu;
						if (flag4)
						{
							result = (xIsTaiwu ? -1 : 1);
						}
						else
						{
							Func<int, bool> isSpecialTeammateFunc = this._isSpecialTeammateFunc;
							bool xIsSpecial = isSpecialTeammateFunc != null && isSpecialTeammateFunc(xCharId);
							Func<int, bool> isSpecialTeammateFunc2 = this._isSpecialTeammateFunc;
							bool yIsSpecial = isSpecialTeammateFunc2 != null && isSpecialTeammateFunc2(yCharId);
							bool flag5 = xIsSpecial != yIsSpecial;
							if (flag5)
							{
								result = (xIsSpecial ? -1 : 1);
							}
							else
							{
								bool flag6 = ((sortData != null) ? sortData.ItemStates : null) != null;
								if (flag6)
								{
									foreach (SortItemState itemState in sortData.ItemStates)
									{
										short sortId = itemState.SortId;
										ESortDirection order = itemState.SortDirection;
										int comparisonResult = CharacterDisplayDataForGeneralScrollListUtils.CompareBySortId(x.Main, y.Main, sortId);
										bool flag7 = comparisonResult != 0;
										if (flag7)
										{
											return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
										}
									}
								}
								result = xCharId.CompareTo(yCharId);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600A66A RID: 42602 RVA: 0x004D716C File Offset: 0x004D536C
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>();
			sortIds.AddRange(CharacterDisplayDataForGeneralScrollListUtils.SortIdsList);
			return new SortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = new List<int>(new int[sortIds.Count]),
				DefaultSortIds = new List<short>
				{
					0
				},
				DefaultSortState = new SortUiState
				{
					ItemStates = new List<SortItemState>
					{
						new SortItemState
						{
							SortId = 0,
							SortDirection = ESortDirection.Ascending
						}
					}
				}
			};
		}

		// Token: 0x04008316 RID: 33558
		private readonly Func<int, bool> _isTaiwuFunc;

		// Token: 0x04008317 RID: 33559
		private readonly Func<int, bool> _isSpecialTeammateFunc;
	}
}
