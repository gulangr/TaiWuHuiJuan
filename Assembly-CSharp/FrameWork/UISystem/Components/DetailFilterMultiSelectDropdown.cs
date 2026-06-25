using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonSortAndFilterLegacy;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001015 RID: 4117
	public class DetailFilterMultiSelectDropdown : BaseMultiSelectDropdown<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig>
	{
		// Token: 0x17001539 RID: 5433
		// (get) Token: 0x0600BC11 RID: 48145 RVA: 0x005586FC File Offset: 0x005568FC
		// (set) Token: 0x0600BC12 RID: 48146 RVA: 0x00558714 File Offset: 0x00556914
		public bool DisableAutoHeight
		{
			get
			{
				return this._disableAutoHeight;
			}
			set
			{
				this._disableAutoHeight = value;
				this.RefreshAutoHeightHelperEnabled();
			}
		}

		// Token: 0x0600BC13 RID: 48147 RVA: 0x00558725 File Offset: 0x00556925
		public int GetOptionOrder(int index)
		{
			return this._optionItems.IndexOf(this._optionItemDict[index]);
		}

		// Token: 0x1700153A RID: 5434
		// (get) Token: 0x0600BC14 RID: 48148 RVA: 0x0055873E File Offset: 0x0055693E
		protected override bool IsMenuShow
		{
			get
			{
				return this.showHideMenuTarget.gameObject.activeSelf;
			}
		}

		// Token: 0x1700153B RID: 5435
		// (get) Token: 0x0600BC15 RID: 48149 RVA: 0x00558750 File Offset: 0x00556950
		private int? FirstSelectedIndex
		{
			get
			{
				return new int?(this.SelectedIndices.First<int>());
			}
		}

		// Token: 0x0600BC16 RID: 48150 RVA: 0x00558764 File Offset: 0x00556964
		protected override void Awake()
		{
			this.itemTemplate.SetActive(false);
			this.menuBarTemplate.SetActive(false);
			bool flag = this.menuBarTemplateDependency != null;
			if (flag)
			{
				this.menuBarTemplateDependency.SetActive(false);
			}
			this.scrollRect.GetComponent<CScrollRectLegacy>().ScrollSpeed = 10f;
			this.RefreshAutoHeightHelperEnabled();
			base.Awake();
		}

		// Token: 0x0600BC17 RID: 48151 RVA: 0x005587CC File Offset: 0x005569CC
		private void RefreshAutoHeightHelperEnabled()
		{
			if (this._autoHeightHelper == null)
			{
				this._autoHeightHelper = base.GetComponent<ScrollRectAutoHeightHelper>();
			}
			this._autoHeightHelper.enabled = (!this.DisableAutoHeight && this.dropdownDirectionMode == DetailFilterMultiSelectDropdown.EDropdownDirectionMode.AlwaysDown);
		}

		// Token: 0x0600BC18 RID: 48152 RVA: 0x00558804 File Offset: 0x00556A04
		protected override GameObject InstantiateMenuBar()
		{
			bool flag = base.Config.Dependency != null && this.menuBarTemplateDependency != null;
			GameObject result;
			if (flag)
			{
				result = Object.Instantiate<GameObject>(this.menuBarTemplateDependency, this.menuBarSlot);
			}
			else
			{
				result = Object.Instantiate<GameObject>(this.menuBarTemplate, this.menuBarSlot);
			}
			return result;
		}

		// Token: 0x0600BC19 RID: 48153 RVA: 0x00558864 File Offset: 0x00556A64
		protected override void Update()
		{
			bool disableBySettingMode = this._disableBySettingMode;
			if (!disableBySettingMode)
			{
				base.Update();
				DetailedDropdownOptionDragHandler detailedDropdownOptionDragHandler = this._detailedDropdownOptionDragHandler;
				if (detailedDropdownOptionDragHandler != null)
				{
					detailedDropdownOptionDragHandler.TickHoldDrag();
				}
			}
		}

		// Token: 0x0600BC1A RID: 48154 RVA: 0x00558898 File Offset: 0x00556A98
		public void SetupMenuBarHoldDrag(GameObject forwardTarget, RectTransform baseRect, CommonFilterHoldDragButton.HoldDragEvent onMenuBarPointerDownEvent, CommonFilterHoldDragButton.HoldDragEvent onMenuBarPointerUpEvent, CommonFilterHoldDragButton.HoldDragEvent onMenuBarBeginDragEvent, CommonFilterHoldDragButton.HoldDragEvent onMenuBarEndDragEvent, CommonFilterHoldDragButton.HoldDragEvent onMenuBarDragEvent)
		{
			this._baseRect = baseRect;
			CommonFilterHoldDragButton holdDragButton = this.MenuBarInstance.GetComponent<CommonFilterHoldDragButton>();
			holdDragButton.OnPointerDownEvent = delegate(GameObject go, PointerEventData eventData)
			{
				this.ScrollToVisiblePosition();
				onMenuBarPointerDownEvent(go, eventData);
			};
			holdDragButton.OnPointerUpEvent = delegate(GameObject go, PointerEventData eventData)
			{
				onMenuBarPointerUpEvent(go, eventData);
				PointerTrigger pointerTrigger = this.MenuBarInstance.GetComponent<PointerTrigger>();
				bool flag = CommonFakeHidePanel.IsMouseInRect(this.menuBarSlot);
				if (flag)
				{
					bool flag2 = !this._pointerTriggered;
					if (flag2)
					{
						this._pointerTriggered = true;
						pointerTrigger.OnPointerEnter(eventData);
					}
				}
				else
				{
					bool pointerTriggered = this._pointerTriggered;
					if (pointerTriggered)
					{
						this._pointerTriggered = false;
						pointerTrigger.OnPointerExit(eventData);
					}
				}
			};
			holdDragButton.OnBeginDragEvent = onMenuBarBeginDragEvent;
			holdDragButton.OnEndDragEvent = onMenuBarEndDragEvent;
			holdDragButton.OnDragEvent = onMenuBarDragEvent;
			holdDragButton.ForwardTarget = forwardTarget;
		}

		// Token: 0x0600BC1B RID: 48155 RVA: 0x00558918 File Offset: 0x00556B18
		public void SetupOptionsHoldDragHandler(Action onCustomOrderChange)
		{
			bool flag = !base.Config.EnableDragHold;
			if (flag)
			{
				this._onCustomOrderChange = null;
				this._detailedDropdownOptionDragHandler = null;
				foreach (DetailFilterMultiSelectDropdown.OptionItem optionItem in this._optionItems)
				{
					Refers refers = optionItem.Refers;
					CommonFilterHoldDragButton holdDragButton = refers.CGet<CommonFilterHoldDragButton>("HoldDrag");
					holdDragButton.OnPointerDownEvent = null;
					holdDragButton.OnPointerUpEvent = null;
					holdDragButton.OnBeginDragEvent = null;
					holdDragButton.OnEndDragEvent = null;
					holdDragButton.OnDragEvent = null;
				}
			}
			else
			{
				this._onCustomOrderChange = onCustomOrderChange;
				this._detailedDropdownOptionDragHandler = new DetailedDropdownOptionDragHandler(this.itemRoot.GetComponent<VerticalLayoutGroup>(), this.viewport, this._optionItems, 1, delegate()
				{
					Action onCustomOrderChange2 = this._onCustomOrderChange;
					if (onCustomOrderChange2 != null)
					{
						onCustomOrderChange2();
					}
				}, new Action<int>(this.OnEnterHold));
				foreach (DetailFilterMultiSelectDropdown.OptionItem optionItem2 in this._optionItems)
				{
					Refers refers2 = optionItem2.Refers;
					CommonFilterHoldDragButton holdDragButton2 = refers2.CGet<CommonFilterHoldDragButton>("HoldDrag");
					DetailFilterMultiSelectDropdown.OptionItem item = optionItem2;
					holdDragButton2.OnPointerDownEvent = delegate(GameObject _, PointerEventData eventData)
					{
						bool flag2 = eventData != null && eventData.button > PointerEventData.InputButton.Left;
						if (!flag2)
						{
							bool flag3 = !this._disableBySettingMode;
							if (flag3)
							{
								this._detailedDropdownOptionDragHandler.OnOptionPointerDown(item);
							}
						}
					};
					holdDragButton2.OnPointerUpEvent = delegate(GameObject _, PointerEventData eventData)
					{
						this._detailedDropdownOptionDragHandler.OnOptionPointerUp(eventData);
					};
					holdDragButton2.OnBeginDragEvent = delegate(GameObject _, PointerEventData _)
					{
						this._detailedDropdownOptionDragHandler.OnOptionBeginDrag();
					};
					holdDragButton2.OnEndDragEvent = delegate(GameObject _, PointerEventData _)
					{
						this._detailedDropdownOptionDragHandler.OnOptionEndDrag();
					};
					holdDragButton2.OnDragEvent = delegate(GameObject _, PointerEventData _)
					{
						this._detailedDropdownOptionDragHandler.OnOptionDrag();
					};
				}
			}
		}

		// Token: 0x0600BC1C RID: 48156 RVA: 0x00558AE8 File Offset: 0x00556CE8
		public void ForceResetOptionDragState()
		{
			DetailedDropdownOptionDragHandler detailedDropdownOptionDragHandler = this._detailedDropdownOptionDragHandler;
			if (detailedDropdownOptionDragHandler != null)
			{
				detailedDropdownOptionDragHandler.ForceResetState();
			}
		}

		// Token: 0x0600BC1D RID: 48157 RVA: 0x00558AFD File Offset: 0x00556CFD
		public void SetupCanShowMenuChecker(DetailFilterMultiSelectDropdown.CanShowMenuChecker canShowMenuChecker)
		{
			this._canShowMenuChecker = canShowMenuChecker;
		}

		// Token: 0x0600BC1E RID: 48158 RVA: 0x00558B07 File Offset: 0x00556D07
		public void SetupParentScrollView(RectTransform parentLayoutRect, ScrollRect parentScrollRect)
		{
			this._parentLayoutRect = parentLayoutRect;
			this._parentScrollRect = parentScrollRect;
		}

		// Token: 0x0600BC1F RID: 48159 RVA: 0x00558B18 File Offset: 0x00556D18
		private void OnEnterHold(int itemId)
		{
			this._holdingItemId = itemId;
		}

		// Token: 0x0600BC20 RID: 48160 RVA: 0x00558B24 File Offset: 0x00556D24
		private void ScrollToVisiblePosition()
		{
			bool settingMode = this._settingMode;
			if (!settingMode)
			{
				bool flag = this._parentLayoutRect == null || this._parentScrollRect == null;
				if (!flag)
				{
					RectTransform myRect = base.GetComponent<RectTransform>();
					bool flag2 = myRect == null;
					if (!flag2)
					{
						Rect mySize = myRect.rect;
						RectTransform viewportRect = this._parentScrollRect.viewport;
						Rect viewportSize = viewportRect.rect;
						RectTransform contentRect = this._parentScrollRect.content;
						Rect contentSize = contentRect.rect;
						Vector3 myWorldMin = myRect.TransformPoint(new Vector3(mySize.xMin, mySize.yMin, 0f));
						Vector3 myWorldMax = myRect.TransformPoint(new Vector3(mySize.xMax, mySize.yMax, 0f));
						Vector3 myViewportMin = viewportRect.InverseTransformPoint(myWorldMin);
						Vector3 myViewportMax = viewportRect.InverseTransformPoint(myWorldMax);
						Vector2 viewportMin = new Vector2(viewportSize.xMin, viewportSize.yMin);
						Vector2 viewportMax = new Vector2(viewportSize.xMax, viewportSize.yMax);
						Vector2 scrollDelta = Vector2.zero;
						bool flag3 = myViewportMax.y > viewportMax.y;
						if (flag3)
						{
							scrollDelta.y = myViewportMax.y - viewportMax.y;
						}
						else
						{
							bool flag4 = myViewportMin.y < viewportMin.y;
							if (flag4)
							{
								scrollDelta.y = myViewportMin.y - viewportMin.y;
							}
						}
						bool flag5 = myViewportMax.x > viewportMax.x;
						if (flag5)
						{
							scrollDelta.x = myViewportMax.x - viewportMax.x;
						}
						else
						{
							bool flag6 = myViewportMin.x < viewportMin.x;
							if (flag6)
							{
								scrollDelta.x = myViewportMin.x - viewportMin.x;
							}
						}
						bool flag7 = scrollDelta != Vector2.zero;
						if (flag7)
						{
							Vector2 currentPosition = contentRect.anchoredPosition;
							Vector2 newPosition = currentPosition - scrollDelta;
							float maxScrollY = Mathf.Max(0f, contentSize.height - viewportSize.height);
							float maxScrollX = Mathf.Max(0f, contentSize.width - viewportSize.width);
							newPosition.y = Mathf.Clamp(newPosition.y, 0f, maxScrollY);
							newPosition.x = Mathf.Clamp(newPosition.x, -maxScrollX, 0f);
							contentRect.anchoredPosition = newPosition;
						}
					}
				}
			}
		}

		// Token: 0x0600BC21 RID: 48161 RVA: 0x00558D98 File Offset: 0x00556F98
		public void ApplyOptionCustomOrder(MenuOptionsCustomOrderData orderData)
		{
			bool flag = !base.Config.EnableDragHold;
			if (!flag)
			{
				bool flag2 = orderData.Version < base.Config.Version;
				if (!flag2)
				{
					List<int> orderList = orderData.OptionOrderList;
					bool flag3 = orderList == null || orderList.Count == 0;
					if (!flag3)
					{
						this._optionItems.RemoveAll((DetailFilterMultiSelectDropdown.OptionItem o) => !o.IsSpecial);
						for (int i = 0; i < orderList.Count; i++)
						{
							int optionId = orderList[i];
							DetailFilterMultiSelectDropdown.OptionItem optionItem = this._optionItemDict[optionId];
							this._optionItems.Add(optionItem);
						}
						for (int j = 0; j < this._optionItems.Count; j++)
						{
							DetailFilterMultiSelectDropdown.OptionItem optionItem2 = this._optionItems[j];
							optionItem2.Refers.transform.SetSiblingIndex(j);
						}
					}
				}
			}
		}

		// Token: 0x0600BC22 RID: 48162 RVA: 0x00558EB0 File Offset: 0x005570B0
		public void ResetOptionCustomOrder()
		{
			this._optionItems.Sort((DetailFilterMultiSelectDropdown.OptionItem a, DetailFilterMultiSelectDropdown.OptionItem b) => a.Id.CompareTo(b.Id));
			for (int i = 0; i < this._optionItems.Count; i++)
			{
				DetailFilterMultiSelectDropdown.OptionItem optionItem = this._optionItems[i];
				optionItem.Refers.transform.SetSiblingIndex(i);
			}
		}

		// Token: 0x0600BC23 RID: 48163 RVA: 0x00558F24 File Offset: 0x00557124
		public MenuOptionsCustomOrderData? GetOptionCustomOrderData()
		{
			bool flag = !base.Config.EnableDragHold;
			MenuOptionsCustomOrderData? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				List<int> orderList = new List<int>();
				foreach (DetailFilterMultiSelectDropdown.OptionItem optionItem in this._optionItems)
				{
					bool isSpecial = optionItem.IsSpecial;
					if (!isSpecial)
					{
						orderList.Add(optionItem.Id);
					}
				}
				result = new MenuOptionsCustomOrderData?(new MenuOptionsCustomOrderData
				{
					Version = base.Config.Version,
					OptionOrderList = orderList
				});
			}
			return result;
		}

		// Token: 0x0600BC24 RID: 48164 RVA: 0x00558FE8 File Offset: 0x005571E8
		public void OnSwitchCustomOrderSettingMode(bool isSettingMode)
		{
			this._settingMode = isSettingMode;
			bool flag = base.Config.Dependency != null || !this.isMultiSelect;
			if (flag)
			{
				this.SetDrowDownDisable(isSettingMode);
			}
			else
			{
				DetailedDropdownOptionDragHandler detailedDropdownOptionDragHandler = this._detailedDropdownOptionDragHandler;
				if (detailedDropdownOptionDragHandler != null)
				{
					detailedDropdownOptionDragHandler.OnSwitchCustomOrderSettingMode(isSettingMode);
				}
				bool flag2 = !isSettingMode;
				if (flag2)
				{
					this.UpdateAllSwapTogglesVisibility(false);
				}
			}
		}

		// Token: 0x0600BC25 RID: 48165 RVA: 0x00559054 File Offset: 0x00557254
		private void SetDrowDownDisable(bool isSettingMode)
		{
			this.disableStyleRoot.SetStyleEffect(isSettingMode, false);
			if (isSettingMode)
			{
				this.HideMenu();
			}
			this._disableBySettingMode = isSettingMode;
		}

		// Token: 0x0600BC26 RID: 48166 RVA: 0x00559088 File Offset: 0x00557288
		public void SetupSwapToggle(Action onSwapToggleClicked)
		{
			Refers refers = this.MenuBarInstance.GetComponent<Refers>();
			CToggleObsolete swapToggle = refers.CGet<CToggleObsolete>("SwapToggle");
			swapToggle.onValueChanged.RemoveAllListeners();
			swapToggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				bool flag = !this._disableBySettingMode;
				if (flag)
				{
					Action onSwapToggleClicked2 = onSwapToggleClicked;
					if (onSwapToggleClicked2 != null)
					{
						onSwapToggleClicked2();
					}
				}
			});
		}

		// Token: 0x0600BC27 RID: 48167 RVA: 0x005590E8 File Offset: 0x005572E8
		private void UpdateAllSwapTogglesVisibility(bool isVisible)
		{
			foreach (DetailFilterMultiSelectDropdown.OptionItem optionItem in this._optionItems)
			{
				bool isSpecial = optionItem.IsSpecial;
				if (!isSpecial)
				{
					Refers refers = optionItem.Refers;
					CToggleObsolete swapToggle = refers.CGet<CToggleObsolete>("SwapToggle");
					bool flag = swapToggle != null;
					if (flag)
					{
						swapToggle.gameObject.SetActive(isVisible);
						swapToggle.isOn = false;
					}
				}
			}
		}

		// Token: 0x0600BC28 RID: 48168 RVA: 0x00559180 File Offset: 0x00557380
		protected override void CheckHideMenu()
		{
			DetailedDropdownOptionDragHandler detailedDropdownOptionDragHandler = this._detailedDropdownOptionDragHandler;
			bool flag = detailedDropdownOptionDragHandler != null && detailedDropdownOptionDragHandler.IsInDragMode;
			if (!flag)
			{
				base.CheckHideMenu();
			}
		}

		// Token: 0x0600BC29 RID: 48169 RVA: 0x005591B0 File Offset: 0x005573B0
		protected override void CheckShowMenu()
		{
			DetailedDropdownOptionDragHandler detailedDropdownOptionDragHandler = this._detailedDropdownOptionDragHandler;
			bool flag = detailedDropdownOptionDragHandler != null && detailedDropdownOptionDragHandler.IsInDragMode;
			if (!flag)
			{
				bool flag2 = !this.IsInValidArea();
				if (!flag2)
				{
					bool flag3 = CommonFakeHidePanel._fakeHideMenuCount > 0;
					if (!flag3)
					{
						bool flag4 = CommonFakeHidePanel.IsMouseInRect(this.menuBarSlot) && this._pointerTriggered;
						if (flag4)
						{
							this.ShowMenu();
						}
					}
				}
			}
		}

		// Token: 0x0600BC2A RID: 48170 RVA: 0x00559218 File Offset: 0x00557418
		protected override void BeforeFadeFakeMask(bool goFakeHide)
		{
			base.BeforeFadeFakeMask(goFakeHide);
			CanvasGroup viewPortCanvasGroup = this.viewport.GetComponent<CanvasGroup>();
			viewPortCanvasGroup.alpha = (float)(goFakeHide ? 1 : 0);
			foreach (CanvasGroup extraCanvasGroup in this.extraFadeTargets)
			{
				extraCanvasGroup.alpha = (float)(goFakeHide ? 1 : 0);
			}
		}

		// Token: 0x0600BC2B RID: 48171 RVA: 0x00559274 File Offset: 0x00557474
		protected override void JoinFadeFakeMask(Sequence fakeMaskTween, bool goFakeHide)
		{
			base.JoinFadeFakeMask(fakeMaskTween, goFakeHide);
			this._fakeMaskTween.Join(this.viewport.GetComponent<CanvasGroup>().DOFade((float)(goFakeHide ? 0 : 1), 0.2f));
			foreach (CanvasGroup extraCanvasGroup in this.extraFadeTargets)
			{
				this._fakeMaskTween.Join(extraCanvasGroup.DOFade((float)(goFakeHide ? 0 : 1), 0.2f));
			}
		}

		// Token: 0x0600BC2C RID: 48172 RVA: 0x005592F0 File Offset: 0x005574F0
		protected override void FadeFakeMaskOver(bool goFakeHide)
		{
			base.FadeFakeMaskOver(goFakeHide);
			this.viewport.GetComponent<CanvasGroup>().alpha = (float)(goFakeHide ? 0 : 1);
			foreach (CanvasGroup extraCanvasGroup in this.extraFadeTargets)
			{
				extraCanvasGroup.alpha = (float)(goFakeHide ? 0 : 1);
			}
		}

		// Token: 0x0600BC2D RID: 48173 RVA: 0x0055934C File Offset: 0x0055754C
		protected override bool NeedFakeHideMenu()
		{
			DetailedDropdownOptionDragHandler detailedDropdownOptionDragHandler = this._detailedDropdownOptionDragHandler;
			bool flag = detailedDropdownOptionDragHandler != null && detailedDropdownOptionDragHandler.IsInDragMode;
			return !flag && base.NeedFakeHideMenu();
		}

		// Token: 0x0600BC2E RID: 48174 RVA: 0x00559380 File Offset: 0x00557580
		public bool IsInValidArea()
		{
			this.scrollRect.GetWorldCorners(this._tempCorners);
			float innerLeftX = this._tempCorners[0].x;
			float innerRightX = this._tempCorners[3].x;
			bool flag = this._baseRect == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				this._baseRect.GetWorldCorners(this._tempCorners);
				float outerLeftX = this._tempCorners[0].x;
				float outerRightX = this._tempCorners[3].x;
				result = (innerLeftX >= outerLeftX && innerRightX <= outerRightX);
			}
			return result;
		}

		// Token: 0x0600BC2F RID: 48175 RVA: 0x00559428 File Offset: 0x00557628
		protected override void SetupMenuBarInternal()
		{
			this.RefreshMenuBar();
			PointerTrigger pointerTrigger = this.MenuBarInstance.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnMouseEnterMenuBar));
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnMouseExitMenuBar));
		}

		// Token: 0x0600BC30 RID: 48176 RVA: 0x00559494 File Offset: 0x00557694
		private void OnMouseEnterMenuBar()
		{
			this._pointerTriggered = true;
			bool flag = !this.ShouldBlockShowMenuDueToOtherMenuFakeHide();
			if (flag)
			{
				this.ShowMenu();
			}
			Action onMouseEnter = this.OnMouseEnter;
			if (onMouseEnter != null)
			{
				onMouseEnter();
			}
		}

		// Token: 0x0600BC31 RID: 48177 RVA: 0x005594D4 File Offset: 0x005576D4
		private bool ShouldBlockShowMenuDueToOtherMenuFakeHide()
		{
			DetailFilterMultiSelectDropdown[] allDropdowns = Object.FindObjectsOfType<DetailFilterMultiSelectDropdown>();
			foreach (DetailFilterMultiSelectDropdown dropdown in allDropdowns)
			{
				bool flag = dropdown == this;
				if (!flag)
				{
					bool flag2 = dropdown.IsMenuShow && dropdown.NeedFakeHideMenu();
					if (flag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600BC32 RID: 48178 RVA: 0x00559532 File Offset: 0x00557732
		private void OnMouseExitMenuBar()
		{
			this._pointerTriggered = false;
			Action onMouseExit = this.OnMouseExit;
			if (onMouseExit != null)
			{
				onMouseExit();
			}
		}

		// Token: 0x0600BC33 RID: 48179 RVA: 0x00559550 File Offset: 0x00557750
		public void UpdateMenuBarSwapToggleVisibility(bool isVisible)
		{
			Refers refers = this.MenuBarInstance.GetComponent<Refers>();
			CToggleObsolete swapToggle = refers.CGet<CToggleObsolete>("SwapToggle");
			swapToggle.gameObject.SetActive(isVisible);
			swapToggle.SetIsOnWithoutNotify(false);
		}

		// Token: 0x0600BC34 RID: 48180 RVA: 0x0055958C File Offset: 0x0055778C
		public void RefreshMenuBarSwapToggle(bool isVisible, bool isOn)
		{
			Refers refers = this.MenuBarInstance.GetComponent<Refers>();
			CToggleObsolete swapToggle = refers.CGet<CToggleObsolete>("SwapToggle");
			bool flag = swapToggle == null;
			if (!flag)
			{
				swapToggle.gameObject.SetActive(isVisible);
				swapToggle.isOn = isOn;
			}
		}

		// Token: 0x0600BC35 RID: 48181 RVA: 0x005595D4 File Offset: 0x005577D4
		private void OnEnable()
		{
			int selectedCount = this.SelectedIndices.Count;
			base.StartCoroutine(this.UpdateMenuBarSize(selectedCount > 0));
		}

		// Token: 0x0600BC36 RID: 48182 RVA: 0x00559600 File Offset: 0x00557800
		private void RefreshMenuBar()
		{
			FilterMenuBar refers = this.MenuBarInstance.GetComponent<FilterMenuBar>();
			int selectedCount = this.SelectedIndices.Count;
			int num = selectedCount;
			int num2 = num;
			string labelText;
			if (num2 != 0)
			{
				if (num2 != 1)
				{
					string format = "{0}({1})";
					MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig> config = base.Config;
					labelText = string.Format(format, config.MenuBarConfig.MenuBarLabel.GetString(), selectedCount);
				}
				else
				{
					labelText = base.Config.MenuItemConfigs[this.FirstSelectedIndex.Value].Text.GetString();
				}
			}
			else
			{
				MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig> config = base.Config;
				labelText = config.MenuBarConfig.MenuBarLabel.GetString();
			}
			refers.SetLabelText(labelText);
			this.MenuBarInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			float clearButtonWidth = this.clearAllButton.GetComponent<RectTransform>().rect.width;
			bool activeInHierarchy = base.gameObject.activeInHierarchy;
			if (activeInHierarchy)
			{
				base.StartCoroutine(this.UpdateMenuBarSize(selectedCount > 0));
			}
			this.RefreshMenuBarSwapToggle(false, false);
		}

		// Token: 0x0600BC37 RID: 48183 RVA: 0x00559718 File Offset: 0x00557918
		private void RefreshMenuBarIconDirection()
		{
			bool flag = !this.MenuBarInstance;
			if (!flag)
			{
				FilterMenuBar refers = this.MenuBarInstance.GetComponent<FilterMenuBar>();
				refers.SetStatusIcon(this.IsMenuShow);
				refers.SetSelected(this.IsMenuShow);
			}
		}

		// Token: 0x0600BC38 RID: 48184 RVA: 0x00559760 File Offset: 0x00557960
		protected override void SetupMenu()
		{
			int menuConfigsCount = this.GetDropdownCount();
			CommonUtils.PrepareEnoughChildren(this.itemRoot, this.itemTemplate, menuConfigsCount, null);
			this._optionItems.Clear();
			this._optionItemDict.Clear();
			for (int i = 0; i < menuConfigsCount; i++)
			{
				Transform item = this.itemRoot.GetChild(i);
				Refers refers = item.GetComponent<Refers>();
				int dataIndex = this.GetDataIndex(i);
				DetailFilterMultiSelectDropdown.OptionItem optionItem = new DetailFilterMultiSelectDropdown.OptionItem
				{
					Refers = refers,
					Id = dataIndex
				};
				this.RefreshItem(i, optionItem);
				this._optionItems.Add(optionItem);
				this._optionItemDict[dataIndex] = optionItem;
			}
			this.CalcMaxHeight();
			this.clearAllButton.ClearAndAddListener(new Action(this.OnClickClear));
		}

		// Token: 0x0600BC39 RID: 48185 RVA: 0x0055983C File Offset: 0x00557A3C
		private void OnClickClear()
		{
			bool isSettingMode = this._detailedDropdownOptionDragHandler.IsSettingMode;
			if (!isSettingMode)
			{
				this.UnSelectAll(true);
			}
		}

		// Token: 0x0600BC3A RID: 48186 RVA: 0x00559864 File Offset: 0x00557A64
		public void UnSelectAll(bool triggerEvent = true)
		{
			this.SelectedIndices.Clear();
			this.RefreshItems();
			this.RefreshMenuBar();
			if (triggerEvent)
			{
				Action onSelectionChanged = this.OnSelectionChanged;
				if (onSelectionChanged != null)
				{
					onSelectionChanged();
				}
			}
		}

		// Token: 0x0600BC3B RID: 48187 RVA: 0x005598A4 File Offset: 0x00557AA4
		private void SelectAll()
		{
			bool isSettingMode = this._detailedDropdownOptionDragHandler.IsSettingMode;
			if (!isSettingMode)
			{
				bool flag = this.SelectedIndices.Count == base.Config.MenuItemConfigs.Count;
				if (flag)
				{
					this.SelectedIndices.Clear();
				}
				else
				{
					this.SelectedIndices.Clear();
					bool isMultiSelect = this.isMultiSelect;
					if (isMultiSelect)
					{
						for (int i = 0; i < base.Config.MenuItemConfigs.Count; i++)
						{
							this.SelectedIndices.Add(i);
						}
					}
				}
				this.RefreshItems();
				this.RefreshMenuBar();
				Action onSelectionChanged = this.OnSelectionChanged;
				if (onSelectionChanged != null)
				{
					onSelectionChanged();
				}
			}
		}

		// Token: 0x0600BC3C RID: 48188 RVA: 0x00559960 File Offset: 0x00557B60
		private int GetDropdownCount()
		{
			int extraItemCount = 1;
			List<DetailFilterMultiSelectDropdownItemConfig> menuItemConfigs = base.Config.MenuItemConfigs;
			return ((menuItemConfigs != null) ? menuItemConfigs.Count : 0) + extraItemCount;
		}

		// Token: 0x0600BC3D RID: 48189 RVA: 0x00559990 File Offset: 0x00557B90
		private int GetDataIndex(int index)
		{
			return index - 1;
		}

		// Token: 0x0600BC3E RID: 48190 RVA: 0x005599A8 File Offset: 0x00557BA8
		private void RefreshItems()
		{
			for (int i = 0; i < this._optionItems.Count; i++)
			{
				DetailFilterMultiSelectDropdown.OptionItem item = this._optionItems[i];
				bool isSpecial = item.IsSpecial;
				if (!isSpecial)
				{
					this.RefreshItem(i, item);
				}
			}
		}

		// Token: 0x0600BC3F RID: 48191 RVA: 0x005599F4 File Offset: 0x00557BF4
		private void OnClickMenuItem(int id)
		{
			bool flag = this._holdingItemId == id;
			if (flag)
			{
				this._holdingItemId = -1;
			}
			else
			{
				DetailedDropdownOptionDragHandler detailedDropdownOptionDragHandler = this._detailedDropdownOptionDragHandler;
				bool flag2 = detailedDropdownOptionDragHandler != null && detailedDropdownOptionDragHandler.CheckAndClearWasInDragMode();
				if (!flag2)
				{
					DetailedDropdownOptionDragHandler detailedDropdownOptionDragHandler2 = this._detailedDropdownOptionDragHandler;
					bool flag3 = detailedDropdownOptionDragHandler2 != null && detailedDropdownOptionDragHandler2.IsSettingMode;
					if (flag3)
					{
						int itemIndex = this._optionItems.FindIndex((DetailFilterMultiSelectDropdown.OptionItem item) => item.Id == id);
						bool flag4 = itemIndex >= 0;
						if (flag4)
						{
							this.UpdateAllSwapTogglesVisibility(false);
							foreach (DetailFilterMultiSelectDropdown.OptionItem optionItem in this._optionItems)
							{
								bool isSpecial = optionItem.IsSpecial;
								if (!isSpecial)
								{
									Refers refers = optionItem.Refers;
									CToggleObsolete swapToggle = refers.CGet<CToggleObsolete>("SwapToggle");
									bool flag5 = swapToggle != null;
									if (flag5)
									{
										swapToggle.gameObject.SetActive(true);
										swapToggle.SetIsOnWithoutNotify(optionItem.Id == id);
									}
								}
							}
							this._detailedDropdownOptionDragHandler.HandleToggleClick(itemIndex);
						}
					}
					else
					{
						this.SelectedIndices.Clear();
						base.ToggleSelected(id);
						this.RefreshItems();
						this.RefreshMenuBar();
						Action onSelectionChanged = this.OnSelectionChanged;
						if (onSelectionChanged != null)
						{
							onSelectionChanged();
						}
					}
				}
			}
		}

		// Token: 0x0600BC40 RID: 48192 RVA: 0x00559B84 File Offset: 0x00557D84
		private void OnClickCheckBox(int id)
		{
			bool settingMode = this._settingMode;
			if (!settingMode)
			{
				base.ToggleSelected(id);
				this.RefreshItems();
				this.RefreshMenuBar();
				Action onSelectionChanged = this.OnSelectionChanged;
				if (onSelectionChanged != null)
				{
					onSelectionChanged();
				}
			}
		}

		// Token: 0x0600BC41 RID: 48193 RVA: 0x00559BC7 File Offset: 0x00557DC7
		private void RefreshItem(int index, DetailFilterMultiSelectDropdown.OptionItem item)
		{
			this.RefreshItemDisplay(index, item);
			this.RefreshItemButton(index, item);
			this.RefreshItemCheckButton(index, item);
			this.RefreshItemSwapToggle(index, item);
		}

		// Token: 0x0600BC42 RID: 48194 RVA: 0x00559BF0 File Offset: 0x00557DF0
		private void RefreshItemSwapToggle(int index, DetailFilterMultiSelectDropdown.OptionItem item)
		{
			Refers refers = item.Refers;
			CToggleObsolete swapToggle = refers.CGet<CToggleObsolete>("SwapToggle");
			bool flag = swapToggle == null;
			if (!flag)
			{
				bool isSpecial = item.IsSpecial;
				if (isSpecial)
				{
					swapToggle.gameObject.SetActive(false);
				}
				else
				{
					DetailedDropdownOptionDragHandler detailedDropdownOptionDragHandler = this._detailedDropdownOptionDragHandler;
					bool shouldBeVisible = detailedDropdownOptionDragHandler != null && detailedDropdownOptionDragHandler.IsSettingMode;
					swapToggle.gameObject.SetActive(shouldBeVisible && this._detailedDropdownOptionDragHandler.IsWaitingForSecondItemSelection());
					swapToggle.onValueChanged.RemoveAllListeners();
					swapToggle.onValueChanged.AddListener(delegate(bool isOn)
					{
						this.OnItemSwapToggleClicked(item.Id, isOn);
					});
					bool shouldBeOn = this._detailedDropdownOptionDragHandler != null && this._detailedDropdownOptionDragHandler.IsWaitingForSecondItemSelection() && this._detailedDropdownOptionDragHandler.GetFirstSelectedItemId() == item.Id;
					bool flag2 = swapToggle.isOn != shouldBeOn;
					if (flag2)
					{
						swapToggle.SetIsOnWithoutNotify(shouldBeOn);
					}
				}
			}
		}

		// Token: 0x0600BC43 RID: 48195 RVA: 0x00559D04 File Offset: 0x00557F04
		private void OnItemSwapToggleClicked(int id, bool isOn)
		{
			bool flag = this._detailedDropdownOptionDragHandler == null;
			if (!flag)
			{
				int itemIndex = this._optionItems.FindIndex((DetailFilterMultiSelectDropdown.OptionItem item) => item.Id == id);
				bool flag2 = itemIndex < 0;
				if (!flag2)
				{
					if (isOn)
					{
						this._detailedDropdownOptionDragHandler.HandleToggleClick(itemIndex);
					}
					else
					{
						this._detailedDropdownOptionDragHandler.HandleToggleClickOff(itemIndex);
					}
					bool flag3 = isOn && !this._detailedDropdownOptionDragHandler.IsWaitingForSecondItemSelection();
					if (flag3)
					{
						this.UpdateAllSwapTogglesVisibility(false);
					}
				}
			}
		}

		// Token: 0x0600BC44 RID: 48196 RVA: 0x00559D9C File Offset: 0x00557F9C
		private void RefreshItemCheckButton(int index, DetailFilterMultiSelectDropdown.OptionItem item)
		{
			Refers refers = item.Refers;
			CButtonObsolete checkBox = refers.CGet<CButtonObsolete>("CheckBox");
			bool visible = index > 0 && this.isMultiSelect;
			checkBox.gameObject.SetActive(visible);
			bool flag = visible;
			if (flag)
			{
				checkBox.ClearAndAddListener(delegate
				{
					this.OnClickCheckBox(item.Id);
				});
			}
		}

		// Token: 0x0600BC45 RID: 48197 RVA: 0x00559E10 File Offset: 0x00558010
		private void RefreshItemButton(int index, DetailFilterMultiSelectDropdown.OptionItem item)
		{
			Refers refers = item.Refers;
			CButtonObsolete button = refers.CGet<CButtonObsolete>("Button");
			if (index != 0)
			{
				button.ClearAndAddListener(delegate
				{
					this.OnClickMenuItem(item.Id);
				});
			}
			else
			{
				button.ClearAndAddListener(new Action(this.SelectAll));
			}
		}

		// Token: 0x0600BC46 RID: 48198 RVA: 0x00559E80 File Offset: 0x00558080
		private void RefreshItemDisplay(int index, DetailFilterMultiSelectDropdown.OptionItem item)
		{
			Refers refers = item.Refers;
			List<TextMeshProUGUI> labelList = refers.CGetList<TextMeshProUGUI>("Label_");
			bool flag = index == 0;
			if (flag)
			{
				string allString = this.isMultiSelect ? ((base.Config.LogicType == FilterLogic.Or) ? LocalStringManager.Get(LanguageKey.LK_Common_All) : LocalStringManager.Get(LanguageKey.LK_CommonSortAndFilter_SelectAll)) : LocalStringManager.Get(LanguageKey.LK_Common_All);
				foreach (TextMeshProUGUI label in labelList)
				{
					label.text = allString;
				}
				DetailFilterMultiSelectDropdown.RefreshSelectedInner(refers, false, this.isMultiSelect);
			}
			else
			{
				int id = item.Id;
				string str = base.Config.MenuItemConfigs[id].Text.GetString();
				foreach (TextMeshProUGUI label2 in labelList)
				{
					label2.text = str;
				}
				this.RefreshItemSelected(id, refers);
			}
		}

		// Token: 0x0600BC47 RID: 48199 RVA: 0x00559FBC File Offset: 0x005581BC
		private void RefreshItemSelected(int id, Refers refers)
		{
			bool selected = this.SelectedIndices.Contains(id);
			DetailFilterMultiSelectDropdown.RefreshSelectedInner(refers, selected, this.isMultiSelect);
		}

		// Token: 0x0600BC48 RID: 48200 RVA: 0x00559FE8 File Offset: 0x005581E8
		private static void RefreshSelectedInner(Refers refers, bool selected, bool isMultiSelect)
		{
			GameObject selectMark = refers.CGet<GameObject>("SelectMark");
			selectMark.SetActive(selected);
			GameObject buttonSelected = refers.CGet<GameObject>("ButtonSelected");
			buttonSelected.SetActive(selected && !isMultiSelect);
		}

		// Token: 0x0600BC49 RID: 48201 RVA: 0x0055A028 File Offset: 0x00558228
		private void CalcMaxHeight()
		{
			VerticalLayoutGroup layout = this.itemRoot.GetComponent<VerticalLayoutGroup>();
			RectOffset padding = layout.padding;
			float spacing = layout.spacing;
			float itemHeight = this.itemTemplate.GetComponent<RectTransform>().rect.height;
			int itemCount = this.GetDropdownCount();
			bool flag = !this.DisableAutoHeight;
			if (flag)
			{
				if (this._autoHeightHelper == null)
				{
					this._autoHeightHelper = base.GetComponent<ScrollRectAutoHeightHelper>();
				}
				this._autoHeightHelper.maxHeight = (float)(padding.top + padding.bottom) + itemHeight * (float)itemCount + spacing * (float)(itemCount - 1);
			}
		}

		// Token: 0x0600BC4A RID: 48202 RVA: 0x0055A0C0 File Offset: 0x005582C0
		protected override void ShowMenu()
		{
			bool flag = !this._canShowMenuChecker() || this._disableBySettingMode;
			if (!flag)
			{
				this.showHideMenuTarget.gameObject.SetActive(true);
				this.RefreshMenuBarIconDirection();
				this.menuOffsetTarget.anchoredPosition = new Vector2(this.GetDropdownX(), this.menuOffsetTarget.anchoredPosition.y);
				bool activeInHierarchy = base.gameObject.activeInHierarchy;
				if (activeInHierarchy)
				{
					base.StartCoroutine(this.UpdateMenuBarSize(this.clearAllButton.gameObject.activeSelf));
				}
			}
		}

		// Token: 0x0600BC4B RID: 48203 RVA: 0x0055A156 File Offset: 0x00558356
		private IEnumerator UpdateMenuBarSize(bool showClearAllButton)
		{
			yield return null;
			RectTransform rect = base.GetComponent<RectTransform>();
			LayoutElement layoutElement = base.GetComponent<LayoutElement>();
			float maxOptionWidth = this.GetMaxOptionWidth();
			float clearButtonWidth = this.clearAllButton.GetComponent<RectTransform>().rect.width;
			layoutElement.preferredWidth = maxOptionWidth + (showClearAllButton ? clearButtonWidth : 0f);
			float myWidth = layoutElement.preferredWidth;
			myWidth = Math.Max(myWidth, this.baseWidth);
			float myHeight = rect.rect.height;
			float menuBarSizeX = myWidth - (showClearAllButton ? clearButtonWidth : 0f);
			float menuBarSizeY = myHeight;
			this.MenuBarInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			Vector2 size = new Vector2(menuBarSizeX, menuBarSizeY);
			this.MenuBarInstance.GetComponent<RectTransform>().SetSize(size);
			this.menuBarSlot.GetComponent<RectTransform>().SetSize(size);
			this.clearAllButton.gameObject.SetActive(showClearAllButton);
			yield break;
		}

		// Token: 0x0600BC4C RID: 48204 RVA: 0x0055A16C File Offset: 0x0055836C
		public override void HideMenu()
		{
			this.showHideMenuTarget.gameObject.SetActive(false);
			this.RefreshMenuBarIconDirection();
		}

		// Token: 0x0600BC4D RID: 48205 RVA: 0x0055A188 File Offset: 0x00558388
		public void SetPointerTriggerInteractable(bool interactable)
		{
			PointerTrigger pointerTrigger = this.MenuBarInstance.GetComponent<PointerTrigger>();
			bool flag = pointerTrigger != null;
			if (flag)
			{
				pointerTrigger.enabled = interactable;
			}
			bool flag2 = this.IsMenuShow && !interactable;
			if (flag2)
			{
				this.HideMenu();
			}
		}

		// Token: 0x0600BC4E RID: 48206 RVA: 0x0055A1D4 File Offset: 0x005583D4
		private float GetDropdownX()
		{
			return this.HasVisibleCheckBox() ? -90f : -18f;
		}

		// Token: 0x0600BC4F RID: 48207 RVA: 0x0055A1FC File Offset: 0x005583FC
		private bool HasVisibleCheckBox()
		{
			foreach (DetailFilterMultiSelectDropdown.OptionItem item in this._optionItems)
			{
				Refers refers = item.Refers;
				CButtonObsolete checkBox = refers.CGet<CButtonObsolete>("CheckBox");
				bool activeSelf = checkBox.gameObject.activeSelf;
				if (activeSelf)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600BC50 RID: 48208 RVA: 0x0055A280 File Offset: 0x00558480
		private float GetMaxOptionWidth()
		{
			float maxWidth = 0f;
			foreach (DetailFilterMultiSelectDropdown.OptionItem item in this._optionItems)
			{
				Refers refers = item.Refers;
				CButtonObsolete button = refers.CGet<CButtonObsolete>("Button");
				RectTransform rectTransform = button.GetComponent<RectTransform>();
				LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
				float width = rectTransform.rect.width;
				bool flag = width > maxWidth;
				if (flag)
				{
					maxWidth = width;
				}
				LayoutElement layoutElement = button.GetComponent<LayoutElement>();
				bool flag2 = layoutElement != null && layoutElement.minWidth > maxWidth;
				if (flag2)
				{
					maxWidth = layoutElement.minWidth;
				}
			}
			return maxWidth;
		}

		// Token: 0x0600BC51 RID: 48209 RVA: 0x0055A354 File Offset: 0x00558554
		public void SetOptionInteractable(int optionIndex, bool interactable, string disabledTooltip)
		{
			DetailFilterMultiSelectDropdown.OptionItem optionItem;
			bool flag = !this._optionItemDict.TryGetValue(optionIndex, out optionItem);
			if (!flag)
			{
				Refers refers = optionItem.Refers;
				CButtonObsolete button = refers.CGet<CButtonObsolete>("Button");
				bool flag2 = button != null;
				if (flag2)
				{
					button.interactable = interactable;
				}
				CButtonObsolete checkBox = refers.CGet<CButtonObsolete>("CheckBox");
				bool flag3 = checkBox != null;
				if (flag3)
				{
					checkBox.interactable = interactable;
				}
				DisableStyleRoot styleRoot = refers.GetComponent<DisableStyleRoot>();
				bool flag4 = styleRoot != null;
				if (flag4)
				{
					styleRoot.SetStyleEffect(!interactable, false);
				}
				TooltipInvoker tip = refers.GetComponent<TooltipInvoker>();
				bool flag5 = tip != null;
				if (flag5)
				{
					tip.enabled = !interactable;
					bool flag6 = !interactable;
					if (flag6)
					{
						tip.Type = TipType.SingleDesc;
						TooltipInvoker tooltipInvoker = tip;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						tip.RuntimeParam.Set("arg0", disabledTooltip);
					}
				}
			}
		}

		// Token: 0x0600BC52 RID: 48210 RVA: 0x0055A458 File Offset: 0x00558658
		public void SetOption(int optionIndex)
		{
			bool flag = optionIndex < 0;
			if (flag)
			{
				this.SelectAll();
			}
			else
			{
				this.OnClickMenuItem(optionIndex);
			}
		}

		// Token: 0x040090EC RID: 37100
		[Header("有依赖项时（衍生列表）")]
		[SerializeField]
		protected GameObject menuBarTemplateDependency;

		// Token: 0x040090ED RID: 37101
		private bool _disableAutoHeight;

		// Token: 0x040090EE RID: 37102
		public const int MultiSelectionModeAllIndex = 0;

		// Token: 0x040090EF RID: 37103
		private ScrollRectAutoHeightHelper _autoHeightHelper;

		// Token: 0x040090F0 RID: 37104
		private List<DetailFilterMultiSelectDropdownItemConfig> _itemConfigs;

		// Token: 0x040090F1 RID: 37105
		private readonly List<DetailFilterMultiSelectDropdown.OptionItem> _optionItems = new List<DetailFilterMultiSelectDropdown.OptionItem>();

		// Token: 0x040090F2 RID: 37106
		private readonly Dictionary<int, DetailFilterMultiSelectDropdown.OptionItem> _optionItemDict = new Dictionary<int, DetailFilterMultiSelectDropdown.OptionItem>();

		// Token: 0x040090F3 RID: 37107
		private DetailedDropdownOptionDragHandler _detailedDropdownOptionDragHandler;

		// Token: 0x040090F4 RID: 37108
		private float _maxHeight;

		// Token: 0x040090F5 RID: 37109
		[NonSerialized]
		public Action OnSelectionChanged;

		// Token: 0x040090F6 RID: 37110
		[NonSerialized]
		public Action OnMouseEnter;

		// Token: 0x040090F7 RID: 37111
		[NonSerialized]
		public Action OnMouseExit;

		// Token: 0x040090F8 RID: 37112
		private DetailFilterMultiSelectDropdown.CanShowMenuChecker _canShowMenuChecker;

		// Token: 0x040090F9 RID: 37113
		private Action _onCustomOrderChange;

		// Token: 0x040090FA RID: 37114
		private int _holdingItemId = -1;

		// Token: 0x040090FB RID: 37115
		protected RectTransform _baseRect;

		// Token: 0x040090FC RID: 37116
		private bool _disableBySettingMode = false;

		// Token: 0x040090FD RID: 37117
		private bool _pointerTriggered = false;

		// Token: 0x040090FE RID: 37118
		private bool _settingMode = false;

		// Token: 0x040090FF RID: 37119
		private RectTransform _parentLayoutRect;

		// Token: 0x04009100 RID: 37120
		private ScrollRect _parentScrollRect;

		// Token: 0x04009101 RID: 37121
		[SerializeField]
		private DetailFilterMultiSelectDropdown.EDropdownDirectionMode dropdownDirectionMode = DetailFilterMultiSelectDropdown.EDropdownDirectionMode.AlwaysDown;

		// Token: 0x04009102 RID: 37122
		[SerializeField]
		private bool autoAdjustScrollHeight = true;

		// Token: 0x04009103 RID: 37123
		[SerializeField]
		private float autoAdjustBottomMargin = 20f;

		// Token: 0x04009104 RID: 37124
		[SerializeField]
		private float baseWidth = 150f;

		// Token: 0x04009105 RID: 37125
		[SerializeField]
		private RectTransform showHideMenuTarget;

		// Token: 0x04009106 RID: 37126
		[SerializeField]
		private RectTransform menuOffsetTarget;

		// Token: 0x04009107 RID: 37127
		[SerializeField]
		private RectTransform scrollRect;

		// Token: 0x04009108 RID: 37128
		[SerializeField]
		private RectTransform itemRoot;

		// Token: 0x04009109 RID: 37129
		[SerializeField]
		private RectTransform viewport;

		// Token: 0x0400910A RID: 37130
		[SerializeField]
		private CButtonObsolete clearAllButton;

		// Token: 0x0400910B RID: 37131
		[SerializeField]
		private CanvasGroup[] extraFadeTargets;

		// Token: 0x0200264E RID: 9806
		// (Invoke) Token: 0x06011B8A RID: 72586
		public delegate bool CanShowMenuChecker();

		// Token: 0x0200264F RID: 9807
		private enum EDropdownDirectionMode
		{
			// Token: 0x0400EA37 RID: 59959
			AlwaysDown
		}

		// Token: 0x02002650 RID: 9808
		public class OptionItem
		{
			// Token: 0x17001BB6 RID: 7094
			// (get) Token: 0x06011B8D RID: 72589 RVA: 0x006877ED File Offset: 0x006859ED
			public bool IsSpecial
			{
				get
				{
					return this.Id < 0;
				}
			}

			// Token: 0x0400EA38 RID: 59960
			public Refers Refers;

			// Token: 0x0400EA39 RID: 59961
			public int Id;
		}
	}
}
