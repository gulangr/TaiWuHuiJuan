using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Components.Common
{
	// Token: 0x02000F92 RID: 3986
	public class PageSwitchHorizontalController : MonoBehaviour
	{
		// Token: 0x1400008C RID: 140
		// (add) Token: 0x0600B73C RID: 46908 RVA: 0x0053824C File Offset: 0x0053644C
		// (remove) Token: 0x0600B73D RID: 46909 RVA: 0x00538284 File Offset: 0x00536484
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int> OnSelectIndexChangeEvent;

		// Token: 0x170014AC RID: 5292
		// (get) Token: 0x0600B73E RID: 46910 RVA: 0x005382B9 File Offset: 0x005364B9
		// (set) Token: 0x0600B73F RID: 46911 RVA: 0x005382C1 File Offset: 0x005364C1
		public int PageCount { get; private set; }

		// Token: 0x170014AD RID: 5293
		// (get) Token: 0x0600B740 RID: 46912 RVA: 0x005382CA File Offset: 0x005364CA
		// (set) Token: 0x0600B741 RID: 46913 RVA: 0x005382D2 File Offset: 0x005364D2
		public int CurPageIndex { get; private set; }

		// Token: 0x0600B742 RID: 46914 RVA: 0x005382DC File Offset: 0x005364DC
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

		// Token: 0x0600B743 RID: 46915 RVA: 0x00538462 File Offset: 0x00536662
		private void OnEnable()
		{
			this.UpdateBtnInteractable();
			this.ClampTargetItemInViewport(this.CurPageIndex, true);
		}

		// Token: 0x0600B744 RID: 46916 RVA: 0x0053847C File Offset: 0x0053667C
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

		// Token: 0x0600B745 RID: 46917 RVA: 0x0053850B File Offset: 0x0053670B
		public void RegisterOnSelectIndexChangeHandler(Action<int> handler)
		{
			this.OnSelectIndexChangeEvent -= handler;
			this.OnSelectIndexChangeEvent += handler;
		}

		// Token: 0x0600B746 RID: 46918 RVA: 0x0053851E File Offset: 0x0053671E
		public void UnregisterOnSelectIndexChangeHandler(Action<int> handler)
		{
			this.OnSelectIndexChangeEvent -= handler;
		}

		// Token: 0x0600B747 RID: 46919 RVA: 0x0053852C File Offset: 0x0053672C
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

		// Token: 0x0600B748 RID: 46920 RVA: 0x00538610 File Offset: 0x00536810
		public void SelectNext()
		{
			bool flag = this.CurPageIndex < this.PageCount - 1;
			if (flag)
			{
				this.SetSelect(this.CurPageIndex + 1, true);
			}
		}

		// Token: 0x0600B749 RID: 46921 RVA: 0x00538644 File Offset: 0x00536844
		public void SelectPrev()
		{
			bool flag = this.CurPageIndex > 0;
			if (flag)
			{
				this.SetSelect(this.CurPageIndex - 1, true);
			}
		}

		// Token: 0x0600B74A RID: 46922 RVA: 0x00538670 File Offset: 0x00536870
		public void SelectFirst()
		{
			bool flag = this.CurPageIndex > 0;
			if (flag)
			{
				this.StopSpringAnimation();
				this.SetSelect(0, true);
			}
		}

		// Token: 0x0600B74B RID: 46923 RVA: 0x005386A0 File Offset: 0x005368A0
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

		// Token: 0x0600B74C RID: 46924 RVA: 0x005386E8 File Offset: 0x005368E8
		public void SelectLast()
		{
			bool flag = this.CurPageIndex < this.PageCount - 1;
			if (flag)
			{
				this.StopSpringAnimation();
				this.SetSelect(this.PageCount - 1, true);
			}
		}

		// Token: 0x0600B74D RID: 46925 RVA: 0x00538724 File Offset: 0x00536924
		public Refers GetPageItem(int pageIndex)
		{
			return this._pageItemList.Find((Refers e) => e.UserInt == pageIndex);
		}

		// Token: 0x0600B74E RID: 46926 RVA: 0x0053875C File Offset: 0x0053695C
		private void UpdateBtnInteractable()
		{
			this.BtnPrevPage.interactable = (this.CurPageIndex > 0);
			this.BtnNextPage.interactable = (this.CurPageIndex < this.PageCount - 1);
			this.BtnFirst.interactable = (this.CurPageIndex > 0);
			this.BtnLast.interactable = (this.CurPageIndex < this.PageCount - 1);
		}

		// Token: 0x0600B74F RID: 46927 RVA: 0x005387CC File Offset: 0x005369CC
		protected virtual Refers GetFreePageItem()
		{
			GameObject obj = this._itemPool.GetObject();
			bool flag = obj.transform.parent != this.ItemLayoutRoot;
			if (flag)
			{
				obj.transform.SetParent(this.ItemLayoutRoot, false);
			}
			return obj.GetComponent<Refers>();
		}

		// Token: 0x0600B750 RID: 46928 RVA: 0x00538820 File Offset: 0x00536A20
		protected virtual Refers GetAndFillPageItem(int pageIndex)
		{
			Refers item = this.GetFreePageItem();
			CToggle tempToggle = item.GetComponent<CToggle>();
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

		// Token: 0x0600B751 RID: 46929 RVA: 0x0053892C File Offset: 0x00536B2C
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

		// Token: 0x0600B752 RID: 46930 RVA: 0x00538A34 File Offset: 0x00536C34
		protected virtual void SetupHeadElementRect(RectTransform itemRectTrans, float width)
		{
			itemRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, this.Padding, width);
			itemRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, this.ItemLayoutRoot.rect.height);
			itemRectTrans.SetAnchor(Vector2.one * 0.5f, Vector2.one * 0.5f);
		}

		// Token: 0x0600B753 RID: 46931 RVA: 0x00538A98 File Offset: 0x00536C98
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

		// Token: 0x0600B754 RID: 46932 RVA: 0x00538BC4 File Offset: 0x00536DC4
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

		// Token: 0x0600B755 RID: 46933 RVA: 0x00538ED8 File Offset: 0x005370D8
		protected virtual void FillViewPort()
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

		// Token: 0x0600B756 RID: 46934 RVA: 0x005391A2 File Offset: 0x005373A2
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

		// Token: 0x0600B757 RID: 46935 RVA: 0x005391C0 File Offset: 0x005373C0
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

		// Token: 0x0600B758 RID: 46936 RVA: 0x00539430 File Offset: 0x00537630
		public void OnEndDrag()
		{
			this.CheckEdgeSpring();
		}

		// Token: 0x0600B759 RID: 46937 RVA: 0x0053943C File Offset: 0x0053763C
		public void OnDrag(BaseEventData eventData)
		{
			bool flag = this._disableScroll || this._showingSpringAnimation;
			if (!flag)
			{
				PointerEventData pointerEventData = eventData as PointerEventData;
				this.Scroll(pointerEventData.delta.x);
			}
		}

		// Token: 0x0600B75A RID: 46938 RVA: 0x0053947C File Offset: 0x0053767C
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

		// Token: 0x04008E53 RID: 36435
		public float ScrollSpeed;

		// Token: 0x04008E54 RID: 36436
		public CButton BtnPrevPage;

		// Token: 0x04008E55 RID: 36437
		public CButton BtnNextPage;

		// Token: 0x04008E56 RID: 36438
		public CButton BtnFirst;

		// Token: 0x04008E57 RID: 36439
		public CButton BtnLast;

		// Token: 0x04008E58 RID: 36440
		public RectTransform ItemLayoutRoot;

		// Token: 0x04008E59 RID: 36441
		public float Gap;

		// Token: 0x04008E5A RID: 36442
		public float Padding;

		// Token: 0x04008E5B RID: 36443
		public RectTransform HideDotLeft;

		// Token: 0x04008E5C RID: 36444
		public RectTransform HideDotRight;

		// Token: 0x04008E5D RID: 36445
		public Refers PageItemPrefab;

		// Token: 0x04008E5E RID: 36446
		protected List<Refers> _pageItemList;

		// Token: 0x04008E5F RID: 36447
		protected PoolItem _itemPool;

		// Token: 0x04008E60 RID: 36448
		private bool _showingSpringAnimation;

		// Token: 0x04008E61 RID: 36449
		private Tweener _springTweener;

		// Token: 0x04008E62 RID: 36450
		private bool _disableScroll;

		// Token: 0x04008E63 RID: 36451
		public Action<int, Refers> PageItemRefreshHandler;

		// Token: 0x04008E64 RID: 36452
		public Action<Refers, bool> SetItemSelectStateHandler;

		// Token: 0x04008E65 RID: 36453
		private bool _awakeFlag;

		// Token: 0x04008E66 RID: 36454
		private Coroutine _springCoroutine;
	}
}
