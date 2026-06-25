using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002BB RID: 699
public class MouseTipMakeWugKing : MouseTipBase
{
	// Token: 0x06002AC5 RID: 10949 RVA: 0x0014793C File Offset: 0x00145B3C
	protected override void Init(ArgumentBox argsBox)
	{
		base.GetComponent<CanvasGroup>().alpha = 0f;
		sbyte templateId;
		argsBox.Get("TemplateId", out templateId);
		List<int> costPoison;
		argsBox.Get<List<int>>("CostPoison", out costPoison);
		int costPoisonValue;
		argsBox.Get("CostPoisonValue", out costPoisonValue);
		int allPoisonValue;
		argsBox.Get("AllPoisonValue", out allPoisonValue);
		bool flag = templateId >= 0;
		string wugKingName;
		if (flag)
		{
			WugKingItem config = WugKing.Instance[templateId];
			MedicineItem medicineConfig = Medicine.Instance[config.WugMedicine];
			wugKingName = medicineConfig.Name;
		}
		else
		{
			wugKingName = LocalStringManager.Get(LanguageKey.LK_Unknow);
		}
		string content = wugKingName;
		base.CGet<TextMeshProUGUI>("Content").text = content;
		string color = (allPoisonValue >= costPoisonValue) ? "brightblue" : "brightred";
		base.CGet<TextMeshProUGUI>("Cost").text = string.Format("{0}/{1}", allPoisonValue.ToString().SetColor(color), costPoisonValue);
		RectTransform poisonHolder = base.CGet<RectTransform>("PoisonHolder");
		for (int i = 0; i < 6; i++)
		{
			int value = costPoison[i];
			Transform item = poisonHolder.GetChild(i);
			item.gameObject.SetActive(value > 0);
			Refers refers = item.GetComponent<Refers>();
			PoisonItem config2 = Poison.Instance[i];
			refers.CGet<CImage>("Icon").SetSprite(config2.Icon, false, null);
			refers.CGet<TextMeshProUGUI>("Name").text = config2.Name.SetColor(config2.FontColor);
			refers.CGet<TextMeshProUGUI>("Value").text = value.ToString().SetColor("pinkyellow");
		}
		RectTransform rectTransform = base.GetComponent<RectTransform>();
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		});
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(3U, delegate
		{
			this.GetComponent<CanvasGroup>().alpha = 1f;
		});
	}
}
