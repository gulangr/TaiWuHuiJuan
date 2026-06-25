using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A5 RID: 165
[ExecuteInEditMode]
public class UICircleChild : MonoBehaviour
{
	// Token: 0x060005BC RID: 1468 RVA: 0x000263BF File Offset: 0x000245BF
	private void Start()
	{
		this.ReCircle();
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x000263CC File Offset: 0x000245CC
	public void ReCircle()
	{
		List<Transform> toHandleList = new List<Transform>();
		int i = 0;
		int max = base.transform.childCount;
		while (i < max)
		{
			Transform child = base.transform.GetChild(i);
			bool activeSelf = child.gameObject.activeSelf;
			if (activeSelf)
			{
				toHandleList.Add(child);
			}
			i++;
		}
		int j = 0;
		int max2 = toHandleList.Count;
		while (j < max2)
		{
			Transform child2 = toHandleList[j];
			bool inverse = this.Inverse;
			if (inverse)
			{
				child2.localPosition = Quaternion.AngleAxis(this.StartVec - 360f / (float)max2 * (float)j, Vector3.back) * Vector3.up * this.Range;
			}
			else
			{
				child2.localPosition = Quaternion.AngleAxis(this.StartVec + 360f / (float)max2 * (float)j, Vector3.back) * Vector3.up * this.Range;
			}
			Action<Transform, int, int> onPlace = this.OnPlace;
			if (onPlace != null)
			{
				onPlace(child2, max2, j);
			}
			j++;
		}
	}

	// Token: 0x040004AD RID: 1197
	public Action<Transform, int, int> OnPlace;

	// Token: 0x040004AE RID: 1198
	public float StartVec;

	// Token: 0x040004AF RID: 1199
	public bool Inverse = false;

	// Token: 0x040004B0 RID: 1200
	public float Range;
}
