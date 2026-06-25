using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x02000FA9 RID: 4009
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public abstract class LoopScrollRectBase : UIBehaviour, IInitializePotentialDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutElement, ILayoutGroup, ILayoutController
	{
		// Token: 0x0600B7F9 RID: 47097
		protected abstract float GetSize(RectTransform item, bool includeSpacing = true);

		// Token: 0x0600B7FA RID: 47098
		protected abstract float GetDimension(Vector2 vector);

		// Token: 0x0600B7FB RID: 47099
		protected abstract float GetAbsDimension(Vector2 vector);

		// Token: 0x0600B7FC RID: 47100
		protected abstract Vector2 GetVector(float value);

		// Token: 0x170014B6 RID: 5302
		// (get) Token: 0x0600B7FD RID: 47101 RVA: 0x0053DEB8 File Offset: 0x0053C0B8
		protected float contentSpacing
		{
			get
			{
				bool contentSpaceInit = this.m_ContentSpaceInit;
				float contentSpacing;
				if (contentSpaceInit)
				{
					contentSpacing = this.m_ContentSpacing;
				}
				else
				{
					this.m_ContentSpaceInit = true;
					this.m_ContentSpacing = 0f;
					bool flag = this.m_Content != null;
					if (flag)
					{
						HorizontalOrVerticalLayoutGroup layout = this.m_Content.GetComponent<HorizontalOrVerticalLayoutGroup>();
						bool flag2 = layout != null;
						if (flag2)
						{
							this.m_ContentSpacing = layout.spacing;
							this.m_ContentLeftPadding = (float)layout.padding.left;
							this.m_ContentRightPadding = (float)layout.padding.right;
							this.m_ContentTopPadding = (float)layout.padding.top;
							this.m_ContentBottomPadding = (float)layout.padding.bottom;
						}
						this.m_GridLayout = this.m_Content.GetComponent<GridLayoutGroup>();
						bool flag3 = this.m_GridLayout != null;
						if (flag3)
						{
							this.m_ContentSpacing = this.GetAbsDimension(this.m_GridLayout.spacing);
							this.m_ContentLeftPadding = (float)this.m_GridLayout.padding.left;
							this.m_ContentRightPadding = (float)this.m_GridLayout.padding.right;
							this.m_ContentTopPadding = (float)this.m_GridLayout.padding.top;
							this.m_ContentBottomPadding = (float)this.m_GridLayout.padding.bottom;
						}
					}
					contentSpacing = this.m_ContentSpacing;
				}
				return contentSpacing;
			}
		}

		// Token: 0x170014B7 RID: 5303
		// (get) Token: 0x0600B7FE RID: 47102 RVA: 0x0053E018 File Offset: 0x0053C218
		protected int contentConstraintCount
		{
			get
			{
				bool contentConstraintCountInit = this.m_ContentConstraintCountInit;
				int contentConstraintCount;
				if (contentConstraintCountInit)
				{
					contentConstraintCount = this.m_ContentConstraintCount;
				}
				else
				{
					this.m_ContentConstraintCountInit = true;
					this.m_ContentConstraintCount = 1;
					bool flag = this.m_Content != null;
					if (flag)
					{
						GridLayoutGroup layout2 = this.m_Content.GetComponent<GridLayoutGroup>();
						bool flag2 = layout2 != null;
						if (flag2)
						{
							bool flag3 = layout2.constraint == GridLayoutGroup.Constraint.Flexible;
							if (flag3)
							{
								Debug.LogWarning("[LoopScrollRect] Flexible not supported yet");
							}
							this.m_ContentConstraintCount = layout2.constraintCount;
						}
					}
					contentConstraintCount = this.m_ContentConstraintCount;
				}
				return contentConstraintCount;
			}
		}

		// Token: 0x170014B8 RID: 5304
		// (get) Token: 0x0600B7FF RID: 47103 RVA: 0x0053E0A8 File Offset: 0x0053C2A8
		protected int StartLine
		{
			get
			{
				return Mathf.CeilToInt((float)this.itemTypeStart / (float)this.contentConstraintCount);
			}
		}

		// Token: 0x170014B9 RID: 5305
		// (get) Token: 0x0600B800 RID: 47104 RVA: 0x0053E0D0 File Offset: 0x0053C2D0
		protected int CurrentLines
		{
			get
			{
				return Mathf.CeilToInt((float)(this.itemTypeEnd - this.itemTypeStart) / (float)this.contentConstraintCount);
			}
		}

		// Token: 0x170014BA RID: 5306
		// (get) Token: 0x0600B801 RID: 47105 RVA: 0x0053E100 File Offset: 0x0053C300
		protected int TotalLines
		{
			get
			{
				return Mathf.CeilToInt((float)this.totalCount / (float)this.contentConstraintCount);
			}
		}

		// Token: 0x0600B802 RID: 47106 RVA: 0x0053E128 File Offset: 0x0053C328
		protected virtual bool UpdateItems(ref Bounds viewBounds, ref Bounds contentBounds)
		{
			return false;
		}

		// Token: 0x170014BB RID: 5307
		// (get) Token: 0x0600B803 RID: 47107 RVA: 0x0053E13C File Offset: 0x0053C33C
		// (set) Token: 0x0600B804 RID: 47108 RVA: 0x0053E154 File Offset: 0x0053C354
		public RectTransform content
		{
			get
			{
				return this.m_Content;
			}
			set
			{
				this.m_Content = value;
			}
		}

		// Token: 0x170014BC RID: 5308
		// (get) Token: 0x0600B805 RID: 47109 RVA: 0x0053E160 File Offset: 0x0053C360
		// (set) Token: 0x0600B806 RID: 47110 RVA: 0x0053E178 File Offset: 0x0053C378
		public bool horizontal
		{
			get
			{
				return this.m_Horizontal;
			}
			set
			{
				this.m_Horizontal = value;
			}
		}

		// Token: 0x170014BD RID: 5309
		// (get) Token: 0x0600B807 RID: 47111 RVA: 0x0053E184 File Offset: 0x0053C384
		// (set) Token: 0x0600B808 RID: 47112 RVA: 0x0053E19C File Offset: 0x0053C39C
		public bool vertical
		{
			get
			{
				return this.m_Vertical;
			}
			set
			{
				this.m_Vertical = value;
			}
		}

		// Token: 0x170014BE RID: 5310
		// (get) Token: 0x0600B809 RID: 47113 RVA: 0x0053E1A8 File Offset: 0x0053C3A8
		// (set) Token: 0x0600B80A RID: 47114 RVA: 0x0053E1C0 File Offset: 0x0053C3C0
		public LoopScrollRectBase.MovementType movementType
		{
			get
			{
				return this.m_MovementType;
			}
			set
			{
				this.m_MovementType = value;
			}
		}

		// Token: 0x170014BF RID: 5311
		// (get) Token: 0x0600B80B RID: 47115 RVA: 0x0053E1CC File Offset: 0x0053C3CC
		// (set) Token: 0x0600B80C RID: 47116 RVA: 0x0053E1E4 File Offset: 0x0053C3E4
		public float elasticity
		{
			get
			{
				return this.m_Elasticity;
			}
			set
			{
				this.m_Elasticity = value;
			}
		}

		// Token: 0x170014C0 RID: 5312
		// (get) Token: 0x0600B80D RID: 47117 RVA: 0x0053E1F0 File Offset: 0x0053C3F0
		// (set) Token: 0x0600B80E RID: 47118 RVA: 0x0053E208 File Offset: 0x0053C408
		public bool inertia
		{
			get
			{
				return this.m_Inertia;
			}
			set
			{
				this.m_Inertia = value;
			}
		}

		// Token: 0x170014C1 RID: 5313
		// (get) Token: 0x0600B80F RID: 47119 RVA: 0x0053E214 File Offset: 0x0053C414
		// (set) Token: 0x0600B810 RID: 47120 RVA: 0x0053E22C File Offset: 0x0053C42C
		public float decelerationRate
		{
			get
			{
				return this.m_DecelerationRate;
			}
			set
			{
				this.m_DecelerationRate = value;
			}
		}

		// Token: 0x170014C2 RID: 5314
		// (get) Token: 0x0600B811 RID: 47121 RVA: 0x0053E238 File Offset: 0x0053C438
		// (set) Token: 0x0600B812 RID: 47122 RVA: 0x0053E250 File Offset: 0x0053C450
		public float scrollSensitivity
		{
			get
			{
				return this.m_ScrollSensitivity;
			}
			set
			{
				this.m_ScrollSensitivity = value;
			}
		}

		// Token: 0x170014C3 RID: 5315
		// (get) Token: 0x0600B813 RID: 47123 RVA: 0x0053E25C File Offset: 0x0053C45C
		// (set) Token: 0x0600B814 RID: 47124 RVA: 0x0053E274 File Offset: 0x0053C474
		public RectTransform viewport
		{
			get
			{
				return this.m_Viewport;
			}
			set
			{
				this.m_Viewport = value;
				this.SetDirtyCaching();
			}
		}

		// Token: 0x170014C4 RID: 5316
		// (get) Token: 0x0600B815 RID: 47125 RVA: 0x0053E288 File Offset: 0x0053C488
		// (set) Token: 0x0600B816 RID: 47126 RVA: 0x0053E2A0 File Offset: 0x0053C4A0
		public Scrollbar horizontalScrollbar
		{
			get
			{
				return this.m_HorizontalScrollbar;
			}
			set
			{
				bool flag = this.m_HorizontalScrollbar;
				if (flag)
				{
					this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
				}
				this.m_HorizontalScrollbar = value;
				bool flag2 = this.m_HorizontalScrollbar;
				if (flag2)
				{
					this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
				}
				this.SetDirtyCaching();
			}
		}

		// Token: 0x170014C5 RID: 5317
		// (get) Token: 0x0600B817 RID: 47127 RVA: 0x0053E314 File Offset: 0x0053C514
		// (set) Token: 0x0600B818 RID: 47128 RVA: 0x0053E32C File Offset: 0x0053C52C
		public Scrollbar verticalScrollbar
		{
			get
			{
				return this.m_VerticalScrollbar;
			}
			set
			{
				bool flag = this.m_VerticalScrollbar;
				if (flag)
				{
					this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
				}
				this.m_VerticalScrollbar = value;
				bool flag2 = this.m_VerticalScrollbar;
				if (flag2)
				{
					this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
				}
				this.SetDirtyCaching();
			}
		}

		// Token: 0x170014C6 RID: 5318
		// (get) Token: 0x0600B819 RID: 47129 RVA: 0x0053E3A0 File Offset: 0x0053C5A0
		// (set) Token: 0x0600B81A RID: 47130 RVA: 0x0053E3B8 File Offset: 0x0053C5B8
		public LoopScrollRectBase.ScrollbarVisibility horizontalScrollbarVisibility
		{
			get
			{
				return this.m_HorizontalScrollbarVisibility;
			}
			set
			{
				this.m_HorizontalScrollbarVisibility = value;
				this.SetDirtyCaching();
			}
		}

		// Token: 0x170014C7 RID: 5319
		// (get) Token: 0x0600B81B RID: 47131 RVA: 0x0053E3CC File Offset: 0x0053C5CC
		// (set) Token: 0x0600B81C RID: 47132 RVA: 0x0053E3E4 File Offset: 0x0053C5E4
		public LoopScrollRectBase.ScrollbarVisibility verticalScrollbarVisibility
		{
			get
			{
				return this.m_VerticalScrollbarVisibility;
			}
			set
			{
				this.m_VerticalScrollbarVisibility = value;
				this.SetDirtyCaching();
			}
		}

		// Token: 0x170014C8 RID: 5320
		// (get) Token: 0x0600B81D RID: 47133 RVA: 0x0053E3F8 File Offset: 0x0053C5F8
		// (set) Token: 0x0600B81E RID: 47134 RVA: 0x0053E410 File Offset: 0x0053C610
		public float horizontalScrollbarSpacing
		{
			get
			{
				return this.m_HorizontalScrollbarSpacing;
			}
			set
			{
				this.m_HorizontalScrollbarSpacing = value;
				this.SetDirty();
			}
		}

		// Token: 0x170014C9 RID: 5321
		// (get) Token: 0x0600B81F RID: 47135 RVA: 0x0053E424 File Offset: 0x0053C624
		// (set) Token: 0x0600B820 RID: 47136 RVA: 0x0053E43C File Offset: 0x0053C63C
		public float verticalScrollbarSpacing
		{
			get
			{
				return this.m_VerticalScrollbarSpacing;
			}
			set
			{
				this.m_VerticalScrollbarSpacing = value;
				this.SetDirty();
			}
		}

		// Token: 0x170014CA RID: 5322
		// (get) Token: 0x0600B821 RID: 47137 RVA: 0x0053E450 File Offset: 0x0053C650
		// (set) Token: 0x0600B822 RID: 47138 RVA: 0x0053E468 File Offset: 0x0053C668
		public LoopScrollRectBase.ScrollRectEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		// Token: 0x170014CB RID: 5323
		// (get) Token: 0x0600B823 RID: 47139 RVA: 0x0053E472 File Offset: 0x0053C672
		// (set) Token: 0x0600B824 RID: 47140 RVA: 0x0053E47A File Offset: 0x0053C67A
		public float fixedHorizontalScrollbarSize
		{
			get
			{
				return this.m_FixedHorizontalScrollbarSize;
			}
			set
			{
				this.m_FixedHorizontalScrollbarSize = Mathf.Clamp01(value);
				this.SetDirty();
			}
		}

		// Token: 0x170014CC RID: 5324
		// (get) Token: 0x0600B825 RID: 47141 RVA: 0x0053E490 File Offset: 0x0053C690
		// (set) Token: 0x0600B826 RID: 47142 RVA: 0x0053E498 File Offset: 0x0053C698
		public float fixedVerticalScrollbarSize
		{
			get
			{
				return this.m_FixedVerticalScrollbarSize;
			}
			set
			{
				this.m_FixedVerticalScrollbarSize = Mathf.Clamp01(value);
				this.SetDirty();
			}
		}

		// Token: 0x170014CD RID: 5325
		// (get) Token: 0x0600B827 RID: 47143 RVA: 0x0053E4B0 File Offset: 0x0053C6B0
		protected RectTransform viewRect
		{
			get
			{
				bool flag = this.m_ViewRect == null;
				if (flag)
				{
					this.m_ViewRect = this.m_Viewport;
				}
				bool flag2 = this.m_ViewRect == null;
				if (flag2)
				{
					this.m_ViewRect = (RectTransform)base.transform;
				}
				return this.m_ViewRect;
			}
		}

		// Token: 0x170014CE RID: 5326
		// (get) Token: 0x0600B828 RID: 47144 RVA: 0x0053E508 File Offset: 0x0053C708
		// (set) Token: 0x0600B829 RID: 47145 RVA: 0x0053E520 File Offset: 0x0053C720
		public Vector2 velocity
		{
			get
			{
				return this.m_Velocity;
			}
			set
			{
				this.m_Velocity = value;
			}
		}

		// Token: 0x170014CF RID: 5327
		// (get) Token: 0x0600B82A RID: 47146 RVA: 0x0053E52C File Offset: 0x0053C72C
		private RectTransform rectTransform
		{
			get
			{
				bool flag = this.m_Rect == null;
				if (flag)
				{
					this.m_Rect = base.GetComponent<RectTransform>();
				}
				return this.m_Rect;
			}
		}

		// Token: 0x0600B82C RID: 47148 RVA: 0x0053E6B0 File Offset: 0x0053C8B0
		protected override void Awake()
		{
			base.Awake();
			bool isPlaying = Application.isPlaying;
			if (isPlaying)
			{
				float value = (float)((this.reverseDirection ^ this.direction == LoopScrollRectBase.LoopScrollRectDirection.Horizontal) ? 0 : 1);
				bool flag = this.m_Content != null;
				if (flag)
				{
					Debug.Assert(this.GetAbsDimension(this.m_Content.pivot) == value, this);
					Debug.Assert(this.GetAbsDimension(this.m_Content.anchorMin) == value, this);
					Debug.Assert(this.GetAbsDimension(this.m_Content.anchorMax) == value, this);
				}
				bool flag2 = this.direction == LoopScrollRectBase.LoopScrollRectDirection.Vertical;
				if (flag2)
				{
					Debug.Assert(this.m_Vertical && !this.m_Horizontal, this);
				}
				else
				{
					Debug.Assert(!this.m_Vertical && this.m_Horizontal, this);
				}
			}
		}

		// Token: 0x0600B82D RID: 47149 RVA: 0x0053E790 File Offset: 0x0053C990
		public void ClearCells()
		{
			bool isPlaying = Application.isPlaying;
			if (isPlaying)
			{
				this.itemTypeStart = 0;
				this.itemTypeEnd = 0;
				this.itemTypeSize = 0f;
				this.totalCount = 0;
				for (int i = this.m_Content.childCount - 1; i >= 0; i--)
				{
					this.prefabSource.ReturnObject(this.m_Content.GetChild(i));
				}
			}
		}

		// Token: 0x0600B82E RID: 47150 RVA: 0x0053E804 File Offset: 0x0053CA04
		public int GetFirstItem(out float offset)
		{
			bool flag = this.direction == LoopScrollRectBase.LoopScrollRectDirection.Vertical;
			if (flag)
			{
				offset = this.m_ContentBounds.max.y - this.m_ViewBounds.max.y;
			}
			else
			{
				offset = this.m_ViewBounds.min.x - this.m_ContentBounds.min.x;
			}
			int idx = 0;
			bool flag2 = this.itemTypeEnd > this.itemTypeStart;
			if (flag2)
			{
				float size = this.GetSize(this.m_Content.GetChild(0) as RectTransform, true);
				while (offset - size >= 0f && this.itemTypeStart + idx + this.contentConstraintCount < this.itemTypeEnd)
				{
					offset -= size;
					idx += this.contentConstraintCount;
					size = this.GetSize(this.m_Content.GetChild(idx) as RectTransform, true);
				}
			}
			return idx + this.itemTypeStart;
		}

		// Token: 0x0600B82F RID: 47151 RVA: 0x0053E904 File Offset: 0x0053CB04
		public int GetLastItem(out float offset)
		{
			bool flag = this.direction == LoopScrollRectBase.LoopScrollRectDirection.Vertical;
			if (flag)
			{
				offset = this.m_ViewBounds.min.y - this.m_ContentBounds.min.y;
			}
			else
			{
				offset = this.m_ContentBounds.max.x - this.m_ViewBounds.max.x;
			}
			int idx = 0;
			bool flag2 = this.itemTypeEnd > this.itemTypeStart;
			if (flag2)
			{
				int totalChildCount = this.m_Content.childCount;
				float size = this.GetSize(this.m_Content.GetChild(totalChildCount - idx - 1) as RectTransform, true);
				while (offset - size >= 0f && this.itemTypeStart < this.itemTypeEnd - idx - this.contentConstraintCount)
				{
					offset -= size;
					idx += this.contentConstraintCount;
					size = this.GetSize(this.m_Content.GetChild(totalChildCount - idx - 1) as RectTransform, true);
				}
			}
			int item = this.itemTypeEnd - 1 - idx;
			bool flag3 = this.totalCount >= 0 && idx > 0 && item % this.contentConstraintCount != 0;
			if (flag3)
			{
				item = item / this.contentConstraintCount * this.contentConstraintCount;
			}
			return item;
		}

		// Token: 0x0600B830 RID: 47152 RVA: 0x0053EA50 File Offset: 0x0053CC50
		public void ScrollToCell(int index, float speed, float offset = 0f, LoopScrollRectBase.ScrollMode mode = LoopScrollRectBase.ScrollMode.ToStart)
		{
			bool flag = this.totalCount >= 0 && (index < 0 || index >= this.totalCount);
			if (flag)
			{
				Debug.LogErrorFormat("invalid index {0}", new object[]
				{
					index
				});
			}
			else
			{
				bool flag2 = speed <= 0f;
				if (flag2)
				{
					Debug.LogErrorFormat("invalid speed {0}", new object[]
					{
						index
					});
				}
				else
				{
					base.StopAllCoroutines();
					base.StartCoroutine(this.ScrollToCellCoroutine(index, speed, offset, mode));
				}
			}
		}

		// Token: 0x0600B831 RID: 47153 RVA: 0x0053EAE4 File Offset: 0x0053CCE4
		public void ScrollToCellWithinTime(int index, float time, float offset = 0f, LoopScrollRectBase.ScrollMode mode = LoopScrollRectBase.ScrollMode.ToStart)
		{
			bool flag = this.totalCount >= 0 && (index < 0 || index >= this.totalCount);
			if (flag)
			{
				Debug.LogErrorFormat("invalid index {0}", new object[]
				{
					index
				});
			}
			else
			{
				bool flag2 = time <= 0f;
				if (flag2)
				{
					Debug.LogErrorFormat("invalid time {0}", new object[]
					{
						time
					});
				}
				else
				{
					bool flag3 = mode == LoopScrollRectBase.ScrollMode.JustAppear;
					if (flag3)
					{
						Debug.LogErrorFormat("scroll mode {0} not supported yet.", new object[]
						{
							mode
						});
					}
					else
					{
						base.StopAllCoroutines();
						float dist = 0f;
						float currentOffset = 0f;
						int currentFirst = this.reverseDirection ? this.GetLastItem(out currentOffset) : this.GetFirstItem(out currentOffset);
						int TargetLine = index / this.contentConstraintCount;
						int CurrentLine = currentFirst / this.contentConstraintCount;
						bool flag4 = this.sizeHelper != null;
						if (flag4)
						{
							int delta = this.reverseDirection ? 1 : 0;
							bool flag5 = TargetLine > CurrentLine;
							if (flag5)
							{
								dist = this.sizeHelper.GetItemsSize(currentFirst + delta, index + delta) + this.contentSpacing * (float)(TargetLine - CurrentLine);
							}
							else
							{
								bool flag6 = TargetLine < CurrentLine;
								if (flag6)
								{
									dist = -this.sizeHelper.GetItemsSize(index + delta, currentFirst + delta) + this.contentSpacing * (float)(TargetLine - CurrentLine);
								}
							}
						}
						else
						{
							float elementSize = this.EstimiateElementSize();
							dist = elementSize * (float)(TargetLine - CurrentLine) + this.contentSpacing * (float)(TargetLine - CurrentLine);
						}
						dist += (this.reverseDirection ? currentOffset : (-currentOffset));
						bool flag7 = mode == LoopScrollRectBase.ScrollMode.ToCenter;
						if (flag7)
						{
							float sizeToFill = this.GetAbsDimension(this.viewRect.rect.size);
							bool flag8 = this.sizeHelper != null;
							float elementSize2;
							if (flag8)
							{
								elementSize2 = this.sizeHelper.GetItemsSize(index, index + 1);
							}
							else
							{
								elementSize2 = this.EstimiateElementSize();
							}
							float centerOffset = (sizeToFill - elementSize2) * 0.5f;
							dist += (this.reverseDirection ? centerOffset : (-centerOffset));
						}
						bool flag9 = this.direction == LoopScrollRectBase.LoopScrollRectDirection.Horizontal;
						if (flag9)
						{
							dist = -dist;
						}
						dist += offset;
						base.StartCoroutine(this.ScrollToCellCoroutine(index, Mathf.Abs(dist) / time, offset, mode));
					}
				}
			}
		}

		// Token: 0x0600B832 RID: 47154 RVA: 0x0053ED2D File Offset: 0x0053CF2D
		private IEnumerator ScrollToCellCoroutine(int index, float speed, float offset, LoopScrollRectBase.ScrollMode mode)
		{
			bool needMoving = true;
			while (needMoving)
			{
				yield return null;
				bool flag = !this.m_Dragging;
				if (flag)
				{
					float move = 0f;
					bool flag2 = index < this.itemTypeStart;
					if (flag2)
					{
						move = -Time.deltaTime * speed;
					}
					else
					{
						bool flag3 = index >= this.itemTypeEnd;
						if (flag3)
						{
							move = Time.deltaTime * speed;
						}
						else
						{
							this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
							Bounds m_ItemBounds = this.GetBounds4Item(index);
							float delta = 0f;
							bool flag4 = mode == LoopScrollRectBase.ScrollMode.ToStart;
							if (flag4)
							{
								bool flag5 = this.direction == LoopScrollRectBase.LoopScrollRectDirection.Vertical;
								if (flag5)
								{
									delta = (this.reverseDirection ? (this.m_ViewBounds.min.y - m_ItemBounds.min.y) : (this.m_ViewBounds.max.y - m_ItemBounds.max.y));
								}
								else
								{
									delta = (this.reverseDirection ? (m_ItemBounds.max.x - this.m_ViewBounds.max.x) : (m_ItemBounds.min.x - this.m_ViewBounds.min.x));
								}
							}
							else
							{
								bool flag6 = mode == LoopScrollRectBase.ScrollMode.ToCenter;
								if (flag6)
								{
									delta = this.GetDimension(this.m_ViewBounds.center - m_ItemBounds.center);
								}
								else
								{
									float min_delta = this.GetDimension(this.m_ViewBounds.min - m_ItemBounds.min);
									float max_delta = this.GetDimension(this.m_ViewBounds.max - m_ItemBounds.max);
									bool flag7 = this.direction == LoopScrollRectBase.LoopScrollRectDirection.Vertical;
									if (flag7)
									{
										bool flag8 = min_delta > 0f;
										if (flag8)
										{
											delta = min_delta;
										}
										else
										{
											bool flag9 = max_delta < 0f;
											if (flag9)
											{
												delta = max_delta;
											}
										}
									}
									else
									{
										bool flag10 = min_delta < 0f;
										if (flag10)
										{
											delta = min_delta;
										}
										else
										{
											bool flag11 = max_delta > 0f;
											if (flag11)
											{
												delta = max_delta;
											}
										}
									}
								}
							}
							delta += offset;
							bool flag12 = this.totalCount >= 0;
							if (flag12)
							{
								bool flag13 = delta > 0f && this.itemTypeEnd == this.totalCount;
								if (flag13)
								{
									m_ItemBounds = this.GetBounds4Item(this.totalCount - 1);
									bool flag14 = (this.direction == LoopScrollRectBase.LoopScrollRectDirection.Vertical && m_ItemBounds.min.y > this.m_ViewBounds.min.y) || (this.direction == LoopScrollRectBase.LoopScrollRectDirection.Horizontal && m_ItemBounds.max.x < this.m_ViewBounds.max.x);
									if (flag14)
									{
										needMoving = false;
										break;
									}
								}
								else
								{
									bool flag15 = delta < 0f && this.itemTypeStart == 0;
									if (flag15)
									{
										m_ItemBounds = this.GetBounds4Item(0);
										bool flag16 = (this.direction == LoopScrollRectBase.LoopScrollRectDirection.Vertical && m_ItemBounds.max.y < this.m_ViewBounds.max.y) || (this.direction == LoopScrollRectBase.LoopScrollRectDirection.Horizontal && m_ItemBounds.min.x > this.m_ViewBounds.min.x);
										if (flag16)
										{
											needMoving = false;
											break;
										}
									}
								}
							}
							float maxMove = Mathf.Max(Time.deltaTime * speed, 0.001f);
							bool flag17 = Mathf.Abs(delta) < maxMove;
							if (flag17)
							{
								needMoving = false;
								move = delta;
							}
							else
							{
								move = Mathf.Sign(delta) * maxMove;
							}
							m_ItemBounds = default(Bounds);
						}
					}
					bool flag18 = move != 0f;
					if (flag18)
					{
						Vector2 delta2 = this.GetVector(move);
						this.m_Content.anchoredPosition += delta2;
						this.m_PrevPosition += delta2;
						this.m_ContentStartPosition += delta2;
						this.UpdateBounds(true);
						delta2 = default(Vector2);
					}
				}
			}
			this.StopMovement();
			this.UpdatePrevData();
			yield break;
		}

		// Token: 0x0600B833 RID: 47155
		protected abstract void ProvideData(Transform transform, int index);

		// Token: 0x0600B834 RID: 47156 RVA: 0x0053ED5C File Offset: 0x0053CF5C
		public void RefreshCells()
		{
			bool flag = Application.isPlaying && base.isActiveAndEnabled;
			if (flag)
			{
				this.itemTypeEnd = this.itemTypeStart;
				this.itemTypeSize = 0f;
				for (int i = 0; i < this.m_Content.childCount; i++)
				{
					bool flag2 = this.itemTypeEnd < this.totalCount || this.totalCount < 0;
					if (flag2)
					{
						this.ProvideData(this.m_Content.GetChild(i), this.itemTypeEnd);
						bool flag3 = this.itemTypeEnd % this.contentConstraintCount == 0;
						if (flag3)
						{
							this.itemTypeSize += this.GetSize(this.m_Content.GetChild(i).GetComponent<RectTransform>(), true);
						}
						this.itemTypeEnd++;
					}
					else
					{
						this.prefabSource.ReturnObject(this.m_Content.GetChild(i));
						i--;
					}
				}
				this.UpdateBounds(true);
				this.UpdateScrollbars(Vector2.zero);
			}
		}

		// Token: 0x0600B835 RID: 47157 RVA: 0x0053EE74 File Offset: 0x0053D074
		public void RefillCellsFromEnd(int endItem = 0, float contentOffset = 0f)
		{
			bool flag = !Application.isPlaying;
			if (!flag)
			{
				this.itemTypeEnd = (this.reverseDirection ? endItem : (this.totalCount - endItem));
				this.itemTypeStart = this.itemTypeEnd;
				this.itemTypeSize = 0f;
				bool flag2 = this.totalCount >= 0 && this.itemTypeStart % this.contentConstraintCount != 0;
				if (flag2)
				{
					this.itemTypeStart = this.itemTypeStart / this.contentConstraintCount * this.contentConstraintCount;
				}
				this.ReturnToTempPool(!this.reverseDirection, this.m_Content.childCount);
				this.EnsureLayoutHasRebuilt();
				float sizeToFill = this.GetAbsDimension(this.viewRect.rect.size) + contentOffset;
				float sizeFilled = 0f;
				bool flag3 = this.itemTypeStart < this.itemTypeEnd;
				if (flag3)
				{
					this.itemTypeEnd = this.itemTypeStart;
					float size = this.NewItemAtEnd();
					bool flag4 = size >= 0f;
					if (flag4)
					{
						sizeFilled += size;
					}
				}
				while (sizeToFill > sizeFilled)
				{
					float size2 = this.NewItemAtStart();
					bool flag5 = size2 < 0f;
					if (flag5)
					{
						break;
					}
					sizeFilled += size2;
				}
				float sizeFilledAtStart = sizeFilled;
				while (sizeToFill > sizeFilled)
				{
					float size3 = this.NewItemAtEnd();
					bool flag6 = size3 < 0f;
					if (flag6)
					{
						break;
					}
					sizeFilled += size3;
				}
				float sizeFilledAtEnd = sizeFilled - sizeFilledAtStart;
				Vector2 pos = this.m_Content.anchoredPosition;
				float padding_dist = this.GetAbsDimension(new Vector2(this.m_ContentLeftPadding + this.m_ContentRightPadding, this.m_ContentTopPadding + this.m_ContentBottomPadding));
				bool flag7 = this.reverseDirection;
				float offset;
				if (flag7)
				{
					offset = Mathf.Max(0f, sizeFilledAtEnd + padding_dist - sizeToFill) + contentOffset;
				}
				else
				{
					offset = Mathf.Max(0f, sizeFilledAtStart + padding_dist - sizeToFill);
				}
				bool flag8 = this.direction == LoopScrollRectBase.LoopScrollRectDirection.Vertical;
				if (flag8)
				{
					pos.y = (this.reverseDirection ? (-offset) : offset);
				}
				else
				{
					pos.x = (this.reverseDirection ? offset : (-offset));
				}
				this.m_Content.anchoredPosition = pos;
				this.m_ContentStartPosition = pos;
				this.ClearTempPool();
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.m_Content);
				Canvas.ForceUpdateCanvases();
				this.UpdateBounds(true);
				this.UpdateScrollbars(Vector2.zero);
				this.StopMovement();
				this.UpdatePrevData();
			}
		}

		// Token: 0x0600B836 RID: 47158 RVA: 0x0053F0E8 File Offset: 0x0053D2E8
		public void RefillCells(int startItem = 0, float contentOffset = 0f)
		{
			bool flag = !Application.isPlaying;
			if (!flag)
			{
				this.itemTypeStart = (this.reverseDirection ? (this.totalCount - startItem) : startItem);
				bool flag2 = this.totalCount >= 0 && this.itemTypeStart % this.contentConstraintCount != 0;
				if (flag2)
				{
					this.itemTypeStart = this.itemTypeStart / this.contentConstraintCount * this.contentConstraintCount;
				}
				this.itemTypeEnd = this.itemTypeStart;
				this.itemTypeSize = 0f;
				this.ReturnToTempPool(this.reverseDirection, this.m_Content.childCount);
				this.EnsureLayoutHasRebuilt();
				float sizeToFill = this.GetAbsDimension(this.viewRect.rect.size) + contentOffset;
				float sizeFilled = 0f;
				while (sizeToFill > sizeFilled)
				{
					float size = this.NewItemAtEnd();
					bool flag3 = size < 0f;
					if (flag3)
					{
						break;
					}
					sizeFilled += size;
				}
				float sizeFilledAtEnd = sizeFilled;
				while (sizeToFill > sizeFilled)
				{
					float size2 = this.NewItemAtStart();
					bool flag4 = size2 < 0f;
					if (flag4)
					{
						break;
					}
					sizeFilled += size2;
				}
				float sizeFilledAtStart = sizeFilled - sizeFilledAtEnd;
				Vector2 pos = this.m_Content.anchoredPosition;
				float padding_dist = this.GetAbsDimension(new Vector2(this.m_ContentLeftPadding + this.m_ContentRightPadding, this.m_ContentTopPadding + this.m_ContentBottomPadding));
				bool flag5 = this.reverseDirection;
				float offset;
				if (flag5)
				{
					offset = Mathf.Max(0f, sizeFilledAtEnd + padding_dist - sizeToFill);
				}
				else
				{
					offset = sizeFilledAtStart + contentOffset;
				}
				bool flag6 = this.direction == LoopScrollRectBase.LoopScrollRectDirection.Vertical;
				if (flag6)
				{
					pos.y = (this.reverseDirection ? (-offset) : offset);
				}
				else
				{
					pos.x = (this.reverseDirection ? offset : (-offset));
				}
				this.m_Content.anchoredPosition = pos;
				this.m_ContentStartPosition = pos;
				this.ClearTempPool();
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.m_Content);
				Canvas.ForceUpdateCanvases();
				this.UpdateBounds(true);
				this.UpdateScrollbars(Vector2.zero);
				this.StopMovement();
				this.UpdatePrevData();
			}
		}

		// Token: 0x0600B837 RID: 47159 RVA: 0x0053F308 File Offset: 0x0053D508
		protected float NewItemAtStart()
		{
			bool flag = this.totalCount >= 0 && this.itemTypeStart - this.contentConstraintCount < 0;
			float result;
			if (flag)
			{
				result = -1f;
			}
			else
			{
				bool includeSpacing = this.CurrentLines > 0;
				float size = 0f;
				for (int i = 0; i < this.contentConstraintCount; i++)
				{
					this.itemTypeStart--;
					RectTransform newItem = this.GetFromTempPool(this.itemTypeStart);
					newItem.SetSiblingIndex(this.deletedItemTypeStart);
					size = Mathf.Max(this.GetSize(newItem, includeSpacing), size);
				}
				this.threshold = Mathf.Max(this.threshold, size * 1.5f);
				bool flag2 = size > 0f;
				if (flag2)
				{
					this.SetDirtyCaching();
					this.m_HasRebuiltLayout = false;
					this.m_PrevPositionValid = true;
					bool flag3 = !this.reverseDirection;
					if (flag3)
					{
						Vector2 offset = this.GetVector(size);
						this.m_Content.anchoredPosition += offset;
						this.m_PrevPosition += offset;
						this.m_ContentStartPosition += offset;
					}
					this.itemTypeSize += size;
				}
				result = size;
			}
			return result;
		}

		// Token: 0x0600B838 RID: 47160 RVA: 0x0053F454 File Offset: 0x0053D654
		protected float DeleteItemAtStart()
		{
			bool flag = (this.m_Dragging || this.m_Velocity != Vector2.zero) && this.totalCount >= 0 && this.itemTypeEnd >= this.totalCount - this.contentConstraintCount;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				int availableChilds = this.m_Content.childCount - this.deletedItemTypeStart - this.deletedItemTypeEnd;
				Debug.Assert(availableChilds >= 0);
				bool flag2 = availableChilds == 0;
				if (flag2)
				{
					result = 0f;
				}
				else
				{
					bool includeSpacing = this.CurrentLines > 1;
					float size = 0f;
					for (int i = 0; i < this.contentConstraintCount; i++)
					{
						RectTransform oldItem = this.m_Content.GetChild(this.deletedItemTypeStart) as RectTransform;
						size = Mathf.Max(this.GetSize(oldItem, includeSpacing), size);
						this.ReturnToTempPool(true, 1);
						availableChilds--;
						this.itemTypeStart++;
						bool flag3 = availableChilds == 0;
						if (flag3)
						{
							break;
						}
					}
					bool flag4 = size > 0f;
					if (flag4)
					{
						this.SetDirtyCaching();
						this.m_HasRebuiltLayout = false;
						this.m_PrevPositionValid = true;
						bool flag5 = !this.reverseDirection;
						if (flag5)
						{
							Vector2 offset = this.GetVector(size);
							this.m_Content.anchoredPosition -= offset;
							this.m_PrevPosition -= offset;
							this.m_ContentStartPosition -= offset;
						}
						this.itemTypeSize -= size;
					}
					result = size;
				}
			}
			return result;
		}

		// Token: 0x0600B839 RID: 47161 RVA: 0x0053F600 File Offset: 0x0053D800
		protected float NewItemAtEnd()
		{
			bool flag = this.totalCount >= 0 && this.itemTypeEnd >= this.totalCount;
			float result;
			if (flag)
			{
				result = -1f;
			}
			else
			{
				bool includeSpacing = this.CurrentLines > 0;
				float size = 0f;
				int availableChilds = this.m_Content.childCount - this.deletedItemTypeStart - this.deletedItemTypeEnd;
				int count = this.contentConstraintCount - availableChilds % this.contentConstraintCount;
				for (int i = 0; i < count; i++)
				{
					RectTransform newItem = this.GetFromTempPool(this.itemTypeEnd);
					newItem.SetSiblingIndex(this.m_Content.childCount - this.deletedItemTypeEnd - 1);
					size = Mathf.Max(this.GetSize(newItem, includeSpacing), size);
					this.itemTypeEnd++;
					bool flag2 = this.totalCount >= 0 && this.itemTypeEnd >= this.totalCount;
					if (flag2)
					{
						break;
					}
				}
				this.threshold = Mathf.Max(this.threshold, size * 1.5f);
				bool flag3 = size > 0f;
				if (flag3)
				{
					this.SetDirtyCaching();
					this.m_HasRebuiltLayout = false;
					this.m_PrevPositionValid = true;
					bool flag4 = this.reverseDirection;
					if (flag4)
					{
						Vector2 offset = this.GetVector(size);
						this.m_Content.anchoredPosition -= offset;
						this.m_PrevPosition -= offset;
						this.m_ContentStartPosition -= offset;
					}
					this.itemTypeSize += size;
				}
				result = size;
			}
			return result;
		}

		// Token: 0x0600B83A RID: 47162 RVA: 0x0053F7AC File Offset: 0x0053D9AC
		protected float DeleteItemAtEnd()
		{
			bool flag = (this.m_Dragging || this.m_Velocity != Vector2.zero) && this.totalCount >= 0 && this.itemTypeStart < this.contentConstraintCount;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				int availableChilds = this.m_Content.childCount - this.deletedItemTypeStart - this.deletedItemTypeEnd;
				Debug.Assert(availableChilds >= 0);
				bool flag2 = availableChilds == 0;
				if (flag2)
				{
					result = 0f;
				}
				else
				{
					bool includeSpacing = this.CurrentLines > 1;
					float size = 0f;
					for (int i = 0; i < this.contentConstraintCount; i++)
					{
						RectTransform oldItem = this.m_Content.GetChild(this.m_Content.childCount - this.deletedItemTypeEnd - 1) as RectTransform;
						size = Mathf.Max(this.GetSize(oldItem, includeSpacing), size);
						this.ReturnToTempPool(false, 1);
						availableChilds--;
						this.itemTypeEnd--;
						bool flag3 = this.itemTypeEnd % this.contentConstraintCount == 0 || availableChilds == 0;
						if (flag3)
						{
							break;
						}
					}
					bool flag4 = size > 0f;
					if (flag4)
					{
						this.SetDirtyCaching();
						this.m_HasRebuiltLayout = false;
						this.m_PrevPositionValid = true;
						bool flag5 = this.reverseDirection;
						if (flag5)
						{
							Vector2 offset = this.GetVector(size);
							this.m_Content.anchoredPosition += offset;
							this.m_PrevPosition += offset;
							this.m_ContentStartPosition += offset;
						}
						this.itemTypeSize -= size;
					}
					result = size;
				}
			}
			return result;
		}

		// Token: 0x0600B83B RID: 47163
		protected abstract RectTransform GetFromTempPool(int itemIdx);

		// Token: 0x0600B83C RID: 47164
		protected abstract void ReturnToTempPool(bool fromStart, int count = 1);

		// Token: 0x0600B83D RID: 47165
		protected abstract void ClearTempPool();

		// Token: 0x0600B83E RID: 47166 RVA: 0x0053F96D File Offset: 0x0053DB6D
		[Obsolete("SrollToCell(int, float) has been renamed to ScrollToCell(int, float).")]
		public void SrollToCell(int index, float speed)
		{
			this.ScrollToCell(index, speed, 0f, LoopScrollRectBase.ScrollMode.ToStart);
		}

		// Token: 0x0600B83F RID: 47167 RVA: 0x0053F97F File Offset: 0x0053DB7F
		[Obsolete("SrollToCellWithinTime(int, float) has been renamed to ScrollToCellWithinTime(int, float).")]
		public void SrollToCellWithinTime(int index, float time)
		{
			this.ScrollToCellWithinTime(index, time, 0f, LoopScrollRectBase.ScrollMode.ToStart);
		}

		// Token: 0x0600B840 RID: 47168 RVA: 0x0053F994 File Offset: 0x0053DB94
		public virtual void Rebuild(CanvasUpdate executing)
		{
			bool flag = executing == CanvasUpdate.Prelayout;
			if (flag)
			{
				this.UpdateCachedData();
			}
			bool flag2 = executing == CanvasUpdate.PostLayout;
			if (flag2)
			{
				this.UpdateBounds(false);
				this.UpdateScrollbars(Vector2.zero);
				this.UpdatePrevData();
				this.m_HasRebuiltLayout = true;
				this.m_PrevPositionValid = false;
			}
		}

		// Token: 0x0600B841 RID: 47169 RVA: 0x0053F9E6 File Offset: 0x0053DBE6
		public virtual void LayoutComplete()
		{
		}

		// Token: 0x0600B842 RID: 47170 RVA: 0x0053F9E9 File Offset: 0x0053DBE9
		public virtual void GraphicUpdateComplete()
		{
		}

		// Token: 0x0600B843 RID: 47171 RVA: 0x0053F9EC File Offset: 0x0053DBEC
		private void UpdateCachedData()
		{
			Transform transform = base.transform;
			this.m_HorizontalScrollbarRect = ((this.m_HorizontalScrollbar == null) ? null : (this.m_HorizontalScrollbar.transform as RectTransform));
			this.m_VerticalScrollbarRect = ((this.m_VerticalScrollbar == null) ? null : (this.m_VerticalScrollbar.transform as RectTransform));
			bool viewIsChild = this.viewRect.parent == transform;
			bool hScrollbarIsChild = !this.m_HorizontalScrollbarRect || this.m_HorizontalScrollbarRect.parent == transform;
			bool vScrollbarIsChild = !this.m_VerticalScrollbarRect || this.m_VerticalScrollbarRect.parent == transform;
			bool allAreChildren = viewIsChild && hScrollbarIsChild && vScrollbarIsChild;
			this.m_HSliderExpand = (allAreChildren && this.m_HorizontalScrollbarRect && this.horizontalScrollbarVisibility == LoopScrollRectBase.ScrollbarVisibility.AutoHideAndExpandViewport);
			this.m_VSliderExpand = (allAreChildren && this.m_VerticalScrollbarRect && this.verticalScrollbarVisibility == LoopScrollRectBase.ScrollbarVisibility.AutoHideAndExpandViewport);
			this.m_HSliderHeight = ((this.m_HorizontalScrollbarRect == null) ? 0f : this.m_HorizontalScrollbarRect.rect.height);
			this.m_VSliderWidth = ((this.m_VerticalScrollbarRect == null) ? 0f : this.m_VerticalScrollbarRect.rect.width);
		}

		// Token: 0x0600B844 RID: 47172 RVA: 0x0053FB50 File Offset: 0x0053DD50
		protected override void OnEnable()
		{
			base.OnEnable();
			bool flag = this.m_HorizontalScrollbar;
			if (flag)
			{
				this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
			}
			bool flag2 = this.m_VerticalScrollbar;
			if (flag2)
			{
				this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
			this.SetDirty();
		}

		// Token: 0x0600B845 RID: 47173 RVA: 0x0053FBCC File Offset: 0x0053DDCC
		protected override void OnDisable()
		{
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			bool flag = this.m_HorizontalScrollbar;
			if (flag)
			{
				this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
			}
			bool flag2 = this.m_VerticalScrollbar;
			if (flag2)
			{
				this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
			}
			this.m_Dragging = false;
			this.m_Scrolling = false;
			this.m_HasRebuiltLayout = false;
			this.m_Tracker.Clear();
			this.m_Velocity = Vector2.zero;
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			base.OnDisable();
		}

		// Token: 0x0600B846 RID: 47174 RVA: 0x0053FC78 File Offset: 0x0053DE78
		public override bool IsActive()
		{
			return base.IsActive() && this.m_Content != null;
		}

		// Token: 0x0600B847 RID: 47175 RVA: 0x0053FCA4 File Offset: 0x0053DEA4
		private void EnsureLayoutHasRebuilt()
		{
			bool flag = !this.m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout();
			if (flag)
			{
				Canvas.ForceUpdateCanvases();
			}
		}

		// Token: 0x0600B848 RID: 47176 RVA: 0x0053FCCF File Offset: 0x0053DECF
		public virtual void StopMovement()
		{
			this.m_Velocity = Vector2.zero;
		}

		// Token: 0x0600B849 RID: 47177 RVA: 0x0053FCE0 File Offset: 0x0053DEE0
		public virtual void OnScroll(PointerEventData data)
		{
			bool flag = !this.IsActive();
			if (!flag)
			{
				this.EnsureLayoutHasRebuilt();
				this.UpdateBounds(false);
				Vector2 delta = data.scrollDelta;
				delta.y *= -1f;
				bool flag2 = this.vertical && !this.horizontal;
				if (flag2)
				{
					bool flag3 = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);
					if (flag3)
					{
						delta.y = delta.x;
					}
					delta.x = 0f;
				}
				bool flag4 = this.horizontal && !this.vertical;
				if (flag4)
				{
					bool flag5 = Mathf.Abs(delta.y) > Mathf.Abs(delta.x);
					if (flag5)
					{
						delta.x = delta.y;
					}
					delta.y = 0f;
				}
				bool flag6 = data.IsScrolling();
				if (flag6)
				{
					this.m_Scrolling = true;
				}
				Vector2 position = this.m_Content.anchoredPosition;
				position += delta * this.m_ScrollSensitivity;
				bool flag7 = this.m_MovementType == LoopScrollRectBase.MovementType.Clamped;
				if (flag7)
				{
					position += this.CalculateOffset(position - this.m_Content.anchoredPosition);
				}
				this.SetContentAnchoredPosition(position);
				this.UpdateBounds(false);
			}
		}

		// Token: 0x0600B84A RID: 47178 RVA: 0x0053FE3C File Offset: 0x0053E03C
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			bool flag = eventData.button > PointerEventData.InputButton.Left;
			if (!flag)
			{
				this.m_Velocity = Vector2.zero;
			}
		}

		// Token: 0x0600B84B RID: 47179 RVA: 0x0053FE64 File Offset: 0x0053E064
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			bool flag = eventData.button > PointerEventData.InputButton.Left;
			if (!flag)
			{
				bool flag2 = !this.IsActive();
				if (!flag2)
				{
					this.UpdateBounds(false);
					this.m_PointerStartLocalCursor = Vector2.zero;
					RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, out this.m_PointerStartLocalCursor);
					this.m_ContentStartPosition = this.m_Content.anchoredPosition;
					this.m_Dragging = true;
				}
			}
		}

		// Token: 0x0600B84C RID: 47180 RVA: 0x0053FEDC File Offset: 0x0053E0DC
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			bool flag = eventData.button > PointerEventData.InputButton.Left;
			if (!flag)
			{
				this.m_Dragging = false;
			}
		}

		// Token: 0x0600B84D RID: 47181 RVA: 0x0053FF00 File Offset: 0x0053E100
		public virtual void OnDrag(PointerEventData eventData)
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
						Vector2 localCursor;
						bool flag4 = !RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, out localCursor);
						if (!flag4)
						{
							this.UpdateBounds(false);
							Vector2 pointerDelta = localCursor - this.m_PointerStartLocalCursor;
							Vector2 position = this.m_ContentStartPosition + pointerDelta;
							Vector2 offset = this.CalculateOffset(position - this.m_Content.anchoredPosition);
							position += offset;
							bool flag5 = this.m_MovementType == LoopScrollRectBase.MovementType.Elastic;
							if (flag5)
							{
								bool flag6 = offset.x != 0f;
								if (flag6)
								{
									position.x -= LoopScrollRectBase.RubberDelta(offset.x, this.m_ViewBounds.size.x);
								}
								bool flag7 = offset.y != 0f;
								if (flag7)
								{
									position.y -= LoopScrollRectBase.RubberDelta(offset.y, this.m_ViewBounds.size.y);
								}
							}
							this.SetContentAnchoredPosition(position);
						}
					}
				}
			}
		}

		// Token: 0x0600B84E RID: 47182 RVA: 0x00540050 File Offset: 0x0053E250
		public virtual void SetContentAnchoredPosition(Vector2 position)
		{
			bool flag = !this.m_Horizontal;
			if (flag)
			{
				position.x = this.m_Content.anchoredPosition.x;
			}
			bool flag2 = !this.m_Vertical;
			if (flag2)
			{
				position.y = this.m_Content.anchoredPosition.y;
			}
			bool flag3 = (position - this.m_Content.anchoredPosition).sqrMagnitude > 0.001f;
			if (flag3)
			{
				this.m_Content.anchoredPosition = position;
				this.UpdateBounds(true);
			}
		}

		// Token: 0x0600B84F RID: 47183 RVA: 0x005400E4 File Offset: 0x0053E2E4
		protected virtual void LateUpdate()
		{
			bool flag = !this.m_Content;
			if (!flag)
			{
				this.EnsureLayoutHasRebuilt();
				this.UpdateBounds(false);
				float deltaTime = Time.unscaledDeltaTime;
				Vector2 offset = this.CalculateOffset(Vector2.zero);
				bool flag2 = !this.m_Dragging && (offset != Vector2.zero || this.m_Velocity != Vector2.zero);
				if (flag2)
				{
					Vector2 position = this.m_Content.anchoredPosition;
					for (int axis = 0; axis < 2; axis++)
					{
						bool flag3 = this.m_MovementType == LoopScrollRectBase.MovementType.Elastic && offset[axis] != 0f;
						if (flag3)
						{
							float speed = this.m_Velocity[axis];
							float smoothTime = this.m_Elasticity;
							bool scrolling = this.m_Scrolling;
							if (scrolling)
							{
								smoothTime *= 3f;
							}
							position[axis] = Mathf.SmoothDamp(this.m_Content.anchoredPosition[axis], this.m_Content.anchoredPosition[axis] + offset[axis], ref speed, smoothTime, float.PositiveInfinity, deltaTime);
							bool flag4 = Mathf.Abs(speed) < 1f;
							if (flag4)
							{
								speed = 0f;
							}
							this.m_Velocity[axis] = speed;
						}
						else
						{
							bool inertia = this.m_Inertia;
							if (inertia)
							{
								ref Vector2 ptr = ref this.m_Velocity;
								int index = axis;
								ptr[index] *= Mathf.Pow(this.m_DecelerationRate, deltaTime);
								bool flag5 = Mathf.Abs(this.m_Velocity[axis]) < 1f;
								if (flag5)
								{
									this.m_Velocity[axis] = 0f;
								}
								ptr = ref position;
								index = axis;
								ptr[index] += this.m_Velocity[axis] * deltaTime;
							}
							else
							{
								this.m_Velocity[axis] = 0f;
							}
						}
					}
					bool flag6 = this.m_MovementType == LoopScrollRectBase.MovementType.Clamped;
					if (flag6)
					{
						offset = this.CalculateOffset(position - this.m_Content.anchoredPosition);
						position += offset;
					}
					this.SetContentAnchoredPosition(position);
				}
				bool flag7 = this.m_Dragging && this.m_Inertia;
				if (flag7)
				{
					Vector3 newVelocity = (this.m_Content.anchoredPosition - this.m_PrevPosition) / deltaTime;
					this.m_Velocity = Vector3.Lerp(this.m_Velocity, newVelocity, deltaTime * 10f);
				}
				bool flag8 = this.m_ViewBounds != this.m_PrevViewBounds || this.m_ContentBounds != this.m_PrevContentBounds || this.m_Content.anchoredPosition != this.m_PrevPosition;
				if (flag8)
				{
					this.UpdateScrollbars(offset);
					UISystemProfilerApi.AddMarker("ScrollRect.value", this);
					this.m_OnValueChanged.Invoke(this.normalizedPosition);
					this.UpdatePrevData();
				}
				this.UpdateScrollbarVisibility();
				this.m_Scrolling = false;
			}
		}

		// Token: 0x0600B850 RID: 47184 RVA: 0x00540430 File Offset: 0x0053E630
		private void UpdatePrevData()
		{
			bool flag = !this.m_PrevPositionValid;
			if (flag)
			{
				bool flag2 = this.m_Content == null;
				if (flag2)
				{
					this.m_PrevPosition = Vector2.zero;
				}
				else
				{
					this.m_PrevPosition = this.m_Content.anchoredPosition;
				}
			}
			this.m_PrevViewBounds = this.m_ViewBounds;
			this.m_PrevContentBounds = this.m_ContentBounds;
		}

		// Token: 0x0600B851 RID: 47185 RVA: 0x00540494 File Offset: 0x0053E694
		protected float EstimiateElementSize()
		{
			int childCount = this.m_Content.childCount;
			bool flag = this.CurrentLines == 0;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				float elementSize = (this.itemTypeSize - this.contentSpacing * (float)(this.CurrentLines - 1)) / (float)this.CurrentLines;
				result = elementSize;
			}
			return result;
		}

		// Token: 0x0600B852 RID: 47186 RVA: 0x005404EC File Offset: 0x0053E6EC
		public void GetHorizonalOffsetAndSize(out float totalSize, out float offset)
		{
			float paddingSize = this.m_ContentLeftPadding + this.m_ContentRightPadding;
			bool flag = this.sizeHelper != null;
			if (flag)
			{
				totalSize = this.sizeHelper.GetItemsSize(0, this.TotalLines) + this.contentSpacing * (float)(this.TotalLines - 1) + paddingSize;
				offset = this.m_ContentBounds.min.x - this.sizeHelper.GetItemsSize(0, this.StartLine) - this.contentSpacing * (float)this.StartLine;
			}
			else
			{
				float elementSize = this.EstimiateElementSize();
				totalSize = elementSize * (float)this.TotalLines + this.contentSpacing * (float)(this.TotalLines - 1) + paddingSize;
				offset = this.m_ContentBounds.min.x - elementSize * (float)this.StartLine - this.contentSpacing * (float)this.StartLine;
			}
		}

		// Token: 0x0600B853 RID: 47187 RVA: 0x005405C8 File Offset: 0x0053E7C8
		public void GetVerticalOffsetAndSize(out float totalSize, out float offset)
		{
			float paddingSize = this.m_ContentTopPadding + this.m_ContentBottomPadding;
			bool flag = this.sizeHelper != null;
			if (flag)
			{
				totalSize = this.sizeHelper.GetItemsSize(0, this.TotalLines) + this.contentSpacing * (float)(this.TotalLines - 1) + paddingSize;
				offset = this.m_ContentBounds.max.y + this.sizeHelper.GetItemsSize(0, this.StartLine) + this.contentSpacing * (float)this.StartLine;
			}
			else
			{
				float elementSize = this.EstimiateElementSize();
				totalSize = elementSize * (float)this.TotalLines + this.contentSpacing * (float)(this.TotalLines - 1) + paddingSize;
				offset = this.m_ContentBounds.max.y + elementSize * (float)this.StartLine + this.contentSpacing * (float)this.StartLine;
			}
		}

		// Token: 0x0600B854 RID: 47188 RVA: 0x005406A4 File Offset: 0x0053E8A4
		protected virtual void UpdateScrollbars(Vector2 offset)
		{
			bool flag = this.m_HorizontalScrollbar;
			if (flag)
			{
				bool flag2 = this.m_ContentBounds.size.x > 0f && this.totalCount > 0;
				if (flag2)
				{
					float totalSize;
					float _;
					this.GetHorizonalOffsetAndSize(out totalSize, out _);
					float size = Mathf.Clamp01((this.m_ViewBounds.size.x - Mathf.Abs(offset.x)) / totalSize);
					bool flag3 = this.m_FixedHorizontalScrollbarSize > 0f;
					if (flag3)
					{
						size = size / (this.m_ViewBounds.size.x / totalSize) * this.m_FixedHorizontalScrollbarSize;
					}
					this.m_HorizontalScrollbar.size = size;
				}
				else
				{
					this.m_HorizontalScrollbar.size = 1f;
				}
				this.m_HorizontalScrollbar.SetValueWithoutNotify(this.horizontalNormalizedPosition);
			}
			bool flag4 = this.m_VerticalScrollbar;
			if (flag4)
			{
				bool flag5 = this.m_ContentBounds.size.y > 0f && this.totalCount > 0;
				if (flag5)
				{
					float totalSize2;
					float _2;
					this.GetVerticalOffsetAndSize(out totalSize2, out _2);
					float size2 = Mathf.Clamp01((this.m_ViewBounds.size.y - Mathf.Abs(offset.y)) / totalSize2);
					bool flag6 = this.m_FixedVerticalScrollbarSize > 0f;
					if (flag6)
					{
						size2 = size2 / (this.m_ViewBounds.size.y / totalSize2) * this.m_FixedVerticalScrollbarSize;
					}
					this.m_VerticalScrollbar.size = size2;
				}
				else
				{
					this.m_VerticalScrollbar.size = 1f;
				}
				this.m_VerticalScrollbar.SetValueWithoutNotify(this.verticalNormalizedPosition);
			}
		}

		// Token: 0x170014D0 RID: 5328
		// (get) Token: 0x0600B855 RID: 47189 RVA: 0x00540860 File Offset: 0x0053EA60
		// (set) Token: 0x0600B856 RID: 47190 RVA: 0x00540883 File Offset: 0x0053EA83
		public Vector2 normalizedPosition
		{
			get
			{
				return new Vector2(this.horizontalNormalizedPosition, this.verticalNormalizedPosition);
			}
			set
			{
				this.SetNormalizedPosition(value.x, 0);
				this.SetNormalizedPosition(value.y, 1);
			}
		}

		// Token: 0x170014D1 RID: 5329
		// (get) Token: 0x0600B857 RID: 47191 RVA: 0x005408A4 File Offset: 0x0053EAA4
		// (set) Token: 0x0600B858 RID: 47192 RVA: 0x0054094E File Offset: 0x0053EB4E
		public float horizontalNormalizedPosition
		{
			get
			{
				this.UpdateBounds(false);
				bool flag = this.totalCount > 0 && this.itemTypeEnd > this.itemTypeStart;
				float result;
				if (flag)
				{
					float totalSize;
					float offset;
					this.GetHorizonalOffsetAndSize(out totalSize, out offset);
					bool flag2 = totalSize <= this.m_ViewBounds.size.x;
					if (flag2)
					{
						result = (float)((this.m_ViewBounds.min.x > offset) ? 1 : 0);
					}
					else
					{
						result = (this.m_ViewBounds.min.x - offset) / (totalSize - this.m_ViewBounds.size.x);
					}
				}
				else
				{
					result = 0.5f;
				}
				return result;
			}
			set
			{
				this.SetNormalizedPosition(value, 0);
			}
		}

		// Token: 0x170014D2 RID: 5330
		// (get) Token: 0x0600B859 RID: 47193 RVA: 0x0054095C File Offset: 0x0053EB5C
		// (set) Token: 0x0600B85A RID: 47194 RVA: 0x00540A06 File Offset: 0x0053EC06
		public float verticalNormalizedPosition
		{
			get
			{
				this.UpdateBounds(false);
				bool flag = this.totalCount > 0 && this.itemTypeEnd > this.itemTypeStart;
				float result;
				if (flag)
				{
					float totalSize;
					float offset;
					this.GetVerticalOffsetAndSize(out totalSize, out offset);
					bool flag2 = totalSize <= this.m_ViewBounds.size.y;
					if (flag2)
					{
						result = (float)((offset > this.m_ViewBounds.max.y) ? 1 : 0);
					}
					else
					{
						result = (offset - this.m_ViewBounds.max.y) / (totalSize - this.m_ViewBounds.size.y);
					}
				}
				else
				{
					result = 0.5f;
				}
				return result;
			}
			set
			{
				this.SetNormalizedPosition(value, 1);
			}
		}

		// Token: 0x0600B85B RID: 47195 RVA: 0x00540A12 File Offset: 0x0053EC12
		private void SetHorizontalNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 0);
		}

		// Token: 0x0600B85C RID: 47196 RVA: 0x00540A1E File Offset: 0x0053EC1E
		private void SetVerticalNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 1);
		}

		// Token: 0x0600B85D RID: 47197 RVA: 0x00540A2C File Offset: 0x0053EC2C
		protected virtual void SetNormalizedPosition(float value, int axis)
		{
			bool flag = this.totalCount <= 0 || this.itemTypeEnd <= this.itemTypeStart;
			if (!flag)
			{
				this.EnsureLayoutHasRebuilt();
				this.UpdateBounds(false);
				float newAnchoredPosition = this.m_Content.anchoredPosition[axis];
				bool flag2 = axis == 0;
				if (flag2)
				{
					float totalSize;
					float offset;
					this.GetHorizonalOffsetAndSize(out totalSize, out offset);
					bool flag3 = totalSize >= this.m_ViewBounds.size.x;
					if (flag3)
					{
						newAnchoredPosition += this.m_ViewBounds.min.x - value * (totalSize - this.m_ViewBounds.size.x) - offset;
					}
				}
				else
				{
					float totalSize;
					float offset;
					this.GetVerticalOffsetAndSize(out totalSize, out offset);
					bool flag4 = totalSize >= this.m_ViewBounds.size.y;
					if (flag4)
					{
						newAnchoredPosition -= offset - value * (totalSize - this.m_ViewBounds.size.y) - this.m_ViewBounds.max.y;
					}
				}
				Vector3 anchoredPosition = this.m_Content.anchoredPosition;
				bool flag5 = Mathf.Abs(anchoredPosition[axis] - newAnchoredPosition) > 0.01f;
				if (flag5)
				{
					anchoredPosition[axis] = newAnchoredPosition;
					this.m_Content.anchoredPosition = anchoredPosition;
					this.m_Velocity[axis] = 0f;
					this.UpdateBounds(true);
				}
			}
		}

		// Token: 0x0600B85E RID: 47198 RVA: 0x00540BA4 File Offset: 0x0053EDA4
		protected static float RubberDelta(float overStretching, float viewSize)
		{
			return (1f - 1f / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1f)) * viewSize * Mathf.Sign(overStretching);
		}

		// Token: 0x0600B85F RID: 47199 RVA: 0x00540BE0 File Offset: 0x0053EDE0
		protected override void OnRectTransformDimensionsChange()
		{
			this.SetDirty();
			bool isActiveAndEnabled = base.isActiveAndEnabled;
			if (isActiveAndEnabled)
			{
				this.UpdateBounds(true);
			}
		}

		// Token: 0x170014D3 RID: 5331
		// (get) Token: 0x0600B860 RID: 47200 RVA: 0x00540C0C File Offset: 0x0053EE0C
		protected bool vScrollingNeeded
		{
			get
			{
				bool isPlaying = Application.isPlaying;
				return !isPlaying || this.m_ContentBounds.size.y > this.m_ViewBounds.size.y + 0.01f;
			}
		}

		// Token: 0x170014D4 RID: 5332
		// (get) Token: 0x0600B861 RID: 47201 RVA: 0x00540C54 File Offset: 0x0053EE54
		protected bool hScrollingNeeded
		{
			get
			{
				bool isPlaying = Application.isPlaying;
				return !isPlaying || this.m_ContentBounds.size.x > this.m_ViewBounds.size.x + 0.01f;
			}
		}

		// Token: 0x0600B862 RID: 47202 RVA: 0x00540C9B File Offset: 0x0053EE9B
		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		// Token: 0x0600B863 RID: 47203 RVA: 0x00540C9E File Offset: 0x0053EE9E
		public virtual void CalculateLayoutInputVertical()
		{
		}

		// Token: 0x170014D5 RID: 5333
		// (get) Token: 0x0600B864 RID: 47204 RVA: 0x00540CA4 File Offset: 0x0053EEA4
		public virtual float minWidth
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170014D6 RID: 5334
		// (get) Token: 0x0600B865 RID: 47205 RVA: 0x00540CBC File Offset: 0x0053EEBC
		public virtual float preferredWidth
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170014D7 RID: 5335
		// (get) Token: 0x0600B866 RID: 47206 RVA: 0x00540CD4 File Offset: 0x0053EED4
		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170014D8 RID: 5336
		// (get) Token: 0x0600B867 RID: 47207 RVA: 0x00540CEC File Offset: 0x0053EEEC
		public virtual float minHeight
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170014D9 RID: 5337
		// (get) Token: 0x0600B868 RID: 47208 RVA: 0x00540D04 File Offset: 0x0053EF04
		public virtual float preferredHeight
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170014DA RID: 5338
		// (get) Token: 0x0600B869 RID: 47209 RVA: 0x00540D1C File Offset: 0x0053EF1C
		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170014DB RID: 5339
		// (get) Token: 0x0600B86A RID: 47210 RVA: 0x00540D34 File Offset: 0x0053EF34
		public virtual int layoutPriority
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x0600B86B RID: 47211 RVA: 0x00540D48 File Offset: 0x0053EF48
		public virtual void SetLayoutHorizontal()
		{
			this.m_Tracker.Clear();
			bool flag = this.m_HSliderExpand || this.m_VSliderExpand;
			if (flag)
			{
				this.m_Tracker.Add(this, this.viewRect, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.SizeDeltaY);
				this.viewRect.anchorMin = Vector2.zero;
				this.viewRect.anchorMax = Vector2.one;
				this.viewRect.sizeDelta = Vector2.zero;
				this.viewRect.anchoredPosition = Vector2.zero;
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.m_Content);
				this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			bool flag2 = this.m_VSliderExpand && this.vScrollingNeeded;
			if (flag2)
			{
				this.viewRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.viewRect.sizeDelta.y);
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.m_Content);
				this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			bool flag3 = this.m_HSliderExpand && this.hScrollingNeeded;
			if (flag3)
			{
				this.viewRect.sizeDelta = new Vector2(this.viewRect.sizeDelta.x, -(this.m_HSliderHeight + this.m_HorizontalScrollbarSpacing));
				this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			bool flag4 = this.m_VSliderExpand && this.vScrollingNeeded && this.viewRect.sizeDelta.x == 0f && this.viewRect.sizeDelta.y < 0f;
			if (flag4)
			{
				this.viewRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.viewRect.sizeDelta.y);
			}
		}

		// Token: 0x0600B86C RID: 47212 RVA: 0x00540FCC File Offset: 0x0053F1CC
		public virtual void SetLayoutVertical()
		{
			this.UpdateScrollbarLayout();
			this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
			this.m_ContentBounds = this.GetBounds();
		}

		// Token: 0x0600B86D RID: 47213 RVA: 0x00541028 File Offset: 0x0053F228
		private void UpdateScrollbarVisibility()
		{
			LoopScrollRectBase.UpdateOneScrollbarVisibility(this.vScrollingNeeded, this.m_Vertical, this.m_VerticalScrollbarVisibility, this.m_VerticalScrollbar);
			LoopScrollRectBase.UpdateOneScrollbarVisibility(this.hScrollingNeeded, this.m_Horizontal, this.m_HorizontalScrollbarVisibility, this.m_HorizontalScrollbar);
		}

		// Token: 0x0600B86E RID: 47214 RVA: 0x00541068 File Offset: 0x0053F268
		private static void UpdateOneScrollbarVisibility(bool xScrollingNeeded, bool xAxisEnabled, LoopScrollRectBase.ScrollbarVisibility scrollbarVisibility, Scrollbar scrollbar)
		{
			bool flag = scrollbar;
			if (flag)
			{
				bool flag2 = scrollbarVisibility == LoopScrollRectBase.ScrollbarVisibility.Permanent;
				if (flag2)
				{
					bool flag3 = scrollbar.gameObject.activeSelf != xAxisEnabled;
					if (flag3)
					{
						scrollbar.gameObject.SetActive(xAxisEnabled);
					}
				}
				else
				{
					bool flag4 = scrollbar.gameObject.activeSelf != xScrollingNeeded;
					if (flag4)
					{
						scrollbar.gameObject.SetActive(xScrollingNeeded);
					}
				}
			}
		}

		// Token: 0x0600B86F RID: 47215 RVA: 0x005410D4 File Offset: 0x0053F2D4
		private void UpdateScrollbarLayout()
		{
			bool flag = this.m_VSliderExpand && this.m_HorizontalScrollbar;
			if (flag)
			{
				this.m_Tracker.Add(this, this.m_HorizontalScrollbarRect, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.SizeDeltaX);
				this.m_HorizontalScrollbarRect.anchorMin = new Vector2(0f, this.m_HorizontalScrollbarRect.anchorMin.y);
				this.m_HorizontalScrollbarRect.anchorMax = new Vector2(1f, this.m_HorizontalScrollbarRect.anchorMax.y);
				this.m_HorizontalScrollbarRect.anchoredPosition = new Vector2(0f, this.m_HorizontalScrollbarRect.anchoredPosition.y);
				bool vScrollingNeeded = this.vScrollingNeeded;
				if (vScrollingNeeded)
				{
					this.m_HorizontalScrollbarRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.m_HorizontalScrollbarRect.sizeDelta.y);
				}
				else
				{
					this.m_HorizontalScrollbarRect.sizeDelta = new Vector2(0f, this.m_HorizontalScrollbarRect.sizeDelta.y);
				}
			}
			bool flag2 = this.m_HSliderExpand && this.m_VerticalScrollbar;
			if (flag2)
			{
				this.m_Tracker.Add(this, this.m_VerticalScrollbarRect, DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaY);
				this.m_VerticalScrollbarRect.anchorMin = new Vector2(this.m_VerticalScrollbarRect.anchorMin.x, 0f);
				this.m_VerticalScrollbarRect.anchorMax = new Vector2(this.m_VerticalScrollbarRect.anchorMax.x, 1f);
				this.m_VerticalScrollbarRect.anchoredPosition = new Vector2(this.m_VerticalScrollbarRect.anchoredPosition.x, 0f);
				bool hScrollingNeeded = this.hScrollingNeeded;
				if (hScrollingNeeded)
				{
					this.m_VerticalScrollbarRect.sizeDelta = new Vector2(this.m_VerticalScrollbarRect.sizeDelta.x, -(this.m_HSliderHeight + this.m_HorizontalScrollbarSpacing));
				}
				else
				{
					this.m_VerticalScrollbarRect.sizeDelta = new Vector2(this.m_VerticalScrollbarRect.sizeDelta.x, 0f);
				}
			}
		}

		// Token: 0x0600B870 RID: 47216 RVA: 0x005412F8 File Offset: 0x0053F4F8
		protected void UpdateBounds(bool updateItems = false)
		{
			this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
			this.m_ContentBounds = this.GetBounds();
			bool flag = this.m_Content == null;
			if (!flag)
			{
				bool flag2 = !this.m_HasRebuiltLayout;
				if (flag2)
				{
					updateItems = false;
				}
				bool flag3 = Application.isPlaying && updateItems && this.UpdateItems(ref this.m_ViewBounds, ref this.m_ContentBounds);
				if (flag3)
				{
					this.EnsureLayoutHasRebuilt();
					this.m_ContentBounds = this.GetBounds();
				}
				Vector3 contentSize = this.m_ContentBounds.size;
				Vector3 contentPos = this.m_ContentBounds.center;
				Vector2 contentPivot = this.m_Content.pivot;
				LoopScrollRectBase.AdjustBounds(ref this.m_ViewBounds, ref contentPivot, ref contentSize, ref contentPos);
				this.m_ContentBounds.size = contentSize;
				this.m_ContentBounds.center = contentPos;
				bool flag4 = this.movementType == LoopScrollRectBase.MovementType.Clamped;
				if (flag4)
				{
					Vector2 delta = Vector2.zero;
					bool flag5 = this.m_ViewBounds.max.x > this.m_ContentBounds.max.x;
					if (flag5)
					{
						delta.x = Math.Min(this.m_ViewBounds.min.x - this.m_ContentBounds.min.x, this.m_ViewBounds.max.x - this.m_ContentBounds.max.x);
					}
					else
					{
						bool flag6 = this.m_ViewBounds.min.x < this.m_ContentBounds.min.x;
						if (flag6)
						{
							delta.x = Math.Max(this.m_ViewBounds.min.x - this.m_ContentBounds.min.x, this.m_ViewBounds.max.x - this.m_ContentBounds.max.x);
						}
					}
					bool flag7 = this.m_ViewBounds.min.y < this.m_ContentBounds.min.y;
					if (flag7)
					{
						delta.y = Math.Max(this.m_ViewBounds.min.y - this.m_ContentBounds.min.y, this.m_ViewBounds.max.y - this.m_ContentBounds.max.y);
					}
					else
					{
						bool flag8 = this.m_ViewBounds.max.y > this.m_ContentBounds.max.y;
						if (flag8)
						{
							delta.y = Math.Min(this.m_ViewBounds.min.y - this.m_ContentBounds.min.y, this.m_ViewBounds.max.y - this.m_ContentBounds.max.y);
						}
					}
					bool flag9 = delta.sqrMagnitude > float.Epsilon;
					if (flag9)
					{
						contentPos = this.m_Content.anchoredPosition + delta;
						bool flag10 = !this.m_Horizontal;
						if (flag10)
						{
							contentPos.x = this.m_Content.anchoredPosition.x;
						}
						bool flag11 = !this.m_Vertical;
						if (flag11)
						{
							contentPos.y = this.m_Content.anchoredPosition.y;
						}
						LoopScrollRectBase.AdjustBounds(ref this.m_ViewBounds, ref contentPivot, ref contentSize, ref contentPos);
					}
				}
			}
		}

		// Token: 0x0600B871 RID: 47217 RVA: 0x0054168C File Offset: 0x0053F88C
		internal static void AdjustBounds(ref Bounds viewBounds, ref Vector2 contentPivot, ref Vector3 contentSize, ref Vector3 contentPos)
		{
			Vector3 excess = viewBounds.size - contentSize;
			bool flag = excess.x > 0f;
			if (flag)
			{
				contentPos.x -= excess.x * (contentPivot.x - 0.5f);
				contentSize.x = viewBounds.size.x;
			}
			bool flag2 = excess.y > 0f;
			if (flag2)
			{
				contentPos.y -= excess.y * (contentPivot.y - 0.5f);
				contentSize.y = viewBounds.size.y;
			}
		}

		// Token: 0x0600B872 RID: 47218 RVA: 0x00541730 File Offset: 0x0053F930
		private Bounds GetBounds()
		{
			bool flag = this.m_Content == null;
			Bounds result;
			if (flag)
			{
				result = default(Bounds);
			}
			else
			{
				this.m_Content.GetWorldCorners(this.m_Corners);
				Matrix4x4 viewWorldToLocalMatrix = this.viewRect.worldToLocalMatrix;
				result = LoopScrollRectBase.InternalGetBounds(this.m_Corners, ref viewWorldToLocalMatrix);
			}
			return result;
		}

		// Token: 0x0600B873 RID: 47219 RVA: 0x0054178C File Offset: 0x0053F98C
		internal static Bounds InternalGetBounds(Vector3[] corners, ref Matrix4x4 viewWorldToLocalMatrix)
		{
			Vector3 vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			for (int i = 0; i < 4; i++)
			{
				Vector3 v = viewWorldToLocalMatrix.MultiplyPoint3x4(corners[i]);
				vMin = Vector3.Min(v, vMin);
				vMax = Vector3.Max(v, vMax);
			}
			Bounds bounds = new Bounds(vMin, Vector3.zero);
			bounds.Encapsulate(vMax);
			return bounds;
		}

		// Token: 0x0600B874 RID: 47220 RVA: 0x00541818 File Offset: 0x0053FA18
		private Bounds GetBounds4Item(int index)
		{
			bool flag = this.m_Content == null;
			Bounds result;
			if (flag)
			{
				result = default(Bounds);
			}
			else
			{
				int offset = index - this.itemTypeStart;
				bool flag2 = offset < 0 || offset >= this.m_Content.childCount;
				if (flag2)
				{
					result = default(Bounds);
				}
				else
				{
					RectTransform rt = this.m_Content.GetChild(offset) as RectTransform;
					bool flag3 = rt == null;
					if (flag3)
					{
						result = default(Bounds);
					}
					else
					{
						rt.GetWorldCorners(this.m_Corners);
						Matrix4x4 viewWorldToLocalMatrix = this.viewRect.worldToLocalMatrix;
						result = LoopScrollRectBase.InternalGetBounds(this.m_Corners, ref viewWorldToLocalMatrix);
					}
				}
			}
			return result;
		}

		// Token: 0x0600B875 RID: 47221 RVA: 0x005418D8 File Offset: 0x0053FAD8
		protected Vector2 CalculateOffset(Vector2 delta)
		{
			bool flag = this.totalCount < 0 || this.movementType == LoopScrollRectBase.MovementType.Unrestricted;
			Vector2 result;
			if (flag)
			{
				result = delta;
			}
			else
			{
				Bounds contentBound = this.m_ContentBounds;
				bool horizontal = this.m_Horizontal;
				if (horizontal)
				{
					float totalSize;
					float offset;
					this.GetHorizonalOffsetAndSize(out totalSize, out offset);
					Vector3 center = contentBound.center;
					center.x = offset;
					contentBound.Encapsulate(center);
					center.x = offset + totalSize;
					contentBound.Encapsulate(center);
				}
				bool vertical = this.m_Vertical;
				if (vertical)
				{
					float totalSize2;
					float offset2;
					this.GetVerticalOffsetAndSize(out totalSize2, out offset2);
					Vector3 center2 = contentBound.center;
					center2.y = offset2;
					contentBound.Encapsulate(center2);
					center2.y = offset2 - totalSize2;
					contentBound.Encapsulate(center2);
				}
				result = LoopScrollRectBase.InternalCalculateOffset(ref this.m_ViewBounds, ref contentBound, this.m_Horizontal, this.m_Vertical, this.m_MovementType, ref delta);
			}
			return result;
		}

		// Token: 0x0600B876 RID: 47222 RVA: 0x005419C8 File Offset: 0x0053FBC8
		internal static Vector2 InternalCalculateOffset(ref Bounds viewBounds, ref Bounds contentBounds, bool horizontal, bool vertical, LoopScrollRectBase.MovementType movementType, ref Vector2 delta)
		{
			Vector2 offset = Vector2.zero;
			bool flag = movementType == LoopScrollRectBase.MovementType.Unrestricted;
			Vector2 result;
			if (flag)
			{
				result = offset;
			}
			else
			{
				Vector2 min = contentBounds.min;
				Vector2 max = contentBounds.max;
				if (horizontal)
				{
					min.x += delta.x;
					max.x += delta.x;
					float maxOffset = viewBounds.max.x - max.x;
					float minOffset = viewBounds.min.x - min.x;
					bool flag2 = minOffset < -0.001f;
					if (flag2)
					{
						offset.x = minOffset;
					}
					else
					{
						bool flag3 = maxOffset > 0.001f;
						if (flag3)
						{
							offset.x = maxOffset;
						}
					}
				}
				if (vertical)
				{
					min.y += delta.y;
					max.y += delta.y;
					float maxOffset2 = viewBounds.max.y - max.y;
					float minOffset2 = viewBounds.min.y - min.y;
					bool flag4 = maxOffset2 > 0.001f;
					if (flag4)
					{
						offset.y = maxOffset2;
					}
					else
					{
						bool flag5 = minOffset2 < -0.001f;
						if (flag5)
						{
							offset.y = minOffset2;
						}
					}
				}
				result = offset;
			}
			return result;
		}

		// Token: 0x0600B877 RID: 47223 RVA: 0x00541B20 File Offset: 0x0053FD20
		protected void SetDirty()
		{
			bool flag = !this.IsActive();
			if (!flag)
			{
				LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			}
		}

		// Token: 0x0600B878 RID: 47224 RVA: 0x00541B4C File Offset: 0x0053FD4C
		protected void SetDirtyCaching()
		{
			bool flag = !this.IsActive();
			if (!flag)
			{
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
				LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			}
		}

		// Token: 0x0600B879 RID: 47225 RVA: 0x00541B7C File Offset: 0x0053FD7C
		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}

		// Token: 0x04008EE7 RID: 36583
		[HideInInspector]
		[NonSerialized]
		public LoopScrollPrefabSource prefabSource = null;

		// Token: 0x04008EE8 RID: 36584
		[Tooltip("Total count, negative means INFINITE mode")]
		public int totalCount;

		// Token: 0x04008EE9 RID: 36585
		[HideInInspector]
		[NonSerialized]
		public LoopScrollSizeHelper sizeHelper = null;

		// Token: 0x04008EEA RID: 36586
		protected float threshold = 0f;

		// Token: 0x04008EEB RID: 36587
		[Tooltip("Reverse direction for dragging")]
		public bool reverseDirection = false;

		// Token: 0x04008EEC RID: 36588
		protected int itemTypeStart = 0;

		// Token: 0x04008EED RID: 36589
		protected int itemTypeEnd = 0;

		// Token: 0x04008EEE RID: 36590
		protected float itemTypeSize = 0f;

		// Token: 0x04008EEF RID: 36591
		protected LoopScrollRectBase.LoopScrollRectDirection direction = LoopScrollRectBase.LoopScrollRectDirection.Horizontal;

		// Token: 0x04008EF0 RID: 36592
		private bool m_ContentSpaceInit = false;

		// Token: 0x04008EF1 RID: 36593
		private float m_ContentSpacing = 0f;

		// Token: 0x04008EF2 RID: 36594
		protected float m_ContentLeftPadding = 0f;

		// Token: 0x04008EF3 RID: 36595
		protected float m_ContentRightPadding = 0f;

		// Token: 0x04008EF4 RID: 36596
		protected float m_ContentTopPadding = 0f;

		// Token: 0x04008EF5 RID: 36597
		protected float m_ContentBottomPadding = 0f;

		// Token: 0x04008EF6 RID: 36598
		protected GridLayoutGroup m_GridLayout = null;

		// Token: 0x04008EF7 RID: 36599
		private bool m_ContentConstraintCountInit = false;

		// Token: 0x04008EF8 RID: 36600
		private int m_ContentConstraintCount = 0;

		// Token: 0x04008EF9 RID: 36601
		[SerializeField]
		protected RectTransform m_Content;

		// Token: 0x04008EFA RID: 36602
		[SerializeField]
		private bool m_Horizontal = true;

		// Token: 0x04008EFB RID: 36603
		[SerializeField]
		private bool m_Vertical = true;

		// Token: 0x04008EFC RID: 36604
		[SerializeField]
		private LoopScrollRectBase.MovementType m_MovementType = LoopScrollRectBase.MovementType.Elastic;

		// Token: 0x04008EFD RID: 36605
		[SerializeField]
		private float m_Elasticity = 0.1f;

		// Token: 0x04008EFE RID: 36606
		[SerializeField]
		private bool m_Inertia = true;

		// Token: 0x04008EFF RID: 36607
		[SerializeField]
		private float m_DecelerationRate = 0.135f;

		// Token: 0x04008F00 RID: 36608
		[SerializeField]
		private float m_ScrollSensitivity = 100f;

		// Token: 0x04008F01 RID: 36609
		[SerializeField]
		private RectTransform m_Viewport;

		// Token: 0x04008F02 RID: 36610
		[SerializeField]
		private Scrollbar m_HorizontalScrollbar;

		// Token: 0x04008F03 RID: 36611
		[SerializeField]
		private Scrollbar m_VerticalScrollbar;

		// Token: 0x04008F04 RID: 36612
		[SerializeField]
		private LoopScrollRectBase.ScrollbarVisibility m_HorizontalScrollbarVisibility;

		// Token: 0x04008F05 RID: 36613
		[SerializeField]
		private LoopScrollRectBase.ScrollbarVisibility m_VerticalScrollbarVisibility;

		// Token: 0x04008F06 RID: 36614
		[SerializeField]
		private float m_HorizontalScrollbarSpacing;

		// Token: 0x04008F07 RID: 36615
		[SerializeField]
		private float m_VerticalScrollbarSpacing;

		// Token: 0x04008F08 RID: 36616
		[SerializeField]
		private LoopScrollRectBase.ScrollRectEvent m_OnValueChanged = new LoopScrollRectBase.ScrollRectEvent();

		// Token: 0x04008F09 RID: 36617
		[SerializeField]
		[Range(0f, 1f)]
		private float m_FixedHorizontalScrollbarSize = 0f;

		// Token: 0x04008F0A RID: 36618
		[SerializeField]
		[Range(0f, 1f)]
		private float m_FixedVerticalScrollbarSize = 0f;

		// Token: 0x04008F0B RID: 36619
		protected Vector2 m_PointerStartLocalCursor = Vector2.zero;

		// Token: 0x04008F0C RID: 36620
		protected Vector2 m_ContentStartPosition = Vector2.zero;

		// Token: 0x04008F0D RID: 36621
		private RectTransform m_ViewRect;

		// Token: 0x04008F0E RID: 36622
		protected Bounds m_ContentBounds;

		// Token: 0x04008F0F RID: 36623
		protected Bounds m_ViewBounds;

		// Token: 0x04008F10 RID: 36624
		private Vector2 m_Velocity;

		// Token: 0x04008F11 RID: 36625
		protected bool m_Dragging;

		// Token: 0x04008F12 RID: 36626
		protected bool m_Scrolling;

		// Token: 0x04008F13 RID: 36627
		private Vector2 m_PrevPosition = Vector2.zero;

		// Token: 0x04008F14 RID: 36628
		private Bounds m_PrevContentBounds;

		// Token: 0x04008F15 RID: 36629
		private Bounds m_PrevViewBounds;

		// Token: 0x04008F16 RID: 36630
		[NonSerialized]
		private bool m_HasRebuiltLayout = false;

		// Token: 0x04008F17 RID: 36631
		[NonSerialized]
		private bool m_PrevPositionValid = false;

		// Token: 0x04008F18 RID: 36632
		private bool m_HSliderExpand;

		// Token: 0x04008F19 RID: 36633
		private bool m_VSliderExpand;

		// Token: 0x04008F1A RID: 36634
		private float m_HSliderHeight;

		// Token: 0x04008F1B RID: 36635
		private float m_VSliderWidth;

		// Token: 0x04008F1C RID: 36636
		[NonSerialized]
		private RectTransform m_Rect;

		// Token: 0x04008F1D RID: 36637
		private RectTransform m_HorizontalScrollbarRect;

		// Token: 0x04008F1E RID: 36638
		private RectTransform m_VerticalScrollbarRect;

		// Token: 0x04008F1F RID: 36639
		private DrivenRectTransformTracker m_Tracker;

		// Token: 0x04008F20 RID: 36640
		protected int deletedItemTypeStart = 0;

		// Token: 0x04008F21 RID: 36641
		protected int deletedItemTypeEnd = 0;

		// Token: 0x04008F22 RID: 36642
		private readonly Vector3[] m_Corners = new Vector3[4];

		// Token: 0x020025E3 RID: 9699
		protected enum LoopScrollRectDirection
		{
			// Token: 0x0400E96F RID: 59759
			Vertical,
			// Token: 0x0400E970 RID: 59760
			Horizontal
		}

		// Token: 0x020025E4 RID: 9700
		public enum MovementType
		{
			// Token: 0x0400E972 RID: 59762
			Unrestricted,
			// Token: 0x0400E973 RID: 59763
			Elastic,
			// Token: 0x0400E974 RID: 59764
			Clamped
		}

		// Token: 0x020025E5 RID: 9701
		public enum ScrollbarVisibility
		{
			// Token: 0x0400E976 RID: 59766
			Permanent,
			// Token: 0x0400E977 RID: 59767
			AutoHide,
			// Token: 0x0400E978 RID: 59768
			AutoHideAndExpandViewport
		}

		// Token: 0x020025E6 RID: 9702
		[Serializable]
		public class ScrollRectEvent : UnityEvent<Vector2>
		{
		}

		// Token: 0x020025E7 RID: 9703
		public enum ScrollMode
		{
			// Token: 0x0400E97A RID: 59770
			ToStart,
			// Token: 0x0400E97B RID: 59771
			ToCenter,
			// Token: 0x0400E97C RID: 59772
			JustAppear
		}
	}
}
