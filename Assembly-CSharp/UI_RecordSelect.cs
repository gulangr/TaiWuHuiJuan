using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.ModSystem;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.DLC;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Mod;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.GameDataBridge;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000399 RID: 921
public class UI_RecordSelect : UIBase
{
	// Token: 0x06003758 RID: 14168 RVA: 0x001BD380 File Offset: 0x001BB580
	private void OnStartBtnTriggerEnter()
	{
		bool interactable = this._startButton.interactable;
		if (interactable)
		{
			Refers buttonRefers = this._startButton.GetComponent<Refers>();
			CImage aurora = buttonRefers.CGet<CImage>("Aurora");
			SkeletonGraphic blade = buttonRefers.CGet<SkeletonGraphic>("Animation");
			this.ChangeAmbiencePlaying(true);
			aurora.sprite = buttonRefers.CGet<Sprite>("AuroraAlive");
			aurora.rectTransform.DOKill(false);
			aurora.rectTransform.DOSizeDelta(aurora.sprite.rect.size, 0.3f, false).SetEase(Ease.OutExpo);
			buttonRefers.CGet<GameObject>("EffectAlive").SetActive(true);
			buttonRefers.CGet<GameObject>("EffectDead").SetActive(false);
			blade.timeScale = 1f;
		}
	}

	// Token: 0x06003759 RID: 14169 RVA: 0x001BD44C File Offset: 0x001BB64C
	private void OnStartBtnTriggerExit()
	{
		Refers buttonRefers = this._startButton.GetComponent<Refers>();
		CImage aurora = buttonRefers.CGet<CImage>("Aurora");
		SkeletonGraphic blade = buttonRefers.CGet<SkeletonGraphic>("Animation");
		this.ChangeAmbiencePlaying(false);
		aurora.sprite = buttonRefers.CGet<Sprite>("AuroraDead");
		aurora.rectTransform.DOKill(false);
		aurora.rectTransform.DOSizeDelta(aurora.sprite.rect.size, 0.3f, false).SetEase(Ease.OutExpo);
		buttonRefers.CGet<GameObject>("EffectAlive").SetActive(false);
		buttonRefers.CGet<GameObject>("EffectDead").SetActive(true);
		blade.timeScale = 0f;
	}

	// Token: 0x0600375A RID: 14170 RVA: 0x001BD500 File Offset: 0x001BB700
	private void OnRecordRootToggleChanged(CToggleObsolete newTog, CToggleObsolete preTog)
	{
		this._startButton.interactable = false;
		bool flag = null != newTog;
		if (flag)
		{
			this._currSlot = (sbyte)newTog.Key;
			Refers newTogRefers = newTog.GetComponent<Refers>();
			RectTransform newTogBtnLayout = newTogRefers.CGet<RectTransform>("BtnLayout");
			newTogBtnLayout.DOAnchorPosY(40f, 0.3f, false);
			newTogBtnLayout.DOScaleX(1f, 0.3f);
			bool flag2 = this._emptySlots.Contains((int)this._currSlot);
			if (flag2)
			{
				this.OnEnteringNewGame();
				return;
			}
			ScrollHelper.OnOnRecordRootToggleChanged(base.CGet<Refers>("TaiwuScroll"), (int)this._currSlot);
		}
		bool flag3 = null != preTog;
		if (flag3)
		{
			Refers preTogRefers = preTog.GetComponent<Refers>();
			RectTransform preTogBtnLayout = preTogRefers.CGet<RectTransform>("BtnLayout");
			preTogBtnLayout.DOAnchorPosY(160f, 0.3f, false);
			preTogBtnLayout.DOScaleX(0f, 0.3f);
		}
		this.OnScrollTabChange();
	}

	// Token: 0x0600375B RID: 14171 RVA: 0x001BD5F4 File Offset: 0x001BB7F4
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
					this.<OnArchiveInfoLoaded>g__Refresh|26_0();
					this.HideMask();
				});
				return;
			}
		}
		this.<OnArchiveInfoLoaded>g__Refresh|26_0();
	}

	// Token: 0x0600375C RID: 14172 RVA: 0x001BD6D8 File Offset: 0x001BB8D8
	private void OnEnterWorldLoadStart()
	{
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 50));
	}

	// Token: 0x0600375D RID: 14173 RVA: 0x001BD6F8 File Offset: 0x001BB8F8
	private void OnEnterWorldLoadFinish()
	{
		GameApp.Instance.ChangeGameState(EGameState.InGame, null);
		bool flag = SingletonObject.getInstance<BasicGameData>().CurrDate > 8;
		if (flag)
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

	// Token: 0x0600375E RID: 14174 RVA: 0x001BD780 File Offset: 0x001BB980
	private void OnPlayIntroSucceeded()
	{
		SingletonObject.getInstance<GlobalSettings>().BgmOn = false;
		UIManager.Instance.StackToUI(UIElement.BlockInteract);
	}

	// Token: 0x0600375F RID: 14175 RVA: 0x001BD79F File Offset: 0x001BB99F
	private void OnPlayIntroFailed()
	{
		PredefinedLog.Show(9);
		UIElement.CgPlayer.Hide(false);
	}

	// Token: 0x06003760 RID: 14176 RVA: 0x001BD7B6 File Offset: 0x001BB9B6
	private void OnEnteringNewGame()
	{
		this._panels[(int)this._currSlot].CGet<CButtonObsolete>("BtnStart").interactable = false;
		SingletonObject.getInstance<YieldHelper>().StartYield(this.ToNewGame());
	}

	// Token: 0x06003761 RID: 14177 RVA: 0x001BD7E8 File Offset: 0x001BB9E8
	private void OnEnteringAvatarCreation()
	{
		bool useInscribedChar = GlobalOperations.InscribedCharacters != null && GlobalOperations.InscribedCharacters.Count > 0;
		UIElement.NewGame.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("Index", this._currSlot).Set("UseInscribedChar", useInscribedChar));
		GameApp.Instance.ChangeGameState(EGameState.NewGame, null);
	}

	// Token: 0x06003762 RID: 14178 RVA: 0x001BD848 File Offset: 0x001BBA48
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

	// Token: 0x06003763 RID: 14179 RVA: 0x001BD884 File Offset: 0x001BBA84
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
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, new Action(ScrollHelper.OnSaveFileDeleted));
		}
	}

	// Token: 0x06003764 RID: 14180 RVA: 0x001BD914 File Offset: 0x001BBB14
	private void OnRevertBtnClicked()
	{
		ArchiveInfo archiveInfo = GlobalOperations.ArchivesInfo[(int)this._currSlot];
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("OnConfirmEnter", new Action<long>(this.OnEnterBackupWorldConfirmed));
		argBox.SetObject("ArchiveData", archiveInfo);
		argBox.Set("ArchiveIndex", this._currSlot);
		UIElement.RevertArchive.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.RevertArchive, true);
	}

	// Token: 0x06003765 RID: 14181 RVA: 0x001BD98C File Offset: 0x001BBB8C
	private void OnDeleteBtnClicked()
	{
		base.CGet<CToggleGroupObsolete>("RecordRoot").Set((int)this._currSlot, false, false);
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
			dialogCmd2.Content = LocalStringManager.GetFormat(LanguageKey.UI_RecordSelect_DeleteRecord_Confirm, UI_RecordSelect.GetCharacterName(this.GetArchiveInfo(this._currSlot).WorldInfo)).ColorReplace();
			dialogCmd2.Type = 1;
			arg = dialogCmd2;
			dialogCmd2.Yes = new Action(this.OnDeleteOperationConfirmed);
		}
		dialog.SetOnInitArgs(argumentBox.SetObject(key, arg));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06003766 RID: 14182 RVA: 0x001BDA98 File Offset: 0x001BBC98
	private void OnContinueBtnClicked()
	{
		this.CheckEnterGame(delegate
		{
			GameApp.ClockAndLogInfo("Clicked LoadWorld button", true);
			this._panels[(int)this._currSlot].CGet<CButtonObsolete>("BtnStart").interactable = false;
			bool flag = this._performing == null;
			if (flag)
			{
				base.StartCoroutine(this._performing = this.LoadGame(this._currSlot));
			}
		}, -1L);
	}

	// Token: 0x06003767 RID: 14183 RVA: 0x001BDAB0 File Offset: 0x001BBCB0
	public override void OnInit(ArgumentBox argsBox)
	{
		this._modDependenciesChangedSlotIndex = -1;
		base.GetComponent<CanvasGroup>().alpha = 1f;
		base.CGet<CButtonObsolete>("EnterGame").GetComponent<CanvasGroup>().alpha = 1f;
		ScrollHelper.SetQuickHideBanned(false);
	}

	// Token: 0x06003768 RID: 14184 RVA: 0x001BDAED File Offset: 0x001BBCED
	public override void PlayAudioOut()
	{
	}

	// Token: 0x06003769 RID: 14185 RVA: 0x001BDAF0 File Offset: 0x001BBCF0
	protected override void OnClick(Transform btn)
	{
		ScrollHelper.OnClick(btn.GetComponent<CButtonObsolete>());
		bool flag = btn.name == "CloseButton";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x0600376A RID: 14186 RVA: 0x001BDB28 File Offset: 0x001BBD28
	public override void QuickHide()
	{
		bool quickHideBanned = ScrollHelper.GetQuickHideBanned();
		if (quickHideBanned)
		{
			ScrollHelper.QuickHide();
		}
		else
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			base.QuickHide();
			this.ChangeAmbiencePlaying(false);
		}
	}

	// Token: 0x0600376B RID: 14187 RVA: 0x001BDB6C File Offset: 0x001BBD6C
	private void Awake()
	{
		this._panels = new RecordInfo[15];
		CToggleGroupObsolete toggleGroup = base.CGet<CToggleGroupObsolete>("RecordRoot");
		for (int i = 0; i < 15; i++)
		{
			string key = string.Format("RecordInfo_{0}", i);
			RecordInfo panel = base.CGet<RecordInfo>("RecordInfo_0");
			bool flag = i != 0;
			if (flag)
			{
				panel = Object.Instantiate<RecordInfo>(panel, panel.transform.parent);
				panel.name = key;
				CToggleObsolete toggle = panel.GetComponent<CToggleObsolete>();
				toggle.Key = i;
				toggleGroup.Add(toggle);
			}
			this._panels[i] = panel;
			RectTransform btnLayout = panel.CGet<RectTransform>("BtnLayout");
			btnLayout.DOAnchorPosY(160f, 0.3f, false);
			btnLayout.DOScaleX(0f, 0.3f);
			this.SetAsEmptyArchive(panel);
		}
		toggleGroup.InitPreOnToggle(-1);
		toggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnRecordRootToggleChanged);
		this._startButton = base.CGet<CButtonObsolete>("EnterGame");
		PointerTrigger buttonTrigger = this._startButton.GetComponent<PointerTrigger>();
		buttonTrigger.EnterEvent.AddListener(new UnityAction(this.OnStartBtnTriggerEnter));
		buttonTrigger.ExitEvent.AddListener(new UnityAction(this.OnStartBtnTriggerExit));
		base.CGet<CToggleGroupObsolete>("TabGroup").InitPreOnToggle(-1);
		base.CGet<CToggleGroupObsolete>("TabGroup").OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnScrollTabToggleChange);
		this.btnPageLeft.ClearAndAddListener(new Action(this.OnPageLeft));
		this.btnPageRight.ClearAndAddListener(new Action(this.OnPageRight));
	}

	// Token: 0x0600376C RID: 14188 RVA: 0x001BDD1C File Offset: 0x001BBF1C
	private void OnPageLeft()
	{
		bool flag = this._currentPage > 0;
		if (flag)
		{
			this._currentPage--;
			this.RefreshPageDisplay();
		}
	}

	// Token: 0x0600376D RID: 14189 RVA: 0x001BDD50 File Offset: 0x001BBF50
	private void OnPageRight()
	{
		bool flag = this._currentPage < 2;
		if (flag)
		{
			this._currentPage++;
			this.RefreshPageDisplay();
		}
	}

	// Token: 0x0600376E RID: 14190 RVA: 0x001BDD84 File Offset: 0x001BBF84
	private void RefreshPageDisplay()
	{
		for (int i = 0; i < 15; i++)
		{
			int pageOfSlot = i / 5;
			this._panels[i].gameObject.SetActive(pageOfSlot == this._currentPage);
		}
		bool showLeft = this._currentPage > 0;
		bool showRight = this._currentPage < 2;
		this.btnPageLeft.gameObject.SetActive(showLeft);
		this.textPageLeft.gameObject.SetActive(showLeft);
		this.textPageLeft.text = string.Format("{0}/{1}", this._currentPage, 3);
		this.btnPageRight.gameObject.SetActive(showRight);
		this.textPageRight.gameObject.SetActive(showRight);
		this.textPageRight.text = string.Format("{0}/{1}", this._currentPage + 2, 3);
	}

	// Token: 0x0600376F RID: 14191 RVA: 0x001BDE74 File Offset: 0x001BC074
	private void OnEnable()
	{
		this._startButton.interactable = false;
		this._emptySlots.Clear();
		this._readySlots.Clear();
		CToggleGroupObsolete toggleGroup = base.CGet<CToggleGroupObsolete>("RecordRoot");
		for (sbyte i = 0; i < 15; i += 1)
		{
			toggleGroup.Set((int)i, false, false);
		}
		GEvent.Add(EEvents.ArchivesInfoReady, new GEvent.Callback(this.OnArchiveInfoLoaded));
		GameApp.ClockAndLogInfo("Send GetArchivesInfo", true);
		GlobalOperations.GetArchivesInfo();
		ModManager.UpdateModList(null);
	}

	// Token: 0x06003770 RID: 14192 RVA: 0x001BDF00 File Offset: 0x001BC100
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

	// Token: 0x06003771 RID: 14193 RVA: 0x001BDF4C File Offset: 0x001BC14C
	public void ChangeAmbiencePlaying(bool playing)
	{
		bool quickHideBanned = ScrollHelper.GetQuickHideBanned();
		if (!quickHideBanned)
		{
			AudioManager.Instance.PlayAmbience(playing ? "Continue_unclick" : AudioManager.DummyAudioName, 0.5f, 100);
		}
	}

	// Token: 0x06003772 RID: 14194 RVA: 0x001BDF86 File Offset: 0x001BC186
	private IEnumerator ToNewGame()
	{
		this.ChangeAmbiencePlaying(false);
		ScrollHelper.SetQuickHideBanned(true);
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

	// Token: 0x06003773 RID: 14195 RVA: 0x001BDF95 File Offset: 0x001BC195
	private IEnumerator LoadGame(sbyte index)
	{
		this.ChangeAmbiencePlaying(false);
		ScrollHelper.SetQuickHideBanned(true);
		SingletonObject.getInstance<GlobalSettings>().LastEnterWorldIndex = index;
		SingletonObject.getInstance<GlobalSettings>().HaveDoneSave = true;
		GlobalOperations.LoadWorld(index, -1L);
		GameApp.Instance.ChangeGameState(EGameState.Loading, EasyPool.Get<ArgumentBox>().SetObject("OnLoadingFinish", new Action(this.OnEnterWorldLoadFinish)).SetObject("OnLoadingStart", new Action(this.OnEnterWorldLoadStart)));
		this._performing = null;
		yield break;
	}

	// Token: 0x06003774 RID: 14196 RVA: 0x001BDFAC File Offset: 0x001BC1AC
	private void SetAsEmptyArchive(RecordInfo refers)
	{
		refers.CGet<Renamer>("Renamer").Refresh(LocalStringManager.Get(LanguageKey.UI_RecordSelect_EmptyScroll), 6, false, null);
		refers.CGet<GameObject>("RecordBroken").SetActive(false);
		refers.CGet<Game.Components.Avatar.Avatar>("Avatar").gameObject.SetActive(false);
		refers.CGet<GameObject>("LocationBar").SetActive(false);
		refers.CGet<TooltipInvoker>("SettingsTips").gameObject.SetActive(false);
		refers.CGet<TextMeshProUGUI>("GameTime").text = string.Empty;
		refers.CGet<TextMeshProUGUI>("AgeSamsara").text = string.Empty;
		refers.CGet<CImage>("Click").SetSprite("newgame_kuang_0_5", false, null);
	}

	// Token: 0x06003775 RID: 14197 RVA: 0x001BE070 File Offset: 0x001BC270
	private void RefreshArchiveAtIndex(sbyte index, RecordInfo refers)
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
		TooltipInvoker mouseTip = refers.CGet<TooltipInvoker>("MouseTip");
		mouseTip.enabled = false;
		CButtonObsolete deleteBtn = refers.CGet<CButtonObsolete>("BtnDelete");
		CButtonObsolete revertBtn = refers.CGet<CButtonObsolete>("BtnRevert");
		CButtonObsolete continueBtn = refers.CGet<CButtonObsolete>("BtnContinue");
		CButtonObsolete btnWarning = refers.CGet<CButtonObsolete>("BtnWarning");
		btnWarning.gameObject.SetActive(false);
		refers.CGet<RectTransform>("BtnLayout").gameObject.SetActive(archiveStatus != 0);
		this._emptySlots.Remove((int)index);
		this._readySlots.Remove((int)index);
		bool flag3 = archiveStatus == 0;
		if (flag3)
		{
			this.SetAsEmptyArchive(refers);
			mouseTip.enabled = true;
			mouseTip.PresetParam = new string[]
			{
				LocalStringManager.Get(LanguageKey.UI_RecordSelect_Tip_CreateTaiwu_Title),
				LocalStringManager.Get(LanguageKey.UI_RecordSelect_Tip_CreateTaiwu_Content)
			};
			this._emptySlots.Add((int)index);
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
				string characterName = UI_RecordSelect.GetCharacterName(archiveInfo.WorldInfo);
				refers.CGet<CImage>("Click").SetSprite("newgame_kuang_0_0", false, null);
				refers.CGet<GameObject>("RecordBroken").SetActive(false);
				ClothingItem clothingItem = null;
				Clothing.Instance.Iterate(delegate(ClothingItem c)
				{
					bool flag11 = c.DisplayId == worldInfo.AvatarRelatedData.ClothingDisplayId;
					bool result;
					if (flag11)
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
				bool flag7 = avatarCheckOk;
				if (flag7)
				{
					Game.Components.Avatar.Avatar avatar = refers.CGet<Game.Components.Avatar.Avatar>("Avatar");
					avatar.gameObject.SetActive(true);
					avatar.Refresh(worldInfo.AvatarRelatedData);
				}
				else
				{
					AtlasInfo.Instance.GetSprite("recordcelect_unknowncharacter", delegate(Sprite sp)
					{
						refers.CGet<Game.Components.Avatar.Avatar>("Avatar").gameObject.SetActive(true);
						refers.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(sp);
					});
				}
				refers.Index = (int)(index + 1);
				refers.CharacterName = characterName;
				refers.CGet<Renamer>("Renamer").Refresh(refers.GetRecordFullName(null), 6, true, refers.GetRecordFullName(""));
				refers.CGet<GameObject>("LocationBar").SetActive(true);
				refers.CGet<TextMeshProUGUI>("Location").text = worldInfo.MapStateName;
				refers.CGet<TextMeshProUGUI>("Location2").text = worldInfo.MapAreaName;
				refers.CGet<TextMeshProUGUI>("GameTime").text = utcSave.ToLocalTime().ToString("yyyy-MM-dd [HH:mm]");
				refers.CGet<TextMeshProUGUI>("AgeSamsara").text = LocalStringManager.GetFormat(LanguageKey.UI_RecordSelect_Year, year, worldInfo.TaiwuGenerationsCount);
				revertBtn.gameObject.SetActive(true);
				bool flag8 = archiveInfo.BackupWorldsInfo.Count > 0;
				if (flag8)
				{
					revertBtn.interactable = true;
					revertBtn.ClearAndAddListener(new Action(this.OnRevertBtnClicked));
					revertBtn.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
				}
				else
				{
					revertBtn.interactable = false;
					revertBtn.GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
				}
				deleteBtn.gameObject.SetActive(true);
				deleteBtn.interactable = true;
				deleteBtn.ClearAndAddListener(new Action(this.OnDeleteBtnClicked));
				continueBtn.gameObject.SetActive(true);
				continueBtn.interactable = true;
				continueBtn.ClearAndAddListener(new Action(this.OnContinueBtnClicked));
				continueBtn.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
				refers.GetComponent<PointClickBridge>().OnDoubleClick = delegate()
				{
					bool interactable = continueBtn.interactable;
					if (interactable)
					{
						AudioManager.Instance.PlaySound("Continue_click", false, false);
						continueBtn.onClick.Invoke();
					}
				};
				mouseTip.PresetParam = new string[]
				{
					characterName,
					this.GetWorldSettingsDesc(archiveInfo.WorldInfo)
				};
				mouseTip.enabled = true;
				this._readySlots.Add((int)index);
				bool hasDataDifference = this.CheckArchiveContentDifference(worldInfo);
				bool isArchiveVersionHigher = this.IsArchiveVersionHigher(worldInfo);
				btnWarning.gameObject.SetActive(hasDataDifference || isArchiveVersionHigher);
				btnWarning.interactable = hasDataDifference;
				btnWarning.GetComponent<PointerTrigger>().enabled = btnWarning.interactable;
				btnWarning.GetComponent<DisableStyleRoot>().SetStyleEffect(!btnWarning.interactable, false);
				btnWarning.ClearAndAddListener(delegate
				{
					ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("ModList", worldInfo.ModIds).SetObject("DLCList", worldInfo.DlcIds);
					UIElement.RecordContent.SetOnInitArgs(args);
					UIManager.Instance.ShowUI(UIElement.RecordContent, true);
				});
				UI_RecordSelect.RefreshWarningTips(btnWarning, hasDataDifference, isArchiveVersionHigher, worldInfo);
			}
			else
			{
				bool flag9 = archiveInfo == null;
				if (flag9)
				{
					throw new Exception("null archiveInfo when archiveStatus is NotEmpty");
				}
				refers.CGet<GameObject>("RecordBroken").SetActive(true);
				refers.CGet<Game.Components.Avatar.Avatar>("Avatar").gameObject.SetActive(false);
				refers.CGet<CImage>("Click").SetSprite("newgame_kuang_0_1", false, null);
				refers.CGet<Renamer>("Renamer").Refresh("", 6, false, null);
				refers.CGet<GameObject>("LocationBar").SetActive(false);
				refers.CGet<TooltipInvoker>("SettingsTips").gameObject.SetActive(false);
				refers.CGet<TextMeshProUGUI>("GameTime").text = string.Empty;
				refers.CGet<TextMeshProUGUI>("AgeSamsara").text = string.Empty;
				continueBtn.gameObject.SetActive(true);
				continueBtn.interactable = false;
				continueBtn.GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
				List<ValueTuple<long, WorldInfo>> backupWorldsInfo = archiveInfo.BackupWorldsInfo;
				bool flag10 = backupWorldsInfo != null && backupWorldsInfo.Count > 0;
				if (flag10)
				{
					revertBtn.gameObject.SetActive(true);
					revertBtn.interactable = true;
					revertBtn.ClearAndAddListener(new Action(this.OnRevertBtnClicked));
					revertBtn.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
				}
				else
				{
					revertBtn.interactable = false;
					revertBtn.GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
				}
				deleteBtn.gameObject.SetActive(true);
				deleteBtn.interactable = true;
				deleteBtn.ClearAndAddListener(new Action(this.OnDeleteBtnClicked));
			}
		}
	}

	// Token: 0x06003776 RID: 14198 RVA: 0x001BE7DC File Offset: 0x001BC9DC
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

	// Token: 0x06003777 RID: 14199 RVA: 0x001BE9A4 File Offset: 0x001BCBA4
	private static void RefreshWarningTips(CButtonObsolete btnWarning, bool hasDataDifference, bool isArchiveVersionHigher, WorldInfo worldInfo)
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

	// Token: 0x06003778 RID: 14200 RVA: 0x001BEA80 File Offset: 0x001BCC80
	private ArchiveInfo GetArchiveInfo(sbyte index)
	{
		return GlobalOperations.ArchivesInfo.CheckIndex((int)index) ? GlobalOperations.ArchivesInfo[(int)index] : null;
	}

	// Token: 0x06003779 RID: 14201 RVA: 0x001BEAAC File Offset: 0x001BCCAC
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

	// Token: 0x0600377A RID: 14202 RVA: 0x001BEB18 File Offset: 0x001BCD18
	private string GetWorldSettingsDesc(WorldInfo worldInfo)
	{
		UI_RecordSelect.<>c__DisplayClass57_0 CS$<>8__locals1;
		CS$<>8__locals1.builder = EasyPool.Get<StringBuilder>();
		CS$<>8__locals1.builder.Clear();
		CS$<>8__locals1.builder.AppendLine("<SpName=mousetip_lingxing>" + LocalStringManager.Get(LanguageKey.UI_NewGame_WorldDetailSettings).SetColor("pinkyellow"));
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(1, (int)worldInfo.CombatDifficulty, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(11, (int)worldInfo.EnemyPracticeLevel, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(5, (int)worldInfo.HereticsAmountType, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(6, (int)worldInfo.BossInvasionSpeedType, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(7, (int)worldInfo.WorldResourceAmountType, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(12, (int)worldInfo.FavorabilityChange, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(14, (int)worldInfo.LootYield, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(2, (int)worldInfo.ReadingDifficulty, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(3, (int)worldInfo.BreakoutDifficulty, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(4, (int)worldInfo.LoopingDifficulty, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(13, (int)worldInfo.ProfessionUpgrade, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(8, (int)worldInfo.WorldPopulationType, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(0, (int)worldInfo.CharacterLifespanType, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(9, worldInfo.RestrictOptionsBehaviorType ? 0 : 1, ref CS$<>8__locals1);
		UI_RecordSelect.<GetWorldSettingsDesc>g__AppendItem|57_0(10, worldInfo.AllowRandomTaiwuHeir ? 0 : 1, ref CS$<>8__locals1);
		string res = CS$<>8__locals1.builder.ToString();
		EasyPool.Free<StringBuilder>(CS$<>8__locals1.builder);
		return res;
	}

	// Token: 0x0600377B RID: 14203 RVA: 0x001BEC78 File Offset: 0x001BCE78
	private void CheckEnterGame(Action onConfirm, long timeStamp = -1L)
	{
		UI_RecordSelect.<>c__DisplayClass58_0 CS$<>8__locals1 = new UI_RecordSelect.<>c__DisplayClass58_0();
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

	// Token: 0x0600377C RID: 14204 RVA: 0x001BEDD4 File Offset: 0x001BCFD4
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

	// Token: 0x0600377D RID: 14205 RVA: 0x001BEE90 File Offset: 0x001BD090
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

	// Token: 0x0600377E RID: 14206 RVA: 0x001BEF78 File Offset: 0x001BD178
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

	// Token: 0x0600377F RID: 14207 RVA: 0x001BF024 File Offset: 0x001BD224
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

	// Token: 0x06003780 RID: 14208 RVA: 0x001BF0D0 File Offset: 0x001BD2D0
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

	// Token: 0x06003781 RID: 14209 RVA: 0x001BF15C File Offset: 0x001BD35C
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

	// Token: 0x06003782 RID: 14210 RVA: 0x001BF250 File Offset: 0x001BD450
	private void OnScrollTabToggleChange(CToggleObsolete newTog, CToggleObsolete pewTog)
	{
		UI_RecordSelect._curScrollIndex = (sbyte)newTog.Key;
		this.OnScrollTabChange();
	}

	// Token: 0x06003783 RID: 14211 RVA: 0x001BF266 File Offset: 0x001BD466
	private void OnScrollTabChange()
	{
		base.CGet<Refers>("AvatarScrollTab").gameObject.SetActive(UI_RecordSelect._curScrollIndex == 1);
		base.CGet<Refers>("MapAreaAreaScrollTab").gameObject.SetActive(UI_RecordSelect._curScrollIndex == 2);
	}

	// Token: 0x06003784 RID: 14212 RVA: 0x001BF2A8 File Offset: 0x001BD4A8
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

	// Token: 0x06003785 RID: 14213 RVA: 0x001BF311 File Offset: 0x001BD511
	private void HideMask()
	{
		UIElement.FullScreenMask.Hide(false);
		this.StopMaskTimer();
	}

	// Token: 0x06003786 RID: 14214 RVA: 0x001BF327 File Offset: 0x001BD527
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

	// Token: 0x06003787 RID: 14215 RVA: 0x001BF338 File Offset: 0x001BD538
	private void StopMaskTimer()
	{
		bool flag = this._corMaskTimer == null;
		if (!flag)
		{
			base.StopCoroutine(this._corMaskTimer);
			this._corMaskTimer = null;
		}
	}

	// Token: 0x06003788 RID: 14216 RVA: 0x001BF369 File Offset: 0x001BD569
	private void StartMaskTimer()
	{
		this._corMaskTimer = base.StartCoroutine(this.CorMaskTimer());
	}

	// Token: 0x0600378B RID: 14219 RVA: 0x001BF4A4 File Offset: 0x001BD6A4
	[CompilerGenerated]
	private void <OnArchiveInfoLoaded>g__Refresh|26_0()
	{
		for (sbyte i = 0; i < 15; i += 1)
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
					goto IL_F7;
				}
				bool flag2 = GlobalOperations.ArchivesInfo[(int)j].Status == 1;
				if (flag2)
				{
					break;
				}
				j += 1;
			}
			index = j;
			IL_F7:;
		}
		bool flag3 = index >= 0;
		if (flag3)
		{
			this._currentPage = (int)(index / 5);
			this.RefreshPageDisplay();
			base.CGet<CToggleGroupObsolete>("RecordRoot").Set((int)index, true, false);
		}
		else
		{
			this._currentPage = 0;
			this.RefreshPageDisplay();
		}
		GameApp.ClockAndLogInfo("UI_RecordSelect:RefreshCharacters Complete", false);
	}

	// Token: 0x0600378D RID: 14221 RVA: 0x001BF660 File Offset: 0x001BD860
	[CompilerGenerated]
	internal static void <GetWorldSettingsDesc>g__AppendItem|57_0(byte worldCreationKey, int index, ref UI_RecordSelect.<>c__DisplayClass57_0 A_2)
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

	// Token: 0x0400280B RID: 10251
	private RecordInfo[] _panels;

	// Token: 0x0400280C RID: 10252
	public const sbyte RecodeCount = 15;

	// Token: 0x0400280D RID: 10253
	private const int SlotsPerPage = 5;

	// Token: 0x0400280E RID: 10254
	private const int TotalPages = 3;

	// Token: 0x0400280F RID: 10255
	private int _currentPage = 0;

	// Token: 0x04002810 RID: 10256
	[SerializeField]
	private CButton btnPageLeft;

	// Token: 0x04002811 RID: 10257
	[SerializeField]
	private CButton btnPageRight;

	// Token: 0x04002812 RID: 10258
	[SerializeField]
	private TextMeshProUGUI textPageLeft;

	// Token: 0x04002813 RID: 10259
	[SerializeField]
	private TextMeshProUGUI textPageRight;

	// Token: 0x04002814 RID: 10260
	private IEnumerator _performing;

	// Token: 0x04002815 RID: 10261
	private CButtonObsolete _startButton;

	// Token: 0x04002816 RID: 10262
	private HashSet<int> _emptySlots = new HashSet<int>();

	// Token: 0x04002817 RID: 10263
	private HashSet<int> _readySlots = new HashSet<int>();

	// Token: 0x04002818 RID: 10264
	private sbyte _currSlot = -1;

	// Token: 0x04002819 RID: 10265
	private DialogCmd _modCheckDialog = new DialogCmd
	{
		Title = LocalStringManager.Get("")
	};

	// Token: 0x0400281A RID: 10266
	private HashSet<string> enabledMods = new HashSet<string>();

	// Token: 0x0400281B RID: 10267
	private HashSet<string> removedMods = new HashSet<string>();

	// Token: 0x0400281C RID: 10268
	private readonly List<ModId> _modDependenciesChangedList = new List<ModId>();

	// Token: 0x0400281D RID: 10269
	private sbyte _modDependenciesChangedSlotIndex = -1;

	// Token: 0x0400281E RID: 10270
	private static sbyte _curScrollIndex;

	// Token: 0x0400281F RID: 10271
	private readonly List<ModId> _removedModList = new List<ModId>();

	// Token: 0x04002820 RID: 10272
	private readonly List<DlcId> _addedDlcList = new List<DlcId>();

	// Token: 0x04002821 RID: 10273
	private readonly List<DlcId> _removedDlcList = new List<DlcId>();

	// Token: 0x04002822 RID: 10274
	private Coroutine _corMaskTimer;
}
