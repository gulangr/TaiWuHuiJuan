using System;
using System.Collections.Generic;
using GameData.Utilities;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ECE RID: 3790
	public class MultiplyButtonCell : MonoBehaviour, ICellContent<MultiplyButtonCellData>, ICellContent
	{
		// Token: 0x0600AF09 RID: 44809 RVA: 0x004FC06C File Offset: 0x004FA26C
		public void SetData(MultiplyButtonCellData data)
		{
			bool flag = this.template.transform.parent != this.layout;
			if (flag)
			{
				this.template.gameObject.SetActive(false);
			}
			List<SingleButtonCellData> singleButtonCellDataList = data.SingleButtonCellDataList;
			bool flag2 = singleButtonCellDataList == null || singleButtonCellDataList.Count <= 0;
			if (flag2)
			{
				AdaptableLog.Error("SingleButtonCellDataList is invalid");
			}
			else
			{
				for (int i = 0; i < data.SingleButtonCellDataList.Count; i++)
				{
					SingleButtonCell cell = (i < this.layout.childCount) ? this.layout.GetChild(i).GetComponent<SingleButtonCell>() : Object.Instantiate<SingleButtonCell>(this.template, this.layout);
					cell.SetData(data.SingleButtonCellDataList[i]);
					cell.gameObject.SetActive(true);
				}
				for (int j = data.SingleButtonCellDataList.Count; j < this.layout.childCount; j++)
				{
					Transform cell2 = this.layout.GetChild(j);
					cell2.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0400878E RID: 34702
		[SerializeField]
		private Transform layout;

		// Token: 0x0400878F RID: 34703
		[SerializeField]
		private SingleButtonCell template;
	}
}
