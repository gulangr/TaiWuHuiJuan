using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.NewGame
{
	// Token: 0x020007F5 RID: 2037
	public class NewGameSubPageAvatarFacePartPage : NewGameSubPageAvatarPageBase
	{
		// Token: 0x0600635A RID: 25434 RVA: 0x002D7D4C File Offset: 0x002D5F4C
		public void SetPartType(EAvatarElementsType type)
		{
			if (!true)
			{
			}
			FacePartType facePartType;
			switch (type)
			{
			case EAvatarElementsType.Eye:
				facePartType = FacePartType.Eyes;
				goto IL_37;
			case EAvatarElementsType.EyeBrow:
				facePartType = FacePartType.Eyebrow;
				goto IL_37;
			case EAvatarElementsType.Nose:
				facePartType = FacePartType.Nose;
				goto IL_37;
			case EAvatarElementsType.Mouth:
				facePartType = FacePartType.Mouth;
				goto IL_37;
			}
			facePartType = FacePartType.Eyebrow;
			IL_37:
			if (!true)
			{
			}
			FacePartType newType = facePartType;
			bool flag = this.partType != newType;
			if (flag)
			{
				this.partType = newType;
				bool isActiveAndEnabled = base.isActiveAndEnabled;
				if (isActiveAndEnabled)
				{
					this.LoadParts();
					this.UpdateUIElementsVisibility();
					this.LoadColors();
					this.UpdateTitle();
					AvatarData avatarData = base.GetAvatarData();
					bool flag2 = avatarData != null;
					if (flag2)
					{
						this.RefreshSliders(avatarData);
						this.RefreshColorSelection(avatarData);
					}
				}
			}
		}

		// Token: 0x0600635B RID: 25435 RVA: 0x002D7DFE File Offset: 0x002D5FFE
		private void Awake()
		{
			this.InitializeUI();
		}

		// Token: 0x0600635C RID: 25436 RVA: 0x002D7E08 File Offset: 0x002D6008
		private void InitializeUI()
		{
			bool flag = this.colorToggleTemplate != null;
			if (flag)
			{
				this.colorToggleTemplate.gameObject.SetActive(false);
			}
			bool flag2 = this.partTemplate != null;
			if (flag2)
			{
				this.partTemplate.gameObject.SetActive(false);
			}
			bool flag3 = this.distanceAdjust != null;
			if (flag3)
			{
				this.distanceAdjust.Init(null, 0, new UnityAction<short>(this.OnDistanceChanged), delegate()
				{
					this.RandomizeValue(this.distanceAdjust);
				}, delegate()
				{
					this.ResetValue(this.distanceAdjust);
				});
			}
			bool flag4 = this.heightAdjust != null;
			if (flag4)
			{
				this.heightAdjust.Init(null, 0, new UnityAction<short>(this.OnHeightChanged), delegate()
				{
					this.RandomizeValue(this.heightAdjust);
				}, delegate()
				{
					this.ResetValue(this.heightAdjust);
				});
			}
			bool flag5 = this.scaleAdjust != null;
			if (flag5)
			{
				this.scaleAdjust.Init(null, 100, new UnityAction<short>(this.OnScaleChanged), delegate()
				{
					this.RandomizeValue(this.scaleAdjust);
				}, delegate()
				{
					this.ResetValue(this.scaleAdjust);
				});
			}
			bool flag6 = this.angleAdjust != null;
			if (flag6)
			{
				this.angleAdjust.Init(null, 0, new UnityAction<short>(this.OnAngleChanged), delegate()
				{
					this.RandomizeValue(this.angleAdjust);
				}, delegate()
				{
					this.ResetValue(this.angleAdjust);
				});
			}
			bool flag7 = this.scrollRect != null;
			if (flag7)
			{
				this.scrollRect.OnScrollEvent += this.OnScroll;
			}
		}

		// Token: 0x0600635D RID: 25437 RVA: 0x002D7F94 File Offset: 0x002D6194
		protected override void OnEnable()
		{
			this.LoadParts();
			this.UpdateUIElementsVisibility();
			this.LoadColors();
			this.UpdateTitle();
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData != null;
			if (flag)
			{
				this.RefreshSliders(avatarData);
				this.RefreshColorSelection(avatarData);
			}
		}

		// Token: 0x0600635E RID: 25438 RVA: 0x002D7FDF File Offset: 0x002D61DF
		public override void UpdateUI()
		{
			this.RefreshFromAvatarData();
		}

		// Token: 0x0600635F RID: 25439 RVA: 0x002D7FEC File Offset: 0x002D61EC
		private void UpdateTitle()
		{
			bool flag = this.pageTitleText == null;
			if (!flag)
			{
				TextMeshProUGUI textMeshProUGUI = this.pageTitleText;
				FacePartType facePartType = this.partType;
				if (!true)
				{
				}
				string text;
				switch (facePartType)
				{
				case FacePartType.Eyebrow:
					text = LanguageKey.UI_NewGame_AdjustTitle_5.Tr();
					break;
				case FacePartType.Eyes:
					text = LanguageKey.UI_NewGame_AdjustTitle_6.Tr();
					break;
				case FacePartType.Nose:
					text = LanguageKey.UI_NewGame_AdjustTitle_7.Tr();
					break;
				case FacePartType.Mouth:
					text = LanguageKey.UI_NewGame_AdjustTitle_8.Tr();
					break;
				default:
					text = "";
					break;
				}
				if (!true)
				{
				}
				textMeshProUGUI.text = text;
			}
		}

		// Token: 0x06006360 RID: 25440 RVA: 0x002D8080 File Offset: 0x002D6280
		private void UpdateUIElementsVisibility()
		{
			bool showDistance = (this.partType == FacePartType.Eyes || this.partType == FacePartType.Eyebrow) && !this.AvatarPage.OnlyShowShaveItems();
			bool showHeight = !this.AvatarPage.OnlyShowShaveItems();
			bool showScale = !this.AvatarPage.OnlyShowShaveItems();
			bool showAngle = (this.partType == FacePartType.Eyes || this.partType == FacePartType.Eyebrow) && !this.AvatarPage.OnlyShowShaveItems();
			bool flag = this.distanceAdjust != null;
			if (flag)
			{
				this.distanceAdjust.SetActive(showDistance);
			}
			bool flag2 = this.heightAdjust != null;
			if (flag2)
			{
				this.heightAdjust.SetActive(showHeight);
			}
			bool flag3 = this.scaleAdjust != null;
			if (flag3)
			{
				this.scaleAdjust.SetActive(showScale);
			}
			bool flag4 = this.angleAdjust != null;
			if (flag4)
			{
				this.angleAdjust.SetActive(showAngle);
			}
			bool showColor = this.partType != FacePartType.Nose;
			bool flag5 = showColor && this.partType == FacePartType.Mouth;
			if (flag5)
			{
				showColor = (this.GetCurrentMouthColorGroup() > 0);
			}
			bool flag6 = this.colorArea != null;
			if (flag6)
			{
				this.colorArea.SetActive(showColor);
			}
			this.partContainer.gameObject.SetActive(!this.AvatarPage.OnlyShowShaveItems());
		}

		// Token: 0x06006361 RID: 25441 RVA: 0x002D81E0 File Offset: 0x002D63E0
		private void LoadColors()
		{
			bool flag = this.partType == FacePartType.Nose;
			if (flag)
			{
				this._currentColors = null;
			}
			else
			{
				bool flag2 = this.partType == FacePartType.Mouth && this.GetCurrentMouthColorGroup() == 0;
				if (flag2)
				{
					this._currentColors = null;
				}
				else
				{
					FacePartType facePartType = this.partType;
					if (!true)
					{
					}
					List<ValueTuple<byte, Color>> currentColors;
					switch (facePartType)
					{
					case FacePartType.Eyebrow:
						currentColors = NewGameSubPageAvatarColorHelper.HairColors;
						goto IL_81;
					case FacePartType.Eyes:
						currentColors = NewGameSubPageAvatarColorHelper.EyeBallColors;
						goto IL_81;
					case FacePartType.Mouth:
						currentColors = NewGameSubPageAvatarColorHelper.LipColors;
						goto IL_81;
					}
					currentColors = NewGameSubPageAvatarColorHelper.FeatureColors;
					IL_81:
					if (!true)
					{
					}
					this._currentColors = currentColors;
				}
			}
			this.CreateColorToggles();
		}

		// Token: 0x06006362 RID: 25442 RVA: 0x002D8284 File Offset: 0x002D6484
		private void CreateColorToggles()
		{
			bool flag = this.colorToggleGroup == null || this.colorToggleTemplate == null;
			if (!flag)
			{
				Transform container = this.colorToggleGroup.transform;
				List<ValueTuple<byte, Color>> currentColors = this._currentColors;
				int count = (currentColors != null) ? currentColors.Count : 0;
				CommonUtils.PrepareEnoughChildren(container, this.colorToggleTemplate.gameObject, count, null);
				this.colorToggleGroup.Clear();
				bool flag2 = this._currentColors != null;
				if (flag2)
				{
					for (int i = 0; i < count; i++)
					{
						Color color = this._currentColors[i].Item2;
						Transform toggleTrans = container.GetChild(i);
						CImage colorImage = toggleTrans.GetChild(0).GetChild(0).GetComponent<CImage>();
						bool flag3 = colorImage != null;
						if (flag3)
						{
							colorImage.color = color;
						}
						this.colorToggleGroup.Add(toggleTrans.GetComponent<CToggle>());
					}
				}
				this.colorToggleGroup.Init(-1);
				bool flag4 = !this._isColorToggleInitialized;
				if (flag4)
				{
					this.colorToggleGroup.OnActiveIndexChange += this.OnColorChanged;
					this._isColorToggleInitialized = true;
				}
			}
		}

		// Token: 0x06006363 RID: 25443 RVA: 0x002D83C4 File Offset: 0x002D65C4
		private void LoadParts()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				AvatarGroup group = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)avatarData.AvatarId);
				bool flag2 = group == null;
				if (!flag2)
				{
					this._eyebrowList = null;
					this._eyesList = null;
					this._noseList = null;
					this._mouthList = null;
					int count = 0;
					switch (this.partType)
					{
					case FacePartType.Eyebrow:
					{
						this._eyebrowList = group.EyeBrowRes;
						IAvatarSubPageParent avatarPage = this.AvatarPage;
						Func<List<AvatarAsset>, List<AvatarAsset>> eyebrowFilter = (avatarPage != null) ? avatarPage.GetEyebrowFilter() : null;
						bool flag3 = eyebrowFilter != null && this._eyebrowList != null;
						if (flag3)
						{
							this._eyebrowList = eyebrowFilter(this._eyebrowList);
						}
						List<AvatarAsset> eyebrowList = this._eyebrowList;
						count = ((eyebrowList != null) ? eyebrowList.Count : 0);
						break;
					}
					case FacePartType.Eyes:
					{
						this._eyesList = group.EyesGroup;
						IAvatarSubPageParent avatarPage2 = this.AvatarPage;
						Func<List<EyeRes>, List<EyeRes>> eyesFilter = (avatarPage2 != null) ? avatarPage2.GetEyesFilter() : null;
						bool flag4 = eyesFilter != null && this._eyesList != null;
						if (flag4)
						{
							this._eyesList = eyesFilter(this._eyesList);
						}
						List<EyeRes> eyesList = this._eyesList;
						count = ((eyesList != null) ? eyesList.Count : 0);
						break;
					}
					case FacePartType.Nose:
					{
						this._noseList = group.NoseRes;
						IAvatarSubPageParent avatarPage3 = this.AvatarPage;
						Func<List<AvatarAsset>, List<AvatarAsset>> noseFilter = (avatarPage3 != null) ? avatarPage3.GetNoseFilter() : null;
						bool flag5 = noseFilter != null && this._noseList != null;
						if (flag5)
						{
							this._noseList = noseFilter(this._noseList);
						}
						List<AvatarAsset> noseList = this._noseList;
						count = ((noseList != null) ? noseList.Count : 0);
						break;
					}
					case FacePartType.Mouth:
					{
						this._mouthList = group.MouthRes;
						IAvatarSubPageParent avatarPage4 = this.AvatarPage;
						Func<List<MouthRes>, List<MouthRes>> mouthFilter = (avatarPage4 != null) ? avatarPage4.GetMouthFilter() : null;
						bool flag6 = mouthFilter != null && this._mouthList != null;
						if (flag6)
						{
							this._mouthList = mouthFilter(this._mouthList);
						}
						List<MouthRes> mouthList = this._mouthList;
						count = ((mouthList != null) ? mouthList.Count : 0);
						break;
					}
					}
					this.CreatePartItems(count);
				}
			}
		}

		// Token: 0x06006364 RID: 25444 RVA: 0x002D85D4 File Offset: 0x002D67D4
		private void CreatePartItems(int resCount)
		{
			bool flag = this.partTemplate == null || this.partContainer == null || this.partToggleGroup == null;
			if (!flag)
			{
				CommonUtils.PrepareEnoughChildren(this.partContainer, this.partTemplate.gameObject, resCount, null);
				this.partToggleGroup.Clear();
				this._partItems.Clear();
				for (int i = 0; i < resCount; i++)
				{
					NewGameSubPageAvatarFacePart item = this.partContainer.GetChild(i).GetComponent<NewGameSubPageAvatarFacePart>();
					bool flag2 = item != null;
					if (flag2)
					{
						this._partItems.Add(item);
						this.partToggleGroup.Add(item.Toggle);
					}
				}
				this.partToggleGroup.Init(-1);
				bool flag3 = !this._isPartToggleInitialized;
				if (flag3)
				{
					this.partToggleGroup.OnActiveIndexChange += this.OnPartChanged;
					this._isPartToggleInitialized = true;
				}
				this.RefreshAllItems();
			}
		}

		// Token: 0x06006365 RID: 25445 RVA: 0x002D86F0 File Offset: 0x002D68F0
		private void RefreshFromAvatarData()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				this.LoadParts();
				this.UpdateUIElementsVisibility();
				this.LoadColors();
				this.RefreshColorSelection(avatarData);
				this.RefreshSliders(avatarData);
			}
		}

		// Token: 0x06006366 RID: 25446 RVA: 0x002D8734 File Offset: 0x002D6934
		private void RefreshSliders(AvatarData avatarData)
		{
			bool isSliderAdjusting = this._isSliderAdjusting;
			if (!isSliderAdjusting)
			{
				AvatarElementPositionItem posCfg = avatarData.PositionConfig;
				bool flag = this.distanceAdjust != null && this.distanceAdjust.gameObject.activeSelf;
				if (flag)
				{
					FacePartType facePartType = this.partType;
					FacePartType facePartType2 = facePartType;
					if (facePartType2 != FacePartType.Eyebrow)
					{
						if (facePartType2 == FacePartType.Eyes)
						{
							this.distanceAdjust.SetShortValueWithoutNotify(posCfg.EyeDistanceRange, 0, avatarData.EyesDistance);
						}
					}
					else
					{
						this.distanceAdjust.SetShortValueWithoutNotify(posCfg.EyebrowDistanceRange, 0, avatarData.EyebrowDistance);
					}
				}
				bool flag2 = this.heightAdjust != null && this.heightAdjust.gameObject.activeSelf;
				if (flag2)
				{
					switch (this.partType)
					{
					case FacePartType.Eyebrow:
						this.heightAdjust.SetShortValueWithoutNotify(posCfg.EyebrowHeightRange, 0, avatarData.EyebrowHeight);
						break;
					case FacePartType.Eyes:
						this.heightAdjust.SetShortValueWithoutNotify(posCfg.EyeHeightRange, 0, avatarData.EyesHeight);
						break;
					case FacePartType.Nose:
						this.heightAdjust.SetShortValueWithoutNotify(posCfg.NoseHeightRange, 0, avatarData.NoseHeight);
						break;
					case FacePartType.Mouth:
						this.heightAdjust.SetShortValueWithoutNotify(posCfg.MouthHeightRange, 0, avatarData.MouthHeight);
						break;
					}
				}
				bool flag3 = this.scaleAdjust != null && this.scaleAdjust.gameObject.activeSelf;
				if (flag3)
				{
					switch (this.partType)
					{
					case FacePartType.Eyebrow:
						this.scaleAdjust.SetShortValueWithoutNotify(posCfg.EyebrowScaleRange, 100, avatarData.EyebrowScale);
						break;
					case FacePartType.Eyes:
						this.scaleAdjust.SetShortValueWithoutNotify(posCfg.EyeScaleRange, 100, avatarData.EyesScale);
						break;
					case FacePartType.Nose:
						this.scaleAdjust.SetShortValueWithoutNotify(posCfg.NoseScaleRange, 100, avatarData.NoseScale);
						break;
					case FacePartType.Mouth:
						this.scaleAdjust.SetShortValueWithoutNotify(posCfg.MouthScaleRange, 100, avatarData.MouthScale);
						break;
					}
				}
				bool flag4 = this.angleAdjust != null && this.angleAdjust.gameObject.activeSelf;
				if (flag4)
				{
					FacePartType facePartType3 = this.partType;
					FacePartType facePartType4 = facePartType3;
					if (facePartType4 != FacePartType.Eyebrow)
					{
						if (facePartType4 == FacePartType.Eyes)
						{
							this.angleAdjust.SetShortValueWithoutNotify(posCfg.EyeAngleRange, 0, -avatarData.EyesAngle);
						}
					}
					else
					{
						this.angleAdjust.SetShortValueWithoutNotify(posCfg.EyebrowAngleRange, 0, -avatarData.EyebrowAngle);
					}
				}
			}
		}

		// Token: 0x06006367 RID: 25447 RVA: 0x002D89C8 File Offset: 0x002D6BC8
		private void RefreshColorSelection(AvatarData avatarData)
		{
			bool flag = this._currentColors != null && this.colorToggleGroup != null;
			if (flag)
			{
				byte colorId = 0;
				switch (this.partType)
				{
				case FacePartType.Eyebrow:
					colorId = avatarData.ColorEyebrowId;
					break;
				case FacePartType.Eyes:
					colorId = avatarData.ColorEyeballId;
					break;
				case FacePartType.Mouth:
					colorId = avatarData.ColorMouthId;
					break;
				}
				int index = this._currentColors.FindIndex(([TupleElementNames(new string[]
				{
					"id",
					"color"
				})] ValueTuple<byte, Color> x) => x.Item1 == colorId);
				bool flag2 = index >= 0;
				if (flag2)
				{
					this.colorToggleGroup.SetWithoutNotify(index);
				}
			}
		}

		// Token: 0x06006368 RID: 25448 RVA: 0x002D8A84 File Offset: 0x002D6C84
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
					IAvatarSubPageParent avatarPage = this.AvatarPage;
					short age = (avatarPage != null) ? avatarPage.GetAge() : 20;
					int selectedIndex = -1;
					ValueTuple<int, int> visibleRange = this.GetVisibleRange();
					int visibleStart = visibleRange.Item1;
					int visibleEnd = visibleRange.Item2;
					for (int i = 0; i < this._partItems.Count; i++)
					{
						bool flag2 = this.partType == FacePartType.Eyes && this._eyesList != null && i < this._eyesList.Count;
						bool isSelected;
						if (flag2)
						{
							EyeRes eyeRes = this._eyesList[i];
							isSelected = (eyeRes.Id == avatarData.EyesMainId && eyeRes.LeftEye.SubId == avatarData.EyesLeftId && eyeRes.RightEye.SubId == avatarData.EyesRightId);
						}
						else
						{
							short itemId = this.GetPartId(i);
							short currentId = this.GetCurrentPartId(avatarData);
							isSelected = (itemId == currentId);
						}
						bool flag3 = isSelected;
						if (flag3)
						{
							selectedIndex = i;
						}
						AvatarData previewData = this.CreatePreviewData(avatarData, i);
						bool isVisible = i >= visibleStart && i <= visibleEnd;
						this._partItems[i].Refresh(i, previewData, age, this.partType, this.previewSetting, isSelected, false, isVisible);
					}
					bool flag4 = selectedIndex >= 0 && this.partToggleGroup != null;
					if (flag4)
					{
						this.partToggleGroup.Set(selectedIndex, false);
					}
					this._isRefreshing = false;
				}
			}
		}

		// Token: 0x06006369 RID: 25449 RVA: 0x002D8C34 File Offset: 0x002D6E34
		[return: TupleElementNames(new string[]
		{
			"start",
			"end"
		})]
		private ValueTuple<int, int> GetVisibleRange()
		{
			bool flag = this.scrollRect == null || this.partContainer == null || this._partItems.Count == 0;
			ValueTuple<int, int> result;
			if (flag)
			{
				result = new ValueTuple<int, int>(0, this._partItems.Count - 1);
			}
			else
			{
				RectTransform viewport = this.scrollRect.Viewport;
				bool flag2 = viewport == null;
				if (flag2)
				{
					result = new ValueTuple<int, int>(0, this._partItems.Count - 1);
				}
				else
				{
					int visibleStart = 0;
					int visibleEnd = this._partItems.Count - 1;
					for (int i = 0; i < this._partItems.Count; i++)
					{
						RectTransform itemRect = this._partItems[i].transform as RectTransform;
						bool flag3 = itemRect != null && this.IsVisible(itemRect, viewport);
						if (flag3)
						{
							visibleStart = Mathf.Max(0, i - 5);
							break;
						}
					}
					for (int j = this._partItems.Count - 1; j >= 0; j--)
					{
						RectTransform itemRect2 = this._partItems[j].transform as RectTransform;
						bool flag4 = itemRect2 != null && this.IsVisible(itemRect2, viewport);
						if (flag4)
						{
							visibleEnd = Mathf.Min(this._partItems.Count - 1, j + 5);
							break;
						}
					}
					result = new ValueTuple<int, int>(visibleStart, visibleEnd);
				}
			}
			return result;
		}

		// Token: 0x0600636A RID: 25450 RVA: 0x002D8DB8 File Offset: 0x002D6FB8
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

		// Token: 0x0600636B RID: 25451 RVA: 0x002D8E48 File Offset: 0x002D7048
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
					while (i <= visibleEnd && i < this._partItems.Count)
					{
						bool needsAvatarRefresh = this._partItems[i].NeedsAvatarRefresh;
						if (needsAvatarRefresh)
						{
							this._partItems[i].RefreshAvatarIfNeeded();
						}
						i++;
					}
				}
			}
		}

		// Token: 0x0600636C RID: 25452 RVA: 0x002D8F20 File Offset: 0x002D7120
		private AvatarData CreatePreviewData(AvatarData baseData, int index)
		{
			AvatarData previewData = new AvatarData(baseData);
			bool flag = this.AvatarPage.IsCopyCloth();
			if (flag)
			{
				previewData.ClothDisplayId = baseData.ClothDisplayId;
			}
			switch (this.partType)
			{
			case FacePartType.Eyebrow:
			{
				bool flag2 = this._eyebrowList != null && index < this._eyebrowList.Count;
				if (flag2)
				{
					previewData.EyebrowId = this._eyebrowList[index].Id;
				}
				previewData.EyebrowDistance = 0;
				previewData.EyebrowHeight = 0;
				previewData.EyebrowScale = 100;
				previewData.EyebrowAngle = 0;
				break;
			}
			case FacePartType.Eyes:
			{
				bool flag3 = this._eyesList != null && index < this._eyesList.Count;
				if (flag3)
				{
					EyeRes eyeRes = this._eyesList[index];
					previewData.EyesMainId = eyeRes.Id;
					previewData.EyesLeftId = (short)((byte)eyeRes.LeftEye.SubId);
					previewData.EyesRightId = (short)((byte)eyeRes.RightEye.SubId);
				}
				previewData.EyesDistance = 0;
				previewData.EyesHeight = 0;
				previewData.EyesScale = 100;
				previewData.EyesAngle = 0;
				break;
			}
			case FacePartType.Nose:
			{
				bool flag4 = this._noseList != null && index < this._noseList.Count;
				if (flag4)
				{
					previewData.NoseId = this._noseList[index].Id;
				}
				previewData.NoseHeight = 0;
				previewData.NoseScale = 100;
				break;
			}
			case FacePartType.Mouth:
			{
				bool flag5 = this._mouthList != null && index < this._mouthList.Count;
				if (flag5)
				{
					previewData.MouthId = this._mouthList[index].Id;
				}
				previewData.MouthHeight = 0;
				previewData.MouthScale = 100;
				break;
			}
			}
			return previewData;
		}

		// Token: 0x0600636D RID: 25453 RVA: 0x002D90E8 File Offset: 0x002D72E8
		private Sprite GetPartSprite(short partId)
		{
			bool flag = partId == 0;
			Sprite result;
			if (flag)
			{
				result = null;
			}
			else
			{
				AvatarData avatarData = base.GetAvatarData();
				bool flag2 = avatarData == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					AvatarManager manager = SingletonObject.getInstance<AvatarManager>();
					EAvatarElementsType elemType = this.GetElemTypeFromPartType();
					result = manager.GetSprite((int)avatarData.AvatarId, elemType, 1, new short[]
					{
						partId
					});
				}
			}
			return result;
		}

		// Token: 0x0600636E RID: 25454 RVA: 0x002D9144 File Offset: 0x002D7344
		private EAvatarElementsType GetElemTypeFromPartType()
		{
			FacePartType facePartType = this.partType;
			if (!true)
			{
			}
			EAvatarElementsType result;
			switch (facePartType)
			{
			case FacePartType.Eyebrow:
				result = EAvatarElementsType.EyeBrow;
				break;
			case FacePartType.Eyes:
				result = EAvatarElementsType.Eye;
				break;
			case FacePartType.Nose:
				result = EAvatarElementsType.Nose;
				break;
			case FacePartType.Mouth:
				result = EAvatarElementsType.Mouth;
				break;
			default:
				result = EAvatarElementsType.EyeBrow;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600636F RID: 25455 RVA: 0x002D9194 File Offset: 0x002D7394
		private short GetPartId(int index)
		{
			bool flag = index < 0;
			short result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				switch (this.partType)
				{
				case FacePartType.Eyebrow:
				{
					List<AvatarAsset> eyebrowList = this._eyebrowList;
					result = ((index < ((eyebrowList != null) ? eyebrowList.Count : 0)) ? this._eyebrowList[index].Id : 0);
					break;
				}
				case FacePartType.Eyes:
				{
					List<EyeRes> eyesList = this._eyesList;
					result = ((index < ((eyesList != null) ? eyesList.Count : 0)) ? this._eyesList[index].Id : 0);
					break;
				}
				case FacePartType.Nose:
				{
					List<AvatarAsset> noseList = this._noseList;
					result = ((index < ((noseList != null) ? noseList.Count : 0)) ? this._noseList[index].Id : 0);
					break;
				}
				case FacePartType.Mouth:
				{
					List<MouthRes> mouthList = this._mouthList;
					result = ((index < ((mouthList != null) ? mouthList.Count : 0)) ? this._mouthList[index].Id : 0);
					break;
				}
				default:
					result = 0;
					break;
				}
			}
			return result;
		}

		// Token: 0x06006370 RID: 25456 RVA: 0x002D9294 File Offset: 0x002D7494
		private short GetCurrentPartId(AvatarData avatarData)
		{
			FacePartType facePartType = this.partType;
			if (!true)
			{
			}
			short result;
			switch (facePartType)
			{
			case FacePartType.Eyebrow:
				result = avatarData.EyebrowId;
				break;
			case FacePartType.Eyes:
				result = avatarData.EyesMainId;
				break;
			case FacePartType.Nose:
				result = avatarData.NoseId;
				break;
			case FacePartType.Mouth:
				result = avatarData.MouthId;
				break;
			default:
				result = 0;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06006371 RID: 25457 RVA: 0x002D92F8 File Offset: 0x002D74F8
		private byte GetCurrentMouthColorGroup()
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null || this._mouthList == null || this._mouthList.Count == 0;
			byte result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int index = this._mouthList.FindIndex((MouthRes m) => m.Id == avatarData.MouthId);
				bool flag2 = index < 0;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					AvatarAsset mouth = this._mouthList[index].Mouth;
					byte? b;
					if (mouth == null)
					{
						b = null;
					}
					else
					{
						AvatarElementsItem config = mouth.Config;
						b = ((config != null) ? new byte?(config.ColorGroup) : null);
					}
					byte? b2 = b;
					result = b2.GetValueOrDefault();
				}
			}
			return result;
		}

		// Token: 0x06006372 RID: 25458 RVA: 0x002D93B4 File Offset: 0x002D75B4
		private void OnColorChanged(int newIndex, int oldIndex)
		{
			bool flag = this._currentColors == null || newIndex < 0 || newIndex >= this._currentColors.Count;
			if (!flag)
			{
				AvatarData avatarData = base.GetAvatarData();
				bool flag2 = avatarData == null;
				if (!flag2)
				{
					byte colorId = this._currentColors[newIndex].Item1;
					switch (this.partType)
					{
					case FacePartType.Eyebrow:
						avatarData.ColorFrontHairId = colorId;
						avatarData.ColorBackHairId = colorId;
						avatarData.ColorEyebrowId = colorId;
						avatarData.ColorBeard1Id = colorId;
						avatarData.ColorBeard2Id = colorId;
						break;
					case FacePartType.Eyes:
						avatarData.ColorEyeballId = colorId;
						break;
					case FacePartType.Mouth:
						avatarData.ColorMouthId = colorId;
						break;
					}
					base.RefreshAvatarAndMarkDirty();
				}
			}
		}

		// Token: 0x06006373 RID: 25459 RVA: 0x002D9474 File Offset: 0x002D7674
		private void OnPartChanged(int newIndex, int oldIndex)
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				switch (this.partType)
				{
				case FacePartType.Eyebrow:
					avatarData.EyebrowId = this.GetPartId(newIndex);
					break;
				case FacePartType.Eyes:
				{
					bool flag2 = this._eyesList != null && newIndex >= 0 && newIndex < this._eyesList.Count;
					if (flag2)
					{
						EyeRes eyeRes = this._eyesList[newIndex];
						avatarData.EyesMainId = eyeRes.Id;
						avatarData.EyesLeftId = (short)((byte)eyeRes.LeftEye.SubId);
						avatarData.EyesRightId = (short)((byte)eyeRes.RightEye.SubId);
					}
					break;
				}
				case FacePartType.Nose:
					avatarData.NoseId = this.GetPartId(newIndex);
					break;
				case FacePartType.Mouth:
					avatarData.MouthId = this.GetPartId(newIndex);
					break;
				}
				base.RefreshAvatarAndMarkDirty();
				bool flag3 = this.partType == FacePartType.Mouth;
				if (flag3)
				{
					this.UpdateUIElementsVisibility();
					this.LoadColors();
					this.RefreshColorSelection(avatarData);
				}
				this.RefreshAllItems();
			}
		}

		// Token: 0x06006374 RID: 25460 RVA: 0x002D9588 File Offset: 0x002D7788
		private void OnDistanceChanged(short shortVal)
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				this._isSliderAdjusting = true;
				bool flag2 = this.partType == FacePartType.Eyebrow;
				if (flag2)
				{
					avatarData.EyebrowDistance = shortVal;
				}
				else
				{
					bool flag3 = this.partType == FacePartType.Eyes;
					if (flag3)
					{
						avatarData.EyesDistance = shortVal;
					}
				}
				base.RefreshAvatarAndMarkDirty();
				this._isSliderAdjusting = false;
			}
		}

		// Token: 0x06006375 RID: 25461 RVA: 0x002D95E8 File Offset: 0x002D77E8
		private void OnHeightChanged(short shortVal)
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				this._isSliderAdjusting = true;
				switch (this.partType)
				{
				case FacePartType.Eyebrow:
					avatarData.EyebrowHeight = shortVal;
					break;
				case FacePartType.Eyes:
					avatarData.EyesHeight = shortVal;
					break;
				case FacePartType.Nose:
					avatarData.NoseHeight = shortVal;
					break;
				case FacePartType.Mouth:
					avatarData.MouthHeight = shortVal;
					break;
				}
				base.RefreshAvatarAndMarkDirty();
				this._isSliderAdjusting = false;
			}
		}

		// Token: 0x06006376 RID: 25462 RVA: 0x002D9664 File Offset: 0x002D7864
		private void OnScaleChanged(short shortVal)
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				this._isSliderAdjusting = true;
				switch (this.partType)
				{
				case FacePartType.Eyebrow:
					avatarData.EyebrowScale = shortVal;
					break;
				case FacePartType.Eyes:
					avatarData.EyesScale = shortVal;
					break;
				case FacePartType.Nose:
					avatarData.NoseScale = shortVal;
					break;
				case FacePartType.Mouth:
					avatarData.MouthScale = shortVal;
					break;
				}
				base.RefreshAvatarAndMarkDirty();
				this._isSliderAdjusting = false;
			}
		}

		// Token: 0x06006377 RID: 25463 RVA: 0x002D96E0 File Offset: 0x002D78E0
		private void OnAngleChanged(short shortVal)
		{
			AvatarData avatarData = base.GetAvatarData();
			bool flag = avatarData == null;
			if (!flag)
			{
				this._isSliderAdjusting = true;
				bool flag2 = this.partType == FacePartType.Eyebrow;
				if (flag2)
				{
					avatarData.EyebrowAngle = -shortVal;
				}
				else
				{
					bool flag3 = this.partType == FacePartType.Eyes;
					if (flag3)
					{
						avatarData.EyesAngle = -shortVal;
					}
				}
				base.RefreshAvatarAndMarkDirty();
				this._isSliderAdjusting = false;
			}
		}

		// Token: 0x06006378 RID: 25464 RVA: 0x002D9744 File Offset: 0x002D7944
		private void RandomizeValue(NewGameSubPageAvatarAdjustLine item)
		{
			float newVal = Random.Range(0f, 1f);
			item.Value = newVal;
		}

		// Token: 0x06006379 RID: 25465 RVA: 0x002D976A File Offset: 0x002D796A
		private void ResetValue(NewGameSubPageAvatarAdjustLine item)
		{
			item.Value = 0.5f;
		}

		// Token: 0x0400454E RID: 17742
		[Header("Title")]
		[SerializeField]
		private TextMeshProUGUI pageTitleText;

		// Token: 0x0400454F RID: 17743
		[SerializeField]
		private FacePartType partType;

		// Token: 0x04004550 RID: 17744
		[Header("颜色")]
		[SerializeField]
		private CToggleGroup colorToggleGroup;

		// Token: 0x04004551 RID: 17745
		[SerializeField]
		private GameObject colorArea;

		// Token: 0x04004552 RID: 17746
		[SerializeField]
		private CToggle colorToggleTemplate;

		// Token: 0x04004553 RID: 17747
		[Header("调整项")]
		[SerializeField]
		private NewGameSubPageAvatarAdjustLine distanceAdjust;

		// Token: 0x04004554 RID: 17748
		[SerializeField]
		private NewGameSubPageAvatarAdjustLine heightAdjust;

		// Token: 0x04004555 RID: 17749
		[SerializeField]
		private NewGameSubPageAvatarAdjustLine scaleAdjust;

		// Token: 0x04004556 RID: 17750
		[SerializeField]
		private NewGameSubPageAvatarAdjustLine angleAdjust;

		// Token: 0x04004557 RID: 17751
		[Header("部件选择")]
		[SerializeField]
		private CToggleGroup partToggleGroup;

		// Token: 0x04004558 RID: 17752
		[SerializeField]
		private Transform partContainer;

		// Token: 0x04004559 RID: 17753
		[SerializeField]
		private NewGameSubPageAvatarFacePart partTemplate;

		// Token: 0x0400455A RID: 17754
		[SerializeField]
		private CScrollRect scrollRect;

		// Token: 0x0400455B RID: 17755
		[Header("Avatar预览配置（所有部件共用）")]
		[SerializeField]
		private FacePartAvatarPreviewSetting previewSetting;

		// Token: 0x0400455C RID: 17756
		[TupleElementNames(new string[]
		{
			"id",
			"color"
		})]
		private List<ValueTuple<byte, Color>> _currentColors;

		// Token: 0x0400455D RID: 17757
		private List<AvatarAsset> _eyebrowList;

		// Token: 0x0400455E RID: 17758
		private List<EyeRes> _eyesList;

		// Token: 0x0400455F RID: 17759
		private List<AvatarAsset> _noseList;

		// Token: 0x04004560 RID: 17760
		private List<MouthRes> _mouthList;

		// Token: 0x04004561 RID: 17761
		private readonly List<NewGameSubPageAvatarFacePart> _partItems = new List<NewGameSubPageAvatarFacePart>();

		// Token: 0x04004562 RID: 17762
		private bool _isColorToggleInitialized = false;

		// Token: 0x04004563 RID: 17763
		private bool _isPartToggleInitialized = false;

		// Token: 0x04004564 RID: 17764
		private Vector2 _lastContentPosition = Vector2.zero;

		// Token: 0x04004565 RID: 17765
		private bool _isRefreshing = false;

		// Token: 0x04004566 RID: 17766
		private bool _isSliderAdjusting;
	}
}
