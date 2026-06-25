using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.GameLineScroll
{
	// Token: 0x02000A1C RID: 2588
	public class TaiwuScroll : MonoBehaviour
	{
		// Token: 0x17000DBE RID: 3518
		// (get) Token: 0x06007EE2 RID: 32482 RVA: 0x003B1FB5 File Offset: 0x003B01B5
		// (set) Token: 0x06007EE3 RID: 32483 RVA: 0x003B1FBD File Offset: 0x003B01BD
		public int CurrSaveSlot
		{
			get
			{
				return this._currSaveSlot;
			}
			set
			{
				this._currSaveSlot = value;
			}
		}

		// Token: 0x17000DBF RID: 3519
		// (get) Token: 0x06007EE4 RID: 32484 RVA: 0x003B1FC6 File Offset: 0x003B01C6
		// (set) Token: 0x06007EE5 RID: 32485 RVA: 0x003B1FD0 File Offset: 0x003B01D0
		private bool IsScrlolling
		{
			get
			{
				return this._isScrlolling;
			}
			set
			{
				this._isScrlolling = value;
				bool flag = !this._isScrlolling;
				if (flag)
				{
					this.SyncPageToggleToCurIndex();
				}
			}
		}

		// Token: 0x17000DC0 RID: 3520
		// (get) Token: 0x06007EE6 RID: 32486 RVA: 0x003B1FF9 File Offset: 0x003B01F9
		// (set) Token: 0x06007EE7 RID: 32487 RVA: 0x003B2001 File Offset: 0x003B0201
		public int CurIndex
		{
			get
			{
				return this._curIndex;
			}
			set
			{
				int min = 0;
				List<TaiwuLifeSummary> taiwuLifeSummaryDatas = this._taiwuLifeSummaryDatas;
				this._curIndex = Math.Clamp(value, min, (taiwuLifeSummaryDatas != null) ? taiwuLifeSummaryDatas.Count : 0);
				this.OnCurIndexChange();
			}
		}

		// Token: 0x06007EE8 RID: 32488 RVA: 0x003B202A File Offset: 0x003B022A
		private void OnCurIndexChange()
		{
			this.SyncPageToggleToCurIndex();
		}

		// Token: 0x06007EE9 RID: 32489 RVA: 0x003B2034 File Offset: 0x003B0234
		private void StartAutoPlay(int direction)
		{
			this._isAutoPlaying = true;
			this._autoPlayDirection = direction;
			this.clickMask.gameObject.SetActive(true);
			this.taiwuScrollItemScroll.Scroll.SetScrollEnable(false);
			bool flag = this._autoPlayCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._autoPlayCoroutine);
			}
			this._autoPlayCoroutine = base.StartCoroutine(this.AutoPlayCoroutine());
		}

		// Token: 0x06007EEA RID: 32490 RVA: 0x003B20A0 File Offset: 0x003B02A0
		private void StopAutoPlay()
		{
			this._isAutoPlaying = false;
			this.clickMask.gameObject.SetActive(false);
			this.taiwuScrollItemScroll.Scroll.SetScrollEnable(true);
			bool flag = this._autoPlayTweener != null && this._autoPlayTweener.IsActive();
			if (flag)
			{
				this._autoPlayTweener.Kill(false);
			}
			this._autoPlayTweener = null;
			bool flag2 = this._autoPlayCoroutine != null;
			if (flag2)
			{
				base.StopCoroutine(this._autoPlayCoroutine);
				this._autoPlayCoroutine = null;
			}
		}

		// Token: 0x06007EEB RID: 32491 RVA: 0x003B212C File Offset: 0x003B032C
		private void CleanupAutoPlay()
		{
			this._isAutoPlaying = false;
			this.clickMask.gameObject.SetActive(false);
			this.taiwuScrollItemScroll.Scroll.SetScrollEnable(true);
			bool flag = this._autoPlayTweener != null && this._autoPlayTweener.IsActive();
			if (flag)
			{
				this._autoPlayTweener.Kill(false);
			}
			this._autoPlayTweener = null;
		}

		// Token: 0x06007EEC RID: 32492 RVA: 0x003B2193 File Offset: 0x003B0393
		private IEnumerator AutoPlayCoroutine()
		{
			yield return null;
			while (this._isAutoPlaying)
			{
				TaiwuScroll.<>c__DisplayClass35_0 CS$<>8__locals1 = new TaiwuScroll.<>c__DisplayClass35_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.scroll = this.taiwuScrollItemScroll.Scroll;
				CS$<>8__locals1.content = CS$<>8__locals1.scroll.Content;
				float viewportWidth = CS$<>8__locals1.scroll.Viewport.rect.width;
				float maxScroll = Mathf.Max(0f, CS$<>8__locals1.content.rect.width * CS$<>8__locals1.content.localScale.x - CS$<>8__locals1.scroll.Viewport.rect.width);
				float currentX = CS$<>8__locals1.content.anchoredPosition.x;
				bool flag = this._autoPlayDirection < 0;
				if (flag)
				{
					bool flag2 = currentX <= 0.1f;
					if (flag2)
					{
						this.CleanupAutoPlay();
						yield break;
					}
					CS$<>8__locals1.targetX = Mathf.Max(0f, currentX - viewportWidth);
				}
				else
				{
					bool flag3 = currentX >= maxScroll - 0.1f;
					if (flag3)
					{
						this.CleanupAutoPlay();
						yield break;
					}
					CS$<>8__locals1.targetX = Mathf.Min(maxScroll, currentX + viewportWidth);
				}
				CS$<>8__locals1.startX = currentX;
				CS$<>8__locals1.startY = CS$<>8__locals1.content.anchoredPosition.y;
				CS$<>8__locals1.scrollEnd = false;
				this._autoPlayTweener = DOVirtual.Float(0f, 1f, 3f, delegate(float stepValue)
				{
					CS$<>8__locals1.content.anchoredPosition = new Vector2(Mathf.Lerp(CS$<>8__locals1.startX, CS$<>8__locals1.targetX, stepValue), CS$<>8__locals1.startY);
					CS$<>8__locals1.<>4__this.taiwuScrollItemScroll.RefreshDisplayRange();
					CS$<>8__locals1.<>4__this.OnTaiwuScrollItemScroll();
				}).SetUpdate(true).SetAutoKill(true).OnComplete(delegate
				{
					CS$<>8__locals1.scroll.UpdateScrollBarValue();
					CS$<>8__locals1.scrollEnd = true;
				});
				while (!CS$<>8__locals1.scrollEnd)
				{
					yield return null;
				}
				this._autoPlayTweener = null;
				this.CurIndex = this.GetShowingIndex();
				CS$<>8__locals1 = null;
			}
			yield break;
		}

		// Token: 0x06007EED RID: 32493 RVA: 0x003B21A4 File Offset: 0x003B03A4
		public void Set(bool isInGame, int currSaveSlot, UIElement uiElement = null)
		{
			this._isInGame = isInGame;
			this._debugArchiveWrapCount = 0;
			this._uiElement = uiElement;
			if (isInGame)
			{
				TaiwuDomainMethod.AsyncCall.GetTaiwuLifeSummaryDisplayData(null, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._displayData);
					this._characterDisplayDatas = this._displayData.TotalTaiwuDisplayDatas;
					this._taiwuLifeSummaryDatas = this._displayData.TotalTaiwuLifeSummaries;
					this.ApplyDebugFakeMultiPageTaiwuIfNeeded();
					this.taiwuScrollItemScroll.SetDataCount(this._taiwuLifeSummaryDatas.Count);
					this.RebuildPageToggleGroup(this._taiwuLifeSummaryDatas.Count);
					CScrollRect scroll2 = this.taiwuScrollItemScroll.Scroll;
					if (scroll2.OnScrollEnd == null)
					{
						scroll2.OnScrollEnd = delegate()
						{
							this.IsScrlolling = false;
						};
					}
					this.OnCurIndexChange();
				});
			}
			else
			{
				bool flag = currSaveSlot < 0;
				if (!flag)
				{
					this.CurrSaveSlot = currSaveSlot;
					WorldInfo worldInfo = GlobalOperations.ArchivesInfo[currSaveSlot].WorldInfo;
					this._totalTaiwuLifeSummaryInfo = ((worldInfo != null) ? worldInfo.TotalTaiwuLifeSummaryInfo : null);
					WorldInfo worldInfo2 = GlobalOperations.ArchivesInfo[currSaveSlot].WorldInfo;
					List<TaiwuLifeSummary> taiwuLifeSummaryDatas;
					if (worldInfo2 == null)
					{
						taiwuLifeSummaryDatas = null;
					}
					else
					{
						TotalTaiwuLifeSummaryInfo totalTaiwuLifeSummaryInfo = worldInfo2.TotalTaiwuLifeSummaryInfo;
						taiwuLifeSummaryDatas = ((totalTaiwuLifeSummaryInfo != null) ? totalTaiwuLifeSummaryInfo.TotalTaiwuLifeSummaries : null);
					}
					this._taiwuLifeSummaryDatas = taiwuLifeSummaryDatas;
					this.ApplyDebugFakeMultiPageTaiwuIfNeeded();
					List<TaiwuLifeSummary> taiwuLifeSummaryDatas2 = this._taiwuLifeSummaryDatas;
					int safeCount = (taiwuLifeSummaryDatas2 != null) ? taiwuLifeSummaryDatas2.Count : 0;
					bool flag2 = this._totalTaiwuLifeSummaryInfo != null && safeCount > 0;
					if (flag2)
					{
						List<AvatarRelatedData> totalTaiwuAvatarRelatedDatas = this._totalTaiwuLifeSummaryInfo.TotalTaiwuAvatarRelatedDatas;
						int avatarCount = (totalTaiwuAvatarRelatedDatas != null) ? totalTaiwuAvatarRelatedDatas.Count : safeCount;
						List<string> totalTaiwuSurnames = this._totalTaiwuLifeSummaryInfo.TotalTaiwuSurnames;
						int surnameCount = (totalTaiwuSurnames != null) ? totalTaiwuSurnames.Count : safeCount;
						List<string> totalTaiwuGivenNames = this._totalTaiwuLifeSummaryInfo.TotalTaiwuGivenNames;
						int givenNameCount = (totalTaiwuGivenNames != null) ? totalTaiwuGivenNames.Count : safeCount;
						List<short> totalTaiwuTitleIds = this._totalTaiwuLifeSummaryInfo.TotalTaiwuTitleIds;
						int titleCount = (totalTaiwuTitleIds != null) ? totalTaiwuTitleIds.Count : safeCount;
						bool flag3 = avatarCount < safeCount;
						if (flag3)
						{
							safeCount = avatarCount;
						}
						bool flag4 = surnameCount < safeCount;
						if (flag4)
						{
							safeCount = surnameCount;
						}
						bool flag5 = givenNameCount < safeCount;
						if (flag5)
						{
							safeCount = givenNameCount;
						}
						bool flag6 = titleCount < safeCount;
						if (flag6)
						{
							safeCount = titleCount;
						}
					}
					this.taiwuScrollItemScroll.SetDataCount(safeCount);
					this.RebuildPageToggleGroup(safeCount);
					CScrollRect scroll = this.taiwuScrollItemScroll.Scroll;
					if (scroll.OnScrollEnd == null)
					{
						scroll.OnScrollEnd = delegate()
						{
							this.IsScrlolling = false;
						};
					}
					this.OnCurIndexChange();
				}
			}
		}

		// Token: 0x06007EEE RID: 32494 RVA: 0x003B2358 File Offset: 0x003B0558
		private void Awake()
		{
			bool flag = this.pageToggleGroup != null;
			if (flag)
			{
				this.pageToggleGroup.OnActiveIndexChange += this.OnPageToggleGroupActiveIndexChange;
			}
			this.taiwuScrollItemScroll.OnItemRender += this.TaiwuScrollItemScrollOnOnItemRender;
			this.taiwuScrollItemScroll.AddOnScrollEvent(new Action(this.OnTaiwuScrollItemScroll));
			this.leftBtn.ClearAndAddListener(delegate
			{
				this.StartAutoPlay(1);
			});
			this.rightBtn.ClearAndAddListener(delegate
			{
				this.StartAutoPlay(-1);
			});
			this.clickMask.ClearAndAddListener(new Action(this.StopAutoPlay));
			this.clickMask.gameObject.SetActive(false);
		}

		// Token: 0x06007EEF RID: 32495 RVA: 0x003B2418 File Offset: 0x003B0618
		private void OnEnable()
		{
			this.OnTaiwuScrollItemScroll();
		}

		// Token: 0x06007EF0 RID: 32496 RVA: 0x003B2424 File Offset: 0x003B0624
		private void OnTaiwuScrollItemScroll()
		{
			this.CurIndex = this.GetShowingIndex();
			this.rightBtn.gameObject.SetActive(this.taiwuScrollItemScroll.Scroll.Content.anchoredPosition.x > 1f);
			this.leftBtn.gameObject.SetActive(this.taiwuScrollItemScroll.Scroll.Content.anchoredPosition.x < this.taiwuScrollItemScroll.Scroll.Content.rect.width * this.taiwuScrollItemScroll.Scroll.Content.localScale.x - this.taiwuScrollItemScroll.Scroll.Viewport.rect.width - 1f);
		}

		// Token: 0x06007EF1 RID: 32497 RVA: 0x003B24FC File Offset: 0x003B06FC
		private void TaiwuScrollItemScrollOnOnItemRender(int index, GameObject obj)
		{
			TaiwuScrollItem taiwuScrollItem = obj.GetComponent<TaiwuScrollItem>();
			bool isInGame = this._isInGame;
			if (isInGame)
			{
				taiwuScrollItem.Set(index, this._characterDisplayDatas[index], this._taiwuLifeSummaryDatas[index]);
			}
			else
			{
				int wrap = (this._debugArchiveWrapCount > 0) ? (index % this._debugArchiveWrapCount) : index;
				taiwuScrollItem.Set(index, new TaiwuLifeSummaryInfo
				{
					AvatarRelatedData = this._totalTaiwuLifeSummaryInfo.TotalTaiwuAvatarRelatedDatas[wrap],
					Surname = this._totalTaiwuLifeSummaryInfo.TotalTaiwuSurnames[wrap],
					TaiwuLifeSummaryData = this._taiwuLifeSummaryDatas[index],
					GivenName = this._totalTaiwuLifeSummaryInfo.TotalTaiwuGivenNames[wrap],
					TitleId = this._totalTaiwuLifeSummaryInfo.TotalTaiwuTitleIds[wrap]
				}, this.CurrSaveSlot);
			}
		}

		// Token: 0x06007EF2 RID: 32498 RVA: 0x003B25E4 File Offset: 0x003B07E4
		private void ApplyDebugFakeMultiPageTaiwuIfNeeded()
		{
			bool flag = !this.debugFakeMultiPageTaiwu || this.debugFakeTaiwuTotalCount <= 1;
			if (!flag)
			{
				List<TaiwuLifeSummary> taiwuLifeSummaryDatas = this._taiwuLifeSummaryDatas;
				bool flag2 = taiwuLifeSummaryDatas == null || taiwuLifeSummaryDatas.Count <= 0;
				if (!flag2)
				{
					bool flag3 = this._taiwuLifeSummaryDatas.Count >= this.debugFakeTaiwuTotalCount;
					if (!flag3)
					{
						int target = this.debugFakeTaiwuTotalCount;
						bool isInGame = this._isInGame;
						if (isInGame)
						{
							bool flag4 = this._characterDisplayDatas == null || this._characterDisplayDatas.Count != this._taiwuLifeSummaryDatas.Count;
							if (!flag4)
							{
								List<TaiwuLifeSummary> summaries = new List<TaiwuLifeSummary>(this._taiwuLifeSummaryDatas);
								List<CharacterDisplayData> chars = new List<CharacterDisplayData>(this._characterDisplayDatas);
								int i = summaries.Count;
								for (int j = summaries.Count; j < target; j++)
								{
									int k = j % i;
									summaries.Add(summaries[k]);
									chars.Add(chars[k]);
								}
								this._taiwuLifeSummaryDatas = summaries;
								this._characterDisplayDatas = chars;
							}
						}
						else
						{
							bool flag5 = this._totalTaiwuLifeSummaryInfo == null;
							if (!flag5)
							{
								this._debugArchiveWrapCount = this._taiwuLifeSummaryDatas.Count;
								List<TaiwuLifeSummary> summaries2 = new List<TaiwuLifeSummary>(this._taiwuLifeSummaryDatas);
								for (int l = summaries2.Count; l < target; l++)
								{
									summaries2.Add(summaries2[l % this._debugArchiveWrapCount]);
								}
								this._taiwuLifeSummaryDatas = summaries2;
							}
						}
					}
				}
			}
		}

		// Token: 0x06007EF3 RID: 32499 RVA: 0x003B2788 File Offset: 0x003B0988
		private void OnDestroy()
		{
			bool flag = this.pageToggleGroup != null;
			if (flag)
			{
				this.pageToggleGroup.OnActiveIndexChange -= this.OnPageToggleGroupActiveIndexChange;
			}
			this.StopAutoPlay();
		}

		// Token: 0x06007EF4 RID: 32500 RVA: 0x003B27C8 File Offset: 0x003B09C8
		private void OnPageToggleGroupActiveIndexChange(int newIndex, int oldIndex)
		{
			bool flag = newIndex < 0;
			if (!flag)
			{
				this.StopAutoPlay();
				this.IsScrlolling = true;
				this.taiwuScrollItemScroll.ScrollTo(newIndex, 0.3f);
			}
		}

		// Token: 0x06007EF5 RID: 32501 RVA: 0x003B2804 File Offset: 0x003B0A04
		private void SyncPageToggleToCurIndex()
		{
			bool flag = this.pageToggleGroup == null || this.pageToggleGroup.Count() <= 0;
			if (!flag)
			{
				List<TaiwuLifeSummary> taiwuLifeSummaryDatas = this._taiwuLifeSummaryDatas;
				int i = (taiwuLifeSummaryDatas != null) ? taiwuLifeSummaryDatas.Count : 0;
				bool flag2 = i <= 0 || this.CurIndex < 0 || this.CurIndex >= i || this.CurIndex >= this.pageToggleGroup.Count();
				if (!flag2)
				{
					this.pageToggleGroup.SetWithoutNotify(this.CurIndex);
				}
			}
		}

		// Token: 0x06007EF6 RID: 32502 RVA: 0x003B2894 File Offset: 0x003B0A94
		private void RebuildPageToggleGroup(int count)
		{
			bool flag = this.pageToggleGroup == null;
			if (!flag)
			{
				while (this.pageToggleGroup.Count() > 0)
				{
					CToggle t = this.pageToggleGroup.Get(0);
					this.pageToggleGroup.Remove(0);
					bool flag2 = t != null;
					if (flag2)
					{
						Object.Destroy(t.gameObject);
					}
				}
				this.pageToggleGroup.Clear();
				bool flag3 = count <= 0 || this.pageTogglePrefab == null;
				if (flag3)
				{
					this._curIndex = 0;
				}
				else
				{
					this.pageTogglePrefab.gameObject.SetActive(false);
					for (int i = 0; i < count; i++)
					{
						GameObject go = Object.Instantiate<GameObject>(this.pageTogglePrefab.gameObject, this.pageToggleGroup.transform);
						go.SetActive(true);
						this.pageToggleGroup.Add(go.GetComponent<CToggle>());
						TextMeshProUGUI label = go.GetComponentInChildren<TextMeshProUGUI>();
						bool flag4 = label != null;
						if (flag4)
						{
							label.text = (i + 1).ToString();
						}
					}
					Transform parent = this.pageToggleGroup.transform;
					int firstToggleSiblingIndex = parent.childCount - count;
					for (int targetSibling = count - 1; targetSibling >= 0; targetSibling--)
					{
						int logicalTaiwu = count - 1 - targetSibling;
						this.pageToggleGroup.Get(logicalTaiwu).transform.SetSiblingIndex(firstToggleSiblingIndex + targetSibling);
					}
					this._curIndex = Mathf.Clamp(this._curIndex, 0, count - 1);
					this.pageToggleGroup.Init(this._curIndex);
					ToggleGroupHotkeyController.Set(this._uiElement, this.pageToggleGroup, 1, null);
					ToggleGroupHotkeyController.Set(this._uiElement, this.rightBtn, this.leftBtn, 2, null);
				}
			}
		}

		// Token: 0x06007EF7 RID: 32503 RVA: 0x003B2A70 File Offset: 0x003B0C70
		private int GetShowingIndex()
		{
			float xOffset = ((RectTransform)this.taiwuScrollItemScroll.srcPrefab.transform.parent).anchoredPosition.x;
			float cellWidth = ((RectTransform)this.taiwuScrollItemScroll.srcPrefab.transform).sizeDelta.x;
			float gapX = this.taiwuScrollItemScroll.gap.x;
			return (int)(xOffset / (cellWidth + gapX));
		}

		// Token: 0x040060F1 RID: 24817
		[SerializeField]
		private CToggleGroup pageToggleGroup;

		// Token: 0x040060F2 RID: 24818
		[SerializeField]
		private CToggle pageTogglePrefab;

		// Token: 0x040060F3 RID: 24819
		[SerializeField]
		private InfinityScroll taiwuScrollItemScroll;

		// Token: 0x040060F4 RID: 24820
		[SerializeField]
		private CButton leftBtn;

		// Token: 0x040060F5 RID: 24821
		[SerializeField]
		private CButton rightBtn;

		// Token: 0x040060F6 RID: 24822
		[SerializeField]
		private CButton clickMask;

		// Token: 0x040060F7 RID: 24823
		[Header("DEBUG 多页预览（循环复制已有太吾数据至目标条数，勿提交开启）")]
		[SerializeField]
		private bool debugFakeMultiPageTaiwu;

		// Token: 0x040060F8 RID: 24824
		[SerializeField]
		private int debugFakeTaiwuTotalCount = 24;

		// Token: 0x040060F9 RID: 24825
		private List<CharacterDisplayData> _characterDisplayDatas;

		// Token: 0x040060FA RID: 24826
		private List<TaiwuLifeSummary> _taiwuLifeSummaryDatas;

		// Token: 0x040060FB RID: 24827
		private TaiwuLifeSummaryDisplayData _displayData = new TaiwuLifeSummaryDisplayData();

		// Token: 0x040060FC RID: 24828
		private TotalTaiwuLifeSummaryInfo _totalTaiwuLifeSummaryInfo;

		// Token: 0x040060FD RID: 24829
		private bool _isInGame;

		// Token: 0x040060FE RID: 24830
		private int _currSaveSlot;

		// Token: 0x040060FF RID: 24831
		private UIElement _uiElement;

		// Token: 0x04006100 RID: 24832
		private int _debugArchiveWrapCount;

		// Token: 0x04006101 RID: 24833
		private bool _isScrlolling;

		// Token: 0x04006102 RID: 24834
		private bool _isAutoPlaying;

		// Token: 0x04006103 RID: 24835
		private int _autoPlayDirection;

		// Token: 0x04006104 RID: 24836
		private Coroutine _autoPlayCoroutine;

		// Token: 0x04006105 RID: 24837
		private Tweener _autoPlayTweener;

		// Token: 0x04006106 RID: 24838
		private int _curIndex;
	}
}
