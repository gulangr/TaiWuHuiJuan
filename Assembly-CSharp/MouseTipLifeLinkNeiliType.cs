using System;
using System.Text;
using Config;
using DisplayConfig;
using FrameWork;
using GameData.Domains.Character;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A9 RID: 681
public class MouseTipLifeLinkNeiliType : MouseTipBase
{
	// Token: 0x170004A0 RID: 1184
	// (get) Token: 0x06002A6C RID: 10860 RVA: 0x00144AAE File Offset: 0x00142CAE
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A6D RID: 10861 RVA: 0x00144AB4 File Offset: 0x00142CB4
	protected override void Init(ArgumentBox argsBox)
	{
		sbyte neiliType;
		argsBox.Get("NeiliType", out neiliType);
		NeiliProportionOfFiveElements total;
		argsBox.Get<NeiliProportionOfFiveElements>("Total", out total);
		NeiliTypeItem neiliTypeCfg = NeiliType.Instance[neiliType];
		base.CGet<TextMeshProUGUI>("Title").text = neiliTypeCfg.Name;
		if (this._stringBuilder == null)
		{
			this._stringBuilder = new StringBuilder();
		}
		this.RenderGate(base.CGet<Refers>("LifeGate"), neiliTypeCfg.LifeGateFeatures);
		this.RenderGate(base.CGet<Refers>("DeathGate"), neiliTypeCfg.DeathGateFeatures);
		TextMeshProUGUI fiveElements = base.CGet<TextMeshProUGUI>("FiveElements");
		fiveElements.text = this.GetFiveElementsTips(total, null);
		fiveElements.GetComponent<TMPTextSpriteHelper>().Parse();
		base.DelayFrameCall(new Action(this.RebuildSelf), 2U);
	}

	// Token: 0x06002A6E RID: 10862 RVA: 0x00144B7F File Offset: 0x00142D7F
	private void RebuildSelf()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.RectTransform);
	}

	// Token: 0x06002A6F RID: 10863 RVA: 0x00144B90 File Offset: 0x00142D90
	private void RenderGate(Refers gateRefers, short[] featureIds)
	{
		bool gateExist = featureIds != null && featureIds.Length > 0;
		gateRefers.gameObject.SetActive(gateExist);
		bool flag = !gateExist;
		if (!flag)
		{
			for (int i = 0; i < 2; i++)
			{
				TextMeshProUGUI featureText = gateRefers.CGet<TextMeshProUGUI>(string.Format("Feature_{0}", i));
				bool flag2 = !featureIds.CheckIndex(i);
				if (flag2)
				{
					featureText.gameObject.SetActive(false);
				}
				else
				{
					featureText.gameObject.SetActive(true);
					CharacterFeatureItem featureCfg = CharacterFeature.Instance[featureIds[i]];
					this._stringBuilder.Clear();
					this._stringBuilder.Append("<color=#orange>");
					this._stringBuilder.Append(featureCfg.Name);
					this._stringBuilder.Append("</color>");
					this._stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
					this._stringBuilder.Append(featureCfg.Desc);
					this._stringBuilder.Append('\n');
					this._stringBuilder.Append('-');
					MouseTipLifeLinkNeiliType.AppendFeatureDetail(this._stringBuilder, featureCfg);
					featureText.text = this._stringBuilder.ToString().ColorReplace();
					LayoutRebuilder.MarkLayoutForRebuild(featureText.rectTransform);
					LayoutRebuilder.MarkLayoutForRebuild(featureText.transform.parent as RectTransform);
				}
			}
			LayoutRebuilder.MarkLayoutForRebuild(gateRefers.transform as RectTransform);
			LayoutRebuilder.ForceRebuildLayoutImmediate(gateRefers.transform.parent as RectTransform);
		}
	}

	// Token: 0x06002A70 RID: 10864 RVA: 0x00144D2C File Offset: 0x00142F2C
	private static void AppendFeatureDetail(StringBuilder stringBuilder, CharacterFeatureItem featureCfg)
	{
		sbyte incElementType = -1;
		sbyte decElementType = -1;
		sbyte i = 0;
		while ((int)i < featureCfg.FiveElementPowerBonuses.Length)
		{
			sbyte bonus = featureCfg.FiveElementPowerBonuses[(int)i];
			bool flag = bonus > 0;
			if (flag)
			{
				incElementType = i;
			}
			else
			{
				bool flag2 = bonus < 0;
				if (flag2)
				{
					decElementType = i;
				}
			}
			i += 1;
		}
		short maxHealthPercentBonus = featureCfg.MaxHealthPercentBonus;
		short num = maxHealthPercentBonus;
		if (num <= 0)
		{
			if (num < 0)
			{
				stringBuilder.AppendFormat(LocalStringManager.Get(LanguageKey.LK_LifeLink_Feature_Detail_2), CommonUtils.GetFiveElementsNameByType(decElementType));
			}
		}
		else if (incElementType < 0 || decElementType < 0)
		{
			if (incElementType >= 0 && decElementType < 0)
			{
				stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_LifeLink_Feature_Detail_0));
			}
		}
		else
		{
			stringBuilder.AppendFormat(LocalStringManager.Get(LanguageKey.LK_LifeLink_Feature_Detail_1), CommonUtils.GetFiveElementsNameByType(decElementType), CommonUtils.GetFiveElementsNameByType(incElementType));
		}
	}

	// Token: 0x06002A71 RID: 10865 RVA: 0x00144DF8 File Offset: 0x00142FF8
	private unsafe string GetFiveElementsTips(NeiliProportionOfFiveElements fiveElements, int[] sumElements = null)
	{
		StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
		int sum = (sumElements != null) ? sumElements.Sum() : 0;
		for (sbyte fiveElementType = 0; fiveElementType < 5; fiveElementType += 1)
		{
			FiveElementItem config = FiveElement.Instance[(int)fiveElementType];
			sbyte value = *fiveElements[(int)fiveElementType];
			stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Dot_Symbol));
			stringBuilder.AppendFormat("<SpName={0}>", string.Format("{0}{1}", "ui9_icon_mousetip_elements_", fiveElementType));
			stringBuilder.Append(config.Name);
			stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
			stringBuilder.Append(value);
			stringBuilder.Append('%');
			bool flag = value > 0 && sum > 0;
			if (flag)
			{
				int rate = (int)(value * 100) / sum;
				bool flag2 = rate > 0;
				if (flag2)
				{
					string rateStr = LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Fix, string.Format("{0}%", rate)).SetColor("brightblue");
					stringBuilder.Append(rateStr);
				}
			}
			stringBuilder.AppendLine();
		}
		string result = stringBuilder.ToString();
		EasyPool.Free<StringBuilder>(stringBuilder);
		return result;
	}

	// Token: 0x04001EB4 RID: 7860
	private StringBuilder _stringBuilder;
}
