using System;
using System.Collections.Generic;
using System.Text;
using FrameWork;
using GameData.Domains.Organization.Display;
using TMPro;
using UnityEngine;

// Token: 0x0200027B RID: 635
public class MouseTipCaravanPathDetail : MouseTipBase
{
	// Token: 0x17000485 RID: 1157
	// (get) Token: 0x06002938 RID: 10552 RVA: 0x001337AA File Offset: 0x001319AA
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002939 RID: 10553 RVA: 0x001337B0 File Offset: 0x001319B0
	protected override void Init(ArgumentBox argsBox)
	{
		List<SettlementDisplayData> list;
		argsBox.Get<List<SettlementDisplayData>>("DataList", out list);
		string areaName;
		argsBox.Get("AreaName", out areaName);
		string stateName;
		argsBox.Get("StateName", out stateName);
		this._stringBuilder.Clear();
		this._stringBuilder.Append(stateName);
		this._stringBuilder.Append('-');
		this._stringBuilder.Append(areaName);
		this._stringBuilder.Append('-');
		base.CGet<TextMeshProUGUI>("AreaName").text = areaName;
		int index = 0;
		CommonUtils.PrepareEnoughChildren(this.transAreaLayout, this.settlementItemPrefab.gameObject, list.Count, null);
		foreach (SettlementDisplayData settlementData in list)
		{
			Refers settlementComp = this.transAreaLayout.GetChild(index).GetComponent<Refers>();
			this.SetupSettlementInfo(settlementComp, settlementData, index % 2 == 0);
			index++;
		}
	}

	// Token: 0x0600293A RID: 10554 RVA: 0x001338D0 File Offset: 0x00131AD0
	private void SetupSettlementInfo(Refers settlementComp, SettlementDisplayData settlementData, bool isEven)
	{
		string settlementName = SingletonObject.getInstance<WorldMapModel>().GetSettlementName(settlementData);
		this._stringBuilder.Append(settlementName);
		settlementComp.CGet<GameObject>("bg").SetActive(!isEven);
		settlementComp.CGet<TextMeshProUGUI>("SettlementName").text = this._stringBuilder.ToString();
		this._stringBuilder.Remove(this._stringBuilder.Length - settlementName.Length, settlementName.Length);
		settlementComp.CGet<TextMeshProUGUI>("txt_Cul").text = settlementData.Culture.ToString();
		settlementComp.CGet<TextMeshProUGUI>("txt_Safety").text = settlementData.Safety.ToString();
		bool flag = settlementData.Culture > 50;
		float percentageCul;
		if (flag)
		{
			percentageCul = (float)(settlementData.Culture * 5 - 250);
			settlementComp.CGet<TextMeshProUGUI>("label_influ_desc").text = LocalStringManager.Get("LK_MerchantInfo_IncomeBonus");
		}
		else
		{
			percentageCul = (float)(200 - settlementData.Culture * 4);
			settlementComp.CGet<TextMeshProUGUI>("label_influ_desc").text = LocalStringManager.Get("LK_MerchantInfo_CriticalInfo");
		}
		settlementComp.CGet<TextMeshProUGUI>("txt_CulPercentageNegative").gameObject.SetActive(percentageCul < 0f);
		settlementComp.CGet<TextMeshProUGUI>("txt_CulPercentagePositive").gameObject.SetActive(percentageCul >= 0f);
		percentageCul /= 10f;
		percentageCul = Mathf.Ceil(percentageCul);
		bool flag2 = percentageCul >= 0f;
		if (flag2)
		{
			settlementComp.CGet<TextMeshProUGUI>("txt_CulPercentagePositive").text = string.Format("+{0:f0}%", percentageCul);
		}
		else
		{
			settlementComp.CGet<TextMeshProUGUI>("txt_CulPercentageNegative").text = string.Format("{0:f0}%", percentageCul);
		}
		float percentageRob = (float)(200 - settlementData.Safety * 4);
		settlementComp.CGet<TextMeshProUGUI>("txt_safetyPercentageNegative").gameObject.SetActive(percentageRob < 0f);
		settlementComp.CGet<TextMeshProUGUI>("txt_safetyPercentagePositive").gameObject.SetActive(percentageRob >= 0f);
		percentageRob /= 10f;
		percentageRob = Mathf.Ceil(percentageRob);
		bool flag3 = percentageRob >= 0f;
		if (flag3)
		{
			settlementComp.CGet<TextMeshProUGUI>("txt_safetyPercentagePositive").text = string.Format("+{0:f0}%", percentageRob);
		}
		else
		{
			settlementComp.CGet<TextMeshProUGUI>("txt_safetyPercentageNegative").text = string.Format("{0:f0}%", percentageRob);
		}
		settlementComp.gameObject.SetActive(true);
	}

	// Token: 0x04001E00 RID: 7680
	private StringBuilder _stringBuilder = new StringBuilder();

	// Token: 0x04001E01 RID: 7681
	[SerializeField]
	private Transform transAreaLayout;

	// Token: 0x04001E02 RID: 7682
	[SerializeField]
	private Refers settlementItemPrefab;
}
