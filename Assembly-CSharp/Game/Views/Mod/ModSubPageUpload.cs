using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrameWork.ModSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using Game.Views.Mod.Upload;
using GameData.Domains.Mod;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Mod
{
	// Token: 0x020008D0 RID: 2256
	public class ModSubPageUpload : ModSubPage
	{
		// Token: 0x06006BCE RID: 27598 RVA: 0x0031BBAA File Offset: 0x00319DAA
		public override void Init(ViewMod parentView)
		{
			base.Init(parentView);
			this.Init();
		}

		// Token: 0x06006BCF RID: 27599 RVA: 0x0031BBBC File Offset: 0x00319DBC
		public override void Refresh()
		{
			base.Refresh();
			ModManager.UpdateModList(delegate
			{
				this.searchInputField.SetTextWithoutNotify(string.Empty);
				this.RefreshModList(true, true, true);
			});
		}

		// Token: 0x06006BD0 RID: 27600 RVA: 0x0031BBD8 File Offset: 0x00319DD8
		public override bool QuickHide()
		{
			bool flag = this.editPanel.QuickHide();
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool isEditingUploadMod = this.editPanel.IsEditingUploadMod;
				if (isEditingUploadMod)
				{
					this.editPanel.Cancel(delegate
					{
						this.ParentView.QuickHide();
					}, null);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06006BD1 RID: 27601 RVA: 0x0031BC2C File Offset: 0x00319E2C
		public override void TryChangeTab(Action onSuccess, Action onFailed)
		{
			bool flag = !this.editPanel.CanChangeTab();
			if (flag)
			{
				if (onFailed != null)
				{
					onFailed();
				}
			}
			else
			{
				this.editPanel.Cancel(onSuccess, onFailed);
			}
		}

		// Token: 0x06006BD2 RID: 27602 RVA: 0x0031BC69 File Offset: 0x00319E69
		private void OnDisable()
		{
			this.editPanel.Clear();
			this.ShowEditPanel(false);
			this._curEditMoId = default(ModId);
			this._curEditModInfo = null;
			this.modScroll.SetDataCount(0);
		}

		// Token: 0x06006BD3 RID: 27603 RVA: 0x0031BCA0 File Offset: 0x00319EA0
		private void Init()
		{
			this._selectedIndex = -1;
			this.modScroll.srcPrefab.gameObject.SetActive(false);
			this.modScroll.RemoveOnScrollEvent(new Action(this.OnUploadModScroll));
			this.modScroll.AddOnScrollEvent(new Action(this.OnUploadModScroll));
			this.modScroll.OnItemRender -= this.OnRender;
			this.modScroll.OnItemRender += this.OnRender;
			this._uploadModPageMaxCount = this.modScroll.GetPageMaxCount();
			this.modPageSwitch.OnValueChanged = new Action<int>(this.OnUploadModPageValueChanged);
			this.modPageSwitch.SetValueAndRefresh(1);
			this.searchInputField.onValueChanged.RemoveAllListeners();
			this.searchInputField.onValueChanged.AddListener(new UnityAction<string>(this.OnUploadModSearchInputValueChange));
			this.searchInputField.SetTextWithoutNotify(string.Empty);
			this.buttonCreate.ClearAndAddListener(new Action(this.OnClickCreate));
			this.buttonImport.ClearAndAddListener(new Action(this.OnClickImport));
			this.buttonSyncAll.ClearAndAddListener(new Action(this.OnClickSyncAll));
			this.editPanel.Init(new Action(this.OnClickSync));
			this.editPanel.Clear();
			this.ShowEditPanel(false);
			this._curEditMoId = default(ModId);
			this._curEditModInfo = null;
		}

		// Token: 0x06006BD4 RID: 27604 RVA: 0x0031BE28 File Offset: 0x0031A028
		private void RefreshModList(bool refreshData = true, bool initData = false, bool initScrollBar = false)
		{
			ModSubPageUpload.<>c__DisplayClass25_0 CS$<>8__locals1 = new ModSubPageUpload.<>c__DisplayClass25_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.initScrollBar = initScrollBar;
			if (initData)
			{
				this._selectedIndex = -1;
				this.ParentView.ShowMask();
				ModManager.UpdateUploadedItems(delegate(Dictionary<ModId, bool> _)
				{
					CS$<>8__locals1.<>4__this._originUploadModIdList.Clear();
					CS$<>8__locals1.<>4__this._originUploadModIdList.AddRange(ModManager.ExternalMods);
					CS$<>8__locals1.<>4__this._uploadModIdList.Clear();
					CS$<>8__locals1.<>4__this._uploadModIdList.AddRange(CS$<>8__locals1.<>4__this._originUploadModIdList);
					base.<RefreshModList>g__RealRefresh|1();
					CS$<>8__locals1.<>4__this.ParentView.HideMask();
				});
			}
			else
			{
				if (refreshData)
				{
					this._selectedIndex = -1;
					this._uploadModIdList.Clear();
					this._uploadModIdList.AddRange(this._originUploadModIdList);
				}
				CS$<>8__locals1.<RefreshModList>g__RealRefresh|1();
			}
		}

		// Token: 0x06006BD5 RID: 27605 RVA: 0x0031BEAF File Offset: 0x0031A0AF
		private void ShowEditPanel(bool isShow)
		{
			this.editPanel.gameObject.SetActive(isShow);
			this.noContent.gameObject.SetActive(!isShow);
			this.buttonImport.interactable = isShow;
		}

		// Token: 0x06006BD6 RID: 27606 RVA: 0x0031BEE8 File Offset: 0x0031A0E8
		private void SelectMod(int index)
		{
			ModSubPageUpload.<>c__DisplayClass27_0 CS$<>8__locals1 = new ModSubPageUpload.<>c__DisplayClass27_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.index = index;
			bool flag = CS$<>8__locals1.index >= 0 && this._uploadModIdList.CheckIndex(CS$<>8__locals1.index);
			if (flag)
			{
				ModId modId = this._uploadModIdList[CS$<>8__locals1.index];
				bool flag2 = modId.Equals(this._curEditMoId);
				if (flag2)
				{
					this._selectedIndex = CS$<>8__locals1.index;
					CS$<>8__locals1.<SelectMod>g__Select|2(modId);
					return;
				}
			}
			this.editPanel.Cancel(delegate
			{
				bool flag3 = CS$<>8__locals1.<>4__this._selectedIndex >= 0 && CS$<>8__locals1.<>4__this._uploadModIdList.CheckIndex(CS$<>8__locals1.<>4__this._selectedIndex);
				if (flag3)
				{
					bool flag4 = !CS$<>8__locals1.<>4__this._uploadModIdList[CS$<>8__locals1.<>4__this._selectedIndex].IsValid;
					if (flag4)
					{
						CS$<>8__locals1.<>4__this.RefreshModList(true, false, false);
						return;
					}
				}
				CS$<>8__locals1.<>4__this._selectedIndex = CS$<>8__locals1.index;
				bool flag5 = CS$<>8__locals1.index >= 0 && CS$<>8__locals1.<>4__this._uploadModIdList.CheckIndex(CS$<>8__locals1.index);
				if (flag5)
				{
					ModId modId2 = CS$<>8__locals1.<>4__this._uploadModIdList[CS$<>8__locals1.index];
					base.<SelectMod>g__Select|2(modId2);
				}
				else
				{
					CS$<>8__locals1.<>4__this.editPanel.Clear();
					CS$<>8__locals1.<>4__this.ShowEditPanel(false);
				}
				CS$<>8__locals1.<>4__this.modScroll.SetDataCount(CS$<>8__locals1.<>4__this._uploadModIdList.Count);
			}, delegate
			{
				CS$<>8__locals1.<>4__this.modScroll.ReRender();
			});
		}

		// Token: 0x06006BD7 RID: 27607 RVA: 0x0031BF8D File Offset: 0x0031A18D
		private void OnClickCreate()
		{
			this.editPanel.Cancel(delegate
			{
				this._curEditModInfo = null;
				ModId modId = default(ModId);
				this._uploadModIdList.Add(modId);
				this.RefreshModList(false, false, false);
				this.SelectMod(this._uploadModIdList.Count - 1);
				this.buttonCreate.interactable = false;
			}, null);
		}

		// Token: 0x06006BD8 RID: 27608 RVA: 0x0031BFA9 File Offset: 0x0031A1A9
		private void OnClickImport()
		{
			this.editPanel.Import();
		}

		// Token: 0x06006BD9 RID: 27609 RVA: 0x0031BFB8 File Offset: 0x0031A1B8
		private void OnClickOpenUpload()
		{
		}

		// Token: 0x06006BDA RID: 27610 RVA: 0x0031BFBC File Offset: 0x0031A1BC
		private void OnRender(int index, GameObject go)
		{
			ModUploadTemplate refers = go.GetComponent<ModUploadTemplate>();
			refers.toggle.SetIsOnWithoutNotify(index == this._selectedIndex);
			refers.toggle.onValueChanged.ResetListener(delegate(bool isOn)
			{
				if (isOn)
				{
					this.SelectMod(index);
				}
				else
				{
					this.SelectMod(-1);
				}
			});
			ModId modId = this._uploadModIdList[index];
			bool flag = !modId.IsValid;
			if (flag)
			{
				refers.title.SetText(LocalStringManager.Get(LanguageKey.LK_Mod_Create), true);
				refers.steam.SetActive(false);
				refers.external.SetActive(true);
			}
			else
			{
				ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
				bool flag2 = modInfo == null;
				if (!flag2)
				{
					bool isSteam = ModManager.UploadedMods.Exists((ModId id) => id.FileId == modId.FileId);
					refers.steam.SetActive(isSteam);
					refers.external.SetActive(!isSteam);
					TextMeshProUGUI titleText = refers.title;
					titleText.SetText(modInfo.Title, true);
					TooltipInvoker titleTip = titleText.GetComponent<TooltipInvoker>();
					bool flag3 = titleTip.PresetParam == null || titleTip.PresetParam.Length == 0;
					if (flag3)
					{
						titleTip.PresetParam = new string[1];
					}
					titleTip.PresetParam[0] = modInfo.Title;
				}
			}
		}

		// Token: 0x06006BDB RID: 27611 RVA: 0x0031C128 File Offset: 0x0031A328
		private void OnUploadModPageValueChanged(int index)
		{
			bool isAdd = index > this._lastUploadModPageSwitchValue;
			this._lastUploadModPageSwitchValue = index;
			int targetIndex = this._uploadModPageMaxCount * (isAdd ? index : (index - 1));
			bool flag = index < this.modPageSwitch.MaxValue;
			if (flag)
			{
				targetIndex--;
			}
			targetIndex = Mathf.Max(0, targetIndex);
			this.modScroll.Refresh(targetIndex);
		}

		// Token: 0x06006BDC RID: 27612 RVA: 0x0031C184 File Offset: 0x0031A384
		private void OnUploadModScroll()
		{
			float curValue = this.modScroll.Scroll.ScrollBar.value * (float)this.modScroll.CurrentDataCount;
			float value = curValue / (float)this._uploadModPageMaxCount;
			this.modPageSwitch.SetValueAndRefresh(Mathf.CeilToInt(value));
			this._lastUploadModPageSwitchValue = this.modPageSwitch.Value;
		}

		// Token: 0x06006BDD RID: 27613 RVA: 0x0031C1E4 File Offset: 0x0031A3E4
		private void OnUploadModSearchInputValueChange(string value)
		{
			this.editPanel.Clear();
			CommonUtils.FixToShowAbleString(ref value, this.searchInputField.textComponent.font);
			bool flag = value.IsNullOrEmpty();
			if (flag)
			{
				this.RefreshModList(true, false, false);
			}
			else
			{
				this._uploadModIdList.Clear();
				foreach (ModId modId in this._originUploadModIdList)
				{
					ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
					bool flag2 = modInfo.Title.Contains(value);
					if (flag2)
					{
						this._uploadModIdList.Add(modId);
					}
				}
				this.RefreshModList(false, false, false);
			}
		}

		// Token: 0x06006BDE RID: 27614 RVA: 0x0031C2AC File Offset: 0x0031A4AC
		private void OnClickSyncAll()
		{
			this.OnClickSync(true);
		}

		// Token: 0x06006BDF RID: 27615 RVA: 0x0031C2B6 File Offset: 0x0031A4B6
		private void OnClickSync()
		{
			this.OnClickSync(false);
		}

		// Token: 0x06006BE0 RID: 27616 RVA: 0x0031C2C0 File Offset: 0x0031A4C0
		private void OnClickSync(bool isAll)
		{
			if (isAll)
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Mod_Sync);
				string content = LocalStringManager.Get(LanguageKey.LK_Mod_Sync_All_Confirm_Tip);
				CommonUtils.ShowConfirmDialog(title, content, delegate
				{
					this.StartSync(true);
				}, null, EDialogType.None);
			}
			else
			{
				string title2 = LocalStringManager.Get(LanguageKey.LK_Mod_Sync);
				string content2 = LocalStringManager.Get(LanguageKey.LK_Mod_Sync_Confirm_Tip);
				CommonUtils.ShowConfirmDialog(title2, content2, delegate
				{
					this.StartSync(false);
				}, null, EDialogType.None);
			}
		}

		// Token: 0x06006BE1 RID: 27617 RVA: 0x0031C334 File Offset: 0x0031A534
		private void StartSync(bool isAll)
		{
			this._needSyncModIdList.Clear();
			List<ModId> needSyncModIdList = this._needSyncModIdList;
			IEnumerable<ModId> collection;
			if (!isAll)
			{
				collection = from id in ModManager.UploadedMods
				where id.FileId == this._curEditModInfo.ModId.FileId
				select id;
			}
			else
			{
				IEnumerable<ModId> uploadedMods = ModManager.UploadedMods;
				collection = uploadedMods;
			}
			needSyncModIdList.AddRange(collection);
			base.StartCoroutine(this.CorStartSync(isAll));
		}

		// Token: 0x06006BE2 RID: 27618 RVA: 0x0031C38A File Offset: 0x0031A58A
		private IEnumerator CorStartSync(bool isAll)
		{
			UI_FullScreenMask screenMask = UIElement.FullScreenMask.UiBaseAs<UI_FullScreenMask>();
			string tip = isAll ? LanguageKey.LK_Mod_Sync_All_Tip.Tr() : LanguageKey.LK_Mod_Sync_Tip.Tr();
			screenMask.UpdateMessage(tip);
			GEvent.OnEvent(UiEvents.OnModViewShowMask, null);
			foreach (ModId modId in this._needSyncModIdList)
			{
				PublishedFileId_t publishedFileId = new PublishedFileId_t(modId.FileId);
				SteamUGC.DownloadItem(publishedFileId, true);
				publishedFileId = default(PublishedFileId_t);
				modId = default(ModId);
			}
			List<ModId>.Enumerator enumerator = default(List<ModId>.Enumerator);
			for (;;)
			{
				bool finished = true;
				foreach (ModId modId2 in this._needSyncModIdList)
				{
					bool isInstalled = SteamManager.IsItemStateActive(modId2.FileId, EItemState.k_EItemStateInstalled);
					bool isNeedsUpdate = SteamManager.IsItemStateActive(modId2.FileId, EItemState.k_EItemStateNeedsUpdate);
					bool isDownloading = SteamManager.IsItemStateActive(modId2.FileId, EItemState.k_EItemStateDownloading);
					bool isDownloadPending = SteamManager.IsItemStateActive(modId2.FileId, EItemState.k_EItemStateDownloadPending);
					bool flag = !isInstalled && !isNeedsUpdate && !isDownloading && !isDownloadPending;
					if (!flag)
					{
						bool flag2 = !isInstalled || isNeedsUpdate || isDownloading || isDownloadPending;
						if (flag2)
						{
							finished = false;
						}
						modId2 = default(ModId);
					}
				}
				List<ModId>.Enumerator enumerator2 = default(List<ModId>.Enumerator);
				bool flag3 = finished;
				if (flag3)
				{
					break;
				}
				yield return null;
			}
			bool flag4 = ModManager.SyncCoverLocalMod(this._needSyncModIdList);
			if (flag4)
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				string content = LocalStringManager.Get(LanguageKey.LK_Mod_Notification_Sync_Success);
				CommonUtils.ShowDialog(title, content, delegate()
				{
					this.editPanel.Clear();
					GEvent.OnEvent(UiEvents.OnModViewRefresh, null);
				}, EDialogType.None);
				title = null;
				content = null;
			}
			else
			{
				GEvent.OnEvent(UiEvents.OnModViewHideMask, null);
			}
			yield break;
		}

		// Token: 0x04004E2A RID: 20010
		[SerializeField]
		private InfinityScroll modScroll;

		// Token: 0x04004E2B RID: 20011
		[SerializeField]
		private ModIdSwitch modPageSwitch;

		// Token: 0x04004E2C RID: 20012
		[SerializeField]
		private TMP_InputField searchInputField;

		// Token: 0x04004E2D RID: 20013
		[SerializeField]
		private CButton buttonCreate;

		// Token: 0x04004E2E RID: 20014
		[SerializeField]
		private CButton buttonImport;

		// Token: 0x04004E2F RID: 20015
		[SerializeField]
		private CButton buttonSyncAll;

		// Token: 0x04004E30 RID: 20016
		[SerializeField]
		private ModUploadEditPanel editPanel;

		// Token: 0x04004E31 RID: 20017
		[SerializeField]
		private GameObject noContent;

		// Token: 0x04004E32 RID: 20018
		[SerializeField]
		private CButton buttonOpenUpload;

		// Token: 0x04004E33 RID: 20019
		[SerializeField]
		private ModDirectlyUploadPanel modDirectlyUploadPanel;

		// Token: 0x04004E34 RID: 20020
		private int _lastUploadModPageSwitchValue;

		// Token: 0x04004E35 RID: 20021
		private int _uploadModPageMaxCount;

		// Token: 0x04004E36 RID: 20022
		private readonly List<ModId> _uploadModIdList = new List<ModId>();

		// Token: 0x04004E37 RID: 20023
		private readonly List<ModId> _originUploadModIdList = new List<ModId>();

		// Token: 0x04004E38 RID: 20024
		private string _tempModCreateFromDirectoryPath;

		// Token: 0x04004E39 RID: 20025
		private ModInfoWithDisplayData _curEditModInfo;

		// Token: 0x04004E3A RID: 20026
		private ModId _curEditMoId;

		// Token: 0x04004E3B RID: 20027
		private int _selectedIndex;

		// Token: 0x04004E3C RID: 20028
		private readonly List<ModId> _needSyncModIdList = new List<ModId>();
	}
}
