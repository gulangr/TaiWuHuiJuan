using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200006E RID: 110
public class InfinityScrollForDynamicChildren : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x060003D7 RID: 983 RVA: 0x00017876 File Offset: 0x00015A76
	// (set) Token: 0x060003D8 RID: 984 RVA: 0x0001787E File Offset: 0x00015A7E
	public int DataCount { get; private set; }

	// Token: 0x060003D9 RID: 985 RVA: 0x00017888 File Offset: 0x00015A88
	private void Awake()
	{
		bool flag = null == this.SrcPrefab;
		if (flag)
		{
			Debug.LogError("null SrcPrefab is not allowed");
		}
		else
		{
			RectTransform rectTrans = this.SrcPrefab.GetComponent<RectTransform>();
			rectTrans.pivot = Vector2.up;
			rectTrans.anchorMin = Vector2.up;
			rectTrans.anchorMax = Vector2.up;
			this._linePool = new PoolItem(string.Format("{0}_ScrollLineItemPool", base.GetInstanceID()), this.SrcPrefab);
			this._scrollItemList = new List<InfinityScrollForDynamicChildren.DynamicScrollItemData>();
		}
	}

	// Token: 0x060003DA RID: 986 RVA: 0x00017915 File Offset: 0x00015B15
	private void Update()
	{
		this.CheckChildren();
	}

	// Token: 0x060003DB RID: 987 RVA: 0x0001791F File Offset: 0x00015B1F
	public void OnDrag(PointerEventData eventData)
	{
		throw new NotImplementedException();
	}

	// Token: 0x060003DC RID: 988 RVA: 0x00017927 File Offset: 0x00015B27
	public void OnBeginDrag(PointerEventData eventData)
	{
		throw new NotImplementedException();
	}

	// Token: 0x060003DD RID: 989 RVA: 0x0001792F File Offset: 0x00015B2F
	public void OnEndDrag(PointerEventData eventData)
	{
		throw new NotImplementedException();
	}

	// Token: 0x060003DE RID: 990 RVA: 0x00017937 File Offset: 0x00015B37
	public void SetDataCount(int count)
	{
		this.DataCount = count;
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00017942 File Offset: 0x00015B42
	public void ScrollTo(int index)
	{
		index = Mathf.Clamp(index, 0, this.DataCount - 1);
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00017958 File Offset: 0x00015B58
	private void ProcessScrollItemList()
	{
		for (int i = 0; i < this.DataCount; i++)
		{
			bool flag = this._scrollItemList.CheckIndex(i);
			if (flag)
			{
				InfinityScrollForDynamicChildren.DynamicScrollItemData itemData = this._scrollItemList[i];
				itemData.Index = i;
				itemData.PositionCache = 0f;
				itemData.SizeCache = 0f;
			}
			else
			{
				this._scrollItemList.Add(new InfinityScrollForDynamicChildren.DynamicScrollItemData
				{
					Index = i,
					PositionCache = 0f,
					SizeCache = 0f
				});
			}
		}
		bool flag2 = this._scrollItemList.Count > this.DataCount;
		if (flag2)
		{
			this._scrollItemList.RemoveRange(this._scrollItemList.Count, this.DataCount - this._scrollItemList.Count);
		}
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x00017A30 File Offset: 0x00015C30
	private void RenderItem(InfinityScrollForDynamicChildren.DynamicScrollItemData itemData)
	{
		bool flag = null == itemData.ItemRect;
		if (flag)
		{
			GameObject cell = this._linePool.GetObject();
			itemData.ItemRect = cell.GetComponent<RectTransform>();
		}
		Refers refers = itemData.ItemRect.GetComponent<Refers>();
		Action<Refers, int> onItemRender = this.OnItemRender;
		if (onItemRender != null)
		{
			onItemRender(refers, itemData.Index);
		}
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x00017A90 File Offset: 0x00015C90
	private void HideItem(InfinityScrollForDynamicChildren.DynamicScrollItemData itemData)
	{
		bool flag = null != itemData.ItemRect;
		if (flag)
		{
			Action<Refers> onItemHide = this.OnItemHide;
			if (onItemHide != null)
			{
				onItemHide(itemData.ItemRect.GetComponent<Refers>());
			}
		}
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x00017ACC File Offset: 0x00015CCC
	private void CheckChildren()
	{
		bool flag = this._scrollItemList == null || this._scrollItemList.Count <= 0;
		if (!flag)
		{
			bool flag2 = this._first == null;
			if (flag2)
			{
				this._first = this._scrollItemList[0];
			}
			bool flag3 = this._last == null;
			if (flag3)
			{
				this._last = this._scrollItemList[0];
			}
			bool flag4 = null == this._first.ItemRect;
			if (flag4)
			{
				this.RenderItem(this._first);
			}
			else
			{
				bool flag5 = null == this._last.ItemRect;
				if (flag5)
				{
					this.RenderItem(this._last);
				}
			}
		}
	}

	// Token: 0x0400025D RID: 605
	public RectTransform HidePoint1;

	// Token: 0x0400025E RID: 606
	public RectTransform HidePoint2;

	// Token: 0x0400025F RID: 607
	public GameObject SrcPrefab;

	// Token: 0x04000260 RID: 608
	public CScrollbarLegacy ScrollBar;

	// Token: 0x04000261 RID: 609
	public RectTransform.Axis Axis;

	// Token: 0x04000262 RID: 610
	public Action<Refers, int> OnItemRender;

	// Token: 0x04000263 RID: 611
	public Action<Refers> OnItemHide;

	// Token: 0x04000264 RID: 612
	private List<InfinityScrollForDynamicChildren.DynamicScrollItemData> _scrollItemList;

	// Token: 0x04000265 RID: 613
	private PoolItem _linePool;

	// Token: 0x04000266 RID: 614
	private InfinityScrollForDynamicChildren.DynamicScrollItemData _first;

	// Token: 0x04000267 RID: 615
	private InfinityScrollForDynamicChildren.DynamicScrollItemData _last;

	// Token: 0x020010E0 RID: 4320
	private class DynamicScrollItemData
	{
		// Token: 0x170015B9 RID: 5561
		// (get) Token: 0x0600C0DB RID: 49371 RVA: 0x0056C0ED File Offset: 0x0056A2ED
		public bool LayoutDirtyFlag
		{
			get
			{
				return Mathf.Abs(this.SizeCache) == 0f && Mathf.Abs(this.PositionCache) == 0f;
			}
		}

		// Token: 0x040094A1 RID: 38049
		public int Index;

		// Token: 0x040094A2 RID: 38050
		public RectTransform ItemRect;

		// Token: 0x040094A3 RID: 38051
		public float SizeCache;

		// Token: 0x040094A4 RID: 38052
		public float PositionCache;
	}
}
