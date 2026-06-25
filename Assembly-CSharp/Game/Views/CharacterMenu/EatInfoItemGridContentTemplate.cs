using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B70 RID: 2928
	public class EatInfoItemGridContentTemplate : MonoBehaviour
	{
		// Token: 0x060090C5 RID: 37061 RVA: 0x004378F8 File Offset: 0x00435AF8
		public void SetupGen(string title, List<ValueTuple<string, string, int>> dataList)
		{
			this.RefreshLayout();
			CommonUtils.PrepareEnoughChildren(this.gridLayout.transform, this.cellTemplate.gameObject, dataList.Count, null);
			for (int i = 0; i < dataList.Count; i++)
			{
				this.gridLayout.transform.GetChild(i).GetComponent<EatInfoGridCellTemplate>().Setup(dataList[i].Item1, dataList[i].Item2, dataList[i].Item3, false);
			}
		}

		// Token: 0x060090C6 RID: 37062 RVA: 0x00437990 File Offset: 0x00435B90
		public void Setup(string title, List<ValueTuple<string, string, int>> dataList)
		{
			this.RefreshLayout();
			this.titleTxt.text = title;
			CommonUtils.PrepareEnoughChildren(this.gridLayout.transform, this.cellTemplate.gameObject, dataList.Count, null);
			for (int i = 0; i < dataList.Count; i++)
			{
				this.gridLayout.transform.GetChild(i).GetComponent<EatInfoGridCellTemplate>().Setup(dataList[i].Item1, dataList[i].Item2, dataList[i].Item3, false);
			}
		}

		// Token: 0x060090C7 RID: 37063 RVA: 0x00437A38 File Offset: 0x00435C38
		private void RefreshLayout()
		{
			bool isCN = LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN;
			this.gridLayout.cellSize = (isCN ? this.cellSizeCN : this.cellSizeEN);
			this.gridLayout.constraintCount = (isCN ? this.colAmountCN : this.colAmountEN);
		}

		// Token: 0x04006F7A RID: 28538
		[SerializeField]
		private TextMeshProUGUI titleTxt;

		// Token: 0x04006F7B RID: 28539
		[SerializeField]
		private GridLayoutGroup gridLayout;

		// Token: 0x04006F7C RID: 28540
		[SerializeField]
		private EatInfoGridCellTemplate cellTemplate;

		// Token: 0x04006F7D RID: 28541
		[Header("布局")]
		[SerializeField]
		private Vector2 cellSizeCN = new Vector2(332f, 42f);

		// Token: 0x04006F7E RID: 28542
		[SerializeField]
		private int colAmountCN = 3;

		// Token: 0x04006F7F RID: 28543
		[SerializeField]
		private Vector2 cellSizeEN = new Vector2(500f, 42f);

		// Token: 0x04006F80 RID: 28544
		[SerializeField]
		private int colAmountEN = 2;
	}
}
