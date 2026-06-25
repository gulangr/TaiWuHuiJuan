using System;
using System.Collections.Generic;
using FrameWork;
using Game.Components.SortAndFilter.Item;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E8C RID: 3724
	public class BonusSelectSortController : SortController<SkillBreakBonusSelectableItem>
	{
		// Token: 0x0600AD02 RID: 44290 RVA: 0x004F0200 File Offset: 0x004EE400
		public override Comparison<SkillBreakBonusSelectableItem> GenerateComparer(SortStateData sortData)
		{
			return (SkillBreakBonusSelectableItem x, SkillBreakBonusSelectableItem y) => BonusSelectSortController.CompareData(x, y, sortData);
		}

		// Token: 0x0600AD03 RID: 44291 RVA: 0x004F022C File Offset: 0x004EE42C
		private static int CompareData(SkillBreakBonusSelectableItem x, SkillBreakBonusSelectableItem y, SortStateData sortData)
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
						bool flag4 = x.Type != y.Type;
						if (flag4)
						{
							result = x.Type.CompareTo(y.Type);
						}
						else
						{
							ESortDirection lastOrder = ESortDirection.Ascending;
							bool flag5 = ((sortData != null) ? sortData.ItemStates : null) != null;
							int num3;
							if (flag5)
							{
								foreach (SortItemState itemState in sortData.ItemStates)
								{
									short sortId = itemState.SortId;
									ESortDirection order = itemState.SortDirection;
									lastOrder = order;
									short num = sortId;
									short num2 = num;
									int comparisonResult;
									if (num2 <= 11)
									{
										switch (num2)
										{
										case 0:
											comparisonResult = BonusSelectSortController.CompareByName(x, y);
											break;
										case 1:
											comparisonResult = BonusSelectSortController.CompareByCharGrade(x, y);
											break;
										case 2:
										case 3:
										case 4:
											goto IL_26B;
										case 5:
											comparisonResult = BonusSelectSortController.CompareNullableData<ItemDisplayData, long>(x.ItemDisplayData, y.ItemDisplayData, (ItemDisplayData data) => data.Value);
											break;
										case 6:
											comparisonResult = BonusSelectSortController.CompareNullableData<ItemDisplayData, int>(x.ItemDisplayData, y.ItemDisplayData, (ItemDisplayData data) => data.Weight);
											break;
										default:
											if (num2 != 11)
											{
												goto IL_26B;
											}
											comparisonResult = BonusSelectSortController.CompareNullableData<CharacterDisplayData, short>(x.CharacterDisplayData, y.CharacterDisplayData, (CharacterDisplayData data) => data.FavorabilityToTaiwu);
											break;
										}
									}
									else if (num2 != 17)
									{
										if (num2 != 56)
										{
											if (num2 != 121)
											{
												goto IL_26B;
											}
											num3 = x.BonusData.FriendAttainment;
											comparisonResult = num3.CompareTo(y.BonusData.FriendAttainment);
										}
										else
										{
											comparisonResult = BonusSelectSortController.CompareNullableData<ItemDisplayData, sbyte>(x.ItemDisplayData, y.ItemDisplayData, (ItemDisplayData data) => data.Key.ItemType);
										}
									}
									else
									{
										comparisonResult = BonusSelectSortController.CompareNullableData<ItemDisplayData, int>(x.ItemDisplayData, y.ItemDisplayData, (ItemDisplayData data) => data.Amount);
									}
									IL_270:
									bool flag6 = comparisonResult != 0;
									if (flag6)
									{
										return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
									}
									continue;
									IL_26B:
									comparisonResult = 0;
									goto IL_270;
								}
							}
							ItemDisplayData itemDisplayData = x.ItemDisplayData;
							num3 = ((itemDisplayData != null) ? itemDisplayData.Key.Id : 0);
							ItemDisplayData itemDisplayData2 = y.ItemDisplayData;
							int idCompare = num3.CompareTo((itemDisplayData2 != null) ? itemDisplayData2.Key.Id : 0);
							result = ((lastOrder == ESortDirection.Ascending) ? idCompare : (-idCompare));
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600AD04 RID: 44292 RVA: 0x004F0544 File Offset: 0x004EE744
		private static int CompareNullableData<TSource, TValue>(TSource x, TSource y, Func<TSource, TValue> selector) where TSource : class where TValue : IComparable<TValue>
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
						TValue tvalue = selector(x);
						result = tvalue.CompareTo(selector(y));
					}
				}
			}
			return result;
		}

		// Token: 0x0600AD05 RID: 44293 RVA: 0x004F05B4 File Offset: 0x004EE7B4
		private static int CompareByName(SkillBreakBonusSelectableItem x, SkillBreakBonusSelectableItem y)
		{
			EBonusItemType type = x.Type;
			EBonusItemType ebonusItemType = type;
			int result;
			if (ebonusItemType != EBonusItemType.Exp)
			{
				if (ebonusItemType != EBonusItemType.Character)
				{
					bool flag = x.ItemDisplayData == null && y.ItemDisplayData == null;
					if (flag)
					{
						result = 0;
					}
					else
					{
						bool flag2 = x.ItemDisplayData == null;
						if (flag2)
						{
							result = 1;
						}
						else
						{
							bool flag3 = y.ItemDisplayData == null;
							if (flag3)
							{
								result = -1;
							}
							else
							{
								result = ItemSortController.CompareByName(x.ItemDisplayData, y.ItemDisplayData);
							}
						}
					}
				}
				else
				{
					bool flag4 = x.CharacterDisplayData == null && y.CharacterDisplayData == null;
					if (flag4)
					{
						result = 0;
					}
					else
					{
						bool flag5 = x.CharacterDisplayData == null;
						if (flag5)
						{
							result = 1;
						}
						else
						{
							bool flag6 = y.CharacterDisplayData == null;
							if (flag6)
							{
								result = -1;
							}
							else
							{
								string xName = NameCenter.GetMonasticTitleOrDisplayName(x.CharacterDisplayData, false);
								string yName = NameCenter.GetMonasticTitleOrDisplayName(y.CharacterDisplayData, false);
								result = Utils_Sorting.CompareByCurrentLangEncoding(xName, yName);
							}
						}
					}
				}
			}
			else
			{
				result = x.BonusData.ExpLevel.CompareTo(y.BonusData.ExpLevel);
			}
			return result;
		}

		// Token: 0x0600AD06 RID: 44294 RVA: 0x004F06D4 File Offset: 0x004EE8D4
		private static int CompareByCharGrade(SkillBreakBonusSelectableItem x, SkillBreakBonusSelectableItem y)
		{
			EBonusItemType type = x.Type;
			EBonusItemType ebonusItemType = type;
			int result;
			if (ebonusItemType != EBonusItemType.Character)
			{
				result = 0;
			}
			else
			{
				result = y.BonusData.Grade.CompareTo(x.BonusData.Grade);
			}
			return result;
		}

		// Token: 0x0600AD07 RID: 44295 RVA: 0x004F0718 File Offset: 0x004EE918
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				0,
				56,
				5,
				6,
				0,
				1,
				11,
				121,
				0
			};
			List<short> itemCommonFilter = new List<short>
			{
				0,
				56,
				5,
				6
			};
			List<int> sortNameIndexList = new List<int>();
			for (int i = 0; i < sortIds.Count; i++)
			{
				sortNameIndexList.Add(0);
			}
			Dictionary<ValueTuple<int, int>, List<short>> dictionary = new Dictionary<ValueTuple<int, int>, List<short>>();
			ValueTuple<int, int> key = new ValueTuple<int, int>(1, -1);
			dictionary[key] = new List<short>
			{
				0
			};
			ValueTuple<int, int> key2 = new ValueTuple<int, int>(2, -1);
			dictionary[key2] = new List<short>
			{
				0,
				1,
				11,
				121
			};
			ValueTuple<int, int> key3 = new ValueTuple<int, int>(3, -1);
			dictionary[key3] = new List<short>(itemCommonFilter);
			ValueTuple<int, int> key4 = new ValueTuple<int, int>(4, -1);
			dictionary[key4] = new List<short>(itemCommonFilter);
			ValueTuple<int, int> key5 = new ValueTuple<int, int>(5, -1);
			dictionary[key5] = new List<short>(itemCommonFilter);
			ValueTuple<int, int> key6 = new ValueTuple<int, int>(6, -1);
			dictionary[key6] = new List<short>(itemCommonFilter);
			ValueTuple<int, int> key7 = new ValueTuple<int, int>(7, -1);
			dictionary[key7] = new List<short>(itemCommonFilter);
			Dictionary<ValueTuple<int, int>, List<short>> filterDic = dictionary;
			return new SortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = sortNameIndexList,
				DefaultSortState = default(SortUiState),
				FilterTypeDic = filterDic,
				DefaultSortIds = new List<short>()
			};
		}
	}
}
