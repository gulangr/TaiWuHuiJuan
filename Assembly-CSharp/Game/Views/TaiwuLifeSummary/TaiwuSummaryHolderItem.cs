using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace Game.Views.TaiwuLifeSummary
{
	// Token: 0x0200075C RID: 1884
	public class TaiwuSummaryHolderItem : MonoBehaviour
	{
		// Token: 0x17000AC3 RID: 2755
		// (get) Token: 0x06005B2F RID: 23343 RVA: 0x002A5145 File Offset: 0x002A3345
		public ETaiwuSummaryHolderItemType ItemType
		{
			get
			{
				return this.itemType;
			}
		}

		// Token: 0x06005B30 RID: 23344 RVA: 0x002A5150 File Offset: 0x002A3350
		public void Set(IReadOnlyList<TaiwuLifeSummaryTypeItem> summaryTypeItems, Dictionary<int, int> taiwuLifeSummaryDict, string backImageName = "")
		{
			List<ValueTuple<string, string>> dataList = (from t in summaryTypeItems.Select(delegate(TaiwuLifeSummaryTypeItem summaryTypeItem)
			{
				int value = taiwuLifeSummaryDict.GetValueOrDefault(summaryTypeItem.TemplateId);
				return new ValueTuple<TaiwuLifeSummaryTypeItem, int>(summaryTypeItem, value);
			})
			where !t.Item1.IsDate || t.Item2 > 0
			select new ValueTuple<string, string>(t.Item1.Name, TaiwuSummaryHolderItem.FormatContent(t.Item1, t.Item2))).ToPoolList<ValueTuple<string, string>>();
			this.SetRecord(dataList);
			EasyPool.Free<List<ValueTuple<string, string>>>(dataList);
		}

		// Token: 0x06005B31 RID: 23345 RVA: 0x002A51DC File Offset: 0x002A33DC
		public void Set(IReadOnlyList<TaiwuLifeSummaryTypeItem> summaryTypeItems, TaiwuLifeSummary taiwuLifeSummary, string backImageName = "")
		{
			List<ValueTuple<string, string>> dataList = (from t in summaryTypeItems.Select(delegate(TaiwuLifeSummaryTypeItem summaryTypeItem)
			{
				int value = taiwuLifeSummary.Get(summaryTypeItem.TemplateId);
				return new ValueTuple<TaiwuLifeSummaryTypeItem, int>(summaryTypeItem, value);
			})
			where !t.Item1.IsDate || t.Item2 > 0
			select new ValueTuple<string, string>(t.Item1.Name, TaiwuSummaryHolderItem.FormatContent(t.Item1, t.Item2))).ToPoolList<ValueTuple<string, string>>();
			this.SetRecord(dataList);
			EasyPool.Free<List<ValueTuple<string, string>>>(dataList);
		}

		// Token: 0x06005B32 RID: 23346 RVA: 0x002A5268 File Offset: 0x002A3468
		private static string FormatContent(TaiwuLifeSummaryTypeItem summaryTypeItem, int value)
		{
			bool isDate = summaryTypeItem.IsDate;
			string result;
			if (isDate)
			{
				result = ((value > 0) ? SingletonObject.getInstance<TimeManager>().GetDateDisplayContent(value) : string.Empty);
			}
			else
			{
				bool isTime = summaryTypeItem.IsTime;
				if (isTime)
				{
					result = Mathf.RoundToInt((float)value * 1f / 60f).ToString();
				}
				else
				{
					result = value.ToString();
				}
			}
			return result;
		}

		// Token: 0x06005B33 RID: 23347 RVA: 0x002A52CC File Offset: 0x002A34CC
		private void SetRecord([TupleElementNames(new string[]
		{
			"title",
			"value"
		})] List<ValueTuple<string, string>> dataList)
		{
			bool flag = this.isSingleColumn;
			if (flag)
			{
				CommonUtils.PrepareEnoughChildren(this.root, this.root.GetChild(0).gameObject, dataList.Count, null);
				for (int i = 0; i < dataList.Count; i++)
				{
					Transform child = this.root.GetChild(i);
					TextMeshProUGUI[] labels = child.GetComponentsInChildren<TextMeshProUGUI>();
					TaiwuSummaryHolderItem.SetSummaryLabels(labels, dataList[i]);
				}
			}
			else
			{
				bool flag2 = !this.extraRoot;
				if (flag2)
				{
					Debug.LogWarning("extraRoot is null");
				}
				else
				{
					int lineCount = dataList.Count / 2;
					CommonUtils.PrepareEnoughChildren(this.root, this.root.GetChild(0).gameObject, lineCount + dataList.Count % 2, null);
					CommonUtils.PrepareEnoughChildren(this.extraRoot, this.extraRoot.GetChild(0).gameObject, lineCount, null);
					for (int j = 0; j < dataList.Count; j++)
					{
						TextMeshProUGUI[] labels2 = ((j % 2 == 0) ? this.root.GetChild(j / 2) : this.extraRoot.GetChild(j / 2)).GetComponentsInChildren<TextMeshProUGUI>();
						TaiwuSummaryHolderItem.SetSummaryLabels(labels2, dataList[j]);
					}
				}
			}
		}

		// Token: 0x06005B34 RID: 23348 RVA: 0x002A5438 File Offset: 0x002A3638
		private static void SetSummaryLabels(TextMeshProUGUI[] labels, [TupleElementNames(new string[]
		{
			"title",
			"value"
		})] ValueTuple<string, string> summaryData)
		{
			labels[0].text = summaryData.Item1;
			labels[1].text = summaryData.Item2.ToString();
		}

		// Token: 0x04003EE8 RID: 16104
		[SerializeField]
		private CImage summaryHolderBack;

		// Token: 0x04003EE9 RID: 16105
		[SerializeField]
		private RectTransform root;

		// Token: 0x04003EEA RID: 16106
		[SerializeField]
		private RectTransform extraRoot;

		// Token: 0x04003EEB RID: 16107
		[SerializeField]
		private bool isSingleColumn;

		// Token: 0x04003EEC RID: 16108
		[SerializeField]
		private ETaiwuSummaryHolderItemType itemType;
	}
}
