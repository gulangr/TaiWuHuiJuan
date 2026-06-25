using System;
using System.Collections.Generic;
using System.Text;
using FrameWork;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x0200029F RID: 671
public class MouseTipFulongFlame : MouseTipBase
{
	// Token: 0x17000498 RID: 1176
	// (get) Token: 0x06002A25 RID: 10789 RVA: 0x00141B03 File Offset: 0x0013FD03
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A26 RID: 10790 RVA: 0x00141B06 File Offset: 0x0013FD06
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x06002A27 RID: 10791 RVA: 0x00141B14 File Offset: 0x0013FD14
	public override void Refresh(ArgumentBox argBox)
	{
		base.Refresh();
		int require = GlobalConfig.Instance.FulongFlameExtinguishCost;
		int remaining = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
		bool enoughTime = remaining >= require;
		bool damaged = SingletonObject.getInstance<WorldMapModel>().IsTaiwuGroupGetMaxLevelInjuries;
		base.CGet<TextMeshProUGUI>("Text").text = string.Concat(new string[]
		{
			LocalStringManager.Get(LanguageKey.LK_Cost_Time).SetColor("grey"),
			": ",
			remaining.ToString().SetColor(enoughTime ? "brightblue" : "brightred"),
			"/",
			require.ToString().SetColor("pinkyellow")
		});
		base.CGet<GameObject>("LineType_11").SetActive(false);
		bool flag = !enoughTime || damaged;
		if (flag)
		{
			StringBuilder builder = new StringBuilder();
			bool flag2 = !enoughTime;
			if (flag2)
			{
				builder.AppendLine(LocalStringManager.Get(LanguageKey.LK_MouseTip_Extinguish_Invalid_ActionPoint).ColorReplace());
			}
			bool flag3 = damaged;
			if (flag3)
			{
				TaiwuDomainMethod.AsyncCall.GetSeverelyInjuredGroupCharNames(this, true, delegate(int offset, RawDataPool dataPool)
				{
					List<CharNameRelatedData> names = null;
					Serializer.Deserialize(dataPool, offset, ref names);
					List<NameRelatedData> nameList = EasyPool.Get<List<NameRelatedData>>();
					nameList.Clear();
					foreach (CharNameRelatedData charName in names)
					{
						nameList.Add(charName.NameData);
					}
					bool flag4 = names != null && names.Count > 0;
					if (flag4)
					{
						bool flag5 = names[0].NameData.CharTemplateId < 0;
						if (flag5)
						{
							nameList.RemoveAt(0);
							bool flag6 = nameList.Count > 0;
							if (flag6)
							{
								builder.AppendLine((NameCenter.GetNameSequenceStringByNameRelatedDataList(nameList, false).SetColor("pinkyellow") + LocalStringManager.Get(LanguageKey.LK_MouseTip_Extinguish_Invalid_Damage)).ColorReplace());
							}
						}
						else
						{
							builder.AppendLine((NameCenter.GetNameSequenceStringByNameRelatedDataList(nameList, true).SetColor("pinkyellow") + LocalStringManager.Get(LanguageKey.LK_MouseTip_Extinguish_Invalid_Damage)).ColorReplace());
						}
					}
					EasyPool.Free<List<NameRelatedData>>(nameList);
					this.CGet<TextMeshProUGUI>("NotValidText").text = builder.ToString();
				});
			}
			base.CGet<TextMeshProUGUI>("NotValidText").text = builder.ToString();
			base.CGet<GameObject>("LineType_11").SetActive(true);
		}
	}
}
