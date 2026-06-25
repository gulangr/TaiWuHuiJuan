using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.Organization.Display;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200027A RID: 634
public class MouseTipCaravanPath : MouseTipBase
{
	// Token: 0x17000484 RID: 1156
	// (get) Token: 0x06002934 RID: 10548 RVA: 0x001334B1 File Offset: 0x001316B1
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002935 RID: 10549 RVA: 0x001334B4 File Offset: 0x001316B4
	protected override void Init(ArgumentBox argsBox)
	{
		List<SettlementDisplayData> list;
		argsBox.Get<List<SettlementDisplayData>>("DataList", out list);
		Dictionary<sbyte, List<SettlementDisplayData>> stateDict = (from s in list
		group s by MapArea.Instance[s.AreaTemplateId].StateID).ToDictionary((IGrouping<sbyte, SettlementDisplayData> g) => g.Key, (IGrouping<sbyte, SettlementDisplayData> g) => (from s in g
		select s).ToList<SettlementDisplayData>());
		GameObject stateTemplate = base.transform.GetChild(0).gameObject;
		List<sbyte> stateKeyList = stateDict.Keys.ToList<sbyte>();
		for (int i = 0; i < stateKeyList.Count; i++)
		{
			sbyte stateKey = stateKeyList[i];
			GameObject stateItem = (i < base.transform.childCount) ? base.transform.GetChild(i).gameObject : Object.Instantiate<GameObject>(stateTemplate, base.transform);
			stateItem.gameObject.SetActive(true);
			MapStateItem stateConfig = MapState.Instance[stateKey];
			Refers stateRefers = stateItem.GetComponent<Refers>();
			stateRefers.CGet<TextMeshProUGUI>("Name").SetText(stateConfig.Name.SetColor("orange"), true);
			stateItem.name = stateConfig.Name;
			RectTransform settlementLayout = stateRefers.CGet<RectTransform>("SettlementLayout");
			List<SettlementDisplayData> settlementList = stateDict[stateKey];
			GameObject settlementTemplate = settlementLayout.GetChild(0).gameObject;
			for (int j = 0; j < settlementList.Count; j++)
			{
				SettlementDisplayData settlementDisplayData = settlementList[j];
				GameObject settlementItem = (j < settlementLayout.childCount) ? settlementLayout.GetChild(j).gameObject : Object.Instantiate<GameObject>(settlementTemplate, settlementLayout);
				settlementItem.SetActive(true);
				Refers settlementRefers = settlementItem.GetComponent<Refers>();
				string settlementName = SingletonObject.getInstance<WorldMapModel>().GetSettlementName(settlementDisplayData);
				settlementRefers.CGet<TextMeshProUGUI>("Name").SetText(settlementName, true);
				bool hideArrow = j == settlementList.Count - 1;
				settlementRefers.CGet<GameObject>("Arrow").SetActive(!hideArrow);
				settlementItem.name = settlementName;
			}
			for (int k = settlementList.Count; k < settlementLayout.childCount; k++)
			{
				settlementLayout.GetChild(k).gameObject.SetActive(false);
			}
		}
		for (int l = stateKeyList.Count; l < base.transform.childCount; l++)
		{
			base.transform.GetChild(l).gameObject.SetActive(false);
		}
		this.NeedWaitData = true;
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.RectTransform);
			this.Element.ShowAfterRefresh();
		});
	}
}
