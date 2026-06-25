using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200004F RID: 79
[ExecuteInEditMode]
public class ColorStyleRoot : MonoBehaviour
{
	// Token: 0x06000293 RID: 659 RVA: 0x0000FC28 File Offset: 0x0000DE28
	public void SetColor(Color color, HashSet<Graphic> skipSet = null)
	{
		bool flag = skipSet != null;
		if (flag)
		{
			this._skipSetFromOutside.Clear();
			this._skipSetFromOutside.UnionWith(skipSet);
		}
		else
		{
			this._skipSetFromOutside.Clear();
		}
		this.ApplyNow(color);
	}

	// Token: 0x06000294 RID: 660 RVA: 0x0000FC74 File Offset: 0x0000DE74
	private void ApplyNow(Color color)
	{
		this.BuildSkipSet();
		base.GetComponentsInChildren<Graphic>(true, this._cachedChildGraphics);
		foreach (Graphic graphic in this._cachedChildGraphics)
		{
			bool flag = graphic == null;
			if (!flag)
			{
				bool flag2 = this._skipSet.Contains(graphic);
				if (!flag2)
				{
					graphic.color = color;
				}
			}
		}
	}

	// Token: 0x06000295 RID: 661 RVA: 0x0000FD04 File Offset: 0x0000DF04
	private void BuildSkipSet()
	{
		if (this._skipSet == null)
		{
			this._skipSet = new HashSet<Graphic>();
		}
		this._skipSet.Clear();
		bool flag = this.skipList != null;
		if (flag)
		{
			foreach (Graphic graphic in this.skipList)
			{
				bool flag2 = graphic != null;
				if (flag2)
				{
					this._skipSet.Add(graphic);
				}
			}
		}
		bool flag3 = this.autoSkipList != null;
		if (flag3)
		{
			foreach (Transform trans in this.autoSkipList)
			{
				bool flag4 = trans == null;
				if (!flag4)
				{
					Graphic[] graphics = trans.GetComponentsInChildren<Graphic>(true);
					foreach (Graphic graphic2 in graphics)
					{
						bool flag5 = graphic2 != null;
						if (flag5)
						{
							this._skipSet.Add(graphic2);
						}
					}
				}
			}
		}
		foreach (Graphic graphic3 in this._skipSetFromOutside)
		{
			bool flag6 = graphic3 == null;
			if (!flag6)
			{
				this._skipSet.Add(graphic3);
			}
		}
	}

	// Token: 0x04000154 RID: 340
	[SerializeField]
	private List<Graphic> skipList = new List<Graphic>();

	// Token: 0x04000155 RID: 341
	private readonly HashSet<Graphic> _skipSetFromOutside = new HashSet<Graphic>();

	// Token: 0x04000156 RID: 342
	[Tooltip("选中节点下所有Graphic都会被跳过")]
	[SerializeField]
	private List<Transform> autoSkipList = new List<Transform>();

	// Token: 0x04000157 RID: 343
	private HashSet<Graphic> _skipSet;

	// Token: 0x04000158 RID: 344
	private readonly List<Graphic> _cachedChildGraphics = new List<Graphic>();
}
