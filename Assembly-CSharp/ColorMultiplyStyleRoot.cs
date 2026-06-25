using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200004E RID: 78
[ExecuteInEditMode]
public class ColorMultiplyStyleRoot : MonoBehaviour
{
	// Token: 0x0600028D RID: 653 RVA: 0x0000F8D9 File Offset: 0x0000DAD9
	public void MultiplyColor(Vector4 multiply)
	{
		this.ApplyNow(multiply);
	}

	// Token: 0x0600028E RID: 654 RVA: 0x0000F8E4 File Offset: 0x0000DAE4
	public void RestoreAllToWhite()
	{
		this.CollectAllGraphics(true);
		foreach (Graphic graphic in this._cachedChildGraphics)
		{
			bool flag = graphic == null;
			if (!flag)
			{
				graphic.color = Color.white;
			}
		}
	}

	// Token: 0x0600028F RID: 655 RVA: 0x0000F958 File Offset: 0x0000DB58
	private void ApplyNow(Vector4 multiply)
	{
		this.CollectAllGraphics(true);
		foreach (Graphic graphic in this._cachedChildGraphics)
		{
			bool flag = graphic == null;
			if (!flag)
			{
				Color color = graphic.color;
				graphic.color = new Color(color.r * multiply.x, color.g * multiply.y, color.b * multiply.z, color.a * multiply.w);
			}
		}
	}

	// Token: 0x06000290 RID: 656 RVA: 0x0000FA08 File Offset: 0x0000DC08
	private void CollectAllGraphics(bool includeInactive)
	{
		this._cachedChildGraphics.Clear();
		this._cachedChildGraphicsSet.Clear();
		this._excludeTransformSet.Clear();
		foreach (Transform excludeNode in this.excludeNodes)
		{
			bool flag = excludeNode == null;
			if (!flag)
			{
				this.CollectAllChildTransforms(excludeNode, this._excludeTransformSet);
			}
		}
		base.GetComponentsInChildren<Graphic>(includeInactive, this._cachedChildGraphics);
		for (int i = this._cachedChildGraphics.Count - 1; i >= 0; i--)
		{
			Graphic graphic = this._cachedChildGraphics[i];
			bool flag2 = graphic == null || this._excludeTransformSet.Contains(graphic.transform);
			if (flag2)
			{
				this._cachedChildGraphics.RemoveAt(i);
			}
			else
			{
				this._cachedChildGraphicsSet.Add(graphic);
			}
		}
		Graphic selfGraphic = base.GetComponent<Graphic>();
		bool flag3 = selfGraphic != null && !this._excludeTransformSet.Contains(selfGraphic.transform) && this._cachedChildGraphicsSet.Add(selfGraphic);
		if (flag3)
		{
			this._cachedChildGraphics.Add(selfGraphic);
		}
		bool flag4 = this._cachedChildGraphicsSet.Count != this._cachedChildGraphics.Count;
		if (flag4)
		{
			this._cachedChildGraphics.Clear();
			this._cachedChildGraphics.AddRange(this._cachedChildGraphicsSet);
		}
	}

	// Token: 0x06000291 RID: 657 RVA: 0x0000FBA8 File Offset: 0x0000DDA8
	private void CollectAllChildTransforms(Transform root, HashSet<Transform> result)
	{
		bool flag = root == null;
		if (!flag)
		{
			result.Add(root);
			for (int i = 0; i < root.childCount; i++)
			{
				this.CollectAllChildTransforms(root.GetChild(i), result);
			}
		}
	}

	// Token: 0x04000150 RID: 336
	[SerializeField]
	private List<Transform> excludeNodes = new List<Transform>();

	// Token: 0x04000151 RID: 337
	private readonly List<Graphic> _cachedChildGraphics = new List<Graphic>();

	// Token: 0x04000152 RID: 338
	private readonly HashSet<Graphic> _cachedChildGraphicsSet = new HashSet<Graphic>();

	// Token: 0x04000153 RID: 339
	private readonly HashSet<Transform> _excludeTransformSet = new HashSet<Transform>();
}
