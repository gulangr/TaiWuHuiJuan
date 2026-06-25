using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AED RID: 2797
	[RequireComponent(typeof(TMP_Text))]
	[RequireComponent(typeof(TooltipInvoker))]
	public class CombatBeginFirstMoveText : MonoBehaviour
	{
		// Token: 0x0600897A RID: 35194 RVA: 0x003F9C24 File Offset: 0x003F7E24
		public void Set(EPrepareCombatResult prepareCombatResult, bool noTip = false)
		{
			bool flag = !this.ally;
			if (flag)
			{
				if (!true)
				{
				}
				EPrepareCombatResult eprepareCombatResult;
				if (prepareCombatResult != EPrepareCombatResult.SelfFirst)
				{
					if (prepareCombatResult != EPrepareCombatResult.EnemyFirst)
					{
						eprepareCombatResult = prepareCombatResult;
					}
					else
					{
						eprepareCombatResult = EPrepareCombatResult.SelfFirst;
					}
				}
				else
				{
					eprepareCombatResult = EPrepareCombatResult.EnemyFirst;
				}
				if (!true)
				{
				}
				prepareCombatResult = eprepareCombatResult;
			}
			TMP_Text component = base.GetComponent<TMP_Text>();
			if (!true)
			{
			}
			string text;
			if (prepareCombatResult != EPrepareCombatResult.SelfFirst)
			{
				if (prepareCombatResult != EPrepareCombatResult.EnemyFirst)
				{
					text = LanguageKey.LK_QuestioMark_48.Tr().SetColor("lightgrey");
				}
				else
				{
					text = LanguageKey.UI_LifeSkillBattle_Prepare_Defensive.Tr().SetColor("brightred");
				}
			}
			else
			{
				text = LanguageKey.UI_LifeSkillBattle_Prepare_Offensive.Tr().SetColor("brightblue");
			}
			if (!true)
			{
			}
			component.text = text;
			bool flag2 = !noTip;
			if (flag2)
			{
				CombatBeginFirstMoveText.RefreshTips(base.GetComponent<TooltipInvoker>(), prepareCombatResult);
			}
		}

		// Token: 0x0600897B RID: 35195 RVA: 0x003F9CE0 File Offset: 0x003F7EE0
		private static void RefreshTips(TooltipInvoker tip, EPrepareCombatResult order)
		{
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			ArgumentBox tipParam = tip.RuntimeParam;
			tip.Type = TipType.GeneralLines;
			if (!true)
			{
			}
			LanguageKey languageKey;
			switch (order)
			{
			case EPrepareCombatResult.SelfFirst:
				languageKey = LanguageKey.LK_Combat_First_Move;
				break;
			case EPrepareCombatResult.EnemyFirst:
				languageKey = LanguageKey.LK_Combat_Last_Move;
				break;
			case EPrepareCombatResult.EqualsRandom:
				languageKey = LanguageKey.LK_CombatBegin_FirstMoveTips_Title_Unkown;
				break;
			default:
				throw new ArgumentOutOfRangeException("order", order, null);
			}
			if (!true)
			{
			}
			LanguageKey titleKey = languageKey;
			tipParam.Set("Title", LocalStringManager.Get(titleKey));
			int lineCount = 0;
			GeneralLineData descLine = new GeneralLineData
			{
				Type = 7,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_CombatBegin_FirstMoveTips)
				}
			};
			CombatBeginFirstMoveText.AddNode(tipParam, descLine, ref lineCount);
			bool flag = order != EPrepareCombatResult.EqualsRandom;
			if (flag)
			{
				bool isFirst = order == EPrepareCombatResult.SelfFirst;
				LanguageKey subTitleKey = isFirst ? LanguageKey.LK_CombatBegin_FirstMoveTips_Title_1 : LanguageKey.LK_CombatBegin_FirstMoveTips_Title_2;
				GeneralLineData subTitleLine = new GeneralLineData
				{
					Type = 1,
					Args = new List<string>
					{
						LocalStringManager.Get(subTitleKey)
					}
				};
				CombatBeginFirstMoveText.AddNode(tipParam, subTitleLine, ref lineCount);
				bool flag2 = isFirst;
				if (flag2)
				{
					for (int i = 0; i < CombatBeginFirstMoveText.FirstIcons.Length; i++)
					{
						GeneralLineData generalLineData = CombatBeginFirstMoveText.EffectLine(CombatBeginFirstMoveText.FirstIcons[i], LocalStringManager.Get("LK_CombatBegin_FirstMoveTips_Content_First_" + (i + 1).ToString()));
						CombatBeginFirstMoveText.AddNode(tipParam, generalLineData, ref lineCount);
					}
				}
				else
				{
					for (int j = 0; j < CombatBeginFirstMoveText.LastIcons.Length; j++)
					{
						GeneralLineData generalLineData2 = CombatBeginFirstMoveText.EffectLine(CombatBeginFirstMoveText.LastIcons[j], LocalStringManager.Get("LK_CombatBegin_FirstMoveTips_Content_Last_" + (j + 1).ToString()));
						CombatBeginFirstMoveText.AddNode(tipParam, generalLineData2, ref lineCount);
					}
				}
			}
			tipParam.Set("LineCount", lineCount);
		}

		// Token: 0x0600897C RID: 35196 RVA: 0x003F9ECC File Offset: 0x003F80CC
		private static GeneralLineData EffectLine(string iconName, string text)
		{
			return new GeneralLineData
			{
				Type = 2,
				Args = new List<string>
				{
					iconName,
					text.ColorReplace()
				}
			};
		}

		// Token: 0x0600897D RID: 35197 RVA: 0x003F9F0C File Offset: 0x003F810C
		private static void AddNode(ArgumentBox tipParam, GeneralLineData lineData, ref int lineCount)
		{
			string format = "LineData{0}";
			int num = lineCount + 1;
			lineCount = num;
			tipParam.SetObject(string.Format(format, num), lineData);
		}

		// Token: 0x0400695D RID: 26973
		private static readonly string[] FirstIcons = new string[]
		{
			"mousetip_ciyao_1",
			"mousetip_tiqi",
			"mousetip_jiashi",
			"mousetip_zhiling"
		};

		// Token: 0x0400695E RID: 26974
		private static readonly string[] LastIcons = new string[]
		{
			"mousetip_ciyao_1",
			"mousetip_zhiling"
		};

		// Token: 0x0400695F RID: 26975
		[SerializeField]
		private bool ally;
	}
}
