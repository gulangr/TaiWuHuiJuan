using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.ModSystem;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Views.GameLineScroll;
using GameData.DLC;
using GameData.Domains.Adventure;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Mod;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.GameDataBridge;
using UnityEngine;

namespace Game.Views.RecordSelect
{
	// Token: 0x020007BC RID: 1980
	public class ViewRecordSelect : UIBase
	{
		// Token: 0x06006097 RID: 24727 RVA: 0x002C45F8 File Offset: 0x002C27F8
		private void OnRecordRootToggleChanged(int newTogIndex, int preTogIndex)
		{
			CToggle newTog = this.recordRootToggleGroup.Get(newTogIndex);
			CToggle preTog = this.recordRootToggleGroup.Get(preTogIndex);
			bool flag = null != preTog;
			if (flag)
			{
				preTog.GetComponent<RecordInfoItem>().BtnLayout.gameObject.SetActive(false);
			}
			bool flag2 = null != newTog;
			if (flag2)
			{
				this._currSlot = (sbyte)newTogIndex;
				ArchiveInfo[] archivesInfo = GlobalOperations.ArchivesInfo;
				sbyte? b = (archivesInfo != null) ? new sbyte?(archivesInfo[newTogIndex].Status) : null;
				bool flag3;
				if (b != null)
				{
					sbyte valueOrDefault = b.GetValueOrDefault();
					if (valueOrDefault - 1 <= 1)
					{
						flag3 = true;
						goto IL_9C;
					}
				}
				flag3 = false;
				IL_9C:
				bool isShowBtn = flag3;
				newTog.GetComponent<RecordInfoItem>().BtnLayout.gameObject.SetActive(isShowBtn);
				bool flag4 = this._emptySlots.Contains((int)this._currSlot);
				if (flag4)
				{
					return;
				}
				Game.Views.GameLineScroll.ScrollHelper.SetUIElement(this.Element);
				Game.Views.GameLineScroll.ScrollHelper.OnOnRecordRootToggleChanged(this.taiwuScroll, (int)this._currSlot);
				ToggleGroupHotkeyController.Set(this.Element, this.taiwuScroll.CGet<CToggleGroup>("ScrollTypeToggleGroup"), 0, null);
			}
			this.OnScrollTabChange();
		}

		// Token: 0x06006098 RID: 24728 RVA: 0x002C471C File Offset: 0x002C291C
		private void OnArchiveInfoLoaded(ArgumentBox argBox = null)
		{
			this._modDependenciesChangedList.Clear();
			bool initialized = SteamManager.Initialized;
			if (initialized)
			{
				List<ulong> list = new List<ulong>();
				bool flag = GlobalOperations.ArchivesInfo != null;
				if (flag)
				{
					Action<ModId> <>9__2;
					foreach (ArchiveInfo archiveInfo in GlobalOperations.ArchivesInfo)
					{
						WorldInfo worldInfo = archiveInfo.WorldInfo;
						if (worldInfo != null)
						{
							List<ModId> modIds = worldInfo.ModIds;
							if (modIds != null)
							{
								Action<ModId> action;
								if ((action = <>9__2) == null)
								{
									action = (<>9__2 = delegate(ModId modId)
									{
										list.Add(modId.FileId);
									});
								}
								modIds.ForEach(action);
							}
						}
					}
				}
				bool flag2 = list.Count > 0;
				if (flag2)
				{
					this.ShowMask();
					ModManager.UpdateTargetItems(list, delegate(Dictionary<ModId, bool> dependenciesChangeStateDict)
					{
						bool flag3 = dependenciesChangeStateDict != null;
						if (flag3)
						{
							foreach (KeyValuePair<ModId, bool> keyValuePair in dependenciesChangeStateDict)
							{
								ModId modId2;
								bool flag4;
								keyValuePair.Deconstruct(out modId2, out flag4);
								ModId modId = modId2;
								bool changed = flag4;
								bool flag5 = changed;
								if (flag5)
								{
									this._modDependenciesChangedList.Add(modId);
								}
							}
						}
						this.<OnArchiveInfoLoaded>g__Refresh|25_0();
						this.HideMask();
					});
					return;
				}
			}
			this.<OnArchiveInfoLoaded>g__Refresh|25_0();
		}

		// Token: 0x06006099 RID: 24729 RVA: 0x002C4800 File Offset: 0x002C2A00
		private void OnEnterWorldLoadStart()
		{
			GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 50));
		}

		// Token: 0x0600609A RID: 24730 RVA: 0x002C4820 File Offset: 0x002C2A20
		private void OnEnterWorldLoadFinish()
		{
			GameApp.Instance.ChangeGameState(EGameState.InGame, null);
			AdventureRemakeModel model = SingletonObject.getInstance<AdventureRemakeModel>();
			bool flag;
			if (model.AdventureTaiwu.InAdventure)
			{
				Dictionary<int, AdventureRuntime> adventureRemakeDict = model.AdventureRemakeDict;
				flag = (adventureRemakeDict != null && adventureRemakeDict.Count > 0);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				bool flag3 = SingletonObject.getInstance<BasicGameData>().CurrDate > 8;
				if (flag3)
				{
					UIElement monthNotify = UIElement.MonthNotify;
					monthNotify.OnHide = (Action)Delegate.Combine(monthNotify.OnHide, new Action(TaiwuEventDomainMethod.Call.OnRecordEnterGame));
					UIElement.MonthNotify.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("NeedSave", false));
					UIManager.Instance.ShowUI(UIElement.MonthNotify, true);
				}
				else
				{
					TaiwuEventDomainMethod.Call.OnRecordEnterGame();
				}
			}
		}

		// Token: 0x0600609B RID: 24731 RVA: 0x002C48DE File Offset: 0x002C2ADE
		private void OnPlayIntroSucceeded()
		{
			SingletonObject.getInstance<GlobalSettings>().BgmOn = false;
			UIManager.Instance.StackToUI(UIElement.BlockInteract);
		}

		// Token: 0x0600609C RID: 24732 RVA: 0x002C48FD File Offset: 0x002C2AFD
		private void OnPlayIntroFailed()
		{
			PredefinedLog.Show(9);
			UIElement.CgPlayer.Hide(false);
		}

		// Token: 0x0600609D RID: 24733 RVA: 0x002C4914 File Offset: 0x002C2B14
		private void OnEnteringNewGame()
		{
			bool isEnteringNewGame = this._isEnteringNewGame;
			if (!isEnteringNewGame)
			{
				this._isEnteringNewGame = true;
				this._panels[(int)this._currSlot].SetBtnStartInteractable(false);
				SingletonObject.getInstance<YieldHelper>().StartYield(this.ToNewGame());
			}
		}

		// Token: 0x0600609E RID: 24734 RVA: 0x002C495C File Offset: 0x002C2B5C
		private void OnEnteringAvatarCreation()
		{
			bool useInscribedChar = GlobalOperations.InscribedCharacters != null && GlobalOperations.InscribedCharacters.Count > 0;
			UIElement.NewGame.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("Index", this._currSlot).Set("UseInscribedChar", useInscribedChar));
			GameApp.Instance.ChangeGameState(EGameState.NewGame, null);
		}

		// Token: 0x0600609F RID: 24735 RVA: 0x002C49BC File Offset: 0x002C2BBC
		private void OnEnterBackupWorldConfirmed(long timeStamp)
		{
			this.CheckEnterGame(delegate
			{
				GameApp.ClockAndLogInfo("Clicked RevertWorld button", true);
				SingletonObject.getInstance<GlobalSettings>().LastEnterWorldIndex = this._currSlot;
				GlobalOperations.LoadWorld(this._currSlot, timeStamp);
				GameApp.Instance.ChangeGameState(EGameState.Loading, EasyPool.Get<ArgumentBox>().SetObject("OnLoadingFinish", new Action(this.OnEnterWorldLoadFinish)).SetObject("OnLoadingStart", new Action(this.OnEnterWorldLoadStart)));
			}, timeStamp);
		}

		// Token: 0x060060A0 RID: 24736 RVA: 0x002C49F8 File Offset: 0x002C2BF8
		private void OnDeleteOperationConfirmed()
		{
			bool isGoodStatus = this._readySlots.Contains((int)this._currSlot);
			GlobalOperations.DeleteArchive(this._currSlot);
			ArchiveInfo[] archivesInfo = GlobalOperations.ArchivesInfo;
			int currSlot = (int)this._currSlot;
			if (archivesInfo[currSlot] == null)
			{
				archivesInfo[currSlot] = new ArchiveInfo();
			}
			GlobalOperations.ArchivesInfo[(int)this._currSlot].Status = 0;
			this.RefreshArchiveAtIndex(this._currSlot, this._panels[(int)this._currSlot]);
			bool flag = isGoodStatus;
			if (flag)
			{
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, new Action(Game.Views.GameLineScroll.ScrollHelper.OnSaveFileDeleted));
			}
		}

		// Token: 0x060060A1 RID: 24737 RVA: 0x002C4A88 File Offset: 0x002C2C88
		private void OnRevertBtnClicked()
		{
			ArchiveInfo archiveInfo = GlobalOperations.ArchivesInfo[(int)this._currSlot];
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("OnConfirmEnter", new Action<long>(this.OnEnterBackupWorldConfirmed));
			argBox.SetObject("ArchiveData", archiveInfo);
			argBox.Set("ArchiveIndex", this._currSlot);
			UIElement.RevertArchive.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.RevertArchive);
		}

		// Token: 0x060060A2 RID: 24738 RVA: 0x002C4AFC File Offset: 0x002C2CFC
		private void OnDeleteBtnClicked()
		{
			this.recordRootToggleGroup.Set((int)this._currSlot, false);
			UIElement dialog = UIElement.Dialog;
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			string key = "Cmd";
			object arg;
			if (!this._readySlots.Contains((int)this._currSlot))
			{
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_GameName);
				dialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.UI_RecordSelect_DeleteRecord_Confirm, LocalStringManager.Get(LanguageKey.LK_Record_Broken_Desc)).ColorReplace();
				dialogCmd.Type = 1;
				arg = dialogCmd;
				dialogCmd.Yes = new Action(this.OnDeleteOperationConfirmed);
			}
			else
			{
				DialogCmd dialogCmd2 = new DialogCmd();
				dialogCmd2.Title = LocalStringManager.Get(LanguageKey.UI_RecordSelect_Tip_DeleteRecord_Title);
				dialogCmd2.Content = LocalStringManager.GetFormat(LanguageKey.UI_RecordSelect_DeleteRecord_Confirm, ViewRecordSelect.GetCharacterName(this.GetArchiveInfo(this._currSlot).WorldInfo)).ColorReplace();
				dialogCmd2.Type = 1;
				arg = dialogCmd2;
				dialogCmd2.Yes = new Action(this.OnDeleteOperationConfirmed);
			}
			dialog.SetOnInitArgs(argumentBox.SetObject(key, arg));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x060060A3 RID: 24739 RVA: 0x002C4C04 File Offset: 0x002C2E04
		private void OnContinueBtnClicked()
		{
			bool flag = this._emptySlots.Contains((int)this._currSlot);
			if (flag)
			{
				this.OnEnteringNewGame();
			}
			else
			{
				this.CheckEnterGame(delegate
				{
					GameApp.ClockAndLogInfo("Clicked LoadWorld button", true);
					this._panels[(int)this._currSlot].SetBtnStartInteractable(false);
					bool flag2 = this._performing == null;
					if (flag2)
					{
						base.StartCoroutine(this._performing = this.LoadGame(this._currSlot));
					}
				}, -1L);
			}
		}

		// Token: 0x060060A4 RID: 24740 RVA: 0x002C4C48 File Offset: 0x002C2E48
		private void OnEmptySlotLeftClick()
		{
			bool flag = this._emptySlots.Contains((int)this._currSlot);
			if (flag)
			{
				this.OnEnteringNewGame();
				this.recordRootToggleGroup.DeSelect(true);
			}
		}

		// Token: 0x060060A5 RID: 24741 RVA: 0x002C4C84 File Offset: 0x002C2E84
		public override void OnInit(ArgumentBox argsBox)
		{
			this._isEnteringNewGame = false;
			this._modDependenciesChangedSlotIndex = -1;
			base.GetComponent<CanvasGroup>().alpha = 1f;
			Game.Views.GameLineScroll.ScrollHelper.SetQuickHideBanned(false);
			Vector2 anchoredPosition = this.animationRoot.anchoredPosition;
			anchoredPosition.y = 1500f;
			this.animationRoot.anchoredPosition = anchoredPosition;
		}

		// Token: 0x060060A6 RID: 24742 RVA: 0x002C4CDD File Offset: 0x002C2EDD
		public override void PlayAudioOut()
		{
		}

		// Token: 0x060060A7 RID: 24743 RVA: 0x002C4CE0 File Offset: 0x002C2EE0
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "CloseBtn";
			if (flag)
			{
				this.QuickHide();
			}
			else
			{
				Game.Views.GameLineScroll.ScrollHelper.OnClick(btn);
			}
		}

		// Token: 0x060060A8 RID: 24744 RVA: 0x002C4D14 File Offset: 0x002C2F14
		public override void QuickHide()
		{
			bool quickHideBanned = Game.Views.GameLineScroll.ScrollHelper.GetQuickHideBanned();
			if (quickHideBanned)
			{
				Game.Views.GameLineScroll.ScrollHelper.QuickHide();
			}
			else
			{
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
				base.QuickHide();
				this.ChangeAmbiencePlaying(false);
			}
		}

		// Token: 0x060060A9 RID: 24745 RVA: 0x002C4D58 File Offset: 0x002C2F58
		private void Awake()
		{
			this._panels = new RecordInfoItem[5];
			CToggleGroup toggleGroup = this.recordRootToggleGroup;
			for (int i = 0; i < 5; i++)
			{
				string key = string.Format("RecordInfo_{0}", i);
				RecordInfoItem panel = this.recordInfoTemplate;
				bool flag = i != 0;
				if (flag)
				{
					panel = Object.Instantiate<RecordInfoItem>(panel, panel.transform.parent);
					panel.name = key;
					CToggle toggle = panel.GetComponent<CToggle>();
					toggleGroup.Add(toggle);
				}
				this._panels[i] = panel;
				this.SetAsEmptyArchive(panel);
			}
			toggleGroup.Init(-1);
			toggleGroup.OnActiveIndexChange += this.OnRecordRootToggleChanged;
			this.tabGroupToggleGroup.Init(-1);
			this.tabGroupToggleGroup.OnActiveIndexChange += this.OnScrollTabToggleChange;
		}

		// Token: 0x060060AA RID: 24746 RVA: 0x002C4E30 File Offset: 0x002C3030
		private void RefreshPageDisplay()
		{
			for (int i = 0; i < 5; i++)
			{
				int pageOfSlot = i / 5;
				this._panels[i].gameObject.SetActive(pageOfSlot == this._currentPage);
			}
		}

		// Token: 0x060060AB RID: 24747 RVA: 0x002C4E70 File Offset: 0x002C3070
		private void OnEnable()
		{
			this._emptySlots.Clear();
			this._readySlots.Clear();
			for (sbyte i = 0; i < 5; i += 1)
			{
				this.recordRootToggleGroup.Set((int)i, false);
			}
			GEvent.Add(EEvents.ArchivesInfoReady, new GEvent.Callback(this.OnArchiveInfoLoaded));
			GameApp.ClockAndLogInfo("Send GetArchivesInfo", true);
			GlobalOperations.GetArchivesInfo();
			ModManager.UpdateModList(null);
		}

		// Token: 0x060060AC RID: 24748 RVA: 0x002C4EE8 File Offset: 0x002C30E8
		private void OnDisable()
		{
			GEvent.Remove(EEvents.ArchivesInfoReady, new GEvent.Callback(this.OnArchiveInfoLoaded));
			bool flag = UIManager.Instance.IsFocusElement(this.Element);
			if (flag)
			{
				ModManager.Clear();
				this.HideMask();
			}
		}

		// Token: 0x060060AD RID: 24749 RVA: 0x002C4F34 File Offset: 0x002C3134
		public void ChangeAmbiencePlaying(bool playing)
		{
			bool quickHideBanned = Game.Views.GameLineScroll.ScrollHelper.GetQuickHideBanned();
			if (!quickHideBanned)
			{
				AudioManager.Instance.PlayAmbience(playing ? "Continue_unclick" : AudioManager.DummyAudioName, 0.5f, 100);
			}
		}

		// Token: 0x060060AE RID: 24750 RVA: 0x002C4F6E File Offset: 0x002C316E
		private IEnumerator ToNewGame()
		{
			this.ChangeAmbiencePlaying(false);
			Game.Views.GameLineScroll.ScrollHelper.SetQuickHideBanned(true);
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("CGName", "OPENING").Set("Localized", true).Set("RenderMode", 0).Set("JumpOpen", true).SetObject("OnVideoPlayStart", new Action(this.OnPlayIntroSucceeded)).SetObject("OnVideoPlayError", new Action(this.OnPlayIntroFailed));
			bool musicOn = SingletonObject.getInstance<GlobalSettings>().BgmOn;
			UIElement cgPlayer = UIElement.CgPlayer;
			cgPlayer.OnDeActive = (Action)Delegate.Combine(cgPlayer.OnDeActive, new Action(delegate()
			{
				SingletonObject.getInstance<GlobalSettings>().BgmOn = musicOn;
			}));
			UIElement.CgPlayer.SetOnInitArgs(box);
			UIElement.CgPlayer.Show();
			WaitUntil waitUntil = new WaitUntil(() => !UIElement.CgPlayer.Exist);
			yield return waitUntil;
			ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
			argsBox.SetObject("OnLoadingStart", new Action(this.OnEnteringAvatarCreation));
			GameApp.Instance.ChangeGameState(EGameState.Loading, argsBox);
			yield return new WaitForSeconds(0.5f);
			GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 50));
			yield return new WaitForSeconds(0.5f);
			GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 100));
			yield break;
		}

		// Token: 0x060060AF RID: 24751 RVA: 0x002C4F7D File Offset: 0x002C317D
		private IEnumerator LoadGame(sbyte index)
		{
			this.ChangeAmbiencePlaying(false);
			Game.Views.GameLineScroll.ScrollHelper.SetQuickHideBanned(true);
			SingletonObject.getInstance<GlobalSettings>().LastEnterWorldIndex = index;
			SingletonObject.getInstance<GlobalSettings>().HaveDoneSave = true;
			GlobalOperations.LoadWorld(index, -1L);
			GameApp.Instance.ChangeGameState(EGameState.Loading, EasyPool.Get<ArgumentBox>().SetObject("OnLoadingFinish", new Action(this.OnEnterWorldLoadFinish)).SetObject("OnLoadingStart", new Action(this.OnEnterWorldLoadStart)));
			this._performing = null;
			yield break;
		}

		// Token: 0x060060B0 RID: 24752 RVA: 0x002C4F93 File Offset: 0x002C3193
		private void SetAsEmptyArchive(RecordInfoItem refers)
		{
			refers.SetAsEmptyArchive();
		}

		// Token: 0x060060B1 RID: 24753 RVA: 0x002C4FA0 File Offset: 0x002C31A0
		private void RefreshArchiveAtIndex(sbyte index, RecordInfoItem refers)
		{
			ArchiveInfo archiveInfo = null;
			sbyte archiveStatus = 0;
			ArchiveInfo[] archivesInfo = GlobalOperations.ArchivesInfo;
			bool flag = archivesInfo != null && archivesInfo.CheckIndex((int)index);
			if (flag)
			{
				ArchiveInfo[] archivesInfo2 = GlobalOperations.ArchivesInfo;
				archiveInfo = ((archivesInfo2 != null) ? archivesInfo2[(int)index] : null);
				bool flag2 = archiveInfo != null;
				if (flag2)
				{
					archiveStatus = archiveInfo.Status;
				}
			}
			refers.SetMouseTip(false, null);
			refers.SetBtnLayoutActive(archiveStatus != 0);
			this._emptySlots.Remove((int)index);
			this._readySlots.Remove((int)index);
			refers.PointClickBridge.OnLeftClick = null;
			bool flag3 = archiveStatus == 0;
			if (flag3)
			{
				this.SetAsEmptyArchive(refers);
				refers.SetMouseTip(true, new string[]
				{
					LocalStringManager.Get(LanguageKey.UI_RecordSelect_Tip_CreateTaiwu_Title),
					LocalStringManager.Get(LanguageKey.UI_RecordSelect_Tip_CreateTaiwu_Content)
				});
				this._emptySlots.Add((int)index);
				refers.PointClickBridge.OnLeftClick = new Action(this.OnEmptySlotLeftClick);
				refers.PointClickBridge.OnDoubleClick = new Action(this.OnEmptySlotLeftClick);
			}
			else
			{
				bool flag4 = archiveStatus == 1;
				if (flag4)
				{
					bool flag5 = archiveInfo == null;
					if (flag5)
					{
						throw new Exception("null archiveInfo when archiveStatus is Good");
					}
					WorldInfo worldInfo = archiveInfo.WorldInfo;
					bool flag6 = archiveInfo.WorldInfo.GameVersionInfo != null;
					if (flag6)
					{
						Debug.Log("GameVersionLastSaving " + archiveInfo.WorldInfo.GameVersionInfo.GameVersionLastSaving);
					}
					DateTime utcSave = DateTime.MinValue.AddTicks(Math.Clamp(worldInfo.SavingTimestamp, 0L, DateTime.MaxValue.Ticks));
					int year = worldInfo.CurrDate / 12 + 1;
					string characterName = ViewRecordSelect.GetCharacterName(archiveInfo.WorldInfo);
					ClothingItem clothingItem = null;
					Clothing.Instance.Iterate(delegate(ClothingItem c)
					{
						bool flag9 = c.DisplayId == worldInfo.AvatarRelatedData.ClothingDisplayId;
						bool result;
						if (flag9)
						{
							clothingItem = c;
							result = false;
						}
						else
						{
							result = true;
						}
						return result;
					});
					bool avatarCheckOk = this.CheckAvatarData(clothingItem, archiveInfo);
					refers.SetAvatar(true);
					bool flag7 = avatarCheckOk;
					if (flag7)
					{
						refers.SetArchiveInfo((int)(index + 1), worldInfo, utcSave, year, characterName);
						refers.RefreshAvatar(worldInfo.AvatarRelatedData);
					}
					else
					{
						refers.SetArchiveInfo((int)(index + 1), worldInfo, utcSave, year, characterName);
						refers.SetAvatar(false);
					}
					refers.SetButtonsState(archiveInfo.BackupWorldsInfo.Count > 0, new Action(this.OnRevertBtnClicked), new Action(this.OnDeleteBtnClicked), new Action(this.OnContinueBtnClicked));
					refers.SetMouseTip(true, new string[]
					{
						characterName,
						this.GetWorldSettingsDesc(archiveInfo.WorldInfo)
					});
					this._readySlots.Add((int)index);
					bool hasDataDifference = this.CheckArchiveContentDifference(worldInfo);
					bool isArchiveVersionHigher = this.IsArchiveVersionHigher(worldInfo);
					refers.SetWarningButton((hasDataDifference || isArchiveVersionHigher) && !this._emptySlots.Contains((int)this._currSlot), hasDataDifference, delegate
					{
						ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("ModList", worldInfo.ModIds).SetObject("DLCList", worldInfo.DlcIds);
						UIElement.RecordContent.SetOnInitArgs(args);
						UIManager.Instance.MaskUI(UIElement.RecordContent);
					});
					ViewRecordSelect.RefreshWarningTips(refers.BtnWarning, hasDataDifference, isArchiveVersionHigher, worldInfo);
				}
				else
				{
					bool flag8 = archiveInfo == null;
					if (flag8)
					{
						throw new Exception("null archiveInfo when archiveStatus is NotEmpty");
					}
					refers.SetAsBrokenArchive();
					List<ValueTuple<long, WorldInfo>> backupWorldsInfo = archiveInfo.BackupWorldsInfo;
					refers.SetBrokenArchiveButtons(backupWorldsInfo != null && backupWorldsInfo.Count > 0, new Action(this.OnRevertBtnClicked), new Action(this.OnDeleteBtnClicked));
				}
			}
		}

		// Token: 0x060060B2 RID: 24754 RVA: 0x002C530C File Offset: 0x002C350C
		private bool CheckAvatarData(ClothingItem clothingItem, ArchiveInfo archiveInfo)
		{
			WorldInfo worldInfo = archiveInfo.WorldInfo;
			object obj;
			if (worldInfo == null)
			{
				obj = null;
			}
			else
			{
				AvatarRelatedData avatarRelatedData = worldInfo.AvatarRelatedData;
				obj = ((avatarRelatedData != null) ? avatarRelatedData.AvatarData : null);
			}
			bool flag = obj == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = AvatarClothColors.Instance.GetItem(archiveInfo.WorldInfo.AvatarRelatedData.AvatarData.ColorClothId) == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool hasClothing = false;
					bool flag3 = clothingItem != null;
					if (flag3)
					{
						List<DlcId> warningDlcIdList = new List<DlcId>();
						string text;
						bool removedSpecialDlc = this.CheckRemovedSpecialDlc(archiveInfo.WorldInfo, warningDlcIdList, out text);
						bool flag4 = removedSpecialDlc;
						if (flag4)
						{
							bool exists = warningDlcIdList.Exists(delegate(DlcId dlcId)
							{
								ImplementedDlcItem dlcConfigItem = DlcManager.GetDlcConfigItem((uint)dlcId.AppId);
								return ((dlcConfigItem != null) ? dlcConfigItem.Name : null) == clothingItem.DlcName;
							});
							bool flag5 = !exists;
							if (flag5)
							{
								hasClothing = true;
							}
						}
						else
						{
							hasClothing = true;
						}
					}
					bool hasValidAvatarId = false;
					List<byte> headIdList = AvatarHead.Instance.GetAllKeys();
					for (int i = 0; i < headIdList.Count; i++)
					{
						AvatarHeadItem item = AvatarHead.Instance[headIdList[i]];
						bool flag6 = item == null;
						if (!flag6)
						{
							int avatarId = (int)item.AvatarId;
							WorldInfo worldInfo2 = archiveInfo.WorldInfo;
							byte? b;
							if (worldInfo2 == null)
							{
								b = null;
							}
							else
							{
								AvatarRelatedData avatarRelatedData2 = worldInfo2.AvatarRelatedData;
								if (avatarRelatedData2 == null)
								{
									b = null;
								}
								else
								{
									AvatarData avatarData = avatarRelatedData2.AvatarData;
									b = ((avatarData != null) ? new byte?(avatarData.AvatarId) : null);
								}
							}
							byte? b2 = b;
							int? num = (b2 != null) ? new int?((int)b2.GetValueOrDefault()) : null;
							bool flag7 = avatarId == num.GetValueOrDefault() & num != null;
							if (flag7)
							{
								hasValidAvatarId = true;
								break;
							}
						}
					}
					result = (hasClothing && hasValidAvatarId);
				}
			}
			return result;
		}

		// Token: 0x060060B3 RID: 24755 RVA: 0x002C54D4 File Offset: 0x002C36D4
		private static void RefreshWarningTips(CButton btnWarning, bool hasDataDifference, bool isArchiveVersionHigher, WorldInfo worldInfo)
		{
			TooltipInvoker tip = btnWarning.GetComponent<TooltipInvoker>();
			string displayGameVersion = GameApp.Instance.GameVersion ?? string.Empty;
			bool flag = GameApp.Instance.ParsedGameVersion != null;
			if (flag)
			{
				displayGameVersion = GameApp.Instance.ParsedGameVersion.ToString();
			}
			GameVersionInfo gameVersionInfo = worldInfo.GameVersionInfo;
			string displayRecordVersion = ((gameVersionInfo != null) ? gameVersionInfo.GameVersionLastSaving : null) ?? string.Empty;
			Version parsedRecordVersion = GameVersionInfo.ParseGameVersion(displayRecordVersion);
			bool flag2 = parsedRecordVersion != null;
			if (flag2)
			{
				displayRecordVersion = parsedRecordVersion.ToString();
			}
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Set("ShowDataArea", hasDataDifference).Set("ShowVersionArea", isArchiveVersionHigher).Set("RecordVersion", displayRecordVersion).Set("GameVersion", displayGameVersion);
		}

		// Token: 0x060060B4 RID: 24756 RVA: 0x002C55B0 File Offset: 0x002C37B0
		private ArchiveInfo GetArchiveInfo(sbyte index)
		{
			return GlobalOperations.ArchivesInfo.CheckIndex((int)index) ? GlobalOperations.ArchivesInfo[(int)index] : null;
		}

		// Token: 0x060060B5 RID: 24757 RVA: 0x002C55DC File Offset: 0x002C37DC
		public static string GetCharacterName(WorldInfo worldInfo)
		{
			bool flag = worldInfo == null;
			string result;
			if (flag)
			{
				result = LocalStringManager.Get(LanguageKey.LK_Unknown);
			}
			else
			{
				result = ((SingletonObject.getInstance<GlobalSettings>().HideTaiwuOriginalSurname && WorldFunctionType.Get(worldInfo.WorldFunctionStatuses, 26)) ? NameCenter.FormatName(LocalStringManager.Get(LanguageKey.LK_Taiwu), worldInfo.TaiwuGivenName) : NameCenter.FormatName(worldInfo.TaiwuSurname, worldInfo.TaiwuGivenName));
			}
			return result;
		}

		// Token: 0x060060B6 RID: 24758 RVA: 0x002C5648 File Offset: 0x002C3848
		private string GetWorldSettingsDesc(WorldInfo worldInfo)
		{
			ViewRecordSelect.<>c__DisplayClass55_0 CS$<>8__locals1;
			CS$<>8__locals1.builder = EasyPool.Get<StringBuilder>();
			CS$<>8__locals1.builder.Clear();
			CS$<>8__locals1.builder.AppendLine("<SpName=mousetip_lingxing>" + LocalStringManager.Get(LanguageKey.UI_NewGame_WorldDetailSettings).SetColor("pinkyellow"));
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(1, (int)worldInfo.CombatDifficulty, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(11, (int)worldInfo.EnemyPracticeLevel, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(5, (int)worldInfo.HereticsAmountType, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(6, (int)worldInfo.BossInvasionSpeedType, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(7, (int)worldInfo.WorldResourceAmountType, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(12, (int)worldInfo.FavorabilityChange, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(14, (int)worldInfo.LootYield, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(2, (int)worldInfo.ReadingDifficulty, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(3, (int)worldInfo.BreakoutDifficulty, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(4, (int)worldInfo.LoopingDifficulty, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(13, (int)worldInfo.ProfessionUpgrade, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(8, (int)worldInfo.WorldPopulationType, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(0, (int)worldInfo.CharacterLifespanType, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(9, worldInfo.RestrictOptionsBehaviorType ? 0 : 1, ref CS$<>8__locals1);
			ViewRecordSelect.<GetWorldSettingsDesc>g__AppendItem|55_0(10, worldInfo.AllowRandomTaiwuHeir ? 0 : 1, ref CS$<>8__locals1);
			string res = CS$<>8__locals1.builder.ToString();
			EasyPool.Free<StringBuilder>(CS$<>8__locals1.builder);
			return res;
		}

		// Token: 0x060060B7 RID: 24759 RVA: 0x002C57A8 File Offset: 0x002C39A8
		private void CheckEnterGame(Action onConfirm, long timeStamp = -1L)
		{
			ViewRecordSelect.<>c__DisplayClass56_0 CS$<>8__locals1 = new ViewRecordSelect.<>c__DisplayClass56_0();
			CS$<>8__locals1.timeStamp = timeStamp;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.onConfirm = onConfirm;
			CS$<>8__locals1.archiveInfo = GlobalOperations.ArchivesInfo[(int)this._currSlot];
			WorldInfo worldInfo = (CS$<>8__locals1.timeStamp < 0L) ? CS$<>8__locals1.archiveInfo.WorldInfo : CS$<>8__locals1.archiveInfo.BackupWorldsInfo.Find(([TupleElementNames(new string[]
			{
				"timestamp",
				"worldInfo"
			})] ValueTuple<long, WorldInfo> b) => b.Item1 == CS$<>8__locals1.timeStamp).Item2;
			bool isArchiveVersionHigher = this.IsArchiveVersionHigher(worldInfo);
			bool flag = isArchiveVersionHigher;
			if (flag)
			{
				string title = LocalStringManager.Get(LanguageKey.LK_RecordSelect_ForbidEnter_By_Version_Dialog_Title);
				string content = LocalStringManager.Get(LanguageKey.LK_RecordSelect_ForbidEnter_By_Version_Dialog_Content);
				CommonUtils.ShowDialog(title, content, null, EDialogType.None);
			}
			else
			{
				List<DlcId> warningDlcIdList = new List<DlcId>();
				CS$<>8__locals1.removedSpecialDlc = this.CheckRemovedSpecialDlc(worldInfo, warningDlcIdList, out CS$<>8__locals1.names);
				bool removedSpecialDlc = CS$<>8__locals1.removedSpecialDlc;
				if (removedSpecialDlc)
				{
					string title2 = LocalStringManager.Get(LanguageKey.LK_RecordSelect_ForbidEnter_Dialog_Title);
					string content2 = LocalStringManager.GetFormat(LanguageKey.LK_RecordSelect_ForbidEnter_Dialog_Content, CS$<>8__locals1.names);
					CommonUtils.ShowDialog(title2, content2, null, EDialogType.None);
				}
				else
				{
					bool addedSpecialDlc = this.CheckAddedSpecialDlc(worldInfo, warningDlcIdList, out CS$<>8__locals1.names);
					CS$<>8__locals1.removedSpecialMod = this.CheckRemovedSpecialMod(worldInfo);
					CS$<>8__locals1.confirmAddedSpecialDlc = false;
					CS$<>8__locals1.confirmRemovedSpecialMod = false;
					bool flag2 = addedSpecialDlc;
					if (flag2)
					{
						CS$<>8__locals1.<CheckEnterGame>g__ShowDialogForAddedSpecialDlc|2();
					}
					else
					{
						bool removedSpecialMod = CS$<>8__locals1.removedSpecialMod;
						if (removedSpecialMod)
						{
							CS$<>8__locals1.<CheckEnterGame>g__ShowRemovedSpecialMod|3();
						}
						else
						{
							CS$<>8__locals1.<CheckEnterGame>g__OnConfirm|1();
						}
					}
				}
			}
		}

		// Token: 0x060060B8 RID: 24760 RVA: 0x002C5904 File Offset: 0x002C3B04
		private bool CheckArchiveContentDifference(WorldInfo worldInfo)
		{
			bool modIsDifferent = worldInfo.ModIds.ContentIsDifferent(ModManager.EnabledMods);
			HashSet<ulong> gameDlcSet = (from id in SingletonObject.getInstance<DlcManager>().GetDlcIdList()
			select id.AppId).ToHashSet<ulong>();
			HashSet<ulong> hashSet;
			if (worldInfo.DlcIds != null)
			{
				hashSet = (from id in worldInfo.DlcIds
				select id.AppId).ToHashSet<ulong>();
			}
			else
			{
				hashSet = new HashSet<ulong>();
			}
			HashSet<ulong> worldDlcSet = hashSet;
			bool dlcIsDifferent = gameDlcSet.Except(worldDlcSet).Any<ulong>() || worldDlcSet.Except(gameDlcSet).Any<ulong>();
			return modIsDifferent || dlcIsDifferent;
		}

		// Token: 0x060060B9 RID: 24761 RVA: 0x002C59C0 File Offset: 0x002C3BC0
		private bool CheckRemovedSpecialMod(WorldInfo worldInfo)
		{
			this._removedModList.Clear();
			bool flag = ((worldInfo != null) ? worldInfo.ModIds : null) == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this._removedModList.AddRange(worldInfo.ModIds.Except(ModManager.EnabledMods));
				foreach (ModId modId in this._removedModList)
				{
					bool flag2 = ModManager.PlatformMods.Contains(modId);
					if (flag2)
					{
						bool hasChangeGameConfig = SteamManager.CheckModHasChangeGameConfig(modId);
						bool flag3 = hasChangeGameConfig;
						if (flag3)
						{
							return true;
						}
						ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
						bool flag4 = modInfo != null;
						if (flag4)
						{
							return modInfo.ChangeConfig || modInfo.HasArchive;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060060BA RID: 24762 RVA: 0x002C5AA8 File Offset: 0x002C3CA8
		private bool CheckAddedSpecialDlc(WorldInfo worldInfo, List<DlcId> warningDlcIdList, out string names)
		{
			names = string.Empty;
			warningDlcIdList.Clear();
			this._addedDlcList.Clear();
			List<DlcId> dlcIdList = SingletonObject.getInstance<DlcManager>().GetDlcIdList();
			List<DlcId> addedDlcList = this._addedDlcList;
			WorldInfo worldInfo2 = worldInfo;
			IEnumerable<DlcId> collection;
			if (((worldInfo2 != null) ? worldInfo2.DlcIds : null) != null)
			{
				collection = from gameDlc in dlcIdList
				where worldInfo.DlcIds.All((DlcId worldDlc) => worldDlc.AppId != gameDlc.AppId)
				select gameDlc;
			}
			else
			{
				IEnumerable<DlcId> enumerable = dlcIdList;
				collection = enumerable;
			}
			addedDlcList.AddRange(collection);
			warningDlcIdList.AddRange(this._addedDlcList);
			bool needWarning = this._addedDlcList.Count > 0;
			bool flag = needWarning;
			if (flag)
			{
				names = this.GetDlcNames(warningDlcIdList);
			}
			return needWarning;
		}

		// Token: 0x060060BB RID: 24763 RVA: 0x002C5B54 File Offset: 0x002C3D54
		private bool CheckRemovedSpecialDlc(WorldInfo worldInfo, List<DlcId> warningDlcIdList, out string names)
		{
			names = string.Empty;
			warningDlcIdList.Clear();
			this._removedDlcList.Clear();
			bool flag = ((worldInfo != null) ? worldInfo.DlcIds : null) == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				List<DlcId> dlcIdList = SingletonObject.getInstance<DlcManager>().GetDlcIdList();
				this._removedDlcList.AddRange(from dlc in worldInfo.DlcIds
				where dlcIdList.All((DlcId d) => d.AppId != dlc.AppId)
				select dlc);
				warningDlcIdList.AddRange(this._removedDlcList);
				bool needWarning = this._removedDlcList.Count > 0;
				bool flag2 = needWarning;
				if (flag2)
				{
					names = this.GetDlcNames(warningDlcIdList);
				}
				result = needWarning;
			}
			return result;
		}

		// Token: 0x060060BC RID: 24764 RVA: 0x002C5C00 File Offset: 0x002C3E00
		private string GetDlcNames(List<DlcId> warningDlcIdList)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			for (int i = 0; i < warningDlcIdList.Count; i++)
			{
				DlcId dlcId = warningDlcIdList[i];
				string dlcName = SingletonObject.getInstance<DlcManager>().GetDlcName(dlcId).SetColor("orange");
				sb.Append(dlcName);
				bool flag = i < warningDlcIdList.Count - 1;
				if (flag)
				{
					sb.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
				}
			}
			string result = sb.ToString();
			EasyPool.Free<StringBuilder>(sb);
			return result;
		}

		// Token: 0x060060BD RID: 24765 RVA: 0x002C5C8C File Offset: 0x002C3E8C
		private bool IsArchiveVersionHigher(WorldInfo worldInfo)
		{
			Version parsedGameVersion = GameApp.Instance.ParsedGameVersion;
			GameVersionInfo gameVersionInfo = worldInfo.GameVersionInfo;
			Version archiveVersion = GameVersionInfo.ParseGameVersion((gameVersionInfo != null) ? gameVersionInfo.GameVersionLastSaving : null);
			bool flag = parsedGameVersion == null || archiveVersion == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = parsedGameVersion.Major != archiveVersion.Major;
				if (flag2)
				{
					result = (archiveVersion.Major > parsedGameVersion.Major);
				}
				else
				{
					bool flag3 = parsedGameVersion.Minor != archiveVersion.Minor;
					if (flag3)
					{
						result = (archiveVersion.Minor > parsedGameVersion.Minor);
					}
					else
					{
						bool flag4 = parsedGameVersion.Build != archiveVersion.Build;
						if (flag4)
						{
							result = (archiveVersion.Build > parsedGameVersion.Build);
						}
						else
						{
							bool flag5 = parsedGameVersion.Revision != archiveVersion.Revision;
							result = (flag5 && archiveVersion.Revision > parsedGameVersion.Revision);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060060BE RID: 24766 RVA: 0x002C5D80 File Offset: 0x002C3F80
		private void OnScrollTabToggleChange(int newTogIndex, int preTogIndex)
		{
			ViewRecordSelect._curScrollIndex = (sbyte)newTogIndex;
			this.OnScrollTabChange();
		}

		// Token: 0x060060BF RID: 24767 RVA: 0x002C5D91 File Offset: 0x002C3F91
		private void OnScrollTabChange()
		{
			this.tabGroupToggleGroup.OnActiveIndexChange += delegate(int newTog, int oldTog)
			{
				List<GameObject> refers = this.taiwuScroll.CGetList<GameObject>("ScrollRoot");
				refers[oldTog].SetActive(false);
				refers[newTog].SetActive(true);
			};
		}

		// Token: 0x060060C0 RID: 24768 RVA: 0x002C5DAC File Offset: 0x002C3FAC
		private void ShowMask()
		{
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("ShowBlackMask", true);
			box.Set("ShowWaitAnimation", true);
			box.Set("Message", LocalStringManager.Get(LanguageKey.LK_Waiting));
			UIElement.FullScreenMask.SetOnInitArgs(box);
			UIElement.FullScreenMask.Show();
			this.StopMaskTimer();
			this.StartMaskTimer();
		}

		// Token: 0x060060C1 RID: 24769 RVA: 0x002C5E15 File Offset: 0x002C4015
		private void HideMask()
		{
			UIElement.FullScreenMask.Hide(false);
			this.StopMaskTimer();
		}

		// Token: 0x060060C2 RID: 24770 RVA: 0x002C5E2B File Offset: 0x002C402B
		private IEnumerator CorMaskTimer()
		{
			yield return new WaitForSeconds(30f);
			bool exist = UIElement.FullScreenMask.Exist;
			if (exist)
			{
				this.HideMask();
				string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				string content = LocalStringManager.Get(LanguageKey.LK_Mod_Loading_Failed);
				CommonUtils.ShowDialog(title, content, null, EDialogType.None);
				title = null;
				content = null;
			}
			yield break;
		}

		// Token: 0x060060C3 RID: 24771 RVA: 0x002C5E3C File Offset: 0x002C403C
		private void StopMaskTimer()
		{
			bool flag = this._corMaskTimer == null;
			if (!flag)
			{
				base.StopCoroutine(this._corMaskTimer);
				this._corMaskTimer = null;
			}
		}

		// Token: 0x060060C4 RID: 24772 RVA: 0x002C5E6D File Offset: 0x002C406D
		private void StartMaskTimer()
		{
			this._corMaskTimer = base.StartCoroutine(this.CorMaskTimer());
		}

		// Token: 0x060060C7 RID: 24775 RVA: 0x002C5FB0 File Offset: 0x002C41B0
		[CompilerGenerated]
		private void <OnArchiveInfoLoaded>g__Refresh|25_0()
		{
			for (sbyte i = 0; i < 5; i += 1)
			{
				this.RefreshArchiveAtIndex(i, this._panels[(int)i]);
			}
			sbyte index = SingletonObject.getInstance<GlobalSettings>().LastEnterWorldIndex;
			ArchiveInfo[] archivesInfo = GlobalOperations.ArchivesInfo;
			sbyte? b = (archivesInfo != null) ? new sbyte?(archivesInfo[(int)index].Status) : null;
			int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
			int num2 = 1;
			bool flag = !(num.GetValueOrDefault() == num2 & num != null);
			if (flag)
			{
				index = -1;
				sbyte j = 0;
				for (;;)
				{
					int num3 = (int)j;
					ArchiveInfo[] archivesInfo2 = GlobalOperations.ArchivesInfo;
					num = ((archivesInfo2 != null) ? new int?(archivesInfo2.Length) : null);
					if (!(num3 < num.GetValueOrDefault() & num != null))
					{
						goto IL_F6;
					}
					bool flag2 = GlobalOperations.ArchivesInfo[(int)j].Status == 1;
					if (flag2)
					{
						break;
					}
					j += 1;
				}
				index = j;
				IL_F6:;
			}
			bool flag3 = index >= 0;
			if (flag3)
			{
				this._currentPage = (int)(index / 5);
				this.RefreshPageDisplay();
				this.recordRootToggleGroup.Set((int)index, false);
			}
			else
			{
				this._currentPage = 0;
				this.RefreshPageDisplay();
				this._currSlot = -1;
				this.recordRootToggleGroup.DeSelectWithoutNotify();
			}
			GameApp.ClockAndLogInfo("UI_RecordSelect:RefreshCharacters Complete", false);
			this.animationRoot.anchoredPosition = Vector2.zero;
			Game.Views.GameLineScroll.ScrollHelper.SetUIElement(this.Element);
			Game.Views.GameLineScroll.ScrollHelper.OnOnRecordRootToggleChanged(this.taiwuScroll, (int)this._currSlot);
		}

		// Token: 0x060060C9 RID: 24777 RVA: 0x002C61A0 File Offset: 0x002C43A0
		[CompilerGenerated]
		internal static void <GetWorldSettingsDesc>g__AppendItem|55_0(byte worldCreationKey, int index, ref ViewRecordSelect.<>c__DisplayClass55_0 A_2)
		{
			WorldCreationItem config = WorldCreation.Instance.GetItem(worldCreationKey);
			bool flag = !config.Icons.CheckIndex(index) || !config.Options.CheckIndex(index);
			if (flag)
			{
				A_2.builder.AppendLine("<indent=50>" + config.Name + ":</indent>");
			}
			else
			{
				string iconTag = TMPTextSpriteHelper.GetStringWithTextSpriteTag(config.Icons[index]);
				A_2.builder.AppendLine(string.Concat(new string[]
				{
					"  ",
					iconTag,
					config.Name,
					":",
					config.Options[index]
				}));
			}
		}

		// Token: 0x040042FD RID: 17149
		private RecordInfoItem[] _panels;

		// Token: 0x040042FE RID: 17150
		[SerializeField]
		private Refers taiwuScroll;

		// Token: 0x040042FF RID: 17151
		[SerializeField]
		private CToggleGroup recordRootToggleGroup;

		// Token: 0x04004300 RID: 17152
		[SerializeField]
		private CToggleGroup tabGroupToggleGroup;

		// Token: 0x04004301 RID: 17153
		[SerializeField]
		private RecordInfoItem recordInfoTemplate;

		// Token: 0x04004302 RID: 17154
		[SerializeField]
		private RectTransform animationRoot;

		// Token: 0x04004303 RID: 17155
		public const sbyte RecodeCount = 5;

		// Token: 0x04004304 RID: 17156
		private const int SlotsPerPage = 5;

		// Token: 0x04004305 RID: 17157
		private const int TotalPages = 1;

		// Token: 0x04004306 RID: 17158
		private int _currentPage = 0;

		// Token: 0x04004307 RID: 17159
		private IEnumerator _performing;

		// Token: 0x04004308 RID: 17160
		private HashSet<int> _emptySlots = new HashSet<int>();

		// Token: 0x04004309 RID: 17161
		private HashSet<int> _readySlots = new HashSet<int>();

		// Token: 0x0400430A RID: 17162
		private sbyte _currSlot = -1;

		// Token: 0x0400430B RID: 17163
		private DialogCmd _modCheckDialog = new DialogCmd
		{
			Title = LocalStringManager.Get("")
		};

		// Token: 0x0400430C RID: 17164
		private HashSet<string> enabledMods = new HashSet<string>();

		// Token: 0x0400430D RID: 17165
		private HashSet<string> removedMods = new HashSet<string>();

		// Token: 0x0400430E RID: 17166
		private readonly List<ModId> _modDependenciesChangedList = new List<ModId>();

		// Token: 0x0400430F RID: 17167
		private sbyte _modDependenciesChangedSlotIndex = -1;

		// Token: 0x04004310 RID: 17168
		private static sbyte _curScrollIndex;

		// Token: 0x04004311 RID: 17169
		private bool _isEnteringNewGame = false;

		// Token: 0x04004312 RID: 17170
		private readonly List<ModId> _removedModList = new List<ModId>();

		// Token: 0x04004313 RID: 17171
		private readonly List<DlcId> _addedDlcList = new List<DlcId>();

		// Token: 0x04004314 RID: 17172
		private readonly List<DlcId> _removedDlcList = new List<DlcId>();

		// Token: 0x04004315 RID: 17173
		private Coroutine _corMaskTimer;
	}
}
