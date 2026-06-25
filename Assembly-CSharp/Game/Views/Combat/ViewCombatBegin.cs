using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Combat;
using Game.Views.Bottom;
using Game.Views.CharacterMenu;
using Game.Views.Select;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Organization;
using GameData.Domains.Story;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B36 RID: 2870
	public class ViewCombatBegin : UIBase
	{
		// Token: 0x17000F8C RID: 3980
		// (get) Token: 0x06008E3D RID: 36413 RVA: 0x00422656 File Offset: 0x00420856
		private GlobalSettings SettingData
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>();
			}
		}

		// Token: 0x17000F8D RID: 3981
		// (get) Token: 0x06008E3E RID: 36414 RVA: 0x0042265D File Offset: 0x0042085D
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000F8E RID: 3982
		// (get) Token: 0x06008E3F RID: 36415 RVA: 0x00422664 File Offset: 0x00420864
		private IReadOnlyList<int> SelfTeam
		{
			get
			{
				return this.Model.SelfTeam;
			}
		}

		// Token: 0x17000F8F RID: 3983
		// (get) Token: 0x06008E40 RID: 36416 RVA: 0x00422671 File Offset: 0x00420871
		private IReadOnlyList<int> EnemyTeam
		{
			get
			{
				return this.Model.EnemyTeam;
			}
		}

		// Token: 0x17000F90 RID: 3984
		// (get) Token: 0x06008E41 RID: 36417 RVA: 0x0042267E File Offset: 0x0042087E
		private TeammateCommandChangeData CommandChangeData
		{
			get
			{
				return this.Model.CommandChangeData;
			}
		}

		// Token: 0x17000F91 RID: 3985
		// (get) Token: 0x06008E42 RID: 36418 RVA: 0x0042268B File Offset: 0x0042088B
		private IReadOnlyDictionary<int, CharacterDisplayData> _charDisplayDataDict
		{
			get
			{
				return this.Model.DisplayDataCache;
			}
		}

		// Token: 0x06008E43 RID: 36419 RVA: 0x00422698 File Offset: 0x00420898
		public override void OnInit(ArgumentBox argsBox)
		{
			this.DisableWeaponIcon();
			this._currSelectIndex.Clear();
			this._yuanshanThreeVitalsIdList = this.BuildingModel.GetYuanshanThreeVitalsIdList();
			this._yuanshanOppositeThreeVitalsIdList = this.BuildingModel.GetYuanshanOppositeThreeVitalsIdList();
			this._storyTeammateIdList.Clear();
			this._storyTeammateIdList.AddRange(this._yuanshanThreeVitalsIdList);
			CharacterDisplayData ironPlateCombatCharData = this.WorldMapModel.IronPlateCombatCharData;
			this._ironPlateCombatCharId = ((ironPlateCombatCharData != null) ? ironPlateCombatCharData.CharacterId : -1);
			bool flag = this._ironPlateCombatCharId >= 0;
			if (flag)
			{
				this._storyTeammateIdList.Add(this._ironPlateCombatCharId);
			}
			GEvent.OnEvent(UiEvents.HideMapBlockCharList, null);
			this.Awake();
			this.RenderStatics();
			CombatAiOptions.SyncToBackend();
			this.Model.RequestSimulatePrepareCombat(new AsyncMethodCallbackDelegate(this.HandlerSimulatePrepareCombat));
			this._prepareFinished = false;
			UIElement combat = UIElement.Combat;
			combat.OnShowed = (Action)Delegate.Combine(combat.OnShowed, new Action(this.PlayEndAni));
			this.NeedWaitData = true;
			for (int i = 0; i < 3; i++)
			{
				CombatBeginTeammateSelf selfTeammate = this._selfTeammateHolder.GetChild(i).GetComponent<CombatBeginTeammateSelf>();
				CombatBeginTeammateEnemy enemyTeammate = this._enemyTeammateHolder.GetChild(i).GetComponent<CombatBeginTeammateEnemy>();
				selfTeammate.betrayBubble.gameObject.SetActive(false);
				selfTeammate.commandHolder.gameObject.SetActive(false);
				selfTeammate.commandBubble.gameObject.SetActive(false);
				enemyTeammate.betrayedTips0.gameObject.SetActive(false);
				enemyTeammate.betrayedTips1.gameObject.SetActive(false);
				enemyTeammate.betrayBubble.gameObject.SetActive(false);
				enemyTeammate.commandHolder.gameObject.SetActive(false);
				enemyTeammate.commandBubble.gameObject.SetActive(false);
			}
			UIElement element = this.Element;
			element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(this.OnShowed));
			UIElement element2 = this.Element;
			element2.OnListenerIdReady = (Action)Delegate.Combine(element2.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x06008E44 RID: 36420 RVA: 0x004228C4 File Offset: 0x00420AC4
		private void OnListenerIdReady()
		{
			this._listenerIdReady = true;
			this._pendingAsyncOps++;
			this.RefreshTaiwuCombatSkillPlan(true);
			this.readAndLoop.RequestData();
			this._pendingAsyncOps++;
			this.RefreshLoopReadCountText();
		}

		// Token: 0x06008E45 RID: 36421 RVA: 0x00422910 File Offset: 0x00420B10
		private IEnumerator Show()
		{
			yield return new WaitForEndOfFrame();
			this.maskImg.DOFade(0f, this.maskFadeTime).OnComplete(new TweenCallback(this.AfterShow));
			yield break;
		}

		// Token: 0x06008E46 RID: 36422 RVA: 0x00422920 File Offset: 0x00420B20
		private void OnShowed()
		{
			bool flag = SingletonObject.getInstance<TutorialChapterModel>().IsInTutorialChapter(6);
			if (flag)
			{
				TaiwuEventDomainMethod.Call.TriggerListener("CombatPrepareOnShowed", true);
			}
			AudioManager.Instance.PlayMusic("battle-really", 1f, 100, null);
		}

		// Token: 0x06008E47 RID: 36423 RVA: 0x00422964 File Offset: 0x00420B64
		private void Awake()
		{
			this.ResetMask();
			bool awakeDone = this._awakeDone;
			if (!awakeDone)
			{
				this._awakeDone = true;
				this._selfTeammateHolder = this.selfCharInfo.teammateHolder;
				this._enemyTeammateHolder = this.enemyCharInfo.teammateHolder;
				this._selfAvatars[0] = new CharacterAvatar(this.selfCharInfo.avatar, true);
				this._selfNames[0] = new CharacterName(this.selfCharInfo.charName, null, null);
				this._enemyAvatars[0] = new CharacterAvatar(this.enemyCharInfo.avatar, true);
				this._enemyNames[0] = new CharacterName(this.enemyCharInfo.charName, null, null);
				for (int i = 0; i < 3; i++)
				{
					CombatBeginTeammateSelf selfTeammate = this._selfTeammateHolder.GetChild(i).GetComponent<CombatBeginTeammateSelf>();
					CombatBeginTeammateEnemy enemyTeammate = this._enemyTeammateHolder.GetChild(i).GetComponent<CombatBeginTeammateEnemy>();
					this._selfAvatars[i + 1] = new CharacterAvatar(selfTeammate.avatar, true);
					this._selfNames[i + 1] = new CharacterName(selfTeammate.characterName, null, null);
					this._enemyAvatars[i + 1] = new CharacterAvatar(enemyTeammate.avatar, true);
					this._enemyNames[i + 1] = new CharacterName(enemyTeammate.characterName, null, null);
				}
				CButton selfCharBtn = this.selfCharInfo.openCharMenu;
				selfCharBtn.ClearAndAddListener(delegate
				{
					this.ShowCharMenu(true, this.SelfTeam[0]);
				});
				CButton enemyCharBtn = this.enemyCharInfo.openCharMenu;
				enemyCharBtn.ClearAndAddListener(delegate
				{
					this.ShowCharMenu(false, this.EnemyTeam[0]);
				});
				this.Model.AddEvent(ECombatEvents.BeginDataReady, new OnCombatEvent(this.OnBeginDataReady));
				this.Model.AddEvent(ECombatEvents.BeginTeammateCommandChanged, new OnCombatEvent(this.UpdateTeammateCommandByFinal));
				this.Model.AddEvent(ECombatEvents.OnDataReady, new OnCombatEvent(this.OnCombatDataReady));
				this.taiwuCombatSkillPlan.Init(-1);
				this.taiwuCombatSkillPlan.OnActiveIndexChange += this.HandleEquipPlanToggleChange;
				this.selfWeapon.Init(-1);
				this.selfWeapon.OnActiveIndexChange += this.SelfWeaponChange;
				this._startCombatAnimCtrl = this.startCombatAnimRectTs.GetChild(0).GetComponent<Animator>();
				this.startCombatAnimRectTs.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008E48 RID: 36424 RVA: 0x00422BB4 File Offset: 0x00420DB4
		private void OnDestroy()
		{
			this.Model.RemoveEvent(ECombatEvents.BeginDataReady, new OnCombatEvent(this.OnBeginDataReady));
			this.Model.RemoveEvent(ECombatEvents.BeginTeammateCommandChanged, new OnCombatEvent(this.UpdateTeammateCommandByFinal));
			this.Model.RemoveEvent(ECombatEvents.OnDataReady, new OnCombatEvent(this.OnCombatDataReady));
		}

		// Token: 0x06008E49 RID: 36425 RVA: 0x00422C10 File Offset: 0x00420E10
		private void OnEnable()
		{
			this.startCombatAnimRectTs.gameObject.SetActive(false);
			base.transform.Find("Content").gameObject.SetActive(true);
			this.selfCharInfo.openCharMenuTips.SetActive(false);
			this.enemyCharInfo.openCharMenuTips.SetActive(false);
			GEvent.Add(UiEvents.StopAutoStartCombat, new GEvent.Callback(this.EventStopAutoStartCombat));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			this._allowSelectMultiVitalDemon = false;
			bool taiwuInCombat = this.CombatModel.TaiwuInCombat;
			if (taiwuInCombat)
			{
				OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(this, 5, SectFunctionStatuses.SectFunctionStatusType.UpgradedInteractionUnlocked, new AsyncMethodCallbackDelegate(this.HandlerGetSectFunctionStatus));
			}
		}

		// Token: 0x06008E4A RID: 36426 RVA: 0x00422CCE File Offset: 0x00420ECE
		private void TopUiChanged(ArgumentBox argBox)
		{
			this.RefreshTaiwuCombatSkillPlan(false);
			this.RefreshWeapon();
			this.RefreshMouseTip();
		}

		// Token: 0x06008E4B RID: 36427 RVA: 0x00422CE8 File Offset: 0x00420EE8
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.StopAutoStartCombat, new GEvent.Callback(this.EventStopAutoStartCombat));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			this.ClearElementAndMonitor();
			this.DisableWeaponIcon();
			this.ResetMask();
			this.ResetInjuryMouseTip();
		}

		// Token: 0x06008E4C RID: 36428 RVA: 0x00422D44 File Offset: 0x00420F44
		private void ResetInjuryMouseTip()
		{
			TooltipInvoker selfInjuryTip = this.selfCharInfo.injuryMouseTip;
			CombatUtils.UpdateInjuryTips(selfInjuryTip, -1);
			TooltipInvoker enemyInjuryTip = this.enemyCharInfo.injuryMouseTip;
			CombatUtils.UpdateInjuryTips(enemyInjuryTip, -1);
		}

		// Token: 0x06008E4D RID: 36429 RVA: 0x00422D7A File Offset: 0x00420F7A
		private void ResetMask()
		{
			this.maskImg.gameObject.SetActive(true);
			this.maskImg.SetAlpha(1f);
			this.maskImg.raycastTarget = true;
		}

		// Token: 0x06008E4E RID: 36430 RVA: 0x00422DB0 File Offset: 0x00420FB0
		public void Update()
		{
			bool waitOpenCharacterNeili = SingletonObject.getInstance<TutorialChapterModel>().WaitOpenCharacterNeili;
			if (!waitOpenCharacterNeili)
			{
				bool flag = Input.GetKeyUp(KeyCode.Space) && UIManager.Instance.IsFocusElement(UIElement.CombatBegin);
				if (flag)
				{
					this.ShowCombatUi();
				}
			}
		}

		// Token: 0x06008E4F RID: 36431 RVA: 0x00422DF4 File Offset: 0x00420FF4
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = btnName == "StartCombatBtn";
			if (flag)
			{
				bool waitOpenCharacterNeili = SingletonObject.getInstance<TutorialChapterModel>().WaitOpenCharacterNeili;
				if (!waitOpenCharacterNeili)
				{
					AudioManager.Instance.PlaySound("battle_BattleButton", false, false);
					this.ShowCombatUi();
				}
			}
		}

		// Token: 0x06008E50 RID: 36432 RVA: 0x00422E44 File Offset: 0x00421044
		private void RenderStatics()
		{
			this.combatType.SetSprite(string.Format("ui9_icon_combat_type_{0}_{1}", (LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? "cn" : "en", this.Model.Config.CombatType), false, null);
			this.startIconCn.gameObject.SetActive(LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN);
			this.startIconEn.gameObject.SetActive(LocalStringManager.CurLanguageType > LocalStringManager.LanguageType.CN);
		}

		// Token: 0x06008E51 RID: 36433 RVA: 0x00422EC4 File Offset: 0x004210C4
		private void HandlerSimulatePrepareCombat(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._prepareCombatResult);
			this.RefreshFirstMove((EPrepareCombatResult)this._prepareCombatResult, false);
		}

		// Token: 0x06008E52 RID: 36434 RVA: 0x00422EE3 File Offset: 0x004210E3
		private void RefreshFirstMove(EPrepareCombatResult prepareCombatResult, bool noTip = false)
		{
			this.allyFirstMove.Set(prepareCombatResult, noTip);
			this.enemyFirstMove.Set(prepareCombatResult, noTip);
		}

		// Token: 0x06008E53 RID: 36435 RVA: 0x00422F04 File Offset: 0x00421104
		private void OnBeginDataReady()
		{
			this.UpdateTeammateCommand();
			this.InitElementAndMonitor();
			this.RefreshWeapon();
			foreach (CombatBeginWisdom wisdomComponent in this.wisdomComponents)
			{
				wisdomComponent.DoRequest();
			}
			CharacterDisplayData enemyData;
			bool flag = this._charDisplayDataDict.TryGetValue(this.EnemyTeam[0], out enemyData);
			if (flag)
			{
				this.combatConfigTips.SetTips((int)enemyData.TemplateId);
			}
			this._beginDataReady = true;
			this.TryShowElement();
		}

		// Token: 0x06008E54 RID: 36436 RVA: 0x00422F88 File Offset: 0x00421188
		private void TryShowElement()
		{
			bool flag = this._pendingAsyncOps > 0 || !this._beginDataReady || !this._listenerIdReady;
			if (!flag)
			{
				this.Element.ShowAfterRefresh();
				base.StartCoroutine(this.Show());
			}
		}

		// Token: 0x06008E55 RID: 36437 RVA: 0x00422FD4 File Offset: 0x004211D4
		private void AfterShow()
		{
			bool flag = !this.Model.Config.AllowPrepare;
			if (flag)
			{
				this.ShowCombatUi();
			}
			else
			{
				this.maskImg.raycastTarget = false;
			}
			this.PlayCommandAnim();
			this.PlayBetrayedThreeVitalsAnim();
			this.RefreshWeapon();
			this.CheckGuiding();
		}

		// Token: 0x06008E56 RID: 36438 RVA: 0x0042302B File Offset: 0x0042122B
		private void HandlerGetSectFunctionStatus(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._allowSelectMultiVitalDemon);
			this.AutoSetYuanshanTeammate();
		}

		// Token: 0x06008E57 RID: 36439 RVA: 0x00423044 File Offset: 0x00421244
		private void InitElementAndMonitor()
		{
			this.RefreshTeammateInfo();
			this._enemyNames[0].Anonymous = this.Model.Config.EnemyAnonymous;
			this._selfEquipSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EquipCombatSkillMonitor>(this.SelfTeam[0], false);
			this._selfEquipSkillMonitor.AddConsummateLevelListener(new Action(this.OnSelfConsummateLevelChange));
			this._selfEquipSkillMonitor.AddNeiliTypeListener(new Action(this.OnSelfNeiliTypeChange));
			this._selfEquipSkillMonitor.AddNeiliAllocationListener(new Action(this.OnSelfNeiliAllocationChange));
			this._selfEquipSkillMonitor.AddAllocatedNeiliEffectsListener(new Action(this.OnSelfAllocatedNeiliEffectsChange));
			bool init = this._selfEquipSkillMonitor.Init;
			if (init)
			{
				this._selfEquipSkillMonitor.OnDataInit();
			}
			this._enemyEquipSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EquipCombatSkillMonitor>(this.EnemyTeam[0], false);
			this._enemyEquipSkillMonitor.AddConsummateLevelListener(new Action(this.OnEnemyConsummateLevelChange));
			this._enemyEquipSkillMonitor.AddNeiliTypeListener(new Action(this.OnEnemyNeiliTypeChange));
			this._enemyEquipSkillMonitor.AddNeiliAllocationListener(new Action(this.OnEnemyNeiliAllocationChange));
			this._enemyEquipSkillMonitor.AddAllocatedNeiliEffectsListener(new Action(this.OnEnemyAllocatedNeiliEffectsChange));
			bool init2 = this._enemyEquipSkillMonitor.Init;
			if (init2)
			{
				this._enemyEquipSkillMonitor.OnDataInit();
			}
		}

		// Token: 0x06008E58 RID: 36440 RVA: 0x004231A8 File Offset: 0x004213A8
		private void RefreshTeammateInfo()
		{
			for (int i = 0; i < 4; i++)
			{
				int selfCharId = (this.SelfTeam.Count > i) ? this.SelfTeam[i] : -1;
				int enemyCharId = (this.EnemyTeam.Count > i) ? this.EnemyTeam[i] : -1;
				bool flag = i > 0;
				if (flag)
				{
					int index = i - 1;
					Transform selfTeammate = this._selfTeammateHolder.GetChild(index);
					bool isInGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
					bool isShow = selfCharId >= 0 || (this.HasStoryTeammate && !isInGuiding);
					selfTeammate.gameObject.SetActive(isShow);
					CombatBeginTeammateSelf selfTeammateRefers = selfTeammate.GetComponent<CombatBeginTeammateSelf>();
					CombatDefeatMarkTotalCount selfTeammateMarkCount = selfTeammateRefers.defeatMarksBack;
					this.RefreshYuanshanThreeVitalsBtn(selfTeammateRefers, i);
					bool validCharId = selfCharId >= 0;
					TooltipInvoker tip = selfTeammateRefers.mouseTipDisplayer;
					tip.enabled = validCharId;
					bool flag2 = validCharId;
					if (flag2)
					{
						CombatDomainMethod.AsyncCall.GetDefeatMarksCountOutOfCombat(this, selfCharId, delegate(int offset, RawDataPool dataPool)
						{
							DefeatMarksCountOutOfCombatData markGroupData = new DefeatMarksCountOutOfCombatData();
							Serializer.Deserialize(dataPool, offset, ref markGroupData);
							selfTeammateMarkCount.Set(markGroupData.DefeatMarksDict);
						});
						CombatUtils.UpdateInjuryTips(tip, selfCharId);
					}
					CButton button = selfTeammateRefers.button;
					button.enabled = validCharId;
					button.interactable = validCharId;
					button.GetComponent<PointerTrigger>().enabled = validCharId;
					int targetCharId = selfCharId;
					button.ClearAndAddListener(delegate
					{
						this.ShowCharMenu(true, targetCharId);
					});
					selfTeammateMarkCount.gameObject.SetActive(validCharId);
					selfTeammateRefers.characterName.transform.parent.gameObject.SetActive(validCharId);
					selfTeammateRefers.avatar.gameObject.SetActive(validCharId);
					selfTeammateRefers.invalidAvatar.SetActive(!validCharId);
					this.<RefreshTeammateInfo>g__SetBtnBack|79_0(selfTeammateRefers.buttonFrame, this._storyTeammateIdList, selfCharId, true);
					Transform enemyTeammate = this._enemyTeammateHolder.GetChild(index);
					enemyTeammate.gameObject.SetActive(enemyCharId >= 0);
					CombatBeginTeammateEnemy enemyTeammateRefers = enemyTeammate.GetComponent<CombatBeginTeammateEnemy>();
					TooltipInvoker tip2 = enemyTeammateRefers.mouseTipDisplayer;
					tip2.enabled = (enemyCharId >= 0);
					bool flag3 = enemyCharId >= 0;
					if (flag3)
					{
						CombatDefeatMarkTotalCount enemyTeammateMarkCount = enemyTeammateRefers.defeatMarksBack;
						CombatDomainMethod.AsyncCall.GetDefeatMarksCountOutOfCombat(this, enemyCharId, delegate(int offset, RawDataPool dataPool)
						{
							DefeatMarksCountOutOfCombatData markGroupData = new DefeatMarksCountOutOfCombatData();
							Serializer.Deserialize(dataPool, offset, ref markGroupData);
							enemyTeammateMarkCount.Set(markGroupData.DefeatMarksDict);
						});
						CombatUtils.UpdateInjuryTips(tip2, enemyCharId);
						this.<RefreshTeammateInfo>g__SetBtnBack|79_0(enemyTeammateRefers.buttonFrame, this._yuanshanOppositeThreeVitalsIdList, enemyCharId, false);
					}
				}
				else
				{
					TooltipInvoker selfInjuryTip = this.selfCharInfo.injuryMouseTip;
					CombatUtils.UpdateInjuryTips(selfInjuryTip, selfCharId);
					TooltipInvoker enemyInjuryTip = this.enemyCharInfo.injuryMouseTip;
					CombatUtils.UpdateInjuryTips(enemyInjuryTip, enemyCharId);
				}
				this._selfAvatars[i].CharacterId = selfCharId;
				this._selfNames[i].CharacterId = selfCharId;
				this._enemyAvatars[i].CharacterId = enemyCharId;
				this._enemyNames[i].CharacterId = enemyCharId;
			}
		}

		// Token: 0x06008E59 RID: 36441 RVA: 0x0042349C File Offset: 0x0042169C
		private void ClearElementAndMonitor()
		{
			for (int i = 0; i < 4; i++)
			{
				this._selfAvatars[i].CharacterId = -1;
				this._selfNames[i].CharacterId = -1;
				this._enemyAvatars[i].CharacterId = -1;
				this._enemyNames[i].CharacterId = -1;
			}
			bool flag = this._selfEquipSkillMonitor != null;
			if (flag)
			{
				this._selfEquipSkillMonitor.RemoveConsummateLevelListener(new Action(this.OnSelfConsummateLevelChange));
				this._selfEquipSkillMonitor.RemoveNeiliTypeListener(new Action(this.OnSelfNeiliTypeChange));
				this._selfEquipSkillMonitor.RemoveNeiliAllocationListener(new Action(this.OnSelfNeiliAllocationChange));
				this._selfEquipSkillMonitor.RemoveAllocatedNeiliEffectsListener(new Action(this.OnSelfAllocatedNeiliEffectsChange));
				this._selfEquipSkillMonitor = null;
			}
			bool flag2 = this._enemyEquipSkillMonitor != null;
			if (flag2)
			{
				this._enemyEquipSkillMonitor.RemoveConsummateLevelListener(new Action(this.OnEnemyConsummateLevelChange));
				this._enemyEquipSkillMonitor.RemoveNeiliTypeListener(new Action(this.OnEnemyNeiliTypeChange));
				this._enemyEquipSkillMonitor.RemoveNeiliAllocationListener(new Action(this.OnEnemyNeiliAllocationChange));
				this._enemyEquipSkillMonitor.RemoveAllocatedNeiliEffectsListener(new Action(this.OnEnemyAllocatedNeiliEffectsChange));
				this._enemyEquipSkillMonitor = null;
			}
		}

		// Token: 0x06008E5A RID: 36442 RVA: 0x004235E4 File Offset: 0x004217E4
		private void OnSelfConsummateLevelChange()
		{
			this.UpdateConsummateLevel(true);
		}

		// Token: 0x06008E5B RID: 36443 RVA: 0x004235EF File Offset: 0x004217EF
		private void OnEnemyConsummateLevelChange()
		{
			this.UpdateConsummateLevel(false);
		}

		// Token: 0x06008E5C RID: 36444 RVA: 0x004235FC File Offset: 0x004217FC
		private void UpdateConsummateLevel(bool isAlly)
		{
			sbyte consummateLevel = (isAlly ? this._selfEquipSkillMonitor : this._enemyEquipSkillMonitor).ConsummateLevel;
			CombatBeginCharacterInfo infoRefers = isAlly ? this.selfCharInfo : this.enemyCharInfo;
			CImage consummateIcon = infoRefers.consummateIcon;
			infoRefers.consummateLevel.text = consummateLevel.ToString();
			consummateIcon.SetSprite(CommonUtils.GetConsummateLevelShowDataLegacy(consummateLevel).Item1, false, null);
			consummateIcon.GetComponent<TooltipInvoker>().PresetParam[0] = string.Format("{0}{1}{2}", LocalStringManager.Get(LanguageKey.LK_Consummate_Level), LocalStringManager.Get(LanguageKey.LK_Colon_Symbol), consummateLevel);
			bool flag = this._selfEquipSkillMonitor != null && this._enemyEquipSkillMonitor != null;
			if (flag)
			{
				this.combatConfigTips.SetConSummateLevel(this._selfEquipSkillMonitor.ConsummateLevel, this._enemyEquipSkillMonitor.ConsummateLevel);
			}
		}

		// Token: 0x06008E5D RID: 36445 RVA: 0x004236CC File Offset: 0x004218CC
		private void OnSelfNeiliTypeChange()
		{
			this.UpdateNeiliTypeAndAllocation(true);
		}

		// Token: 0x06008E5E RID: 36446 RVA: 0x004236D7 File Offset: 0x004218D7
		private void OnEnemyNeiliTypeChange()
		{
			this.UpdateNeiliTypeAndAllocation(false);
		}

		// Token: 0x06008E5F RID: 36447 RVA: 0x004236E2 File Offset: 0x004218E2
		private void OnSelfNeiliAllocationChange()
		{
			this.UpdateNeiliTypeAndAllocation(true);
		}

		// Token: 0x06008E60 RID: 36448 RVA: 0x004236ED File Offset: 0x004218ED
		private void OnEnemyNeiliAllocationChange()
		{
			this.UpdateNeiliTypeAndAllocation(false);
		}

		// Token: 0x06008E61 RID: 36449 RVA: 0x004236F8 File Offset: 0x004218F8
		private void OnSelfAllocatedNeiliEffectsChange()
		{
			this.UpdateNeiliTypeAndAllocation(true);
		}

		// Token: 0x06008E62 RID: 36450 RVA: 0x00423703 File Offset: 0x00421903
		private void OnEnemyAllocatedNeiliEffectsChange()
		{
			this.UpdateNeiliTypeAndAllocation(false);
		}

		// Token: 0x06008E63 RID: 36451 RVA: 0x00423710 File Offset: 0x00421910
		private unsafe void UpdateNeiliTypeAndAllocation(bool isAlly)
		{
			EquipCombatSkillMonitor dataMonitor = isAlly ? this._selfEquipSkillMonitor : this._enemyEquipSkillMonitor;
			sbyte neiliType = dataMonitor.NeiliType;
			NeiliAllocation neiliAllocation = dataMonitor.NeiliAllocation;
			NeiliTypeItem typeConfig = NeiliType.Instance[neiliType];
			CombatBeginCharacterInfo infoRefers = isAlly ? this.selfCharInfo : this.enemyCharInfo;
			RectTransform neiliAllocationHolder = infoRefers.neiliAllocationHolder;
			CImage typeBack = infoRefers.neiliTypeBack;
			TooltipInvoker typeTips = typeBack.GetComponent<TooltipInvoker>();
			typeBack.SetSprite(string.Format("ui9_icon_neili_{0}", dataMonitor.NeiliType), false, null);
			typeTips.PresetParam[0] = typeConfig.Name;
			typeTips.PresetParam[1] = typeConfig.Desc.ColorReplace();
			infoRefers.neiliType.text = CommonUtils.GetNeiliTypeName(dataMonitor.NeiliType);
			infoRefers.neiliProgress.fillAmount = ((dataMonitor.MaxNeili > 0) ? ((float)dataMonitor.CurrNeili / (float)dataMonitor.MaxNeili) : 0f);
			for (byte i = 0; i < 4; i += 1)
			{
				short value = *(ref neiliAllocation.Items.FixedElementField + (IntPtr)i * 2);
				Refers neiliAllocationRefers = neiliAllocationHolder.GetChild((int)i).GetComponent<Refers>();
				neiliAllocationRefers.CGet<TextMeshProUGUI>("OrigValue").text = value.ToString();
				neiliAllocationRefers.CGet<TextMeshProUGUI>("CurrValue").text = value.ToString();
				short realValue = *(ref dataMonitor.AllocatedNeiliEffects.Items.FixedElementField + (IntPtr)i * 2);
				TooltipInvoker component = neiliAllocationRefers.GetComponent<TooltipInvoker>();
				ArgumentBox argumentBox;
				if ((argumentBox = component.RuntimeParam) == null)
				{
					argumentBox = (component.RuntimeParam = new ArgumentBox());
				}
				ArgumentBox param = argumentBox;
				param.Clear();
				param.Set("neiliType", neiliType);
				param.Set("current", value);
				param.Set("origin", value);
				param.Set("effect", realValue);
				param.Set("type", i);
			}
		}

		// Token: 0x06008E64 RID: 36452 RVA: 0x0042390C File Offset: 0x00421B0C
		private void UpdateTeammateCommand()
		{
			int negativeCharCount = 0;
			for (int i = 0; i < 3; i++)
			{
				CombatBeginTeammateSelf selfTeammate = this._selfTeammateHolder.GetChild(i).GetComponent<CombatBeginTeammateSelf>();
				CombatBeginTeammateEnemy enemyTeammate = this._enemyTeammateHolder.GetChild(i).GetComponent<CombatBeginTeammateEnemy>();
				List<sbyte> selfData = this.CommandChangeData.LeftTeam.OriginTeammateCommands.GetOrDefault(i).Items;
				List<sbyte> enemyData = this.CommandChangeData.RightTeam.OriginTeammateCommands.GetOrDefault(i).Items;
				this.UpdateTeammateCommand(selfTeammate.commandHolder, selfData, false);
				this.UpdateTeammateCommand(enemyTeammate.commandHolder, enemyData, false);
				bool flag = !this.Model.TaiwuInCombat;
				if (!flag)
				{
					List<sbyte> leftCmdTypes = this.CommandChangeData.LeftTeam.ReplaceTeammateCommands.GetOrDefault(i).Items;
					bool flag2 = leftCmdTypes == null || leftCmdTypes.Count <= 0;
					if (!flag2)
					{
						bool flag3 = leftCmdTypes.Any((sbyte cmdType) => cmdType >= 0 && this.IsNegativeCommand(cmdType));
						if (flag3)
						{
							negativeCharCount++;
						}
					}
				}
			}
			bool flag4 = negativeCharCount > 0;
			if (flag4)
			{
				WorldDomainMethod.Call.RequestSetStat(126, negativeCharCount);
			}
		}

		// Token: 0x06008E65 RID: 36453 RVA: 0x00423A34 File Offset: 0x00421C34
		private void UpdateTeammateCommandByFinal()
		{
			for (int i = 0; i < 3; i++)
			{
				CombatBeginTeammateSelf selfTeammate = this._selfTeammateHolder.GetChild(i).GetComponent<CombatBeginTeammateSelf>();
				CombatBeginTeammateEnemy enemyTeammate = this._enemyTeammateHolder.GetChild(i).GetComponent<CombatBeginTeammateEnemy>();
				List<sbyte> selfData = this.CommandChangeData.LeftTeam.ReplaceTeammateCommands.GetOrDefault(i).Items;
				List<sbyte> enemyData = this.CommandChangeData.RightTeam.ReplaceTeammateCommands.GetOrDefault(i).Items;
				this.UpdateTeammateCommand(selfTeammate.commandHolder, selfData, false);
				this.UpdateTeammateCommand(enemyTeammate.commandHolder, enemyData, false);
			}
		}

		// Token: 0x06008E66 RID: 36454 RVA: 0x00423AD8 File Offset: 0x00421CD8
		private void UpdateTeammateCommand(Transform commandHolder, IReadOnlyList<sbyte> cmdTypes, bool swap = false)
		{
			for (int i = 0; i < 3; i++)
			{
				int cmdType = (int)((cmdTypes != null && i < cmdTypes.Count) ? cmdTypes[i] : -1);
				CharacterTeammateCommand cmdRefers = commandHolder.GetChild(i).GetComponent<CharacterTeammateCommand>();
				if (swap)
				{
					cmdRefers.AnimationTo(cmdType);
				}
				else
				{
					cmdRefers.Set(cmdType, true);
				}
			}
			commandHolder.gameObject.SetActive(true);
		}

		// Token: 0x06008E67 RID: 36455 RVA: 0x00423B44 File Offset: 0x00421D44
		private void PlayBetrayedThreeVitalsAnim()
		{
			Dictionary<int, int> betrayedCharIds = this.CommandChangeData.RightTeam.BetrayedCharIds;
			for (int i = 0; i < 3; i++)
			{
				int charId;
				bool flag = betrayedCharIds.TryGetValue(i, out charId);
				if (flag)
				{
					CombatBeginTeammateEnemy enemyTeammate = this._enemyTeammateHolder.GetChild(i).GetComponent<CombatBeginTeammateEnemy>();
					CombatDefeatMarkTotalCount enemyTeammateMarkCount = enemyTeammate.defeatMarksBack;
					CombatDomainMethod.AsyncCall.GetDefeatMarksCountOutOfCombat(this, charId, delegate(int offset, RawDataPool dataPool)
					{
						DefeatMarksCountOutOfCombatData markGroupData = new DefeatMarksCountOutOfCombatData();
						Serializer.Deserialize(dataPool, offset, ref markGroupData);
						enemyTeammateMarkCount.Set(markGroupData.DefeatMarksDict);
						ParticleSystem particle = enemyTeammate.GetEffectYuanshanThreeDemon(this.BuildingModel.AreVitalsDemon(), true);
						this.SetTargetTexture(charId, particle);
						this.PlaySwitchThreeVitalsAnim(particle, true);
					});
				}
			}
		}

		// Token: 0x06008E68 RID: 36456 RVA: 0x00423BF8 File Offset: 0x00421DF8
		private void PlayCommandAnim()
		{
			List<float> selfDelayPool = EasyPool.Get<List<float>>();
			List<float> enemyDelayPool = EasyPool.Get<List<float>>();
			for (int i = 0; i < 3; i++)
			{
				selfDelayPool.Add(this._commandBubbleDelay + (float)i * this._commandBubbleDelayFixedExtra + Random.Range(-this._commandBubbleDelayRandom, this._commandBubbleDelayRandom));
				enemyDelayPool.Add(this._commandBubbleDelay + (float)i * this._commandBubbleDelayFixedExtra + Random.Range(-this._commandBubbleDelayRandom, this._commandBubbleDelayRandom));
			}
			for (int j = 0; j < 3; j++)
			{
				CombatBeginTeammateSelf selfTeammate = this._selfTeammateHolder.GetChild(j).GetComponent<CombatBeginTeammateSelf>();
				CombatBeginTeammateEnemy enemyTeammate = this._enemyTeammateHolder.GetChild(j).GetComponent<CombatBeginTeammateEnemy>();
				List<sbyte> selfData = this.CommandChangeData.LeftTeam.ReplaceTeammateCommands.GetOrDefault(j).Items;
				List<sbyte> enemyData = this.CommandChangeData.RightTeam.ReplaceTeammateCommands.GetOrDefault(j).Items;
				bool flag = this._currSelectIndex.Contains(j + 1);
				if (flag)
				{
					this.UpdateYuanshanTeammateCommand(j);
				}
				else
				{
					this.UpdateSelfTeammateCommand(j);
				}
				this.UpdateTeammateCommand(enemyTeammate.commandHolder, enemyData, true);
				float selfDelay = selfDelayPool.GetRandom<float>();
				float enemyDelay = enemyDelayPool.GetRandom<float>();
				selfDelayPool.Remove(selfDelay);
				enemyDelayPool.Remove(enemyDelay);
				this.PlayCommandBubble(j, true, selfData != null && selfData.Any(new Func<sbyte, bool>(this.IsNegativeCommand)), selfDelay);
				this.PlayCommandBubble(j, false, enemyData != null && enemyData.Any(new Func<sbyte, bool>(this.IsNegativeCommand)), enemyDelay);
			}
			EasyPool.Free<List<float>>(selfDelayPool);
			EasyPool.Free<List<float>>(enemyDelayPool);
		}

		// Token: 0x06008E69 RID: 36457 RVA: 0x00423DB0 File Offset: 0x00421FB0
		private void PlaySingleSelfTeammateCommandBubble(int index)
		{
			List<sbyte> selfData = this.CommandChangeData.LeftTeam.ReplaceTeammateCommands.GetOrDefault(index).Items;
			this.PlayCommandBubble(index, true, selfData != null && selfData.Any(new Func<sbyte, bool>(this.IsNegativeCommand)), this._commandBubbleDelay);
		}

		// Token: 0x06008E6A RID: 36458 RVA: 0x00423E01 File Offset: 0x00422001
		private bool IsNegativeCommand(sbyte arg)
		{
			return arg >= 0 && TeammateCommand.Instance[arg].Type == ETeammateCommandType.Negative;
		}

		// Token: 0x06008E6B RID: 36459 RVA: 0x00423E20 File Offset: 0x00422020
		private void PlayCommandBubble(int i, bool isAlly, bool negative, float delay)
		{
			Bubble bubble = isAlly ? this._selfTeammateHolder.GetChild(i).GetComponent<CombatBeginTeammateSelf>().commandBubble : this._enemyTeammateHolder.GetChild(i).GetComponent<CombatBeginTeammateEnemy>().commandBubble;
			BaseCombatBeginTeammate baseCombatBeginTeammate = isAlly ? this._selfTeammateHolder.GetChild(i).GetComponent<BaseCombatBeginTeammate>() : this._enemyTeammateHolder.GetChild(i).GetComponent<BaseCombatBeginTeammate>();
			IReadOnlyList<int> team = isAlly ? this.SelfTeam : this.EnemyTeam;
			int teamIndex = i + 1;
			bool flag = team == null || teamIndex < 0 || teamIndex >= team.Count;
			if (flag)
			{
				baseCombatBeginTeammate.ClearSequenceAnim();
				bubble.gameObject.SetActive(false);
			}
			else
			{
				int charId = team[teamIndex];
				bool flag2 = charId < 0;
				if (flag2)
				{
					baseCombatBeginTeammate.ClearSequenceAnim();
					bubble.gameObject.SetActive(false);
				}
				else
				{
					CharacterDisplayData charData = this._charDisplayDataDict[charId];
					string text = this.GetCommandBubbleText(i, isAlly, negative, charData);
					baseCombatBeginTeammate.PlayCommandBubbleAnim(delay, delegate
					{
						bubble.SetText(text, true);
						bubble.GetComponent<CImage>().SetSprite(negative ? "ui9_back_bubblebase_1" : "ui9_back_bubblebase_0", false, null);
						bubble.gameObject.SetActive(true);
					}, 5f, null);
				}
			}
		}

		// Token: 0x06008E6C RID: 36460 RVA: 0x00423F58 File Offset: 0x00422158
		private string GetCommandBubbleText(int i, bool isAlly, bool negative, CharacterDisplayData charData)
		{
			bool flag = !isAlly && i < this.Model.Config.SpecialTeammateCommandBubbleTexts.Length;
			string result;
			if (flag)
			{
				result = this.Model.Config.SpecialTeammateCommandBubbleTexts[i];
			}
			else
			{
				CharacterItem config = Character.Instance[charData.TemplateId];
				string muteText = isAlly ? config.SpecialMuteBubbleSelf : config.SpecialMuteBubbleEnemy;
				bool canNotSpeak = charData.CanNotSpeak;
				if (canNotSpeak)
				{
					result = (muteText.IsNullOrEmpty() ? LocalStringManager.Get(LanguageKey.LK_TeammateCommand_Mute) : muteText);
				}
				else
				{
					result = LocalStringManager.Get(string.Format("LK_TeammateCommand_{0}_{1}", negative ? "Bad" : "Good", charData.BehaviorType));
				}
			}
			return result;
		}

		// Token: 0x06008E6D RID: 36461 RVA: 0x00424011 File Offset: 0x00422211
		private void EventStopAutoStartCombat(ArgumentBox box)
		{
		}

		// Token: 0x06008E6E RID: 36462 RVA: 0x00424014 File Offset: 0x00422214
		private void ShowCharMenu(bool isAlly, int targetCharacterId = -1)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("CharacterIdList", (from x in isAlly ? this.SelfTeam : this.EnemyTeam
			where x >= 0
			select x).ToList<int>());
			argBox.Set("CanOperate", isAlly);
			argBox.Set("OpenFromCombatPrepare", true);
			argBox.Set("Anonymous", !isAlly && this.Model.Config.EnemyAnonymous);
			argBox.Set("PreviousView", 0);
			bool flag = targetCharacterId != -1;
			if (flag)
			{
				argBox.Set("CharacterId", targetCharacterId);
				argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.Character));
				bool flag2 = this.CommandChangeData != null;
				if (flag2)
				{
					Dictionary<int, List<sbyte>> dict = new Dictionary<int, List<sbyte>>();
					for (int i = 0; i < this.CommandChangeData.LeftTeam.TeammateCharIds.Count; i++)
					{
						dict.Add(this.CommandChangeData.LeftTeam.TeammateCharIds[i], this.CommandChangeData.LeftTeam.ReplaceTeammateCommands[i].Items);
					}
					argBox.SetObject("ReplacedTeammateCommands", dict);
				}
			}
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06008E6F RID: 36463 RVA: 0x00424194 File Offset: 0x00422394
		private void ShowCombatUi()
		{
			bool flag = this._prepareFinished || SingletonObject.getInstance<TutorialChapterModel>().WaitOpenCharacterNeili;
			if (!flag)
			{
				bool flag2 = !this.IsDisplayDataReady();
				if (!flag2)
				{
					this.maskImg.raycastTarget = true;
					this._prepareFinished = true;
					for (int i = 0; i < 4; i++)
					{
						bool flag3 = this._selfAvatars[i].CharacterId >= 0;
						if (flag3)
						{
							this._selfAvatars[i].UnbindEvent();
						}
						bool flag4 = this._enemyAvatars[i].CharacterId >= 0;
						if (flag4)
						{
							this._enemyAvatars[i].UnbindEvent();
						}
					}
					this.startCombatAnimRectTs.gameObject.SetActive(true);
					AudioManager.Instance.PlaySound("battle_FlagLoop", true, false);
					AudioManager.Instance.PlaySound("battle_FlagWhoosh", false, false);
					AudioManager.Instance.PlayMusic(string.Empty, 1f, 100, null);
					Sequence seq = DOTween.Sequence();
					seq.AppendInterval(1.3f);
					seq.AppendCallback(delegate
					{
						this.Model.RequestPrepareCombat();
						this.selfCharInfo.openCharMenuTips.SetActive(false);
						this.enemyCharInfo.openCharMenuTips.SetActive(false);
						Debug.Log("#CombatBegin# stack to StateCombat");
						UIManager.Instance.RemoveElement(UIElement.CombatBegin);
						UIManager.Instance.StackToUI(UIElement.StateCombat);
					});
					seq.SetUpdate(UpdateType.Late, true);
					seq.Restart(true, -1f);
				}
			}
		}

		// Token: 0x06008E70 RID: 36464 RVA: 0x004242D1 File Offset: 0x004224D1
		private void PlayEndAni()
		{
		}

		// Token: 0x06008E71 RID: 36465 RVA: 0x004242D4 File Offset: 0x004224D4
		private bool IsDisplayDataReady()
		{
			IReadOnlyDictionary<int, CharacterDisplayData> cache = this._charDisplayDataDict;
			foreach (int charId in this.SelfTeam)
			{
				bool flag = charId >= 0 && !cache.ContainsKey(charId);
				if (flag)
				{
					return false;
				}
			}
			foreach (int charId2 in this.EnemyTeam)
			{
				bool flag2 = charId2 >= 0 && !cache.ContainsKey(charId2);
				if (flag2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06008E72 RID: 36466 RVA: 0x004243A0 File Offset: 0x004225A0
		private void OnCombatDataReady()
		{
			GLog.TagLog("ViewCombatBegin", "Combat data ready, start transition to combat", Array.Empty<object>());
			SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.WaitForTransitionToCombat());
		}

		// Token: 0x06008E73 RID: 36467 RVA: 0x004243C9 File Offset: 0x004225C9
		private IEnumerator WaitForTransitionToCombat()
		{
			WaitForEndOfFrame waitForframeEnd = new WaitForEndOfFrame();
			GLog.TagLog("ViewCombatBegin", "Waiting for transition animation to play out", Array.Empty<object>());
			int num;
			for (int i = 0; i < 30; i = num + 1)
			{
				yield return waitForframeEnd;
				num = i;
			}
			bool flag = this._startCombatAnimCtrl != null;
			if (flag)
			{
				this._startCombatAnimCtrl.SetTrigger("BeginDataReady");
			}
			else
			{
				GLog.TagWarn("ViewCombatBegin", "StartCombatAnimCtrl is null", Array.Empty<object>());
			}
			AudioManager.Instance.StopSound("battle_FlagLoop");
			AudioManager.Instance.PlaySound("battle_FlagWhoosh", false, false);
			yield return new WaitForSecondsRealtime(0.19f);
			yield return waitForframeEnd;
			Transform content = base.transform.Find("Content");
			bool flag2 = content != null;
			if (flag2)
			{
				content.gameObject.SetActive(false);
			}
			else
			{
				GLog.TagWarn("ViewCombatBegin", "Content transform is null", Array.Empty<object>());
			}
			yield return new WaitForSecondsRealtime(1f);
			yield return waitForframeEnd;
			this.ClearElementAndMonitor();
			this.Model.RaiseEvent(ECombatEvents.CombatBeginReady);
			this.Element.Hide(true);
			yield break;
		}

		// Token: 0x06008E74 RID: 36468 RVA: 0x004243D8 File Offset: 0x004225D8
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(8, 39, ulong.MaxValue, null));
		}

		// Token: 0x06008E75 RID: 36469 RVA: 0x004243F4 File Offset: 0x004225F4
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b != 1)
					{
					}
				}
				else
				{
					DataUid uid = notification.Uid;
					bool flag = uid.DomainId == 8 && uid.DataId == 39;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._preferWeaponIndex);
						this.selfWeapon.SetWithoutNotify(this._preferWeaponIndex);
					}
				}
			}
		}

		// Token: 0x06008E76 RID: 36470 RVA: 0x004244C0 File Offset: 0x004226C0
		private void RefreshMouseTip()
		{
			this.RefreshMouseTip(true);
			this.RefreshMouseTip(false);
		}

		// Token: 0x06008E77 RID: 36471 RVA: 0x004244D4 File Offset: 0x004226D4
		private void RefreshMouseTip(bool self)
		{
			int charId = self ? this.SelfTeam[0] : this.EnemyTeam[0];
			CombatSkillDomainMethod.AsyncCall.GetCharacterEquipNeigongBreakList(this, charId, delegate(int offset, RawDataPool dataPool)
			{
				List<short> neigongList = new List<short>();
				Serializer.Deserialize(dataPool, offset, ref neigongList);
				TooltipInvoker neiGongMouseTip = self ? this.selfNeigong : this.enemyNeigong;
				CombatBeginCharacterInfo infoRefers = self ? this.selfCharInfo : this.enemyCharInfo;
				infoRefers.txtNeiGong.SetText(neigongList.Count.ToString(), true);
				neiGongMouseTip.GetComponent<DisableStyleRoot>().SetStyleEffect(neigongList.Count <= 0, false);
				bool flag = neigongList.Count > 0;
				if (flag)
				{
					neiGongMouseTip.Type = TipType.CombatSkillBuff;
					TooltipInvoker tooltipInvoker = neiGongMouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					neiGongMouseTip.RuntimeParam.Clear();
					neiGongMouseTip.RuntimeParam.Set("IsAlly", self);
					neiGongMouseTip.RuntimeParam.Set("CombatBegin", true);
					neiGongMouseTip.RuntimeParam.Set("IsNeiGong", true);
					neiGongMouseTip.RuntimeParam.Set("CharId", charId);
					neiGongMouseTip.RuntimeParam.SetObject("CombatSkillIdList", neigongList);
				}
				else
				{
					neiGongMouseTip.Type = TipType.SingleDesc;
					TooltipInvoker tooltipInvoker = neiGongMouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					neiGongMouseTip.RuntimeParam.Clear();
					neiGongMouseTip.RuntimeParam.Set("arg0", LanguageKey.LK_Combat_Neigong_Empty_Tip.Tr());
				}
			});
			CombatSkillDomainMethod.AsyncCall.GetCharacterEquipAssistanceBreakList(this, charId, delegate(int offset, RawDataPool dataPool)
			{
				List<short> assistList = new List<short>();
				Serializer.Deserialize(dataPool, offset, ref assistList);
				TooltipInvoker juejiMouseTip = self ? this.selfJueji : this.enemyJueji;
				CombatBeginCharacterInfo infoRefers = self ? this.selfCharInfo : this.enemyCharInfo;
				infoRefers.txtJueJi.SetText(assistList.Count.ToString(), true);
				juejiMouseTip.GetComponent<DisableStyleRoot>().SetStyleEffect(assistList.Count <= 0, false);
				bool flag = assistList.Count > 0;
				if (flag)
				{
					juejiMouseTip.Type = TipType.CombatSkillBuff;
					TooltipInvoker tooltipInvoker = juejiMouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					juejiMouseTip.RuntimeParam.Clear();
					juejiMouseTip.RuntimeParam.Set("CombatBegin", true);
					juejiMouseTip.RuntimeParam.Set("IsAlly", self);
					juejiMouseTip.RuntimeParam.Set("IsNeiGong", false);
					juejiMouseTip.RuntimeParam.Set("CharId", charId);
					juejiMouseTip.RuntimeParam.SetObject("CombatSkillIdList", assistList);
				}
				else
				{
					juejiMouseTip.Type = TipType.SingleDesc;
					TooltipInvoker tooltipInvoker = juejiMouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					juejiMouseTip.RuntimeParam.Clear();
					juejiMouseTip.RuntimeParam.Set("arg0", LanguageKey.LK_Combat_Jueji_Empty_Tip.Tr());
				}
			});
		}

		// Token: 0x06008E78 RID: 36472 RVA: 0x00424550 File Offset: 0x00422750
		private void SelfWeaponChange(int newIndex, int arg2)
		{
			GameDataBridge.AddDataModification<int>(8, 39, ulong.MaxValue, uint.MaxValue, newIndex);
		}

		// Token: 0x06008E79 RID: 36473 RVA: 0x00424560 File Offset: 0x00422760
		private void RefreshWeapon()
		{
			bool flag = this._normalWeaponTips == null;
			if (flag)
			{
				this._normalWeaponTips = new List<TooltipInvoker>();
			}
			this._normalWeaponTips.Clear();
			CharacterDomainMethod.AsyncCall.GetAllEquipmentItems(this, this.SelfTeam[0], delegate(int offset, RawDataPool dataPool)
			{
				List<ItemDisplayData> equipments = EasyPool.Get<List<ItemDisplayData>>();
				Serializer.Deserialize(dataPool, offset, ref equipments);
				RectTransform parent = this.selfWeapon.GetComponent<RectTransform>();
				this.RefreshWeapon(parent, equipments, true);
			});
			CharacterDomainMethod.AsyncCall.GetAllEquipmentItems(this, this.EnemyTeam[0], delegate(int offset, RawDataPool dataPool)
			{
				List<ItemDisplayData> equipments = EasyPool.Get<List<ItemDisplayData>>();
				Serializer.Deserialize(dataPool, offset, ref equipments);
				this.RefreshWeapon(this.enemyWeapon, equipments, false);
			});
		}

		// Token: 0x06008E7A RID: 36474 RVA: 0x004245D0 File Offset: 0x004227D0
		private void RefreshWeapon(RectTransform parent, List<ItemDisplayData> equipments, bool ally)
		{
			bool allowUseFreeWeapon = this.CharacterAllowUseFreeWeapon(ally);
			CombatBeginCharacterInfo characterInfo = ally ? this.selfCharInfo : this.enemyCharInfo;
			characterInfo.txtFreeWeaponTitle.gameObject.SetActive(allowUseFreeWeapon);
			for (int i = 0; i < this.freeWeapons.Length; i++)
			{
				Transform weapon = parent.GetChild(i);
				weapon.gameObject.SetActive(allowUseFreeWeapon);
				bool flag = !allowUseFreeWeapon;
				if (!flag)
				{
					CToggle toggle = weapon.GetComponent<CToggle>();
					toggle.interactable = ally;
					WeaponItem weaponConfig = Weapon.Instance[this.freeWeapons[i]];
					CImage icon = weapon.Find("Weapon").GetComponent<CImage>();
					icon.SetSprite(weaponConfig.Icon, false, null);
					icon.gameObject.SetActive(true);
					icon.raycastTarget = true;
					TooltipInvoker tipDisplayer = icon.gameObject.GetOrAddComponent<TooltipInvoker>();
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					ItemDisplayData itemDisplay = new ItemDisplayData(0, this.freeWeapons[i]);
					argBox.Set<ItemDisplayData>("ItemData", itemDisplay);
					argBox.Set("TemplateDataOnly", true);
					argBox.Set("IsInCompareUI", false);
					argBox.Set("DisableCompare", true);
					tipDisplayer.Type = TipType.Weapon;
					tipDisplayer.RuntimeParam = argBox;
					tipDisplayer.gameObject.SetActive(false);
					tipDisplayer.gameObject.SetActive(true);
					this._normalWeaponTips.Add(tipDisplayer);
				}
			}
			CToggleGroup toggleGroup = ally ? this.selfWeapon : this.enemyWeapon.GetComponent<CToggleGroup>();
			int weaponIndex = -1;
			for (int j = this.freeWeapons.Length; j < 3 + this.freeWeapons.Length; j++)
			{
				Transform weapon2 = parent.GetChild(j);
				CToggle toggle2 = weapon2.GetComponent<CToggle>();
				CImage icon2 = weapon2.Find("Weapon").GetComponent<CImage>();
				CImage gradeIcon = weapon2.Find("Grade").GetComponent<CImage>();
				TextMeshProUGUI durability = weapon2.Find("Durability").GetComponent<TextMeshProUGUI>();
				ItemDisplayData itemDisplay2 = equipments[j - this.freeWeapons.Length];
				short templateId = itemDisplay2.Key.TemplateId;
				bool valid = templateId >= 0;
				durability.gameObject.SetActive(valid);
				toggle2.interactable = (valid && ally);
				gradeIcon.gameObject.SetActive(valid);
				bool flag2 = valid && weaponIndex < 0;
				if (flag2)
				{
					weaponIndex = j - this.freeWeapons.Length;
				}
				bool flag3 = templateId >= 0;
				if (flag3)
				{
					WeaponItem weaponConfig2 = Weapon.Instance[templateId];
					icon2.SetSprite(weaponConfig2.Icon, false, null);
					durability.SetText(itemDisplay2.Durability.ToString() + "/" + itemDisplay2.MaxDurability.ToString(), true);
					sbyte grade = ItemTemplateHelper.GetGrade(itemDisplay2.Key.ItemType, itemDisplay2.Key.TemplateId);
					gradeIcon.SetColor(Colors.Instance.GradeColors[(int)grade]);
				}
				else
				{
					icon2.SetSprite("ui9_icon_weapon_empty", false, null);
				}
				icon2.gameObject.SetActive(true);
				bool flag4 = templateId < 0;
				if (!flag4)
				{
					icon2.raycastTarget = true;
					TooltipInvoker tipDisplayer2 = icon2.gameObject.GetOrAddComponent<TooltipInvoker>();
					ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
					argBox2.Set<ItemDisplayData>("ItemData", itemDisplay2);
					argBox2.Set("TemplateDataOnly", true);
					argBox2.Set("IsInCompareUI", false);
					argBox2.Set("DisableCompare", true);
					tipDisplayer2.Type = TipType.Weapon;
					tipDisplayer2.RuntimeParam = argBox2;
					tipDisplayer2.gameObject.SetActive(false);
					tipDisplayer2.gameObject.SetActive(true);
					this._normalWeaponTips.Add(tipDisplayer2);
				}
			}
			bool flag5 = weaponIndex >= 0;
			if (flag5)
			{
				toggleGroup.SetWithoutNotify(weaponIndex);
			}
			else
			{
				List<CToggle> list = toggleGroup.GetAll();
				foreach (CToggle tog in list)
				{
					tog.isOn = false;
				}
			}
		}

		// Token: 0x06008E7B RID: 36475 RVA: 0x00424A1C File Offset: 0x00422C1C
		private void UpdateNormalWeaponTips()
		{
			bool flag = this._normalWeaponTips == null || this._normalWeaponTips.Count <= 0;
			if (!flag)
			{
				bool flag2 = this._showingWeaponTip != null && this._showingWeaponTip.Showing;
				if (!flag2)
				{
					this._showingWeaponTip = null;
					for (int i = 0; i < this._normalWeaponTips.Count; i++)
					{
						TooltipInvoker tip = this._normalWeaponTips[i];
						bool flag3 = tip.Showing && this._showingWeaponTip == null;
						if (flag3)
						{
							this._showingWeaponTip = tip;
						}
						else
						{
							tip.HideTips();
						}
					}
					bool flag4 = this._showingWeaponTip != null;
					if (flag4)
					{
						this._showingWeaponTip.ShowTips();
					}
				}
			}
		}

		// Token: 0x06008E7C RID: 36476 RVA: 0x00424AF8 File Offset: 0x00422CF8
		private bool CharacterAllowUseFreeWeapon(bool ally)
		{
			int charId = ally ? this.SelfTeam[0] : this.EnemyTeam[0];
			CharacterDisplayData characterDisplay;
			bool flag = this._charDisplayDataDict.TryGetValue(charId, out characterDisplay);
			bool result;
			if (flag)
			{
				CharacterItem characterItem = Character.Instance[characterDisplay.TemplateId];
				result = characterItem.AllowUseFreeWeapon;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06008E7D RID: 36477 RVA: 0x00424B5C File Offset: 0x00422D5C
		private void DisableWeaponIcon(RectTransform parent)
		{
			for (int i = 0; i < this.freeWeapons.Length; i++)
			{
				Transform weapon = parent.GetChild(i);
				CImage icon = weapon.Find("Weapon").GetComponent<CImage>();
				icon.gameObject.SetActive(false);
			}
			for (int j = this.freeWeapons.Length; j < 3 + this.freeWeapons.Length; j++)
			{
				Transform weapon2 = parent.GetChild(j);
				CImage icon2 = weapon2.Find("Weapon").GetComponent<CImage>();
				icon2.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008E7E RID: 36478 RVA: 0x00424BFC File Offset: 0x00422DFC
		private void DisableWeaponIcon()
		{
			RectTransform parent = this.selfWeapon.GetComponent<RectTransform>();
			this.DisableWeaponIcon(parent);
			this.DisableWeaponIcon(this.enemyWeapon);
		}

		// Token: 0x06008E7F RID: 36479 RVA: 0x00424C2B File Offset: 0x00422E2B
		private void RefreshLoopReadCountText()
		{
			TaiwuDomainMethod.AsyncCall.GetLoopReadCountDisplayData(this, delegate(int offset, RawDataPool dataPool)
			{
				LoopReadCountDisplayData displayData = new LoopReadCountDisplayData();
				Serializer.Deserialize(dataPool, offset, ref displayData);
				string loopStr = displayData.LoopInCombatCount.ToString().SetColor((displayData.LoopInCombatCount <= 0) ? "brightred" : "brightblue") + "/" + 1.ToString();
				this.loopCountText.SetText(LanguageKey.LK_Reading_ReadInCombat_Count.TrFormat(loopStr), true);
				string readStr = displayData.ReadInCombatCount.ToString().SetColor((displayData.ReadInCombatCount <= 0) ? "brightred" : "brightblue") + "/" + 1.ToString();
				this.readCountText.SetText(LanguageKey.LK_Reading_ReadInCombat_Count.TrFormat(readStr), true);
				this._pendingAsyncOps--;
				this.TryShowElement();
			});
		}

		// Token: 0x06008E80 RID: 36480 RVA: 0x00424C44 File Offset: 0x00422E44
		private void RefreshTaiwuCombatSkillPlan(bool isInit)
		{
			CombatSkillDomainMethod.AsyncCall.GetEquipCombatSkillDisplayData(this, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
			{
				EquipCombatSkillDisplayData displayData = new EquipCombatSkillDisplayData();
				Serializer.Deserialize(dataPool, offset, ref displayData);
				this.RefreshTaiwuCombatSkillPlan(displayData);
				this.RefreshMouseTip();
				bool isInit2 = isInit;
				if (isInit2)
				{
					this._pendingAsyncOps--;
					this.TryShowElement();
				}
			});
		}

		// Token: 0x06008E81 RID: 36481 RVA: 0x00424C84 File Offset: 0x00422E84
		private void RefreshTaiwuCombatSkillPlan(EquipCombatSkillDisplayData displayData)
		{
			for (int i = 0; i < 9; i++)
			{
				this.taiwuCombatSkillPlan.transform.GetChild(i).gameObject.SetActive(i < displayData.PlanCount);
			}
			this.taiwuCombatSkillPlan.SetWithoutNotify(displayData.CurrentPlanId);
		}

		// Token: 0x06008E82 RID: 36482 RVA: 0x00424CDC File Offset: 0x00422EDC
		private void HandleEquipPlanToggleChange(int newIndex, int arg2)
		{
			CharacterDomainMethod.Call.UpdateCombatSkillPlan(SingletonObject.getInstance<BasicGameData>().TaiwuCharId, newIndex);
			this.RefreshMouseTip();
		}

		// Token: 0x17000F92 RID: 3986
		// (get) Token: 0x06008E83 RID: 36483 RVA: 0x00424CF7 File Offset: 0x00422EF7
		private BuildingModel BuildingModel
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x17000F93 RID: 3987
		// (get) Token: 0x06008E84 RID: 36484 RVA: 0x00424CFE File Offset: 0x00422EFE
		private WorldMapModel WorldMapModel
		{
			get
			{
				return SingletonObject.getInstance<WorldMapModel>();
			}
		}

		// Token: 0x17000F94 RID: 3988
		// (get) Token: 0x06008E85 RID: 36485 RVA: 0x00424D05 File Offset: 0x00422F05
		private CombatModel CombatModel
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000F95 RID: 3989
		// (get) Token: 0x06008E86 RID: 36486 RVA: 0x00424D0C File Offset: 0x00422F0C
		private bool HasYuanshanThreeVitals
		{
			get
			{
				return this.BuildingModel.YuanshanThreeVitalsExist();
			}
		}

		// Token: 0x17000F96 RID: 3990
		// (get) Token: 0x06008E87 RID: 36487 RVA: 0x00424D19 File Offset: 0x00422F19
		private bool HasIronPlateChar
		{
			get
			{
				return this._ironPlateCombatCharId >= 0;
			}
		}

		// Token: 0x17000F97 RID: 3991
		// (get) Token: 0x06008E88 RID: 36488 RVA: 0x00424D27 File Offset: 0x00422F27
		private bool HasStoryTeammate
		{
			get
			{
				return this.HasYuanshanThreeVitals || this.HasIronPlateChar;
			}
		}

		// Token: 0x06008E89 RID: 36489 RVA: 0x00424D3C File Offset: 0x00422F3C
		private void RefreshYuanshanThreeVitalsBtn(CombatBeginTeammateSelf teammateSelf, int index)
		{
			bool isCancel = false;
			bool isShow = this.HasStoryTeammate;
			CButton yuanshanThreeVitals = teammateSelf.yuanshanThreeVitalsBtn;
			yuanshanThreeVitals.gameObject.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				TooltipInvoker mouseTip = yuanshanThreeVitals.GetComponent<TooltipInvoker>();
				mouseTip.Type = TipType.SingleDesc;
				DisableStyleRoot disableStyleRoot = yuanshanThreeVitals.GetComponent<DisableStyleRoot>();
				bool interactable = true;
				string tipContent = string.Empty;
				bool flag2 = this._currSelectIndex.Count > 0;
				if (flag2)
				{
					bool flag3 = this._currSelectIndex.Contains(index) || this._allowSelectMultiVitalDemon;
					if (flag3)
					{
						isCancel = this._currSelectIndex.Contains(index);
					}
					else
					{
						interactable = false;
						tipContent = LanguageKey.LK_Combat_YuanshanThreeVitalsAlreadySelect_Tip.Tr();
					}
				}
				else
				{
					bool flag4 = !this.CombatModel.Config.AllowVitalDemon;
					if (flag4)
					{
						interactable = false;
						tipContent = LanguageKey.LK_Combat_YuanshanThreeVitals_ConfigNotAllow_Tip.Tr();
					}
					else
					{
						bool flag5 = this.BuildingModel.YuanshanThreeVitalsAllInPrison();
						if (flag5)
						{
							interactable = false;
							tipContent = LanguageKey.LK_Combat_YuanshanThreeVitalsAllInPrison_Tip.Tr();
						}
						else
						{
							bool flag6 = !this.BuildingModel.YuanshanThreeVitalsAllNotAllowAsTeammate();
							if (flag6)
							{
								interactable = false;
								tipContent = (this.BuildingModel.AreVitalsDemon() ? LanguageKey.LK_Combat_YuanshanThreeVitalsAllowAsTeammate_Tip_Bad.Tr() : LanguageKey.LK_Combat_YuanshanThreeVitalsAllowAsTeammate_Tip_Good.Tr());
							}
						}
					}
					interactable = (interactable || this.HasIronPlateChar);
					bool flag7 = !this.HasIronPlateChar;
					if (flag7)
					{
					}
				}
				yuanshanThreeVitals.spriteState = (isCancel ? teammateSelf.vitalState : teammateSelf.normalState);
				yuanshanThreeVitals.GetComponent<CImage>().sprite = (isCancel ? teammateSelf.vitalSprite : teammateSelf.normalSprite);
				yuanshanThreeVitals.interactable = interactable;
				disableStyleRoot.SetStyleEffect(!interactable, false);
				mouseTip.enabled = !interactable;
				mouseTip.PresetParam = new string[]
				{
					tipContent
				};
				int currIndex = index;
				SelectCharacterCallback <>9__2;
				yuanshanThreeVitals.ClearAndAddListener(delegate
				{
					int charId = (currIndex < this.SelfTeam.Count) ? this.SelfTeam[currIndex] : -1;
					CombatBeginTeammateSelf selfTeammate = this._selfTeammateHolder.GetChild(currIndex - 1).GetComponent<CombatBeginTeammateSelf>();
					bool isIronPlateChar = charId == this._ironPlateCombatCharId;
					ParticleSystem particle = isIronPlateChar ? selfTeammate.effectIronPlateChar : selfTeammate.GetEffectYuanshanThreeDemon(this.BuildingModel.AreVitalsDemon(), false);
					this.SetTargetTexture(charId, particle);
					bool isCancel = isCancel;
					if (isCancel)
					{
						bool flag8 = isIronPlateChar;
						if (flag8)
						{
							this.RequestRevertVitalOnTeammate(StoryTeammateType.IronPlate, index);
						}
						else
						{
							SectStoryThreeVitalsCharacterType type = SectStoryThreeVitalsCharacterType.Earth;
							bool sectStoryThreeVitalsCharacterTypeById = this.BuildingModel.GetSectStoryThreeVitalsCharacterTypeById(charId, ref type);
							if (sectStoryThreeVitalsCharacterTypeById)
							{
								this.RequestRevertVitalOnTeammate((StoryTeammateType)type, currIndex);
							}
						}
					}
					else
					{
						List<int> charIdList = new List<int>();
						bool hasYuanshanThreeVitals = this.HasYuanshanThreeVitals;
						if (hasYuanshanThreeVitals)
						{
							List<int> availableYuanshanThreeVitalsIdList = this.BuildingModel.GetAvailableYuanshanThreeVitalsIdList();
							charIdList.AddRange(availableYuanshanThreeVitalsIdList.Where(new Func<int, bool>(base.<RefreshYuanshanThreeVitalsBtn>g__Predicate|1)));
						}
						bool flag9 = this.HasIronPlateChar && base.<RefreshYuanshanThreeVitalsBtn>g__Predicate|1(this._ironPlateCombatCharId);
						if (flag9)
						{
							charIdList.Add(this._ironPlateCombatCharId);
						}
						List<int> charIds = charIdList;
						List<int> selectedCharIds = null;
						SelectCharacterCallback callback;
						if ((callback = <>9__2) == null)
						{
							callback = (<>9__2 = delegate(List<int> selectedIds)
							{
								int selectedCharId = (selectedIds != null && selectedIds.Count > 0) ? selectedIds[0] : -1;
								this.SelectYuanshanThreeVitalsAction(selectedCharId, currIndex, false);
							});
						}
						VillagerSelectCharacterSelectionHelper.OpenDefaultSelectChar(charIds, selectedCharIds, callback, ESelectCharacterInteractionMode.Slot, ESelectCharacterSelectionMode.Single, 1, this, null, null);
					}
				});
			}
		}

		// Token: 0x06008E8A RID: 36490 RVA: 0x00424F5A File Offset: 0x0042315A
		private void SetTargetTexture(int charId, ParticleSystem particle)
		{
			CombatBeginTeammateCamera.Instance.SetCharData(charId);
			CombatBeginTeammateCamera.Instance.SetRenderTexture(particle.transform.GetChild(0).GetComponent<ParticleSystemRenderer>());
		}

		// Token: 0x06008E8B RID: 36491 RVA: 0x00424F88 File Offset: 0x00423188
		private void PlaySwitchThreeVitalsAnim(ParticleSystem particle, bool isSwitchEnter = true)
		{
			particle.gameObject.SetActive(true);
			try
			{
				particle.transform.GetChild(1).gameObject.SetActive(isSwitchEnter);
			}
			catch
			{
				Debug.LogError(string.Format("PlaySwitchThreeVitalsAnim error, isSwitchEnter:{0}", isSwitchEnter));
			}
			UnityEngine.Material material = particle.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material;
			particle.Play();
			AudioManager.Instance.PlaySound("sancai_change", false, false);
			DOVirtual.Float(-0.1f, 1f, 1f, delegate(float value)
			{
				material.SetFloat("_rongjie", value);
			}).OnComplete(delegate
			{
				CombatBeginTeammateCamera.Instance.Hide();
				particle.gameObject.SetActive(false);
			});
		}

		// Token: 0x06008E8C RID: 36492 RVA: 0x00425074 File Offset: 0x00423274
		private void SelectYuanshanThreeVitalsAction(int charId, int index, bool autoSet)
		{
			bool flag = charId < 0;
			if (!flag)
			{
				bool flag2 = index > this.SelfTeam.Count && !autoSet;
				if (flag2)
				{
					index = this.SelfTeam.Count;
				}
				bool flag3 = charId == this._ironPlateCombatCharId;
				if (flag3)
				{
					this.RequestApplyVitalOnTeammate(StoryTeammateType.IronPlate, index);
				}
				else
				{
					SectStoryThreeVitalsCharacterType type = SectStoryThreeVitalsCharacterType.Earth;
					bool sectStoryThreeVitalsCharacterTypeById = this.BuildingModel.GetSectStoryThreeVitalsCharacterTypeById(charId, ref type);
					if (sectStoryThreeVitalsCharacterTypeById)
					{
						this.RequestApplyVitalOnTeammate((StoryTeammateType)type, index);
					}
				}
			}
		}

		// Token: 0x06008E8D RID: 36493 RVA: 0x004250EA File Offset: 0x004232EA
		private void RequestApplyVitalOnTeammate(StoryTeammateType type, int index)
		{
			this.CombatModel.RequestApplyVitalOnTeammate(type, index, new Action<bool>(this.UpdateVitalOnTeammate));
			this.PlaySelfSwitchThreeVitalsAnim(type, index);
		}

		// Token: 0x06008E8E RID: 36494 RVA: 0x00425110 File Offset: 0x00423310
		private void RequestRevertVitalOnTeammate(StoryTeammateType type, int index)
		{
			this.CombatModel.RequestRevertVitalOnTeammate(type, new Action<bool>(this.UpdateVitalOnTeammate));
			this.PlaySelfSwitchThreeVitalsAnim(type, index);
		}

		// Token: 0x06008E8F RID: 36495 RVA: 0x00425138 File Offset: 0x00423338
		private void PlaySelfSwitchThreeVitalsAnim(StoryTeammateType type, int index)
		{
			CombatBeginTeammateSelf selfTeammate = this._selfTeammateHolder.GetChild(index - 1).GetComponent<CombatBeginTeammateSelf>();
			ParticleSystem particle = (type < StoryTeammateType.IronPlate) ? selfTeammate.GetEffectYuanshanThreeDemon(this.BuildingModel.AreVitalsDemon(), false) : selfTeammate.effectIronPlateChar;
			this.PlaySwitchThreeVitalsAnim(particle, false);
		}

		// Token: 0x06008E90 RID: 36496 RVA: 0x00425184 File Offset: 0x00423384
		private void UpdateVitalOnTeammate(bool success)
		{
			bool flag = !success;
			if (!flag)
			{
				this._currSelectIndex.Clear();
				for (int i = 0; i < this.Model.SelfTeam.Count; i++)
				{
					bool flag2 = this._storyTeammateIdList.Contains(this.Model.SelfTeam[i]);
					if (flag2)
					{
						this._currSelectIndex.Add(i);
					}
				}
				this.Model.RequestSimulatePrepareCombat(new AsyncMethodCallbackDelegate(this.HandlerSimulatePrepareCombat));
				foreach (CombatBeginWisdom wisdomComponent in this.wisdomComponents)
				{
					wisdomComponent.DoRequest();
				}
				this.RefreshTeammateInfo();
				for (int j = 0; j < 3; j++)
				{
					int teamIndex = j + 1;
					CombatBeginTeammateSelf self = this._selfTeammateHolder.GetChild(j).GetComponent<CombatBeginTeammateSelf>();
					bool flag3 = this._currSelectIndex.Contains(teamIndex);
					if (flag3)
					{
						this.UpdateYuanshanTeammateCommand(j);
					}
					else
					{
						this.UpdateSelfTeammateCommand(j);
					}
					this.PlaySingleSelfTeammateCommandBubble(j);
					self.commandBubble.gameObject.SetActive(false);
					self.betrayBubble.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06008E91 RID: 36497 RVA: 0x004252CC File Offset: 0x004234CC
		private void UpdateYuanshanTeammateCommand(int index)
		{
			CombatBeginTeammateSelf selfTeammate = this._selfTeammateHolder.GetChild(index).GetComponent<CombatBeginTeammateSelf>();
			int charId = this.SelfTeam[index + 1];
			short characterTemplateId = (charId == this._ironPlateCombatCharId && this._ironPlateCombatCharId >= 0) ? this.WorldMapModel.IronPlateCombatCharData.TemplateId : this.BuildingModel.GetYuanshanThreeVitalsTemplateId(charId);
			List<sbyte> command = Character.Instance[characterTemplateId].PresetTeammateCommands;
			this.UpdateTeammateCommand(selfTeammate.commandHolder, command, true);
		}

		// Token: 0x06008E92 RID: 36498 RVA: 0x0042534C File Offset: 0x0042354C
		private void UpdateSelfTeammateCommand(int index)
		{
			CombatBeginTeammateSelf selfTeammate = this._selfTeammateHolder.GetChild(index).GetComponent<CombatBeginTeammateSelf>();
			List<sbyte> selfData = this.CommandChangeData.LeftTeam.ReplaceTeammateCommands.GetOrDefault(index).Items;
			this.UpdateTeammateCommand(selfTeammate.commandHolder, selfData, true);
		}

		// Token: 0x06008E93 RID: 36499 RVA: 0x00425397 File Offset: 0x00423597
		private void AutoSetYuanshanTeammate()
		{
			StoryDomainMethod.AsyncCall.GetThreeVitalsReplaceTeammateRecord(this, delegate(int offset1, RawDataPool dataPool1)
			{
				List<int> vitalsReplaceList = new List<int>();
				Serializer.Deserialize(dataPool1, offset1, ref vitalsReplaceList);
				for (int i = vitalsReplaceList.Count - 1; i >= 0; i--)
				{
					bool flag = !this.BuildingModel.ThreeVitalsAllowAsTeammate(vitalsReplaceList[i]);
					if (flag)
					{
						vitalsReplaceList[i] = -1;
					}
				}
				int index = 0;
				foreach (int id in vitalsReplaceList)
				{
					bool flag2 = id < 0 && this.SelfTeam.Count <= index + 1;
					if (!flag2)
					{
						index++;
						this.SelectYuanshanThreeVitalsAction(id, index, true);
						bool flag3 = !this._allowSelectMultiVitalDemon;
						if (flag3)
						{
							break;
						}
					}
				}
			});
		}

		// Token: 0x06008E94 RID: 36500 RVA: 0x004253B0 File Offset: 0x004235B0
		private void CheckGuiding()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(280);
			CharacterDisplayData enemyData;
			bool flag = this._charDisplayDataDict.TryGetValue(this.EnemyTeam[0], out enemyData);
			if (flag)
			{
				short templateId = enemyData.TemplateId;
				bool flag2 = templateId >= 366 && templateId <= 374;
				if (flag2)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(225);
				}
				bool flag3 = enemyData.FeatureIds.Contains(211);
				if (flag3)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(256);
				}
			}
		}

		// Token: 0x06008E98 RID: 36504 RVA: 0x00425508 File Offset: 0x00423708
		[CompilerGenerated]
		private void <RefreshTeammateInfo>g__SetBtnBack|79_0(CImage buttonFrame, List<int> idList, int charId, bool isSelf)
		{
			string backName = string.Empty;
			bool flag = idList != null && idList.Contains(charId);
			if (flag)
			{
				bool flag2 = charId == this._ironPlateCombatCharId;
				if (flag2)
				{
					backName = "ui9_back_combat_begin_teammate_special_3";
				}
				else
				{
					bool isDemon = this.BuildingModel.AreVitalsDemon();
					backName = ((isSelf == isDemon) ? "ui9_back_combat_begin_teammate_special_1" : "ui9_back_combat_begin_teammate_special_0");
				}
			}
			buttonFrame.gameObject.SetActive(backName != string.Empty);
			bool flag3 = backName != string.Empty;
			if (flag3)
			{
				buttonFrame.SetSprite(backName, false, null);
			}
		}

		// Token: 0x04006C6C RID: 27756
		[SerializeField]
		public CombatBeginCharacterInfo selfCharInfo;

		// Token: 0x04006C6D RID: 27757
		[SerializeField]
		public CombatBeginCharacterInfo enemyCharInfo;

		// Token: 0x04006C6E RID: 27758
		[SerializeField]
		public CImage combatType;

		// Token: 0x04006C6F RID: 27759
		[SerializeField]
		public CImage maskImg;

		// Token: 0x04006C70 RID: 27760
		[SerializeField]
		public Game.Views.Bottom.ReadAndLoop readAndLoop;

		// Token: 0x04006C71 RID: 27761
		[SerializeField]
		public CombatBeginFirstMoveText allyFirstMove;

		// Token: 0x04006C72 RID: 27762
		[SerializeField]
		public CombatBeginFirstMoveText enemyFirstMove;

		// Token: 0x04006C73 RID: 27763
		[SerializeField]
		public CToggleGroup taiwuCombatSkillPlan;

		// Token: 0x04006C74 RID: 27764
		[SerializeField]
		public CToggleGroup selfWeapon;

		// Token: 0x04006C75 RID: 27765
		[SerializeField]
		public RectTransform enemyWeapon;

		// Token: 0x04006C76 RID: 27766
		[SerializeField]
		public TooltipInvoker selfNeigong;

		// Token: 0x04006C77 RID: 27767
		[SerializeField]
		public TooltipInvoker selfJueji;

		// Token: 0x04006C78 RID: 27768
		[SerializeField]
		public TooltipInvoker enemyNeigong;

		// Token: 0x04006C79 RID: 27769
		[SerializeField]
		public TooltipInvoker enemyJueji;

		// Token: 0x04006C7A RID: 27770
		[SerializeField]
		public GameObject startIconCn;

		// Token: 0x04006C7B RID: 27771
		[SerializeField]
		public GameObject startIconEn;

		// Token: 0x04006C7C RID: 27772
		public float maskFadeTime = 0.2f;

		// Token: 0x04006C7D RID: 27773
		[SerializeField]
		private CombatConfigTips combatConfigTips;

		// Token: 0x04006C7E RID: 27774
		[SerializeField]
		private CombatBeginWisdom[] wisdomComponents;

		// Token: 0x04006C7F RID: 27775
		[SerializeField]
		public TextMeshProUGUI loopCountText;

		// Token: 0x04006C80 RID: 27776
		[SerializeField]
		public TextMeshProUGUI readCountText;

		// Token: 0x04006C81 RID: 27777
		private const short EasterEggBlockId = 12;

		// Token: 0x04006C82 RID: 27778
		private const sbyte EasterEggOdds = 1;

		// Token: 0x04006C83 RID: 27779
		private const byte TeamCapacity = 4;

		// Token: 0x04006C84 RID: 27780
		private float _commandBubbleDelay = 0.3f;

		// Token: 0x04006C85 RID: 27781
		private float _commandBubbleDelayFixedExtra = 0.2f;

		// Token: 0x04006C86 RID: 27782
		private float _commandBubbleDelayRandom = 0.05f;

		// Token: 0x04006C87 RID: 27783
		[SerializeField]
		public RectTransform startCombatAnimRectTs;

		// Token: 0x04006C88 RID: 27784
		private Animator _startCombatAnimCtrl;

		// Token: 0x04006C89 RID: 27785
		private RectTransform _selfTeammateHolder;

		// Token: 0x04006C8A RID: 27786
		private readonly CharacterAvatar[] _selfAvatars = new CharacterAvatar[4];

		// Token: 0x04006C8B RID: 27787
		private readonly CharacterName[] _selfNames = new CharacterName[4];

		// Token: 0x04006C8C RID: 27788
		private EquipCombatSkillMonitor _selfEquipSkillMonitor;

		// Token: 0x04006C8D RID: 27789
		private RectTransform _enemyTeammateHolder;

		// Token: 0x04006C8E RID: 27790
		private readonly CharacterAvatar[] _enemyAvatars = new CharacterAvatar[4];

		// Token: 0x04006C8F RID: 27791
		private readonly CharacterName[] _enemyNames = new CharacterName[4];

		// Token: 0x04006C90 RID: 27792
		private EquipCombatSkillMonitor _enemyEquipSkillMonitor;

		// Token: 0x04006C91 RID: 27793
		private bool _prepareFinished;

		// Token: 0x04006C92 RID: 27794
		private bool _beginDataReady;

		// Token: 0x04006C93 RID: 27795
		private bool _listenerIdReady;

		// Token: 0x04006C94 RID: 27796
		private int _pendingAsyncOps;

		// Token: 0x04006C95 RID: 27797
		private List<TooltipInvoker> _normalWeaponTips;

		// Token: 0x04006C96 RID: 27798
		private TooltipInvoker _showingWeaponTip;

		// Token: 0x04006C97 RID: 27799
		private int _prepareCombatResult;

		// Token: 0x04006C98 RID: 27800
		private bool _allowSelectMultiVitalDemon;

		// Token: 0x04006C99 RID: 27801
		private bool _awakeDone;

		// Token: 0x04006C9A RID: 27802
		private int _preferWeaponIndex;

		// Token: 0x04006C9B RID: 27803
		private short[] freeWeapons = new short[]
		{
			0,
			1,
			2,
			884
		};

		// Token: 0x04006C9C RID: 27804
		private const int NormalWeaponCount = 3;

		// Token: 0x04006C9D RID: 27805
		private readonly List<int> _storyTeammateIdList = new List<int>();

		// Token: 0x04006C9E RID: 27806
		private int _ironPlateCombatCharId = -1;

		// Token: 0x04006C9F RID: 27807
		private List<int> _yuanshanThreeVitalsIdList;

		// Token: 0x04006CA0 RID: 27808
		private List<int> _yuanshanOppositeThreeVitalsIdList;

		// Token: 0x04006CA1 RID: 27809
		private readonly List<int> _currSelectIndex = new List<int>();
	}
}
