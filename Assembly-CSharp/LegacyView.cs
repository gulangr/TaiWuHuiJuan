using System;
using Config;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000237 RID: 567
public class LegacyView : Refers
{
	// Token: 0x170003C0 RID: 960
	// (get) Token: 0x060024F8 RID: 9464 RVA: 0x001103DA File Offset: 0x0010E5DA
	private TextMeshProUGUI _description
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("Description");
		}
	}

	// Token: 0x060024F9 RID: 9465 RVA: 0x001103E8 File Offset: 0x0010E5E8
	public void RefreshBasicInfo(LegacyItem configData)
	{
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("Grade").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		base.CGet<GameObject>("Valid").SetActive(true);
		base.CGet<GameObject>("Invalid").SetActive(false);
		base.CGet<TextMeshProUGUI>("NameLabel").SetText(configData.Name, true);
		base.CGet<CImage>("Icon").SetSprite(configData.Icon, false, null);
		TextMeshProUGUI desc;
		bool flag = this.CTryGet<TextMeshProUGUI>("Description", out desc);
		if (flag)
		{
			desc.text = configData.Desc.ColorReplace();
		}
	}

	// Token: 0x060024FA RID: 9466 RVA: 0x001104B9 File Offset: 0x0010E6B9
	public void RefreshHighlight(bool showHighlight, bool isExtra, bool isFixed = false)
	{
		base.CGet<GameObject>("HighlightBack").SetActive(showHighlight);
	}

	// Token: 0x060024FB RID: 9467 RVA: 0x001104D0 File Offset: 0x0010E6D0
	public void RefreshCostInfo(LegacyItem configData, bool showCost, bool isSelected, bool hasEnoughPoints, bool isExtra, bool isStarFortune = false)
	{
		int cost = (int)(isStarFortune ? configData.ExtraCost : configData.Cost);
		cost = ((isExtra && cost > 0) ? 0 : cost);
		base.CGet<GameObject>("CostBack").SetActive(showCost);
		base.CGet<GameObject>("CostNotEnoughBack").SetActive(!hasEnoughPoints && !isSelected);
		base.CGet<TextMeshProUGUI>("CostNotEnoughLabel").SetText(cost.ToString(), true);
		base.CGet<TextMeshProUGUI>("CostLabel").SetText(cost.ToString(), true);
	}

	// Token: 0x060024FC RID: 9468 RVA: 0x00110560 File Offset: 0x0010E760
	public void RefreshInteraction(bool isInteractable, bool isSelected, bool isDisabled)
	{
		base.CGet<GameObject>("Selected").SetActive(isSelected);
		CToggleObsolete toggle = base.CGet<CToggleObsolete>("Toggle");
		toggle.SetIsOnWithoutNotify(isSelected);
		toggle.interactable = (isSelected || isInteractable);
		if (isDisabled)
		{
			DisableStyleRoot disableStyleRoot = base.gameObject.GetOrAddComponent<DisableStyleRoot>();
			disableStyleRoot.SetStyleEffect(true, false);
			toggle.interactable = false;
		}
		else
		{
			DisableStyleRoot disableStyleRoot2 = base.gameObject.GetComponent<DisableStyleRoot>();
			bool flag = disableStyleRoot2;
			if (flag)
			{
				disableStyleRoot2.SetStyleEffect(false, false);
			}
		}
	}

	// Token: 0x060024FD RID: 9469 RVA: 0x001105E8 File Offset: 0x0010E7E8
	public void RefreshMouseTip(LegacyItem configData, string desc)
	{
		TooltipInvoker tipDisplayer = base.CGet<TooltipInvoker>("TipDisplayer");
		bool flag = !string.IsNullOrEmpty(desc);
		if (flag)
		{
			tipDisplayer.enabled = true;
			tipDisplayer.PresetParam[0] = configData.Name;
			tipDisplayer.PresetParam[1] = desc;
			tipDisplayer.Refresh(false, -1);
		}
		else
		{
			tipDisplayer.enabled = false;
		}
	}

	// Token: 0x060024FE RID: 9470 RVA: 0x00110644 File Offset: 0x0010E844
	public void SetOnToggleValueChanged(UnityAction<bool> action)
	{
		CToggleObsolete toggle = base.CGet<CToggleObsolete>("Toggle");
		toggle.onValueChanged.RemoveAllListeners();
		bool flag = action != null;
		if (flag)
		{
			toggle.onValueChanged.AddListener(action);
		}
	}
}
