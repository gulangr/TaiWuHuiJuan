using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Kidnap
{
	// Token: 0x02000D21 RID: 3361
	public class KidnapSortController : SortController<KidnapCharDisplayData>
	{
		// Token: 0x0600A75D RID: 42845 RVA: 0x004DDB12 File Offset: 0x004DBD12
		public KidnapSortController(Func<int, bool> isTaiwuFunc)
		{
			this._isTaiwuFunc = isTaiwuFunc;
		}

		// Token: 0x0600A75E RID: 42846 RVA: 0x004DDB24 File Offset: 0x004DBD24
		public override Comparison<KidnapCharDisplayData> GenerateComparer(SortStateData sortData)
		{
			return (KidnapCharDisplayData x, KidnapCharDisplayData y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600A75F RID: 42847 RVA: 0x004DDB58 File Offset: 0x004DBD58
		private int CompareData(KidnapCharDisplayData x, KidnapCharDisplayData y, SortStateData sortData)
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
									int comparisonResult = this.CompareBySortId(x, y, sortId);
									bool flag6 = comparisonResult != 0;
									if (flag6)
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

		// Token: 0x0600A760 RID: 42848 RVA: 0x004DDC94 File Offset: 0x004DBE94
		private int CompareBySortId(KidnapCharDisplayData x, KidnapCharDisplayData y, short sortId)
		{
			switch (sortId)
			{
			case 0:
				return x.CharacterId.CompareTo(y.CharacterId);
			case 1:
				return x.RopeItemKey.TemplateId.CompareTo(y.RopeItemKey.TemplateId);
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 13:
				break;
			case 8:
				return x.PhysiologicalAge.CompareTo(y.PhysiologicalAge);
			case 9:
				return x.Charm.CompareTo(y.Charm);
			case 10:
				return x.Health.CompareTo(y.Health);
			case 11:
				return x.FavorabilityToTaiwu.CompareTo(y.FavorabilityToTaiwu);
			case 12:
				return x.Happiness.CompareTo(y.Happiness);
			case 14:
				return x.KidnapDuration.CompareTo(y.KidnapDuration);
			default:
				switch (sortId)
				{
				case 53:
					return x.DefeatMarkCount.CompareTo(y.DefeatMarkCount);
				case 54:
				case 55:
				case 56:
					break;
				case 57:
					return x.BehaviorType.CompareTo(y.BehaviorType);
				case 58:
					return x.PreexistenceCharCount.CompareTo(y.PreexistenceCharCount);
				case 59:
					return x.Fame.CompareTo(y.Fame);
				default:
					if (sortId == 120)
					{
						return x.TotalResistance.CompareTo(y.TotalResistance);
					}
					break;
				}
				break;
			}
			return this.CompareByPropertyIndex(x, y, sortId);
		}

		// Token: 0x0600A761 RID: 42849 RVA: 0x004DDE50 File Offset: 0x004DC050
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

		// Token: 0x0600A762 RID: 42850 RVA: 0x004DE9F8 File Offset: 0x004DCBF8
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

		// Token: 0x0600A763 RID: 42851 RVA: 0x004DEA48 File Offset: 0x004DCC48
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
				14,
				120,
				1,
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

		// Token: 0x04008369 RID: 33641
		private readonly Func<int, bool> _isTaiwuFunc;

		// Token: 0x0200244D RID: 9293
		public static class SortIds
		{
			// Token: 0x0400E374 RID: 58228
			public const short Name = 0;

			// Token: 0x0400E375 RID: 58229
			public const short Age = 8;

			// Token: 0x0400E376 RID: 58230
			public const short Health = 10;

			// Token: 0x0400E377 RID: 58231
			public const short Charm = 9;

			// Token: 0x0400E378 RID: 58232
			public const short Happiness = 12;

			// Token: 0x0400E379 RID: 58233
			public const short Favor = 11;

			// Token: 0x0400E37A RID: 58234
			public const short Alertness = 130;

			// Token: 0x0400E37B RID: 58235
			public const short DefeatMark = 53;

			// Token: 0x0400E37C RID: 58236
			public const short Behavior = 57;

			// Token: 0x0400E37D RID: 58237
			public const short Samsara = 58;

			// Token: 0x0400E37E RID: 58238
			public const short Fame = 59;

			// Token: 0x0400E37F RID: 58239
			public const short KidnapDuration = 14;

			// Token: 0x0400E380 RID: 58240
			public const short Resistance = 120;

			// Token: 0x0400E381 RID: 58241
			public const short Rope = 1;

			// Token: 0x0400E382 RID: 58242
			public const short Strength = 60;

			// Token: 0x0400E383 RID: 58243
			public const short Dexterity = 61;

			// Token: 0x0400E384 RID: 58244
			public const short Concentration = 62;

			// Token: 0x0400E385 RID: 58245
			public const short Vitality = 63;

			// Token: 0x0400E386 RID: 58246
			public const short Energy = 64;

			// Token: 0x0400E387 RID: 58247
			public const short Intelligence = 65;

			// Token: 0x0400E388 RID: 58248
			public const short OuterPenetrate = 22;

			// Token: 0x0400E389 RID: 58249
			public const short InnerPenetrate = 23;

			// Token: 0x0400E38A RID: 58250
			public const short OuterPenetrateResist = 29;

			// Token: 0x0400E38B RID: 58251
			public const short InnerPenetrateResist = 30;

			// Token: 0x0400E38C RID: 58252
			public const short HitStrength = 24;

			// Token: 0x0400E38D RID: 58253
			public const short HitTechnique = 25;

			// Token: 0x0400E38E RID: 58254
			public const short HitSpeed = 26;

			// Token: 0x0400E38F RID: 58255
			public const short HitMind = 27;

			// Token: 0x0400E390 RID: 58256
			public const short AvoidStrength = 33;

			// Token: 0x0400E391 RID: 58257
			public const short AvoidTechnique = 34;

			// Token: 0x0400E392 RID: 58258
			public const short AvoidSpeed = 35;

			// Token: 0x0400E393 RID: 58259
			public const short AvoidMind = 36;

			// Token: 0x0400E394 RID: 58260
			public const short QiDisorder = 55;

			// Token: 0x0400E395 RID: 58261
			public const short LifeSkill0 = 66;

			// Token: 0x0400E396 RID: 58262
			public const short LifeSkill1 = 67;

			// Token: 0x0400E397 RID: 58263
			public const short LifeSkill2 = 68;

			// Token: 0x0400E398 RID: 58264
			public const short LifeSkill3 = 69;

			// Token: 0x0400E399 RID: 58265
			public const short LifeSkill4 = 70;

			// Token: 0x0400E39A RID: 58266
			public const short LifeSkill5 = 71;

			// Token: 0x0400E39B RID: 58267
			public const short LifeSkill6 = 72;

			// Token: 0x0400E39C RID: 58268
			public const short LifeSkill7 = 73;

			// Token: 0x0400E39D RID: 58269
			public const short LifeSkill8 = 74;

			// Token: 0x0400E39E RID: 58270
			public const short LifeSkill9 = 75;

			// Token: 0x0400E39F RID: 58271
			public const short LifeSkill10 = 76;

			// Token: 0x0400E3A0 RID: 58272
			public const short LifeSkill11 = 77;

			// Token: 0x0400E3A1 RID: 58273
			public const short LifeSkill12 = 78;

			// Token: 0x0400E3A2 RID: 58274
			public const short LifeSkill13 = 79;

			// Token: 0x0400E3A3 RID: 58275
			public const short LifeSkill14 = 80;

			// Token: 0x0400E3A4 RID: 58276
			public const short LifeSkill15 = 81;

			// Token: 0x0400E3A5 RID: 58277
			public const short LifeSkillGrowth = 118;

			// Token: 0x0400E3A6 RID: 58278
			public const short CombatSkill0 = 82;

			// Token: 0x0400E3A7 RID: 58279
			public const short CombatSkill1 = 83;

			// Token: 0x0400E3A8 RID: 58280
			public const short CombatSkill2 = 84;

			// Token: 0x0400E3A9 RID: 58281
			public const short CombatSkill3 = 85;

			// Token: 0x0400E3AA RID: 58282
			public const short CombatSkill4 = 86;

			// Token: 0x0400E3AB RID: 58283
			public const short CombatSkill5 = 87;

			// Token: 0x0400E3AC RID: 58284
			public const short CombatSkill6 = 88;

			// Token: 0x0400E3AD RID: 58285
			public const short CombatSkill7 = 89;

			// Token: 0x0400E3AE RID: 58286
			public const short CombatSkill8 = 90;

			// Token: 0x0400E3AF RID: 58287
			public const short CombatSkill9 = 91;

			// Token: 0x0400E3B0 RID: 58288
			public const short CombatSkill10 = 92;

			// Token: 0x0400E3B1 RID: 58289
			public const short CombatSkill11 = 93;

			// Token: 0x0400E3B2 RID: 58290
			public const short CombatSkill12 = 94;

			// Token: 0x0400E3B3 RID: 58291
			public const short CombatSkill13 = 95;

			// Token: 0x0400E3B4 RID: 58292
			public const short CombatSkillGrowth = 119;

			// Token: 0x0400E3B5 RID: 58293
			public const short Personality0 = 96;

			// Token: 0x0400E3B6 RID: 58294
			public const short Personality1 = 97;

			// Token: 0x0400E3B7 RID: 58295
			public const short Personality2 = 98;

			// Token: 0x0400E3B8 RID: 58296
			public const short Personality3 = 99;

			// Token: 0x0400E3B9 RID: 58297
			public const short Personality4 = 100;

			// Token: 0x0400E3BA RID: 58298
			public const short Personality5 = 101;

			// Token: 0x0400E3BB RID: 58299
			public const short Personality6 = 102;

			// Token: 0x0400E3BC RID: 58300
			public const short Resource0 = 103;

			// Token: 0x0400E3BD RID: 58301
			public const short Resource1 = 104;

			// Token: 0x0400E3BE RID: 58302
			public const short Resource2 = 105;

			// Token: 0x0400E3BF RID: 58303
			public const short Resource3 = 106;

			// Token: 0x0400E3C0 RID: 58304
			public const short Resource4 = 107;

			// Token: 0x0400E3C1 RID: 58305
			public const short Resource5 = 108;

			// Token: 0x0400E3C2 RID: 58306
			public const short Resource6 = 109;

			// Token: 0x0400E3C3 RID: 58307
			public const short Resource7 = 110;

			// Token: 0x0400E3C4 RID: 58308
			public const short InventoryLoad = 37;

			// Token: 0x0400E3C5 RID: 58309
			public const short KidnapCount = 111;

			// Token: 0x0400E3C6 RID: 58310
			public const short AttackMedal = 112;

			// Token: 0x0400E3C7 RID: 58311
			public const short DefenceMedal = 113;

			// Token: 0x0400E3C8 RID: 58312
			public const short WisdomMedal = 114;

			// Token: 0x0400E3C9 RID: 58313
			public const short Command0 = 115;

			// Token: 0x0400E3CA RID: 58314
			public const short Command1 = 116;

			// Token: 0x0400E3CB RID: 58315
			public const short Command2 = 117;
		}
	}
}
