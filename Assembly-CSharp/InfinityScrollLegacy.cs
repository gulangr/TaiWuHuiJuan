using System;
using System.Collections;
using System.Collections.Generic;
using FrameWork;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200006F RID: 111
[Obsolete]
[RequireComponent(typeof(CScrollRectLegacy))]
public class InfinityScrollLegacy : MonoBehaviour, ILanguage
{
	// Token: 0x1700006D RID: 109
	// (get) Token: 0x060003E5 RID: 997 RVA: 0x00017B92 File Offset: 0x00015D92
	// (set) Token: 0x060003E6 RID: 998 RVA: 0x00017B9C File Offset: 0x00015D9C
	public InfinityScrollLegacy.ScrollDirection Direction
	{
		get
		{
			return this.GetDirectionByScrollbarDirection();
		}
		set
		{
			switch (value)
			{
			case InfinityScrollLegacy.ScrollDirection.FromTop:
				this._scroll.ScrollBar.direction = Scrollbar.Direction.TopToBottom;
				break;
			case InfinityScrollLegacy.ScrollDirection.FromBottom:
				this._scroll.ScrollBar.direction = Scrollbar.Direction.BottomToTop;
				break;
			case InfinityScrollLegacy.ScrollDirection.FromLeft:
				this._scroll.ScrollBar.direction = Scrollbar.Direction.LeftToRight;
				break;
			case InfinityScrollLegacy.ScrollDirection.FromRight:
				this._scroll.ScrollBar.direction = Scrollbar.Direction.RightToLeft;
				break;
			}
		}
	}

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060003E7 RID: 999 RVA: 0x00017C1A File Offset: 0x00015E1A
	public int CurrentDataCount
	{
		get
		{
			return this._dataCount;
		}
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x060003E8 RID: 1000 RVA: 0x00017C24 File Offset: 0x00015E24
	public CScrollRectLegacy Scroll
	{
		get
		{
			return this._scroll;
		}
	}

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x060003E9 RID: 1001 RVA: 0x00017C3C File Offset: 0x00015E3C
	public int PageCount
	{
		get
		{
			return this._pageCount;
		}
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x060003EA RID: 1002 RVA: 0x00017C44 File Offset: 0x00015E44
	public CToggleGroupObsolete TogGroup
	{
		get
		{
			return this._togGroup;
		}
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x060003EB RID: 1003 RVA: 0x00017C4C File Offset: 0x00015E4C
	// (set) Token: 0x060003EC RID: 1004 RVA: 0x00017C54 File Offset: 0x00015E54
	public int SelectedTogKey
	{
		get
		{
			return this._selectedTogKey;
		}
		set
		{
			this._selectedTogKey = value;
			bool flag = this._selectedKeysList == null;
			if (flag)
			{
				this._selectedKeysList = new List<int>();
			}
			bool flag2 = value < 0;
			if (flag2)
			{
				this._selectedKeysList.ForEach(new Action<int>(this.AddRefreshIndex));
				this._selectedKeysList.Clear();
			}
			else
			{
				bool flag3 = !this._selectedKeysList.Contains(value);
				if (flag3)
				{
					bool flag4 = null != this._togGroup && this._selectedKeysList.Count >= this._togGroup.AllOnNum;
					if (flag4)
					{
						int removeKey = this._selectedKeysList[0];
						this._selectedKeysList.RemoveAt(0);
						this.AddRefreshIndex(removeKey);
					}
					this._selectedKeysList.Add(value);
				}
				this.AddRefreshIndex(value);
			}
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060003ED RID: 1005 RVA: 0x00017D33 File Offset: 0x00015F33
	private RectTransform.Axis Axis
	{
		get
		{
			return (this.Direction >= InfinityScrollLegacy.ScrollDirection.FromLeft) ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
		}
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060003EE RID: 1006 RVA: 0x00017D42 File Offset: 0x00015F42
	private RectTransform.Axis AntiAxis
	{
		get
		{
			return (this.Direction >= InfinityScrollLegacy.ScrollDirection.FromLeft) ? RectTransform.Axis.Vertical : RectTransform.Axis.Horizontal;
		}
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060003EF RID: 1007 RVA: 0x00017D51 File Offset: 0x00015F51
	private float Size
	{
		get
		{
			return (this.Direction >= InfinityScrollLegacy.ScrollDirection.FromLeft) ? this._cellSize.x : this._cellSize.y;
		}
	}

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060003F0 RID: 1008 RVA: 0x00017D74 File Offset: 0x00015F74
	private float AntiSize
	{
		get
		{
			return (this.Direction >= InfinityScrollLegacy.ScrollDirection.FromLeft) ? this._cellSize.y : this._cellSize.x;
		}
	}

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x060003F1 RID: 1009 RVA: 0x00017D97 File Offset: 0x00015F97
	private float GapSize
	{
		get
		{
			return (this.Direction >= InfinityScrollLegacy.ScrollDirection.FromLeft) ? this.Gap.x : this.Gap.y;
		}
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x060003F2 RID: 1010 RVA: 0x00017DBA File Offset: 0x00015FBA
	private float AntiGapSize
	{
		get
		{
			return (this.Direction >= InfinityScrollLegacy.ScrollDirection.FromLeft) ? this.Gap.y : this.Gap.x;
		}
	}

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x060003F3 RID: 1011 RVA: 0x00017DDD File Offset: 0x00015FDD
	private float PaddingSize
	{
		get
		{
			return (this.Direction >= InfinityScrollLegacy.ScrollDirection.FromLeft) ? this.Padding.x : this.Padding.y;
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x060003F4 RID: 1012 RVA: 0x00017E00 File Offset: 0x00016000
	private float AntiPaddingSize
	{
		get
		{
			return (this.Direction >= InfinityScrollLegacy.ScrollDirection.FromLeft) ? this.Padding.y : this.Padding.x;
		}
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x00017E23 File Offset: 0x00016023
	private void Awake()
	{
		this.InitScroll();
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x00017E30 File Offset: 0x00016030
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

	// Token: 0x060003F7 RID: 1015 RVA: 0x00017EA8 File Offset: 0x000160A8
	public void InitPageCount()
	{
		bool flag = null == this.SrcPrefab;
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
				this._pageCount = Mathf.CeilToInt((checkSize - this.PaddingSize) / (this.Size + this.GapSize) + (float)this.extraLineCount) * this.LineCount;
			}
			this._pageCountInitialized = true;
		}
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x00017F55 File Offset: 0x00016155
	public void OnWindowSizeChanged()
	{
		this.InitPageCount();
		this.UpdateShowingRange(false);
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x00017F67 File Offset: 0x00016167
	private void LateUpdate()
	{
		this.HandlerWaitRefresh();
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x00017F74 File Offset: 0x00016174
	private void SetRefersPos(Refers refer)
	{
		bool flag = null == refer;
		if (!flag)
		{
			int column = refer.UserInt % this.LineCount;
			refer.transform.localPosition = Vector3.zero;
			float posValue = this.IndexToContainerValue(refer.UserInt, 1, false);
			RectTransform rectRefer = refer.GetComponent<RectTransform>();
			Vector2 pos = rectRefer.anchoredPosition;
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
			refer.GetComponent<RectTransform>().anchoredPosition = pos;
		}
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00018040 File Offset: 0x00016240
	private float IndexToContainerValue(int index, int flag = 1, bool ignoreBorder = false)
	{
		int line = index / this.LineCount;
		float posValue = (float)line * (this.Size + this.GapSize);
		bool flag2 = !ignoreBorder;
		if (flag2)
		{
			posValue += this.PaddingSize;
		}
		bool flag3 = this.Direction == InfinityScrollLegacy.ScrollDirection.FromTop || this.Direction == InfinityScrollLegacy.ScrollDirection.FromRight;
		if (flag3)
		{
			posValue *= -1f;
		}
		return posValue * (float)flag;
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x000180A4 File Offset: 0x000162A4
	private InfinityScrollLegacy.ScrollDirection GetDirectionByScrollbarDirection()
	{
		InfinityScrollLegacy.ScrollDirection result;
		switch (this._scroll.ScrollBar.direction)
		{
		case Scrollbar.Direction.LeftToRight:
			result = InfinityScrollLegacy.ScrollDirection.FromLeft;
			break;
		case Scrollbar.Direction.RightToLeft:
			result = InfinityScrollLegacy.ScrollDirection.FromRight;
			break;
		case Scrollbar.Direction.BottomToTop:
			result = InfinityScrollLegacy.ScrollDirection.FromBottom;
			break;
		case Scrollbar.Direction.TopToBottom:
			result = InfinityScrollLegacy.ScrollDirection.FromTop;
			break;
		default:
			result = InfinityScrollLegacy.ScrollDirection.FromTop;
			break;
		}
		return result;
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x000180F4 File Offset: 0x000162F4
	private void InitScroll()
	{
		bool flag = this._scroll;
		if (!flag)
		{
			this._scroll = base.GetComponent<CScrollRectLegacy>();
			this._container = this._scroll.Content;
			this._viewPort = this._scroll.Viewport;
		}
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x00018144 File Offset: 0x00016344
	private void InitContainer()
	{
		this._hasLayout = (base.GetComponent<ILayoutGroup>() != null);
		this.InitScroll();
		this.LineCount = Mathf.Max(this.LineCount, 1);
		bool flag = null == this._scroll;
		if (!flag)
		{
			RectTransform rectPrefab = this.SrcPrefab.GetComponent<RectTransform>();
			switch (this.Direction)
			{
			case InfinityScrollLegacy.ScrollDirection.FromTop:
			{
				bool flag2 = !this._hasLayout;
				if (flag2)
				{
					this._container.pivot = new Vector2(0.5f, 1f);
				}
				rectPrefab.pivot = Vector2.up;
				break;
			}
			case InfinityScrollLegacy.ScrollDirection.FromBottom:
			{
				bool flag3 = !this._hasLayout;
				if (flag3)
				{
					this._container.pivot = new Vector2(0.5f, 0f);
				}
				rectPrefab.pivot = Vector2.zero;
				break;
			}
			case InfinityScrollLegacy.ScrollDirection.FromLeft:
			{
				bool flag4 = !this._hasLayout;
				if (flag4)
				{
					this._container.pivot = new Vector2(0f, 0.5f);
				}
				rectPrefab.pivot = Vector2.up;
				break;
			}
			case InfinityScrollLegacy.ScrollDirection.FromRight:
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
			bool flag7 = this._cellSize.x == 0f || this._cellSize.y == 0f;
			if (flag7)
			{
				LayoutElement layoutElement = rectPrefab.GetComponent<LayoutElement>();
				bool flag8 = layoutElement;
				if (flag8)
				{
					bool flag9 = layoutElement.preferredWidth != 0f;
					if (flag9)
					{
						this._cellSize.x = layoutElement.preferredWidth;
					}
					bool flag10 = layoutElement.preferredHeight != 0f;
					if (flag10)
					{
						this._cellSize.y = layoutElement.preferredHeight;
					}
				}
			}
			bool flag11 = !this._hasLayout;
			if (flag11)
			{
				float containerAntiSize = this.AntiPaddingSize * 2f + (this.AntiGapSize + this.AntiSize) * (float)(this.LineCount - 1) + this.AntiSize;
				this._container.SetSizeWithCurrentAnchors(this.AntiAxis, containerAntiSize);
				this._container.anchoredPosition = Vector2.zero;
			}
			this._scroll.ScrollSpeed = this._cellSize.y / 3f;
			this._refersMap = new Dictionary<int, Refers>();
			bool flag12 = this._waitRefreshIndexList == null;
			if (flag12)
			{
				this._waitRefreshIndexList = new List<int>();
			}
			this._refreshHelperStack = new Stack<int>();
			bool isPlaying = Application.isPlaying;
			if (isPlaying)
			{
				this.SrcPrefab.gameObject.SetActive(false);
				this._scroll.OnScrollEvent -= this.OnScroll;
				this._scroll.OnScrollEvent += this.OnScroll;
				bool flag13 = null != this._togGroup;
				if (flag13)
				{
					this._togGroup.Clear();
				}
				Refers[] refersArray = this._container.GetComponentsInTopChildren(true);
				int index = 0;
				int i = 0;
				int max = refersArray.Length;
				while (i < max)
				{
					Refers refers = refersArray[i];
					bool flag14 = refers == this.SrcPrefab;
					if (!flag14)
					{
						bool flag15 = refers == null;
						if (!flag15)
						{
							Action<Refers> onItemHide = this.OnItemHide;
							if (onItemHide != null)
							{
								onItemHide(refers);
							}
							refers.gameObject.SetActive(false);
							refers.UserInt = index++;
							this.SetRefersPos(refers);
							this.TryAddToggle(refers);
							this._refersMap.Add(refers.UserInt, refers);
							this.AddRefreshIndex(i);
						}
					}
					i++;
				}
				this.SetDirty(true);
			}
			this.InitSuccess = true;
		}
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x000185A4 File Offset: 0x000167A4
	private void RenderItem(int index, Refers refer)
	{
		refer.name = index.ToString();
		bool flag = this._togGroup != null;
		if (flag)
		{
			CToggleObsolete tog = refer.GetComponent<CToggleObsolete>();
			bool flag2 = null != tog;
			if (flag2)
			{
				tog.isOn = this._selectedKeysList.Contains(index);
				this._togGroup.NotifyToggle(tog, tog.isOn, false);
			}
		}
		Action<int, Refers> onItemRender = this.OnItemRender;
		if (onItemRender != null)
		{
			onItemRender(index, refer);
		}
		refer.gameObject.SetActive(true);
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x00018630 File Offset: 0x00016830
	private Refers GetCell()
	{
		Refers refer = null;
		bool flag = null == refer;
		if (flag)
		{
			refer = Object.Instantiate<Refers>(this.SrcPrefab, this._container, false);
			refer.gameObject.SetActive(true);
		}
		return refer;
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00018674 File Offset: 0x00016874
	private void TryAddToggle(Refers refers)
	{
		bool flag = null == this._togGroup;
		if (!flag)
		{
			bool childToggle = this._childToggle;
			CToggleObsolete tog;
			if (childToggle)
			{
				tog = refers.CGet<CToggleObsolete>("Toggle");
			}
			else
			{
				tog = refers.GetComponent<CToggleObsolete>();
			}
			bool flag2 = null == tog;
			if (!flag2)
			{
				tog.Key = refers.UserInt;
				this._togGroup.Add(tog);
			}
		}
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x000186DC File Offset: 0x000168DC
	private void TryRemoveToggle(Refers refers)
	{
		bool flag = null == this._togGroup;
		if (!flag)
		{
			CToggleObsolete tog = refers.GetComponent<CToggleObsolete>();
			bool flag2 = null == tog;
			if (!flag2)
			{
				this._togGroup.Remove(tog);
				tog.Key = -1;
			}
		}
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x00018724 File Offset: 0x00016924
	public void SetTogGroup(CToggleGroupObsolete togGroup, bool allowUncheck = false, bool childToggle = false)
	{
		this._childToggle = childToggle;
		this._togGroup = togGroup;
		togGroup.AllowSwitchOff = true;
		togGroup.AllowUncheck = allowUncheck;
		this.SelectedTogKey = -1;
		this._togGroup.Clear();
		bool initSuccess = this.InitSuccess;
		if (initSuccess)
		{
			foreach (KeyValuePair<int, Refers> pair in this._refersMap)
			{
				this.TryAddToggle(pair.Value);
			}
			togGroup.InitPreOnToggle(-1);
		}
		togGroup.OnActiveToggleChange = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(togGroup.OnActiveToggleChange, new Action<CToggleObsolete, CToggleObsolete>(delegate(CToggleObsolete newTog, CToggleObsolete preTog)
		{
			bool flag = null != preTog;
			if (flag)
			{
				this._selectedKeysList.Remove(preTog.Key);
				this.AddRefreshIndex(preTog.Key);
			}
			bool flag2 = null != newTog;
			if (flag2)
			{
				this.SelectedTogKey = newTog.Key;
				this.AddRefreshIndex(newTog.Key);
			}
		}));
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x000187EC File Offset: 0x000169EC
	private void SetDirty(bool value = true)
	{
		this._dirtyFlag = value;
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x000187F8 File Offset: 0x000169F8
	private void AddRefreshIndex(int index)
	{
		bool flag = this._waitRefreshIndexList == null;
		if (flag)
		{
			this._waitRefreshIndexList = new List<int>();
		}
		bool flag2 = !this._waitRefreshIndexList.Contains(index);
		if (flag2)
		{
			this._waitRefreshIndexList.Add(index);
		}
		this.SetDirty(this._waitRefreshIndexList.Count > 0);
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00018854 File Offset: 0x00016A54
	private void CleanupDestroyedRefers()
	{
		bool flag = this._refersMap == null || this._refersMap.Count == 0;
		if (!flag)
		{
			List<int> keysToRemove = EasyPool.Get<List<int>>();
			keysToRemove.Clear();
			foreach (KeyValuePair<int, Refers> pair in this._refersMap)
			{
				bool flag2 = pair.Value == null;
				if (flag2)
				{
					keysToRemove.Add(pair.Key);
				}
			}
			foreach (int key in keysToRemove)
			{
				this._refersMap.Remove(key);
			}
			EasyPool.Free<List<int>>(keysToRemove);
		}
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x00018948 File Offset: 0x00016B48
	private void HandlerWaitRefresh()
	{
		bool flag = !this._dirtyFlag;
		if (!flag)
		{
			this.CleanupDestroyedRefers();
			this._refreshHelperStack.Clear();
			this._showingRange.y = Mathf.Min(this._showingRange.y, this._dataCount - 1);
			bool flag2 = this._showingRange.y - this._showingRange.x < this._pageCount;
			if (flag2)
			{
				this._showingRange.x = Mathf.Max(this._showingRange.y - this._pageCount, 0);
			}
			foreach (KeyValuePair<int, Refers> pair in this._refersMap)
			{
				bool flag3 = pair.Key < this._showingRange.x || pair.Key > this._showingRange.y || pair.Key >= this._dataCount;
				if (flag3)
				{
					this._refreshHelperStack.Push(pair.Key);
					Action<Refers> onItemHide = this.OnItemHide;
					if (onItemHide != null)
					{
						onItemHide(pair.Value);
					}
					bool flag4 = pair.Value != null;
					if (flag4)
					{
						pair.Value.gameObject.SetActive(false);
					}
				}
			}
			bool flag5 = !this.InitSuccess;
			if (flag5)
			{
				this.SetDirty(false);
			}
			else
			{
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
						Refers refers;
						bool flag7 = !this._refersMap.TryGetValue(i, out refers);
						if (flag7)
						{
							bool flag8 = this._refreshHelperStack.Count > 0;
							if (flag8)
							{
								int reuseIndex = this._refreshHelperStack.Pop();
								this._refersMap.TryGetValue(reuseIndex, out refers);
								this.TryRemoveToggle(refers);
								this._refersMap.Remove(reuseIndex);
							}
							bool flag9 = null == refers;
							if (flag9)
							{
								refers = this.GetCell();
							}
							refers.UserInt = i;
							this.SetRefersPos(refers);
							this._refersMap.Add(refers.UserInt, refers);
							this.TryAddToggle(refers);
							this.AddRefreshIndex(i);
						}
						bool flag10 = refers != null;
						if (flag10)
						{
							refers.gameObject.SetActive(true);
						}
					}
					try
					{
						for (int j = 0; j < this._waitRefreshIndexList.Count; j++)
						{
							int index = this._waitRefreshIndexList[j];
							bool flag11 = index < this._showingRange.x || index > this._showingRange.y || index >= this._dataCount || index < 0;
							if (!flag11)
							{
								Refers refers2;
								bool flag12 = !this._refersMap.TryGetValue(index, out refers2);
								if (!flag12)
								{
									this.RenderItem(index, refers2);
								}
							}
						}
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
						Action onRenderEnd2 = this.OnRenderEnd;
						if (onRenderEnd2 != null)
						{
							onRenderEnd2();
						}
					}
				}
			}
		}
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00018D1C File Offset: 0x00016F1C
	private void UpdateShowingRange(bool adjustPos = false)
	{
		Vector2 anchoredPosition = this._container.anchoredPosition;
		float posValue = (this.Axis == RectTransform.Axis.Horizontal) ? anchoredPosition.x : anchoredPosition.y;
		bool flag = this.Direction == InfinityScrollLegacy.ScrollDirection.FromTop || this.Direction == InfinityScrollLegacy.ScrollDirection.FromRight;
		if (flag)
		{
			posValue = Mathf.Max(0f, posValue);
		}
		else
		{
			posValue = Mathf.Min(0f, posValue);
		}
		posValue = Mathf.Abs(posValue);
		this._showingRange.x = Mathf.Clamp(Mathf.FloorToInt((posValue - this.PaddingSize) / (this.GapSize + this.Size)) * this.LineCount, 0, Mathf.Max(0, this._dataCount - this._pageCount - 1));
		this._showingRange.y = Mathf.Min(this._showingRange.x + this._pageCount, this._dataCount - 1);
		bool flag2 = this._showingRange.y % this.LineCount == 0 && this.LineCount > 1;
		if (flag2)
		{
			this._showingRange.y = Mathf.Min(this._showingRange.y + this.LineCount - 1, this._dataCount - 1);
		}
		if (adjustPos)
		{
			float posMax = (this.Axis == RectTransform.Axis.Vertical) ? (this._container.rect.height - this._viewPort.rect.height) : (this._container.rect.width - this._viewPort.rect.width);
			bool flag3 = posValue > posMax;
			if (flag3)
			{
				this.ScrollTo(this._dataCount, 0.3f);
			}
		}
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00018ED4 File Offset: 0x000170D4
	public void SetDataCount(int count)
	{
		bool flag = !this.InitSuccess;
		if (flag)
		{
			this.InitContainer();
		}
		this._dataCount = count;
		float size = this.PaddingSize * 2f + (this.Size + this.GapSize) * (float)(Mathf.CeilToInt((float)this._dataCount / (float)this.LineCount) - 1) + this.Size;
		bool flag2 = !this._hasLayout;
		if (flag2)
		{
			this._container.SetSizeWithCurrentAnchors(this.Axis, size);
		}
		bool isSetContentSizeToView = this.IsSetContentSizeToView;
		if (isSetContentSizeToView)
		{
			base.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(this.Axis, size);
		}
		this.UpdateShowingRange(true);
		this.ReRender();
		bool flag3 = null != this.EmptyObject;
		if (flag3)
		{
			this.EmptyObject.SetActive(this._dataCount <= 0);
		}
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00018FAF File Offset: 0x000171AF
	public void UpdateData(int newCount)
	{
		this.SetDataCount(newCount);
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x00018FBC File Offset: 0x000171BC
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

	// Token: 0x0600040C RID: 1036 RVA: 0x00019010 File Offset: 0x00017210
	public void Refresh(int index = 0)
	{
		bool flag2 = !this.InitSuccess;
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
				bool flag5 = this.Direction == InfinityScrollLegacy.ScrollDirection.FromTop || this.Direction == InfinityScrollLegacy.ScrollDirection.FromRight;
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
				this._scroll.ScrollTo(anchoredPos, 0.3f);
			}
		}
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x000191A5 File Offset: 0x000173A5
	public void RefreshCell(int index)
	{
		this.AddRefreshIndex(index);
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x000191B0 File Offset: 0x000173B0
	public Refers GetActiveCell(int index)
	{
		Dictionary<int, Refers> refersMap = this._refersMap;
		return (refersMap != null) ? refersMap.GetValueOrDefault(index) : null;
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x000191C5 File Offset: 0x000173C5
	public void ScrollToEnd()
	{
		this.ScrollTo(this._dataCount, 0.3f);
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x000191DC File Offset: 0x000173DC
	public void ScrollTo(int index, float duration = 0.3f)
	{
		bool flag2 = !this.InitSuccess;
		if (!flag2)
		{
			index = Mathf.Clamp(index, 0, this._dataCount);
			float posValue = this.IndexToContainerValue(index, -1, true);
			float containerPos = Mathf.Abs((this.Axis == RectTransform.Axis.Vertical) ? this._container.anchoredPosition.y : this._container.anchoredPosition.x);
			float containerSize = (this.Axis == RectTransform.Axis.Vertical) ? this._container.rect.height : this._container.rect.width;
			float viewSize = (this.Axis == RectTransform.Axis.Horizontal) ? this._viewPort.rect.width : this._viewPort.rect.height;
			bool flag3 = Mathf.Abs(posValue) >= containerPos && posValue <= containerPos + viewSize;
			if (!flag3)
			{
				int flag = -1;
				bool flag4 = this.Direction == InfinityScrollLegacy.ScrollDirection.FromTop || this.Direction == InfinityScrollLegacy.ScrollDirection.FromRight;
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
				float max = Mathf.Max(0f, (containerSize - viewSize) * (float)flag);
				posValue = Mathf.Clamp(posValue, 0f, max);
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

	// Token: 0x06000411 RID: 1041 RVA: 0x00019384 File Offset: 0x00017584
	public bool NeedScroll(int index)
	{
		bool flag = !this.InitSuccess;
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
			float num = (this.Axis == RectTransform.Axis.Vertical) ? this._container.rect.height : this._container.rect.width;
			float viewSize = (this.Axis == RectTransform.Axis.Horizontal) ? this._viewPort.rect.width : this._viewPort.rect.height;
			bool flag2 = Mathf.Abs(posValue) >= containerPos && posValue <= containerPos + viewSize;
			result = !flag2;
		}
		return result;
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00019484 File Offset: 0x00017684
	public void UpdateStyle(Refers srcPrefab, int count)
	{
		this._dataCount = count;
		bool flag = !this.InitSuccess;
		if (flag)
		{
			this.InitContainer();
		}
		this.UpdateStyle(this.Direction, this.LineCount, this.Gap, this.Padding, srcPrefab);
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x000194D0 File Offset: 0x000176D0
	public void UpdateStyle(InfinityScrollLegacy.ScrollDirection dir, int lineCount, Vector2 gap, Vector2 padding, Refers srcPrefab = null)
	{
		bool flag = null == this.SrcPrefab;
		if (!flag)
		{
			this._needRefreshAfterUpdateStyle = true;
			this._scroll.SetScrollEnable(false);
			Refers[] childRefers = this._container.GetComponentsInTopChildren(true);
			childRefers.ForEach(delegate(int index, Refers child)
			{
				bool flag3 = child == this.SrcPrefab;
				bool result;
				if (flag3)
				{
					result = false;
				}
				else
				{
					bool flag4 = null != srcPrefab;
					if (flag4)
					{
						Action<Refers> onItemHide = this.OnItemHide;
						if (onItemHide != null)
						{
							onItemHide(child);
						}
						Object.Destroy(child.gameObject);
					}
					result = false;
				}
				return result;
			});
			bool flag2 = null != srcPrefab;
			if (flag2)
			{
				this.SrcPrefab = srcPrefab;
			}
			this._refersMap.Clear();
			this.Direction = dir;
			this.LineCount = lineCount;
			this.Gap = gap;
			this.Padding = padding;
			bool activeInHierarchy = base.gameObject.activeInHierarchy;
			if (activeInHierarchy)
			{
				base.StartCoroutine(this.DelayUpdateStyle());
			}
		}
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x000195A2 File Offset: 0x000177A2
	private IEnumerator DelayUpdateStyle()
	{
		bool flag = !base.gameObject.activeInHierarchy;
		if (flag)
		{
			yield break;
		}
		yield return new WaitForEndOfFrame();
		this.InitPageCount();
		bool flag2 = !this._hasLayout;
		if (flag2)
		{
			this._container.SetSizeWithCurrentAnchors(this.Axis, this.PaddingSize * 2f + (this.Size + this.GapSize) * (float)(Mathf.CeilToInt((float)this._dataCount / (float)this.LineCount) - 1) + this.Size);
		}
		this._showingRange.y = Mathf.Min(this._showingRange.x + this._pageCount, this._dataCount - 1);
		bool flag3 = this._showingRange.y - this._showingRange.x < this._pageCount;
		if (flag3)
		{
			this._showingRange.x = Mathf.Max(this._showingRange.y - this._pageCount, 0);
		}
		this.ReRender();
		this._scroll.SetScrollEnable(true);
		this._needRefreshAfterUpdateStyle = false;
		yield break;
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x000195B1 File Offset: 0x000177B1
	private IEnumerator DelayInitPageCount()
	{
		bool flag = !base.gameObject.activeInHierarchy;
		if (flag)
		{
			yield break;
		}
		yield return new WaitForEndOfFrame();
		this.InitPageCount();
		yield break;
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x000195C0 File Offset: 0x000177C0
	private void OnScroll()
	{
		bool flag = !this.InitSuccess;
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

	// Token: 0x06000417 RID: 1047 RVA: 0x000196B5 File Offset: 0x000178B5
	public void AddOnScrollEvent(Action action)
	{
		this.InitScroll();
		this._scroll.OnScrollEvent += action;
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x000196CC File Offset: 0x000178CC
	public void RemoveOnScrollEvent(Action action)
	{
		this.InitScroll();
		this._scroll.OnScrollEvent -= action;
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x000196E4 File Offset: 0x000178E4
	public int GetPageMaxCount()
	{
		this.InitScroll();
		float height = this._viewPort.rect.height - this.Padding.y;
		float itemHeight = this.SrcPrefab.GetComponent<RectTransform>().rect.height;
		float count = height / (itemHeight + this.Gap.y);
		return (int)count;
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x0001974C File Offset: 0x0001794C
	public float GetCellHeightPosition(int index)
	{
		int line = index / this.LineCount + 1;
		float itemHeight = this.SrcPrefab.GetComponent<RectTransform>().rect.height;
		return MathF.Max(0f, (itemHeight + this.Gap.y) * (float)line - this._viewPort.rect.height);
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x000197B0 File Offset: 0x000179B0
	public bool IsInViewport(int index)
	{
		bool flag = !this.InitSuccess || index < 0 || index >= this._dataCount;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this._showingRange.x > index && index > this._showingRange.y;
			if (flag2)
			{
				result = false;
			}
			else
			{
				float posValue = this.IndexToContainerValue(index, -1, true);
				float containerPos = (this.Axis == RectTransform.Axis.Vertical) ? this._container.anchoredPosition.y : this._container.anchoredPosition.x;
				float viewSize = (this.Axis == RectTransform.Axis.Horizontal) ? this._viewPort.rect.width : this._viewPort.rect.height;
				float cellSize = (this.Axis == RectTransform.Axis.Horizontal) ? this._cellSize.x : this._cellSize.y;
				float itemStart = posValue;
				float itemEnd = posValue + cellSize;
				float viewStart = containerPos;
				float viewEnd = containerPos + viewSize;
				bool isOverlap = itemEnd > viewStart && itemStart < viewEnd;
				result = isOverlap;
			}
		}
		return result;
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x000198C8 File Offset: 0x00017AC8
	void ILanguage.OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		Refers[] refersArray = this._container.GetComponentsInTopChildren(true);
		int i = 0;
		int max = refersArray.Length;
		while (i < max)
		{
			Refers refers = refersArray[i];
			bool flag = refers == this.SrcPrefab;
			if (!flag)
			{
				Action<Refers, LocalStringManager.LanguageType> onLanguageChange = this.OnLanguageChange;
				if (onLanguageChange != null)
				{
					onLanguageChange(refers, languageType);
				}
			}
			i++;
		}
	}

	// Token: 0x04000269 RID: 617
	public Refers SrcPrefab;

	// Token: 0x0400026A RID: 618
	public int extraLineCount = 2;

	// Token: 0x0400026B RID: 619
	[Tooltip("单排元素数量")]
	public int LineCount = 1;

	// Token: 0x0400026C RID: 620
	[Tooltip("元素间距")]
	public Vector2 Gap = new Vector2(10f, 10f);

	// Token: 0x0400026D RID: 621
	[Tooltip("元素与各边的间距")]
	public Vector2 Padding = new Vector2(10f, 10f);

	// Token: 0x0400026E RID: 622
	[Tooltip("没有任何元素时显示的物体")]
	public GameObject EmptyObject;

	// Token: 0x0400026F RID: 623
	public Action<int, Refers> OnItemRender;

	// Token: 0x04000270 RID: 624
	public Action<Refers> OnItemHide;

	// Token: 0x04000271 RID: 625
	public Action OnRenderEnd;

	// Token: 0x04000272 RID: 626
	public Action<Refers, LocalStringManager.LanguageType> OnLanguageChange;

	// Token: 0x04000273 RID: 627
	private int _dataCount;

	// Token: 0x04000274 RID: 628
	private CScrollRectLegacy _scroll;

	// Token: 0x04000275 RID: 629
	private RectTransform _viewPort;

	// Token: 0x04000276 RID: 630
	private RectTransform _container;

	// Token: 0x04000277 RID: 631
	[NonSerialized]
	public bool InitSuccess;

	// Token: 0x04000278 RID: 632
	private Vector2 _cellSize = Vector2.zero;

	// Token: 0x04000279 RID: 633
	[SerializeField]
	[ReadOnly]
	private int _pageCount;

	// Token: 0x0400027A RID: 634
	private CToggleGroupObsolete _togGroup = null;

	// Token: 0x0400027B RID: 635
	private List<int> _selectedKeysList;

	// Token: 0x0400027C RID: 636
	private int _selectedTogKey;

	// Token: 0x0400027D RID: 637
	private bool _childToggle = false;

	// Token: 0x0400027E RID: 638
	private Dictionary<int, Refers> _refersMap;

	// Token: 0x0400027F RID: 639
	private Vector2Int _showingRange;

	// Token: 0x04000280 RID: 640
	private List<int> _waitRefreshIndexList;

	// Token: 0x04000281 RID: 641
	private Stack<int> _refreshHelperStack;

	// Token: 0x04000282 RID: 642
	private bool _dirtyFlag;

	// Token: 0x04000283 RID: 643
	private bool _needRefreshAfterUpdateStyle;

	// Token: 0x04000284 RID: 644
	private bool _hasLayout;

	// Token: 0x04000285 RID: 645
	[HideInInspector]
	public bool IsSetContentSizeToView;

	// Token: 0x04000286 RID: 646
	private bool _pageCountInitialized = false;

	// Token: 0x020010E1 RID: 4321
	public enum ScrollDirection
	{
		// Token: 0x040094A6 RID: 38054
		FromTop,
		// Token: 0x040094A7 RID: 38055
		FromBottom,
		// Token: 0x040094A8 RID: 38056
		FromLeft,
		// Token: 0x040094A9 RID: 38057
		FromRight
	}
}
