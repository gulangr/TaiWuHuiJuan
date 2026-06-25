using System;
using FrameWork;
using UnityEngine;

// Token: 0x0200037A RID: 890
public class UI_DragShow : UIBase
{
	// Token: 0x060033FF RID: 13311 RVA: 0x0019C19C File Offset: 0x0019A39C
	public override void OnInit(ArgumentBox argsBox)
	{
		bool flag = null != this.DragItem;
		if (flag)
		{
			Object.Destroy(this.DragItem);
		}
		bool flag2 = argsBox.Get<GameObject>("DragItem", out this.DragItem);
		if (flag2)
		{
			this.DragItem.transform.SetParent(base.transform, false);
		}
		else
		{
			this.Element.Hide(false);
		}
	}

	// Token: 0x06003400 RID: 13312 RVA: 0x0019C202 File Offset: 0x0019A402
	private void Awake()
	{
		this._rectTransform = (base.transform as RectTransform);
	}

	// Token: 0x06003401 RID: 13313 RVA: 0x0019C218 File Offset: 0x0019A418
	private void LateUpdate()
	{
		bool flag = null != this.DragItem;
		if (flag)
		{
			this.DragItem.transform.localPosition = UIManager.Instance.MousePosToLocalPos(this._rectTransform);
		}
	}

	// Token: 0x06003402 RID: 13314 RVA: 0x0019C25E File Offset: 0x0019A45E
	private void OnEnable()
	{
		ConchShipCursor.Instance.SetCursorVisible(false);
	}

	// Token: 0x06003403 RID: 13315 RVA: 0x0019C26D File Offset: 0x0019A46D
	private void OnDisable()
	{
		ConchShipCursor.Instance.SetCursorVisible(true);
		Object.Destroy(this.DragItem);
		this.DragItem = null;
	}

	// Token: 0x040025E5 RID: 9701
	private RectTransform _rectTransform;

	// Token: 0x040025E6 RID: 9702
	public GameObject DragItem;
}
