using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using Game.Components.SortAndFilter;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;

namespace Game.Views
{
	// Token: 0x0200071B RID: 1819
	public class VillagerSortController : SortController<VillagerCharDisplayData>
	{
		// Token: 0x060056A5 RID: 22181 RVA: 0x00281F4C File Offset: 0x0028014C
		public VillagerSortController(Func<int, bool> isTaiwuFunc, Func<int, bool> isSpecialTeammateFunc)
		{
			this._isTaiwuFunc = isTaiwuFunc;
			this._isSpecialTeammateFunc = isSpecialTeammateFunc;
		}

		// Token: 0x060056A6 RID: 22182 RVA: 0x00281F64 File Offset: 0x00280164
		public override Comparison<VillagerCharDisplayData> GenerateComparer(SortStateData sortData)
		{
			return (VillagerCharDisplayData x, VillagerCharDisplayData y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x060056A7 RID: 22183 RVA: 0x00281F98 File Offset: 0x00280198
		private int CompareData(VillagerCharDisplayData x, VillagerCharDisplayData y, SortStateData sortData)
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
						Func<int, bool> isTaiwuFunc = this._isTaiwuFunc;
						bool xIsTaiwu = isTaiwuFunc != null && isTaiwuFunc(x.CharacterId);
						Func<int, bool> isTaiwuFunc2 = this._isTaiwuFunc;
						bool yIsTaiwu = isTaiwuFunc2 != null && isTaiwuFunc2(y.CharacterId);
						bool flag4 = xIsTaiwu != yIsTaiwu;
						if (flag4)
						{
							result = (xIsTaiwu ? -1 : 1);
						}
						else
						{
							Func<int, bool> isSpecialTeammateFunc = this._isSpecialTeammateFunc;
							bool xIsSpecial = isSpecialTeammateFunc != null && isSpecialTeammateFunc(x.CharacterId);
							Func<int, bool> isSpecialTeammateFunc2 = this._isSpecialTeammateFunc;
							bool yIsSpecial = isSpecialTeammateFunc2 != null && isSpecialTeammateFunc2(y.CharacterId);
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
										int comparisonResult = this.CompareBySortId(x, y, sortId, order);
										bool flag7 = comparisonResult != 0;
										if (flag7)
										{
											return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
										}
									}
								}
								result = x.CharacterId.CompareTo(y.CharacterId);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060056A8 RID: 22184 RVA: 0x0028212C File Offset: 0x0028032C
		private int CompareBySortId(VillagerCharDisplayData x, VillagerCharDisplayData y, short sortId, ESortDirection order)
		{
			VillagerSortController.<>c__DisplayClass6_0 CS$<>8__locals1;
			CS$<>8__locals1.order = order;
			int result;
			if (sortId != 0)
			{
				switch (sortId)
				{
				case 8:
					result = x.PhysiologicalAge.CompareTo(y.PhysiologicalAge);
					break;
				case 9:
					result = this.GetCharmValue(x, CS$<>8__locals1.order).CompareTo(this.GetCharmValue(y, CS$<>8__locals1.order));
					break;
				case 10:
					result = this.GetHealthValue(x).CompareTo(this.GetHealthValue(y));
					break;
				case 11:
					result = VillagerSortController.<CompareBySortId>g__GetFavor|6_0(x, ref CS$<>8__locals1).CompareTo(VillagerSortController.<CompareBySortId>g__GetFavor|6_0(y, ref CS$<>8__locals1));
					break;
				case 12:
					result = x.Happiness.CompareTo(y.Happiness);
					break;
				default:
					result = this.CompareByPropertyIndex(x, y, sortId);
					break;
				}
			}
			else
			{
				result = x.CharacterId.CompareTo(y.CharacterId);
			}
			return result;
		}

		// Token: 0x060056A9 RID: 22185 RVA: 0x00282218 File Offset: 0x00280418
		public int GetCharmValue(VillagerCharDisplayData data, ESortDirection order)
		{
			bool flag = !data.FaceVisible;
			int result;
			if (flag)
			{
				result = ((order == ESortDirection.Ascending) ? int.MinValue : int.MaxValue);
			}
			else
			{
				bool isFixedCharacter = CreatingType.IsFixedPresetType(data.CreatingType);
				bool flag2 = !isFixedCharacter;
				if (flag2)
				{
					bool flag3 = data.PhysiologicalAge < 16;
					if (flag3)
					{
						return (order == ESortDirection.Ascending) ? -2147483647 : 2147483646;
					}
					bool flag4 = data.ClothDisplayId == 0;
					if (flag4)
					{
						return (order == ESortDirection.Ascending) ? -2147483646 : 2147483645;
					}
				}
				result = (int)data.Charm;
			}
			return result;
		}

		// Token: 0x060056AA RID: 22186 RVA: 0x002822AC File Offset: 0x002804AC
		private int GetHealthValue(VillagerCharDisplayData data)
		{
			EHealthType type = CommonUtils.GetHealthType(data.Health, data.MaxLeftHealth, data.CharacterId);
			long ratio = (data.MaxLeftHealth > 0) ? ((long)data.Health * 10000L / (long)data.MaxLeftHealth) : 0L;
			return (int)(type * (EHealthType)20000 + (int)ratio);
		}

		// Token: 0x060056AB RID: 22187 RVA: 0x00282304 File Offset: 0x00280504
		public static int WorkingRoleOrder(VillagerCharDisplayData x)
		{
			VillagerWorkData villagerWorkData = x.VillagerWorkData;
			return (villagerWorkData == null || villagerWorkData.WorkType != 1) ? -1 : ((x.VillagerWorkData.WorkerIndex == 0) ? 1 : 0);
		}

		// Token: 0x060056AC RID: 22188 RVA: 0x00282338 File Offset: 0x00280538
		public static int WorkingStatusOrder(VillagerCharDisplayData x)
		{
			VillagerWorkData villagerWorkData = x.VillagerWorkData;
			sbyte? b = (villagerWorkData != null) ? new sbyte?(villagerWorkData.WorkType) : null;
			if (!true)
			{
			}
			int result;
			if (b != null)
			{
				sbyte valueOrDefault = b.GetValueOrDefault();
				switch (valueOrDefault)
				{
				case 0:
					result = 4;
					goto IL_6F;
				case 1:
					result = 2;
					goto IL_6F;
				case 2:
					result = 3;
					goto IL_6F;
				default:
					if (valueOrDefault == 12)
					{
						result = 1;
						goto IL_6F;
					}
					if (valueOrDefault == 13)
					{
						result = 0;
						goto IL_6F;
					}
					break;
				}
			}
			result = -1;
			IL_6F:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060056AD RID: 22189 RVA: 0x002823BC File Offset: 0x002805BC
		private unsafe int CompareByPropertyIndex(VillagerCharDisplayData x, VillagerCharDisplayData y, short sortId)
		{
			switch (sortId)
			{
			case 1:
			{
				int num = (int)(x.OrgInfo.Grade * 2);
				short physiologicalAge = x.PhysiologicalAge;
				int num2 = num + ((physiologicalAge >= 0 && physiologicalAge <= 16) ? 0 : 1);
				int num3 = (int)(y.OrgInfo.Grade * 2);
				physiologicalAge = x.PhysiologicalAge;
				return num2.CompareTo(num3 + ((physiologicalAge >= 0 && physiologicalAge <= 16) ? 0 : 1));
			}
			case 22:
				return x.Penetrations.Outer.CompareTo(y.Penetrations.Outer);
			case 23:
				return x.Penetrations.Inner.CompareTo(y.Penetrations.Inner);
			case 24:
			{
				int num2 = x.HitValues[0];
				return num2.CompareTo(y.HitValues[0]);
			}
			case 25:
			{
				int num2 = x.HitValues[1];
				return num2.CompareTo(y.HitValues[1]);
			}
			case 26:
			{
				int num2 = x.HitValues[2];
				return num2.CompareTo(y.HitValues[2]);
			}
			case 27:
			{
				int num2 = x.HitValues[3];
				return num2.CompareTo(y.HitValues[3]);
			}
			case 29:
				return x.PenetrationResists.Outer.CompareTo(y.PenetrationResists.Outer);
			case 30:
				return x.PenetrationResists.Inner.CompareTo(y.PenetrationResists.Inner);
			case 33:
			{
				int num2 = x.AvoidValues[0];
				return num2.CompareTo(y.AvoidValues[0]);
			}
			case 34:
			{
				int num2 = x.AvoidValues[1];
				return num2.CompareTo(y.AvoidValues[1]);
			}
			case 35:
			{
				int num2 = x.AvoidValues[2];
				return num2.CompareTo(y.AvoidValues[2]);
			}
			case 36:
			{
				int num2 = x.AvoidValues[3];
				return num2.CompareTo(y.AvoidValues[3]);
			}
			case 37:
			{
				int valX = (x.MaxInventoryLoad > 0) ? ((int)((long)x.CurrInventoryLoad * 10000L / (long)x.MaxInventoryLoad)) : 0;
				int valY = (y.MaxInventoryLoad > 0) ? ((int)((long)y.CurrInventoryLoad * 10000L / (long)y.MaxInventoryLoad)) : 0;
				return valX.CompareTo(valY);
			}
			case 53:
				return x.DefeatMarkCount.CompareTo(y.DefeatMarkCount);
			case 55:
				return x.DisorderOfQi.CompareTo(y.DisorderOfQi);
			case 57:
				return x.BehaviorType.CompareTo(y.BehaviorType);
			case 58:
				return x.PreexistenceCharCount.CompareTo(y.PreexistenceCharCount);
			case 59:
				return x.Fame.CompareTo(y.Fame);
			case 60:
				return x.MaxMainAttributes[0].CompareTo(*y.MaxMainAttributes[0]);
			case 61:
				return x.MaxMainAttributes[1].CompareTo(*y.MaxMainAttributes[1]);
			case 62:
				return x.MaxMainAttributes[2].CompareTo(*y.MaxMainAttributes[2]);
			case 63:
				return x.MaxMainAttributes[3].CompareTo(*y.MaxMainAttributes[3]);
			case 64:
				return x.MaxMainAttributes[4].CompareTo(*y.MaxMainAttributes[4]);
			case 65:
				return x.MaxMainAttributes[5].CompareTo(*y.MaxMainAttributes[5]);
			case 66:
				return x.LifeSkillQualifications[0].CompareTo(*y.LifeSkillQualifications[0]);
			case 67:
				return x.LifeSkillQualifications[1].CompareTo(*y.LifeSkillQualifications[1]);
			case 68:
				return x.LifeSkillQualifications[2].CompareTo(*y.LifeSkillQualifications[2]);
			case 69:
				return x.LifeSkillQualifications[3].CompareTo(*y.LifeSkillQualifications[3]);
			case 70:
				return x.LifeSkillQualifications[4].CompareTo(*y.LifeSkillQualifications[4]);
			case 71:
				return x.LifeSkillQualifications[5].CompareTo(*y.LifeSkillQualifications[5]);
			case 72:
				return x.LifeSkillQualifications[6].CompareTo(*y.LifeSkillQualifications[6]);
			case 73:
				return x.LifeSkillQualifications[7].CompareTo(*y.LifeSkillQualifications[7]);
			case 74:
				return x.LifeSkillQualifications[8].CompareTo(*y.LifeSkillQualifications[8]);
			case 75:
				return x.LifeSkillQualifications[9].CompareTo(*y.LifeSkillQualifications[9]);
			case 76:
				return x.LifeSkillQualifications[10].CompareTo(*y.LifeSkillQualifications[10]);
			case 77:
				return x.LifeSkillQualifications[11].CompareTo(*y.LifeSkillQualifications[11]);
			case 78:
				return x.LifeSkillQualifications[12].CompareTo(*y.LifeSkillQualifications[12]);
			case 79:
				return x.LifeSkillQualifications[13].CompareTo(*y.LifeSkillQualifications[13]);
			case 80:
				return x.LifeSkillQualifications[14].CompareTo(*y.LifeSkillQualifications[14]);
			case 81:
				return x.LifeSkillQualifications[15].CompareTo(*y.LifeSkillQualifications[15]);
			case 82:
				return x.CombatSkillQualifications[0].CompareTo(*y.CombatSkillQualifications[0]);
			case 83:
				return x.CombatSkillQualifications[1].CompareTo(*y.CombatSkillQualifications[1]);
			case 84:
				return x.CombatSkillQualifications[2].CompareTo(*y.CombatSkillQualifications[2]);
			case 85:
				return x.CombatSkillQualifications[3].CompareTo(*y.CombatSkillQualifications[3]);
			case 86:
				return x.CombatSkillQualifications[4].CompareTo(*y.CombatSkillQualifications[4]);
			case 87:
				return x.CombatSkillQualifications[5].CompareTo(*y.CombatSkillQualifications[5]);
			case 88:
				return x.CombatSkillQualifications[6].CompareTo(*y.CombatSkillQualifications[6]);
			case 89:
				return x.CombatSkillQualifications[7].CompareTo(*y.CombatSkillQualifications[7]);
			case 90:
				return x.CombatSkillQualifications[8].CompareTo(*y.CombatSkillQualifications[8]);
			case 91:
				return x.CombatSkillQualifications[9].CompareTo(*y.CombatSkillQualifications[9]);
			case 92:
				return x.CombatSkillQualifications[10].CompareTo(*y.CombatSkillQualifications[10]);
			case 93:
				return x.CombatSkillQualifications[11].CompareTo(*y.CombatSkillQualifications[11]);
			case 94:
				return x.CombatSkillQualifications[12].CompareTo(*y.CombatSkillQualifications[12]);
			case 95:
				return x.CombatSkillQualifications[13].CompareTo(*y.CombatSkillQualifications[13]);
			case 96:
				return x.Personalities[0].CompareTo(*y.Personalities[0]);
			case 97:
				return x.Personalities[1].CompareTo(*y.Personalities[1]);
			case 98:
				return x.Personalities[2].CompareTo(*y.Personalities[2]);
			case 99:
				return x.Personalities[3].CompareTo(*y.Personalities[3]);
			case 100:
				return x.Personalities[4].CompareTo(*y.Personalities[4]);
			case 101:
				return x.Personalities[5].CompareTo(*y.Personalities[5]);
			case 102:
				return x.Personalities[6].CompareTo(*y.Personalities[6]);
			case 103:
				return x.Resources[0].CompareTo(*y.Resources[0]);
			case 104:
				return x.Resources[1].CompareTo(*y.Resources[1]);
			case 105:
				return x.Resources[2].CompareTo(*y.Resources[2]);
			case 106:
				return x.Resources[3].CompareTo(*y.Resources[3]);
			case 107:
				return x.Resources[4].CompareTo(*y.Resources[4]);
			case 108:
				return x.Resources[5].CompareTo(*y.Resources[5]);
			case 109:
				return x.Resources[6].CompareTo(*y.Resources[6]);
			case 110:
				return x.Resources[7].CompareTo(*y.Resources[7]);
			case 111:
				return x.KidnapCount.CompareTo(y.KidnapCount);
			case 112:
				return x.AttackMedal.CompareTo(y.AttackMedal);
			case 113:
				return x.DefenceMedal.CompareTo(y.DefenceMedal);
			case 114:
				return x.WisdomMedal.CompareTo(y.WisdomMedal);
			case 115:
			{
				int num2 = this.GetCommandValue(x, 0);
				return num2.CompareTo(this.GetCommandValue(y, 0));
			}
			case 116:
			{
				int num2 = this.GetCommandValue(x, 1);
				return num2.CompareTo(this.GetCommandValue(y, 1));
			}
			case 117:
			{
				int num2 = this.GetCommandValue(x, 2);
				return num2.CompareTo(this.GetCommandValue(y, 2));
			}
			case 118:
				return VillagerSortController.GetSkillGrowthAddValue(x.ActualAge, (int)x.LifeSkillGrowthType).CompareTo(VillagerSortController.GetSkillGrowthAddValue(y.ActualAge, (int)y.LifeSkillGrowthType));
			case 119:
				return VillagerSortController.GetSkillGrowthAddValue(x.ActualAge, (int)x.CombatSkillGrowthType).CompareTo(VillagerSortController.GetSkillGrowthAddValue(y.ActualAge, (int)y.CombatSkillGrowthType));
			case 125:
			{
				int ret = x.Location.AreaId.CompareTo(y.Location.AreaId);
				return (ret != 0) ? ret : x.Location.BlockId.CompareTo(y.Location.BlockId);
			}
			case 126:
				if (x.VillagerWorkData == null && y.VillagerWorkData == null)
				{
					return 0;
				}
				if (x.VillagerWorkData == null)
				{
					return -1;
				}
				if (y.VillagerWorkData != null)
				{
					int ret = x.VillagerWorkData.BlockTemplateId.CompareTo(y.VillagerWorkData.BlockTemplateId);
					return (ret != 0) ? ret : x.VillagerWorkData.BlockId.CompareTo(y.VillagerWorkData.BlockId);
				}
				return 1;
			case 127:
			{
				int num2 = VillagerSortController.WorkingRoleOrder(x);
				return num2.CompareTo(VillagerSortController.WorkingRoleOrder(y));
			}
			case 129:
			{
				int num2 = VillagerSortController.WorkingStatusOrder(x);
				return num2.CompareTo(VillagerSortController.WorkingStatusOrder(y));
			}
			}
			return 0;
		}

		// Token: 0x060056AE RID: 22190 RVA: 0x00283260 File Offset: 0x00281460
		private int GetCommandValue(VillagerCharDisplayData data, int commandIndex)
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

		// Token: 0x060056AF RID: 22191 RVA: 0x002832B0 File Offset: 0x002814B0
		private static sbyte GetSkillGrowthAddValue(short actualAge, int growthType)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Math.Min((int)actualAge, AgeEffect.Instance.Count - 1)];
			return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
		}

		// Token: 0x060056B0 RID: 22192 RVA: 0x002832FC File Offset: 0x002814FC
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				1,
				125,
				126,
				127,
				129,
				0,
				8,
				10,
				9,
				12,
				11,
				53,
				57,
				58,
				59,
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
				117
			};
			return new SortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = new List<int>(new int[sortIds.Count]),
				DefaultSortIds = new List<short>
				{
					0
				},
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

		// Token: 0x060056B1 RID: 22193 RVA: 0x002836B9 File Offset: 0x002818B9
		[CompilerGenerated]
		internal static short <CompareBySortId>g__GetFavor|6_0(VillagerCharDisplayData data, ref VillagerSortController.<>c__DisplayClass6_0 A_1)
		{
			return data.IsInteractedWithTaiwu ? data.FavorabilityToTaiwu : ((A_1.order == ESortDirection.Ascending) ? short.MinValue : short.MaxValue);
		}

		// Token: 0x04003B2C RID: 15148
		private readonly Func<int, bool> _isTaiwuFunc;

		// Token: 0x04003B2D RID: 15149
		private readonly Func<int, bool> _isSpecialTeammateFunc;

		// Token: 0x02001BA7 RID: 7079
		public static class SortIds
		{
			// Token: 0x0400BD47 RID: 48455
			public const short Grade = 1;

			// Token: 0x0400BD48 RID: 48456
			public const short WorkingStatus = 129;

			// Token: 0x0400BD49 RID: 48457
			public const short Location = 125;

			// Token: 0x0400BD4A RID: 48458
			public const short WorkingLocation = 126;

			// Token: 0x0400BD4B RID: 48459
			public const short WorkingRole = 127;

			// Token: 0x0400BD4C RID: 48460
			public const short Potential = 128;

			// Token: 0x0400BD4D RID: 48461
			public const short Name = 0;

			// Token: 0x0400BD4E RID: 48462
			public const short Age = 8;

			// Token: 0x0400BD4F RID: 48463
			public const short Health = 10;

			// Token: 0x0400BD50 RID: 48464
			public const short Charm = 9;

			// Token: 0x0400BD51 RID: 48465
			public const short Happiness = 12;

			// Token: 0x0400BD52 RID: 48466
			public const short Favor = 11;

			// Token: 0x0400BD53 RID: 48467
			public const short Alertness = 130;

			// Token: 0x0400BD54 RID: 48468
			public const short DefeatMark = 53;

			// Token: 0x0400BD55 RID: 48469
			public const short Behavior = 57;

			// Token: 0x0400BD56 RID: 48470
			public const short Samsara = 58;

			// Token: 0x0400BD57 RID: 48471
			public const short Fame = 59;

			// Token: 0x0400BD58 RID: 48472
			public const short Strength = 60;

			// Token: 0x0400BD59 RID: 48473
			public const short Dexterity = 61;

			// Token: 0x0400BD5A RID: 48474
			public const short Concentration = 62;

			// Token: 0x0400BD5B RID: 48475
			public const short Vitality = 63;

			// Token: 0x0400BD5C RID: 48476
			public const short Energy = 64;

			// Token: 0x0400BD5D RID: 48477
			public const short Intelligence = 65;

			// Token: 0x0400BD5E RID: 48478
			public const short OuterPenetrate = 22;

			// Token: 0x0400BD5F RID: 48479
			public const short InnerPenetrate = 23;

			// Token: 0x0400BD60 RID: 48480
			public const short OuterPenetrateResist = 29;

			// Token: 0x0400BD61 RID: 48481
			public const short InnerPenetrateResist = 30;

			// Token: 0x0400BD62 RID: 48482
			public const short HitStrength = 24;

			// Token: 0x0400BD63 RID: 48483
			public const short HitTechnique = 25;

			// Token: 0x0400BD64 RID: 48484
			public const short HitSpeed = 26;

			// Token: 0x0400BD65 RID: 48485
			public const short HitMind = 27;

			// Token: 0x0400BD66 RID: 48486
			public const short AvoidStrength = 33;

			// Token: 0x0400BD67 RID: 48487
			public const short AvoidTechnique = 34;

			// Token: 0x0400BD68 RID: 48488
			public const short AvoidSpeed = 35;

			// Token: 0x0400BD69 RID: 48489
			public const short AvoidMind = 36;

			// Token: 0x0400BD6A RID: 48490
			public const short QiDisorder = 55;

			// Token: 0x0400BD6B RID: 48491
			public const short LifeSkill0 = 66;

			// Token: 0x0400BD6C RID: 48492
			public const short LifeSkill1 = 67;

			// Token: 0x0400BD6D RID: 48493
			public const short LifeSkill2 = 68;

			// Token: 0x0400BD6E RID: 48494
			public const short LifeSkill3 = 69;

			// Token: 0x0400BD6F RID: 48495
			public const short LifeSkill4 = 70;

			// Token: 0x0400BD70 RID: 48496
			public const short LifeSkill5 = 71;

			// Token: 0x0400BD71 RID: 48497
			public const short LifeSkill6 = 72;

			// Token: 0x0400BD72 RID: 48498
			public const short LifeSkill7 = 73;

			// Token: 0x0400BD73 RID: 48499
			public const short LifeSkill8 = 74;

			// Token: 0x0400BD74 RID: 48500
			public const short LifeSkill9 = 75;

			// Token: 0x0400BD75 RID: 48501
			public const short LifeSkill10 = 76;

			// Token: 0x0400BD76 RID: 48502
			public const short LifeSkill11 = 77;

			// Token: 0x0400BD77 RID: 48503
			public const short LifeSkill12 = 78;

			// Token: 0x0400BD78 RID: 48504
			public const short LifeSkill13 = 79;

			// Token: 0x0400BD79 RID: 48505
			public const short LifeSkill14 = 80;

			// Token: 0x0400BD7A RID: 48506
			public const short LifeSkill15 = 81;

			// Token: 0x0400BD7B RID: 48507
			public const short LifeSkillGrowth = 118;

			// Token: 0x0400BD7C RID: 48508
			public const short CombatSkill0 = 82;

			// Token: 0x0400BD7D RID: 48509
			public const short CombatSkill1 = 83;

			// Token: 0x0400BD7E RID: 48510
			public const short CombatSkill2 = 84;

			// Token: 0x0400BD7F RID: 48511
			public const short CombatSkill3 = 85;

			// Token: 0x0400BD80 RID: 48512
			public const short CombatSkill4 = 86;

			// Token: 0x0400BD81 RID: 48513
			public const short CombatSkill5 = 87;

			// Token: 0x0400BD82 RID: 48514
			public const short CombatSkill6 = 88;

			// Token: 0x0400BD83 RID: 48515
			public const short CombatSkill7 = 89;

			// Token: 0x0400BD84 RID: 48516
			public const short CombatSkill8 = 90;

			// Token: 0x0400BD85 RID: 48517
			public const short CombatSkill9 = 91;

			// Token: 0x0400BD86 RID: 48518
			public const short CombatSkill10 = 92;

			// Token: 0x0400BD87 RID: 48519
			public const short CombatSkill11 = 93;

			// Token: 0x0400BD88 RID: 48520
			public const short CombatSkill12 = 94;

			// Token: 0x0400BD89 RID: 48521
			public const short CombatSkill13 = 95;

			// Token: 0x0400BD8A RID: 48522
			public const short CombatSkillGrowth = 119;

			// Token: 0x0400BD8B RID: 48523
			public const short Personality0 = 96;

			// Token: 0x0400BD8C RID: 48524
			public const short Personality1 = 97;

			// Token: 0x0400BD8D RID: 48525
			public const short Personality2 = 98;

			// Token: 0x0400BD8E RID: 48526
			public const short Personality3 = 99;

			// Token: 0x0400BD8F RID: 48527
			public const short Personality4 = 100;

			// Token: 0x0400BD90 RID: 48528
			public const short Personality5 = 101;

			// Token: 0x0400BD91 RID: 48529
			public const short Personality6 = 102;

			// Token: 0x0400BD92 RID: 48530
			public const short Resource0 = 103;

			// Token: 0x0400BD93 RID: 48531
			public const short Resource1 = 104;

			// Token: 0x0400BD94 RID: 48532
			public const short Resource2 = 105;

			// Token: 0x0400BD95 RID: 48533
			public const short Resource3 = 106;

			// Token: 0x0400BD96 RID: 48534
			public const short Resource4 = 107;

			// Token: 0x0400BD97 RID: 48535
			public const short Resource5 = 108;

			// Token: 0x0400BD98 RID: 48536
			public const short Resource6 = 109;

			// Token: 0x0400BD99 RID: 48537
			public const short Resource7 = 110;

			// Token: 0x0400BD9A RID: 48538
			public const short InventoryLoad = 37;

			// Token: 0x0400BD9B RID: 48539
			public const short KidnapCount = 111;

			// Token: 0x0400BD9C RID: 48540
			public const short AttackMedal = 112;

			// Token: 0x0400BD9D RID: 48541
			public const short DefenceMedal = 113;

			// Token: 0x0400BD9E RID: 48542
			public const short WisdomMedal = 114;

			// Token: 0x0400BD9F RID: 48543
			public const short Command0 = 115;

			// Token: 0x0400BDA0 RID: 48544
			public const short Command1 = 116;

			// Token: 0x0400BDA1 RID: 48545
			public const short Command2 = 117;
		}
	}
}
