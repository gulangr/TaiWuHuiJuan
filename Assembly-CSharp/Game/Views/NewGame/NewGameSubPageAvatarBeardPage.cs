using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007ED RID: 2029
	public class NewGameSubPageAvatarBeardPage : NewGameSubPageAvatarPageBase
	{
		// Token: 0x06006309 RID: 25353 RVA: 0x002D5334 File Offset: 0x002D3534
		public void SetBeardType(bool isLower)
		{
			bool isUpper = !isLower;
			bool flag = this.isUpperBeard != isUpper;
			if (flag)
			{
				this.isUpperBeard = isUpper;
				bool isActiveAndEnabled = base.isActiveAndEnabled;
				if (isActiveAndEnabled)
				{
					this.LoadBeardStyles();
					this.UpdateTitle();
					this.RefreshFromAvatarData();
				}
			}
		}

		// Token: 0x0600630A RID: 25354 RVA: 0x002D5384 File Offset: 0x002D3584
		private void Awake()
		{
			bool flag = this.beardColorTemplate != null;
			if (flag)
			{
				this.beardColorTemplate.gameObject.SetActive(false);
			}
			bool flag2 = this.beardStyleTemplate != null;
			if (flag2)
			{
				this.beardStyleTemplate.gameObject.SetActive(false);
			}
			bool flag3 = this.scrollRect != null;
			if (flag3)
			{
				this.scrollRect.OnScrollEvent += this.OnScroll;
			}
		}

		// Token: 0x0600630B RID: 25355 RVA: 0x002D53FE File Offset: 0x002D35FE
		protected override void OnEnable()
		{
			this.LoadBeardColors();
			this.CreateBeardColorToggles();
			this.LoadBeardStyles();
			this.UpdateTitle();
		}

		// Token: 0x0600630C RID: 25356 RVA: 0x002D541D File Offset: 0x002D361D
		public override void UpdateUI()
		{
			this.RefreshFromAvatarData();
		}

		// Token: 0x0600630D RID: 25357 RVA: 0x002D5428 File Offset: 0x002D3628
		private void UpdateTitle()
		{
			bool flag = this.pageTitleText == null;
			if (!flag)
			{
				LanguageKey key = this.isUpperBeard ? LanguageKey.UI_NewGame_AdjustTitle_9 : LanguageKey.UI_NewGame_AdjustTitle_10;
				this.pageTitleText.text = LocalStringManager.Get(key);
			}
		}

		// Token: 0x0600630E RID: 25358 RVA: 0x002D5470 File Offset: 0x002D3670
		private void LoadBeardColors()
		{
			bool flag = this._beardColors != null;
			if (!flag)
			{
				this._beardColors = NewGameSubPageAvatarColorHelper.HairColors;
			}
		}

		// Token: 0x0600630F RID: 25359 RVA: 0x002D5498 File Offset: 0x002D3698
		private void CreateBeardColorToggles()
		{
			bool flag = this._beardColors == null;
			if (!flag)
			{
				Transform container = this.beardColorToggleGroup.transform;
				CommonUtils.PrepareEnoughChildren(container, this.beardColorTemplate.gameObject, this._beardColors.Count, null);
				this.beardColorToggleGroup.Clear();
				for (int i = 0; i < this._beardColors.Count; i++)
				{
					Color color = this._beardColors[i].Item2;
					Transform toggleTrans = container.GetChild(i);
					CImage colorImage = toggleTrans.GetChild(0).GetChild(0).GetComponent<CImage>();
					colorImage.color = color;
					this.beardColorToggleGroup.Add(toggleTrans.GetComponent<CToggle>());
				}
				this.beardColorToggleGroup.Init(-1);
				bool flag2 = !this._isColorInitialized;
				if (flag2)
				{
					this.beardColorToggleGroup.OnActiveIndexChange += this.OnBeardColorChanged;
					this._isColorInitialized = true;
				}
			}
		}

		// Token: 0x06006310 RID: 25360 RVA: 0x002D55A0 File Offset: 0x002D37A0
		private void LoadBeardStyles()
		{
			this._beardResList = new List<AvatarAsset>();
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				AvatarManager manager = SingletonObject.getInstance<AvatarManager>();
				AvatarGroup group = manager.GetAvatarGroup((int)avatarData.AvatarId);
				bool flag2 = group == null;
				if (!flag2)
				{
					List<AvatarAsset> sourceList = this.isUpperBeard ? group.Beard1Res : group.Beard2Res;
					bool isFemale = avatarData.AvatarId % 2 == 0;
					bool flag3 = (sourceList == null || sourceList.Count == 0) && !isFemale;
					if (flag3)
					{
						AvatarGroup backupGroup = manager.GetAvatarGroup(1);
						bool flag4 = backupGroup != null;
						if (flag4)
						{
							sourceList = (this.isUpperBeard ? backupGroup.Beard1Res : backupGroup.Beard2Res);
						}
					}
					bool flag5 = sourceList != null;
					if (flag5)
					{
						foreach (AvatarAsset res in sourceList)
						{
							bool flag6 = res.Id != 1;
							if (flag6)
							{
								this._beardResList.Add(res);
							}
						}
					}
					Func<List<AvatarAsset>, List<AvatarAsset>> func;
					if (!this.isUpperBeard)
					{
						IAvatarSubPageParent avatarPage = this.AvatarPage;
						func = ((avatarPage != null) ? avatarPage.GetBeard2Filter() : null);
					}
					else
					{
						IAvatarSubPageParent avatarPage2 = this.AvatarPage;
						func = ((avatarPage2 != null) ? avatarPage2.GetBeard1Filter() : null);
					}
					Func<List<AvatarAsset>, List<AvatarAsset>> filter = func;
					bool flag7 = filter != null;
					if (flag7)
					{
						this._beardResList = filter(this._beardResList);
					}
					this.CreateStyleItems();
				}
			}
		}

		// Token: 0x06006311 RID: 25361 RVA: 0x002D5724 File Offset: 0x002D3924
		private void CreateStyleItems()
		{
			bool flag = this.beardStyleTemplate == null || this.beardStyleContainer == null || this.beardStyleToggleGroup == null;
			if (!flag)
			{
				int totalCount = this._beardResList.Count + 1;
				CommonUtils.PrepareEnoughChildren(this.beardStyleContainer, this.beardStyleTemplate.gameObject, totalCount, null);
				this.beardStyleToggleGroup.Clear();
				this._styleItems.Clear();
				for (int i = 0; i < totalCount; i++)
				{
					NewGameSubPageAvatarStyleItem item = this.beardStyleContainer.GetChild(i).GetComponent<NewGameSubPageAvatarStyleItem>();
					bool flag2 = item != null;
					if (flag2)
					{
						this._styleItems.Add(item);
					}
					this.beardStyleToggleGroup.Add(item.Toggle);
				}
				this.beardStyleToggleGroup.Init(-1);
				bool flag3 = !this._isStyleInitialized;
				if (flag3)
				{
					this.beardStyleToggleGroup.OnActiveIndexChange += this.OnBeardStyleChanged;
					this._isStyleInitialized = true;
				}
				this.RefreshAllItems();
			}
		}

		// Token: 0x06006312 RID: 25362 RVA: 0x002D5848 File Offset: 0x002D3A48
		private void RefreshFromAvatarData()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				bool flag2 = this.beardColorToggleGroup != null && this._beardColors != null;
				if (flag2)
				{
					byte colorId = this.isUpperBeard ? avatarData.ColorBeard1Id : avatarData.ColorBeard2Id;
					int index = this.GetColorIndex(colorId);
					bool flag3 = index >= 0;
					if (flag3)
					{
						this.beardColorToggleGroup.SetWithoutNotify(index);
					}
				}
				this.RefreshAllItems();
			}
		}

		// Token: 0x06006313 RID: 25363 RVA: 0x002D58CC File Offset: 0x002D3ACC
		private int GetColorIndex(byte colorId)
		{
			bool flag = this._beardColors == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < this._beardColors.Count; i++)
				{
					bool flag2 = this._beardColors[i].Item1 == colorId;
					if (flag2)
					{
						return i;
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06006314 RID: 25364 RVA: 0x002D592C File Offset: 0x002D3B2C
		private void RefreshAllItems()
		{
			bool isRefreshing = this._isRefreshing;
			if (!isRefreshing)
			{
				this._isRefreshing = true;
				AvatarData avatarData = base.GetAvatarData();
				bool flag = avatarData == null;
				if (flag)
				{
					this._isRefreshing = false;
				}
				else
				{
					int selectedIndex = -1;
					ValueTuple<int, int> visibleRange = this.GetVisibleRange();
					int visibleStart = visibleRange.Item1;
					int visibleEnd = visibleRange.Item2;
					for (int i = 0; i < this._styleItems.Count; i++)
					{
						bool isVisible = i >= visibleStart && i <= visibleEnd;
						bool isSelected;
						this.RefreshItemAt(i, avatarData, out isSelected, isVisible);
						bool flag2 = isSelected;
						if (flag2)
						{
							selectedIndex = i;
						}
					}
					bool flag3 = selectedIndex >= 0 && this.beardStyleToggleGroup != null;
					if (flag3)
					{
						this.beardStyleToggleGroup.Set(selectedIndex, false);
					}
					this._isRefreshing = false;
				}
			}
		}

		// Token: 0x06006315 RID: 25365 RVA: 0x002D5A04 File Offset: 0x002D3C04
		private void RefreshItemAt(int index, AvatarData avatarData, out bool isSelected, bool refreshAvatarImmediately = true)
		{
			isSelected = false;
			bool flag = index < 0 || index >= this._styleItems.Count;
			if (!flag)
			{
				NewGameSubPageAvatarStyleItem item = this._styleItems[index];
				bool flag2 = item == null;
				if (!flag2)
				{
					sbyte beardType = this.isUpperBeard ? 1 : 2;
					bool isShowing = avatarData.GetGrowableElementShowingState(beardType);
					bool flag3 = index == 0;
					if (flag3)
					{
						isSelected = !isShowing;
						item.Refresh(index, null, 20, isSelected, true, false);
					}
					else
					{
						bool flag4 = index - 1 >= this._beardResList.Count;
						if (!flag4)
						{
							short beardId = this._beardResList[index - 1].Id;
							short currentBeardId = this.isUpperBeard ? avatarData.Beard1Id : avatarData.Beard2Id;
							isSelected = (isShowing && beardId == currentBeardId);
							AvatarData previewDataWithBeard = new AvatarData(avatarData);
							bool flag5 = this.AvatarPage.IsCopyCloth();
							if (flag5)
							{
								previewDataWithBeard.ClothDisplayId = avatarData.ClothDisplayId;
							}
							previewDataWithBeard.SetGrowableElementShowingAbility(beardType, true);
							previewDataWithBeard.SetGrowableElementShowingState(beardType, true);
							bool flag6 = this.isUpperBeard;
							if (flag6)
							{
								previewDataWithBeard.Beard1Id = beardId;
							}
							else
							{
								previewDataWithBeard.Beard2Id = beardId;
							}
							NewGameSubPageAvatarStyleItem newGameSubPageAvatarStyleItem = item;
							AvatarData previewData = previewDataWithBeard;
							IAvatarSubPageParent avatarPage = this.AvatarPage;
							newGameSubPageAvatarStyleItem.Refresh(index, previewData, (avatarPage != null) ? avatarPage.GetAge() : 20, isSelected, false, refreshAvatarImmediately);
						}
					}
				}
			}
		}

		// Token: 0x06006316 RID: 25366 RVA: 0x002D5B64 File Offset: 0x002D3D64
		[return: TupleElementNames(new string[]
		{
			"start",
			"end"
		})]
		private ValueTuple<int, int> GetVisibleRange()
		{
			bool flag = this.scrollRect == null || this.beardStyleContainer == null || this._styleItems.Count == 0;
			ValueTuple<int, int> result;
			if (flag)
			{
				result = new ValueTuple<int, int>(0, this._styleItems.Count - 1);
			}
			else
			{
				RectTransform viewport = this.scrollRect.Viewport;
				bool flag2 = viewport == null;
				if (flag2)
				{
					result = new ValueTuple<int, int>(0, this._styleItems.Count - 1);
				}
				else
				{
					Rect viewportRect = viewport.rect;
					RectTransform contentPos = this.beardStyleContainer.transform as RectTransform;
					bool flag3 = contentPos == null;
					if (flag3)
					{
						result = new ValueTuple<int, int>(0, this._styleItems.Count - 1);
					}
					else
					{
						int visibleStart = 0;
						int visibleEnd = this._styleItems.Count - 1;
						for (int i = 0; i < this._styleItems.Count; i++)
						{
							RectTransform itemRect = this._styleItems[i].transform as RectTransform;
							bool flag4 = itemRect != null && this.IsVisible(itemRect, viewport);
							if (flag4)
							{
								visibleStart = Mathf.Max(0, i - 5);
								break;
							}
						}
						for (int j = this._styleItems.Count - 1; j >= 0; j--)
						{
							RectTransform itemRect2 = this._styleItems[j].transform as RectTransform;
							bool flag5 = itemRect2 != null && this.IsVisible(itemRect2, viewport);
							if (flag5)
							{
								visibleEnd = Mathf.Min(this._styleItems.Count - 1, j + 5);
								break;
							}
						}
						result = new ValueTuple<int, int>(visibleStart, visibleEnd);
					}
				}
			}
			return result;
		}

		// Token: 0x06006317 RID: 25367 RVA: 0x002D5D2C File Offset: 0x002D3F2C
		private bool IsVisible(RectTransform itemRect, RectTransform viewport)
		{
			Vector3[] itemWorldCorners = new Vector3[4];
			Vector3[] viewportWorldCorners = new Vector3[4];
			itemRect.GetWorldCorners(itemWorldCorners);
			viewport.GetWorldCorners(viewportWorldCorners);
			float itemCenterY = (itemWorldCorners[0].y + itemWorldCorners[2].y) / 2f;
			float viewportMinY = viewportWorldCorners[0].y;
			float viewportMaxY = viewportWorldCorners[2].y;
			float tolerance = (viewportMaxY - viewportMinY) * 0.5f;
			return itemCenterY >= viewportMinY - tolerance && itemCenterY <= viewportMaxY + tolerance;
		}

		// Token: 0x06006318 RID: 25368 RVA: 0x002D5DBC File Offset: 0x002D3FBC
		private void OnScroll()
		{
			bool flag = this.scrollRect == null || this.scrollRect.Content == null;
			if (!flag)
			{
				Vector2 currentPosition = this.scrollRect.Content.anchoredPosition;
				bool flag2 = Vector2.Distance(currentPosition, this._lastContentPosition) < 10f;
				if (!flag2)
				{
					this._lastContentPosition = currentPosition;
					ValueTuple<int, int> visibleRange = this.GetVisibleRange();
					int visibleStart = visibleRange.Item1;
					int visibleEnd = visibleRange.Item2;
					int i = visibleStart;
					while (i <= visibleEnd && i < this._styleItems.Count)
					{
						bool needsAvatarRefresh = this._styleItems[i].NeedsAvatarRefresh;
						if (needsAvatarRefresh)
						{
							this._styleItems[i].RefreshAvatarIfNeeded();
						}
						i++;
					}
				}
			}
		}

		// Token: 0x06006319 RID: 25369 RVA: 0x002D5E94 File Offset: 0x002D4094
		private void OnBeardColorChanged(int newIndex, int oldIndex)
		{
			bool flag = this._beardColors == null || newIndex < 0 || newIndex >= this._beardColors.Count;
			if (!flag)
			{
				AvatarData avatarData = base.GetAvatarData();
				bool flag2 = avatarData == null;
				if (!flag2)
				{
					byte colorId = this._beardColors[newIndex].Item1;
					avatarData.ColorFrontHairId = colorId;
					avatarData.ColorBackHairId = colorId;
					avatarData.ColorEyebrowId = colorId;
					avatarData.ColorBeard1Id = colorId;
					avatarData.ColorBeard2Id = colorId;
					base.RefreshAvatarAndMarkDirty();
				}
			}
		}

		// Token: 0x0600631A RID: 25370 RVA: 0x002D5F18 File Offset: 0x002D4118
		private void OnBeardStyleChanged(int newIndex, int oldIndex)
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				sbyte beardType = this.isUpperBeard ? 1 : 2;
				bool flag2 = newIndex == 0;
				if (flag2)
				{
					avatarData.SetGrowableElementShowingAbility(beardType, false);
					avatarData.SetGrowableElementShowingState(beardType, false);
					bool flag3 = this.isUpperBeard;
					if (flag3)
					{
						avatarData.Beard1Id = 1;
					}
					else
					{
						avatarData.Beard2Id = 1;
					}
				}
				else
				{
					bool flag4 = this._beardResList != null && newIndex - 1 < this._beardResList.Count;
					if (flag4)
					{
						short beardId = this._beardResList[newIndex - 1].Id;
						avatarData.SetGrowableElementShowingAbility(beardType, true);
						avatarData.SetGrowableElementShowingState(beardType, true);
						bool flag5 = this.isUpperBeard;
						if (flag5)
						{
							avatarData.Beard1Id = beardId;
						}
						else
						{
							avatarData.Beard2Id = beardId;
						}
					}
				}
				base.RefreshAvatarAndMarkDirty();
			}
		}

		// Token: 0x040044FC RID: 17660
		[Header("Title")]
		[SerializeField]
		private TextMeshProUGUI pageTitleText;

		// Token: 0x040044FD RID: 17661
		[SerializeField]
		private bool isUpperBeard;

		// Token: 0x040044FE RID: 17662
		[SerializeField]
		private bool onlyCreateRes = true;

		// Token: 0x040044FF RID: 17663
		[Header("胡须颜色")]
		[SerializeField]
		private CToggleGroup beardColorToggleGroup;

		// Token: 0x04004500 RID: 17664
		[SerializeField]
		private CToggle beardColorTemplate;

		// Token: 0x04004501 RID: 17665
		[Header("胡须样式")]
		[SerializeField]
		private CToggleGroup beardStyleToggleGroup;

		// Token: 0x04004502 RID: 17666
		[SerializeField]
		private Transform beardStyleContainer;

		// Token: 0x04004503 RID: 17667
		[SerializeField]
		private NewGameSubPageAvatarStyleItem beardStyleTemplate;

		// Token: 0x04004504 RID: 17668
		[SerializeField]
		private CScrollRect scrollRect;

		// Token: 0x04004505 RID: 17669
		private List<AvatarAsset> _beardResList;

		// Token: 0x04004506 RID: 17670
		private readonly List<NewGameSubPageAvatarStyleItem> _styleItems = new List<NewGameSubPageAvatarStyleItem>();

		// Token: 0x04004507 RID: 17671
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		private List<ValueTuple<byte, Color>> _beardColors;

		// Token: 0x04004508 RID: 17672
		private Vector2 _lastContentPosition = Vector2.zero;

		// Token: 0x04004509 RID: 17673
		private bool _isRefreshing = false;

		// Token: 0x0400450A RID: 17674
		private bool _isColorInitialized = false;

		// Token: 0x0400450B RID: 17675
		private bool _isStyleInitialized = false;
	}
}
