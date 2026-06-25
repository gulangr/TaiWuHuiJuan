using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GameData.Utilities;

namespace UnityEngine.UI
{
	// Token: 0x02000FAF RID: 4015
	public class VariableGridLayoutGroupDefaultWidth : VariableGridLayoutGroup
	{
		// Token: 0x170014ED RID: 5357
		// (get) Token: 0x0600B8C0 RID: 47296 RVA: 0x005435BC File Offset: 0x005417BC
		public override float preferredWidth
		{
			get
			{
				return ((this.columnWidths == null) ? this._defaultMaxWidth : (this.columnWidths.Sum() + (float)this.columnWidths.Length * base.spacing.x)) + (float)base.padding.left + (float)base.padding.right;
			}
		}

		// Token: 0x0600B8C1 RID: 47297 RVA: 0x00543614 File Offset: 0x00541814
		protected override void PostProcessColumnWidths()
		{
			base.PostProcessColumnWidths();
			float maxWidth = ((base.rectTransform.parent as RectTransform) ?? base.rectTransform).sizeDelta.x;
			this.ReduceArrayPreSorted(this.columnWidths, maxWidth, this._charWidth);
		}

		// Token: 0x0600B8C2 RID: 47298 RVA: 0x00543662 File Offset: 0x00541862
		public void AddMergeOptions([TupleElementNames(new string[]
		{
			"extraX",
			"extraY"
		})] IEnumerable<ValueTuple<int, int>> mergeOptions)
		{
			this._cellSizes = mergeOptions.ToArray<ValueTuple<int, int>>();
		}

		// Token: 0x0600B8C3 RID: 47299 RVA: 0x00543674 File Offset: 0x00541874
		public void ReduceArrayPreSorted(float[] arr, float targetWidth, float charWidth)
		{
			bool flag = targetWidth < 1f;
			if (flag)
			{
				targetWidth = this._defaultMaxWidth;
			}
			Dictionary<int, int> lineCount = new Dictionary<int, int>();
			float[] originalCopyArr = new float[arr.Length];
			for (int i = 0; i < originalCopyArr.Length; i++)
			{
				originalCopyArr[i] = arr[i];
			}
			float sum = 0f;
			for (int j = 0; j < arr.Length; j++)
			{
				sum += arr[j];
				lineCount[j] = 1;
			}
			sum += this.GetSpacing(arr.Length);
			bool flag2 = sum <= targetWidth;
			if (!flag2)
			{
				List<ValueTuple<float, int>> sorted = new List<ValueTuple<float, int>>();
				for (int k = 0; k < arr.Length; k++)
				{
					sorted.Add(new ValueTuple<float, int>(arr[k], k));
				}
				sorted.Sort(([TupleElementNames(new string[]
				{
					"width",
					"index"
				})] ValueTuple<float, int> a, [TupleElementNames(new string[]
				{
					"width",
					"index"
				})] ValueTuple<float, int> b) => b.CompareTo(a));
				int currentIndex = 0;
				bool protextFlag = false;
				while (sum > targetWidth)
				{
					float colWidthNew = sorted[currentIndex].Item1 / (float)(lineCount[sorted[currentIndex].Item2] + 1) + charWidth;
					bool flag3 = colWidthNew > charWidth;
					if (flag3)
					{
						Dictionary<int, int> dictionary = lineCount;
						int item = sorted[currentIndex].Item2;
						int num = dictionary[item];
						dictionary[item] = num + 1;
						protextFlag = true;
						sum = sum + colWidthNew - arr[sorted[currentIndex].Item2];
						arr[sorted[currentIndex].Item2] = colWidthNew;
					}
					currentIndex++;
					bool flag4 = currentIndex >= sorted.Count;
					if (flag4)
					{
						currentIndex = 0;
						bool flag5 = !protextFlag;
						if (flag5)
						{
							break;
						}
						protextFlag = false;
					}
				}
			}
		}

		// Token: 0x0600B8C4 RID: 47300 RVA: 0x00543840 File Offset: 0x00541A40
		public void ReduceArrayMaxHeap(float[] arr, float targetWidth, float charWidth)
		{
			bool flag = targetWidth < 1f;
			if (flag)
			{
				targetWidth = this._defaultMaxWidth;
			}
			float sum = 0f;
			float[] originalCopyArr = new float[arr.Length];
			for (int i = 0; i < originalCopyArr.Length; i++)
			{
				originalCopyArr[i] = arr[i];
			}
			Dictionary<int, int> lineCount = new Dictionary<int, int>();
			for (int j = 0; j < arr.Length; j++)
			{
				sum += arr[j];
				lineCount[j] = 1;
			}
			sum += this.GetSpacing(arr.Length);
			bool flag2 = sum <= targetWidth;
			if (!flag2)
			{
				MaxHeap<ValueTuple<float, int>> heap = new MaxHeap<ValueTuple<float, int>>(arr.Length);
				for (int k = 0; k < arr.Length; k++)
				{
					heap.Push(new ValueTuple<float, int>(arr[k], k));
				}
				while (sum > targetWidth)
				{
					ValueTuple<float, int> top = heap.Pop();
					float colWidthNew = originalCopyArr[top.Item2] / (float)(lineCount[top.Item2] + 1) + charWidth;
					sum = sum - arr[top.Item2] + colWidthNew;
					Dictionary<int, int> dictionary = lineCount;
					int item = top.Item2;
					int num = dictionary[item];
					dictionary[item] = num + 1;
					arr[top.Item2] = colWidthNew;
					heap.Push(new ValueTuple<float, int>(colWidthNew, top.Item2));
				}
			}
		}

		// Token: 0x0600B8C5 RID: 47301 RVA: 0x005439A0 File Offset: 0x00541BA0
		private float GetSpacing(int length)
		{
			return base.spacing.x * (float)(length - 1) + (float)base.padding.left + (float)base.padding.right;
		}

		// Token: 0x0600B8C6 RID: 47302 RVA: 0x005439DC File Offset: 0x00541BDC
		protected override float GetCellPreferredWidth(int index)
		{
			ValueTuple<int, int>[] cellSizes = this._cellSizes;
			return (cellSizes != null && cellSizes.Length > index && this._cellSizes[index].Item1 != 0) ? 0f : (base.GetCellPreferredWidth(index) + this.cellExtraWidth);
		}

		// Token: 0x0600B8C7 RID: 47303 RVA: 0x00543A1A File Offset: 0x00541C1A
		protected override float GetCellPreferredHeight(int index)
		{
			ValueTuple<int, int>[] cellSizes = this._cellSizes;
			return (cellSizes != null && cellSizes.Length > index && this._cellSizes[index].Item2 != 0) ? 0f : (base.GetCellPreferredHeight(index) + this.cellExtraHeight);
		}

		// Token: 0x0600B8C8 RID: 47304 RVA: 0x00543A58 File Offset: 0x00541C58
		public override float GetCellColumnWidth(int column, int index)
		{
			bool flag = !this._cellSizes.CheckIndex(index);
			float result;
			if (flag)
			{
				result = base.GetColumnWidth(column);
			}
			else
			{
				bool flag2 = column < 0 || column >= base.columns || this._cellSizes[index].Item1 < 0 || this._cellSizes[index].Item2 < 0;
				if (flag2)
				{
					result = 0f;
				}
				else
				{
					result = RuntimeHelpers.GetSubArray<float>(this.columnWidths, new Range(column, Math.Min(column + this._cellSizes[index].Item1 + 1, this.columnWidths.Length))).Sum() + (float)(this._cellSizes[index].Item1 - 1) * base.spacing.x;
				}
			}
			return result;
		}

		// Token: 0x0600B8C9 RID: 47305 RVA: 0x00543B34 File Offset: 0x00541D34
		public override float GetCellRowHeight(int row, int index)
		{
			bool flag = !this._cellSizes.CheckIndex(index);
			float result;
			if (flag)
			{
				result = base.GetRowHeight(row);
			}
			else
			{
				bool flag2 = row < 0 || row >= base.rows || this._cellSizes[index].Item1 < 0 || this._cellSizes[index].Item2 < 0;
				if (flag2)
				{
					result = 0f;
				}
				else
				{
					result = RuntimeHelpers.GetSubArray<float>(this.rowHeights, new Range(row, Math.Min(row + this._cellSizes[index].Item2 + 1, this.rowHeights.Length))).Sum() + (float)(this._cellSizes[index].Item2 - 1) * base.spacing.y;
				}
			}
			return result;
		}

		// Token: 0x04008F3B RID: 36667
		public float _defaultMaxWidth = 1880f;

		// Token: 0x04008F3C RID: 36668
		public float _charWidth = 28.1f;

		// Token: 0x04008F3D RID: 36669
		[SerializeField]
		private float cellExtraWidth = 5f;

		// Token: 0x04008F3E RID: 36670
		[SerializeField]
		private float cellExtraHeight = 0f;

		// Token: 0x04008F3F RID: 36671
		[TupleElementNames(new string[]
		{
			"xlen",
			"ylen"
		})]
		private ValueTuple<int, int>[] _cellSizes = Array.Empty<ValueTuple<int, int>>();
	}
}
