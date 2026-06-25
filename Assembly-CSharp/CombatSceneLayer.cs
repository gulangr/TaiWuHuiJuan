using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x0200015C RID: 348
[ExecuteAlways]
public class CombatSceneLayer : MonoBehaviour
{
	// Token: 0x06001329 RID: 4905 RVA: 0x00076186 File Offset: 0x00074386
	public void Awake()
	{
		this._rectTransform = base.GetComponent<RectTransform>();
		this._children = new List<CombatSceneObject>(base.GetComponentsInChildren<CombatSceneObject>());
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x000761A6 File Offset: 0x000743A6
	public void InitScroll()
	{
	}

	// Token: 0x0600132B RID: 4907 RVA: 0x000761AC File Offset: 0x000743AC
	public void ScaleTo(float lerpT, float duration)
	{
		this._rectTransform.DOKill(false);
		bool flag = duration > 0f;
		if (flag)
		{
			this._rectTransform.DOScale(Mathf.Lerp(this.ScaleRange.x, this.ScaleRange.y, lerpT) * Vector3.one, 0.3f).SetEase(Ease.Linear);
		}
		else
		{
			this._rectTransform.localScale = Mathf.Lerp(this.ScaleRange.x, this.ScaleRange.y, lerpT) * Vector3.one;
		}
	}

	// Token: 0x0600132C RID: 4908 RVA: 0x00076248 File Offset: 0x00074448
	public void ResetScroll()
	{
		bool flag = this._children != null;
		if (flag)
		{
			this._children.ForEach(delegate(CombatSceneObject e)
			{
				e.Reset();
			});
		}
	}

	// Token: 0x0600132D RID: 4909 RVA: 0x00076290 File Offset: 0x00074490
	public void Scroll(float xDelta)
	{
		xDelta *= this.LayerOffsetRate;
		bool flag = Mathf.Abs(xDelta) <= 0f || this._children == null;
		if (!flag)
		{
			xDelta %= this.LayerWidth;
			this._children.ForEach(delegate(CombatSceneObject child)
			{
				child.Scroll(xDelta);
			});
			this.CheckScroll((xDelta > 0f) ? 1 : -1);
		}
	}

	// Token: 0x0600132E RID: 4910 RVA: 0x00076328 File Offset: 0x00074528
	private void CheckScroll(int direction)
	{
		for (int i = 0; i < this._children.Count; i++)
		{
			CombatSceneObject child = this._children[i];
			bool flag = this._rectTransform.rect.Contains(child.RectTrans.offsetMin) || this._rectTransform.rect.Contains(child.RectTrans.offsetMax);
			if (!flag)
			{
				Vector2 pos = child.RectTrans.anchoredPosition;
				bool flag2 = direction == 1;
				if (flag2)
				{
					bool flag3 = child.RectTrans.anchoredPosition.x >= this.LayerWidth / 2f;
					if (flag3)
					{
						pos.x -= this.LayerWidth;
						child.RectTrans.anchoredPosition = pos;
					}
				}
				else
				{
					bool flag4 = direction == -1;
					if (flag4)
					{
						bool flag5 = child.RectTrans.anchoredPosition.x < -this.LayerWidth / 2f;
						if (flag5)
						{
							pos.x += this.LayerWidth;
							child.RectTrans.anchoredPosition = pos;
						}
					}
				}
			}
		}
	}

	// Token: 0x0400101E RID: 4126
	private RectTransform _rectTransform;

	// Token: 0x0400101F RID: 4127
	public float LayerWidth;

	// Token: 0x04001020 RID: 4128
	public Vector2 ScaleRange;

	// Token: 0x04001021 RID: 4129
	public float LayerOffsetRate;

	// Token: 0x04001022 RID: 4130
	private List<CombatSceneObject> _children;

	// Token: 0x04001023 RID: 4131
	private Tweener _scrollTweener;
}
