using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B5E RID: 2910
	public static class CharacterDisplayDataForGeneralScrollListUtils
	{
		// Token: 0x06008FD4 RID: 36820 RVA: 0x004304A3 File Offset: 0x0042E6A3
		public static IEnumerable<ColumnDefinition> GenerateColumnDefinitions<T>(CharacterDisplayDataForGeneralScrollListUtils.SubPage subPage, Func<T, CharacterDisplayDataForGeneralScrollList> dataProvider, Action<int> onCharacterClicked, Action<TooltipInvoker, int> mouseTipModifier = null)
		{
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateAvatarWithNameColumn<T>(dataProvider, onCharacterClicked, mouseTipModifier);
			switch (subPage)
			{
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.State:
			{
				foreach (ColumnDefinition col in CharacterDisplayDataForGeneralScrollListUtils.GenerateStateColumns<T>(dataProvider))
				{
					yield return col;
					col = null;
				}
				IEnumerator<ColumnDefinition> enumerator = null;
				break;
			}
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.Property:
			{
				foreach (ColumnDefinition col2 in CharacterDisplayDataForGeneralScrollListUtils.GeneratePropertyColumns<T>(dataProvider))
				{
					yield return col2;
					col2 = null;
				}
				IEnumerator<ColumnDefinition> enumerator2 = null;
				break;
			}
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.Property2:
			{
				foreach (ColumnDefinition col3 in CharacterDisplayDataForGeneralScrollListUtils.GenerateProperty2Columns<T>(dataProvider))
				{
					yield return col3;
					col3 = null;
				}
				IEnumerator<ColumnDefinition> enumerator3 = null;
				break;
			}
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.LifeSkill:
			{
				foreach (ColumnDefinition col4 in CharacterDisplayDataForGeneralScrollListUtils.GenerateLifeSkillColumns<T>(dataProvider))
				{
					yield return col4;
					col4 = null;
				}
				IEnumerator<ColumnDefinition> enumerator4 = null;
				break;
			}
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.CombatSkill:
			{
				foreach (ColumnDefinition col5 in CharacterDisplayDataForGeneralScrollListUtils.GenerateCombatSkillColumns<T>(dataProvider))
				{
					yield return col5;
					col5 = null;
				}
				IEnumerator<ColumnDefinition> enumerator5 = null;
				break;
			}
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.Personality:
			{
				foreach (ColumnDefinition col6 in CharacterDisplayDataForGeneralScrollListUtils.GeneratePersonalityColumns<T>(dataProvider))
				{
					yield return col6;
					col6 = null;
				}
				IEnumerator<ColumnDefinition> enumerator6 = null;
				break;
			}
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.Item:
			{
				foreach (ColumnDefinition col7 in CharacterDisplayDataForGeneralScrollListUtils.GenerateItemColumns<T>(dataProvider))
				{
					yield return col7;
					col7 = null;
				}
				IEnumerator<ColumnDefinition> enumerator7 = null;
				break;
			}
			case CharacterDisplayDataForGeneralScrollListUtils.SubPage.Command:
			{
				foreach (ColumnDefinition col8 in CharacterDisplayDataForGeneralScrollListUtils.GenerateCommandColumns<T>(dataProvider))
				{
					yield return col8;
					col8 = null;
				}
				IEnumerator<ColumnDefinition> enumerator8 = null;
				break;
			}
			}
			yield break;
			yield break;
		}

		// Token: 0x06008FD5 RID: 36821 RVA: 0x004304C8 File Offset: 0x0042E6C8
		public static int CompareBySortId(CharacterDisplayDataForGeneralScrollList x, CharacterDisplayDataForGeneralScrollList y, short sortId)
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
					result = CharacterDisplayDataForGeneralScrollListUtils.CompareByPropertyIndex(x, y, sortId);
					break;
				}
			}
			else
			{
				result = x.CharacterId.CompareTo(y.CharacterId);
			}
			return result;
		}

		// Token: 0x06008FD6 RID: 36822 RVA: 0x00430584 File Offset: 0x0042E784
		private unsafe static int CompareByPropertyIndex(CharacterDisplayDataForGeneralScrollList x, CharacterDisplayDataForGeneralScrollList y, short sortId)
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
				return CharacterDisplayDataForGeneralScrollListUtils.GetCommandValue(x, 0).CompareTo(CharacterDisplayDataForGeneralScrollListUtils.GetCommandValue(y, 0));
			case 116:
				return CharacterDisplayDataForGeneralScrollListUtils.GetCommandValue(x, 1).CompareTo(CharacterDisplayDataForGeneralScrollListUtils.GetCommandValue(y, 1));
			case 117:
				return CharacterDisplayDataForGeneralScrollListUtils.GetCommandValue(x, 2).CompareTo(CharacterDisplayDataForGeneralScrollListUtils.GetCommandValue(y, 2));
			case 118:
				return x.LifeSkillGrowthType.CompareTo(y.LifeSkillGrowthType);
			case 119:
				return x.CombatSkillGrowthType.CompareTo(y.CombatSkillGrowthType);
			}
			return 0;
		}

		// Token: 0x06008FD7 RID: 36823 RVA: 0x00431180 File Offset: 0x0042F380
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

		// Token: 0x06008FD8 RID: 36824 RVA: 0x004311CF File Offset: 0x0042F3CF
		private static string NegativeInvalidString(int value)
		{
			return (value < 0) ? "-" : value.ToString();
		}

		// Token: 0x06008FD9 RID: 36825 RVA: 0x004311E3 File Offset: 0x0042F3E3
		private static IEnumerable<ColumnDefinition> GenerateStateColumns<T>(Func<T, CharacterDisplayDataForGeneralScrollList> dataProvider)
		{
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Char_Age.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CharacterDisplayDataForGeneralScrollListUtils.NegativeInvalidString((int)data.PhysiologicalAge), 8, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Health.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => (data.Health < 0) ? "-" : CommonUtils.GetCharacterHealthInfo(data.Health, data.MaxLeftHealth, data.CharacterId).Item1, 10, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Injury.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CharacterDisplayDataForGeneralScrollListUtils.NegativeInvalidString((int)data.DefeatMarkCount), 53, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), dataProvider, new Func<CharacterDisplayDataForGeneralScrollList, string>(CharacterDisplayDataForGeneralScrollListUtils.GetCharmDisplayString), 9, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetBehaviorString(data.BehaviorType), 57, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetHappinessString(HappinessType.GetHappinessType(data.Happiness)), 12, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Favorability.Tr(), dataProvider, new Func<CharacterDisplayDataForGeneralScrollList, string>(CharacterDisplayDataForGeneralScrollListUtils.GetFavorDisplayString), 11, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Alertness.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetAlertnessNameByValue(data.Alertness), 130, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Samsara.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CharacterDisplayDataForGeneralScrollListUtils.NegativeInvalidString((int)data.PreexistenceCharCount), 58, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CommonUtils.GetFameString(FameType.GetFameType(data.Fame)), 59, 30f, 90f);
			yield break;
		}

		// Token: 0x06008FDA RID: 36826 RVA: 0x004311F3 File Offset: 0x0042F3F3
		private static IEnumerable<ColumnDefinition> GeneratePropertyColumns<T>(Func<T, CharacterDisplayDataForGeneralScrollList> dataProvider)
		{
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Main_Attribute_Strength.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[0].ToString(), 60, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Main_Attribute_Dexterity.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[1].ToString(), 61, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Main_Attribute_Concentration.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[2].ToString(), 62, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Main_Attribute_Vitality.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[3].ToString(), 63, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Main_Attribute_Energy.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[4].ToString(), 64, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Main_Attribute_Intelligence.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.MaxMainAttributes[5].ToString(), 65, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Penetrate_Outer.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Penetrations.Outer.ToString(), 22, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Penetrate_Inner.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Penetrations.Inner.ToString(), 23, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Penetrate_Resist_Outer.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.PenetrationResists.Outer.ToString(), 29, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Penetrate_Resist_Inner.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.PenetrationResists.Inner.ToString(), 30, 30f, 90f);
			yield break;
		}

		// Token: 0x06008FDB RID: 36827 RVA: 0x00431203 File Offset: 0x0042F403
		private static IEnumerable<ColumnDefinition> GenerateProperty2Columns<T>(Func<T, CharacterDisplayDataForGeneralScrollList> dataProvider)
		{
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_HitType_0.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[0].ToString(), 24, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_HitType_1.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[1].ToString(), 25, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_HitType_2.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[2].ToString(), 26, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_HitType_3.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.HitValues[3].ToString(), 27, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_AvoidType_0.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[0].ToString(), 33, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_AvoidType_1.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[1].ToString(), 34, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_AvoidType_2.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[2].ToString(), 35, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_AvoidType_3.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.AvoidValues[3].ToString(), 36, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Qi_Disorder.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => ((int)(data.DisorderOfQi / 10)).ToString(), 55, 30f, 90f);
			yield break;
		}

		// Token: 0x06008FDC RID: 36828 RVA: 0x00431213 File Offset: 0x0042F413
		private static IEnumerable<ColumnDefinition> GenerateLifeSkillColumns<T>(Func<T, CharacterDisplayDataForGeneralScrollList> dataProvider)
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				CharacterDisplayDataForGeneralScrollListUtils.<>c__DisplayClass11_0<T> CS$<>8__locals1 = new CharacterDisplayDataForGeneralScrollListUtils.<>c__DisplayClass11_0<T>();
				CS$<>8__locals1.index = i;
				yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", CS$<>8__locals1.index)), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.LifeSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(66 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Growth.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CharacterDisplayDataForGeneralScrollListUtils.GetSkillGrowthString(data.ActualAge, data.LifeSkillGrowthType), 118, 30f, 90f);
			yield break;
		}

		// Token: 0x06008FDD RID: 36829 RVA: 0x00431223 File Offset: 0x0042F423
		private static IEnumerable<ColumnDefinition> GenerateCombatSkillColumns<T>(Func<T, CharacterDisplayDataForGeneralScrollList> dataProvider)
		{
			int num;
			for (int i = 0; i < 14; i = num + 1)
			{
				CharacterDisplayDataForGeneralScrollListUtils.<>c__DisplayClass12_0<T> CS$<>8__locals1 = new CharacterDisplayDataForGeneralScrollListUtils.<>c__DisplayClass12_0<T>();
				CS$<>8__locals1.index = i;
				yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", CS$<>8__locals1.index)), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.CombatSkillQualifications[CS$<>8__locals1.index].ToString(), (short)(82 + CS$<>8__locals1.index), 40f, 60f);
				CS$<>8__locals1 = null;
				num = i;
			}
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Growth.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CharacterDisplayDataForGeneralScrollListUtils.GetSkillGrowthString(data.ActualAge, data.CombatSkillGrowthType), 119, 30f, 90f);
			yield break;
		}

		// Token: 0x06008FDE RID: 36830 RVA: 0x00431233 File Offset: 0x0042F433
		private static IEnumerable<ColumnDefinition> GeneratePersonalityColumns<T>(Func<T, CharacterDisplayDataForGeneralScrollList> dataProvider)
		{
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Personality_Calm_Name.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[0].ToString(), 96, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Personality_Clever_Name.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[1].ToString(), 97, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Personality_Enthusiastic_Name.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[2].ToString(), 98, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Personality_Brave_Name.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[3].ToString(), 99, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Personality_Firm_Name.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[4].ToString(), 100, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Personality_Lucky_Name.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[5].ToString(), 101, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Personality_Perceptive_Name.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Personalities[6].ToString(), 102, 30f, 90f);
			yield break;
		}

		// Token: 0x06008FDF RID: 36831 RVA: 0x00431243 File Offset: 0x0042F443
		private static IEnumerable<ColumnDefinition> GenerateItemColumns<T>(Func<T, CharacterDisplayDataForGeneralScrollList> dataProvider)
		{
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Resource_Name_Food.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Resources[0].ToString(), 103, 40f, 60f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Resource_Name_Wood.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Resources[1].ToString(), 104, 40f, 60f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Resource_Name_Metal.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Resources[2].ToString(), 105, 40f, 60f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Resource_Name_Jade.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Resources[3].ToString(), 106, 40f, 60f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Resource_Name_Fabric.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Resources[4].ToString(), 107, 40f, 60f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Resource_Name_Herb.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Resources[5].ToString(), 108, 40f, 60f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Resource_Name_Money.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Resources[6].ToString(), 109, 40f, 60f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Resource_Name_Authority.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.Resources[7].ToString(), 110, 40f, 60f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Inventory.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CharacterDisplayDataForGeneralScrollListUtils.GetInventoryLoadString(data.CurrInventoryLoad, data.MaxInventoryLoad), 37, 30f, 90f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateTextColumn<T>(() => LanguageKey.LK_Kidnap.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => data.KidnapCount.ToString(), 111, 30f, 90f);
			yield break;
		}

		// Token: 0x06008FE0 RID: 36832 RVA: 0x00431253 File Offset: 0x0042F453
		private static IEnumerable<ColumnDefinition> GenerateCommandColumns<T>(Func<T, CharacterDisplayDataForGeneralScrollList> dataProvider)
		{
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateIconAndTextColumn<T>(() => LanguageKey.LK_Feature_Attack.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CharacterDisplayDataForGeneralScrollListUtils.CreateMedalCellData(data.AttackMedal, 0), 112, 80f, 120f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateIconAndTextColumn<T>(() => LanguageKey.LK_Feature_Defence.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CharacterDisplayDataForGeneralScrollListUtils.CreateMedalCellData(data.DefenceMedal, 1), 113, 80f, 120f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateIconAndTextColumn<T>(() => LanguageKey.LK_Feature_Wisdom.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CharacterDisplayDataForGeneralScrollListUtils.CreateMedalCellData(data.WisdomMedal, 2), 114, 80f, 120f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateIconAndTextColumn<T>(() => LanguageKey.LK_Team_Property_Title_Command_0.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CharacterDisplayDataForGeneralScrollListUtils.CreateCommandCellData(data, 0), 115, 80f, 120f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateIconAndTextColumn<T>(() => LanguageKey.LK_Team_Property_Title_Command_1.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CharacterDisplayDataForGeneralScrollListUtils.CreateCommandCellData(data, 1), 116, 80f, 120f);
			yield return CharacterDisplayDataForGeneralScrollListUtils.CreateIconAndTextColumn<T>(() => LanguageKey.LK_Team_Property_Title_Command_2.Tr(), dataProvider, (CharacterDisplayDataForGeneralScrollList data) => CharacterDisplayDataForGeneralScrollListUtils.CreateCommandCellData(data, 2), 117, 80f, 120f);
			yield break;
		}

		// Token: 0x06008FE1 RID: 36833 RVA: 0x00431264 File Offset: 0x0042F464
		private static ColumnDefinition CreateAvatarWithNameColumn<T>(Func<T, CharacterDisplayDataForGeneralScrollList> dataProvider, Action<int> onCharacterClicked, Action<TooltipInvoker, int> mouseTipModifier = null)
		{
			ColumnDefinition<T, AvatarWithNameCellData> columnDefinition = new ColumnDefinition<T, AvatarWithNameCellData>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 330f,
				FlexibleWidth = 0f,
				PreferredWidth = 330f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Char_Name.Tr());
			columnDefinition.CellDataGenerator = delegate(T data)
			{
				CharacterDisplayDataForGeneralScrollList subData = dataProvider(data);
				return AvatarWithNameCellData.FromCharacterDisplayDataForGeneralScrollList(subData, CharacterDisplayDataForGeneralScrollListUtils.<CreateAvatarWithNameColumn>g__IsTaiwu|16_0<T>(subData.CharacterId), onCharacterClicked, mouseTipModifier);
			};
			columnDefinition.SortId = 0;
			return columnDefinition;
		}

		// Token: 0x06008FE2 RID: 36834 RVA: 0x00431310 File Offset: 0x0042F510
		private static ColumnDefinition CreateIconAndTextColumn<T>(Func<string> headerKey, Func<T, CharacterDisplayDataForGeneralScrollList> dataProvider, Func<CharacterDisplayDataForGeneralScrollList, IconAndTextCellData> valueGetter, short sortId = -1, float minWidth = 80f, float preferredWidth = 120f)
		{
			return new ColumnDefinition<T, IconAndTextCellData>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = ((T data) => valueGetter(dataProvider(data))),
				SortId = sortId
			};
		}

		// Token: 0x06008FE3 RID: 36835 RVA: 0x00431390 File Offset: 0x0042F590
		private static ColumnDefinition CreateTextColumn<T>(Func<string> headerKey, Func<T, CharacterDisplayDataForGeneralScrollList> dataProvider, Func<CharacterDisplayDataForGeneralScrollList, string> valueGetter, short sortId = -1, float minWidth = 30f, float preferredWidth = 90f)
		{
			return new ColumnDefinition<T, string>
			{
				LayoutOption = new LayoutOption
				{
					MinWidth = minWidth,
					FlexibleWidth = 1f,
					PreferredWidth = preferredWidth,
					Priority = 1
				},
				TableHeadLabel = headerKey,
				CellDataGenerator = ((T data) => valueGetter(dataProvider(data))),
				SortId = sortId
			};
		}

		// Token: 0x06008FE4 RID: 36836 RVA: 0x00431410 File Offset: 0x0042F610
		private static string GetCharmDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			return CommonUtils.GetCharmLevelText(data.Charm, data.Gender, data.PhysiologicalAge, data.ClothDisplayId, CreatingType.IsFixedPresetType(data.CreatingType), data.FaceVisible);
		}

		// Token: 0x06008FE5 RID: 36837 RVA: 0x00431450 File Offset: 0x0042F650
		private static string GetFavorDisplayString(CharacterDisplayDataForGeneralScrollList data)
		{
			return CommonUtils.GetFavorStringByInteracted(data.FavorabilityToTaiwu, data.IsInteractedWithTaiwu);
		}

		// Token: 0x06008FE6 RID: 36838 RVA: 0x00431474 File Offset: 0x0042F674
		private static string GetSkillGrowthString(short actualAge, sbyte growthType)
		{
			sbyte addValue = CharacterDisplayDataForGeneralScrollListUtils.GetSkillGrowthAddValue(actualAge, (int)growthType);
			string growthName = (growthType == 0) ? LocalStringManager.Get("LK_Qualification_Growth_Average") : ((growthType == 1) ? LocalStringManager.Get("LK_Qualification_Growth_Precocious") : LocalStringManager.Get("LK_Qualification_Growth_LateBlooming"));
			bool flag = addValue > 0;
			string addValueStr;
			if (flag)
			{
				addValueStr = string.Format("+{0}", addValue).SetColor("lightblue");
			}
			else
			{
				bool flag2 = addValue == 0;
				if (flag2)
				{
					addValueStr = "+0".SetColor("lightgrey");
				}
				else
				{
					addValueStr = string.Format("{0}", addValue).SetColor("red");
				}
			}
			return growthName + addValueStr;
		}

		// Token: 0x06008FE7 RID: 36839 RVA: 0x00431520 File Offset: 0x0042F720
		private static sbyte GetSkillGrowthAddValue(short actualAge, int growthType)
		{
			AgeEffectItem ageData = AgeEffect.Instance[Math.Min((int)actualAge, AgeEffect.Instance.Count - 1)];
			return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
		}

		// Token: 0x06008FE8 RID: 36840 RVA: 0x0043156C File Offset: 0x0042F76C
		private static string GetInventoryLoadString(int currLoad, int maxLoad)
		{
			string currLoadStr = ((float)currLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(currLoad, maxLoad));
			return string.Format("{0}/{1:f1}", currLoadStr, (float)maxLoad / 100f);
		}

		// Token: 0x06008FE9 RID: 36841 RVA: 0x004315B8 File Offset: 0x0042F7B8
		private static IconAndTextCellData CreateMedalCellData(int medalCount, int medalType)
		{
			bool flag = medalCount == 0;
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				string iconName = CharacterDisplayDataForGeneralScrollListUtils.GetMedalIconName(medalCount, medalType);
				string text = string.Format(" x{0}", Mathf.Abs(medalCount));
				result = new IconAndTextCellData(iconName, text, true, false, false, false);
			}
			return result;
		}

		// Token: 0x06008FEA RID: 36842 RVA: 0x0043160C File Offset: 0x0042F80C
		private static string GetMedalIconName(int medalCount, int medalType)
		{
			int signKey = (medalCount > 0) ? 1 : ((medalCount < 0) ? -1 : 0);
			if (!true)
			{
			}
			string text;
			switch (medalType)
			{
			case 0:
				text = MedalSummary.AttackMedalIconConfig[signKey];
				break;
			case 1:
				text = MedalSummary.DefenceMedalIconConfig[signKey];
				break;
			case 2:
				text = MedalSummary.WisdomMedalIconConfig[signKey];
				break;
			default:
				text = string.Empty;
				break;
			}
			if (!true)
			{
			}
			string iconNumber = text;
			return "ui9_icon_strategy_big_" + iconNumber;
		}

		// Token: 0x06008FEB RID: 36843 RVA: 0x0043168C File Offset: 0x0042F88C
		private static IconAndTextCellData CreateCommandCellData(CharacterDisplayDataForGeneralScrollList data, int commandIndex)
		{
			bool flag = data.Command.Items == null || !data.Command.Items.CheckIndex(commandIndex);
			IconAndTextCellData result;
			if (flag)
			{
				result = IconAndTextCellData.TextOnly("-");
			}
			else
			{
				sbyte commandId = data.Command.Items[commandIndex];
				bool flag2 = commandId < 0;
				if (flag2)
				{
					result = IconAndTextCellData.TextOnly("-");
				}
				else
				{
					TeammateCommandItem cmdConfig = Config.TeammateCommand.Instance[commandId];
					result = IconAndTextCellData.TextOnly(cmdConfig.Name);
				}
			}
			return result;
		}

		// Token: 0x06008FED RID: 36845 RVA: 0x0043172E File Offset: 0x0042F92E
		[CompilerGenerated]
		internal static bool <CreateAvatarWithNameColumn>g__IsTaiwu|16_0<T>(int charId)
		{
			return charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}

		// Token: 0x04006EB1 RID: 28337
		public static readonly IReadOnlyList<short> SortIdsList = new short[]
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

		// Token: 0x02002155 RID: 8533
		public enum SubPage
		{
			// Token: 0x0400D493 RID: 54419
			State,
			// Token: 0x0400D494 RID: 54420
			Property,
			// Token: 0x0400D495 RID: 54421
			Property2,
			// Token: 0x0400D496 RID: 54422
			LifeSkill,
			// Token: 0x0400D497 RID: 54423
			CombatSkill,
			// Token: 0x0400D498 RID: 54424
			Personality,
			// Token: 0x0400D499 RID: 54425
			Item,
			// Token: 0x0400D49A RID: 54426
			Command
		}

		// Token: 0x02002156 RID: 8534
		public static class SortIds
		{
			// Token: 0x0400D49B RID: 54427
			public const short Name = 0;

			// Token: 0x0400D49C RID: 54428
			public const short Age = 8;

			// Token: 0x0400D49D RID: 54429
			public const short Health = 10;

			// Token: 0x0400D49E RID: 54430
			public const short Charm = 9;

			// Token: 0x0400D49F RID: 54431
			public const short Happiness = 12;

			// Token: 0x0400D4A0 RID: 54432
			public const short Favor = 11;

			// Token: 0x0400D4A1 RID: 54433
			public const short Alertness = 130;

			// Token: 0x0400D4A2 RID: 54434
			public const short DefeatMark = 53;

			// Token: 0x0400D4A3 RID: 54435
			public const short Behavior = 57;

			// Token: 0x0400D4A4 RID: 54436
			public const short Samsara = 58;

			// Token: 0x0400D4A5 RID: 54437
			public const short Fame = 59;

			// Token: 0x0400D4A6 RID: 54438
			public const short Strength = 60;

			// Token: 0x0400D4A7 RID: 54439
			public const short Dexterity = 61;

			// Token: 0x0400D4A8 RID: 54440
			public const short Concentration = 62;

			// Token: 0x0400D4A9 RID: 54441
			public const short Vitality = 63;

			// Token: 0x0400D4AA RID: 54442
			public const short Energy = 64;

			// Token: 0x0400D4AB RID: 54443
			public const short Intelligence = 65;

			// Token: 0x0400D4AC RID: 54444
			public const short OuterPenetrate = 22;

			// Token: 0x0400D4AD RID: 54445
			public const short InnerPenetrate = 23;

			// Token: 0x0400D4AE RID: 54446
			public const short OuterPenetrateResist = 29;

			// Token: 0x0400D4AF RID: 54447
			public const short InnerPenetrateResist = 30;

			// Token: 0x0400D4B0 RID: 54448
			public const short HitStrength = 24;

			// Token: 0x0400D4B1 RID: 54449
			public const short HitTechnique = 25;

			// Token: 0x0400D4B2 RID: 54450
			public const short HitSpeed = 26;

			// Token: 0x0400D4B3 RID: 54451
			public const short HitMind = 27;

			// Token: 0x0400D4B4 RID: 54452
			public const short AvoidStrength = 33;

			// Token: 0x0400D4B5 RID: 54453
			public const short AvoidTechnique = 34;

			// Token: 0x0400D4B6 RID: 54454
			public const short AvoidSpeed = 35;

			// Token: 0x0400D4B7 RID: 54455
			public const short AvoidMind = 36;

			// Token: 0x0400D4B8 RID: 54456
			public const short QiDisorder = 55;

			// Token: 0x0400D4B9 RID: 54457
			public const short LifeSkill0 = 66;

			// Token: 0x0400D4BA RID: 54458
			public const short LifeSkill1 = 67;

			// Token: 0x0400D4BB RID: 54459
			public const short LifeSkill2 = 68;

			// Token: 0x0400D4BC RID: 54460
			public const short LifeSkill3 = 69;

			// Token: 0x0400D4BD RID: 54461
			public const short LifeSkill4 = 70;

			// Token: 0x0400D4BE RID: 54462
			public const short LifeSkill5 = 71;

			// Token: 0x0400D4BF RID: 54463
			public const short LifeSkill6 = 72;

			// Token: 0x0400D4C0 RID: 54464
			public const short LifeSkill7 = 73;

			// Token: 0x0400D4C1 RID: 54465
			public const short LifeSkill8 = 74;

			// Token: 0x0400D4C2 RID: 54466
			public const short LifeSkill9 = 75;

			// Token: 0x0400D4C3 RID: 54467
			public const short LifeSkill10 = 76;

			// Token: 0x0400D4C4 RID: 54468
			public const short LifeSkill11 = 77;

			// Token: 0x0400D4C5 RID: 54469
			public const short LifeSkill12 = 78;

			// Token: 0x0400D4C6 RID: 54470
			public const short LifeSkill13 = 79;

			// Token: 0x0400D4C7 RID: 54471
			public const short LifeSkill14 = 80;

			// Token: 0x0400D4C8 RID: 54472
			public const short LifeSkill15 = 81;

			// Token: 0x0400D4C9 RID: 54473
			public const short LifeSkillGrowth = 118;

			// Token: 0x0400D4CA RID: 54474
			public const short CombatSkill0 = 82;

			// Token: 0x0400D4CB RID: 54475
			public const short CombatSkill1 = 83;

			// Token: 0x0400D4CC RID: 54476
			public const short CombatSkill2 = 84;

			// Token: 0x0400D4CD RID: 54477
			public const short CombatSkill3 = 85;

			// Token: 0x0400D4CE RID: 54478
			public const short CombatSkill4 = 86;

			// Token: 0x0400D4CF RID: 54479
			public const short CombatSkill5 = 87;

			// Token: 0x0400D4D0 RID: 54480
			public const short CombatSkill6 = 88;

			// Token: 0x0400D4D1 RID: 54481
			public const short CombatSkill7 = 89;

			// Token: 0x0400D4D2 RID: 54482
			public const short CombatSkill8 = 90;

			// Token: 0x0400D4D3 RID: 54483
			public const short CombatSkill9 = 91;

			// Token: 0x0400D4D4 RID: 54484
			public const short CombatSkill10 = 92;

			// Token: 0x0400D4D5 RID: 54485
			public const short CombatSkill11 = 93;

			// Token: 0x0400D4D6 RID: 54486
			public const short CombatSkill12 = 94;

			// Token: 0x0400D4D7 RID: 54487
			public const short CombatSkill13 = 95;

			// Token: 0x0400D4D8 RID: 54488
			public const short CombatSkillGrowth = 119;

			// Token: 0x0400D4D9 RID: 54489
			public const short Personality0 = 96;

			// Token: 0x0400D4DA RID: 54490
			public const short Personality1 = 97;

			// Token: 0x0400D4DB RID: 54491
			public const short Personality2 = 98;

			// Token: 0x0400D4DC RID: 54492
			public const short Personality3 = 99;

			// Token: 0x0400D4DD RID: 54493
			public const short Personality4 = 100;

			// Token: 0x0400D4DE RID: 54494
			public const short Personality5 = 101;

			// Token: 0x0400D4DF RID: 54495
			public const short Personality6 = 102;

			// Token: 0x0400D4E0 RID: 54496
			public const short Resource0 = 103;

			// Token: 0x0400D4E1 RID: 54497
			public const short Resource1 = 104;

			// Token: 0x0400D4E2 RID: 54498
			public const short Resource2 = 105;

			// Token: 0x0400D4E3 RID: 54499
			public const short Resource3 = 106;

			// Token: 0x0400D4E4 RID: 54500
			public const short Resource4 = 107;

			// Token: 0x0400D4E5 RID: 54501
			public const short Resource5 = 108;

			// Token: 0x0400D4E6 RID: 54502
			public const short Resource6 = 109;

			// Token: 0x0400D4E7 RID: 54503
			public const short Resource7 = 110;

			// Token: 0x0400D4E8 RID: 54504
			public const short InventoryLoad = 37;

			// Token: 0x0400D4E9 RID: 54505
			public const short KidnapCount = 111;

			// Token: 0x0400D4EA RID: 54506
			public const short AttackMedal = 112;

			// Token: 0x0400D4EB RID: 54507
			public const short DefenceMedal = 113;

			// Token: 0x0400D4EC RID: 54508
			public const short WisdomMedal = 114;

			// Token: 0x0400D4ED RID: 54509
			public const short Command0 = 115;

			// Token: 0x0400D4EE RID: 54510
			public const short Command1 = 116;

			// Token: 0x0400D4EF RID: 54511
			public const short Command2 = 117;
		}
	}
}
