using System;
using System.Collections.Generic;
using FrameWork;
using Game.Views.Select;
using Game.Views.Select.SelectCharacter;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Recruit
{
	// Token: 0x02000D0B RID: 3339
	public class RecruitCharacterSortController : SortController<BuildingRecruitCharacterData>
	{
		// Token: 0x0600A705 RID: 42757 RVA: 0x004DAF08 File Offset: 0x004D9108
		public override Comparison<BuildingRecruitCharacterData> GenerateComparer(SortStateData sortData)
		{
			return (BuildingRecruitCharacterData x, BuildingRecruitCharacterData y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600A706 RID: 42758 RVA: 0x004DAF3C File Offset: 0x004D913C
		private int CompareData(BuildingRecruitCharacterData x, BuildingRecruitCharacterData y, SortStateData sortData)
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
								int comparisonResult = this.CompareBySortId(x, y, sortId);
								bool flag5 = comparisonResult != 0;
								if (flag5)
								{
									return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
								}
							}
						}
						result = x.CharacterData.TemplateId.CompareTo(y.CharacterData.TemplateId);
					}
				}
			}
			return result;
		}

		// Token: 0x0600A707 RID: 42759 RVA: 0x004DB034 File Offset: 0x004D9234
		private unsafe int CompareBySortId(BuildingRecruitCharacterData x, BuildingRecruitCharacterData y, short sortId)
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
						if (sortId <= 11)
						{
							if (sortId == 0)
							{
								return RecruitCharacterSortController.CompareByName(x.CharacterData, y.CharacterData);
							}
							switch (sortId)
							{
							case 8:
								return x.CharacterData.Age.CompareTo(y.CharacterData.Age);
							case 9:
								return x.CharacterData.FinalAttraction.CompareTo(y.CharacterData.FinalAttraction);
							case 11:
								return 0;
							}
						}
						else
						{
							switch (sortId)
							{
							case 53:
								return 0;
							case 54:
							case 55:
							case 56:
							case 58:
							case 59:
								break;
							case 57:
								return x.CharacterData.GetBaseMorality().CompareTo(y.CharacterData.GetBaseMorality());
							case 60:
								return x.CharacterData.MainAttributes[0].CompareTo(*y.CharacterData.MainAttributes[0]);
							case 61:
								return x.CharacterData.MainAttributes[1].CompareTo(*y.CharacterData.MainAttributes[1]);
							case 62:
								return x.CharacterData.MainAttributes[2].CompareTo(*y.CharacterData.MainAttributes[2]);
							case 63:
								return x.CharacterData.MainAttributes[3].CompareTo(*y.CharacterData.MainAttributes[3]);
							case 64:
								return x.CharacterData.MainAttributes[4].CompareTo(*y.CharacterData.MainAttributes[4]);
							case 65:
								return x.CharacterData.MainAttributes[5].CompareTo(*y.CharacterData.MainAttributes[5]);
							default:
								switch (sortId)
								{
								case 96:
									return x.CharacterData.CalculatedPersonalities[0].CompareTo(*y.CharacterData.CalculatedPersonalities[0]);
								case 97:
									return x.CharacterData.CalculatedPersonalities[1].CompareTo(*y.CharacterData.CalculatedPersonalities[1]);
								case 98:
									return x.CharacterData.CalculatedPersonalities[2].CompareTo(*y.CharacterData.CalculatedPersonalities[2]);
								case 99:
									return x.CharacterData.CalculatedPersonalities[3].CompareTo(*y.CharacterData.CalculatedPersonalities[3]);
								case 100:
									return x.CharacterData.CalculatedPersonalities[4].CompareTo(*y.CharacterData.CalculatedPersonalities[4]);
								case 101:
									return x.CharacterData.CalculatedPersonalities[5].CompareTo(*y.CharacterData.CalculatedPersonalities[5]);
								case 102:
									return x.CharacterData.CalculatedPersonalities[6].CompareTo(*y.CharacterData.CalculatedPersonalities[6]);
								default:
									switch (sortId)
									{
									case 115:
										return RecruitCharacterSortController.GetCommandValue(x.CharacterData, 0).CompareTo(RecruitCharacterSortController.GetCommandValue(y.CharacterData, 0));
									case 116:
										return RecruitCharacterSortController.GetCommandValue(x.CharacterData, 1).CompareTo(RecruitCharacterSortController.GetCommandValue(y.CharacterData, 1));
									case 117:
										return RecruitCharacterSortController.GetCommandValue(x.CharacterData, 2).CompareTo(RecruitCharacterSortController.GetCommandValue(y.CharacterData, 2));
									case 118:
										return RecruitCharacterSortController.CompareSkillGrowth((int)x.CharacterData.LifeSkillQualificationGrowthType, (int)x.CharacterData.Age, (int)y.CharacterData.LifeSkillQualificationGrowthType, (int)y.CharacterData.Age);
									case 119:
										return RecruitCharacterSortController.CompareSkillGrowth((int)x.CharacterData.CombatSkillQualificationGrowthType, (int)x.CharacterData.Age, (int)y.CharacterData.CombatSkillQualificationGrowthType, (int)y.CharacterData.Age);
									case 122:
										return x.CharacterData.Gender.CompareTo(y.CharacterData.Gender);
									}
									break;
								}
								break;
							}
						}
						result = RecruitCharacterSortController.CompareLifeOrCombatSkill(x.CharacterData, y.CharacterData, sortId);
					}
				}
			}
			return result;
		}

		// Token: 0x0600A708 RID: 42760 RVA: 0x004DB53C File Offset: 0x004D973C
		private static int CompareByName(RecruitCharacterData x, RecruitCharacterData y)
		{
			ValueTuple<string, string> name = x.FullName.GetName(x.Gender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
			string surname = name.Item1;
			string givenName = name.Item2;
			ValueTuple<string, string> name2 = y.FullName.GetName(y.Gender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
			string surnameY = name2.Item1;
			string givenNameY = name2.Item2;
			return Utils_Sorting.CompareByCurrentLangEncoding(givenName, givenNameY);
		}

		// Token: 0x0600A709 RID: 42761 RVA: 0x004DB5A8 File Offset: 0x004D97A8
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

		// Token: 0x0600A70A RID: 42762 RVA: 0x004DB610 File Offset: 0x004D9810
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

		// Token: 0x0600A70B RID: 42763 RVA: 0x004DB648 File Offset: 0x004D9848
		private unsafe static int CompareLifeOrCombatSkill(RecruitCharacterData x, RecruitCharacterData y, short sortId)
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

		// Token: 0x0600A70C RID: 42764 RVA: 0x004DB710 File Offset: 0x004D9910
		private static int GetCommandValue(RecruitCharacterData data, int commandIndex)
		{
			bool flag = data.TeammateCommands == null || !data.TeammateCommands.CheckIndex(commandIndex);
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				result = (int)data.TeammateCommands[commandIndex];
			}
			return result;
		}

		// Token: 0x0600A70D RID: 42765 RVA: 0x004DB750 File Offset: 0x004D9950
		private static int CompareSkillGrowth(int growthTypeX, int ageX, int growthTypeY, int ageY)
		{
			sbyte addValueX = CommonUtils.GetSkillGrowthAddValue(growthTypeX, ageX);
			sbyte addValueY = CommonUtils.GetSkillGrowthAddValue(growthTypeY, ageY);
			int compare = addValueX.CompareTo(addValueY);
			return (compare != 0) ? compare : growthTypeX.CompareTo(growthTypeY);
		}

		// Token: 0x0600A70E RID: 42766 RVA: 0x004DB78C File Offset: 0x004D998C
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

		// Token: 0x02002439 RID: 9273
		public static class SortIds
		{
			// Token: 0x0400E28C RID: 57996
			public const short Name = 0;

			// Token: 0x0400E28D RID: 57997
			public const short Age = 8;

			// Token: 0x0400E28E RID: 57998
			public const short Gender = 122;

			// Token: 0x0400E28F RID: 57999
			public const short Health = 10;

			// Token: 0x0400E290 RID: 58000
			public const short Charm = 9;

			// Token: 0x0400E291 RID: 58001
			public const short Happiness = 12;

			// Token: 0x0400E292 RID: 58002
			public const short Favor = 11;

			// Token: 0x0400E293 RID: 58003
			public const short DefeatMark = 53;

			// Token: 0x0400E294 RID: 58004
			public const short Behavior = 57;

			// Token: 0x0400E295 RID: 58005
			public const short Samsara = 58;

			// Token: 0x0400E296 RID: 58006
			public const short Fame = 59;

			// Token: 0x0400E297 RID: 58007
			public const short VillagerLeftPotentialCount = 13;

			// Token: 0x0400E298 RID: 58008
			public const short Strength = 60;

			// Token: 0x0400E299 RID: 58009
			public const short Dexterity = 61;

			// Token: 0x0400E29A RID: 58010
			public const short Concentration = 62;

			// Token: 0x0400E29B RID: 58011
			public const short Vitality = 63;

			// Token: 0x0400E29C RID: 58012
			public const short Energy = 64;

			// Token: 0x0400E29D RID: 58013
			public const short Intelligence = 65;

			// Token: 0x0400E29E RID: 58014
			public const short OuterPenetrate = 22;

			// Token: 0x0400E29F RID: 58015
			public const short InnerPenetrate = 23;

			// Token: 0x0400E2A0 RID: 58016
			public const short OuterPenetrateResist = 29;

			// Token: 0x0400E2A1 RID: 58017
			public const short InnerPenetrateResist = 30;

			// Token: 0x0400E2A2 RID: 58018
			public const short HitStrength = 24;

			// Token: 0x0400E2A3 RID: 58019
			public const short HitTechnique = 25;

			// Token: 0x0400E2A4 RID: 58020
			public const short HitSpeed = 26;

			// Token: 0x0400E2A5 RID: 58021
			public const short HitMind = 27;

			// Token: 0x0400E2A6 RID: 58022
			public const short AvoidStrength = 33;

			// Token: 0x0400E2A7 RID: 58023
			public const short AvoidTechnique = 34;

			// Token: 0x0400E2A8 RID: 58024
			public const short AvoidSpeed = 35;

			// Token: 0x0400E2A9 RID: 58025
			public const short AvoidMind = 36;

			// Token: 0x0400E2AA RID: 58026
			public const short QiDisorder = 55;

			// Token: 0x0400E2AB RID: 58027
			public const short LifeSkill0 = 66;

			// Token: 0x0400E2AC RID: 58028
			public const short LifeSkill1 = 67;

			// Token: 0x0400E2AD RID: 58029
			public const short LifeSkill2 = 68;

			// Token: 0x0400E2AE RID: 58030
			public const short LifeSkill3 = 69;

			// Token: 0x0400E2AF RID: 58031
			public const short LifeSkill4 = 70;

			// Token: 0x0400E2B0 RID: 58032
			public const short LifeSkill5 = 71;

			// Token: 0x0400E2B1 RID: 58033
			public const short LifeSkill6 = 72;

			// Token: 0x0400E2B2 RID: 58034
			public const short LifeSkill7 = 73;

			// Token: 0x0400E2B3 RID: 58035
			public const short LifeSkill8 = 74;

			// Token: 0x0400E2B4 RID: 58036
			public const short LifeSkill9 = 75;

			// Token: 0x0400E2B5 RID: 58037
			public const short LifeSkill10 = 76;

			// Token: 0x0400E2B6 RID: 58038
			public const short LifeSkill11 = 77;

			// Token: 0x0400E2B7 RID: 58039
			public const short LifeSkill12 = 78;

			// Token: 0x0400E2B8 RID: 58040
			public const short LifeSkill13 = 79;

			// Token: 0x0400E2B9 RID: 58041
			public const short LifeSkill14 = 80;

			// Token: 0x0400E2BA RID: 58042
			public const short LifeSkill15 = 81;

			// Token: 0x0400E2BB RID: 58043
			public const short CombatSkill0 = 82;

			// Token: 0x0400E2BC RID: 58044
			public const short CombatSkill1 = 83;

			// Token: 0x0400E2BD RID: 58045
			public const short CombatSkill2 = 84;

			// Token: 0x0400E2BE RID: 58046
			public const short CombatSkill3 = 85;

			// Token: 0x0400E2BF RID: 58047
			public const short CombatSkill4 = 86;

			// Token: 0x0400E2C0 RID: 58048
			public const short CombatSkill5 = 87;

			// Token: 0x0400E2C1 RID: 58049
			public const short CombatSkill6 = 88;

			// Token: 0x0400E2C2 RID: 58050
			public const short CombatSkill7 = 89;

			// Token: 0x0400E2C3 RID: 58051
			public const short CombatSkill8 = 90;

			// Token: 0x0400E2C4 RID: 58052
			public const short CombatSkill9 = 91;

			// Token: 0x0400E2C5 RID: 58053
			public const short CombatSkill10 = 92;

			// Token: 0x0400E2C6 RID: 58054
			public const short CombatSkill11 = 93;

			// Token: 0x0400E2C7 RID: 58055
			public const short CombatSkill12 = 94;

			// Token: 0x0400E2C8 RID: 58056
			public const short CombatSkill13 = 95;

			// Token: 0x0400E2C9 RID: 58057
			public const short Personality0 = 96;

			// Token: 0x0400E2CA RID: 58058
			public const short Personality1 = 97;

			// Token: 0x0400E2CB RID: 58059
			public const short Personality2 = 98;

			// Token: 0x0400E2CC RID: 58060
			public const short Personality3 = 99;

			// Token: 0x0400E2CD RID: 58061
			public const short Personality4 = 100;

			// Token: 0x0400E2CE RID: 58062
			public const short Personality5 = 101;

			// Token: 0x0400E2CF RID: 58063
			public const short Personality6 = 102;

			// Token: 0x0400E2D0 RID: 58064
			public const short Resource0 = 103;

			// Token: 0x0400E2D1 RID: 58065
			public const short Resource1 = 104;

			// Token: 0x0400E2D2 RID: 58066
			public const short Resource2 = 105;

			// Token: 0x0400E2D3 RID: 58067
			public const short Resource3 = 106;

			// Token: 0x0400E2D4 RID: 58068
			public const short Resource4 = 107;

			// Token: 0x0400E2D5 RID: 58069
			public const short Resource5 = 108;

			// Token: 0x0400E2D6 RID: 58070
			public const short Resource6 = 109;

			// Token: 0x0400E2D7 RID: 58071
			public const short Resource7 = 110;

			// Token: 0x0400E2D8 RID: 58072
			public const short InventoryLoad = 37;

			// Token: 0x0400E2D9 RID: 58073
			public const short KidnapCount = 111;

			// Token: 0x0400E2DA RID: 58074
			public const short AttackMedal = 112;

			// Token: 0x0400E2DB RID: 58075
			public const short DefenceMedal = 113;

			// Token: 0x0400E2DC RID: 58076
			public const short WisdomMedal = 114;

			// Token: 0x0400E2DD RID: 58077
			public const short Command0 = 115;

			// Token: 0x0400E2DE RID: 58078
			public const short Command1 = 116;

			// Token: 0x0400E2DF RID: 58079
			public const short Command2 = 117;

			// Token: 0x0400E2E0 RID: 58080
			public const short Grade = 1;

			// Token: 0x0400E2E1 RID: 58081
			public const short Infection = 123;

			// Token: 0x0400E2E2 RID: 58082
			public const short TakeAfterMonth = 134;

			// Token: 0x0400E2E3 RID: 58083
			public const short TakeAmount = 135;

			// Token: 0x0400E2E4 RID: 58084
			public const short Relationship = 136;

			// Token: 0x0400E2E5 RID: 58085
			public const short OrganizationPower = 137;

			// Token: 0x0400E2E6 RID: 58086
			public const short LifeSkillGrowth = 118;

			// Token: 0x0400E2E7 RID: 58087
			public const short CombatSkillGrowth = 119;
		}
	}
}
