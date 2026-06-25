using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Coffee.UIExtensions;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using SoftMasking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BA5 RID: 2981
	public class ViewCharacterMenuInfo : UI_CharacterMenuSubPageBase
	{
		// Token: 0x17001005 RID: 4101
		// (get) Token: 0x06009481 RID: 38017 RVA: 0x00452AA2 File Offset: 0x00450CA2
		protected bool IsTaiwu
		{
			get
			{
				return base.CharacterMenu.CurrentCharacterIsTaiwu;
			}
		}

		// Token: 0x17001006 RID: 4102
		// (get) Token: 0x06009482 RID: 38018 RVA: 0x00452AAF File Offset: 0x00450CAF
		private bool IsSpecialMember
		{
			get
			{
				return base.CharacterMenu.IsTaiwuSpecialTeammate(base.CharacterMenu.CurCharacterId);
			}
		}

		// Token: 0x17001007 RID: 4103
		// (get) Token: 0x06009483 RID: 38019 RVA: 0x00452AC7 File Offset: 0x00450CC7
		private bool IsBeastMember
		{
			get
			{
				return base.CharacterMenu.IsTaiwuBeastTeammate(base.CharacterMenu.CurCharacterId);
			}
		}

		// Token: 0x06009484 RID: 38020 RVA: 0x00452AE0 File Offset: 0x00450CE0
		public override void OnInit(ArgumentBox argsBox)
		{
			this.NeedDataListenerId = true;
			this.inscribeButton.interactable = false;
			this.talkButton.GetComponent<TooltipInvoker>().enabled = false;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this.localLoadingAnim.SetLoadingState(true);
				this.RequestAll();
			}));
			PointerTrigger interactionBtnPointTrigger = this.interactionButton.GetComponent<PointerTrigger>();
			PointerTrigger pointerTrigger = interactionBtnPointTrigger;
			if (pointerTrigger.EnterEvent == null)
			{
				pointerTrigger.EnterEvent = new UnityEvent();
			}
			pointerTrigger = interactionBtnPointTrigger;
			if (pointerTrigger.ExitEvent == null)
			{
				pointerTrigger.ExitEvent = new UnityEvent();
			}
			interactionBtnPointTrigger.EnterEvent.AddListener(delegate()
			{
				this.OnInteractionBtnEnter();
			});
			interactionBtnPointTrigger.ExitEvent.AddListener(delegate()
			{
				this.OnInteractionBtnExit();
			});
			PointerTrigger btnRootPointTrigger = this.interactiveButtonRoot.transform.GetChild(0).GetComponent<PointerTrigger>();
			pointerTrigger = btnRootPointTrigger;
			if (pointerTrigger.EnterEvent == null)
			{
				pointerTrigger.EnterEvent = new UnityEvent();
			}
			pointerTrigger = btnRootPointTrigger;
			if (pointerTrigger.ExitEvent == null)
			{
				pointerTrigger.ExitEvent = new UnityEvent();
			}
			btnRootPointTrigger.EnterEvent.AddListener(delegate()
			{
				this.OnBtnRootEnter();
			});
			btnRootPointTrigger.ExitEvent.AddListener(delegate()
			{
				this.OnBtnRootExit();
			});
		}

		// Token: 0x06009485 RID: 38021 RVA: 0x00452C12 File Offset: 0x00450E12
		private void OnBtnRootExit()
		{
			this._isEnterBtnRoot = false;
			this.interactiveButtonRoot.gameObject.SetActive(false);
		}

		// Token: 0x06009486 RID: 38022 RVA: 0x00452C2E File Offset: 0x00450E2E
		private void OnBtnRootEnter()
		{
			this._isEnterBtnRoot = true;
		}

		// Token: 0x06009487 RID: 38023 RVA: 0x00452C38 File Offset: 0x00450E38
		private void OnInteractionBtnExit()
		{
			bool flag = !base.gameObject.activeSelf;
			if (!flag)
			{
				base.StartCoroutine(this.DelayTryHideBtnRoot());
			}
		}

		// Token: 0x06009488 RID: 38024 RVA: 0x00452C67 File Offset: 0x00450E67
		private IEnumerator DelayTryHideBtnRoot()
		{
			yield return null;
			this.TryHideBtnRoot();
			yield break;
		}

		// Token: 0x06009489 RID: 38025 RVA: 0x00452C78 File Offset: 0x00450E78
		private void TryHideBtnRoot()
		{
			bool isEnterBtnRoot = this._isEnterBtnRoot;
			if (!isEnterBtnRoot)
			{
				this.interactiveButtonRoot.SetActive(false);
			}
		}

		// Token: 0x0600948A RID: 38026 RVA: 0x00452C9F File Offset: 0x00450E9F
		private void OnInteractionBtnEnter()
		{
			this.interactiveButtonRoot.SetActive(true);
		}

		// Token: 0x0600948B RID: 38027 RVA: 0x00452CB0 File Offset: 0x00450EB0
		private void RequestAll()
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0;
			if (!flag)
			{
				CharacterDomainMethod.Call.GetCharacterMenuInfoDisplayData(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			}
		}

		// Token: 0x0600948C RID: 38028 RVA: 0x00452CF0 File Offset: 0x00450EF0
		private void TryAddAndApplyOneWayRelation(ushort relationType)
		{
			CharacterDomainMethod.Call.TryAddAndApplyOneWayRelation(SingletonObject.getInstance<BasicGameData>().TaiwuCharId, base.CharacterMenu.CurCharacterId, relationType);
			bool flag = relationType == 16384;
			if (flag)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(247);
			}
			else
			{
				bool flag2 = relationType == 32768;
				if (flag2)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(246);
				}
			}
			ExtraDomainMethod.Call.AddTaiwuOneWayRelationCoolDown(base.CharacterMenu.CurCharacterId, relationType == 16384);
			LifeRecordsController instance = SingletonObject.getInstance<LifeRecordsController>();
			if (instance != null)
			{
				instance.RemoveLatestYearRecordCache();
			}
			GEvent.OnEvent(UiEvents.OnRelationButtonClick, null);
			this.RequestAll();
		}

		// Token: 0x0600948D RID: 38029 RVA: 0x00452D8C File Offset: 0x00450F8C
		private void TryRemoveOneWayRelation(ushort relationType)
		{
			CharacterDomainMethod.Call.TryRemoveOneWayRelation(SingletonObject.getInstance<BasicGameData>().TaiwuCharId, base.CharacterMenu.CurCharacterId, relationType);
			ExtraDomainMethod.Call.AddTaiwuOneWayRelationCoolDown(base.CharacterMenu.CurCharacterId, relationType == 16384);
			LifeRecordsController instance = SingletonObject.getInstance<LifeRecordsController>();
			if (instance != null)
			{
				instance.RemoveLatestYearRecordCache();
			}
			GEvent.OnEvent(UiEvents.OnRelationButtonClick, null);
			this.RequestAll();
		}

		// Token: 0x0600948E RID: 38030 RVA: 0x00452DF8 File Offset: 0x00450FF8
		private void RefreshAll()
		{
			this.RefreshInscribeButton();
			this.RefreshFollowButton();
			this.TryRefreshLoongDebuff();
			this.RefreshPersonalities();
			this.RefreshTalkButton();
			this.RefreshLoveAndHateButtonInfos();
			this.RefreshChangeCommandButtonActive();
			this.RefreshChangeCommandTipsActive();
			this.RefreshTeammateCommands();
			this.RefreshAvatar();
			this.RefreshKickOutButton();
			this.RefreshFeatureScroll();
			this.RefreshMedalSummary();
			this.RefreshChangeCommandButton();
			this.RefreshSamsaraCount();
			this.RefreshDetailInfo();
			this.RefreshAvatarDebug();
			this.RefreshLoveAndHate();
			this.RefreshGearMateEntryButton();
			this.InvokeCharacterInfoGuidingTriggers();
			this.RefreshAlertnessButton();
			this.RefreshInteractionButton();
			this.RefreshTaiwuOnlyContent();
			this.RefreshAiActionToolTip();
		}

		// Token: 0x0600948F RID: 38031 RVA: 0x00452EB0 File Offset: 0x004510B0
		private void RefreshAiActionToolTip()
		{
			bool flag = this.aiActionTooltip == null;
			if (!flag)
			{
				this.aiActionTooltip.gameObject.SetActive(!this.IsTaiwu && this._characterMenuInfoDisplayData.CharacterDisplayData.CreatingType == 1);
				this.aiActionTooltip.Type = TipType.AiAction;
				TooltipInvoker tooltipInvoker = this.aiActionTooltip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				this.aiActionTooltip.RuntimeParam.Set("charId", base.CharacterMenu.CurCharacterId);
			}
		}

		// Token: 0x06009490 RID: 38032 RVA: 0x00452F50 File Offset: 0x00451150
		private void RefreshGearMateEntryButton()
		{
			bool isGearMate = base.CharacterMenu.IsTaiwuGearMate(base.CharacterMenu.CurCharacterId);
			this.gearMateEntryButton.gameObject.SetActive(isGearMate);
			bool interactable = base.CharacterMenu.CanOperate;
			this.gearMateEntryButton.interactable = interactable;
			bool flag = !this.gearMateEntryButton.gameObject.activeSelf;
			if (!flag)
			{
				string resourcePath = Path.Combine("RemakeResources/UIGraphics5.0/Ui9CharacterMenu/", "ui9_btn_growth_{0}_{1}");
				this.LoadInteractiveButtonSprite(this.gearMateEntryButton, resourcePath);
			}
		}

		// Token: 0x06009491 RID: 38033 RVA: 0x00452FD8 File Offset: 0x004511D8
		private void RefreshTalkButton()
		{
			bool isTaiwu = this.IsTaiwu;
			this.talkButton.gameObject.SetActive(!isTaiwu);
			bool isTaiwu2 = this.IsTaiwu;
			if (!isTaiwu2)
			{
				bool isTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
				bool canInteractWithTeammate = SingletonObject.getInstance<EventModel>().CanCommunicateWithMateByEventModel();
				CharacterMenuFunctionControlItem config;
				this.talkButton.interactable = ((isTaiwuTeammate || this.IsSpecialMember) && canInteractWithTeammate && base.CharacterMenu.CanOperate && !base.CharacterMenu.OpenFromCombatPrepare && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Chat)));
				string resourcePath = Path.Combine("RemakeResources/UIGraphics5.0/Ui9CharacterMenu/", "ui9_btn_conversation_{0}_{1}");
				this.LoadInteractiveButtonSprite(this.talkButton, resourcePath);
			}
		}

		// Token: 0x06009492 RID: 38034 RVA: 0x004530AC File Offset: 0x004512AC
		private void RefreshAlertnessButton()
		{
			Selectable selectable = this.alertnessEntryButton;
			int alertness = this._characterMenuInfoDisplayData.Alertness;
			selectable.interactable = (alertness != int.MaxValue && alertness != int.MinValue);
			TooltipInvoker tip = this.alertnessEntryButton.gameObject.GetOrAddComponent<TooltipInvoker>();
			tip.enabled = true;
			tip.Type = TipType.SingleDesc;
			string content = this.alertnessEntryButton.interactable ? LanguageKey.LK_Alertness_Button_Tip.Tr() : LanguageKey.LK_Alertness_Button_Tip_Unknown.Tr();
			tip.PresetParam = new string[]
			{
				content
			};
		}

		// Token: 0x06009493 RID: 38035 RVA: 0x0045313C File Offset: 0x0045133C
		private void RefreshDetailInfo()
		{
			this.detailInfo.Set(this._characterMenuInfoDisplayData);
		}

		// Token: 0x06009494 RID: 38036 RVA: 0x00453154 File Offset: 0x00451354
		private void RefreshAvatarDebug()
		{
			AvatarRelatedData avatarRelatedData = this.GetDebugAvatarRelatedData();
			bool showDebugAvatar = avatarRelatedData != null;
			this.avatarDebugRoot.SetActive(showDebugAvatar);
			this.avatarDebugMask.enabled = !showDebugAvatar;
			bool flag = showDebugAvatar;
			if (flag)
			{
				this.avatarDebug.Refresh(avatarRelatedData);
			}
		}

		// Token: 0x06009495 RID: 38037 RVA: 0x004531A0 File Offset: 0x004513A0
		private void RefreshLoveAndHate()
		{
			bool flag = this.loveAndHate != null;
			if (flag)
			{
				this.loveAndHate.gameObject.SetActive(!this.IsTaiwu);
				bool flag2 = !this.IsTaiwu;
				if (flag2)
				{
					this.loveAndHate.Set(this._characterMenuInfoDisplayData.LoveAndHateItemInfo);
				}
			}
		}

		// Token: 0x06009496 RID: 38038 RVA: 0x00453200 File Offset: 0x00451400
		private void RefreshSamsaraCount()
		{
			short count = this._characterMenuInfoDisplayData.CharacterDisplayData.SamsaraCount;
			this.samsaraCountLabel.SetText(LanguageKey.LK_CharacterMenu_SamsaraCount.TrFormat(count), true);
			this.samsaraEntryButton.interactable = (count > 0);
		}

		// Token: 0x06009497 RID: 38039 RVA: 0x0045324C File Offset: 0x0045144C
		private void RefreshTeammateCommands()
		{
			bool isTaiwu = this.IsTaiwu;
			if (isTaiwu)
			{
				this.teammateCommands.ChangeCommandsActive(false);
			}
			else
			{
				CharacterDisplayData displayData = this._characterMenuInfoDisplayData.CharacterDisplayData;
				this.teammateCommands.Set(displayData.AttackMedal, displayData.DefenceMedal, displayData.WisdomMedal, this._characterMenuInfoDisplayData.TeammateCommandList, base.CharacterMenu.TeamReplacedTeammateCommands);
			}
		}

		// Token: 0x06009498 RID: 38040 RVA: 0x004532B4 File Offset: 0x004514B4
		private void RefreshChangeCommandButton()
		{
			List<sbyte> replacedCommands = base.CharacterMenu.TeamReplacedTeammateCommands;
			bool flag;
			if (replacedCommands != null)
			{
				flag = replacedCommands.Any((sbyte c) => c >= 0 && Config.TeammateCommand.Instance[c].Type == ETeammateCommandType.Negative);
			}
			else
			{
				flag = false;
			}
			bool hasNegativeCommand = flag;
			bool canEnterChangeCommandView = base.CharacterMenu.CanOperate && this.IsChangeCommandButtonVisible(hasNegativeCommand);
			this.changeCommandButton.interactable = canEnterChangeCommandView;
			TooltipInvoker tipDisplayer = this.changeCommandButton.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(canEnterChangeCommandView ? LanguageKey.LK_ChangeTeammateCommand_Entry_Tips : LanguageKey.LK_ChangeTeammateCommand_Entry_Tips_Disable));
		}

		// Token: 0x06009499 RID: 38041 RVA: 0x0045336C File Offset: 0x0045156C
		private void RefreshMedalSummary()
		{
			this.medalsSummary.Set(this._characterMenuInfoDisplayData.CharacterDisplayData);
		}

		// Token: 0x0600949A RID: 38042 RVA: 0x00453386 File Offset: 0x00451586
		private void RefreshFeatureScroll()
		{
			this.featureScroll.Set(this._characterMenuInfoDisplayData.CharacterDisplayData, LocalStringManager.CurLanguageType > LocalStringManager.LanguageType.CN, this.IsTaiwu, this._characterMenuInfoDisplayData.TemporaryFeatureLeftTimes);
		}

		// Token: 0x0600949B RID: 38043 RVA: 0x004533B9 File Offset: 0x004515B9
		private void RefreshAvatar()
		{
			this.characterCircle.Set(this.GetCharacterDisplayDataForAvatar(), this.IsTaiwu, true);
		}

		// Token: 0x0600949C RID: 38044 RVA: 0x004533D8 File Offset: 0x004515D8
		private void RefreshInteractionButton()
		{
			this.interactiveButtonRoot.gameObject.SetActive(false);
			bool isTaiwu = this.IsTaiwu;
			this.interactionButton.gameObject.SetActive(!isTaiwu);
			TooltipInvoker mouseTip = this.interactionButton.GetComponent<TooltipInvoker>();
			mouseTip.enabled = false;
			bool isTaiwu2 = this.IsTaiwu;
			if (!isTaiwu2)
			{
				bool isTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
				this.interactionButton.gameObject.SetActive((isTaiwuTeammate || this.IsSpecialMember) && base.CharacterMenu.CanOperate && !base.CharacterMenu.OpenFromCombatPrepare);
			}
		}

		// Token: 0x0600949D RID: 38045 RVA: 0x0045347C File Offset: 0x0045167C
		private void RefreshTaiwuOnlyContent()
		{
			bool isTaiwu = this.IsTaiwu;
			this.taiwuOnlyRoot.SetActive(isTaiwu);
			this.nonTaiwuRoot.SetActive(!isTaiwu);
		}

		// Token: 0x0600949E RID: 38046 RVA: 0x004534B0 File Offset: 0x004516B0
		private CharacterDisplayData GetCharacterDisplayDataForAvatar()
		{
			CharacterDisplayData displayData = this._characterMenuInfoDisplayData.CharacterDisplayData;
			AvatarRelatedData debugAvatarRelatedData = this.GetDebugAvatarRelatedData();
			bool flag = displayData == null || debugAvatarRelatedData == null;
			CharacterDisplayData result;
			if (flag)
			{
				result = displayData;
			}
			else
			{
				CharacterDisplayData copiedDisplayData = ViewCharacterMenuInfo.CloneCharacterDisplayData(displayData);
				copiedDisplayData.AvatarRelatedData = debugAvatarRelatedData;
				result = copiedDisplayData;
			}
			return result;
		}

		// Token: 0x0600949F RID: 38047 RVA: 0x004534FC File Offset: 0x004516FC
		private AvatarRelatedData GetDebugAvatarRelatedData()
		{
			CharacterDisplayData displayData = this._characterMenuInfoDisplayData.CharacterDisplayData;
			bool flag = !GMCheckAvatarState.Enabled || ((displayData != null) ? displayData.AvatarRelatedData : null) == null;
			AvatarRelatedData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				GMCheckAvatarState.UpdateCurrentCharacter(base.CharacterMenu.CurCharacterId, displayData.AvatarRelatedData);
				result = GMCheckAvatarState.GetCurrentAvatarRelatedDataCopy();
			}
			return result;
		}

		// Token: 0x060094A0 RID: 38048 RVA: 0x00453558 File Offset: 0x00451758
		private static CharacterDisplayData CloneCharacterDisplayData(CharacterDisplayData source)
		{
			bool flag = source == null;
			CharacterDisplayData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				CharacterDisplayData cloned = new CharacterDisplayData();
				for (int i = 0; i < ViewCharacterMenuInfo.CharacterDisplayDataFields.Length; i++)
				{
					FieldInfo field = ViewCharacterMenuInfo.CharacterDisplayDataFields[i];
					field.SetValue(cloned, field.GetValue(source));
				}
				cloned.AvatarRelatedData = ((source.AvatarRelatedData == null) ? null : new AvatarRelatedData(source.AvatarRelatedData));
				result = cloned;
			}
			return result;
		}

		// Token: 0x060094A1 RID: 38049 RVA: 0x004535D0 File Offset: 0x004517D0
		private void RefreshInscribeButton()
		{
			bool isTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu;
			TooltipInvoker mouseTip = this.inscribeButton.GetComponent<TooltipInvoker>();
			mouseTip.Type = TipType.Simple;
			mouseTip.enabled = false;
			sbyte inscriptionStatus = this._characterMenuInfoDisplayData.InscriptionStatus;
			bool flag = inscriptionStatus == 0;
			if (flag)
			{
				CharacterMenuFunctionControlItem config;
				this.inscribeButton.interactable = (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Inscribe));
				this.inscribeButton.gameObject.SetActive(!isTaiwu);
			}
			else
			{
				bool flag2 = inscriptionStatus < 0;
				if (flag2)
				{
					this.inscribeButton.gameObject.SetActive(false);
				}
				else
				{
					this.inscribeButton.interactable = false;
					this.inscribeButton.gameObject.SetActive(!isTaiwu);
					mouseTip.RuntimeParam = new ArgumentBox();
					mouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Save_Char_Tip_Title));
					StringBuilder tipsContent = new StringBuilder();
					for (sbyte i = 1; i < 7; i += 1)
					{
						bool flag3 = ((int)inscriptionStatus & 1 << (int)i) != 0;
						if (flag3)
						{
							switch (i)
							{
							case 1:
								tipsContent.AppendLine(LocalStringManager.Get(LanguageKey.LK_MouseTip_Inscribe_Inscribed));
								break;
							case 2:
								tipsContent.AppendLine(LocalStringManager.Get(LanguageKey.LK_MouseTip_Inscribe_MirrorCharacter));
								break;
							case 3:
								tipsContent.AppendLine(LocalStringManager.Get(LanguageKey.LK_MouseTip_Inscribe_Teammate));
								break;
							case 4:
								tipsContent.AppendLine(LocalStringManager.Get(LanguageKey.LK_MouseTip_Inscribe_Adult));
								break;
							case 5:
								tipsContent.AppendLine(LocalStringManager.Get(LanguageKey.LK_MouseTip_Inscribe_Favorability));
								break;
							case 6:
								tipsContent.AppendLine(LocalStringManager.Get(LanguageKey.LK_MouseTip_Inscribe_IsBeastCarrier));
								break;
							}
						}
					}
					mouseTip.RuntimeParam.Set("arg1", tipsContent.ToString());
					mouseTip.enabled = true;
					mouseTip.encyclopediaConfigTypeId = 25;
				}
			}
			bool flag4 = !this.inscribeButton.gameObject;
			if (!flag4)
			{
				string resourcePath = Path.Combine("RemakeResources/UIGraphics5.0/Ui9CharacterMenu/", "ui9_btn_mirro_{0}_{1}");
				this.LoadInteractiveButtonSprite(this.inscribeButton, resourcePath);
			}
		}

		// Token: 0x060094A2 RID: 38050 RVA: 0x00453818 File Offset: 0x00451A18
		private void LoadInteractiveButtonSprite(CButton btn, string path)
		{
			CImage btnImg = btn.GetComponent<CImage>();
			SpriteState spriteState = default(SpriteState);
			ResLoader.Load<Sprite>(string.Format(path, SingletonObject.getInstance<GlobalSettings>().Language.ToLower(), 0), delegate(Sprite normalSprite)
			{
				btnImg.sprite = normalSprite;
			}, null, false);
			Action<Sprite> <>9__3;
			Action<Sprite> <>9__2;
			ResLoader.Load<Sprite>(string.Format(path, SingletonObject.getInstance<GlobalSettings>().Language.ToLower(), 1), delegate(Sprite selectSprite)
			{
				spriteState.highlightedSprite = selectSprite;
				spriteState.selectedSprite = selectSprite;
				string assetPath = string.Format(path, SingletonObject.getInstance<GlobalSettings>().Language.ToLower(), 2);
				Action<Sprite> onLoad;
				if ((onLoad = <>9__2) == null)
				{
					onLoad = (<>9__2 = delegate(Sprite pressSprite)
					{
						spriteState.pressedSprite = pressSprite;
						string assetPath2 = string.Format(path, SingletonObject.getInstance<GlobalSettings>().Language.ToLower(), 3);
						Action<Sprite> onLoad2;
						if ((onLoad2 = <>9__3) == null)
						{
							onLoad2 = (<>9__3 = delegate(Sprite disableSprite)
							{
								spriteState.disabledSprite = disableSprite;
								btn.spriteState = spriteState;
							});
						}
						ResLoader.Load<Sprite>(assetPath2, onLoad2, null, false);
					});
				}
				ResLoader.Load<Sprite>(assetPath, onLoad, null, false);
			}, null, false);
		}

		// Token: 0x060094A3 RID: 38051 RVA: 0x004538C0 File Offset: 0x00451AC0
		private void TryRefreshLoongDebuff()
		{
			bool isDlcInstalled = SingletonObject.getInstance<DlcManager>().IsDlcInstalled(DlcManager.DlcIdFiveLoong);
			bool flag = !isDlcInstalled;
			if (!flag)
			{
				List<LoongInfo> loongInfos = this._characterMenuInfoDisplayData.FiveLoongLocation;
				bool flag2 = loongInfos == null;
				if (!flag2)
				{
					this.RefreshLoongDebuff();
				}
			}
		}

		// Token: 0x060094A4 RID: 38052 RVA: 0x00453908 File Offset: 0x00451B08
		private void RefreshLoongDebuff()
		{
			CharacterDisplayData displayData = this._characterMenuInfoDisplayData.CharacterDisplayData;
			CharacterItem characterConfig = Character.Instance[displayData.TemplateId];
			bool flag = characterConfig.CreatingType != 1;
			if (!flag)
			{
				List<LoongInfo> longExist = new List<LoongInfo>();
				List<short> longTemplatedIds = new List<short>();
				List<LoongInfo> loongInfos = this._characterMenuInfoDisplayData.FiveLoongLocation;
				foreach (LoongInfo loongInfo in loongInfos)
				{
					bool flag2 = loongInfo.IsDisappear || loongInfo.GetCharacterDebuffCount(displayData.CharacterId) <= 0;
					if (!flag2)
					{
						longExist.Add(loongInfo);
						longTemplatedIds.Add(loongInfo.CharacterTemplateId);
					}
				}
				bool flag3 = longExist.Count <= 0;
				if (flag3)
				{
					this.ClearLoongDebuffEffects();
				}
				else
				{
					this.ApplyLoongDebuffEffects(longTemplatedIds);
				}
			}
		}

		// Token: 0x060094A5 RID: 38053 RVA: 0x00453A08 File Offset: 0x00451C08
		private void ApplyLoongDebuffEffects(List<short> loongTemplatedIds)
		{
			ViewCharacterMenuInfo.<>c__DisplayClass49_0 CS$<>8__locals1 = new ViewCharacterMenuInfo.<>c__DisplayClass49_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = this.loongDebuffEffectRoot == null;
			if (!flag)
			{
				this.ClearLoongDebuffEffects();
				bool flag2 = loongTemplatedIds == null || loongTemplatedIds.Count <= 0;
				if (!flag2)
				{
					ViewCharacterMenuInfo.<>c__DisplayClass49_0 CS$<>8__locals2 = CS$<>8__locals1;
					int num = this._loongDebuffEffectRequestVersion + 1;
					this._loongDebuffEffectRequestVersion = num;
					CS$<>8__locals2.requestVersion = num;
					for (int i = 0; i < loongTemplatedIds.Count; i++)
					{
						string effectName;
						bool flag3 = !CommonUtils.FiveLongEffectMapping.TryGetValue(loongTemplatedIds[i], out effectName);
						if (!flag3)
						{
							string path = "RemakeResources/Particle/UIEffectPrefabs/CharacterMenu_wulong/" + effectName;
							string assetPath = path;
							Action<GameObject> onLoad;
							if ((onLoad = CS$<>8__locals1.<>9__0) == null)
							{
								onLoad = (CS$<>8__locals1.<>9__0 = delegate(GameObject prefab)
								{
									bool flag4 = CS$<>8__locals1.requestVersion != CS$<>8__locals1.<>4__this._loongDebuffEffectRequestVersion;
									if (!flag4)
									{
										bool flag5 = CS$<>8__locals1.<>4__this.loongDebuffEffectRoot == null;
										if (!flag5)
										{
											bool flag6 = prefab == null;
											if (!flag6)
											{
												GameObject effectInstance = Object.Instantiate<GameObject>(prefab, CS$<>8__locals1.<>4__this.loongDebuffEffectRoot.transform, false);
												effectInstance.transform.localScale = new Vector3(0.0069444445f, 0.0069444445f, 0.0069444445f);
												effectInstance.gameObject.SetActive(true);
												CS$<>8__locals1.<>4__this._loongDebuffEffectInstances.Add(effectInstance);
												CS$<>8__locals1.<>4__this.loongDebuffEffectRoot.RefreshParticles();
											}
										}
									}
								});
							}
							ResLoader.Load<GameObject>(assetPath, onLoad, null, false);
						}
					}
				}
			}
		}

		// Token: 0x060094A6 RID: 38054 RVA: 0x00453AE8 File Offset: 0x00451CE8
		private void ClearLoongDebuffEffects()
		{
			this._loongDebuffEffectRequestVersion++;
			for (int i = 0; i < this._loongDebuffEffectInstances.Count; i++)
			{
				bool flag = this._loongDebuffEffectInstances[i] != null;
				if (flag)
				{
					Object.Destroy(this._loongDebuffEffectInstances[i]);
				}
			}
			this._loongDebuffEffectInstances.Clear();
			bool flag2 = this.loongDebuffEffectRoot != null;
			if (flag2)
			{
				this.loongDebuffEffectRoot.RefreshParticles();
			}
		}

		// Token: 0x060094A7 RID: 38055 RVA: 0x00453B70 File Offset: 0x00451D70
		private void RefreshPersonalities()
		{
			this.personalities.Set(this._characterMenuInfoDisplayData.CharacterDisplayData);
			RectTransform rect = this.personalities.transform as RectTransform;
			rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -43.6f);
		}

		// Token: 0x060094A8 RID: 38056 RVA: 0x00453BC4 File Offset: 0x00451DC4
		private void RefreshFollowButton()
		{
			byte creatingType = this._characterMenuInfoDisplayData.CharacterDisplayData.CreatingType;
			bool isNonEvolutionaryType = CreatingType.IsNonEvolutionaryType(creatingType);
			this.followButton.gameObject.SetActive(!base.CharacterMenu.CurrentCharacterIsTaiwu && !isNonEvolutionaryType);
			GameObject selected = this.followButton.transform.Find("Selected").gameObject;
			GameObject gameObject = selected;
			CharacterDisplayData characterDisplayData = this._characterMenuInfoDisplayData.CharacterDisplayData;
			gameObject.SetActive(characterDisplayData != null && characterDisplayData.IsFollowedByTaiwu);
			bool interactable = !this._characterMenuInfoDisplayData.IsFollowingNpcListMax;
			this.followButton.interactable = interactable;
			TooltipInvoker tipDisplayer = this.followButton.GetComponent<TooltipInvoker>();
			tipDisplayer.Type = TipType.Simple;
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Following_Tips_Title));
			tipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(interactable ? LanguageKey.LK_Following_Tips_Content : LanguageKey.LK_Following_Tips_Content_Disable));
		}

		// Token: 0x060094A9 RID: 38057 RVA: 0x00453CDC File Offset: 0x00451EDC
		private void RefreshLoveAndHateButtonInfos()
		{
			int code = this._characterMenuInfoDisplayData.OneWayRelationResultCode;
			this._hasAdoredRelationship = ((8 & code) != 0);
			this._hasEnemyRelationship = ((16 & code) != 0);
			this._hasAdoredRelationshipNpc2Taiwu = ((64 & code) != 0);
			this.loveButton.GetComponent<Transform>().Find("Selected").gameObject.SetActive(this._hasAdoredRelationship);
			this.hateButton.GetComponent<Transform>().Find("Selected").gameObject.SetActive(this._hasEnemyRelationship);
			bool canAdore = true;
			bool canHate = true;
			TooltipInvoker loveMouseTip = this.loveButton.GetComponent<TooltipInvoker>();
			loveMouseTip.Type = TipType.GeneralLines;
			TooltipInvoker tooltipInvoker = loveMouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			loveMouseTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_RelationShip_Adored));
			int lineCount = 1;
			loveMouseTip.RuntimeParam.SetObject("LineData1", new GeneralLineData(3, new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_RelationShip_Adored_Tip).SetColor("grey")
			}, null));
			bool isTaiwuEvil = (code & 4096) != 0;
			bool isNpdEvil = (code & 16384) != 0;
			bool isTaiwuGood = (code & 2048) != 0;
			bool isNpcGood = (code & 8192) != 0;
			bool flag = !this._hasAdoredRelationship;
			if (flag)
			{
				bool flag2 = (code & 32) != 0;
				if (flag2)
				{
					GeneralLineData lineData = new GeneralLineData(2, new List<string>
					{
						string.Empty,
						LocalStringManager.Get(LanguageKey.LK_RelationShip_MainStoryLineProgress_Tip).SetColor("pinkyellow")
					}, null);
					loveMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), lineData);
					canAdore = false;
				}
				bool flag3 = (code & 1024) != 0;
				if (flag3)
				{
					GeneralLineData lineData2 = new GeneralLineData(2, new List<string>
					{
						string.Empty,
						LocalStringManager.Get(LanguageKey.LK_RelationShip_NotFriend_Tip).SetColor("pinkyellow")
					}, null);
					loveMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), lineData2);
					canAdore = false;
				}
				bool flag4 = (code & 1024) != 0 && (code & 64) != 0;
				if (flag4)
				{
					GeneralLineData lineData3 = new GeneralLineData(2, new List<string>
					{
						string.Empty,
						LocalStringManager.Get(LanguageKey.LK_RelationShip_DifferentLocation_Tip).SetColor("pinkyellow")
					}, null);
					loveMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), lineData3);
					canAdore = false;
				}
				bool flag5 = (code & 128) != 0;
				if (flag5)
				{
					GeneralLineData lineData4 = new GeneralLineData(2, new List<string>
					{
						string.Empty,
						LocalStringManager.Get(LanguageKey.LK_RelationShip_SelfNotAdult_Tip).SetColor("pinkyellow")
					}, null);
					loveMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), lineData4);
					canAdore = false;
				}
				bool flag6 = (code & 256) != 0;
				if (flag6)
				{
					GeneralLineData lineData5 = new GeneralLineData(2, new List<string>
					{
						string.Empty,
						LocalStringManager.Get(LanguageKey.LK_RelationShip_NotAdult_Tip).SetColor("pinkyellow")
					}, null);
					loveMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), lineData5);
					canAdore = false;
				}
				bool flag7 = (code & 512) != 0;
				if (flag7)
				{
					GeneralLineData lineData6 = new GeneralLineData(2, new List<string>
					{
						string.Empty,
						LocalStringManager.Get(LanguageKey.LK_RelationShip_CloseRelative_Tip).SetColor("pinkyellow")
					}, null);
					loveMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), lineData6);
					canAdore = false;
				}
				bool flag8 = (isTaiwuEvil && !isNpcGood) || (isNpdEvil && !isTaiwuGood);
				if (flag8)
				{
					GeneralLineData lineData7 = new GeneralLineData(2, new List<string>
					{
						string.Empty,
						LocalStringManager.Get(LanguageKey.LK_MythPower_BlockLoveTarget).SetColor("pinkyellow")
					}, null);
					loveMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), lineData7);
					canAdore = false;
				}
			}
			bool flag9 = (code & 2) != 0;
			if (flag9)
			{
				bool flag10 = lineCount > 1;
				if (flag10)
				{
					GeneralLineData emptyLineData = new GeneralLineData(4, null, null);
					loveMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), emptyLineData);
				}
				GeneralLineData lineData8 = new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.GetFormat(LanguageKey.LK_RelationShip_CoolDown, this._characterMenuInfoDisplayData.AdoredCoolDown).SetColor("pinkyellow")
				}, null);
				loveMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), lineData8);
				canAdore = false;
			}
			loveMouseTip.RuntimeParam.Set("LineCount", lineCount);
			loveMouseTip.RuntimeParam.Set("EncyclopediaLink", 96);
			TooltipInvoker hateMouseTip = this.hateButton.GetComponent<TooltipInvoker>();
			hateMouseTip.Type = TipType.GeneralLines;
			tooltipInvoker = hateMouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			hateMouseTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_RelationShip_Enemy));
			lineCount = 1;
			hateMouseTip.RuntimeParam.SetObject("LineData1", new GeneralLineData(3, new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_RelationShip_Enemy_Tip).SetColor("grey")
			}, null));
			bool flag11 = !this._hasEnemyRelationship && ((isNpcGood && !isTaiwuEvil) || (isTaiwuGood && !isNpdEvil));
			if (flag11)
			{
				GeneralLineData lineData9 = new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_MythPower_BlockHateTarget).SetColor("pinkyellow")
				}, null);
				hateMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), lineData9);
				canHate = false;
			}
			bool flag12 = (code & 4) != 0;
			if (flag12)
			{
				bool flag13 = lineCount > 1;
				if (flag13)
				{
					GeneralLineData emptyLineData2 = new GeneralLineData(4, null, null);
					hateMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), emptyLineData2);
				}
				GeneralLineData lineData10 = new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.GetFormat(LanguageKey.LK_RelationShip_CoolDown, this._characterMenuInfoDisplayData.EnemyCoolDown).SetColor("pinkyellow")
				}, null);
				hateMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), lineData10);
				canHate = false;
			}
			bool flag14 = (code & 32768) != 0;
			if (flag14)
			{
				bool flag15 = lineCount > 1;
				if (flag15)
				{
					GeneralLineData emptyLineData3 = new GeneralLineData(4, null, null);
					hateMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), emptyLineData3);
				}
				GeneralLineData lineData11 = new GeneralLineData(2, new List<string>
				{
					string.Empty,
					LocalStringManager.Get(LanguageKey.LK_HateButton_UnknownTarget).ColorReplace()
				}, null);
				hateMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), lineData11);
				canHate = false;
			}
			hateMouseTip.RuntimeParam.Set("LineCount", lineCount);
			hateMouseTip.RuntimeParam.Set("EncyclopediaLink", 97);
			this._canAdoredRelationship = canAdore;
			this._canEnemyRelationship = canHate;
			this.loveButton.interactable = this._canAdoredRelationship;
			this.hateButton.interactable = this._canEnemyRelationship;
			loveMouseTip.Refresh(false, -1);
			hateMouseTip.Refresh(false, -1);
			bool isTemporaryIntelligentCharacter = this._characterMenuInfoDisplayData.IsTemporaryIntelligentCharacter;
			byte creatingType = this._characterMenuInfoDisplayData.CharacterDisplayData.CreatingType;
			bool isNonEvolutionaryType = CreatingType.IsNonEvolutionaryType(creatingType);
			bool visible = !base.CharacterMenu.CurrentCharacterIsTaiwu && !isNonEvolutionaryType && !isTemporaryIntelligentCharacter;
			this.loveButton.gameObject.SetActive(visible);
			this.hateButton.gameObject.SetActive(visible);
		}

		// Token: 0x060094AA RID: 38058 RVA: 0x0045456C File Offset: 0x0045276C
		private void RefreshChangeCommandButtonActive()
		{
			List<sbyte> replacedCommands = base.CharacterMenu.TeamReplacedTeammateCommands;
			bool flag;
			if (replacedCommands != null)
			{
				flag = replacedCommands.Any((sbyte c) => c >= 0 && Config.TeammateCommand.Instance[c].Type == ETeammateCommandType.Negative);
			}
			else
			{
				flag = false;
			}
			bool hasNegativeCommand = flag;
			this.changeCommandButton.interactable = this.IsChangeCommandButtonVisible(hasNegativeCommand);
		}

		// Token: 0x060094AB RID: 38059 RVA: 0x004545C8 File Offset: 0x004527C8
		private void RefreshChangeCommandTipsActive()
		{
			List<int> changedTeammateCharIds = this._characterMenuInfoDisplayData.ChangedTeammateCharIds;
			bool isPinned = changedTeammateCharIds != null && changedTeammateCharIds.Contains(base.CharacterMenu.CurCharacterId);
			this.teammateCommandPinObject.SetActive(isPinned);
			bool flag = isPinned;
			if (flag)
			{
				this.RefreshChangeCommandTipsContent();
			}
		}

		// Token: 0x060094AC RID: 38060 RVA: 0x00454614 File Offset: 0x00452814
		private void RefreshChangeCommandTipsContent()
		{
			TooltipInvoker tipDisplayer = this.teammateCommandPinObject.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			bool flag = base.CharacterMenu.IsTaiwuGearMate(base.CharacterMenu.CurCharacterId);
			if (flag)
			{
				string gearMateName = NameCenter.GetMonasticTitleOrDisplayName(this._characterMenuInfoDisplayData.CharacterDisplayData, false);
				tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.LK_Pinned_TeammateCommand_Tips_GearMate, gearMateName));
			}
			else
			{
				tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Pinned_TeammateCommand_Tips));
			}
		}

		// Token: 0x060094AD RID: 38061 RVA: 0x004546B4 File Offset: 0x004528B4
		private void RefreshKickOutButton()
		{
			this.kickOutButton.gameObject.SetActive(!this.IsTaiwu && !this.IsBeastMember);
			this.kickOutBeastButton.gameObject.SetActive(!this.IsTaiwu && this.IsBeastMember);
			bool isTaiwu = this.IsTaiwu;
			if (!isTaiwu)
			{
				bool isTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
				bool canInteractWithTeammate = SingletonObject.getInstance<EventModel>().CanCommunicateWithMateByEventModel();
				bool isLeaderOfTaiwuVillage = SingletonObject.getInstance<BuildingModel>().VillageManagementUnlocked || SingletonObject.getInstance<TaiwuCharacterModel>().HasInheritedTaiwu;
				CButton button = this.IsBeastMember ? this.kickOutBeastButton : this.kickOutButton;
				CharacterMenuFunctionControlItem config;
				button.interactable = (isLeaderOfTaiwuVillage && (isTaiwuTeammate || this.IsSpecialMember) && base.CharacterMenu.CanOperate && !base.CharacterMenu.OpenFromCombatPrepare && canInteractWithTeammate && AgeGroup.GetAgeGroup(this._characterMenuInfoDisplayData.CharacterDisplayData.PhysiologicalAge) != 0 && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Leave)));
				TooltipInvoker tipDisplayer = button.GetComponent<TooltipInvoker>();
				bool flag = this.IsBeastMember && button.interactable && WorldMapModel.Traveling;
				if (flag)
				{
					button.interactable = false;
					tipDisplayer.enabled = true;
					TooltipInvoker tooltipInvoker = tipDisplayer;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterMenuInfo_Kickout_BeastTeammate_Traveling));
				}
				else
				{
					tipDisplayer.enabled = false;
				}
				TextMeshProUGUI label = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
				label.text = LocalStringManager.Get(this.IsBeastMember ? LanguageKey.LK_CharacterMenu_Btn_Leave_BeastCarrier : LanguageKey.LK_CharacterMenu_Btn_Leave);
				string nameStr = this.IsBeastMember ? "ui9_btn_dismiss_beast_{0}_{1}" : "ui9_btn_dismiss_{0}_{1}";
				string resourcePath = Path.Combine("RemakeResources/UIGraphics5.0/Ui9CharacterMenu/", nameStr);
				this.LoadInteractiveButtonSprite(button, resourcePath);
			}
		}

		// Token: 0x060094AE RID: 38062 RVA: 0x004548BC File Offset: 0x00452ABC
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "TalkBtn" == btnName;
			if (flag)
			{
				this.OnTalkButtonClick();
			}
			else
			{
				bool flag2 = "KickOutBtn" == btnName;
				if (flag2)
				{
					this.OnKickOutButtonClick();
				}
				else
				{
					bool flag3 = "KickOutBeastBtn" == btnName;
					if (flag3)
					{
						this.OnKickOutButtonClick();
					}
					else
					{
						bool flag4 = "FollowButton" == btnName;
						if (flag4)
						{
							this.OnFollowButtonClick();
						}
						else
						{
							bool flag5 = "ChangeCommandButton" == btnName;
							if (flag5)
							{
								this.OnChangeCommandButtonClick();
							}
							else
							{
								bool flag6 = "InscribeBtn" == btnName;
								if (flag6)
								{
									this.OnInscribeButtonClick();
								}
								else
								{
									bool flag7 = "GearMateEntryBtn" == btnName;
									if (flag7)
									{
										this.OnGearMateEntryButtonClick();
									}
									else
									{
										bool flag8 = "LoveButton" == btnName;
										if (flag8)
										{
											this.OnLoveButtonClick();
										}
										else
										{
											bool flag9 = "HateButton" == btnName;
											if (flag9)
											{
												this.OnHateButtonClick();
											}
											else
											{
												bool flag10 = "SamsaraDetailButton" == btnName;
												if (flag10)
												{
													this.OnClickSamsaraDetailButton();
												}
												else
												{
													bool flag11 = "AlertnessButton" == btnName;
													if (flag11)
													{
														this.OnClickAlertnessButton();
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060094AF RID: 38063 RVA: 0x004549FC File Offset: 0x00452BFC
		private void OnClickAlertnessButton()
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("charId", base.CharacterMenu.CurCharacterId);
			UIElement.Alertness.SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UIElement.Alertness, true);
		}

		// Token: 0x060094B0 RID: 38064 RVA: 0x00454A44 File Offset: 0x00452C44
		private void OnClickSamsaraDetailButton()
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("charId", base.CharacterMenu.CurCharacterId);
			UIElement.Samsara.SetOnInitArgs(argumentBox);
			UIManager.Instance.MaskUI(UIElement.Samsara);
		}

		// Token: 0x060094B1 RID: 38065 RVA: 0x00454A8C File Offset: 0x00452C8C
		private void OnLoveButtonClick()
		{
			bool flag = !this._hasAdoredRelationship && this._canAdoredRelationship;
			if (flag)
			{
				this.TryAddAndApplyOneWayRelation(16384);
			}
			else
			{
				bool flag2 = this._hasAdoredRelationship && this._hasAdoredRelationshipNpc2Taiwu && this._canAdoredRelationship;
				if (flag2)
				{
					DialogCmd cmd = new DialogCmd
					{
						Title = LocalStringManager.Get(LanguageKey.LK_RelationShip_RemoveAdored_Title).ColorReplace(),
						Content = LocalStringManager.Get(LanguageKey.LK_RelationShip_RemoveAdored_Content).ColorReplace(),
						Yes = delegate()
						{
							this.TryRemoveOneWayRelation(16384);
						}
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
				else
				{
					this.TryRemoveOneWayRelation(16384);
				}
			}
		}

		// Token: 0x060094B2 RID: 38066 RVA: 0x00454B60 File Offset: 0x00452D60
		private void OnHateButtonClick()
		{
			bool flag = !this._hasEnemyRelationship && this._canEnemyRelationship;
			if (flag)
			{
				UIElement dialog = UIElement.Dialog;
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				string key = "Cmd";
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Type = 1;
				dialogCmd.Title = LanguageKey.LK_Common_Attention.Tr();
				dialogCmd.Content = LanguageKey.LK_CharacterMenuInfo_Add_Enemy_Tips.Tr();
				dialogCmd.Yes = delegate()
				{
					this.TryAddAndApplyOneWayRelation(32768);
				};
				dialogCmd.No = delegate()
				{
				};
				dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.TryRemoveOneWayRelation(32768);
			}
		}

		// Token: 0x060094B3 RID: 38067 RVA: 0x00454C28 File Offset: 0x00452E28
		private void OnGearMateEntryButtonClick()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", base.CharacterMenu.CurCharacterId);
			argBox.Set("TargetSubPageIndex", 0);
			UIElement.GearMate.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.GearMate, true);
		}

		// Token: 0x060094B4 RID: 38068 RVA: 0x00454C80 File Offset: 0x00452E80
		private void OnInscribeButtonClick()
		{
			DialogCmd dialog = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_CharacterMenu_Btn_Save),
				Content = LocalStringManager.Get(LanguageKey.LK_CharacterMenu_Btn_Save_Tips),
				Type = 1,
				Yes = delegate()
				{
					this.inscribeButton.interactable = false;
					GlobalDomainMethod.Call.InscribeCharacter(base.CharacterMenu.CurCharacterId);
					this.RequestAll();
				},
				No = null
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialog));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x060094B5 RID: 38069 RVA: 0x00454D00 File Offset: 0x00452F00
		private void OnChangeCommandButtonClick()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", base.CharacterMenu.CurCharacterId);
			UIElement.ChangeTeammateCommand.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.ChangeTeammateCommand);
		}

		// Token: 0x060094B6 RID: 38070 RVA: 0x00454D48 File Offset: 0x00452F48
		private void OnFollowButtonClick()
		{
			bool flag = this._characterMenuInfoDisplayData.CharacterDisplayData == null;
			if (!flag)
			{
				bool isFollowed = this._characterMenuInfoDisplayData.CharacterDisplayData.IsFollowedByTaiwu;
				bool flag2 = isFollowed;
				if (flag2)
				{
					TaiwuDomainMethod.Call.TaiwuUnfollowNpc(base.CharacterMenu.CurCharacterId);
				}
				else
				{
					TaiwuDomainMethod.Call.TaiwuFollowNpc(base.CharacterMenu.CurCharacterId);
				}
				this.RequestAll();
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", base.CharacterMenu.CurCharacterId);
				GEvent.OnEvent(UiEvents.RefreshFollowing, argBox);
				TaiwuDomainMethod.Call.GetIsFollowingNpcListMax(this.Element.GameDataListenerId);
			}
		}

		// Token: 0x060094B7 RID: 38071 RVA: 0x00454DF0 File Offset: 0x00452FF0
		private void OnKickOutButtonClick()
		{
			DialogCmd dialog = new DialogCmd
			{
				Title = LocalStringManager.Get(this.IsBeastMember ? LanguageKey.LK_CharacterMenu_Btn_Leave_BeastCarrier : LanguageKey.LK_CharacterMenu_Btn_Leave),
				Content = LocalStringManager.Get(this.IsBeastMember ? LanguageKey.LK_CharacterMenu_Btn_Leave_BeastCarrier_DialogContent : LanguageKey.LK_CharacterMenu_Btn_Leave_Tips),
				Type = 1,
				Yes = delegate()
				{
					int kickCharId = base.CharacterMenu.CurCharacterId;
					bool isBeast = this.IsBeastMember;
					UIManager.Instance.HideUI(UIElement.CharacterMenu);
					bool flag = isBeast;
					if (flag)
					{
						ExtraDomainMethod.Call.HunterSkill_AnimalCharacterToItem(kickCharId);
					}
					else
					{
						TaiwuEventDomainMethod.Call.OnLetTeammateLeaveGroup(kickCharId);
					}
				},
				No = null
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialog));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x060094B8 RID: 38072 RVA: 0x00454E8D File Offset: 0x0045308D
		private void OnTalkButtonClick()
		{
			TaiwuEventDomainMethod.Call.OnCharacterClicked(base.CharacterMenu.CurCharacterId);
			UIManager.Instance.HideUI(UIElement.CharacterMenu);
		}

		// Token: 0x060094B9 RID: 38073 RVA: 0x00454EB4 File Offset: 0x004530B4
		private void OnSomeoneTeammateCommandChanged(ArgumentBox argumentBox)
		{
			int characterId;
			argumentBox.Get("CharacterId", out characterId);
			bool flag = characterId == base.CharacterMenu.CurCharacterId && !base.CharacterMenu.CurrentCharacterIsTaiwu;
			if (flag)
			{
				this.RequestAll();
			}
		}

		// Token: 0x060094BA RID: 38074 RVA: 0x00454EFC File Offset: 0x004530FC
		private void OnSomeoneEquipChanged(ArgumentBox argumentBox)
		{
			int characterId;
			argumentBox.Get("CharacterId", out characterId);
			bool flag = characterId == base.CharacterMenu.CurCharacterId;
			if (flag)
			{
				this.RequestAll();
			}
		}

		// Token: 0x060094BB RID: 38075 RVA: 0x00454F34 File Offset: 0x00453134
		private void OnTaiwuFeatureDeleted(ArgumentBox argumentBox)
		{
			bool currentCharacterIsTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu;
			if (currentCharacterIsTaiwu)
			{
				this.RequestAll();
			}
		}

		// Token: 0x060094BC RID: 38076 RVA: 0x00454F58 File Offset: 0x00453158
		private void OnLanguageChange(ArgumentBox argumentBox)
		{
			this.OnLanguageChange(LocalStringManager.CurLanguageType);
		}

		// Token: 0x060094BD RID: 38077 RVA: 0x00454F68 File Offset: 0x00453168
		protected override void OnNotifyGameDataFiltered(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 4 && notification.MethodId == 195;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._characterMenuInfoDisplayData);
						this.RefreshAll();
						this.localLoadingAnim.SetLoadingState(false);
						this.Element.ShowAfterRefresh();
					}
				}
			}
		}

		// Token: 0x060094BE RID: 38078 RVA: 0x0045502C File Offset: 0x0045322C
		private bool IsChangeCommandButtonVisible(bool hasNegativeCommand)
		{
			CharacterMenuFunctionControlItem config;
			return !hasNegativeCommand && base.CharacterMenu.CurrentCharacterIsTaiwuTeammate && !base.CharacterMenu.IsTaiwuBeastTeammate(base.CharacterMenu.CurCharacterId) && !SingletonObject.getInstance<BuildingModel>().GetYuanshanThreeVitalsIdList().Contains(base.CharacterMenu.CurCharacterId) && !SingletonObject.getInstance<BuildingModel>().GetYuanshanOppositeThreeVitalsIdList().Contains(base.CharacterMenu.CurCharacterId) && (!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.Command));
		}

		// Token: 0x060094BF RID: 38079 RVA: 0x004550CC File Offset: 0x004532CC
		private void InvokeCharacterInfoGuidingTriggers()
		{
			CharacterDisplayData displayData = this._characterMenuInfoDisplayData.CharacterDisplayData;
			bool flag = displayData == null;
			if (!flag)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(69);
				CharacterLoveAndHateItemInfo loveAndHateItemInfo = this._characterMenuInfoDisplayData.LoveAndHateItemInfo;
				bool flag2 = (loveAndHateItemInfo != null && loveAndHateItemInfo.LovingItemRevealed) || (loveAndHateItemInfo != null && loveAndHateItemInfo.HatingItemRevealed);
				if (flag2)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(68);
				}
				bool flag3 = ViewCharacterMenuInfo.HasReincarnationBonusFeature(displayData.FeatureIds);
				if (flag3)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(127);
				}
				bool flag4 = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate && displayData.PhysiologicalAge >= 16 && FavorabilityType.GetFavorabilityType(displayData.FavorabilityToTaiwu) == 6;
				if (flag4)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(132);
				}
				bool flag5 = this.IsTaiwu && displayData.SamsaraCount >= 1;
				if (flag5)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(243);
				}
			}
		}

		// Token: 0x060094C0 RID: 38080 RVA: 0x004551AC File Offset: 0x004533AC
		private static bool HasReincarnationBonusFeature(List<short> featureIds)
		{
			bool flag = featureIds == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				result = featureIds.Exists((short featureId) => featureId >= 232 && featureId <= 241);
			}
			return result;
		}

		// Token: 0x060094C1 RID: 38081 RVA: 0x004551F0 File Offset: 0x004533F0
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.CharacterBase && curSubPage == ECharacterSubPage.Character;
		}

		// Token: 0x17001008 RID: 4104
		// (get) Token: 0x060094C2 RID: 38082 RVA: 0x0045520C File Offset: 0x0045340C
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_CharacterMenu_Title_Info;
			}
		}

		// Token: 0x17001009 RID: 4105
		// (get) Token: 0x060094C3 RID: 38083 RVA: 0x00455213 File Offset: 0x00453413
		public override bool ShowBaseAttribute
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060094C4 RID: 38084 RVA: 0x00455218 File Offset: 0x00453418
		public override void OnSubpageVisible()
		{
			base.OnSubpageVisible();
			GEvent.Add(UiEvents.TeammateCommandChanged, new GEvent.Callback(this.OnSomeoneTeammateCommandChanged));
			bool ready = this.Element.Ready;
			if (ready)
			{
				this.RequestAll();
			}
		}

		// Token: 0x060094C5 RID: 38085 RVA: 0x0045525C File Offset: 0x0045345C
		public override void OnSubpageInVisible()
		{
			base.OnSubpageInVisible();
			GEvent.Remove(UiEvents.TeammateCommandChanged, new GEvent.Callback(this.OnSomeoneTeammateCommandChanged));
		}

		// Token: 0x060094C6 RID: 38086 RVA: 0x00455280 File Offset: 0x00453480
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0;
			if (!flag)
			{
				this.localLoadingAnim.SetLoadingState(true);
				bool ready = this.Element.Ready;
				if (ready)
				{
					this.RequestAll();
				}
			}
		}

		// Token: 0x060094C7 RID: 38087 RVA: 0x004552C8 File Offset: 0x004534C8
		private void Awake()
		{
			GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
			GEvent.Add(UiEvents.OnChangeCharacterClothing, new GEvent.Callback(this.OnSomeoneEquipChanged));
			GEvent.Add(UiEvents.OnTaiwuFeatureDeleted, new GEvent.Callback(this.OnTaiwuFeatureDeleted));
			GMCheckAvatarState.StateChanged += this.OnAvatarDebugStateChanged;
		}

		// Token: 0x060094C8 RID: 38088 RVA: 0x0045533C File Offset: 0x0045353C
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
			GEvent.Remove(UiEvents.OnChangeCharacterClothing, new GEvent.Callback(this.OnSomeoneEquipChanged));
			GEvent.Remove(UiEvents.OnTaiwuFeatureDeleted, new GEvent.Callback(this.OnTaiwuFeatureDeleted));
			GMCheckAvatarState.StateChanged -= this.OnAvatarDebugStateChanged;
			this.ClearLoongDebuffEffects();
		}

		// Token: 0x060094C9 RID: 38089 RVA: 0x004553B8 File Offset: 0x004535B8
		private new void OnDisable()
		{
			this.interactiveButtonRoot.gameObject.SetActive(false);
			this.interactionButton.gameObject.SetActive(false);
			this.talkButton.gameObject.SetActive(false);
			this.inscribeButton.gameObject.SetActive(false);
			this.gearMateEntryButton.gameObject.SetActive(false);
			this.kickOutButton.gameObject.SetActive(false);
			this.kickOutBeastButton.gameObject.SetActive(false);
			this.followButton.gameObject.SetActive(false);
			this.loveButton.gameObject.SetActive(false);
			this.hateButton.gameObject.SetActive(false);
			this.ClearLoongDebuffEffects();
			this.avatarDebugRoot.SetActive(false);
			this.avatarDebugMask.enabled = true;
		}

		// Token: 0x060094CA RID: 38090 RVA: 0x0045549C File Offset: 0x0045369C
		private void OnAvatarDebugStateChanged()
		{
			bool flag = !base.gameObject.activeInHierarchy;
			if (!flag)
			{
				bool flag2 = this._characterMenuInfoDisplayData.CharacterDisplayData == null;
				if (!flag2)
				{
					this.RefreshAvatar();
					this.RefreshAvatarDebug();
				}
			}
		}

		// Token: 0x04007251 RID: 29265
		private static readonly FieldInfo[] CharacterDisplayDataFields = typeof(CharacterDisplayData).GetFields(BindingFlags.Instance | BindingFlags.Public);

		// Token: 0x04007252 RID: 29266
		private CharacterMenuInfoDisplayData _characterMenuInfoDisplayData = new CharacterMenuInfoDisplayData();

		// Token: 0x04007253 RID: 29267
		private bool _hasAdoredRelationship;

		// Token: 0x04007254 RID: 29268
		private bool _hasEnemyRelationship;

		// Token: 0x04007255 RID: 29269
		private bool _canAdoredRelationship;

		// Token: 0x04007256 RID: 29270
		private bool _canEnemyRelationship;

		// Token: 0x04007257 RID: 29271
		private bool _hasAdoredRelationshipNpc2Taiwu;

		// Token: 0x04007258 RID: 29272
		private readonly List<GameObject> _loongDebuffEffectInstances = new List<GameObject>();

		// Token: 0x04007259 RID: 29273
		private int _loongDebuffEffectRequestVersion;

		// Token: 0x0400725A RID: 29274
		private bool _isEnterBtnRoot;

		// Token: 0x0400725B RID: 29275
		private const float PersonalitiesOffsetIsTaiwu = -43.6f;

		// Token: 0x0400725C RID: 29276
		private const float PersonalitiesOffsetNotTaiwu = 51f;

		// Token: 0x0400725D RID: 29277
		[SerializeField]
		private Game.Components.Character.Personalities personalities;

		// Token: 0x0400725E RID: 29278
		[SerializeField]
		private GameObject interactiveButtonRoot;

		// Token: 0x0400725F RID: 29279
		[SerializeField]
		private CButton interactionButton;

		// Token: 0x04007260 RID: 29280
		[SerializeField]
		private CButton talkButton;

		// Token: 0x04007261 RID: 29281
		[SerializeField]
		private CButton inscribeButton;

		// Token: 0x04007262 RID: 29282
		[SerializeField]
		private CButton gearMateEntryButton;

		// Token: 0x04007263 RID: 29283
		[SerializeField]
		private CButton kickOutButton;

		// Token: 0x04007264 RID: 29284
		[SerializeField]
		private CButton kickOutBeastButton;

		// Token: 0x04007265 RID: 29285
		[SerializeField]
		private CButton loveButton;

		// Token: 0x04007266 RID: 29286
		[SerializeField]
		private CButton hateButton;

		// Token: 0x04007267 RID: 29287
		[SerializeField]
		private CButton followButton;

		// Token: 0x04007268 RID: 29288
		[SerializeField]
		private CButton changeCommandButton;

		// Token: 0x04007269 RID: 29289
		[SerializeField]
		private CButton samsaraEntryButton;

		// Token: 0x0400726A RID: 29290
		[SerializeField]
		private CButton alertnessEntryButton;

		// Token: 0x0400726B RID: 29291
		[SerializeField]
		private CharacterCircle characterCircle;

		// Token: 0x0400726C RID: 29292
		[SerializeField]
		private UIParticle loongDebuffEffectRoot;

		// Token: 0x0400726D RID: 29293
		[SerializeField]
		private FeatureScroll featureScroll;

		// Token: 0x0400726E RID: 29294
		[SerializeField]
		private MedalSummary medalsSummary;

		// Token: 0x0400726F RID: 29295
		[SerializeField]
		private GameObject teammateCommandPinObject;

		// Token: 0x04007270 RID: 29296
		[SerializeField]
		private TeammateCommands teammateCommands;

		// Token: 0x04007271 RID: 29297
		[SerializeField]
		private TextMeshProUGUI samsaraCountLabel;

		// Token: 0x04007272 RID: 29298
		[SerializeField]
		private CharacterMenuInfoDetailInfo detailInfo;

		// Token: 0x04007273 RID: 29299
		[SerializeField]
		private LoveAndHate loveAndHate;

		// Token: 0x04007274 RID: 29300
		[SerializeField]
		private GameObject avatarDebugRoot;

		// Token: 0x04007275 RID: 29301
		[SerializeField]
		private Game.Components.Avatar.Avatar avatarDebug;

		// Token: 0x04007276 RID: 29302
		[SerializeField]
		private SoftMask avatarDebugMask;

		// Token: 0x04007277 RID: 29303
		[SerializeField]
		private GameObject taiwuOnlyRoot;

		// Token: 0x04007278 RID: 29304
		[SerializeField]
		private GameObject nonTaiwuRoot;

		// Token: 0x04007279 RID: 29305
		[SerializeField]
		private TooltipInvoker aiActionTooltip;

		// Token: 0x0400727A RID: 29306
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;
	}
}
