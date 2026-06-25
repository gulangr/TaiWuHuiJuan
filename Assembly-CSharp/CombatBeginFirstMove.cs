using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Combat;
using UnityEngine;

// Token: 0x02000154 RID: 340
[RequireComponent(typeof(CImage))]
[RequireComponent(typeof(TooltipInvoker))]
public class CombatBeginFirstMove : MonoBehaviour
{
	// Token: 0x060012D1 RID: 4817 RVA: 0x00072D6C File Offset: 0x00070F6C
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
		CImage component = base.GetComponent<CImage>();
		if (!true)
		{
		}
		string spriteName;
		switch (prepareCombatResult)
		{
		case EPrepareCombatResult.SelfFirst:
			spriteName = ((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? (this.ally ? "ui9_btn_first_cn_0" : "ui9_btn_first_cn_1") : (this.ally ? "ui9_btn_first_en_0" : "ui9_btn_first_en_1"));
			break;
		case EPrepareCombatResult.EnemyFirst:
			spriteName = ((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? (this.ally ? "ui9_btn_second_cn_0" : "ui9_btn_second_cn_1") : (this.ally ? "ui9_btn_second_en_0" : "ui9_btn_second_en_1"));
			break;
		case EPrepareCombatResult.EqualsRandom:
			spriteName = "ui9_btn_move_random_0";
			break;
		default:
			throw new ArgumentException("invalid prepareCombatResult");
		}
		if (!true)
		{
		}
		component.SetSprite(spriteName, true, null);
		bool flag2 = !noTip;
		if (flag2)
		{
			CombatBeginFirstMove.RefreshTips(base.GetComponent<TooltipInvoker>(), prepareCombatResult);
		}
	}

	// Token: 0x060012D2 RID: 4818 RVA: 0x00072E68 File Offset: 0x00071068
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
		CombatBeginFirstMove.AddNode(tipParam, descLine, ref lineCount);
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
			CombatBeginFirstMove.AddNode(tipParam, subTitleLine, ref lineCount);
			bool flag2 = isFirst;
			if (flag2)
			{
				for (int i = 0; i < CombatBeginFirstMove.FirstIcons.Length; i++)
				{
					GeneralLineData generalLineData = CombatBeginFirstMove.EffectLine(CombatBeginFirstMove.FirstIcons[i], LocalStringManager.Get("LK_CombatBegin_FirstMoveTips_Content_First_" + (i + 1).ToString()));
					CombatBeginFirstMove.AddNode(tipParam, generalLineData, ref lineCount);
				}
			}
			else
			{
				for (int j = 0; j < CombatBeginFirstMove.LastIcons.Length; j++)
				{
					GeneralLineData generalLineData2 = CombatBeginFirstMove.EffectLine(CombatBeginFirstMove.LastIcons[j], LocalStringManager.Get("LK_CombatBegin_FirstMoveTips_Content_Last_" + (j + 1).ToString()));
					CombatBeginFirstMove.AddNode(tipParam, generalLineData2, ref lineCount);
				}
			}
		}
		tipParam.Set("LineCount", lineCount);
	}

	// Token: 0x060012D3 RID: 4819 RVA: 0x00073054 File Offset: 0x00071254
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

	// Token: 0x060012D4 RID: 4820 RVA: 0x00073094 File Offset: 0x00071294
	private static void AddNode(ArgumentBox tipParam, GeneralLineData lineData, ref int lineCount)
	{
		string format = "LineData{0}";
		int num = lineCount + 1;
		lineCount = num;
		tipParam.SetObject(string.Format(format, num), lineData);
	}

	// Token: 0x04000FF5 RID: 4085
	private static readonly string[] FirstIcons = new string[]
	{
		"mousetip_ciyao_1",
		"mousetip_tiqi",
		"mousetip_jiashi",
		"mousetip_zhiling"
	};

	// Token: 0x04000FF6 RID: 4086
	private static readonly string[] LastIcons = new string[]
	{
		"mousetip_ciyao_1",
		"mousetip_zhiling"
	};

	// Token: 0x04000FF7 RID: 4087
	[SerializeField]
	private bool ally;
}
