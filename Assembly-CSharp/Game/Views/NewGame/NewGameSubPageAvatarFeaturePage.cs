using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.NewGame
{
	// Token: 0x020007F6 RID: 2038
	public class NewGameSubPageAvatarFeaturePage : NewGameSubPageAvatarPageBase
	{
		// Token: 0x06006383 RID: 25475 RVA: 0x002D9828 File Offset: 0x002D7A28
		public void SetFeatureType(EAvatarElementsType type)
		{
			int newIndex = (type == EAvatarElementsType.Feature1) ? 1 : 2;
			bool flag = this.featureIndex != newIndex;
			if (flag)
			{
				this.featureIndex = newIndex;
				bool isActiveAndEnabled = base.isActiveAndEnabled;
				if (isActiveAndEnabled)
				{
					this.UpdateTitle();
					this.LoadFeatures();
					this.UpdateColorAreaVisibility();
					this.UpdatePositionAreaVisibility();
					this.featureContainer.gameObject.SetActive(!this.AvatarPage.OnlyShowShaveItems());
				}
			}
		}

		// Token: 0x06006384 RID: 25476 RVA: 0x002D98A0 File Offset: 0x002D7AA0
		private void Awake()
		{
			this.positionToggleGroup.Init(-1);
			this.positionToggleGroup.OnActiveIndexChange += this.OnPositionChanged;
			this.colorToggleTemplate.gameObject.SetActive(false);
			this.featureTemplate.gameObject.SetActive(false);
		}

		// Token: 0x06006385 RID: 25477 RVA: 0x002D98F7 File Offset: 0x002D7AF7
		protected override void OnEnable()
		{
			this.UpdateTitle();
			this.LoadFeatureColors();
			this.CreateColorToggles();
			this.LoadFeatures();
		}

		// Token: 0x06006386 RID: 25478 RVA: 0x002D9916 File Offset: 0x002D7B16
		public override void UpdateUI()
		{
			this.RefreshFromAvatarData();
		}

		// Token: 0x06006387 RID: 25479 RVA: 0x002D9920 File Offset: 0x002D7B20
		private void UpdateTitle()
		{
			string key = (this.featureIndex == 1) ? "UI_NewGame_AdjustTitle_11" : "UI_NewGame_AdjustTitle_12";
			this.pageTitleText.text = LocalStringManager.Get(key);
		}

		// Token: 0x06006388 RID: 25480 RVA: 0x002D9958 File Offset: 0x002D7B58
		private void LoadFeatureColors()
		{
			bool flag = this._featureColors != null;
			if (!flag)
			{
				this._featureColors = NewGameSubPageAvatarColorHelper.FeatureColors;
			}
		}

		// Token: 0x06006389 RID: 25481 RVA: 0x002D9980 File Offset: 0x002D7B80
		private void CreateColorToggles()
		{
			bool flag = this._featureColors == null;
			if (!flag)
			{
				Transform container = this.colorToggleGroup.transform;
				CommonUtils.PrepareEnoughChildren(container, this.colorToggleTemplate.gameObject, this._featureColors.Count, null);
				this.colorToggleGroup.Clear();
				for (int i = 0; i < this._featureColors.Count; i++)
				{
					Color color = this._featureColors[i].Item2;
					Transform toggleTrans = container.GetChild(i);
					CImage colorImage = toggleTrans.GetChild(0).GetChild(0).GetComponent<CImage>();
					colorImage.color = color;
					this.colorToggleGroup.Add(toggleTrans.GetComponent<CToggle>());
				}
				bool flag2 = !this._isColorToggleInitialized;
				if (flag2)
				{
					this.colorToggleGroup.Init(-1);
					this.colorToggleGroup.OnActiveIndexChange += this.OnColorChanged;
					this._isColorToggleInitialized = true;
				}
			}
		}

		// Token: 0x0600638A RID: 25482 RVA: 0x002D9A88 File Offset: 0x002D7C88
		private int GetColorIndex(byte colorId)
		{
			bool flag = this._featureColors == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < this._featureColors.Count; i++)
				{
					bool flag2 = this._featureColors[i].Item1 == colorId;
					if (flag2)
					{
						return i;
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x0600638B RID: 25483 RVA: 0x002D9AE8 File Offset: 0x002D7CE8
		private void LoadFeatures()
		{
			this._featureList = new List<AvatarAsset>();
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				AvatarGroup group = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)avatarData.AvatarId);
				bool flag2 = group == null;
				if (!flag2)
				{
					List<AvatarAsset> sourceList = (this.featureIndex == 1) ? group.Feature1Res : group.Feature2Res;
					bool flag3 = sourceList != null;
					if (flag3)
					{
						List<AvatarAsset> filteredList = AvatarGroup.GetFeatureResExcludeDelete(sourceList);
						foreach (AvatarAsset feature in filteredList)
						{
							bool flag4 = feature.Id != 1;
							if (flag4)
							{
								this._featureList.Add(feature);
							}
						}
					}
					Func<List<AvatarAsset>, List<AvatarAsset>> func;
					if (this.featureIndex != 1)
					{
						IAvatarSubPageParent avatarPage = this.AvatarPage;
						func = ((avatarPage != null) ? avatarPage.GetFeature2Filter() : null);
					}
					else
					{
						IAvatarSubPageParent avatarPage2 = this.AvatarPage;
						func = ((avatarPage2 != null) ? avatarPage2.GetFeature1Filter() : null);
					}
					Func<List<AvatarAsset>, List<AvatarAsset>> filter = func;
					bool flag5 = filter != null;
					if (flag5)
					{
						this._featureList = filter(this._featureList);
					}
					this.CreateFeatureItems();
				}
			}
		}

		// Token: 0x0600638C RID: 25484 RVA: 0x002D9C1C File Offset: 0x002D7E1C
		private void CreateFeatureItems()
		{
			List<AvatarAsset> featureList = this._featureList;
			int totalCount = ((featureList != null) ? featureList.Count : 0) + 1;
			CommonUtils.PrepareEnoughChildren(this.featureContainer, this.featureTemplate.gameObject, totalCount, null);
			this.featureToggleGroup.Clear();
			this._featureItems.Clear();
			for (int i = 0; i < totalCount; i++)
			{
				NewGameSubPageAvatarStyleItem item = this.featureContainer.GetChild(i).GetComponent<NewGameSubPageAvatarStyleItem>();
				this._featureItems.Add(item);
				this.featureToggleGroup.Add(item.Toggle);
			}
			this.featureToggleGroup.Init(-1);
			bool flag = !this._isFeatureToggleInitialized;
			if (flag)
			{
				this.featureToggleGroup.OnActiveIndexChange += this.OnFeatureChanged;
				this._isFeatureToggleInitialized = true;
			}
			this.RefreshAllItems();
		}

		// Token: 0x0600638D RID: 25485 RVA: 0x002D9D04 File Offset: 0x002D7F04
		private void RefreshFromAvatarData()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				sbyte mirrorType = (this.featureIndex == 1) ? avatarData.Feature1MirrorType : avatarData.Feature2MirrorType;
				this.positionToggleGroup.Set((int)mirrorType, false);
				bool flag2 = this._featureColors != null;
				if (flag2)
				{
					byte colorId = (this.featureIndex == 1) ? avatarData.ColorFeature1Id : avatarData.ColorFeature2Id;
					int index = this.GetColorIndex(colorId);
					bool flag3 = index >= 0;
					if (flag3)
					{
						this.colorToggleGroup.Set(index, false);
					}
				}
				this.UpdateColorAreaVisibility();
				this.UpdatePositionAreaVisibility();
				this.featureContainer.gameObject.SetActive(!this.AvatarPage.OnlyShowShaveItems());
				this.RefreshAllItems();
			}
		}

		// Token: 0x0600638E RID: 25486 RVA: 0x002D9DD0 File Offset: 0x002D7FD0
		private void RefreshAllItems()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				int selectedIndex = -1;
				for (int i = 0; i < this._featureItems.Count; i++)
				{
					bool isSelected;
					this.RefreshItemAt(i, avatarData, out isSelected);
					bool flag2 = isSelected;
					if (flag2)
					{
						selectedIndex = i;
					}
				}
				bool flag3 = selectedIndex >= 0;
				if (flag3)
				{
					this.featureToggleGroup.Set(selectedIndex, false);
				}
			}
		}

		// Token: 0x0600638F RID: 25487 RVA: 0x002D9E44 File Offset: 0x002D8044
		private void RefreshItemAt(int index, AvatarData avatarData, out bool isSelected)
		{
			isSelected = false;
			bool flag = index < 0 || index >= this._featureItems.Count;
			if (!flag)
			{
				NewGameSubPageAvatarStyleItem item = this._featureItems[index];
				bool flag2 = item == null;
				if (!flag2)
				{
					bool isEmpty = index == 0;
					short featureId = isEmpty ? 1 : 0;
					bool flag3 = !isEmpty && this._featureList != null && index - 1 < this._featureList.Count;
					if (flag3)
					{
						featureId = this._featureList[index - 1].Id;
					}
					short currentFeatureId = (this.featureIndex == 1) ? avatarData.Feature1Id : avatarData.Feature2Id;
					bool currentIsEmpty = currentFeatureId == 1;
					bool thisIsEmpty = featureId == 1;
					bool flag4 = currentIsEmpty && thisIsEmpty;
					if (flag4)
					{
						isSelected = true;
					}
					else
					{
						isSelected = (featureId == currentFeatureId);
					}
					AvatarData previewData = null;
					bool flag5 = !isEmpty;
					if (flag5)
					{
						previewData = new AvatarData(avatarData);
						bool flag6 = this.featureIndex == 1;
						if (flag6)
						{
							previewData.Feature1Id = featureId;
						}
						else
						{
							previewData.Feature2Id = featureId;
						}
						bool flag7 = this.AvatarPage.IsCopyCloth();
						if (flag7)
						{
							previewData.ClothDisplayId = avatarData.ClothDisplayId;
						}
					}
					NewGameSubPageAvatarStyleItem newGameSubPageAvatarStyleItem = item;
					AvatarData previewData2 = previewData;
					IAvatarSubPageParent avatarPage = this.AvatarPage;
					newGameSubPageAvatarStyleItem.Refresh(index, previewData2, (avatarPage != null) ? avatarPage.GetAge() : 20, isSelected, isEmpty, true);
				}
			}
		}

		// Token: 0x06006390 RID: 25488 RVA: 0x002D9F94 File Offset: 0x002D8194
		private void UpdateColorAreaVisibility()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				short featureId = (this.featureIndex == 1) ? avatarData.Feature1Id : avatarData.Feature2Id;
				bool hasColor = false;
				bool flag2 = featureId != 1 && this._featureList != null;
				if (flag2)
				{
					AvatarAsset feature = this._featureList.Find((AvatarAsset f) => f.Id == featureId);
					bool flag3 = ((feature != null) ? feature.Config : null) != null;
					if (flag3)
					{
						hasColor = (feature.Config.ColorGroup > 0);
					}
				}
				this.colorArea.SetActive(hasColor);
			}
		}

		// Token: 0x06006391 RID: 25489 RVA: 0x002DA048 File Offset: 0x002D8248
		private void UpdatePositionAreaVisibility()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				short featureId = (this.featureIndex == 1) ? avatarData.Feature1Id : avatarData.Feature2Id;
				bool canMirror = true;
				bool flag2 = featureId != 1 && this._featureList != null;
				if (flag2)
				{
					AvatarAsset feature = this._featureList.Find((AvatarAsset f) => f.Id == featureId);
					bool flag3 = ((feature != null) ? feature.Config : null) != null;
					if (flag3)
					{
						canMirror = feature.Config.CanMirror;
					}
				}
				this.positionToggleGroup.transform.parent.gameObject.SetActive(canMirror && !this.AvatarPage.OnlyShowShaveItems());
			}
		}

		// Token: 0x06006392 RID: 25490 RVA: 0x002DA11C File Offset: 0x002D831C
		private void OnPositionChanged(int newIndex, int oldIndex)
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				bool flag2 = this.featureIndex == 1;
				if (flag2)
				{
					avatarData.Feature1MirrorType = (sbyte)newIndex;
				}
				else
				{
					avatarData.Feature2MirrorType = (sbyte)newIndex;
				}
				base.RefreshAvatarAndMarkDirty();
			}
		}

		// Token: 0x06006393 RID: 25491 RVA: 0x002DA164 File Offset: 0x002D8364
		private void OnColorChanged(int newIndex, int oldIndex)
		{
			bool flag = this._featureColors == null || newIndex < 0 || newIndex >= this._featureColors.Count;
			if (!flag)
			{
				AvatarData avatarData = base.GetAvatarData();
				bool flag2 = avatarData == null;
				if (!flag2)
				{
					byte colorId = this._featureColors[newIndex].Item1;
					bool flag3 = this.featureIndex == 1;
					if (flag3)
					{
						avatarData.ColorFeature1Id = colorId;
					}
					else
					{
						avatarData.ColorFeature2Id = colorId;
					}
					base.RefreshAvatarAndMarkDirty();
				}
			}
		}

		// Token: 0x06006394 RID: 25492 RVA: 0x002DA1E4 File Offset: 0x002D83E4
		private void OnFeatureChanged(int newIndex, int oldIndex)
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				short featureId = 1;
				bool flag2 = newIndex > 0 && this._featureList != null && newIndex - 1 < this._featureList.Count;
				if (flag2)
				{
					featureId = this._featureList[newIndex - 1].Id;
				}
				bool flag3 = this.featureIndex == 1;
				if (flag3)
				{
					avatarData.Feature1Id = featureId;
				}
				else
				{
					avatarData.Feature2Id = featureId;
				}
				base.RefreshAvatarAndMarkDirty();
				this.UpdateColorAreaVisibility();
				this.UpdatePositionAreaVisibility();
				this.featureContainer.gameObject.SetActive(!this.AvatarPage.OnlyShowShaveItems());
				this.RefreshAllItems();
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.mainContent);
			}
		}

		// Token: 0x04004567 RID: 17767
		[Header("Title")]
		[SerializeField]
		private TextMeshProUGUI pageTitleText;

		// Token: 0x04004568 RID: 17768
		[SerializeField]
		private int featureIndex = 1;

		// Token: 0x04004569 RID: 17769
		[Header("位置")]
		[SerializeField]
		private CToggleGroup positionToggleGroup;

		// Token: 0x0400456A RID: 17770
		[Header("颜色")]
		[SerializeField]
		private CToggleGroup colorToggleGroup;

		// Token: 0x0400456B RID: 17771
		[SerializeField]
		private CToggle colorToggleTemplate;

		// Token: 0x0400456C RID: 17772
		[SerializeField]
		private GameObject colorArea;

		// Token: 0x0400456D RID: 17773
		[Header("特征选择")]
		[SerializeField]
		private CToggleGroup featureToggleGroup;

		// Token: 0x0400456E RID: 17774
		[SerializeField]
		private Transform featureContainer;

		// Token: 0x0400456F RID: 17775
		[SerializeField]
		private NewGameSubPageAvatarStyleItem featureTemplate;

		// Token: 0x04004570 RID: 17776
		[SerializeField]
		private RectTransform mainContent;

		// Token: 0x04004571 RID: 17777
		private bool _isFeatureToggleInitialized;

		// Token: 0x04004572 RID: 17778
		private bool _isColorToggleInitialized;

		// Token: 0x04004573 RID: 17779
		private List<AvatarAsset> _featureList;

		// Token: 0x04004574 RID: 17780
		private readonly List<NewGameSubPageAvatarStyleItem> _featureItems = new List<NewGameSubPageAvatarStyleItem>();

		// Token: 0x04004575 RID: 17781
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		private List<ValueTuple<byte, Color>> _featureColors;
	}
}
