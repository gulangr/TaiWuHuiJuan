using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using GameData.Domains.Item;
using UnityEngine;
using UnityEngine.Profiling;

namespace Game.Views.NewGame
{
	// Token: 0x020007F0 RID: 2032
	public class NewGameSubPageAvatarClothPage : NewGameSubPageAvatarPageBase
	{
		// Token: 0x17000BF1 RID: 3057
		// (get) Token: 0x06006332 RID: 25394 RVA: 0x002D6A59 File Offset: 0x002D4C59
		// (set) Token: 0x06006333 RID: 25395 RVA: 0x002D6A61 File Offset: 0x002D4C61
		public bool OnlyCreateRes
		{
			get
			{
				return this.onlyCreateRes;
			}
			set
			{
				this.onlyCreateRes = value;
			}
		}

		// Token: 0x06006334 RID: 25396 RVA: 0x002D6A6C File Offset: 0x002D4C6C
		private void Awake()
		{
			this.onlyCreateRes = true;
			this.colorToggleTemplate.gameObject.SetActive(false);
			this.clothTemplate.gameObject.SetActive(false);
			this.scrollRect.OnScrollEvent += this.OnScroll;
		}

		// Token: 0x06006335 RID: 25397 RVA: 0x002D6AC0 File Offset: 0x002D4CC0
		protected override void OnEnable()
		{
			this.LoadClothColors();
			this.CreateColorToggles();
			this.clothContainer.gameObject.SetActive(!this.AvatarPage.OnlyShowShaveItems());
			bool flag = !this.AvatarPage.OnlyShowShaveItems();
			if (flag)
			{
				this.LoadClothes();
			}
		}

		// Token: 0x06006336 RID: 25398 RVA: 0x002D6B16 File Offset: 0x002D4D16
		public override void UpdateUI()
		{
			this.RefreshFromAvatarData();
		}

		// Token: 0x06006337 RID: 25399 RVA: 0x002D6B20 File Offset: 0x002D4D20
		private void LoadClothColors()
		{
			bool flag = this._clothColors != null;
			if (!flag)
			{
				this._clothColors = NewGameSubPageAvatarColorHelper.ClothColors;
			}
		}

		// Token: 0x06006338 RID: 25400 RVA: 0x002D6B48 File Offset: 0x002D4D48
		private void CreateColorToggles()
		{
			bool flag = this._clothColors == null || this.colorToggleTemplate == null || this.colorToggleGroup == null;
			if (!flag)
			{
				Transform container = this.colorToggleGroup.transform;
				CommonUtils.PrepareEnoughChildren(container, this.colorToggleTemplate.gameObject, this._clothColors.Count, null);
				this.colorToggleGroup.Clear();
				for (int i = 0; i < this._clothColors.Count; i++)
				{
					Color color = this._clothColors[i].Item2;
					Transform toggleTrans = container.GetChild(i);
					CImage colorImage = toggleTrans.GetChild(0).GetChild(0).GetComponent<CImage>();
					bool flag2 = colorImage != null;
					if (flag2)
					{
						colorImage.color = color;
					}
					this.colorToggleGroup.Add(toggleTrans.GetComponent<CToggle>());
				}
				bool flag3 = !this._isColorToggleInitialized;
				if (flag3)
				{
					this.colorToggleGroup.Init(-1);
					this.colorToggleGroup.OnActiveIndexChange += this.OnColorChanged;
					this._isColorToggleInitialized = true;
				}
			}
		}

		// Token: 0x06006339 RID: 25401 RVA: 0x002D6C7C File Offset: 0x002D4E7C
		private int GetColorIndex(byte colorId)
		{
			bool flag = this._clothColors == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < this._clothColors.Count; i++)
				{
					bool flag2 = this._clothColors[i].Item1 == colorId;
					if (flag2)
					{
						return i;
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x0600633A RID: 25402 RVA: 0x002D6CDC File Offset: 0x002D4EDC
		private void OnColorChanged(int newIndex, int oldIndex)
		{
			bool flag = this._clothColors == null || newIndex < 0 || newIndex >= this._clothColors.Count;
			if (!flag)
			{
				AvatarData avatarData = base.GetAvatarData();
				bool flag2 = avatarData == null;
				if (!flag2)
				{
					avatarData.ColorClothId = this._clothColors[newIndex].Item1;
					IAvatarSubPageParent avatarPage = this.AvatarPage;
					if (avatarPage != null)
					{
						avatarPage.RefreshAvatar();
					}
					IAvatarSubPageParent avatarPage2 = this.AvatarPage;
					if (avatarPage2 != null)
					{
						avatarPage2.MarkDirtyWithoutInscriptionClear();
					}
					this.RefreshAllItems();
				}
			}
		}

		// Token: 0x0600633B RID: 25403 RVA: 0x002D6D64 File Offset: 0x002D4F64
		private void LoadClothes()
		{
			this._bodyResList = new List<BodyRes>();
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				AvatarGroup group = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)avatarData.AvatarId);
				bool flag2 = group == null;
				if (!flag2)
				{
					bool flag3 = group.BodyRes != null;
					if (flag3)
					{
						foreach (BodyRes bodyRes in group.BodyRes)
						{
							bool flag4;
							if (this.onlyCreateRes)
							{
								AvatarAsset cloth = bodyRes.Cloth;
								if (cloth == null)
								{
									flag4 = false;
								}
								else
								{
									AvatarElementsItem config = cloth.Config;
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
								AvatarAsset cloth2 = bodyRes.Cloth;
								bool flag8;
								if (cloth2 == null)
								{
									flag8 = true;
								}
								else
								{
									AvatarElementsItem config2 = cloth2.Config;
									bool? flag5 = (config2 != null) ? new bool?(config2.CanCreate) : null;
									bool flag6 = true;
									flag8 = !(flag5.GetValueOrDefault() == flag6 & flag5 != null);
								}
								short num;
								bool flag9 = flag8 && !ItemTemplateHelper.TryGetClothingTemplateIdByDisplayId((byte)bodyRes.Id, out num);
								if (!flag9)
								{
									this._bodyResList.Add(bodyRes);
								}
							}
						}
					}
					this.CreateClothItems();
				}
			}
		}

		// Token: 0x0600633C RID: 25404 RVA: 0x002D6EE4 File Offset: 0x002D50E4
		private void CreateClothItems()
		{
			bool flag = this.clothTemplate == null || this.clothContainer == null || this.clothToggleGroup == null;
			if (!flag)
			{
				CommonUtils.PrepareEnoughChildren(this.clothContainer, this.clothTemplate.gameObject, this._bodyResList.Count, null);
				this.clothToggleGroup.Clear();
				this._clothItems.Clear();
				for (int i = 0; i < this._bodyResList.Count; i++)
				{
					NewGameSubPageAvatarClothItem item = this.clothContainer.GetChild(i).GetComponent<NewGameSubPageAvatarClothItem>();
					bool flag2 = item != null;
					if (flag2)
					{
						this._clothItems.Add(item);
						this.clothToggleGroup.Add(item.Toggle);
					}
				}
				this.clothToggleGroup.Init(-1);
				bool flag3 = !this._isClothToggleInitialized;
				if (flag3)
				{
					this.clothToggleGroup.OnActiveIndexChange += this.OnClothChanged;
					this._isClothToggleInitialized = true;
				}
				this.RefreshAllItems();
			}
		}

		// Token: 0x0600633D RID: 25405 RVA: 0x002D700C File Offset: 0x002D520C
		private void RefreshAllItems()
		{
			bool isRefreshing = this._isRefreshing;
			if (!isRefreshing)
			{
				this._isRefreshing = true;
				Profiler.BeginSample("ClothPage.RefreshAllItems");
				AvatarData avatarData = base.GetAvatarData();
				bool flag = avatarData == null;
				if (flag)
				{
					this._isRefreshing = false;
					Profiler.EndSample();
				}
				else
				{
					short currentClothId = avatarData.ClothDisplayId;
					int selectedIndex = -1;
					ValueTuple<int, int> visibleRange = this.GetVisibleRange();
					int visibleStart = visibleRange.Item1;
					int visibleEnd = visibleRange.Item2;
					for (int i = 0; i < this._clothItems.Count; i++)
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
					bool flag3 = selectedIndex >= 0;
					if (flag3)
					{
						this.clothToggleGroup.Set(selectedIndex, false);
					}
					Profiler.EndSample();
					this._isRefreshing = false;
				}
			}
		}

		// Token: 0x0600633E RID: 25406 RVA: 0x002D70F8 File Offset: 0x002D52F8
		private void RefreshFromAvatarData()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				bool flag2 = this.colorToggleGroup != null && this._clothColors != null;
				if (flag2)
				{
					int index = this.GetColorIndex(avatarData.ColorClothId);
					bool flag3 = index >= 0;
					if (flag3)
					{
						this.colorToggleGroup.Set(index, false);
					}
				}
				this.RefreshAllItems();
			}
		}

		// Token: 0x0600633F RID: 25407 RVA: 0x002D7168 File Offset: 0x002D5368
		private void RefreshItemAt(int index, AvatarData avatarData, out bool isSelected, bool refreshAvatarImmediately = true)
		{
			isSelected = false;
			bool flag = index < 0 || index >= this._clothItems.Count || index >= this._bodyResList.Count;
			if (!flag)
			{
				NewGameSubPageAvatarClothItem item = this._clothItems[index];
				bool flag2 = item == null;
				if (!flag2)
				{
					short clothId = this._bodyResList[index].Id;
					isSelected = (clothId == avatarData.ClothDisplayId);
					AvatarData previewData = new AvatarData(avatarData);
					previewData.ClothDisplayId = clothId;
					NewGameSubPageAvatarClothItem newGameSubPageAvatarClothItem = item;
					AvatarData previewData2 = previewData;
					IAvatarSubPageParent avatarPage = this.AvatarPage;
					newGameSubPageAvatarClothItem.Refresh(index, previewData2, (avatarPage != null) ? avatarPage.GetAge() : 20, isSelected, false, refreshAvatarImmediately);
				}
			}
		}

		// Token: 0x06006340 RID: 25408 RVA: 0x002D7210 File Offset: 0x002D5410
		[return: TupleElementNames(new string[]
		{
			"start",
			"end"
		})]
		private ValueTuple<int, int> GetVisibleRange()
		{
			bool flag = this.scrollRect == null || this.clothContainer == null || this._clothItems.Count == 0;
			ValueTuple<int, int> result;
			if (flag)
			{
				result = new ValueTuple<int, int>(0, this._clothItems.Count - 1);
			}
			else
			{
				RectTransform viewport = this.scrollRect.Viewport;
				bool flag2 = viewport == null;
				if (flag2)
				{
					result = new ValueTuple<int, int>(0, this._clothItems.Count - 1);
				}
				else
				{
					int visibleStart = 0;
					int visibleEnd = this._clothItems.Count - 1;
					for (int i = 0; i < this._clothItems.Count; i++)
					{
						RectTransform itemRect = this._clothItems[i].transform as RectTransform;
						bool flag3 = itemRect != null && this.IsVisible(itemRect, viewport);
						if (flag3)
						{
							visibleStart = Mathf.Max(0, i - 5);
							break;
						}
					}
					for (int j = this._clothItems.Count - 1; j >= 0; j--)
					{
						RectTransform itemRect2 = this._clothItems[j].transform as RectTransform;
						bool flag4 = itemRect2 != null && this.IsVisible(itemRect2, viewport);
						if (flag4)
						{
							visibleEnd = Mathf.Min(this._clothItems.Count - 1, j + 5);
							break;
						}
					}
					result = new ValueTuple<int, int>(visibleStart, visibleEnd);
				}
			}
			return result;
		}

		// Token: 0x06006341 RID: 25409 RVA: 0x002D7394 File Offset: 0x002D5594
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

		// Token: 0x06006342 RID: 25410 RVA: 0x002D7424 File Offset: 0x002D5624
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
					while (i <= visibleEnd && i < this._clothItems.Count)
					{
						bool needsAvatarRefresh = this._clothItems[i].NeedsAvatarRefresh;
						if (needsAvatarRefresh)
						{
							this._clothItems[i].RefreshAvatarIfNeeded();
						}
						i++;
					}
				}
			}
		}

		// Token: 0x06006343 RID: 25411 RVA: 0x002D74FC File Offset: 0x002D56FC
		private void OnClothChanged(int newIndex, int oldIndex)
		{
			bool flag = this._bodyResList == null || newIndex < 0 || newIndex >= this._bodyResList.Count;
			if (!flag)
			{
				AvatarData avatarData = base.GetAvatarData();
				bool flag2 = avatarData == null;
				if (!flag2)
				{
					avatarData.ClothDisplayId = this._bodyResList[newIndex].Id;
					IAvatarSubPageParent avatarPage = this.AvatarPage;
					if (avatarPage != null)
					{
						avatarPage.RefreshAvatar();
					}
					IAvatarSubPageParent avatarPage2 = this.AvatarPage;
					if (avatarPage2 != null)
					{
						avatarPage2.MarkDirtyWithoutInscriptionClear();
					}
					this.RefreshAllItems();
				}
			}
		}

		// Token: 0x0400451E RID: 17694
		[SerializeField]
		private bool onlyCreateRes = true;

		// Token: 0x0400451F RID: 17695
		[Header("颜色")]
		[SerializeField]
		private CToggleGroup colorToggleGroup;

		// Token: 0x04004520 RID: 17696
		[SerializeField]
		private CToggle colorToggleTemplate;

		// Token: 0x04004521 RID: 17697
		[Header("衣服选择")]
		[SerializeField]
		private CToggleGroup clothToggleGroup;

		// Token: 0x04004522 RID: 17698
		[SerializeField]
		private Transform clothContainer;

		// Token: 0x04004523 RID: 17699
		[SerializeField]
		private NewGameSubPageAvatarClothItem clothTemplate;

		// Token: 0x04004524 RID: 17700
		[SerializeField]
		private CScrollRect scrollRect;

		// Token: 0x04004525 RID: 17701
		private bool _isClothToggleInitialized;

		// Token: 0x04004526 RID: 17702
		private bool _isColorToggleInitialized;

		// Token: 0x04004527 RID: 17703
		private List<BodyRes> _bodyResList;

		// Token: 0x04004528 RID: 17704
		private readonly List<NewGameSubPageAvatarClothItem> _clothItems = new List<NewGameSubPageAvatarClothItem>();

		// Token: 0x04004529 RID: 17705
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		private List<ValueTuple<byte, Color>> _clothColors;

		// Token: 0x0400452A RID: 17706
		private Vector2 _lastContentPosition = Vector2.zero;

		// Token: 0x0400452B RID: 17707
		private bool _isRefreshing = false;
	}
}
