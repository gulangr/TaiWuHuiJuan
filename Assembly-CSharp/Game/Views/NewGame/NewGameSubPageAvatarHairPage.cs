using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007F7 RID: 2039
	public class NewGameSubPageAvatarHairPage : NewGameSubPageAvatarPageBase
	{
		// Token: 0x06006396 RID: 25494 RVA: 0x002DA2C4 File Offset: 0x002D84C4
		public void SetHairType(bool backHair)
		{
			bool isFront = !backHair;
			bool flag = this.isFrontHair != isFront;
			if (flag)
			{
				this.isFrontHair = isFront;
				bool isActiveAndEnabled = base.isActiveAndEnabled;
				if (isActiveAndEnabled)
				{
					this.LoadHairStyles();
					this.UpdateTitle();
					this.RefreshFromAvatarData();
				}
			}
		}

		// Token: 0x06006397 RID: 25495 RVA: 0x002DA314 File Offset: 0x002D8514
		private void Awake()
		{
			bool flag = this.hairColorTemplate != null;
			if (flag)
			{
				this.hairColorTemplate.gameObject.SetActive(false);
			}
			bool flag2 = this.hairStyleTemplate != null;
			if (flag2)
			{
				this.hairStyleTemplate.gameObject.SetActive(false);
			}
			this.scrollRect.OnScrollEvent += this.OnScroll;
		}

		// Token: 0x06006398 RID: 25496 RVA: 0x002DA380 File Offset: 0x002D8580
		protected override void OnEnable()
		{
			this.LoadHairColors();
			this.CreateHairColorToggles();
			this.LoadHairStyles();
			this.UpdateTitle();
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData != null;
			if (flag)
			{
				this.RefreshColorSelection(avatarData);
			}
		}

		// Token: 0x06006399 RID: 25497 RVA: 0x002DA3C3 File Offset: 0x002D85C3
		public override void UpdateUI()
		{
			this.RefreshFromAvatarData();
		}

		// Token: 0x0600639A RID: 25498 RVA: 0x002DA3D0 File Offset: 0x002D85D0
		private void UpdateTitle()
		{
			bool flag = this.pageTitleText == null;
			if (!flag)
			{
				LanguageKey key = this.isFrontHair ? LanguageKey.UI_NewGame_AdjustTitle_3 : LanguageKey.UI_NewGame_AdjustTitle_4;
				this.pageTitleText.text = LocalStringManager.Get(key);
			}
		}

		// Token: 0x0600639B RID: 25499 RVA: 0x002DA418 File Offset: 0x002D8618
		private void LoadHairColors()
		{
			bool flag = this._hairColors != null;
			if (!flag)
			{
				this._hairColors = NewGameSubPageAvatarColorHelper.HairColors;
			}
		}

		// Token: 0x0600639C RID: 25500 RVA: 0x002DA440 File Offset: 0x002D8640
		private void CreateHairColorToggles()
		{
			bool flag = this._hairColors == null;
			if (!flag)
			{
				Transform container = this.hairColorToggleGroup.transform;
				CommonUtils.PrepareEnoughChildren(container, this.hairColorTemplate.gameObject, this._hairColors.Count, null);
				this.hairColorToggleGroup.Clear();
				for (int i = 0; i < this._hairColors.Count; i++)
				{
					Color color = this._hairColors[i].Item2;
					Transform toggleTrans = container.GetChild(i);
					CImage colorImage = toggleTrans.GetChild(0).GetChild(0).GetComponent<CImage>();
					colorImage.color = color;
					this.hairColorToggleGroup.Add(toggleTrans.GetComponent<CToggle>());
				}
				this.hairColorToggleGroup.Init(-1);
				bool flag2 = !this._isColorInitialized;
				if (flag2)
				{
					this.hairColorToggleGroup.OnActiveIndexChange += this.OnHairColorChanged;
					this._isColorInitialized = true;
				}
			}
		}

		// Token: 0x0600639D RID: 25501 RVA: 0x002DA548 File Offset: 0x002D8748
		private void LoadHairStyles()
		{
			this._hairResList = new List<HairRes>();
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				AvatarGroup group = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)avatarData.AvatarId);
				bool flag2 = group == null;
				if (!flag2)
				{
					List<HairRes> sourceList = this.isFrontHair ? group.Hair1Res : group.Hair2Res;
					bool flag3 = sourceList != null;
					if (flag3)
					{
						foreach (HairRes hairRes in sourceList)
						{
							bool flag4;
							if (this.onlyCreateRes)
							{
								AvatarAsset hair = hairRes.Hair;
								if (hair == null)
								{
									flag4 = false;
								}
								else
								{
									AvatarElementsItem config = hair.Config;
									bool? flag5 = (config != null) ? new bool?(config.CanCreate) : null;
									bool flag6 = true;
									flag4 = (flag5.GetValueOrDefault() == flag6 & flag5 != null);
								}
							}
							else
							{
								flag4 = true;
							}
							bool flag7 = flag4;
							if (flag7)
							{
								this._hairResList.Add(hairRes);
							}
						}
					}
					Func<List<HairRes>, List<HairRes>> func;
					if (!this.isFrontHair)
					{
						IAvatarSubPageParent avatarPage = this.AvatarPage;
						func = ((avatarPage != null) ? avatarPage.GetBackHairFilter() : null);
					}
					else
					{
						IAvatarSubPageParent avatarPage2 = this.AvatarPage;
						func = ((avatarPage2 != null) ? avatarPage2.GetFrontHairFilter() : null);
					}
					Func<List<HairRes>, List<HairRes>> filter = func;
					bool flag8 = filter != null;
					if (flag8)
					{
						this._hairResList = filter(this._hairResList);
					}
					this.CreateStyleItems();
				}
			}
		}

		// Token: 0x0600639E RID: 25502 RVA: 0x002DA6BC File Offset: 0x002D88BC
		private void CreateStyleItems()
		{
			bool flag = this.hairStyleTemplate == null || this.hairStyleContainer == null || this.hairStyleToggleGroup == null;
			if (!flag)
			{
				int totalCount = this._hairResList.Count + 1;
				CommonUtils.PrepareEnoughChildren(this.hairStyleContainer, this.hairStyleTemplate.gameObject, totalCount, null);
				this.hairStyleToggleGroup.Clear();
				this._styleItems.Clear();
				for (int i = 0; i < totalCount; i++)
				{
					NewGameSubPageAvatarStyleItem item = this.hairStyleContainer.GetChild(i).GetComponent<NewGameSubPageAvatarStyleItem>();
					bool flag2 = item != null;
					if (flag2)
					{
						this._styleItems.Add(item);
					}
					this.hairStyleToggleGroup.Add(item.Toggle);
				}
				this.hairStyleToggleGroup.Init(-1);
				bool flag3 = !this._isStyleInitialized;
				if (flag3)
				{
					this.hairStyleToggleGroup.OnActiveIndexChange += this.OnHairStyleChanged;
					this._isStyleInitialized = true;
				}
				this.RefreshAllItems();
			}
		}

		// Token: 0x0600639F RID: 25503 RVA: 0x002DA7E4 File Offset: 0x002D89E4
		private void RefreshFromAvatarData()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				this.RefreshColorSelection(avatarData);
				this.RefreshAllItems();
			}
		}

		// Token: 0x060063A0 RID: 25504 RVA: 0x002DA814 File Offset: 0x002D8A14
		private int GetHairColorIndex(byte colorId)
		{
			bool flag = this._hairColors == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < this._hairColors.Count; i++)
				{
					bool flag2 = this._hairColors[i].Item1 == colorId;
					if (flag2)
					{
						return i;
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x060063A1 RID: 25505 RVA: 0x002DA874 File Offset: 0x002D8A74
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
					bool flag3 = selectedIndex >= 0 && this.hairStyleToggleGroup != null;
					if (flag3)
					{
						this.hairStyleToggleGroup.Get(selectedIndex).isOn = false;
						this.hairStyleToggleGroup.Set(selectedIndex, false);
					}
					this._isRefreshing = false;
				}
			}
		}

		// Token: 0x060063A2 RID: 25506 RVA: 0x002DA960 File Offset: 0x002D8B60
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
					bool flag3 = index == 0;
					if (flag3)
					{
						isSelected = !avatarData.GetGrowableElementShowingState(0);
						item.Refresh(index, null, 20, isSelected, true, false);
					}
					else
					{
						bool flag4 = index - 1 >= this._hairResList.Count;
						if (!flag4)
						{
							short hairId = this._hairResList[index - 1].Id;
							short currentHairId = this.isFrontHair ? avatarData.FrontHairId : avatarData.BackHairId;
							isSelected = (hairId == currentHairId && avatarData.GetGrowableElementShowingState(0));
							AvatarData previewData = new AvatarData(avatarData);
							bool flag5 = this.AvatarPage.IsCopyCloth();
							if (flag5)
							{
								previewData.ClothDisplayId = avatarData.ClothDisplayId;
							}
							previewData.SetGrowableElementShowingState(0, true);
							bool flag6 = this.isFrontHair;
							if (flag6)
							{
								previewData.FrontHairId = hairId;
							}
							else
							{
								previewData.BackHairId = hairId;
							}
							NewGameSubPageAvatarStyleItem newGameSubPageAvatarStyleItem = item;
							AvatarData previewData2 = previewData;
							IAvatarSubPageParent avatarPage = this.AvatarPage;
							newGameSubPageAvatarStyleItem.Refresh(index, previewData2, (avatarPage != null) ? avatarPage.GetAge() : 20, isSelected, false, refreshAvatarImmediately);
						}
					}
				}
			}
		}

		// Token: 0x060063A3 RID: 25507 RVA: 0x002DAAA8 File Offset: 0x002D8CA8
		[return: TupleElementNames(new string[]
		{
			"start",
			"end"
		})]
		private ValueTuple<int, int> GetVisibleRange()
		{
			bool flag = this.scrollRect == null || this.hairStyleContainer == null || this._styleItems.Count == 0;
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
					RectTransform contentPos = this.hairStyleContainer.transform as RectTransform;
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

		// Token: 0x060063A4 RID: 25508 RVA: 0x002DAC70 File Offset: 0x002D8E70
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

		// Token: 0x060063A5 RID: 25509 RVA: 0x002DAD00 File Offset: 0x002D8F00
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

		// Token: 0x060063A6 RID: 25510 RVA: 0x002DADD8 File Offset: 0x002D8FD8
		private void RefreshColorSelection(AvatarData avatarData)
		{
			bool flag = this.hairColorToggleGroup != null && this._hairColors != null;
			if (flag)
			{
				byte colorId = this.isFrontHair ? avatarData.ColorFrontHairId : avatarData.ColorBackHairId;
				int index = this.GetHairColorIndex(colorId);
				bool flag2 = index >= 0;
				if (flag2)
				{
					this.hairColorToggleGroup.SetWithoutNotify(index);
				}
			}
		}

		// Token: 0x060063A7 RID: 25511 RVA: 0x002DAE40 File Offset: 0x002D9040
		private void OnHairColorChanged(int newIndex, int oldIndex)
		{
			bool flag = this._hairColors == null || newIndex < 0 || newIndex >= this._hairColors.Count;
			if (!flag)
			{
				AvatarData avatarData = base.GetAvatarData();
				bool flag2 = avatarData == null;
				if (!flag2)
				{
					byte colorId = this._hairColors[newIndex].Item1;
					avatarData.ColorFrontHairId = colorId;
					avatarData.ColorBackHairId = colorId;
					avatarData.ColorEyebrowId = colorId;
					avatarData.ColorBeard1Id = colorId;
					avatarData.ColorBeard2Id = colorId;
					base.RefreshAvatarAndMarkDirty();
				}
			}
		}

		// Token: 0x060063A8 RID: 25512 RVA: 0x002DAEC4 File Offset: 0x002D90C4
		private void OnHairStyleChanged(int newIndex, int oldIndex)
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				bool flag2 = newIndex == 0;
				if (flag2)
				{
					avatarData.SetGrowableElementShowingState(0, false);
					AvatarGroup group = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)avatarData.AvatarId);
					bool flag3 = group != null;
					if (flag3)
					{
						List<HairRes> hairRes = this.isFrontHair ? group.Hair1Res : group.Hair2Res;
						bool flag4 = hairRes != null && hairRes.Count > 0;
						if (flag4)
						{
							bool flag5 = this.isFrontHair;
							if (flag5)
							{
								avatarData.FrontHairId = hairRes[0].Id;
							}
							else
							{
								avatarData.BackHairId = hairRes[0].Id;
							}
						}
					}
				}
				else
				{
					bool flag6 = this._hairResList != null && newIndex - 1 < this._hairResList.Count;
					if (flag6)
					{
						short hairId = this._hairResList[newIndex - 1].Id;
						avatarData.SetGrowableElementShowingState(0, true);
						bool flag7 = this.isFrontHair;
						if (flag7)
						{
							avatarData.FrontHairId = hairId;
							IAvatarSubPageParent avatarPage = this.AvatarPage;
							if (avatarPage != null)
							{
								avatarPage.RefreshHairTabsIfNeeded();
							}
						}
						else
						{
							avatarData.BackHairId = hairId;
						}
					}
				}
				base.RefreshAvatarAndMarkDirty();
			}
		}

		// Token: 0x04004576 RID: 17782
		[Header("Title")]
		[SerializeField]
		private TextMeshProUGUI pageTitleText;

		// Token: 0x04004577 RID: 17783
		[SerializeField]
		private bool isFrontHair;

		// Token: 0x04004578 RID: 17784
		[SerializeField]
		private bool onlyCreateRes = true;

		// Token: 0x04004579 RID: 17785
		[Header("发色")]
		[SerializeField]
		private CToggleGroup hairColorToggleGroup;

		// Token: 0x0400457A RID: 17786
		[SerializeField]
		private CToggle hairColorTemplate;

		// Token: 0x0400457B RID: 17787
		[Header("发型")]
		[SerializeField]
		private CToggleGroup hairStyleToggleGroup;

		// Token: 0x0400457C RID: 17788
		[SerializeField]
		private Transform hairStyleContainer;

		// Token: 0x0400457D RID: 17789
		[SerializeField]
		private NewGameSubPageAvatarStyleItem hairStyleTemplate;

		// Token: 0x0400457E RID: 17790
		[SerializeField]
		private CScrollRect scrollRect;

		// Token: 0x0400457F RID: 17791
		private List<HairRes> _hairResList;

		// Token: 0x04004580 RID: 17792
		private readonly List<NewGameSubPageAvatarStyleItem> _styleItems = new List<NewGameSubPageAvatarStyleItem>();

		// Token: 0x04004581 RID: 17793
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		private List<ValueTuple<byte, Color>> _hairColors;

		// Token: 0x04004582 RID: 17794
		private Vector2 _lastContentPosition = Vector2.zero;

		// Token: 0x04004583 RID: 17795
		private bool _isRefreshing = false;

		// Token: 0x04004584 RID: 17796
		private bool _isColorInitialized = false;

		// Token: 0x04004585 RID: 17797
		private bool _isStyleInitialized = false;
	}
}
