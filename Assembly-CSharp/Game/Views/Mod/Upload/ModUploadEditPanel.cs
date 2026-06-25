using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.ModSystem;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using GameData.Domains.Mod;
using GameData.Utilities;
using Steamworks;
using TMPro;
using UnityEngine;

namespace Game.Views.Mod.Upload
{
	// Token: 0x020008D9 RID: 2265
	public class ModUploadEditPanel : MonoBehaviour
	{
		// Token: 0x17000CAF RID: 3247
		// (get) Token: 0x06006C6D RID: 27757 RVA: 0x0031FA76 File Offset: 0x0031DC76
		// (set) Token: 0x06006C6E RID: 27758 RVA: 0x0031FA7E File Offset: 0x0031DC7E
		public bool IsEditingUploadMod { get; set; }

		// Token: 0x17000CB0 RID: 3248
		// (get) Token: 0x06006C6F RID: 27759 RVA: 0x0031FA87 File Offset: 0x0031DC87
		private bool CurEditModIsNotCreated
		{
			get
			{
				return ModManager.UploadedMods.All((ModId u) => u.FileId != this._curEditModInfo.ModId.FileId);
			}
		}

		// Token: 0x06006C70 RID: 27760 RVA: 0x0031FAA0 File Offset: 0x0031DCA0
		public void Init(Action onClickSync)
		{
			this._onClickSync = onClickSync;
			this._targetDetailImageToggleKey = -1;
			this.setProgramPanel.Init();
			this.setProgramPanel.Hide();
			this.buttonProgram.ClearAndAddListener(new Action(this.OnClickSetProgram));
			this.editSettingPanel.Init();
			this.editSettingPanel.Hide();
			this.settingPanel.Init(new Action<List<string>, List<SettingEntry>>(this.OnConfirmSetting));
			this.settingPanel.Hide(false, null);
			this.buttonSetting.ClearAndAddListener(new Action(this.OnClickSetting));
			CScrollbar scrollbar = this.descriptionInputField.verticalScrollbar as CScrollbar;
			bool flag = scrollbar != null;
			if (flag)
			{
				scrollbar.DisableHover = true;
			}
			this.setDependencePanel.Init();
			this.setDependencePanel.Hide();
			this.buttonDependence.ClearAndAddListener(new Action(this.OnClickDependency));
			this.nameInputField.onValueChanged.RemoveAllListeners();
			this.nameInputField.onValueChanged.AddListener(delegate(string value)
			{
				bool flag2 = this._curEditModInfo != null && value != this._curEditModInfo.Title;
				if (flag2)
				{
					this.IsEditingUploadMod = true;
				}
				this.RefreshUploadWarningMark();
			});
			this.versionInputField.onEndEdit.RemoveAllListeners();
			this.versionInputField.onEndEdit.AddListener(delegate(string value)
			{
				ulong version = ModManager.VersionStringToUlong(value);
				string versionStr = ModManager.VersionUlongToString(version);
				this.versionInputField.SetTextWithoutNotify(versionStr);
				bool flag2 = this._curEditModInfo != null && version != this._curEditModInfo.ModId.Version;
				if (flag2)
				{
					this.IsEditingUploadMod = true;
				}
				this.RefreshUploadWarningMark();
			});
			this.gameVersionInputField.onEndEdit.RemoveAllListeners();
			this.gameVersionInputField.onEndEdit.AddListener(delegate(string value)
			{
				ulong version = ModManager.VersionStringToUlong(value);
				string versionStr = ModManager.VersionUlongToString(version);
				this.gameVersionInputField.SetTextWithoutNotify(versionStr);
				bool flag2 = this._curEditModInfo != null && versionStr != this._curEditModInfo.GameVersionStr;
				if (flag2)
				{
					this.IsEditingUploadMod = true;
				}
			});
			this.textCurGameVersion.text = LanguageKey.LK_Mod_GameVersion_Current.TrFormat(GameApp.Instance.GameVersion);
			this.visibilityDropdown.ClearOptions();
			this.visibilityDropdown.AddOptions(SteamManager.VisibilityOptionList);
			this.visibilityDropdown.onValueChanged.RemoveAllListeners();
			this.visibilityDropdown.onValueChanged.AddListener(delegate(int value)
			{
				bool flag2 = this._curEditModInfo != null && value != this._curEditModInfo.Visibility.ToInt();
				if (flag2)
				{
					this.IsEditingUploadMod = true;
				}
			});
			this.tagToggleGroupMultiSelect.Init();
			this.tagToggleGroupMultiSelect.OnActiveIndexChange -= this.TagToggleGroupMultiSelectIndexChange;
			this.tagToggleGroupMultiSelect.OnActiveIndexChange += this.TagToggleGroupMultiSelectIndexChange;
			this.tagFilterButton.onClick.ResetListener(delegate()
			{
				this.tagToggleGroupMultiSelect.gameObject.SetActive(!this.tagToggleGroupMultiSelect.gameObject.activeSelf);
			});
			this.clearTagButton.onClick.ResetListener(delegate()
			{
				this.tagToggleGroupMultiSelect.DeSelectAll(true);
			});
			this.tagsFilterMask.onClick.ResetListener(delegate()
			{
				this.tagToggleGroupMultiSelect.gameObject.SetActive(false);
			});
			this.changeConfigToggle.onValueChanged.ResetListener(delegate(bool isOn)
			{
				bool flag2 = isOn != this._curEditModInfo.ChangeConfig;
				if (flag2)
				{
					this.IsEditingUploadMod = true;
				}
			});
			this.hasArchiveToggle.onValueChanged.ResetListener(delegate(bool isOn)
			{
				bool flag2 = isOn != this._curEditModInfo.HasArchive;
				if (flag2)
				{
					this.IsEditingUploadMod = true;
				}
			});
			this.needRestartToggle.onValueChanged.ResetListener(delegate(bool isOn)
			{
				bool flag2 = isOn != this._curEditModInfo.NeedRestartWhenSettingChanged;
				if (flag2)
				{
					this.IsEditingUploadMod = true;
				}
			});
			this.buttonDeleteLocal.ClearAndAddListener(new Action(this.OnClickDelete));
			this.buttonDeleteRemote.ClearAndAddListener(new Action(this.OnClickDelete));
			this.buttonUpload.ClearAndAddListener(new Action(this.OnClickUpload));
			this.buttonUpdate.ClearAndAddListener(new Action(this.OnClickUpload));
			this.buttonSync.ClearAndAddListener(new Action(this.OnClickSync));
			this.buttonSave.ClearAndAddListener(new Action(this.OnClickSave));
			this.buttonOpenExplorer.ClearAndAddListener(new Action(this.OnClickOpenExplorer));
		}

		// Token: 0x06006C71 RID: 27761 RVA: 0x0031FE1C File Offset: 0x0031E01C
		public void Refresh(ModInfoWithDisplayData modInfo)
		{
			ModSetProgramPanel.Clear();
			this._showUploadWarningMark = false;
			this.RefreshUploadWarningMark();
			this.ClearTempUploadModTextures();
			this._curEditModInfo = modInfo;
			bool flag = !modInfo.ModId.IsValid;
			if (flag)
			{
				this.IsEditingUploadMod = true;
			}
			this._tempModDetailImageFileNameList.Clear();
			this._tempModDetailImageFileNameList.AddRange(this._curEditModInfo.DetailImageList);
			this._tempModDetailImageFilePathList.Clear();
			this._tempModDetailImageFilePathList.AddRange(this._curEditModInfo.DetailImageList);
			this._tempModCoverFileName = this._curEditModInfo.Cover;
			this._tempModSettingEntries.Clear();
			this._tempModSettingEntries.AddRange(this._curEditModInfo.ModSettingEntries);
			this._tempModSettingGroups.Clear();
			this._tempModSettingGroups.AddRange(this._curEditModInfo.ModSettingGroups);
			this._tempFrontendPlugins.Clear();
			this._tempFrontendPlugins.AddRange(this._curEditModInfo.FrontendPlugins);
			this._tempBackendPlugins.Clear();
			this._tempBackendPlugins.AddRange(this._curEditModInfo.BackendPlugins);
			this._tempModDependencyList.Clear();
			this._tempModDependencyList.AddRange(from fileId in this._curEditModInfo.RemoteDependencies
			where fileId != modInfo.ModId.FileId
			select fileId);
			this.nameInputField.SetTextWithoutNotify(modInfo.Title);
			string versionString = ModManager.VersionUlongToString(modInfo.ModId.Version);
			this.versionInputField.SetTextWithoutNotify(versionString);
			ulong gameVersion = ModManager.VersionStringToUlong(modInfo.GameVersionStr);
			this.gameVersionInputField.SetTextWithoutNotify((gameVersion > 0UL) ? modInfo.GameVersionStr : GameApp.Instance.GameVersion);
			string author = this._curEditModInfo.Author.IsNullOrEmpty() ? SteamFriends.GetPersonaName() : this._curEditModInfo.Author;
			this.authorValue.text = "<u>" + author + "</u>";
			this.visibilityDropdown.value = modInfo.Visibility.ToInt();
			this._tempModUsedTagList.Clear();
			List<string> tagList = modInfo.TagList;
			int tagCount = (tagList != null) ? tagList.Count : 0;
			bool flag2 = tagCount > 0;
			if (flag2)
			{
				this._tempModUsedTagList.AddRange(SteamManager.GetTagContentList(modInfo.TagList));
			}
			this.RefreshUploadModTag();
			this.RefreshCover(this.modImageInfo, modInfo);
			this.descriptionInputField.SetTextWithoutNotify(modInfo.Description);
			bool isRemote = ModManager.UploadedMods.Exists((ModId u) => u.FileId == this._curEditModInfo.ModId.FileId);
			this.buttonDeleteLocal.gameObject.SetActive(!isRemote);
			this.buttonDeleteRemote.gameObject.SetActive(isRemote);
			this.buttonUpload.gameObject.SetActive(!isRemote);
			this.buttonUpdate.gameObject.SetActive(isRemote);
			this.buttonSync.gameObject.SetActive(isRemote);
			this.changeConfigToggle.isOn = modInfo.ChangeConfig;
			this.hasArchiveToggle.isOn = modInfo.HasArchive;
			this.needRestartToggle.isOn = modInfo.NeedRestartWhenSettingChanged;
			this.RefreshUploadModButtonOpenExplorer();
		}

		// Token: 0x06006C72 RID: 27762 RVA: 0x003201B8 File Offset: 0x0031E3B8
		public void Clear()
		{
			this._curEditModInfo = null;
			this.nameInputField.text = string.Empty;
			this.versionInputField.text = string.Empty;
			this.gameVersionInputField.text = string.Empty;
			this.descriptionInputField.text = string.Empty;
			this._tempModUsedTagList.Clear();
			this.RefreshUploadModTag();
			this.visibilityDropdown.value = EModVisibility.Public.ToInt();
			this.ClearCover();
			this.RefreshUploadModButtonOpenExplorer();
			this._tempModCreateFromDirectoryPath = null;
			this.IsEditingUploadMod = false;
		}

		// Token: 0x06006C73 RID: 27763 RVA: 0x00320258 File Offset: 0x0031E458
		public void Cancel(Action onConfirm, Action onCancel = null)
		{
			bool isEditingUploadMod = this.IsEditingUploadMod;
			if (isEditingUploadMod)
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Mod_CancelEdit_Title);
				string content = LocalStringManager.Get(LanguageKey.LK_Mod_CancelEdit_Content);
				CommonUtils.ShowConfirmDialog(title, content, delegate
				{
					this.Clear();
					Action onConfirm3 = onConfirm;
					if (onConfirm3 != null)
					{
						onConfirm3();
					}
				}, onCancel, EDialogType.None);
			}
			else
			{
				this.Clear();
				Action onConfirm2 = onConfirm;
				if (onConfirm2 != null)
				{
					onConfirm2();
				}
			}
		}

		// Token: 0x06006C74 RID: 27764 RVA: 0x003202D0 File Offset: 0x0031E4D0
		public bool QuickHide()
		{
			bool activeSelf = this.editSettingPanel.gameObject.activeSelf;
			bool result;
			if (activeSelf)
			{
				this.editSettingPanel.Hide();
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
				result = true;
			}
			else
			{
				bool activeSelf2 = this.settingPanel.gameObject.activeSelf;
				if (activeSelf2)
				{
					this.settingPanel.Hide(true, delegate
					{
						AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
					});
					result = true;
				}
				else
				{
					bool activeSelf3 = this.setDependencePanel.gameObject.activeSelf;
					if (activeSelf3)
					{
						this.setDependencePanel.Hide();
						AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
						result = true;
					}
					else
					{
						bool activeSelf4 = this.setProgramPanel.gameObject.activeSelf;
						if (activeSelf4)
						{
							this.setProgramPanel.Hide();
							AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
							result = true;
						}
						else
						{
							bool activeSelf5 = this.modUploadConfirmDialog.gameObject.activeSelf;
							if (activeSelf5)
							{
								this.modUploadConfirmDialog.Hide();
								AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
								result = true;
							}
							else
							{
								result = false;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06006C75 RID: 27765 RVA: 0x00320410 File Offset: 0x0031E610
		public bool CanChangeTab()
		{
			bool activeSelf = this.editSettingPanel.gameObject.activeSelf;
			bool result;
			if (activeSelf)
			{
				result = false;
			}
			else
			{
				bool activeSelf2 = this.settingPanel.gameObject.activeSelf;
				if (activeSelf2)
				{
					result = false;
				}
				else
				{
					bool activeSelf3 = this.setDependencePanel.gameObject.activeSelf;
					if (activeSelf3)
					{
						result = false;
					}
					else
					{
						bool activeSelf4 = this.setProgramPanel.gameObject.activeSelf;
						if (activeSelf4)
						{
							result = false;
						}
						else
						{
							bool activeSelf5 = this.modUploadConfirmDialog.gameObject.activeSelf;
							result = !activeSelf5;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06006C76 RID: 27766 RVA: 0x003204A0 File Offset: 0x0031E6A0
		private bool CheckIsReadyToSave(out bool nameIsDuplicated)
		{
			return this.CheckModNameIsValid(out nameIsDuplicated) && this.CheckModVersionIsValid();
		}

		// Token: 0x06006C77 RID: 27767 RVA: 0x003204C4 File Offset: 0x0031E6C4
		private bool CheckModNameIsValid(out bool isDuplicated)
		{
			isDuplicated = false;
			string text = this.nameInputField.text.Trim();
			bool flag = text.IsNullOrEmpty();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = ModManager.LocalMods.Any((KeyValuePair<string, ModInfoWithDisplayData> pair) => pair.Value.Title == text && this._curEditModInfo.Title != text);
				if (flag2)
				{
					isDuplicated = true;
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06006C78 RID: 27768 RVA: 0x00320534 File Offset: 0x0031E734
		private bool CheckModVersionIsValid()
		{
			ulong version = ModManager.VersionStringToUlong(this.versionInputField.text.Trim());
			return version > 0UL;
		}

		// Token: 0x06006C79 RID: 27769 RVA: 0x00320564 File Offset: 0x0031E764
		private void RefreshUploadWarningMark()
		{
			bool flag;
			this.nameWarningMark.SetActive(this._showUploadWarningMark && !this.CheckModNameIsValid(out flag));
			this.versionWarningMark.SetActive(this._showUploadWarningMark && !this.CheckModVersionIsValid());
		}

		// Token: 0x06006C7A RID: 27770 RVA: 0x003205B4 File Offset: 0x0031E7B4
		private void RefreshCover(ModImageInfo imageInfo, ModInfoWithDisplayData modInfo)
		{
			ModUploadEditPanel.<>c__DisplayClass69_0 CS$<>8__locals1 = new ModUploadEditPanel.<>c__DisplayClass69_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.imageInfo = imageInfo;
			CS$<>8__locals1.modInfo = modInfo;
			CS$<>8__locals1.imageToggleGroup = CS$<>8__locals1.imageInfo.imageToggleGroup;
			CS$<>8__locals1.coverImageBack = CS$<>8__locals1.imageInfo.coverImageBack;
			CS$<>8__locals1.detailImageBack = CS$<>8__locals1.imageInfo.detailImageBack;
			CButton buttonSetImage = CS$<>8__locals1.imageInfo.buttonSetImage;
			CButton buttonChangeImage = CS$<>8__locals1.imageInfo.buttonChangeImage;
			CS$<>8__locals1.detailImageToggleGroup = CS$<>8__locals1.detailImageBack.detailImageToggleGroup;
			CS$<>8__locals1.imageToggleGroup.Init(-1);
			CS$<>8__locals1.imageToggleGroup.OnActiveIndexChange -= this.ImageToggleGroupOnActiveIndexChange;
			CS$<>8__locals1.imageToggleGroup.OnActiveIndexChange += this.ImageToggleGroupOnActiveIndexChange;
			CS$<>8__locals1.imageToggleGroup.Set(0, true);
			bool isCover = CS$<>8__locals1.imageToggleGroup.GetActiveIndex() == 0;
			this.RefreshButtonSetImage(CS$<>8__locals1.imageInfo, isCover);
			buttonSetImage.ClearAndAddListener(new Action(CS$<>8__locals1.<RefreshCover>g__Action|0));
			buttonChangeImage.ClearAndAddListener(new Action(CS$<>8__locals1.<RefreshCover>g__Action|0));
		}

		// Token: 0x06006C7B RID: 27771 RVA: 0x003206CC File Offset: 0x0031E8CC
		private void ImageToggleGroupOnActiveIndexChange(int newTog, int oldTog)
		{
			this._targetDetailImageToggleKey = -1;
			bool isCover = newTog == 0;
			this.modImageInfo.coverImageBack.gameObject.SetActive(isCover);
			this.modImageInfo.detailImageBack.gameObject.SetActive(!isCover);
			bool flag = isCover;
			if (flag)
			{
				this.RefreshCoverImage(this.modImageInfo.coverImageBack, this._curEditModInfo, true);
			}
			else
			{
				this.RefreshDetailImage(this.modImageInfo, this._curEditModInfo, true);
			}
			this.RefreshButtonSetImage(this.modImageInfo, isCover);
		}

		// Token: 0x06006C7C RID: 27772 RVA: 0x0032075C File Offset: 0x0031E95C
		private bool RefreshCoverImage(ModImageInfoCover refers, ModInfoWithDisplayData modInfo, bool useTempCover = false)
		{
			CRawImage coverImg = refers.coverImage;
			bool flag = modInfo == null;
			bool result;
			if (flag)
			{
				coverImg.enabled = false;
				result = false;
			}
			else
			{
				bool hasTempCover = useTempCover && null != this._tempModCoverTexture;
				Texture2D localCover = ModManager.GetModCoverTexture(modInfo.ModId);
				bool hasLocalCover = localCover != null;
				bool hasModCoverTextureCache = ModManager.HasPreviewModCoverTexture(modInfo.ModId);
				bool showCover = hasTempCover || hasLocalCover || hasModCoverTextureCache;
				coverImg.enabled = showCover;
				bool flag2 = showCover;
				if (flag2)
				{
					bool flag3 = hasTempCover;
					if (flag3)
					{
						coverImg.texture = this._tempModCoverTexture;
					}
					else
					{
						bool flag4 = hasLocalCover;
						if (flag4)
						{
							coverImg.texture = localCover;
						}
						else
						{
							bool flag5 = hasModCoverTextureCache;
							if (flag5)
							{
								coverImg.texture = ModManager.GetPreviewModCoverTexture(modInfo.ModId);
							}
						}
					}
					this.ResetCoverSize(refers);
				}
				bool flag6 = !showCover && modInfo.PreviewFileHandle != UGCHandle_t.Invalid;
				if (flag6)
				{
					SteamManager.DownloadPreviewCoverImage(modInfo);
				}
				bool flag7 = !this._tempModCoverFileName.IsNullOrEmpty() && modInfo.Cover != this._tempModCoverFileName;
				if (flag7)
				{
					this.IsEditingUploadMod = true;
				}
				result = coverImg.enabled;
			}
			return result;
		}

		// Token: 0x06006C7D RID: 27773 RVA: 0x00320880 File Offset: 0x0031EA80
		private bool RefreshDetailImage(ModImageInfo imageInfo, ModInfoWithDisplayData modInfo, bool init)
		{
			ModImageInfoDetail detailImageBack = imageInfo.detailImageBack;
			CToggleGroup detailImageToggleGroup = detailImageBack.detailImageToggleGroup;
			CRawImage coverImage = detailImageBack.coverImage;
			for (int i = 0; i < this._tempModDetailImageFileNameList.Count; i++)
			{
				bool flag = i < detailImageToggleGroup.transform.childCount;
				if (flag)
				{
					Transform child = detailImageToggleGroup.transform.GetChild(i);
					child.gameObject.SetActive(true);
					CToggle toggle = child.GetComponent<CToggle>();
					foreach (TextMeshProUGUI label in toggle.GetComponentsInChildren<TextMeshProUGUI>())
					{
						label.text = (i + 1).ToString();
					}
				}
			}
			for (int j = this._tempModDetailImageFileNameList.Count; j < detailImageToggleGroup.transform.childCount; j++)
			{
				detailImageToggleGroup.transform.GetChild(j).gameObject.SetActive(false);
			}
			detailImageToggleGroup.Clear();
			detailImageToggleGroup.AddAllChildToggles();
			detailImageToggleGroup.Init(-1);
			detailImageToggleGroup.ClearOnActiveIndexChange();
			detailImageToggleGroup.OnActiveIndexChange += delegate(int newTog, int oldTog)
			{
				this._targetDetailImageToggleKey = newTog;
				Texture2D texture = null;
				bool flag5 = this._tempModDetailImageFileNameList.CheckIndex(newTog);
				if (flag5)
				{
					string textureName = this._tempModDetailImageFileNameList[newTog];
					bool flag6 = !textureName.IsNullOrEmpty();
					if (flag6)
					{
						bool flag7 = !this._tempModDetailImageDict.TryGetValue(textureName, out texture);
						if (flag7)
						{
							texture = ModManager.GetModDetailTexture(modInfo.ModId, textureName);
						}
					}
					bool flag8 = !texture;
					if (flag8)
					{
						IReadOnlyList<Texture2D> list = ModManager.GetDetailModCoverTexture(modInfo.ModId);
						bool flag9 = list != null && list.Count > newTog;
						if (flag9)
						{
							texture = list[newTog];
						}
					}
				}
				coverImage.texture = texture;
				bool flag10 = texture;
				if (flag10)
				{
					coverImage.enabled = true;
					this.ResetCoverSize(detailImageBack);
				}
				else
				{
					coverImage.enabled = false;
				}
				this.RefreshButtonSetImage(imageInfo, false);
			};
			bool flag2 = init && this._tempModDetailImageFileNameList.Count > 0;
			if (flag2)
			{
				this._targetDetailImageToggleKey = 0;
			}
			detailImageToggleGroup.Set(this._targetDetailImageToggleKey, true);
			bool flag3 = !detailImageToggleGroup.AnyTogglesOn();
			if (flag3)
			{
				coverImage.enabled = false;
			}
			CButton buttonAddImage = detailImageBack.buttonAddImage;
			bool showAdd = this._tempModDetailImageFileNameList.Count < detailImageToggleGroup.transform.childCount;
			buttonAddImage.gameObject.SetActive(showAdd);
			buttonAddImage.ClearAndAddListener(delegate
			{
				this.IsEditingUploadMod = true;
				this._tempModDetailImageFileNameList.Add(string.Empty);
				this._tempModDetailImageFilePathList.Add(string.Empty);
				this._targetDetailImageToggleKey = this._tempModDetailImageFileNameList.Count - 1;
				this.RefreshDetailImage(imageInfo, modInfo, false);
			});
			buttonAddImage.interactable = !this._tempModDetailImageFileNameList.Contains(string.Empty);
			CButton buttonRemoveImage = detailImageBack.buttonRemoveImage;
			int curDetailImageIndex = (detailImageToggleGroup.GetActiveIndex() >= 0) ? detailImageToggleGroup.GetActiveIndex() : 0;
			bool showRemove = this._tempModDetailImageFileNameList.CheckIndex(curDetailImageIndex);
			buttonRemoveImage.gameObject.SetActive(showRemove);
			buttonRemoveImage.ClearAndAddListener(delegate
			{
				this.IsEditingUploadMod = true;
				int index = (detailImageToggleGroup.GetActiveIndex() >= 0) ? detailImageToggleGroup.GetActiveIndex() : 0;
				string fileName = this._tempModDetailImageFileNameList[index];
				this._tempModDetailImageFileNameList.RemoveAt(index);
				this._tempModDetailImageFilePathList.RemoveAt(index);
				Texture2D texture;
				bool flag5 = !this._tempModDetailImageFileNameList.Contains(fileName) && this._tempModDetailImageDict.TryGetValue(fileName, out texture);
				if (flag5)
				{
					Object.Destroy(texture);
					this._tempModDetailImageDict.Remove(fileName);
				}
				this._targetDetailImageToggleKey = index - 1;
				this.RefreshDetailImage(imageInfo, modInfo, false);
				this.RefreshButtonSetImage(imageInfo, false);
			});
			bool flag4 = this._curEditModInfo != null && this._tempModDetailImageFileNameList.ContentIsDifferent(this._curEditModInfo.DetailImageList);
			if (flag4)
			{
				this.IsEditingUploadMod = true;
			}
			return coverImage.enabled;
		}

		// Token: 0x06006C7E RID: 27774 RVA: 0x00320B70 File Offset: 0x0031ED70
		private void RefreshButtonSetImage(ModImageInfo imageInfo, bool isCover)
		{
			ModImageInfoDetail detailImageBack = imageInfo.detailImageBack;
			CToggleGroup detailImageToggleGroup = detailImageBack.detailImageToggleGroup;
			bool canSetImage = isCover || detailImageToggleGroup.GetActiveIndex() >= 0;
			CButton buttonSetImage = imageInfo.buttonSetImage;
			buttonSetImage.interactable = canSetImage;
			bool isValid = isCover ? imageInfo.coverImageBack.coverImage.enabled : imageInfo.detailImageBack.coverImage.enabled;
			imageInfo.buttonSetImage.gameObject.SetActive(!isValid);
			imageInfo.buttonChangeImage.gameObject.SetActive(isValid);
		}

		// Token: 0x06006C7F RID: 27775 RVA: 0x00320C00 File Offset: 0x0031EE00
		private void SetImagePath(ModImageInfo imageInfo, ModImageInfoCover refers, string filePath)
		{
			CRawImage coverImg = refers.coverImage;
			CToggleGroup imageToggleGroup = imageInfo.imageToggleGroup;
			bool isCover = imageToggleGroup.GetActiveIndex() == 0;
			coverImg.gameObject.SetActive(true);
			byte[] buffer = File.ReadAllBytes(filePath);
			Texture2D texture = new Texture2D(0, 0);
			bool loaded = false;
			try
			{
				loaded = texture.LoadImage(buffer);
				bool flag = isCover;
				if (flag)
				{
					bool flag2 = this._tempModCoverTexture;
					if (flag2)
					{
						Object.Destroy(this._tempModCoverTexture);
					}
					this._tempModCoverTexture = texture;
				}
				else
				{
					ModImageInfoDetail detail = refers as ModImageInfoDetail;
					bool flag3 = detail != null;
					if (flag3)
					{
						CToggleGroup detailImageToggleGroup = detail.detailImageToggleGroup;
						int index = detailImageToggleGroup.GetActiveIndex();
						string name = this._tempModDetailImageFileNameList[index];
						this._tempModDetailImageDict[name] = texture;
					}
				}
				coverImg.texture = texture;
				coverImg.enabled = true;
				this.ResetCoverSize(refers);
				this.RefreshButtonSetImage(imageInfo, isCover);
			}
			catch (Exception e)
			{
				GLog.TagError("UI_ModPanel", e.ToString(), Array.Empty<object>());
				throw;
			}
			finally
			{
				bool flag4 = !loaded;
				if (flag4)
				{
					this.ClearCover();
				}
			}
		}

		// Token: 0x06006C80 RID: 27776 RVA: 0x00320D40 File Offset: 0x0031EF40
		private void ResetCoverSize(ModImageInfoCover refers)
		{
			CRawImage coverImg = refers.coverImage;
			Texture texture = coverImg.texture;
			bool flag = !texture || texture.width == 0 || texture.height == 0;
			if (!flag)
			{
				RectTransform coverImgRectTrans = coverImg.GetComponent<RectTransform>();
				Vector2 anchor = Vector2.one * 0.5f;
				coverImgRectTrans.SetAnchor(anchor, anchor);
				coverImgRectTrans.SetSize(new Vector2((float)texture.width, (float)texture.height));
				float widthScale = refers.coverImageBorder.rect.width / (float)texture.width;
				float heightScale = refers.coverImageBorder.rect.height / (float)texture.height;
				coverImgRectTrans.localScale = Vector3.one * Mathf.Min(widthScale, heightScale);
			}
		}

		// Token: 0x06006C81 RID: 27777 RVA: 0x00320E14 File Offset: 0x0031F014
		private void ClearCover()
		{
			this._tempModCoverFileName = string.Empty;
			this._tempModCoverFilePath = string.Empty;
			this._tempModDetailImageFileNameList.Clear();
			this._tempModDetailImageFilePathList.Clear();
			this.modImageInfo.imageToggleGroup.SetWithoutNotify(0);
			this.RefreshCover(this.modImageInfo, null);
			this.ClearTempUploadModTextures();
		}

		// Token: 0x06006C82 RID: 27778 RVA: 0x00320E78 File Offset: 0x0031F078
		private void ClearTempUploadModTextures()
		{
			bool flag = this._tempModCoverTexture;
			if (flag)
			{
				Object.Destroy(this._tempModCoverTexture);
				this._tempModCoverTexture = null;
			}
			foreach (KeyValuePair<string, Texture2D> pair in this._tempModDetailImageDict)
			{
				bool flag2 = pair.Value;
				if (flag2)
				{
					Object.Destroy(this._tempModCoverTexture);
				}
			}
			this._tempModDetailImageDict.Clear();
		}

		// Token: 0x06006C83 RID: 27779 RVA: 0x00320F14 File Offset: 0x0031F114
		private void RefreshUploadModTag()
		{
			for (int index2 = 0; index2 < SteamManager.AllTagList.Count; index2++)
			{
				this.tagToggleGroupMultiSelect.DeSelectWithoutNotify(index2);
			}
			Transform template = this.tagLayout.GetChild(0);
			for (int i = 0; i < this._tempModUsedTagList.Count; i++)
			{
				Transform child = (i < this.tagLayout.childCount) ? this.tagLayout.GetChild(i) : Object.Instantiate<Transform>(template, this.tagLayout);
				child.gameObject.SetActive(true);
				ModTagItem tagItem = child.GetComponent<ModTagItem>();
				int index = i;
				string tagContent = this._tempModUsedTagList[index];
				tagItem.Refresh(tagContent, delegate
				{
					this._tempModUsedTagList.RemoveAt(index);
					this.RefreshUploadModTag();
				});
				int keyIndex = SteamManager.AllTagList.IndexOf(tagContent);
				this.tagToggleGroupMultiSelect.SelectWithoutNotify(keyIndex);
			}
			for (int j = this._tempModUsedTagList.Count; j < this.tagLayout.childCount; j++)
			{
				this.tagLayout.GetChild(j).gameObject.SetActive(false);
			}
			this.tagLayout.gameObject.SetActive(this._tempModUsedTagList.Count > 0);
			bool flag = this._curEditModInfo != null;
			if (flag)
			{
				List<string> contentList = SteamManager.GetTagContentList(this._curEditModInfo.TagList);
				bool flag2 = this._tempModUsedTagList.ContentIsDifferent(contentList);
				if (flag2)
				{
					this.IsEditingUploadMod = true;
				}
			}
			this.RefreshUploadWarningMark();
		}

		// Token: 0x06006C84 RID: 27780 RVA: 0x003210C0 File Offset: 0x0031F2C0
		private void TagToggleGroupMultiSelectIndexChange(int arg1, int arg2)
		{
			List<int> activeIndices = this.tagToggleGroupMultiSelect.GetActiveIndices();
			this._tempModUsedTagList.Clear();
			this._tempModUsedTagList.AddRange(from i in activeIndices
			select SteamManager.AllTagList[i]);
			this.RefreshUploadModTag();
		}

		// Token: 0x06006C85 RID: 27781 RVA: 0x0032111E File Offset: 0x0031F31E
		private void OnClickSetting()
		{
			this.settingPanel.Show(this._tempModSettingGroups, this._tempModSettingEntries);
		}

		// Token: 0x06006C86 RID: 27782 RVA: 0x00321139 File Offset: 0x0031F339
		private void OnConfirmSetting(List<string> settingGroupList, List<SettingEntry> settingEntryList)
		{
			this._tempModSettingGroups.ClearAndAddRange(settingGroupList);
			this._tempModSettingEntries.ClearAndAddRange(settingEntryList);
		}

		// Token: 0x06006C87 RID: 27783 RVA: 0x00321156 File Offset: 0x0031F356
		private void OnClickSetProgram()
		{
			this.setProgramPanel.Show(this._curEditModInfo, this._tempFrontendPlugins, this._tempBackendPlugins, new Action(this.SaveMod));
		}

		// Token: 0x06006C88 RID: 27784 RVA: 0x00321183 File Offset: 0x0031F383
		private void OnClickDependency()
		{
			this.setDependencePanel.Show(this._curEditModInfo.ModId.FileId, this._tempModDependencyList);
		}

		// Token: 0x06006C89 RID: 27785 RVA: 0x003211A8 File Offset: 0x0031F3A8
		private void OnClickOpenExplorer()
		{
			this.OpenExplorer(this._curEditModInfo.DirectoryName);
		}

		// Token: 0x06006C8A RID: 27786 RVA: 0x003211C0 File Offset: 0x0031F3C0
		private void OpenExplorer(string path)
		{
			GEvent.OnEvent(UiEvents.OnModViewShowMask, null);
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, delegate
			{
				GEvent.OnEvent(UiEvents.OnModViewHideMask, null);
			});
			Process.Start("Explorer.exe", path);
		}

		// Token: 0x06006C8B RID: 27787 RVA: 0x0032121C File Offset: 0x0031F41C
		private void RefreshUploadModButtonOpenExplorer()
		{
			bool show = this._curEditModInfo != null && !this._curEditModInfo.DirectoryName.IsNullOrEmpty();
			this.buttonOpenExplorer.gameObject.SetActive(show);
		}

		// Token: 0x06006C8C RID: 27788 RVA: 0x0032125C File Offset: 0x0031F45C
		public void Import()
		{
			bool flag = this._curEditModInfo.DirectoryName.IsNullOrEmpty();
			if (flag)
			{
				this.<Import>g__Action|87_0();
			}
			else
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Mod_CreateFromDirectory_Warning_Title);
				string content = LocalStringManager.Get(LanguageKey.LK_Mod_CreateFromDirectory_Warning_Content);
				CommonUtils.ShowConfirmDialog(title, content, new Action(this.<Import>g__Action|87_0), null, EDialogType.None);
			}
		}

		// Token: 0x06006C8D RID: 27789 RVA: 0x003212BC File Offset: 0x0031F4BC
		private void OnClickSave()
		{
			bool nameIsDuplicated;
			bool flag = this.CheckIsReadyToSave(out nameIsDuplicated);
			if (flag)
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				string content = LocalStringManager.Get(LanguageKey.LK_Mod_Save_Confirm_Content);
				CommonUtils.ShowConfirmDialog(title, content, delegate
				{
					this.SaveMod();
					ModManager.UpdateModList(delegate
					{
						GEvent.OnEvent(UiEvents.OnModViewRefresh, null);
					});
				}, null, EDialogType.None);
			}
			else
			{
				string title2 = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				LanguageKey contentKey = nameIsDuplicated ? LanguageKey.LK_Mod_Save_NameIsDuplicated_Tip : LanguageKey.LK_Mod_Save_NotReady_Tip;
				string content2 = LocalStringManager.Get(contentKey);
				CommonUtils.ShowDialog(title2, content2, null, EDialogType.None);
				this._showUploadWarningMark = true;
				this.RefreshUploadWarningMark();
			}
		}

		// Token: 0x06006C8E RID: 27790 RVA: 0x00321348 File Offset: 0x0031F548
		private void SaveMod()
		{
			this.IsEditingUploadMod = false;
			string titleText = this.nameInputField.text.Trim();
			bool flag = this._curEditModInfo.DirectoryName.IsNullOrEmpty();
			if (flag)
			{
				this._curEditModInfo.DirectoryName = Path.Combine(ModManager.GetModRootFolder(), titleText.RemoveAllRichTags());
			}
			bool flag2 = !this._curEditModInfo.ModId.IsValid;
			if (flag2)
			{
				this._curEditModInfo.ModId = ModManager.CreateTempModId(Path.GetFileName(this._curEditModInfo.DirectoryName), true);
			}
			this._curEditModInfo.Title = titleText;
			this._curEditModInfo.Description = this.descriptionInputField.text;
			this._curEditModInfo.ModId.Version = ModManager.VersionStringToUlong(this.versionInputField.text);
			this._curEditModInfo.GameVersionStr = this.gameVersionInputField.text;
			this._curEditModInfo.Visibility = (EModVisibility)this.visibilityDropdown.value;
			this._curEditModInfo.TagList = SteamManager.GetTagKeyList(this._tempModUsedTagList);
			this._curEditModInfo.ModSettingEntries.Clear();
			this._curEditModInfo.ModSettingEntries.AddRange(this._tempModSettingEntries);
			this._curEditModInfo.ModSettingGroups.Clear();
			this._curEditModInfo.ModSettingGroups.AddRange(this._tempModSettingGroups);
			this._curEditModInfo.Dependencies.Clear();
			this._curEditModInfo.Dependencies.AddRange(this._tempModDependencyList);
			bool flag3 = this._curEditModInfo.Author.IsNullOrEmpty();
			if (flag3)
			{
				string personaName = SteamFriends.GetPersonaName();
				bool flag4 = !personaName.IsNullOrEmpty();
				if (flag4)
				{
					this._curEditModInfo.Author = personaName;
				}
			}
			string modRootFolder = Path.GetFullPath(ModManager.GetModRootFolder());
			string sourceFolderPath = this._tempModCreateFromDirectoryPath;
			string destinationFolderPath = Path.Combine(modRootFolder, Path.GetFileName(this._curEditModInfo.DirectoryName));
			bool flag5 = this._curEditModInfo.ModId.Source == 0 && !this._tempModCreateFromDirectoryPath.IsNullOrEmpty() && sourceFolderPath != destinationFolderPath;
			if (flag5)
			{
				bool flag6 = Directory.Exists(destinationFolderPath);
				if (flag6)
				{
					Directory.Delete(destinationFolderPath, true);
				}
				bool flag7 = !Directory.Exists(destinationFolderPath);
				if (flag7)
				{
					Directory.CreateDirectory(destinationFolderPath);
				}
				string[] rootFiles = Directory.GetFiles(sourceFolderPath, "*.*", SearchOption.AllDirectories);
				foreach (string file in rootFiles)
				{
					string relativePath = file.Substring(sourceFolderPath.Length).TrimStart(Path.DirectorySeparatorChar);
					string destFile = Path.Combine(destinationFolderPath, relativePath);
					bool flag8 = !Directory.Exists(Path.GetDirectoryName(destFile));
					if (flag8)
					{
						Directory.CreateDirectory(Path.GetDirectoryName(destFile));
					}
					bool flag9 = file != destFile;
					if (flag9)
					{
						File.Copy(file, destFile, true);
					}
				}
			}
			else
			{
				bool flag10 = !Directory.Exists(destinationFolderPath);
				if (flag10)
				{
					Directory.CreateDirectory(destinationFolderPath);
				}
			}
			this._tempModCreateFromDirectoryPath = null;
			this._curEditModInfo.Cover = this._tempModCoverFileName;
			this._curEditModInfo.WorkshopCover = this._tempModCoverFileName;
			this._curEditModInfo.DetailImageList.Clear();
			this._curEditModInfo.DetailImageList.AddRange(from s in this._tempModDetailImageFileNameList
			where !s.IsNullOrEmpty()
			select s);
			bool flag11 = !this._tempModCoverFilePath.IsNullOrEmpty() && File.Exists(this._tempModCoverFilePath);
			if (flag11)
			{
				string destCoverPath = Path.Combine(this._curEditModInfo.DirectoryName, this._tempModCoverFileName);
				bool flag12 = this._tempModCoverFilePath != destCoverPath;
				if (flag12)
				{
					File.Copy(this._tempModCoverFilePath, destCoverPath, true);
				}
			}
			for (int index = 0; index < this._tempModDetailImageFilePathList.Count; index++)
			{
				string src = this._tempModDetailImageFilePathList[index];
				bool flag13 = !src.IsNullOrEmpty() && File.Exists(src);
				if (flag13)
				{
					string destImagePath = Path.Combine(this._curEditModInfo.DirectoryName, this._tempModDetailImageFileNameList[index]);
					bool flag14 = src != destImagePath;
					if (flag14)
					{
						File.Copy(src, destImagePath, true);
					}
				}
			}
			bool needBackup = ModSetProgramPanel.NeedBackup;
			if (needBackup)
			{
				this.BackupModPlugins();
			}
			this.CopyModPlugins(this._tempFrontendPlugins);
			this.CopyModPlugins(this._tempBackendPlugins);
			List<string> originList = this._curEditModInfo.FrontendPlugins.Union(this._curEditModInfo.BackendPlugins).ToList<string>();
			List<string> tempList = this._tempFrontendPlugins.Union(this._tempBackendPlugins).ToList<string>();
			this.DeleteModPlugins(originList, tempList);
			this._curEditModInfo.FrontendPlugins.Clear();
			this._curEditModInfo.FrontendPlugins.AddRange(this._tempFrontendPlugins);
			this._curEditModInfo.BackendPlugins.Clear();
			this._curEditModInfo.BackendPlugins.AddRange(this._tempBackendPlugins);
			ModSetProgramPanel.Clear();
			this._curEditModInfo.ChangeConfig = this.changeConfigToggle.isOn;
			this._curEditModInfo.HasArchive = this.hasArchiveToggle.isOn;
			this._curEditModInfo.NeedRestartWhenSettingChanged = this.needRestartToggle.isOn;
			ModManager.SaveModInfo(this._curEditModInfo);
			ModManager.SaveModSettings(false);
			this.RefreshUploadModButtonOpenExplorer();
		}

		// Token: 0x06006C8F RID: 27791 RVA: 0x003218C0 File Offset: 0x0031FAC0
		private void BackupModPlugins()
		{
			ModUploadEditPanel.<>c__DisplayClass90_0 CS$<>8__locals1;
			CS$<>8__locals1.pluginDirectory = Path.Combine(this._curEditModInfo.DirectoryName, "Plugins");
			CS$<>8__locals1.legacyDirectory = Path.Combine(this._curEditModInfo.DirectoryName, "LegacyPlugins");
			bool flag = Directory.Exists(CS$<>8__locals1.legacyDirectory);
			if (flag)
			{
				Directory.Delete(CS$<>8__locals1.legacyDirectory, true);
			}
			bool flag2 = !Directory.Exists(CS$<>8__locals1.legacyDirectory);
			if (flag2)
			{
				Directory.CreateDirectory(CS$<>8__locals1.legacyDirectory);
			}
			ModUploadEditPanel.<BackupModPlugins>g__CopyFile|90_0(this._curEditModInfo.FrontendPlugins, ref CS$<>8__locals1);
			ModUploadEditPanel.<BackupModPlugins>g__CopyFile|90_0(this._curEditModInfo.BackendPlugins, ref CS$<>8__locals1);
			this._curEditModInfo.FrontendPluginsLegacy.Clear();
			this._curEditModInfo.FrontendPluginsLegacy.AddRange(this._curEditModInfo.FrontendPlugins);
			this._curEditModInfo.BackendPluginsLegacy.Clear();
			this._curEditModInfo.BackendPluginsLegacy.AddRange(this._curEditModInfo.BackendPlugins);
		}

		// Token: 0x06006C90 RID: 27792 RVA: 0x003219C4 File Offset: 0x0031FBC4
		private void CopyModPlugins(List<string> tempList)
		{
			string pluginDirectory = Path.Combine(this._curEditModInfo.DirectoryName, "Plugins");
			bool flag = !Directory.Exists(pluginDirectory);
			if (flag)
			{
				Directory.CreateDirectory(pluginDirectory);
			}
			for (int index = 0; index < tempList.Count; index++)
			{
				string fileName = tempList[index];
				string path;
				bool flag2 = !ModSetProgramPanel.TempFileNameToPathDict.TryGetValue(fileName, out path);
				if (!flag2)
				{
					bool flag3 = !File.Exists(path);
					if (flag3)
					{
						tempList[index] = string.Empty;
					}
					else
					{
						string newPath = Path.Combine(pluginDirectory, fileName);
						bool flag4 = path != newPath;
						if (flag4)
						{
							File.Copy(path, newPath, true);
						}
					}
				}
			}
			tempList.RemoveAll((string s) => s.IsNullOrEmpty());
		}

		// Token: 0x06006C91 RID: 27793 RVA: 0x00321AA0 File Offset: 0x0031FCA0
		private void DeleteModPlugins(List<string> originList, List<string> tempList)
		{
			string pluginDirectory = Path.Combine(this._curEditModInfo.DirectoryName, "Plugins");
			bool flag = !Directory.Exists(pluginDirectory);
			if (!flag)
			{
				string[] files = Directory.GetFiles(pluginDirectory);
				foreach (string filePath in files)
				{
					string fileName = Path.GetFileName(filePath);
					bool flag2 = originList.Contains(fileName) && !tempList.Contains(fileName);
					if (flag2)
					{
						File.Delete(filePath);
					}
				}
			}
		}

		// Token: 0x06006C92 RID: 27794 RVA: 0x00321B28 File Offset: 0x0031FD28
		private void OnClickUpload()
		{
			bool nameIsDuplicated;
			bool flag = this.CheckIsReadyToSave(out nameIsDuplicated);
			if (flag)
			{
				this.modUploadConfirmDialog.Show(this.CurEditModIsNotCreated, delegate(string log)
				{
					this.SaveMod();
					this._tempModUpdateLogList.Clear();
					bool flag2 = !log.IsNullOrEmpty();
					if (flag2)
					{
						this._tempModUpdateLogList.Add(log);
					}
					this.UploadMod(true);
				});
			}
			else
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				LanguageKey contentKey = nameIsDuplicated ? LanguageKey.LK_Mod_Save_NameIsDuplicated_Tip : LanguageKey.LK_Mod_Upload_NotReady_Tip;
				string content = LocalStringManager.Get(contentKey);
				CommonUtils.ShowDialog(title, content, null, EDialogType.None);
				this._showUploadWarningMark = true;
				this.RefreshUploadWarningMark();
			}
		}

		// Token: 0x06006C93 RID: 27795 RVA: 0x00321BA4 File Offset: 0x0031FDA4
		private void UploadMod(bool isEdit)
		{
			ModUploadEditPanel.<>c__DisplayClass95_0 CS$<>8__locals1 = new ModUploadEditPanel.<>c__DisplayClass95_0();
			CS$<>8__locals1.isEdit = isEdit;
			CS$<>8__locals1.<>4__this = this;
			SteamManager.IsEditMod = CS$<>8__locals1.isEdit;
			List<string> logList = new List<string>(this._tempModUpdateLogList);
			bool isEdit2 = CS$<>8__locals1.isEdit;
			if (isEdit2)
			{
				this.ExcludeModSettingFile();
			}
			GEvent.OnEvent(UiEvents.OnModViewShowMask, null);
			ERemoteStoragePublishedFileVisibility visibility = (ERemoteStoragePublishedFileVisibility)this._curEditModInfo.Visibility;
			bool curEditModIsNotCreated = this.CurEditModIsNotCreated;
			if (curEditModIsNotCreated)
			{
				SteamManager.CreateItem(this._curEditModInfo.DirectoryName, this._curEditModInfo, EWorkshopFileType.k_EWorkshopFileTypeFirst, visibility, logList, new Action<UGCUpdateHandle_t>(CS$<>8__locals1.<UploadMod>g__OnSucceed|0), new Action(CS$<>8__locals1.<UploadMod>g__OnFailed|1));
			}
			else
			{
				SteamManager.UploadItemUpdate(0UL, this._curEditModInfo.DirectoryName, this._curEditModInfo, visibility, logList, new Action<UGCUpdateHandle_t>(CS$<>8__locals1.<UploadMod>g__OnSucceed|0), new Action(CS$<>8__locals1.<UploadMod>g__OnFailed|1));
			}
		}

		// Token: 0x06006C94 RID: 27796 RVA: 0x00321C84 File Offset: 0x0031FE84
		private void UpdateUploadProgress(UGCUpdateHandle_t updateHandle, bool isEdit, Action onFailed)
		{
			base.StartCoroutine(this.CoroutineUploadMod(updateHandle, isEdit, onFailed));
		}

		// Token: 0x06006C95 RID: 27797 RVA: 0x00321C97 File Offset: 0x0031FE97
		private IEnumerator CoroutineUploadMod(UGCUpdateHandle_t updateHandle, bool isEdit, Action onFailed)
		{
			UI_FullScreenMask screenMask = UIElement.FullScreenMask.UiBaseAs<UI_FullScreenMask>();
			bool succeed = true;
			ulong bytesProcess;
			ulong byteTotal;
			for (;;)
			{
				EItemUpdateStatus progress = SteamUGC.GetItemUpdateProgress(updateHandle, out bytesProcess, out byteTotal);
				string progressMessage = LocalStringManager.Get(ModUploadEditPanel.UploadStatusMessages[progress]);
				screenMask.UpdateMessage(progressMessage);
				bool flag = progress == EItemUpdateStatus.k_EItemUpdateStatusInvalid && bytesProcess == byteTotal;
				if (flag)
				{
					break;
				}
				yield return null;
				progressMessage = null;
			}
			bool flag2 = byteTotal > 0UL;
			if (flag2)
			{
				succeed = (bytesProcess > 0UL);
			}
			bool flag3 = succeed;
			if (flag3)
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("ModId", this._curEditModInfo.ModId);
				GEvent.OnEvent(UiEvents.OnModViewDownload, args);
				string title = LocalStringManager.Get(LanguageKey.LK_Mod_Upload_Succeed_Title);
				string content = LocalStringManager.GetFormat(LanguageKey.LK_Mod_Upload_Succeed_Content, this._curEditModInfo.ModId.FileId, this._curEditModInfo.Title);
				CommonUtils.ShowDialog(title, content, null, EDialogType.None);
				this._tempModUpdateLogList.Clear();
				ModManager.UpdateModList(delegate
				{
					GEvent.OnEvent(UiEvents.OnModViewRefresh, null);
					bool flag4 = !isEdit;
					if (flag4)
					{
						this._curEditModInfo = null;
					}
				});
				args = null;
				title = null;
				content = null;
			}
			else
			{
				string title2 = LocalStringManager.Get(LanguageKey.LK_Steam_Fail_Title);
				string reason = LocalStringManager.Get(LanguageKey.LK_Unknow);
				string content2 = LocalStringManager.GetFormat(LanguageKey.LK_Steam_Fail_Content, reason);
				CommonUtils.ShowDialog(title2, content2, null, EDialogType.None);
				onFailed();
				title2 = null;
				reason = null;
				content2 = null;
			}
			yield break;
		}

		// Token: 0x06006C96 RID: 27798 RVA: 0x00321CBC File Offset: 0x0031FEBC
		private void ExcludeModSettingFile()
		{
			this._curEditModInfo.ModId.Source = 1;
			ModManager.SaveModInfo(this._curEditModInfo);
			string settingPath = Path.Combine(this._curEditModInfo.DirectoryName, "Settings.Lua");
			bool flag = File.Exists(settingPath);
			if (flag)
			{
				string tempDirectory = Path.Combine(ModManager.GetModRootFolder(), ".TempFileForUploading");
				bool flag2 = !Directory.Exists(tempDirectory);
				if (flag2)
				{
					Directory.CreateDirectory(tempDirectory);
				}
				DirectoryInfo tempDirectoryInfo = new DirectoryInfo(tempDirectory);
				tempDirectoryInfo.Attributes = (FileAttributes.Hidden | FileAttributes.Directory);
				string newSettingPath = Path.Combine(tempDirectory, "Settings.Lua");
				bool flag3 = File.Exists(newSettingPath);
				if (flag3)
				{
					File.Delete(newSettingPath);
				}
				File.Move(settingPath, newSettingPath);
			}
		}

		// Token: 0x06006C97 RID: 27799 RVA: 0x00321D6C File Offset: 0x0031FF6C
		private void RecoverModSettingFile()
		{
			this._curEditModInfo.ModId.Source = 0;
			ModManager.SaveModInfo(this._curEditModInfo);
			string tempDirectory = Path.Combine(ModManager.GetModRootFolder(), ".TempFileForUploading");
			bool flag = Directory.Exists(tempDirectory);
			if (flag)
			{
				string tempSettingPath = Path.Combine(tempDirectory, "Settings.Lua");
				bool flag2 = File.Exists(tempSettingPath);
				if (flag2)
				{
					string originSettingsPath = Path.Combine(this._curEditModInfo.DirectoryName, "Settings.Lua");
					bool flag3 = File.Exists(originSettingsPath);
					if (flag3)
					{
						File.Delete(originSettingsPath);
					}
					File.Move(tempSettingPath, originSettingsPath);
				}
			}
		}

		// Token: 0x06006C98 RID: 27800 RVA: 0x00321E00 File Offset: 0x00320000
		private void OnClickRemoveCurMod()
		{
			bool flag = this._curEditModInfo.ModId.Source == 1;
			if (flag)
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Mod_Remove_Subscribed_Tip_Title);
				string content = LocalStringManager.Get(LanguageKey.LK_Mod_Remove_Subscribed_Tip_Content);
				CommonUtils.ShowConfirmDialog(title, content, delegate
				{
					GEvent.OnEvent(UiEvents.OnModViewShowMask, null);
					ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("ModId", this._curEditModInfo.ModId);
					GEvent.OnEvent(UiEvents.OnModViewUnSubscribe, args);
					SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, delegate
					{
						GEvent.OnEvent(UiEvents.OnModViewRefresh, null);
					});
				}, null, EDialogType.None);
			}
			else
			{
				string title2 = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Local_Tip_Title);
				string content2 = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Local_Tip_Content);
				CommonUtils.ShowConfirmDialog(title2, content2, delegate
				{
					ModManager.DeleteLocalMod(this._curEditModInfo);
					GEvent.OnEvent(UiEvents.OnModViewRefresh, null);
				}, null, EDialogType.None);
			}
		}

		// Token: 0x06006C99 RID: 27801 RVA: 0x00321E88 File Offset: 0x00320088
		private void OnClickDelete()
		{
			bool flag = ModManager.UploadedMods.Exists((ModId u) => u.FileId == this._curEditModInfo.ModId.FileId);
			if (flag)
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Uploaded_Tip_Title);
				string content = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Uploaded_Tip_Content);
				CommonUtils.ShowConfirmDialog(title, content, delegate
				{
					this.IsEditingUploadMod = false;
					ModManager.UnSubscribeItem(this._curEditModInfo.ModId);
					ModManager.DeleteUploadedMod(this._curEditModInfo.ModId);
					ModManager.UpdateModList(delegate
					{
						GEvent.OnEvent(UiEvents.OnModViewRefresh, null);
						this.Refresh(this._curEditModInfo);
					});
				}, null, EDialogType.None);
			}
			else
			{
				string title2 = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Local_Tip_Title);
				string content2 = LocalStringManager.Get(LanguageKey.LK_Mod_Delete_Local_Tip_Content);
				CommonUtils.ShowConfirmDialog(title2, content2, delegate
				{
					this.IsEditingUploadMod = false;
					ModManager.DeleteLocalMod(this._curEditModInfo);
					ModManager.UpdateModList(delegate
					{
						GEvent.OnEvent(UiEvents.OnModViewRefresh, null);
						this.Clear();
					});
				}, null, EDialogType.None);
			}
		}

		// Token: 0x06006C9A RID: 27802 RVA: 0x00321F10 File Offset: 0x00320110
		private void OnClickSync()
		{
			this._onClickSync();
		}

		// Token: 0x06006CA8 RID: 27816 RVA: 0x00322234 File Offset: 0x00320434
		[CompilerGenerated]
		private void <Import>g__Action|87_0()
		{
			string path = LocalDialog.GetUnitySaveDir("Select your Mod directory", ModManager.GetModRootFolder());
			bool flag = string.IsNullOrEmpty(path) || !Directory.Exists(path);
			if (flag)
			{
				string title = LocalStringManager.Get("LK_Mod_Upload_Invalid_Dir_Title");
				string content = LocalStringManager.Get("LK_Mod_Upload_Invalid_Dir_Content");
				CommonUtils.ShowDialog(title, content, null, EDialogType.None);
			}
			else
			{
				this.IsEditingUploadMod = true;
				this._tempModCreateFromDirectoryPath = path;
				string configPath = Path.Combine(path, "Config.lua");
				bool flag2 = File.Exists(configPath);
				if (flag2)
				{
					ModId oldModId = this._curEditModInfo.ModId;
					string oldDirectoryName = this._curEditModInfo.DirectoryName.RemoveAllRichTags();
					this._curEditModInfo = ModManager.ReadModInfo(configPath, string.Empty, true, false);
					this._tempModCoverFilePath = (this._curEditModInfo.Cover.IsNullOrEmpty() ? string.Empty : Path.Combine(this._curEditModInfo.DirectoryName, this._curEditModInfo.Cover));
					bool isValid = oldModId.IsValid;
					if (isValid)
					{
						this._curEditModInfo.ModId.FileId = oldModId.FileId;
						this._curEditModInfo.ModId.Source = oldModId.Source;
						this._curEditModInfo.DirectoryName = oldDirectoryName;
					}
					else
					{
						this._curEditModInfo.ModId.FileId = 0UL;
						this._curEditModInfo.DirectoryName = null;
					}
					bool flag3 = !this._tempModCoverFilePath.IsNullOrEmpty();
					if (flag3)
					{
						ModManager.RemoveModCoverTexture(this._curEditModInfo.ModId);
						ModManager.AddModCoverTexture(this._curEditModInfo.ModId, this._tempModCoverFilePath);
					}
				}
				this.Refresh(this._curEditModInfo);
			}
		}

		// Token: 0x06006CAA RID: 27818 RVA: 0x00322410 File Offset: 0x00320610
		[CompilerGenerated]
		internal static void <BackupModPlugins>g__CopyFile|90_0(List<string> plugins, ref ModUploadEditPanel.<>c__DisplayClass90_0 A_1)
		{
			foreach (string fileName in plugins)
			{
				string oldPath = Path.Combine(A_1.pluginDirectory, fileName);
				bool flag = File.Exists(oldPath);
				if (flag)
				{
					string newPath = Path.Combine(A_1.legacyDirectory, fileName);
					bool flag2 = oldPath != newPath;
					if (flag2)
					{
						File.Copy(oldPath, newPath, true);
					}
				}
			}
		}

		// Token: 0x04004EAC RID: 20140
		[Header("顶部区域")]
		[SerializeField]
		private ModImageInfo modImageInfo;

		// Token: 0x04004EAD RID: 20141
		[SerializeField]
		private TMP_InputField descriptionInputField;

		// Token: 0x04004EAE RID: 20142
		[SerializeField]
		private TMP_InputField nameInputField;

		// Token: 0x04004EAF RID: 20143
		[SerializeField]
		private GameObject nameWarningMark;

		// Token: 0x04004EB0 RID: 20144
		[SerializeField]
		private TextMeshProUGUI authorValue;

		// Token: 0x04004EB1 RID: 20145
		[SerializeField]
		private TMP_InputField versionInputField;

		// Token: 0x04004EB2 RID: 20146
		[SerializeField]
		private TMP_InputField gameVersionInputField;

		// Token: 0x04004EB3 RID: 20147
		[SerializeField]
		private TextMeshProUGUI textCurGameVersion;

		// Token: 0x04004EB4 RID: 20148
		[SerializeField]
		private GameObject versionWarningMark;

		// Token: 0x04004EB5 RID: 20149
		[SerializeField]
		private CToggle changeConfigToggle;

		// Token: 0x04004EB6 RID: 20150
		[SerializeField]
		private CToggle hasArchiveToggle;

		// Token: 0x04004EB7 RID: 20151
		[SerializeField]
		private CToggle needRestartToggle;

		// Token: 0x04004EB8 RID: 20152
		[SerializeField]
		private CDropdown visibilityDropdown;

		// Token: 0x04004EB9 RID: 20153
		[Header("标签")]
		[SerializeField]
		private RectTransform tagLayout;

		// Token: 0x04004EBA RID: 20154
		[SerializeField]
		private CToggleGroupMultiSelect tagToggleGroupMultiSelect;

		// Token: 0x04004EBB RID: 20155
		[SerializeField]
		private CButton tagFilterButton;

		// Token: 0x04004EBC RID: 20156
		[SerializeField]
		private CButton clearTagButton;

		// Token: 0x04004EBD RID: 20157
		[SerializeField]
		private CButton tagsFilterMask;

		// Token: 0x04004EBE RID: 20158
		[Header("程序")]
		[SerializeField]
		private CButton buttonProgram;

		// Token: 0x04004EBF RID: 20159
		[SerializeField]
		private ModSetProgramPanel setProgramPanel;

		// Token: 0x04004EC0 RID: 20160
		[Header("设置")]
		[SerializeField]
		private CButton buttonSetting;

		// Token: 0x04004EC1 RID: 20161
		[SerializeField]
		private ModSettingPanel settingPanel;

		// Token: 0x04004EC2 RID: 20162
		[SerializeField]
		private ModEditSettingPanel editSettingPanel;

		// Token: 0x04004EC3 RID: 20163
		[Header("依赖")]
		[SerializeField]
		private CButton buttonDependence;

		// Token: 0x04004EC4 RID: 20164
		[SerializeField]
		private ModSetDependencePanel setDependencePanel;

		// Token: 0x04004EC5 RID: 20165
		[Header("底部操作栏")]
		[SerializeField]
		private ModUploadConfirmDialog modUploadConfirmDialog;

		// Token: 0x04004EC6 RID: 20166
		[SerializeField]
		private CButton buttonDeleteRemote;

		// Token: 0x04004EC7 RID: 20167
		[SerializeField]
		private CButton buttonDeleteLocal;

		// Token: 0x04004EC8 RID: 20168
		[SerializeField]
		private CButton buttonUpload;

		// Token: 0x04004EC9 RID: 20169
		[SerializeField]
		private CButton buttonUpdate;

		// Token: 0x04004ECA RID: 20170
		[SerializeField]
		private CButton buttonSave;

		// Token: 0x04004ECB RID: 20171
		[SerializeField]
		private CButton buttonSync;

		// Token: 0x04004ECC RID: 20172
		[SerializeField]
		private CButton buttonOpenExplorer;

		// Token: 0x04004ECE RID: 20174
		private int _lastUploadModPageSwitchValue;

		// Token: 0x04004ECF RID: 20175
		private int _uploadModPageMaxCount;

		// Token: 0x04004ED0 RID: 20176
		private readonly List<string> _tempModUsedTagList = new List<string>();

		// Token: 0x04004ED1 RID: 20177
		private readonly List<SettingEntry> _tempModSettingEntries = new List<SettingEntry>();

		// Token: 0x04004ED2 RID: 20178
		private readonly List<string> _tempModSettingGroups = new List<string>();

		// Token: 0x04004ED3 RID: 20179
		private string _tempModCreateFromDirectoryPath;

		// Token: 0x04004ED4 RID: 20180
		private ModInfoWithDisplayData _curEditModInfo;

		// Token: 0x04004ED5 RID: 20181
		private readonly List<string> _tempFrontendPlugins = new List<string>();

		// Token: 0x04004ED6 RID: 20182
		private readonly List<string> _tempBackendPlugins = new List<string>();

		// Token: 0x04004ED7 RID: 20183
		private readonly List<string> _tempModUpdateLogList = new List<string>();

		// Token: 0x04004ED8 RID: 20184
		private string _tempModCoverFileName;

		// Token: 0x04004ED9 RID: 20185
		private readonly List<string> _tempModDetailImageFileNameList = new List<string>();

		// Token: 0x04004EDA RID: 20186
		private string _tempModCoverFilePath;

		// Token: 0x04004EDB RID: 20187
		private readonly List<string> _tempModDetailImageFilePathList = new List<string>();

		// Token: 0x04004EDC RID: 20188
		private Texture2D _tempModCoverTexture;

		// Token: 0x04004EDD RID: 20189
		private readonly Dictionary<string, Texture2D> _tempModDetailImageDict = new Dictionary<string, Texture2D>();

		// Token: 0x04004EDE RID: 20190
		private int _targetDetailImageToggleKey = -1;

		// Token: 0x04004EDF RID: 20191
		private readonly List<ulong> _tempModDependencyList = new List<ulong>();

		// Token: 0x04004EE0 RID: 20192
		private bool _showUploadWarningMark;

		// Token: 0x04004EE1 RID: 20193
		private Action _onClickSync;

		// Token: 0x04004EE2 RID: 20194
		private const string TempFileForUploadingDirectoryName = ".TempFileForUploading";

		// Token: 0x04004EE3 RID: 20195
		private static readonly Dictionary<EItemUpdateStatus, string> UploadStatusMessages = new Dictionary<EItemUpdateStatus, string>
		{
			{
				EItemUpdateStatus.k_EItemUpdateStatusInvalid,
				"LK_Mod_Update_Status_Invalid"
			},
			{
				EItemUpdateStatus.k_EItemUpdateStatusPreparingConfig,
				"LK_Mod_Update_Status_Preparing_Config"
			},
			{
				EItemUpdateStatus.k_EItemUpdateStatusPreparingContent,
				"LK_Mod_Update_Status_Preparing_Content"
			},
			{
				EItemUpdateStatus.k_EItemUpdateStatusUploadingContent,
				"LK_Mod_Update_Status_Uploading_Content"
			},
			{
				EItemUpdateStatus.k_EItemUpdateStatusUploadingPreviewFile,
				"LK_Mod_Update_Status_Uploading_Preview_File"
			},
			{
				EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges,
				"LK_Mod_Update_Status_Committing_Changes"
			}
		};
	}
}
