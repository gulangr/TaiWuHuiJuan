using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C98 RID: 3224
	public class FilterDropdown : MonoBehaviour
	{
		// Token: 0x1700111E RID: 4382
		// (get) Token: 0x0600A3F6 RID: 41974 RVA: 0x004CA278 File Offset: 0x004C8478
		public bool IsInFakeHideState
		{
			get
			{
				return this._isInFakeHideState;
			}
		}

		// Token: 0x0600A3F7 RID: 41975 RVA: 0x004CA280 File Offset: 0x004C8480
		private void Awake()
		{
			this.itemTemplate.SetActive(false);
			this.menuBarTemplate.SetActive(false);
			bool flag = this.fakeHideMark != null;
			if (flag)
			{
				CanvasGroup cg = this.fakeHideMark.GetComponent<CanvasGroup>();
				bool flag2 = cg == null;
				if (flag2)
				{
					cg = this.fakeHideMark.gameObject.AddComponent<CanvasGroup>();
				}
				cg.alpha = 0f;
			}
		}

		// Token: 0x0600A3F8 RID: 41976 RVA: 0x004CA2F0 File Offset: 0x004C84F0
		private void Update()
		{
			bool isMenuShow = this.IsMenuShow;
			if (isMenuShow)
			{
				this.CheckFakeHide();
				this.CheckHideMenu();
			}
		}

		// Token: 0x0600A3F9 RID: 41977 RVA: 0x004CA318 File Offset: 0x004C8518
		private void OnDisable()
		{
			this.ExitFakeHide();
		}

		// Token: 0x0600A3FA RID: 41978 RVA: 0x004CA324 File Offset: 0x004C8524
		public void Setup(FilterDropdownConfig config)
		{
			this.Setup(config, default(FilterDropdownContext));
		}

		// Token: 0x0600A3FB RID: 41979 RVA: 0x004CA344 File Offset: 0x004C8544
		public void Setup(FilterDropdownConfig config, FilterDropdownContext context)
		{
			this._config = config;
			this._context = context;
			this._selectedIndices.Clear();
			bool flag = config.DefaultSelectedIndices != null;
			if (flag)
			{
				this._selectedIndices.UnionWith(config.DefaultSelectedIndices);
			}
			this.SetupMenuBar();
			this.SetupMenu();
			this.HideMenu();
		}

		// Token: 0x1700111F RID: 4383
		// (get) Token: 0x0600A3FC RID: 41980 RVA: 0x004CA3A0 File Offset: 0x004C85A0
		public FilterDropdownConfig Config
		{
			get
			{
				return this._config;
			}
		}

		// Token: 0x17001120 RID: 4384
		// (get) Token: 0x0600A3FD RID: 41981 RVA: 0x004CA3A8 File Offset: 0x004C85A8
		public FilterDropdownContext Context
		{
			get
			{
				return this._context;
			}
		}

		// Token: 0x17001121 RID: 4385
		// (get) Token: 0x0600A3FE RID: 41982 RVA: 0x004CA3B0 File Offset: 0x004C85B0
		public bool HasDependency
		{
			get
			{
				return this._context.Dependency != null;
			}
		}

		// Token: 0x0600A3FF RID: 41983 RVA: 0x004CA3C2 File Offset: 0x004C85C2
		public IReadOnlyCollection<int> GetSelectedIndices()
		{
			return this._selectedIndices;
		}

		// Token: 0x0600A400 RID: 41984 RVA: 0x004CA3CC File Offset: 0x004C85CC
		public void UnSelectAll(bool triggerEvent = true)
		{
			this._selectedIndices.Clear();
			this.RefreshAllItems();
			this.RefreshMenuBar();
			if (triggerEvent)
			{
				Action onSelectionChanged = this.OnSelectionChanged;
				if (onSelectionChanged != null)
				{
					onSelectionChanged();
				}
			}
			this.UpdateSelfWidth(false);
		}

		// Token: 0x0600A401 RID: 41985 RVA: 0x004CA414 File Offset: 0x004C8614
		public void ShowMenu()
		{
			bool flag = this._canShowMenuChecker != null && !this._canShowMenuChecker();
			if (!flag)
			{
				this.showHideMenuTarget.gameObject.SetActive(true);
				this.RefreshMenuBarIconDirection();
				this.AdjustMenuHeight();
				bool activeInHierarchy = base.gameObject.activeInHierarchy;
				if (activeInHierarchy)
				{
					base.StartCoroutine(this.UpdateClearButtonVisibility());
				}
			}
		}

		// Token: 0x0600A402 RID: 41986 RVA: 0x004CA480 File Offset: 0x004C8680
		private void AdjustMenuHeight()
		{
			bool flag = this.menuArea == null;
			if (!flag)
			{
				VerticalLayoutGroup layout = this.itemRoot.GetComponent<VerticalLayoutGroup>();
				bool flag2 = layout == null;
				if (!flag2)
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)this.itemRoot);
					RectOffset padding = layout.padding;
					float spacing = layout.spacing;
					float itemHeight = this.itemTemplate.GetComponent<RectTransform>().rect.height;
					int actualItemCount = this.GetTotalItemCount();
					float actualContentHeight = (float)(padding.top + padding.bottom) + itemHeight * (float)actualItemCount + spacing * (float)Mathf.Max(0, actualItemCount - 1);
					float defaultTargetHeight = (float)(padding.top + padding.bottom) + itemHeight * 7.5f + spacing * (float)Mathf.Max(0, Mathf.FloorToInt(7.5f) - 1);
					float targetHeight = Mathf.Min(actualContentHeight, defaultTargetHeight);
					float maxAllowedHeight = this.CalculateMaxAllowedHeight();
					bool flag3 = targetHeight > maxAllowedHeight;
					if (flag3)
					{
						float singleRowHeight = itemHeight + spacing;
						float quantumHeight = singleRowHeight * 0.5f;
						float adjustedRows = Mathf.Floor(maxAllowedHeight / quantumHeight) * 0.5f;
						targetHeight = (float)(padding.top + padding.bottom) + itemHeight * adjustedRows + spacing * (float)Mathf.Max(0, Mathf.FloorToInt(adjustedRows) - 1);
						targetHeight = Mathf.Max(targetHeight, itemHeight + (float)padding.top + (float)padding.bottom);
					}
					this.menuArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
				}
			}
		}

		// Token: 0x0600A403 RID: 41987 RVA: 0x004CA5EC File Offset: 0x004C87EC
		private float CalculateMaxAllowedHeight()
		{
			bool flag = this.menuArea == null;
			float result;
			if (flag)
			{
				result = float.MaxValue;
			}
			else
			{
				CanvasScaler canvasScaler = base.GetComponentInParent<CanvasScaler>();
				bool flag2 = canvasScaler == null;
				if (flag2)
				{
					result = float.MaxValue;
				}
				else
				{
					RectTransform gameCanvas = canvasScaler.GetComponent<RectTransform>();
					float canvasHeight = gameCanvas.rect.height;
					float bottomLocalY = -canvasHeight * gameCanvas.pivot.y + 20f;
					Vector3 bottomWorld = gameCanvas.TransformPoint(new Vector3(0f, bottomLocalY, 0f));
					Vector3 topWorld = this.menuArea.position;
					float allowedWorldHeight = topWorld.y - bottomWorld.y;
					float localHeight = allowedWorldHeight / this.menuArea.lossyScale.y;
					result = Mathf.Max(0f, localHeight);
				}
			}
			return result;
		}

		// Token: 0x0600A404 RID: 41988 RVA: 0x004CA6C1 File Offset: 0x004C88C1
		private IEnumerator UpdateClearButtonVisibility()
		{
			yield return null;
			bool hasSelection = this._selectedIndices.Count > 0;
			this.UpdateSelfWidth(hasSelection);
			yield break;
		}

		// Token: 0x0600A405 RID: 41989 RVA: 0x004CA6D0 File Offset: 0x004C88D0
		public void HideMenu()
		{
			this.showHideMenuTarget.gameObject.SetActive(false);
			this.RefreshMenuBarIconDirection();
			this.ExitFakeHide();
		}

		// Token: 0x17001122 RID: 4386
		// (get) Token: 0x0600A406 RID: 41990 RVA: 0x004CA6F3 File Offset: 0x004C88F3
		public bool IsMenuShow
		{
			get
			{
				return this.showHideMenuTarget.gameObject.activeSelf;
			}
		}

		// Token: 0x17001123 RID: 4387
		// (get) Token: 0x0600A407 RID: 41991 RVA: 0x004CA705 File Offset: 0x004C8905
		public GameObject MenuBarInstance
		{
			get
			{
				return this._menuBarInstance;
			}
		}

		// Token: 0x0600A408 RID: 41992 RVA: 0x004CA710 File Offset: 0x004C8910
		public void SetMenuBarInteractable(bool interactable)
		{
			this._menuBarInteractable = interactable;
			bool flag = this.IsMenuShow && !interactable;
			if (flag)
			{
				this.HideMenu();
			}
		}

		// Token: 0x0600A409 RID: 41993 RVA: 0x004CA744 File Offset: 0x004C8944
		public void SetOptionInteractable(int optionIndex, bool interactable, string disabledTooltip)
		{
			FilterDropdown.OptionItemCache optionItem;
			bool flag = !this._optionItemDict.TryGetValue(optionIndex, out optionItem);
			if (!flag)
			{
				Refers refers = optionItem.Option.GetComponent<Refers>();
				bool flag2 = refers == null;
				if (!flag2)
				{
					optionItem.Option.SetInteractable(interactable);
				}
			}
		}

		// Token: 0x0600A40A RID: 41994 RVA: 0x004CA790 File Offset: 0x004C8990
		public void SetOption(int optionIndex)
		{
			bool flag = optionIndex < 0;
			if (flag)
			{
				this._selectedIndices.Clear();
				for (int i = 0; i < this._config.ItemConfigs.Count; i++)
				{
					this._selectedIndices.Add(i);
				}
			}
			else
			{
				bool flag2 = !this._config.IsMultiSelect;
				if (flag2)
				{
					this._selectedIndices.Clear();
				}
				this._selectedIndices.Add(optionIndex);
			}
			this.RefreshAllItems();
			this.RefreshMenuBar();
		}

		// Token: 0x0600A40B RID: 41995 RVA: 0x004CA81D File Offset: 0x004C8A1D
		public void SetupParentScrollView(RectTransform parentLayoutRect, ScrollRect parentScrollRect)
		{
		}

		// Token: 0x0600A40C RID: 41996 RVA: 0x004CA820 File Offset: 0x004C8A20
		public void SetupCanShowMenuChecker(Func<bool> canShowMenuChecker)
		{
			this._canShowMenuChecker = canShowMenuChecker;
		}

		// Token: 0x0600A40D RID: 41997 RVA: 0x004CA82C File Offset: 0x004C8A2C
		private void SetupMenuBar()
		{
			bool flag = !this._menuBarInstance;
			if (flag)
			{
				this._menuBarInstance = Object.Instantiate<GameObject>(this.menuBarTemplate, this.menuBarSlot);
			}
			this._menuBarInstance.SetActive(true);
			this.RefreshMenuBar();
			FilterMenuBar menuBar = this._menuBarInstance.GetComponent<FilterMenuBar>();
			bool flag2 = menuBar != null;
			if (flag2)
			{
				menuBar.OnPointerEnterEvent = new Action(this.OnMouseEnterMenuBar);
				menuBar.OnPointerExitEvent = new Action(this.OnMouseExitMenuBar);
			}
		}

		// Token: 0x0600A40E RID: 41998 RVA: 0x004CA8B4 File Offset: 0x004C8AB4
		private void OnMouseEnterMenuBar()
		{
			bool flag = !this._menuBarInteractable;
			if (!flag)
			{
				bool flag2 = !this.ShouldBlockShowMenuDueToOtherMenuFakeHide();
				if (flag2)
				{
					this.ShowMenu();
				}
				Action onMouseEnter = this.OnMouseEnter;
				if (onMouseEnter != null)
				{
					onMouseEnter();
				}
			}
		}

		// Token: 0x0600A40F RID: 41999 RVA: 0x004CA8FC File Offset: 0x004C8AFC
		private bool ShouldBlockShowMenuDueToOtherMenuFakeHide()
		{
			FilterDropdown[] allDropdowns = Object.FindObjectsOfType<FilterDropdown>();
			foreach (FilterDropdown dropdown in allDropdowns)
			{
				bool flag = dropdown == this;
				if (!flag)
				{
					bool flag2 = dropdown.IsMenuShow && dropdown.NeedFakeHide();
					if (flag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600A410 RID: 42000 RVA: 0x004CA95A File Offset: 0x004C8B5A
		private void OnMouseExitMenuBar()
		{
			Action onMouseExit = this.OnMouseExit;
			if (onMouseExit != null)
			{
				onMouseExit();
			}
		}

		// Token: 0x0600A411 RID: 42001 RVA: 0x004CA970 File Offset: 0x004C8B70
		private void RefreshMenuBar()
		{
			FilterMenuBar refers = this._menuBarInstance.GetComponent<FilterMenuBar>();
			bool flag = refers == null;
			if (!flag)
			{
				int selectedCount = this._selectedIndices.Count;
				if (!true)
				{
				}
				string text;
				if (selectedCount != 0)
				{
					if (selectedCount != 1)
					{
						text = string.Format("{0}({1})", this._config.MenuBarLabel.GetString(), selectedCount);
					}
					else
					{
						text = this.GetFirstSelectedText();
					}
				}
				else
				{
					text = this._config.MenuBarLabel.GetString();
				}
				if (!true)
				{
				}
				string labelText = text;
				refers.SetLabelText(labelText);
				this._menuBarInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			}
		}

		// Token: 0x0600A412 RID: 42002 RVA: 0x004CAA1C File Offset: 0x004C8C1C
		private string GetFirstSelectedText()
		{
			foreach (int index in this._selectedIndices)
			{
				bool flag = index >= 0 && index < this._config.ItemConfigs.Count;
				if (flag)
				{
					return this._config.ItemConfigs[index].Text.GetString();
				}
			}
			return this._config.MenuBarLabel.GetString();
		}

		// Token: 0x0600A413 RID: 42003 RVA: 0x004CAAC4 File Offset: 0x004C8CC4
		private void RefreshMenuBarIconDirection()
		{
			bool flag = !this._menuBarInstance;
			if (!flag)
			{
				FilterMenuBar refers = this._menuBarInstance.GetComponent<FilterMenuBar>();
				bool flag2 = refers == null;
				if (!flag2)
				{
					refers.SetStatusImage(this.IsMenuShow);
				}
			}
		}

		// Token: 0x0600A414 RID: 42004 RVA: 0x004CAB0C File Offset: 0x004C8D0C
		private void SetupMenu()
		{
			int itemCount = this.GetTotalItemCount();
			CommonUtils.PrepareEnoughChildren(this.itemRoot, this.itemTemplate, itemCount, null);
			this._optionItems.Clear();
			this._optionItemDict.Clear();
			for (int i = 0; i < itemCount; i++)
			{
				Transform item = this.itemRoot.GetChild(i);
				int dataIndex = this.GetDataIndex(i);
				FilterDropdown.OptionItemCache optionItem = new FilterDropdown.OptionItemCache
				{
					Option = item.GetComponent<FilterMenuOption>(),
					OriginIndex = dataIndex
				};
				this.RefreshItem(i, optionItem);
				this._optionItems.Add(optionItem);
				this._optionItemDict[dataIndex] = optionItem;
			}
			this.SetupClearButton();
		}

		// Token: 0x0600A415 RID: 42005 RVA: 0x004CABCC File Offset: 0x004C8DCC
		private void SetupClearButton()
		{
			bool flag = this.clearAllButton == null;
			if (!flag)
			{
				this.clearAllButton.ClearAndAddListener(new Action(this.OnClickClear));
			}
		}

		// Token: 0x0600A416 RID: 42006 RVA: 0x004CAC04 File Offset: 0x004C8E04
		private void OnClickClear()
		{
			this.UnSelectAll(true);
		}

		// Token: 0x0600A417 RID: 42007 RVA: 0x004CAC10 File Offset: 0x004C8E10
		private void UpdateSelfWidth(bool showClearButton)
		{
			LayoutElement layoutElement = base.GetComponent<LayoutElement>();
			bool flag = layoutElement == null;
			if (!flag)
			{
				float clearButtonWidth = this.clearAllButton.GetComponent<RectTransform>().rect.width;
				layoutElement.preferredWidth = 300f + (showClearButton ? clearButtonWidth : 0f);
				this.clearAllButton.gameObject.SetActive(showClearButton);
			}
		}

		// Token: 0x0600A418 RID: 42008 RVA: 0x004CAC78 File Offset: 0x004C8E78
		private int GetTotalItemCount()
		{
			List<FilterDropdownItemConfig> itemConfigs = this._config.ItemConfigs;
			return ((itemConfigs != null) ? itemConfigs.Count : 0) + 1;
		}

		// Token: 0x0600A419 RID: 42009 RVA: 0x004CACA4 File Offset: 0x004C8EA4
		private int GetDataIndex(int uiIndex)
		{
			return uiIndex - 1;
		}

		// Token: 0x0600A41A RID: 42010 RVA: 0x004CACBC File Offset: 0x004C8EBC
		private void RefreshAllItems()
		{
			for (int i = 0; i < this._optionItems.Count; i++)
			{
				FilterDropdown.OptionItemCache item = this._optionItems[i];
				bool isSpecial = item.IsSpecial;
				if (!isSpecial)
				{
					this.RefreshItem(i, item);
				}
			}
		}

		// Token: 0x0600A41B RID: 42011 RVA: 0x004CAD08 File Offset: 0x004C8F08
		private void RefreshItem(int uiIndex, FilterDropdown.OptionItemCache item)
		{
			this.RefreshItemDisplay(uiIndex, item);
			this.RefreshItemButton(uiIndex, item);
			this.RefreshItemCheckBox(uiIndex, item);
		}

		// Token: 0x0600A41C RID: 42012 RVA: 0x004CAD28 File Offset: 0x004C8F28
		private void RefreshItemDisplay(int uiIndex, FilterDropdown.OptionItemCache item)
		{
			bool flag = uiIndex == 0;
			if (flag)
			{
				string allString = LocalStringManager.Get(LanguageKey.LK_Common_All);
				item.Option.SetLabel(allString);
				item.Option.SetSelected(false);
			}
			else
			{
				int dataIndex = item.OriginIndex;
				string str = this._config.ItemConfigs[dataIndex].Text.GetString();
				item.Option.SetLabel(str);
				bool selected = this._selectedIndices.Contains(dataIndex);
				item.Option.SetSelected(selected);
			}
		}

		// Token: 0x0600A41D RID: 42013 RVA: 0x004CADB8 File Offset: 0x004C8FB8
		private void RefreshItemButton(int uiIndex, FilterDropdown.OptionItemCache item)
		{
			FilterMenuOption option = item.Option;
			bool flag = uiIndex == 0;
			if (flag)
			{
				option.BindButton(new Action(this.OnClickSelectAll));
			}
			else
			{
				int dataIndex = item.OriginIndex;
				option.BindButton(delegate
				{
					this.OnClickMenuItem(dataIndex);
				});
			}
		}

		// Token: 0x0600A41E RID: 42014 RVA: 0x004CAE18 File Offset: 0x004C9018
		private void RefreshItemCheckBox(int uiIndex, FilterDropdown.OptionItemCache item)
		{
			FilterMenuOption option = item.Option;
			bool visible = uiIndex > 0 && this._config.IsMultiSelect;
			option.SetCheckBoxActive(visible);
			bool flag = visible;
			if (flag)
			{
				int dataIndex = item.OriginIndex;
				option.BindCheckBoxButton(delegate
				{
					this.OnClickCheckBox(dataIndex);
				});
			}
		}

		// Token: 0x0600A41F RID: 42015 RVA: 0x004CAE7C File Offset: 0x004C907C
		private void OnClickSelectAll()
		{
			bool isMultiSelect = this._config.IsMultiSelect;
			if (isMultiSelect)
			{
				bool flag = this._selectedIndices.Count == this._config.ItemConfigs.Count;
				if (flag)
				{
					this._selectedIndices.Clear();
				}
				else
				{
					this._selectedIndices.Clear();
					for (int i = 0; i < this._config.ItemConfigs.Count; i++)
					{
						this._selectedIndices.Add(i);
					}
				}
			}
			else
			{
				this._selectedIndices.Clear();
			}
			this.RefreshAllItems();
			this.RefreshMenuBar();
			Action onSelectionChanged = this.OnSelectionChanged;
			if (onSelectionChanged != null)
			{
				onSelectionChanged();
			}
			this.UpdateSelfWidth(this._selectedIndices.Count > 0);
		}

		// Token: 0x0600A420 RID: 42016 RVA: 0x004CAF4C File Offset: 0x004C914C
		private void OnClickMenuItem(int dataIndex)
		{
			bool flag = this._selectedIndices.Contains(dataIndex);
			if (flag)
			{
				this._selectedIndices.Clear();
			}
			else
			{
				this._selectedIndices.Clear();
				this._selectedIndices.Add(dataIndex);
			}
			this.RefreshAllItems();
			this.RefreshMenuBar();
			Action onSelectionChanged = this.OnSelectionChanged;
			if (onSelectionChanged != null)
			{
				onSelectionChanged();
			}
			this.UpdateSelfWidth(this._selectedIndices.Count > 0);
		}

		// Token: 0x0600A421 RID: 42017 RVA: 0x004CAFCA File Offset: 0x004C91CA
		private void OnClickCheckBox(int dataIndex)
		{
			this.ToggleSelected(dataIndex);
			this.RefreshAllItems();
			this.RefreshMenuBar();
			Action onSelectionChanged = this.OnSelectionChanged;
			if (onSelectionChanged != null)
			{
				onSelectionChanged();
			}
			this.UpdateSelfWidth(this._selectedIndices.Count > 0);
		}

		// Token: 0x0600A422 RID: 42018 RVA: 0x004CB00C File Offset: 0x004C920C
		private void ToggleSelected(int index)
		{
			bool flag = !this._config.IsMultiSelect && !this._selectedIndices.Contains(index);
			if (flag)
			{
				this._selectedIndices.Clear();
			}
			bool flag2 = !this._selectedIndices.Add(index);
			if (flag2)
			{
				this._selectedIndices.Remove(index);
			}
		}

		// Token: 0x0600A423 RID: 42019 RVA: 0x004CB06C File Offset: 0x004C926C
		private void CheckFakeHide()
		{
			bool needFakeHide = this.NeedFakeHide();
			bool flag = needFakeHide && !this._isInFakeHideState;
			if (flag)
			{
				this.EnterFakeHide();
			}
			else
			{
				bool flag2 = !needFakeHide && this._isInFakeHideState;
				if (flag2)
				{
					this.ExitFakeHide();
				}
			}
		}

		// Token: 0x0600A424 RID: 42020 RVA: 0x004CB0B8 File Offset: 0x004C92B8
		private bool NeedFakeHide()
		{
			bool flag = this.fakeHideCheckRects == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (RectTransform rect in this.fakeHideCheckRects)
				{
					bool flag2 = rect == null || !rect.gameObject.activeSelf;
					if (!flag2)
					{
						bool flag3 = FilterDropdown.IsMouseInRect(rect);
						if (flag3)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600A425 RID: 42021 RVA: 0x004CB130 File Offset: 0x004C9330
		private void CheckHideMenu()
		{
			bool flag = this.exitCheckRects != null;
			if (flag)
			{
				foreach (RectTransform rect in this.exitCheckRects)
				{
					bool flag2 = rect == null || !rect.gameObject.activeSelf;
					if (!flag2)
					{
						bool flag3 = FilterDropdown.IsMouseInRect(rect);
						if (flag3)
						{
							return;
						}
					}
				}
			}
			this.HideMenu();
		}

		// Token: 0x0600A426 RID: 42022 RVA: 0x004CB1A0 File Offset: 0x004C93A0
		private void EnterFakeHide()
		{
			this._isInFakeHideState = true;
			this.DoFadeTransition(true);
		}

		// Token: 0x0600A427 RID: 42023 RVA: 0x004CB1B4 File Offset: 0x004C93B4
		private void ExitFakeHide()
		{
			bool flag = !this._isInFakeHideState;
			if (!flag)
			{
				this._isInFakeHideState = false;
				this.DoFadeTransition(false);
			}
		}

		// Token: 0x0600A428 RID: 42024 RVA: 0x004CB1E0 File Offset: 0x004C93E0
		private void DoFadeTransition(bool toFakeHide)
		{
			Sequence fakeHideTween = this._fakeHideTween;
			if (fakeHideTween != null)
			{
				fakeHideTween.Kill(true);
			}
			this._fakeHideTween = DOTween.Sequence();
			bool flag = this.fakeHideMark != null;
			if (flag)
			{
				CanvasGroup markCg = this.fakeHideMark.GetComponent<CanvasGroup>();
				bool flag2 = markCg != null;
				if (flag2)
				{
					markCg.alpha = (toFakeHide ? 0f : 0.4f);
					this._fakeHideTween.Join(markCg.DOFade(toFakeHide ? 0.4f : 0f, 0.2f));
				}
			}
			bool flag3 = this.menuContentCanvasGroup != null;
			if (flag3)
			{
				this.menuContentCanvasGroup.alpha = (toFakeHide ? 1f : 0f);
				this._fakeHideTween.Join(this.menuContentCanvasGroup.DOFade(toFakeHide ? 0f : 1f, 0.2f));
			}
		}

		// Token: 0x0600A429 RID: 42025 RVA: 0x004CB2CC File Offset: 0x004C94CC
		private static bool IsMouseInRect(RectTransform rect)
		{
			Vector2 localPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, UIManager.Instance.UiCamera, out localPos);
			return rect.rect.Contains(localPos);
		}

		// Token: 0x040081DC RID: 33244
		public const int AllOptionIndex = 0;

		// Token: 0x040081DD RID: 33245
		private const float DefaultVisibleRows = 7.5f;

		// Token: 0x040081DE RID: 33246
		private const float RowQuantum = 0.5f;

		// Token: 0x040081DF RID: 33247
		private const float BottomMargin = 20f;

		// Token: 0x040081E0 RID: 33248
		[NonSerialized]
		public Action OnSelectionChanged;

		// Token: 0x040081E1 RID: 33249
		[NonSerialized]
		public Action OnMouseEnter;

		// Token: 0x040081E2 RID: 33250
		[NonSerialized]
		public Action OnMouseExit;

		// Token: 0x040081E3 RID: 33251
		private FilterDropdownConfig _config;

		// Token: 0x040081E4 RID: 33252
		private FilterDropdownContext _context;

		// Token: 0x040081E5 RID: 33253
		private readonly HashSet<int> _selectedIndices = new HashSet<int>();

		// Token: 0x040081E6 RID: 33254
		private readonly List<FilterDropdown.OptionItemCache> _optionItems = new List<FilterDropdown.OptionItemCache>();

		// Token: 0x040081E7 RID: 33255
		private readonly Dictionary<int, FilterDropdown.OptionItemCache> _optionItemDict = new Dictionary<int, FilterDropdown.OptionItemCache>();

		// Token: 0x040081E8 RID: 33256
		private GameObject _menuBarInstance;

		// Token: 0x040081E9 RID: 33257
		private bool _menuBarInteractable = true;

		// Token: 0x040081EA RID: 33258
		private bool _isInFakeHideState;

		// Token: 0x040081EB RID: 33259
		private Sequence _fakeHideTween;

		// Token: 0x040081EC RID: 33260
		private const float FakeHideAlpha = 0.4f;

		// Token: 0x040081ED RID: 33261
		private const float FakeHideDuration = 0.2f;

		// Token: 0x040081EE RID: 33262
		private Func<bool> _canShowMenuChecker;

		// Token: 0x040081EF RID: 33263
		[Header("基础")]
		[SerializeField]
		private GameObject menuBarTemplate;

		// Token: 0x040081F0 RID: 33264
		[SerializeField]
		private GameObject itemTemplate;

		// Token: 0x040081F1 RID: 33265
		[SerializeField]
		private RectTransform menuBarSlot;

		// Token: 0x040081F2 RID: 33266
		[SerializeField]
		private RectTransform showHideMenuTarget;

		// Token: 0x040081F3 RID: 33267
		[SerializeField]
		private Transform itemRoot;

		// Token: 0x040081F4 RID: 33268
		[SerializeField]
		private RectTransform viewport;

		// Token: 0x040081F5 RID: 33269
		[SerializeField]
		private CScrollRect scrollRect;

		// Token: 0x040081F6 RID: 33270
		[SerializeField]
		private RectTransform menuArea;

		// Token: 0x040081F7 RID: 33271
		[Header("FakeHide 假隐藏")]
		[SerializeField]
		private RectTransform fakeHideMark;

		// Token: 0x040081F8 RID: 33272
		[SerializeField]
		private CanvasGroup menuContentCanvasGroup;

		// Token: 0x040081F9 RID: 33273
		[SerializeField]
		private RectTransform[] fakeHideCheckRects;

		// Token: 0x040081FA RID: 33274
		[SerializeField]
		private RectTransform[] exitCheckRects;

		// Token: 0x040081FB RID: 33275
		[Header("清除按钮")]
		[SerializeField]
		private CButton clearAllButton;

		// Token: 0x020023F2 RID: 9202
		private class OptionItemCache
		{
			// Token: 0x17001A82 RID: 6786
			// (get) Token: 0x0601051C RID: 66844 RVA: 0x0065DE39 File Offset: 0x0065C039
			public bool IsSpecial
			{
				get
				{
					return this.OriginIndex < 0;
				}
			}

			// Token: 0x0400E107 RID: 57607
			public FilterMenuOption Option;

			// Token: 0x0400E108 RID: 57608
			public int OriginIndex;
		}
	}
}
