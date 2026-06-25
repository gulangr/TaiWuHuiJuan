using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Views.Select;
using Game.Views.Select.SelectCharacter;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000D00 RID: 3328
	public class SelectCharacterSortController : SortController<ISelectCharacterData>
	{
		// Token: 0x0600A6D0 RID: 42704 RVA: 0x004D893C File Offset: 0x004D6B3C
		public SelectCharacterSortController(List<short> defaultSortIds, bool skipFallbackSort = false)
		{
			this._skipFallbackSort = skipFallbackSort;
			bool flag = defaultSortIds != null && defaultSortIds.Count > 0;
			if (flag)
			{
				this._defaultSortIds = defaultSortIds;
			}
		}

		// Token: 0x0600A6D1 RID: 42705 RVA: 0x004D89B8 File Offset: 0x004D6BB8
		public override Comparison<ISelectCharacterData> GenerateComparer(SortStateData sortData)
		{
			return (ISelectCharacterData x, ISelectCharacterData y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600A6D2 RID: 42706 RVA: 0x004D89EC File Offset: 0x004D6BEC
		private int CompareData(ISelectCharacterData x, ISelectCharacterData y, SortStateData sortData)
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
						bool flag4 = ((sortData != null) ? sortData.ItemStates : null) != null;
						if (flag4)
						{
							foreach (SortItemState itemState in sortData.ItemStates)
							{
								short sortId = itemState.SortId;
								ESortDirection order = itemState.SortDirection;
								int comparisonResult = this.CompareUnknownValues(x, y, sortId);
								bool flag5 = comparisonResult != 0;
								if (flag5)
								{
									return comparisonResult;
								}
								comparisonResult = this.CompareBySortId(x, y, sortId);
								bool flag6 = comparisonResult != 0;
								if (flag6)
								{
									return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
								}
							}
						}
						bool skipFallbackSort = this._skipFallbackSort;
						if (skipFallbackSort)
						{
							result = 0;
						}
						else
						{
							result = x.CharacterId.CompareTo(y.CharacterId);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600A6D3 RID: 42707 RVA: 0x004D8B0C File Offset: 0x004D6D0C
		private int GetCharmValue(ISelectCharacterData charData)
		{
			CharacterDisplayDataForGeneralScrollList data = (charData != null) ? charData.GetGeneralScrollListData() : null;
			bool flag = data == null;
			int result;
			if (flag)
			{
				result = int.MinValue;
			}
			else
			{
				bool flag2 = !data.FaceVisible;
				if (flag2)
				{
					result = int.MinValue;
				}
				else
				{
					bool isFixedCharacter = CreatingType.IsFixedPresetType(data.CreatingType);
					bool flag3 = !isFixedCharacter;
					if (flag3)
					{
						bool flag4 = data.PhysiologicalAge < 16;
						if (flag4)
						{
							return int.MinValue;
						}
						bool flag5 = data.ClothDisplayId == 0;
						if (flag5)
						{
							return int.MinValue;
						}
					}
					result = (int)data.Charm;
				}
			}
			return result;
		}

		// Token: 0x0600A6D4 RID: 42708 RVA: 0x004D8BA4 File Offset: 0x004D6DA4
		private static int CompareWithUnknownOnTop<T>(T x, T y, Func<T, bool> isUnknown)
		{
			bool xIsUnknown = isUnknown(x);
			return (xIsUnknown ^ isUnknown(y)) ? (xIsUnknown ? -1 : 1) : 0;
		}

		// Token: 0x0600A6D5 RID: 42709 RVA: 0x004D8BD4 File Offset: 0x004D6DD4
		private int CompareUnknownValues(ISelectCharacterData x, ISelectCharacterData y, short sortId)
		{
			int result;
			if (sortId != 9)
			{
				if (sortId != 11)
				{
					switch (sortId)
					{
					case 126:
						result = SelectCharacterSortController.CompareWithUnknownOnTop<ISelectCharacterData>(x, y, delegate(ISelectCharacterData z)
						{
							VillagerCharDisplayData villagerCharDisplayData = x as VillagerCharDisplayData;
							VillagerWorkData workData = (villagerCharDisplayData != null) ? villagerCharDisplayData.VillagerWorkData : null;
							return workData != null && workData.BuildingBlockIndex != -1 && SelectCharacterSortController.WorkingStatusOrder(workData) == 2;
						});
						break;
					case 127:
						result = SelectCharacterSortController.CompareWithUnknownOnTop<ISelectCharacterData>(x, y, delegate(ISelectCharacterData z)
						{
							VillagerCharDisplayData villagerCharDisplayData = x as VillagerCharDisplayData;
							return ((villagerCharDisplayData != null) ? villagerCharDisplayData.VillagerWorkData : null) == null;
						});
						break;
					case 128:
						result = SelectCharacterSortController.CompareWithUnknownOnTop<ISelectCharacterData>(x, y, (ISelectCharacterData z) => !(x is VillagerCharDisplayData));
						break;
					case 129:
						result = SelectCharacterSortController.CompareWithUnknownOnTop<ISelectCharacterData>(x, y, delegate(ISelectCharacterData z)
						{
							VillagerCharDisplayData villagerCharDisplayData = x as VillagerCharDisplayData;
							return ((villagerCharDisplayData != null) ? villagerCharDisplayData.VillagerWorkData : null) == null;
						});
						break;
					default:
						result = 0;
						break;
					}
				}
				else
				{
					result = SelectCharacterSortController.CompareWithUnknownOnTop<ISelectCharacterData>(x, y, delegate(ISelectCharacterData z)
					{
						CharacterDisplayDataForGeneralScrollList characterDisplayDataForGeneralScrollList = (z != null) ? z.GetGeneralScrollListData() : null;
						return characterDisplayDataForGeneralScrollList == null || !characterDisplayDataForGeneralScrollList.IsInteractedWithTaiwu;
					});
				}
			}
			else
			{
				result = SelectCharacterSortController.CompareWithUnknownOnTop<ISelectCharacterData>(x, y, (ISelectCharacterData z) => this.GetCharmValue(z) == int.MinValue);
			}
			return result;
		}

		// Token: 0x0600A6D6 RID: 42710 RVA: 0x004D8CE4 File Offset: 0x004D6EE4
		private unsafe int CompareBySortId(ISelectCharacterData x, ISelectCharacterData y, short sortId)
		{
			CharacterDisplayDataForGeneralScrollList dataX = (x != null) ? x.GetGeneralScrollListData() : null;
			CharacterDisplayDataForGeneralScrollList dataY = (y != null) ? y.GetGeneralScrollListData() : null;
			bool flag = dataX == null && dataY == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = dataX == null;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					bool flag3 = dataY == null;
					if (flag3)
					{
						result = -1;
					}
					else
					{
						switch (sortId)
						{
						case 0:
							return SelectCharacterSortController.CompareByName(dataX, dataY);
						case 1:
							return SelectCharacterSortController.CompareByGrade(dataX, dataY);
						case 5:
						{
							ISelectCharacterWagerValueRow rowX = x as ISelectCharacterWagerValueRow;
							ISelectCharacterWagerValueRow rowY;
							bool flag4;
							if (rowX != null)
							{
								rowY = (y as ISelectCharacterWagerValueRow);
								flag4 = (rowY != null);
							}
							else
							{
								flag4 = false;
							}
							bool flag5 = flag4;
							if (flag5)
							{
								return rowX.WagerValue.CompareTo(rowY.WagerValue);
							}
							return 0;
						}
						case 8:
							return dataX.PhysiologicalAge.CompareTo(dataY.PhysiologicalAge);
						case 9:
							return dataX.Charm.CompareTo(dataY.Charm);
						case 10:
						{
							EHealthType xType = CommonUtils.GetHealthType(dataX.Health, dataX.MaxLeftHealth, dataX.CharacterId);
							EHealthType yType = CommonUtils.GetHealthType(dataY.Health, dataY.MaxLeftHealth, dataY.CharacterId);
							return xType.CompareTo(yType);
						}
						case 11:
							return dataX.FavorabilityToTaiwu.CompareTo(dataY.FavorabilityToTaiwu);
						case 12:
							return dataX.Happiness.CompareTo(dataY.Happiness);
						case 13:
							return SelectCharacterSortController.GetVillagerLeftPotentialCount(x).CompareTo(SelectCharacterSortController.GetVillagerLeftPotentialCount(y));
						case 15:
							return this.CompareByServity(x, y);
						case 16:
							return this.CompareByBounty(x, y);
						case 22:
							return dataX.Penetrations.Outer.CompareTo(dataY.Penetrations.Outer);
						case 23:
							return dataX.Penetrations.Inner.CompareTo(dataY.Penetrations.Inner);
						case 24:
							return dataX.HitValues[0].CompareTo(dataY.HitValues[0]);
						case 25:
							return dataX.HitValues[1].CompareTo(dataY.HitValues[1]);
						case 26:
							return dataX.HitValues[2].CompareTo(dataY.HitValues[2]);
						case 27:
							return dataX.HitValues[3].CompareTo(dataY.HitValues[3]);
						case 29:
							return dataX.PenetrationResists.Outer.CompareTo(dataY.PenetrationResists.Outer);
						case 30:
							return dataX.PenetrationResists.Inner.CompareTo(dataY.PenetrationResists.Inner);
						case 33:
							return dataX.AvoidValues[0].CompareTo(dataY.AvoidValues[0]);
						case 34:
							return dataX.AvoidValues[1].CompareTo(dataY.AvoidValues[1]);
						case 35:
							return dataX.AvoidValues[2].CompareTo(dataY.AvoidValues[2]);
						case 36:
							return dataX.AvoidValues[3].CompareTo(dataY.AvoidValues[3]);
						case 37:
							return SelectCharacterSortController.GetInventoryLoadValue(dataX).CompareTo(SelectCharacterSortController.GetInventoryLoadValue(dataY));
						case 53:
							return dataX.DefeatMarkCount.CompareTo(dataY.DefeatMarkCount);
						case 55:
							return dataX.DisorderOfQi.CompareTo(dataY.DisorderOfQi);
						case 57:
							return dataX.BehaviorType.CompareTo(dataY.BehaviorType);
						case 58:
							return dataX.PreexistenceCharCount.CompareTo(dataY.PreexistenceCharCount);
						case 59:
							return dataX.Fame.CompareTo(dataY.Fame);
						case 60:
							return dataX.MaxMainAttributes[0].CompareTo(*dataY.MaxMainAttributes[0]);
						case 61:
							return dataX.MaxMainAttributes[1].CompareTo(*dataY.MaxMainAttributes[1]);
						case 62:
							return dataX.MaxMainAttributes[2].CompareTo(*dataY.MaxMainAttributes[2]);
						case 63:
							return dataX.MaxMainAttributes[3].CompareTo(*dataY.MaxMainAttributes[3]);
						case 64:
							return dataX.MaxMainAttributes[4].CompareTo(*dataY.MaxMainAttributes[4]);
						case 65:
							return dataX.MaxMainAttributes[5].CompareTo(*dataY.MaxMainAttributes[5]);
						case 66:
							return dataX.LifeSkillQualifications[0].CompareTo(*dataY.LifeSkillQualifications[0]);
						case 67:
							return dataX.LifeSkillQualifications[1].CompareTo(*dataY.LifeSkillQualifications[1]);
						case 68:
							return dataX.LifeSkillQualifications[2].CompareTo(*dataY.LifeSkillQualifications[2]);
						case 69:
							return dataX.LifeSkillQualifications[3].CompareTo(*dataY.LifeSkillQualifications[3]);
						case 70:
							return dataX.LifeSkillQualifications[4].CompareTo(*dataY.LifeSkillQualifications[4]);
						case 71:
							return dataX.LifeSkillQualifications[5].CompareTo(*dataY.LifeSkillQualifications[5]);
						case 72:
							return dataX.LifeSkillQualifications[6].CompareTo(*dataY.LifeSkillQualifications[6]);
						case 73:
							return dataX.LifeSkillQualifications[7].CompareTo(*dataY.LifeSkillQualifications[7]);
						case 74:
							return dataX.LifeSkillQualifications[8].CompareTo(*dataY.LifeSkillQualifications[8]);
						case 75:
							return dataX.LifeSkillQualifications[9].CompareTo(*dataY.LifeSkillQualifications[9]);
						case 76:
							return dataX.LifeSkillQualifications[10].CompareTo(*dataY.LifeSkillQualifications[10]);
						case 77:
							return dataX.LifeSkillQualifications[11].CompareTo(*dataY.LifeSkillQualifications[11]);
						case 78:
							return dataX.LifeSkillQualifications[12].CompareTo(*dataY.LifeSkillQualifications[12]);
						case 79:
							return dataX.LifeSkillQualifications[13].CompareTo(*dataY.LifeSkillQualifications[13]);
						case 80:
							return dataX.LifeSkillQualifications[14].CompareTo(*dataY.LifeSkillQualifications[14]);
						case 81:
							return dataX.LifeSkillQualifications[15].CompareTo(*dataY.LifeSkillQualifications[15]);
						case 82:
							return dataX.CombatSkillQualifications[0].CompareTo(*dataY.CombatSkillQualifications[0]);
						case 83:
							return dataX.CombatSkillQualifications[1].CompareTo(*dataY.CombatSkillQualifications[1]);
						case 84:
							return dataX.CombatSkillQualifications[2].CompareTo(*dataY.CombatSkillQualifications[2]);
						case 85:
							return dataX.CombatSkillQualifications[3].CompareTo(*dataY.CombatSkillQualifications[3]);
						case 86:
							return dataX.CombatSkillQualifications[4].CompareTo(*dataY.CombatSkillQualifications[4]);
						case 87:
							return dataX.CombatSkillQualifications[5].CompareTo(*dataY.CombatSkillQualifications[5]);
						case 88:
							return dataX.CombatSkillQualifications[6].CompareTo(*dataY.CombatSkillQualifications[6]);
						case 89:
							return dataX.CombatSkillQualifications[7].CompareTo(*dataY.CombatSkillQualifications[7]);
						case 90:
							return dataX.CombatSkillQualifications[8].CompareTo(*dataY.CombatSkillQualifications[8]);
						case 91:
							return dataX.CombatSkillQualifications[9].CompareTo(*dataY.CombatSkillQualifications[9]);
						case 92:
							return dataX.CombatSkillQualifications[10].CompareTo(*dataY.CombatSkillQualifications[10]);
						case 93:
							return dataX.CombatSkillQualifications[11].CompareTo(*dataY.CombatSkillQualifications[11]);
						case 94:
							return dataX.CombatSkillQualifications[12].CompareTo(*dataY.CombatSkillQualifications[12]);
						case 95:
							return dataX.CombatSkillQualifications[13].CompareTo(*dataY.CombatSkillQualifications[13]);
						case 96:
							return dataX.Personalities[0].CompareTo(*dataY.Personalities[0]);
						case 97:
							return dataX.Personalities[1].CompareTo(*dataY.Personalities[1]);
						case 98:
							return dataX.Personalities[2].CompareTo(*dataY.Personalities[2]);
						case 99:
							return dataX.Personalities[3].CompareTo(*dataY.Personalities[3]);
						case 100:
							return dataX.Personalities[4].CompareTo(*dataY.Personalities[4]);
						case 101:
							return dataX.Personalities[5].CompareTo(*dataY.Personalities[5]);
						case 102:
							return dataX.Personalities[6].CompareTo(*dataY.Personalities[6]);
						case 103:
							return SelectCharacterSortController.GetResourceValue(dataX, 0).CompareTo(SelectCharacterSortController.GetResourceValue(dataY, 0));
						case 104:
							return SelectCharacterSortController.GetResourceValue(dataX, 1).CompareTo(SelectCharacterSortController.GetResourceValue(dataY, 1));
						case 105:
							return SelectCharacterSortController.GetResourceValue(dataX, 2).CompareTo(SelectCharacterSortController.GetResourceValue(dataY, 2));
						case 106:
							return SelectCharacterSortController.GetResourceValue(dataX, 3).CompareTo(SelectCharacterSortController.GetResourceValue(dataY, 3));
						case 107:
							return SelectCharacterSortController.GetResourceValue(dataX, 4).CompareTo(SelectCharacterSortController.GetResourceValue(dataY, 4));
						case 108:
							return SelectCharacterSortController.GetResourceValue(dataX, 5).CompareTo(SelectCharacterSortController.GetResourceValue(dataY, 5));
						case 109:
							return SelectCharacterSortController.GetResourceValue(dataX, 6).CompareTo(SelectCharacterSortController.GetResourceValue(dataY, 6));
						case 110:
							return SelectCharacterSortController.GetResourceValue(dataX, 7).CompareTo(SelectCharacterSortController.GetResourceValue(dataY, 7));
						case 111:
							return SelectCharacterSortController.GetKidnapCountValue(dataX).CompareTo(SelectCharacterSortController.GetKidnapCountValue(dataY));
						case 112:
							return SelectCharacterSortController.GetMedalValue(dataX, "Attack").CompareTo(SelectCharacterSortController.GetMedalValue(dataY, "Attack"));
						case 113:
							return SelectCharacterSortController.GetMedalValue(dataX, "Defence").CompareTo(SelectCharacterSortController.GetMedalValue(dataY, "Defence"));
						case 114:
							return SelectCharacterSortController.GetMedalValue(dataX, "Wisdom").CompareTo(SelectCharacterSortController.GetMedalValue(dataY, "Wisdom"));
						case 115:
							return SelectCharacterSortController.GetCommandValue(dataX, 0).CompareTo(SelectCharacterSortController.GetCommandValue(dataY, 0));
						case 116:
							return SelectCharacterSortController.GetCommandValue(dataX, 1).CompareTo(SelectCharacterSortController.GetCommandValue(dataY, 1));
						case 117:
							return SelectCharacterSortController.GetCommandValue(dataX, 2).CompareTo(SelectCharacterSortController.GetCommandValue(dataY, 2));
						case 118:
							return SelectCharacterSortController.GetSkillGrowthAddValue(dataX.ActualAge, (int)dataX.LifeSkillGrowthType).CompareTo(SelectCharacterSortController.GetSkillGrowthAddValue(dataY.ActualAge, (int)dataY.LifeSkillGrowthType));
						case 119:
							return SelectCharacterSortController.GetSkillGrowthAddValue(dataX.ActualAge, (int)dataX.CombatSkillGrowthType).CompareTo(SelectCharacterSortController.GetSkillGrowthAddValue(dataY.ActualAge, (int)dataY.CombatSkillGrowthType));
						case 122:
							return dataX.Gender.CompareTo(dataY.Gender);
						case 123:
							return SelectCharacterSortController.CompareByInfection(x, y);
						case 126:
						{
							VillagerCharDisplayData villagerCharDisplayData = x as VillagerCharDisplayData;
							VillagerWorkData workDataX = (villagerCharDisplayData != null) ? villagerCharDisplayData.VillagerWorkData : null;
							VillagerCharDisplayData villagerCharDisplayData2 = y as VillagerCharDisplayData;
							VillagerWorkData workDataY = (villagerCharDisplayData2 != null) ? villagerCharDisplayData2.VillagerWorkData : null;
							bool flag6 = workDataX == null ^ workDataY == null;
							if (flag6)
							{
								return (workDataX == null) ? -1 : 1;
							}
							bool flag7 = workDataX == null;
							if (flag7)
							{
								return 0;
							}
							int ret = workDataX.BlockTemplateId.CompareTo(workDataY.BlockTemplateId);
							return (ret != 0) ? ret : workDataX.BlockId.CompareTo(workDataY.BlockId);
						}
						case 127:
						{
							VillagerCharDisplayData villagerCharDisplayData3 = x as VillagerCharDisplayData;
							VillagerWorkData workDataX2 = (villagerCharDisplayData3 != null) ? villagerCharDisplayData3.VillagerWorkData : null;
							VillagerCharDisplayData villagerCharDisplayData4 = y as VillagerCharDisplayData;
							VillagerWorkData workDataY2 = (villagerCharDisplayData4 != null) ? villagerCharDisplayData4.VillagerWorkData : null;
							bool flag8 = workDataX2 == null ^ workDataY2 == null;
							if (flag8)
							{
								return (workDataX2 == null) ? -1 : 1;
							}
							bool flag9 = workDataX2 == null;
							if (flag9)
							{
								return 0;
							}
							return SelectCharacterSortController.WorkingRoleOrder(workDataX2).CompareTo(SelectCharacterSortController.WorkingRoleOrder(workDataY2));
						}
						case 128:
						{
							VillagerCharDisplayData villagerDataX = x as VillagerCharDisplayData;
							VillagerCharDisplayData villagerDataY = y as VillagerCharDisplayData;
							bool flag10 = villagerDataX == null ^ villagerDataY == null;
							if (flag10)
							{
								return (villagerDataX == null) ? -1 : 1;
							}
							bool flag11 = villagerDataX == null;
							if (flag11)
							{
								return 0;
							}
							return villagerDataX.Potential.CompareTo(villagerDataY.Potential);
						}
						case 129:
						{
							VillagerCharDisplayData villagerCharDisplayData5 = x as VillagerCharDisplayData;
							VillagerWorkData workDataX3 = (villagerCharDisplayData5 != null) ? villagerCharDisplayData5.VillagerWorkData : null;
							VillagerCharDisplayData villagerCharDisplayData6 = y as VillagerCharDisplayData;
							VillagerWorkData workDataY3 = (villagerCharDisplayData6 != null) ? villagerCharDisplayData6.VillagerWorkData : null;
							bool flag12 = workDataX3 == null ^ workDataY3 == null;
							if (flag12)
							{
								return (workDataX3 == null) ? -1 : 1;
							}
							bool flag13 = workDataX3 == null;
							if (flag13)
							{
								return 0;
							}
							return SelectCharacterSortController.WorkingStatusOrder(workDataX3).CompareTo(SelectCharacterSortController.WorkingStatusOrder(workDataY3));
						}
						case 134:
							return this.CompareTakeAfterMonth(x, y);
						case 135:
							return this.CompareTakeAmount(x, y);
						case 136:
						{
							int indexX = CommonUtils.GetHighestPriorityRelationIndex(dataX.RelationToTaiwu, dataX.IsSameFactionWithTaiwu);
							int indexY = CommonUtils.GetHighestPriorityRelationIndex(dataY.RelationToTaiwu, dataY.IsSameFactionWithTaiwu);
							return indexX.CompareTo(indexY);
						}
						case 137:
							return this.CompareOrganizationPower(x, y);
						case 143:
							return dataX.ConsummateLevel.CompareTo(dataY.ConsummateLevel);
						case 151:
							return this.CompareByOriginalGrade(x, y);
						case 178:
							return SelectCharacterSortController.<CompareBySortId>g__SortKey|9_0(x).CompareTo(SelectCharacterSortController.<CompareBySortId>g__SortKey|9_0(y));
						case 201:
							return dataX.DarkAshRemainTime.CompareTo(dataY.DarkAshRemainTime);
						case 202:
							return dataX.TripodVesselProtectRemainTime.CompareTo(dataY.TripodVesselProtectRemainTime);
						case 203:
							return dataX.OrgInfo.OrgTemplateId.CompareTo(dataY.OrgInfo.OrgTemplateId);
						case 205:
							return SelectCharacterSortController.CompareLifeSkills(x, y, new sbyte[]
							{
								8,
								9
							});
						case 206:
							return SelectCharacterSortController.CompareLifeSkills(x, y, new sbyte[]
							{
								1,
								0,
								5
							});
						case 207:
							return SelectCharacterSortController.CompareLifeSkills(x, y, new sbyte[]
							{
								13,
								12
							});
						case 208:
							return SelectCharacterSortController.CreateCombatSkillCompareCellData(x, y);
						case 218:
							return SelectCharacterSortController.CompareByNeiliType(x, y, 0);
						case 219:
							return SelectCharacterSortController.CompareByNeiliType(x, y, 1);
						case 220:
							return SelectCharacterSortController.CompareByNeiliType(x, y, 2);
						case 221:
							return SelectCharacterSortController.CompareByNeiliType(x, y, 3);
						case 222:
							return SelectCharacterSortController.CompareByNeiliType(x, y, 4);
						}
						result = SelectCharacterSortController.CompareLifeOrCombatSkill(dataX, dataY, sortId);
					}
				}
			}
			return result;
		}

		// Token: 0x0600A6D7 RID: 42711 RVA: 0x004DA038 File Offset: 0x004D8238
		private static sbyte GetSkillGrowthAddValue(short actualAge, int growthType)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Math.Min((int)actualAge, AgeEffect.Instance.Count - 1)];
			return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
		}

		// Token: 0x0600A6D8 RID: 42712 RVA: 0x004DA084 File Offset: 0x004D8284
		private int CompareOrganizationPower(ISelectCharacterData x, ISelectCharacterData y)
		{
			ItemNeedCharacterDisplayData villagerDataX = x as ItemNeedCharacterDisplayData;
			ItemNeedCharacterDisplayData villagerDataY = y as ItemNeedCharacterDisplayData;
			bool flag = villagerDataX == null ^ villagerDataY == null;
			int result;
			if (flag)
			{
				result = ((villagerDataX == null) ? -1 : 1);
			}
			else
			{
				bool flag2 = villagerDataX == null;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					result = villagerDataX.PowerLevel.CompareTo(villagerDataY.PowerLevel);
				}
			}
			return result;
		}

		// Token: 0x0600A6D9 RID: 42713 RVA: 0x004DA0DC File Offset: 0x004D82DC
		private int CompareTakeAfterMonth(ISelectCharacterData x, ISelectCharacterData y)
		{
			ItemNeedCharacterDisplayData villagerDataX = x as ItemNeedCharacterDisplayData;
			ItemNeedCharacterDisplayData villagerDataY = y as ItemNeedCharacterDisplayData;
			bool flag = villagerDataX == null ^ villagerDataY == null;
			int result;
			if (flag)
			{
				result = ((villagerDataX == null) ? -1 : 1);
			}
			else
			{
				bool flag2 = villagerDataX == null;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					result = villagerDataX.TakeTime.CompareTo(villagerDataY.TakeTime);
				}
			}
			return result;
		}

		// Token: 0x0600A6DA RID: 42714 RVA: 0x004DA134 File Offset: 0x004D8334
		private int CompareTakeAmount(ISelectCharacterData x, ISelectCharacterData y)
		{
			ItemNeedCharacterDisplayData villagerDataX = x as ItemNeedCharacterDisplayData;
			ItemNeedCharacterDisplayData villagerDataY = y as ItemNeedCharacterDisplayData;
			bool flag = villagerDataX == null ^ villagerDataY == null;
			int result;
			if (flag)
			{
				result = ((villagerDataX == null) ? -1 : 1);
			}
			else
			{
				bool flag2 = villagerDataX == null;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					result = villagerDataX.TakeAmount.CompareTo(villagerDataY.TakeAmount);
				}
			}
			return result;
		}

		// Token: 0x0600A6DB RID: 42715 RVA: 0x004DA18C File Offset: 0x004D838C
		private unsafe static int CompareLifeSkills(ISelectCharacterData x, ISelectCharacterData y, params sbyte[] lifeSkills)
		{
			int valueResultX = 0;
			int valueResultY = 0;
			for (int i = 0; i < lifeSkills.Length; i++)
			{
				short newValueX = *x.GetGeneralScrollListData().LifeSkillQualifications[(int)lifeSkills[i]];
				bool flag = (int)newValueX >= valueResultX;
				if (flag)
				{
					valueResultX = (int)newValueX;
				}
				short newValueY = *y.GetGeneralScrollListData().LifeSkillQualifications[(int)lifeSkills[i]];
				bool flag2 = (int)newValueY >= valueResultY;
				if (flag2)
				{
					valueResultY = (int)newValueY;
				}
			}
			return valueResultX.CompareTo(valueResultY);
		}

		// Token: 0x0600A6DC RID: 42716 RVA: 0x004DA214 File Offset: 0x004D8414
		private unsafe static int CreateCombatSkillCompareCellData(ISelectCharacterData x, ISelectCharacterData y)
		{
			int valueResultX = 0;
			int valueResultY = 0;
			for (int i = 0; i < 14; i++)
			{
				short newValueX = *x.GetGeneralScrollListData().CombatSkillQualifications[i];
				bool flag = (int)newValueX >= valueResultX;
				if (flag)
				{
					valueResultX = (int)newValueX;
				}
				short newValueY = *y.GetGeneralScrollListData().CombatSkillQualifications[i];
				bool flag2 = (int)newValueY >= valueResultY;
				if (flag2)
				{
					valueResultY = (int)newValueY;
				}
			}
			return valueResultX.CompareTo(valueResultY);
		}

		// Token: 0x0600A6DD RID: 42717 RVA: 0x004DA294 File Offset: 0x004D8494
		private int CompareByOriginalGrade(ISelectCharacterData x, ISelectCharacterData y)
		{
			return ((BountyCharacterDataAdapter)x).Data.OrgInfo.Grade.CompareTo(((BountyCharacterDataAdapter)y).Data.OrgInfo.Grade);
		}

		// Token: 0x0600A6DE RID: 42718 RVA: 0x004DA2D8 File Offset: 0x004D84D8
		private int CompareByServity(ISelectCharacterData x, ISelectCharacterData y)
		{
			BountyCharacterDataAdapter dataX = (BountyCharacterDataAdapter)x;
			BountyCharacterDataAdapter dataY = (BountyCharacterDataAdapter)y;
			PunishmentTypeItem configX = PunishmentType.Instance[dataX.Data.SettlementBounty.PunishmentType];
			PunishmentTypeItem configY = PunishmentType.Instance[dataY.Data.SettlementBounty.PunishmentType];
			return configX.GetSeverity(dataX.CurrentStateTemplateId, true, false).CompareTo(configY.GetSeverity(dataY.CurrentStateTemplateId, true, false));
		}

		// Token: 0x0600A6DF RID: 42719 RVA: 0x004DA358 File Offset: 0x004D8558
		private int CompareByBounty(ISelectCharacterData x, ISelectCharacterData y)
		{
			return ((BountyCharacterDataAdapter)x).Data.SettlementBounty.BountyAmount.CompareTo(((BountyCharacterDataAdapter)y).Data.SettlementBounty.BountyAmount);
		}

		// Token: 0x0600A6E0 RID: 42720 RVA: 0x004DA39C File Offset: 0x004D859C
		private static int CompareByName(CharacterDisplayDataForGeneralScrollList x, CharacterDisplayDataForGeneralScrollList y)
		{
			NameRelatedData xNameData = x.NameData;
			NameRelatedData yNameData = y.NameData;
			string xName = NameCenter.GetMonasticTitleOrDisplayName(ref xNameData, false, false);
			string yName = NameCenter.GetMonasticTitleOrDisplayName(ref yNameData, false, false);
			return Utils_Sorting.CompareByCurrentLangEncoding(xName, yName);
		}

		// Token: 0x0600A6E1 RID: 42721 RVA: 0x004DA3DC File Offset: 0x004D85DC
		private static int CompareByGrade(CharacterDisplayDataForGeneralScrollList x, CharacterDisplayDataForGeneralScrollList y)
		{
			sbyte xGrade = x.OrgInfo.Grade;
			sbyte yGrade = y.OrgInfo.Grade;
			return xGrade.CompareTo(yGrade);
		}

		// Token: 0x0600A6E2 RID: 42722 RVA: 0x004DA410 File Offset: 0x004D8610
		private static int CompareByInfection(ISelectCharacterData x, ISelectCharacterData y)
		{
			int xInfection = 0;
			int yInfection = 0;
			YuanshanSelectCharacterDataAdapter xAdapter = x as YuanshanSelectCharacterDataAdapter;
			bool flag = xAdapter != null;
			if (flag)
			{
				CharacterDisplayDataForYuanshanSelect data = xAdapter.Data;
				xInfection = (int)((data != null) ? data.Infection : 0);
			}
			YuanshanSelectCharacterDataAdapter yAdapter = y as YuanshanSelectCharacterDataAdapter;
			bool flag2 = yAdapter != null;
			if (flag2)
			{
				CharacterDisplayDataForYuanshanSelect data2 = yAdapter.Data;
				yInfection = (int)((data2 != null) ? data2.Infection : 0);
			}
			return xInfection.CompareTo(yInfection);
		}

		// Token: 0x0600A6E3 RID: 42723 RVA: 0x004DA478 File Offset: 0x004D8678
		private static int GetVillagerLeftPotentialCount(ISelectCharacterData data)
		{
			VillagerSelectCharacterDataAdapter villagerData = data as VillagerSelectCharacterDataAdapter;
			bool flag = villagerData != null;
			int result;
			if (flag)
			{
				VillagerSelectCharacterDisplayData rawData = villagerData.GetRawData();
				result = (int)((rawData != null) ? rawData.LeftPotentialCount : 0);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x0600A6E4 RID: 42724 RVA: 0x004DA4B0 File Offset: 0x004D86B0
		private unsafe static int CompareLifeOrCombatSkill(CharacterDisplayDataForGeneralScrollList x, CharacterDisplayDataForGeneralScrollList y, short sortId)
		{
			bool flag = sortId >= 66 && sortId < 82;
			int result;
			if (flag)
			{
				int index = (int)(sortId - 66);
				bool flag2 = index >= 0 && index < 16 && index >= 0 && index < 16;
				if (flag2)
				{
					result = x.LifeSkillQualifications[index].CompareTo(*y.LifeSkillQualifications[index]);
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				bool flag3 = sortId >= 82 && sortId < 96;
				if (flag3)
				{
					int index2 = (int)(sortId - 82);
					bool flag4 = index2 >= 0 && index2 < 14 && index2 >= 0 && index2 < 14;
					if (flag4)
					{
						result = x.CombatSkillQualifications[index2].CompareTo(*y.CombatSkillQualifications[index2]);
					}
					else
					{
						result = 0;
					}
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x0600A6E5 RID: 42725 RVA: 0x004DA578 File Offset: 0x004D8778
		private unsafe static int CompareByNeiliType(ISelectCharacterData x, ISelectCharacterData y, sbyte neiliType)
		{
			BaihuaLifeLinkSelectCharacterDataAdapter adapterX = x as BaihuaLifeLinkSelectCharacterDataAdapter;
			BaihuaLifeLinkSelectCharacterDataAdapter adapterY;
			bool flag;
			if (adapterX != null)
			{
				adapterY = (y as BaihuaLifeLinkSelectCharacterDataAdapter);
				flag = (adapterY == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			int result;
			if (flag2)
			{
				result = 0;
			}
			else
			{
				CharacterDisplayDataForLifeLink baihuaDataX = adapterX.GetRawData();
				CharacterDisplayDataForLifeLink baihuaDataY = adapterY.GetRawData();
				result = baihuaDataX.NeiliPercent[(int)neiliType].CompareTo(*baihuaDataY.NeiliPercent[(int)neiliType]);
			}
			return result;
		}

		// Token: 0x0600A6E6 RID: 42726 RVA: 0x004DA5E0 File Offset: 0x004D87E0
		private unsafe static int GetResourceValue(CharacterDisplayDataForGeneralScrollList data, int index)
		{
			return *data.Resources[index];
		}

		// Token: 0x0600A6E7 RID: 42727 RVA: 0x004DA600 File Offset: 0x004D8800
		private static int GetInventoryLoadValue(CharacterDisplayDataForGeneralScrollList data)
		{
			return data.CurrInventoryLoad;
		}

		// Token: 0x0600A6E8 RID: 42728 RVA: 0x004DA618 File Offset: 0x004D8818
		private static int GetKidnapCountValue(CharacterDisplayDataForGeneralScrollList data)
		{
			return (int)data.KidnapCount;
		}

		// Token: 0x0600A6E9 RID: 42729 RVA: 0x004DA630 File Offset: 0x004D8830
		private static int GetMedalValue(CharacterDisplayDataForGeneralScrollList data, string medalType)
		{
			if (!true)
			{
			}
			int result;
			if (!(medalType == "Attack"))
			{
				if (!(medalType == "Defence"))
				{
					if (!(medalType == "Wisdom"))
					{
						result = 0;
					}
					else
					{
						result = data.WisdomMedal;
					}
				}
				else
				{
					result = data.DefenceMedal;
				}
			}
			else
			{
				result = data.AttackMedal;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600A6EA RID: 42730 RVA: 0x004DA693 File Offset: 0x004D8893
		public static int WorkingRoleOrder(VillagerWorkData x)
		{
			return (x == null || x.WorkType != 1) ? -1 : ((x.WorkerIndex == 0) ? 1 : 0);
		}

		// Token: 0x0600A6EB RID: 42731 RVA: 0x004DA6B0 File Offset: 0x004D88B0
		public static int WorkingStatusOrder(VillagerWorkData x)
		{
			sbyte workType = x.WorkType;
			if (!true)
			{
			}
			int result;
			if (workType <= 2)
			{
				if (workType == 1)
				{
					result = 2;
					goto IL_3D;
				}
				if (workType == 2)
				{
					result = 3;
					goto IL_3D;
				}
			}
			else
			{
				if (workType == 12)
				{
					result = 1;
					goto IL_3D;
				}
				if (workType == 13)
				{
					result = 0;
					goto IL_3D;
				}
			}
			result = -1;
			IL_3D:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600A6EC RID: 42732 RVA: 0x004DA700 File Offset: 0x004D8900
		private static int GetCommandValue(CharacterDisplayDataForGeneralScrollList data, int commandIndex)
		{
			bool flag = data.Command.Items == null || !data.Command.Items.CheckIndex(commandIndex);
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				result = (int)data.Command.Items[commandIndex];
			}
			return result;
		}

		// Token: 0x0600A6ED RID: 42733 RVA: 0x004DA750 File Offset: 0x004D8950
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				0,
				8,
				122,
				10,
				9,
				12,
				11,
				53,
				57,
				58,
				59,
				13,
				143,
				151,
				203,
				60,
				61,
				62,
				63,
				64,
				65,
				22,
				23,
				29,
				30,
				24,
				25,
				26,
				27,
				33,
				34,
				35,
				36,
				55,
				66,
				67,
				68,
				69,
				70,
				71,
				72,
				73,
				74,
				75,
				76,
				77,
				78,
				79,
				80,
				81,
				82,
				83,
				84,
				85,
				86,
				87,
				88,
				89,
				90,
				91,
				92,
				93,
				94,
				95,
				96,
				97,
				98,
				99,
				100,
				101,
				102,
				103,
				104,
				105,
				106,
				107,
				108,
				109,
				110,
				37,
				111,
				112,
				113,
				114,
				115,
				116,
				117,
				15,
				16,
				129,
				126,
				127,
				128
			};
			return new SortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = new List<int>(new int[sortIds.Count]),
				DefaultSortIds = this._defaultSortIds,
				FixedSortId = new List<short>
				{
					0,
					1
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

		// Token: 0x0600A6EE RID: 42734 RVA: 0x004DAB4C File Offset: 0x004D8D4C
		[CompilerGenerated]
		internal static int <CompareBySortId>g__SortKey|9_0(ISelectCharacterData x)
		{
			OrganizationMemberDisplayDataForGeneralScrollList org = x as OrganizationMemberDisplayDataForGeneralScrollList;
			return (org != null) ? org.ApprovingRate : -1;
		}

		// Token: 0x04008330 RID: 33584
		private List<short> _defaultSortIds = new List<short>
		{
			0,
			8,
			9,
			59,
			11,
			143
		};

		// Token: 0x04008331 RID: 33585
		private readonly bool _skipFallbackSort;

		// Token: 0x02002433 RID: 9267
		public static class SortIds
		{
			// Token: 0x0400E20F RID: 57871
			public const short Name = 0;

			// Token: 0x0400E210 RID: 57872
			public const short Age = 8;

			// Token: 0x0400E211 RID: 57873
			public const short Gender = 122;

			// Token: 0x0400E212 RID: 57874
			public const short Health = 10;

			// Token: 0x0400E213 RID: 57875
			public const short Charm = 9;

			// Token: 0x0400E214 RID: 57876
			public const short Happiness = 12;

			// Token: 0x0400E215 RID: 57877
			public const short Favor = 11;

			// Token: 0x0400E216 RID: 57878
			public const short Alertness = 130;

			// Token: 0x0400E217 RID: 57879
			public const short DefeatMark = 53;

			// Token: 0x0400E218 RID: 57880
			public const short Behavior = 57;

			// Token: 0x0400E219 RID: 57881
			public const short Samsara = 58;

			// Token: 0x0400E21A RID: 57882
			public const short Fame = 59;

			// Token: 0x0400E21B RID: 57883
			public const short VillagerLeftPotentialCount = 13;

			// Token: 0x0400E21C RID: 57884
			public const short Organization = 203;

			// Token: 0x0400E21D RID: 57885
			public const short Strength = 60;

			// Token: 0x0400E21E RID: 57886
			public const short Dexterity = 61;

			// Token: 0x0400E21F RID: 57887
			public const short Concentration = 62;

			// Token: 0x0400E220 RID: 57888
			public const short Vitality = 63;

			// Token: 0x0400E221 RID: 57889
			public const short Energy = 64;

			// Token: 0x0400E222 RID: 57890
			public const short Intelligence = 65;

			// Token: 0x0400E223 RID: 57891
			public const short OuterPenetrate = 22;

			// Token: 0x0400E224 RID: 57892
			public const short InnerPenetrate = 23;

			// Token: 0x0400E225 RID: 57893
			public const short OuterPenetrateResist = 29;

			// Token: 0x0400E226 RID: 57894
			public const short InnerPenetrateResist = 30;

			// Token: 0x0400E227 RID: 57895
			public const short HitStrength = 24;

			// Token: 0x0400E228 RID: 57896
			public const short HitTechnique = 25;

			// Token: 0x0400E229 RID: 57897
			public const short HitSpeed = 26;

			// Token: 0x0400E22A RID: 57898
			public const short HitMind = 27;

			// Token: 0x0400E22B RID: 57899
			public const short AvoidStrength = 33;

			// Token: 0x0400E22C RID: 57900
			public const short AvoidTechnique = 34;

			// Token: 0x0400E22D RID: 57901
			public const short AvoidSpeed = 35;

			// Token: 0x0400E22E RID: 57902
			public const short AvoidMind = 36;

			// Token: 0x0400E22F RID: 57903
			public const short QiDisorder = 55;

			// Token: 0x0400E230 RID: 57904
			public const short LifeSkill0 = 66;

			// Token: 0x0400E231 RID: 57905
			public const short LifeSkill1 = 67;

			// Token: 0x0400E232 RID: 57906
			public const short LifeSkill2 = 68;

			// Token: 0x0400E233 RID: 57907
			public const short LifeSkill3 = 69;

			// Token: 0x0400E234 RID: 57908
			public const short LifeSkill4 = 70;

			// Token: 0x0400E235 RID: 57909
			public const short LifeSkill5 = 71;

			// Token: 0x0400E236 RID: 57910
			public const short LifeSkill6 = 72;

			// Token: 0x0400E237 RID: 57911
			public const short LifeSkill7 = 73;

			// Token: 0x0400E238 RID: 57912
			public const short LifeSkill8 = 74;

			// Token: 0x0400E239 RID: 57913
			public const short LifeSkill9 = 75;

			// Token: 0x0400E23A RID: 57914
			public const short LifeSkill10 = 76;

			// Token: 0x0400E23B RID: 57915
			public const short LifeSkill11 = 77;

			// Token: 0x0400E23C RID: 57916
			public const short LifeSkill12 = 78;

			// Token: 0x0400E23D RID: 57917
			public const short LifeSkill13 = 79;

			// Token: 0x0400E23E RID: 57918
			public const short LifeSkill14 = 80;

			// Token: 0x0400E23F RID: 57919
			public const short LifeSkill15 = 81;

			// Token: 0x0400E240 RID: 57920
			public const short CombatSkill0 = 82;

			// Token: 0x0400E241 RID: 57921
			public const short CombatSkill1 = 83;

			// Token: 0x0400E242 RID: 57922
			public const short CombatSkill2 = 84;

			// Token: 0x0400E243 RID: 57923
			public const short CombatSkill3 = 85;

			// Token: 0x0400E244 RID: 57924
			public const short CombatSkill4 = 86;

			// Token: 0x0400E245 RID: 57925
			public const short CombatSkill5 = 87;

			// Token: 0x0400E246 RID: 57926
			public const short CombatSkill6 = 88;

			// Token: 0x0400E247 RID: 57927
			public const short CombatSkill7 = 89;

			// Token: 0x0400E248 RID: 57928
			public const short CombatSkill8 = 90;

			// Token: 0x0400E249 RID: 57929
			public const short CombatSkill9 = 91;

			// Token: 0x0400E24A RID: 57930
			public const short CombatSkill10 = 92;

			// Token: 0x0400E24B RID: 57931
			public const short CombatSkill11 = 93;

			// Token: 0x0400E24C RID: 57932
			public const short CombatSkill12 = 94;

			// Token: 0x0400E24D RID: 57933
			public const short CombatSkill13 = 95;

			// Token: 0x0400E24E RID: 57934
			public const short Personality0 = 96;

			// Token: 0x0400E24F RID: 57935
			public const short Personality1 = 97;

			// Token: 0x0400E250 RID: 57936
			public const short Personality2 = 98;

			// Token: 0x0400E251 RID: 57937
			public const short Personality3 = 99;

			// Token: 0x0400E252 RID: 57938
			public const short Personality4 = 100;

			// Token: 0x0400E253 RID: 57939
			public const short Personality5 = 101;

			// Token: 0x0400E254 RID: 57940
			public const short Personality6 = 102;

			// Token: 0x0400E255 RID: 57941
			public const short Resource0 = 103;

			// Token: 0x0400E256 RID: 57942
			public const short Resource1 = 104;

			// Token: 0x0400E257 RID: 57943
			public const short Resource2 = 105;

			// Token: 0x0400E258 RID: 57944
			public const short Resource3 = 106;

			// Token: 0x0400E259 RID: 57945
			public const short Resource4 = 107;

			// Token: 0x0400E25A RID: 57946
			public const short Resource5 = 108;

			// Token: 0x0400E25B RID: 57947
			public const short Resource6 = 109;

			// Token: 0x0400E25C RID: 57948
			public const short Resource7 = 110;

			// Token: 0x0400E25D RID: 57949
			public const short InventoryLoad = 37;

			// Token: 0x0400E25E RID: 57950
			public const short KidnapCount = 111;

			// Token: 0x0400E25F RID: 57951
			public const short AttackMedal = 112;

			// Token: 0x0400E260 RID: 57952
			public const short DefenceMedal = 113;

			// Token: 0x0400E261 RID: 57953
			public const short WisdomMedal = 114;

			// Token: 0x0400E262 RID: 57954
			public const short Command0 = 115;

			// Token: 0x0400E263 RID: 57955
			public const short Command1 = 116;

			// Token: 0x0400E264 RID: 57956
			public const short Command2 = 117;

			// Token: 0x0400E265 RID: 57957
			public const short Grade = 1;

			// Token: 0x0400E266 RID: 57958
			public const short Infection = 123;

			// Token: 0x0400E267 RID: 57959
			public const short TakeAfterMonth = 134;

			// Token: 0x0400E268 RID: 57960
			public const short TakeAmount = 135;

			// Token: 0x0400E269 RID: 57961
			public const short Relationship = 136;

			// Token: 0x0400E26A RID: 57962
			public const short WagerValue = 5;

			// Token: 0x0400E26B RID: 57963
			public const short OrganizationPower = 137;

			// Token: 0x0400E26C RID: 57964
			public const short ApprovingRate = 178;

			// Token: 0x0400E26D RID: 57965
			public const short ConsummateLevel = 143;

			// Token: 0x0400E26E RID: 57966
			public const short PunishmentSeverity = 15;

			// Token: 0x0400E26F RID: 57967
			public const short PunishmentBounty = 16;

			// Token: 0x0400E270 RID: 57968
			public const short OriginalGrade = 151;

			// Token: 0x0400E271 RID: 57969
			public const short WorkingStatus = 129;

			// Token: 0x0400E272 RID: 57970
			public const short WorkingLocation = 126;

			// Token: 0x0400E273 RID: 57971
			public const short WorkingRole = 127;

			// Token: 0x0400E274 RID: 57972
			public const short Potential = 128;

			// Token: 0x0400E275 RID: 57973
			public const short TripodVesselProtect = 202;

			// Token: 0x0400E276 RID: 57974
			public const short DarkAsh = 201;

			// Token: 0x0400E277 RID: 57975
			public const short DoctorLifeSkill = 205;

			// Token: 0x0400E278 RID: 57976
			public const short LiteratiLifeSkill = 206;

			// Token: 0x0400E279 RID: 57977
			public const short TombKeeperLifeSkill = 207;

			// Token: 0x0400E27A RID: 57978
			public const short HighestCombatSkill = 208;
		}
	}
}
