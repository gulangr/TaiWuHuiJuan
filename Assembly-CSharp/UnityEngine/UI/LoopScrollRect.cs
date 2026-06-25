using System;
using System.Diagnostics;
using FrameWork.UISystem.UIElements;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x02000FA8 RID: 4008
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public abstract class LoopScrollRect : LoopScrollRectBase, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x0600B7D8 RID: 47064 RVA: 0x0053D0B0 File Offset: 0x0053B2B0
		public void InitLoop(GameObject item, int loopMax, Action<Transform, int> callback, Action<Transform> cellReturn)
		{
			this.prefabSource = new LoopScrollPrefabSource
			{
				prefab = item,
				callback = cellReturn
			};
			this.dataSource = callback;
			this.totalCount = loopMax;
			this.RefillCells(0, false);
		}

		// Token: 0x0600B7D9 RID: 47065 RVA: 0x0053D0E4 File Offset: 0x0053B2E4
		public void InitLoop(int loopMax, Action<Transform, int> callback, Action<Transform> cellReturn)
		{
			this.prefabSource = new LoopScrollPrefabSource
			{
				prefab = this.templatePrefab,
				callback = cellReturn
			};
			this.dataSource = callback;
			this.totalCount = loopMax;
			this.RefillCells(0, false);
		}

		// Token: 0x0600B7DA RID: 47066 RVA: 0x0053D11C File Offset: 0x0053B31C
		public void StopLoop()
		{
			this.prefabSource = null;
			this.dataSource = null;
		}

		// Token: 0x1400008D RID: 141
		// (add) Token: 0x0600B7DB RID: 47067 RVA: 0x0053D130 File Offset: 0x0053B330
		// (remove) Token: 0x0600B7DC RID: 47068 RVA: 0x0053D168 File Offset: 0x0053B368
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnScrollEvent;

		// Token: 0x170014B3 RID: 5299
		// (get) Token: 0x0600B7DD RID: 47069 RVA: 0x0053D1A0 File Offset: 0x0053B3A0
		// (set) Token: 0x0600B7DE RID: 47070 RVA: 0x0053D1B8 File Offset: 0x0053B3B8
		public bool automaticallyChangeScrollbarSize
		{
			get
			{
				return this.m_AutomaticallyChangeScrollbarSize;
			}
			set
			{
				this.m_AutomaticallyChangeScrollbarSize = value;
				base.SetDirty();
			}
		}

		// Token: 0x170014B4 RID: 5300
		// (get) Token: 0x0600B7DF RID: 47071 RVA: 0x0053D1C9 File Offset: 0x0053B3C9
		public int ItemTypeStart
		{
			get
			{
				return this.itemTypeStart;
			}
		}

		// Token: 0x170014B5 RID: 5301
		// (get) Token: 0x0600B7E0 RID: 47072 RVA: 0x0053D1D1 File Offset: 0x0053B3D1
		public int ItemTypeEnd
		{
			get
			{
				return this.itemTypeEnd;
			}
		}

		// Token: 0x0600B7E1 RID: 47073 RVA: 0x0053D1DC File Offset: 0x0053B3DC
		public bool GetVScrollingNeeded()
		{
			return this._shouldDisplayScrollbar || base.vScrollingNeeded;
		}

		// Token: 0x0600B7E2 RID: 47074 RVA: 0x0053D1FF File Offset: 0x0053B3FF
		public void SetShouldDisplayScrollbar(bool value)
		{
			this._shouldDisplayScrollbar = value;
		}

		// Token: 0x0600B7E3 RID: 47075 RVA: 0x0053D209 File Offset: 0x0053B409
		protected override void ProvideData(Transform transform, int index)
		{
			Action<Transform, int> action = this.dataSource;
			if (action != null)
			{
				action(transform, index);
			}
		}

		// Token: 0x0600B7E4 RID: 47076 RVA: 0x0053D220 File Offset: 0x0053B420
		protected override RectTransform GetFromTempPool(int itemIdx)
		{
			bool flag = this.deletedItemTypeStart > 0;
			RectTransform nextItem;
			if (flag)
			{
				this.deletedItemTypeStart--;
				nextItem = (this.m_Content.GetChild(0) as RectTransform);
				nextItem.SetSiblingIndex(itemIdx - this.itemTypeStart + this.deletedItemTypeStart);
			}
			else
			{
				bool flag2 = this.deletedItemTypeEnd > 0;
				if (flag2)
				{
					this.deletedItemTypeEnd--;
					nextItem = (this.m_Content.GetChild(this.m_Content.childCount - 1) as RectTransform);
					nextItem.SetSiblingIndex(itemIdx - this.itemTypeStart + this.deletedItemTypeStart);
				}
				else
				{
					nextItem = (this.prefabSource.GetObject().transform as RectTransform);
					nextItem.transform.SetParent(this.m_Content, false);
					nextItem.gameObject.SetActive(true);
				}
			}
			this.ProvideData(nextItem, itemIdx);
			return nextItem;
		}

		// Token: 0x0600B7E5 RID: 47077 RVA: 0x0053D314 File Offset: 0x0053B514
		protected override void ReturnToTempPool(bool fromStart, int count = 1)
		{
			if (fromStart)
			{
				this.deletedItemTypeStart += count;
			}
			else
			{
				this.deletedItemTypeEnd += count;
			}
		}

		// Token: 0x0600B7E6 RID: 47078 RVA: 0x0053D348 File Offset: 0x0053B548
		protected override void ClearTempPool()
		{
			while (this.deletedItemTypeStart > 0)
			{
				this.deletedItemTypeStart--;
				this.prefabSource.ReturnObject(this.m_Content.GetChild(0));
			}
			while (this.deletedItemTypeEnd > 0)
			{
				this.deletedItemTypeEnd--;
				this.prefabSource.ReturnObject(this.m_Content.GetChild(this.m_Content.childCount - 1));
			}
		}

		// Token: 0x0600B7E7 RID: 47079 RVA: 0x0053D3D0 File Offset: 0x0053B5D0
		public new void ClearCells()
		{
			bool isPlaying = Application.isPlaying;
			if (isPlaying)
			{
				this.itemTypeStart = 0;
				this.itemTypeEnd = 0;
				this.itemTypeSize = 0f;
				this.totalCount = 0;
				this.dataSource = null;
				for (int i = this.m_Content.childCount - 1; i >= 0; i--)
				{
					this.prefabSource.ReturnObject(this.m_Content.GetChild(i));
				}
			}
		}

		// Token: 0x0600B7E8 RID: 47080 RVA: 0x0053D449 File Offset: 0x0053B649
		public void RefillCells(int offset = 0, bool fillViewRect = false)
		{
			base.RefillCells(offset, 0f);
		}

		// Token: 0x0600B7E9 RID: 47081 RVA: 0x0053D459 File Offset: 0x0053B659
		public void RefillCellsFromEnd(int offset = 0, bool alignStart = false)
		{
			base.RefillCellsFromEnd(offset, 0f);
		}

		// Token: 0x0600B7EA RID: 47082 RVA: 0x0053D46C File Offset: 0x0053B66C
		public new void SrollToCell(int index, float speed)
		{
			bool flag = speed <= 0f;
			if (flag)
			{
				base.RefillCells(index, 0f);
			}
			else
			{
				base.ScrollToCell(index, speed, 0f, LoopScrollRectBase.ScrollMode.ToStart);
			}
		}

		// Token: 0x0600B7EB RID: 47083 RVA: 0x0053D4A8 File Offset: 0x0053B6A8
		public void RefillCellsAtCurrentPosition()
		{
			bool flag = !Application.isPlaying || !base.isActiveAndEnabled;
			if (!flag)
			{
				this.StopMovement();
				bool flag2 = this.totalCount <= 0;
				if (flag2)
				{
					this.itemTypeStart = 0;
					this.itemTypeEnd = 0;
					this.itemTypeSize = 0f;
					this.ReturnToTempPool(!this.reverseDirection, this.m_Content.childCount);
					this.ClearTempPool();
					base.UpdateBounds(true);
					this.UpdateScrollbars(Vector2.zero);
				}
				else
				{
					bool flag3 = this.totalCount > 0 && this.m_Content.childCount == 0;
					if (flag3)
					{
						base.RefillCells(this.itemTypeStart, 0f);
					}
					else
					{
						bool flag4 = this.totalCount > 0 && this.itemTypeStart >= this.totalCount;
						if (flag4)
						{
							this.RefillCellsFromEnd(0, false);
						}
						else
						{
							bool flag5 = this.itemTypeEnd > this.totalCount;
							if (flag5)
							{
								this.itemTypeEnd = this.totalCount;
							}
							for (int i = 0; i < this.m_Content.childCount; i++)
							{
								int itemIndex = this.itemTypeStart + i;
								bool flag6 = itemIndex < this.itemTypeEnd && itemIndex < this.totalCount;
								if (flag6)
								{
									this.ProvideData(this.m_Content.GetChild(i), itemIndex);
								}
							}
							for (int j = this.m_Content.childCount - 1; j >= 0; j--)
							{
								int itemIndex2 = this.itemTypeStart + j;
								bool flag7 = itemIndex2 >= this.totalCount;
								if (flag7)
								{
									this.prefabSource.ReturnObject(this.m_Content.GetChild(j));
								}
							}
							base.UpdateBounds(true);
							this.UpdateScrollbars(Vector2.zero);
						}
					}
				}
			}
		}

		// Token: 0x0600B7EC RID: 47084 RVA: 0x0053D69C File Offset: 0x0053B89C
		public void MoveToShowEdgeLine(RectTransform lineRectTransform)
		{
			Vector3[] viewportCorners = new Vector3[4];
			base.viewport.GetWorldCorners(viewportCorners);
			Vector3[] lineCorners = new Vector3[4];
			lineRectTransform.GetWorldCorners(lineCorners);
			float offset = 0f;
			bool flag = this.direction == LoopScrollRectBase.LoopScrollRectDirection.Vertical;
			if (flag)
			{
				float viewportTop = viewportCorners[2].y;
				float viewportBottom = viewportCorners[0].y;
				float lineTop = lineCorners[2].y;
				float lineBottom = lineCorners[0].y;
				bool flag2 = lineTop > viewportTop;
				if (flag2)
				{
					offset = viewportTop - lineTop;
				}
				else
				{
					bool flag3 = lineBottom < viewportBottom;
					if (flag3)
					{
						offset = viewportBottom - lineBottom;
					}
				}
				this.m_Content.position += new Vector3(0f, offset, 0f);
			}
			else
			{
				float viewportRight = viewportCorners[2].x;
				float viewportLeft = viewportCorners[0].x;
				float lineRight = lineCorners[2].x;
				float lineLeft = lineCorners[0].x;
				bool flag4 = lineRight > viewportRight;
				if (flag4)
				{
					offset = viewportRight - lineRight;
				}
				else
				{
					bool flag5 = lineLeft < viewportLeft;
					if (flag5)
					{
						offset = viewportLeft - lineLeft;
					}
				}
				this.m_Content.position += new Vector3(offset, 0f, 0f);
			}
		}

		// Token: 0x0600B7ED RID: 47085 RVA: 0x0053D7F8 File Offset: 0x0053B9F8
		public void RefillCellsAtNextPosition()
		{
			int offset = Math.Min(this.itemTypeStart, this.totalCount);
			Vector2 oldPos = this.m_Content.anchoredPosition;
			base.RefillCells(offset, 0f);
			Vector2 newPos = this.m_Content.anchoredPosition;
			bool flag = this.direction == LoopScrollRectBase.LoopScrollRectDirection.Vertical;
			if (flag)
			{
				newPos.y = oldPos.y;
			}
			else
			{
				newPos.x = oldPos.x;
			}
			this.m_Content.anchoredPosition = newPos;
			this.m_ContentStartPosition = newPos;
			this.ClearTempPool();
			this.UpdateScrollbars(Vector2.zero);
		}

		// Token: 0x0600B7EE RID: 47086 RVA: 0x0053D890 File Offset: 0x0053BA90
		public void RefillCellsAtLastPosition()
		{
			int offset = Math.Max(this.itemTypeStart - 1, 0);
			Vector2 oldPos = this.m_Content.anchoredPosition;
			base.RefillCells(offset, 0f);
			Vector2 newPos = this.m_Content.anchoredPosition;
			bool flag = this.direction == LoopScrollRectBase.LoopScrollRectDirection.Vertical;
			if (flag)
			{
				newPos.y = ((offset == 0) ? 0f : oldPos.y);
			}
			else
			{
				newPos.x = ((offset == 0) ? 0f : oldPos.x);
			}
			this.m_Content.anchoredPosition = newPos;
			this.m_ContentStartPosition = newPos;
			this.ClearTempPool();
			this.UpdateScrollbars(Vector2.zero);
		}

		// Token: 0x0600B7EF RID: 47087 RVA: 0x0053D938 File Offset: 0x0053BB38
		public override void OnDrag(PointerEventData eventData)
		{
			bool flag = !this.m_Dragging;
			if (!flag)
			{
				bool flag2 = eventData.button > PointerEventData.InputButton.Left;
				if (!flag2)
				{
					bool flag3 = !this.IsActive();
					if (!flag3)
					{
						this.NotifyScrollbarActivity();
						Vector2 localCursor;
						bool flag4 = !RectTransformUtility.ScreenPointToLocalPointInRectangle(base.viewRect, eventData.position, eventData.pressEventCamera, out localCursor);
						if (!flag4)
						{
							base.UpdateBounds(false);
							Vector2 pointerDelta = localCursor - this.m_PointerStartLocalCursor;
							Vector2 position = this.m_ContentStartPosition + pointerDelta;
							Vector2 offset = base.CalculateOffset(position - this.m_Content.anchoredPosition);
							position += offset;
							bool flag5 = base.movementType == LoopScrollRectBase.MovementType.Elastic;
							if (flag5)
							{
								bool flag6 = offset.x != 0f;
								if (flag6)
								{
									position.x -= LoopScrollRectBase.RubberDelta(offset.x, this.m_ViewBounds.size.x) * this.rubberScale;
								}
								bool flag7 = offset.y != 0f;
								if (flag7)
								{
									position.y -= LoopScrollRectBase.RubberDelta(offset.y, this.m_ViewBounds.size.y) * this.rubberScale;
								}
							}
							this.SetContentAnchoredPosition(position);
							Action onScrollEvent = this.OnScrollEvent;
							if (onScrollEvent != null)
							{
								onScrollEvent();
							}
						}
					}
				}
			}
		}

		// Token: 0x0600B7F0 RID: 47088 RVA: 0x0053DAAC File Offset: 0x0053BCAC
		public override void OnScroll(PointerEventData data)
		{
			base.OnScroll(data);
			Action onScrollEvent = this.OnScrollEvent;
			if (onScrollEvent != null)
			{
				onScrollEvent();
			}
			this.NotifyScrollbarActivity();
		}

		// Token: 0x0600B7F1 RID: 47089 RVA: 0x0053DAD0 File Offset: 0x0053BCD0
		protected override void LateUpdate()
		{
			base.LateUpdate();
			bool pendingBoundsUpdate = this._pendingBoundsUpdate;
			if (pendingBoundsUpdate)
			{
				this._pendingBoundsUpdate = false;
				base.UpdateBounds(true);
			}
			bool flag = base.verticalScrollbarVisibility != LoopScrollRectBase.ScrollbarVisibility.Permanent && base.verticalScrollbar != null;
			if (flag)
			{
				bool vScrollNeeded = this._shouldDisplayScrollbar || base.vScrollingNeeded;
				bool flag2 = base.verticalScrollbar.gameObject.activeSelf != vScrollNeeded;
				if (flag2)
				{
					base.verticalScrollbar.gameObject.SetActive(vScrollNeeded);
				}
			}
		}

		// Token: 0x0600B7F2 RID: 47090 RVA: 0x0053DB5C File Offset: 0x0053BD5C
		protected override void OnRectTransformDimensionsChange()
		{
			base.SetDirty();
			bool isActiveAndEnabled = base.isActiveAndEnabled;
			if (isActiveAndEnabled)
			{
				this._pendingBoundsUpdate = true;
			}
		}

		// Token: 0x0600B7F3 RID: 47091 RVA: 0x0053DB84 File Offset: 0x0053BD84
		protected override void UpdateScrollbars(Vector2 offset)
		{
			bool flag = base.horizontalScrollbar;
			if (flag)
			{
				bool automaticallyChangeScrollbarSize = this.m_AutomaticallyChangeScrollbarSize;
				if (automaticallyChangeScrollbarSize)
				{
					bool flag2 = this.m_ContentBounds.size.x > 0f && this.totalCount > 0;
					if (flag2)
					{
						float totalSize;
						float _;
						base.GetHorizonalOffsetAndSize(out totalSize, out _);
						float size = Mathf.Clamp01((this.m_ViewBounds.size.x - Mathf.Abs(offset.x)) / totalSize);
						base.horizontalScrollbar.size = size;
					}
					else
					{
						base.horizontalScrollbar.size = 1f;
					}
				}
				base.horizontalScrollbar.SetValueWithoutNotify(base.horizontalNormalizedPosition);
				CScrollbarLegacy cScrollBar = base.horizontalScrollbar as CScrollbarLegacy;
				bool flag3 = cScrollBar != null;
				if (flag3)
				{
					RectTransform handleMark = cScrollBar.HandleMark;
					RectTransform handleRect = cScrollBar.handleRect;
					bool flag4 = handleMark != null && handleRect != null;
					if (flag4)
					{
						handleMark.gameObject.SetActive(handleMark.rect.width * 2f < handleRect.rect.width);
					}
				}
			}
			bool flag5 = base.verticalScrollbar;
			if (flag5)
			{
				bool automaticallyChangeScrollbarSize2 = this.m_AutomaticallyChangeScrollbarSize;
				if (automaticallyChangeScrollbarSize2)
				{
					bool flag6 = this.m_ContentBounds.size.y > 0f && this.totalCount > 0;
					if (flag6)
					{
						float totalSize2;
						float _2;
						base.GetVerticalOffsetAndSize(out totalSize2, out _2);
						float size2 = Mathf.Clamp01((this.m_ViewBounds.size.y - Mathf.Abs(offset.y)) / totalSize2);
						base.verticalScrollbar.size = size2;
					}
					else
					{
						base.verticalScrollbar.size = 1f;
					}
				}
				base.verticalScrollbar.SetValueWithoutNotify(base.verticalNormalizedPosition);
				CScrollbarLegacy cScrollBar2 = base.verticalScrollbar as CScrollbarLegacy;
				bool flag7 = cScrollBar2 != null;
				if (flag7)
				{
					RectTransform handleMark2 = cScrollBar2.HandleMark;
					RectTransform handleRect2 = cScrollBar2.handleRect;
					bool flag8 = handleMark2 != null && handleRect2 != null;
					if (flag8)
					{
						handleMark2.gameObject.SetActive(handleMark2.rect.height * 2f < handleRect2.rect.height);
					}
				}
			}
		}

		// Token: 0x0600B7F4 RID: 47092 RVA: 0x0053DDF8 File Offset: 0x0053BFF8
		private void NotifyScrollbarActivity()
		{
			CScrollbar hScrollbar = base.horizontalScrollbar as CScrollbar;
			bool flag = hScrollbar != null;
			if (flag)
			{
				hScrollbar.NotifyScrollActivity();
			}
			CScrollbar vScrollbar = base.verticalScrollbar as CScrollbar;
			bool flag2 = vScrollbar != null;
			if (flag2)
			{
				vScrollbar.NotifyScrollActivity();
			}
		}

		// Token: 0x0600B7F5 RID: 47093 RVA: 0x0053DE3C File Offset: 0x0053C03C
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			this.NotifyViewportHover(true);
		}

		// Token: 0x0600B7F6 RID: 47094 RVA: 0x0053DE47 File Offset: 0x0053C047
		public virtual void OnPointerExit(PointerEventData eventData)
		{
			this.NotifyViewportHover(false);
		}

		// Token: 0x0600B7F7 RID: 47095 RVA: 0x0053DE54 File Offset: 0x0053C054
		private void NotifyViewportHover(bool isHovering)
		{
			CScrollbar hScrollbar = base.horizontalScrollbar as CScrollbar;
			bool flag = hScrollbar != null;
			if (flag)
			{
				hScrollbar.NotifyViewportHover(isHovering);
			}
			CScrollbar vScrollbar = base.verticalScrollbar as CScrollbar;
			bool flag2 = vScrollbar != null;
			if (flag2)
			{
				vScrollbar.NotifyViewportHover(isHovering);
			}
		}

		// Token: 0x04008EE0 RID: 36576
		[SerializeField]
		private GameObject templatePrefab;

		// Token: 0x04008EE1 RID: 36577
		private Action<Transform, int> dataSource;

		// Token: 0x04008EE3 RID: 36579
		[Tooltip("Rubber scale for outside")]
		public float rubberScale = 1f;

		// Token: 0x04008EE4 RID: 36580
		[SerializeField]
		private bool m_AutomaticallyChangeScrollbarSize;

		// Token: 0x04008EE5 RID: 36581
		private bool _shouldDisplayScrollbar = false;

		// Token: 0x04008EE6 RID: 36582
		private bool _pendingBoundsUpdate;
	}
}
