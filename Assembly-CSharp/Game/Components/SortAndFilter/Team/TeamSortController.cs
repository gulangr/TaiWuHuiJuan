using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Team
{
	// Token: 0x02000CD9 RID: 3289
	public class TeamSortController : SortController<GroupCharDisplayData>
	{
		// Token: 0x0600A620 RID: 42528 RVA: 0x004D4BFB File Offset: 0x004D2DFB
		public TeamSortController(Func<int, bool> isTaiwuFunc)
		{
			this._isTaiwuFunc = isTaiwuFunc;
		}

		// Token: 0x0600A621 RID: 42529 RVA: 0x004D4C0C File Offset: 0x004D2E0C
		public override Comparison<GroupCharDisplayData> GenerateComparer(SortStateData sortData)
		{
			return (GroupCharDisplayData x, GroupCharDisplayData y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600A622 RID: 42530 RVA: 0x004D4C40 File Offset: 0x004D2E40
		private int CompareData(GroupCharDisplayData x, GroupCharDisplayData y, SortStateData sortData)
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
							bool flag5 = ((sortData != null) ? sortData.ItemStates : null) != null;
							if (flag5)
							{
								foreach (SortItemState itemState in sortData.ItemStates)
								{
									short sortId = itemState.SortId;
									ESortDirection order = itemState.SortDirection;
									int unknownComparison = this.CompareUnknownValues(x, y, sortId);
									bool flag6 = unknownComparison != 0;
									if (flag6)
									{
										return unknownComparison;
									}
									int comparisonResult = this.CompareBySortId(x, y, sortId);
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
			return result;
		}

		// Token: 0x0600A623 RID: 42531 RVA: 0x004D4D9C File Offset: 0x004D2F9C
		private int CompareBySortId(GroupCharDisplayData x, GroupCharDisplayData y, short sortId)
		{
			int result;
			if (sortId != 0)
			{
				switch (sortId)
				{
				case 8:
					result = x.PhysiologicalAge.CompareTo(y.PhysiologicalAge);
					break;
				case 9:
					result = this.GetCharmValue(x).CompareTo(this.GetCharmValue(y));
					break;
				case 10:
					result = this.GetHealthValue(x).CompareTo(this.GetHealthValue(y));
					break;
				case 11:
					result = x.FavorabilityToTaiwu.CompareTo(y.FavorabilityToTaiwu);
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

		// Token: 0x0600A624 RID: 42532 RVA: 0x004D4E64 File Offset: 0x004D3064
		private static int CompareWithUnknownOnTop(int xValue, int yValue)
		{
			bool xIsUnknown = xValue == int.MinValue;
			bool yIsUnknown = yValue == int.MinValue;
			bool flag = xIsUnknown && yIsUnknown;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = xIsUnknown;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					bool flag3 = yIsUnknown;
					if (flag3)
					{
						result = 1;
					}
					else
					{
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x0600A625 RID: 42533 RVA: 0x004D4EAC File Offset: 0x004D30AC
		private static int CompareFavorWithUnknownOnTop(GroupCharDisplayData x, GroupCharDisplayData y)
		{
			bool xIsUnknown = !x.IsInteractedWithTaiwu;
			bool yIsUnknown = !y.IsInteractedWithTaiwu;
			bool flag = xIsUnknown && yIsUnknown;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = xIsUnknown;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					bool flag3 = yIsUnknown;
					if (flag3)
					{
						result = 1;
					}
					else
					{
						result = x.FavorabilityToTaiwu.CompareTo(y.FavorabilityToTaiwu);
					}
				}
			}
			return result;
		}

		// Token: 0x0600A626 RID: 42534 RVA: 0x004D4F04 File Offset: 0x004D3104
		private int CompareCommandWithUnknownOnTop(GroupCharDisplayData x, GroupCharDisplayData y, int commandIndex)
		{
			int xCommand = this.GetCommandValue(x, commandIndex);
			int yCommand = this.GetCommandValue(y, commandIndex);
			bool xIsUnknown = xCommand < 0;
			bool yIsUnknown = yCommand < 0;
			bool flag = xIsUnknown && yIsUnknown;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = xIsUnknown;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					bool flag3 = yIsUnknown;
					if (flag3)
					{
						result = 1;
					}
					else
					{
						result = xCommand.CompareTo(yCommand);
					}
				}
			}
			return result;
		}

		// Token: 0x0600A627 RID: 42535 RVA: 0x004D4F64 File Offset: 0x004D3164
		private int CompareUnknownValues(GroupCharDisplayData x, GroupCharDisplayData y, short sortId)
		{
			int result;
			if (sortId != 9)
			{
				if (sortId != 11)
				{
					switch (sortId)
					{
					case 115:
						result = this.CompareCommandWithUnknownOnTop(x, y, 0);
						break;
					case 116:
						result = this.CompareCommandWithUnknownOnTop(x, y, 1);
						break;
					case 117:
						result = this.CompareCommandWithUnknownOnTop(x, y, 2);
						break;
					default:
						result = 0;
						break;
					}
				}
				else
				{
					result = TeamSortController.CompareFavorWithUnknownOnTop(x, y);
				}
			}
			else
			{
				int xValue = this.GetCharmValue(x);
				int yValue = this.GetCharmValue(y);
				result = TeamSortController.CompareWithUnknownOnTop(xValue, yValue);
			}
			return result;
		}

		// Token: 0x0600A628 RID: 42536 RVA: 0x004D4FF0 File Offset: 0x004D31F0
		private unsafe int CompareByPropertyIndex(GroupCharDisplayData x, GroupCharDisplayData y, short sortId)
		{
			switch (sortId)
			{
			case 22:
				return x.Penetrations.Outer.CompareTo(y.Penetrations.Outer);
			case 23:
				return x.Penetrations.Inner.CompareTo(y.Penetrations.Inner);
			case 24:
				return x.HitValues[0].CompareTo(y.HitValues[0]);
			case 25:
				return x.HitValues[1].CompareTo(y.HitValues[1]);
			case 26:
				return x.HitValues[2].CompareTo(y.HitValues[2]);
			case 27:
				return x.HitValues[3].CompareTo(y.HitValues[3]);
			case 29:
				return x.PenetrationResists.Outer.CompareTo(y.PenetrationResists.Outer);
			case 30:
				return x.PenetrationResists.Inner.CompareTo(y.PenetrationResists.Inner);
			case 33:
				return x.AvoidValues[0].CompareTo(y.AvoidValues[0]);
			case 34:
				return x.AvoidValues[1].CompareTo(y.AvoidValues[1]);
			case 35:
				return x.AvoidValues[2].CompareTo(y.AvoidValues[2]);
			case 36:
				return x.AvoidValues[3].CompareTo(y.AvoidValues[3]);
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
				return this.GetCommandValue(x, 0).CompareTo(this.GetCommandValue(y, 0));
			case 116:
				return this.GetCommandValue(x, 1).CompareTo(this.GetCommandValue(y, 1));
			case 117:
				return this.GetCommandValue(x, 2).CompareTo(this.GetCommandValue(y, 2));
			case 118:
				return TeamSortController.GetSkillGrowthAddValue(x.ActualAge, (int)x.LifeSkillGrowthType).CompareTo(TeamSortController.GetSkillGrowthAddValue(y.ActualAge, (int)y.LifeSkillGrowthType));
			case 119:
				return TeamSortController.GetSkillGrowthAddValue(x.ActualAge, (int)x.CombatSkillGrowthType).CompareTo(TeamSortController.GetSkillGrowthAddValue(y.ActualAge, (int)y.CombatSkillGrowthType));
			case 130:
				return x.Alertness.CompareTo(y.Alertness);
			}
			return 0;
		}

		// Token: 0x0600A629 RID: 42537 RVA: 0x004D5CAC File Offset: 0x004D3EAC
		private int GetCommandValue(GroupCharDisplayData data, int commandIndex)
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

		// Token: 0x0600A62A RID: 42538 RVA: 0x004D5CFC File Offset: 0x004D3EFC
		private static sbyte GetSkillGrowthAddValue(short actualAge, int growthType)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Math.Min((int)actualAge, AgeEffect.Instance.Count - 1)];
			return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
		}

		// Token: 0x0600A62B RID: 42539 RVA: 0x004D5D48 File Offset: 0x004D3F48
		private int GetCharmValue(GroupCharDisplayData data)
		{
			bool flag = !data.FaceVisible;
			int result;
			if (flag)
			{
				result = int.MinValue;
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
						return int.MinValue;
					}
					bool flag4 = data.ClothDisplayId == 0;
					if (flag4)
					{
						return int.MinValue;
					}
				}
				result = (int)data.Charm;
			}
			return result;
		}

		// Token: 0x0600A62C RID: 42540 RVA: 0x004D5DBC File Offset: 0x004D3FBC
		private int GetHealthValue(GroupCharDisplayData data)
		{
			EHealthType type = CommonUtils.GetHealthType(data.Health, data.MaxLeftHealth, data.CharacterId);
			long ratio = (data.MaxLeftHealth > 0) ? ((long)data.Health * 10000L / (long)data.MaxLeftHealth) : 0L;
			return (int)(type * (EHealthType)20000 + (int)ratio);
		}

		// Token: 0x0600A62D RID: 42541 RVA: 0x004D5E14 File Offset: 0x004D4014
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
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
				130,
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

		// Token: 0x04008308 RID: 33544
		private readonly Func<int, bool> _isTaiwuFunc;

		// Token: 0x0200241E RID: 9246
		public static class SortIds
		{
			// Token: 0x0400E172 RID: 57714
			public const short Name = 0;

			// Token: 0x0400E173 RID: 57715
			public const short Age = 8;

			// Token: 0x0400E174 RID: 57716
			public const short Health = 10;

			// Token: 0x0400E175 RID: 57717
			public const short Charm = 9;

			// Token: 0x0400E176 RID: 57718
			public const short Happiness = 12;

			// Token: 0x0400E177 RID: 57719
			public const short Favor = 11;

			// Token: 0x0400E178 RID: 57720
			public const short DefeatMark = 53;

			// Token: 0x0400E179 RID: 57721
			public const short Behavior = 57;

			// Token: 0x0400E17A RID: 57722
			public const short Samsara = 58;

			// Token: 0x0400E17B RID: 57723
			public const short Fame = 59;

			// Token: 0x0400E17C RID: 57724
			public const short Alertness = 130;

			// Token: 0x0400E17D RID: 57725
			public const short Strength = 60;

			// Token: 0x0400E17E RID: 57726
			public const short Dexterity = 61;

			// Token: 0x0400E17F RID: 57727
			public const short Concentration = 62;

			// Token: 0x0400E180 RID: 57728
			public const short Vitality = 63;

			// Token: 0x0400E181 RID: 57729
			public const short Energy = 64;

			// Token: 0x0400E182 RID: 57730
			public const short Intelligence = 65;

			// Token: 0x0400E183 RID: 57731
			public const short OuterPenetrate = 22;

			// Token: 0x0400E184 RID: 57732
			public const short InnerPenetrate = 23;

			// Token: 0x0400E185 RID: 57733
			public const short OuterPenetrateResist = 29;

			// Token: 0x0400E186 RID: 57734
			public const short InnerPenetrateResist = 30;

			// Token: 0x0400E187 RID: 57735
			public const short HitStrength = 24;

			// Token: 0x0400E188 RID: 57736
			public const short HitTechnique = 25;

			// Token: 0x0400E189 RID: 57737
			public const short HitSpeed = 26;

			// Token: 0x0400E18A RID: 57738
			public const short HitMind = 27;

			// Token: 0x0400E18B RID: 57739
			public const short AvoidStrength = 33;

			// Token: 0x0400E18C RID: 57740
			public const short AvoidTechnique = 34;

			// Token: 0x0400E18D RID: 57741
			public const short AvoidSpeed = 35;

			// Token: 0x0400E18E RID: 57742
			public const short AvoidMind = 36;

			// Token: 0x0400E18F RID: 57743
			public const short QiDisorder = 55;

			// Token: 0x0400E190 RID: 57744
			public const short LifeSkill0 = 66;

			// Token: 0x0400E191 RID: 57745
			public const short LifeSkill1 = 67;

			// Token: 0x0400E192 RID: 57746
			public const short LifeSkill2 = 68;

			// Token: 0x0400E193 RID: 57747
			public const short LifeSkill3 = 69;

			// Token: 0x0400E194 RID: 57748
			public const short LifeSkill4 = 70;

			// Token: 0x0400E195 RID: 57749
			public const short LifeSkill5 = 71;

			// Token: 0x0400E196 RID: 57750
			public const short LifeSkill6 = 72;

			// Token: 0x0400E197 RID: 57751
			public const short LifeSkill7 = 73;

			// Token: 0x0400E198 RID: 57752
			public const short LifeSkill8 = 74;

			// Token: 0x0400E199 RID: 57753
			public const short LifeSkill9 = 75;

			// Token: 0x0400E19A RID: 57754
			public const short LifeSkill10 = 76;

			// Token: 0x0400E19B RID: 57755
			public const short LifeSkill11 = 77;

			// Token: 0x0400E19C RID: 57756
			public const short LifeSkill12 = 78;

			// Token: 0x0400E19D RID: 57757
			public const short LifeSkill13 = 79;

			// Token: 0x0400E19E RID: 57758
			public const short LifeSkill14 = 80;

			// Token: 0x0400E19F RID: 57759
			public const short LifeSkill15 = 81;

			// Token: 0x0400E1A0 RID: 57760
			public const short LifeSkillGrowth = 118;

			// Token: 0x0400E1A1 RID: 57761
			public const short CombatSkill0 = 82;

			// Token: 0x0400E1A2 RID: 57762
			public const short CombatSkill1 = 83;

			// Token: 0x0400E1A3 RID: 57763
			public const short CombatSkill2 = 84;

			// Token: 0x0400E1A4 RID: 57764
			public const short CombatSkill3 = 85;

			// Token: 0x0400E1A5 RID: 57765
			public const short CombatSkill4 = 86;

			// Token: 0x0400E1A6 RID: 57766
			public const short CombatSkill5 = 87;

			// Token: 0x0400E1A7 RID: 57767
			public const short CombatSkill6 = 88;

			// Token: 0x0400E1A8 RID: 57768
			public const short CombatSkill7 = 89;

			// Token: 0x0400E1A9 RID: 57769
			public const short CombatSkill8 = 90;

			// Token: 0x0400E1AA RID: 57770
			public const short CombatSkill9 = 91;

			// Token: 0x0400E1AB RID: 57771
			public const short CombatSkill10 = 92;

			// Token: 0x0400E1AC RID: 57772
			public const short CombatSkill11 = 93;

			// Token: 0x0400E1AD RID: 57773
			public const short CombatSkill12 = 94;

			// Token: 0x0400E1AE RID: 57774
			public const short CombatSkill13 = 95;

			// Token: 0x0400E1AF RID: 57775
			public const short CombatSkillGrowth = 119;

			// Token: 0x0400E1B0 RID: 57776
			public const short Personality0 = 96;

			// Token: 0x0400E1B1 RID: 57777
			public const short Personality1 = 97;

			// Token: 0x0400E1B2 RID: 57778
			public const short Personality2 = 98;

			// Token: 0x0400E1B3 RID: 57779
			public const short Personality3 = 99;

			// Token: 0x0400E1B4 RID: 57780
			public const short Personality4 = 100;

			// Token: 0x0400E1B5 RID: 57781
			public const short Personality5 = 101;

			// Token: 0x0400E1B6 RID: 57782
			public const short Personality6 = 102;

			// Token: 0x0400E1B7 RID: 57783
			public const short Resource0 = 103;

			// Token: 0x0400E1B8 RID: 57784
			public const short Resource1 = 104;

			// Token: 0x0400E1B9 RID: 57785
			public const short Resource2 = 105;

			// Token: 0x0400E1BA RID: 57786
			public const short Resource3 = 106;

			// Token: 0x0400E1BB RID: 57787
			public const short Resource4 = 107;

			// Token: 0x0400E1BC RID: 57788
			public const short Resource5 = 108;

			// Token: 0x0400E1BD RID: 57789
			public const short Resource6 = 109;

			// Token: 0x0400E1BE RID: 57790
			public const short Resource7 = 110;

			// Token: 0x0400E1BF RID: 57791
			public const short InventoryLoad = 37;

			// Token: 0x0400E1C0 RID: 57792
			public const short KidnapCount = 111;

			// Token: 0x0400E1C1 RID: 57793
			public const short AttackMedal = 112;

			// Token: 0x0400E1C2 RID: 57794
			public const short DefenceMedal = 113;

			// Token: 0x0400E1C3 RID: 57795
			public const short WisdomMedal = 114;

			// Token: 0x0400E1C4 RID: 57796
			public const short Command0 = 115;

			// Token: 0x0400E1C5 RID: 57797
			public const short Command1 = 116;

			// Token: 0x0400E1C6 RID: 57798
			public const short Command2 = 117;
		}
	}
}
