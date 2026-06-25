using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000151 RID: 337
public class CombatBeginWisdom : MonoBehaviour
{
	// Token: 0x17000215 RID: 533
	// (get) Token: 0x0600129F RID: 4767 RVA: 0x000713B6 File Offset: 0x0006F5B6
	private CombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<CombatModel>();
		}
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x000713BD File Offset: 0x0006F5BD
	public void DoRequest()
	{
		this.Model.RequestGetCharacterWisdomList(this.isAlly, new AsyncMethodCallbackDelegate(this.HandlerGetCharacterWisdomList));
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x000713E0 File Offset: 0x0006F5E0
	private void HandlerGetCharacterWisdomList(int offset, RawDataPool pool)
	{
		List<int> wisdomList = null;
		Serializer.Deserialize(pool, offset, ref wisdomList);
		int wisdomCount = 0;
		IReadOnlyList<int> team = this.isAlly ? this.Model.SelfTeam : this.Model.EnemyTeam;
		foreach (int value in wisdomList)
		{
			wisdomCount += value;
		}
		this.wisdomText.text = string.Format("{0}", Mathf.Abs(wisdomCount));
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		List<GeneralLineData> dataList = new List<GeneralLineData>
		{
			new GeneralLineData(7, new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_CombatBegin_WisdomTip_Description1)
			}, null)
		};
		for (int i = 0; i < wisdomList.Count; i++)
		{
			int charId = team[i];
			bool flag = charId < 0;
			if (!flag)
			{
				int value2 = wisdomList[i];
				dataList.Add(new GeneralLineData(9, new List<string>
				{
					"-" + NameCenter.GetMonasticTitleOrDisplayName(this.Model.DisplayDataCache[team[i]], charId == taiwuCharId).SetColor("orange") + "：",
					(value2 >= 0) ? "mousetip_characteristic_11" : "mousetip_characteristic_5",
					string.Format("x{0}", Mathf.Abs(value2))
				}, null));
			}
		}
		dataList.Insert(1, new GeneralLineData(8, new List<string>
		{
			LocalStringManager.Get(LanguageKey.LK_CombatBegin_WisdomTip_Description2),
			(wisdomCount >= 0) ? "mousetip_characteristic_11" : "mousetip_characteristic_5",
			string.Format("x{0}", Mathf.Abs(wisdomCount))
		}, null));
		TooltipInvoker tooltipInvoker = this.mouseTip;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		this.mouseTip.RuntimeParam.Clear();
		this.mouseTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_CombatBegin_WisdomTip_Title));
		this.mouseTip.RuntimeParam.Set("LineCount", dataList.Count);
		for (int j = 0; j < dataList.Count; j++)
		{
			this.mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", j + 1), dataList[j]);
		}
		this.mouseTip.Refresh(false, -1);
	}

	// Token: 0x04000FD5 RID: 4053
	[SerializeField]
	private bool isAlly;

	// Token: 0x04000FD6 RID: 4054
	[SerializeField]
	private TooltipInvoker mouseTip;

	// Token: 0x04000FD7 RID: 4055
	[SerializeField]
	private TextMeshProUGUI wisdomText;
}
