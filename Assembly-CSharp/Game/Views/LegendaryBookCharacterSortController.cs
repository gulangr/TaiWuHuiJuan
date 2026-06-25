using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using Game.Components.SortAndFilter;
using GameData.Domains.Character.Creation;
using GameData.Domains.LegendaryBook;

namespace Game.Views
{
	// Token: 0x020006EF RID: 1775
	public class LegendaryBookCharacterSortController : SortController<LegendaryBookCharacterRelatedData>
	{
		// Token: 0x0600544C RID: 21580 RVA: 0x00270870 File Offset: 0x0026EA70
		public LegendaryBookCharacterSortController(Func<int, bool> isTaiwuFunc, Func<int, bool> isSpecialTeammateFunc)
		{
			this._isTaiwuFunc = isTaiwuFunc;
			this._isSpecialTeammateFunc = isSpecialTeammateFunc;
		}

		// Token: 0x0600544D RID: 21581 RVA: 0x00270888 File Offset: 0x0026EA88
		public override Comparison<LegendaryBookCharacterRelatedData> GenerateComparer(SortStateData sortData)
		{
			return (LegendaryBookCharacterRelatedData x, LegendaryBookCharacterRelatedData y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600544E RID: 21582 RVA: 0x002708BC File Offset: 0x0026EABC
		private int CompareData(LegendaryBookCharacterRelatedData x, LegendaryBookCharacterRelatedData y, SortStateData sortData)
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
						bool xIsTaiwu = isTaiwuFunc != null && isTaiwuFunc(x.Id);
						Func<int, bool> isTaiwuFunc2 = this._isTaiwuFunc;
						bool yIsTaiwu = isTaiwuFunc2 != null && isTaiwuFunc2(y.Id);
						bool flag4 = xIsTaiwu != yIsTaiwu;
						if (flag4)
						{
							result = (xIsTaiwu ? -1 : 1);
						}
						else
						{
							Func<int, bool> isSpecialTeammateFunc = this._isSpecialTeammateFunc;
							bool xIsSpecial = isSpecialTeammateFunc != null && isSpecialTeammateFunc(x.Id);
							Func<int, bool> isSpecialTeammateFunc2 = this._isSpecialTeammateFunc;
							bool yIsSpecial = isSpecialTeammateFunc2 != null && isSpecialTeammateFunc2(y.Id);
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
								result = x.Id.CompareTo(y.Id);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600544F RID: 21583 RVA: 0x00270A50 File Offset: 0x0026EC50
		private int CompareBySortId(LegendaryBookCharacterRelatedData x, LegendaryBookCharacterRelatedData y, short sortId, ESortDirection order)
		{
			LegendaryBookCharacterSortController.<>c__DisplayClass6_0 CS$<>8__locals1;
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
					result = LegendaryBookCharacterSortController.<CompareBySortId>g__GetFavor|6_0(x, ref CS$<>8__locals1).CompareTo(LegendaryBookCharacterSortController.<CompareBySortId>g__GetFavor|6_0(y, ref CS$<>8__locals1));
					break;
				case 12:
					result = x.HappinessType.CompareTo(y.HappinessType);
					break;
				default:
					result = this.CompareByPropertyIndex(x, y, sortId);
					break;
				}
			}
			else
			{
				result = x.Id.CompareTo(y.Id);
			}
			return result;
		}

		// Token: 0x06005450 RID: 21584 RVA: 0x00270B3C File Offset: 0x0026ED3C
		public int GetCharmValue(LegendaryBookCharacterRelatedData data, ESortDirection order)
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

		// Token: 0x06005451 RID: 21585 RVA: 0x00270BD0 File Offset: 0x0026EDD0
		private int GetHealthValue(LegendaryBookCharacterRelatedData data)
		{
			sbyte type = data.HealthType;
			long ratio = (data.MaxLeftHealth > 0) ? ((long)data.Health * 10000L / (long)data.MaxLeftHealth) : 0L;
			return (int)type * 20000 + (int)ratio;
		}

		// Token: 0x06005452 RID: 21586 RVA: 0x00270C18 File Offset: 0x0026EE18
		private unsafe int CompareByPropertyIndex(LegendaryBookCharacterRelatedData x, LegendaryBookCharacterRelatedData y, short sortId)
		{
			switch (sortId)
			{
			case 1:
			{
				int num = (int)(x.OrganizationInfo.Grade * 2);
				short physiologicalAge = x.PhysiologicalAge;
				int num2 = num + ((physiologicalAge >= 0 && physiologicalAge <= 16) ? 0 : 1);
				int num3 = (int)(y.OrganizationInfo.Grade * 2);
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
				return x.FameType.CompareTo(y.FameType);
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
				return LegendaryBookCharacterSortController.GetSkillGrowthAddValue(x.ActualAge, (int)x.LifeSkillGrowthType).CompareTo(LegendaryBookCharacterSortController.GetSkillGrowthAddValue(y.ActualAge, (int)y.LifeSkillGrowthType));
			case 119:
				return LegendaryBookCharacterSortController.GetSkillGrowthAddValue(x.ActualAge, (int)x.CombatSkillGrowthType).CompareTo(LegendaryBookCharacterSortController.GetSkillGrowthAddValue(y.ActualAge, (int)y.CombatSkillGrowthType));
			case 125:
			{
				int ret = x.Location.AreaId.CompareTo(y.Location.AreaId);
				return (ret != 0) ? ret : x.Location.BlockId.CompareTo(y.Location.BlockId);
			}
			case 126:
				return x.OrganizationInfo.OrgTemplateId.CompareTo(y.OrganizationInfo.OrgTemplateId);
			case 129:
				return x.BookType.CompareTo(y.BookType);
			case 143:
				return x.ConsummateLevel.CompareTo(y.ConsummateLevel);
			}
			return 0;
		}

		// Token: 0x06005453 RID: 21587 RVA: 0x00271A3C File Offset: 0x0026FC3C
		private int GetCommandValue(LegendaryBookCharacterRelatedData data, int commandIndex)
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

		// Token: 0x06005454 RID: 21588 RVA: 0x00271A8C File Offset: 0x0026FC8C
		private static sbyte GetSkillGrowthAddValue(short actualAge, int growthType)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Math.Min((int)actualAge, AgeEffect.Instance.Count - 1)];
			return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
		}

		// Token: 0x06005455 RID: 21589 RVA: 0x00271AD8 File Offset: 0x0026FCD8
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				1,
				125,
				129,
				127,
				126,
				143,
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

		// Token: 0x06005456 RID: 21590 RVA: 0x00271E85 File Offset: 0x00270085
		[CompilerGenerated]
		internal static short <CompareBySortId>g__GetFavor|6_0(LegendaryBookCharacterRelatedData data, ref LegendaryBookCharacterSortController.<>c__DisplayClass6_0 A_1)
		{
			return data.IsInteractedWithTaiwu ? data.Favorability : ((A_1.order == ESortDirection.Ascending) ? short.MinValue : short.MaxValue);
		}

		// Token: 0x04003914 RID: 14612
		private readonly Func<int, bool> _isTaiwuFunc;

		// Token: 0x04003915 RID: 14613
		private readonly Func<int, bool> _isSpecialTeammateFunc;

		// Token: 0x02001B42 RID: 6978
		public static class SortIds
		{
			// Token: 0x0400B995 RID: 47509
			public const short RoleGrade = 1;

			// Token: 0x0400B996 RID: 47510
			public const short BookType = 129;

			// Token: 0x0400B997 RID: 47511
			public const short Location = 125;

			// Token: 0x0400B998 RID: 47512
			public const short Sect = 126;

			// Token: 0x0400B999 RID: 47513
			public const short WorkingRole = 127;

			// Token: 0x0400B99A RID: 47514
			public const short ConsummateLevel = 143;

			// Token: 0x0400B99B RID: 47515
			public const short Name = 0;

			// Token: 0x0400B99C RID: 47516
			public const short Age = 8;

			// Token: 0x0400B99D RID: 47517
			public const short Health = 10;

			// Token: 0x0400B99E RID: 47518
			public const short Charm = 9;

			// Token: 0x0400B99F RID: 47519
			public const short Happiness = 12;

			// Token: 0x0400B9A0 RID: 47520
			public const short Favor = 11;

			// Token: 0x0400B9A1 RID: 47521
			public const short Alertness = 130;

			// Token: 0x0400B9A2 RID: 47522
			public const short DefeatMark = 53;

			// Token: 0x0400B9A3 RID: 47523
			public const short Behavior = 57;

			// Token: 0x0400B9A4 RID: 47524
			public const short Samsara = 58;

			// Token: 0x0400B9A5 RID: 47525
			public const short Fame = 59;

			// Token: 0x0400B9A6 RID: 47526
			public const short Strength = 60;

			// Token: 0x0400B9A7 RID: 47527
			public const short Dexterity = 61;

			// Token: 0x0400B9A8 RID: 47528
			public const short Concentration = 62;

			// Token: 0x0400B9A9 RID: 47529
			public const short Vitality = 63;

			// Token: 0x0400B9AA RID: 47530
			public const short Energy = 64;

			// Token: 0x0400B9AB RID: 47531
			public const short Intelligence = 65;

			// Token: 0x0400B9AC RID: 47532
			public const short OuterPenetrate = 22;

			// Token: 0x0400B9AD RID: 47533
			public const short InnerPenetrate = 23;

			// Token: 0x0400B9AE RID: 47534
			public const short OuterPenetrateResist = 29;

			// Token: 0x0400B9AF RID: 47535
			public const short InnerPenetrateResist = 30;

			// Token: 0x0400B9B0 RID: 47536
			public const short HitStrength = 24;

			// Token: 0x0400B9B1 RID: 47537
			public const short HitTechnique = 25;

			// Token: 0x0400B9B2 RID: 47538
			public const short HitSpeed = 26;

			// Token: 0x0400B9B3 RID: 47539
			public const short HitMind = 27;

			// Token: 0x0400B9B4 RID: 47540
			public const short AvoidStrength = 33;

			// Token: 0x0400B9B5 RID: 47541
			public const short AvoidTechnique = 34;

			// Token: 0x0400B9B6 RID: 47542
			public const short AvoidSpeed = 35;

			// Token: 0x0400B9B7 RID: 47543
			public const short AvoidMind = 36;

			// Token: 0x0400B9B8 RID: 47544
			public const short QiDisorder = 55;

			// Token: 0x0400B9B9 RID: 47545
			public const short LifeSkill0 = 66;

			// Token: 0x0400B9BA RID: 47546
			public const short LifeSkill1 = 67;

			// Token: 0x0400B9BB RID: 47547
			public const short LifeSkill2 = 68;

			// Token: 0x0400B9BC RID: 47548
			public const short LifeSkill3 = 69;

			// Token: 0x0400B9BD RID: 47549
			public const short LifeSkill4 = 70;

			// Token: 0x0400B9BE RID: 47550
			public const short LifeSkill5 = 71;

			// Token: 0x0400B9BF RID: 47551
			public const short LifeSkill6 = 72;

			// Token: 0x0400B9C0 RID: 47552
			public const short LifeSkill7 = 73;

			// Token: 0x0400B9C1 RID: 47553
			public const short LifeSkill8 = 74;

			// Token: 0x0400B9C2 RID: 47554
			public const short LifeSkill9 = 75;

			// Token: 0x0400B9C3 RID: 47555
			public const short LifeSkill10 = 76;

			// Token: 0x0400B9C4 RID: 47556
			public const short LifeSkill11 = 77;

			// Token: 0x0400B9C5 RID: 47557
			public const short LifeSkill12 = 78;

			// Token: 0x0400B9C6 RID: 47558
			public const short LifeSkill13 = 79;

			// Token: 0x0400B9C7 RID: 47559
			public const short LifeSkill14 = 80;

			// Token: 0x0400B9C8 RID: 47560
			public const short LifeSkill15 = 81;

			// Token: 0x0400B9C9 RID: 47561
			public const short LifeSkillGrowth = 118;

			// Token: 0x0400B9CA RID: 47562
			public const short CombatSkill0 = 82;

			// Token: 0x0400B9CB RID: 47563
			public const short CombatSkill1 = 83;

			// Token: 0x0400B9CC RID: 47564
			public const short CombatSkill2 = 84;

			// Token: 0x0400B9CD RID: 47565
			public const short CombatSkill3 = 85;

			// Token: 0x0400B9CE RID: 47566
			public const short CombatSkill4 = 86;

			// Token: 0x0400B9CF RID: 47567
			public const short CombatSkill5 = 87;

			// Token: 0x0400B9D0 RID: 47568
			public const short CombatSkill6 = 88;

			// Token: 0x0400B9D1 RID: 47569
			public const short CombatSkill7 = 89;

			// Token: 0x0400B9D2 RID: 47570
			public const short CombatSkill8 = 90;

			// Token: 0x0400B9D3 RID: 47571
			public const short CombatSkill9 = 91;

			// Token: 0x0400B9D4 RID: 47572
			public const short CombatSkill10 = 92;

			// Token: 0x0400B9D5 RID: 47573
			public const short CombatSkill11 = 93;

			// Token: 0x0400B9D6 RID: 47574
			public const short CombatSkill12 = 94;

			// Token: 0x0400B9D7 RID: 47575
			public const short CombatSkill13 = 95;

			// Token: 0x0400B9D8 RID: 47576
			public const short CombatSkillGrowth = 119;

			// Token: 0x0400B9D9 RID: 47577
			public const short Personality0 = 96;

			// Token: 0x0400B9DA RID: 47578
			public const short Personality1 = 97;

			// Token: 0x0400B9DB RID: 47579
			public const short Personality2 = 98;

			// Token: 0x0400B9DC RID: 47580
			public const short Personality3 = 99;

			// Token: 0x0400B9DD RID: 47581
			public const short Personality4 = 100;

			// Token: 0x0400B9DE RID: 47582
			public const short Personality5 = 101;

			// Token: 0x0400B9DF RID: 47583
			public const short Personality6 = 102;

			// Token: 0x0400B9E0 RID: 47584
			public const short Resource0 = 103;

			// Token: 0x0400B9E1 RID: 47585
			public const short Resource1 = 104;

			// Token: 0x0400B9E2 RID: 47586
			public const short Resource2 = 105;

			// Token: 0x0400B9E3 RID: 47587
			public const short Resource3 = 106;

			// Token: 0x0400B9E4 RID: 47588
			public const short Resource4 = 107;

			// Token: 0x0400B9E5 RID: 47589
			public const short Resource5 = 108;

			// Token: 0x0400B9E6 RID: 47590
			public const short Resource6 = 109;

			// Token: 0x0400B9E7 RID: 47591
			public const short Resource7 = 110;

			// Token: 0x0400B9E8 RID: 47592
			public const short InventoryLoad = 37;

			// Token: 0x0400B9E9 RID: 47593
			public const short KidnapCount = 111;

			// Token: 0x0400B9EA RID: 47594
			public const short AttackMedal = 112;

			// Token: 0x0400B9EB RID: 47595
			public const short DefenceMedal = 113;

			// Token: 0x0400B9EC RID: 47596
			public const short WisdomMedal = 114;

			// Token: 0x0400B9ED RID: 47597
			public const short Command0 = 115;

			// Token: 0x0400B9EE RID: 47598
			public const short Command1 = 116;

			// Token: 0x0400B9EF RID: 47599
			public const short Command2 = 117;
		}
	}
}
