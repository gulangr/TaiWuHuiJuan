using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000AD RID: 173
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class UIRectSizeController : UIBehaviour
{
	// Token: 0x060005FA RID: 1530 RVA: 0x00028411 File Offset: 0x00026611
	protected override void Awake()
	{
		base.Awake();
		this._selfTrans = base.GetComponent<RectTransform>();
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x00028428 File Offset: 0x00026628
	protected override void OnRectTransformDimensionsChange()
	{
		bool flag = this.ControlList == null || null == this._selfTrans;
		if (!flag)
		{
			Vector2 size = this._selfTrans.rect.size;
			foreach (UIRectSizeController.FollowerConfig config in this.ControlList)
			{
				bool flag2 = null == config.Target;
				if (!flag2)
				{
					bool flag3 = this._selfTrans.parent == config.Target;
					if (flag3)
					{
						bool flag4 = Math.Abs(this._selfTrans.anchorMin.x - this._selfTrans.anchorMax.x) > 0.0001f || Math.Abs(this._selfTrans.anchorMin.y - this._selfTrans.anchorMax.y) > 0.0001f;
						if (flag4)
						{
							continue;
						}
					}
					Vector2 newSize = new Vector2(size.x + config.SizeOffset.x, size.y + config.SizeOffset.y);
					newSize = this.SizeCheck(newSize);
					bool flag5 = this.freeHorizontal;
					if (flag5)
					{
						newSize.x = config.Target.sizeDelta.x;
					}
					bool flag6 = this.freeVertical;
					if (flag6)
					{
						newSize.y = config.Target.sizeDelta.y;
					}
					bool flag7 = newSize != config.Target.sizeDelta;
					if (flag7)
					{
						config.Target.SetSize(newSize);
						bool flag8 = this.ignoreLayout;
						if (!flag8)
						{
							LayoutGroup layoutGroup = config.Target.GetComponentInParent<LayoutGroup>();
							bool flag9 = null != layoutGroup;
							if (flag9)
							{
								HorizontalLayoutGroup horizontalLayoutGroup = layoutGroup as HorizontalLayoutGroup;
								bool flag10 = horizontalLayoutGroup != null;
								if (flag10)
								{
									horizontalLayoutGroup.CalculateLayoutInputHorizontal();
									horizontalLayoutGroup.SetLayoutHorizontal();
								}
								else
								{
									VerticalLayoutGroup verticalLayoutGroup = layoutGroup as VerticalLayoutGroup;
									bool flag11 = verticalLayoutGroup != null;
									if (flag11)
									{
										verticalLayoutGroup.CalculateLayoutInputVertical();
										verticalLayoutGroup.SetLayoutVertical();
									}
									else
									{
										GridLayoutGroup gridLayoutGroup = layoutGroup as GridLayoutGroup;
										bool flag12 = gridLayoutGroup != null;
										if (flag12)
										{
											gridLayoutGroup.cellSize = newSize;
										}
									}
								}
								LayoutRebuilder.MarkLayoutForRebuild(layoutGroup.GetComponent<RectTransform>());
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x000286B8 File Offset: 0x000268B8
	protected virtual Vector2 SizeCheck(Vector2 newSize)
	{
		return newSize;
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x000286CB File Offset: 0x000268CB
	protected override void OnEnable()
	{
		this.OnRectTransformDimensionsChange();
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x000286D8 File Offset: 0x000268D8
	public void TrySetSizeManually()
	{
		ILayoutElement layoutElement = base.GetComponent<ILayoutElement>();
		bool flag = layoutElement == null;
		if (!flag)
		{
			(base.transform as RectTransform).SetSize(new Vector2(layoutElement.preferredWidth, layoutElement.preferredHeight));
			this.OnRectTransformDimensionsChange();
		}
	}

	// Token: 0x040004E7 RID: 1255
	[SerializeField]
	private bool ignoreLayout = false;

	// Token: 0x040004E8 RID: 1256
	[SerializeField]
	private bool freeHorizontal;

	// Token: 0x040004E9 RID: 1257
	[SerializeField]
	private bool freeVertical;

	// Token: 0x040004EA RID: 1258
	public List<UIRectSizeController.FollowerConfig> ControlList;

	// Token: 0x040004EB RID: 1259
	protected RectTransform _selfTrans;

	// Token: 0x02001111 RID: 4369
	[Serializable]
	public struct FollowerConfig
	{
		// Token: 0x04009586 RID: 38278
		public RectTransform Target;

		// Token: 0x04009587 RID: 38279
		public Vector2 SizeOffset;
	}
}
