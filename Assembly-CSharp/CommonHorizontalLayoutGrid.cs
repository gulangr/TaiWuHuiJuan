using System;
using EasyButtons;
using UnityEngine;

// Token: 0x0200032A RID: 810
public class CommonHorizontalLayoutGrid : Refers
{
	// Token: 0x06002F16 RID: 12054 RVA: 0x00172110 File Offset: 0x00170310
	[Button("清理", LabelColor = "#FF0000")]
	private void ClearObject()
	{
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(base.transform.childCount - 1);
			bool flag = !child.gameObject.Equals(this.columnTemplate0) && !child.gameObject.Equals(this.columnTemplate1);
			if (flag)
			{
				Object.DestroyImmediate(child.gameObject);
			}
		}
	}

	// Token: 0x06002F17 RID: 12055 RVA: 0x00172194 File Offset: 0x00170394
	[Button("根据‘列宽数据’生成")]
	private void CreateObject()
	{
		Debug.Assert(this.columnTemplate0 != null);
		Debug.Assert(this.columnTemplate1 != null);
		int instantiateCount = this.columnWeightArray.Length - base.transform.childCount;
		for (int i = 0; i < instantiateCount; i++)
		{
			GameObject column = Object.Instantiate<GameObject>(this.columnTemplate1, base.transform);
		}
		for (int j = 0; j < this.columnWeightArray.Length; j++)
		{
			Transform child = base.transform.GetChild(j);
			child.name = string.Format("Column_{0}", j);
			RectTransform rectTransform = child.GetComponent<RectTransform>();
			rectTransform.SetWidth(this.columnWeightArray[j]);
		}
	}

	// Token: 0x06002F18 RID: 12056 RVA: 0x00172260 File Offset: 0x00170460
	[Button("根据‘列宽数据’设置宽度")]
	private void SetWeight()
	{
		Debug.Assert(this.columnTemplate0 != null);
		Debug.Assert(this.columnTemplate1 != null);
		for (int i = 0; i < this.columnWeightArray.Length; i++)
		{
			Transform child = base.transform.GetChild(i);
			child.name = string.Format("Column_{0}", i);
			RectTransform rectTransform = child.GetComponent<RectTransform>();
			rectTransform.SetWidth(this.columnWeightArray[i]);
		}
	}

	// Token: 0x06002F19 RID: 12057 RVA: 0x001722E8 File Offset: 0x001704E8
	[Button("根据‘特殊背景的列索引’设置特殊背景")]
	private void SetSpecialBg()
	{
		for (int i = 0; i < this.columnWeightArray.Length; i++)
		{
			Transform child = base.transform.GetChild(i);
			Transform spBg = child.Find("SpBg");
			bool flag = spBg != null;
			if (flag)
			{
				spBg.gameObject.SetActive(false);
			}
		}
		for (int j = 0; j < this.specialBgShowIndex.Length; j++)
		{
			Transform child2 = base.transform.GetChild(this.specialBgShowIndex[j]);
			bool flag2 = child2 != null;
			if (flag2)
			{
				Transform spBg2 = child2.Find("SpBg");
				bool flag3 = spBg2 != null;
				if (flag3)
				{
					spBg2.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x06002F1A RID: 12058 RVA: 0x001723B8 File Offset: 0x001705B8
	public void SetRowBg(int index)
	{
		CImage rowBg = base.GetComponent<CImage>();
		rowBg.SetSprite((index % 2 == 0) ? this.evenRowBgName : this.oddRowBgName, false, null);
	}

	// Token: 0x06002F1B RID: 12059 RVA: 0x001723EC File Offset: 0x001705EC
	public void SetSpecialBg(int index, bool active)
	{
		Transform child = base.transform.GetChild(index);
		Transform spBg = child.Find("SpBg");
		bool flag = spBg != null;
		if (flag)
		{
			CImage specialBg = spBg.GetComponent<CImage>();
			specialBg.SetSprite(this.specialBgName, false, null);
			spBg.gameObject.SetActive(active);
		}
	}

	// Token: 0x04002235 RID: 8757
	[SerializeField]
	private GameObject columnTemplate0;

	// Token: 0x04002236 RID: 8758
	[SerializeField]
	private GameObject columnTemplate1;

	// Token: 0x04002237 RID: 8759
	[Header("偶数行背景图名称")]
	[SerializeField]
	private string evenRowBgName;

	// Token: 0x04002238 RID: 8760
	[Header("奇数行背景图名称")]
	[SerializeField]
	private string oddRowBgName;

	// Token: 0x04002239 RID: 8761
	[Header("特殊列格背景图名称")]
	[SerializeField]
	private string specialBgName;

	// Token: 0x0400223A RID: 8762
	[Header("列宽数组")]
	[SerializeField]
	private float[] columnWeightArray;

	// Token: 0x0400223B RID: 8763
	[Header("显示特殊背景的列索引")]
	[SerializeField]
	private int[] specialBgShowIndex;
}
