using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000066 RID: 102
public class HorizontalPageSwitchController : MonoBehaviour
{
	// Token: 0x14000003 RID: 3
	// (add) Token: 0x06000359 RID: 857 RVA: 0x000147BC File Offset: 0x000129BC
	// (remove) Token: 0x0600035A RID: 858 RVA: 0x000147F4 File Offset: 0x000129F4
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action<int> OnSelectIndexChangeEvent;

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x0600035B RID: 859 RVA: 0x00014829 File Offset: 0x00012A29
	// (set) Token: 0x0600035C RID: 860 RVA: 0x00014831 File Offset: 0x00012A31
	public int PageCount { get; private set; }

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x0600035D RID: 861 RVA: 0x0001483A File Offset: 0x00012A3A
	// (set) Token: 0x0600035E RID: 862 RVA: 0x00014842 File Offset: 0x00012A42
	public int CurPageIndex { get; private set; }

	// Token: 0x0600035F RID: 863 RVA: 0x0001484C File Offset: 0x00012A4C
	private void Awake()
	{
		bool flag = null == this.ItemLayoutRoot;
		if (flag)
		{
			throw new Exception("PageSwitchController " + base.name + ": ItemLayoutController can not be null!");
		}
		bool flag2 = null == this.PageItemPrefab;
		if (flag2)
		{
			throw new Exception("PageSwitchController " + base.name + ": PageItemPrefab can not be null!");
		}
		bool awakeFlag = this._awakeFlag;
		if (!awakeFlag)
		{
			bool flag3 = null != this.BtnPrevPage;
			if (flag3)
			{
				this.BtnPrevPage.onClick.AddListener(new UnityAction(this.SelectPrev));
			}
			bool flag4 = null != this.BtnNextPage;
			if (flag4)
			{
				this.BtnNextPage.onClick.AddListener(new UnityAction(this.SelectNext));
			}
			bool flag5 = null != this.BtnFirst;
			if (flag5)
			{
				this.BtnFirst.onClick.AddListener(new UnityAction(this.SelectFirst));
			}
			bool flag6 = null != this.BtnLast;
			if (flag6)
			{
				this.BtnLast.onClick.AddListener(new UnityAction(this.SelectLast));
			}
			this._pageItemList = new List<Refers>();
			this._itemPool = new PoolItem(base.GetInstanceID().ToString(), this.PageItemPrefab.gameObject);
			Refers[] children = this.ItemLayoutRoot.GetComponentsInTopChildren(false);
			Array.ForEach<Refers>(children, delegate(Refers e)
			{
				this._itemPool.DestroyObject(e.gameObject);
			});
			this._awakeFlag = true;
		}
	}

	// Token: 0x06000360 RID: 864 RVA: 0x000149D2 File Offset: 0x00012BD2
	private void OnEnable()
	{
		this.UpdateBtnInteractable();
		this.ClampTargetItemInViewport(this.CurPageIndex, true);
	}

	// Token: 0x06000361 RID: 865 RVA: 0x000149EC File Offset: 0x00012BEC
	public void InitPageCount(int pageCount, int selectIndex = 0, bool initAllElements = false)
	{
		bool flag = !this._awakeFlag;
		if (flag)
		{
			this.Awake();
		}
		this.PageCount = pageCount;
		this.CurPageIndex = selectIndex;
		this.InitLayout(initAllElements);
		bool flag2 = selectIndex >= 0 && selectIndex < this.PageCount && this.PageCount > 0;
		if (flag2)
		{
			this.SetSelect(selectIndex, false);
		}
		this.UpdateBtnInteractable();
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
		if (activeInHierarchy)
		{
			this.CheckEdgeSpring();
			this.ClampTargetItemInViewport(this.CurPageIndex, false);
		}
	}

	// Token: 0x06000362 RID: 866 RVA: 0x00014A7B File Offset: 0x00012C7B
	public void RegisterOnSelectIndexChangeHandler(Action<int> handler)
	{
		this.OnSelectIndexChangeEvent -= handler;
		this.OnSelectIndexChangeEvent += handler;
	}

	// Token: 0x06000363 RID: 867 RVA: 0x00014A8E File Offset: 0x00012C8E
	public void UnregisterOnSelectIndexChangeHandler(Action<int> handler)
	{
		this.OnSelectIndexChangeEvent -= handler;
	}

	// Token: 0x06000364 RID: 868 RVA: 0x00014A9C File Offset: 0x00012C9C
	public void SetSelect(int pageIndex, bool clampWithAnimation = true)
	{
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
		if (activeInHierarchy)
		{
			this.ClampTargetItemInViewport(pageIndex, clampWithAnimation);
		}
		bool flag = this.CurPageIndex == pageIndex;
		if (flag)
		{
			Action<int> onSelectIndexChangeEvent = this.OnSelectIndexChangeEvent;
			if (onSelectIndexChangeEvent != null)
			{
				onSelectIndexChangeEvent(this.CurPageIndex);
			}
		}
		else
		{
			Refers prevRefers = this.GetPageItem(this.CurPageIndex);
			bool flag2 = null != prevRefers;
			if (flag2)
			{
				Action<Refers, bool> setItemSelectStateHandler = this.SetItemSelectStateHandler;
				if (setItemSelectStateHandler != null)
				{
					setItemSelectStateHandler(prevRefers, false);
				}
			}
			this.CurPageIndex = Mathf.Clamp(pageIndex, -1, this.PageCount - 1);
			Refers curRefers = this.GetPageItem(this.CurPageIndex);
			bool flag3 = null != curRefers;
			if (flag3)
			{
				Action<Refers, bool> setItemSelectStateHandler2 = this.SetItemSelectStateHandler;
				if (setItemSelectStateHandler2 != null)
				{
					setItemSelectStateHandler2(curRefers, true);
				}
			}
			this.UpdateBtnInteractable();
			Action<int> onSelectIndexChangeEvent2 = this.OnSelectIndexChangeEvent;
			if (onSelectIndexChangeEvent2 != null)
			{
				onSelectIndexChangeEvent2(this.CurPageIndex);
			}
		}
	}

	// Token: 0x06000365 RID: 869 RVA: 0x00014B80 File Offset: 0x00012D80
	public void SelectNext()
	{
		bool flag = this.CurPageIndex < this.PageCount - 1;
		if (flag)
		{
			this.SetSelect(this.CurPageIndex + 1, true);
		}
	}

	// Token: 0x06000366 RID: 870 RVA: 0x00014BB4 File Offset: 0x00012DB4
	public void SelectPrev()
	{
		bool flag = this.CurPageIndex > 0;
		if (flag)
		{
			this.SetSelect(this.CurPageIndex - 1, true);
		}
	}

	// Token: 0x06000367 RID: 871 RVA: 0x00014BE0 File Offset: 0x00012DE0
	public void SelectFirst()
	{
		bool flag = this.CurPageIndex > 0;
		if (flag)
		{
			this.StopSpringAnimation();
			this.SetSelect(0, true);
		}
	}

	// Token: 0x06000368 RID: 872 RVA: 0x00014C10 File Offset: 0x00012E10
	private void StopSpringAnimation()
	{
		CommonUtils.TryKillTween(this._springTweener, false);
		this._showingSpringAnimation = false;
		bool flag = this._springCoroutine != null;
		if (flag)
		{
			base.StopCoroutine(this._springCoroutine);
			this._springCoroutine = null;
		}
	}

	// Token: 0x06000369 RID: 873 RVA: 0x00014C58 File Offset: 0x00012E58
	public void SelectLast()
	{
		bool flag = this.CurPageIndex < this.PageCount - 1;
		if (flag)
		{
			this.StopSpringAnimation();
			this.SetSelect(this.PageCount - 1, true);
		}
	}

	// Token: 0x0600036A RID: 874 RVA: 0x00014C94 File Offset: 0x00012E94
	public Refers GetPageItem(int pageIndex)
	{
		return this._pageItemList.Find((Refers e) => e.UserInt == pageIndex);
	}

	// Token: 0x0600036B RID: 875 RVA: 0x00014CCC File Offset: 0x00012ECC
	private void UpdateBtnInteractable()
	{
		this.BtnPrevPage.interactable = (this.CurPageIndex > 0);
		this.BtnNextPage.interactable = (this.CurPageIndex < this.PageCount - 1);
		this.BtnFirst.interactable = (this.CurPageIndex > 0);
		this.BtnLast.interactable = (this.CurPageIndex < this.PageCount - 1);
	}

	// Token: 0x0600036C RID: 876 RVA: 0x00014D3C File Offset: 0x00012F3C
	private Refers GetFreePageItem()
	{
		GameObject obj = this._itemPool.GetObject();
		bool flag = obj.transform.parent != this.ItemLayoutRoot;
		if (flag)
		{
			obj.transform.SetParent(this.ItemLayoutRoot, false);
		}
		return obj.GetComponent<Refers>();
	}

	// Token: 0x0600036D RID: 877 RVA: 0x00014D90 File Offset: 0x00012F90
	private Refers GetAndFillPageItem(int pageIndex)
	{
		Refers item = this.GetFreePageItem();
		CToggleObsolete tempToggle = item.GetComponent<CToggleObsolete>();
		item.UserInt = pageIndex;
		item.name = string.Format("item_{0}", pageIndex);
		this._pageItemList.Add(item);
		Action<int, Refers> pageItemRefreshHandler = this.PageItemRefreshHandler;
		if (pageItemRefreshHandler != null)
		{
			pageItemRefreshHandler(pageIndex, item);
		}
		Action<Refers, bool> setItemSelectStateHandler = this.SetItemSelectStateHandler;
		if (setItemSelectStateHandler != null)
		{
			setItemSelectStateHandler(item, pageIndex == this.CurPageIndex);
		}
		ContentSizeFitter[] contentSizeFitters = item.GetComponentsInChildren<ContentSizeFitter>();
		bool flag = contentSizeFitters != null;
		if (flag)
		{
			foreach (ContentSizeFitter contentSizeFitter in contentSizeFitters)
			{
				RectTransform itemRectTrans = contentSizeFitter.transform as RectTransform;
				LayoutRebuilder.ForceRebuildLayoutImmediate(itemRectTrans);
				contentSizeFitter.SetLayoutHorizontal();
			}
		}
		UIRectSizeController[] sizeControllers = item.GetComponentsInChildren<UIRectSizeController>();
		Array.ForEach<UIRectSizeController>(sizeControllers, delegate(UIRectSizeController e)
		{
			e.TrySetSizeManually();
		});
		ContentSizeFitter component = item.GetComponent<ContentSizeFitter>();
		if (component != null)
		{
			component.SetLayoutHorizontal();
		}
		return item;
	}

	// Token: 0x0600036E RID: 878 RVA: 0x00014E9C File Offset: 0x0001309C
	private void InitLayout(bool initAllElements)
	{
		bool flag = !this._awakeFlag;
		if (flag)
		{
			this.Awake();
		}
		this._pageItemList.ForEach(delegate(Refers e)
		{
			this._itemPool.DestroyObject(e.gameObject);
		});
		this._pageItemList.Clear();
		bool flag2 = this.PageCount <= 0;
		if (!flag2)
		{
			RectTransform itemRectTrans = this.GetAndFillPageItem(0).transform as RectTransform;
			itemRectTrans.localPosition = itemRectTrans.localPosition.SetY(0f);
			float width = itemRectTrans.rect.width;
			this.SetupHeadElementRect(itemRectTrans, width);
			if (initAllElements)
			{
				for (int i = 1; i < this.PageCount; i++)
				{
					Refers item = this.GetAndFillPageItem(i);
					item.RectTransform.localPosition = new Vector3(itemRectTrans.localPosition.x + (width + this.Padding) * (float)i, 0f, 0f);
				}
			}
			this.FillViewPort();
		}
	}

	// Token: 0x0600036F RID: 879 RVA: 0x00014FA4 File Offset: 0x000131A4
	protected virtual void SetupHeadElementRect(RectTransform itemRectTrans, float width)
	{
		itemRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, this.Padding, width);
		itemRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, this.ItemLayoutRoot.rect.height);
		itemRectTrans.SetAnchor(Vector2.one * 0.5f, Vector2.one * 0.5f);
	}

	// Token: 0x06000370 RID: 880 RVA: 0x00015008 File Offset: 0x00013208
	protected virtual void Scroll(float deltaX)
	{
		Refers firstCell = this.GetPageItem(0);
		Refers lastCell = this.GetPageItem(this.PageCount - 1);
		bool ignoreReuse = null != firstCell && null != lastCell;
		for (int i = this._pageItemList.Count - 1; i >= 0; i--)
		{
			RectTransform itemRectTrans = this._pageItemList[i].transform as RectTransform;
			itemRectTrans.anchoredPosition = itemRectTrans.anchoredPosition.SetX(itemRectTrans.anchoredPosition.x + deltaX);
			bool flag = ignoreReuse;
			if (!flag)
			{
				bool flag2 = itemRectTrans.position.x < this.HideDotLeft.position.x || itemRectTrans.position.x > this.HideDotRight.position.x;
				if (flag2)
				{
					this._pageItemList.RemoveAt(i);
					this._itemPool.DestroyObject(itemRectTrans.gameObject);
					itemRectTrans.name = "poolItem";
				}
			}
		}
		bool flag3 = !ignoreReuse;
		if (flag3)
		{
			this.FillViewPort();
		}
	}

	// Token: 0x06000371 RID: 881 RVA: 0x00015134 File Offset: 0x00013334
	private void ClampTargetItemInViewport(int itemIndex, bool withAnimation = true)
	{
		bool flag = itemIndex < 0 || itemIndex >= this.PageCount;
		if (!flag)
		{
			Rect rootRect = this.ItemLayoutRoot.rect;
			Refers cell = this.GetPageItem(itemIndex);
			bool flag2 = null != cell;
			if (flag2)
			{
				RectTransform cellRectTrans = cell.transform as RectTransform;
				bool flag3 = cellRectTrans.localPosition.x + cellRectTrans.rect.xMin > rootRect.xMin + this.Padding && cellRectTrans.localPosition.x + cellRectTrans.rect.xMax < rootRect.xMax - this.Padding;
				if (flag3)
				{
					return;
				}
				bool flag4 = cellRectTrans.localPosition.x + cellRectTrans.rect.xMin < rootRect.xMin + this.Padding;
				if (flag4)
				{
					base.StopAllCoroutines();
					this._springCoroutine = base.StartCoroutine(this.ShowSpringAnimation(rootRect.xMin + this.Padding - cellRectTrans.localPosition.x + cellRectTrans.rect.xMin + cellRectTrans.rect.width, withAnimation));
					return;
				}
				bool flag5 = cellRectTrans.localPosition.x + cellRectTrans.rect.xMax > rootRect.xMax - this.Padding;
				if (flag5)
				{
					base.StopAllCoroutines();
					this._springCoroutine = base.StartCoroutine(this.ShowSpringAnimation(rootRect.xMax - this.Padding - cellRectTrans.localPosition.x + cellRectTrans.rect.xMax - cellRectTrans.rect.width, withAnimation));
					return;
				}
			}
			this._pageItemList.Sort((Refers left, Refers right) => left.UserInt - right.UserInt);
			int firstIndex = this._pageItemList[0].UserInt;
			int lastIndex = this._pageItemList[this._pageItemList.Count - 1].UserInt;
			this._pageItemList.ForEach(delegate(Refers e)
			{
				this._itemPool.DestroyObject(e.gameObject);
				e.name = "poolItem";
			});
			this._pageItemList.Clear();
			cell = this.GetAndFillPageItem(itemIndex);
			RectTransform itemRectTrans = cell.transform as RectTransform;
			itemRectTrans.localPosition = itemRectTrans.localPosition.SetY(0f);
			bool flag6 = itemIndex <= firstIndex;
			if (flag6)
			{
				itemRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, this.Padding, itemRectTrans.rect.width);
			}
			else
			{
				bool flag7 = itemIndex >= lastIndex;
				if (flag7)
				{
					itemRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, this.Padding, itemRectTrans.rect.width);
				}
			}
			itemRectTrans.SetAnchor(Vector2.one * 0.5f, Vector2.one * 0.5f);
			this.FillViewPort();
		}
	}

	// Token: 0x06000372 RID: 882 RVA: 0x00015448 File Offset: 0x00013648
	protected void FillViewPort()
	{
		Refers cellMin = null;
		Refers cellMax = null;
		for (int i = this._pageItemList.Count - 1; i >= 0; i--)
		{
			Refers cell = this._pageItemList[i];
			bool flag = null == cellMin && null == cellMax;
			if (flag)
			{
				cellMin = cell;
				cellMax = cell;
			}
			else
			{
				bool flag2 = cell.transform.localPosition.x < cellMin.transform.localPosition.x;
				if (flag2)
				{
					cellMin = cell;
				}
				bool flag3 = cell.transform.localPosition.x > cellMax.transform.localPosition.x;
				if (flag3)
				{
					cellMax = cell;
				}
			}
		}
		bool flag4 = null != cellMin;
		if (flag4)
		{
			for (int j = cellMin.UserInt - 1; j >= 0; j--)
			{
				Refers item = this.GetAndFillPageItem(j);
				RectTransform itemRectTrans = item.transform as RectTransform;
				RectTransform cellMinRectTrans = cellMin.transform as RectTransform;
				float x = cellMinRectTrans.localPosition.x - cellMinRectTrans.rect.width * cellMinRectTrans.pivot.x - itemRectTrans.rect.width * (1f - itemRectTrans.pivot.x) - this.Gap;
				itemRectTrans.localPosition = itemRectTrans.localPosition.SetX(x).SetY(cellMinRectTrans.localPosition.y);
				bool flag5 = itemRectTrans.position.x < this.HideDotLeft.position.x;
				if (flag5)
				{
					break;
				}
				cellMin = item;
			}
		}
		bool flag6 = null != cellMax;
		if (flag6)
		{
			for (int k = cellMax.UserInt + 1; k < this.PageCount; k++)
			{
				Refers item2 = this.GetAndFillPageItem(k);
				RectTransform itemRectTrans2 = item2.transform as RectTransform;
				RectTransform cellMaxRectTrans = cellMax.transform as RectTransform;
				float x2 = cellMaxRectTrans.localPosition.x + cellMaxRectTrans.rect.width * (1f - cellMaxRectTrans.pivot.x) + itemRectTrans2.rect.width * itemRectTrans2.pivot.x + this.Gap;
				itemRectTrans2.localPosition = itemRectTrans2.localPosition.SetX(x2).SetY(cellMaxRectTrans.localPosition.y);
				bool flag7 = itemRectTrans2.position.x > this.HideDotRight.position.x;
				if (flag7)
				{
					break;
				}
				cellMax = item2;
			}
		}
	}

	// Token: 0x06000373 RID: 883 RVA: 0x00015712 File Offset: 0x00013912
	private IEnumerator ShowSpringAnimation(float offset, bool withAnimation = true)
	{
		CommonUtils.TryKillTween(this._springTweener, false);
		this._showingSpringAnimation = true;
		float animDuration = withAnimation ? 0.3f : 0f;
		float stepLastValue = 0f;
		this._springTweener = DOVirtual.Float(0f, 1f, animDuration, delegate(float stepValue)
		{
			float stepOffset = stepValue - stepLastValue;
			this.Scroll(offset * stepOffset);
			stepLastValue = stepValue;
		}).SetUpdate(UpdateType.Normal, true);
		yield return new WaitForSecondsRealtime(animDuration);
		this._showingSpringAnimation = false;
		yield break;
	}

	// Token: 0x06000374 RID: 884 RVA: 0x00015730 File Offset: 0x00013930
	private void CheckEdgeSpring()
	{
		bool flag = this._pageItemList == null || this._pageItemList.Count <= 0;
		if (!flag)
		{
			Refers cellMin = null;
			Refers cellMax = null;
			for (int i = this._pageItemList.Count - 1; i >= 0; i--)
			{
				Refers cell = this._pageItemList[i];
				bool flag2 = null == cellMin && null == cellMax;
				if (flag2)
				{
					cellMin = cell;
					cellMax = cell;
				}
				else
				{
					bool flag3 = cell.transform.localPosition.x < cellMin.transform.localPosition.x;
					if (flag3)
					{
						cellMin = cell;
					}
					bool flag4 = cell.transform.localPosition.x > cellMax.transform.localPosition.x;
					if (flag4)
					{
						cellMax = cell;
					}
				}
			}
			Rect rootRect = this.ItemLayoutRoot.rect;
			RectTransform cellMinRectTrans = cellMin.transform as RectTransform;
			RectTransform cellMaxRectTrans = cellMax.transform as RectTransform;
			bool flag5 = cellMin.UserInt == 0;
			if (flag5)
			{
				bool flag6 = cellMax.UserInt == this.PageCount - 1;
				if (flag6)
				{
					bool flag7 = cellMaxRectTrans.offsetMax.x - cellMinRectTrans.offsetMin.x < rootRect.width;
					if (flag7)
					{
						base.StopAllCoroutines();
						this._springCoroutine = base.StartCoroutine(this.ShowSpringAnimation(rootRect.xMin + this.Padding - cellMinRectTrans.offsetMin.x, true));
						return;
					}
				}
				float cellMinOffset = cellMinRectTrans.localPosition.x + cellMinRectTrans.rect.xMin - rootRect.xMin - this.Padding;
				bool flag8 = cellMinOffset > 0f;
				if (flag8)
				{
					base.StopAllCoroutines();
					this._springCoroutine = base.StartCoroutine(this.ShowSpringAnimation(-cellMinOffset, true));
					return;
				}
			}
			bool flag9 = cellMax.UserInt == this.PageCount - 1;
			if (flag9)
			{
				float cellMaxOffset = cellMaxRectTrans.localPosition.x + cellMaxRectTrans.rect.xMax - rootRect.xMax + this.Padding;
				bool flag10 = cellMaxOffset < 0f;
				if (flag10)
				{
					base.StopAllCoroutines();
					this._springCoroutine = base.StartCoroutine(this.ShowSpringAnimation(-cellMaxOffset, true));
				}
			}
		}
	}

	// Token: 0x06000375 RID: 885 RVA: 0x000159A0 File Offset: 0x00013BA0
	public void OnEndDrag()
	{
		this.CheckEdgeSpring();
	}

	// Token: 0x06000376 RID: 886 RVA: 0x000159AC File Offset: 0x00013BAC
	public void OnDrag(BaseEventData eventData)
	{
		bool flag = this._disableScroll || this._showingSpringAnimation;
		if (!flag)
		{
			PointerEventData pointerEventData = eventData as PointerEventData;
			this.Scroll(pointerEventData.delta.x);
		}
	}

	// Token: 0x06000377 RID: 887 RVA: 0x000159EC File Offset: 0x00013BEC
	public void OnScroll(BaseEventData eventData)
	{
		bool showingSpringAnimation = this._showingSpringAnimation;
		if (!showingSpringAnimation)
		{
			PointerEventData pointerEventData = eventData as PointerEventData;
			this.Scroll(this.ScrollSpeed * pointerEventData.scrollDelta.y);
			this.CheckEdgeSpring();
		}
	}

	// Token: 0x04000206 RID: 518
	public float ScrollSpeed;

	// Token: 0x04000207 RID: 519
	public CButtonObsolete BtnPrevPage;

	// Token: 0x04000208 RID: 520
	public CButtonObsolete BtnNextPage;

	// Token: 0x04000209 RID: 521
	public CButtonObsolete BtnFirst;

	// Token: 0x0400020A RID: 522
	public CButtonObsolete BtnLast;

	// Token: 0x0400020B RID: 523
	public RectTransform ItemLayoutRoot;

	// Token: 0x0400020C RID: 524
	public float Gap;

	// Token: 0x0400020D RID: 525
	public float Padding;

	// Token: 0x0400020E RID: 526
	public RectTransform HideDotLeft;

	// Token: 0x0400020F RID: 527
	public RectTransform HideDotRight;

	// Token: 0x04000210 RID: 528
	public Refers PageItemPrefab;

	// Token: 0x04000211 RID: 529
	protected List<Refers> _pageItemList;

	// Token: 0x04000212 RID: 530
	protected PoolItem _itemPool;

	// Token: 0x04000213 RID: 531
	private bool _showingSpringAnimation;

	// Token: 0x04000214 RID: 532
	private Tweener _springTweener;

	// Token: 0x04000215 RID: 533
	private bool _disableScroll;

	// Token: 0x04000216 RID: 534
	public Action<int, Refers> PageItemRefreshHandler;

	// Token: 0x04000217 RID: 535
	public Action<Refers, bool> SetItemSelectStateHandler;

	// Token: 0x04000218 RID: 536
	private bool _awakeFlag;

	// Token: 0x04000219 RID: 537
	private Coroutine _springCoroutine;
}
