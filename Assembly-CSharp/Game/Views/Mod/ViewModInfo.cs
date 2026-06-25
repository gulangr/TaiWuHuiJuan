using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.ModSystem;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Mod;
using Steamworks;
using TMPro;
using UnityEngine;

namespace Game.Views.Mod
{
	// Token: 0x020008D2 RID: 2258
	public class ViewModInfo : UIBase
	{
		// Token: 0x17000CAC RID: 3244
		// (get) Token: 0x06006C00 RID: 27648 RVA: 0x0031CA29 File Offset: 0x0031AC29
		private bool IsInGame
		{
			get
			{
				return GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			}
		}

		// Token: 0x17000CAD RID: 3245
		// (get) Token: 0x06006C01 RID: 27649 RVA: 0x0031CA38 File Offset: 0x0031AC38
		private bool IsLocalMod
		{
			get
			{
				return this._data != null && this._data.ModId.Source == 0;
			}
		}

		// Token: 0x17000CAE RID: 3246
		// (get) Token: 0x06006C02 RID: 27650 RVA: 0x0031CA58 File Offset: 0x0031AC58
		// (set) Token: 0x06006C03 RID: 27651 RVA: 0x0031CA60 File Offset: 0x0031AC60
		public bool IsSettingChanged
		{
			get
			{
				return this._isSettingChanged;
			}
			set
			{
				this._isSettingChanged = value;
				this.confirmBtn.interactable = this._isSettingChanged;
			}
		}

		// Token: 0x06006C04 RID: 27652 RVA: 0x0031CA7C File Offset: 0x0031AC7C
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = !this._inited;
			if (flag)
			{
				this.InitSettingItemPools();
				this._inited = true;
			}
			argsBox.Get<ModInfoWithDisplayData>("DisplayData", out this._data);
			argsBox.Get("OriginName", out this._originName);
			this._originId.FileId = 0UL;
			argsBox.Get<ModId>("OriginId", out this._originId);
			argsBox.Get("IsShowSettings", out this._isShowSettings);
			Action _onRefresh;
			bool flag2 = !argsBox.Get<Action>("OnSetGrade", out _onRefresh);
			if (flag2)
			{
				_onRefresh = null;
			}
			this.IsSettingChanged = false;
			this.UpdateDependencies();
		}

		// Token: 0x06006C05 RID: 27653 RVA: 0x0031CB24 File Offset: 0x0031AD24
		private void UpdateDependencies()
		{
			bool flag = this._data == null || this._data.Dependencies == null || this._data.Dependencies.Count == 0;
			if (flag)
			{
				this.Refresh();
				this.Element.ShowAfterRefresh();
			}
			else
			{
				ModManager.UpdateTargetItems(this._data.Dependencies, delegate(Dictionary<ModId, bool> _)
				{
					this.Refresh();
					this.Element.ShowAfterRefresh();
				});
			}
		}

		// Token: 0x06006C06 RID: 27654 RVA: 0x0031CB94 File Offset: 0x0031AD94
		public override void QuickHide()
		{
			bool flag = this.IsLocalMod && !this._localModExists;
			if (flag)
			{
				this.CheckAndDeleteLocalMod();
			}
			else
			{
				bool flag2 = !this.IsSettingChanged;
				if (flag2)
				{
					base.QuickHide();
				}
				else
				{
					UIElement.SaveChangedSettingDialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("ConfirmAction", new Action(delegate
					{
						base.QuickHide();
						Action onRefresh = this._onRefresh;
						if (onRefresh != null)
						{
							onRefresh();
						}
					})).SetObject("SaveAction", new Action(delegate
					{
						this.ConfirmSettings();
						base.QuickHide();
						Action onRefresh = this._onRefresh;
						if (onRefresh != null)
						{
							onRefresh();
						}
					})));
					UIManager.Instance.MaskUI(UIElement.SaveChangedSettingDialog);
				}
			}
		}

		// Token: 0x06006C07 RID: 27655 RVA: 0x0031CC28 File Offset: 0x0031AE28
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (a == "ButtonCloseView")
			{
				this.QuickHide();
			}
		}

		// Token: 0x06006C08 RID: 27656 RVA: 0x0031CC5C File Offset: 0x0031AE5C
		private void Awake()
		{
			this.authorNameBtn.ClearAndAddListener(delegate
			{
				Debug.Log("跳转创意工坊页签并搜索");
			});
			this.settingGroupToggleGroup.OnActiveIndexChange -= this.OnGroupToggleChanged;
			this.settingGroupToggleGroup.OnActiveIndexChange += this.OnGroupToggleChanged;
			this.likeToggle.onValueChanged.ResetListener(new Action<bool>(this.OnLikeToggleChanged));
			this.unLikeToggle.onValueChanged.ResetListener(new Action<bool>(this.OnUnlikeToggleChanged));
			this.shareBtn.ClearAndAddListener(new Action(this.OnClickShare));
			this.jumpBtn.ClearAndAddListener(new Action(this.OnClickJump));
			this.confirmBtn.ClearAndAddListener(delegate
			{
				this.ConfirmSettings();
			});
			this.showInExplorerBtn.ClearAndAddListener(delegate
			{
				bool flag = this._data == null || string.IsNullOrEmpty(this._data.DirectoryName);
				if (!flag)
				{
					ViewModInfo.OpenExplorer(this._data.DirectoryName);
				}
			});
			this.subscribeToggle.onValueChanged.ResetListener(new Action<bool>(this.OnSubscribeToggleChanged));
		}

		// Token: 0x06006C09 RID: 27657 RVA: 0x0031CD7C File Offset: 0x0031AF7C
		private void InitSettingItemPools()
		{
			bool flag = this.dropdownSettingItem != null;
			if (flag)
			{
				PoolManager.SetSrcObject("DropdownSettingItem", this.dropdownSettingItem.gameObject);
			}
			bool flag2 = this.inputFieldSettingItem != null;
			if (flag2)
			{
				PoolManager.SetSrcObject("InputFieldSettingItem", this.inputFieldSettingItem.gameObject);
			}
			bool flag3 = this.sliderSettingItem != null;
			if (flag3)
			{
				PoolManager.SetSrcObject("SliderSettingItem", this.sliderSettingItem.gameObject);
			}
			bool flag4 = this.toggleSettingItem != null;
			if (flag4)
			{
				PoolManager.SetSrcObject("ToggleSettingItem", this.toggleSettingItem.gameObject);
			}
		}

		// Token: 0x06006C0A RID: 27658 RVA: 0x0031CE22 File Offset: 0x0031B022
		private void ClearSettingItemPools()
		{
			PoolManager.RemoveData("DropdownSettingItem");
			PoolManager.RemoveData("InputFieldSettingItem");
			PoolManager.RemoveData("SliderSettingItem");
			PoolManager.RemoveData("ToggleSettingItem");
		}

		// Token: 0x06006C0B RID: 27659 RVA: 0x0031CE51 File Offset: 0x0031B051
		private void OnDestroy()
		{
			this.ClearSettingItemPools();
		}

		// Token: 0x06006C0C RID: 27660 RVA: 0x0031CE5C File Offset: 0x0031B05C
		private void Refresh()
		{
			bool flag = this._data == null;
			if (!flag)
			{
				this.modNameTitleLabel.SetText(this._data.Title, true);
				this.modNameLabel.SetText(this._data.Title, true);
				this.authorNameLabel.SetText(this._data.Author ?? "", true);
				this.versionLabel.SetText(this._data.GetVersionString(), true);
				this.SetModSource(this._data.ModId.Source);
				this.modIdLabel.SetText(this._data.ModId.FileId.ToString(), true);
				double fileSizeMB = Math.Round((double)this._data.FileSize / 1048576.0, 3);
				this.modSizeLabel.SetText(string.Format("{0} MB", fileSizeMB), true);
				this.updateDateLabel.SetText(LanguageKey.LK_Mod_UpdateTime.TrFormat(ViewModInfo.GetTimeString(this._data.UpdateData)), true);
				this.publishDateLabel.SetText(LanguageKey.LK_Mod_UploadTime.TrFormat(ViewModInfo.GetTimeString(this._data.CreateData)), true);
				this.versionNotice.gameObject.SetActive(ModManager.IsModOutdated(this._data));
				this.RefreshCover();
				this.SetTags();
				this.descLabel.SetText(this._data.Description, true);
				this.UpdateSettings();
				this.InitVoteToggleState();
				this.SetDependencies(this._isShowSettings && this._data.Dependencies != null && this._data.Dependencies.Count > 0);
				this.SetOriginMod(this._isShowSettings && false);
				bool isLocalMod = this.IsLocalMod;
				if (isLocalMod)
				{
					this._localModExists = true;
					this.subscribeToggle.SetIsOnWithoutNotify(true);
					this.subscribeToggle.GetComponent<ToggleStyle>().SetLabelText(LanguageKey.LK_Mod_Info_UnInstall.Tr());
				}
				else
				{
					this.subscribeToggle.SetIsOnWithoutNotify(this._data.IsSubscribed);
					this.subscribeToggle.GetComponent<ToggleStyle>().SetLabelText(LanguageKey.LK_Mod_Subscribe.Tr());
				}
				this.likeToggle.gameObject.SetActive(!this.IsLocalMod);
				this.unLikeToggle.gameObject.SetActive(!this.IsLocalMod);
				this.shareBtn.gameObject.SetActive(!this.IsLocalMod);
				this.jumpBtn.gameObject.SetActive(!this.IsLocalMod);
				this.modInfoToggleGroup.Init(-1);
				this.modInfoToggleGroup.OnActiveIndexChange += this.OnModInfoToggleGroupActiveIndexChange;
				this.modInfoToggleGroup.transform.GetChild(1).gameObject.SetActive(this._isShowSettings);
				this.modInfoToggleGroup.GetComponent<CImage>().enabled = this._isShowSettings;
				CToggleGroup ctoggleGroup = this.modInfoToggleGroup;
				int index;
				if (this._isShowSettings)
				{
					ModInfoWithDisplayData data = this._data;
					bool flag2;
					if (data == null)
					{
						flag2 = false;
					}
					else
					{
						List<SettingEntry> modSettingEntries = data.ModSettingEntries;
						int? num = (modSettingEntries != null) ? new int?(modSettingEntries.Count) : null;
						int num2 = 0;
						flag2 = (num.GetValueOrDefault() > num2 & num != null);
					}
					if (flag2)
					{
						index = 1;
						goto IL_360;
					}
				}
				index = 0;
				IL_360:
				ctoggleGroup.Set(index, true);
				this.showInExplorerBtn.gameObject.SetActive(this._isShowSettings);
				this.confirmBtn.gameObject.SetActive(this._isShowSettings);
			}
		}

		// Token: 0x06006C0D RID: 27661 RVA: 0x0031D1FE File Offset: 0x0031B3FE
		private void OnModInfoToggleGroupActiveIndexChange(int newIndex, int preIndex)
		{
			this.descHolder.gameObject.SetActive(newIndex == 0);
			this.settingHolder.gameObject.SetActive(newIndex == 1);
		}

		// Token: 0x06006C0E RID: 27662 RVA: 0x0031D22C File Offset: 0x0031B42C
		private void SetModSource(byte source)
		{
			if (source != 0)
			{
				if (source != 1)
				{
					this.sourceLabel.SetText(string.Empty, true);
				}
				else
				{
					this.sourceLabel.SetText("<SpName=ui9_modpanel_icon_logo_0>" + LanguageKey.LK_Mod_Steam.Tr(), true);
				}
			}
			else
			{
				this.sourceLabel.SetText("<SpName=ui9_modpanel_icon_local_0>" + LanguageKey.LK_Mod_Local.Tr(), true);
			}
			this.sourceLabel.GetComponent<TMPTextSpriteHelper>().Parse();
		}

		// Token: 0x06006C0F RID: 27663 RVA: 0x0031D2B4 File Offset: 0x0031B4B4
		private void RefreshCover()
		{
			bool flag = this._data == null;
			if (flag)
			{
				this.coverImage.enabled = false;
			}
			else
			{
				Texture2D localCover = ModManager.GetModCoverTexture(this._data.ModId);
				bool flag2 = localCover != null;
				if (flag2)
				{
					this.coverImage.texture = localCover;
					this.coverImage.enabled = true;
				}
				else
				{
					Texture2D previewCover = ModManager.GetPreviewModCoverTexture(this._data.ModId);
					bool flag3 = previewCover != null;
					if (flag3)
					{
						this.coverImage.texture = previewCover;
						this.coverImage.enabled = true;
					}
					else
					{
						this.coverImage.enabled = false;
					}
				}
			}
		}

		// Token: 0x06006C10 RID: 27664 RVA: 0x0031D364 File Offset: 0x0031B564
		private void SetTags()
		{
			ModInfoWithDisplayData data = this._data;
			List<string> tags = (data != null) ? data.TagList : null;
			List<string> tagContentList = SteamManager.GetTagContentList(tags);
			bool flag = tags == null || tags.Count == 0;
			if (flag)
			{
				this.tagsLineHolder.gameObject.SetActive(false);
			}
			else
			{
				this.tagsLineHolder.gameObject.SetActive(true);
				int lineCount = Mathf.CeilToInt((float)tags.Count / 3f);
				bool flag2 = this.tagsLineHolder.childCount > 0;
				if (flag2)
				{
					CommonUtils.PrepareEnoughChildren(this.tagsLineHolder, this.tagsLineHolder.GetChild(0).gameObject, lineCount, null);
				}
				for (int i = 0; i < this.tagsLineHolder.childCount; i++)
				{
					Transform line = this.tagsLineHolder.GetChild(i);
					for (int j = 0; j < line.childCount; j++)
					{
						int tagIndex = i * 3 + j;
						Transform tagItem = line.GetChild(j);
						bool flag3 = tagIndex < tags.Count;
						if (flag3)
						{
							tagItem.gameObject.SetActive(true);
							TextMeshProUGUI textComponent;
							bool flag4 = tagItem.GetChild(0).TryGetComponent<TextMeshProUGUI>(out textComponent);
							if (flag4)
							{
								textComponent.SetText(tagContentList[tagIndex], true);
							}
						}
						else
						{
							tagItem.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x06006C11 RID: 27665 RVA: 0x0031D4E0 File Offset: 0x0031B6E0
		public static string GetTimeString(uint time)
		{
			bool flag = time == 0U;
			string result;
			if (flag)
			{
				result = "/";
			}
			else
			{
				result = DateTimeOffset.FromUnixTimeSeconds((long)((ulong)time)).DateTime.ToLocalTime().ToString(FrameCommon.DateTimeToStringFormat_1);
			}
			return result;
		}

		// Token: 0x06006C12 RID: 27666 RVA: 0x0031D528 File Offset: 0x0031B728
		private void SetDependencies(bool isShow)
		{
			this.dependenciesHolder.gameObject.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				TMP_Text tmp_Text = this.dependenciesTitleLabel;
				LanguageKey languageKey = LanguageKey.LK_Mod_Info_Dependencies;
				ModInfoWithDisplayData data = this._data;
				int? num;
				if (data == null)
				{
					num = null;
				}
				else
				{
					List<ulong> dependencies = data.Dependencies;
					num = ((dependencies != null) ? new int?(dependencies.Count) : null);
				}
				tmp_Text.text = languageKey.TrFormat(num);
				CommonUtils.PrepareEnoughChildren(this.dependencyItemsHolder, this.dependencyItemsHolder.GetChild(0).gameObject, this._data.Dependencies.Count, null);
				for (int i = 0; i < this._data.Dependencies.Count; i++)
				{
					ulong fileId = this._data.Dependencies[i];
					DependecyItem dependencyItem = this.dependencyItemsHolder.GetChild(i).GetComponent<DependecyItem>();
					ModInfoWithDisplayData depModInfo = ModManager.GetModInfo(new ModId
					{
						FileId = fileId,
						Source = 1
					});
					ModInfoWithDisplayData depModInfo2 = depModInfo;
					string depName = ((depModInfo2 != null) ? depModInfo2.Title : null) ?? string.Format("{0}", fileId);
					bool isInstalled = depModInfo != null && ModManager.PlatformMods.Exists((ModId id) => id.FileId == fileId);
					dependencyItem.Set(isInstalled, depName, delegate()
					{
						ViewModInfo.OnClickDependency(depModInfo, fileId);
					});
				}
			}
		}

		// Token: 0x06006C13 RID: 27667 RVA: 0x0031D6D0 File Offset: 0x0031B8D0
		private void SetOriginMod(bool isShow)
		{
			this.originModHolder.gameObject.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				ModInfoWithDisplayData originModInfo = ModManager.GetModInfo(this._originId);
				ModInfoWithDisplayData originModInfo2 = originModInfo;
				string text;
				if ((text = ((originModInfo2 != null) ? originModInfo2.Title : null)) == null)
				{
					text = (this._originName ?? string.Format("{0}", this._originId));
				}
				string originName = text;
				this.originModItem.Set(originName, delegate()
				{
					ViewModInfo.OnClickDependency(originModInfo, this._originId.FileId);
				});
			}
		}

		// Token: 0x06006C14 RID: 27668 RVA: 0x0031D768 File Offset: 0x0031B968
		private static void OnClickDependency(ModInfoWithDisplayData depModInfo, ulong fileId)
		{
			string url = string.Format("https://steamcommunity.com/sharedfiles/filedetails/?id={0}", fileId);
			SteamFriends.ActivateGameOverlayToWebPage(url, EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
		}

		// Token: 0x06006C15 RID: 27669 RVA: 0x0031D790 File Offset: 0x0031B990
		private void UpdateSettings()
		{
			this.ClearSettingItems();
			ModInfoWithDisplayData data = this._data;
			bool flag = ((data != null) ? data.ModSettingEntries : null) == null || this._data.ModSettingEntries.Count == 0;
			if (flag)
			{
				this.settingGroupToggleGroup.gameObject.SetActive(false);
				this._groups = null;
			}
			else
			{
				List<string> modSettingGroups = this._data.ModSettingGroups;
				Dictionary<string, List<SettingEntry>> groupedEntries = (from e in this._data.ModSettingEntries
				group e by string.IsNullOrEmpty(e.GroupName) ? string.Empty : e.GroupName).ToDictionary((IGrouping<string, SettingEntry> g) => g.Key, (IGrouping<string, SettingEntry> g) => g.ToList<SettingEntry>());
				this._groups = new List<ValueTuple<string, List<SettingEntry>>>();
				bool flag2 = modSettingGroups != null && modSettingGroups.Count > 0;
				if (flag2)
				{
					foreach (string groupName in modSettingGroups)
					{
						string key = string.IsNullOrEmpty(groupName) ? string.Empty : groupName;
						List<SettingEntry> entries;
						bool flag3 = groupedEntries.TryGetValue(key, out entries);
						if (flag3)
						{
							this._groups.Add(new ValueTuple<string, List<SettingEntry>>(key, entries));
							groupedEntries.Remove(key);
						}
					}
				}
				foreach (KeyValuePair<string, List<SettingEntry>> pair in groupedEntries)
				{
					this._groups.Add(new ValueTuple<string, List<SettingEntry>>(pair.Key, pair.Value));
				}
				bool flag4 = this._groups.Any(([TupleElementNames(new string[]
				{
					"GroupName",
					"Entries"
				})] ValueTuple<string, List<SettingEntry>> g) => g.Item1 != string.Empty);
				if (flag4)
				{
					this.settingGroupToggleGroup.gameObject.SetActive(true);
					this.CreateGroupToggles(this._groups);
				}
				else
				{
					this.settingGroupToggleGroup.gameObject.SetActive(false);
				}
				this.settingGroupToggleGroup.Clear();
				this.settingGroupToggleGroup.AddAllChildToggles();
				this.settingGroupToggleGroup.Init(-1);
				this._currentGroupIndex = 0;
				bool flag5 = this._groups.Count > 0;
				if (flag5)
				{
					this.CreateSettingGroup(this._groups[0].Item1, this._groups[0].Item2);
				}
				bool flag6 = this._groupToggles.Count > 0;
				if (flag6)
				{
					this.settingGroupToggleGroup.Set(0, true);
				}
			}
		}

		// Token: 0x06006C16 RID: 27670 RVA: 0x0031DA68 File Offset: 0x0031BC68
		private void CreateGroupToggles([TupleElementNames(new string[]
		{
			"GroupName",
			"Entries"
		})] List<ValueTuple<string, List<SettingEntry>>> groups)
		{
			CommonUtils.PrepareEnoughChildren(this.settingGroupToggleGroup.transform, this.settingGroupTemplate.gameObject, groups.Count, null);
			for (int i = 0; i < groups.Count; i++)
			{
				GameObject toggleGo = this.settingGroupToggleGroup.transform.GetChild(i).gameObject;
				toggleGo.SetActive(true);
				toggleGo.name = "Toggle_" + groups[i].Item1;
				CToggle toggle = toggleGo.GetComponent<CToggle>();
				ToggleStyle toggleStyle = toggleGo.GetComponent<ToggleStyle>();
				bool flag = toggleStyle != null;
				if (flag)
				{
					toggleStyle.SetLabelText(groups[i].Item1);
				}
				this._groupToggles.Add(toggle);
			}
		}

		// Token: 0x06006C17 RID: 27671 RVA: 0x0031DB3C File Offset: 0x0031BD3C
		private void CreateSettingGroup(string groupName, List<SettingEntry> entries)
		{
			foreach (SettingEntry entry in entries)
			{
				this.CreateSettingItem(entry);
			}
		}

		// Token: 0x06006C18 RID: 27672 RVA: 0x0031DB90 File Offset: 0x0031BD90
		private void ClearSettingItemsOnly()
		{
			foreach (SettingItemBase item in this._settingItems)
			{
				bool flag = item != null;
				if (flag)
				{
					item.OnValueChanged -= this.OnSettingValueChanged;
					if (!true)
					{
					}
					string text;
					if (!(item is ToggleSettingItem))
					{
						if (!(item is SliderSettingItem))
						{
							if (!(item is DropdownSettingItem))
							{
								if (!(item is InputFieldSettingItem))
								{
									text = null;
								}
								else
								{
									text = "InputFieldSettingItem";
								}
							}
							else
							{
								text = "DropdownSettingItem";
							}
						}
						else
						{
							text = "SliderSettingItem";
						}
					}
					else
					{
						text = "ToggleSettingItem";
					}
					if (!true)
					{
					}
					string poolKey = text;
					item.transform.SetParent(null);
					bool flag2 = poolKey != null;
					if (flag2)
					{
						PoolManager.Destroy(poolKey, item.gameObject);
					}
					else
					{
						Object.Destroy(item.gameObject);
					}
				}
			}
			this._settingItems.Clear();
		}

		// Token: 0x06006C19 RID: 27673 RVA: 0x0031DCA4 File Offset: 0x0031BEA4
		private void CreateSettingItem(SettingEntry entry)
		{
			if (!true)
			{
			}
			SettingItemBase settingItemBase;
			if (!(entry is ToggleSetting))
			{
				if (!(entry is SliderSetting))
				{
					if (!(entry is DropdownSetting))
					{
						if (!(entry is InputFieldSetting))
						{
							if (!(entry is ToggleGroupSetting))
							{
								settingItemBase = null;
							}
							else
							{
								settingItemBase = this.CreateDropdownFromToggleGroup();
							}
						}
						else
						{
							settingItemBase = PoolManager.GetObject<InputFieldSettingItem>("InputFieldSettingItem");
						}
					}
					else
					{
						settingItemBase = PoolManager.GetObject<DropdownSettingItem>("DropdownSettingItem");
					}
				}
				else
				{
					settingItemBase = PoolManager.GetObject<SliderSettingItem>("SliderSettingItem");
				}
			}
			else
			{
				settingItemBase = PoolManager.GetObject<ToggleSettingItem>("ToggleSettingItem");
			}
			if (!true)
			{
			}
			SettingItemBase item = settingItemBase;
			bool flag = item != null;
			if (flag)
			{
				item.transform.SetParent(this.settingItemsHolder, false);
				item.gameObject.SetActive(true);
				item.Initialize(entry);
				item.OnValueChanged += this.OnSettingValueChanged;
				item.transform.localScale = Vector3.one;
				this._settingItems.Add(item);
			}
		}

		// Token: 0x06006C1A RID: 27674 RVA: 0x0031DD90 File Offset: 0x0031BF90
		private DropdownSettingItem CreateDropdownFromToggleGroup()
		{
			return PoolManager.GetObject<DropdownSettingItem>("DropdownSettingItem");
		}

		// Token: 0x06006C1B RID: 27675 RVA: 0x0031DDB0 File Offset: 0x0031BFB0
		private void OnSettingValueChanged(bool changed)
		{
			if (changed)
			{
				this.IsSettingChanged = true;
				this.CheckNeedRestart();
			}
		}

		// Token: 0x06006C1C RID: 27676 RVA: 0x0031DDD4 File Offset: 0x0031BFD4
		private void CheckNeedRestart()
		{
			ModInfoWithDisplayData data = this._data;
			bool flag = data != null && data.NeedRestartWhenSettingChanged;
			if (flag)
			{
				this._needRestart = true;
			}
		}

		// Token: 0x06006C1D RID: 27677 RVA: 0x0031DE04 File Offset: 0x0031C004
		public void ConfirmSettings()
		{
			bool flag = !this.IsSettingChanged;
			if (!flag)
			{
				foreach (SettingItemBase item in this._settingItems)
				{
					item.ApplyValue();
				}
				this.SaveAllSettings();
			}
		}

		// Token: 0x06006C1E RID: 27678 RVA: 0x0031DE74 File Offset: 0x0031C074
		public void CancelSettings()
		{
			bool flag = !this.IsSettingChanged;
			if (!flag)
			{
				foreach (SettingItemBase item in this._settingItems)
				{
					item.ResetValue();
				}
				this.SetSettingChangeState(false);
			}
		}

		// Token: 0x06006C1F RID: 27679 RVA: 0x0031DEE4 File Offset: 0x0031C0E4
		private void SaveAllSettings()
		{
			bool isInGame = this.IsInGame;
			if (isInGame)
			{
				this.SaveAllSettingsInGame();
			}
			else
			{
				this.Apply();
			}
		}

		// Token: 0x06006C20 RID: 27680 RVA: 0x0031DF10 File Offset: 0x0031C110
		private void SaveAllSettingsInGame()
		{
			bool flag = !this.IsSettingChanged;
			if (!flag)
			{
				ModManager.SaveModSettings(false);
				bool needRestart = this._needRestart;
				if (needRestart)
				{
					this.ShowRestart();
				}
				else
				{
					this.SetSettingChangeState(false);
					bool flag2 = this._data != null;
					if (flag2)
					{
						ModManager.UpdateModSettingsInGame(this._data.ModId);
					}
				}
			}
		}

		// Token: 0x06006C21 RID: 27681 RVA: 0x0031DF74 File Offset: 0x0031C174
		private void Apply()
		{
			bool flag = this.IsSettingChanged && this._needRestart;
			if (flag)
			{
				this.ShowRestart();
			}
			else
			{
				ModManager.SaveModSettings(false);
				foreach (ModId modId in ModManager.EnabledMods)
				{
					ModManager.UpdateModSettingsInGame(modId);
				}
				this.SetSettingChangeState(false);
			}
		}

		// Token: 0x06006C22 RID: 27682 RVA: 0x0031DFFC File Offset: 0x0031C1FC
		private void SetSettingChangeState(bool changed)
		{
			this.IsSettingChanged = changed;
			bool flag = !changed;
			if (flag)
			{
				this._needRestart = false;
			}
		}

		// Token: 0x06006C23 RID: 27683 RVA: 0x0031E024 File Offset: 0x0031C224
		private void ShowRestart()
		{
			DialogCmd restartDialog = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_Apply_Restart_Title),
				Content = LocalStringManager.Get(LanguageKey.LK_Apply_Restart_Content),
				Type = 1,
				Yes = new Action(this.OnConfirmRestart)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", restartDialog));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06006C24 RID: 27684 RVA: 0x0031E09C File Offset: 0x0031C29C
		private void OnConfirmRestart()
		{
			ModManager.SaveModSettings(false);
			GameApp.Instance.ReStart();
		}

		// Token: 0x06006C25 RID: 27685 RVA: 0x0031E0B4 File Offset: 0x0031C2B4
		private void OnLikeToggleChanged(bool isOn)
		{
			bool flag = this._data == null || this._data.ModId.Source != 1;
			if (!flag)
			{
				if (isOn)
				{
					PublishedFileId_t fileId = new PublishedFileId_t(this._data.ModId.FileId);
					SteamAPICall_t steamAPICall = SteamUGC.SetUserItemVote(fileId, true);
					CallResult<SetUserItemVoteResult_t> callResult = CallResult<SetUserItemVoteResult_t>.Create(null);
					callResult.Set(steamAPICall, delegate(SetUserItemVoteResult_t result, bool failure)
					{
						if (!failure)
						{
							this.RefreshVoteToggleState(result.m_bVoteUp, !result.m_bVoteUp);
						}
					});
				}
			}
		}

		// Token: 0x06006C26 RID: 27686 RVA: 0x0031E12C File Offset: 0x0031C32C
		private void OnUnlikeToggleChanged(bool isOn)
		{
			bool flag = this._data == null || this._data.ModId.Source != 1;
			if (!flag)
			{
				if (isOn)
				{
					PublishedFileId_t fileId = new PublishedFileId_t(this._data.ModId.FileId);
					SteamAPICall_t steamAPICall = SteamUGC.SetUserItemVote(fileId, false);
					CallResult<SetUserItemVoteResult_t> callResult = CallResult<SetUserItemVoteResult_t>.Create(null);
					callResult.Set(steamAPICall, delegate(SetUserItemVoteResult_t result, bool failure)
					{
						if (!failure)
						{
							this.RefreshVoteToggleState(result.m_bVoteUp, !result.m_bVoteUp);
						}
					});
				}
			}
		}

		// Token: 0x06006C27 RID: 27687 RVA: 0x0031E1A4 File Offset: 0x0031C3A4
		private void RefreshVoteToggleState(bool isUp, bool isDown)
		{
			this.likeToggle.SetIsOnWithoutNotify(isUp);
			this.unLikeToggle.SetIsOnWithoutNotify(isDown);
		}

		// Token: 0x06006C28 RID: 27688 RVA: 0x0031E1C4 File Offset: 0x0031C3C4
		private void InitVoteToggleState()
		{
			bool flag = this._data == null || this._data.ModId.Source != 1;
			if (flag)
			{
				this.RefreshVoteToggleState(false, false);
			}
			else
			{
				PublishedFileId_t fileId = new PublishedFileId_t(this._data.ModId.FileId);
				SteamAPICall_t steamAPICall = SteamUGC.GetUserItemVote(fileId);
				CallResult<GetUserItemVoteResult_t> callResult = CallResult<GetUserItemVoteResult_t>.Create(null);
				callResult.Set(steamAPICall, delegate(GetUserItemVoteResult_t result, bool failure)
				{
					if (!failure)
					{
						this.RefreshVoteToggleState(result.m_bVotedUp, result.m_bVotedDown);
					}
				});
			}
		}

		// Token: 0x06006C29 RID: 27689 RVA: 0x0031E23C File Offset: 0x0031C43C
		private void OnClickShare()
		{
			bool flag = this._data == null || this._data.ModId.Source != 1;
			if (!flag)
			{
				string url = string.Format("https://steamcommunity.com/sharedfiles/filedetails/?id={0}", this._data.ModId.FileId);
				GUIUtility.systemCopyBuffer = url;
				GLog.Log("链接已复制到剪贴板");
			}
		}

		// Token: 0x06006C2A RID: 27690 RVA: 0x0031E2A4 File Offset: 0x0031C4A4
		private void OnClickJump()
		{
			bool flag = this._data == null || this._data.ModId.Source != 1;
			if (!flag)
			{
				string url = string.Format("https://steamcommunity.com/sharedfiles/filedetails/?id={0}", this._data.ModId.FileId);
				SteamFriends.ActivateGameOverlayToWebPage(url, EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
			}
		}

		// Token: 0x06006C2B RID: 27691 RVA: 0x0031E304 File Offset: 0x0031C504
		private void OnSubscribeToggleChanged(bool isOn)
		{
			bool flag = this._data == null;
			if (!flag)
			{
				bool isLocalMod = this.IsLocalMod;
				if (isLocalMod)
				{
					this._localModExists = isOn;
					this.subscribeToggle.GetComponent<ToggleStyle>().SetLabelText(isOn ? LanguageKey.LK_Mod_Info_UnInstall.Tr() : LanguageKey.LK_Mod_Info_Installed.Tr());
				}
				else
				{
					if (isOn)
					{
						ModManager.SubscribeItem(this._data.ModId, true);
					}
					else
					{
						ModManager.UnSubscribeItem(this._data.ModId);
					}
					this.subscribeToggle.GetComponent<ToggleStyle>().SetLabelText(isOn ? LanguageKey.LK_Mod_Subscribe.Tr() : LanguageKey.LK_Mod_Subscribe.Tr());
				}
			}
		}

		// Token: 0x06006C2C RID: 27692 RVA: 0x0031E3BC File Offset: 0x0031C5BC
		private void CheckAndDeleteLocalMod()
		{
			bool flag = !this.IsLocalMod || this._localModExists;
			if (!flag)
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Local_Tip_Title);
				string content = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Local_Tip_Content);
				CommonUtils.ShowConfirmDialog(title, content, delegate
				{
					ModManager.DeleteLocalMod(this._data);
					GLog.Log("本地Mod " + this._data.Title + " 已删除");
					GEvent.OnEvent(UiEvents.OnModViewRefresh, null);
					base.QuickHide();
					Action onRefresh = this._onRefresh;
					if (onRefresh != null)
					{
						onRefresh();
					}
				}, delegate
				{
					this._localModExists = true;
					this.subscribeToggle.SetIsOnWithoutNotify(true);
					this.subscribeToggle.GetComponent<ToggleStyle>().SetLabelText(LanguageKey.LK_Mod_Info_UnInstall.Tr());
				}, EDialogType.None);
			}
		}

		// Token: 0x06006C2D RID: 27693 RVA: 0x0031E418 File Offset: 0x0031C618
		private void OnGroupToggleChanged(int newIndex, int oldIndex)
		{
			bool flag = this._isSyncing || newIndex < 0 || this._groups == null;
			if (!flag)
			{
				bool flag2 = newIndex == this._currentGroupIndex;
				if (!flag2)
				{
					this._currentGroupIndex = newIndex;
					this.ClearSettingItemsOnly();
					bool flag3 = newIndex < this._groups.Count;
					if (flag3)
					{
						this.CreateSettingGroup(this._groups[newIndex].Item1, this._groups[newIndex].Item2);
					}
				}
			}
		}

		// Token: 0x06006C2E RID: 27694 RVA: 0x0031E49E File Offset: 0x0031C69E
		private void ClearSettingItems()
		{
			this.ClearSettingItemsOnly();
			this._groupToggles.Clear();
			this._groups = null;
			this._currentGroupIndex = 0;
		}

		// Token: 0x06006C2F RID: 27695 RVA: 0x0031E4C2 File Offset: 0x0031C6C2
		private static void OpenExplorer(string path)
		{
			Process.Start("Explorer.exe", path);
		}

		// Token: 0x06006C30 RID: 27696 RVA: 0x0031E4D4 File Offset: 0x0031C6D4
		private void DelayFrameCall(Action action, int frameCount = 1)
		{
			bool activeSelf = base.gameObject.activeSelf;
			if (activeSelf)
			{
				base.StartCoroutine(this.CoroutineDelayFrameCall(action, frameCount));
			}
		}

		// Token: 0x06006C31 RID: 27697 RVA: 0x0031E500 File Offset: 0x0031C700
		private IEnumerator CoroutineDelayFrameCall(Action action, int frameCount)
		{
			int num;
			for (int i = 0; i < frameCount; i = num + 1)
			{
				yield return null;
				num = i;
			}
			if (action != null)
			{
				action();
			}
			yield break;
		}

		// Token: 0x04004E44 RID: 20036
		[SerializeField]
		private TextMeshProUGUI modNameTitleLabel;

		// Token: 0x04004E45 RID: 20037
		[SerializeField]
		private CRawImage coverImage;

		// Token: 0x04004E46 RID: 20038
		[SerializeField]
		private RectTransform tagsLineHolder;

		// Token: 0x04004E47 RID: 20039
		[SerializeField]
		private RectTransform versionNotice;

		// Token: 0x04004E48 RID: 20040
		[SerializeField]
		private TextMeshProUGUI modNameLabel;

		// Token: 0x04004E49 RID: 20041
		[SerializeField]
		private TextMeshProUGUI authorNameLabel;

		// Token: 0x04004E4A RID: 20042
		[SerializeField]
		private CButton authorNameBtn;

		// Token: 0x04004E4B RID: 20043
		[SerializeField]
		private TextMeshProUGUI versionLabel;

		// Token: 0x04004E4C RID: 20044
		[SerializeField]
		private TextMeshProUGUI sourceLabel;

		// Token: 0x04004E4D RID: 20045
		[SerializeField]
		private TextMeshProUGUI modIdLabel;

		// Token: 0x04004E4E RID: 20046
		[SerializeField]
		private TextMeshProUGUI modSizeLabel;

		// Token: 0x04004E4F RID: 20047
		[SerializeField]
		private TextMeshProUGUI updateDateLabel;

		// Token: 0x04004E50 RID: 20048
		[SerializeField]
		private TextMeshProUGUI publishDateLabel;

		// Token: 0x04004E51 RID: 20049
		[SerializeField]
		private RectTransform dependenciesHolder;

		// Token: 0x04004E52 RID: 20050
		[SerializeField]
		private TextMeshProUGUI dependenciesTitleLabel;

		// Token: 0x04004E53 RID: 20051
		[SerializeField]
		private RectTransform dependencyItemsHolder;

		// Token: 0x04004E54 RID: 20052
		[SerializeField]
		private RectTransform originModHolder;

		// Token: 0x04004E55 RID: 20053
		[SerializeField]
		private DependecyItem originModItem;

		// Token: 0x04004E56 RID: 20054
		[SerializeField]
		private RectTransform originModItemHolder;

		// Token: 0x04004E57 RID: 20055
		[SerializeField]
		private CToggleGroup modInfoToggleGroup;

		// Token: 0x04004E58 RID: 20056
		[SerializeField]
		private RectTransform descHolder;

		// Token: 0x04004E59 RID: 20057
		[SerializeField]
		private TextMeshProUGUI descLabel;

		// Token: 0x04004E5A RID: 20058
		[SerializeField]
		private RectTransform settingHolder;

		// Token: 0x04004E5B RID: 20059
		[SerializeField]
		private CToggleGroup settingGroupToggleGroup;

		// Token: 0x04004E5C RID: 20060
		[SerializeField]
		private CToggle settingGroupTemplate;

		// Token: 0x04004E5D RID: 20061
		[SerializeField]
		private RectTransform settingItemsHolder;

		// Token: 0x04004E5E RID: 20062
		[SerializeField]
		private CScrollRect settingItemsScrollRect;

		// Token: 0x04004E5F RID: 20063
		[SerializeField]
		private DropdownSettingItem dropdownSettingItem;

		// Token: 0x04004E60 RID: 20064
		[SerializeField]
		private InputFieldSettingItem inputFieldSettingItem;

		// Token: 0x04004E61 RID: 20065
		[SerializeField]
		private SliderSettingItem sliderSettingItem;

		// Token: 0x04004E62 RID: 20066
		[SerializeField]
		private ToggleSettingItem toggleSettingItem;

		// Token: 0x04004E63 RID: 20067
		[SerializeField]
		private CToggle likeToggle;

		// Token: 0x04004E64 RID: 20068
		[SerializeField]
		private CToggle unLikeToggle;

		// Token: 0x04004E65 RID: 20069
		[SerializeField]
		private CButton shareBtn;

		// Token: 0x04004E66 RID: 20070
		[SerializeField]
		private CButton jumpBtn;

		// Token: 0x04004E67 RID: 20071
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x04004E68 RID: 20072
		[SerializeField]
		private CButton showInExplorerBtn;

		// Token: 0x04004E69 RID: 20073
		[SerializeField]
		private CToggle subscribeToggle;

		// Token: 0x04004E6A RID: 20074
		private bool _inited;

		// Token: 0x04004E6B RID: 20075
		private ModInfoWithDisplayData _data;

		// Token: 0x04004E6C RID: 20076
		private string _originName;

		// Token: 0x04004E6D RID: 20077
		private ModId _originId;

		// Token: 0x04004E6E RID: 20078
		private Action _onRefresh;

		// Token: 0x04004E6F RID: 20079
		private const string DropdownSettingItemPoolKey = "DropdownSettingItem";

		// Token: 0x04004E70 RID: 20080
		private const string InputFieldSettingItemPoolKey = "InputFieldSettingItem";

		// Token: 0x04004E71 RID: 20081
		private const string SliderSettingItemPoolKey = "SliderSettingItem";

		// Token: 0x04004E72 RID: 20082
		private const string ToggleSettingItemPoolKey = "ToggleSettingItem";

		// Token: 0x04004E73 RID: 20083
		private readonly List<SettingItemBase> _settingItems = new List<SettingItemBase>();

		// Token: 0x04004E74 RID: 20084
		private readonly List<CToggle> _groupToggles = new List<CToggle>();

		// Token: 0x04004E75 RID: 20085
		[TupleElementNames(new string[]
		{
			"GroupName",
			"Entries"
		})]
		private List<ValueTuple<string, List<SettingEntry>>> _groups;

		// Token: 0x04004E76 RID: 20086
		private int _currentGroupIndex;

		// Token: 0x04004E77 RID: 20087
		private bool _isSyncing;

		// Token: 0x04004E78 RID: 20088
		private bool _isSettingChanged;

		// Token: 0x04004E79 RID: 20089
		private bool _needRestart;

		// Token: 0x04004E7A RID: 20090
		private bool _localModExists;

		// Token: 0x04004E7B RID: 20091
		private bool _isShowSettings;
	}
}
