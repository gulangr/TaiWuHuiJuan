using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000445 RID: 1093
	public class ItemSortController : CommonSortController<ItemDisplayData>
	{
		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x06004025 RID: 16421 RVA: 0x001FD3D1 File Offset: 0x001FB5D1
		private bool IsCricketDataReady
		{
			get
			{
				return ItemSortController._cricketDataDic.Count >= this._needCricketData;
			}
		}

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x06004026 RID: 16422 RVA: 0x001FD3E8 File Offset: 0x001FB5E8
		private bool IsWeaponDataReady
		{
			get
			{
				return ItemSortController._weaponInnerRatioDic.Count >= this._needWeaponData;
			}
		}

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x06004027 RID: 16423 RVA: 0x001FD3FF File Offset: 0x001FB5FF
		private bool IsAllDataReady
		{
			get
			{
				return this.IsCricketDataReady && this.IsWeaponDataReady;
			}
		}

		// Token: 0x06004028 RID: 16424 RVA: 0x001FD414 File Offset: 0x001FB614
		public override void Sort(List<ItemDisplayData> dataList, SortStateData sortData, Action actionAfterSort)
		{
			ItemSortController._cricketDataDic.Clear();
			ItemSortController._weaponInnerRatioDic.Clear();
			this._needCricketData = 0;
			this._needWeaponData = 0;
			List<SortItemState> sortStates = sortData.ItemStates;
			bool needCricketInfo = sortStates.Any((SortItemState t) => this._cricketOptions.Contains((int)t.SortId));
			bool needWeaponInfo = sortStates.Any((SortItemState t) => this._weaponOptions.Contains((int)t.SortId));
			Comparison<ItemDisplayData> <>9__3;
			this._actionAfterGetDataFromBackend = delegate()
			{
				bool flag6 = !this.IsAllDataReady;
				if (!flag6)
				{
					List<ItemDisplayData> dataList2 = dataList;
					Comparison<ItemDisplayData> comparison;
					if ((comparison = <>9__3) == null)
					{
						comparison = (<>9__3 = ((ItemDisplayData x, ItemDisplayData y) => ItemSortController.CompareData(x, y, sortData)));
					}
					dataList2.Sort(comparison);
					Action actionAfterSort2 = actionAfterSort;
					if (actionAfterSort2 != null)
					{
						actionAfterSort2();
					}
				}
			};
			bool flag = !needCricketInfo && !needWeaponInfo;
			if (flag)
			{
				this.OnFinishGetData();
			}
			bool flag2 = needCricketInfo;
			if (flag2)
			{
				using (List<ItemDisplayData>.Enumerator enumerator = dataList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ItemDisplayData item = enumerator.Current;
						bool flag3 = item.Key.ItemType == 11;
						if (flag3)
						{
							this._needCricketData++;
							ItemDomainMethod.AsyncCall.GetCricketData(null, item.Key.Id, delegate(int offset, RawDataPool dataPool)
							{
								this.OnGetCricketData(offset, dataPool, item);
							});
						}
					}
				}
			}
			bool flag4 = needWeaponInfo;
			if (flag4)
			{
				using (List<ItemDisplayData>.Enumerator enumerator2 = dataList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ItemDisplayData item = enumerator2.Current;
						bool flag5 = item.Key.ItemType == 0;
						if (flag5)
						{
							this._needWeaponData++;
							CombatDomainMethod.AsyncCall.GetWeaponInnerRatio(null, item.Key, delegate(int offset, RawDataPool dataPool)
							{
								this.OnGetWeaponInnerRatio(offset, dataPool, item);
							});
						}
					}
				}
			}
		}

		// Token: 0x06004029 RID: 16425 RVA: 0x001FD628 File Offset: 0x001FB828
		private void OnFinishGetData()
		{
			Action actionAfterGetDataFromBackend = this._actionAfterGetDataFromBackend;
			if (actionAfterGetDataFromBackend != null)
			{
				actionAfterGetDataFromBackend();
			}
		}

		// Token: 0x0600402A RID: 16426 RVA: 0x001FD640 File Offset: 0x001FB840
		private void OnGetWeaponInnerRatio(int offset, RawDataPool dataPool, ItemDisplayData item)
		{
			sbyte innerRatio = 0;
			Serializer.Deserialize(dataPool, offset, ref innerRatio);
			ItemSortController._weaponInnerRatioDic[item] = innerRatio;
			bool flag = ItemSortController._weaponInnerRatioDic.Count >= this._needWeaponData;
			if (flag)
			{
				this.OnFinishGetData();
			}
		}

		// Token: 0x0600402B RID: 16427 RVA: 0x001FD68C File Offset: 0x001FB88C
		private void OnGetCricketData(int offset, RawDataPool dataPool, ItemDisplayData item)
		{
			CricketData cricketData = null;
			Serializer.Deserialize(dataPool, offset, ref cricketData);
			ItemSortController._cricketDataDic[item] = cricketData;
			bool flag = ItemSortController._cricketDataDic.Count >= this._needCricketData;
			if (flag)
			{
				this.OnFinishGetData();
			}
		}

		// Token: 0x0600402C RID: 16428 RVA: 0x001FD6D8 File Offset: 0x001FB8D8
		private static int GetCricketHp(ItemDisplayData itemDisplayData, CricketData cricketData)
		{
			CricketPartsItem _colorConfig = CricketParts.Instance[itemDisplayData.CricketColorId];
			CricketPartsItem _partConfig = CricketParts.Instance[itemDisplayData.CricketPartId];
			bool _isCombineCricket = itemDisplayData.CricketPartId > 0;
			int maxHp = (int)(_colorConfig.HP + (_isCombineCricket ? _partConfig.HP : 0));
			return Mathf.Max(maxHp - (int)cricketData.InjuryHp, 0);
		}

		// Token: 0x0600402D RID: 16429 RVA: 0x001FD740 File Offset: 0x001FB940
		private static int GetCricketSp(ItemDisplayData itemDisplayData, CricketData cricketData)
		{
			CricketPartsItem _colorConfig = CricketParts.Instance[itemDisplayData.CricketColorId];
			CricketPartsItem _partConfig = CricketParts.Instance[itemDisplayData.CricketPartId];
			bool _isCombineCricket = itemDisplayData.CricketPartId > 0;
			int maxSp = (int)(_colorConfig.SP + (_isCombineCricket ? _partConfig.SP : 0));
			return Mathf.Max(maxSp - (int)cricketData.InjurySp, 0);
		}

		// Token: 0x0600402E RID: 16430 RVA: 0x001FD7A8 File Offset: 0x001FB9A8
		private static int GetCricketVigor(ItemDisplayData itemDisplayData, CricketData cricketData)
		{
			CricketPartsItem _colorConfig = CricketParts.Instance[itemDisplayData.CricketColorId];
			CricketPartsItem _partConfig = CricketParts.Instance[itemDisplayData.CricketPartId];
			bool _isCombineCricket = itemDisplayData.CricketPartId > 0;
			int maxVigor = (int)(_colorConfig.Vigor + (_isCombineCricket ? _partConfig.Vigor : 0));
			return Mathf.Max(maxVigor - (int)cricketData.InjuryVigor, 0);
		}

		// Token: 0x0600402F RID: 16431 RVA: 0x001FD810 File Offset: 0x001FBA10
		private static int GetCricketStrength(ItemDisplayData itemDisplayData, CricketData cricketData)
		{
			CricketPartsItem _colorConfig = CricketParts.Instance[itemDisplayData.CricketColorId];
			CricketPartsItem _partConfig = CricketParts.Instance[itemDisplayData.CricketPartId];
			bool _isCombineCricket = itemDisplayData.CricketPartId > 0;
			int maxStrength = (int)(_colorConfig.Strength + (_isCombineCricket ? _partConfig.Strength : 0));
			return Mathf.Max(maxStrength - (int)cricketData.InjuryStrength, 0);
		}

		// Token: 0x06004030 RID: 16432 RVA: 0x001FD878 File Offset: 0x001FBA78
		private static int GetCricketBite(ItemDisplayData itemDisplayData, CricketData cricketData)
		{
			CricketPartsItem _colorConfig = CricketParts.Instance[itemDisplayData.CricketColorId];
			CricketPartsItem _partConfig = CricketParts.Instance[itemDisplayData.CricketPartId];
			bool _isCombineCricket = itemDisplayData.CricketPartId > 0;
			int maxBite = (int)(_colorConfig.Bite + (_isCombineCricket ? _partConfig.Bite : 0));
			return Mathf.Max(maxBite - (int)cricketData.InjuryBite, 0);
		}

		// Token: 0x06004031 RID: 16433 RVA: 0x001FD8E0 File Offset: 0x001FBAE0
		private static int CompareData(ItemDisplayData x, ItemDisplayData y, SortStateData sortData)
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
						ItemKey key = x.Key;
						ItemKey key2 = y.Key;
						bool flag4 = false;
						if (flag4)
						{
							ItemKey key3 = x.Key;
							result = -1;
						}
						else
						{
							int priorityResult = ItemSortController.ComparePriority(x, y);
							bool flag5 = priorityResult != 0;
							if (flag5)
							{
								result = priorityResult;
							}
							else
							{
								bool flag6 = ((sortData != null) ? sortData.ItemStates : null) != null;
								if (flag6)
								{
									Func<CricketData, int> <>9__3;
									Func<CricketData, int> <>9__4;
									Func<CricketData, int> <>9__5;
									Func<CricketData, int> <>9__6;
									Func<CricketData, int> <>9__7;
									Func<CricketData, int> <>9__8;
									Func<CricketData, int> <>9__9;
									Func<CricketData, int> <>9__10;
									Func<CricketData, int> <>9__11;
									Func<CricketData, int> <>9__12;
									foreach (SortItemState itemState in sortData.ItemStates)
									{
										short sortId = itemState.SortId;
										ESortDirection order = itemState.SortDirection;
										short num = sortId;
										short num2 = num;
										int comparisonResult;
										switch (num2)
										{
										case 0:
											comparisonResult = ItemSortController.CompareByName(x, y);
											goto IL_8D6;
										case 1:
											comparisonResult = ItemSortController.CompareByGrade(x, y);
											goto IL_8D6;
										case 2:
										case 3:
										case 4:
										case 7:
										case 8:
										case 9:
										case 10:
										case 11:
										case 12:
										case 13:
										case 14:
										case 15:
										case 51:
										case 52:
										case 53:
										case 54:
										case 55:
											break;
										case 5:
											comparisonResult = x.Value.CompareTo(y.Value);
											goto IL_8D6;
										case 6:
											comparisonResult = x.Weight.CompareTo(y.Weight);
											goto IL_8D6;
										case 16:
										case 17:
											comparisonResult = x.Amount.CompareTo(y.Amount);
											goto IL_8D6;
										case 18:
											comparisonResult = x.Durability.CompareTo(y.Durability);
											goto IL_8D6;
										case 19:
											comparisonResult = x.PowerInfo.Power.CompareTo(y.PowerInfo.Power);
											goto IL_8D6;
										case 20:
										case 28:
											comparisonResult = x.EquipmentAttack.CompareTo(y.EquipmentAttack);
											goto IL_8D6;
										case 21:
											comparisonResult = x.EquipmentDefense.CompareTo(y.EquipmentDefense);
											goto IL_8D6;
										case 22:
											comparisonResult = ItemSortController.GetOuterPenetrate(x).CompareTo(ItemSortController.GetOuterPenetrate(y));
											goto IL_8D6;
										case 23:
											comparisonResult = ItemSortController.GetInnerPenetrate(x).CompareTo(ItemSortController.GetInnerPenetrate(y));
											goto IL_8D6;
										case 24:
										case 33:
											comparisonResult = x.HitAvoidFactor[0].CompareTo(y.HitAvoidFactor[0]);
											goto IL_8D6;
										case 25:
										case 34:
											comparisonResult = x.HitAvoidFactor[1].CompareTo(y.HitAvoidFactor[1]);
											goto IL_8D6;
										case 26:
										case 35:
											comparisonResult = x.HitAvoidFactor[2].CompareTo(y.HitAvoidFactor[2]);
											goto IL_8D6;
										case 27:
										case 36:
											comparisonResult = x.HitAvoidFactor[3].CompareTo(y.HitAvoidFactor[3]);
											goto IL_8D6;
										case 29:
											comparisonResult = x.PenetrationInfo.Item1.CompareTo(y.PenetrationInfo.Item1);
											goto IL_8D6;
										case 30:
											comparisonResult = x.PenetrationInfo.Item2.CompareTo(y.PenetrationInfo.Item2);
											goto IL_8D6;
										case 31:
											comparisonResult = x.InjuryFactors.Inner.CompareTo(y.InjuryFactors.Inner);
											goto IL_8D6;
										case 32:
											comparisonResult = x.InjuryFactors.Outer.CompareTo(y.InjuryFactors.Outer);
											goto IL_8D6;
										case 37:
										case 38:
										case 39:
										case 40:
										case 41:
										{
											CarrierItem configX = Carrier.Instance[x.Key.TemplateId];
											CarrierItem configY = Carrier.Instance[y.Key.TemplateId];
											bool flag7 = sortId == 37;
											if (flag7)
											{
												comparisonResult = configX.BaseMaxInventoryLoadBonus.CompareTo(configY.BaseMaxInventoryLoadBonus);
											}
											else
											{
												bool flag8 = sortId == 38;
												if (flag8)
												{
													comparisonResult = configX.BaseTravelTimeReduction.CompareTo(configY.BaseTravelTimeReduction);
												}
												else
												{
													bool flag9 = sortId == 39;
													if (flag9)
													{
														comparisonResult = configX.DropRate.CompareTo(configY.DropRate);
													}
													else
													{
														bool flag10 = sortId == 40;
														if (flag10)
														{
															comparisonResult = configX.BaseCaptureRateBonus.CompareTo(configY.BaseCaptureRateBonus);
														}
														else
														{
															comparisonResult = configX.TamePoint.CompareTo(configY.TamePoint);
														}
													}
												}
											}
											goto IL_8D6;
										}
										case 42:
											comparisonResult = x.Durability.CompareTo(y.Durability);
											goto IL_8D6;
										case 43:
											comparisonResult = ItemSortController.CompareCricketData(x, y, (CricketData d) => d.AgeProgress);
											goto IL_8D6;
										case 44:
											comparisonResult = ItemSortController.CompareCricketData(x, y, (CricketData d) => (int)d.WinsCount);
											goto IL_8D6;
										case 45:
											comparisonResult = ItemSortController.CompareCricketData(x, y, (CricketData d) => (int)d.LossesCount);
											goto IL_8D6;
										case 46:
										{
											ItemDisplayData x2 = x;
											ItemDisplayData y2 = y;
											Func<CricketData, int> selectorX;
											if ((selectorX = <>9__3) == null)
											{
												selectorX = (<>9__3 = ((CricketData d) => ItemSortController.GetCricketHp(x, d)));
											}
											Func<CricketData, int> selectorY;
											if ((selectorY = <>9__4) == null)
											{
												selectorY = (<>9__4 = ((CricketData d) => ItemSortController.GetCricketHp(y, d)));
											}
											comparisonResult = ItemSortController.CompareCricketData(x2, y2, selectorX, selectorY);
											goto IL_8D6;
										}
										case 47:
										{
											ItemDisplayData x3 = x;
											ItemDisplayData y3 = y;
											Func<CricketData, int> selectorX2;
											if ((selectorX2 = <>9__5) == null)
											{
												selectorX2 = (<>9__5 = ((CricketData d) => ItemSortController.GetCricketSp(x, d)));
											}
											Func<CricketData, int> selectorY2;
											if ((selectorY2 = <>9__6) == null)
											{
												selectorY2 = (<>9__6 = ((CricketData d) => ItemSortController.GetCricketSp(y, d)));
											}
											comparisonResult = ItemSortController.CompareCricketData(x3, y3, selectorX2, selectorY2);
											goto IL_8D6;
										}
										case 48:
										{
											ItemDisplayData x4 = x;
											ItemDisplayData y4 = y;
											Func<CricketData, int> selectorX3;
											if ((selectorX3 = <>9__7) == null)
											{
												selectorX3 = (<>9__7 = ((CricketData d) => ItemSortController.GetCricketVigor(x, d)));
											}
											Func<CricketData, int> selectorY3;
											if ((selectorY3 = <>9__8) == null)
											{
												selectorY3 = (<>9__8 = ((CricketData d) => ItemSortController.GetCricketVigor(y, d)));
											}
											comparisonResult = ItemSortController.CompareCricketData(x4, y4, selectorX3, selectorY3);
											goto IL_8D6;
										}
										case 49:
										{
											ItemDisplayData x5 = x;
											ItemDisplayData y5 = y;
											Func<CricketData, int> selectorX4;
											if ((selectorX4 = <>9__9) == null)
											{
												selectorX4 = (<>9__9 = ((CricketData d) => ItemSortController.GetCricketStrength(x, d)));
											}
											Func<CricketData, int> selectorY4;
											if ((selectorY4 = <>9__10) == null)
											{
												selectorY4 = (<>9__10 = ((CricketData d) => ItemSortController.GetCricketStrength(y, d)));
											}
											comparisonResult = ItemSortController.CompareCricketData(x5, y5, selectorX4, selectorY4);
											goto IL_8D6;
										}
										case 50:
										{
											ItemDisplayData x6 = x;
											ItemDisplayData y6 = y;
											Func<CricketData, int> selectorX5;
											if ((selectorX5 = <>9__11) == null)
											{
												selectorX5 = (<>9__11 = ((CricketData d) => ItemSortController.GetCricketBite(x, d)));
											}
											Func<CricketData, int> selectorY5;
											if ((selectorY5 = <>9__12) == null)
											{
												selectorY5 = (<>9__12 = ((CricketData d) => ItemSortController.GetCricketBite(y, d)));
											}
											comparisonResult = ItemSortController.CompareCricketData(x6, y6, selectorX5, selectorY5);
											goto IL_8D6;
										}
										case 56:
										{
											ItemKey key4 = x.Key;
											comparisonResult = key4.ItemType.CompareTo(y.Key.ItemType);
											goto IL_8D6;
										}
										default:
											if (num2 == 131)
											{
												comparisonResult = x.SpecialArg.CompareTo(y.SpecialArg);
												goto IL_8D6;
											}
											if (num2 == 132)
											{
												comparisonResult = x.SpecialArg.CompareTo(y.SpecialArg);
												goto IL_8D6;
											}
											break;
										}
										continue;
										IL_8D6:
										bool flag11 = comparisonResult != 0;
										if (flag11)
										{
											return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
										}
									}
								}
								int fallbackResult = ItemSortController.CompareFallback(x, y);
								bool flag12 = fallbackResult != 0;
								if (flag12)
								{
									result = fallbackResult;
								}
								else
								{
									ItemKey key4 = x.Key;
									result = key4.Id.CompareTo(y.Key.Id);
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06004032 RID: 16434 RVA: 0x001FE26C File Offset: 0x001FC46C
		private static int CompareCricketData(ItemDisplayData x, ItemDisplayData y, Func<CricketData, int> selector)
		{
			bool flag = x.Key.ItemType != 11 && y.Key.ItemType != 11;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = x.Key.ItemType != 11;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					bool flag3 = y.Key.ItemType != 11;
					if (flag3)
					{
						result = 1;
					}
					else
					{
						CricketData cricketX;
						bool hasX = ItemSortController._cricketDataDic.TryGetValue(x, out cricketX);
						CricketData cricketY;
						bool hasY = ItemSortController._cricketDataDic.TryGetValue(y, out cricketY);
						bool flag4 = hasX != hasY;
						if (flag4)
						{
							result = (hasX ? -1 : 1);
						}
						else
						{
							bool flag5 = !hasX;
							if (flag5)
							{
								result = 0;
							}
							else
							{
								result = selector(cricketX).CompareTo(selector(cricketY));
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06004033 RID: 16435 RVA: 0x001FE348 File Offset: 0x001FC548
		private static int CompareCricketData(ItemDisplayData x, ItemDisplayData y, Func<CricketData, int> selectorX, Func<CricketData, int> selectorY)
		{
			CricketData cricketX;
			bool hasX = ItemSortController._cricketDataDic.TryGetValue(x, out cricketX);
			CricketData cricketY;
			bool hasY = ItemSortController._cricketDataDic.TryGetValue(y, out cricketY);
			bool flag = hasX != hasY;
			int result;
			if (flag)
			{
				result = (hasX ? -1 : 1);
			}
			else
			{
				bool flag2 = !hasX;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					result = selectorX(cricketX).CompareTo(selectorY(cricketY));
				}
			}
			return result;
		}

		// Token: 0x06004034 RID: 16436 RVA: 0x001FE3B8 File Offset: 0x001FC5B8
		private static int GetOuterPenetrate(ItemDisplayData x)
		{
			sbyte innerRatio;
			bool flag = !ItemSortController._weaponInnerRatioDic.TryGetValue(x, out innerRatio);
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int totalPenetrate = (int)(x.PenetrationInfo.Item1 * x.PowerInfo.Power / 100);
				result = totalPenetrate * (int)(100 - innerRatio) / 100;
			}
			return result;
		}

		// Token: 0x06004035 RID: 16437 RVA: 0x001FE408 File Offset: 0x001FC608
		private static int GetInnerPenetrate(ItemDisplayData x)
		{
			sbyte innerRatio;
			bool flag = !ItemSortController._weaponInnerRatioDic.TryGetValue(x, out innerRatio);
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int totalPenetrate = (int)(x.PenetrationInfo.Item1 * x.PowerInfo.Power / 100);
				result = totalPenetrate * (int)innerRatio / 100;
			}
			return result;
		}

		// Token: 0x06004036 RID: 16438 RVA: 0x001FE454 File Offset: 0x001FC654
		private static int ComparePriority(ItemDisplayData x, ItemDisplayData y)
		{
			bool leftIsInvalid = x.Key.Equals(ItemKey.Invalid);
			bool rightIsInvalid = y.Key.Equals(ItemKey.Invalid);
			bool flag = leftIsInvalid != rightIsInvalid;
			int result;
			if (flag)
			{
				result = (leftIsInvalid ? -1 : 1);
			}
			else
			{
				bool leftIsEmptyTool = ItemTemplateHelper.IsEmptyTool(x.Key.ItemType, x.Key.TemplateId);
				bool rightIsEmptyTool = ItemTemplateHelper.IsEmptyTool(y.Key.ItemType, y.Key.TemplateId);
				bool flag2 = leftIsEmptyTool != rightIsEmptyTool;
				if (flag2)
				{
					result = rightIsEmptyTool.CompareTo(leftIsEmptyTool);
				}
				else
				{
					bool flag3 = x.Interactable != y.Interactable;
					if (flag3)
					{
						result = (x.Interactable ? -1 : 1);
					}
					else
					{
						bool flag4 = x.UnavailableType != y.UnavailableType;
						if (flag4)
						{
							result = ((x.UnavailableType == ItemDisplayData.ItemUnavailableType.Valid) ? -1 : 1);
						}
						else
						{
							bool leftIsTianSui = CommonUtils.IsTianSuiBaoLuItem(x.Key.ItemType, x.Key.TemplateId);
							bool rightIsTianSui = CommonUtils.IsTianSuiBaoLuItem(y.Key.ItemType, y.Key.TemplateId);
							bool flag5 = leftIsTianSui != rightIsTianSui;
							if (flag5)
							{
								result = (leftIsTianSui ? -1 : 1);
							}
							else
							{
								bool leftIsResource = ItemTemplateHelper.IsMiscResource(x.Key.ItemType, x.Key.TemplateId);
								bool rightIsResource = ItemTemplateHelper.IsMiscResource(y.Key.ItemType, y.Key.TemplateId);
								bool flag6 = leftIsResource != rightIsResource;
								if (flag6)
								{
									result = (leftIsResource ? -1 : 1);
								}
								else
								{
									int usingCompare = y.UsingType.CompareTo(x.UsingType);
									bool flag7 = usingCompare != 0;
									if (flag7)
									{
										result = usingCompare;
									}
									else
									{
										result = 0;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06004037 RID: 16439 RVA: 0x001FE638 File Offset: 0x001FC838
		private static int CompareFallback(ItemDisplayData x, ItemDisplayData y)
		{
			bool flag = x.Key.ItemType != y.Key.ItemType;
			int result;
			if (flag)
			{
				ItemKey key = x.Key;
				result = key.ItemType.CompareTo(y.Key.ItemType);
			}
			else
			{
				short subTypeX = ItemTemplateHelper.GetItemSubType(x.Key.ItemType, x.Key.TemplateId);
				short subTypeY = ItemTemplateHelper.GetItemSubType(y.Key.ItemType, y.Key.TemplateId);
				bool flag2 = subTypeX != subTypeY;
				if (flag2)
				{
					result = subTypeX.CompareTo(subTypeY);
				}
				else
				{
					sbyte gradeX = ItemTemplateHelper.GetGrade(x.Key.ItemType, x.Key.TemplateId);
					sbyte gradeY = ItemTemplateHelper.GetGrade(y.Key.ItemType, y.Key.TemplateId);
					bool flag3 = gradeX != gradeY;
					if (flag3)
					{
						result = gradeY.CompareTo(gradeX);
					}
					else
					{
						bool stackableX = ItemTemplateHelper.IsStackable(x.Key.ItemType, x.Key.TemplateId);
						bool stackableY = ItemTemplateHelper.IsStackable(y.Key.ItemType, y.Key.TemplateId);
						bool flag4 = stackableX != stackableY;
						if (flag4)
						{
							result = (stackableX ? 1 : -1);
						}
						else
						{
							bool flag5 = x.Key.ItemType == 11;
							ItemKey key;
							if (flag5)
							{
								bool flag6 = x.CricketColorId != y.CricketColorId;
								if (flag6)
								{
									return x.CricketColorId.CompareTo(y.CricketColorId);
								}
								bool flag7 = x.CricketPartId != y.CricketPartId;
								if (flag7)
								{
									return x.CricketPartId.CompareTo(y.CricketPartId);
								}
								bool flag8 = x.Key.Id != y.Key.Id;
								if (flag8)
								{
									key = x.Key;
									return key.Id.CompareTo(y.Key.Id);
								}
							}
							sbyte poisonTypeX = ItemTemplateHelper.GetMedicineItemPoisonType(x.Key.ItemType, x.Key.TemplateId);
							sbyte poisonTypeY = ItemTemplateHelper.GetMedicineItemPoisonType(y.Key.ItemType, y.Key.TemplateId);
							bool flag9 = poisonTypeX != poisonTypeY;
							if (flag9)
							{
								sbyte orderX = PoisonSortingOrder.GetSortingOrderByType(poisonTypeX);
								sbyte orderY = PoisonSortingOrder.GetSortingOrderByType(poisonTypeY);
								bool flag10 = orderX != orderY;
								if (flag10)
								{
									return orderX.CompareTo(orderY);
								}
							}
							key = x.Key;
							bool flag11 = key.TemplateEquals(y.Key);
							if (flag11)
							{
								int poisonCompare = ItemSortController.ComparePoisonInfo(x, y);
								bool flag12 = poisonCompare != 0;
								if (flag12)
								{
									return poisonCompare;
								}
							}
							bool flag13 = x.Key.TemplateId != y.Key.TemplateId;
							if (flag13)
							{
								key = x.Key;
								result = key.TemplateId.CompareTo(y.Key.TemplateId);
							}
							else
							{
								bool flag14 = x.ExtraGoodsType != y.ExtraGoodsType;
								if (flag14)
								{
									result = x.ExtraGoodsType.CompareTo(y.ExtraGoodsType);
								}
								else
								{
									result = 0;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06004038 RID: 16440 RVA: 0x001FE980 File Offset: 0x001FCB80
		private static int ComparePoisonInfo(ItemDisplayData x, ItemDisplayData y)
		{
			bool flag = x.HasAnyPoison != y.HasAnyPoison;
			int result;
			if (flag)
			{
				result = (x.HasAnyPoison ? -1 : 1);
			}
			else
			{
				bool flag2 = !x.HasAnyPoison;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					bool flag3 = x.PoisonIsIdentified != y.PoisonIsIdentified;
					if (flag3)
					{
						result = (x.PoisonIsIdentified ? -1 : 1);
					}
					else
					{
						FullPoisonEffects poisonEffects = x.PoisonEffects;
						int poisonCountX = (poisonEffects != null) ? poisonEffects.GetTotalPoisonCount() : 0;
						FullPoisonEffects poisonEffects2 = y.PoisonEffects;
						int poisonCountY = (poisonEffects2 != null) ? poisonEffects2.GetTotalPoisonCount() : 0;
						bool flag4 = poisonCountX != poisonCountY;
						if (flag4)
						{
							result = poisonCountY.CompareTo(poisonCountX);
						}
						else
						{
							FullPoisonEffects poisonEffects3 = x.PoisonEffects;
							int poisonGradeX = (poisonEffects3 != null) ? poisonEffects3.GetMaxGrade() : 0;
							FullPoisonEffects poisonEffects4 = y.PoisonEffects;
							int poisonGradeY = (poisonEffects4 != null) ? poisonEffects4.GetMaxGrade() : 0;
							bool flag5 = poisonGradeX != poisonGradeY;
							if (flag5)
							{
								result = poisonGradeY.CompareTo(poisonGradeX);
							}
							else
							{
								result = 0;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06004039 RID: 16441 RVA: 0x001FEA80 File Offset: 0x001FCC80
		private static int CompareByName(ItemDisplayData x, ItemDisplayData y)
		{
			string xName = ItemSortController.GetItemName(x);
			string yName = ItemSortController.GetItemName(y);
			return Utils_Sorting.CompareByCurrentLangEncoding(xName, yName);
		}

		// Token: 0x0600403A RID: 16442 RVA: 0x001FEAA8 File Offset: 0x001FCCA8
		private static int CompareByGrade(ItemDisplayData x, ItemDisplayData y)
		{
			sbyte xGrade = ItemTemplateHelper.GetGrade(x.Key.ItemType, x.Key.TemplateId);
			sbyte yGrade = ItemTemplateHelper.GetGrade(y.Key.ItemType, y.Key.TemplateId);
			return xGrade.CompareTo(yGrade);
		}

		// Token: 0x0600403B RID: 16443 RVA: 0x001FEAFC File Offset: 0x001FCCFC
		private static string GetItemName(ItemDisplayData item)
		{
			return ItemSortController.IsCricket(item) ? item.CalcCricketName() : ItemTemplateHelper.GetName(item.Key.ItemType, item.Key.TemplateId);
		}

		// Token: 0x0600403C RID: 16444 RVA: 0x001FEB3C File Offset: 0x001FCD3C
		private static bool IsCricket(ItemDisplayData item)
		{
			return item.Key.ItemType == 11;
		}

		// Token: 0x0600403D RID: 16445 RVA: 0x001FEB60 File Offset: 0x001FCD60
		public override CommonSortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				0,
				1,
				5,
				6,
				17,
				18,
				19,
				20,
				21,
				22,
				23,
				24,
				25,
				26,
				27,
				28,
				29,
				30,
				31,
				32,
				33,
				34,
				35,
				36,
				37,
				38,
				39,
				40,
				41,
				42,
				43,
				44,
				45,
				46,
				47,
				48,
				49,
				50
			};
			List<int> sortNameIndexList = new List<int>();
			for (int i = 0; i < sortIds.Count; i++)
			{
				sortNameIndexList.Add(0);
			}
			List<short> itemCommonFilter = new List<short>
			{
				0,
				1,
				5,
				6
			};
			Dictionary<ValueTuple<int, int>, List<short>> filterDic = new Dictionary<ValueTuple<int, int>, List<short>>();
			filterDic[new ValueTuple<int, int>(1, -1)] = new List<short>(itemCommonFilter);
			filterDic[new ValueTuple<int, int>(1, -1)].Add(17);
			filterDic[new ValueTuple<int, int>(2, -1)] = new List<short>(itemCommonFilter);
			filterDic[new ValueTuple<int, int>(2, -1)].Add(17);
			filterDic[new ValueTuple<int, int>(3, -1)] = new List<short>(itemCommonFilter);
			filterDic[new ValueTuple<int, int>(3, -1)].Add(18);
			filterDic[new ValueTuple<int, int>(3, -1)].Add(19);
			filterDic[new ValueTuple<int, int>(3, 0)] = new List<short>
			{
				20,
				21,
				22,
				23,
				24,
				25,
				26,
				27
			};
			List<short> aromorFilter = new List<short>
			{
				28,
				29,
				30,
				31,
				32,
				33,
				34,
				35,
				36
			};
			filterDic[new ValueTuple<int, int>(3, 1)] = aromorFilter;
			filterDic[new ValueTuple<int, int>(3, 2)] = aromorFilter;
			filterDic[new ValueTuple<int, int>(3, 3)] = aromorFilter;
			filterDic[new ValueTuple<int, int>(3, 4)] = aromorFilter;
			filterDic[new ValueTuple<int, int>(3, 7)] = new List<short>
			{
				37,
				38,
				39,
				40,
				41
			};
			filterDic[new ValueTuple<int, int>(4, -1)] = new List<short>(itemCommonFilter);
			filterDic[new ValueTuple<int, int>(4, -1)].Add(18);
			filterDic[new ValueTuple<int, int>(5, -1)] = new List<short>(itemCommonFilter);
			filterDic[new ValueTuple<int, int>(5, -1)].Add(18);
			filterDic[new ValueTuple<int, int>(6, -1)] = new List<short>(itemCommonFilter);
			filterDic[new ValueTuple<int, int>(6, -1)].Add(17);
			filterDic[new ValueTuple<int, int>(7, -1)] = new List<short>(itemCommonFilter);
			filterDic[new ValueTuple<int, int>(7, 1)] = new List<short>
			{
				17
			};
			filterDic[new ValueTuple<int, int>(7, 3)] = new List<short>
			{
				17
			};
			filterDic[new ValueTuple<int, int>(7, 4)] = new List<short>
			{
				42,
				43,
				44,
				45,
				46,
				47,
				48,
				49,
				50
			};
			filterDic[new ValueTuple<int, int>(7, 6)] = new List<short>
			{
				17
			};
			return new CommonSortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = sortNameIndexList,
				DefaultSortState = null,
				FilterTypeDic = filterDic,
				DefaultSortIds = itemCommonFilter
			};
		}

		// Token: 0x04002DCF RID: 11727
		private HashSet<int> _cricketOptions = new HashSet<int>
		{
			42,
			43,
			44,
			45,
			46,
			47,
			48,
			49,
			50
		};

		// Token: 0x04002DD0 RID: 11728
		private HashSet<int> _weaponOptions = new HashSet<int>
		{
			22,
			23
		};

		// Token: 0x04002DD1 RID: 11729
		private static Dictionary<ItemDisplayData, CricketData> _cricketDataDic = new Dictionary<ItemDisplayData, CricketData>();

		// Token: 0x04002DD2 RID: 11730
		private static Dictionary<ItemDisplayData, sbyte> _weaponInnerRatioDic = new Dictionary<ItemDisplayData, sbyte>();

		// Token: 0x04002DD3 RID: 11731
		private int _needCricketData = 0;

		// Token: 0x04002DD4 RID: 11732
		private int _needWeaponData = 0;

		// Token: 0x04002DD5 RID: 11733
		private Action _actionAfterGetDataFromBackend;
	}
}
