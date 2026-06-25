using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Taiwu.Debate;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002B0 RID: 688
public class MouseTipLifeSkillCombatUnit : MouseTipBase
{
	// Token: 0x170004A6 RID: 1190
	// (get) Token: 0x06002A8A RID: 10890 RVA: 0x00145E2C File Offset: 0x0014402C
	private LifeSkillCombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>();
		}
	}

	// Token: 0x06002A8B RID: 10891 RVA: 0x00145E34 File Offset: 0x00144034
	protected override void Init(ArgumentBox argsBox)
	{
		CanvasGroup canvasGroup = base.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		Pawn pawn;
		argsBox.Get<Pawn>("Pawn", out pawn);
		base.CGet<TextMeshProUGUI>("Desc").text = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Unit_Tip_Content).ColorReplace();
		base.CGet<TextMeshProUGUI>("Desc").GetComponent<TMPTextSpriteHelper>().Parse();
		bool showPower = pawn.IsRevealed || pawn.IsOwnedByTaiwu || this.Model.ShowHiddenInfo;
		base.CGet<GameObject>("PowerLayout").SetActive(showPower);
		int power = this.Model.DebateGame.GetPawnBases(pawn.Id, -1, false, true);
		base.CGet<TextMeshProUGUI>("Power").text = LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_Unit_Tip_Power_Content, power, pawn.Bases).ColorReplace();
		List<DebateGame.EffectItem> factorList = this.Model.DebateGame.GetPawnBasesFactorList(pawn.Id, false, true);
		GameObject effectItem = base.CGet<GameObject>("EffectItem");
		Transform effectItemLayout = effectItem.transform.parent;
		bool hasReverseEffect = factorList.Exists((DebateGame.EffectItem e) => e.EffectTemplateId == 8);
		for (int i = 0; i < factorList.Count; i++)
		{
			GameObject effect = (i < effectItemLayout.childCount) ? effectItemLayout.GetChild(i).gameObject : Object.Instantiate<GameObject>(effectItem, effectItemLayout);
			effect.gameObject.SetActive(true);
			DebateGame.EffectItem data = factorList[i];
			TextMeshProUGUI text = effect.GetComponentInChildren<TextMeshProUGUI>();
			DebateStrategyItem strategyConfig = DebateStrategy.Instance[data.StrategyTemplateId];
			bool flag = data.EffectTemplateId == 8;
			if (flag)
			{
				text.text = LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_Unit_Tip_Effect_Content_Reverse, strategyConfig.Name);
			}
			else
			{
				string effectText = (data.Value > 0) ? string.Format("+{0}%", data.Value).SetColor("brightblue") : string.Format("{0}%", data.Value).SetColor("brightred");
				string reverseText = hasReverseEffect ? LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Unit_Tip_Effect_Content_Reversed).ColorReplace() : string.Empty;
				text.text = LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_Unit_Tip_Effect_Content, strategyConfig.Name, effectText, reverseText).ColorReplace();
			}
		}
		for (int j = factorList.Count; j < effectItemLayout.childCount; j++)
		{
			effectItemLayout.GetChild(j).gameObject.SetActive(false);
		}
		bool showEffect = factorList.Count > 0;
		base.CGet<GameObject>("EffectLayout").SetActive(showEffect);
		base.CGet<TextMeshProUGUI>("State").text = LocalStringManager.Get(pawn.IsRevealed ? LanguageKey.LK_LifeSkillCombat_Unit_Tip_State_Revealed : LanguageKey.LK_LifeSkillCombat_Unit_Tip_State_NotRevealed).ColorReplace();
		base.CGet<TextMeshProUGUI>("State").GetComponent<TMPTextSpriteHelper>().Parse();
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

	// Token: 0x06002A8C RID: 10892 RVA: 0x001461C8 File Offset: 0x001443C8
	public override void UpdateOffsetPos()
	{
		float offsetY = (this.Model.FocusingCardItem == null) ? 0f : (-ConchShipCursor.Instance.GetLifeSkillCombatUseStrategyTipHeight());
		this.RightDownOffsetPos = new Vector2(0f, offsetY);
	}
}
