using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Prison
{
	// Token: 0x02000D14 RID: 3348
	public class PrisonerSortController : SortController<CharacterDisplayDataForSettlementPrisoner>
	{
		// Token: 0x0600A726 RID: 42790 RVA: 0x004DBEE8 File Offset: 0x004DA0E8
		public override Comparison<CharacterDisplayDataForSettlementPrisoner> GenerateComparer(SortStateData sortData)
		{
			return (CharacterDisplayDataForSettlementPrisoner x, CharacterDisplayDataForSettlementPrisoner y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600A727 RID: 42791 RVA: 0x004DBF1C File Offset: 0x004DA11C
		private int CompareData(CharacterDisplayDataForSettlementPrisoner x, CharacterDisplayDataForSettlementPrisoner y, SortStateData sortData)
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
						result = x.KidnapCharDisplayData.CharacterId.CompareTo(y.KidnapCharDisplayData.CharacterId);
					}
				}
			}
			return result;
		}

		// Token: 0x0600A728 RID: 42792 RVA: 0x004DC014 File Offset: 0x004DA214
		private int CompareBySortId(CharacterDisplayDataForSettlementPrisoner x, CharacterDisplayDataForSettlementPrisoner y, short sortId)
		{
			if (sortId <= 15)
			{
				if (sortId == 1)
				{
					return x.KidnapCharDisplayData.RopeItemKey.TemplateId.CompareTo(y.KidnapCharDisplayData.RopeItemKey.TemplateId);
				}
				if (sortId == 14)
				{
					int curDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
					int remainX = x.SettlementPrisoner.Duration - (curDate - x.SettlementPrisoner.KidnapBeginDate);
					int remainY = y.SettlementPrisoner.Duration - (curDate - y.SettlementPrisoner.KidnapBeginDate);
					return remainX.CompareTo(remainY);
				}
				if (sortId == 15)
				{
					return x.SettlementPrisoner.PunishmentSeverity.CompareTo(y.SettlementPrisoner.PunishmentSeverity);
				}
			}
			else
			{
				if (sortId == 120)
				{
					return x.Resistance.CompareTo(y.Resistance);
				}
				if (sortId == 138)
				{
					return x.SettlementPrisoner.PunishmentType.CompareTo(y.SettlementPrisoner.PunishmentType);
				}
				if (sortId == 139)
				{
					return x.KidnapCharDisplayData.OrganizationInfo.Grade.CompareTo(y.KidnapCharDisplayData.OrganizationInfo.Grade);
				}
			}
			return this.CompareBySortId(x.KidnapCharDisplayData, y.KidnapCharDisplayData, sortId);
		}

		// Token: 0x0600A729 RID: 42793 RVA: 0x004DC174 File Offset: 0x004DA374
		private int CompareBySortId(KidnapCharDisplayData x, KidnapCharDisplayData y, short sortId)
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
					result = x.Charm.CompareTo(y.Charm);
					break;
				case 10:
					result = x.Health.CompareTo(y.Health);
					break;
				case 11:
					result = x.FavorabilityToTaiwu.CompareTo(y.FavorabilityToTaiwu);
					break;
				case 12:
					result = x.Happiness.CompareTo(y.Happiness);
					break;
				default:
					switch (sortId)
					{
					case 53:
						return x.DefeatMarkCount.CompareTo(y.DefeatMarkCount);
					case 57:
						return x.BehaviorType.CompareTo(y.BehaviorType);
					case 58:
						return x.PreexistenceCharCount.CompareTo(y.PreexistenceCharCount);
					case 59:
						return x.Fame.CompareTo(y.Fame);
					}
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

		// Token: 0x0600A72A RID: 42794 RVA: 0x004DC2B4 File Offset: 0x004DA4B4
		private unsafe int CompareByPropertyIndex(KidnapCharDisplayData x, KidnapCharDisplayData y, short sortId)
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
				return x.CurrInventoryLoad.CompareTo(y.CurrInventoryLoad);
			case 55:
				return x.DisorderOfQi.CompareTo(y.DisorderOfQi);
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
				return x.LifeSkillGrowthType.CompareTo(y.LifeSkillGrowthType);
			case 119:
				return x.CombatSkillGrowthType.CompareTo(y.CombatSkillGrowthType);
			}
			return 0;
		}

		// Token: 0x0600A72B RID: 42795 RVA: 0x004DCE5C File Offset: 0x004DB05C
		private int GetCommandValue(KidnapCharDisplayData data, int commandIndex)
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

		// Token: 0x0600A72C RID: 42796 RVA: 0x004DCEAC File Offset: 0x004DB0AC
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				138,
				15,
				14,
				120,
				1,
				139,
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
					0,
					14,
					15,
					139
				},
				DefaultSortState = default(SortUiState)
			};
		}

		// Token: 0x02002442 RID: 9282
		public static class SortIds
		{
			// Token: 0x0400E2FD RID: 58109
			public const short Name = 0;

			// Token: 0x0400E2FE RID: 58110
			public const short Age = 8;

			// Token: 0x0400E2FF RID: 58111
			public const short Health = 10;

			// Token: 0x0400E300 RID: 58112
			public const short Charm = 9;

			// Token: 0x0400E301 RID: 58113
			public const short Happiness = 12;

			// Token: 0x0400E302 RID: 58114
			public const short Favor = 11;

			// Token: 0x0400E303 RID: 58115
			public const short DefeatMark = 53;

			// Token: 0x0400E304 RID: 58116
			public const short Behavior = 57;

			// Token: 0x0400E305 RID: 58117
			public const short Samsara = 58;

			// Token: 0x0400E306 RID: 58118
			public const short Fame = 59;

			// Token: 0x0400E307 RID: 58119
			public const short PrisonerDuration = 14;

			// Token: 0x0400E308 RID: 58120
			public const short Resistance = 120;

			// Token: 0x0400E309 RID: 58121
			public const short Rope = 1;

			// Token: 0x0400E30A RID: 58122
			public const short PunishmentType = 138;

			// Token: 0x0400E30B RID: 58123
			public const short PunishmentSeverity = 15;

			// Token: 0x0400E30C RID: 58124
			public const short CharacterIdentity = 139;

			// Token: 0x0400E30D RID: 58125
			public const short Strength = 60;

			// Token: 0x0400E30E RID: 58126
			public const short Dexterity = 61;

			// Token: 0x0400E30F RID: 58127
			public const short Concentration = 62;

			// Token: 0x0400E310 RID: 58128
			public const short Vitality = 63;

			// Token: 0x0400E311 RID: 58129
			public const short Energy = 64;

			// Token: 0x0400E312 RID: 58130
			public const short Intelligence = 65;

			// Token: 0x0400E313 RID: 58131
			public const short OuterPenetrate = 22;

			// Token: 0x0400E314 RID: 58132
			public const short InnerPenetrate = 23;

			// Token: 0x0400E315 RID: 58133
			public const short OuterPenetrateResist = 29;

			// Token: 0x0400E316 RID: 58134
			public const short InnerPenetrateResist = 30;

			// Token: 0x0400E317 RID: 58135
			public const short HitStrength = 24;

			// Token: 0x0400E318 RID: 58136
			public const short HitTechnique = 25;

			// Token: 0x0400E319 RID: 58137
			public const short HitSpeed = 26;

			// Token: 0x0400E31A RID: 58138
			public const short HitMind = 27;

			// Token: 0x0400E31B RID: 58139
			public const short AvoidStrength = 33;

			// Token: 0x0400E31C RID: 58140
			public const short AvoidTechnique = 34;

			// Token: 0x0400E31D RID: 58141
			public const short AvoidSpeed = 35;

			// Token: 0x0400E31E RID: 58142
			public const short AvoidMind = 36;

			// Token: 0x0400E31F RID: 58143
			public const short QiDisorder = 55;

			// Token: 0x0400E320 RID: 58144
			public const short LifeSkill0 = 66;

			// Token: 0x0400E321 RID: 58145
			public const short LifeSkill1 = 67;

			// Token: 0x0400E322 RID: 58146
			public const short LifeSkill2 = 68;

			// Token: 0x0400E323 RID: 58147
			public const short LifeSkill3 = 69;

			// Token: 0x0400E324 RID: 58148
			public const short LifeSkill4 = 70;

			// Token: 0x0400E325 RID: 58149
			public const short LifeSkill5 = 71;

			// Token: 0x0400E326 RID: 58150
			public const short LifeSkill6 = 72;

			// Token: 0x0400E327 RID: 58151
			public const short LifeSkill7 = 73;

			// Token: 0x0400E328 RID: 58152
			public const short LifeSkill8 = 74;

			// Token: 0x0400E329 RID: 58153
			public const short LifeSkill9 = 75;

			// Token: 0x0400E32A RID: 58154
			public const short LifeSkill10 = 76;

			// Token: 0x0400E32B RID: 58155
			public const short LifeSkill11 = 77;

			// Token: 0x0400E32C RID: 58156
			public const short LifeSkill12 = 78;

			// Token: 0x0400E32D RID: 58157
			public const short LifeSkill13 = 79;

			// Token: 0x0400E32E RID: 58158
			public const short LifeSkill14 = 80;

			// Token: 0x0400E32F RID: 58159
			public const short LifeSkill15 = 81;

			// Token: 0x0400E330 RID: 58160
			public const short LifeSkillGrowth = 118;

			// Token: 0x0400E331 RID: 58161
			public const short CombatSkill0 = 82;

			// Token: 0x0400E332 RID: 58162
			public const short CombatSkill1 = 83;

			// Token: 0x0400E333 RID: 58163
			public const short CombatSkill2 = 84;

			// Token: 0x0400E334 RID: 58164
			public const short CombatSkill3 = 85;

			// Token: 0x0400E335 RID: 58165
			public const short CombatSkill4 = 86;

			// Token: 0x0400E336 RID: 58166
			public const short CombatSkill5 = 87;

			// Token: 0x0400E337 RID: 58167
			public const short CombatSkill6 = 88;

			// Token: 0x0400E338 RID: 58168
			public const short CombatSkill7 = 89;

			// Token: 0x0400E339 RID: 58169
			public const short CombatSkill8 = 90;

			// Token: 0x0400E33A RID: 58170
			public const short CombatSkill9 = 91;

			// Token: 0x0400E33B RID: 58171
			public const short CombatSkill10 = 92;

			// Token: 0x0400E33C RID: 58172
			public const short CombatSkill11 = 93;

			// Token: 0x0400E33D RID: 58173
			public const short CombatSkill12 = 94;

			// Token: 0x0400E33E RID: 58174
			public const short CombatSkill13 = 95;

			// Token: 0x0400E33F RID: 58175
			public const short CombatSkillGrowth = 119;

			// Token: 0x0400E340 RID: 58176
			public const short Personality0 = 96;

			// Token: 0x0400E341 RID: 58177
			public const short Personality1 = 97;

			// Token: 0x0400E342 RID: 58178
			public const short Personality2 = 98;

			// Token: 0x0400E343 RID: 58179
			public const short Personality3 = 99;

			// Token: 0x0400E344 RID: 58180
			public const short Personality4 = 100;

			// Token: 0x0400E345 RID: 58181
			public const short Personality5 = 101;

			// Token: 0x0400E346 RID: 58182
			public const short Personality6 = 102;

			// Token: 0x0400E347 RID: 58183
			public const short Resource0 = 103;

			// Token: 0x0400E348 RID: 58184
			public const short Resource1 = 104;

			// Token: 0x0400E349 RID: 58185
			public const short Resource2 = 105;

			// Token: 0x0400E34A RID: 58186
			public const short Resource3 = 106;

			// Token: 0x0400E34B RID: 58187
			public const short Resource4 = 107;

			// Token: 0x0400E34C RID: 58188
			public const short Resource5 = 108;

			// Token: 0x0400E34D RID: 58189
			public const short Resource6 = 109;

			// Token: 0x0400E34E RID: 58190
			public const short Resource7 = 110;

			// Token: 0x0400E34F RID: 58191
			public const short InventoryLoad = 37;

			// Token: 0x0400E350 RID: 58192
			public const short KidnapCount = 111;

			// Token: 0x0400E351 RID: 58193
			public const short AttackMedal = 112;

			// Token: 0x0400E352 RID: 58194
			public const short DefenceMedal = 113;

			// Token: 0x0400E353 RID: 58195
			public const short WisdomMedal = 114;

			// Token: 0x0400E354 RID: 58196
			public const short Command0 = 115;

			// Token: 0x0400E355 RID: 58197
			public const short Command1 = 116;

			// Token: 0x0400E356 RID: 58198
			public const short Command2 = 117;
		}
	}
}
