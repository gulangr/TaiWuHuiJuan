using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Debate;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.LifeSkillCombat
{
	// Token: 0x02000989 RID: 2441
	public class ViewLifeSkillCombatBegin : UIBase
	{
		// Token: 0x17000D36 RID: 3382
		// (get) Token: 0x0600752E RID: 29998 RVA: 0x00369784 File Offset: 0x00367984
		private LifeSkillCombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<LifeSkillCombatModel>();
			}
		}

		// Token: 0x17000D37 RID: 3383
		// (get) Token: 0x0600752F RID: 29999 RVA: 0x0036978B File Offset: 0x0036798B
		private GlobalSettings Settings
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>();
			}
		}

		// Token: 0x17000D38 RID: 3384
		// (get) Token: 0x06007530 RID: 30000 RVA: 0x00369792 File Offset: 0x00367992
		private bool IsTaiwuFirst
		{
			get
			{
				return !this._isGivenUp && (this._forceSilentSucceed || this._wisdomSelf > this._wisdomAdversary || (this._wisdomSelf == this._wisdomAdversary && this._isTaiwuFirstByLuck > 0));
			}
		}

		// Token: 0x17000D39 RID: 3385
		// (get) Token: 0x06007531 RID: 30001 RVA: 0x003697D2 File Offset: 0x003679D2
		public bool HasForceGiveIn
		{
			get
			{
				return this._hasForceGiveIn;
			}
		}

		// Token: 0x17000D3A RID: 3386
		// (get) Token: 0x06007532 RID: 30002 RVA: 0x003697DA File Offset: 0x003679DA
		public int AvailableSpectatorCount
		{
			get
			{
				List<int> availableSpectatorIds = this._availableSpectatorIds;
				return (availableSpectatorIds != null) ? availableSpectatorIds.Count : 0;
			}
		}

		// Token: 0x06007533 RID: 30003 RVA: 0x003697F0 File Offset: 0x003679F0
		public bool ShouldDisableQuickSelectButton(List<CharacterDisplayData> teammateDisplayDatas)
		{
			bool hideTaiwuAudience = this.Model.HideTaiwuAudience;
			bool result;
			if (hideTaiwuAudience)
			{
				result = true;
			}
			else
			{
				int num;
				if (teammateDisplayDatas == null)
				{
					num = 0;
				}
				else
				{
					num = teammateDisplayDatas.Count((CharacterDisplayData d) => d != null);
				}
				int arrangedCount = num;
				bool flag = this.AvailableSpectatorCount <= 3 && arrangedCount >= this.AvailableSpectatorCount;
				result = flag;
			}
			return result;
		}

		// Token: 0x17000D3B RID: 3387
		// (get) Token: 0x06007534 RID: 30004 RVA: 0x00369862 File Offset: 0x00367A62
		private LifeSkillShorts SelfAttainments
		{
			get
			{
				return this._combatBeginDisplayData.SelfAttainments;
			}
		}

		// Token: 0x17000D3C RID: 3388
		// (get) Token: 0x06007535 RID: 30005 RVA: 0x0036986F File Offset: 0x00367A6F
		private LifeSkillShorts EnemyAttainments
		{
			get
			{
				return this._combatBeginDisplayData.EnemyAttainments;
			}
		}

		// Token: 0x06007536 RID: 30006 RVA: 0x0036987C File Offset: 0x00367A7C
		public override void OnInit(ArgumentBox argsBox)
		{
			this._wisdomCost = 1;
			this._wisdomCostNpc = 1;
			this._hasForceGiveIn = false;
			this._isInited = false;
			this.startBtn.enabled = false;
			this._spectatorsInitialized = false;
			this.briberyBtn.interactable = true;
			this.ParseArguments(argsBox);
			this.RequestData();
			this.ani1.gameObject.SetActive(false);
			this.ani2.gameObject.SetActive(false);
			YieldHelper yieldHelper = SingletonObject.getInstance<YieldHelper>();
			bool flag = this._animRoutine != null;
			if (flag)
			{
				yieldHelper.StopYield(this._animRoutine);
			}
			this._animRoutine = yieldHelper.StartYield(this.<OnInit>g__AnimRoutine|78_0());
			UIElement element = this.Element;
			element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(this.PlayBeginAni));
		}

		// Token: 0x06007537 RID: 30007 RVA: 0x00369958 File Offset: 0x00367B58
		private void Awake()
		{
			this.observeBtn.ClearAndAddListener(delegate
			{
				this.CurrentStage = ((this.CurrentStage == ELifeSkillCombatBeginStage.SelectSkillObserve) ? ELifeSkillCombatBeginStage.SelectSkillNormal : ELifeSkillCombatBeginStage.SelectSkillObserve);
			});
			this.giveUpBtn.ClearAndAddListener(delegate
			{
				this.SetTalkInfo(1, 3);
				this.GiveUpSelection();
			});
			this.briberyBtn.ClearAndAddListener(delegate
			{
				TaiwuEventDomainMethod.Call.OnLifeSkillCombatForceSilent(this._enemyCharId, 0, 0);
				this.briberyBtn.interactable = false;
			});
			this.startBtn.ClearAndAddListener(new Action(this.StartCombat));
			this.presetStrategyToggleGroup.Init(-1);
			this.strategyToggleGroup.Init();
			this.strategyToggleGroup.OnActiveIndexChange += this.OnStrategyToggleGroupActiveIndexChange;
			this.strategyToggleGroup.DeSelectAll(false);
			this.presetStrategyToggleGroup.OnActiveIndexChange += this.OnPresetStrategyToggleGroupActiveIndexChange;
			this.loopingBtn.ClearAndAddListener(delegate
			{
				TaiwuDomainMethod.AsyncCall.GetLoopingViewDisplayData(this, delegate(int offset, RawDataPool dataPool)
				{
					LoopingViewDisplayData displayData = null;
					Serializer.Deserialize(dataPool, offset, ref displayData);
					UIElement.Looping.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("LoopingViewDisplayData", displayData));
					UIManager.Instance.ShowUI(UIElement.Looping, true);
					UIElement looping = UIElement.Looping;
					looping.OnHide = (Action)Delegate.Combine(looping.OnHide, new Action(this.RequestCombatBeginDisplayData));
				});
			});
			this.readingBtn.ClearAndAddListener(delegate
			{
				UIManager.Instance.ShowUI(UIElement.Reading, true);
				UIElement reading = UIElement.Reading;
				reading.OnHide = (Action)Delegate.Combine(reading.OnHide, new Action(this.RequestCombatBeginDisplayData));
			});
			PointerTrigger loopingPointerTrigger = this.loopingBtn.GetComponent<PointerTrigger>();
			loopingPointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.loopingBtn.transform.GetChild(0).gameObject.SetActive(true);
			});
			loopingPointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.loopingBtn.transform.GetChild(0).gameObject.SetActive(false);
			});
			PointerTrigger readingPointerTrigger = this.readingBtn.GetComponent<PointerTrigger>();
			readingPointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.readingBtn.transform.GetChild(0).gameObject.SetActive(true);
			});
			readingPointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.readingBtn.transform.GetChild(0).gameObject.SetActive(false);
			});
		}

		// Token: 0x06007538 RID: 30008 RVA: 0x00369AC4 File Offset: 0x00367CC4
		private void OnEnable()
		{
			GEvent.Add(UiEvents.OnLifeSkillCombatForceSilentResult, new GEvent.Callback(this.OnForceSilentResult));
			GlobalDomainMethod.Call.InvokeGuidingTrigger(74);
		}

		// Token: 0x06007539 RID: 30009 RVA: 0x00369AEC File Offset: 0x00367CEC
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.OnLifeSkillCombatForceSilentResult, new GEvent.Callback(this.OnForceSilentResult));
			bool flag = this._animRoutine != null;
			if (flag)
			{
				YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
				if (instance != null)
				{
					instance.StopYield(this._animRoutine);
				}
				this._animRoutine = null;
			}
		}

		// Token: 0x17000D3D RID: 3389
		// (get) Token: 0x0600753A RID: 30010 RVA: 0x00369B43 File Offset: 0x00367D43
		// (set) Token: 0x0600753B RID: 30011 RVA: 0x00369B4B File Offset: 0x00367D4B
		public ELifeSkillCombatBeginStage CurrentStage
		{
			get
			{
				return this._currentStage;
			}
			set
			{
				this._currentStage = value;
				this.OnCurrentStageChanged();
			}
		}

		// Token: 0x0600753C RID: 30012 RVA: 0x00369B5C File Offset: 0x00367D5C
		private void OnCurrentStageChanged()
		{
			this.selectSkillHolder.gameObject.SetActive(this.CurrentStage != ELifeSkillCombatBeginStage.SelectStrategy);
			this.selectStrategyHolder.gameObject.SetActive(this.CurrentStage == ELifeSkillCombatBeginStage.SelectStrategy);
			this.startBtn.enabled = (this.CurrentStage == ELifeSkillCombatBeginStage.SelectStrategy);
			this.skillNameObj.SetActive(this.CurrentStage != ELifeSkillCombatBeginStage.SelectStrategy);
			this.selectSkillTextBg.SetActive(this.CurrentStage == ELifeSkillCombatBeginStage.SelectStrategy);
			bool flag = this.CurrentStage == ELifeSkillCombatBeginStage.SelectStrategy;
			if (flag)
			{
				this.selectSkillText.text = Config.LifeSkillType.Instance[this._selectedSkillType].Name;
			}
			TextMeshProUGUI textMeshProUGUI = this.notice;
			ELifeSkillCombatBeginStage currentStage = this.CurrentStage;
			if (!true)
			{
			}
			string text;
			if (currentStage != ELifeSkillCombatBeginStage.SelectStrategy)
			{
				text = LanguageKey.LK_LifeSkillBattle_Begin_BanSkillNotice.Tr();
			}
			else
			{
				text = LanguageKey.LK_LifeSkillBattle_Begin_SeletStrategyNotice.Tr();
			}
			if (!true)
			{
			}
			textMeshProUGUI.text = text;
			this.selectSkillHolderBg.SetSprite("ui9_back_life_skill_combat_select_skill_pattern_base_" + ((this.CurrentStage == ELifeSkillCombatBeginStage.SelectSkillObserve) ? 1 : 0).ToString(), false, null);
			switch (this.CurrentStage)
			{
			case ELifeSkillCombatBeginStage.SelectSkillNormal:
				this.observeBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = LanguageKey.UI_LifeSkillBattle_Prepare_Operation_Observe.Tr();
				this.SetObserveBtnInteractable();
				this.giveUpBtn.interactable = true;
				this.operateTitle.text = LanguageKey.UI_LifeSkillBattle_Prepare_SelectSkillType.Tr();
				this.RefreshCharacterItems();
				this.SetSelectSkillItems();
				break;
			case ELifeSkillCombatBeginStage.SelectSkillObserve:
				this.observeBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = LanguageKey.UI_LifeSkillBattle_Prepare_Operation_QuickObserve.Tr();
				this.SetObserveBtnInteractable();
				this.giveUpBtn.interactable = true;
				this.operateTitle.text = LanguageKey.UI_LifeSkillBattle_Prepare_InObserve.Tr();
				this.RefreshCharacterItems();
				this.SetSelectSkillItems();
				break;
			case ELifeSkillCombatBeginStage.SelectSkillAI:
				this.observeBtn.interactable = false;
				this.giveUpBtn.interactable = false;
				this.RefreshCharacterItems();
				this.SetSelectSkillItems();
				break;
			case ELifeSkillCombatBeginStage.SelectStrategy:
			{
				bool flag2 = !this._spectatorsInitialized;
				if (flag2)
				{
					this.InitSpectators(delegate
					{
						this.RefreshCharacterItems();
						this.ShowAttainment(this._selectedSkillType);
					});
					this._spectatorsInitialized = true;
				}
				else
				{
					this.RefreshCharacterItems();
					this.ShowAttainment(this._selectedSkillType);
				}
				this.operateTitle.text = LanguageKey.UI_LifeSkillBattle_Prepare_SelectStrategy.Tr();
				this.presetStrategyToggleGroup.Set(0, false);
				this.SetSelectStrategyItems();
				break;
			}
			}
		}

		// Token: 0x0600753D RID: 30013 RVA: 0x00369E08 File Offset: 0x00368008
		private void ParseArguments(ArgumentBox argsBox)
		{
			argsBox.Get("Self", out this._selfCharId);
			argsBox.Get("EnemyId", out this._enemyCharId);
			bool needSkillSelection;
			if (argsBox.Get("LifeSkillType", out this._selectedSkillType))
			{
				sbyte selectedSkillType = this._selectedSkillType;
				needSkillSelection = (selectedSkillType == -1 || selectedSkillType == 16);
			}
			else
			{
				needSkillSelection = true;
			}
			this._needSkillSelection = needSkillSelection;
			argsBox.Get<Action<sbyte, sbyte>>("CallBack", out this._onSkillSelectedCallback);
		}

		// Token: 0x0600753E RID: 30014 RVA: 0x00369E84 File Offset: 0x00368084
		private void RequestData()
		{
			this._selfTeam.Clear();
			this._selfTeam.Add(SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			this._enemyTeam.Clear();
			this._enemyTeam.Add(this._enemyCharId);
			for (sbyte i = 0; i < 16; i += 1)
			{
				this._skillTypeStates[(int)i] = new ViewLifeSkillCombatBegin.LifeSkillTypeState(i);
			}
			this.FetchWisdomData();
			this.RequestLifeSkillStrategyPlans();
			this.FetchAllCharacterData();
			this.RequestCombatBeginDisplayData();
		}

		// Token: 0x0600753F RID: 30015 RVA: 0x00369F10 File Offset: 0x00368110
		private void InitSpectators(Action action)
		{
			this.Model.ClearAudience();
			TaiwuDomainMethod.AsyncCall.DebateGamePickSpectators(this, this._selectedSkillType, this._enemyCharId, true, null, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._availableSpectatorIds);
			});
			AsyncMethodCallbackDelegate <>9__2;
			TaiwuDomainMethod.AsyncCall.DebateGamePickSpectators(this, this._selectedSkillType, this._enemyCharId, false, null, delegate(int offset, RawDataPool pool)
			{
				List<int> charList = new List<int>();
				Serializer.Deserialize(pool, offset, ref charList);
				bool flag = charList == null || charList.Count <= 0;
				if (flag)
				{
					Action action2 = action;
					if (action2 != null)
					{
						action2();
					}
				}
				else
				{
					IAsyncMethodRequestHandler <>4__this = this;
					List<int> charIdList = charList;
					AsyncMethodCallbackDelegate callback;
					if ((callback = <>9__2) == null)
					{
						callback = (<>9__2 = delegate(int offset2, RawDataPool pool2)
						{
							List<CharacterDisplayData> charDataList = new List<CharacterDisplayData>();
							Serializer.Deserialize(pool2, offset2, ref charDataList);
							bool flag2 = charDataList != null;
							if (flag2)
							{
								List<CharacterDisplayData> enemyAudienceList = this.Model.GetAudienceList(false);
								for (int i = 0; i < charDataList.Count; i++)
								{
									bool flag3 = enemyAudienceList.CheckIndex(i);
									if (flag3)
									{
										enemyAudienceList[i] = charDataList[i];
									}
								}
							}
							Action action3 = action;
							if (action3 != null)
							{
								action3();
							}
						});
					}
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(<>4__this, charIdList, callback);
				}
			});
		}

		// Token: 0x06007540 RID: 30016 RVA: 0x00369F80 File Offset: 0x00368180
		private void FetchAllCharacterData()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, new List<int>
			{
				this._selfCharId,
				this._enemyCharId
			}, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayData> list = new List<CharacterDisplayData>();
				Serializer.Deserialize(pool, offset, ref list);
				bool flag = list == null;
				if (!flag)
				{
					foreach (CharacterDisplayData data in list)
					{
						bool flag2 = data.CharacterId == this._selfCharId;
						if (flag2)
						{
							this._selfCharData = data;
						}
						else
						{
							bool flag3 = data.CharacterId == this._enemyCharId;
							if (flag3)
							{
								this._enemyCharData = data;
							}
						}
					}
				}
			});
			TaiwuDomainMethod.AsyncCall.GetIsTaiwuFirstByLuck(this, this._enemyCharId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._isTaiwuFirstByLuck);
				this.OnCharacterDataReady();
			});
		}

		// Token: 0x06007541 RID: 30017 RVA: 0x00369FD9 File Offset: 0x003681D9
		private void RequestCombatBeginDisplayData()
		{
			TaiwuDomainMethod.AsyncCall.GetLifeSkillCombatBeginDisplayData(this, this._enemyCharId, delegate(int offset, RawDataPool dataPool)
			{
				this._combatBeginDisplayData = new LifeSkillCombatBeginDisplayData();
				Serializer.Deserialize(dataPool, offset, ref this._combatBeginDisplayData);
				this.SetLoopingAndReading();
				bool flag = !this._isInited;
				if (flag)
				{
					this.GetAvailableSkillTypes();
					this.CurrentStage = (this._needSkillSelection ? ELifeSkillCombatBeginStage.SelectSkillNormal : ELifeSkillCombatBeginStage.SelectStrategy);
					this._isInited = true;
				}
				else
				{
					this.CurrentStage = this.CurrentStage;
				}
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06007542 RID: 30018 RVA: 0x00369FF5 File Offset: 0x003681F5
		private void FetchWisdomData()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterWisdomCountById(this, this._selfCharId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._wisdomSelf);
				this._wisdomSelf = Math.Abs(this._wisdomSelf);
			});
			CharacterDomainMethod.AsyncCall.GetCharacterWisdomCountById(this, this._enemyCharId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._wisdomAdversary);
				this._wisdomAdversary = Math.Abs(this._wisdomAdversary);
			});
		}

		// Token: 0x06007543 RID: 30019 RVA: 0x0036A02A File Offset: 0x0036822A
		private void RequestLifeSkillStrategyPlans()
		{
			TaiwuDomainMethod.AsyncCall.RequestLifeSkillStrategyPlans(this, delegate(int offset, RawDataPool pool)
			{
				LifeSkillStrategyPlanDisplayData lifeSkillStrategyPlanDisplayData = new LifeSkillStrategyPlanDisplayData();
				Serializer.Deserialize(pool, offset, ref lifeSkillStrategyPlanDisplayData);
				this._lifeSkillStrategyPlans = lifeSkillStrategyPlanDisplayData.LifeSkillStrategyPlans;
			});
		}

		// Token: 0x06007544 RID: 30020 RVA: 0x0036A040 File Offset: 0x00368240
		private void OnCharacterDataReady()
		{
			this.RefreshCharacterItems();
			this.SetTalkInfo(0, 0);
			TaiwuDomainMethod.AsyncCall.GetAiBriberyDataOnPrepareLifeSkillCombat(this, this._enemyCharId, delegate(int offset, RawDataPool pool)
			{
				BriberyData bribery = null;
				Serializer.Deserialize(pool, offset, ref bribery);
				this.ShowAiBriberyDialog(bribery);
			});
		}

		// Token: 0x06007545 RID: 30021 RVA: 0x0036A06C File Offset: 0x0036826C
		private void RefreshCharacterItems()
		{
			List<CharacterDisplayData> selfAudienceList = this.Model.GetAudienceList(true);
			List<CharacterDisplayData> enemyAudienceList = this.Model.GetAudienceList(false);
			this.leftCharacterItem.Set(true, this._selfCharData, selfAudienceList, this.IsTaiwuFirst, this._wisdomSelf);
			this.rightCharacterItem.Set(false, this._enemyCharData, enemyAudienceList, !this.IsTaiwuFirst, this._wisdomAdversary);
		}

		// Token: 0x06007546 RID: 30022 RVA: 0x0036A0D8 File Offset: 0x003682D8
		private void SetLoopingAndReading()
		{
			this.loopingProgressHolder.gameObject.SetActive(this._combatBeginDisplayData.LoopingNeigongId >= 0);
			this.loopingIcon.gameObject.SetActive(this._combatBeginDisplayData.LoopingNeigongId >= 0);
			string currentCountCombatSkill = (this._combatBeginDisplayData.LoopInLifeSkillCombatCount == 0) ? this._combatBeginDisplayData.LoopInLifeSkillCombatCount.ToString().SetColor("brightred") : this._combatBeginDisplayData.LoopInLifeSkillCombatCount.ToString();
			this.loopEventCount.text = LanguageKey.LK_LoopingEvent_LoopingInLifeSkillCombat.TrFormat(string.Format("{0}/{1}", currentCountCombatSkill, 1));
			bool flag = this._combatBeginDisplayData.LoopingNeigongId >= 0;
			if (flag)
			{
				this.loopingIcon.SetSprite(CombatSkill.Instance[this._combatBeginDisplayData.LoopingNeigongId].Icon, false, null);
			}
			this.readingProgressHolderCombatSkill.gameObject.SetActive(this._combatBeginDisplayData.CurReadingBook.TemplateId >= 0 && SkillBook.Instance[this._combatBeginDisplayData.CurReadingBook.TemplateId].ItemSubType == 1001);
			this.readingProgressHolderLifeSkill.gameObject.SetActive(this._combatBeginDisplayData.CurReadingBook.TemplateId >= 0 && SkillBook.Instance[this._combatBeginDisplayData.CurReadingBook.TemplateId].ItemSubType == 1000);
			this.readingIcon.gameObject.SetActive(this._combatBeginDisplayData.CurReadingBook.TemplateId >= 0);
			bool flag2 = this._combatBeginDisplayData.CurReadingBook.TemplateId >= 0;
			if (flag2)
			{
				this.readingIcon.SetSprite(SkillBook.Instance[this._combatBeginDisplayData.CurReadingBook.TemplateId].Icon, false, null);
			}
			string currentCountLifeSkill = (this._combatBeginDisplayData.ReadInLifeSkillCombatCount == 0) ? this._combatBeginDisplayData.ReadInLifeSkillCombatCount.ToString().SetColor("brightred") : this._combatBeginDisplayData.ReadInLifeSkillCombatCount.ToString();
			this.readingEventCount.text = LanguageKey.LK_Reading_ReadInLifeSkillCombat_Count.TrFormat(string.Format("{0}/{1}", currentCountLifeSkill, 1));
			this.SetReadingEventBtnMouseTip(this.readingEventCount.GetComponentInChildren<TooltipInvoker>(), (int)this._combatBeginDisplayData.ReadInCombatCount, (int)this._combatBeginDisplayData.ReadInLifeSkillCombatCount);
			this.SetReadingEventBtnMouseTip(this.readingBtn.GetComponent<TooltipInvoker>(), (int)this._combatBeginDisplayData.ReadInCombatCount, (int)this._combatBeginDisplayData.ReadInLifeSkillCombatCount);
			this.SetLoopingEventBtnMouseTip(this.loopEventCount.GetComponentInChildren<TooltipInvoker>(), (int)this._combatBeginDisplayData.LoopInCombatCount, (int)this._combatBeginDisplayData.LoopInLifeSkillCombatCount);
			this.SetLoopingEventBtnMouseTip(this.loopingBtn.GetComponent<TooltipInvoker>(), (int)this._combatBeginDisplayData.LoopInCombatCount, (int)this._combatBeginDisplayData.LoopInLifeSkillCombatCount);
		}

		// Token: 0x06007547 RID: 30023 RVA: 0x0036A3D4 File Offset: 0x003685D4
		private void BanSkill(int availableIndex)
		{
			ViewLifeSkillCombatBegin.LifeSkillTypeState state = this.FindStateByAvailableIndex(availableIndex);
			state.Interactable = false;
			bool flag = this.CheckTypeIsTopThree(state.LifeSkillType, true);
			if (flag)
			{
				this.SetTalkInfo(2, 1);
			}
			bool flag2 = this._skillTypeStates.Count((ViewLifeSkillCombatBegin.LifeSkillTypeState s) => s.Interactable) == 1;
			if (flag2)
			{
				ViewLifeSkillCombatBegin.LifeSkillTypeState lastState = this._skillTypeStates.First((ViewLifeSkillCombatBegin.LifeSkillTypeState s) => s.Interactable);
				this._selectedSkillType = lastState.LifeSkillType;
				lastState.Visible = true;
				this.RefreshCharacterItems();
				this.SetSelectSkillItems();
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, new Action(this.CompleteSkillSelection));
			}
			else
			{
				this.CurrentStage = ELifeSkillCombatBeginStage.SelectSkillAI;
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(Random.Range(1f, 2f), delegate
				{
					bool flag3 = this.CurrentStage == ELifeSkillCombatBeginStage.SelectSkillAI;
					if (flag3)
					{
						this.AiBanSkill();
					}
				});
			}
		}

		// Token: 0x06007548 RID: 30024 RVA: 0x0036A4D8 File Offset: 0x003686D8
		private void ObserveSkill(int availableIndex)
		{
			ViewLifeSkillCombatBegin.LifeSkillTypeState state = this.FindStateByAvailableIndex(availableIndex);
			this._wisdomSelf -= this._wisdomCost;
			this._wisdomCost = Math.Min(this._wisdomCost + 1, 3);
			state.Visible = true;
			this.SetTalkInfo(2, 2);
			this.RefreshCharacterItems();
			this.SetSelectSkillItems();
			this.SetObserveBtnInteractable();
		}

		// Token: 0x06007549 RID: 30025 RVA: 0x0036A53C File Offset: 0x0036873C
		private void SetObserveBtnInteractable()
		{
			bool wisdomEnough = this._wisdomSelf >= this._wisdomCost;
			bool isAnySkillTypes = this._skillTypeStates.Any((ViewLifeSkillCombatBegin.LifeSkillTypeState s) => s.Interactable && !s.Visible);
			this.observeBtn.interactable = ((wisdomEnough && isAnySkillTypes) || this._currentStage == ELifeSkillCombatBeginStage.SelectSkillObserve);
			TooltipInvoker tip = this.observeBtn.GetComponent<TooltipInvoker>();
			bool interactable = this.observeBtn.interactable;
			if (interactable)
			{
				tip.Type = TipType.Simple;
				tip.PresetParam[0] = "UI_LifeSkillBattle_Prepare_Operation_Observe";
				tip.PresetParam[1] = "UI_LifeSkillBattle_Prepare_Operation_Observe_Tip";
			}
			else
			{
				bool flag = !isAnySkillTypes;
				if (flag)
				{
					tip.Type = TipType.SingleDesc;
					tip.PresetParam[0] = "LK_LifeSkillCombatBegin_Observe_NoSkill";
				}
				else
				{
					bool flag2 = !wisdomEnough;
					if (flag2)
					{
						tip.Type = TipType.SingleDesc;
						tip.PresetParam[0] = "LK_LifeSkillCombatBegin_Observe_NoWisdom";
					}
				}
			}
		}

		// Token: 0x0600754A RID: 30026 RVA: 0x0036A628 File Offset: 0x00368828
		private void GiveUpSelection()
		{
			this._isGivenUp = true;
			this._selectedSkillType = (from s in this._skillTypeStates
			where s.Interactable
			select s.LifeSkillType into lt
			orderby this.EnemyAttainments.Get((int)lt) descending
			select lt).First<sbyte>();
			this.SetSkillTypeVisible(this._selectedSkillType, true);
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, new Action(this.CompleteSkillSelection));
		}

		// Token: 0x0600754B RID: 30027 RVA: 0x0036A6D4 File Offset: 0x003688D4
		private void CompleteSkillSelection()
		{
			Action<sbyte, sbyte> onSkillSelectedCallback = this._onSkillSelectedCallback;
			if (onSkillSelectedCallback != null)
			{
				onSkillSelectedCallback(this._selectedSkillType, this.IsTaiwuFirst ? 1 : -1);
			}
			this.Model.LifeSkillType = this._selectedSkillType;
			this.SetTalkInfo(0, 10);
			this.CurrentStage = ELifeSkillCombatBeginStage.SelectStrategy;
		}

		// Token: 0x0600754C RID: 30028 RVA: 0x0036A72C File Offset: 0x0036892C
		private void AiBanSkill()
		{
			sbyte banType = this.CheckVisibleTypeContainsTopCanSelect();
			bool flag = banType >= 0;
			if (flag)
			{
				this.DoAiBanOperation(banType);
			}
			else
			{
				int visibleCell = this.CheckIsNotVisibleCell();
				bool flag2 = visibleCell >= 0;
				if (flag2)
				{
					this.DoAiObserveOperation((sbyte)visibleCell);
				}
				else
				{
					banType = this.GetBanTypeForAi();
					bool flag3 = banType >= 0;
					if (flag3)
					{
						this.DoAiBanOperation(banType);
					}
				}
			}
		}

		// Token: 0x0600754D RID: 30029 RVA: 0x0036A794 File Offset: 0x00368994
		private void DoAiObserveOperation(sbyte skillType)
		{
			this._wisdomAdversary -= this._wisdomCostNpc;
			this._wisdomCostNpc++;
			this.SetSkillTypeVisible(skillType, true);
			AudioManager.Instance.PlaySound("ui_art_prove", false, false);
			this.SetTalkInfo(1, 2);
			this.CurrentStage = ELifeSkillCombatBeginStage.SelectSkillNormal;
		}

		// Token: 0x0600754E RID: 30030 RVA: 0x0036A7F0 File Offset: 0x003689F0
		private void DoAiBanOperation(sbyte skillType)
		{
			this.SetSkillTypeInteractable(skillType, false);
			AudioManager.Instance.PlaySound("ui_art_prohibit", false, false);
			bool flag = this.CheckTypeIsTopThree(skillType, false);
			if (flag)
			{
				this.SetTalkInfo(1, 1);
			}
			bool flag2 = this._skillTypeStates.Count((ViewLifeSkillCombatBegin.LifeSkillTypeState s) => s.Interactable) == 1;
			if (flag2)
			{
				ViewLifeSkillCombatBegin.LifeSkillTypeState lastState = this._skillTypeStates.First((ViewLifeSkillCombatBegin.LifeSkillTypeState s) => s.Interactable);
				this._selectedSkillType = lastState.LifeSkillType;
				this.SetSkillTypeVisible(this._selectedSkillType, true);
				this.RefreshCharacterItems();
				this.SetSelectSkillItems();
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, new Action(this.CompleteSkillSelection));
			}
			else
			{
				this.CurrentStage = ELifeSkillCombatBeginStage.SelectSkillNormal;
			}
		}

		// Token: 0x0600754F RID: 30031 RVA: 0x0036A8DC File Offset: 0x00368ADC
		private sbyte GetBanTypeForAi()
		{
			int visibleCount = this._skillTypeStates.Count((ViewLifeSkillCombatBegin.LifeSkillTypeState s) => s.Interactable && s.Visible);
			bool flag = visibleCount > 0;
			sbyte result;
			if (flag)
			{
				result = (from s in this._skillTypeStates
				where s.Interactable && s.Visible
				select s.LifeSkillType into lt
				orderby this.EnemyAttainments.Get((int)lt)
				select lt).First<sbyte>();
			}
			else
			{
				using (IEnumerator<int> enumerator = this.GetVisibleIndices().GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						int index = enumerator.Current;
						return (sbyte)index;
					}
				}
				result = -1;
			}
			return result;
		}

		// Token: 0x06007550 RID: 30032 RVA: 0x0036A9CC File Offset: 0x00368BCC
		private ViewLifeSkillCombatBegin.LifeSkillTypeState FindStateByAvailableIndex(int availableIndex)
		{
			for (int i = 0; i < this._skillTypeStates.Length; i++)
			{
				bool flag = this._skillTypeStates[i].LifeSkillType == this._availableSkillTypes[availableIndex].LifeSkillType;
				if (flag)
				{
					return this._skillTypeStates[i];
				}
			}
			return null;
		}

		// Token: 0x06007551 RID: 30033 RVA: 0x0036AA24 File Offset: 0x00368C24
		private void SetSkillTypeVisible(sbyte skillType, bool visible)
		{
			for (int i = 0; i < this._skillTypeStates.Length; i++)
			{
				bool flag = this._skillTypeStates[i].LifeSkillType == skillType;
				if (flag)
				{
					ViewLifeSkillCombatBegin.LifeSkillTypeState state = this._skillTypeStates[i];
					state.Visible = visible;
					this._skillTypeStates[i] = state;
					break;
				}
			}
		}

		// Token: 0x06007552 RID: 30034 RVA: 0x0036AA7C File Offset: 0x00368C7C
		private void SetSkillTypeInteractable(sbyte skillType, bool interactable)
		{
			for (int i = 0; i < this._skillTypeStates.Length; i++)
			{
				bool flag = this._skillTypeStates[i].LifeSkillType == skillType;
				if (flag)
				{
					ViewLifeSkillCombatBegin.LifeSkillTypeState state = this._skillTypeStates[i];
					state.Interactable = interactable;
					this._skillTypeStates[i] = state;
					break;
				}
			}
		}

		// Token: 0x06007553 RID: 30035 RVA: 0x0036AAD4 File Offset: 0x00368CD4
		private sbyte CheckVisibleTypeContainsTopCanSelect()
		{
			List<sbyte> visibleTypes = (from s in this._skillTypeStates
			where s.Visible && s.Interactable
			select s.LifeSkillType).ToList<sbyte>();
			bool flag = visibleTypes.Count == 0;
			sbyte result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				sbyte topSkill = (from s in this._skillTypeStates
				where s.Interactable
				select s.LifeSkillType into lt
				orderby this.SelfAttainments.Get((int)lt) descending
				select lt).First<sbyte>();
				result = (visibleTypes.Contains(topSkill) ? topSkill : -1);
			}
			return result;
		}

		// Token: 0x06007554 RID: 30036 RVA: 0x0036ABC0 File Offset: 0x00368DC0
		private int CheckIsNotVisibleCell()
		{
			bool flag = this._skillTypeStates.Count((ViewLifeSkillCombatBegin.LifeSkillTypeState s) => s.Interactable && !s.Visible) > 0 && this._wisdomAdversary >= this._wisdomCostNpc;
			if (flag)
			{
				using (IEnumerator<int> enumerator = this.GetVisibleIndices().GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						return enumerator.Current;
					}
				}
			}
			return -1;
		}

		// Token: 0x06007555 RID: 30037 RVA: 0x0036AC5C File Offset: 0x00368E5C
		private IEnumerable<int> GetVisibleIndices()
		{
			int[] indices = Enumerable.Range(0, this._skillTypeStates.Length).ToArray<int>();
			indices.Shuffle(1);
			return from i in indices
			where this._skillTypeStates[i].Interactable && !this._skillTypeStates[i].Visible
			select i;
		}

		// Token: 0x06007556 RID: 30038 RVA: 0x0036AC9C File Offset: 0x00368E9C
		private bool CheckTypeIsTopThree(sbyte type, bool isTaiwu)
		{
			int range = 1;
			foreach (int index in this.GetDescendingOrderedIndices(isTaiwu ? this.SelfAttainments : this.EnemyAttainments))
			{
				bool flag = index == (int)type;
				if (flag)
				{
					return true;
				}
				bool flag2 = ++range > 3;
				if (flag2)
				{
					break;
				}
			}
			return false;
		}

		// Token: 0x06007557 RID: 30039 RVA: 0x0036AD20 File Offset: 0x00368F20
		private IEnumerable<int> GetAscendingOrderedIndices(LifeSkillShorts shorts)
		{
			return from i in Enumerable.Range(0, 16)
			orderby shorts.Get((int)((sbyte)i))
			select i;
		}

		// Token: 0x06007558 RID: 30040 RVA: 0x0036AD58 File Offset: 0x00368F58
		private IEnumerable<int> GetDescendingOrderedIndices(LifeSkillShorts shorts)
		{
			return from i in Enumerable.Range(0, 16)
			orderby shorts.Get((int)((sbyte)i)) descending
			select i;
		}

		// Token: 0x06007559 RID: 30041 RVA: 0x0036AD90 File Offset: 0x00368F90
		private void GetAvailableSkillTypes()
		{
			for (sbyte k = 0; k < 16; k += 1)
			{
				this._skillTypeStates[(int)k] = new ViewLifeSkillCombatBegin.LifeSkillTypeState(k);
			}
			this.BanWorstSkillTypes(this.SelfAttainments, 6);
			this.BanWorstSkillTypes(this.EnemyAttainments, 6);
			List<int> left = (from i in Enumerable.Range(0, 16)
			where this._skillTypeStates[i].Interactable
			select i).ToList<int>();
			CollectionUtils.Shuffle<int>(GameApp.Random, left);
			for (int j = left.Count - 1; j >= 6; j--)
			{
				this._skillTypeStates[left[j]].Interactable = false;
			}
			IEnumerable<int> visibleIndices = this.GetVisibleIndices().Take(3);
			foreach (int index in visibleIndices)
			{
				this._skillTypeStates[index].Visible = true;
				this._skillTypeStates[index].InitiallyVisible = true;
			}
			int availableIndex = 0;
			foreach (ViewLifeSkillCombatBegin.LifeSkillTypeState item in from s in this._skillTypeStates
			where s.Interactable
			select s)
			{
				this._availableSkillTypes[availableIndex++] = item;
				bool flag = availableIndex >= 6;
				if (flag)
				{
					break;
				}
			}
		}

		// Token: 0x0600755A RID: 30042 RVA: 0x0036AF28 File Offset: 0x00369128
		private void BanWorstSkillTypes(LifeSkillShorts attainments, int count)
		{
			int i = 0;
			foreach (int index in this.GetAscendingOrderedIndices(attainments))
			{
				this._skillTypeStates[index].Interactable = false;
				bool flag = ++i >= count;
				if (flag)
				{
					break;
				}
			}
		}

		// Token: 0x0600755B RID: 30043 RVA: 0x0036AF98 File Offset: 0x00369198
		private void SetSelectSkillItems()
		{
			int count = this._availableSkillTypes.Count((ViewLifeSkillCombatBegin.LifeSkillTypeState x) => x != null);
			CommonUtils.PrepareEnoughChildren(this.selectSkillBtnHolder, this.selectSkillBtnHolder.GetChild(0).gameObject, count, null);
			bool isObserve = this.CurrentStage == ELifeSkillCombatBeginStage.SelectSkillObserve;
			sbyte i = 0;
			while ((int)i < count)
			{
				sbyte skillType = this._availableSkillTypes[(int)i].LifeSkillType;
				short selfValue = this.SelfAttainments.Get((int)skillType);
				short enemyValue = this.EnemyAttainments.Get((int)skillType);
				bool flag = !this._availableSkillTypes[(int)i].Interactable;
				ESelectSkillType selectSkillType;
				bool canInteract;
				if (flag)
				{
					selectSkillType = ESelectSkillType.Banned;
					canInteract = false;
				}
				else
				{
					bool visible = this._availableSkillTypes[(int)i].Visible;
					if (visible)
					{
						selectSkillType = ESelectSkillType.Normal;
						canInteract = !isObserve;
					}
					else
					{
						bool flag2 = isObserve;
						if (flag2)
						{
							selectSkillType = ESelectSkillType.Observe;
							canInteract = (this._wisdomSelf >= this._wisdomCost);
						}
						else
						{
							selectSkillType = ESelectSkillType.UnKnown;
							canInteract = true;
						}
					}
				}
				bool isObserved = this._availableSkillTypes[(int)i].Visible && !this._availableSkillTypes[(int)i].InitiallyVisible;
				LifeSkillCombatBeginSelectSkillItem selectSkillItem = this.selectSkillBtnHolder.GetChild((int)i).GetComponent<LifeSkillCombatBeginSelectSkillItem>();
				selectSkillItem.Set(i, skillType, selectSkillType, selfValue > enemyValue, Math.Abs((int)(selfValue - enemyValue)), this._wisdomCost, canInteract, isObserved);
				i += 1;
			}
			this.ShowAttainment(this._availableSkillTypes.First((ViewLifeSkillCombatBegin.LifeSkillTypeState x) => x.Visible).LifeSkillType);
		}

		// Token: 0x0600755C RID: 30044 RVA: 0x0036B150 File Offset: 0x00369350
		internal void OnSelectSkillItemClicked(sbyte index)
		{
			ELifeSkillCombatBeginStage currentStage = this.CurrentStage;
			ELifeSkillCombatBeginStage elifeSkillCombatBeginStage = currentStage;
			if (elifeSkillCombatBeginStage != ELifeSkillCombatBeginStage.SelectSkillNormal)
			{
				if (elifeSkillCombatBeginStage == ELifeSkillCombatBeginStage.SelectSkillObserve)
				{
					this.ObserveSkill((int)index);
				}
			}
			else
			{
				bool forceSilentSucceed = this._forceSilentSucceed;
				if (forceSilentSucceed)
				{
					this._selectedSkillType = this._availableSkillTypes[(int)index].LifeSkillType;
					this.CompleteSkillSelection();
				}
				else
				{
					this.BanSkill((int)index);
				}
			}
		}

		// Token: 0x0600755D RID: 30045 RVA: 0x0036B1B0 File Offset: 0x003693B0
		private void SetTalkInfo(sbyte isTaiwu, short talkTemplateId)
		{
			ViewLifeSkillCombatBegin.<>c__DisplayClass118_0 CS$<>8__locals1;
			CS$<>8__locals1.config = LifeSkillCombatTalk.Instance[talkTemplateId];
			bool flag = CS$<>8__locals1.config == null;
			if (!flag)
			{
				switch (isTaiwu)
				{
				case 0:
					this.leftCharacterItem.ShowBubble(ViewLifeSkillCombatBegin.<SetTalkInfo>g__GetTalkInfo|118_0(this._selfCharData.BehaviorType, ref CS$<>8__locals1));
					this.rightCharacterItem.ShowBubble(ViewLifeSkillCombatBegin.<SetTalkInfo>g__GetTalkInfo|118_0(this._enemyCharData.BehaviorType, ref CS$<>8__locals1));
					break;
				case 1:
					this.leftCharacterItem.ShowBubble(ViewLifeSkillCombatBegin.<SetTalkInfo>g__GetTalkInfo|118_0(this._selfCharData.BehaviorType, ref CS$<>8__locals1));
					break;
				case 2:
					this.rightCharacterItem.ShowBubble(ViewLifeSkillCombatBegin.<SetTalkInfo>g__GetTalkInfo|118_0(this._enemyCharData.BehaviorType, ref CS$<>8__locals1));
					break;
				}
			}
		}

		// Token: 0x0600755E RID: 30046 RVA: 0x0036B27C File Offset: 0x0036947C
		private void OnForceSilentResult(ArgumentBox argBox)
		{
			sbyte type;
			argBox.Get("Type", out type);
			bool result;
			argBox.Get("Result", out result);
			this._hasForceSilent = true;
			if (!true)
			{
			}
			short num;
			if (type != 0)
			{
				if (type != 1)
				{
					num = 0;
				}
				else
				{
					num = (result ? 5 : 7);
				}
			}
			else
			{
				num = (result ? 4 : 6);
			}
			if (!true)
			{
			}
			short taiwuKey = num;
			this.SetTalkInfo(result ? 2 : 1, taiwuKey);
			bool flag = result;
			if (flag)
			{
				this._forceSilentSucceed = true;
				for (int i = 0; i < this._availableSkillTypes.Length; i++)
				{
					bool flag2 = this._availableSkillTypes[i] != null;
					if (flag2)
					{
						this._availableSkillTypes[i].Visible = true;
					}
				}
			}
			this.CurrentStage = ELifeSkillCombatBeginStage.SelectSkillNormal;
		}

		// Token: 0x0600755F RID: 30047 RVA: 0x0036B340 File Offset: 0x00369540
		private void ShowAiBriberyDialog(BriberyData bribery)
		{
			bool flag = bribery == null;
			if (!flag)
			{
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.Set<CharacterDisplayData>("charData", this._enemyCharData);
				bool isItem = bribery.ItemDisplayData != null;
				bool isSecretInformation = bribery.SecretInformationDisplayPackage != null;
				bool isItem2 = isItem;
				if (isItem2)
				{
					box.Set("isItem", true);
					box.SetObject("itemDisplayData", bribery.ItemDisplayData);
				}
				else
				{
					bool flag2 = isSecretInformation;
					if (flag2)
					{
						box.SetObject("secretInformationDisplayData", bribery.SecretInformationDisplayPackage.SecretInformationDisplayDataList[0]);
						box.SetObject("secretInformationDisplayPackage", bribery.SecretInformationDisplayPackage);
					}
					else
					{
						box.Set("AiThreaten", true);
					}
				}
				box.SetObject("onYes", new Action(delegate
				{
					short key = isItem ? 5 : 4;
					this.SetTalkInfo(2, key);
					this.GiveUpSelection();
				}));
				box.SetObject("onNo", new Action(delegate
				{
					short key = isItem ? 7 : 6;
					this.SetTalkInfo(1, key);
				}));
				UIElement.AiForceSilenceDialog.SetOnInitArgs(box);
				UIManager.Instance.MaskUI(UIElement.AiForceSilenceDialog);
			}
		}

		// Token: 0x06007560 RID: 30048 RVA: 0x0036B460 File Offset: 0x00369660
		private void OnPresetStrategyToggleGroupActiveIndexChange(int newIndex, int oldIndex)
		{
			if (this._lifeSkillStrategyPlans == null)
			{
				this._lifeSkillStrategyPlans = new Dictionary<int, GameData.Utilities.ShortList>();
			}
			bool flag = oldIndex >= 0;
			if (flag)
			{
				GameData.Utilities.ShortList oldData;
				bool flag2 = !this._lifeSkillStrategyPlans.TryGetValue(oldIndex, out oldData) || oldData.Items == null;
				if (flag2)
				{
					oldData = GameData.Utilities.ShortList.Create();
					ref List<short> ptr = ref oldData.Items;
					if (ptr == null)
					{
						ptr = new List<short>();
					}
				}
				oldData.Items.Clear();
				short i = 0;
				while ((int)i < this.strategyToggleGroup.GetAll().Count)
				{
					bool isOn = this.strategyToggleGroup.Get((int)i).isOn;
					if (isOn)
					{
						oldData.Items.Add(i);
					}
					i += 1;
				}
				this._lifeSkillStrategyPlans[oldIndex] = oldData;
				TaiwuDomainMethod.Call.SetLifeSkillStrategyPlansElement(oldIndex, oldData);
			}
			GameData.Utilities.ShortList data;
			bool flag3 = this._lifeSkillStrategyPlans == null || !this._lifeSkillStrategyPlans.TryGetValue(newIndex, out data) || data.Items == null;
			if (flag3)
			{
				this.strategyToggleGroup.DeSelectAll(false);
			}
			else
			{
				this.strategyToggleGroup.DeSelectAll(false);
				short j = 0;
				while ((int)j < data.Items.Count)
				{
					this.strategyToggleGroup.Select((int)data.Items[(int)j], false);
					j += 1;
				}
			}
		}

		// Token: 0x06007561 RID: 30049 RVA: 0x0036B5C4 File Offset: 0x003697C4
		private void SetSelectStrategyItems()
		{
			sbyte i = 0;
			while ((int)i < this.strategyToggleGroup.GetAll().Count)
			{
				this.strategyToggleGroup.Get((int)i).GetComponent<LifeSkillCombatBeginSkillStrategyItem>().Set(i, this._combatBeginDisplayData.UnlockedCountList[(int)i]);
				i += 1;
			}
		}

		// Token: 0x06007562 RID: 30050 RVA: 0x0036B620 File Offset: 0x00369820
		private void OnStrategyToggleGroupActiveIndexChange(int newIndex, int cancelledIndex)
		{
			bool isMaxReached = this.strategyToggleGroup.SelectedCount() >= 4;
			for (int i = 0; i < this.strategyToggleGroup.GetAll().Count; i++)
			{
				CToggle toggle = this.strategyToggleGroup.Get(i);
				toggle.interactable = (!isMaxReached || toggle.isOn);
				toggle.GetComponent<HSVStyleRoot>().SetInteractable(toggle.interactable);
			}
		}

		// Token: 0x06007563 RID: 30051 RVA: 0x0036B694 File Offset: 0x00369894
		private void ConfirmStrategyTypes()
		{
			this._selectedStrategyTypes.Clear();
			sbyte i = 0;
			while ((int)i < this.strategyToggleGroup.GetAll().Count)
			{
				bool isOn = this.strategyToggleGroup.Get((int)i).isOn;
				if (isOn)
				{
					this._selectedStrategyTypes.Add(i);
				}
				i += 1;
			}
			TaiwuDomainMethod.Call.DebateGameSetTaiwuSelectedCardTypes(this._selectedStrategyTypes);
		}

		// Token: 0x06007564 RID: 30052 RVA: 0x0036B700 File Offset: 0x00369900
		private void QuickSelectAudience()
		{
			ViewLifeSkillCombatBegin.<>c__DisplayClass125_0 CS$<>8__locals1 = new ViewLifeSkillCombatBegin.<>c__DisplayClass125_0();
			CS$<>8__locals1.<>4__this = this;
			bool hideTaiwuAudience = this.Model.HideTaiwuAudience;
			if (!hideTaiwuAudience)
			{
				CS$<>8__locals1.selfAudienceList = this.Model.GetAudienceList(true);
				List<CharacterDisplayData> enemyAudienceList = this.Model.GetAudienceList(false);
				List<int> existingIds = (from d in enemyAudienceList
				where d != null
				select d.CharacterId).ToList<int>();
				ViewLifeSkillCombatBegin.<>c__DisplayClass125_0 CS$<>8__locals2 = CS$<>8__locals1;
				CharacterDisplayData selfCharData = this._selfCharData;
				CS$<>8__locals2.taiwuBehaviorType = ((selfCharData != null) ? selfCharData.BehaviorType : -1);
				TaiwuDomainMethod.AsyncCall.DebateGamePickSpectators(this, this._selectedSkillType, this._enemyCharId, true, existingIds, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref CS$<>8__locals1.<>4__this._availableSpectatorIds);
					bool flag = CS$<>8__locals1.<>4__this._availableSpectatorIds == null || CS$<>8__locals1.<>4__this._availableSpectatorIds.Count == 0;
					if (flag)
					{
						CS$<>8__locals1.<>4__this.RefreshCharacterItems();
					}
					else
					{
						IAsyncMethodRequestHandler requestHandler = null;
						List<int> availableSpectatorIds = CS$<>8__locals1.<>4__this._availableSpectatorIds;
						AsyncMethodCallbackDelegate callback;
						if ((callback = CS$<>8__locals1.<>9__3) == null)
						{
							callback = (CS$<>8__locals1.<>9__3 = delegate(int offset2, RawDataPool dataPool)
							{
								List<CharacterDisplayDataForGeneralScrollList> displayDataList = new List<CharacterDisplayDataForGeneralScrollList>();
								Serializer.Deserialize(dataPool, offset2, ref displayDataList);
								bool flag2 = displayDataList == null || displayDataList.Count == 0;
								if (flag2)
								{
									CS$<>8__locals1.<>4__this.RefreshCharacterItems();
								}
								else
								{
									IOrderedEnumerable<CharacterDisplayDataForGeneralScrollList> source = from d in displayDataList
									orderby d.FavorabilityToTaiwu descending
									select d;
									Func<CharacterDisplayDataForGeneralScrollList, int> keySelector;
									if ((keySelector = CS$<>8__locals1.<>9__5) == null)
									{
										keySelector = (CS$<>8__locals1.<>9__5 = ((CharacterDisplayDataForGeneralScrollList d) => (d.BehaviorType != CS$<>8__locals1.taiwuBehaviorType) ? 1 : 0));
									}
									List<CharacterDisplayDataForGeneralScrollList> sortedList = source.ThenByDescending(keySelector).ThenBy((CharacterDisplayDataForGeneralScrollList d) => Random.value).Take(3).ToList<CharacterDisplayDataForGeneralScrollList>();
									List<int> selectedIds = (from d in sortedList
									select d.CharacterId).ToList<int>();
									IAsyncMethodRequestHandler <>4__this = CS$<>8__locals1.<>4__this;
									List<int> charIdList = selectedIds;
									AsyncMethodCallbackDelegate callback2;
									if ((callback2 = CS$<>8__locals1.<>9__8) == null)
									{
										callback2 = (CS$<>8__locals1.<>9__8 = delegate(int offset3, RawDataPool pool3)
										{
											List<CharacterDisplayData> charDataList = new List<CharacterDisplayData>();
											Serializer.Deserialize(pool3, offset3, ref charDataList);
											for (int i = 0; i < CS$<>8__locals1.selfAudienceList.Count; i++)
											{
												CS$<>8__locals1.selfAudienceList[i] = null;
											}
											bool flag3 = charDataList != null;
											if (flag3)
											{
												for (int j = 0; j < Math.Min(charDataList.Count, 3); j++)
												{
													bool flag4 = charDataList[j] != null && charDataList[j].CharacterId >= 0;
													if (flag4)
													{
														CS$<>8__locals1.selfAudienceList[j] = charDataList[j];
													}
												}
											}
											CS$<>8__locals1.<>4__this.RefreshCharacterItems();
										});
									}
									CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(<>4__this, charIdList, callback2);
								}
							});
						}
						CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(requestHandler, availableSpectatorIds, callback);
					}
				});
			}
		}

		// Token: 0x06007565 RID: 30053 RVA: 0x0036B7D8 File Offset: 0x003699D8
		private void AutoFillAudience(Action onComplete)
		{
			List<int> selfAudienceList = (from d in this.Model.GetAudienceList(true)
			where d != null
			select d.CharacterId).ToList<int>();
			List<int> enemyAudienceList = (from d in this.Model.GetAudienceList(false)
			where d != null
			select d.CharacterId).ToList<int>();
			bool flag = selfAudienceList.Count >= 3 || this.Model.HideTaiwuAudience;
			if (flag)
			{
				Action onComplete2 = onComplete;
				if (onComplete2 != null)
				{
					onComplete2();
				}
			}
			else
			{
				List<int> existingIds = selfAudienceList.Union(enemyAudienceList).ToList<int>();
				AsyncMethodCallbackDelegate <>9__5;
				TaiwuDomainMethod.AsyncCall.DebateGamePickSpectators(this, this._selectedSkillType, this._enemyCharId, true, existingIds, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._availableSpectatorIds);
					List<int> availableSpectatorIds = this._availableSpectatorIds;
					bool flag2 = availableSpectatorIds != null && availableSpectatorIds.Count > 0;
					if (flag2)
					{
						IAsyncMethodRequestHandler <>4__this = this;
						List<int> availableSpectatorIds2 = this._availableSpectatorIds;
						AsyncMethodCallbackDelegate callback;
						if ((callback = <>9__5) == null)
						{
							callback = (<>9__5 = delegate(int offset2, RawDataPool pool2)
							{
								List<CharacterDisplayData> charDataList = new List<CharacterDisplayData>();
								Serializer.Deserialize(pool2, offset2, ref charDataList);
								bool flag3 = charDataList != null;
								if (flag3)
								{
									List<CharacterDisplayData> selfAudienceDataList = this.Model.GetAudienceList(true);
									foreach (CharacterDisplayData data in charDataList)
									{
										int emptyIndex = selfAudienceDataList.FindIndex((CharacterDisplayData d) => d == null);
										bool flag4 = selfAudienceDataList.CheckIndex(emptyIndex);
										if (flag4)
										{
											selfAudienceDataList[emptyIndex] = data;
										}
									}
								}
								Action onComplete4 = onComplete;
								if (onComplete4 != null)
								{
									onComplete4();
								}
							});
						}
						CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(<>4__this, availableSpectatorIds2, callback);
					}
					else
					{
						Action onComplete3 = onComplete;
						if (onComplete3 != null)
						{
							onComplete3();
						}
					}
				});
			}
		}

		// Token: 0x06007566 RID: 30054 RVA: 0x0036B910 File Offset: 0x00369B10
		internal void SelectTeammate(int index)
		{
			bool hideTaiwuAudience = this.Model.HideTaiwuAudience;
			if (hideTaiwuAudience)
			{
				this.ShowSelectChar(new List<int>());
			}
			else
			{
				List<CharacterDisplayData> enemyAudienceList = this.Model.GetAudienceList(false);
				List<int> existingIds = (from d in enemyAudienceList
				where d != null
				select d.CharacterId).ToList<int>();
				TaiwuDomainMethod.AsyncCall.DebateGamePickSpectators(this, this._selectedSkillType, this._enemyCharId, true, existingIds, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._availableSpectatorIds);
					this.ShowSelectChar(this._availableSpectatorIds);
				});
			}
		}

		// Token: 0x06007567 RID: 30055 RVA: 0x0036B9B9 File Offset: 0x00369BB9
		private void ShowSelectChar(List<int> selectionList)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(null, selectionList, delegate(int offset, RawDataPool dataPool)
			{
				List<CharacterDisplayDataForGeneralScrollList> displayData = new List<CharacterDisplayDataForGeneralScrollList>();
				Serializer.Deserialize(dataPool, offset, ref displayData);
				List<ISelectCharacterData> selectList = (from item in displayData
				select new BasicSelectCharacterDataAdapter(item)).ToList<ISelectCharacterData>();
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
				config.InteractionMode = ESelectCharacterInteractionMode.Slot;
				config.SelectionMode = ESelectCharacterSelectionMode.Multiple;
				config.TargetCount = 3;
				config.InitialSelectedCharacterIds = (from d in this.Model.GetAudienceList(true)
				where d != null
				select d.CharacterId).ToList<int>();
				config.MouseTipModifier = delegate(TooltipInvoker tip, int charId)
				{
					tip.Type = TipType.LifeSkillCombatAudience;
					tip.RuntimeParam.Set("CharId", charId);
				};
				UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", selectList).SetObject("SelectCharacterCallback", new SelectCharacterCallback(delegate(List<int> v)
				{
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, v, delegate(int offset2, RawDataPool pool)
					{
						List<CharacterDisplayData> list = new List<CharacterDisplayData>();
						Serializer.Deserialize(pool, offset2, ref list);
						List<CharacterDisplayData> audienceList = this.Model.GetAudienceList(true);
						bool flag = list == null || list.Count == 0;
						if (flag)
						{
							for (int i = 0; i < audienceList.Count; i++)
							{
								audienceList[i] = null;
							}
						}
						else
						{
							for (int j = 0; j < audienceList.Count; j++)
							{
								audienceList[j] = ((list.CheckIndex(j) && list[j] != null && list[j].CharacterId >= 0) ? list[j] : null);
							}
						}
						this.RefreshCharacterItems();
					});
				})));
				UIManager.Instance.MaskUI(UIElement.SelectChar);
			});
		}

		// Token: 0x06007568 RID: 30056 RVA: 0x0036B9D0 File Offset: 0x00369BD0
		private void StartCombat()
		{
			this._hasForceGiveIn = true;
			this.RefreshCharacterItems();
			this.ConfirmStrategyTypes();
			this.AutoFillAudience(delegate
			{
				UIElement debate = UIElement.Debate;
				debate.OnShowed = (Action)Delegate.Combine(debate.OnShowed, new Action(delegate()
				{
					UIManager.Instance.HideUI(UIElement.LifeSkillCombatBegin);
					GlobalDomainMethod.Call.InvokeGuidingTrigger(249);
				}));
				UIManager.Instance.ShowUI(UIElement.Debate, true);
			});
		}

		// Token: 0x06007569 RID: 30057 RVA: 0x0036BA10 File Offset: 0x00369C10
		internal unsafe void ShowAttainment(sbyte skillId)
		{
			bool flag = skillId == -1;
			if (flag)
			{
				this.leftCharacterItem.ShowAttainment(-1, 0);
				this.rightCharacterItem.ShowAttainment(-1, 0);
			}
			else
			{
				this.leftCharacterItem.ShowAttainment(skillId, (int)(*this.SelfAttainments[(int)skillId]));
				this.rightCharacterItem.ShowAttainment(skillId, (int)(*this.EnemyAttainments[(int)skillId]));
			}
		}

		// Token: 0x0600756A RID: 30058 RVA: 0x0036BA84 File Offset: 0x00369C84
		internal void OnCharacterItemExtraButtonClicked(bool isTaiwu)
		{
			bool flag = !isTaiwu;
			if (flag)
			{
				this.startBtn.enabled = false;
				this._hasForceGiveIn = true;
				this.RefreshCharacterItems();
				TaiwuDomainMethod.AsyncCall.DebateGameTryForceWinInCombatBegin(this, this._selectedSkillType, delegate(int offset, RawDataPool pool)
				{
					bool canForceWin = false;
					Serializer.Deserialize(pool, offset, ref canForceWin);
					bool flag2 = canForceWin;
					if (flag2)
					{
						this.SetTalkInfo(1, this.GetPlaySpeakConfigOnGameOver(true));
						this.SetTalkInfo(2, this.GetPlaySpeakConfigOnGameOver(false));
						DOVirtual.DelayedCall(1f, delegate
						{
							TaiwuDomainMethod.AsyncCall.DebateGameOver(this, true, false, delegate(int offset, RawDataPool dataPool)
							{
								DebateResult debateResult = null;
								Serializer.Deserialize(dataPool, offset, ref debateResult);
								this.Model.ClearAudience();
								ArgumentBox box = EasyPool.Get<ArgumentBox>();
								box.SetObject("DebateResult", debateResult);
								UIElement.DebateResult.SetOnInitArgs(box);
								UIElement debateResult2 = UIElement.DebateResult;
								debateResult2.OnShowed = (Action)Delegate.Combine(debateResult2.OnShowed, new Action(delegate()
								{
									UIManager.Instance.HideUI(this.Element);
								}));
								UIManager.Instance.MaskUI(UIElement.DebateResult);
								Time.timeScale = 1f;
							});
						}, false);
					}
					else
					{
						this.startBtn.enabled = true;
						this.RefreshCharacterItems();
						this.SetTalkInfo(1, 16);
						this.SetTalkInfo(2, 17);
					}
				});
			}
			else
			{
				this.QuickSelectAudience();
			}
		}

		// Token: 0x0600756B RID: 30059 RVA: 0x0036BADB File Offset: 0x00369CDB
		private short GetPlaySpeakConfigOnGameOver(bool isWin)
		{
			return isWin ? 19 : 18;
		}

		// Token: 0x0600756C RID: 30060 RVA: 0x0036BAE8 File Offset: 0x00369CE8
		private void PlayBeginAni()
		{
			this.ani1.AnimationState.SetAnimation(0, "animation", true);
			this.ani2.AnimationState.SetAnimation(0, "animation", true);
			this.ani1.gameObject.SetActive(true);
			this.ani2.gameObject.SetActive(true);
		}

		// Token: 0x0600756D RID: 30061 RVA: 0x0036BB4C File Offset: 0x00369D4C
		private void SetReadingEventBtnMouseTip(TooltipInvoker tipDisplayer, int readInCombatCount, int readInLifeSkillCombatCount)
		{
			tipDisplayer.Type = TipType.ReadingEvent;
			if (tipDisplayer.RuntimeParam == null)
			{
				tipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tipDisplayer.RuntimeParam.Clear();
			tipDisplayer.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Reading_ReadInCombatTitleTip));
			tipDisplayer.RuntimeParam.Set("Content", LocalStringManager.Get(LanguageKey.LK_Reading_ReadInCombatContentTip));
			tipDisplayer.RuntimeParam.Set("SubHeadingText", LocalStringManager.Get(LanguageKey.LK_Reading_Monthly_ReadInCombat));
			string readInCombatCountStr = ViewLifeSkillCombatBegin.FormatCountString(readInCombatCount, "lightblue", "brightred", "pinkyellow");
			string readInLifeSkillCombatCountStr = ViewLifeSkillCombatBegin.FormatCountString(readInLifeSkillCombatCount, "lightblue", "brightred", "pinkyellow");
			tipDisplayer.RuntimeParam.Set("CombatCountText", LocalStringManager.GetFormat(LanguageKey.LK_Reading_ReadInCombat_Count, readInCombatCountStr));
			tipDisplayer.RuntimeParam.Set("LifeSkillCountText", LocalStringManager.GetFormat(LanguageKey.LK_Reading_ReadInLifeSkillCombat_Count, readInLifeSkillCombatCountStr));
		}

		// Token: 0x0600756E RID: 30062 RVA: 0x0036BC3C File Offset: 0x00369E3C
		private static string FormatCountString(int count, string trueColor, string falseColor, string suffixColor)
		{
			string countColor = (count > 0) ? trueColor : falseColor;
			return count.ToString().SetColor(countColor) + "/1".SetColor(suffixColor);
		}

		// Token: 0x0600756F RID: 30063 RVA: 0x0036BC74 File Offset: 0x00369E74
		private void SetLoopingEventBtnMouseTip(TooltipInvoker tipDisplayer, int loopingInCombatCount, int loopingInLifeSkillCombatCount)
		{
			if (tipDisplayer.RuntimeParam == null)
			{
				tipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tipDisplayer.RuntimeParam.Clear();
			tipDisplayer.Type = TipType.LoopingEvent;
			string loopingInCombatCountStr = ViewLifeSkillCombatBegin.FormatCountString(loopingInCombatCount, "lightblue", "brightred", "pinkyellow");
			string loopingInLifeSkillCombatCountStr = ViewLifeSkillCombatBegin.FormatCountString(loopingInLifeSkillCombatCount, "lightblue", "brightred", "pinkyellow");
			tipDisplayer.RuntimeParam.Set("CombatCountText", LocalStringManager.GetFormat(LanguageKey.LK_LoopingEvent_Tips_SubTitle_1, loopingInCombatCountStr));
			tipDisplayer.RuntimeParam.Set("LifeSkillCountText", LocalStringManager.GetFormat(LanguageKey.LK_LoopingEvent_Tips_SubTitle_2, loopingInLifeSkillCombatCountStr));
		}

		// Token: 0x06007571 RID: 30065 RVA: 0x0036BD80 File Offset: 0x00369F80
		[CompilerGenerated]
		private IEnumerator <OnInit>g__AnimRoutine|78_0()
		{
			ViewLifeSkillCombatBegin.<>c__DisplayClass78_0 CS$<>8__locals1;
			CS$<>8__locals1.timePoints = new float[]
			{
				0f,
				0.2f,
				1.3f,
				4f,
				5.1f,
				7.67f
			};
			ParticleSystem particle = this.aniControl;
			AudioManager audioManager = AudioManager.Instance;
			audioManager.PlayAmbience("art_AmbienceFan", 0.1f, 100);
			do
			{
				CS$<>8__locals1.timePointIndex = 0;
				particle.Stop();
				yield return new WaitForSeconds(ViewLifeSkillCombatBegin.<OnInit>g__GetAndAdvanceTimePoint|78_1(ref CS$<>8__locals1));
				audioManager.PlaySound("art_AnimationFan", false, false);
				yield return new WaitForSeconds(ViewLifeSkillCombatBegin.<OnInit>g__GetAndAdvanceTimePoint|78_1(ref CS$<>8__locals1));
				particle.Stop();
				particle.Play();
				yield return new WaitForSeconds(ViewLifeSkillCombatBegin.<OnInit>g__GetAndAdvanceTimePoint|78_1(ref CS$<>8__locals1));
				audioManager.PlaySound("art_AnimationFan", false, false);
				yield return new WaitForSeconds(ViewLifeSkillCombatBegin.<OnInit>g__GetAndAdvanceTimePoint|78_1(ref CS$<>8__locals1));
				particle.Stop();
				particle.Play();
				yield return new WaitForSeconds(ViewLifeSkillCombatBegin.<OnInit>g__GetAndAdvanceTimePoint|78_1(ref CS$<>8__locals1));
			}
			while (particle != null && particle.gameObject != null && particle.gameObject.activeInHierarchy);
			yield break;
		}

		// Token: 0x06007572 RID: 30066 RVA: 0x0036BDA0 File Offset: 0x00369FA0
		[CompilerGenerated]
		internal static float <OnInit>g__GetAndAdvanceTimePoint|78_1(ref ViewLifeSkillCombatBegin.<>c__DisplayClass78_0 A_0)
		{
			float timePoint = A_0.timePoints[A_0.timePointIndex + 1] - A_0.timePoints[A_0.timePointIndex];
			A_0.timePointIndex++;
			return timePoint;
		}

		// Token: 0x0600758B RID: 30091 RVA: 0x0036C1F4 File Offset: 0x0036A3F4
		[CompilerGenerated]
		internal static string <SetTalkInfo>g__GetTalkInfo|118_0(sbyte behaviorType, ref ViewLifeSkillCombatBegin.<>c__DisplayClass118_0 A_1)
		{
			if (!true)
			{
			}
			string text;
			switch (behaviorType)
			{
			case 0:
				text = A_1.config.JustContent;
				break;
			case 1:
				text = A_1.config.KindContent;
				break;
			case 2:
				text = A_1.config.EvenContent;
				break;
			case 3:
				text = A_1.config.RebelContent;
				break;
			case 4:
				text = A_1.config.EgoisticContent;
				break;
			default:
				text = A_1.config.NormalContent;
				break;
			}
			if (!true)
			{
			}
			string content = text;
			return content.IsNullOrEmpty() ? A_1.config.NormalContent : content;
		}

		// Token: 0x040057FC RID: 22524
		private const byte TeamCapacity = 4;

		// Token: 0x040057FD RID: 22525
		private const byte LifeSkillStrategyPlanCount = 9;

		// Token: 0x040057FE RID: 22526
		private const byte StrategyMaxSelectCount = 4;

		// Token: 0x040057FF RID: 22527
		private const sbyte AvailableSkillTypeCount = 6;

		// Token: 0x04005800 RID: 22528
		[SerializeField]
		private LifeSkillCombatBeginCharacterItem leftCharacterItem;

		// Token: 0x04005801 RID: 22529
		[SerializeField]
		private LifeSkillCombatBeginCharacterItem rightCharacterItem;

		// Token: 0x04005802 RID: 22530
		[SerializeField]
		private RectTransform selectStrategyHolder;

		// Token: 0x04005803 RID: 22531
		[SerializeField]
		private CToggleGroup presetStrategyToggleGroup;

		// Token: 0x04005804 RID: 22532
		[SerializeField]
		private CToggleGroupMultiSelect strategyToggleGroup;

		// Token: 0x04005805 RID: 22533
		[SerializeField]
		private RectTransform selectSkillHolder;

		// Token: 0x04005806 RID: 22534
		[SerializeField]
		private RectTransform selectSkillBtnHolder;

		// Token: 0x04005807 RID: 22535
		[SerializeField]
		private CImage selectSkillBg;

		// Token: 0x04005808 RID: 22536
		[SerializeField]
		private TextMeshProUGUI operateTitle;

		// Token: 0x04005809 RID: 22537
		[SerializeField]
		private CButton startBtn;

		// Token: 0x0400580A RID: 22538
		[SerializeField]
		private CButton observeBtn;

		// Token: 0x0400580B RID: 22539
		[SerializeField]
		private CButton giveUpBtn;

		// Token: 0x0400580C RID: 22540
		[SerializeField]
		private CButton briberyBtn;

		// Token: 0x0400580D RID: 22541
		[SerializeField]
		private CButton loopingBtn;

		// Token: 0x0400580E RID: 22542
		[SerializeField]
		private CImage loopingIcon;

		// Token: 0x0400580F RID: 22543
		[SerializeField]
		private RectTransform loopingProgressHolder;

		// Token: 0x04005810 RID: 22544
		[SerializeField]
		private TextMeshProUGUI loopEventCount;

		// Token: 0x04005811 RID: 22545
		[SerializeField]
		private CButton readingBtn;

		// Token: 0x04005812 RID: 22546
		[SerializeField]
		private CImage readingIcon;

		// Token: 0x04005813 RID: 22547
		[SerializeField]
		private RectTransform readingProgressHolderCombatSkill;

		// Token: 0x04005814 RID: 22548
		[SerializeField]
		private RectTransform readingProgressHolderLifeSkill;

		// Token: 0x04005815 RID: 22549
		[SerializeField]
		private TextMeshProUGUI readingEventCount;

		// Token: 0x04005816 RID: 22550
		[SerializeField]
		private TextMeshProUGUI notice;

		// Token: 0x04005817 RID: 22551
		[SerializeField]
		private CImage selectSkillHolderBg;

		// Token: 0x04005818 RID: 22552
		[SerializeField]
		private ParticleSystem aniControl;

		// Token: 0x04005819 RID: 22553
		[SerializeField]
		private SkeletonGraphic ani1;

		// Token: 0x0400581A RID: 22554
		[SerializeField]
		private SkeletonGraphic ani2;

		// Token: 0x0400581B RID: 22555
		[SerializeField]
		private GameObject skillNameObj;

		// Token: 0x0400581C RID: 22556
		[SerializeField]
		private GameObject selectSkillTextBg;

		// Token: 0x0400581D RID: 22557
		[SerializeField]
		private TextMeshProUGUI selectSkillText;

		// Token: 0x0400581E RID: 22558
		private LifeSkillCombatBeginDisplayData _combatBeginDisplayData;

		// Token: 0x0400581F RID: 22559
		private bool _needSkillSelection;

		// Token: 0x04005820 RID: 22560
		private ELifeSkillCombatBeginStage _currentStage;

		// Token: 0x04005821 RID: 22561
		private int _selfCharId;

		// Token: 0x04005822 RID: 22562
		private int _enemyCharId;

		// Token: 0x04005823 RID: 22563
		private CharacterDisplayData _selfCharData;

		// Token: 0x04005824 RID: 22564
		private CharacterDisplayData _enemyCharData;

		// Token: 0x04005825 RID: 22565
		private readonly List<int> _selfTeam = new List<int>();

		// Token: 0x04005826 RID: 22566
		private readonly List<int> _enemyTeam = new List<int>();

		// Token: 0x04005827 RID: 22567
		private int _isTaiwuFirstByLuck;

		// Token: 0x04005828 RID: 22568
		private bool _isGivenUp;

		// Token: 0x04005829 RID: 22569
		private bool _forceSilentSucceed;

		// Token: 0x0400582A RID: 22570
		private readonly ViewLifeSkillCombatBegin.LifeSkillTypeState[] _skillTypeStates = new ViewLifeSkillCombatBegin.LifeSkillTypeState[16];

		// Token: 0x0400582B RID: 22571
		private readonly ViewLifeSkillCombatBegin.LifeSkillTypeState[] _availableSkillTypes = new ViewLifeSkillCombatBegin.LifeSkillTypeState[6];

		// Token: 0x0400582C RID: 22572
		private sbyte _selectedSkillType = -1;

		// Token: 0x0400582D RID: 22573
		private bool _hasForceSilent;

		// Token: 0x0400582E RID: 22574
		private bool _hasForceGiveIn;

		// Token: 0x0400582F RID: 22575
		private Coroutine _animRoutine;

		// Token: 0x04005830 RID: 22576
		private int _wisdomSelf;

		// Token: 0x04005831 RID: 22577
		private int _wisdomAdversary;

		// Token: 0x04005832 RID: 22578
		private int _wisdomCost = 1;

		// Token: 0x04005833 RID: 22579
		private int _wisdomCostNpc = 1;

		// Token: 0x04005834 RID: 22580
		private List<sbyte> _selectedStrategyTypes = new List<sbyte>();

		// Token: 0x04005835 RID: 22581
		private Dictionary<int, GameData.Utilities.ShortList> _lifeSkillStrategyPlans;

		// Token: 0x04005836 RID: 22582
		private List<int> _availableSpectatorIds;

		// Token: 0x04005837 RID: 22583
		private bool _spectatorsInitialized;

		// Token: 0x04005838 RID: 22584
		private Action<sbyte, sbyte> _onSkillSelectedCallback;

		// Token: 0x04005839 RID: 22585
		private bool _isInited = false;

		// Token: 0x02001EA6 RID: 7846
		private class LifeSkillTypeState
		{
			// Token: 0x0600F13B RID: 61755 RVA: 0x00615A4F File Offset: 0x00613C4F
			public LifeSkillTypeState(sbyte type)
			{
				this.LifeSkillType = type;
				this.Visible = false;
				this.Interactable = true;
				this.InitiallyVisible = false;
			}

			// Token: 0x0400CA80 RID: 51840
			public readonly sbyte LifeSkillType;

			// Token: 0x0400CA81 RID: 51841
			public bool Visible;

			// Token: 0x0400CA82 RID: 51842
			public bool Interactable;

			// Token: 0x0400CA83 RID: 51843
			public bool InitiallyVisible;
		}
	}
}
