using System;
using System.Collections.Generic;
using Game.Views.Main.Reading;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EDA RID: 3802
	public class ReadingStrategiesCell : MonoBehaviour, ICellContent<ReadingStrategiesCellData>, ICellContent
	{
		// Token: 0x0600AF2A RID: 44842 RVA: 0x004FCDAC File Offset: 0x004FAFAC
		public void SetData(ReadingStrategiesCellData data)
		{
			IReadOnlyList<string> strategyNames = data.StrategyNames;
			int count = (strategyNames == null) ? 0 : strategyNames.Count;
			this.EnsureItems(Mathf.Max(1, count));
			bool flag = count <= 0;
			if (flag)
			{
				this._strategyItems[0].gameObject.SetActive(true);
				this._strategyItems[0].SetStrategy(false);
				this._strategyItems[0].ClearTip();
				for (int i = 1; i < this._strategyItems.Count; i++)
				{
					this._strategyItems[i].ClearTip();
					this._strategyItems[i].gameObject.SetActive(false);
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					ReadingBriefStrategy item = this._strategyItems[j];
					item.gameObject.SetActive(true);
					item.SetStrategy(true);
					item.strategy.text = strategyNames[j];
					IReadOnlyList<ReadingStrategyTipData> tipData = data.StrategyTips;
					bool flag2 = tipData != null && j < tipData.Count;
					if (flag2)
					{
						item.SetTip(tipData[j].Name, tipData[j].Desc);
					}
					else
					{
						item.ClearTip();
					}
				}
				for (int k = count; k < this._strategyItems.Count; k++)
				{
					this._strategyItems[k].ClearTip();
					this._strategyItems[k].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600AF2B RID: 44843 RVA: 0x004FCF6C File Offset: 0x004FB16C
		private void EnsureItems(int count)
		{
			CommonUtils.PrepareEnoughChildren(this.strategyRoot, this.strategyTemplate.gameObject, count, null);
			this._strategyItems.Clear();
			for (int i = 0; i < this.strategyRoot.childCount; i++)
			{
				ReadingBriefStrategy strategyItem = this.strategyRoot.GetChild(i).GetComponent<ReadingBriefStrategy>();
				this._strategyItems.Add(strategyItem);
			}
		}

		// Token: 0x040087B0 RID: 34736
		[SerializeField]
		private Transform strategyRoot;

		// Token: 0x040087B1 RID: 34737
		[SerializeField]
		private ReadingBriefStrategy strategyTemplate;

		// Token: 0x040087B2 RID: 34738
		private readonly List<ReadingBriefStrategy> _strategyItems = new List<ReadingBriefStrategy>();
	}
}
