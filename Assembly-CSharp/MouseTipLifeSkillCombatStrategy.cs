using System;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002AE RID: 686
public class MouseTipLifeSkillCombatStrategy : MouseTipBase
{
	// Token: 0x170004A3 RID: 1187
	// (get) Token: 0x06002A82 RID: 10882 RVA: 0x00145941 File Offset: 0x00143B41
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x00145944 File Offset: 0x00143B44
	protected override void Init(ArgumentBox argsBox)
	{
		this.Element.ForceListenCommand = true;
		CanvasGroup canvasGroup = base.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		short templateId;
		argsBox.Get("TemplateId", out templateId);
		bool isTargetMeet;
		argsBox.Get("IsTargetMeet", out isTargetMeet);
		bool isPointMeet;
		argsBox.Get("IsPointMeet", out isPointMeet);
		DebateStrategyItem config = DebateStrategy.Instance[templateId];
		base.CGet<TextMeshProUGUI>("Title").text = config.Name;
		base.CGet<TextMeshProUGUI>("Desc").text = config.StyleDesc.ColorReplace();
		TextMeshProUGUI effectDesc = base.CGet<TextMeshProUGUI>("EffectDesc");
		effectDesc.text = config.Desc.ColorReplace();
		effectDesc.GetComponent<TMPTextSpriteHelper>().Parse();
		base.CGet<CImage>("TypeIcon").SetSprite(string.Format("mousetip_jiyi_{0}", config.LifeSkillType), false, null);
		base.CGet<TextMeshProUGUI>("TypeName").text = LifeSkillType.Instance[config.LifeSkillType].Name;
		base.CGet<TextMeshProUGUI>("StrategyPoint").text = LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_Strategy_Tip_Condition_Content, config.UsedCost.ToString());
		bool flag = !isTargetMeet && config.NoTargetTip.IsNullOrEmpty();
		if (flag)
		{
			isTargetMeet = true;
		}
		base.CGet<GameObject>("TipLayout").SetActive(!isTargetMeet || !isPointMeet);
		base.CGet<GameObject>("PointTip").SetActive(!isPointMeet);
		TextMeshProUGUI targetTip = base.CGet<TextMeshProUGUI>("TargetTip");
		targetTip.text = config.NoTargetTip;
		targetTip.gameObject.SetActive(!isTargetMeet);
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

	// Token: 0x06002A84 RID: 10884 RVA: 0x00145B78 File Offset: 0x00143D78
	private void Update()
	{
		bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.DebateStrategy);
		}
	}
}
