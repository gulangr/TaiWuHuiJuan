using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Creation;
using GameData.Domains.Global;
using GameData.Domains.Global.Inscription;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.NewGame
{
	// Token: 0x0200081B RID: 2075
	public class ViewNewGame : UIBase
	{
		// Token: 0x17000C48 RID: 3144
		// (get) Token: 0x060065C7 RID: 26055 RVA: 0x002E7F83 File Offset: 0x002E6183
		public AvatarData CreationInfoAvatarData
		{
			get
			{
				return this._creationInfoAvatarData;
			}
		}

		// Token: 0x17000C49 RID: 3145
		// (get) Token: 0x060065C8 RID: 26056 RVA: 0x002E7F8B File Offset: 0x002E618B
		public bool UseInscription
		{
			get
			{
				return this._inscribedCharacter != null;
			}
		}

		// Token: 0x17000C4A RID: 3146
		// (get) Token: 0x060065C9 RID: 26057 RVA: 0x002E7F99 File Offset: 0x002E6199
		public sbyte Gender
		{
			get
			{
				return this.avatarPage.GetGender();
			}
		}

		// Token: 0x17000C4B RID: 3147
		// (get) Token: 0x060065CA RID: 26058 RVA: 0x002E7FA6 File Offset: 0x002E61A6
		public int BirthMonth
		{
			get
			{
				return (int)this.namePage.BirthMonth;
			}
		}

		// Token: 0x17000C4C RID: 3148
		// (get) Token: 0x060065CB RID: 26059 RVA: 0x002E7FB3 File Offset: 0x002E61B3
		public sbyte NeiliType
		{
			get
			{
				return this.namePage.NeiliType;
			}
		}

		// Token: 0x17000C4D RID: 3149
		// (get) Token: 0x060065CC RID: 26060 RVA: 0x002E7FC0 File Offset: 0x002E61C0
		public sbyte OrganizationTemplateId
		{
			get
			{
				sbyte id;
				return sbyte.TryParse(this.CreationInfoMap.GetValueOrDefault("TaiwuVillageStateTemplateId"), out id) ? id : 1;
			}
		}

		// Token: 0x17000C4E RID: 3150
		// (get) Token: 0x060065CD RID: 26061 RVA: 0x002E7FEA File Offset: 0x002E61EA
		// (set) Token: 0x060065CE RID: 26062 RVA: 0x002E7FF4 File Offset: 0x002E61F4
		public InscribedCharacter InscribedCharacter
		{
			get
			{
				return this._inscribedCharacter;
			}
			set
			{
				bool flag = this.InscribedCharacter == value;
				if (!flag)
				{
					NewGameSubPageName newGameSubPageName = this.namePage;
					this._inscribedCharacter = value;
					newGameSubPageName.SetInscriptionCharacter(value);
				}
			}
		}

		// Token: 0x17000C4F RID: 3151
		// (get) Token: 0x060065CF RID: 26063 RVA: 0x002E8027 File Offset: 0x002E6227
		public static sbyte DefaultDifficulty
		{
			get
			{
				return WorldCreationInfo.EDifficultyLevel.Level2.ToSbyte();
			}
		}

		// Token: 0x060065D0 RID: 26064 RVA: 0x002E8034 File Offset: 0x002E6234
		private void Awake()
		{
			this.startGame.onClick.ResetListener(new Action(this.TryStartNewGame));
			this.returnToMainMenu.onClick.ResetListener(new Action(this.ReturnToMainMenu));
			this.bottomToggleGroup.Init(-1);
			this.bottomToggleGroup.OnActiveIndexChange += delegate(int togNew, int togOld)
			{
				bool flag = togOld >= 0;
				if (flag)
				{
					this.childPages[togOld].gameObject.SetActive(false);
				}
				bool flag2 = togNew >= 0;
				if (flag2)
				{
					this.childPages[togNew].gameObject.SetActive(true);
				}
			};
			this.InitStartGameButtonHoverEffect();
		}

		// Token: 0x060065D1 RID: 26065 RVA: 0x002E80A8 File Offset: 0x002E62A8
		private void OnEnable()
		{
			DOTweenAnimation[] animations = this.moveIn.GetComponents<DOTweenAnimation>();
			for (int i = 0; i < animations.Length; i++)
			{
				animations[i].DORewind();
			}
			GEvent.Add(UiEvents.OnLoadingElementHide, new GEvent.Callback(this.OnLoadingElementHide));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
		}

		// Token: 0x060065D2 RID: 26066 RVA: 0x002E810F File Offset: 0x002E630F
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.OnLoadingElementHide, new GEvent.Callback(this.OnLoadingElementHide));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			AudioManager.Instance.ExitVideoMode();
		}

		// Token: 0x060065D3 RID: 26067 RVA: 0x002E8150 File Offset: 0x002E6350
		private void OnLoadingElementHide(ArgumentBox argsBox)
		{
			DOTweenAnimation[] animations = this.moveIn.GetComponents<DOTweenAnimation>();
			for (int i = 0; i < animations.Length; i++)
			{
				animations[i].DORestart();
			}
		}

		// Token: 0x060065D4 RID: 26068 RVA: 0x002E8188 File Offset: 0x002E6388
		private void InitStartGameButtonHoverEffect()
		{
			bool flag = this.startGamePointerTrigger == null;
			if (!flag)
			{
				PointerTrigger pointerTrigger = this.startGamePointerTrigger;
				if (pointerTrigger.EnterEvent == null)
				{
					pointerTrigger.EnterEvent = new UnityEvent();
				}
				pointerTrigger = this.startGamePointerTrigger;
				if (pointerTrigger.ExitEvent == null)
				{
					pointerTrigger.ExitEvent = new UnityEvent();
				}
				this.startGamePointerTrigger.EnterEvent.AddListener(delegate()
				{
					bool flag2 = !this.startGame.interactable;
					if (!flag2)
					{
						this.ChangeAmbiencePlaying(true);
						bool flag3 = this.startGameAurora != null && this.startGameAuroraAlive != null;
						if (flag3)
						{
							this.startGameAurora.sprite = this.startGameAuroraAlive;
							this.startGameAurora.rectTransform.DOKill(false);
							this.startGameAurora.rectTransform.DOSizeDelta(this.startGameAuroraAlive.rect.size, 0.3f, false).SetEase(Ease.OutExpo);
							this.startGameAurora.rectTransform.rotation = Quaternion.identity;
							this.startGameAurora.rectTransform.DOLocalRotate(new Vector3(0f, 0f, -360f), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
						}
						bool flag4 = this.startGameEffectAlive != null;
						if (flag4)
						{
							this.startGameEffectAlive.SetActive(true);
						}
						bool flag5 = this.startGameEffectDead != null;
						if (flag5)
						{
							this.startGameEffectDead.SetActive(false);
						}
						bool flag6 = this.startGameBlade != null;
						if (flag6)
						{
							this.startGameBlade.timeScale = 1f;
						}
					}
				});
				this.startGamePointerTrigger.ExitEvent.AddListener(delegate()
				{
					this.ChangeAmbiencePlaying(false);
					bool flag2 = this.startGameAurora != null && this.startGameAuroraDead != null;
					if (flag2)
					{
						this.startGameAurora.sprite = this.startGameAuroraDead;
						this.startGameAurora.rectTransform.DOKill(false);
						this.startGameAurora.rectTransform.DOSizeDelta(this.startGameAuroraDead.rect.size, 0.3f, false).SetEase(Ease.OutExpo);
					}
					bool flag3 = this.startGameEffectAlive != null;
					if (flag3)
					{
						this.startGameEffectAlive.SetActive(false);
					}
					bool flag4 = this.startGameEffectDead != null;
					if (flag4)
					{
						this.startGameEffectDead.SetActive(true);
					}
					bool flag5 = this.startGameBlade != null;
					if (flag5)
					{
						this.startGameBlade.timeScale = 0f;
					}
				});
				this.startGameBlade.timeScale = 0f;
			}
		}

		// Token: 0x060065D5 RID: 26069 RVA: 0x002E8228 File Offset: 0x002E6428
		private void ChangeAmbiencePlaying(bool playing)
		{
			if (playing)
			{
				AudioManager.Instance.PlayAmbience("Continue_unclick", 0.5f, 100);
			}
			else
			{
				AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 0.5f, 100);
			}
		}

		// Token: 0x17000C50 RID: 3152
		// (get) Token: 0x060065D6 RID: 26070 RVA: 0x002E826B File Offset: 0x002E646B
		// (set) Token: 0x060065D7 RID: 26071 RVA: 0x002E8278 File Offset: 0x002E6478
		public int CurrentPage
		{
			get
			{
				return this.bottomToggleGroup.GetActiveIndex();
			}
			set
			{
				this.bottomToggleGroup.Set(value, false);
			}
		}

		// Token: 0x060065D8 RID: 26072 RVA: 0x002E8288 File Offset: 0x002E6488
		private void ReturnToMainMenu()
		{
			UIElement dialog = UIElement.Dialog;
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			string key = "Cmd";
			DialogCmd dialogCmd = new DialogCmd();
			dialogCmd.Title = LanguageKey.LK_Return_To_Main_Menu.Tr();
			dialogCmd.Content = LanguageKey.LK_Discard_Character_Edit.Tr();
			dialogCmd.Yes = delegate()
			{
				GameApp.ReturnToMainMenu(null, null, null);
			};
			dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x060065D9 RID: 26073 RVA: 0x002E830C File Offset: 0x002E650C
		public void SetInteractable(bool interactable)
		{
			foreach (GameObject root in this.grayRootArray)
			{
				root.SetActive(!interactable);
			}
			foreach (CToggle toggle in this.grayToggleArray)
			{
				toggle.interactable = interactable;
			}
			if (interactable)
			{
				this.RefreshToggleLocks();
			}
		}

		// Token: 0x060065DA RID: 26074 RVA: 0x002E837C File Offset: 0x002E657C
		public void ContinueStartNewGame()
		{
			bool flag = !this._tryingStartNewGame;
			if (!flag)
			{
				this.TryStartNewGame();
			}
		}

		// Token: 0x060065DB RID: 26075 RVA: 0x002E83A0 File Offset: 0x002E65A0
		public void TryStartNewGame()
		{
			bool doingStartNewGame = this._doingStartNewGame;
			if (!doingStartNewGame)
			{
				foreach (NewGameSubPage child in this.childPages)
				{
					try
					{
						this._tryingStartNewGame = false;
						DialogCmd cmd;
						bool flag;
						if (!child.StartGameChecked)
						{
							cmd = child.StartGameCheck;
							flag = (cmd == null);
						}
						else
						{
							flag = true;
						}
						bool flag2 = flag;
						if (!flag2)
						{
							this._tryingStartNewGame = true;
							UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
							UIManager.Instance.MaskUI(UIElement.Dialog);
							return;
						}
					}
					catch (Exception e)
					{
						this._tryingStartNewGame = false;
						Debug.LogException(e);
					}
				}
				this._tryingStartNewGame = false;
				this.DoStartNewGame();
			}
		}

		// Token: 0x060065DC RID: 26076 RVA: 0x002E8470 File Offset: 0x002E6670
		public void ResetChecks()
		{
			this.BornAreaPlayingVideo = false;
			foreach (NewGameSubPage child in this.childPages)
			{
				child.StartGameChecked = false;
			}
		}

		// Token: 0x060065DD RID: 26077 RVA: 0x002E84A8 File Offset: 0x002E66A8
		private void DoStartNewGame()
		{
			bool doingStartNewGame = this._doingStartNewGame;
			if (!doingStartNewGame)
			{
				this._doingStartNewGame = true;
				GameApp.ClockAndLogInfo("UI_NewGame.DoStartNewGame begin", true);
				GlobalSettings globalSettings = SingletonObject.getInstance<GlobalSettings>();
				globalSettings.LastEnterWorldIndex = this._dataIndex;
				globalSettings.HaveDoneSave = true;
				ProtagonistCreationInfo protagonistCreationInfo = new ProtagonistCreationInfo
				{
					Avatar = new AvatarData(),
					TaiwuVillageStateTemplateId = 1
				};
				WorldCreationInfo worldCreationInfo = default(WorldCreationInfo);
				foreach (NewGameSubPage childPage in this.childPages)
				{
					try
					{
						childPage.DoStartGame(protagonistCreationInfo, ref worldCreationInfo);
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Format("DoStartNewGame exception: {0}\nat {1}", ex, new StackTrace(ex, true)));
						bool flag = childPage is NewGameSubPageBornArea;
						if (flag)
						{
							Debug.LogError("Error in NewGameSubPageBornArea found, set default value instead.");
							this.CreationInfoMap["TaiwuVillageStateTemplateId"] = "1";
							this.CreationInfoMap["TaiwuVillageLandFormType"] = "1";
						}
					}
				}
				CreationInfoHelper.Save(protagonistCreationInfo.Avatar, this.CreationInfoMap);
				GameApp.Instance.ChangeGameState(EGameState.Loading, EasyPool.Get<ArgumentBox>().SetObject("OnLoadingFinish", new Action(this.OnLoadFinish)).SetObject("OnLoadingStart", new Action(this.OnLoadStart)));
				WorldDomainMethod.Call.CreateWorld(worldCreationInfo, ViewNewGame.ChallengeModeIds);
				CharacterDomainMethod.Call.CreateProtagonist(this.Element.GameDataListenerId, protagonistCreationInfo);
				WorldDomainMethod.Call.RequestSetStat(59, 1);
				GlobalOperations.SaveWorld();
				GameApp.ClockAndLogInfo("UI_NewGame.DoStartNewGame end", false);
			}
		}

		// Token: 0x060065DE RID: 26078 RVA: 0x002E864C File Offset: 0x002E684C
		private void OnLoadStart()
		{
			GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 50).Set("AniTime", 50f));
		}

		// Token: 0x060065DF RID: 26079 RVA: 0x002E867B File Offset: 0x002E687B
		private void OnLoadFinish()
		{
			GameApp.ClockAndLogInfo("UI_NewGame.OnLoadFinish", false);
			GameApp.Instance.ChangeGameState(EGameState.InGame, null);
			TaiwuEventDomainMethod.Call.OnRecordEnterGame();
		}

		// Token: 0x060065E0 RID: 26080 RVA: 0x002E86A0 File Offset: 0x002E68A0
		public override void OnInit(ArgumentBox argsBox)
		{
			this.ResetChecks();
			this.NeedDataListenerId = true;
			this._doingStartNewGame = false;
			Dictionary<string, string> additionalInfo;
			AvatarData avatarData = CreationInfoHelper.Load(out additionalInfo);
			foreach (KeyValuePair<string, string> kvp in additionalInfo)
			{
				this.CreationInfoMap[kvp.Key] = kvp.Value;
			}
			string loadedSurname;
			bool flag = this.CreationInfoMap.TryGetValue("Surname", out loadedSurname);
			if (flag)
			{
				int maxSurnameLen = NameCenter.GetMaxSurnameLength();
				bool flag2 = loadedSurname.Length > maxSurnameLen;
				if (flag2)
				{
					this.CreationInfoMap["Surname"] = loadedSurname.Substring(0, maxSurnameLen);
				}
			}
			string loadedGivenName;
			bool flag3 = this.CreationInfoMap.TryGetValue("GivenName", out loadedGivenName);
			if (flag3)
			{
				int maxNameLen = NameCenter.GetMaxNameLength();
				bool flag4 = loadedGivenName.Length > maxNameLen;
				if (flag4)
				{
					this.CreationInfoMap["GivenName"] = loadedGivenName.Substring(0, maxNameLen);
				}
			}
			this._creationInfoAvatarData = avatarData;
			argsBox.Get("Index", out this._dataIndex);
			GlobalOperations.EnterNewWorld(this._dataIndex);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this._getGlobalFlagIndex = 0;
				GlobalDomainMethod.Call.GetGlobalFlag(this.Element.GameDataListenerId, 1);
				GlobalDomainMethod.Call.GetGlobalFlag(this.Element.GameDataListenerId, 0);
			}));
		}

		// Token: 0x060065E1 RID: 26081 RVA: 0x002E8808 File Offset: 0x002E6A08
		public override void InitMonitorFieldIds()
		{
			base.InitMonitorFieldIds();
			this.MonitorFields.Add(new UIBase.MonitorDataField(0, 10, ulong.MaxValue, null));
		}

		// Token: 0x060065E2 RID: 26082 RVA: 0x002E882C File Offset: 0x002E6A2C
		public void OnInitDone()
		{
			ViewNewGame.ChallengeModeIds.Clear();
			ViewNewGame.ChallengeModeInfo = null;
			this.RefreshRootEffectOn();
			sbyte difficulty = this.GetInitDifficulty();
			this.InitWorldCreationInfo(difficulty);
			int index = 0;
			foreach (NewGameSubPage child in this.childPages)
			{
				child.gameObject.SetActive(child is NewGameSubPageAvatar);
				child.Init();
				child.InitSetPageIndex(index++);
			}
			this.RefreshToggleLocks();
			this.RefreshNextStepButton();
			this.bottomToggleGroup.Set(0, false);
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x060065E3 RID: 26083 RVA: 0x002E88D4 File Offset: 0x002E6AD4
		private void RefreshToggleLocks()
		{
			int toggleCount = this.bottomToggleGroup.Count();
			for (int i = 0; i < toggleCount; i++)
			{
				CToggle toggle = this.bottomToggleGroup.Get(i);
				bool flag = toggle != null;
				if (flag)
				{
					NewGameToggleHelper helper = toggle.GetComponent<NewGameToggleHelper>();
					bool flag2 = helper != null;
					if (flag2)
					{
						helper.SetLocked(false);
					}
					else
					{
						toggle.interactable = true;
					}
				}
			}
		}

		// Token: 0x060065E4 RID: 26084 RVA: 0x002E8949 File Offset: 0x002E6B49
		private void RefreshNextStepButton()
		{
			this.nextStepButton.gameObject.SetActive(false);
			this.RefreshDisableEnterGameReason();
		}

		// Token: 0x060065E5 RID: 26085 RVA: 0x002E8965 File Offset: 0x002E6B65
		private void OnDestroy()
		{
			this.CreationInfoMap.Clear();
		}

		// Token: 0x060065E6 RID: 26086 RVA: 0x002E8974 File Offset: 0x002E6B74
		private void RefreshPresetTogglesVisibility()
		{
			foreach (NewGameSubPage child in this.childPages)
			{
				NewGameSubPageCustomPreset customPreset = child as NewGameSubPageCustomPreset;
				bool flag = customPreset != null;
				if (flag)
				{
					customPreset.RefreshPresetTogglesVisibility();
				}
			}
		}

		// Token: 0x060065E7 RID: 26087 RVA: 0x002E89B8 File Offset: 0x002E6BB8
		private void Update()
		{
			bool flag = UIElement.Loading.Exist || this.BornAreaPlayingVideo;
			if (!flag)
			{
				bool flag2 = UIManager.Instance.IsFocusElement(UIElement.NewGame) && (CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false));
				if (flag2)
				{
					bool flag3 = this.TryHandleFeatureSelectionEsc();
					if (!flag3)
					{
						bool flag4 = !UIManager.Instance.EscHandled;
						if (flag4)
						{
							this.ReturnToMainMenu();
						}
					}
				}
				else
				{
					bool flag5 = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this.startGame.interactable;
					if (flag5)
					{
						this.TryStartNewGame();
					}
				}
			}
		}

		// Token: 0x060065E8 RID: 26088 RVA: 0x002E8A88 File Offset: 0x002E6C88
		private bool TryHandleFeatureSelectionEsc()
		{
			for (int i = 0; i < this.childPages.Length; i++)
			{
				NewGameSubPageCustomPreset customPreset = this.childPages[i] as NewGameSubPageCustomPreset;
				bool flag = customPreset != null && customPreset.TryHandleFeatureSelectionEsc();
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060065E9 RID: 26089 RVA: 0x002E8AD8 File Offset: 0x002E6CD8
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 1;
				if (flag)
				{
					Notification notification2 = notification;
					Notification notification3 = notification2;
					ushort domainId = notification3.DomainId;
					if (domainId != 0)
					{
						if (domainId == 4)
						{
							ushort methodId = notification3.MethodId;
							if (methodId != 0)
							{
								if (methodId == 4)
								{
									this.namePage.OnGenerateRandomHanNameReturn(notification.ValueOffset, wrapper.DataPool);
								}
							}
							else
							{
								GameApp.ClockAndLogInfo("Protagonist created", false);
								GlobalOperations.OnWorldDataReady();
								this.Element.Destroy();
							}
						}
					}
					else
					{
						ushort methodId = notification3.MethodId;
						if (methodId == 12)
						{
							int getGlobalFlagIndex = this._getGlobalFlagIndex;
							this._getGlobalFlagIndex = getGlobalFlagIndex + 1;
							int num = getGlobalFlagIndex;
							int num2 = num;
							if (num2 != 0)
							{
								if (num2 == 1)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref ViewNewGame.PastEnding);
									this.OnInitDone();
								}
							}
							else
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref ViewNewGame.PastPerformArea);
							}
						}
					}
				}
				else
				{
					bool flag2 = notification.Type == 0 && notification.Uid.DomainId == 0 && notification.Uid.DataId == 10;
					if (flag2)
					{
						CustomProtagonistPreset preset = null;
						SerializerHolder<CustomProtagonistPreset>.Deserialize(wrapper.DataPool, notification.ValueOffset, ref preset);
						NewGameCustomPresetHelper.SetCustomProtagonistPreset(preset);
						this.RefreshPresetTogglesVisibility();
					}
				}
			}
		}

		// Token: 0x060065EA RID: 26090 RVA: 0x002E8C90 File Offset: 0x002E6E90
		public void RefreshDisableEnterGameReason()
		{
			StringBuilder sb = null;
			foreach (NewGameSubPage child in this.childPages)
			{
				bool flag = !string.IsNullOrEmpty(child.DisableEnterGameReason);
				if (flag)
				{
					StringBuilder stringBuilder;
					if ((stringBuilder = sb) == null)
					{
						stringBuilder = (sb = new StringBuilder());
					}
					stringBuilder.AppendLine(child.DisableEnterGameReason);
				}
			}
			bool flag2 = sb != null;
			if (flag2)
			{
				this.startGameTips.enabled = true;
				this.startGame.interactable = false;
				this.startGameTips.PresetParam[0] = sb.ToString();
				DisableStyleRoot[] ds = this.startGame.transform.GetComponentsInChildren<DisableStyleRoot>();
				for (int i = 0; i < ds.Length; i++)
				{
					ds[i].SetStyleEffect(true, false);
				}
			}
			else
			{
				this.startGameTips.enabled = false;
				this.startGame.interactable = true;
				this.startGameTips.PresetParam[0] = "";
				DisableStyleRoot[] ds2 = this.startGame.transform.GetComponentsInChildren<DisableStyleRoot>();
				for (int j = 0; j < ds2.Length; j++)
				{
					ds2[j].SetStyleEffect(false, false);
				}
			}
		}

		// Token: 0x060065EB RID: 26091 RVA: 0x002E8DC0 File Offset: 0x002E6FC0
		public T GetTargetSubPage<T>(ViewNewGame.ENewGameSubType subType) where T : NewGameSubPage
		{
			bool flag = subType >= (ViewNewGame.ENewGameSubType)this.childPages.Length;
			T result;
			if (flag)
			{
				Debug.LogError(string.Format("ChildPage is not enough ! | PageLen: {0} ", this.childPages.Length) + string.Format("Index: {0} SubType: {1}", (int)subType, subType));
				result = default(T);
			}
			else
			{
				result = (T)((object)this.childPages[(int)subType]);
			}
			return result;
		}

		// Token: 0x060065EC RID: 26092 RVA: 0x002E8E38 File Offset: 0x002E7038
		public CToggle GetTargetSubToggle(ViewNewGame.ENewGameSubType subType)
		{
			List<CToggle> toggleList = this.bottomToggleGroup.GetAll();
			bool flag = subType >= (ViewNewGame.ENewGameSubType)toggleList.Count;
			if (flag)
			{
				Debug.LogError(string.Format("ToggleList's Count is not enough ! | ToggleList.Count: {0} ", toggleList.Count) + string.Format("Index: {0} SubType: {1}", (int)subType, subType));
			}
			return this.bottomToggleGroup.Get((int)subType);
		}

		// Token: 0x060065ED RID: 26093 RVA: 0x002E8EAC File Offset: 0x002E70AC
		private void RefreshRootEffectOn()
		{
			this.rootEffectOn.SetActive(ViewNewGame.ChallengeModeIds.Count > 0);
		}

		// Token: 0x060065EE RID: 26094 RVA: 0x002E8EC7 File Offset: 0x002E70C7
		private void TopUiChanged(ArgumentBox _)
		{
			this.RefreshRootEffectOn();
		}

		// Token: 0x060065EF RID: 26095 RVA: 0x002E8ED0 File Offset: 0x002E70D0
		public void InitWorldCreationInfo(sbyte difficulty)
		{
			bool flag = (int)difficulty == WorldCreationInfo.EDifficultyLevel.Custom.ToInt();
			if (flag)
			{
				ViewNewGame.TempWorldCreationInfo = default(WorldCreationInfo);
				foreach (WorldCreationItem creationCfg in ((IEnumerable<WorldCreationItem>)WorldCreation.Instance))
				{
					string valueStr;
					byte value;
					bool flag2 = this.CreationInfoMap.TryGetValue(creationCfg.SaveFileKey, out valueStr) && byte.TryParse(valueStr, out value);
					if (flag2)
					{
						ViewNewGame.TempWorldCreationInfo.Set(creationCfg.TemplateId, value);
					}
				}
			}
			else
			{
				ViewNewGame.TempWorldCreationInfo = WorldCreationInfo.CreateByDifficultyPreset(difficulty);
			}
		}

		// Token: 0x060065F0 RID: 26096 RVA: 0x002E8F80 File Offset: 0x002E7180
		public sbyte GetInitDifficulty()
		{
			string difficultyStr;
			this.CreationInfoMap.TryGetValue("OverallDifficulty", out difficultyStr);
			sbyte difficulty;
			sbyte result = sbyte.TryParse(difficultyStr, out difficulty) ? difficulty : ViewNewGame.DefaultDifficulty;
			return Math.Clamp(result, WorldCreationInfo.EDifficultyLevel.Level1.ToSbyte(), WorldCreationInfo.EDifficultyLevel.Custom.ToSbyte());
		}

		// Token: 0x04004706 RID: 18182
		[SerializeField]
		private CButton startGame;

		// Token: 0x04004707 RID: 18183
		[SerializeField]
		private CButton returnToMainMenu;

		// Token: 0x04004708 RID: 18184
		[SerializeField]
		private CButton nextStepButton;

		// Token: 0x04004709 RID: 18185
		[SerializeField]
		private TooltipInvoker startGameTips;

		// Token: 0x0400470A RID: 18186
		[SerializeField]
		private GameObject rootEffectOn;

		// Token: 0x0400470B RID: 18187
		[Header("开始按钮悬停特效")]
		[SerializeField]
		private PointerTrigger startGamePointerTrigger;

		// Token: 0x0400470C RID: 18188
		[SerializeField]
		private CImage startGameAurora;

		// Token: 0x0400470D RID: 18189
		[SerializeField]
		private Sprite startGameAuroraAlive;

		// Token: 0x0400470E RID: 18190
		[SerializeField]
		private Sprite startGameAuroraDead;

		// Token: 0x0400470F RID: 18191
		[SerializeField]
		private GameObject startGameEffectAlive;

		// Token: 0x04004710 RID: 18192
		[SerializeField]
		private GameObject startGameEffectDead;

		// Token: 0x04004711 RID: 18193
		[SerializeField]
		private SkeletonGraphic startGameBlade;

		// Token: 0x04004712 RID: 18194
		[SerializeField]
		private CToggleGroup bottomToggleGroup;

		// Token: 0x04004713 RID: 18195
		[SerializeField]
		private NewGameSubPage[] childPages;

		// Token: 0x04004714 RID: 18196
		[SerializeField]
		private NewGameSubPageName namePage;

		// Token: 0x04004715 RID: 18197
		[SerializeField]
		private NewGameSubPageAvatar avatarPage;

		// Token: 0x04004716 RID: 18198
		[SerializeField]
		private DOTweenAnimation moveIn;

		// Token: 0x04004717 RID: 18199
		[SerializeField]
		private GameObject[] grayRootArray;

		// Token: 0x04004718 RID: 18200
		[SerializeField]
		private CToggle[] grayToggleArray;

		// Token: 0x04004719 RID: 18201
		[NonSerialized]
		public bool BornAreaPlayingVideo = false;

		// Token: 0x0400471A RID: 18202
		public Dictionary<string, string> CreationInfoMap = new Dictionary<string, string>();

		// Token: 0x0400471B RID: 18203
		private AvatarData _creationInfoAvatarData;

		// Token: 0x0400471C RID: 18204
		private InscribedCharacter _inscribedCharacter;

		// Token: 0x0400471D RID: 18205
		public static bool PastPerformArea;

		// Token: 0x0400471E RID: 18206
		public static bool PastEnding;

		// Token: 0x0400471F RID: 18207
		public static WorldCreationInfo TempWorldCreationInfo;

		// Token: 0x04004720 RID: 18208
		public static readonly List<int> ChallengeModeIds = new List<int>();

		// Token: 0x04004721 RID: 18209
		public static ChallengeModeInfo ChallengeModeInfo;

		// Token: 0x04004722 RID: 18210
		private int _getGlobalFlagIndex;

		// Token: 0x04004723 RID: 18211
		public const string CreationInfoMapKey = "OverallDifficulty";

		// Token: 0x04004724 RID: 18212
		private bool _tryingStartNewGame = false;

		// Token: 0x04004725 RID: 18213
		private bool _doingStartNewGame;

		// Token: 0x04004726 RID: 18214
		private sbyte _dataIndex;

		// Token: 0x04004727 RID: 18215
		public const int SubPageAvatar = 0;

		// Token: 0x04004728 RID: 18216
		public const int SubPageBornArea = 1;

		// Token: 0x04004729 RID: 18217
		public const int SubPageName = 2;

		// Token: 0x0400472A RID: 18218
		public const int SubPageCustomPreset = 3;

		// Token: 0x0400472B RID: 18219
		public const int SubPageFeature = 4;

		// Token: 0x0400472C RID: 18220
		public const int SubPageWorldDetail = 5;

		// Token: 0x0400472D RID: 18221
		public const int SubPageReserve = 6;

		// Token: 0x0400472E RID: 18222
		public const string SaveFileName = "CreationInfo.lua";

		// Token: 0x0400472F RID: 18223
		public const string TaiwuVillageStateTemplateId = "TaiwuVillageStateTemplateId";

		// Token: 0x04004730 RID: 18224
		public const string TaiwuVillageLandFormType = "TaiwuVillageLandFormType";

		// Token: 0x04004731 RID: 18225
		public const string Surname = "Surname";

		// Token: 0x04004732 RID: 18226
		public const string GivenName = "GivenName";

		// Token: 0x02001D4F RID: 7503
		public enum ENewGameSubType
		{
			// Token: 0x0400C5BD RID: 50621
			Avatar,
			// Token: 0x0400C5BE RID: 50622
			BornArea,
			// Token: 0x0400C5BF RID: 50623
			Name,
			// Token: 0x0400C5C0 RID: 50624
			CustomPreset,
			// Token: 0x0400C5C1 RID: 50625
			Feature,
			// Token: 0x0400C5C2 RID: 50626
			WorldDetail,
			// Token: 0x0400C5C3 RID: 50627
			Other
		}
	}
}
