using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Views.Make;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using UnityEngine;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D9C RID: 3484
	public class ItemSortController : SortController<ITradeableContent>
	{
		// Token: 0x17001254 RID: 4692
		// (get) Token: 0x0600A917 RID: 43287 RVA: 0x004E2AD9 File Offset: 0x004E0CD9
		// (set) Token: 0x0600A918 RID: 43288 RVA: 0x004E2AE1 File Offset: 0x004E0CE1
		public Func<ITradeableContent, int> RequireCharacterAmountGetter { get; set; }

		// Token: 0x0600A919 RID: 43289 RVA: 0x004E2AEC File Offset: 0x004E0CEC
		public override Comparison<ITradeableContent> GenerateComparer(SortStateData sortData)
		{
			return (ITradeableContent x, ITradeableContent y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600A91A RID: 43290 RVA: 0x004E2B20 File Offset: 0x004E0D20
		private int CompareData(ITradeableContent x, ITradeableContent y, SortStateData sortData)
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
						bool flag4 = x.GetContentType() > 0 || y.GetContentType() > 0;
						if (flag4)
						{
							result = (int)(y.GetContentType() - x.GetContentType());
						}
						else
						{
							bool enablePriorityCompare = this.EnablePriorityCompare;
							if (enablePriorityCompare)
							{
								int priorityResult = ItemSortController.ComparePriority(x, y);
								bool flag5 = priorityResult != 0;
								if (flag5)
								{
									return priorityResult;
								}
								ItemKey itemKey = x.RealKey;
								bool flag6;
								if (!itemKey.IsValid() && x.RealKey.ItemType < 0)
								{
									itemKey = x.RealKey;
									flag6 = itemKey.HasTemplate;
								}
								else
								{
									flag6 = false;
								}
								bool leftIsMakeType = flag6;
								itemKey = y.RealKey;
								bool flag7;
								if (!itemKey.IsValid() && y.RealKey.ItemType < 0)
								{
									itemKey = x.RealKey;
									flag7 = itemKey.HasTemplate;
								}
								else
								{
									flag7 = false;
								}
								bool rightIsMakeType = flag7;
								bool flag8 = leftIsMakeType && rightIsMakeType;
								if (flag8)
								{
									itemKey = x.Key;
									return itemKey.TemplateId.CompareTo(y.Key.TemplateId);
								}
								bool flag9 = leftIsMakeType || rightIsMakeType;
								if (flag9)
								{
									return rightIsMakeType.CompareTo(leftIsMakeType);
								}
							}
							bool flag10 = ((sortData != null) ? sortData.ItemStates : null) != null;
							if (flag10)
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
									ItemSortController.<>c__DisplayClass6_1 CS$<>8__locals2;
									CS$<>8__locals2.order = itemState.SortDirection;
									short num = sortId;
									short num2 = num;
									int comparisonResult;
									if (num2 <= 131)
									{
										switch (num2)
										{
										case 0:
											comparisonResult = ItemSortController.CompareByName(x, y);
											goto IL_DEA;
										case 1:
											comparisonResult = ItemSortController.CompareByGrade(x, y);
											goto IL_DEA;
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
											goto IL_3D6;
										case 6:
										{
											int num3 = x.Weight;
											comparisonResult = num3.CompareTo(y.Weight);
											goto IL_DEA;
										}
										case 16:
										case 17:
										{
											int num3 = x.Amount;
											comparisonResult = num3.CompareTo(y.Amount);
											goto IL_DEA;
										}
										case 18:
										{
											int num3 = x.Durability;
											comparisonResult = num3.CompareTo(y.Durability);
											goto IL_DEA;
										}
										case 19:
											comparisonResult = x.PowerInfo.Power.CompareTo(y.PowerInfo.Power);
											goto IL_DEA;
										case 20:
										case 28:
										{
											int num3 = x.EquipmentAttack;
											comparisonResult = num3.CompareTo(y.EquipmentAttack);
											goto IL_DEA;
										}
										case 21:
										{
											int num3 = x.EquipmentDefense;
											comparisonResult = num3.CompareTo(y.EquipmentDefense);
											goto IL_DEA;
										}
										case 22:
										{
											int num3 = ItemSortController.GetOuterPenetrate(x);
											comparisonResult = num3.CompareTo(ItemSortController.GetOuterPenetrate(y));
											goto IL_DEA;
										}
										case 23:
										{
											int num3 = ItemSortController.GetInnerPenetrate(x);
											comparisonResult = num3.CompareTo(ItemSortController.GetInnerPenetrate(y));
											goto IL_DEA;
										}
										case 24:
										case 33:
											comparisonResult = x.HitAvoidFactor[0].CompareTo(y.HitAvoidFactor[0]);
											goto IL_DEA;
										case 25:
										case 34:
											comparisonResult = x.HitAvoidFactor[1].CompareTo(y.HitAvoidFactor[1]);
											goto IL_DEA;
										case 26:
										case 35:
											comparisonResult = x.HitAvoidFactor[2].CompareTo(y.HitAvoidFactor[2]);
											goto IL_DEA;
										case 27:
										case 36:
											comparisonResult = x.HitAvoidFactor[3].CompareTo(y.HitAvoidFactor[3]);
											goto IL_DEA;
										case 29:
										{
											ValueTuple<short, short> penetrationInfo = x.PenetrationInfo;
											comparisonResult = penetrationInfo.Item1.CompareTo(y.PenetrationInfo.Item1);
											goto IL_DEA;
										}
										case 30:
										{
											ValueTuple<short, short> penetrationInfo = x.PenetrationInfo;
											comparisonResult = penetrationInfo.Item2.CompareTo(y.PenetrationInfo.Item2);
											goto IL_DEA;
										}
										case 31:
										{
											OuterAndInnerShorts injuryFactors = x.InjuryFactors;
											comparisonResult = injuryFactors.Inner.CompareTo(y.InjuryFactors.Inner);
											goto IL_DEA;
										}
										case 32:
										{
											OuterAndInnerShorts injuryFactors = x.InjuryFactors;
											comparisonResult = injuryFactors.Outer.CompareTo(y.InjuryFactors.Outer);
											goto IL_DEA;
										}
										case 37:
										case 38:
										case 39:
										case 40:
										case 41:
										{
											CarrierItem configX = Carrier.Instance[x.Key.TemplateId];
											CarrierItem configY = Carrier.Instance[y.Key.TemplateId];
											bool flag11 = sortId == 37;
											if (flag11)
											{
												comparisonResult = configX.BaseMaxInventoryLoadBonus.CompareTo(configY.BaseMaxInventoryLoadBonus);
											}
											else
											{
												bool flag12 = sortId == 38;
												if (flag12)
												{
													comparisonResult = configX.BaseTravelTimeReduction.CompareTo(configY.BaseTravelTimeReduction);
												}
												else
												{
													bool flag13 = sortId == 39;
													if (flag13)
													{
														comparisonResult = configX.DropRate.CompareTo(configY.DropRate);
													}
													else
													{
														bool flag14 = sortId == 40;
														if (flag14)
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
											goto IL_DEA;
										}
										case 42:
										{
											int num3 = x.Durability;
											comparisonResult = num3.CompareTo(y.Durability);
											goto IL_DEA;
										}
										case 43:
											comparisonResult = ItemSortController.CompareCricketData(x, y, (CricketData d) => d.AgeProgress);
											goto IL_DEA;
										case 44:
											comparisonResult = ItemSortController.CompareCricketData(x, y, (CricketData d) => (int)d.WinsCount);
											goto IL_DEA;
										case 45:
											comparisonResult = ItemSortController.CompareCricketData(x, y, (CricketData d) => (int)d.LossesCount);
											goto IL_DEA;
										case 46:
										{
											ITradeableContent x2 = x;
											ITradeableContent y2 = y;
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
											goto IL_DEA;
										}
										case 47:
										{
											ITradeableContent x3 = x;
											ITradeableContent y3 = y;
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
											goto IL_DEA;
										}
										case 48:
										{
											ITradeableContent x4 = x;
											ITradeableContent y4 = y;
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
											goto IL_DEA;
										}
										case 49:
										{
											ITradeableContent x5 = x;
											ITradeableContent y5 = y;
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
											goto IL_DEA;
										}
										case 50:
										{
											ITradeableContent x6 = x;
											ITradeableContent y6 = y;
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
											goto IL_DEA;
										}
										case 56:
										{
											int num3 = ItemSortController.<CompareData>g__GetSubType|6_13(x.Key, ref CS$<>8__locals2);
											comparisonResult = num3.CompareTo(ItemSortController.<CompareData>g__GetSubType|6_13(y.Key, ref CS$<>8__locals2));
											goto IL_DEA;
										}
										default:
											if (num2 == 124)
											{
												ItemKey itemKey = x.Key;
												comparisonResult = itemKey.TemplateId.CompareTo(y.Key.TemplateId);
												goto IL_DEA;
											}
											if (num2 == 131)
											{
												ItemDisplayData xData = x as ItemDisplayData;
												ItemDisplayData yData;
												bool flag15;
												if (xData != null)
												{
													yData = (y as ItemDisplayData);
													flag15 = (yData != null);
												}
												else
												{
													flag15 = false;
												}
												bool flag16 = flag15;
												if (flag16)
												{
													comparisonResult = xData.SpecialBreakProgress.CompareTo(yData.SpecialBreakProgress);
												}
												else
												{
													comparisonResult = 0;
												}
												goto IL_DEA;
											}
											break;
										}
									}
									else if (num2 <= 158)
									{
										if (num2 == 133)
										{
											Func<ITradeableContent, int> requireCharacterAmountGetter = this.RequireCharacterAmountGetter;
											int num3 = (requireCharacterAmountGetter != null) ? requireCharacterAmountGetter(x) : 0;
											Func<ITradeableContent, int> requireCharacterAmountGetter2 = this.RequireCharacterAmountGetter;
											comparisonResult = num3.CompareTo((requireCharacterAmountGetter2 != null) ? requireCharacterAmountGetter2(y) : 0);
											goto IL_DEA;
										}
										switch (num2)
										{
										case 156:
										{
											int num3 = x.MakeNeedAttainment;
											comparisonResult = num3.CompareTo(y.MakeNeedAttainment);
											goto IL_DEA;
										}
										case 157:
										{
											int num3 = x.MakeAvailableToolCount;
											comparisonResult = num3.CompareTo(y.MakeAvailableToolCount);
											goto IL_DEA;
										}
										case 158:
										{
											int num3 = x.MakeAvailableMaterialCount;
											comparisonResult = num3.CompareTo(y.MakeAvailableMaterialCount);
											goto IL_DEA;
										}
										}
									}
									else
									{
										switch (num2)
										{
										case 165:
											comparisonResult = ItemSortController.CompareByPoisonCountAndType(x, y);
											goto IL_DEA;
										case 166:
											break;
										case 167:
											comparisonResult = Config.Material.Instance[x.RealKey.TemplateId].RefiningEffect.CompareTo(Config.Material.Instance[y.RealKey.TemplateId].RefiningEffect);
											goto IL_DEA;
										case 168:
										{
											sbyte xCount = ((ItemDisplayData)x).RefiningEffects.GetTotalRefiningCount();
											sbyte yCount = ((ItemDisplayData)y).RefiningEffects.GetTotalRefiningCount();
											comparisonResult = xCount.CompareTo(yCount);
											goto IL_DEA;
										}
										default:
											switch (num2)
											{
											case 209:
											{
												ItemDisplayData weavedClothingTemplateIdX = x as ItemDisplayData;
												ItemDisplayData weavedClothingTemplateIdY;
												bool flag17;
												if (weavedClothingTemplateIdX != null)
												{
													weavedClothingTemplateIdY = (y as ItemDisplayData);
													flag17 = (weavedClothingTemplateIdY != null);
												}
												else
												{
													flag17 = false;
												}
												bool flag18 = flag17;
												if (flag18)
												{
													comparisonResult = Utils_Sorting.CompareByCurrentLangEncoding(Clothing.Instance[weavedClothingTemplateIdX.WeavedClothingTemplateId].Name, Clothing.Instance[weavedClothingTemplateIdY.WeavedClothingTemplateId].Name);
												}
												else
												{
													comparisonResult = 0;
												}
												goto IL_DEA;
											}
											case 210:
											{
												ItemDisplayData weavedCountX = x as ItemDisplayData;
												ItemDisplayData weavedCountY;
												bool flag19;
												if (weavedCountX != null)
												{
													weavedCountY = (y as ItemDisplayData);
													flag19 = (weavedCountY != null);
												}
												else
												{
													flag19 = false;
												}
												bool flag20 = flag19;
												if (flag20)
												{
													comparisonResult = weavedCountX.Amount.CompareTo(weavedCountY.Amount);
												}
												else
												{
													comparisonResult = 0;
												}
												goto IL_DEA;
											}
											case 211:
												comparisonResult = CraftTool.Instance[x.RealKey.TemplateId].AttainmentBonus.CompareTo(CraftTool.Instance[y.RealKey.TemplateId].AttainmentBonus);
												goto IL_DEA;
											case 212:
											{
												int xValue = ItemTemplateHelper.GetBaseValue(x.RealKey.ItemType, x.RealKey.TemplateId);
												int yValue = ItemTemplateHelper.GetBaseValue(y.RealKey.ItemType, y.RealKey.TemplateId);
												comparisonResult = xValue.CompareTo(yValue);
												goto IL_DEA;
											}
											case 213:
												goto IL_3D6;
											case 217:
												comparisonResult = Medicine.Instance[x.RealKey.TemplateId].EffectValue.CompareTo(Medicine.Instance[y.RealKey.TemplateId].EffectValue);
												goto IL_DEA;
											}
											break;
										}
									}
									continue;
									IL_DEA:
									bool flag21 = comparisonResult != 0;
									if (flag21)
									{
										return (CS$<>8__locals2.order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
									}
									continue;
									IL_3D6:
									comparisonResult = x.Value.CompareTo(y.Value);
									goto IL_DEA;
								}
							}
							int fallbackResult = ItemSortController.CompareFallback(x, y);
							bool flag22 = fallbackResult != 0;
							if (flag22)
							{
								result = fallbackResult;
							}
							else
							{
								ItemKey itemKey = x.Key;
								result = itemKey.Id.CompareTo(y.Key.Id);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600A91B RID: 43291 RVA: 0x004E39C0 File Offset: 0x004E1BC0
		private static int GetCricketHp(ITradeableContent itemDisplayData, CricketData cricketData)
		{
			CricketPartsItem colorConfig = CricketParts.Instance[itemDisplayData.CricketColorId];
			CricketPartsItem partConfig = CricketParts.Instance[itemDisplayData.CricketPartId];
			bool isCombineCricket = itemDisplayData.CricketPartId > 0;
			int maxHp = (int)(colorConfig.HP + (isCombineCricket ? partConfig.HP : 0));
			return Mathf.Max(maxHp - (int)cricketData.InjuryHp, 0);
		}

		// Token: 0x0600A91C RID: 43292 RVA: 0x004E3A24 File Offset: 0x004E1C24
		private static int GetCricketSp(ITradeableContent itemDisplayData, CricketData cricketData)
		{
			CricketPartsItem colorConfig = CricketParts.Instance[itemDisplayData.CricketColorId];
			CricketPartsItem partConfig = CricketParts.Instance[itemDisplayData.CricketPartId];
			bool isCombineCricket = itemDisplayData.CricketPartId > 0;
			int maxSp = (int)(colorConfig.SP + (isCombineCricket ? partConfig.SP : 0));
			return Mathf.Max(maxSp - (int)cricketData.InjurySp, 0);
		}

		// Token: 0x0600A91D RID: 43293 RVA: 0x004E3A88 File Offset: 0x004E1C88
		private static int GetCricketVigor(ITradeableContent itemDisplayData, CricketData cricketData)
		{
			CricketPartsItem colorConfig = CricketParts.Instance[itemDisplayData.CricketColorId];
			CricketPartsItem partConfig = CricketParts.Instance[itemDisplayData.CricketPartId];
			bool isCombineCricket = itemDisplayData.CricketPartId > 0;
			int maxVigor = (int)(colorConfig.Vigor + (isCombineCricket ? partConfig.Vigor : 0));
			return Mathf.Max(maxVigor - (int)cricketData.InjuryVigor, 0);
		}

		// Token: 0x0600A91E RID: 43294 RVA: 0x004E3AEC File Offset: 0x004E1CEC
		private static int GetCricketStrength(ITradeableContent itemDisplayData, CricketData cricketData)
		{
			CricketPartsItem colorConfig = CricketParts.Instance[itemDisplayData.CricketColorId];
			CricketPartsItem partConfig = CricketParts.Instance[itemDisplayData.CricketPartId];
			bool isCombineCricket = itemDisplayData.CricketPartId > 0;
			int maxStrength = (int)(colorConfig.Strength + (isCombineCricket ? partConfig.Strength : 0));
			return Mathf.Max(maxStrength - (int)cricketData.InjuryStrength, 0);
		}

		// Token: 0x0600A91F RID: 43295 RVA: 0x004E3B50 File Offset: 0x004E1D50
		private static int GetCricketBite(ITradeableContent itemDisplayData, CricketData cricketData)
		{
			CricketPartsItem colorConfig = CricketParts.Instance[itemDisplayData.CricketColorId];
			CricketPartsItem partConfig = CricketParts.Instance[itemDisplayData.CricketPartId];
			bool isCombineCricket = itemDisplayData.CricketPartId > 0;
			int maxBite = (int)(colorConfig.Bite + (isCombineCricket ? partConfig.Bite : 0));
			return Mathf.Max(maxBite - (int)cricketData.InjuryBite, 0);
		}

		// Token: 0x0600A920 RID: 43296 RVA: 0x004E3BB4 File Offset: 0x004E1DB4
		private static int CompareCricketData(ITradeableContent x, ITradeableContent y, Func<CricketData, int> selector)
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
						CricketData cricketX = x.CricketData;
						CricketData cricketY = y.CricketData;
						bool hasX = cricketX != null;
						bool hasY = cricketY != null;
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

		// Token: 0x0600A921 RID: 43297 RVA: 0x004E3C8C File Offset: 0x004E1E8C
		private static int CompareCricketData(ITradeableContent x, ITradeableContent y, Func<CricketData, int> selectorX, Func<CricketData, int> selectorY)
		{
			CricketData cricketX = x.CricketData;
			CricketData cricketY = y.CricketData;
			bool hasX = cricketX != null;
			bool hasY = cricketY != null;
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

		// Token: 0x0600A922 RID: 43298 RVA: 0x004E3CF8 File Offset: 0x004E1EF8
		private static int GetOuterPenetrate(ITradeableContent x)
		{
			sbyte innerRatio = x.WeaponInnerRatio;
			int totalPenetrate = (int)(x.PenetrationInfo.Item1 * x.PowerInfo.Power / 100);
			return totalPenetrate * (int)(100 - innerRatio) / 100;
		}

		// Token: 0x0600A923 RID: 43299 RVA: 0x004E3D38 File Offset: 0x004E1F38
		private static int GetInnerPenetrate(ITradeableContent x)
		{
			sbyte innerRatio = x.WeaponInnerRatio;
			int totalPenetrate = (int)(x.PenetrationInfo.Item1 * x.PowerInfo.Power / 100);
			return totalPenetrate * (int)innerRatio / 100;
		}

		// Token: 0x0600A924 RID: 43300 RVA: 0x004E3D74 File Offset: 0x004E1F74
		private static int ComparePriority(ITradeableContent x, ITradeableContent y)
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

		// Token: 0x0600A925 RID: 43301 RVA: 0x004E3F58 File Offset: 0x004E2158
		private static int CompareFallback(ITradeableContent x, ITradeableContent y)
		{
			bool flag = x.CharacterId != y.CharacterId;
			int result;
			if (flag)
			{
				result = x.CharacterId.CompareTo(y.CharacterId);
			}
			else
			{
				bool flag2 = x.Key.ItemType != y.Key.ItemType;
				if (flag2)
				{
					ItemKey key = x.Key;
					result = key.ItemType.CompareTo(y.Key.ItemType);
				}
				else
				{
					bool xIsRandomMake = MakeSubPageMakeHelper.CheckIsRandomMake(x);
					bool yIsRandomMake = MakeSubPageMakeHelper.CheckIsRandomMake(y);
					bool flag3 = xIsRandomMake && yIsRandomMake;
					if (flag3)
					{
						ItemKey key = x.Key;
						result = key.TemplateId.CompareTo(y.Key.TemplateId);
					}
					else
					{
						short subTypeX = ItemTemplateHelper.GetItemSubType(x.Key.ItemType, x.Key.TemplateId);
						short subTypeY = ItemTemplateHelper.GetItemSubType(y.Key.ItemType, y.Key.TemplateId);
						bool flag4 = subTypeX != subTypeY;
						if (flag4)
						{
							result = subTypeX.CompareTo(subTypeY);
						}
						else
						{
							sbyte gradeX = ItemTemplateHelper.GetGrade(x.Key.ItemType, x.Key.TemplateId);
							sbyte gradeY = ItemTemplateHelper.GetGrade(y.Key.ItemType, y.Key.TemplateId);
							bool flag5 = gradeX != gradeY;
							if (flag5)
							{
								result = gradeY.CompareTo(gradeX);
							}
							else
							{
								bool stackableX = ItemTemplateHelper.IsStackable(x.Key.ItemType, x.Key.TemplateId);
								bool stackableY = ItemTemplateHelper.IsStackable(y.Key.ItemType, y.Key.TemplateId);
								bool flag6 = stackableX != stackableY;
								if (flag6)
								{
									result = (stackableX ? 1 : -1);
								}
								else
								{
									bool flag7 = x.Key.ItemType == 11;
									ItemKey key;
									if (flag7)
									{
										bool flag8 = x.CricketColorId != y.CricketColorId;
										if (flag8)
										{
											return x.CricketColorId.CompareTo(y.CricketColorId);
										}
										bool flag9 = x.CricketPartId != y.CricketPartId;
										if (flag9)
										{
											return x.CricketPartId.CompareTo(y.CricketPartId);
										}
										bool flag10 = x.Key.Id != y.Key.Id;
										if (flag10)
										{
											key = x.Key;
											return key.Id.CompareTo(y.Key.Id);
										}
									}
									sbyte poisonTypeX = ItemTemplateHelper.GetMedicineItemPoisonType(x.Key.ItemType, x.Key.TemplateId);
									sbyte poisonTypeY = ItemTemplateHelper.GetMedicineItemPoisonType(y.Key.ItemType, y.Key.TemplateId);
									bool flag11 = poisonTypeX != poisonTypeY;
									if (flag11)
									{
										sbyte orderX = PoisonSortingOrder.GetSortingOrderByType(poisonTypeX);
										sbyte orderY = PoisonSortingOrder.GetSortingOrderByType(poisonTypeY);
										bool flag12 = orderX != orderY;
										if (flag12)
										{
											return orderX.CompareTo(orderY);
										}
									}
									key = x.Key;
									bool flag13 = key.TemplateEquals(y.Key);
									if (flag13)
									{
										int poisonCompare = ItemSortController.ComparePoisonInfo(x, y);
										bool flag14 = poisonCompare != 0;
										if (flag14)
										{
											return poisonCompare;
										}
									}
									bool flag15 = x.Key.TemplateId != y.Key.TemplateId;
									if (flag15)
									{
										key = x.Key;
										result = key.TemplateId.CompareTo(y.Key.TemplateId);
									}
									else
									{
										bool flag16 = x.ExtraGoodsType != y.ExtraGoodsType;
										if (flag16)
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
				}
			}
			return result;
		}

		// Token: 0x0600A926 RID: 43302 RVA: 0x004E4318 File Offset: 0x004E2518
		private static int ComparePoisonInfo(ITradeableContent x, ITradeableContent y)
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

		// Token: 0x0600A927 RID: 43303 RVA: 0x004E4418 File Offset: 0x004E2618
		public static int CompareByName(ITradeableContent x, ITradeableContent y)
		{
			string xName = ItemSortController.GetItemName(x);
			string yName = ItemSortController.GetItemName(y);
			return Utils_Sorting.CompareByCurrentLangEncoding(xName, yName);
		}

		// Token: 0x0600A928 RID: 43304 RVA: 0x004E4440 File Offset: 0x004E2640
		private static int CompareByGrade(ITradeableContent x, ITradeableContent y)
		{
			sbyte xGrade = ItemTemplateHelper.GetGrade(x.Key.ItemType, x.Key.TemplateId);
			sbyte yGrade = ItemTemplateHelper.GetGrade(y.Key.ItemType, y.Key.TemplateId);
			return xGrade.CompareTo(yGrade);
		}

		// Token: 0x0600A929 RID: 43305 RVA: 0x004E4492 File Offset: 0x004E2692
		private static string GetItemName(ITradeableContent item)
		{
			return item.GetName(false);
		}

		// Token: 0x0600A92A RID: 43306 RVA: 0x004E449C File Offset: 0x004E269C
		private static int CompareByPoisonCountAndType(ITradeableContent x, ITradeableContent y)
		{
			FullPoisonEffects poisonEffects = x.PoisonEffects;
			int? num;
			if (poisonEffects == null)
			{
				num = null;
			}
			else
			{
				List<PoisonSlot> poisonSlotList = poisonEffects.PoisonSlotList;
				num = ((poisonSlotList != null) ? new int?(poisonSlotList.Count) : null);
			}
			int? num2 = num;
			int countX = num2.GetValueOrDefault();
			FullPoisonEffects poisonEffects2 = y.PoisonEffects;
			int? num3;
			if (poisonEffects2 == null)
			{
				num3 = null;
			}
			else
			{
				List<PoisonSlot> poisonSlotList2 = poisonEffects2.PoisonSlotList;
				num3 = ((poisonSlotList2 != null) ? new int?(poisonSlotList2.Count) : null);
			}
			num2 = num3;
			int countY = num2.GetValueOrDefault();
			bool flag = countX != 0 && countY != 0;
			if (flag)
			{
				bool poisonIsIdentifiedX = x.HasAnyPoison && !x.PoisonIsIdentified;
				bool poisonIsIdentifiedY = y.HasAnyPoison && !y.PoisonIsIdentified;
				bool flag2 = (poisonIsIdentifiedY != poisonIsIdentifiedX && poisonIsIdentifiedX) || poisonIsIdentifiedY;
				if (flag2)
				{
					return (poisonIsIdentifiedX && !poisonIsIdentifiedY) ? -1 : 1;
				}
				bool flag3 = countX == countY;
				if (flag3)
				{
					for (int i = 0; i < countX; i++)
					{
						PoisonSlot poisonX = x.PoisonEffects.PoisonSlotList[i];
						PoisonSlot poisonY = y.PoisonEffects.PoisonSlotList[i];
						int result = poisonY.GetPoisonsAndLevels().GetLevel((int)poisonY.MedicineConfig.PoisonType).CompareTo(poisonX.GetPoisonsAndLevels().GetLevel((int)poisonX.MedicineConfig.PoisonType));
						bool flag4 = result != 0;
						if (flag4)
						{
							return result;
						}
					}
				}
			}
			return countX.CompareTo(countY);
		}

		// Token: 0x0600A92B RID: 43307 RVA: 0x004E4644 File Offset: 0x004E2844
		public override SortUiConfig GenerateConfig()
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
				50,
				165,
				217,
				156,
				157,
				158
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
			Dictionary<ValueTuple<int, int>, List<short>> dictionary = new Dictionary<ValueTuple<int, int>, List<short>>();
			ValueTuple<int, int> key = new ValueTuple<int, int>(1, -1);
			dictionary[key] = new List<short>(itemCommonFilter)
			{
				17
			};
			ValueTuple<int, int> key2 = new ValueTuple<int, int>(2, -1);
			dictionary[key2] = new List<short>(itemCommonFilter)
			{
				17
			};
			ValueTuple<int, int> key3 = new ValueTuple<int, int>(3, -1);
			dictionary[key3] = new List<short>(itemCommonFilter)
			{
				18,
				19
			};
			ValueTuple<int, int> key4 = new ValueTuple<int, int>(3, 0);
			dictionary[key4] = new List<short>
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
			Dictionary<ValueTuple<int, int>, List<short>> filterDic = dictionary;
			List<short> armorFilter = new List<short>
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
			filterDic[new ValueTuple<int, int>(3, 1)] = armorFilter;
			filterDic[new ValueTuple<int, int>(3, 2)] = armorFilter;
			filterDic[new ValueTuple<int, int>(3, 3)] = armorFilter;
			filterDic[new ValueTuple<int, int>(3, 4)] = armorFilter;
			filterDic[new ValueTuple<int, int>(3, 7)] = new List<short>
			{
				37,
				38,
				39,
				40,
				41
			};
			filterDic[new ValueTuple<int, int>(4, -1)] = new List<short>(itemCommonFilter)
			{
				18
			};
			filterDic[new ValueTuple<int, int>(5, -1)] = new List<short>(itemCommonFilter)
			{
				18,
				211
			};
			filterDic[new ValueTuple<int, int>(6, -1)] = new List<short>(itemCommonFilter)
			{
				17,
				167
			};
			filterDic[new ValueTuple<int, int>(45, -1)] = new List<short>(itemCommonFilter)
			{
				17,
				167
			};
			filterDic[new ValueTuple<int, int>(7, -1)] = new List<short>(itemCommonFilter);
			filterDic[new ValueTuple<int, int>(7, MiscTypeSecondaryMenu.GetSecondaryFilterIndex(EMiscFilterKey.KeyItem))] = new List<short>
			{
				17
			};
			filterDic[new ValueTuple<int, int>(7, MiscTypeSecondaryMenu.GetSecondaryFilterIndex(EMiscFilterKey.Cricket))] = new List<short>
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
			filterDic[new ValueTuple<int, int>(7, MiscTypeSecondaryMenu.GetSecondaryFilterIndex(EMiscFilterKey.WesternPresent))] = new List<short>
			{
				17
			};
			return new SortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = sortNameIndexList,
				DefaultSortState = default(SortUiState),
				FilterTypeDic = filterDic,
				DefaultSortIds = itemCommonFilter,
				FixedSortId = new List<short>
				{
					0,
					1
				}
			};
		}

		// Token: 0x0600A92D RID: 43309 RVA: 0x004E4BA4 File Offset: 0x004E2DA4
		[CompilerGenerated]
		internal static int <CompareData>g__GetSubType|6_13(ItemKey key, ref ItemSortController.<>c__DisplayClass6_1 A_1)
		{
			sbyte itemType = key.ItemType;
			return (itemType >= 0 && itemType <= 12) ? ((int)ItemTemplateHelper.GetItemSubType(key.ItemType, key.TemplateId)) : ((A_1.order == ESortDirection.Ascending) ? int.MinValue : int.MaxValue);
		}

		// Token: 0x04008482 RID: 33922
		public bool EnablePriorityCompare = true;
	}
}
