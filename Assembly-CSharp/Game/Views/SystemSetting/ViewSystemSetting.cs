using System;
using System.Collections.Generic;
using System.IO;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Combat;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000784 RID: 1924
	public class ViewSystemSetting : UIBase
	{
		// Token: 0x06005D19 RID: 23833 RVA: 0x002AC82C File Offset: 0x002AAA2C
		private void InitPrefabMap()
		{
			this._prefabMap.Clear();
			bool flag = this.boolSettingPrefab != null;
			if (flag)
			{
				this._prefabMap[SettingUIType.Toggle] = this.boolSettingPrefab;
			}
			bool flag2 = this.intSettingPrefab != null;
			if (flag2)
			{
				this._prefabMap[SettingUIType.SliderInt] = this.intSettingPrefab;
			}
			bool flag3 = this.floatSettingPrefab != null;
			if (flag3)
			{
				this._prefabMap[SettingUIType.SliderFloat] = this.floatSettingPrefab;
			}
			bool flag4 = this.enumSettingPrefab != null;
			if (flag4)
			{
				this._prefabMap[SettingUIType.Dropdown] = this.enumSettingPrefab;
			}
			bool flag5 = this.switchButtonSettingPrefab != null;
			if (flag5)
			{
				this._prefabMap[SettingUIType.SwitchButton] = this.switchButtonSettingPrefab;
			}
			bool flag6 = this.multiToggleGroupSettingPrefab != null;
			if (flag6)
			{
				this._prefabMap[SettingUIType.MultiToggleGroup] = this.multiToggleGroupSettingPrefab;
			}
			bool flag7 = this.teammateCommandSettingPrefab != null;
			if (flag7)
			{
				this._prefabMap[SettingUIType.TeammateCommand] = this.teammateCommandSettingPrefab;
			}
			bool flag8 = this.hotKeySettingPrefab != null;
			if (flag8)
			{
				this._prefabMap[SettingUIType.HotKey] = this.hotKeySettingPrefab;
			}
		}

		// Token: 0x06005D1A RID: 23834 RVA: 0x002AC968 File Offset: 0x002AAB68
		private void SetAiOptions()
		{
			this._aiOptions = new AiOptions();
			this._aiOptionsSaveFilePath = ViewSystemSetting.GetAiOptionsSaveFilePath();
			bool flag = File.Exists(this._aiOptionsSaveFilePath);
			if (flag)
			{
				try
				{
					GameData.Serializer.CommonObjectSerializer.Deserialize<AiOptions>(File.ReadAllText(this._aiOptionsSaveFilePath), out this._aiOptions, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
				}
				catch (Exception e)
				{
					Debug.LogError(string.Format("[ViewSystemSetting] Failed to load AiOptions: {0}", e));
					this._aiOptions.Reset();
				}
			}
			else
			{
				this._aiOptions.Reset();
			}
			SystemSettingMapping.AiOptionsRef = this._aiOptions;
		}

		// Token: 0x06005D1B RID: 23835 RVA: 0x002ACA08 File Offset: 0x002AAC08
		private void OnEnable()
		{
			GEvent.Add(EEvents.OnFullScreenChange, new GEvent.Callback(this.OnFullScreenChangeEvent));
			GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
		}

		// Token: 0x06005D1C RID: 23836 RVA: 0x002ACA40 File Offset: 0x002AAC40
		private void OnDisable()
		{
			GEvent.Remove(EEvents.OnFullScreenChange, new GEvent.Callback(this.OnFullScreenChangeEvent));
			GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
			this.SaveAiOptions();
			SingletonObject.getInstance<GlobalSettings>().SaveSettings();
			bool flag = this._currentCategoryIndex >= 0;
			if (flag)
			{
				ViewSystemSetting._lastCategoryIndex = this._currentCategoryIndex;
				bool flag2 = this.settingScrollRect != null && this.settingScrollRect.Content != null;
				if (flag2)
				{
					ViewSystemSetting.CategoryScrollPositions[this._currentCategoryIndex] = this.settingScrollRect.Content.anchoredPosition;
				}
			}
		}

		// Token: 0x06005D1D RID: 23837 RVA: 0x002ACAFB File Offset: 0x002AACFB
		private void OnLanguageChange(ArgumentBox argBox)
		{
			ViewSystemSetting._lastCategoryIndex = 0;
			ViewSystemSetting.CategoryScrollPositions.Clear();
		}

		// Token: 0x06005D1E RID: 23838 RVA: 0x002ACB10 File Offset: 0x002AAD10
		private void OnFullScreenChangeEvent(ArgumentBox argBox)
		{
			bool flag = this._currentCategoryIndex != 1;
			if (!flag)
			{
				List<SettingGroupItem> groups;
				bool flag2 = this._categoryGroups.TryGetValue(this._currentCategoryIndex, out groups);
				if (flag2)
				{
					for (int i = 0; i < groups.Count; i++)
					{
						bool flag3 = groups[i].SubCategory == ESettingSubCategory.Video;
						if (flag3)
						{
							groups[i].RefreshItems();
							break;
						}
					}
				}
			}
		}

		// Token: 0x06005D1F RID: 23839 RVA: 0x002ACB88 File Offset: 0x002AAD88
		private void SaveAiOptions()
		{
			bool flag = this._aiOptions == null || string.IsNullOrEmpty(this._aiOptionsSaveFilePath);
			if (!flag)
			{
				try
				{
					string marshalData;
					GameData.Serializer.CommonObjectSerializer.Serialize<AiOptions>(this._aiOptions, out marshalData, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
					File.WriteAllText(this._aiOptionsSaveFilePath, marshalData);
				}
				catch (Exception e)
				{
					Debug.LogError(string.Format("[ViewSystemSetting] Failed to save AiOptions: {0}", e));
				}
			}
		}

		// Token: 0x06005D20 RID: 23840 RVA: 0x002ACBFC File Offset: 0x002AADFC
		private static string GetAiOptionsSaveFilePath()
		{
			string archiveDir = GameApp.GetArchiveDirPath();
			bool flag = !Directory.Exists(archiveDir);
			if (flag)
			{
				Directory.CreateDirectory(archiveDir);
			}
			return Path.Combine(archiveDir, "CombatAiSetting.lua");
		}

		// Token: 0x06005D21 RID: 23841 RVA: 0x002ACC34 File Offset: 0x002AAE34
		private void InitHotKeySettingItems()
		{
			this.AddKitToGroup(ESettingSubCategory.HotKeyView, CommandKitBase.CommonCommandKit);
			this.AddKitToGroup(ESettingSubCategory.HotKeyView, CommandKitBase.TipsCommandKit);
			this.AddKitToGroup(ESettingSubCategory.HotKeyView, CommandKitBase.TabSwitchCommandKit);
			this.AddKitToGroup(ESettingSubCategory.HotKeyView, CommandKitBase.EventWindowKit);
			this.AddKitToGroup(ESettingSubCategory.HotKeySence, CommandKitBase.MainInterfaceCommandKit);
			this.AddKitToGroup(ESettingSubCategory.HotKeySence, CommandKitBase.MainInterfaceFunctionCommandKit);
			this.AddKitToGroup(ESettingSubCategory.HotKeySence, CommandKitBase.MapCommandKit);
			this.AddKitToGroup(ESettingSubCategory.HotKeySence, CommandKitBase.CharacterMenuCommandKit);
			this.AddKitToGroup(ESettingSubCategory.HotKeyCombat, CommandKitBase.CombatCommandKit);
			this.AddKitToGroup(ESettingSubCategory.HotKeyCombat, CommandKitBase.CombatBehaviorCommandKit);
		}

		// Token: 0x06005D22 RID: 23842 RVA: 0x002ACCD0 File Offset: 0x002AAED0
		private void AddKitToGroup(ESettingSubCategory subCategory, CommandKitBase kit)
		{
			bool flag = kit == null || kit.GroupCommand == null;
			if (!flag)
			{
				bool flag2 = !kit.ShowInSettings;
				if (!flag2)
				{
					List<ISettingItemInfo> list;
					bool flag3 = !this._subCategoryInfos.TryGetValue(subCategory, out list);
					if (flag3)
					{
						list = new List<ISettingItemInfo>();
						this._subCategoryInfos.Add(subCategory, list);
					}
					for (int i = 0; i < kit.GroupCommand.Length; i++)
					{
						HotKeyCommand cmd = kit.GroupCommand[i];
						bool flag4 = cmd == null;
						if (!flag4)
						{
							bool flag5 = cmd == CommonCommandKit.OpenGMPanel && !GameApp.Instance.EnableGMPanel;
							if (!flag5)
							{
								HotKeySettingItemInfo info = new HotKeySettingItemInfo(cmd, kit.Id, subCategory, i);
								list.Add(info);
								HotKeyService.RegisterCommand(subCategory, cmd);
							}
						}
					}
				}
			}
		}

		// Token: 0x06005D23 RID: 23843 RVA: 0x002ACDB0 File Offset: 0x002AAFB0
		private void SetSettingItems()
		{
			for (int i = 0; i < ViewSystemSetting.Categories.Length; i++)
			{
				RectTransform holder = this.settingScrollContent.GetChild(i) as RectTransform;
				this._settingItemHolders.Add(holder);
				holder.gameObject.SetActive(false);
				holder.gameObject.name = string.Format("Holder_{0}", ViewSystemSetting.Categories[i].Category);
				SettingCategoryData categoryData = ViewSystemSetting.Categories[i];
				List<SettingGroupItem> groupItems;
				bool flag = !this._categoryGroups.TryGetValue(i, out groupItems);
				if (flag)
				{
					groupItems = new List<SettingGroupItem>();
				}
				groupItems.Clear();
				CommonUtils.PrepareEnoughChildren(holder, holder.GetChild(0).gameObject, categoryData.SubCategories.Length, null);
				for (int j = 0; j < categoryData.SubCategories.Length; j++)
				{
					SubCategoryInfo subCategoryInfo = categoryData.SubCategories[j];
					Transform groupTrans = holder.GetChild(j);
					SettingGroupItem settingGroupItem = groupTrans.GetComponent<SettingGroupItem>();
					List<ISettingItemInfo> infos;
					bool hasContent = this._subCategoryInfos.TryGetValue(subCategoryInfo.SubCategory, out infos) && infos != null && infos.Count > 0;
					groupTrans.gameObject.SetActive(hasContent);
					bool flag2 = !hasContent;
					if (!flag2)
					{
						settingGroupItem.Set(subCategoryInfo.SubCategory, subCategoryInfo.Title, infos, this._prefabMap);
						settingGroupItem.SetGroupTitle(HotKeyService.IsHotKeyGroupFirst(subCategoryInfo.SubCategory), HotKeyService.GetHotKeyGroupName(subCategoryInfo.SubCategory));
						groupItems.Add(settingGroupItem);
					}
				}
				this._categoryGroups[i] = groupItems;
			}
		}

		// Token: 0x06005D24 RID: 23844 RVA: 0x002ACF64 File Offset: 0x002AB164
		private void OnCategoryChanged(int newIndex, int oldIndex)
		{
			bool flag = this._isSyncing || newIndex < 0 || newIndex >= ViewSystemSetting.Categories.Length;
			if (!flag)
			{
				bool flag2 = oldIndex != newIndex && oldIndex >= 0 && oldIndex < ViewSystemSetting.Categories.Length && this.settingScrollRect != null && this.settingScrollRect.Content != null;
				if (flag2)
				{
					ViewSystemSetting.CategoryScrollPositions[oldIndex] = this.settingScrollRect.Content.anchoredPosition;
				}
				this._currentCategoryIndex = newIndex;
				this.RebuildSubCategoryToggles(ViewSystemSetting.Categories[newIndex]);
				for (int i = 0; i < this._settingItemHolders.Count; i++)
				{
					bool flag3 = i == this._currentCategoryIndex;
					if (flag3)
					{
						LayoutRebuilder.ForceRebuildLayoutImmediate(this._settingItemHolders[i]);
					}
					this._settingItemHolders[i].gameObject.SetActive(i == this._currentCategoryIndex);
				}
				this.titleRoot.gameObject.SetActive(false);
				Vector2 savedPos;
				bool hasSavedPosition = ViewSystemSetting.CategoryScrollPositions.TryGetValue(newIndex, out savedPos);
				base.DelayFrameCall(delegate
				{
					this.CalculateSubCategoryPositions();
					bool hasSavedPosition = hasSavedPosition;
					if (hasSavedPosition)
					{
						bool flag4 = this.settingScrollRect != null && this.settingScrollRect.Content != null;
						if (flag4)
						{
							float scrollMaxY = Mathf.Max(0f, this.settingScrollRect.Content.rect.height - this.settingScrollRect.Viewport.rect.height);
							Vector2 clampedPos = new Vector2(0f, Mathf.Clamp(savedPos.y, 0f, scrollMaxY));
							this.settingScrollRect.ScrollTo(clampedPos, 0f);
						}
					}
					else
					{
						bool flag5 = this.currentSubCategoryToggleGroup != null && ViewSystemSetting.Categories[newIndex].SubCategories.Length != 0;
						if (flag5)
						{
							this.currentSubCategoryToggleGroup.Set(0, false);
						}
					}
				}, 1U);
				this.UpdateCategoryToggleStyles(newIndex);
				this.HideRightTips();
			}
		}

		// Token: 0x06005D25 RID: 23845 RVA: 0x002AD0E8 File Offset: 0x002AB2E8
		private void UpdateCategoryToggleStyles(int selectedIndex)
		{
			bool flag = this.categoryToggleGroup == null;
			if (!flag)
			{
				List<CToggle> toggles = this.categoryToggleGroup.GetAll();
				for (int i = 0; i < toggles.Count; i++)
				{
					CToggle toggle = toggles[i];
					bool flag2 = toggle == null || toggle.targetGraphic == null;
					if (!flag2)
					{
						CImage graphicImage = toggle.targetGraphic as CImage;
						bool flag3 = graphicImage == null;
						if (!flag3)
						{
							int stateIndex = (i == selectedIndex) ? 2 : 0;
							string spritePath = string.Format("ui9_icon_lowerpopup_systemsettings_icon_tab_{0}_{1}", i, stateIndex);
							graphicImage.SetSprite(spritePath, false, null);
							bool flag4 = this.categoryToggleTitleColors != null && this.categoryToggleTitleColors.Length > 1;
							if (flag4)
							{
								ToggleStyle toggleStyle = toggle.GetComponent<ToggleStyle>();
								bool flag5 = toggleStyle != null && toggleStyle.Label != null;
								if (flag5)
								{
									int colorIndex = (i == selectedIndex) ? 1 : 0;
									toggleStyle.Label.color = this.categoryToggleTitleColors[colorIndex];
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06005D26 RID: 23846 RVA: 0x002AD21C File Offset: 0x002AB41C
		private void RebuildSubCategoryToggles(SettingCategoryData categoryData)
		{
			SubCategoryInfo[] subCategories = categoryData.SubCategories;
			CommonUtils.PrepareEnoughChildren(this.currentSubCategoryToggleGroup.transform, this.subCategoryToggleTemplate.gameObject, subCategories.Length, null);
			for (int i = 0; i < subCategories.Length; i++)
			{
				GameObject toggleGo = this.currentSubCategoryToggleGroup.transform.GetChild(i).gameObject;
				toggleGo.name = string.Format("Toggle_{0}", subCategories[i].SubCategory);
				List<ISettingItemInfo> infos;
				bool hasContent = this._subCategoryInfos.TryGetValue(subCategories[i].SubCategory, out infos) && infos != null && infos.Count > 0;
				toggleGo.SetActive(hasContent);
				bool flag = hasContent;
				if (flag)
				{
					toggleGo.GetComponent<ToggleStyle>().SetLabelText(subCategories[i].Title.Tr());
					toggleGo.GetComponent<SubCategoryItem>().Set(HotKeyService.IsHotKeyGroupFirst(subCategories[i].SubCategory), HotKeyService.GetHotKeyGroupName(subCategories[i].SubCategory));
				}
			}
			this.currentSubCategoryToggleGroup.Clear();
			this.currentSubCategoryToggleGroup.AddAllChildToggles();
			this.currentSubCategoryToggleGroup.Init(-1);
		}

		// Token: 0x06005D27 RID: 23847 RVA: 0x002AD34C File Offset: 0x002AB54C
		private void CalculateSubCategoryPositions()
		{
			this._subCategoryPositions.Clear();
			List<SettingGroupItem> groups;
			bool flag = !this._categoryGroups.TryGetValue(this._currentCategoryIndex, out groups);
			if (!flag)
			{
				float accumulatedHeight = 0f;
				for (int i = 0; i < groups.Count; i++)
				{
					this._subCategoryPositions[i] = accumulatedHeight;
					accumulatedHeight += (groups[i].transform as RectTransform).sizeDelta.y + 44f;
				}
			}
		}

		// Token: 0x06005D28 RID: 23848 RVA: 0x002AD3D4 File Offset: 0x002AB5D4
		private void OnSubCategoryChanged(int newIndex, int oldIndex)
		{
			bool isSyncing = this._isSyncing;
			if (!isSyncing)
			{
				SettingCategoryData categoryData = ViewSystemSetting.Categories[this._currentCategoryIndex];
				bool flag = newIndex < 0 || newIndex >= categoryData.SubCategories.Length;
				if (!flag)
				{
					this._currentSubCategoryIndex = newIndex;
					this.ScrollToSubCategory(newIndex);
				}
			}
		}

		// Token: 0x06005D29 RID: 23849 RVA: 0x002AD428 File Offset: 0x002AB628
		private void ScrollToSubCategory(int subCategoryIndex)
		{
			float position;
			bool flag = !this._subCategoryPositions.TryGetValue(subCategoryIndex, out position);
			if (!flag)
			{
				this._isSyncing = true;
				Vector2 targetPos = new Vector2(0f, position);
				this.settingScrollRect.ScrollTo(targetPos, 0.2f);
				this._isSyncing = false;
			}
		}

		// Token: 0x06005D2A RID: 23850 RVA: 0x002AD47C File Offset: 0x002AB67C
		private void OnScrollChanged()
		{
			bool flag = this._isSyncing || this.currentSubCategoryToggleGroup == null || this._currentCategoryIndex < 0 || this.settingScrollRect == null || this.settingScrollRect.Content == null;
			if (!flag)
			{
				SettingCategoryData categoryData = ViewSystemSetting.Categories[this._currentCategoryIndex];
				float scrollY = this.settingScrollRect.Content.anchoredPosition.y + 5f;
				int targetIndex = 0;
				for (int i = 0; i < categoryData.SubCategories.Length; i++)
				{
					float position;
					bool flag2 = this._subCategoryPositions.TryGetValue(i, out position);
					if (flag2)
					{
						bool flag3 = position <= scrollY;
						if (!flag3)
						{
							break;
						}
						targetIndex = i;
					}
				}
				this.titleRoot.gameObject.SetActive(scrollY > 20f);
				this.titleRoot.GetChild(0).GetComponent<TextMeshProUGUI>().text = categoryData.SubCategories[targetIndex].Title.Tr();
				ESettingSubCategory subCategory = categoryData.SubCategories[targetIndex].SubCategory;
				bool canReset = ViewSystemSetting.CanResetCategory(subCategory);
				this.titleRoot.GetChild(0).GetChild(0).gameObject.SetActive(canReset);
				bool flag4 = canReset && this._currentSubCategoryIndex != targetIndex;
				if (flag4)
				{
					this._currentResetSubCategory = subCategory;
					this.titleRoot.GetChild(0).GetComponentInChildren<CButton>().ClearAndAddListener(delegate
					{
						this.OnResetButtonClick(categoryData.SubCategories[targetIndex].Title.Tr());
					});
				}
				this._isSyncing = true;
				this.currentSubCategoryToggleGroup.SetWithoutNotify(targetIndex);
				this._currentSubCategoryIndex = targetIndex;
				this._isSyncing = false;
			}
		}

		// Token: 0x06005D2B RID: 23851 RVA: 0x002AD66E File Offset: 0x002AB86E
		private void OnResetButtonClick(string subCategoryTitle)
		{
			ViewSystemSetting.ResetCategory(this._currentResetSubCategory, true, subCategoryTitle, delegate
			{
				this._categoryGroups[this._currentCategoryIndex][this._currentSubCategoryIndex].RefreshItems();
			});
		}

		// Token: 0x06005D2C RID: 23852 RVA: 0x002AD68C File Offset: 0x002AB88C
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = !this._inited;
			if (flag)
			{
				if (this._subCategoryInfos == null)
				{
					this._subCategoryInfos = SettingScanner.ScanSettings(ViewSystemSetting.Mapping);
				}
				this.InitHotKeySettingItems();
				this.InitPrefabMap();
				this.titleRoot.gameObject.SetActive(false);
				this.categoryToggleGroup.Init(-1);
				ToggleGroupHotkeyController.Set(this.Element, this.categoryToggleGroup, 0, null);
				this.categoryToggleGroup.OnActiveIndexChange += this.OnCategoryChanged;
				bool flag2 = this.settingScrollRect != null;
				if (flag2)
				{
					this.settingScrollRect.OnScrollEvent += this.OnScrollChanged;
				}
				this.currentSubCategoryToggleGroup.OnActiveIndexChange += this.OnSubCategoryChanged;
				this._inited = true;
			}
			this.Refresh();
			UIElement element = this.Element;
			element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(delegate()
			{
				this.categoryToggleGroup.Set(ViewSystemSetting._lastCategoryIndex, true);
			}));
		}

		// Token: 0x06005D2D RID: 23853 RVA: 0x002AD794 File Offset: 0x002AB994
		public void Refresh()
		{
			List<RectTransform> settingItemHolders = this._settingItemHolders;
			if (settingItemHolders != null)
			{
				settingItemHolders.Clear();
			}
			Dictionary<int, List<SettingGroupItem>> categoryGroups = this._categoryGroups;
			if (categoryGroups != null)
			{
				categoryGroups.Clear();
			}
			this.SetAiOptions();
			this.SetSettingItems();
			bool isShowing = this.Element.IsShowing;
			if (isShowing)
			{
				this.categoryToggleGroup.Set(ViewSystemSetting._lastCategoryIndex, true);
			}
		}

		// Token: 0x06005D2E RID: 23854 RVA: 0x002AD7F8 File Offset: 0x002AB9F8
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (!(a == "CloseBtn"))
			{
				if (a == "BtnReset")
				{
					this.ResetSetting();
				}
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x06005D2F RID: 23855 RVA: 0x002AD840 File Offset: 0x002ABA40
		public override void QuickHide()
		{
			bool activeSelf = this.hotKeyEditPanel.gameObject.activeSelf;
			if (activeSelf)
			{
				bool flag = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
				if (flag)
				{
					this.hotKeyEditPanel.OnCancelClick();
				}
			}
			else
			{
				base.QuickHide();
			}
		}

		// Token: 0x06005D30 RID: 23856 RVA: 0x002AD894 File Offset: 0x002ABA94
		private void ResetSetting()
		{
			DialogCmd dialogCmd = new DialogCmd
			{
				Type = 1,
				Title = LanguageKey.LK_SystemSetting_ResetDefaultValue.Tr(),
				Content = LanguageKey.LK_SystemSetting_ResetDefaultValue_Content.TrFormat(this.categoryToggleGroup.Get(this._currentCategoryIndex).GetComponent<ToggleStyle>().Label.text),
				Yes = delegate()
				{
					foreach (SettingGroupItem item in this._categoryGroups[this._currentCategoryIndex])
					{
						ViewSystemSetting.ResetCategory(item.SubCategory, false, null, null);
						item.RefreshItems();
					}
				}
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06005D31 RID: 23857 RVA: 0x002AD92C File Offset: 0x002ABB2C
		public static bool IsAiOptionsCategory(ESettingSubCategory subCategory)
		{
			return subCategory >= ESettingSubCategory.AutoAttack && subCategory <= ESettingSubCategory.AutoAssist;
		}

		// Token: 0x06005D32 RID: 23858 RVA: 0x002AD950 File Offset: 0x002ABB50
		public static bool IsHotKeyCategory(ESettingSubCategory subCategory)
		{
			return subCategory >= ESettingSubCategory.HotKeyView && subCategory <= ESettingSubCategory.HotKeyCombat;
		}

		// Token: 0x06005D33 RID: 23859 RVA: 0x002AD974 File Offset: 0x002ABB74
		public static bool CanResetCategory(ESettingSubCategory subCategory)
		{
			bool flag = ViewSystemSetting.IsAiOptionsCategory(subCategory) || ViewSystemSetting.IsHotKeyCategory(subCategory) || subCategory == ESettingSubCategory.Encyclopedia || subCategory == ESettingSubCategory.Localization || subCategory == ESettingSubCategory.RegionStory;
			return flag || SingletonObject.getInstance<GlobalSettings>().CanResetCategory(subCategory);
		}

		// Token: 0x06005D34 RID: 23860 RVA: 0x002AD9B8 File Offset: 0x002ABBB8
		public static void ResetCategory(ESettingSubCategory subCategory, bool isShowConfirm, string categoryTitle = null, Action onConfirm = null)
		{
			ViewSystemSetting.<>c__DisplayClass64_0 CS$<>8__locals1 = new ViewSystemSetting.<>c__DisplayClass64_0();
			CS$<>8__locals1.subCategory = subCategory;
			CS$<>8__locals1.onConfirm = onConfirm;
			bool flag = !isShowConfirm;
			if (flag)
			{
				CS$<>8__locals1.<ResetCategory>g__ConfirmReset|0();
			}
			else
			{
				DialogCmd dialogCmd = new DialogCmd
				{
					Type = 1,
					Title = LanguageKey.LK_SystemSetting_ResetDefaultValue.Tr(),
					Content = LanguageKey.LK_SystemSetting_ResetDefaultValue_Content.TrFormat(categoryTitle ?? string.Empty),
					Yes = new Action(CS$<>8__locals1.<ResetCategory>g__ConfirmReset|0)
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x06005D35 RID: 23861 RVA: 0x002ADA64 File Offset: 0x002ABC64
		public static void ResetAiOptionsCategory(ESettingSubCategory subCategory)
		{
			AiOptions aiOptions = SystemSettingMapping.AiOptionsRef;
			bool flag = aiOptions == null;
			if (!flag)
			{
				switch (subCategory)
				{
				case ESettingSubCategory.AutoAttack:
					aiOptions.AutoAttack = true;
					aiOptions.AutoChangeWeapon = false;
					aiOptions.AutoChangeTrick = true;
					aiOptions.AutoUnlock = false;
					aiOptions.SkipRawCreate = false;
					break;
				case ESettingSubCategory.AutoMove:
					aiOptions.AutoMove = true;
					aiOptions.TryDodge = true;
					aiOptions.SaveMoveTarget = false;
					break;
				case ESettingSubCategory.AutoAction:
				{
					bool flag2 = aiOptions.AutoUseOtherAction != null;
					if (flag2)
					{
						for (int i = 0; i < aiOptions.AutoUseOtherAction.Length; i++)
						{
							aiOptions.AutoUseOtherAction[i] = true;
						}
						bool[] autoUseOtherAction = aiOptions.AutoUseOtherAction;
						autoUseOtherAction[autoUseOtherAction.Length - 1] = false;
					}
					break;
				}
				case ESettingSubCategory.AutoCast:
				{
					bool flag3 = aiOptions.AutoCastSkill != null;
					if (flag3)
					{
						for (int j = 0; j < aiOptions.AutoCastSkill.Length; j++)
						{
							aiOptions.AutoCastSkill[j] = true;
						}
					}
					aiOptions.AutoInterrupt = false;
					aiOptions.AutoClearAgile = false;
					aiOptions.AutoClearDefense = false;
					aiOptions.AutoCostNeiliAllocation = false;
					aiOptions.AutoCostTrick = false;
					break;
				}
				case ESettingSubCategory.AutoAssist:
				{
					bool flag4 = aiOptions.AutoUseTeammateCommand != null;
					if (flag4)
					{
						for (int k = 0; k < aiOptions.AutoUseTeammateCommand.Length; k++)
						{
							aiOptions.AutoUseTeammateCommand[k] = true;
						}
					}
					break;
				}
				}
			}
		}

		// Token: 0x06005D36 RID: 23862 RVA: 0x002ADBD0 File Offset: 0x002ABDD0
		public static void RefreshRightTips(LanguageKey title = LanguageKey.Invalid, LanguageKey content = LanguageKey.Invalid, SettingItemBaseAttribute attribute = null)
		{
			ViewSystemSetting systemSetting = UIElement.SystemSetting.UiBaseAs<ViewSystemSetting>();
			bool isUpdate = title != LanguageKey.Invalid && content != LanguageKey.Invalid;
			bool flag = !isUpdate;
			if (!flag)
			{
				systemSetting.rightTipsRoot.gameObject.SetActive(true);
				systemSetting.rightTipsTitleText.text = title.Tr();
				bool flag2 = attribute.Key == ESettingKey.SectStory;
				if (flag2)
				{
					systemSetting.rightTipsContentText.text = content.TrFormat(Organization.Instance[attribute.Order].Name);
				}
				else
				{
					systemSetting.rightTipsContentText.text = content.Tr();
				}
			}
		}

		// Token: 0x06005D37 RID: 23863 RVA: 0x002ADC6F File Offset: 0x002ABE6F
		private void HideRightTips()
		{
			this.rightTipsRoot.gameObject.SetActive(false);
		}

		// Token: 0x06005D38 RID: 23864 RVA: 0x002ADC84 File Offset: 0x002ABE84
		public void ShowHotKeyEditPanel(HotKeyCommand hotKeyCommand, byte kitId, ESettingSubCategory subCategory, bool isMouseKey, Action onEditComplete = null, RectTransform triggerButton = null)
		{
			bool flag = hotKeyCommand == null;
			if (!flag)
			{
				bool flag2 = this.hotKeyEditPanel != null;
				if (flag2)
				{
					this.hotKeyEditPanel.Set(hotKeyCommand, kitId, subCategory, isMouseKey, onEditComplete);
					UIManager.Instance.MaskComponent(this.hotKeyEditPanel.transform as RectTransform);
					bool flag3 = triggerButton != null;
					if (flag3)
					{
						this.PositionPanelAtButton(this.hotKeyEditPanel.transform as RectTransform, triggerButton);
					}
				}
			}
		}

		// Token: 0x06005D39 RID: 23865 RVA: 0x002ADD04 File Offset: 0x002ABF04
		private void PositionPanelAtButton(RectTransform panel, RectTransform button)
		{
			bool flag = panel == null || button == null;
			if (!flag)
			{
				Vector3[] buttonWorldCorners = new Vector3[4];
				button.GetWorldCorners(buttonWorldCorners);
				Vector3 buttonTopLeftWorld = buttonWorldCorners[1];
				Camera uiCamera = UIManager.Instance.UiCamera;
				Vector3 screenPos = uiCamera.WorldToScreenPoint(buttonTopLeftWorld);
				Vector3 worldPos = uiCamera.ScreenToWorldPoint(screenPos);
				panel.position = worldPos;
				panel.anchoredPosition += new Vector2(-79f, 133f);
				panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, Math.Max(panel.anchoredPosition.y, -1000f));
			}
		}

		// Token: 0x06005D3A RID: 23866 RVA: 0x002ADDBC File Offset: 0x002ABFBC
		public void EndHotKeyEdit()
		{
			bool flag = this.hotKeyEditPanel != null;
			if (flag)
			{
				UIManager.Instance.UnMaskComponent(this.hotKeyEditPanel.transform as RectTransform);
			}
		}

		// Token: 0x06005D3B RID: 23867 RVA: 0x002ADDF8 File Offset: 0x002ABFF8
		public void RefreshAllHotKeySettingItems()
		{
			bool flag = this._categoryGroups == null;
			if (!flag)
			{
				foreach (KeyValuePair<int, List<SettingGroupItem>> kvp in this._categoryGroups)
				{
					List<SettingGroupItem> groups = kvp.Value;
					bool flag2 = groups == null;
					if (!flag2)
					{
						for (int i = 0; i < groups.Count; i++)
						{
							groups[i].RefreshItems();
						}
					}
				}
			}
		}

		// Token: 0x04003FDF RID: 16351
		public static readonly SettingCategoryData[] Categories = new SettingCategoryData[]
		{
			new SettingCategoryData(ESettingCategory.Game, new SubCategoryInfo[]
			{
				new SubCategoryInfo(ESettingSubCategory.Localization, LanguageKey.LK_SystemSetting_LocalizationSetting),
				new SubCategoryInfo(ESettingSubCategory.Saving, LanguageKey.LK_SystemSettings_SavingSetting),
				new SubCategoryInfo(ESettingSubCategory.Game, LanguageKey.LK_SystemSettings_GameSetting),
				new SubCategoryInfo(ESettingSubCategory.Encyclopedia, LanguageKey.LK_SystemSetting_EncyclopediaSetting),
				new SubCategoryInfo(ESettingSubCategory.Tips, LanguageKey.LK_SystemSetting_TipsDisplaySetting),
				new SubCategoryInfo(ESettingSubCategory.MapExplore, LanguageKey.LK_SystemSetting_MapExploreSetting),
				new SubCategoryInfo(ESettingSubCategory.RegionStory, LanguageKey.LK_SystemSetting_BaseSettings_SectMain)
			}),
			new SettingCategoryData(ESettingCategory.AudioVisual, new SubCategoryInfo[]
			{
				new SubCategoryInfo(ESettingSubCategory.Video, LanguageKey.LK_SystemSettings_VideoSetting),
				new SubCategoryInfo(ESettingSubCategory.Audio, LanguageKey.LK_SystemSetting_AudioSetting)
			}),
			new SettingCategoryData(ESettingCategory.Combat, new SubCategoryInfo[]
			{
				new SubCategoryInfo(ESettingSubCategory.Function, LanguageKey.LK_SystemSettings_FunctionSetting),
				new SubCategoryInfo(ESettingSubCategory.Display, LanguageKey.LK_SystemSettings_DisplaySetting)
			}),
			new SettingCategoryData(ESettingCategory.AutoCombat, new SubCategoryInfo[]
			{
				new SubCategoryInfo(ESettingSubCategory.AutoAttack, LanguageKey.LK_SystemSetting_AutoAttackSetting),
				new SubCategoryInfo(ESettingSubCategory.AutoMove, LanguageKey.LK_SystemSetting_AutoMoveSetting),
				new SubCategoryInfo(ESettingSubCategory.AutoAction, LanguageKey.LK_SystemSetting_AutoActionSetting),
				new SubCategoryInfo(ESettingSubCategory.AutoCast, LanguageKey.LK_SystemSetting_AutoCastSetting),
				new SubCategoryInfo(ESettingSubCategory.AutoAssist, LanguageKey.LK_SystemSetting_AutoAssistSetting)
			}),
			new SettingCategoryData(ESettingCategory.HotKey, new SubCategoryInfo[]
			{
				new SubCategoryInfo(ESettingSubCategory.HotKeyView, LanguageKey.LK_SystemSetting_HotKeyGroup_Interface),
				new SubCategoryInfo(ESettingSubCategory.HotKeySence, LanguageKey.LK_SystemSetting_HotKeyGroup_Sence),
				new SubCategoryInfo(ESettingSubCategory.HotKeyCombat, LanguageKey.LK_SystemSetting_HotKeyGroup_Combat)
			})
		};

		// Token: 0x04003FE0 RID: 16352
		[Header("UI容器")]
		[SerializeField]
		private CToggleGroup categoryToggleGroup;

		// Token: 0x04003FE1 RID: 16353
		[SerializeField]
		private CToggleGroup currentSubCategoryToggleGroup;

		// Token: 0x04003FE2 RID: 16354
		[SerializeField]
		private CToggle subCategoryToggleTemplate;

		// Token: 0x04003FE3 RID: 16355
		[SerializeField]
		private RectTransform settingScrollContent;

		// Token: 0x04003FE4 RID: 16356
		[SerializeField]
		private CScrollRect settingScrollRect;

		// Token: 0x04003FE5 RID: 16357
		[SerializeField]
		private RectTransform titleRoot;

		// Token: 0x04003FE6 RID: 16358
		[Header("设置项预制体")]
		[SerializeField]
		private BoolSettingItem boolSettingPrefab;

		// Token: 0x04003FE7 RID: 16359
		[SerializeField]
		private IntSettingItem intSettingPrefab;

		// Token: 0x04003FE8 RID: 16360
		[SerializeField]
		private FloatSettingItem floatSettingPrefab;

		// Token: 0x04003FE9 RID: 16361
		[SerializeField]
		private EnumSettingItem enumSettingPrefab;

		// Token: 0x04003FEA RID: 16362
		[SerializeField]
		private SwitchButtonSettingItem switchButtonSettingPrefab;

		// Token: 0x04003FEB RID: 16363
		[SerializeField]
		private MultiToggleGroupSettingItem multiToggleGroupSettingPrefab;

		// Token: 0x04003FEC RID: 16364
		[SerializeField]
		private TeammateCommandSettingItem teammateCommandSettingPrefab;

		// Token: 0x04003FED RID: 16365
		[SerializeField]
		private HotKeySettingItem hotKeySettingPrefab;

		// Token: 0x04003FEE RID: 16366
		[Header("设置项预制体")]
		[SerializeField]
		private Color[] categoryToggleTitleColors;

		// Token: 0x04003FEF RID: 16367
		[Header("右侧Tips")]
		[SerializeField]
		private RectTransform rightTipsRoot;

		// Token: 0x04003FF0 RID: 16368
		[SerializeField]
		private TextMeshProUGUI rightTipsTitleText;

		// Token: 0x04003FF1 RID: 16369
		[SerializeField]
		private TextMeshProUGUI rightTipsContentText;

		// Token: 0x04003FF2 RID: 16370
		[Header("快捷键编辑面板")]
		[SerializeField]
		private HotKeyEditPanel hotKeyEditPanel;

		// Token: 0x04003FF3 RID: 16371
		private static readonly SystemSettingMapping Mapping = new SystemSettingMapping();

		// Token: 0x04003FF4 RID: 16372
		private readonly Dictionary<SettingUIType, SettingItemBase> _prefabMap = new Dictionary<SettingUIType, SettingItemBase>();

		// Token: 0x04003FF5 RID: 16373
		private Dictionary<ESettingSubCategory, List<ISettingItemInfo>> _subCategoryInfos;

		// Token: 0x04003FF6 RID: 16374
		private readonly List<RectTransform> _settingItemHolders = new List<RectTransform>();

		// Token: 0x04003FF7 RID: 16375
		private readonly Dictionary<int, List<SettingGroupItem>> _categoryGroups = new Dictionary<int, List<SettingGroupItem>>();

		// Token: 0x04003FF8 RID: 16376
		private int _currentCategoryIndex = -1;

		// Token: 0x04003FF9 RID: 16377
		private int _currentSubCategoryIndex = -1;

		// Token: 0x04003FFA RID: 16378
		private readonly Dictionary<int, float> _subCategoryPositions = new Dictionary<int, float>();

		// Token: 0x04003FFB RID: 16379
		private static int _lastCategoryIndex = 0;

		// Token: 0x04003FFC RID: 16380
		private static readonly Dictionary<int, Vector2> CategoryScrollPositions = new Dictionary<int, Vector2>();

		// Token: 0x04003FFD RID: 16381
		private bool _isSyncing;

		// Token: 0x04003FFE RID: 16382
		private ESettingSubCategory _currentResetSubCategory;

		// Token: 0x04003FFF RID: 16383
		private AiOptions _aiOptions;

		// Token: 0x04004000 RID: 16384
		private string _aiOptionsSaveFilePath;

		// Token: 0x04004001 RID: 16385
		private const int LineSpace = 44;

		// Token: 0x04004002 RID: 16386
		private const float ScrollTopPadding = 20f;

		// Token: 0x04004003 RID: 16387
		private bool _inited = false;

		// Token: 0x04004004 RID: 16388
		private const int HotKeyEditPanelOffsetX = 79;

		// Token: 0x04004005 RID: 16389
		private const int HotkeyEditPanelOffsetY = 133;

		// Token: 0x04004006 RID: 16390
		private const int EditPanelLimitY = -1000;
	}
}
