using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.Tools;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Combat;
using Game.Views.CharacterMenu;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Debate;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game.Views.Debate
{
	// Token: 0x02000AA7 RID: 2727
	public class ViewDebate : UIBase
	{
		// Token: 0x17000EBC RID: 3772
		// (get) Token: 0x060085A7 RID: 34215 RVA: 0x003E0EFA File Offset: 0x003DF0FA
		// (set) Token: 0x060085A8 RID: 34216 RVA: 0x003E0F01 File Offset: 0x003DF101
		public static DebateCardItem FocusingCardItem { get; set; }

		// Token: 0x17000EBD RID: 3773
		// (get) Token: 0x060085A9 RID: 34217 RVA: 0x003E0F09 File Offset: 0x003DF109
		private LifeSkillCombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<LifeSkillCombatModel>();
			}
		}

		// Token: 0x17000EBE RID: 3774
		// (get) Token: 0x060085AA RID: 34218 RVA: 0x003E0F10 File Offset: 0x003DF110
		private GlobalSettings SettingData
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>();
			}
		}

		// Token: 0x060085AB RID: 34219 RVA: 0x003E0F18 File Offset: 0x003DF118
		public override void OnInit(ArgumentBox argsBox)
		{
			this.StopAutoFight();
			this.Model.RemovingCards.Clear();
			this.Model.IsGameOver = false;
			this.Model.IsPlayingOperation = false;
			this.selfRecord.Clear();
			this.enemyRecord.Clear();
			this.selfRecord.SetIsTaiwu(true);
			this.enemyRecord.SetIsTaiwu(false);
			this.SetTimeScale(this.SettingData.DebateSpeed);
			this.ShowOperationMask(true);
			LifeSkillTypeItem config = LifeSkillType.Instance.GetItem(this.Model.LifeSkillType);
			Assert.IsNotNull<LifeSkillTypeItem>(config, string.Format("combat skill type invalid:{0}", this.Model.LifeSkillType));
			this.textLifeSkillTypeName.text = config.Name;
			this.selfPlayer.Init(true, this.Model.TaiwuCharData);
			this.enemyPlayer.Init(false, this.Model.EnemyCharData);
			this.selfPlayer.ScoreChanged = (this.enemyPlayer.ScoreChanged = delegate()
			{
				this._flagsNeedUpdate = true;
			});
			this.debateGrid.Init();
			this.selfPlayer.Speak(0, 4f);
			this.enemyPlayer.Speak(0, 4f);
			this.selfResetCardView.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
			this.enemyResetCardView.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
			this.unitGradeAreaMask.gameObject.SetActive(false);
			this.unitGradeAreaMask.ClearAndAddListener(delegate
			{
				this.HideCreateUnitArea(true);
			});
			this.unitGradeArea.Init();
			this.selectCardArea.gameObject.SetActive(false);
			this.roundEndEffect.Stop();
			this.roundEndEffect.gameObject.SetActive(true);
			this.removeWarning.SetActive(false);
			this.buttonRoundEnd.gameObject.SetActive(false);
			this.buttonRoundEnemy.gameObject.SetActive(false);
			this.buttonRoundResult.gameObject.SetActive(false);
			this.HideCreateUnitArea(true);
			this.UpdateAutoFightMark(false);
			this.ResetFlags();
			this._flagsNeedUpdate = true;
			this.NeedWaitData = true;
			List<int> selfAudienceList = (from d in this.Model.GetAudienceList(true)
			where d != null
			select d.CharacterId).ToList<int>();
			List<int> enemyAudienceList = (from d in this.Model.GetAudienceList(false)
			where d != null
			select d.CharacterId).ToList<int>();
			List<int> allAudienceList = selfAudienceList.Union(enemyAudienceList).ToList<int>();
			this._allAudienceList = allAudienceList;
			this.btnOpenCharMenu.gameObject.SetActive(this._allAudienceList != null && this._allAudienceList.Count > 0);
			TaiwuDomainMethod.AsyncCall.DebateGameInitialize(this, this.Model.LifeSkillType, this.Model.DebateGame.IsTaiwuFirst, this.Model.EnemyCharId, allAudienceList, delegate(int offset, RawDataPool dataPool)
			{
				DebateGame debateGame = null;
				Serializer.Deserialize(dataPool, offset, ref debateGame);
				this.Model.SetDebateGame(debateGame);
				this.RefreshRoundAndStage();
				this.RefreshData(false);
				this.Element.ShowAfterRefresh();
				this.FinishStage();
				this._flagsNeedUpdate = true;
			});
		}

		// Token: 0x060085AC RID: 34220 RVA: 0x003E12C0 File Offset: 0x003DF4C0
		private void Awake()
		{
			this.buttonSpeedDown.ClearAndAddListener(new Action(this.OnClickButtonSpeedDown));
			this.buttonSpeedUp.ClearAndAddListener(new Action(this.OnClickButtonSpeedUp));
			this.buttonAutoFight.ClearAndAddListener(new Action(this.OnClickButtonAutoFight));
			this.buttonRoundEnd.ClearAndAddListener(new Action(this.OnClickButtonRoundEnd));
			this.buttonForceGiveUp.ClearAndAddListener(new Action(this.OnClickButtonForceGiveUp));
			this.buttonGiveUp.ClearAndAddListener(new Action(this.OnClickButtonGiveUp));
			this.buttonResetStrategy.ClearAndAddListener(new Action(this.OnClickButtonResetStrategy));
			this.btnOpenCharMenu.ClearAndAddListener(new Action(this.OnClickBtnOpenCharMenu));
		}

		// Token: 0x060085AD RID: 34221 RVA: 0x003E1390 File Offset: 0x003DF590
		private void Update()
		{
			bool exist = UIElement.Encyclopedia.Exist;
			if (exist)
			{
				bool isAuto = this.Model.IsAuto;
				if (isAuto)
				{
					this.OnClickButtonAutoFight();
					this._encyclopediaCausedCloseAuto = true;
				}
				Time.timeScale = 0f;
			}
			else
			{
				bool encyclopediaCausedCloseAuto = this._encyclopediaCausedCloseAuto;
				if (encyclopediaCausedCloseAuto)
				{
					this.OnClickButtonAutoFight();
					this._encyclopediaCausedCloseAuto = false;
				}
				bool encyclopediaCausedPause = this._encyclopediaCausedPause;
				if (encyclopediaCausedPause)
				{
					Time.timeScale = this.Model.Speed;
					this._encyclopediaCausedPause = false;
				}
				bool flag = this._flagsNeedUpdate && this.FlagsCanUpdate();
				if (flag)
				{
					this.UpdateFlags();
					this._flagsNeedUpdate = false;
				}
				bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false);
				if (flag2)
				{
					bool activeSelf = this.operationMask.gameObject.activeSelf;
					if (!activeSelf)
					{
						bool activeSelf2 = this.unitGradeAreaMask.gameObject.activeSelf;
						if (activeSelf2)
						{
							this.HideCreateUnitArea(true);
							AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
						}
						else
						{
							bool isRemovingCards = this.Model.IsRemovingCards;
							if (isRemovingCards)
							{
								this.ExitSelectRemovingCardMode();
								AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
							}
							else
							{
								bool flag3 = ViewDebate.FocusingCardItem != null;
								if (flag3)
								{
									AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
									bool flag4 = this._unitSelectionStack.Count > 0;
									if (flag4)
									{
										this.RemoveRepeatUnitSelection();
										bool flag5 = this.Model.NeedSelectTarget(ViewDebate.FocusingCardItem.CardView.CardConfig);
										if (flag5)
										{
											this.CheckTarget();
										}
									}
									else
									{
										this.cardArea.UnselectCard(true);
									}
								}
							}
						}
					}
				}
				else
				{
					bool flag6 = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
					if (flag6)
					{
						bool flag7 = this.Model.IsAuto || this.Model.IsGameOver || this.Model.IsPlayingOperation;
						if (!flag7)
						{
							bool flag8 = !this.buttonRoundEnd.gameObject.activeSelf || !this.buttonRoundEnd.interactable;
							if (!flag8)
							{
								bool flag9 = ViewDebate.FocusingCardItem == null;
								if (flag9)
								{
									this.OnClickButtonRoundEnd();
								}
								else
								{
									this.cardArea.UnselectCard(true);
									this.RefreshCards();
								}
							}
						}
					}
					else
					{
						bool flag10 = CombatCommandKit.SpeedUp.Check(this.Element, false, false, false, false, false) && this.buttonSpeedUp.gameObject.activeSelf && this.buttonSpeedUp.interactable;
						if (flag10)
						{
							this.OnClickButtonSpeedUp();
						}
						else
						{
							bool flag11 = CombatCommandKit.SpeedDown.Check(this.Element, false, false, false, false, false) && this.buttonSpeedDown.gameObject.activeSelf && this.buttonSpeedDown.interactable;
							if (flag11)
							{
								this.OnClickButtonSpeedDown();
							}
						}
					}
				}
			}
		}

		// Token: 0x060085AE RID: 34222 RVA: 0x003E16BC File Offset: 0x003DF8BC
		private void OnEnable()
		{
			AudioManager.Instance.PlayMusic(ViewDebate.BGM, 1f, 100, null);
			AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1f, 100);
			GEvent.Add(UiEvents.OnLifeSkillCombatEnterCardFocusMode, new GEvent.Callback(this.EnterCardFocusMode));
			GEvent.Add(UiEvents.OnLifeSkillCombatExitCardFocusMode, new GEvent.Callback(this.ExitCardFocusMode));
			GEvent.Add(UiEvents.ShowCombatLifeSkillHiddenInfo, new GEvent.Callback(this.ShowCombatLifeSkillHiddenInfo));
			GEvent.Add(UiEvents.ShowCombatLifeSkillTalk, new GEvent.Callback(this.ShowCombatLifeSkillTalk));
			GEvent.Add(UiEvents.CombatLifeSkillClickUnit, new GEvent.Callback(this.CombatLifeSkillClickUnit));
			GEvent.Add(UiEvents.CombatLifeSkillHoverUnit, new GEvent.Callback(this.CombatLifeSkillHoverUnit));
			GEvent.Add(UiEvents.CombatLifeSkillClickBlock, new GEvent.Callback(this.CombatLifeSkillClickBlock));
			GEvent.Add(UiEvents.ChangeLifeSkillCombatData, new GEvent.Callback(this.ChangeLifeSkillCombatData));
			GEvent.Add(UiEvents.CombatLifeSkillHoverStrategy, new GEvent.Callback(this.CombatLifeSkillHoverStrategy));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
		}

		// Token: 0x060085AF RID: 34223 RVA: 0x003E1810 File Offset: 0x003DFA10
		private void OnDisable()
		{
			this.Model.IsGameOver = true;
			base.transform.DOKill(false);
			this.debateGrid.Clear();
			this.Model.ResetPauseAndTimeScale();
			this.RefreshHalfRoundEffect(false);
			GEvent.Remove(UiEvents.OnLifeSkillCombatEnterCardFocusMode, new GEvent.Callback(this.EnterCardFocusMode));
			GEvent.Remove(UiEvents.OnLifeSkillCombatExitCardFocusMode, new GEvent.Callback(this.ExitCardFocusMode));
			GEvent.Remove(UiEvents.ShowCombatLifeSkillHiddenInfo, new GEvent.Callback(this.ShowCombatLifeSkillHiddenInfo));
			GEvent.Remove(UiEvents.ShowCombatLifeSkillTalk, new GEvent.Callback(this.ShowCombatLifeSkillTalk));
			GEvent.Remove(UiEvents.CombatLifeSkillClickUnit, new GEvent.Callback(this.CombatLifeSkillClickUnit));
			GEvent.Remove(UiEvents.CombatLifeSkillHoverUnit, new GEvent.Callback(this.CombatLifeSkillHoverUnit));
			GEvent.Remove(UiEvents.CombatLifeSkillClickBlock, new GEvent.Callback(this.CombatLifeSkillClickBlock));
			GEvent.Remove(UiEvents.ChangeLifeSkillCombatData, new GEvent.Callback(this.ChangeLifeSkillCombatData));
			GEvent.Remove(UiEvents.CombatLifeSkillHoverStrategy, new GEvent.Callback(this.CombatLifeSkillHoverStrategy));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			SingletonObject.getInstance<GameSort>().SetCharacterSortConfig("UI_SelectChar", new CharacterSortFilterSetting());
			AudioManager.Instance.StopSound(ViewDebate.SoundHalfRound);
			DebateCardCameraManager.Instance.Hide();
			DebateUnitCameraManager.Instance.Hide();
		}

		// Token: 0x060085B0 RID: 34224 RVA: 0x003E19A8 File Offset: 0x003DFBA8
		private void TopUiChanged(ArgumentBox argumentBox)
		{
			bool needPause = UIElement.SystemOption.IsShowing || UIElement.SystemSetting.IsShowing || GameApp.Instance.GetCurrentGameStateName() == EGameState.Loading;
			this.Model.SetPause(needPause);
			bool isFocusElement = UIManager.Instance.IsFocusElement(this.Element);
			this.cardArea.OnTopUiChange(isFocusElement);
			bool flag = isFocusElement;
			if (flag)
			{
				this.SetTimeScale(this.SettingData.DebateSpeed);
			}
		}

		// Token: 0x060085B1 RID: 34225 RVA: 0x003E1A24 File Offset: 0x003DFC24
		private void RefreshRoundAndStage()
		{
			sbyte roundStage = this.Model.DebateGame.State;
			sbyte b = roundStage;
			sbyte b2 = b;
			string roundName;
			if (b2 - -1 > 2)
			{
				if (b2 != 2)
				{
					throw new ArgumentOutOfRangeException("roundStage", roundStage, null);
				}
				roundName = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage_Result);
			}
			else
			{
				bool isTaiwuRound = this.Model.IsTaiwuRound;
				if (isTaiwuRound)
				{
					bool playerCanMakeMove = this.Model.DebateGame.GetPlayerCanMakeMove(true);
					if (playerCanMakeMove)
					{
						roundName = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage_SelfStart);
					}
					else
					{
						roundName = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage_SelfEnd);
					}
				}
				else
				{
					roundName = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage_Enemy);
				}
			}
			bool isPlayerRound = roundStage == 0 || roundStage == 1;
			this.buttonRoundEnd.gameObject.SetActive(isPlayerRound && this.Model.IsTaiwuRound);
			this.buttonRoundEnemy.gameObject.SetActive(isPlayerRound && !this.Model.IsTaiwuRound);
			this.buttonRoundResult.gameObject.SetActive(roundStage == 2);
			this.textRoundName.text = roundName;
			this.imageNumberCurRound.Set(this.Model.DebateGame.Round);
			this.imageNumberMaxRound.Set(DebateConstants.MaxRound);
			this.RefreshHalfRoundEffect(true);
			this.RefreshStageTip();
		}

		// Token: 0x060085B2 RID: 34226 RVA: 0x003E1B7C File Offset: 0x003DFD7C
		private void RefreshStageTip()
		{
			this.stageTip.Type = TipType.Simple;
			string[] presetParam = this.stageTip.PresetParam;
			bool flag = presetParam == null || presetParam.Length != 2;
			if (flag)
			{
				this.stageTip.PresetParam = new string[2];
			}
			this.stageTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage);
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			stringBuilder.Clear();
			stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage_Tip));
			bool isHalfRound = this.Model.IsHalfRound;
			if (isHalfRound)
			{
				int remainRound = DebateConstants.MaxRound - this.Model.DebateGame.Round;
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_Stage_GameOver_Tip, remainRound).ColorReplace());
			}
			this.stageTip.PresetParam[1] = stringBuilder.ToString();
			stringBuilder.Clear();
			EasyPool.Free<StringBuilder>(stringBuilder);
		}

		// Token: 0x060085B3 RID: 34227 RVA: 0x003E1C6C File Offset: 0x003DFE6C
		private void RefreshHalfRoundEffect(bool show)
		{
			bool flag = !show;
			if (flag)
			{
				this.halfRoundEffect.alpha = 0f;
				this.halfRoundEffect.gameObject.SetActive(false);
			}
			else
			{
				bool flag2 = !this.halfRoundEffect.gameObject.activeSelf && this.Model.IsHalfRound;
				if (flag2)
				{
					this.halfRoundEffect.alpha = 0f;
					this.halfRoundEffect.gameObject.SetActive(true);
					this.halfRoundEffect.DOKill(false);
					this.halfRoundEffect.DOFade(1f, 2f);
					AudioManager.Instance.PlaySound(ViewDebate.SoundHalfRound, true, true);
				}
			}
		}

		// Token: 0x060085B4 RID: 34228 RVA: 0x003E1D28 File Offset: 0x003DFF28
		private void FinishStage()
		{
			this.SetButtonInteractable(this.buttonRoundEnd, false);
			bool isGameOver = this.Model.IsGameOver;
			if (!isGameOver)
			{
				this.ShowOperationMask(true);
				TaiwuDomainMethod.AsyncCall.DebateGameNextState(this, delegate(int offset, RawDataPool dataPool)
				{
					DebateGame debateGame = null;
					Serializer.Deserialize(dataPool, offset, ref debateGame);
					this.Model.SetDebateGame(debateGame);
					this.Model.RemovingCards.Clear();
					this.debateGrid.RefreshAllBlock();
					bool flag = !this.Model.IsTaiwuRound || this.Model.IsAuto;
					if (flag)
					{
						this.RefreshRoundAndStage();
						this.RefreshButtonInteractable();
					}
					float duration = (this.Model.IsTaiwuRound && !this.Model.IsAuto) ? 0f : this.PlayStageSwitchAnim();
					this.PlayOperation(duration, delegate
					{
						this.RefreshData(true);
						bool flag2 = !this.Model.IsTaiwuRound;
						if (flag2)
						{
							this.FinishStage();
						}
						else
						{
							bool isAuto = this.Model.IsAuto;
							if (isAuto)
							{
								this.AutoAction();
							}
							else
							{
								float duration2 = this.PlayStageSwitchAnim();
								DOVirtual.DelayedCall(duration2, delegate
								{
									this.RefreshRoundAndStage();
									this.RefreshButtonInteractable();
									this.ShowOperationMask(false);
									bool isAuto2 = this.Model.IsAuto;
									if (isAuto2)
									{
										this.AutoAction();
									}
								}, false);
							}
						}
					});
				});
			}
		}

		// Token: 0x060085B5 RID: 34229 RVA: 0x003E1D70 File Offset: 0x003DFF70
		private void AutoAction()
		{
			this.SetButtonInteractable(this.buttonRoundEnd, false);
			this.ShowOperationMask(true);
			TaiwuDomainMethod.AsyncCall.DebateGameSetTaiwuAi(null, true, delegate(int offset, RawDataPool pool)
			{
				DebateGame debateGame = null;
				Serializer.Deserialize(pool, offset, ref debateGame);
				bool flag = !this.Model.DebateGame.IsTaiwuAiProcessedInRound && debateGame.IsTaiwuAiProcessedInRound;
				if (flag)
				{
					this.Model.SetDebateGame(debateGame);
					this.PlayOperation(0f, delegate
					{
						this.SetButtonInteractable(this.buttonRoundEnd, false);
						this.RefreshData(true);
						bool isAuto2 = this.Model.IsAuto;
						if (isAuto2)
						{
							this.FinishStage();
						}
						else
						{
							this.ShowOperationMask(false);
						}
					});
				}
				else
				{
					this.SetButtonInteractable(this.buttonRoundEnd, false);
					bool flag2 = !this.Model.IsPlayingOperation;
					if (flag2)
					{
						this.ShowOperationMask(this.Model.IsAuto);
						bool isAuto = this.Model.IsAuto;
						if (isAuto)
						{
							this.FinishStage();
						}
					}
				}
			});
		}

		// Token: 0x060085B6 RID: 34230 RVA: 0x003E1DA0 File Offset: 0x003DFFA0
		private void GameOver(bool isTaiwuWin, bool isSurrender = false)
		{
			bool isGameOver = this.Model.IsGameOver;
			if (!isGameOver)
			{
				this.Model.IsGameOver = true;
				this.selfPlayer.Speak(this.GetPlaySpeakConfigOnGameOver(isTaiwuWin), 4f);
				this.enemyPlayer.Speak(this.GetPlaySpeakConfigOnGameOver(!isTaiwuWin), 4f);
				DOVirtual.DelayedCall(1f, delegate
				{
					TaiwuDomainMethod.AsyncCall.DebateGameOver(this, isTaiwuWin, isSurrender, delegate(int offset, RawDataPool dataPool)
					{
						DebateResult debateResult = null;
						Serializer.Deserialize(dataPool, offset, ref debateResult);
						ArgumentBox box = EasyPool.Get<ArgumentBox>();
						box.SetObject("DebateResult", debateResult);
						UIElement.DebateResult.SetOnInitArgs(box);
						UIManager.Instance.MaskUI(UIElement.DebateResult);
						Time.timeScale = 1f;
					});
				}, false);
			}
		}

		// Token: 0x060085B7 RID: 34231 RVA: 0x003E1E3C File Offset: 0x003E003C
		private float PlayStageSwitchAnim()
		{
			sbyte state = this.Model.DebateGame.State;
			bool flag = state == 0 || state == 1;
			float result;
			if (flag)
			{
				bool isTaiwuRound = this.Model.IsTaiwuRound;
				if (isTaiwuRound)
				{
					result = this.ShowTaiwuRound();
				}
				else
				{
					result = this.ShowEnemyRound();
				}
			}
			else
			{
				result = this.ShowResultRound();
			}
			return result;
		}

		// Token: 0x060085B8 RID: 34232 RVA: 0x003E1EA0 File Offset: 0x003E00A0
		private float ShowResultRound()
		{
			this.LogStepInfo("entering result round ...", true);
			this.roundTransitionSelf.SetActive(false);
			this.roundTransitionEnemy.SetActive(false);
			this.roundTransitionResult.SetActive(true);
			this.textRoundName.text = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stage_Result).SetColor("enemyround");
			CanvasGroup canvasGroup = this.roundTransitionGroup;
			canvasGroup.alpha = 0f;
			canvasGroup.gameObject.SetActive(true);
			DG.Tweening.Sequence sequence = DOTween.Sequence();
			sequence.Append(canvasGroup.DOFade(1f, 0.3f));
			sequence.AppendInterval(0.5f);
			sequence.Append(canvasGroup.DOFade(0f, 0.2f));
			sequence.AppendCallback(delegate
			{
				canvasGroup.gameObject.SetActive(false);
			});
			sequence.SetAutoKill(true);
			sequence.Play<DG.Tweening.Sequence>();
			AudioManager.Instance.PlaySound(ViewDebate.SoundStage3, false, true);
			return sequence.Duration(true);
		}

		// Token: 0x060085B9 RID: 34233 RVA: 0x003E1FC0 File Offset: 0x003E01C0
		private float ShowTaiwuRound()
		{
			this.LogStepInfo("entering taiwu round ...", true);
			this.roundTransitionSelf.SetActive(true);
			this.roundTransitionEnemy.SetActive(false);
			this.roundTransitionResult.SetActive(false);
			this.textRoundName.text = LocalStringManager.Get(LanguageKey.UI_LifeSkillCombat_RoundSelf).SetColor("enemyround");
			CanvasGroup canvasGroup = this.roundTransitionGroup;
			canvasGroup.alpha = 0f;
			canvasGroup.gameObject.SetActive(true);
			DG.Tweening.Sequence sequence = DOTween.Sequence();
			sequence.Append(canvasGroup.DOFade(1f, 0.3f));
			sequence.AppendInterval(0.5f);
			sequence.Append(canvasGroup.DOFade(0f, 0.2f));
			sequence.AppendCallback(delegate
			{
				canvasGroup.gameObject.SetActive(false);
			});
			sequence.SetAutoKill(true);
			sequence.Play<DG.Tweening.Sequence>();
			AudioManager.Instance.PlaySound(ViewDebate.SoundStage1, false, true);
			return sequence.Duration(true);
		}

		// Token: 0x060085BA RID: 34234 RVA: 0x003E20E0 File Offset: 0x003E02E0
		private float ShowEnemyRound()
		{
			this.LogStepInfo("entering enemy round ...", true);
			this.roundTransitionSelf.SetActive(false);
			this.roundTransitionEnemy.SetActive(true);
			this.roundTransitionResult.SetActive(false);
			this.textRoundName.text = LocalStringManager.Get(LanguageKey.UI_LifeSkillCombat_RoundEnemy).SetColor("taiwuround");
			CanvasGroup canvasGroup = this.roundTransitionGroup;
			canvasGroup.alpha = 0f;
			canvasGroup.gameObject.SetActive(true);
			DG.Tweening.Sequence sequence = DOTween.Sequence();
			sequence.Append(canvasGroup.DOFade(1f, 0.3f));
			sequence.AppendInterval(0.5f);
			sequence.Append(canvasGroup.DOFade(0f, 0.2f));
			sequence.AppendCallback(delegate
			{
				canvasGroup.gameObject.SetActive(false);
			});
			sequence.SetAutoKill(true);
			sequence.Play<DG.Tweening.Sequence>();
			AudioManager.Instance.PlaySound(ViewDebate.SoundStage2, false, true);
			return sequence.Duration(true);
		}

		// Token: 0x060085BB RID: 34235 RVA: 0x003E2200 File Offset: 0x003E0400
		private void CreateUnit(Vector2Int position, sbyte grade)
		{
			this.HideCreateUnitArea(true);
			this.ShowOperationMask(true);
			IntPair coordinate = new IntPair(position.x, position.y);
			TaiwuDomainMethod.AsyncCall.DebateGameMakeMove(this, coordinate, true, grade, true, delegate(int offset, RawDataPool dataPool)
			{
				DebateGame debateGame = null;
				Serializer.Deserialize(dataPool, offset, ref debateGame);
				this.Model.SetDebateGame(debateGame);
				this.debateGrid.RefreshAllBlock();
				this.PlayOperation(0f, delegate
				{
					this.RefreshData(true);
					this.RefreshRoundAndStage();
					this.ShowOperationMask(false);
				});
			});
		}

		// Token: 0x060085BC RID: 34236 RVA: 0x003E224A File Offset: 0x003E044A
		private void ClickUnit(DebateUnit unit)
		{
		}

		// Token: 0x060085BD RID: 34237 RVA: 0x003E2250 File Offset: 0x003E0450
		private void CombatLifeSkillClickUnit(ArgumentBox argumentBox)
		{
			DebateUnit unit;
			argumentBox.Get<DebateUnit>("Unit", out unit);
			this.ClickUnit(unit);
		}

		// Token: 0x060085BE RID: 34238 RVA: 0x003E2274 File Offset: 0x003E0474
		private void CombatLifeSkillHoverUnit(ArgumentBox argumentBox)
		{
			bool flag = !this.Model.IsTaiwuRound || this.Model.IsAuto || this.Model.IsGameOver;
			if (!flag)
			{
				bool flag2 = ViewDebate.FocusingCardItem == null;
				if (!flag2)
				{
					DebateUnit unit;
					argumentBox.Get<DebateUnit>("Unit", out unit);
					bool isEnter;
					argumentBox.Get("IsEnter", out isEnter);
					bool flag3 = !unit.Pawn.IsOwnedByTaiwu;
					if (!flag3)
					{
						bool isRecycleBases = ViewDebate.FocusingCardItem.CardConfig.EffectList.Exists((IntPair e) => e.First == 27);
						bool flag4 = isRecycleBases;
						if (flag4)
						{
							int curEnergy = isEnter ? (this.selfPlayer.Energy + this.Model.DebateGame.GetPawnBases(unit.Pawn.Id, -1, true, true)) : this.selfPlayer.Energy;
							this.selfPlayer.PreviewEnergy(curEnergy, false);
						}
					}
				}
			}
		}

		// Token: 0x060085BF RID: 34239 RVA: 0x003E2384 File Offset: 0x003E0584
		private void CombatLifeSkillHoverStrategy(ArgumentBox argumentBox)
		{
			bool flag = !this.Model.IsTaiwuRound || this.Model.IsAuto || this.Model.IsGameOver;
			if (!flag)
			{
				DebateCardView card = null;
				bool isEnter = true;
				if (argumentBox != null)
				{
					argumentBox.Get<DebateCardView>("Card", out card);
				}
				if (argumentBox != null)
				{
					argumentBox.Get("IsEnter", out isEnter);
				}
				DebateCardItem focusingCardItem = ViewDebate.FocusingCardItem;
				DebateCardView curCard = ((focusingCardItem != null) ? focusingCardItem.CardView : null) ?? card;
				int previewStrategyCount = isEnter ? (this.selfPlayer.StrategyCount - (int)curCard.CardConfig.UsedCost) : this.selfPlayer.StrategyCount;
				DebateCardItem focusingCardItem2 = ViewDebate.FocusingCardItem;
				bool flag2;
				if (focusingCardItem2 == null)
				{
					flag2 = false;
				}
				else
				{
					flag2 = focusingCardItem2.CardConfig.EffectList.Exists((IntPair e) => e.First == 30);
				}
				bool isRecycleStrategy = flag2;
				bool flag3 = isRecycleStrategy;
				if (flag3)
				{
					foreach (StrategyTarget strategyTarget in this._selectedStrategyTargetList)
					{
						foreach (ulong num in strategyTarget.List)
						{
							int index = (int)num;
							DebateCardItem cardItem = this.cardArea.CardList[index];
							previewStrategyCount += (int)cardItem.CardConfig.UsedCost;
						}
					}
				}
				this.selfPlayer.PreviewStrategyCount(previewStrategyCount);
			}
		}

		// Token: 0x060085C0 RID: 34240 RVA: 0x003E2530 File Offset: 0x003E0730
		private void ClickBlock(DebateBlock block)
		{
			this.ShowCreateUnitArea(block, null);
		}

		// Token: 0x060085C1 RID: 34241 RVA: 0x003E253C File Offset: 0x003E073C
		private void CombatLifeSkillClickBlock(ArgumentBox argumentBox)
		{
			DebateBlock block;
			argumentBox.Get<DebateBlock>("Block", out block);
			this.ClickBlock(block);
		}

		// Token: 0x060085C2 RID: 34242 RVA: 0x003E2560 File Offset: 0x003E0760
		private void ChangeLifeSkillCombatData(ArgumentBox argumentBox)
		{
			this.ShowOperationMask(true);
			DebateGame debateGame;
			argumentBox.Get<DebateGame>("DebateGame", out debateGame);
			this.Model.SetDebateGame(debateGame);
			this.PlayOperation(0f, delegate
			{
				this.RefreshData(true);
				this.ShowOperationMask(false);
			});
		}

		// Token: 0x060085C3 RID: 34243 RVA: 0x003E25AC File Offset: 0x003E07AC
		private void PlayOperation(float delay = 0f, Action onPlayEnd = null)
		{
			ViewDebate.<>c__DisplayClass129_0 CS$<>8__locals1 = new ViewDebate.<>c__DisplayClass129_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.onPlayEnd = onPlayEnd;
			bool isGameOver = this.Model.IsGameOver;
			if (!isGameOver)
			{
				bool flag = this.Model.DebateGame.IsGameOver && this.Model.DebateGame.Round >= DebateConstants.MaxRound;
				if (flag)
				{
					this.GameOver(this.Model.DebateGame.IsTaiwuWin, false);
				}
				else
				{
					bool flag2 = this.Model.DebateGame.DebateOperations == null;
					if (flag2)
					{
						this.Model.IsPlayingOperation = false;
						Action onPlayEnd2 = CS$<>8__locals1.onPlayEnd;
						if (onPlayEnd2 != null)
						{
							onPlayEnd2();
						}
					}
					else
					{
						this.Model.IsPlayingOperation = true;
						CS$<>8__locals1.sequence = DOTween.Sequence().SetTarget(base.transform);
						CS$<>8__locals1.sequence.AppendInterval(delay);
						for (int index = 0; index < this.Model.DebateGame.DebateOperations.Count; index++)
						{
							ViewDebate.<>c__DisplayClass129_1 CS$<>8__locals2 = new ViewDebate.<>c__DisplayClass129_1();
							CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
							CS$<>8__locals2.operation = this.Model.DebateGame.DebateOperations[index];
							float animTime = 0.2f;
							ViewDebate.<>c__DisplayClass129_2 CS$<>8__locals3 = new ViewDebate.<>c__DisplayClass129_2();
							CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
							switch (CS$<>8__locals3.CS$<>8__locals2.operation.OperationType)
							{
							case 0:
							{
								animTime = 1f;
								CS$<>8__locals3.selfPawn = this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
								CS$<>8__locals3.enemyPawn = this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.NpcPawnId];
								CS$<>8__locals3.selfUnit = this.debateGrid.FindUnit(CS$<>8__locals3.selfPawn.Id);
								CS$<>8__locals3.enemyUnit = this.debateGrid.FindUnit(CS$<>8__locals3.enemyPawn.Id);
								bool hasUnrevealedObject = CS$<>8__locals3.enemyUnit.HasUnrevealedObject();
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.ConflictUnit(CS$<>8__locals3.selfPawn, CS$<>8__locals3.enemyPawn, CS$<>8__locals3.CS$<>8__locals2.operation.Result, CS$<>8__locals3.CS$<>8__locals2.operation.Target);
								});
								float conflictEffectDelay = 0.25f;
								bool flag3 = hasUnrevealedObject;
								if (flag3)
								{
									conflictEffectDelay += 0.5f;
								}
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(conflictEffectDelay);
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									switch (CS$<>8__locals3.CS$<>8__locals2.operation.Result)
									{
									case 0:
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.effectPlayer.PlayEffectAt(CS$<>8__locals3.enemyUnit.ConflictEffectRoot, "eff_lifeskillcombat_ui_pen_lanqiang", 1f, false);
										break;
									case 1:
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.effectPlayer.PlayEffectAt(CS$<>8__locals3.selfUnit.ConflictEffectRoot, "eff_lifeskillcombat_ui_pen_hongqiang", 1f, true);
										break;
									case 2:
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.effectPlayer.PlayEffectAt(CS$<>8__locals3.selfUnit.ConflictDrawEffectRoot, "eff_lifeskillcombat_ui_pen_yiyangqiang", 1f, false);
										break;
									}
								});
								bool flag4 = CS$<>8__locals3.CS$<>8__locals2.operation.Result != 2;
								if (flag4)
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(0.5f);
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
									{
										bool isTaiwuWin = CS$<>8__locals3.CS$<>8__locals2.operation.Result == 0;
										DebateUnit loseUnit = isTaiwuWin ? CS$<>8__locals3.enemyUnit : CS$<>8__locals3.selfUnit;
										string destroyEffectName = isTaiwuWin ? "eff_lifeskillcombat_ui_pen_posui2" : "eff_lifeskillcombat_ui_pen_posui1";
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.effectPlayer.PlayEffectAt(loseUnit.DestroyEffectRoot, destroyEffectName, 1f, !isTaiwuWin);
										DOVirtual.DelayedCall(0.2f, delegate
										{
											loseUnit.RectTrans.localScale = Vector3.zero;
										}, false);
										bool flag23 = isTaiwuWin;
										if (flag23)
										{
											CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.Speak(13, 4f);
											CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.Speak(14, 4f);
										}
										else
										{
											CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.Speak(14, 4f);
											CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.Speak(13, 4f);
										}
									});
								}
								break;
							}
							case 1:
							{
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									Pawn pawn = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.MoveUnit(pawn, new Vector2Int(CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.Target.Second));
								});
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
								bool flag5 = CS$<>8__locals3.CS$<>8__locals2.operation.Result > 0;
								if (flag5)
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
									{
										DebateUnit unit = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.FindUnit(CS$<>8__locals3.CS$<>8__locals2.operation.PawnId);
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.PlayCreateUnitAnim(true, unit.RectTrans.position, unit.Pawn.IsOwnedByTaiwu);
									});
								}
								break;
							}
							case 2:
							{
								bool isTaiwu = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu;
								if (isTaiwu)
								{
									this.SetButtonInteractable(this.buttonForceGiveUp, false);
								}
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									Vector2Int position = new Vector2Int(CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.Target.Second);
									bool isFailed2 = CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed;
									if (isFailed2)
									{
										DebateBlock block = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.FindBlock(position);
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.PlayCreateUnitAnim(true, block.RectTrans.position, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
										DOVirtual.DelayedCall(0.2f, delegate
										{
											CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayMakeMoveFailedAnim(CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu, block.RectTrans.position);
										}, false);
									}
									else
									{
										Pawn pawn = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.CreateUnit(position, pawn, CS$<>8__locals3.CS$<>8__locals2.operation.Result);
										DebatePlayer player = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu ? CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer : CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer;
										player.Speak(12, 4f);
									}
								});
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
								bool isFailed = CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed;
								if (isFailed)
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(1f);
								}
								break;
							}
							case 3:
							{
								int result = CS$<>8__locals3.CS$<>8__locals2.operation.Result;
								bool flag6 = result == 2 || result == 3;
								if (flag6)
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
									{
										AudioManager.Instance.PlaySound(ViewDebate.SoundStrategyDeleteUnit, false, true);
										string effectName = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu ? "EffectStrategyDeleteTaiwuPawn" : "EffectStrategyDeleteEnemyPawn";
										DebateUnit unit = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.FindUnit(CS$<>8__locals3.CS$<>8__locals2.operation.PawnId);
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.effectPlayer.PlayEffectAt(unit.RectTrans, effectName, 0.5f, false);
									});
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(0.1f);
								}
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									Pawn pawn = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.DeleteUnit(pawn);
								});
								break;
							}
							case 4:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									Pawn pawn = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayReducePointAnimOnArrive(pawn, CS$<>8__locals3.CS$<>8__locals2.operation);
								});
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(1.5f);
								break;
							case 5:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									string targetCharName = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu ? CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.PlayerName : CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.PlayerName;
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.AddComment(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId, CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu, targetCharName);
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.AddComment(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId, CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu, targetCharName);
								});
								break;
							case 6:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									AudioManager.Instance.PlaySound(ViewDebate.SoundAddUnitStrategy, false, true);
									Pawn pawn = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
									DebateUnit unit = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.FindUnit(CS$<>8__locals3.CS$<>8__locals2.operation.PawnId);
									int strategyId = pawn.Strategies[CS$<>8__locals3.CS$<>8__locals2.operation.Result];
									ActivatedStrategy strategy = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.ActivatedStrategies[strategyId];
									unit.PlayAddStrategyAnim(CS$<>8__locals3.CS$<>8__locals2.operation.Result, strategy.IsCastedByTaiwu, strategy.IsCastedByTaiwu);
									bool isRevealed = strategy.IsRevealed;
									if (isRevealed)
									{
										unit.SetStrategyRevealed();
										unit.PlayRevealSound();
									}
								});
								break;
							case 8:
							{
								int num = index;
								IEnumerable<DebateOperation> source = from o in this.Model.DebateGame.DebateOperations
								where o.OperationType == 8
								select o;
								Func<DebateOperation, int> selector;
								if ((selector = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>9__14) == null)
								{
									selector = (CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>9__14 = ((DebateOperation o) => CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.DebateOperations.IndexOf(o)));
								}
								bool isLastCard = num == source.Max(selector);
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayCardChange(CS$<>8__locals3.CS$<>8__locals2.operation);
								});
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(isLastCard ? (0.5f + animTime) : 0.25f);
								break;
							}
							case 9:
							{
								bool needPlayTaiwuStressReduceScoreAnim = this.selfPlayer.Stress < CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuPressure && this.selfPlayer.Score > CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint;
								bool flag7 = needPlayTaiwuStressReduceScoreAnim;
								if (flag7)
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
									{
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.SetStress(CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuPressure, true);
									});
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(1f);
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
									{
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.PlayStressReduceScoreAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint, CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuPressure);
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.SetScore(CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint);
									});
								}
								bool needPlayEnemyStressReduceScoreAnim = this.enemyPlayer.Stress < CS$<>8__locals3.CS$<>8__locals2.operation.NpcPressure && this.enemyPlayer.Score > CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint;
								bool flag8 = needPlayEnemyStressReduceScoreAnim;
								if (flag8)
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
									{
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.SetStress(CS$<>8__locals3.CS$<>8__locals2.operation.NpcPressure, true);
									});
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(1f);
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
									{
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.PlayStressReduceScoreAnim(CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint, CS$<>8__locals3.CS$<>8__locals2.operation.NpcPressure);
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.SetScore(CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint);
									});
								}
								break;
							}
							case 10:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									Pawn pawn = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.Pawns[CS$<>8__locals3.CS$<>8__locals2.operation.PawnId];
									DebateUnit findUnit = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.FindUnit(pawn.Id);
									if (findUnit != null)
									{
										findUnit.RefreshPower(CS$<>8__locals3.CS$<>8__locals2.operation.Result, false);
									}
								});
								break;
							case 11:
							{
								bool hasExtraTriggeredAnim = DebateBlock.HasExtraTriggeredAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId);
								bool flag9 = hasExtraTriggeredAnim;
								if (flag9)
								{
									DebateNodeEffectItem effectConfig = DebateNodeEffect.Instance[CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId];
									int reduceScoreIndex = effectConfig.TriggerEffectList.FindIndex((IntPair e) => e.First == 43);
									bool needReduceScore = reduceScoreIndex >= 0;
									bool flag10 = needReduceScore;
									if (flag10)
									{
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
										{
											DebatePlayer player = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu ? CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer : CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer;
											int removeScore = effectConfig.TriggerEffectList[reduceScoreIndex].Second;
											for (int i = 0; i < removeScore; i++)
											{
												int oldScore = player.Score - i;
												RectTransform oldScoreTrans = player.GetScoreRectTrans(oldScore);
												CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayBlockEffectTriggeredExtraAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId, oldScoreTrans);
											}
										});
									}
								}
								else
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
									{
										bool flag23 = CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint > CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.Score;
										if (flag23)
										{
											CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.PlayScoreAddedAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
										}
										else
										{
											bool flag24 = CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint > CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.Score;
											if (flag24)
											{
												CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.PlayScoreAddedAnim(CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
											}
											else
											{
												bool flag25 = CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint < CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.Score;
												if (flag25)
												{
													CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.PlayScoreReducedAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuGamePoint, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
												}
												else
												{
													bool flag26 = CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint < CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.Score;
													if (flag26)
													{
														CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.PlayScoreReducedAnim(CS$<>8__locals3.CS$<>8__locals2.operation.NpcGamePoint, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
													}
												}
											}
										}
									});
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
								}
								break;
							}
							case 12:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									DebateUnit unit = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.FindUnit(CS$<>8__locals3.CS$<>8__locals2.operation.PawnId);
									unit.PlayProtectEffect();
								});
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(1f);
								break;
							case 13:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									DebateUnit unit = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.FindUnit(CS$<>8__locals3.CS$<>8__locals2.operation.PawnId);
									if (unit != null)
									{
										unit.PlayRemoveStrategyAnim(CS$<>8__locals3.CS$<>8__locals2.operation.Result, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
									}
								});
								break;
							case 14:
							{
								ViewDebate.<>c__DisplayClass129_2 CS$<>8__locals5 = CS$<>8__locals3;
								List<StrategyTarget> strategyTargets = CS$<>8__locals3.CS$<>8__locals2.operation.StrategyTargets;
								List<DebateUnit> targetUnitList;
								if (strategyTargets == null)
								{
									targetUnitList = null;
								}
								else
								{
									IEnumerable<ulong> source2 = (from s in strategyTargets
									where s.Type == EDebateStrategyTargetObjectType.Pawn
									select s).SelectMany((StrategyTarget s) => s.List);
									Func<ulong, DebateUnit> selector2;
									if ((selector2 = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>9__26) == null)
									{
										selector2 = (CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>9__26 = ((ulong id) => CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.FindUnit((int)id)));
									}
									targetUnitList = source2.Select(selector2).ToList<DebateUnit>();
								}
								CS$<>8__locals5.targetUnitList = targetUnitList;
								List<DebateUnit> targetUnitList2 = CS$<>8__locals3.targetUnitList;
								if (targetUnitList2 != null)
								{
									targetUnitList2.RemoveAll((DebateUnit u) => u == null);
								}
								float useCardTargetPawnAnimDuration = 0f;
								bool needPlayUseCardTargetPawnAnim = !CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed && this.NeedPlayUseCardTargetPawnAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId, CS$<>8__locals3.targetUnitList, out useCardTargetPawnAnimDuration);
								bool flag11 = !CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu;
								if (flag11)
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
									{
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayEnemyUseCardAnim(CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed, CS$<>8__locals3.CS$<>8__locals2.operation, CS$<>8__locals3.targetUnitList);
									});
									float interval = CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed ? 1f : 0.7f;
									bool flag12 = needPlayUseCardTargetPawnAnim;
									if (flag12)
									{
										interval += useCardTargetPawnAnimDuration;
									}
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(interval);
								}
								else
								{
									this.SetButtonInteractable(this.buttonForceGiveUp, false);
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
									{
										List<DebateCardItem> cardList = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.cardArea.CardList;
										Predicate<DebateCardItem> match;
										if ((match = CS$<>8__locals3.CS$<>8__locals2.<>9__38) == null)
										{
											match = (CS$<>8__locals3.CS$<>8__locals2.<>9__38 = ((DebateCardItem c) => c.Visible && c.CardView.CardConfig.TemplateId == CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId));
										}
										DebateCardItem cardItem = cardList.Find(match);
										bool flag23 = ViewDebate.FocusingCardItem == null;
										if (flag23)
										{
											CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.cardArea.SelectCard(cardItem);
										}
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayTaiwuUseCardAnim(CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed, CS$<>8__locals3.CS$<>8__locals2.operation, CS$<>8__locals3.targetUnitList);
									});
									float interval2 = CS$<>8__locals3.CS$<>8__locals2.operation.IsFailed ? 1f : 0.7f;
									bool flag13 = needPlayUseCardTargetPawnAnim;
									if (flag13)
									{
										interval2 += useCardTargetPawnAnimDuration;
									}
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(interval2);
								}
								break;
							}
							case 15:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									Vector2Int pos = new Vector2Int(CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.Target.Second);
									DebateBlock block = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.FindBlock(pos);
									block.PlayEffectAddedAnim(CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState);
									CharacterDisplayData charData = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.GetAudienceData(CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState.CasterId);
									DebateNodeEffectItem nodeEffectConfig = DebateNodeEffect.Instance[CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState.TemplateId];
									bool useTaiwuColor = (CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState.TemplateId == 3) ? block.IsSelf : CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState.IsHelpTaiwu;
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.AudienceSpeak(charData.CharacterId, nodeEffectConfig.BubbleContent, useTaiwuColor);
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.AudienceSpeak(charData.CharacterId, nodeEffectConfig.BubbleContent, useTaiwuColor);
								});
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
								break;
							case 16:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									Vector2Int pos = new Vector2Int(CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.Target.Second);
									DebateBlock block = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.FindBlock(pos);
									bool isLast2 = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.HasOnlyOneBlockEffect((short)CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState.TemplateId);
									block.PlayEffectRemovedAnim(isLast2);
								});
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
								break;
							case 19:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									Vector2Int pos = new Vector2Int(CS$<>8__locals3.CS$<>8__locals2.operation.Target.First, CS$<>8__locals3.CS$<>8__locals2.operation.Target.Second);
									DebateBlock block = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.debateGrid.FindBlock(pos);
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayBlockEffectTriggeredAnim(CS$<>8__locals3.CS$<>8__locals2.operation.NodeEffectState, block.RectTrans, CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
								});
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
								break;
							case 20:
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
								{
									CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddRecord(CS$<>8__locals3.CS$<>8__locals2.operation);
								});
								animTime = 0f;
								break;
							case 21:
							{
								bool hasExtraTriggeredAnim2 = DebateBlock.HasExtraTriggeredAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId);
								bool flag14 = hasExtraTriggeredAnim2;
								if (flag14)
								{
									DebateNodeEffectItem effectConfig2 = DebateNodeEffect.Instance[CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId];
									bool needAddEnergy = effectConfig2.TriggerEffectList.Exists((IntPair e) => e.First == 9);
									bool flag15 = needAddEnergy;
									if (flag15)
									{
										DebatePlayer player = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu ? this.selfPlayer : this.enemyPlayer;
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
										{
											CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.PlayBlockEffectTriggeredExtraAnim(CS$<>8__locals3.CS$<>8__locals2.operation.TemplateId, player.EnergySliderTrans);
										});
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
										{
											CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.SetEnergy(CS$<>8__locals3.CS$<>8__locals2.operation.TaiwuBases, true);
											CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.SetEnergy(CS$<>8__locals3.CS$<>8__locals2.operation.NpcBases, true);
										});
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
									}
								}
								break;
							}
							case 22:
							{
								List<short> cardIdList = CS$<>8__locals3.CS$<>8__locals2.operation.CardIdList;
								bool flag16 = cardIdList != null && cardIdList.Count > 0;
								if (flag16)
								{
									DebateCardView cardView = CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu ? this.selfResetCardView : this.enemyResetCardView;
									float duration = 0.1f;
									using (List<short>.Enumerator enumerator = CS$<>8__locals3.CS$<>8__locals2.operation.CardIdList.GetEnumerator())
									{
										TweenCallback <>9__43;
										while (enumerator.MoveNext())
										{
											short cardId = enumerator.Current;
											DG.Tweening.Sequence sequence = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence;
											TweenerCore<Quaternion, Vector3, QuaternionOptions> t = cardView.transform.DOLocalRotate(new Vector3(0f, 0f, 12f), duration, DG.Tweening.RotateMode.Fast).From(new Vector3(0f, 0f, -12f), true, false).OnStart(delegate
											{
												cardView.SetData(cardId, 0);
												cardView.SetEnabled(true, false);
												cardView.SetInteractable(false);
												cardView.ShowCover(!CS$<>8__locals3.CS$<>8__locals2.operation.IsTaiwu);
												AudioManager.Instance.PlaySound(ViewDebate.SoundCardFly, false, false);
											});
											TweenCallback action;
											if ((action = <>9__43) == null)
											{
												action = (<>9__43 = delegate()
												{
													cardView.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
												});
											}
											sequence.Append(t.OnComplete(action));
											CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(duration);
										}
									}
								}
								break;
							}
							}
							bool flag17 = CS$<>8__locals2.operation.OperationType == 20;
							if (!flag17)
							{
								bool flag18 = CS$<>8__locals2.operation.OperationType == 8;
								if (!flag18)
								{
									sbyte operationType = CS$<>8__locals2.operation.OperationType;
									bool flag19 = operationType == 15 || operationType == 16 || operationType == 19 || operationType == 21;
									if (!flag19)
									{
										operationType = CS$<>8__locals2.operation.OperationType;
										bool flag20 = operationType == 6 || operationType == 13;
										if (flag20)
										{
											int lastIndex = index - 1;
											DebateOperation lastOperation = this.Model.DebateGame.DebateOperations.CheckIndex(lastIndex) ? this.Model.DebateGame.DebateOperations[lastIndex] : null;
											if (lastOperation == null)
											{
												goto IL_110F;
											}
											operationType = lastOperation.OperationType;
											if (operationType != 6 && operationType != 13)
											{
												goto IL_110F;
											}
											bool flag21 = lastOperation.OperationType != CS$<>8__locals2.operation.OperationType;
											IL_1110:
											bool changeType = flag21;
											int num2 = index;
											IEnumerable<DebateOperation> source3 = this.Model.DebateGame.DebateOperations.Where(delegate(DebateOperation o)
											{
												sbyte operationType2 = o.OperationType;
												return operationType2 == 6 || operationType2 == 13;
											});
											Func<DebateOperation, int> selector3;
											if ((selector3 = CS$<>8__locals2.CS$<>8__locals1.<>9__45) == null)
											{
												selector3 = (CS$<>8__locals2.CS$<>8__locals1.<>9__45 = ((DebateOperation o) => CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.DebateOperations.IndexOf(o)));
											}
											bool isLast = num2 == source3.Max(selector3);
											bool flag22 = changeType || isLast;
											if (flag22)
											{
												CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(1.5f);
											}
											goto IL_11DE;
											IL_110F:
											flag21 = false;
											goto IL_1110;
										}
										CS$<>8__locals2.CS$<>8__locals1.sequence.AppendInterval(animTime);
										CS$<>8__locals2.CS$<>8__locals1.sequence.AppendCallback(delegate
										{
											CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.SetEnergy(CS$<>8__locals2.operation.TaiwuBases, true);
											CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.SetStress(CS$<>8__locals2.operation.TaiwuPressure, true);
											CS$<>8__locals2.CS$<>8__locals1.<>4__this.selfPlayer.SetScore(CS$<>8__locals2.operation.TaiwuGamePoint);
											CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.SetEnergy(CS$<>8__locals2.operation.NpcBases, true);
											CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.SetStress(CS$<>8__locals2.operation.NpcPressure, true);
											CS$<>8__locals2.CS$<>8__locals1.<>4__this.enemyPlayer.SetScore(CS$<>8__locals2.operation.NpcGamePoint);
											bool flag23 = CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.IsGameOver && (CS$<>8__locals2.operation.TaiwuGamePoint <= 0 || CS$<>8__locals2.operation.NpcGamePoint <= 0);
											if (flag23)
											{
												CS$<>8__locals2.CS$<>8__locals1.<>4__this.GameOver(CS$<>8__locals2.CS$<>8__locals1.<>4__this.Model.DebateGame.IsTaiwuWin, false);
												CS$<>8__locals2.CS$<>8__locals1.sequence.Pause<DG.Tweening.Sequence>();
											}
										});
									}
								}
							}
							IL_11DE:;
						}
						CS$<>8__locals1.sequence.AppendCallback(delegate
						{
							CS$<>8__locals1.<>4__this.Model.IsPlayingOperation = false;
							Action onPlayEnd3 = CS$<>8__locals1.onPlayEnd;
							if (onPlayEnd3 != null)
							{
								onPlayEnd3();
							}
						});
						CS$<>8__locals1.sequence.Play<DG.Tweening.Sequence>();
					}
				}
			}
		}

		// Token: 0x060085C4 RID: 34244 RVA: 0x003E3800 File Offset: 0x003E1A00
		public void PlayBlockEffectTriggeredAnim(DebateNodeEffectState nodeEffectState, Transform target, bool isTaiwu)
		{
			DebateNodeEffectItem nodeEffectConfig = DebateNodeEffect.Instance[nodeEffectState.TemplateId];
			AudioManager.Instance.PlaySound(nodeEffectConfig.TriggerSound, false, true);
			int index = nodeEffectState.TemplateId + 1;
			string triggeredAnimName = DebateBlock.HasExtraTriggeredAnim((short)nodeEffectState.TemplateId) ? string.Format("eff_lifeskillcombat_5dalei_{0}xs1", index) : string.Format("eff_lifeskillcombat_5dalei_{0}xs", index);
			bool isAddEnergy = nodeEffectState.TemplateId == 1;
			bool flag = isAddEnergy;
			if (flag)
			{
				GameObject effectObj = this.effectPlayer.PlayEffectAt(target, triggeredAnimName, 1f, false);
				effectObj.transform.DOKill(false);
				DebatePlayer player = isTaiwu ? this.selfPlayer : this.enemyPlayer;
				effectObj.transform.DOMove(player.EnergySliderTrans.position, 0.5f, false);
			}
			else
			{
				this.effectPlayer.PlayEffectAt(target, triggeredAnimName, 1f, false);
			}
		}

		// Token: 0x060085C5 RID: 34245 RVA: 0x003E38F0 File Offset: 0x003E1AF0
		public void PlayBlockEffectTriggeredExtraAnim(short nodeEffectTemplateId, Transform target)
		{
			DebateNodeEffectItem nodeEffectConfig = DebateNodeEffect.Instance[nodeEffectTemplateId];
			AudioManager.Instance.PlaySound(nodeEffectConfig.ExtraTriggerSound, false, true);
			int index = (int)(nodeEffectTemplateId + 1);
			string triggeredExtraAnimName = string.Format("eff_lifeskillcombat_5dalei_{0}xs2", index);
			this.effectPlayer.PlayEffectAt(target, triggeredExtraAnimName, 1f, false);
		}

		// Token: 0x060085C6 RID: 34246 RVA: 0x003E3948 File Offset: 0x003E1B48
		private void AddRecord(DebateOperation operation)
		{
			bool isTaiwu = operation.IsTaiwu;
			if (isTaiwu)
			{
				this.selfRecord.Add(operation);
			}
			else
			{
				this.enemyRecord.Add(operation);
			}
		}

		// Token: 0x060085C7 RID: 34247 RVA: 0x003E397C File Offset: 0x003E1B7C
		private float PlayBlockEffectAnim()
		{
			return 0f;
		}

		// Token: 0x060085C8 RID: 34248 RVA: 0x003E3994 File Offset: 0x003E1B94
		private void PlayCardChange(DebateOperation operation)
		{
			float duration = 0.5f;
			float minScale = 0.2f;
			Vector3 rotation = Vector3.zero.SetZ(90f);
			bool flag = operation.Source == 2;
			if (flag)
			{
				AudioManager.Instance.PlaySound(ViewDebate.SoundMoveCard, false, true);
				int num = operation.Destination;
				GameObject controlPoint = (num == 0 || num == 1) ? this.leftCardAnimControlPoint : this.rightCardAnimControlPoint;
				Vector3 controlPos = controlPoint.transform.position;
				DebateCardItem cardItem = this.cardArea.CardList[operation.Index];
				RectTransform rectTrans = cardItem.CardView.RectTransform;
				rectTrans.DOKill(true);
				Transform originParent = rectTrans.parent;
				rectTrans.SetParent(this.cardArea.transform);
				Transform destination = this.GetCardGroupTransform(operation.Destination);
				Vector3 sourcePos = rectTrans.position;
				DOVirtual.Float(0f, 1f, duration, delegate(float stepValue)
				{
					rectTrans.position = BezierMath.Bezier2(sourcePos, controlPos, destination.position, stepValue);
				}).SetTarget(rectTrans).SetEase(Ease.OutQuart);
				rectTrans.DOScale(minScale, duration);
				rectTrans.DORotate(rotation, duration, DG.Tweening.RotateMode.Fast).OnComplete(delegate
				{
					cardItem.SetVisible(false, true);
					rectTrans.SetParent(originParent);
					rectTrans.localPosition = Vector3.zero;
					rectTrans.localScale = Vector3.one;
					rectTrans.localRotation = Quaternion.identity;
					this.cardArea.LayoutSelfCards();
				});
			}
			else
			{
				bool flag2 = operation.Destination == 2;
				if (flag2)
				{
					AudioManager.Instance.PlaySound(ViewDebate.SoundMoveCard, false, true);
					int num = operation.Source;
					GameObject controlPoint2 = (num == 0 || num == 1) ? this.leftCardAnimControlPoint : this.rightCardAnimControlPoint;
					Vector3 controlPos = controlPoint2.transform.position;
					DebateCardItem cardItem = this.cardArea.CardList[operation.Index];
					RectTransform rectTrans = cardItem.CardView.RectTransform;
					rectTrans.DOKill(true);
					cardItem.SetVisible(true, true);
					cardItem.CardView.SetData(operation.TemplateId, operation.Index);
					cardItem.CardView.SetEnabled(true, false);
					cardItem.CardView.SetInteractable(false);
					cardItem.CardView.SetPointerTrigger(false);
					this.cardArea.LayoutSelfCards();
					Transform originParent = rectTrans.parent;
					Transform source = this.GetCardGroupTransform(operation.Source);
					rectTrans.localScale = Vector3.zero;
					TweenCallback <>9__4;
					DOVirtual.DelayedCall(0.1f, delegate
					{
						Vector3 sourcePos = source.transform.position;
						Vector3 destinationPos = cardItem.RectTrans.position;
						rectTrans.SetParent(this.cardArea.transform);
						rectTrans.position = sourcePos;
						DOVirtual.Float(0f, 1f, duration, delegate(float stepValue)
						{
							rectTrans.position = BezierMath.Bezier2(sourcePos, controlPos, destinationPos, stepValue);
						}).SetTarget(rectTrans).SetEase(Ease.OutQuart);
						rectTrans.DOScale(0.8f, duration).From(minScale, true, false);
						TweenerCore<Quaternion, Vector3, QuaternionOptions> t = rectTrans.DORotate(Vector3.zero, duration, DG.Tweening.RotateMode.Fast).From(rotation, true, false);
						TweenCallback action;
						if ((action = <>9__4) == null)
						{
							action = (<>9__4 = delegate()
							{
								rectTrans.SetParent(originParent);
								rectTrans.localPosition = Vector3.zero;
								rectTrans.localScale = Vector3.one;
								rectTrans.localRotation = Quaternion.identity;
							});
						}
						t.OnComplete(action);
					}, false);
				}
			}
		}

		// Token: 0x060085C9 RID: 34249 RVA: 0x003E3CE8 File Offset: 0x003E1EE8
		private void PlayTaiwuUseCardAnim(bool usingFailed, DebateOperation operation, List<DebateUnit> targetUnitList)
		{
			if (usingFailed)
			{
				ViewDebate.FocusingCardItem.SetVisible(false, true);
				this.PlayUseCardFailedAnim(true, operation.TemplateId, ViewDebate.FocusingCardItem.CardView.transform.position);
				this.cardArea.UnselectCard(true);
			}
			else
			{
				AudioManager.Instance.PlaySound(ViewDebate.SoundUseCard, false, true);
				ViewDebate.FocusingCardItem.CardView.PlayUseEffect();
				this.cardArea.UnselectCard(false);
				this.PlayUseCardTargetPawnAnim(true, operation.TemplateId, targetUnitList);
			}
		}

		// Token: 0x060085CA RID: 34250 RVA: 0x003E3D78 File Offset: 0x003E1F78
		private void PlayEnemyUseCardAnim(bool usingFailed, DebateOperation operation, List<DebateUnit> targetUnitList)
		{
			AudioManager.Instance.PlaySound(ViewDebate.SoundMoveCard, false, true);
			this.enemyUsingCardView.SetData(operation.TemplateId, 0);
			this.enemyUsingCardView.ShowCover(true);
			this.enemyUsingCardView.SetEnabled(true, false);
			this.enemyUsingCardView.SetInteractable(false);
			this.enemyUsingCardView.SetPointerTrigger(false);
			this.enemyUsingCardView.DOKill(false);
			this.enemyUsingCardView.gameObject.SetActive(true);
			this.enemyUsingCardView.RectTransform.DOMove(this.enemyUsingCardPoint.transform.position, 0.4f, false).From(this.enemyPlayer.RectTrans.position, true, false).SetEase(Ease.OutQuart);
			this.enemyUsingCardView.RectTransform.DOScale(Vector3.one.SetX(-1f), 0.4f).From(Vector3.zero, true, false).SetEase(Ease.OutQuart);
			this.enemyUsingCardView.RectTransform.DORotate(new Vector3(0f, 0f, 360f), 0.4f, DG.Tweening.RotateMode.FastBeyond360).From(Vector3.zero, true, false);
			TweenCallback <>9__1;
			DOVirtual.DelayedCall(0.4f, delegate
			{
				bool usingFailed2 = usingFailed;
				if (usingFailed2)
				{
					this.enemyUsingCardView.gameObject.SetActive(false);
					this.PlayUseCardFailedAnim(false, operation.TemplateId, this.enemyUsingCardView.transform.position);
				}
				else
				{
					this.enemyUsingCardView.PlayUseEffect();
					AudioManager.Instance.PlaySound(ViewDebate.SoundUseCard, false, true);
					this.PlayUseCardTargetPawnAnim(false, operation.TemplateId, targetUnitList);
					float delay = 0.7f;
					TweenCallback callback;
					if ((callback = <>9__1) == null)
					{
						callback = (<>9__1 = delegate()
						{
							AudioManager.Instance.PlaySound(ViewDebate.SoundMoveCard, false, true);
							Transform destination = this.GetCardGroupTransform(4);
							this.enemyUsingCardView.RectTransform.DOMove(destination.position, 0.4f, false).From(this.enemyUsingCardPoint.transform.position, true, false).SetEase(Ease.OutQuart);
							this.enemyUsingCardView.RectTransform.DOScale(Vector3.zero, 0.4f).From(Vector3.one.SetX(-1f), true, false).SetEase(Ease.OutQuart);
							this.enemyUsingCardView.RectTransform.DORotate(new Vector3(0f, 0f, 720f), 0.4f, DG.Tweening.RotateMode.FastBeyond360).From(new Vector3(0f, 0f, 360f), true, false);
							bool flag = targetUnitList != null;
							if (flag)
							{
								foreach (DebateUnit unit in targetUnitList)
								{
									unit.RefreshStrategy(0, false, false);
								}
							}
						});
					}
					DOVirtual.DelayedCall(delay, callback, false).SetTarget(this.enemyUsingCardView);
				}
			}, false).SetTarget(this.enemyUsingCardView);
		}

		// Token: 0x060085CB RID: 34251 RVA: 0x003E3F00 File Offset: 0x003E2100
		private bool NeedPlayUseCardTargetPawnAnim(short strategyTemplateId, List<DebateUnit> targetUnitList, out float duration)
		{
			DebateStrategyItem config = DebateStrategy.Instance[strategyTemplateId];
			bool hasTargetEffect = targetUnitList != null && targetUnitList.Count > 0 && config.MarkType == EDebateStrategyMarkType.Affect;
			duration = (hasTargetEffect ? 0.5f : 0f);
			return hasTargetEffect;
		}

		// Token: 0x060085CC RID: 34252 RVA: 0x003E3F4C File Offset: 0x003E214C
		private void PlayUseCardTargetPawnAnim(bool isTaiwuCast, short strategyTemplateId, List<DebateUnit> targetUnitList)
		{
			string effectName = isTaiwuCast ? "eff_lifeskillcombat_ui_dxkp_shan_lan" : "eff_lifeskillcombat_ui_dxkp_shan_hong";
			float duration;
			bool hasTargetEffect = this.NeedPlayUseCardTargetPawnAnim(strategyTemplateId, targetUnitList, out duration);
			bool flag = hasTargetEffect;
			if (flag)
			{
				foreach (DebateUnit unit in targetUnitList)
				{
					this.effectPlayer.PlayEffectAt(unit.GetTransform(), effectName, duration, false);
				}
			}
		}

		// Token: 0x060085CD RID: 34253 RVA: 0x003E3FD0 File Offset: 0x003E21D0
		private void PlayUseCardFailedAnim(bool isTaiwu, short strategyTemplateId, Vector3 pos)
		{
			AudioManager.Instance.PlaySound(ViewDebate.SoundUseCardFailed, false, true);
			DebateCardCameraManager.Instance.Show(isTaiwu, strategyTemplateId);
			this.castStrategyFailedEffect.transform.position = pos;
			this.castStrategyFailedEffect.gameObject.SetActive(true);
			RawImage image = this.castStrategyFailedEffect.GetComponentInChildren<RawImage>();
			image.texture = DebateCardCameraManager.Instance.Camera.targetTexture;
			DOVirtual.Float(1f, 0.5f, 1f, delegate(float value)
			{
				image.material.SetFloat("_rongjie", value);
			}).OnComplete(delegate
			{
				DebateCardCameraManager.Instance.Hide();
				this.castStrategyFailedEffect.gameObject.SetActive(false);
			});
		}

		// Token: 0x060085CE RID: 34254 RVA: 0x003E408C File Offset: 0x003E228C
		private void PlayMakeMoveFailedAnim(bool isTaiwu, Vector3 pos)
		{
			AudioManager.Instance.PlaySound(ViewDebate.SoundCreateUnitFailed, false, true);
			DebateUnitCameraManager.Instance.Show(isTaiwu, this.Model.LifeSkillType);
			this.makeMoveFailedEffect.transform.position = pos;
			this.makeMoveFailedEffect.gameObject.SetActive(true);
			RawImage image = this.makeMoveFailedEffect.GetComponentInChildren<RawImage>();
			image.texture = DebateUnitCameraManager.Instance.Camera.targetTexture;
			DOVirtual.Float(1f, 0.5f, 1f, delegate(float value)
			{
				image.material.SetFloat("_rongjie", value);
			}).OnComplete(delegate
			{
				DebateUnitCameraManager.Instance.Hide();
				this.makeMoveFailedEffect.gameObject.SetActive(false);
			});
		}

		// Token: 0x060085CF RID: 34255 RVA: 0x003E4154 File Offset: 0x003E2354
		private void PlayReducePointAnimOnArrive(Pawn pawn, DebateOperation operation)
		{
			AudioManager.Instance.PlaySound(ViewDebate.SoundUnitArrive, false, true);
			DebateUnit findUnit = this.debateGrid.FindUnit(pawn.Id);
			findUnit.RectTrans.localScale = Vector3.zero;
			bool isReduceTaiwu = !pawn.IsOwnedByTaiwu;
			string arriveEffectName = isReduceTaiwu ? "eff_lifeskillcombat_ui_dx_hualizi2" : "eff_lifeskillcombat_ui_dx_hualizi";
			float arriveEffectDuration = 0.5f;
			float flyDuration = 0.25f;
			this.effectPlayer.PlayEffectAt(findUnit.RectTrans, arriveEffectName, arriveEffectDuration, false);
			DOVirtual.DelayedCall(arriveEffectDuration, delegate
			{
				DebatePlayer player = isReduceTaiwu ? this.selfPlayer : this.enemyPlayer;
				int curScore = isReduceTaiwu ? operation.TaiwuGamePoint : operation.NpcGamePoint;
				curScore = Mathf.Max(0, curScore);
				int lastScore = player.Score;
				TweenCallback <>9__1;
				for (int i = lastScore; i > curScore; i--)
				{
					string flyEffectName = isReduceTaiwu ? string.Format("eff_lifeskillcombat_ui_defentuowei_hong{0}_{1}", pawn.Coordinate.Second + 1, 7 - i) : string.Format("eff_lifeskillcombat_ui_defentuowei_lan{0}_{1}", pawn.Coordinate.Second + 1, 7 - i);
					this.effectPlayer.PlayEffectAt(findUnit.RectTrans, flyEffectName, flyDuration, isReduceTaiwu);
					float delay = flyDuration * 0.6f;
					TweenCallback callback;
					if ((callback = <>9__1) == null)
					{
						callback = (<>9__1 = delegate()
						{
							player.PlayScoreReducedAnim(curScore, pawn.IsOwnedByTaiwu);
						});
					}
					DOVirtual.DelayedCall(delay, callback, false);
				}
			}, false);
		}

		// Token: 0x060085D0 RID: 34256 RVA: 0x003E422C File Offset: 0x003E242C
		private bool FlagsCanUpdate()
		{
			foreach (Spine.AnimationState flagState in from ske in this.flagsRoot.GetComponentsInChildren<SkeletonGraphic>()
			select ske.AnimationState)
			{
				TrackEntry currentTrack = flagState.GetCurrent(0);
				bool flag = currentTrack != null && !currentTrack.IsComplete && !currentTrack.Loop;
				if (flag)
				{
					return false;
				}
			}
			return this.Model.DebateGame.State != 2 && !this.Model.IsPlayingOperation;
		}

		// Token: 0x060085D1 RID: 34257 RVA: 0x003E42F4 File Offset: 0x003E24F4
		private void ResetFlags()
		{
			SkeletonGraphic[] skeletonGraphics = this.flagsRoot.GetComponentsInChildren<SkeletonGraphic>();
			foreach (SkeletonGraphic skeletonGraphic in skeletonGraphics)
			{
				Spine.AnimationState flagState = skeletonGraphic.AnimationState;
				flagState.SetAnimation(0, "grey", true);
			}
		}

		// Token: 0x060085D2 RID: 34258 RVA: 0x003E433C File Offset: 0x003E253C
		private void UpdateFlags()
		{
			ViewDebate.<>c__DisplayClass144_0 CS$<>8__locals1 = new ViewDebate.<>c__DisplayClass144_0();
			int diff = this.selfPlayer.Score - this.enemyPlayer.Score;
			CS$<>8__locals1.target = ((diff == 0) ? "grey" : ((diff > 0) ? "blue" : "red"));
			CS$<>8__locals1.audioManager = AudioManager.Instance;
			SkeletonGraphic[] skeletonGraphics = this.flagsRoot.GetComponentsInChildren<SkeletonGraphic>();
			SkeletonGraphic[] array = skeletonGraphics;
			int i = 0;
			while (i < array.Length)
			{
				SkeletonGraphic skeletonGraphic = array[i];
				ViewDebate.<>c__DisplayClass144_1 CS$<>8__locals2 = new ViewDebate.<>c__DisplayClass144_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.flagState = skeletonGraphic.AnimationState;
				TrackEntry currentTrack = CS$<>8__locals2.flagState.GetCurrent(0);
				string current = (currentTrack != null) ? currentTrack.Animation.Name : null;
				bool flag = !string.IsNullOrEmpty(current);
				if (flag)
				{
					bool flag2 = current == CS$<>8__locals2.CS$<>8__locals1.target;
					if (!flag2)
					{
						int plastic = current.IndexOf('_');
						bool flag3 = plastic < 0;
						if (flag3)
						{
							TrackEntry track = CS$<>8__locals2.flagState.SetAnimation(0, current + "_" + CS$<>8__locals2.CS$<>8__locals1.target, false);
							track.MixDuration = 0f;
							track.Complete += delegate(TrackEntry _)
							{
								CS$<>8__locals2.CS$<>8__locals1.<UpdateFlags>g__PlayTargetAnimation|0(CS$<>8__locals2.flagState);
							};
							CS$<>8__locals2.CS$<>8__locals1.audioManager.PlaySound(ViewDebate.SoundChangeFlag, false, true);
						}
						else
						{
							ViewDebate.<>c__DisplayClass144_2 CS$<>8__locals3 = new ViewDebate.<>c__DisplayClass144_2();
							CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
							ViewDebate.<>c__DisplayClass144_2 CS$<>8__locals4 = CS$<>8__locals3;
							string text = current;
							int num = plastic + 1;
							CS$<>8__locals4.ending = text.Substring(num, text.Length - num);
							currentTrack.MixDuration = 0f;
							currentTrack.Complete += ((CS$<>8__locals3.ending == CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.target) ? delegate(TrackEntry _)
							{
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<UpdateFlags>g__PlayTargetAnimation|0(CS$<>8__locals3.CS$<>8__locals2.flagState);
							} : delegate(TrackEntry _)
							{
								TrackEntry track2 = CS$<>8__locals3.CS$<>8__locals2.flagState.SetAnimation(0, CS$<>8__locals3.ending + "_" + CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.target, false);
								track2.MixDuration = 0f;
								TrackEntry trackEntry = track2;
								Spine.AnimationState.TrackEntryDelegate value;
								if ((value = CS$<>8__locals3.CS$<>8__locals2.<>9__4) == null)
								{
									value = (CS$<>8__locals3.CS$<>8__locals2.<>9__4 = delegate(TrackEntry _)
									{
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<UpdateFlags>g__PlayTargetAnimation|0(CS$<>8__locals3.CS$<>8__locals2.flagState);
									});
								}
								trackEntry.Complete += value;
								CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.audioManager.PlaySound(ViewDebate.SoundChangeFlag, false, true);
							});
						}
					}
				}
				else
				{
					CS$<>8__locals2.CS$<>8__locals1.<UpdateFlags>g__PlayTargetAnimation|0(CS$<>8__locals2.flagState);
				}
				IL_200:
				i++;
				continue;
				goto IL_200;
			}
		}

		// Token: 0x060085D3 RID: 34259 RVA: 0x003E455C File Offset: 0x003E275C
		private void RefreshButtonInteractable()
		{
			bool interactable = this.Model.IsTaiwuRound && !this.Model.Pause && !this.Model.IsPlayingOperation && !this.Model.IsAuto;
			this.SetButtonInteractable(this.buttonForceGiveUp, interactable);
			this.SetButtonInteractable(this.buttonGiveUp, interactable);
			this.SetButtonInteractable(this.buttonRoundEnd, interactable);
		}

		// Token: 0x060085D4 RID: 34260 RVA: 0x003E45D0 File Offset: 0x003E27D0
		private void OnClickButtonRoundEnd()
		{
			int count;
			bool flag = this.Model.DebateGame.TryGetPlayerCardRemovingCount(true, out count) && this.Model.RemovingCards.Count != count;
			if (flag)
			{
				this.EnterSelectRemovingCardMode(count);
			}
			else
			{
				this.roundEndEffect.Play();
				this.FinishStage();
			}
		}

		// Token: 0x060085D5 RID: 34261 RVA: 0x003E4630 File Offset: 0x003E2830
		private void OnClickButtonGiveUp()
		{
			bool flag = !this.Model.IsTaiwuRound || this.Model.IsGameOver;
			if (!flag)
			{
				string title = LocalStringManager.Get(LanguageKey.UI_LifeSkillCombat_Tips_SelfGiveUpTitle);
				string content = LocalStringManager.Get(LanguageKey.UI_LifeSkillCombat_Tips_SelfGiveUpContent);
				CommonUtils.ShowConfirmDialog(title, content, delegate
				{
					this.LogStepInfo("send:RequestCurrentPlayerCommitOperation: self give up", true);
					this.GameOver(false, true);
					this.selfPlayer.Speak(15, 4f);
				}, null, EDialogType.None);
			}
		}

		// Token: 0x060085D6 RID: 34262 RVA: 0x003E468C File Offset: 0x003E288C
		private void OnClickButtonForceGiveUp()
		{
			bool flag = !this.Model.IsTaiwuRound || this.Model.IsGameOver;
			if (!flag)
			{
				this.SetButtonInteractable(this.buttonForceGiveUp, false);
				this.LogStepInfo("send:RequestCurrentPlayerCommitOperation: force adversary give up", true);
				TaiwuDomainMethod.AsyncCall.DebateGameTryForceWin(this, delegate(int offset, RawDataPool pool)
				{
					bool canForceWin = false;
					Serializer.Deserialize(pool, offset, ref canForceWin);
					bool flag2 = canForceWin;
					if (flag2)
					{
						this.GameOver(true, false);
					}
					else
					{
						this.enemyPlayer.Speak(17, 4f);
					}
					this.selfPlayer.Speak(16, 4f);
					bool flag3 = !canForceWin;
					if (flag3)
					{
						this.FinishStage();
					}
				});
			}
		}

		// Token: 0x060085D7 RID: 34263 RVA: 0x003E46EC File Offset: 0x003E28EC
		private void WatchEnemyStrategy()
		{
			bool isShowingEnemyCard = this.cardArea.IsShowingEnemyCard();
			this.cardArea.ShowEnemyCard(!isShowingEnemyCard);
		}

		// Token: 0x060085D8 RID: 34264 RVA: 0x003E4718 File Offset: 0x003E2918
		private void OnClickBtnOpenCharMenu()
		{
			bool flag = this._allAudienceList == null || this._allAudienceList.Count == 0;
			if (!flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("CharacterIdList", this._allAudienceList);
				argBox.Set("CanOperate", false);
				argBox.Set("OpenFromCombatPrepare", true);
				argBox.Set("PreviousView", 3);
				argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.AttainmentBase, ECharacterSubPage.AttainmentLifeSkill));
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIElement characterMenu = UIElement.CharacterMenu;
				characterMenu.OnShowed = (Action)Delegate.Combine(characterMenu.OnShowed, new Action(delegate()
				{
					GEvent.OnEvent(UiEvents.CombatLifeSkillClickChar, null);
					Time.timeScale = 1f;
				}));
				UIElement characterMenu2 = UIElement.CharacterMenu;
				characterMenu2.OnHide = (Action)Delegate.Combine(characterMenu2.OnHide, new Action(delegate()
				{
					this.Model.RefreshTimeScale();
				}));
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			}
		}

		// Token: 0x060085D9 RID: 34265 RVA: 0x003E481C File Offset: 0x003E2A1C
		private void EnterCardFocusMode(ArgumentBox argumentBox)
		{
			bool isAuto = this.Model.IsAuto;
			if (!isAuto)
			{
				this.buttonConfirm.ClearAndAddListener(new Action(this.TaiwuUseCard));
				this.buttonConfirm.gameObject.SetActive(true);
				bool flag = this.PrepareUseCard();
				if (!flag)
				{
					this.controlPanel.transform.SetParent(this.cardArea.transform);
					ViewDebate.FocusingCardItem.CardView.transform.SetParent(this.highlightRoot);
					this.selectCardArea.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x060085DA RID: 34266 RVA: 0x003E48BC File Offset: 0x003E2ABC
		private void ExitCardFocusMode(ArgumentBox argumentBox)
		{
			this.controlPanel.transform.SetParent(base.transform);
			ConchShipCursor.Instance.HideLifeSkillCombatUseStrategyTip();
			bool refreshStrategy;
			bool flag = argumentBox == null || !argumentBox.Get("refreshStrategy", out refreshStrategy);
			if (flag)
			{
				refreshStrategy = true;
			}
			this.HideAllStrategyTarget(refreshStrategy);
			this.HideCreateUnitArea(false);
			bool activeSelf = this.selectCardArea.activeSelf;
			if (activeSelf)
			{
				ViewDebate.FocusingCardItem.CardView.RectTransform.SetParent(ViewDebate.FocusingCardItem.transform);
				ViewDebate.FocusingCardItem.CardView.RectTransform.anchoredPosition = Vector3.zero;
				this.selectCardArea.gameObject.SetActive(false);
			}
			this.selfPlayer.PreviewEnergy(this.selfPlayer.Energy, false);
			this.selfPlayer.PreviewStrategyCount(this.selfPlayer.StrategyCount);
			this._highlightCardTargetStack.Clear();
			this._unitSelectionStack.Clear();
			this._selectedStrategyTargetList.Clear();
			this._selectedSelectableTargetList.Clear();
		}

		// Token: 0x060085DB RID: 34267 RVA: 0x003E49D8 File Offset: 0x003E2BD8
		private void HideAllStrategyTarget(bool refreshStrategy = true)
		{
			while (this._highlightCardTargetStack.Count > 0)
			{
				ValueTuple<IDebateSelectable, Transform, int, Action> info = this._highlightCardTargetStack.Pop();
				info.Item1.GetTransform().SetParent(info.Item2);
				info.Item1.GetTransform().SetSiblingIndex(info.Item3);
				info.Item4();
			}
			bool flag = this.cardArea.IsShowingEnemyCard();
			if (flag)
			{
				this.cardArea.ShowEnemyCard(false);
			}
			this.cardArea.selectedCardHasScale = true;
			this.debateGrid.RefreshAllUnit(false, refreshStrategy);
			this.debateGrid.RefreshAllBlock();
		}

		// Token: 0x060085DC RID: 34268 RVA: 0x003E4A84 File Offset: 0x003E2C84
		private bool PrepareUseCard()
		{
			bool needSelectTarget = this.Model.NeedSelectTarget(ViewDebate.FocusingCardItem.CardView.CardConfig);
			bool flag = !needSelectTarget;
			bool result;
			if (flag)
			{
				short strategyTemplateId = ViewDebate.FocusingCardItem.CardView.CardConfig.TemplateId;
				List<StrategyTarget> list;
				bool targetIsMeet = this.Model.DebateGame.TryGetStrategyTarget(strategyTemplateId, true, out list);
				bool costIsMeet = this.Model.CheckCost(ViewDebate.FocusingCardItem.CardView.CardConfig);
				this.buttonConfirm.interactable = (targetIsMeet && costIsMeet);
				int previewStrategyCount = this.selfPlayer.StrategyCount - (int)ViewDebate.FocusingCardItem.CardView.CardConfig.UsedCost;
				this.selfPlayer.PreviewStrategyCount(previewStrategyCount);
				this.buttonConfirm.gameObject.SetActive(!this.buttonConfirm.interactable);
				bool interactable = this.buttonConfirm.interactable;
				if (interactable)
				{
					this.TaiwuUseCard();
					result = true;
				}
				else
				{
					ConchShipCursor.Instance.HideLifeSkillCombatUseStrategyTip();
					result = false;
				}
			}
			else
			{
				foreach (short[] targetInfo in ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList)
				{
					DebateStrategyTargetItem targetConfig = DebateStrategyTarget.Instance[targetInfo[0]];
					this._selectedStrategyTargetList.Add(new StrategyTarget(targetConfig.ObjectType, new List<ulong>()));
					this._selectedSelectableTargetList.Add(new List<IDebateSelectable>());
				}
				result = this.CheckTarget();
			}
			return result;
		}

		// Token: 0x060085DD RID: 34269 RVA: 0x003E4C30 File Offset: 0x003E2E30
		private bool CheckTarget()
		{
			this.CombatLifeSkillHoverStrategy(null);
			short[] lastShowingTargetInfo;
			int curIndex;
			bool isMax;
			bool targetIsMeet = this.CheckTargetIsAllMeet(out lastShowingTargetInfo, out curIndex, out isMax);
			bool costIsMeet = this.Model.CheckCost(ViewDebate.FocusingCardItem.CardView.CardConfig);
			this.buttonConfirm.interactable = (targetIsMeet && costIsMeet);
			short[] targetInfo = lastShowingTargetInfo;
			short targetTemplateId = targetInfo[0];
			DebateStrategyTargetItem targetConfig = DebateStrategyTarget.Instance[targetTemplateId];
			this.RefreshTargetCount(curIndex);
			bool flag = this.CheckTargetIsAllFixedCount();
			if (flag)
			{
				this.buttonConfirm.gameObject.SetActive(false);
				bool interactable = this.buttonConfirm.interactable;
				if (interactable)
				{
					this.TaiwuUseCard();
					return true;
				}
			}
			short strategyTemplateId = ViewDebate.FocusingCardItem.CardView.CardConfig.TemplateId;
			List<StrategyTarget> strategyTargetList;
			this.Model.DebateGame.TryGetStrategyTarget(strategyTemplateId, true, out strategyTargetList);
			StrategyTarget strategyTarget = strategyTargetList.CheckIndex(curIndex) ? strategyTargetList[curIndex] : null;
			List<ulong> list = (strategyTarget != null) ? strategyTarget.List : null;
			bool flag2 = list == null || list.Count <= 0;
			bool result;
			if (flag2)
			{
				this.HideAllStrategyTarget(true);
				ViewDebate.FocusingCardItem.CardView.SetSelected(true);
				result = false;
			}
			else
			{
				bool flag3 = !this.GetNeedMergeTarget() && targetConfig.ObjectType != EDebateStrategyTargetObjectType.PawnGrade;
				if (flag3)
				{
					for (int i = 0; i < this._selectedSelectableTargetList.Count; i++)
					{
						bool flag4 = i < curIndex;
						if (flag4)
						{
							foreach (IDebateSelectable lifeSkillCombatSelectable in this._selectedSelectableTargetList[i])
							{
								lifeSkillCombatSelectable.ShowStrategyTargetMark(false, null);
								lifeSkillCombatSelectable.SetSelected(true);
							}
						}
					}
					using (Stack<ValueTuple<IDebateSelectable, Transform, int, Action>>.Enumerator enumerator2 = this._highlightCardTargetStack.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							ValueTuple<IDebateSelectable, Transform, int, Action> valueTuple = enumerator2.Current;
							Predicate<IDebateSelectable> <>9__1;
							bool flag5 = this._selectedSelectableTargetList.Exists(delegate(List<IDebateSelectable> l)
							{
								Predicate<IDebateSelectable> match;
								if ((match = <>9__1) == null)
								{
									match = (<>9__1 = ((IDebateSelectable s) => s == valueTuple.Item1));
								}
								return l.Exists(match);
							});
							if (!flag5)
							{
								valueTuple.Item1.ShowStrategyTargetMark(false, null);
							}
						}
					}
				}
				switch (targetConfig.ObjectType)
				{
				case EDebateStrategyTargetObjectType.Pawn:
					this.ShowStrategyTargetPawn(targetInfo, strategyTarget, isMax);
					break;
				case EDebateStrategyTargetObjectType.Node:
					this.ShowStrategyTargetNode(targetInfo, strategyTarget, isMax);
					break;
				case EDebateStrategyTargetObjectType.PawnGrade:
					this.ShowStrategyTargetPawnGrade(targetInfo, strategyTarget);
					break;
				case EDebateStrategyTargetObjectType.StrategyCard:
					this.ShowStrategyTargetStrategyCard(targetInfo, strategyTarget, isMax);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060085DE RID: 34270 RVA: 0x003E4F0C File Offset: 0x003E310C
		private StrategyTarget GetSelectedStrategyTarget(int index)
		{
			return this._selectedStrategyTargetList.CheckIndex(index) ? this._selectedStrategyTargetList[index] : null;
		}

		// Token: 0x060085DF RID: 34271 RVA: 0x003E4F2B File Offset: 0x003E312B
		private List<IDebateSelectable> GetSelectedSelectableTarget(int index)
		{
			return this._selectedSelectableTargetList.CheckIndex(index) ? this._selectedSelectableTargetList[index] : null;
		}

		// Token: 0x060085E0 RID: 34272 RVA: 0x003E4F4C File Offset: 0x003E314C
		private bool CheckTargetIsAllMeet(out short[] lastShowingTargetInfo, out int curIndex, out bool isMax)
		{
			bool allIsMeet = true;
			lastShowingTargetInfo = null;
			curIndex = 0;
			isMax = false;
			bool targetTypeIsOr = false;
			bool targetTypeIsAnd = true;
			for (int index = 0; index < ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList.Count; index++)
			{
				short[] targetInfo = ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList[index];
				StrategyTarget selectedStrategyTarget = this.GetSelectedStrategyTarget(index);
				int curCount = (selectedStrategyTarget != null) ? selectedStrategyTarget.List.Count : 0;
				short minCount = targetInfo[1];
				short maxCount = targetInfo[2];
				bool isMeet = curCount >= (int)minCount && curCount <= (int)maxCount;
				isMax = (curCount == (int)maxCount);
				bool flag = isMeet;
				if (flag)
				{
					bool flag2 = targetTypeIsOr;
					if (flag2)
					{
						break;
					}
				}
				else
				{
					bool flag3 = lastShowingTargetInfo == null;
					if (flag3)
					{
						curIndex = index;
						lastShowingTargetInfo = targetInfo;
					}
					allIsMeet = false;
					bool flag4 = targetTypeIsAnd;
					if (flag4)
					{
						break;
					}
				}
			}
			bool flag5 = allIsMeet;
			if (flag5)
			{
				curIndex = ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList.Count - 1;
				lastShowingTargetInfo = ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList.Last<short[]>();
			}
			return allIsMeet;
		}

		// Token: 0x060085E1 RID: 34273 RVA: 0x003E507C File Offset: 0x003E327C
		private bool CheckTargetIsAllFixedCount()
		{
			foreach (short[] targetInfo in ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList)
			{
				short minCount = targetInfo[1];
				short maxCount = targetInfo[2];
				bool flag = minCount != maxCount;
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060085E2 RID: 34274 RVA: 0x003E5100 File Offset: 0x003E3300
		private void RefreshTargetCount(int curIndex)
		{
			string content = this.GetTargetCountText(curIndex);
			ConchShipCursor.Instance.ShowLifeSkillCombatUseStrategyTip(content);
		}

		// Token: 0x060085E3 RID: 34275 RVA: 0x003E5124 File Offset: 0x003E3324
		private bool GetNeedMergeTarget()
		{
			bool needMerge = ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList.Count > 0;
			bool flag = needMerge;
			if (flag)
			{
				short curTargetTemplateId = ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList[0][0];
				foreach (short[] target in ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList)
				{
					bool flag2 = curTargetTemplateId != target[0];
					if (flag2)
					{
						needMerge = false;
						break;
					}
				}
			}
			return needMerge;
		}

		// Token: 0x060085E4 RID: 34276 RVA: 0x003E51E4 File Offset: 0x003E33E4
		private string GetTargetCountText(int index)
		{
			bool needMerge = this.GetNeedMergeTarget();
			short[] targetInfo = ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList[index];
			StrategyTarget selectedStrategyTarget = this.GetSelectedStrategyTarget(index);
			bool flag = needMerge;
			int curCount;
			int minCount;
			int maxCount;
			if (flag)
			{
				curCount = this._selectedStrategyTargetList.Sum((StrategyTarget t) => t.List.Count);
				minCount = ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList.Sum((short[] t) => (int)t[1]);
				maxCount = ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList.Sum((short[] t) => (int)t[2]);
			}
			else
			{
				curCount = ((selectedStrategyTarget != null) ? selectedStrategyTarget.List.Count : 0);
				minCount = (int)targetInfo[1];
				maxCount = (int)targetInfo[2];
			}
			bool isMeet = curCount >= minCount && curCount <= maxCount;
			short targetTemplateId = targetInfo[0];
			DebateStrategyTargetItem targetConfig = DebateStrategyTarget.Instance[targetTemplateId];
			string color = isMeet ? "brightblue" : "brightred";
			return LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_TargetState, targetConfig.Name, curCount.ToString().SetColor(color), maxCount.ToString());
		}

		// Token: 0x060085E5 RID: 34277 RVA: 0x003E5354 File Offset: 0x003E3554
		private void ShowStrategyTargetPawn(short[] targetInfo, StrategyTarget strategyTarget, bool isMax)
		{
			ViewDebate.<>c__DisplayClass167_0 CS$<>8__locals1 = new ViewDebate.<>c__DisplayClass167_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.targetInfo = targetInfo;
			CS$<>8__locals1.needMerge = this.GetNeedMergeTarget();
			short targetTemplateId = CS$<>8__locals1.targetInfo[0];
			CS$<>8__locals1.targetConfig = DebateStrategyTarget.Instance[targetTemplateId];
			ViewDebate.<>c__DisplayClass167_0 CS$<>8__locals2 = CS$<>8__locals1;
			int maxCount;
			if (!CS$<>8__locals1.needMerge)
			{
				maxCount = (int)CS$<>8__locals1.targetInfo[2];
			}
			else
			{
				maxCount = ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList.Sum((short[] t) => (int)t[2]);
			}
			CS$<>8__locals2.maxCount = maxCount;
			CS$<>8__locals1.isAttach = (ViewDebate.FocusingCardItem.CardConfig.MarkType == EDebateStrategyMarkType.Attach);
			CS$<>8__locals1.isRepeatable = (targetTemplateId == 0 || targetTemplateId == 2 || targetTemplateId == 4);
			strategyTarget.List.RemoveAll((ulong id) => CS$<>8__locals1.<>4__this.debateGrid.FindUnit((int)id) == null);
			strategyTarget.List.Sort(delegate(ulong a, ulong b)
			{
				DebateUnit unitA = CS$<>8__locals1.<>4__this.debateGrid.FindUnit((int)a);
				DebateUnit unitB = CS$<>8__locals1.<>4__this.debateGrid.FindUnit((int)b);
				return unitA.Position.y.CompareTo(unitB.Position.y);
			});
			using (List<ulong>.Enumerator enumerator = strategyTarget.List.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ViewDebate.<>c__DisplayClass167_1 CS$<>8__locals3 = new ViewDebate.<>c__DisplayClass167_1();
					CS$<>8__locals3.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals3.id = enumerator.Current;
					DebateUnit unit = this.debateGrid.FindUnit((int)CS$<>8__locals3.id);
					this._highlightCardTargetStack.Push(new ValueTuple<IDebateSelectable, Transform, int, Action>(unit, unit.RectTrans.parent, unit.RectTrans.GetSiblingIndex(), delegate()
					{
						unit.ShowStrategyTargetMark(false, null);
					}));
					unit.RectTrans.SetParent(this.highlightRoot);
					bool isSelected2 = this.IsSelected(CS$<>8__locals3.id, CS$<>8__locals3.CS$<>8__locals1.targetConfig.ObjectType);
					bool canSelect = !isMax || isSelected2;
					int curSelectedCount = this._unitSelectionStack.Count((DebateUnit u) => u == unit);
					Func<DebateUnit, bool> <>9__6;
					unit.ShowStrategyTargetMark(canSelect, delegate(bool isSelected)
					{
						bool isAttach = CS$<>8__locals3.CS$<>8__locals1.isAttach;
						if (isAttach)
						{
							IEnumerable<DebateUnit> unitSelectionStack = CS$<>8__locals3.CS$<>8__locals1.<>4__this._unitSelectionStack;
							Func<DebateUnit, bool> predicate;
							if ((predicate = <>9__6) == null)
							{
								predicate = (<>9__6 = ((DebateUnit u) => u == unit));
							}
							int curSelectedCount2 = unitSelectionStack.Count(predicate);
							bool canAdd = unit.CurStrategyCount + curSelectedCount2 < unit.MaxStrategyCount && CS$<>8__locals3.CS$<>8__locals1.<>4__this._unitSelectionStack.Count < CS$<>8__locals3.CS$<>8__locals1.maxCount;
							bool flag = canAdd && ((!CS$<>8__locals3.CS$<>8__locals1.needMerge | CS$<>8__locals3.CS$<>8__locals1.isRepeatable) || curSelectedCount2 < 1);
							if (flag)
							{
								CS$<>8__locals3.CS$<>8__locals1.<>4__this.AddRepeatUnitSelection(CS$<>8__locals3.CS$<>8__locals1.targetInfo, unit);
							}
							unit.SetSelected(true);
						}
						else
						{
							bool flag2 = isSelected && !CS$<>8__locals3.CS$<>8__locals1.<>4__this.IsSelected(CS$<>8__locals3.id, CS$<>8__locals3.CS$<>8__locals1.targetConfig.ObjectType);
							if (flag2)
							{
								CS$<>8__locals3.CS$<>8__locals1.<>4__this.AddSelection(CS$<>8__locals3.CS$<>8__locals1.targetInfo, CS$<>8__locals3.id, unit);
							}
							else
							{
								CS$<>8__locals3.CS$<>8__locals1.<>4__this.RemoveSelection(CS$<>8__locals3.id, unit);
							}
						}
						CS$<>8__locals3.CS$<>8__locals1.<>4__this.CheckTarget();
					}, curSelectedCount);
				}
			}
		}

		// Token: 0x060085E6 RID: 34278 RVA: 0x003E55A8 File Offset: 0x003E37A8
		private void AddRepeatUnitSelection(short[] targetInfo, DebateUnit unit)
		{
			this._unitSelectionStack.Push(unit);
			this.AddSelection(targetInfo, (ulong)((long)unit.Pawn.Id), unit);
			int curSelectedCount = this._unitSelectionStack.Count((DebateUnit u) => u == unit);
			unit.RefreshStrategy(curSelectedCount, false, true);
		}

		// Token: 0x060085E7 RID: 34279 RVA: 0x003E561C File Offset: 0x003E381C
		private void RemoveRepeatUnitSelection()
		{
			DebateUnit unit = this._unitSelectionStack.Pop();
			this.RemoveSelection((ulong)((long)unit.Pawn.Id), unit);
			int curSelectedCount = this._unitSelectionStack.Count((DebateUnit u) => u == unit);
			unit.RefreshStrategy(curSelectedCount, false, true);
			unit.SetSelected(curSelectedCount > 0);
		}

		// Token: 0x060085E8 RID: 34280 RVA: 0x003E5698 File Offset: 0x003E3898
		private void ShowStrategyTargetNode(short[] targetInfo, StrategyTarget strategyTarget, bool isMax)
		{
			ViewDebate.<>c__DisplayClass170_0 CS$<>8__locals1 = new ViewDebate.<>c__DisplayClass170_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.targetInfo = targetInfo;
			short targetTemplateId = CS$<>8__locals1.targetInfo[0];
			CS$<>8__locals1.targetConfig = DebateStrategyTarget.Instance[targetTemplateId];
			strategyTarget.List.Sort(delegate(ulong id1, ulong id2)
			{
				DebateBlock block = CS$<>8__locals1.<>4__this.GetBlockByPos((IntPair)id1);
				DebateBlock block2 = CS$<>8__locals1.<>4__this.GetBlockByPos((IntPair)id2);
				return block.Position.y.CompareTo(block2.Position.y);
			});
			using (List<ulong>.Enumerator enumerator = strategyTarget.List.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ViewDebate.<>c__DisplayClass170_1 CS$<>8__locals2 = new ViewDebate.<>c__DisplayClass170_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.id = enumerator.Current;
					DebateBlock block = this.GetBlockByPos((IntPair)CS$<>8__locals2.id);
					this._highlightCardTargetStack.Push(new ValueTuple<IDebateSelectable, Transform, int, Action>(block, block.RectTrans.parent, block.RectTrans.GetSiblingIndex(), delegate()
					{
						block.ShowStrategyTargetMark(false, null);
					}));
					block.RectTrans.SetParent(this.highlightRoot);
					bool isSelected2 = this.IsSelected(CS$<>8__locals2.id, CS$<>8__locals2.CS$<>8__locals1.targetConfig.ObjectType);
					bool canSelect = !isMax || isSelected2;
					block.ShowStrategyTargetMark(canSelect, delegate(bool isSelected)
					{
						bool flag = isSelected && !CS$<>8__locals2.CS$<>8__locals1.<>4__this.IsSelected(CS$<>8__locals2.id, CS$<>8__locals2.CS$<>8__locals1.targetConfig.ObjectType);
						if (flag)
						{
							CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSelection(CS$<>8__locals2.CS$<>8__locals1.targetInfo, CS$<>8__locals2.id, block);
						}
						else
						{
							CS$<>8__locals2.CS$<>8__locals1.<>4__this.RemoveSelection(CS$<>8__locals2.id, block);
						}
						CS$<>8__locals2.CS$<>8__locals1.<>4__this.CheckTarget();
					});
				}
			}
		}

		// Token: 0x060085E9 RID: 34281 RVA: 0x003E5830 File Offset: 0x003E3A30
		private void ShowStrategyTargetPawnGrade(short[] targetInfo, StrategyTarget strategyTarget)
		{
			int index = ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList.IndexOf(targetInfo);
			StrategyTarget nodeTarget = this._selectedStrategyTargetList.Find(delegate(StrategyTarget t)
			{
				bool result;
				if (t.Type == EDebateStrategyTargetObjectType.Node)
				{
					List<ulong> list = t.List;
					result = (list != null && list.Count > 0);
				}
				else
				{
					result = false;
				}
				return result;
			});
			bool flag = nodeTarget == null;
			if (flag)
			{
				throw new Exception("not found block to select unit grade");
			}
			DebateBlock block = this.GetBlockByPos((IntPair)nodeTarget.List.First<ulong>());
			this.ShowCreateUnitArea(block, delegate(int grade)
			{
				this.HideCreateUnitArea(false);
				StrategyTarget target = this.GetSelectedStrategyTarget(index);
				target.List.Add((ulong)((long)grade));
				this.TaiwuUseCard();
			});
			ConchShipCursor.Instance.SetSelectCountCur(1, "brightblue");
		}

		// Token: 0x060085EA RID: 34282 RVA: 0x003E58E8 File Offset: 0x003E3AE8
		private void ShowStrategyTargetStrategyCard(short[] targetInfo, StrategyTarget strategyTarget, bool isMax)
		{
			ViewDebate.<>c__DisplayClass172_0 CS$<>8__locals1 = new ViewDebate.<>c__DisplayClass172_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.targetInfo = targetInfo;
			short targetTemplateId = CS$<>8__locals1.targetInfo[0];
			CS$<>8__locals1.targetConfig = DebateStrategyTarget.Instance[targetTemplateId];
			this.cardArea.selectedCardHasScale = false;
			using (List<ulong>.Enumerator enumerator = strategyTarget.List.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ViewDebate.<>c__DisplayClass172_1 CS$<>8__locals2 = new ViewDebate.<>c__DisplayClass172_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.id = enumerator.Current;
					int index = (int)CS$<>8__locals2.id;
					bool flag = index == ViewDebate.FocusingCardItem.CardView.Index;
					if (!flag)
					{
						DebateCardView cardView = this.cardArea.CardList[index].CardView;
						this._highlightCardTargetStack.Push(new ValueTuple<IDebateSelectable, Transform, int, Action>(cardView, cardView.transform.parent, cardView.transform.GetSiblingIndex(), delegate()
						{
							cardView.ShowStrategyTargetMark(false, null);
							cardView.SetEnabled(true, false);
						}));
						cardView.transform.SetParent(this.highlightRoot);
						bool isSelected2 = this.IsSelected(CS$<>8__locals2.id, CS$<>8__locals2.CS$<>8__locals1.targetConfig.ObjectType);
						bool canSelect = !isMax || isSelected2;
						cardView.ShowStrategyTargetMark(canSelect, delegate(bool isSelected)
						{
							bool flag2 = isSelected && !CS$<>8__locals2.CS$<>8__locals1.<>4__this.IsSelected(CS$<>8__locals2.id, CS$<>8__locals2.CS$<>8__locals1.targetConfig.ObjectType);
							if (flag2)
							{
								CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSelection(CS$<>8__locals2.CS$<>8__locals1.targetInfo, CS$<>8__locals2.id, cardView);
							}
							else
							{
								CS$<>8__locals2.CS$<>8__locals1.<>4__this.RemoveSelection(CS$<>8__locals2.id, cardView);
							}
							cardView.SetSelected(isSelected);
							CS$<>8__locals2.CS$<>8__locals1.<>4__this.CheckTarget();
						});
					}
				}
			}
		}

		// Token: 0x060085EB RID: 34283 RVA: 0x003E5AA0 File Offset: 0x003E3CA0
		private DebateBlock GetBlockByPos(IntPair nodePos)
		{
			Vector2Int pos = new Vector2Int(nodePos.First, nodePos.Second);
			return this.debateGrid.FindBlock(pos);
		}

		// Token: 0x060085EC RID: 34284 RVA: 0x003E5AD4 File Offset: 0x003E3CD4
		private bool IsSelected(ulong id, EDebateStrategyTargetObjectType type)
		{
			return this._selectedStrategyTargetList.Exists((StrategyTarget t) => t.Type == type && t.List.Contains(id));
		}

		// Token: 0x060085ED RID: 34285 RVA: 0x003E5B14 File Offset: 0x003E3D14
		private void AddSelection(short[] targetInfo, ulong id, IDebateSelectable selectable)
		{
			int index = ViewDebate.FocusingCardItem.CardView.CardConfig.TargetList.IndexOf(targetInfo);
			List<ulong> selectedList = this.GetSelectedStrategyTarget(index).List;
			selectedList.Add(id);
			List<IDebateSelectable> selectedSelectableList = this.GetSelectedSelectableTarget(index);
			selectedSelectableList.Add(selectable);
		}

		// Token: 0x060085EE RID: 34286 RVA: 0x003E5B64 File Offset: 0x003E3D64
		private void RemoveSelection(ulong id, IDebateSelectable selectable)
		{
			foreach (StrategyTarget target in this._selectedStrategyTargetList)
			{
				target.List.Remove(id);
			}
			foreach (List<IDebateSelectable> target2 in this._selectedSelectableTargetList)
			{
				target2.Remove(selectable);
			}
		}

		// Token: 0x060085EF RID: 34287 RVA: 0x003E5C08 File Offset: 0x003E3E08
		private void UnselectAllSelectableTarget()
		{
			bool flag = ViewDebate.FocusingCardItem != null;
			if (flag)
			{
				for (int i = 0; i < this._selectedSelectableTargetList.Count; i++)
				{
					List<IDebateSelectable> list = this._selectedSelectableTargetList[i];
					for (int j = 0; j < list.Count; j++)
					{
						IDebateSelectable selectable = list[j];
						DebateBlock block = selectable as DebateBlock;
						bool flag2 = block != null;
						if (flag2)
						{
							block.OnClick();
						}
					}
				}
			}
		}

		// Token: 0x060085F0 RID: 34288 RVA: 0x003E5C92 File Offset: 0x003E3E92
		private void TaiwuUseCard()
		{
			this.HideAllStrategyTarget(true);
			this.ShowOperationMask(true);
			TaiwuDomainMethod.AsyncCall.DebateGameCastStrategy(this, ViewDebate.FocusingCardItem.CardView.Index, true, this._selectedStrategyTargetList, delegate(int offset, RawDataPool pool)
			{
				DebateGame debateGame = null;
				Serializer.Deserialize(pool, offset, ref debateGame);
				this.Model.SetDebateGame(debateGame);
				this.PlayOperation(0f, delegate
				{
					this.RefreshData(true);
					this.ShowOperationMask(false);
				});
			});
		}

		// Token: 0x060085F1 RID: 34289 RVA: 0x003E5CD0 File Offset: 0x003E3ED0
		private void RefreshCards()
		{
			this.cardArea.RefreshSelfCards(this.Model.DebateGame.PlayerLeft.CanUseCards, this.Model.IsTaiwuRound);
			this.cardArea.RefreshEnemyCards(this.Model.DebateGame.PlayerRight.CanUseCards);
			this.cardArea.RefreshCardGroup(this.Model.DebateGame.PlayerLeft, true);
			this.cardArea.RefreshCardGroup(this.Model.DebateGame.PlayerRight, false);
		}

		// Token: 0x060085F2 RID: 34290 RVA: 0x003E5D68 File Offset: 0x003E3F68
		private void SetTimeScale(float speed)
		{
			this.Model.SetSpeed(speed);
			this.buttonSpeedUp.interactable = (this.Model.SpeedIndex < CombatTimeScaleToggle.AvailableTimeScales.Length - 1);
			DisableStyleRoot component = this.buttonSpeedUp.GetComponent<DisableStyleRoot>();
			if (component != null)
			{
				component.SetStyleEffect(!this.buttonSpeedUp.interactable, false);
			}
			this.buttonSpeedDown.interactable = (this.Model.SpeedIndex > 0);
			DisableStyleRoot component2 = this.buttonSpeedDown.GetComponent<DisableStyleRoot>();
			if (component2 != null)
			{
				component2.SetStyleEffect(!this.buttonSpeedDown.interactable, false);
			}
			this.textSpeed.text = string.Format("X{0}", this.Model.Speed);
			this.Model.RefreshTimeScale();
			this.RefreshButtonInteractable();
			bool autoSaveDebateSpeed = this.SettingData.AutoSaveDebateSpeed;
			if (autoSaveDebateSpeed)
			{
				this.SettingData.DebateSpeed = speed;
			}
		}

		// Token: 0x060085F3 RID: 34291 RVA: 0x003E5E60 File Offset: 0x003E4060
		private void OnClickButtonSpeedUp()
		{
			int index = this.Model.SpeedIndex;
			index++;
			bool flag = index >= CombatTimeScaleToggle.AvailableTimeScales.Length;
			if (flag)
			{
				index = 0;
			}
			this.SetTimeScale(CombatTimeScaleToggle.AvailableTimeScales[index]);
		}

		// Token: 0x060085F4 RID: 34292 RVA: 0x003E5EA0 File Offset: 0x003E40A0
		private void OnClickButtonSpeedDown()
		{
			int index = this.Model.SpeedIndex;
			index--;
			bool flag = index < 0;
			if (flag)
			{
				index = CombatTimeScaleToggle.AvailableTimeScales.Length - 1;
			}
			this.SetTimeScale(CombatTimeScaleToggle.AvailableTimeScales[index]);
		}

		// Token: 0x060085F5 RID: 34293 RVA: 0x003E5EE0 File Offset: 0x003E40E0
		private void OnClickButtonAutoFight()
		{
			bool isGameOver = this.Model.IsGameOver;
			if (!isGameOver)
			{
				this.Model.IsAuto = !this.Model.IsAuto;
				this.UpdateAutoFightMark(this.Model.IsAuto);
				bool isAuto = this.Model.IsAuto;
				if (isAuto)
				{
					bool flag = this.buttonRoundEnd.interactable && this.Model.IsTaiwuRound && !this.Model.IsPlayingOperation;
					if (flag)
					{
						this.AutoAction();
					}
				}
				else
				{
					TaiwuDomainMethod.Call.DebateGameSetTaiwuAi(this.Element.GameDataListenerId, false);
				}
				this.RefreshButtonInteractable();
			}
		}

		// Token: 0x060085F6 RID: 34294 RVA: 0x003E5F92 File Offset: 0x003E4192
		private void StopAutoFight()
		{
			this.Model.IsAuto = false;
			this.UpdateAutoFightMark(this.Model.IsAuto);
		}

		// Token: 0x060085F7 RID: 34295 RVA: 0x003E5FB4 File Offset: 0x003E41B4
		private void UpdateAutoFightMark(bool autoCombat)
		{
			this.autoFightTipsOpen.SetActive(autoCombat);
			this.autoFightTipsClose.SetActive(!autoCombat);
		}

		// Token: 0x060085F8 RID: 34296 RVA: 0x003E5FD4 File Offset: 0x003E41D4
		private void EnterSelectRemovingCardMode(int count)
		{
			List<short> cards = this.Model.DebateGame.GetPlayerByPlayerIsTaiwu(true).CanUseCards;
			this.Model.IsRemovingCards = true;
			this.Model.RemovingCards.Clear();
			this.removeWarningText.text = LanguageKey.LK_LifeskillCombat_RemoveWarning.TrFormat(GlobalConfig.Instance.DebateMaxCanUseCards, count);
			this.RefreshSelectRemovingCardMode(count);
			this.cardArea.selectedCardHasScale = false;
			int i = 0;
			while (i < cards.Count && i < this.cardArea.CardList.Count)
			{
				int index = i;
				DebateCardView cardView = this.cardArea.CardList[index].CardView;
				this._highlightCardTargetStack.Push(new ValueTuple<IDebateSelectable, Transform, int, Action>(cardView, cardView.transform.parent, cardView.transform.GetSiblingIndex(), delegate()
				{
					cardView.ShowStrategyTargetMark(false, null);
					cardView.SetEnabled(true, false);
				}));
				cardView.transform.SetParent(this.highlightRoot);
				cardView.ShowStrategyTargetMark(true, delegate(bool isSelected)
				{
					bool flag = this.Model.RemovingCards.Contains(index);
					if (flag)
					{
						this.Model.RemovingCards.Remove(index);
					}
					else
					{
						this.Model.RemovingCards.Add(index);
					}
					cardView.SetSelected(isSelected);
					this.RefreshSelectRemovingCardMode(count);
				});
				i++;
			}
			this.removeWarning.SetActive(true);
			this.buttonConfirm.gameObject.SetActive(false);
			this.controlPanel.transform.SetParent(this.cardArea.transform);
			this.selectCardArea.gameObject.SetActive(true);
		}

		// Token: 0x060085F9 RID: 34297 RVA: 0x003E61A0 File Offset: 0x003E43A0
		private void RefreshSelectRemovingCardMode(int count)
		{
			bool flag = this.Model.RemovingCards.Count == count;
			if (flag)
			{
				this.ShowOperationMask(true);
				this.ExitSelectRemovingCardMode();
				TaiwuDomainMethod.AsyncCall.DebateGameRemoveCards(this, true, this.Model.RemovingCards, delegate(int offset, RawDataPool pool)
				{
					DebateGame debateGame = null;
					Serializer.Deserialize(pool, offset, ref debateGame);
					this.Model.SetDebateGame(debateGame);
					this.PlayOperation(0f, delegate
					{
						this.RefreshData(true);
						this.ShowOperationMask(false);
						this.OnClickButtonRoundEnd();
					});
				});
			}
			else
			{
				ConchShipCursor.Instance.ShowLifeSkillCombatUseStrategyTip(LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_TargetState, LanguageKey.LK_LifeSkillCombat_CardGroup_Used.Tr(), this.Model.RemovingCards.Count.ToString().SetColor("brightred"), count.ToString()));
			}
		}

		// Token: 0x060085FA RID: 34298 RVA: 0x003E6240 File Offset: 0x003E4440
		private void ExitSelectRemovingCardMode()
		{
			this.Model.IsRemovingCards = false;
			this.HideAllStrategyTarget(true);
			this.controlPanel.transform.SetParent(base.transform);
			ConchShipCursor.Instance.HideLifeSkillCombatUseStrategyTip();
			this.selectCardArea.gameObject.SetActive(false);
			this.removeWarning.SetActive(false);
		}

		// Token: 0x060085FB RID: 34299 RVA: 0x003E62A4 File Offset: 0x003E44A4
		private void OnClickButtonResetStrategy()
		{
			bool activeSelf = this.operationMask.activeSelf;
			if (!activeSelf)
			{
				this.operationMask.SetActive(true);
				TaiwuDomainMethod.AsyncCall.DebateGameResetCards(this, true, true, delegate(int offset, RawDataPool pool)
				{
					DebateGame debateGame = null;
					Serializer.Deserialize(pool, offset, ref debateGame);
					this.Model.SetDebateGame(debateGame);
					this.PlayOperation(0f, delegate
					{
						this.RefreshData(true);
						this.operationMask.SetActive(false);
					});
				});
			}
		}

		// Token: 0x060085FC RID: 34300 RVA: 0x003E62E8 File Offset: 0x003E44E8
		private void RefreshButtonResetStrategy()
		{
			DebatePlayer player = this.Model.DebateGame.GetPlayerByPlayerIsTaiwu(true);
			bool playerCanUseResetStrategy = this.Model.DebateGame.GetPlayerCanUseResetStrategy(true);
			if (playerCanUseResetStrategy)
			{
				this.buttonResetStrategy.interactable = true;
				this.buttonResetStrategy.gameObject.SetActive(true);
			}
			else
			{
				bool flag = player.OwnedCards.Count == 0 && player.UsedCards.Count != 0;
				if (flag)
				{
					this.buttonResetStrategy.interactable = false;
					this.buttonResetStrategy.gameObject.SetActive(true);
				}
				else
				{
					this.buttonResetStrategy.gameObject.SetActive(false);
				}
			}
			this.buttonResetStrategy.GetComponent<TooltipInvoker>().PresetParam = ((player.Pressure == player.MaxPressure) ? this.Model.ResetStrategyPresetTipContent[1] : this.Model.ResetStrategyPresetTipContent[0]);
		}

		// Token: 0x060085FD RID: 34301 RVA: 0x003E63E0 File Offset: 0x003E45E0
		public void OnEnterButtonResetStrategy()
		{
			DebatePlayer player = this.Model.DebateGame.GetPlayerByPlayerIsTaiwu(true);
			this.selfPlayer.ShowPreviewPressure(player.Pressure, player.MaxPressure, GlobalConfig.Instance.DebateResetCardsPressureDelta);
		}

		// Token: 0x060085FE RID: 34302 RVA: 0x003E6422 File Offset: 0x003E4622
		public void OnExitButtonResetStrategy()
		{
			this.selfPlayer.HidePreviewPressure();
		}

		// Token: 0x060085FF RID: 34303 RVA: 0x003E6434 File Offset: 0x003E4634
		private void RefreshData(bool hasAnim = true)
		{
			this.debateGrid.RefreshAllUnit(true, true);
			this.debateGrid.RefreshAllBlock();
			this.selfPlayer.Refresh(this.Model.DebateGame.PlayerLeft, hasAnim);
			this.enemyPlayer.Refresh(this.Model.DebateGame.PlayerRight, hasAnim);
			this.RefreshCards();
			this.RefreshButtonResetStrategy();
		}

		// Token: 0x06008600 RID: 34304 RVA: 0x003E64A4 File Offset: 0x003E46A4
		private void LogStepInfo(string message, bool appendAdversaryInfo = true)
		{
			string adversaryInfo = string.Empty;
			if (appendAdversaryInfo)
			{
				adversaryInfo = string.Format("[enemy character {0}] | ", this.Model.EnemyCharId);
			}
			GLog.TagLog("LifeSkillCombat", string.Format("{0}{1:MM/dd-HH:mm:ss} | {2}", adversaryInfo, DateTime.Now, message), Array.Empty<object>());
		}

		// Token: 0x06008601 RID: 34305 RVA: 0x003E64FE File Offset: 0x003E46FE
		private void SetButtonInteractable(CButton button, bool interactable)
		{
			button.interactable = interactable;
			HSVStyleRoot component = button.GetComponent<HSVStyleRoot>();
			if (component != null)
			{
				component.SetInteractable(interactable);
			}
		}

		// Token: 0x06008602 RID: 34306 RVA: 0x003E651C File Offset: 0x003E471C
		private void ShowCombatLifeSkillHiddenInfo(ArgumentBox argumentBox)
		{
			this.debateGrid.RefreshAllUnit(true, true);
			this.enemyPlayer.Refresh(this.Model.DebateGame.PlayerRight, false);
		}

		// Token: 0x06008603 RID: 34307 RVA: 0x003E654C File Offset: 0x003E474C
		private void ShowCombatLifeSkillTalk(ArgumentBox argumentBox)
		{
			short id;
			argumentBox.Get("Id", out id);
			bool isTaiwu;
			bool flag = !argumentBox.Get("IsTaiwu", out isTaiwu);
			if (flag)
			{
				isTaiwu = true;
			}
			DebatePlayer player = isTaiwu ? this.selfPlayer : this.enemyPlayer;
			player.Speak(id, 4f);
		}

		// Token: 0x06008604 RID: 34308 RVA: 0x003E659D File Offset: 0x003E479D
		private void ShowOperationMask(bool show)
		{
			this.operationMask.gameObject.SetActive(show);
		}

		// Token: 0x06008605 RID: 34309 RVA: 0x003E65B1 File Offset: 0x003E47B1
		private short GetPlaySpeakConfigOnGameOver(bool isWin)
		{
			return isWin ? 19 : 18;
		}

		// Token: 0x06008606 RID: 34310 RVA: 0x003E65BC File Offset: 0x003E47BC
		private Transform GetCardGroupTransform(int location)
		{
			return (location == 5) ? this.enemyPlayer.RectTrans : this.cardArea.GetCardGroupTransform(location);
		}

		// Token: 0x06008607 RID: 34311 RVA: 0x003E65EC File Offset: 0x003E47EC
		private void ShowCreateUnitArea(DebateBlock block, Action<int> onSelectGradeForStrategy = null)
		{
			this.unitGradeAreaMask.gameObject.SetActive(true);
			this.unitGradeArea.Refresh(block, onSelectGradeForStrategy, delegate(int cost)
			{
				this.selfPlayer.PreviewEnergy(this.selfPlayer.Energy - cost, false);
			}, new Action<Vector2Int, sbyte>(this.CreateUnit));
			bool flag = !this.unitGradeArea.gameObject.activeSelf;
			if (flag)
			{
				this.unitGradeArea.gameObject.SetActive(true);
			}
			AudioManager.Instance.PlaySound(ViewDebate.SoundShowCreateUnitPanel, false, true);
		}

		// Token: 0x06008608 RID: 34312 RVA: 0x003E6670 File Offset: 0x003E4870
		private void HideCreateUnitArea(bool unselect = true)
		{
			this.unitGradeAreaMask.gameObject.SetActive(false);
			bool activeSelf = this.unitGradeArea.gameObject.activeSelf;
			if (activeSelf)
			{
				this.unitGradeArea.gameObject.SetActive(false);
			}
			if (unselect)
			{
				this.UnselectAllSelectableTarget();
			}
		}

		// Token: 0x0400668B RID: 26251
		[Header("选手")]
		[SerializeField]
		private DebatePlayer selfPlayer;

		// Token: 0x0400668C RID: 26252
		[SerializeField]
		private DebatePlayer enemyPlayer;

		// Token: 0x0400668D RID: 26253
		[SerializeField]
		private Game.Views.Debate.DebateRecord selfRecord;

		// Token: 0x0400668E RID: 26254
		[SerializeField]
		private Game.Views.Debate.DebateRecord enemyRecord;

		// Token: 0x0400668F RID: 26255
		[SerializeField]
		private CButton btnOpenCharMenu;

		// Token: 0x04006690 RID: 26256
		[Header("棋盘棋子")]
		[SerializeField]
		private DebateGrid debateGrid;

		// Token: 0x04006691 RID: 26257
		[SerializeField]
		private CButton unitGradeAreaMask;

		// Token: 0x04006692 RID: 26258
		[SerializeField]
		private DebateUnitGradeArea unitGradeArea;

		// Token: 0x04006693 RID: 26259
		[SerializeField]
		private GameObject operationMask;

		// Token: 0x04006694 RID: 26260
		[SerializeField]
		private RectTransform highlightRoot;

		// Token: 0x04006695 RID: 26261
		[Header("控制")]
		[SerializeField]
		private GameObject controlPanel;

		// Token: 0x04006696 RID: 26262
		[SerializeField]
		private CButton buttonSpeedDown;

		// Token: 0x04006697 RID: 26263
		[SerializeField]
		private CButton buttonSpeedUp;

		// Token: 0x04006698 RID: 26264
		[SerializeField]
		private TextMeshProUGUI textSpeed;

		// Token: 0x04006699 RID: 26265
		[SerializeField]
		private CButton buttonAutoFight;

		// Token: 0x0400669A RID: 26266
		[SerializeField]
		private GameObject autoFightTipsClose;

		// Token: 0x0400669B RID: 26267
		[SerializeField]
		private GameObject autoFightTipsOpen;

		// Token: 0x0400669C RID: 26268
		[SerializeField]
		private CButton buttonRoundEnd;

		// Token: 0x0400669D RID: 26269
		[SerializeField]
		private CButton buttonRoundResult;

		// Token: 0x0400669E RID: 26270
		[SerializeField]
		private CButton buttonRoundEnemy;

		// Token: 0x0400669F RID: 26271
		[SerializeField]
		private CButton buttonForceGiveUp;

		// Token: 0x040066A0 RID: 26272
		[SerializeField]
		private CButton buttonGiveUp;

		// Token: 0x040066A1 RID: 26273
		[SerializeField]
		private CButton buttonResetStrategy;

		// Token: 0x040066A2 RID: 26274
		[Header("回合")]
		[SerializeField]
		private TextMeshProUGUI textLifeSkillTypeName;

		// Token: 0x040066A3 RID: 26275
		[SerializeField]
		private TextMeshProUGUI textRoundName;

		// Token: 0x040066A4 RID: 26276
		[SerializeField]
		private ImageNumber imageNumberCurRound;

		// Token: 0x040066A5 RID: 26277
		[SerializeField]
		private ImageNumber imageNumberMaxRound;

		// Token: 0x040066A6 RID: 26278
		[SerializeField]
		private TooltipInvoker stageTip;

		// Token: 0x040066A7 RID: 26279
		[SerializeField]
		private CanvasGroup roundTransitionGroup;

		// Token: 0x040066A8 RID: 26280
		[SerializeField]
		private GameObject roundTransitionSelf;

		// Token: 0x040066A9 RID: 26281
		[SerializeField]
		private GameObject roundTransitionEnemy;

		// Token: 0x040066AA RID: 26282
		[SerializeField]
		private GameObject roundTransitionResult;

		// Token: 0x040066AB RID: 26283
		[Header("卡牌、策略")]
		[SerializeField]
		private DebateCardArea cardArea;

		// Token: 0x040066AC RID: 26284
		[SerializeField]
		private DebateCardView selfResetCardView;

		// Token: 0x040066AD RID: 26285
		[SerializeField]
		private DebateCardView enemyResetCardView;

		// Token: 0x040066AE RID: 26286
		[SerializeField]
		private DebateCardView enemyUsingCardView;

		// Token: 0x040066AF RID: 26287
		[SerializeField]
		private GameObject selectCardArea;

		// Token: 0x040066B0 RID: 26288
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x040066B1 RID: 26289
		[SerializeField]
		private GameObject removeWarning;

		// Token: 0x040066B2 RID: 26290
		[SerializeField]
		private TextMeshProUGUI removeWarningText;

		// Token: 0x040066B3 RID: 26291
		[SerializeField]
		private GameObject leftCardAnimControlPoint;

		// Token: 0x040066B4 RID: 26292
		[SerializeField]
		private GameObject rightCardAnimControlPoint;

		// Token: 0x040066B5 RID: 26293
		[SerializeField]
		private GameObject enemyUsingCardPoint;

		// Token: 0x040066B6 RID: 26294
		[Header("特效")]
		[SerializeField]
		private CanvasGroup halfRoundEffect;

		// Token: 0x040066B7 RID: 26295
		[SerializeField]
		private UIParticle roundEndEffect;

		// Token: 0x040066B8 RID: 26296
		[SerializeField]
		private GameObject castStrategyFailedEffect;

		// Token: 0x040066B9 RID: 26297
		[SerializeField]
		private GameObject makeMoveFailedEffect;

		// Token: 0x040066BA RID: 26298
		[SerializeField]
		private RectTransform flagsRoot;

		// Token: 0x040066BB RID: 26299
		[SerializeField]
		private EffectPlayer effectPlayer;

		// Token: 0x040066BC RID: 26300
		public const string EffectConflictTaiwuWin = "eff_lifeskillcombat_ui_pen_lanqiang";

		// Token: 0x040066BD RID: 26301
		public const string EffectConflictEnemyWin = "eff_lifeskillcombat_ui_pen_hongqiang";

		// Token: 0x040066BE RID: 26302
		public const string EffectConflictDraw = "eff_lifeskillcombat_ui_pen_yiyangqiang";

		// Token: 0x040066BF RID: 26303
		public const string EffectConflictDestroyTaiwu = "eff_lifeskillcombat_ui_pen_posui1";

		// Token: 0x040066C0 RID: 26304
		public const string EffectConflictDestroyEnemy = "eff_lifeskillcombat_ui_pen_posui2";

		// Token: 0x040066C1 RID: 26305
		public const string EffectArriveEndEnemy = "eff_lifeskillcombat_ui_dx_hualizi2";

		// Token: 0x040066C2 RID: 26306
		public const string EffectArriveEndTaiwu = "eff_lifeskillcombat_ui_dx_hualizi";

		// Token: 0x040066C3 RID: 26307
		public const string EffectStrategyDeleteTaiwuPawn = "EffectStrategyDeleteTaiwuPawn";

		// Token: 0x040066C4 RID: 26308
		public const string EffectStrategyDeleteEnemyPawn = "EffectStrategyDeleteEnemyPawn";

		// Token: 0x040066C5 RID: 26309
		public const string EffectSelfStrategyTargetPawn = "eff_lifeskillcombat_ui_dxkp_shan_lan";

		// Token: 0x040066C6 RID: 26310
		public const string EffectEnemyStrategyTargetPawn = "eff_lifeskillcombat_ui_dxkp_shan_hong";

		// Token: 0x040066C7 RID: 26311
		public static readonly string BGM = "Mu_art_jiaoyi_3";

		// Token: 0x040066C8 RID: 26312
		public static readonly string SoundConflict = "art_battle";

		// Token: 0x040066C9 RID: 26313
		public static readonly string SoundShowCreateUnitPanel = "art_level";

		// Token: 0x040066CA RID: 26314
		public static readonly string SoundHoverCreateUnitPanel = "art_ClickHover";

		// Token: 0x040066CB RID: 26315
		public static readonly string SoundCreateUnit = "art_generate";

		// Token: 0x040066CC RID: 26316
		public static readonly string SoundMoveUnit = "art_ChessMove";

		// Token: 0x040066CD RID: 26317
		public static readonly string SoundProtectUnit = "art_protect";

		// Token: 0x040066CE RID: 26318
		public static readonly string SoundStrategyDeleteUnit = "art_prohibit";

		// Token: 0x040066CF RID: 26319
		public static readonly string SoundReveal = "art_generate";

		// Token: 0x040066D0 RID: 26320
		public static readonly string SoundMoveCard = "art_CardMove";

		// Token: 0x040066D1 RID: 26321
		public static readonly string SoundUseCard = "art_use";

		// Token: 0x040066D2 RID: 26322
		public static readonly string SoundAddUnitStrategy = "ui_art_card";

		// Token: 0x040066D3 RID: 26323
		public static readonly string SoundRemoveUnitStrategy = "art_remove";

		// Token: 0x040066D4 RID: 26324
		public static readonly string SoundUnitArrive = "art_ChessHurt";

		// Token: 0x040066D5 RID: 26325
		public static readonly string SoundAddScore = "art_add";

		// Token: 0x040066D6 RID: 26326
		public static readonly string SoundRemoveScore = "art_hurt";

		// Token: 0x040066D7 RID: 26327
		public static readonly string SoundAddStress = "art_stressed";

		// Token: 0x040066D8 RID: 26328
		public static readonly string SoundStressReduceScore = "art_burn";

		// Token: 0x040066D9 RID: 26329
		public static readonly string SoundOpenCardGroup = "art_OpenCard";

		// Token: 0x040066DA RID: 26330
		public static readonly string SoundStage1 = "ui_art_round_1";

		// Token: 0x040066DB RID: 26331
		public static readonly string SoundStage2 = "ui_art_round_2";

		// Token: 0x040066DC RID: 26332
		public static readonly string SoundStage3 = "art_EndDrum";

		// Token: 0x040066DD RID: 26333
		public static readonly string SoundStressEffect = "art_debuff";

		// Token: 0x040066DE RID: 26334
		public static readonly string SoundChangeFlag = "art_FlagChange";

		// Token: 0x040066DF RID: 26335
		public static readonly string SoundAddNodeEffect = "art_grid";

		// Token: 0x040066E0 RID: 26336
		public static readonly string SoundUseCardFailed = "art_burn";

		// Token: 0x040066E1 RID: 26337
		public static readonly string SoundCreateUnitFailed = "art_burn";

		// Token: 0x040066E2 RID: 26338
		public static readonly string SoundStressUp = "art_PressureUp";

		// Token: 0x040066E3 RID: 26339
		public static readonly string SoundStressDown = "art_PressureDown";

		// Token: 0x040066E4 RID: 26340
		public static readonly string SoundHalfRound = "art_BackgroundFire";

		// Token: 0x040066E5 RID: 26341
		public static readonly string SoundUnitPowerUp = "art_PointUp";

		// Token: 0x040066E6 RID: 26342
		public static readonly string SoundUnitPowerDown = "art_PointDown";

		// Token: 0x040066E7 RID: 26343
		public static readonly string SoundCardFly = "art_CardFly";

		// Token: 0x040066E8 RID: 26344
		private bool _flagsNeedUpdate;

		// Token: 0x040066E9 RID: 26345
		private List<int> _allAudienceList = new List<int>();

		// Token: 0x040066EB RID: 26347
		private bool _encyclopediaCausedPause = false;

		// Token: 0x040066EC RID: 26348
		private bool _encyclopediaCausedCloseAuto = false;

		// Token: 0x040066ED RID: 26349
		[TupleElementNames(new string[]
		{
			"self",
			"parent",
			"childIndex",
			"onCancel"
		})]
		private readonly Stack<ValueTuple<IDebateSelectable, Transform, int, Action>> _highlightCardTargetStack = new Stack<ValueTuple<IDebateSelectable, Transform, int, Action>>();

		// Token: 0x040066EE RID: 26350
		private readonly List<StrategyTarget> _selectedStrategyTargetList = new List<StrategyTarget>();

		// Token: 0x040066EF RID: 26351
		private readonly Stack<DebateUnit> _unitSelectionStack = new Stack<DebateUnit>();

		// Token: 0x040066F0 RID: 26352
		private readonly List<List<IDebateSelectable>> _selectedSelectableTargetList = new List<List<IDebateSelectable>>();
	}
}
