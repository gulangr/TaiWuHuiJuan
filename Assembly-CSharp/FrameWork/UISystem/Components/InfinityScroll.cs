using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace FrameWork.UISystem.Components
{
	// Token: 0x0200101A RID: 4122
	[RequireComponent(typeof(CScrollRect))]
	public class InfinityScroll : MonoBehaviour, ILanguage
	{
		// Token: 0x17001541 RID: 5441
		// (get) Token: 0x0600BCA5 RID: 48293 RVA: 0x0055C0CE File Offset: 0x0055A2CE
		// (set) Token: 0x0600BCA6 RID: 48294 RVA: 0x0055C0D8 File Offset: 0x0055A2D8
		public InfinityScroll.ScrollDirection Direction
		{
			get
			{
				return this.scrollDirection;
			}
			set
			{
				this.scrollDirection = value;
				CScrollRect scroll = this._scroll;
				if (!true)
				{
				}
				CScrollRect.ScrollDirection direction;
				if (value > InfinityScroll.ScrollDirection.FromBottom)
				{
					if (value - InfinityScroll.ScrollDirection.FromLeft > 1)
					{
						direction = CScrollRect.ScrollDirection.Vertical;
					}
					else
					{
						direction = CScrollRect.ScrollDirection.Horizontal;
					}
				}
				else
				{
					direction = CScrollRect.ScrollDirection.Vertical;
				}
				if (!true)
				{
				}
				scroll.Direction = direction;
				bool flag = this._scroll.ScrollBar == null;
				if (!flag)
				{
					Scrollbar scrollBar = this._scroll.ScrollBar;
					if (!true)
					{
					}
					Scrollbar.Direction direction2;
					switch (value)
					{
					case InfinityScroll.ScrollDirection.FromTop:
						direction2 = Scrollbar.Direction.TopToBottom;
						break;
					case InfinityScroll.ScrollDirection.FromBottom:
						direction2 = Scrollbar.Direction.BottomToTop;
						break;
					case InfinityScroll.ScrollDirection.FromLeft:
						direction2 = Scrollbar.Direction.LeftToRight;
						break;
					case InfinityScroll.ScrollDirection.FromRight:
						direction2 = Scrollbar.Direction.RightToLeft;
						break;
					default:
						direction2 = Scrollbar.Direction.TopToBottom;
						break;
					}
					if (!true)
					{
					}
					scrollBar.direction = direction2;
				}
			}
		}

		// Token: 0x14000094 RID: 148
		// (add) Token: 0x0600BCA7 RID: 48295 RVA: 0x0055C184 File Offset: 0x0055A384
		// (remove) Token: 0x0600BCA8 RID: 48296 RVA: 0x0055C1BC File Offset: 0x0055A3BC
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, GameObject> OnItemRender;

		// Token: 0x14000095 RID: 149
		// (add) Token: 0x0600BCA9 RID: 48297 RVA: 0x0055C1F4 File Offset: 0x0055A3F4
		// (remove) Token: 0x0600BCAA RID: 48298 RVA: 0x0055C22C File Offset: 0x0055A42C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<GameObject> OnItemHide;

		// Token: 0x14000096 RID: 150
		// (add) Token: 0x0600BCAB RID: 48299 RVA: 0x0055C264 File Offset: 0x0055A464
		// (remove) Token: 0x0600BCAC RID: 48300 RVA: 0x0055C29C File Offset: 0x0055A49C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnRenderEnd;

		// Token: 0x14000097 RID: 151
		// (add) Token: 0x0600BCAD RID: 48301 RVA: 0x0055C2D4 File Offset: 0x0055A4D4
		// (remove) Token: 0x0600BCAE RID: 48302 RVA: 0x0055C30C File Offset: 0x0055A50C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<GameObject, LocalStringManager.LanguageType> OnLanguageChanged;

		// Token: 0x17001542 RID: 5442
		// (get) Token: 0x0600BCAF RID: 48303 RVA: 0x0055C341 File Offset: 0x0055A541
		public int CurrentDataCount
		{
			get
			{
				return this._dataCount;
			}
		}

		// Token: 0x17001543 RID: 5443
		// (get) Token: 0x0600BCB0 RID: 48304 RVA: 0x0055C349 File Offset: 0x0055A549
		public CScrollRect Scroll
		{
			get
			{
				return this._scroll;
			}
		}

		// Token: 0x17001544 RID: 5444
		// (get) Token: 0x0600BCB1 RID: 48305 RVA: 0x0055C351 File Offset: 0x0055A551
		public int PageCount
		{
			get
			{
				return this._pageCount;
			}
		}

		// Token: 0x17001545 RID: 5445
		// (get) Token: 0x0600BCB2 RID: 48306 RVA: 0x0055C359 File Offset: 0x0055A559
		private RectTransform.Axis Axis
		{
			get
			{
				return (this.Direction >= InfinityScroll.ScrollDirection.FromLeft) ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
			}
		}

		// Token: 0x17001546 RID: 5446
		// (get) Token: 0x0600BCB3 RID: 48307 RVA: 0x0055C368 File Offset: 0x0055A568
		private RectTransform.Axis AntiAxis
		{
			get
			{
				return (this.Direction >= InfinityScroll.ScrollDirection.FromLeft) ? RectTransform.Axis.Vertical : RectTransform.Axis.Horizontal;
			}
		}

		// Token: 0x17001547 RID: 5447
		// (get) Token: 0x0600BCB4 RID: 48308 RVA: 0x0055C377 File Offset: 0x0055A577
		private float Size
		{
			get
			{
				return (this.Direction >= InfinityScroll.ScrollDirection.FromLeft) ? this._cellSize.x : this._cellSize.y;
			}
		}

		// Token: 0x17001548 RID: 5448
		// (get) Token: 0x0600BCB5 RID: 48309 RVA: 0x0055C39A File Offset: 0x0055A59A
		private float AntiSize
		{
			get
			{
				return (this.Direction >= InfinityScroll.ScrollDirection.FromLeft) ? this._cellSize.y : this._cellSize.x;
			}
		}

		// Token: 0x17001549 RID: 5449
		// (get) Token: 0x0600BCB6 RID: 48310 RVA: 0x0055C3BD File Offset: 0x0055A5BD
		private float GapSize
		{
			get
			{
				return (this.Direction >= InfinityScroll.ScrollDirection.FromLeft) ? this.gap.x : this.gap.y;
			}
		}

		// Token: 0x1700154A RID: 5450
		// (get) Token: 0x0600BCB7 RID: 48311 RVA: 0x0055C3E0 File Offset: 0x0055A5E0
		private float AntiGapSize
		{
			get
			{
				return (this.Direction >= InfinityScroll.ScrollDirection.FromLeft) ? this.gap.y : this.gap.x;
			}
		}

		// Token: 0x1700154B RID: 5451
		// (get) Token: 0x0600BCB8 RID: 48312 RVA: 0x0055C403 File Offset: 0x0055A603
		private float PaddingSize
		{
			get
			{
				return (this.Direction >= InfinityScroll.ScrollDirection.FromLeft) ? this.padding.x : this.padding.y;
			}
		}

		// Token: 0x1700154C RID: 5452
		// (get) Token: 0x0600BCB9 RID: 48313 RVA: 0x0055C426 File Offset: 0x0055A626
		private float AntiPaddingSize
		{
			get
			{
				return (this.Direction >= InfinityScroll.ScrollDirection.FromLeft) ? this.padding.y : this.padding.x;
			}
		}

		// Token: 0x0600BCBA RID: 48314 RVA: 0x0055C449 File Offset: 0x0055A649
		private void Awake()
		{
			this.InitScroll();
		}

		// Token: 0x0600BCBB RID: 48315 RVA: 0x0055C454 File Offset: 0x0055A654
		private void OnEnable()
		{
			bool flag = this._needRefreshAfterUpdateStyle && base.gameObject.activeInHierarchy;
			if (flag)
			{
				base.StartCoroutine(this.DelayUpdateStyle());
			}
			else
			{
				bool activeInHierarchy = base.gameObject.activeInHierarchy;
				if (activeInHierarchy)
				{
					bool pageCountInitialized = this._pageCountInitialized;
					if (!pageCountInitialized)
					{
						base.StartCoroutine(this.DelayInitPageCount());
					}
				}
				else
				{
					bool pageCountInitialized2 = this._pageCountInitialized;
					if (!pageCountInitialized2)
					{
						this.InitPageCount();
					}
				}
			}
		}

		// Token: 0x0600BCBC RID: 48316 RVA: 0x0055C4CC File Offset: 0x0055A6CC
		public void InitPageCount()
		{
			bool flag = this.srcPrefab == null;
			if (!flag)
			{
				this.InitContainer();
				bool hasLayout = this._hasLayout;
				if (hasLayout)
				{
					this._pageCount = int.MaxValue;
				}
				else
				{
					float checkSize = (this.Axis == RectTransform.Axis.Horizontal) ? this._viewPort.rect.width : this._viewPort.rect.height;
					int oldPageCount = this._pageCount;
					this._pageCount = Mathf.CeilToInt((checkSize - this.PaddingSize) / (this.Size + this.GapSize) + (float)this.extraLineCount) * this.lineCount;
					bool flag2 = this._pageCount != oldPageCount && this._dataCount > 0;
					if (flag2)
					{
						this.UpdateShowingRange(false);
						this.ReRender();
					}
				}
				this._pageCountInitialized = true;
			}
		}

		// Token: 0x0600BCBD RID: 48317 RVA: 0x0055C5B1 File Offset: 0x0055A7B1
		public void OnWindowSizeChanged()
		{
			this.InitPageCount();
			this.UpdateShowingRange(false);
		}

		// Token: 0x0600BCBE RID: 48318 RVA: 0x0055C5C3 File Offset: 0x0055A7C3
		private void LateUpdate()
		{
			this.HandlerWaitRefresh();
		}

		// Token: 0x0600BCBF RID: 48319 RVA: 0x0055C5D0 File Offset: 0x0055A7D0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetCellActive(GameObject cell, bool active)
		{
			bool flag = this.isItemCellAwaysActive;
			if (flag)
			{
				RectTransform rectTransform = cell.transform as RectTransform;
				bool flag2 = !active;
				if (flag2)
				{
					bool flag3 = rectTransform.anchoredPosition3D.z > -5000f;
					if (flag3)
					{
						rectTransform.anchoredPosition3D += new Vector3(-10000f, -10000f, -10000f);
					}
				}
				else
				{
					bool flag4 = rectTransform.anchoredPosition3D.z < -5000f;
					if (flag4)
					{
						rectTransform.anchoredPosition3D -= new Vector3(-10000f, -10000f, -10000f);
					}
				}
			}
			else
			{
				bool flag5 = cell.activeSelf != active;
				if (flag5)
				{
					cell.SetActive(active);
				}
			}
		}

		// Token: 0x0600BCC0 RID: 48320 RVA: 0x0055C6A0 File Offset: 0x0055A8A0
		public int GetCellIndex(GameObject cell)
		{
			int index;
			bool flag = this._indexMap != null && this._indexMap.TryGetValue(cell, out index);
			int result;
			if (flag)
			{
				result = index;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x0600BCC1 RID: 48321 RVA: 0x0055C6D4 File Offset: 0x0055A8D4
		private void SetCellPos(GameObject cell, int index)
		{
			bool flag = cell == null;
			if (!flag)
			{
				int column = index % this.lineCount;
				cell.transform.localPosition = Vector3.zero;
				float posValue = this.IndexToContainerValue(index, 1, false);
				RectTransform rectCell = cell.transform as RectTransform;
				Vector3 pos = rectCell.anchoredPosition;
				pos.z = 0f;
				bool flag2 = this.Axis == RectTransform.Axis.Horizontal;
				if (flag2)
				{
					pos.x = posValue;
					pos.y = -(this.AntiPaddingSize + (float)column * (this.AntiGapSize + this.AntiSize));
				}
				else
				{
					pos.x = this.AntiPaddingSize + (float)column * (this.AntiGapSize + this.AntiSize);
					pos.y = posValue;
				}
				rectCell.anchoredPosition3D = pos;
			}
		}

		// Token: 0x0600BCC2 RID: 48322 RVA: 0x0055C7A8 File Offset: 0x0055A9A8
		private float IndexToContainerValue(int index, int flag = 1, bool ignoreBorder = false)
		{
			int line = index / this.lineCount;
			float posValue = (float)line * (this.Size + this.GapSize);
			bool flag2 = !ignoreBorder;
			if (flag2)
			{
				posValue += this.PaddingSize;
			}
			bool flag3 = this.Direction == InfinityScroll.ScrollDirection.FromTop || this.Direction == InfinityScroll.ScrollDirection.FromRight;
			if (flag3)
			{
				posValue *= -1f;
			}
			return posValue * (float)flag;
		}

		// Token: 0x0600BCC3 RID: 48323 RVA: 0x0055C80C File Offset: 0x0055AA0C
		private void InitScroll()
		{
			bool flag = this._scroll != null;
			if (!flag)
			{
				this._scroll = base.GetComponent<CScrollRect>();
				this._container = this._scroll.Content;
				this._viewPort = this._scroll.Viewport;
			}
		}

		// Token: 0x0600BCC4 RID: 48324 RVA: 0x0055C85C File Offset: 0x0055AA5C
		private void InitContainer()
		{
			ILayoutGroup layoutGroup;
			this._hasLayout = base.TryGetComponent<ILayoutGroup>(out layoutGroup);
			this.InitScroll();
			this.lineCount = Mathf.Max(this.lineCount, 1);
			bool flag = this._scroll == null;
			if (!flag)
			{
				this.RefreshStyleMetrics();
				this._cellMap = new Dictionary<int, GameObject>();
				this._indexMap = new Dictionary<GameObject, int>();
				if (this._waitRefreshIndexList == null)
				{
					this._waitRefreshIndexList = new List<int>();
				}
				this._refreshHelperStack = new Stack<int>();
				bool isPlaying = Application.isPlaying;
				if (isPlaying)
				{
					this.srcPrefab.SetActive(false);
					this._scroll.OnScrollEvent -= this.OnScroll;
					this._scroll.OnScrollEvent += this.OnScroll;
					int index = 0;
					for (int i = 0; i < this._container.childCount; i++)
					{
						GameObject child = this._container.GetChild(i).gameObject;
						bool flag2 = child == this.srcPrefab;
						if (!flag2)
						{
							Action<GameObject> onItemHide = this.OnItemHide;
							if (onItemHide != null)
							{
								onItemHide(child);
							}
							this.SetCellActive(child, false);
							this._cellMap.Add(index, child);
							this._indexMap[child] = index;
							this.SetCellPos(child, index);
							this.AddRefreshIndex(index);
							index++;
						}
					}
					this.SetDirty(true);
				}
				this.initSuccess = true;
			}
		}

		// Token: 0x0600BCC5 RID: 48325 RVA: 0x0055C9E0 File Offset: 0x0055ABE0
		private void RefreshStyleMetrics()
		{
			bool flag = this._scroll == null || this.srcPrefab == null;
			if (!flag)
			{
				RectTransform rectPrefab = this.srcPrefab.GetComponent<RectTransform>();
				switch (this.Direction)
				{
				case InfinityScroll.ScrollDirection.FromTop:
				{
					bool flag2 = !this._hasLayout;
					if (flag2)
					{
						this._container.pivot = new Vector2(0.5f, 1f);
					}
					rectPrefab.pivot = Vector2.up;
					break;
				}
				case InfinityScroll.ScrollDirection.FromBottom:
				{
					bool flag3 = !this._hasLayout;
					if (flag3)
					{
						this._container.pivot = new Vector2(0.5f, 0f);
					}
					rectPrefab.pivot = Vector2.zero;
					break;
				}
				case InfinityScroll.ScrollDirection.FromLeft:
				{
					bool flag4 = !this._hasLayout;
					if (flag4)
					{
						this._container.pivot = new Vector2(0f, 0.5f);
					}
					rectPrefab.pivot = Vector2.up;
					break;
				}
				case InfinityScroll.ScrollDirection.FromRight:
				{
					bool flag5 = !this._hasLayout;
					if (flag5)
					{
						this._container.pivot = new Vector2(1f, 0.5f);
					}
					rectPrefab.pivot = Vector2.one;
					break;
				}
				}
				bool flag6 = !this._hasLayout;
				if (flag6)
				{
					this._container.anchorMin = this._container.pivot;
					this._container.anchorMax = this._container.pivot;
				}
				rectPrefab.anchorMin = rectPrefab.pivot;
				rectPrefab.anchorMax = rectPrefab.pivot;
				this._cellSize = rectPrefab.rect.size;
				LayoutElement layoutElement;
				bool flag7 = (Mathf.Approximately(this._cellSize.x, 0f) || Mathf.Approximately(this._cellSize.y, 0f)) && rectPrefab.TryGetComponent<LayoutElement>(out layoutElement);
				if (flag7)
				{
					bool flag8 = !Mathf.Approximately(layoutElement.preferredWidth, 0f);
					if (flag8)
					{
						this._cellSize.x = layoutElement.preferredWidth;
					}
					bool flag9 = !Mathf.Approximately(layoutElement.preferredHeight, 0f);
					if (flag9)
					{
						this._cellSize.y = layoutElement.preferredHeight;
					}
				}
				bool flag10 = !this._hasLayout;
				if (flag10)
				{
					float containerAntiSize = this.AntiPaddingSize * 2f + (this.AntiGapSize + this.AntiSize) * (float)(this.lineCount - 1) + this.AntiSize;
					this._container.SetSizeWithCurrentAnchors(this.AntiAxis, containerAntiSize);
					this._container.anchoredPosition = Vector2.zero;
					this._scroll.UpdateScrollBarValue();
				}
			}
		}

		// Token: 0x0600BCC6 RID: 48326 RVA: 0x0055CC9B File Offset: 0x0055AE9B
		private void RenderItem(int index, GameObject cell)
		{
			cell.name = index.ToString();
			Profiler.BeginSample("InfinityScroll.RenderItem");
			Action<int, GameObject> onItemRender = this.OnItemRender;
			if (onItemRender != null)
			{
				onItemRender(index, cell);
			}
			Profiler.EndSample();
			this.SetCellActive(cell, true);
		}

		// Token: 0x0600BCC7 RID: 48327 RVA: 0x0055CCDC File Offset: 0x0055AEDC
		private GameObject GetCell()
		{
			GameObject cell = null;
			bool flag = cell == null;
			if (flag)
			{
				cell = Object.Instantiate<GameObject>(this.srcPrefab, this._container, false);
				cell.SetActive(true);
			}
			return cell;
		}

		// Token: 0x0600BCC8 RID: 48328 RVA: 0x0055CD19 File Offset: 0x0055AF19
		private void SetDirty(bool value = true)
		{
			this._dirtyFlag = value;
		}

		// Token: 0x0600BCC9 RID: 48329 RVA: 0x0055CD24 File Offset: 0x0055AF24
		private void AddRefreshIndex(int index)
		{
			if (this._waitRefreshIndexList == null)
			{
				this._waitRefreshIndexList = new List<int>();
			}
			bool flag = !this._waitRefreshIndexList.Contains(index);
			if (flag)
			{
				this._waitRefreshIndexList.Add(index);
			}
			this.SetDirty(this._waitRefreshIndexList.Count > 0);
		}

		// Token: 0x0600BCCA RID: 48330 RVA: 0x0055CD7C File Offset: 0x0055AF7C
		private void CleanupDestroyedCells()
		{
			bool flag = this._cellMap == null || this._cellMap.Count == 0;
			if (!flag)
			{
				List<int> keysToRemove = EasyPool.Get<List<int>>();
				keysToRemove.Clear();
				foreach (KeyValuePair<int, GameObject> pair in this._cellMap)
				{
					bool flag2 = pair.Value == null;
					if (flag2)
					{
						keysToRemove.Add(pair.Key);
					}
				}
				foreach (int key in keysToRemove)
				{
					this._cellMap.Remove(key);
				}
				List<GameObject> cellsToRemove = EasyPool.Get<List<GameObject>>();
				cellsToRemove.Clear();
				foreach (KeyValuePair<GameObject, int> keyValuePair in this._indexMap)
				{
					GameObject gameObject;
					int num;
					keyValuePair.Deconstruct(out gameObject, out num);
					GameObject key2 = gameObject;
					bool flag3 = key2 == null;
					if (flag3)
					{
						cellsToRemove.Add(key2);
					}
				}
				foreach (GameObject cell in cellsToRemove)
				{
					this._indexMap.Remove(cell);
				}
				EasyPool.Free<List<GameObject>>(cellsToRemove);
				EasyPool.Free<List<int>>(keysToRemove);
			}
		}

		// Token: 0x0600BCCB RID: 48331 RVA: 0x0055CF34 File Offset: 0x0055B134
		private void HandlerWaitRefresh()
		{
			bool flag = !this._dirtyFlag;
			if (!flag)
			{
				bool flag2 = !this.initSuccess;
				if (flag2)
				{
					this.SetDirty(false);
				}
				else
				{
					this.CleanupDestroyedCells();
					this._refreshHelperStack.Clear();
					this._showingRange.y = Mathf.Min(this._showingRange.y, this._dataCount - 1);
					bool flag3 = this._showingRange.y - this._showingRange.x < this._pageCount;
					if (flag3)
					{
						this._showingRange.x = Mathf.Max(this._showingRange.y - this._pageCount, 0);
					}
					foreach (KeyValuePair<int, GameObject> pair in this._cellMap)
					{
						bool flag4 = pair.Key < this._showingRange.x || pair.Key > this._showingRange.y || pair.Key >= this._dataCount;
						if (flag4)
						{
							this._refreshHelperStack.Push(pair.Key);
							Action<GameObject> onItemHide = this.OnItemHide;
							if (onItemHide != null)
							{
								onItemHide(pair.Value);
							}
							bool flag5 = pair.Value != null;
							if (flag5)
							{
								this.SetCellActive(pair.Value, false);
							}
						}
					}
					bool flag6 = this._dataCount <= 0;
					if (flag6)
					{
						this.SetDirty(false);
						Action onRenderEnd = this.OnRenderEnd;
						if (onRenderEnd != null)
						{
							onRenderEnd();
						}
					}
					else
					{
						for (int i = this._showingRange.x; i <= this._showingRange.y; i++)
						{
							GameObject cell;
							bool flag7 = !this._cellMap.TryGetValue(i, out cell);
							if (flag7)
							{
								bool flag8 = this._refreshHelperStack.Count > 0;
								if (flag8)
								{
									int reuseIndex = this._refreshHelperStack.Pop();
									this._cellMap.TryGetValue(reuseIndex, out cell);
									bool flag9 = cell != null;
									if (flag9)
									{
										this._indexMap.Remove(cell);
									}
									this._cellMap.Remove(reuseIndex);
								}
								bool flag10 = cell == null;
								if (flag10)
								{
									cell = this.GetCell();
								}
								this._indexMap[cell] = i;
								this.SetCellPos(cell, i);
								this._cellMap.Add(i, cell);
								this.AddRefreshIndex(i);
							}
							bool flag11 = cell != null;
							if (flag11)
							{
								this.SetCellActive(cell, true);
							}
						}
						try
						{
							Profiler.BeginSample("InfinityScroll.RenderItems");
							for (int j = 0; j < this._waitRefreshIndexList.Count; j++)
							{
								int index = this._waitRefreshIndexList[j];
								bool flag12 = index < this._showingRange.x || index > this._showingRange.y || index >= this._dataCount || index < 0;
								if (!flag12)
								{
									GameObject cell2;
									bool flag13 = !this._cellMap.TryGetValue(index, out cell2);
									if (!flag13)
									{
										this.RenderItem(index, cell2);
									}
								}
							}
							Profiler.EndSample();
						}
						catch (Exception e)
						{
							AdaptableLog.Error(e.ToString());
						}
						finally
						{
							this._waitRefreshIndexList.Clear();
							this._refreshHelperStack.Clear();
							this.SetDirty(false);
							Profiler.BeginSample("InfinityScroll.OnRenderEnd");
							Action onRenderEnd2 = this.OnRenderEnd;
							if (onRenderEnd2 != null)
							{
								onRenderEnd2();
							}
							Profiler.EndSample();
						}
					}
				}
			}
		}

		// Token: 0x0600BCCC RID: 48332 RVA: 0x0055D32C File Offset: 0x0055B52C
		private void UpdateShowingRange(bool adjustPos = false)
		{
			Vector2 anchoredPosition = this._container.anchoredPosition;
			float posValue = (this.Axis == RectTransform.Axis.Horizontal) ? anchoredPosition.x : anchoredPosition.y;
			bool flag = this.Direction == InfinityScroll.ScrollDirection.FromTop || this.Direction == InfinityScroll.ScrollDirection.FromRight;
			if (flag)
			{
				posValue = Mathf.Max(0f, posValue);
			}
			else
			{
				posValue = Mathf.Min(0f, posValue);
			}
			posValue = Mathf.Abs(posValue);
			this._showingRange.x = Mathf.Clamp(Mathf.FloorToInt((posValue - this.PaddingSize) / (this.GapSize + this.Size)) * this.lineCount, 0, Mathf.Max(0, this._dataCount - this._pageCount - 1));
			this._showingRange.y = Mathf.Min(this._showingRange.x + this._pageCount, this._dataCount - 1);
			bool flag2 = this._showingRange.y % this.lineCount == 0 && this.lineCount > 1;
			if (flag2)
			{
				this._showingRange.y = Mathf.Min(this._showingRange.y + this.lineCount - 1, this._dataCount - 1);
			}
			if (adjustPos)
			{
				float posMax = (this.Axis == RectTransform.Axis.Vertical) ? (this._container.rect.height - this._viewPort.rect.height) : (this._container.rect.width - this._viewPort.rect.width);
				bool flag3 = posValue > posMax;
				if (flag3)
				{
					this.ScrollTo(this._dataCount, 0f);
				}
			}
		}

		// Token: 0x0600BCCD RID: 48333 RVA: 0x0055D4E0 File Offset: 0x0055B6E0
		public void SetDataCount(int count)
		{
			bool flag = !this.initSuccess;
			if (flag)
			{
				this.InitContainer();
			}
			this._dataCount = count;
			float totalSize = this.PaddingSize * 2f + (this.Size + this.GapSize) * (float)(Mathf.CeilToInt((float)this._dataCount / (float)this.lineCount) - 1) + this.Size;
			bool flag2 = !this._hasLayout;
			if (flag2)
			{
				this._container.SetSizeWithCurrentAnchors(this.Axis, totalSize);
			}
			bool flag3 = this.isSetContentSizeToView;
			if (flag3)
			{
				base.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(this.Axis, totalSize);
			}
			bool needRefreshAfterUpdateStyle = this._needRefreshAfterUpdateStyle;
			if (needRefreshAfterUpdateStyle)
			{
				this._needRefreshAfterUpdateStyle = false;
				this._pageCountInitialized = false;
				this.InitPageCount();
				this._scroll.SetScrollEnable(true);
			}
			this.UpdateShowingRange(true);
			this.ReRender();
			bool flag4 = this.emptyObject != null;
			if (flag4)
			{
				this.emptyObject.SetActive(this._dataCount <= 0);
			}
		}

		// Token: 0x0600BCCE RID: 48334 RVA: 0x0055D5E9 File Offset: 0x0055B7E9
		public void UpdateData(int newCount)
		{
			this.SetDataCount(newCount);
		}

		// Token: 0x0600BCCF RID: 48335 RVA: 0x0055D5F4 File Offset: 0x0055B7F4
		public void ReRender()
		{
			for (int i = this._showingRange.x; i <= this._showingRange.y; i++)
			{
				bool flag = i < this._dataCount;
				if (flag)
				{
					this.AddRefreshIndex(i);
				}
			}
			this.SetDirty(true);
		}

		// Token: 0x0600BCD0 RID: 48336 RVA: 0x0055D648 File Offset: 0x0055B848
		public void RefreshDisplayRange()
		{
			Vector2Int preRange = this._showingRange;
			this.UpdateShowingRange(false);
			bool flag = preRange.x != this._showingRange.x || preRange.y != this._showingRange.y;
			if (flag)
			{
				this.ReRender();
			}
		}

		// Token: 0x0600BCD1 RID: 48337 RVA: 0x0055D6A0 File Offset: 0x0055B8A0
		public void Refresh(int index = 0)
		{
			bool flag2 = !this.initSuccess;
			if (!flag2)
			{
				Vector3 anchoredPos = this._container.anchoredPosition;
				float value = this.IndexToContainerValue(index, -1, true);
				float curValue = (this.Axis == RectTransform.Axis.Horizontal) ? anchoredPos.x : anchoredPos.y;
				float viewSize = (this.Axis == RectTransform.Axis.Horizontal) ? this._viewPort.rect.width : this._viewPort.rect.height;
				float containerSize = (this.Axis == RectTransform.Axis.Horizontal) ? this._container.rect.width : this._container.rect.height;
				bool flag3 = (curValue > value - viewSize + this.Size && curValue < value) || containerSize <= viewSize;
				if (flag3)
				{
					Action onRenderEnd = this.OnRenderEnd;
					if (onRenderEnd != null)
					{
						onRenderEnd();
					}
					Action onScrollEnd = this._scroll.OnScrollEnd;
					if (onScrollEnd != null)
					{
						onScrollEnd();
					}
				}
				else
				{
					bool flag4 = curValue < value - viewSize;
					if (flag4)
					{
						value = value - viewSize + this.Size * 1.2f;
					}
					int flag = -1;
					bool flag5 = this.Direction == InfinityScroll.ScrollDirection.FromTop || this.Direction == InfinityScroll.ScrollDirection.FromRight;
					if (flag5)
					{
						flag = 1;
					}
					value = Mathf.Clamp(value, 0f, (containerSize - viewSize) * (float)flag);
					bool flag6 = this.Axis == RectTransform.Axis.Horizontal;
					if (flag6)
					{
						anchoredPos.x = value;
					}
					else
					{
						anchoredPos.y = value;
					}
					this._scroll.ScrollTo(anchoredPos, 0f);
				}
			}
		}

		// Token: 0x0600BCD2 RID: 48338 RVA: 0x0055D835 File Offset: 0x0055BA35
		public void RefreshCell(int index)
		{
			this.AddRefreshIndex(index);
		}

		// Token: 0x0600BCD3 RID: 48339 RVA: 0x0055D840 File Offset: 0x0055BA40
		public GameObject GetActiveCell(int index)
		{
			Dictionary<int, GameObject> cellMap = this._cellMap;
			return (cellMap != null) ? cellMap.GetValueOrDefault(index) : null;
		}

		// Token: 0x0600BCD4 RID: 48340 RVA: 0x0055D855 File Offset: 0x0055BA55
		public void ScrollToEnd()
		{
			this.ScrollTo(this._dataCount, 0.3f);
		}

		// Token: 0x0600BCD5 RID: 48341 RVA: 0x0055D86C File Offset: 0x0055BA6C
		public void ClearCache()
		{
			bool flag = this._cellMap != null;
			if (flag)
			{
				foreach (KeyValuePair<int, GameObject> pair in this._cellMap)
				{
					bool flag2 = pair.Value != null;
					if (flag2)
					{
						Action<GameObject> onItemHide = this.OnItemHide;
						if (onItemHide != null)
						{
							onItemHide(pair.Value);
						}
						bool isPlaying = Application.isPlaying;
						if (isPlaying)
						{
							Object.Destroy(pair.Value);
						}
						else
						{
							Object.DestroyImmediate(pair.Value);
						}
					}
				}
				this._cellMap.Clear();
			}
			bool flag3 = this._indexMap != null;
			if (flag3)
			{
				this._indexMap.Clear();
			}
			List<int> waitRefreshIndexList = this._waitRefreshIndexList;
			if (waitRefreshIndexList != null)
			{
				waitRefreshIndexList.Clear();
			}
			Stack<int> refreshHelperStack = this._refreshHelperStack;
			if (refreshHelperStack != null)
			{
				refreshHelperStack.Clear();
			}
			this._showingRange = Vector2Int.zero;
			this._dirtyFlag = false;
			this._dataCount = 0;
			this._pageCountInitialized = false;
			this.initSuccess = false;
			bool flag4 = this.emptyObject != null;
			if (flag4)
			{
				this.emptyObject.SetActive(true);
			}
		}

		// Token: 0x0600BCD6 RID: 48342 RVA: 0x0055D9B4 File Offset: 0x0055BBB4
		public void ScrollTo(int index, float duration = 0.3f)
		{
			bool flag2 = !this.initSuccess;
			if (!flag2)
			{
				index = Mathf.Clamp(index, 0, this._dataCount);
				float posValue = this.IndexToContainerValue(index, -1, true);
				float containerPos = Mathf.Abs((this.Axis == RectTransform.Axis.Vertical) ? this._container.anchoredPosition.y : this._container.anchoredPosition.x);
				float containerSize = (this.Axis == RectTransform.Axis.Vertical) ? this._container.rect.height : this._container.rect.width;
				float viewSize = (this.Axis == RectTransform.Axis.Horizontal) ? this._viewPort.rect.width : this._viewPort.rect.height;
				bool flag3 = Mathf.Abs(posValue) >= containerPos && Mathf.Abs(posValue) <= containerPos + viewSize;
				if (!flag3)
				{
					int flag = -1;
					bool flag4 = this.Direction == InfinityScroll.ScrollDirection.FromTop || this.Direction == InfinityScroll.ScrollDirection.FromRight;
					if (flag4)
					{
						flag = 1;
					}
					bool flag5 = containerSize <= viewSize;
					if (flag5)
					{
						posValue = 0f;
					}
					else
					{
						bool flag6 = Mathf.Abs(posValue) - containerSize > viewSize;
						if (flag6)
						{
							posValue = (float)flag * (containerSize - viewSize);
						}
					}
					float maxScroll = containerSize - viewSize;
					float min = Mathf.Min(0f, maxScroll * (float)flag);
					float max = Mathf.Max(0f, maxScroll * (float)flag);
					posValue = Mathf.Clamp(posValue, min, max);
					bool flag7 = this.Axis == RectTransform.Axis.Horizontal;
					if (flag7)
					{
						this._scroll.ScrollTo(new Vector2(posValue, 0f), duration);
					}
					else
					{
						this._scroll.ScrollTo(new Vector2(0f, posValue), duration);
					}
				}
			}
		}

		// Token: 0x0600BCD7 RID: 48343 RVA: 0x0055DB70 File Offset: 0x0055BD70
		public bool NeedScroll(int index)
		{
			bool flag = !this.initSuccess;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				index = Mathf.Clamp(index, 0, this._dataCount);
				float posValue = this.IndexToContainerValue(index, -1, true);
				float containerPos = Mathf.Abs((this.Axis == RectTransform.Axis.Vertical) ? this._container.anchoredPosition.y : this._container.anchoredPosition.x);
				float viewSize = (this.Axis == RectTransform.Axis.Horizontal) ? this._viewPort.rect.width : this._viewPort.rect.height;
				result = (Mathf.Abs(posValue) < containerPos || posValue > containerPos + viewSize);
			}
			return result;
		}

		// Token: 0x0600BCD8 RID: 48344 RVA: 0x0055DC28 File Offset: 0x0055BE28
		public void UpdateStyle(GameObject srcPrefab, int count)
		{
			this._dataCount = count;
			bool flag = !this.initSuccess;
			if (flag)
			{
				this.InitContainer();
			}
			this.UpdateStyle(this.Direction, this.lineCount, this.gap, this.padding, srcPrefab);
		}

		// Token: 0x0600BCD9 RID: 48345 RVA: 0x0055DC74 File Offset: 0x0055BE74
		public void UpdateStyle(InfinityScroll.ScrollDirection dir, int lineCount, Vector2 gap, Vector2 padding, GameObject srcPrefab = null)
		{
			bool flag = this.srcPrefab == null;
			if (!flag)
			{
				bool flag2 = !this.initSuccess;
				if (flag2)
				{
					this.InitContainer();
				}
				this._needRefreshAfterUpdateStyle = true;
				this._scroll.SetScrollEnable(false);
				for (int i = this._container.childCount - 1; i >= 0; i--)
				{
					GameObject child = this._container.GetChild(i).gameObject;
					bool flag3 = child == this.srcPrefab;
					if (!flag3)
					{
						bool flag4 = srcPrefab != null;
						if (flag4)
						{
							Action<GameObject> onItemHide = this.OnItemHide;
							if (onItemHide != null)
							{
								onItemHide(child);
							}
							Object.Destroy(child);
						}
					}
				}
				bool flag5 = srcPrefab != null;
				if (flag5)
				{
					this.srcPrefab = srcPrefab;
				}
				this._cellMap.Clear();
				this._indexMap.Clear();
				this._showingRange = Vector2Int.zero;
				this.Direction = dir;
				this.lineCount = lineCount;
				this.gap = gap;
				this.padding = padding;
				this.RefreshStyleMetrics();
				bool activeInHierarchy = base.gameObject.activeInHierarchy;
				if (activeInHierarchy)
				{
					base.StartCoroutine(this.DelayUpdateStyle());
				}
			}
		}

		// Token: 0x0600BCDA RID: 48346 RVA: 0x0055DDAE File Offset: 0x0055BFAE
		private IEnumerator DelayUpdateStyle()
		{
			bool flag = !base.gameObject.activeInHierarchy;
			if (flag)
			{
				yield break;
			}
			yield return new WaitForEndOfFrame();
			bool flag2 = !this._needRefreshAfterUpdateStyle;
			if (flag2)
			{
				yield break;
			}
			this.InitPageCount();
			bool flag3 = !this._hasLayout;
			if (flag3)
			{
				this._container.SetSizeWithCurrentAnchors(this.Axis, this.PaddingSize * 2f + (this.Size + this.GapSize) * (float)(Mathf.CeilToInt((float)this._dataCount / (float)this.lineCount) - 1) + this.Size);
			}
			this._showingRange.y = Mathf.Min(this._showingRange.x + this._pageCount, this._dataCount - 1);
			bool flag4 = this._showingRange.y - this._showingRange.x < this._pageCount;
			if (flag4)
			{
				this._showingRange.x = Mathf.Max(this._showingRange.y - this._pageCount, 0);
			}
			this.ReRender();
			this._scroll.SetScrollEnable(true);
			this._needRefreshAfterUpdateStyle = false;
			bool flag5 = this.emptyObject != null;
			if (flag5)
			{
				this.emptyObject.SetActive(this._dataCount <= 0);
			}
			yield break;
		}

		// Token: 0x0600BCDB RID: 48347 RVA: 0x0055DDBD File Offset: 0x0055BFBD
		private IEnumerator DelayInitPageCount()
		{
			bool flag = !base.gameObject.activeInHierarchy;
			if (flag)
			{
				yield break;
			}
			yield return new WaitForEndOfFrame();
			this.InitPageCount();
			this.UpdateShowingRange(false);
			this.ReRender();
			yield break;
		}

		// Token: 0x0600BCDC RID: 48348 RVA: 0x0055DDCC File Offset: 0x0055BFCC
		private void OnScroll()
		{
			bool flag = !this.initSuccess;
			if (!flag)
			{
				int prevX = this._showingRange.x;
				int prevY = this._showingRange.y;
				this.UpdateShowingRange(false);
				bool flag2 = prevX != this._showingRange.x || prevY != this._showingRange.y;
				if (flag2)
				{
					bool flag3 = this._showingRange.x < prevX;
					if (flag3)
					{
						for (int i = this._showingRange.x; i < prevX; i++)
						{
							this.AddRefreshIndex(i);
						}
					}
					bool flag4 = this._showingRange.y > prevY;
					if (flag4)
					{
						for (int j = prevY; j < this._showingRange.y; j++)
						{
							this.AddRefreshIndex(j);
						}
					}
					this.SetDirty(true);
				}
			}
		}

		// Token: 0x0600BCDD RID: 48349 RVA: 0x0055DEBD File Offset: 0x0055C0BD
		public void AddOnScrollEvent(Action action)
		{
			this.InitScroll();
			this._scroll.OnScrollEvent += action;
		}

		// Token: 0x0600BCDE RID: 48350 RVA: 0x0055DED4 File Offset: 0x0055C0D4
		public void RemoveOnScrollEvent(Action action)
		{
			this.InitScroll();
			this._scroll.OnScrollEvent -= action;
		}

		// Token: 0x0600BCDF RID: 48351 RVA: 0x0055DEEC File Offset: 0x0055C0EC
		public int GetPageMaxCount()
		{
			this.InitScroll();
			float height = this._viewPort.rect.height - this.padding.y;
			float itemHeight = this.srcPrefab.GetComponent<RectTransform>().rect.height;
			float count = height / (itemHeight + this.gap.y);
			return (int)count;
		}

		// Token: 0x0600BCE0 RID: 48352 RVA: 0x0055DF54 File Offset: 0x0055C154
		public float GetCellHeightPosition(int index)
		{
			int line = index / this.lineCount + 1;
			float itemHeight = this.srcPrefab.GetComponent<RectTransform>().rect.height;
			return MathF.Max(0f, (itemHeight + this.gap.y) * (float)line - this._viewPort.rect.height);
		}

		// Token: 0x0600BCE1 RID: 48353 RVA: 0x0055DFB8 File Offset: 0x0055C1B8
		public bool IsInViewport(int index)
		{
			bool flag = !this.initSuccess || index < 0 || index >= this._dataCount;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Dictionary<int, GameObject> cellMap = this._cellMap;
				GameObject cell = (cellMap != null) ? cellMap.GetValueOrDefault(index) : null;
				bool flag2 = cell == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					RectTransform cellRect;
					bool flag3 = !cell.TryGetComponent<RectTransform>(out cellRect);
					if (flag3)
					{
						result = false;
					}
					else
					{
						Vector3[] viewportCorners = new Vector3[4];
						Vector3[] cellCorners = new Vector3[4];
						this._viewPort.GetWorldCorners(viewportCorners);
						cellRect.GetWorldCorners(cellCorners);
						result = (cellCorners[0].x >= viewportCorners[0].x && cellCorners[2].x <= viewportCorners[2].x && cellCorners[0].y >= viewportCorners[0].y && cellCorners[2].y <= viewportCorners[2].y);
					}
				}
			}
			return result;
		}

		// Token: 0x0600BCE2 RID: 48354 RVA: 0x0055E0C8 File Offset: 0x0055C2C8
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			bool flag = !this.initSuccess;
			if (!flag)
			{
				Action<GameObject, LocalStringManager.LanguageType> onLanguageChanged = this.OnLanguageChanged;
				if (onLanguageChanged != null)
				{
					onLanguageChanged(base.gameObject, languageType);
				}
				foreach (KeyValuePair<int, GameObject> keyValuePair in this._cellMap)
				{
					int num;
					GameObject gameObject;
					keyValuePair.Deconstruct(out num, out gameObject);
					GameObject obj = gameObject;
					bool flag2 = obj != null && obj != this.srcPrefab;
					if (flag2)
					{
						Action<GameObject, LocalStringManager.LanguageType> onLanguageChanged2 = this.OnLanguageChanged;
						if (onLanguageChanged2 != null)
						{
							onLanguageChanged2(obj, languageType);
						}
					}
				}
			}
		}

		// Token: 0x04009130 RID: 37168
		public GameObject srcPrefab;

		// Token: 0x04009131 RID: 37169
		public int extraLineCount = 2;

		// Token: 0x04009132 RID: 37170
		[SerializeField]
		private InfinityScroll.ScrollDirection scrollDirection;

		// Token: 0x04009133 RID: 37171
		[Tooltip("单排元素数量")]
		public int lineCount = 1;

		// Token: 0x04009134 RID: 37172
		[Tooltip("元素间距")]
		public Vector2 gap = new Vector2(10f, 10f);

		// Token: 0x04009135 RID: 37173
		[Tooltip("元素与各边的间距")]
		public Vector2 padding = new Vector2(10f, 10f);

		// Token: 0x04009136 RID: 37174
		[Tooltip("没有任何元素时显示的物体")]
		public GameObject emptyObject;

		// Token: 0x04009137 RID: 37175
		[Tooltip("不关闭元素，避免Active切换带来的开销，Item内使用OnItemHide自行管理，将会使用z轴管理可见不可见状态，可使用的范围限制为-4999~4999")]
		public bool isItemCellAwaysActive = false;

		// Token: 0x0400913C RID: 37180
		private int _dataCount;

		// Token: 0x0400913D RID: 37181
		private CScrollRect _scroll;

		// Token: 0x0400913E RID: 37182
		private RectTransform _viewPort;

		// Token: 0x0400913F RID: 37183
		private RectTransform _container;

		// Token: 0x04009140 RID: 37184
		[NonSerialized]
		public bool initSuccess;

		// Token: 0x04009141 RID: 37185
		private Vector2 _cellSize = Vector2.zero;

		// Token: 0x04009142 RID: 37186
		[SerializeField]
		[ReadOnly]
		private int _pageCount;

		// Token: 0x04009143 RID: 37187
		private Dictionary<int, GameObject> _cellMap;

		// Token: 0x04009144 RID: 37188
		private Dictionary<GameObject, int> _indexMap;

		// Token: 0x04009145 RID: 37189
		private Vector2Int _showingRange;

		// Token: 0x04009146 RID: 37190
		private List<int> _waitRefreshIndexList;

		// Token: 0x04009147 RID: 37191
		private Stack<int> _refreshHelperStack;

		// Token: 0x04009148 RID: 37192
		private bool _dirtyFlag;

		// Token: 0x04009149 RID: 37193
		private bool _needRefreshAfterUpdateStyle;

		// Token: 0x0400914A RID: 37194
		private bool _hasLayout;

		// Token: 0x0400914B RID: 37195
		private bool _pageCountInitialized;

		// Token: 0x0400914C RID: 37196
		[HideInInspector]
		public bool isSetContentSizeToView;

		// Token: 0x02002667 RID: 9831
		public enum ScrollDirection
		{
			// Token: 0x0400EA7E RID: 60030
			FromTop,
			// Token: 0x0400EA7F RID: 60031
			FromBottom,
			// Token: 0x0400EA80 RID: 60032
			FromLeft,
			// Token: 0x0400EA81 RID: 60033
			FromRight
		}
	}
}
