using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using GameData.Combat.Math;
using GameData.Utilities;
using UnityEngine;

namespace AiEditor
{
	// Token: 0x02000684 RID: 1668
	public static class AiParamHelper
	{
		// Token: 0x06004EAF RID: 20143 RVA: 0x0024E7EA File Offset: 0x0024C9EA
		private static bool IsValidForMapping(this IReadOnlyDictionary<LanguageKey, int> mapping, string input)
		{
			return mapping.Keys.Select(new Func<LanguageKey, string>(LocalStringManager.Get)).Any(new Func<string, bool>(input.Equals));
		}

		// Token: 0x06004EB0 RID: 20144 RVA: 0x0024E818 File Offset: 0x0024CA18
		private static int ConvertIntFromMapping(this IReadOnlyDictionary<LanguageKey, int> mapping, string input)
		{
			return mapping[mapping.Keys.First((LanguageKey x) => LocalStringManager.Get(x) == input)];
		}

		// Token: 0x06004EB1 RID: 20145 RVA: 0x0024E850 File Offset: 0x0024CA50
		public static string ParseAliases(this AiParamItem config, string input, bool toAnalysis = true)
		{
			string[] array = config.PrintingAliases;
			bool flag;
			if (array != null && array.Length > 0)
			{
				array = config.AnalysisAliases;
				flag = (array == null || array.Length <= 0);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			string result;
			if (flag2)
			{
				result = input;
			}
			else
			{
				string[] srcAliases = toAnalysis ? config.PrintingAliases : config.AnalysisAliases;
				string[] dstAliases = toAnalysis ? config.AnalysisAliases : config.PrintingAliases;
				Tester.Assert(srcAliases.Length == dstAliases.Length, "");
				for (int i = 0; i < srcAliases.Length; i++)
				{
					input = input.Replace(srcAliases[i], dstAliases[i]);
				}
				result = input;
			}
			return result;
		}

		// Token: 0x06004EB2 RID: 20146 RVA: 0x0024E8F8 File Offset: 0x0024CAF8
		public static bool IsValid(this AiParamItem config, string input)
		{
			bool result;
			try
			{
				input = config.ParseAliases(input, true);
				EAiParamType type4 = config.Type;
				if (!true)
				{
				}
				bool flag;
				switch (type4)
				{
				case EAiParamType.Int:
				{
					int num;
					flag = int.TryParse(input, out num);
					break;
				}
				case EAiParamType.Bool:
				case EAiParamType.IsAlly:
				case EAiParamType.IsForward:
				case EAiParamType.IsDirect:
				case EAiParamType.IsInner:
				case EAiParamType.IsNotOnlyInCombat:
				case EAiParamType.IsGood:
				{
					bool flag2;
					flag = bool.TryParse(input, out flag2);
					break;
				}
				case EAiParamType.CombatSkill:
					flag = (!string.IsNullOrEmpty(input) && CombatSkill.Instance.Any((CombatSkillItem x) => x.Name == input));
					break;
				case EAiParamType.String:
					flag = !string.IsNullOrEmpty(input);
					break;
				case EAiParamType.CombatDifficulty:
					flag = CombatDifficulty.Instance.Any((CombatDifficultyItem x) => x.Name == input);
					break;
				case EAiParamType.TeammateCommand:
					flag = TeammateCommand.Instance.Any((TeammateCommandItem x) => x.Name == input && x.Type != ETeammateCommandType.Negative);
					break;
				case EAiParamType.ProactiveSkillType:
					flag = AiParamHelper.MappingProactiveSkillType.IsValidForMapping(input);
					break;
				case EAiParamType.OtherActionType:
					flag = AiParamHelper.MappingOtherActionType.IsValidForMapping(input);
					break;
				case EAiParamType.BodyPartType:
					flag = BodyPart.Instance.Any((BodyPartItem x) => x.Name == input);
					break;
				case EAiParamType.Expression:
				{
					CExpression cexpression;
					flag = ExpressionBuilderHelper.TryBuildExpression(input, out cexpression);
					break;
				}
				case EAiParamType.PoisonType:
					flag = Poison.Instance.Any((PoisonItem x) => x.Name == input);
					break;
				case EAiParamType.WugType:
				{
					sbyte type;
					flag = (sbyte.TryParse(input, out type) && type >= 0 && type < 8);
					break;
				}
				case EAiParamType.NeiliAllocationType:
				{
					sbyte type2;
					flag = (sbyte.TryParse(input, out type2) && type2 >= 0 && type2 < 4);
					break;
				}
				case EAiParamType.TrickType:
					flag = TrickType.Instance.Any((TrickTypeItem x) => x.Name == input);
					break;
				case EAiParamType.Weapon:
					flag = Weapon.Instance.Any((WeaponItem x) => x.Name == input);
					break;
				case EAiParamType.WeaponSubType:
					flag = AiParamHelper.MappingWeaponSubType.IsValidForMapping(input);
					break;
				case EAiParamType.FiveElementsType:
				{
					sbyte type3;
					flag = (sbyte.TryParse(input, out type3) && type3 >= 0 && type3 <= 5);
					break;
				}
				case EAiParamType.CombatType:
					flag = AiParamHelper.MappingCombatType.IsValidForMapping(input);
					break;
				case EAiParamType.CombatStateName:
					flag = CombatState.Instance.Any((CombatStateItem x) => x.Name == input);
					break;
				case EAiParamType.Misc:
					flag = Misc.Instance.Any((MiscItem x) => x.Name == input);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				result = flag;
			}
			catch (Exception e)
			{
				Debug.LogWarning("IsValid catch exception " + e.Message + "\nstacktrace\n" + e.StackTrace);
				result = false;
			}
			return result;
		}

		// Token: 0x06004EB3 RID: 20147 RVA: 0x0024EC24 File Offset: 0x0024CE24
		public static int ConvertInt(this AiParamItem config, string input)
		{
			AiParamHelper.<>c__DisplayClass8_0 CS$<>8__locals1 = new AiParamHelper.<>c__DisplayClass8_0();
			CS$<>8__locals1.config = config;
			CS$<>8__locals1.input = input;
			CS$<>8__locals1.input = CS$<>8__locals1.config.ParseAliases(CS$<>8__locals1.input, true);
			bool flag = !CS$<>8__locals1.config.IsValid(CS$<>8__locals1.input);
			if (flag)
			{
				throw new AiParamInvalidException(string.Format("{0} mismatch {1}", CS$<>8__locals1.config.Type, CS$<>8__locals1.input));
			}
			EAiParamType type = CS$<>8__locals1.config.Type;
			if (!true)
			{
			}
			int result;
			switch (type)
			{
			case EAiParamType.Int:
				result = int.Parse(CS$<>8__locals1.input);
				goto IL_2A1;
			case EAiParamType.Bool:
			case EAiParamType.IsAlly:
			case EAiParamType.IsForward:
			case EAiParamType.IsDirect:
			case EAiParamType.IsInner:
			case EAiParamType.IsNotOnlyInCombat:
			case EAiParamType.IsGood:
				result = (bool.Parse(CS$<>8__locals1.input) ? 1 : 0);
				goto IL_2A1;
			case EAiParamType.CombatSkill:
				result = (int)CombatSkill.Instance.First((CombatSkillItem x) => x.Name == CS$<>8__locals1.input).TemplateId;
				goto IL_2A1;
			case EAiParamType.CombatDifficulty:
				result = (int)CombatDifficulty.Instance.First((CombatDifficultyItem x) => x.Name == CS$<>8__locals1.input).TemplateId;
				goto IL_2A1;
			case EAiParamType.TeammateCommand:
				result = (int)TeammateCommand.Instance.First((TeammateCommandItem x) => x.Name == CS$<>8__locals1.input).Implement;
				goto IL_2A1;
			case EAiParamType.ProactiveSkillType:
				result = AiParamHelper.MappingProactiveSkillType.ConvertIntFromMapping(CS$<>8__locals1.input);
				goto IL_2A1;
			case EAiParamType.OtherActionType:
				result = AiParamHelper.MappingOtherActionType.ConvertIntFromMapping(CS$<>8__locals1.input);
				goto IL_2A1;
			case EAiParamType.BodyPartType:
				result = (int)BodyPart.Instance.First((BodyPartItem x) => x.Name == CS$<>8__locals1.input).TemplateId;
				goto IL_2A1;
			case EAiParamType.PoisonType:
				result = (int)Poison.Instance.First((PoisonItem x) => x.Name == CS$<>8__locals1.input).TemplateId;
				goto IL_2A1;
			case EAiParamType.WugType:
				result = (int)sbyte.Parse(CS$<>8__locals1.input);
				goto IL_2A1;
			case EAiParamType.NeiliAllocationType:
				result = (int)sbyte.Parse(CS$<>8__locals1.input);
				goto IL_2A1;
			case EAiParamType.TrickType:
				result = (int)TrickType.Instance.First((TrickTypeItem x) => x.Name == CS$<>8__locals1.input).TemplateId;
				goto IL_2A1;
			case EAiParamType.Weapon:
				result = (int)Weapon.Instance.First((WeaponItem x) => x.Name == CS$<>8__locals1.input).TemplateId;
				goto IL_2A1;
			case EAiParamType.WeaponSubType:
				result = AiParamHelper.MappingWeaponSubType.ConvertIntFromMapping(CS$<>8__locals1.input);
				goto IL_2A1;
			case EAiParamType.FiveElementsType:
				result = (int)sbyte.Parse(CS$<>8__locals1.input);
				goto IL_2A1;
			case EAiParamType.CombatType:
				result = AiParamHelper.MappingCombatType.ConvertIntFromMapping(CS$<>8__locals1.input);
				goto IL_2A1;
			case EAiParamType.Misc:
				result = (int)Misc.Instance.First((MiscItem x) => x.Name == CS$<>8__locals1.input).TemplateId;
				goto IL_2A1;
			}
			result = CS$<>8__locals1.<ConvertInt>g__Throw|0();
			IL_2A1:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06004EB4 RID: 20148 RVA: 0x0024EEE0 File Offset: 0x0024D0E0
		public static string ConvertString(this AiParamItem config, string input)
		{
			AiParamHelper.<>c__DisplayClass9_0 CS$<>8__locals1;
			CS$<>8__locals1.config = config;
			input = CS$<>8__locals1.config.ParseAliases(input, true);
			bool flag = !CS$<>8__locals1.config.IsValid(input);
			if (flag)
			{
				throw new AiParamInvalidException(string.Format("{0} mismatch {1}", CS$<>8__locals1.config.Type, input));
			}
			EAiParamType type = CS$<>8__locals1.config.Type;
			if (!true)
			{
			}
			string result;
			if (type != EAiParamType.String)
			{
				if (type != EAiParamType.Expression)
				{
					if (type != EAiParamType.CombatStateName)
					{
						result = AiParamHelper.<ConvertString>g__Throw|9_0(ref CS$<>8__locals1);
					}
					else
					{
						result = input;
					}
				}
				else
				{
					CExpression expression;
					result = (ExpressionBuilderHelper.TryBuildExpression(input, out expression) ? CExpression.ToBase64(expression) : string.Empty);
				}
			}
			else
			{
				result = input;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06004EB6 RID: 20150 RVA: 0x0024F146 File Offset: 0x0024D346
		[CompilerGenerated]
		internal static string <ConvertString>g__Throw|9_0(ref AiParamHelper.<>c__DisplayClass9_0 A_0)
		{
			throw new AiParamInvalidException(string.Format("Cannot convert to string by type {0}", A_0.config.Type));
		}

		// Token: 0x04003647 RID: 13895
		private static readonly Dictionary<LanguageKey, int> MappingCombatType = new Dictionary<LanguageKey, int>
		{
			{
				LanguageKey.LK_Combat_Type_0,
				0
			},
			{
				LanguageKey.LK_Combat_Type_1,
				1
			},
			{
				LanguageKey.LK_Combat_Type_2,
				2
			},
			{
				LanguageKey.LK_Combat_Type_3,
				3
			}
		};

		// Token: 0x04003648 RID: 13896
		private static readonly Dictionary<LanguageKey, int> MappingProactiveSkillType = new Dictionary<LanguageKey, int>
		{
			{
				LanguageKey.LK_CombatSkill_EquipType_1,
				1
			},
			{
				LanguageKey.LK_CombatSkill_EquipType_2,
				2
			},
			{
				LanguageKey.LK_CombatSkill_EquipType_3,
				3
			}
		};

		// Token: 0x04003649 RID: 13897
		private static readonly Dictionary<LanguageKey, int> MappingOtherActionType = new Dictionary<LanguageKey, int>
		{
			{
				LanguageKey.LK_HotKeyGroup_Combat_Heal_Injury,
				0
			},
			{
				LanguageKey.LK_HotKeyGroup_Combat_Heal_Poison,
				1
			},
			{
				LanguageKey.LK_HotKeyGroup_Combat_Flee,
				2
			}
		};

		// Token: 0x0400364A RID: 13898
		private static readonly Dictionary<LanguageKey, int> MappingWeaponSubType = new Dictionary<LanguageKey, int>
		{
			{
				LanguageKey.LK_ItemSubType_0,
				0
			},
			{
				LanguageKey.LK_ItemSubType_1,
				1
			},
			{
				LanguageKey.LK_ItemSubType_2,
				2
			},
			{
				LanguageKey.LK_ItemSubType_3,
				3
			},
			{
				LanguageKey.LK_ItemSubType_4,
				4
			},
			{
				LanguageKey.LK_ItemSubType_5,
				5
			},
			{
				LanguageKey.LK_ItemSubType_6,
				6
			},
			{
				LanguageKey.LK_ItemSubType_7,
				7
			},
			{
				LanguageKey.LK_ItemSubType_8,
				8
			},
			{
				LanguageKey.LK_ItemSubType_9,
				9
			},
			{
				LanguageKey.LK_ItemSubType_10,
				10
			},
			{
				LanguageKey.LK_ItemSubType_11,
				11
			},
			{
				LanguageKey.LK_ItemSubType_12,
				12
			},
			{
				LanguageKey.LK_ItemSubType_13,
				13
			},
			{
				LanguageKey.LK_ItemSubType_14,
				14
			},
			{
				LanguageKey.LK_ItemSubType_15,
				15
			},
			{
				LanguageKey.LK_ItemSubType_16,
				16
			},
			{
				LanguageKey.LK_ItemSubType_17,
				17
			}
		};
	}
}
