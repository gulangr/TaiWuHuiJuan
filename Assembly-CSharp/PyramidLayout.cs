using System;
using EasyButtons;
using UnityEngine;

// Token: 0x020003D8 RID: 984
public class PyramidLayout : MonoBehaviour
{
	// Token: 0x06003B2B RID: 15147 RVA: 0x001DF564 File Offset: 0x001DD764
	[Button]
	public void UpdateChild()
	{
		int count = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			bool activeSelf = base.transform.GetChild(i).gameObject.activeSelf;
			if (activeSelf)
			{
				count++;
			}
		}
		this.UpdateChild(count);
	}

	// Token: 0x06003B2C RID: 15148 RVA: 0x001DF5B4 File Offset: 0x001DD7B4
	private void UpdateChild(int count)
	{
		int index = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform t = base.transform.GetChild(i);
			bool activeSelf = t.gameObject.activeSelf;
			if (activeSelf)
			{
				this.UpdateChild(t, index, count);
				index++;
			}
		}
	}

	// Token: 0x06003B2D RID: 15149 RVA: 0x001DF610 File Offset: 0x001DD810
	private void UpdateChild(Transform t, int index, int count)
	{
		bool flag = count < 2 || (count == 3 && index == 2);
		float x;
		if (flag)
		{
			x = 0f;
		}
		else
		{
			x = ((index % 2 == 0) ? this.alignSize : (-this.alignSize));
		}
		bool flag2 = count < 3;
		float y;
		if (flag2)
		{
			y = 0f;
		}
		else
		{
			y = ((index < 2) ? (-this.alignSize) : this.alignSize);
		}
		bool flag3 = count % 2 == 0 && this.reverseAxisInEven;
		if (flag3)
		{
			float num = y;
			float num2 = x;
			x = num;
			y = num2;
		}
		bool flag4 = !Mathf.Approximately(x, 0f);
		if (flag4)
		{
			x = ((x < 0f) ? (x - this.spacing.x / 2f) : (x + this.spacing.x / 2f));
		}
		bool flag5 = !Mathf.Approximately(y, 0f);
		if (flag5)
		{
			y = ((y < 0f) ? (y - this.spacing.y / 2f) : (y + this.spacing.y / 2f));
		}
		t.localPosition = new Vector2(x, y);
	}

	// Token: 0x04002A8B RID: 10891
	public float alignSize = 30f;

	// Token: 0x04002A8C RID: 10892
	public Vector2 spacing = Vector2.zero;

	// Token: 0x04002A8D RID: 10893
	public bool reverseAxisInEven = false;
}
