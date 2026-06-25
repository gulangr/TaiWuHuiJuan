using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FrameWork;
using GameData.Combat.Math;

namespace AiEditor
{
	// Token: 0x02000687 RID: 1671
	public static class ExpressionBuilderHelper
	{
		// Token: 0x06004EC4 RID: 20164 RVA: 0x0024F3C4 File Offset: 0x0024D5C4
		private static IReadOnlyList<IExpressionPartBuilder> AnalysisAllPartBuilders()
		{
			return (from type in typeof(ExpressionBuilderHelper).Assembly.GetTypes()
			where type.GetInterfaces().Contains(typeof(IExpressionPartBuilder))
			select (IExpressionPartBuilder)Activator.CreateInstance(type)).ToList<IExpressionPartBuilder>();
		}

		// Token: 0x06004EC5 RID: 20165 RVA: 0x0024F438 File Offset: 0x0024D638
		public static bool TryBuildExpression(string input, out CExpression expression)
		{
			ExpressionBuilderHelper.<>c__DisplayClass6_0 CS$<>8__locals1 = new ExpressionBuilderHelper.<>c__DisplayClass6_0();
			CS$<>8__locals1.input = input;
			CS$<>8__locals1.input = ExpressionBuilderHelper.OperatorAliases.Aggregate(CS$<>8__locals1.input, (string current, KeyValuePair<string, string> alias) => current.Replace(alias.Key, alias.Value));
			CS$<>8__locals1.input = ExpressionBuilderHelper.LanguageAliases.Aggregate(CS$<>8__locals1.input, (string current, KeyValuePair<LanguageKey, string> alias) => current.Replace(LocalStringManager.Get(alias.Key), alias.Value));
			CS$<>8__locals1.input = ExpressionBuilderHelper.RegexNotValid.Replace(CS$<>8__locals1.input, "");
			expression = null;
			bool flag = string.IsNullOrEmpty(CS$<>8__locals1.input);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				CS$<>8__locals1.builder = EasyPool.Get<ExpressionBuilder>();
				CS$<>8__locals1.operators = EasyPool.Get<Stack<char>>();
				CS$<>8__locals1.builder.Clear();
				CS$<>8__locals1.operators.Clear();
				int j;
				int i;
				for (i = 0; i < CS$<>8__locals1.input.Length; i = j + 1)
				{
					char c = CS$<>8__locals1.input[i];
					IExpressionPartBuilder partBuilder = ExpressionBuilderHelper.PartBuilders.FirstOrDefault((IExpressionPartBuilder x) => x.CanBuild(CS$<>8__locals1.input, i));
					bool flag2 = partBuilder != null;
					if (flag2)
					{
						Regex regex = partBuilder.Regex;
						string input2 = CS$<>8__locals1.input;
						j = i;
						Match match = regex.Match(input2.Substring(j, input2.Length - j));
						i += match.Length - 1;
						partBuilder.Build(CS$<>8__locals1.builder, match);
					}
					else
					{
						bool flag3 = CS$<>8__locals1.operators.Count == 0;
						if (flag3)
						{
							CS$<>8__locals1.operators.Push(c);
						}
						else
						{
							for (;;)
							{
								if (CS$<>8__locals1.operators.Count == 0)
								{
									goto IL_231;
								}
								char c2 = CS$<>8__locals1.operators.Peek();
								if (c2 != '×' && c2 != '÷')
								{
									goto IL_231;
								}
								bool flag4 = c == '+' || c == '-' || c == '×' || c == '÷';
								IL_232:
								if (!flag4)
								{
									break;
								}
								CS$<>8__locals1.<TryBuildExpression>g__PopBuildOperator|2();
								continue;
								IL_231:
								flag4 = false;
								goto IL_232;
							}
							bool flag5 = c == ')';
							if (flag5)
							{
								while (CS$<>8__locals1.operators.Peek() != '(')
								{
									CS$<>8__locals1.<TryBuildExpression>g__PopBuildOperator|2();
								}
								CS$<>8__locals1.operators.Pop();
							}
							else
							{
								CS$<>8__locals1.operators.Push(c);
							}
						}
					}
					j = i;
				}
				while (CS$<>8__locals1.operators.Count != 0)
				{
					CS$<>8__locals1.<TryBuildExpression>g__PopBuildOperator|2();
				}
				expression = CS$<>8__locals1.builder.ToExpressionNoWarnings();
				EasyPool.Free<ExpressionBuilder>(CS$<>8__locals1.builder);
				EasyPool.Free<Stack<char>>(CS$<>8__locals1.operators);
				result = (expression != null);
			}
			return result;
		}

		// Token: 0x0400364E RID: 13902
		private static readonly Dictionary<string, string> OperatorAliases = new Dictionary<string, string>
		{
			{
				"*",
				"×"
			},
			{
				"/",
				"÷"
			},
			{
				"（",
				"("
			},
			{
				"）",
				")"
			}
		};

		// Token: 0x0400364F RID: 13903
		private static readonly Dictionary<LanguageKey, string> LanguageAliases = new Dictionary<LanguageKey, string>
		{
			{
				LanguageKey.LK_Personality_Calm_Name,
				"$P" + 0.ToString()
			},
			{
				LanguageKey.LK_Personality_Clever_Name,
				"$P" + 1.ToString()
			},
			{
				LanguageKey.LK_Personality_Enthusiastic_Name,
				"$P" + 2.ToString()
			},
			{
				LanguageKey.LK_Personality_Brave_Name,
				"$P" + 3.ToString()
			},
			{
				LanguageKey.LK_Personality_Firm_Name,
				"$P" + 4.ToString()
			},
			{
				LanguageKey.LK_Personality_Lucky_Name,
				"$P" + 5.ToString()
			},
			{
				LanguageKey.LK_Personality_Perceptive_Name,
				"$P" + 6.ToString()
			},
			{
				LanguageKey.LK_ConsummateLevel,
				"$CL"
			},
			{
				LanguageKey.LK_Main_SummaryInfo_Behavior,
				"$BT"
			}
		};

		// Token: 0x04003650 RID: 13904
		private static readonly IReadOnlyList<IExpressionPartBuilder> PartBuilders = ExpressionBuilderHelper.AnalysisAllPartBuilders();

		// Token: 0x04003651 RID: 13905
		private static readonly Dictionary<char, EExpressionOperatorType> OperatorTypes = new Dictionary<char, EExpressionOperatorType>
		{
			{
				'+',
				EExpressionOperatorType.Add
			},
			{
				'-',
				EExpressionOperatorType.Sub
			},
			{
				'×',
				EExpressionOperatorType.Mul
			},
			{
				'÷',
				EExpressionOperatorType.Div
			}
		};

		// Token: 0x04003652 RID: 13906
		private static readonly Regex RegexNotValid = new Regex("[^0-9a-zA-Z()+\\-×÷\\$]", RegexOptions.Compiled);
	}
}
