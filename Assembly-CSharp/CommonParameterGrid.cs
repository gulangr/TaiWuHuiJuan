using System;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200032D RID: 813
[RequireComponent(typeof(GridLayoutGroup))]
public class CommonParameterGrid : MonoBehaviour
{
	// Token: 0x06002F39 RID: 12089 RVA: 0x00172BBD File Offset: 0x00170DBD
	[Button("清理并生成物体")]
	private void ReCreate()
	{
		this.Clear();
		this.Create();
	}

	// Token: 0x06002F3A RID: 12090 RVA: 0x00172BD0 File Offset: 0x00170DD0
	[Button("清理物体")]
	private void Clear()
	{
		List<GameObject> childList = new List<GameObject>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			childList.Add(base.transform.GetChild(i).gameObject);
		}
		foreach (GameObject obj in childList)
		{
			Object x = obj;
			GameObject gameObject = this.template;
			bool flag = x != ((gameObject != null) ? gameObject.gameObject : null);
			if (flag)
			{
				Object.DestroyImmediate(obj);
			}
		}
	}

	// Token: 0x06002F3B RID: 12091 RVA: 0x00172C7C File Offset: 0x00170E7C
	private void Create()
	{
		bool flag = this.template == null;
		if (flag)
		{
			throw new Exception("template is null");
		}
		bool flag2 = this.gridLayoutGroup.constraint != GridLayoutGroup.Constraint.FixedColumnCount;
		if (flag2)
		{
			throw new Exception("only support Constraint.FixedColumnCount");
		}
		bool flag3 = this.useTemplateSize;
		if (flag3)
		{
			RectTransform rectTrans = this.template.GetComponent<RectTransform>();
			Vector2 size = rectTrans.rect.size;
			this.gridLayoutGroup.cellSize = size;
		}
		for (int i = 0; i < this.count - 1; i++)
		{
			this.GetCell();
		}
		this.RefreshBack();
	}

	// Token: 0x06002F3C RID: 12092 RVA: 0x00172D24 File Offset: 0x00170F24
	private int GetActualColumnCount()
	{
		bool flag = base.transform.childCount == 0;
		int result;
		if (flag)
		{
			result = this.gridLayoutGroup.constraintCount;
		}
		else
		{
			bool flag2 = base.transform.childCount == 1 || this.gridLayoutGroup.constraintCount == 1;
			if (flag2)
			{
				result = 1;
			}
			else
			{
				Dictionary<float, List<int>> rowYPositions = new Dictionary<float, List<int>>();
				Dictionary<float, List<int>> columnXPositions = new Dictionary<float, List<int>>();
				for (int i = 0; i < base.transform.childCount; i++)
				{
					RectTransform rt = base.transform.GetChild(i) as RectTransform;
					bool flag3 = rt != null;
					if (flag3)
					{
						float roundedY = Mathf.Round(rt.localPosition.y * 10f) / 10f;
						float roundedX = Mathf.Round(rt.localPosition.x * 10f) / 10f;
						bool flag4 = !rowYPositions.ContainsKey(roundedY);
						if (flag4)
						{
							rowYPositions[roundedY] = new List<int>();
						}
						rowYPositions[roundedY].Add(i);
						bool flag5 = !columnXPositions.ContainsKey(roundedX);
						if (flag5)
						{
							columnXPositions[roundedX] = new List<int>();
						}
						columnXPositions[roundedX].Add(i);
					}
				}
				int maxItemsPerRow = 0;
				foreach (List<int> row in rowYPositions.Values)
				{
					maxItemsPerRow = Mathf.Max(maxItemsPerRow, row.Count);
				}
				int distinctColumns = columnXPositions.Count;
				int actualColumns = Mathf.Min(maxItemsPerRow, distinctColumns);
				bool flag6 = actualColumns > this.gridLayoutGroup.constraintCount && this.gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount;
				if (flag6)
				{
					actualColumns = this.gridLayoutGroup.constraintCount;
				}
				result = Mathf.Max(1, actualColumns);
			}
		}
		return result;
	}

	// Token: 0x06002F3D RID: 12093 RVA: 0x00172F20 File Offset: 0x00171120
	[Button("仅更新背景")]
	public void RefreshBack()
	{
		int actualColumnCount = this.GetActualColumnCount();
		Dictionary<float, int> firstIndexInRow = new Dictionary<float, int>();
		Dictionary<int, Vector2Int> itemPositions = new Dictionary<int, Vector2Int>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			RectTransform rt = base.transform.GetChild(i) as RectTransform;
			bool flag = rt != null;
			if (flag)
			{
				float roundedY = Mathf.Round(rt.localPosition.y * 10f) / 10f;
				float roundedX = Mathf.Round(rt.localPosition.x * 10f) / 10f;
				bool flag2 = !firstIndexInRow.ContainsKey(roundedY);
				int rowIndex;
				if (flag2)
				{
					rowIndex = firstIndexInRow.Count;
					firstIndexInRow[roundedY] = rowIndex;
				}
				else
				{
					rowIndex = firstIndexInRow[roundedY];
				}
				int colIndex = 0;
				foreach (object entry in base.transform)
				{
					RectTransform other = entry as RectTransform;
					bool flag3 = other != null && Mathf.Approximately(Mathf.Round(other.localPosition.y * 10f) / 10f, roundedY) && Mathf.Round(other.localPosition.x * 10f) / 10f < roundedX;
					if (flag3)
					{
						colIndex++;
					}
				}
				itemPositions[i] = new Vector2Int(rowIndex, colIndex);
			}
		}
		for (int j = 0; j < base.transform.childCount; j++)
		{
			Transform obj = base.transform.GetChild(j);
			bool flag4 = j > 0 && this.autoNameByTemplate;
			if (flag4)
			{
				obj.name = string.Format("{0}_{1}", this.template.name, j);
			}
			CImage backImage = obj.gameObject.GetOrAddComponent<CImage>();
			backImage.raycastTarget = false;
			backImage.type = Image.Type.Sliced;
			bool flag5 = !itemPositions.ContainsKey(j);
			bool isEven;
			if (flag5)
			{
				isEven = (j % 2 == 0);
			}
			else
			{
				Vector2Int pos = itemPositions[j];
				CommonParameterGrid.GridBackgroundMode gridBackgroundMode = this.backgroundMode;
				CommonParameterGrid.GridBackgroundMode gridBackgroundMode2 = gridBackgroundMode;
				if (gridBackgroundMode2 != CommonParameterGrid.GridBackgroundMode.Checkerboard)
				{
					if (gridBackgroundMode2 != CommonParameterGrid.GridBackgroundMode.AlternatingRows)
					{
						isEven = (j % 2 == 0);
					}
					else
					{
						isEven = (pos.x % 2 == 0);
					}
				}
				else
				{
					isEven = ((pos.x + pos.y) % 2 == 0);
				}
			}
			Sprite backImageSprite = isEven ? this.back1 : this.back2;
			backImage.sprite = backImageSprite;
		}
	}

	// Token: 0x06002F3E RID: 12094 RVA: 0x001731E8 File Offset: 0x001713E8
	private GameObject GetCell()
	{
		GameObject go = null;
		bool flag = null == go;
		if (flag)
		{
			go = Object.Instantiate<GameObject>(this.template, base.transform);
			go.gameObject.SetActive(true);
		}
		return go;
	}

	// Token: 0x04002251 RID: 8785
	[SerializeField]
	private GridLayoutGroup gridLayoutGroup;

	// Token: 0x04002252 RID: 8786
	[SerializeField]
	[Range(1f, 100f)]
	private int count;

	// Token: 0x04002253 RID: 8787
	[SerializeField]
	private GameObject template;

	// Token: 0x04002254 RID: 8788
	[SerializeField]
	private bool useTemplateSize;

	// Token: 0x04002255 RID: 8789
	[SerializeField]
	private Sprite back1;

	// Token: 0x04002256 RID: 8790
	[SerializeField]
	private Sprite back2;

	// Token: 0x04002257 RID: 8791
	[Header("背景模式设置")]
	[SerializeField]
	private CommonParameterGrid.GridBackgroundMode backgroundMode = CommonParameterGrid.GridBackgroundMode.Checkerboard;

	// Token: 0x04002258 RID: 8792
	[Header("是否自动根据模板命名物体")]
	[SerializeField]
	private bool autoNameByTemplate = true;

	// Token: 0x020016AA RID: 5802
	public enum GridBackgroundMode
	{
		// Token: 0x0400A8A2 RID: 43170
		Checkerboard,
		// Token: 0x0400A8A3 RID: 43171
		AlternatingRows
	}
}
