using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonSortAndFilterLegacy;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001019 RID: 4121
	public class IndividualFilterMultiSelectDropdown : BaseMultiSelectDropdown<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig>
	{
		// Token: 0x1700153E RID: 5438
		// (get) Token: 0x0600BC6D RID: 48237 RVA: 0x0055AB3C File Offset: 0x00558D3C
		// (set) Token: 0x0600BC6E RID: 48238 RVA: 0x0055AB54 File Offset: 0x00558D54
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

		// Token: 0x0600BC6F RID: 48239 RVA: 0x0055AB65 File Offset: 0x00558D65
		public int GetOptionOrder(int index)
		{
			return this._optionItems.IndexOf(this._optionItemDict[index]);
		}

		// Token: 0x1700153F RID: 5439
		// (get) Token: 0x0600BC70 RID: 48240 RVA: 0x0055AB7E File Offset: 0x00558D7E
		protected override bool IsMenuShow
		{
			get
			{
				return this.showHideMenuTarget.gameObject.activeSelf;
			}
		}

		// Token: 0x17001540 RID: 5440
		// (get) Token: 0x0600BC71 RID: 48241 RVA: 0x0055AB90 File Offset: 0x00558D90
		private int? FirstSelectedIndex
		{
			get
			{
				return new int?(this.SelectedIndices.First<int>());
			}
		}

		// Token: 0x0600BC72 RID: 48242 RVA: 0x0055ABA4 File Offset: 0x00558DA4
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

		// Token: 0x0600BC73 RID: 48243 RVA: 0x0055AC0C File Offset: 0x00558E0C
		private void RefreshAutoHeightHelperEnabled()
		{
			if (this._autoHeightHelper == null)
			{
				this._autoHeightHelper = base.GetComponent<ScrollRectAutoHeightHelper>();
			}
			this._autoHeightHelper.enabled = (!this.DisableAutoHeight && this.dropdownDirectionMode == IndividualFilterMultiSelectDropdown.EDropdownDirectionMode.AlwaysDown);
		}

		// Token: 0x0600BC74 RID: 48244 RVA: 0x0055AC44 File Offset: 0x00558E44
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

		// Token: 0x0600BC75 RID: 48245 RVA: 0x0055ACA4 File Offset: 0x00558EA4
		protected override void Update()
		{
			bool disableBySettingMode = this._disableBySettingMode;
			if (!disableBySettingMode)
			{
				base.Update();
			}
		}

		// Token: 0x0600BC76 RID: 48246 RVA: 0x0055ACC6 File Offset: 0x00558EC6
		public void SetupCanShowMenuChecker(IndividualFilterMultiSelectDropdown.CanShowMenuChecker canShowMenuChecker)
		{
			this._canShowMenuChecker = canShowMenuChecker;
		}

		// Token: 0x0600BC77 RID: 48247 RVA: 0x0055ACD0 File Offset: 0x00558ED0
		public void SetupParentScrollView(RectTransform parentLayoutRect, ScrollRect parentScrollRect)
		{
			this._parentLayoutRect = parentLayoutRect;
			this._parentScrollRect = parentScrollRect;
		}

		// Token: 0x0600BC78 RID: 48248 RVA: 0x0055ACE1 File Offset: 0x00558EE1
		private void OnEnterHold(int itemId)
		{
			this._holdingItemId = itemId;
		}

		// Token: 0x0600BC79 RID: 48249 RVA: 0x0055ACEC File Offset: 0x00558EEC
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

		// Token: 0x0600BC7A RID: 48250 RVA: 0x0055AF60 File Offset: 0x00559160
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
						this._optionItems.RemoveAll((IndividualFilterMultiSelectDropdown.OptionItem o) => !o.IsSpecial);
						for (int i = 0; i < orderList.Count; i++)
						{
							int optionId = orderList[i];
							IndividualFilterMultiSelectDropdown.OptionItem optionItem = this._optionItemDict[optionId];
							this._optionItems.Add(optionItem);
						}
						for (int j = 0; j < this._optionItems.Count; j++)
						{
							IndividualFilterMultiSelectDropdown.OptionItem optionItem2 = this._optionItems[j];
							optionItem2.Refers.transform.SetSiblingIndex(j);
						}
					}
				}
			}
		}

		// Token: 0x0600BC7B RID: 48251 RVA: 0x0055B078 File Offset: 0x00559278
		public void ResetOptionCustomOrder()
		{
			this._optionItems.Sort((IndividualFilterMultiSelectDropdown.OptionItem a, IndividualFilterMultiSelectDropdown.OptionItem b) => a.Id.CompareTo(b.Id));
			for (int i = 0; i < this._optionItems.Count; i++)
			{
				IndividualFilterMultiSelectDropdown.OptionItem optionItem = this._optionItems[i];
				optionItem.Refers.transform.SetSiblingIndex(i);
			}
		}

		// Token: 0x0600BC7C RID: 48252 RVA: 0x0055B0EC File Offset: 0x005592EC
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
				foreach (IndividualFilterMultiSelectDropdown.OptionItem optionItem in this._optionItems)
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

		// Token: 0x0600BC7D RID: 48253 RVA: 0x0055B1B0 File Offset: 0x005593B0
		private void SetDrowDownDisable(bool isSettingMode)
		{
			this.disableStyleRoot.SetStyleEffect(isSettingMode, false);
			if (isSettingMode)
			{
				this.HideMenu();
			}
			this._disableBySettingMode = isSettingMode;
		}

		// Token: 0x0600BC7E RID: 48254 RVA: 0x0055B1E1 File Offset: 0x005593E1
		protected override void CheckHideMenu()
		{
			base.CheckHideMenu();
		}

		// Token: 0x0600BC7F RID: 48255 RVA: 0x0055B1EC File Offset: 0x005593EC
		protected override void CheckShowMenu()
		{
			bool flag = !this.IsInValidArea();
			if (!flag)
			{
				bool flag2 = CommonFakeHidePanel.IsMouseInRect(this.menuBarSlot) && this._pointerTriggered;
				if (flag2)
				{
					this.ShowMenu();
				}
			}
		}

		// Token: 0x0600BC80 RID: 48256 RVA: 0x0055B22C File Offset: 0x0055942C
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

		// Token: 0x0600BC81 RID: 48257 RVA: 0x0055B288 File Offset: 0x00559488
		protected override void JoinFadeFakeMask(Sequence fakeMaskTween, bool goFakeHide)
		{
			base.JoinFadeFakeMask(fakeMaskTween, goFakeHide);
			this._fakeMaskTween.Join(this.viewport.GetComponent<CanvasGroup>().DOFade((float)(goFakeHide ? 0 : 1), 0.2f));
			foreach (CanvasGroup extraCanvasGroup in this.extraFadeTargets)
			{
				this._fakeMaskTween.Join(extraCanvasGroup.DOFade((float)(goFakeHide ? 0 : 1), 0.2f));
			}
		}

		// Token: 0x0600BC82 RID: 48258 RVA: 0x0055B304 File Offset: 0x00559504
		protected override void FadeFakeMaskOver(bool goFakeHide)
		{
			base.FadeFakeMaskOver(goFakeHide);
			this.viewport.GetComponent<CanvasGroup>().alpha = (float)(goFakeHide ? 0 : 1);
			foreach (CanvasGroup extraCanvasGroup in this.extraFadeTargets)
			{
				extraCanvasGroup.alpha = (float)(goFakeHide ? 0 : 1);
			}
		}

		// Token: 0x0600BC83 RID: 48259 RVA: 0x0055B360 File Offset: 0x00559560
		protected override bool NeedFakeHideMenu()
		{
			return base.NeedFakeHideMenu();
		}

		// Token: 0x0600BC84 RID: 48260 RVA: 0x0055B378 File Offset: 0x00559578
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

		// Token: 0x0600BC85 RID: 48261 RVA: 0x0055B420 File Offset: 0x00559620
		protected override void SetupMenuBarInternal()
		{
			this.RefreshMenuBar();
			PointerTrigger pointerTrigger = this.MenuBarInstance.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnMouseEnterMenuBar));
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnMouseExitMenuBar));
		}

		// Token: 0x0600BC86 RID: 48262 RVA: 0x0055B489 File Offset: 0x00559689
		private void OnMouseEnterMenuBar()
		{
			this._pointerTriggered = true;
			this.ShowMenu();
			Action onMouseEnter = this.OnMouseEnter;
			if (onMouseEnter != null)
			{
				onMouseEnter();
			}
		}

		// Token: 0x0600BC87 RID: 48263 RVA: 0x0055B4AC File Offset: 0x005596AC
		private void OnMouseExitMenuBar()
		{
			this._pointerTriggered = false;
			Action onMouseExit = this.OnMouseExit;
			if (onMouseExit != null)
			{
				onMouseExit();
			}
		}

		// Token: 0x0600BC88 RID: 48264 RVA: 0x0055B4C8 File Offset: 0x005596C8
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
		}

		// Token: 0x0600BC89 RID: 48265 RVA: 0x0055B5D4 File Offset: 0x005597D4
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

		// Token: 0x0600BC8A RID: 48266 RVA: 0x0055B61C File Offset: 0x0055981C
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
				IndividualFilterMultiSelectDropdown.OptionItem optionItem = new IndividualFilterMultiSelectDropdown.OptionItem
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

		// Token: 0x0600BC8B RID: 48267 RVA: 0x0055B6F5 File Offset: 0x005598F5
		private void OnClickClear()
		{
			this.UnSelectAll(true);
		}

		// Token: 0x0600BC8C RID: 48268 RVA: 0x0055B700 File Offset: 0x00559900
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

		// Token: 0x0600BC8D RID: 48269 RVA: 0x0055B740 File Offset: 0x00559940
		public void SetSelectWithoutNotify(int index)
		{
			bool flag = index < 0;
			if (flag)
			{
				this.UnSelectAll(false);
			}
			else
			{
				base.ToggleSelected(index);
				this.RefreshItems();
				this.RefreshMenuBar();
			}
		}

		// Token: 0x0600BC8E RID: 48270 RVA: 0x0055B77C File Offset: 0x0055997C
		private void SelectAll()
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

		// Token: 0x0600BC8F RID: 48271 RVA: 0x0055B820 File Offset: 0x00559A20
		private int GetDropdownCount()
		{
			int extraItemCount = 1;
			List<DetailFilterMultiSelectDropdownItemConfig> menuItemConfigs = base.Config.MenuItemConfigs;
			return ((menuItemConfigs != null) ? menuItemConfigs.Count : 0) + extraItemCount;
		}

		// Token: 0x0600BC90 RID: 48272 RVA: 0x0055B850 File Offset: 0x00559A50
		private int GetDataIndex(int index)
		{
			return index - 1;
		}

		// Token: 0x0600BC91 RID: 48273 RVA: 0x0055B868 File Offset: 0x00559A68
		private void RefreshItems()
		{
			for (int i = 0; i < this._optionItems.Count; i++)
			{
				IndividualFilterMultiSelectDropdown.OptionItem item = this._optionItems[i];
				bool isSpecial = item.IsSpecial;
				if (!isSpecial)
				{
					this.RefreshItem(i, item);
				}
			}
		}

		// Token: 0x0600BC92 RID: 48274 RVA: 0x0055B8B4 File Offset: 0x00559AB4
		private void OnClickMenuItem(int id)
		{
			bool flag = this._holdingItemId == id;
			if (flag)
			{
				this._holdingItemId = -1;
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

		// Token: 0x0600BC93 RID: 48275 RVA: 0x0055B910 File Offset: 0x00559B10
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

		// Token: 0x0600BC94 RID: 48276 RVA: 0x0055B953 File Offset: 0x00559B53
		private void RefreshItem(int index, IndividualFilterMultiSelectDropdown.OptionItem item)
		{
			this.RefreshItemDisplay(index, item);
			this.RefreshItemButton(index, item);
			this.RefreshItemCheckButton(index, item);
		}

		// Token: 0x0600BC95 RID: 48277 RVA: 0x0055B974 File Offset: 0x00559B74
		private void RefreshItemCheckButton(int index, IndividualFilterMultiSelectDropdown.OptionItem item)
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

		// Token: 0x0600BC96 RID: 48278 RVA: 0x0055B9E8 File Offset: 0x00559BE8
		private void RefreshItemButton(int index, IndividualFilterMultiSelectDropdown.OptionItem item)
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

		// Token: 0x0600BC97 RID: 48279 RVA: 0x0055BA58 File Offset: 0x00559C58
		private void RefreshItemDisplay(int index, IndividualFilterMultiSelectDropdown.OptionItem item)
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
				IndividualFilterMultiSelectDropdown.RefreshSelectedInner(refers, false, this.isMultiSelect);
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

		// Token: 0x0600BC98 RID: 48280 RVA: 0x0055BB94 File Offset: 0x00559D94
		private void RefreshItemSelected(int id, Refers refers)
		{
			bool selected = this.SelectedIndices.Contains(id);
			IndividualFilterMultiSelectDropdown.RefreshSelectedInner(refers, selected, this.isMultiSelect);
		}

		// Token: 0x0600BC99 RID: 48281 RVA: 0x0055BBC0 File Offset: 0x00559DC0
		private static void RefreshSelectedInner(Refers refers, bool selected, bool isMultiSelect)
		{
			GameObject selectMark = refers.CGet<GameObject>("SelectMark");
			selectMark.SetActive(selected);
			GameObject buttonSelected = refers.CGet<GameObject>("ButtonSelected");
			buttonSelected.SetActive(selected && !isMultiSelect);
		}

		// Token: 0x0600BC9A RID: 48282 RVA: 0x0055BC00 File Offset: 0x00559E00
		private void OnEnable()
		{
			int selectedCount = this.SelectedIndices.Count;
			base.StartCoroutine(this.UpdateMenuBarSize(selectedCount > 0));
		}

		// Token: 0x0600BC9B RID: 48283 RVA: 0x0055BC2C File Offset: 0x00559E2C
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

		// Token: 0x0600BC9C RID: 48284 RVA: 0x0055BCC4 File Offset: 0x00559EC4
		protected override void ShowMenu()
		{
			bool flag = (this._canShowMenuChecker != null && !this._canShowMenuChecker()) || this._disableBySettingMode;
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

		// Token: 0x0600BC9D RID: 48285 RVA: 0x0055BD62 File Offset: 0x00559F62
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

		// Token: 0x0600BC9E RID: 48286 RVA: 0x0055BD78 File Offset: 0x00559F78
		public override void HideMenu()
		{
			this.showHideMenuTarget.gameObject.SetActive(false);
			this.RefreshMenuBarIconDirection();
		}

		// Token: 0x0600BC9F RID: 48287 RVA: 0x0055BD94 File Offset: 0x00559F94
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

		// Token: 0x0600BCA0 RID: 48288 RVA: 0x0055BDE0 File Offset: 0x00559FE0
		private float GetDropdownX()
		{
			return this.HasVisibleCheckBox() ? -90f : -18f;
		}

		// Token: 0x0600BCA1 RID: 48289 RVA: 0x0055BE08 File Offset: 0x0055A008
		private bool HasVisibleCheckBox()
		{
			foreach (IndividualFilterMultiSelectDropdown.OptionItem item in this._optionItems)
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

		// Token: 0x0600BCA2 RID: 48290 RVA: 0x0055BE8C File Offset: 0x0055A08C
		private float GetMaxOptionWidth()
		{
			float maxWidth = 0f;
			foreach (IndividualFilterMultiSelectDropdown.OptionItem item in this._optionItems)
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

		// Token: 0x0600BCA3 RID: 48291 RVA: 0x0055BF60 File Offset: 0x0055A160
		public void SetOptionInteractable(int optionIndex, bool interactable, string disabledTooltip)
		{
			IndividualFilterMultiSelectDropdown.OptionItem optionItem;
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

		// Token: 0x04009111 RID: 37137
		[Header("有依赖项时（衍生列表）")]
		[SerializeField]
		protected GameObject menuBarTemplateDependency;

		// Token: 0x04009112 RID: 37138
		public const int MultiSelectionModeAllIndex = 0;

		// Token: 0x04009113 RID: 37139
		private ScrollRectAutoHeightHelper _autoHeightHelper;

		// Token: 0x04009114 RID: 37140
		private bool _disableAutoHeight;

		// Token: 0x04009115 RID: 37141
		private List<DetailFilterMultiSelectDropdownItemConfig> _itemConfigs;

		// Token: 0x04009116 RID: 37142
		private readonly List<IndividualFilterMultiSelectDropdown.OptionItem> _optionItems = new List<IndividualFilterMultiSelectDropdown.OptionItem>();

		// Token: 0x04009117 RID: 37143
		private readonly Dictionary<int, IndividualFilterMultiSelectDropdown.OptionItem> _optionItemDict = new Dictionary<int, IndividualFilterMultiSelectDropdown.OptionItem>();

		// Token: 0x04009118 RID: 37144
		private float _maxHeight;

		// Token: 0x04009119 RID: 37145
		[NonSerialized]
		public Action OnSelectionChanged;

		// Token: 0x0400911A RID: 37146
		[NonSerialized]
		public Action OnMouseEnter;

		// Token: 0x0400911B RID: 37147
		[NonSerialized]
		public Action OnMouseExit;

		// Token: 0x0400911C RID: 37148
		private IndividualFilterMultiSelectDropdown.CanShowMenuChecker _canShowMenuChecker;

		// Token: 0x0400911D RID: 37149
		private Action _onCustomOrderChange;

		// Token: 0x0400911E RID: 37150
		private int _holdingItemId = -1;

		// Token: 0x0400911F RID: 37151
		protected RectTransform _baseRect;

		// Token: 0x04009120 RID: 37152
		private bool _disableBySettingMode = false;

		// Token: 0x04009121 RID: 37153
		private bool _pointerTriggered = false;

		// Token: 0x04009122 RID: 37154
		private bool _settingMode = false;

		// Token: 0x04009123 RID: 37155
		private RectTransform _parentLayoutRect;

		// Token: 0x04009124 RID: 37156
		private ScrollRect _parentScrollRect;

		// Token: 0x04009125 RID: 37157
		[SerializeField]
		private IndividualFilterMultiSelectDropdown.EDropdownDirectionMode dropdownDirectionMode = IndividualFilterMultiSelectDropdown.EDropdownDirectionMode.AlwaysDown;

		// Token: 0x04009126 RID: 37158
		[SerializeField]
		private bool autoAdjustScrollHeight = true;

		// Token: 0x04009127 RID: 37159
		[SerializeField]
		private float autoAdjustBottomMargin = 20f;

		// Token: 0x04009128 RID: 37160
		[SerializeField]
		private float baseWidth = 150f;

		// Token: 0x04009129 RID: 37161
		[SerializeField]
		private RectTransform showHideMenuTarget;

		// Token: 0x0400912A RID: 37162
		[SerializeField]
		private RectTransform menuOffsetTarget;

		// Token: 0x0400912B RID: 37163
		[SerializeField]
		private RectTransform scrollRect;

		// Token: 0x0400912C RID: 37164
		[SerializeField]
		private RectTransform itemRoot;

		// Token: 0x0400912D RID: 37165
		[SerializeField]
		private RectTransform viewport;

		// Token: 0x0400912E RID: 37166
		[SerializeField]
		private CButtonObsolete clearAllButton;

		// Token: 0x0400912F RID: 37167
		[SerializeField]
		private CanvasGroup[] extraFadeTargets;

		// Token: 0x02002660 RID: 9824
		// (Invoke) Token: 0x06011BB5 RID: 72629
		public delegate bool CanShowMenuChecker();

		// Token: 0x02002661 RID: 9825
		private enum EDropdownDirectionMode
		{
			// Token: 0x0400EA66 RID: 60006
			AlwaysDown
		}

		// Token: 0x02002662 RID: 9826
		public class OptionItem
		{
			// Token: 0x17001BBB RID: 7099
			// (get) Token: 0x06011BB8 RID: 72632 RVA: 0x00687CD2 File Offset: 0x00685ED2
			public bool IsSpecial
			{
				get
				{
					return this.Id < 0;
				}
			}

			// Token: 0x0400EA67 RID: 60007
			public Refers Refers;

			// Token: 0x0400EA68 RID: 60008
			public int Id;
		}
	}
}
