using System;
using FrameWork;
using GameData.Domains.Taiwu.Debate;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002AF RID: 687
public class MouseTipLifeSkillCombatStress : MouseTipBase
{
	// Token: 0x170004A4 RID: 1188
	// (get) Token: 0x06002A86 RID: 10886 RVA: 0x00145BB3 File Offset: 0x00143DB3
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170004A5 RID: 1189
	// (get) Token: 0x06002A87 RID: 10887 RVA: 0x00145BB6 File Offset: 0x00143DB6
	private LifeSkillCombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>();
		}
	}

	// Token: 0x06002A88 RID: 10888 RVA: 0x00145BC0 File Offset: 0x00143DC0
	protected override void Init(ArgumentBox argsBox)
	{
		CanvasGroup canvasGroup = base.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		bool isTaiwu;
		argsBox.Get("IsTaiwu", out isTaiwu);
		DebatePlayer player = isTaiwu ? this.Model.DebateGame.PlayerLeft : this.Model.DebateGame.PlayerRight;
		string curStr = player.Pressure.ToString().SetColor("brightred");
		string maxStr = player.MaxPressure.ToString().SetColor("pinkyellow");
		base.CGet<TextMeshProUGUI>("State").text = LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_Stress_Tip_State_Content, curStr, maxStr);
		RectTransform addContent = base.CGet<RectTransform>("AddContent");
		for (int i = 0; i < addContent.childCount; i++)
		{
			GameObject child = addContent.GetChild(i).gameObject;
			LanguageKey key = LanguageKey.LK_LifeSkillCombat_Stress_Tip_Add_Content_1 + i;
			string text = LocalStringManager.GetFormat(key, Array.Empty<object>()).ColorReplace();
			child.GetComponentInChildren<TextMeshProUGUI>(true).text = text;
		}
		RectTransform effectContent = base.CGet<RectTransform>("EffectContent");
		for (int j = 0; j < effectContent.childCount; j++)
		{
			GameObject child2 = effectContent.GetChild(j).gameObject;
			LanguageKey key2 = LanguageKey.LK_LifeSkillCombat_Stress_Tip_Effect_Content_1 + j;
			string text2 = LocalStringManager.GetFormat(key2, Array.Empty<object>()).ColorReplace();
			if (!true)
			{
			}
			bool flag;
			switch (j)
			{
			case 1:
				flag = (player.Pressure < DebateConstants.LowPressurePercent);
				break;
			case 2:
				flag = (player.Pressure < DebateConstants.MidPressurePercent);
				break;
			case 3:
				flag = (player.Pressure < DebateConstants.HighPressurePercent);
				break;
			default:
				flag = false;
				break;
			}
			if (!true)
			{
			}
			bool disable = flag;
			DisableStyleRoot componentInChildren = child2.GetComponentInChildren<DisableStyleRoot>(true);
			if (componentInChildren != null)
			{
				componentInChildren.SetStyleEffect(disable, false);
			}
			child2.GetComponentInChildren<TextMeshProUGUI>(true).text = text2;
		}
		ContentSizeFitter[] contentSizeFitters = base.GetComponentsInChildren<ContentSizeFitter>();
		foreach (ContentSizeFitter contentSizeFitter in contentSizeFitters)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(contentSizeFitter.GetComponent<RectTransform>());
		}
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.RectTransform);
		});
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			canvasGroup.alpha = 1f;
		});
	}
}
